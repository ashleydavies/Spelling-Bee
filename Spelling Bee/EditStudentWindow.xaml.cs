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
	public partial class EditStudentWindow 
	{
		public EditStudentWindow()
		{
			this.InitializeComponent();
		}

        Student selectedStudent_;

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // Open the select window
            SelectStudentWindow ssw = new SelectStudentWindow();
            ssw.ShowDialog();
            // If they selected one, set up the form
            if (ssw.selected)
            {
                selectedStudent_ = ssw.student;

                // Enable all buttons
                btnName.IsEnabled = true;
                btnYear.IsEnabled = true;
                btnClassAdd.IsEnabled = true;
                btnClassEdit.IsEnabled = true;
                btnClassRemoveMembership.IsEnabled = true;
                btnChangePassword.IsEnabled = true;
                btnDelete.IsEnabled = true;

                // And show the information about the user
                updateInformation();
            }
        }

        private void updateInformation()
        {
            // Fill form contents with the relevant values
            lblID.Content = selectedStudent_.StudentID;
            lblName.Content = selectedStudent_.StudentName;
            lblYear.Content = selectedStudent_.StudentYear;

            populateClassesList();
        }

        private void btnName_Click(object sender, RoutedEventArgs e)
        {
            // Prompt for a new name
            string newName = App.getString("Enter new name");

            // If they submitted one
            if (newName != null && newName.Trim() != null)
            {
                // Update the form and student
                lblName.Content = newName;
                selectedStudent_.StudentName = newName;
                App.db.SubmitChanges();
            }
        }

        private void btnYear_Click(object sender, RoutedEventArgs e)
        {
            int newYear = App.getYear("Enter new year group");

            // > 2 ensures that it isn't 0 (the return value if they didn't choose a year.)
            if (newYear > 2 && newYear != selectedStudent_.StudentYear)
            {
                // Make sure they haven't accidentally clicked it
                if (App.getBoolean("Are you sure?", "This will remove the student from all their current classes."))
                {
                    // Update form
                    lblYear.Content = newYear;
                    selectedStudent_.StudentYear = newYear;

                    // Delete all relevant StudentClass links (Change of year = different classes)
                    App.db.StudentClasses.DeleteAllOnSubmit(
                        App.db.StudentClasses.Where(
                            sc => sc.StudentID == selectedStudent_.StudentID
                        )
                    );

                    // Submit changes and fix the list of classes
                    App.db.SubmitChanges();

                    populateClassesList();
                }
            }
        }

        private void btnClassAdd_Click(object sender, RoutedEventArgs e)
        {
            // Open the add student to class window
            AddStudentToClassWindow astcw = new AddStudentToClassWindow(selectedStudent_);
            astcw.ShowDialog();

            // Fix class list
            populateClassesList();
        }

        private void btnClassEdit_Click(object sender, RoutedEventArgs e)
        {
            // Open edit class window & show & then fix list of classes
            EditClassWindow ecw = new EditClassWindow();
            ecw.ShowDialog();

            populateClassesList();
        }

        private void btnClassRemoveMembership_Click(object sender, RoutedEventArgs e)
        {
            // If they have selected a class
            if (lstClasses.SelectedIndex != -1)
            {
                // Extract class from form
                Class selectedClass = (Class)lstClasses.SelectedItem;

                // Delete relevant studentclasses
                App.db.StudentClasses.DeleteAllOnSubmit(
                    App.db.StudentClasses.Where(
                        sc => (sc.ClassID == selectedClass.ClassID && sc.StudentID == selectedStudent_.StudentID)
                    )
                );

                // Submit changes to db
                App.db.SubmitChanges();
            }

            // Fix classes list
            populateClassesList();
        }



        private void populateClassesList()
        {
            // Get all the classes which they are in using the StudentClasses link table, simple selection query
            lstClasses.ItemsSource = from classes in App.db.Classes
                                     // Join the StudentClasses table
                                     join studentClasses in App.db.StudentClasses
                                     // Use it to check if they are in the class
                                        on classes.ClassID equals studentClasses.ClassID
                                     where studentClasses.StudentID == selectedStudent_.StudentID
                                     select classes;
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            // Prompt for a new password from user
            string newPassword = App.getString("Enter new password");

            // If they're sure, change the password
            if (App.getBoolean("Are you sure?", "Are you sure you want to change the password?"))
                selectedStudent_.StudentPassword = newPassword;

            // Submit any changes if appropriate
            App.db.SubmitChanges();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Make sure they're certain (Since they can't go back)
            if (App.getBoolean("Are you sure?", "Are you sure you want to delete this student? Deletion is permanent."))
            {
                // Now delete all their results
                // Loop their results and delete the test question results
                foreach (TestResult result in selectedStudent_.TestResults)
                {
                    App.db.TestQuestionResults.DeleteAllOnSubmit(result.TestQuestionResults);
                }
                // Delete student class links, test results, and then the student
                App.db.StudentClasses.DeleteAllOnSubmit(selectedStudent_.StudentClasses);
                App.db.TestResults.DeleteAllOnSubmit(selectedStudent_.TestResults);
                App.db.Students.DeleteOnSubmit(selectedStudent_);
                // And submit the changes
                App.db.SubmitChanges();

                // Disable form since student no longer exists
                lblID.Content = "";
                lblName.Content = "";
                lblYear.Content = "";
                btnName.IsEnabled = false;
                btnYear.IsEnabled = false;
                btnClassAdd.IsEnabled = false;
                btnClassEdit.IsEnabled = false;
                btnClassRemoveMembership.IsEnabled = false;
                btnChangePassword.IsEnabled = false;
                btnDelete.IsEnabled = false;

                lstClasses.ItemsSource = null;
                // Finally, set the student property to null
                selectedStudent_ = null;
            }
        }
	}
}