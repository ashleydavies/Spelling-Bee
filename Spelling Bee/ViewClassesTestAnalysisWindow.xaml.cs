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
    /// Interaction logic for ViewClassesTestAnalysisWindow.xaml
    /// </summary>
    public partial class ViewClassesTestAnalysisWindow 
    {
        Test test_;

        public ViewClassesTestAnalysisWindow(Test test)
        {
            InitializeComponent();

            test_ = test;

            lblTitle.Content = "Analysis for: " + test_.TestName + ", by classes";

            // Given a test, we want to get a list of the classes that sat it.
            // LINQ would be very overcomplicated here, so we'll just use a loop.
            // The loop will check every class of every student and add each unique one to a list.
            List<Class> classes = new List<Class>();

            foreach (TestResult result in test.TestResults)
            {
                // Loop each result
                // Get the student that sat this
                Student student = result.Student;
                
                // Loop their classes and check if they should be added to the classes list (Naturally add if necessary)
                foreach (StudentClass i_sclass in student.StudentClasses)
                {
                    // We ask it to count the number of classes where the ClassID is the same as the linked Class ID.
                    // If so, add it to the list.
                    if (classes.Count(x => x.ClassID == i_sclass.ClassID) == 0)
                        classes.Add(i_sclass.Class);
                }
            }

            // List formed, add to the list.
            lstClasses.ItemsSource = classes;
        }

        private void btnViewAnalysis_Click(object sender, RoutedEventArgs e)
        {
            ViewClassTestAnalysisWindow vctaw = new ViewClassTestAnalysisWindow(test_, (Class)lstClasses.SelectedItem);
            vctaw.Show();
        }

        private void lstClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When changes, enable the view analysis button, provided something is selected.
            if (lstClasses.SelectedIndex != -1)
                btnViewAnalysis.IsEnabled = true;
            else
                btnViewAnalysis.IsEnabled = false;
        }
    }
}
