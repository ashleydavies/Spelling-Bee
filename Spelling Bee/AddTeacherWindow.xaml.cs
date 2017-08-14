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

namespace Spelling_Bee
{
	public partial class AddTeacherWindow 
	{
		public AddTeacherWindow()
		{
			this.InitializeComponent();
		}

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            // Validate form data
            // Make sure they've provided a forename
            if (txtForename.Text.Trim() == "")
            {
                // If not, return and give an error
                App.Message("Validation Error", "Please type a forename.");
                return;
            }
            // ... and a surname
            if (txtSurname.Text.Trim() == "")
            {
                // If not, return and give an error
                App.Message("Validation Error", "Please type a surname.");
                return;
            }
            // ... and a password
            if (txtPassword.Password.Trim() == "")
            {
                // If not, return and give an error
                App.Message("Validation Error", "Please type a password.");
                return;
            }
            // ... and a prefix
            if (cmbPrefix.SelectedIndex == -1)
            {
                // If not, return and give an error
                App.Message("Validation Error", "Please select a staff prefix.");
                return;
            }

            // Create a new staff member based on the form data provided
            Staff newStaff = new Staff()
            {
                IsAdministrative = radSysadmin.IsChecked.Value,
                StaffForename = txtForename.Text,
                StaffSurname = txtSurname.Text,
                StaffPrefix = ((ComboBoxItem)cmbPrefix.SelectedItem).Content.ToString(),
                StaffPassword = txtPassword.Password
            };

            // Submit to the database
            App.db.Staffs.InsertOnSubmit(newStaff);
            App.db.SubmitChanges();
            App.Message("Success", "Created staff member");
            // Close form to stop confusion
            this.Close();
        }
	}
}