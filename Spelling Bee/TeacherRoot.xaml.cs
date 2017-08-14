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
	public partial class TeacherRoot 
	{
        Staff login_;

		public TeacherRoot(Staff login)
		{
			this.InitializeComponent();

            // Capture & store the logged in user, and set the welcome message
            login_ = login;
            lblWelcome.Content = "Welcome, " + login_.getFullName();

            // Update the lists to display the information
            updateTestsList();
            updateClassList();

            // Check if the end of term buttons should be enabled
            checkEndTerm();
		}

        private void updateTestsList()
        {
            // Show tests for whole years or where the teacher teaches the class
            lstTests.ItemsSource = from tests in App.db.Tests
                                   where tests.WholeYearTest == true
                                   || tests.Class.TeacherID == login_.StaffID
                                   orderby tests.TestBegin descending
                                   select tests;
        }

        private void checkEndTerm()
        {
            // We need to check if there is a test that is:
            // 1. Closed
            // 2. End of term / End of year
            // 3. The end of term action has not been executed (I.e. it's still that term)
            int tests = (from test in App.db.Tests
                         where test.TestOpen == false &&
                         (test.LastTestOfYear == true || test.LastTestOfTerm == true) &&
                         (test.LastTestActionExecuted.Value == false || !test.LastTestActionExecuted.HasValue)
                         select test).Count();
            if (tests > 0)
            {
                // Enable merit assembly information to be accessed
                btnMeritAssembly.IsEnabled = true;
                btnAdvanceTerm.IsEnabled = true;
            }
            else
            {
                btnMeritAssembly.IsEnabled = false;
                btnAdvanceTerm.IsEnabled = false;
            }
        }

        private void updateClassList()
        {
            // Show classes where the TeacherID of the class corresponds to the currently logged in user's
            //  StaffID.
            lstClasses.ItemsSource = from classes in App.db.Classes
                                     where classes.TeacherID.Value == login_.StaffID
                                     select classes;
        }

        private void btnAddTest_Click(object sender, RoutedEventArgs e)
        {
            AddEditTestWindow atw = new AddEditTestWindow(login_);
            atw.ShowDialog();

            // Update tests listbox to reflect this change
            updateTestsList();

            // Check if the test fulfilled the end of term requirements
            checkEndTerm();
        }

        private void btnEditTest_Click(object sender, RoutedEventArgs e)
        {
            // First, make sure that they have selected a test.
            if (lstTests.SelectedIndex != -1)
            {
                Test selectedTest = (Test)lstTests.SelectedItem;
                AddEditTestWindow atw = new AddEditTestWindow(login_, selectedTest);
                atw.ShowDialog();
                
                // Now update the list, and buttons, to reflect any changes that may have taken place
                updateTestsList();
                enableTestButtons(selectedTest);
            }

            // Check if it was edited to fulfill the end of term requirements
            checkEndTerm();
        }

        // This is for the sole checkbox on the form, which enables/disables show all classes
        //  otherwise, it just shows classes they teach.
        private void chkViewAllClasses_Checked(object sender, RoutedEventArgs e)
        {
            // Update lstClasses with all classes, not just ones taught by this teachers
            lstClasses.ItemsSource = App.db.Classes;
        }

        private void chkViewAllClasses_Unchecked(object sender, RoutedEventArgs e)
        {
            // Update lstClasses, showing all classes where the teacher is the current teacher (via login ID)
            lstClasses.ItemsSource = from classes in App.db.Classes
                                     where classes.TeacherID.Value == login_.StaffID
                                     select classes;
        }

        private void btnCloseTest_Click(object sender, RoutedEventArgs e)
        {
            if (lstTests.SelectedIndex != -1 && App.getBoolean("Are you sure?", "After a test is closed, it cannot be reopened, but you will be able to view the results of the test."))
            {
                Test selectedTest = (Test)lstTests.SelectedItem;

                // Set the test to closed (False open = closed)
                selectedTest.TestOpen = false;

                // Update buttons to reflect changes, then submit to database.
                enableTestButtons(selectedTest);
                App.db.SubmitChanges();
            }
            else
            {
                App.Message("Validation Error", "Please select a test to close");
            }

            // Check if closing this test fulfilled the end of term requirements
            checkEndTerm();
        }

        private void lstTests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           if (lstTests.SelectedIndex != -1)
           {
               Test selectedTest = (Test)lstTests.SelectedItem;
               enableTestButtons(selectedTest);
           }
        }

        /// <summary>
        /// Enables (or disables) buttons based on the test provided
        /// </summary>
        /// <param name="test">Used to identify which buttons should be enabled and disabled</param>
        private void enableTestButtons(Test test)
        {
            btnCloseTest.IsEnabled = false;
            btnEditTest.IsEnabled = false;
            btnViewReportTest.IsEnabled = false;
            

            // If the test is open and it has started, they may close it
            if (test.TestOpen.Value && test.TestBegin.Value < DateTime.Now)
                btnCloseTest.IsEnabled = true;
            
            // If the test has not started, they can edit it
            if (test.TestBegin > DateTime.Now)
                btnEditTest.IsEnabled = true;

            // If the test is closed, they can view the report
            if (test.TestOpen.Value == false)
                btnViewReportTest.IsEnabled = true;
        }

        private void btnViewReportTest_Click(object sender, RoutedEventArgs e)
        {
            if (lstTests.SelectedIndex != -1)
            {
                Test test = (Test)lstTests.SelectedItem;
                ViewTestAnalysisWindow vtaw = new ViewTestAnalysisWindow(test);

                // Don't do a modal show, so they can compare two tests side-by-side.
                vtaw.Show();
            }
            else
            {
                App.Message("Validation Error", "Please select a test to view report for.");
            }
        }

        private void lstClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When they select something, enable the class buttons. Do the opposite if they deselect somehow.
            if (lstClasses.SelectedIndex == -1)
            {
                btnViewReportClass.IsEnabled = false;
                btnEditClass.IsEnabled = false;
            }
            else
            {
                btnViewReportClass.IsEnabled = true;
                btnEditClass.IsEnabled = true;
            }
        }

        private void btnViewReportClass_Click(object sender, RoutedEventArgs e)
        {
            // Show the report dialog for a class using the selected class as the class
            if (lstClasses.SelectedIndex != -1)
            {
                // As usual per analysis windows, we show regularly, not dialog, so they can side-by-side compare.
                ViewClassAnalysisWindow vcaw = new ViewClassAnalysisWindow((Class)lstClasses.SelectedItem);
                vcaw.Show();
            }
            else
            {
                App.Message("Validation Error", "Please select a class to view the report of.");
            }
        }

        private void btnEditClass_Click(object sender, RoutedEventArgs e)
        {
            // Show the edit class dialog. Most dialogs like this are only visible by sysadmins, but this one
            //  is shared, since it makes more sense for a teacher to be able to edit classes.
            // First, naturally, we make sure they've selected a class and then we pass it as an argument
            if (lstClasses.SelectedIndex != -1)
            {
                EditClassWindow ecw = new EditClassWindow((Class)lstClasses.SelectedItem);
                ecw.ShowDialog();
            }
            else
            {
                App.Message("Validation Error", "Please select a class to edit.");
            }
        }

        private void btnMeritAssembly_Click(object sender, RoutedEventArgs e)
        {
            MeritAssemblyWindow maw = new MeritAssemblyWindow();
            maw.ShowDialog();
        }

        private void btnAdvanceTerm_Click(object sender, RoutedEventArgs e)
        {
            // First, check if it's a half term advancement by finding the test causing this change
            // We need to check if there is a test that is:
            // 1. Closed
            // 2. End of term / End of year
            // 3. The end of term action has not been executed (I.e. it's still that term)
            Test changer = (from test in App.db.Tests
                            where test.TestOpen == false &&
                            (test.LastTestOfYear == true || test.LastTestOfTerm == true) &&
                            (test.LastTestActionExecuted.Value == false || !test.LastTestActionExecuted.HasValue)
                            select test).First();
                            // We use first in this case, since this code routine is only capable of running if one exists

            if (changer.LastTestOfYear.Value == true)
            {
                // End of year test
                // We want to do a good number of these here, which include:

                // 1. Delete all data relevant to year 6 pupils, as they will have left now
                // 2. Advance all other pupils forward a year
                // 3. Attempt to rename the classes automatically - Possibly the best part
                if (App.getBoolean("Are you sure?", "This will delete ALL data from year 6 pupils permanently, and make a huge amount of other changes such as moving pupils forward. This cannot be undone automatically."))
                {
                    // Delete all year 6 data
                    List<Student> studentList = (from students in App.db.Students
                                                 where students.StudentYear == 6
                                                 select students).ToList();
                    // Now delete all their results and such

                    // Automatically delete year 6 classes and studentclass relations using a simple where lambda
                    App.db.StudentClasses.DeleteAllOnSubmit(App.db.StudentClasses.Where(x => x.Class.ClassYear == 6));
                    App.db.Classes.DeleteAllOnSubmit(App.db.Classes.Where(x => x.ClassYear == 6));

                    foreach (Student student in studentList)
                    {
                        // Loop their results and delete the test question results
                        foreach (TestResult result in student.TestResults)
                        {
                            App.db.TestQuestionResults.DeleteAllOnSubmit(result.TestQuestionResults);
                        }
                        App.db.TestResults.DeleteAllOnSubmit(student.TestResults);
                        App.db.Students.DeleteOnSubmit(student);

                        App.db.SubmitChanges();
                    }

                    App.Message("Success", "All of the old data has been deleted. You will now be prompted to rename classes.");

                    if (App.getBoolean("Automatically rename?", "Would you like the system to attempt to automatically rename classes?"))
                    {
                        List<KeyValuePair<string, string>> renames = new List<KeyValuePair<string, string>>();

                        // Let's try to automatically update all class names
                        foreach (Class class_ in App.db.Classes)
                        {
                            string oldName = class_.ClassName;
                            // Replace any year digits with the new year
                            string newName = oldName.Replace((class_.ClassYear).ToString(), (class_.ClassYear + 1).ToString());

                            class_.ClassName = newName;

                            // Add the two names into the list so we can inform the user of the changes
                            renames.Add(new KeyValuePair<string,string>(oldName, newName));
                        }

                        // Inform them of the changes
                        string summary = "";

                        // Automatically concat in the form "4U=>5U " to the summary string
                        renames.ForEach(x => summary += x.Key + "=>" + x.Value + "  ");

                        App.Message("Success!", "Changed the following class names: " + summary);

                        // Submit all changes
                        App.db.SubmitChanges();
                    }

                    // Finally, advance all students & classes one year
                    foreach (Student student in App.db.Students)
                    {
                        student.StudentYear += 1;
                    }

                    foreach (Class class_ in App.db.Classes)
                    {
                        class_.ClassYear += 1;
                    }

                    App.db.SubmitChanges();
                }
            }
            else
            {
                // End of term test, which is much simpler. We just need to lock it and give a warning about merit assemblies.
                if (App.getBoolean("Are you sure?", "Please only do this after you have conducted the merit assembly! This will lock the merit assembly facility."))
                {
                    // Lock it down
                    // Get all tests where the last test action (Basically this) wasn't executed, convert them to a list, and run a simple lambda to set it to true. Nothing too complicated.
                    App.db.Tests.Where(x => x.LastTestActionExecuted.HasValue == false || x.LastTestActionExecuted.Value == false).ToList().ForEach(x => x.LastTestActionExecuted = true);
                    App.db.SubmitChanges();
                }
            }

            checkEndTerm();
        }
	}
}