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
	public partial class EditClassWindow 
	{
		public EditClassWindow(Class classToEdit = null)
		{
			this.InitializeComponent();
			
			// If they passed in a class to edit as the optional argument, then we set the form up using it.
            if (classToEdit != null)
            {
                selectedClass_ = classToEdit;
                enableButtons();
                showClassDetails();
            }
		}

        Class selectedClass_;

        /// <summary>
        /// Enables all the buttons on the form (For when class is selected)
        /// </summary>
        private void enableButtons()
        {
            btnName.IsEnabled = true;
            btnYear.IsEnabled = true;
            btnTeacher.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnAddStudent.IsEnabled = true;
            btnRemoveStudent.IsEnabled = true;
        }

        /// <summary>
        /// Disables all the buttons on the form (For when class is deleted)
        /// </summary>
        private void disableButtons()
        {
            btnName.IsEnabled = false;
            btnYear.IsEnabled = false;
            btnTeacher.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnAddStudent.IsEnabled = false;
            btnRemoveStudent.IsEnabled = false;
        }

        private void showClassDetails()
        {
            // Basic property setting for the form display
            lblName.Content = selectedClass_.ClassName;
            lblYear.Content = "Year " + selectedClass_.ClassYear.Value;
            lblTeacher.Content = selectedClass_.Staff.FullName;

            // Fill the list of students
            updateStudentsList();
        }

        private void updateStudentsList()
        {
            // Quite a bit more complicated than the above. Here, we want to set the source of the
            //  list view to the students in the class. Due to the added layer of a join table, this must be
            //  done through a LINQ query.
            lstStudents.ItemsSource = from students in App.db.Students
                                      // Use a JOIN to get data from the join table.
                                      join studentClasses in App.db.StudentClasses
                                          // Drop any irrelevant data
                                      on students.StudentID equals studentClasses.StudentID
                                      where studentClasses.Class == selectedClass_
                                      select students;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // Select a class to edit using the SelectClassWindow.
            SelectClassWindow scw = new SelectClassWindow();
            scw.ShowDialog();

            if (scw.selected)
            {
                // Set the selected class & enable buttons, and finally show details in the form
                selectedClass_ = scw.selectedClass;
                enableButtons();
                showClassDetails();
            }
        }

        private void btnName_Click(object sender, RoutedEventArgs e)
        {
            string newName = App.getString("Enter a new class name");

            // Class names must be unique, so check that first:
            int uniqueClassTest = (from classes in App.db.Classes
                                   where classes.ClassName == newName
                                   select classes).Count();

            // If there's already a class with that name, tell them and cancel routine
            if (uniqueClassTest > 0)
            {
                App.Message("Validation Error", "There is already a class with the name" + newName);
                return;
            }


            if (newName != null && newName.Trim() != "")
            {
                // Update name & save changes to database
                lblName.Content = newName;
                selectedClass_.ClassName = newName;
                App.db.SubmitChanges();
            }
        }

        private void btnYear_Click(object sender, RoutedEventArgs e)
        {
            int newYear = App.getYear("Select a new class year.");

            // > 2 is a check to make sure they selected a year.
            if (newYear > 2 && App.getBoolean("Are you sure?", "This will remove all students from the class, as they are not in the correct year."))
            {
                // Delete student -> class data for this class: If the year is changed, the students should no longer be in this class.
                App.db.StudentClasses.DeleteAllOnSubmit(
                    // Use a lambda to select from the classes which match this ID
                    App.db.StudentClasses.Where(x => x.ClassID == selectedClass_.ClassID)
                );

                // Update form, class and database
                lblYear.Content = "Year " + newYear;
                selectedClass_.ClassYear = newYear;
                App.db.SubmitChanges();
            }
        }

        private void btnTeacher_Click(object sender, RoutedEventArgs e)
        {
            // Open a select teacher window
            SelectTeacherWindow stw = new SelectTeacherWindow();
            stw.ShowDialog();

            // If they selected a member of staff
            if (stw.selected)
            {
                // Set the teacher label to match their name and update the class & db
                lblTeacher.Content = stw.selectedTeacher.FullName;
                selectedClass_.Staff = stw.selectedTeacher;
                App.db.SubmitChanges();
            }
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            // Open the add student to class window
            AddStudentToClassWindow adtcw = new AddStudentToClassWindow();
            adtcw.ShowDialog();

            // Update the student list
            updateStudentsList();
        }

        private void btnRemoveStudent_Click(object sender, RoutedEventArgs e)
        {
            if (lstStudents.SelectedIndex != -1)
            {
                // We can get a student object from the list, and find the StudentClass instance from a simple
                //  selection
                Student student = (Student)lstStudents.SelectedItem;

                // Now do a where with a lambda to select the StudentClass objects and delete them
                App.db.StudentClasses.DeleteAllOnSubmit(
                    App.db.StudentClasses.Where(x => x.StudentID == student.StudentID && x.ClassID == selectedClass_.ClassID)
                );

                // Push changes to the database
                App.db.SubmitChanges();

                updateStudentsList();
            }
            else
                App.Message("Validation Error", "You need to select a student to remove from this class.");
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Confirm that they want to delete for certain
            if (App.getBoolean("Are you sure?", "You cannot undo deletions, and this will delete all information about this class, including tests set for this class."))
            {
                // Select and delete all student/class relations for this class
                App.db.StudentClasses.DeleteAllOnSubmit(
                    App.db.StudentClasses.Where(x => x.ClassID == selectedClass_.ClassID)
                );

                // Loop all tests where the class ID corresponds to this class and it is not a whole year test.
                foreach (Test test in App.db.Tests.Where(x => x.ClassID == selectedClass_.ClassID && x.WholeYearTest == false))
                {
                    // Delete all questions, results and questionresults for this test
                    App.db.TestQuestions.DeleteAllOnSubmit(
                        App.db.TestQuestions.Where(
                            x => x.TestID == test.TestID
                        )
                    );

                    // Loop and delete all results and corresponding QuestionResults
                    foreach (TestResult result in App.db.TestResults.Where(x => x.TestID == test.TestID))
                    {
                        App.db.TestQuestionResults.DeleteAllOnSubmit(
                            App.db.TestQuestionResults.Where(
                                x => x.ResultsID == result.ResultsID
                            )
                        );

                        App.db.TestResults.DeleteOnSubmit(result);
                    }
                }

                // Finally, delete the class and push the changes to the database.
                App.db.Classes.DeleteOnSubmit(selectedClass_);
                App.db.SubmitChanges();

                // And of course, disable the form
                lblName.Content = "";
                lblTeacher.Content = "";
                lblYear.Content = "";
                disableButtons();
            }
        }
	}
}