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
    public partial class ViewStudentAnalysisWindow 
    {
        Student student_;

        public ViewStudentAnalysisWindow(Student student)
        {
            InitializeComponent();

            student_ = student;

            // Set the default form information such as the title of the form and the number of tests the user has sat
            lblTitle.Content = "Viewing student analysis for: " + student_.StudentName;
            lblTestNumber.Content = student_.TestResults.Count.ToString();

            if (student_.TestResults.Count > 0)
            {
                // Get average percentage:
                int totalP = 0;
                // Loop the test results (Which we cast to a list to do so) and increase totalP by the percentage
                student_.TestResults.ToList().ForEach(x => totalP += x.Percentage);
                int average = totalP / student_.TestResults.Count;

                // And update the form to represent this information
                lblAverageMark.Content = average + "%";
            }
            else
            {
                // If there are no tests sat by this user we give some feedback on that fact
                lblAverageMark.Content = "No data available (No tests sat)";
            }

            // Show the data relevant to the lists
            lstClassData.ItemsSource = student_.StudentClasses;
            lstTestData.ItemsSource = student_.TestResults;
        }

        private void lstTestData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable view test analysis button
            btnViewTestAnalysis.IsEnabled = true;
        }

        private void lstClassData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable view class test analysis button
            btnViewClassAnalysis.IsEnabled = true;
        }

        private void btnViewTestAnalysis_Click(object sender, RoutedEventArgs e)
        {
            // Open a view test analysis window for the selected test (we cast to a test to do so)
            ViewStudentTestAnalysisWindow vtaw = new ViewStudentTestAnalysisWindow((TestResult)lstTestData.SelectedItem);
            vtaw.Show();
        }

        private void btnViewClassAnalysis_Click(object sender, RoutedEventArgs e)
        {
            // Cast selected item to a StudentClass and pass the class to a viewclassanalysiswindow
            ViewClassAnalysisWindow vcaw = new ViewClassAnalysisWindow(((StudentClass)lstClassData.SelectedItem).Class);
        }
    }
}
