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
    /// Interaction logic for ViewClassAnalysisWindow.xaml
    /// </summary>
    public partial class ViewClassAnalysisWindow 
    {
        Class class_;

        public ViewClassAnalysisWindow(Class classS)
        {
            InitializeComponent();

            class_ = classS;

            // Set the title first of all
            lblTitle.Content = "Viewing overall analysis for class: " + class_.ClassName;

            // Grab a list of the students in this class:
            List<Student> students = new List<Student>();
            foreach (StudentClass link in class_.StudentClasses)
                students.Add(link.Student);

            // Now we have the list of students we can get a list of the unique tests sat by this class
            // Loop the students, loop their test results, and if it's unique, add it.
            List<Test> tests = new List<Test>();
            foreach (Student iStudent in students)
                foreach (TestResult result in iStudent.TestResults)
                    if (!tests.Contains(result.Test))
                        tests.Add(result.Test);

            // For the whole year tests, we want to grab all information from every test relevant to this year group.
            // We don't just use Where on 'tests' because that might leave out ones which noone in this class sat (Perhaps everyone was absent?)
            // So we make sure it's a WholeYearTest and it's the same TestYear as our class_.
            List<Test> wholeYearTests = App.db.Tests.Where(x => x.WholeYearTest == true && x.TestYear.Value == class_.ClassYear.Value).ToList();

            // Work out number of results total, along with other summed variables
            int results = 0;
            int totalPossibleMark = 0;
            int totalMark = 0;

            // Loop the test results to total those variables.
            foreach (Student iStudent in students)
                foreach (TestResult result in iStudent.TestResults)
                    if (wholeYearTests.Contains(result.Test))
                    {
                        results++;
                        totalMark += result.Score.Value;
                        totalPossibleMark += result.MaxScore;
                    }

            int avgMark;
            int avgMarkMax;
            int avgMarkPercent;

            // Calculate some variables using simple division and percent formulas
            if (results > 0)
            {
                avgMark = totalMark / results;
                avgMarkMax = totalPossibleMark / results;
                avgMarkPercent = (100 / avgMarkMax) * avgMark;

                // Given these, we can now calculate some of the values for the form:
                lblAverageMark.Content = avgMark + " / " + avgMarkMax + " (" + avgMarkPercent + "%)";
            }
            else
            {
                lblAverageMark.Content = "None sat";
            }

            lblPupilCount.Content = class_.StudentClasses.Count;
            // And throw the list of StudentClasses in the class into the list view.
            lstStudentData.ItemsSource = class_.StudentClasses;
            // And finally throw the list of Tests into the tests view
            lstTestData.ItemsSource = tests;
        }

        private void btnViewStudentAnalysis_Click(object sender, RoutedEventArgs e)
        {
            // Extract a StudentClass, and use that to get the Student
            Student selectedStudent = ((StudentClass)lstStudentData.SelectedItem).Student;

            // Create and show the student analysis dialog
            ViewStudentAnalysisWindow vstaw = new ViewStudentAnalysisWindow(selectedStudent);
            vstaw.Show();
        }

        private void lstStudentData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable or disable the button depending on whether one is selected.
            if (lstStudentData.SelectedIndex != -1)
                btnViewStudentAnalysis.IsEnabled = true;
            else
                btnViewStudentAnalysis.IsEnabled = false;
        }

        private void btnViewTestAnalysis_Click(object sender, RoutedEventArgs e)
        {
            // Get a test from the list
            Test selectedTest = (Test)lstTestData.SelectedItem;

            // Open the test analysis window for this class
            ViewClassTestAnalysisWindow vctaw = new ViewClassTestAnalysisWindow(selectedTest, class_);
            vctaw.Show();
        }

        private void lstTestData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // This method also enables or disables the relevant button depending on whether an item is selected
            if (lstTestData.SelectedIndex != -1)
                btnViewTestAnalysis.IsEnabled = true;
            else
                btnViewTestAnalysis.IsEnabled = false;
        }
    }
}
