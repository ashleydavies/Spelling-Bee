using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public partial class SelectStudentWindow 
    {
        public bool selected = false;
        public Student student;

        public SelectStudentWindow()
        {
            InitializeComponent();

            // Set the list view to display all students using a simple LINQ query
            lstStudents.ItemsSource = from students in App.db.Students
                                      select students;
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Filter students based on whether their name contains the substring provided in the textbox
            lstStudents.ItemsSource = (from students in App.db.Students
                                       where students.StudentName.Contains(txtName.Text)
                                       select students);

            // If they have not selected one, disable the select button
            if (lstStudents.SelectedItem == null)
                btnSelect.IsEnabled = false;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // Set selected to true and student to the relevant student
            selected = true;
            student = (Student)lstStudents.SelectedItem;

            // Success!
            Close();
        }

        private void lstStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If they have selected a student
            if (lstStudents.SelectedIndex != -1)
            {
                // Set the selection button to enabled
                btnSelect.IsEnabled = true;
            }
        }
    }
}
