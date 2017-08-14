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
    /// Interaction logic for ViewStudentTestAnalysisWindow.xaml
    /// </summary>
    public partial class ViewStudentTestAnalysisWindow 
    {
        TestResult result_;
        int correct_;
        int minor_;
        int major_;

        public ViewStudentTestAnalysisWindow(TestResult result)
        {
            InitializeComponent();

            result_ = result;

            setFormContents();
        }

        void calculateValues()
        {
            // Simple recursion to get the variables set up
            foreach (TestQuestionResult tqr in result_.TestQuestionResults)
            {
                if (tqr.Score == 0)
                    major_++;
                else if (tqr.Score == 1)
                    minor_++;
                else if (tqr.Score == 2)
                    correct_++;
            }
        }

        void setFormContents()
        {
            calculateValues();            

            // Set the form up
            lblTitle.Content = "Analysis for: " + result_.Test.TestName + ", completed by " + result_.Student.StudentName;

            lblCorrect.Content = correct_;
            lblMinor.Content = minor_;
            lblMajor.Content = major_;

            txtAnalysis.Text = generateAnalysis();

            lstStudentData.ItemsSource = result_.TestQuestionResults;
        }

        /// <summary>
        /// For generating analysis for the window
        /// </summary>
        /// <returns>Textual analysis</returns>
        string generateAnalysis()
        {
            // This method is full of relatively straightforward comparisons to generate a string.

            string returnString = "";

            returnString += result_.Student.StudentName + " did this test ";

            // We want to check their percentage.
            // 100% is classed as 'perfectly'
            // 80% and above is classed as 'very well'
            // 60% and above is classed as 'well'
            // 40% and above is classed as 'poorly'
            // 0% and above is classed as 'badly'
            int percent = result_.Percentage;

            if (percent == 100)
                returnString += "perfectly";
            else if (percent >= 80)
                returnString += "very well";
            else if (percent >= 60)
                returnString += "well";
            else if (percent >= 40)
                returnString += "poorly";
            else
                returnString += "badly";
            
            returnString += " with their score of " + result_.Score + " out of " + result_.MaxScore + ". ";

            // Now we want to give feedback on their minor/major status
            if (major_ == 0 && minor_ == 0)
                returnString += "They made no errors at all, which is a fantastic result!";
            else if (minor_ > major_)
                returnString += "Most of their errors were minor which can be righted quickly.";
            else if (major_ > minor_)
                returnString += "Most of their errors were major, and they might need a bit more improving in this respect, and a lot more revision.";
            else
                returnString += "They made both minor and major mistakes and will need to revise more for the next test.";

            // Now give the teacher some quick feedback on how to help them improve
            returnString += " It is recommended that you ";

            if (percent == 100)
                returnString += "allow them to express their potential more.";
            else if (percent > 80)
                returnString += "make sure they are more confident with righting simple mistakes";
            else
                returnString += "ensure that they revise more in future.";

            return returnString;
        }
    }
}
