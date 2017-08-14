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
    /// Interaction logic for ViewTestAnalysisWindow.xaml
    /// </summary>
    public partial class ViewTestAnalysisWindow 
    {
        Test test_;
        System.Linq.IQueryable<TestResult> results_;

        int studentCount_;
        int totalMark_;
        int averageMark_;
        double standardDeviation_;

        public ViewTestAnalysisWindow(Test test)
        {
            InitializeComponent();

            // Do a quick exit if the test has no questions, to avoid division by zero errors
            if (test.TestQuestions.Count() == 0)
            {
                App.Message("Validation Error", "That test has no questions, and cannot be analysed.");
                Close();
            }

            // Set up window
            test_ = test;


            lblTitle.Content = "Results Overview For: " + test_.TestName;


            // Set up results
            results_ = from testResults in App.db.TestResults
                       where testResults.TestID == test_.TestID
                       select testResults;

            // Set up the student list
            lstStudentData.ItemsSource = results_;

            calculateVariables();

            lblStudentCount.Content = studentCount_;
            lblAveragePercentage.Content = averageMark_ + " / " + test.MaxScore;
            // Add percentage onto the label content with +=
            lblAveragePercentage.Content += " (" + (100 / test.MaxScore) * averageMark_ + "%)";
            lblStandardDeviation.Content = Math.Round(standardDeviation_, 2);

            // The textbox for the top/bottom achieving students is slightly more complicated
            //  but still relatively straightforward
            int lastScore = -1;

            // Only do this code if there are results, since otherwise it's invalid.
            if (results_.Count() != 0)
            {
                foreach (TestResult result in results_.OrderByDescending(x => x.Score.Value))
                {
                    // results.Score == lastScore ensures that if we're no longer the top in the class, we stop iterating
                    if (lastScore == -1 || result.Score.Value == lastScore)
                    {
                        // lastScore != -1 signifies non-first iteration
                        if (lastScore != -1)
                        {
                            // Put a new line in if it isn't the first line.
                            // We don't want to do this in the first line to avoid
                            //  having a new line to start.
                            txtTopInClass.Text += "\r\n";
                        }

                        txtTopInClass.Text += result.Student.StudentName;
                        // Update last score for future iterations
                        lastScore = result.Score.Value;
                    }
                    else
                    {
                        break;
                    }
                }

                int topScore = lastScore; // Used to break the worst in class if everyone had the same score

                lastScore = -1; // Set to -1 again for first-time initialisation trigger
                // Exactly the same but ordering ascendingly, and only occurs if the top score =/= to the bottom score.
                foreach (TestResult result in results_.OrderBy(x => x.Score.Value))
                {
                    // Quick check to break if the worst are also the top (i.e. everyone had the same mark)
                    if (topScore == result.Score.Value)
                        break;

                    // results.Score == lastScore ensures that if we're no longer the top in the class, we stop iterating
                    if (lastScore == -1 || result.Score.Value == lastScore)
                    {
                        if (lastScore != -1)
                        {
                            txtWorstInClass.Text += "\r\n";
                        }

                        txtWorstInClass.Text += result.Student.StudentName;

                        // Update last score for future iterations
                        lastScore = result.Score.Value;
                    }
                    else
                    {
                        break;
                    }
                }


                // Now calculate which word(s) were spelt wrong the most and least. Simple procedure, 
                Dictionary<string, int> scores = getWordsScoreOverall(results_);

                if (scores != null)
                {
                    // To order them, we're going to need a List of KeyValuePair instead
                    List<KeyValuePair<string, int>> scoresList = scores.ToList();

                    // Sort it using a simple lamba
                    scoresList.Sort((x, y) => x.Value.CompareTo(y.Value));

                    // Now grab the top and bottom scores
                    int bottomQScore = scoresList.First().Value;
                    int topQScore = scoresList.Last().Value;

                    // Now we list the top words
                    List<string> topWords = new List<string>();
                    List<string> bottomWords = new List<string>();

                    foreach (KeyValuePair<string, int> score in scoresList)
                    {
                        // Elseif is used intentionally to ensure that if the topQScore
                        //  is equal to the bottomQScore 
                        if (score.Value == topQScore)
                            topWords.Add(score.Key);
                        else if (score.Value == bottomQScore)
                            bottomWords.Add(score.Key);
                    }
                    txtSpeltRightMost.Text = string.Join("\r\n", topWords);
                    txtSpeltWrongMost.Text = string.Join("\r\n", bottomWords);
                }
            }

            // Finally, if it's a class test (And therefore only has one class),
            //  we hide the button to show class statistics
            if (test_.WholeYearTest == false)
                btnClassPerformance.Visibility = System.Windows.Visibility.Hidden;
        }
        
        private Dictionary<string, int> getWordsScoreOverall(IQueryable<TestResult> results)
        {
            if (results.Count() == 0)
                return null;

            Dictionary<string, int> scores = new Dictionary<string, int>();

            // Loop for every question position in the test
            // And create a new key/value pair in the score to avoid OutOfRange exceptions later on
            foreach (TestQuestionResult question in results.First().TestQuestionResults)
                scores.Add(question.TestQuestion.Answer, 0);

            // Loop each result
            foreach (TestResult result in results)
            {
                // For each result, loop the individual question results
                foreach (TestQuestionResult qResult in result.TestQuestionResults)
                {
                    // Find it's position in the index of the questions
                    scores[qResult.TestQuestion.Answer] += qResult.Score.Value;
                }
            }

            return scores;
        }

        private void calculateVariables()
        {
            // To allow += to operate correctly.
            // Probably gets simplified out by the compiler at some point anyway.
            totalMark_ = 0;

            studentCount_ = results_.Count();

            // Calculate total mark
            foreach (TestResult result in results_)
            {
                totalMark_ += result.Score.Value;
            }

            // Calculate average mark (Average = sum / n)
            if (studentCount_ != 0) // Avoid 0 division errors
                averageMark_ = totalMark_ / studentCount_;
            else
                averageMark_ = 0;

            // Standard deviation is a little more complicated, but luckily covered in Statistics.
            // First, we work out each difference from the mean squared and average the result.
            // This gets us the variance
            double variance = 0;

            foreach (TestResult result in results_)
            {
                variance += Math.Pow(averageMark_ - result.Score.Value, 2);
            }

            // Now divide by the student count, which gives us the true variance. (σ * σ)
            // Ordinarilly, we might divide by n-1, however we are not taking the results of a sample,
            //  but the whole population of test-takers, so we divide by n.
            variance = variance / studentCount_;

            // And the standard deviation is the root of the variance
            standardDeviation_ = Math.Sqrt(variance);
        }

        private void lstStudentData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Disable the button unless they've selected something
            btnViewAnalysis.IsEnabled = false;

            if (lstStudentData.SelectedIndex != -1)
            {
                btnViewAnalysis.IsEnabled = true;
            }
        }

        private void btnViewAnalysis_Click(object sender, RoutedEventArgs e)
        {
            TestResult result = (TestResult)lstStudentData.SelectedItem;

            // Bit of a mouthfull of a class name.
            ViewStudentTestAnalysisWindow vstaw = new ViewStudentTestAnalysisWindow(result);
            vstaw.Show();
            // As for TeacherRoot -> This Form we show it non-modally so that they can analyse multiple results next to eachother.
        }

        private void btnClassPerformance_Click(object sender, RoutedEventArgs e)
        {
            // This means they want to know how each class did for the test, so we'll bring up the ViewClassesTestAnalysisWindow.
            ViewClassesTestAnalysisWindow vctaw = new ViewClassesTestAnalysisWindow(test_);
            vctaw.Show();
        }
    }
}
