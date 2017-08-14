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
using System.Diagnostics;

namespace Spelling_Bee
{
	public partial class AddClassWindow 
	{
		public AddClassWindow()
		{
			this.InitializeComponent();
		}

        private void populateTeacherComboBox()
        {
            // Wipe the combobox
            cmbTeacher.Items.Clear();

            // For each staff, put them in the teacher list
            foreach (Staff teacher in App.db.Staffs)
            {
                cmbTeacher.Items.Add(teacher.StaffID.ToString() + " " + teacher.getFullName());
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            // Check all form data is filled in
            if (cmbTeacher.SelectedIndex == -1)
            {
                App.Message("Validation Error", "Please select a teacher.");
                return;
            }

            // Check all form data is filled in
            if (cmbYear.SelectedIndex == -1)
            {
                App.Message("Validation Error", "Please select a year.");
                return;
            }

            // Check all form data is filled in
            if (txtName.Text.Trim() == "")
            {
                App.Message("Validation Error", "Please type a name.");
                return;
            }


            // Teacher ID is defined by the numeric characters at the beginning of the cmbTeacher selected item.
            // Therefore, we must select these.
            string comboboxExtracted = cmbTeacher.SelectedItem.ToString();
            string teacherID = "";
            for (int i = 0; i < comboboxExtracted.Length; i++)
            {
                // If it's a digit, extract it
                if (Char.IsDigit(comboboxExtracted[i]))
                {
                    teacherID += comboboxExtracted[i];
                }
                else
                {
                    break;
                }
            }


            // Class names must be unique, so check that first:
            int uniqueClassTest = (from classes in App.db.Classes
                                     where classes.ClassName == txtName.Text
                                     select classes).Count();

            // If there's already a class with that name, tell them and cancel routine
            if (uniqueClassTest > 0)
            {
                App.Message("Validation Error", "There is already a class with the name" + txtName.Text);
                return;
            }

            // Create a new class
            Class newClass = new Class()
            {
                ClassName = txtName.Text,
                ClassYear = int.Parse(((ComboBoxItem)cmbYear.SelectedItem).Content.ToString().Substring(5)),
                TeacherID = int.Parse(teacherID),
            };

            // Insert into database
            App.db.Classes.InsertOnSubmit(newClass);
            App.db.SubmitChanges();

            // Close window and give a success message
            this.Close();
            App.Message("Success", "Class created successfully", false);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Fill combobox with teachers when the window is loaded
            populateTeacherComboBox();
        }

        private void lblCreateTeacher_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Create a teacher and repopulate the list box to reflect this change
            AddTeacherWindow atw = new AddTeacherWindow();
            atw.ShowDialog();

            populateTeacherComboBox();
        }
	}
}