using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	public partial class AddStudentWindow 
	{
		public AddStudentWindow()
		{
			this.InitializeComponent();
		}

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            // Validate form data
            // Make sure they've provided a name
            if (txtName.Text.Trim() == "")
            {
                // If not, return and give an error
                App.Message("Validation Error", "Please type a name.");
                return;
            }
            // Make sure they've provided a password
            if (txtPassword.Password.Trim() == "")
            {
                // If not, return and give an error
                App.Message("Validation Error", "Please type a password.");
                return;
            }
            // Make sure they've selected a year group
            if (cmbYear.SelectedIndex == -1)
            {
                // If not, return and give an error
                App.Message("Validation Error", "Please select a year group.");
                return;
            }

            // Create a new student using the form data
            Student student = new Student()
            {
                StudentName = txtName.Text,
                StudentPassword = txtPassword.Password,
                StudentYear = int.Parse(((ComboBoxItem)cmbYear.SelectedItem).Content.ToString().Substring(5)),
                StudentDOB = dtpDOB.SelectedDate.Value,
            };

            // Submit them to the database
            App.db.Students.InsertOnSubmit(student);
            App.db.SubmitChanges();

            // Close this window and give a success message
            this.Close();
            App.Message("Success", "Created student successfully.");
        }
	}
}