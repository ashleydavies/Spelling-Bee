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
    /// <summary>
    /// Interaction logic for StudentRoot.xaml
    /// </summary>
    public partial class StudentRoot 
    {
        private Student login;

        public StudentRoot(Student login)
        {
            InitializeComponent();

            // Capture logged in user from parameter, and store into the login variable
            this.login = login;
            // Display welcome label correctly
            lblName.Content = "Welcome, " + login.StudentName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Add filters into the classes box for filtering tests
            cmbClasses.Items.Add("All");
            cmbClasses.Items.Add("All Year " + login.StudentYear);
            
            // Now add one for each of their classes
            foreach (StudentClass studentClass in login.StudentClasses)
            {
                Class s_class = studentClass.Class;
                cmbClasses.Items.Add(s_class.ClassName);
            }

            // And fill the test list
            populateTestList_All();
        }

        void updateTestsList()
        {
            // If they've not selected one
            if (cmbClasses.SelectedItem == null)
            {
                // Fill it with all of the tests
                populateTestList_All();
                return;
            }

            string selectedString = cmbClasses.SelectedItem.ToString();

            // Now check if they've submitted "All" or "All Year", in which case just follow the predictable actions of displaying all or all of their whole-yeargroup tests
            if (selectedString == "All")
            {
                populateTestList_All();
            }
            else if (selectedString.StartsWith("All Year"))
            {
                populateTestList_Year(login.StudentYear);
            }
            else
            {
                // Otherwise, notify the class method with the class name string
                populateTestList_Class(selectedString);
            }

            
        }

        void populateTestList_All()
        {
            // Fill the items of the tests list with all tests
            lstTests_Revise.ItemsSource = from tests in App.db.Tests
                                          // Join to find any classes that this student is in and count those in too
                                          join studentClasses in App.db.StudentClasses
                                          on new { X = tests.ClassID.Value, Y = login.StudentID }
                                          equals new { X = studentClasses.ClassID, Y = studentClasses.StudentID }
                                          into ssClasses
                                          from subStudentClass in ssClasses.DefaultIfEmpty()
                                          where (
                                          // Only choose entries where they are for the whole year AND it's this student's year
                                           ((tests.WholeYearTest == true && tests.TestYear == login.StudentYear)
                                           ||
                                           (subStudentClass != null && subStudentClass.StudentID == login.StudentID))
                                           &&
                                           // They must be in the future to revise so check the date
                                           tests.TestBegin.Value > DateTime.Now
                                          )
                                          // Order by the date so ones which are being tested soonest come up first
                                          orderby tests.TestBegin.Value ascending
                                          select tests;
            
            // This query is almost identical to above, albeit with slight modifications, as annotated. See above annotations for information.
            lstTests_Take.ItemsSource = (from tests in App.db.Tests
                                         join studentClasses in App.db.StudentClasses
                                         on new { X = tests.ClassID.Value, Y = login.StudentID }
                                         equals new { X = studentClasses.ClassID, Y = studentClasses.StudentID }
                                         into ssClasses
                                         from subStudentClass in ssClasses.DefaultIfEmpty()
                                         where (
                                          ((tests.WholeYearTest == true && tests.TestYear == login.StudentYear)
                                          ||
                                          (subStudentClass != null && subStudentClass.StudentID == login.StudentID))
                                          &&
                                          // Make sure the test is happening now, by checking if it was meant to start in the past.
                                          tests.TestBegin.Value < DateTime.Now
                                          &&
                                          // And it must be an open test
                                          tests.TestOpen == true
                                         )
                                         orderby tests.TestBegin.Value ascending
                                         // Now we have to filter out tests the user has already taken
                                         //  which can be easily done with a where statement and a simple lambda.
                                         //  the lambda (=>) checks the database for any existing results for this student
                                         //  that correspond to each particular test we check, to ensure that
                                         //  a student cannot take the same test twice.
                                         select tests).Where(y => ((from results in App.db.TestResults
                                                                    where results.StudentID == login.StudentID
                                                                    && results.TestID == y.TestID
                                                                    // We then check if the count is 0, meaning there are no acceptable entries
                                                                    //  which is what we want (It means they have not taken the test yet,
                                                                    //  so we display it)
                                                                    select results).Count() == 0));
        }

        void populateTestList_Class(string className)
        {
            // Select tests where the class corresponds to the className given.
            // For revision, we check if the date is in the future, and for
            //  testing we check if the date is in the past.
            // As usual, we organise by date ascending to ensure that more urgent
            //  tests appear first.
            lstTests_Revise.ItemsSource = from tests in App.db.Tests
                                          where tests.Class.ClassName == className
                                          && tests.TestBegin > DateTime.Now
                                          orderby tests.TestBegin.Value ascending
                                          select tests;

            lstTests_Take.ItemsSource = (from tests in App.db.Tests
                                         where tests.Class.ClassName == className
                                         && tests.TestBegin < DateTime.Now
                                         && tests.TestOpen == true
                                         orderby tests.TestBegin.Value ascending
                                         // Now we again have to filter out tests the user has already taken.
                                         // See documentation for identical statement above.
                                         select tests).Where(y => ((from results in App.db.TestResults
                                                                    where results.StudentID == login.StudentID
                                                                    && results.TestID == y.TestID
                                                                    // We then check if the count is 0, meaning there are no acceptable entries
                                                                    //  which is what we want (It means they have not taken the test yet,
                                                                    //  so we display it)
                                                                    select results).Count() == 0));
        }

        void populateTestList_Year(int year)
        {
            // Select from tests that correspond to this year group
            //  whereby the whole year test must be true and it must be corresponding to the year provided above.
            lstTests_Revise.ItemsSource = from tests in App.db.Tests
                                          where tests.WholeYearTest == true && tests.TestYear == year
                                          && tests.TestBegin > DateTime.Now
                                          orderby tests.TestBegin.Value ascending
                                          select tests;

            lstTests_Take.ItemsSource = (from tests in App.db.Tests
                                         where tests.WholeYearTest == true && tests.TestYear == year
                                         && tests.TestBegin < DateTime.Now
                                         && tests.TestOpen == true
                                         orderby tests.TestBegin.Value ascending
                                         // Now we again have to filter out tests the user has already taken.
                                         // See documentation for identical statement above.
                                         select tests).Where(y => ((from results in App.db.TestResults
                                                                    where results.StudentID == login.StudentID
                                                                    && results.TestID == y.TestID
                                                                    // We then check if the count is 0, meaning there are no acceptable entries
                                                                    //  which is what we want (It means they have not taken the test yet,
                                                                    //  so we display it)
                                                                    select results).Count() == 0));
        }

        private void cmbClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When it changes, call the updateTestsList method to automatically filter tests
            updateTestsList();
        }


        // Load the revision window for the selected test
        private void btnRevise_Click(object sender, RoutedEventArgs e)
        {
            // Simple selection validation
            if (lstTests_Revise.SelectedIndex == -1)
            {
                App.Message("Sorry, I don't know what to do.", "Please select a test to revise.");
                return;
            }

            // Cast it to a test and open the revision window and show it
            Test reviseTest = (Test)lstTests_Revise.SelectedItem;
            SitTestWindow stw = new SitTestWindow('r', reviseTest, login);
            stw.ShowDialog();
        }

        private void btnTake_Click(object sender, RoutedEventArgs e)
        {
            // Make sure they've selected one
            if (lstTests_Take.SelectedIndex == -1)
            {
                App.Message("Sorry, I don't know what to do.", "Please select a test to sit.");
                return;
            }

            // Cast selection to a test and open the take test window, and also show it.
            Test takeTest = (Test)lstTests_Take.SelectedItem;
            SitTestWindow stw = new SitTestWindow('t', takeTest, login);
            stw.ShowDialog();

            // Update the tests lists, so that if they finished it they can't sit it again.
            updateTestsList();
        }
    }
}
