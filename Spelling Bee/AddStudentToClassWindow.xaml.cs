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
    public partial class AddStudentToClassWindow 
    {
        public AddStudentToClassWindow(Student baseStudent = null)
        {
            InitializeComponent();

            // If there was a student passed in as a parameter, we select them by default
            if (baseStudent != null)
            {
                studentSelected(baseStudent);
            }
        }

        Student selectedStudent_;

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // Open a select student window, and then if they select one, we push a call to the studentSelected method for it to do it's logic
            SelectStudentWindow selectionWindow = new SelectStudentWindow();
            selectionWindow.ShowDialog();

            if (selectionWindow.selected)
            {
                studentSelected(selectionWindow.student);
            }
        }

        private void studentSelected(Student student)
        {
            // Change the property in the object to match the newly selected student
            selectedStudent_ = student;

            // Change the name label
            lblStudentName.Content = student.StudentName;

            // Enable & populate combobox
            cmbClass.IsEnabled = true;
            cmbClass.Items.Clear();

            // See which classes there are in their year group
            List<Class> classList = (from classes in App.db.Classes
                                     where classes.ClassYear == student.StudentYear
                                     select classes).ToList();

            // Loop and put into combobox, but only if they're /not/ in the class. (You can't be added to a class you're already in, of course)
            foreach (Class cl in classList)
            {
                // Logic for making sure they aren't in the class: The number of linking StudentClasses for their StudentID and the class' ClassID should be 0.
                if ((from studentClasses in App.db.StudentClasses
                     where studentClasses.ClassID == cl.ClassID && studentClasses.StudentID == student.StudentID
                     select studentClasses).Count() == 0)
                {
                    // Add it to the combobox if there aren't any instances
                    cmbClass.Items.Add(cl.ClassName);
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Class s_class;
            
            // Catch the single exception
            try
            {
                s_class = (from classes in App.db.Classes
                                 where classes.ClassName == cmbClass.SelectedItem.ToString()
                                 select classes).Single();
            }
            catch (Exception)
            {
                // The only real exception that the code above can cause is if Single throws an exception
                //  due to no matches, meaning they didn't select a class.
                App.Message("Validation Error", "You must select a class.");
                return;
            }

            // Make sure they've selected a student
            if (selectedStudent_ == null)
            {
                App.Message("Validation Error", "You haven't selected a student.");
                return;
            }

            // Make a new link object for the student and class
            StudentClass scLink = new StudentClass()
            {
               Class = s_class,
               Student = selectedStudent_,
            };

            // Insert the new link into the database
            App.db.StudentClasses.InsertOnSubmit(scLink);
            App.db.SubmitChanges();

            // Close this window and give a success message with no annoying sound (Since they will be adding a lot of these every year and the noise would get irritating)
            this.Close();
            App.Message("Success", "Student added to class successfully.", /* Noise parameter = false */ false);
        }
    }
}
