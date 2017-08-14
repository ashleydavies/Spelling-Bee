using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;

namespace Spelling_Bee
{
	/// <summary>
	/// Interaction logic for EditTeacherWindow.xaml
	/// </summary>
	public partial class EditTeacherWindow 
	{
		public EditTeacherWindow(Staff loggedInAs)
		{
			this.InitializeComponent();

            // Set the logged in variable to keep track of them - This is to stop them deleting themselves
            loggedInAs_ = loggedInAs;
		}

        // loggedInAs prevents deletion of the currently in-use account.
        Staff loggedInAs_;
        Staff selectedStaff_;

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // Open the select teacher window so they can select which teacher they want to view details on or modify
            SelectTeacherWindow stw = new SelectTeacherWindow();
            stw.ShowDialog();

            // If they selected a teacher
            if (stw.selected)
            {
                // Assign the selected variable to keep track of they staff they selected
                selectedStaff_ = stw.selectedTeacher;

                // Enable buttons
                btnChangePassword.IsEnabled = true;
                btnDeleteStaff.IsEnabled = true;
                btnForename.IsEnabled = true;
                btnSurname.IsEnabled = true;

                // And fill the form contents
                lblID.Content = selectedStaff_.StaffID;
                lblForename.Content = selectedStaff_.StaffForename;
                lblSurname.Content = selectedStaff_.StaffSurname;
                lblType.Content = selectedStaff_.IsAdministrative.Value ? "Admin" : "Teacher";
            }
        }

        private void btnForename_Click(object sender, RoutedEventArgs e)
        {
            // Prompt for a new name
            string newName = App.getString("Enter a new forename");

            // If they submitted one
            if (newName != null && newName.Trim() != "")
            {
                // We assign the forename and put the new one in the form
                selectedStaff_.StaffForename = newName;
                lblForename.Content = newName;
                // And submit the changes
                App.db.SubmitChanges();
            }
        }

        private void btnSurname_Click(object sender, RoutedEventArgs e)
        {
            // Prompt for a new name
            string newName = App.getString("Enter a new surname");

            // If one was provided
            if (newName != null && newName.Trim() != "")
            {
                // We assign the property and change the form
                selectedStaff_.StaffSurname = newName;
                lblSurname.Content = newName;

                // And submit the changes
                App.db.SubmitChanges();
            }
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            // Check that they are certain about this
            if (App.getBoolean("Are you sure?", "Are you sure that you want to change the password?"))
            {
                // If so, ask for a new password
                string newPassword = App.getString("Enter new password");

                // If one was provided
                if (newPassword != "")
                {
                    // Update it
                    selectedStaff_.StaffPassword = newPassword;
                    // And submit the changes
                    App.db.SubmitChanges();
                }
            }
        }

        private void btnDeleteStaff_Click(object sender, RoutedEventArgs e)
        {
            // If they are modifying themselves
            if (selectedStaff_.StaffID == loggedInAs_.StaffID)
            {
                // Deleting the logged in user would cause all sorts of errors and a huge mess to clean-up, so we don't allow it.
                App.Message("Error", "You can't delete the account that is in use.");
                return;
            }
            
            // If they have any classes
            if ((from classes in App.db.Classes
                    where classes.TeacherID == selectedStaff_.StaffID
                    select classes).Count() > 0)
            {
                // We error. They should delete the classes or assign a new teacher first.
                App.Message("Error", "Sorry, you can't delete a teacher that is currently teaching a class. Please change all of their classes to another teacher (Or delete them) first.");
                return;
            }

            // If they're definitely certain
            if (App.getBoolean("Are you sure?", "Are you sure you want to delete this member of staff? Deletion is permanent."))
            {
                // Delete the staff member (They have no classes to worry about as seen above)
                App.db.Staffs.DeleteOnSubmit(selectedStaff_);
                // Submit changes
                App.db.SubmitChanges();

                // Disable form since staff no longer exists
                lblID.Content = "";
                lblForename.Content = "";
                lblSurname.Content = "";
                lblType.Content = "";
                btnForename.IsEnabled = false;
                btnSurname.IsEnabled = false;
                btnChangePassword.IsEnabled = false;
                btnDeleteStaff.IsEnabled = false;

                selectedStaff_ = null;
            }
        }
	}
}