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
    public partial class SelectTeacherWindow 
    {
        public bool selected = false;
        public Staff selectedTeacher;

        public SelectTeacherWindow()
        {
            InitializeComponent();

            // Set the teachers view to display all of the teachers
            lstTeachers.ItemsSource = from teachers in App.db.Staffs
                                      select teachers;
        }

        private void lstTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If a teacher is selected
            if (lstTeachers.SelectedIndex != -1)
            {
                // Enable the selection button
                btnSelect.IsEnabled = true;
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (lstTeachers.SelectedIndex != -1)
            {
                selected = true;
                selectedTeacher = (Staff)lstTeachers.SelectedItem;

                // Successfully chosen class, so close form
                Close();
            }
        }
    }
}
