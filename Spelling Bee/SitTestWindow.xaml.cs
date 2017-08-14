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
using System.Windows.Threading;

namespace Spelling_Bee
{
    // Enum to simplify switch code and make the code easier to understand
    enum TestMode
    {
        TESTMODE_REVISE,
        TESTMODE_TAKE
    }

    /// <summary>
    /// Interaction logic for SitTestWindow.xaml
    /// </summary>
    public partial class SitTestWindow 
    {
        TestMode mode_;
        Test test_;
        Student student_;
        TimeSpan time_;
        TimeSpan timeRemaining_;
        DispatcherTimer timer_;

        List<Label> definitions;
        List<TextBox> answers;

        public SitTestWindow(char mode, Test test, Student student)
        {
            InitializeComponent();

            // Check if they want to revise or take the test
            switch (mode)
            {
                case 'r':
                    mode_ = TestMode.TESTMODE_REVISE;
                    break;
                case 't':
                    mode_ = TestMode.TESTMODE_TAKE;
                    break;
            }

            // Assign the properties for test and student
            test_ = test;
            student_ = student;

            // Set form labels and form grid
            lblName.Content = "Test: " + test_.TestName;
            switch (mode_)
            {
                case TestMode.TESTMODE_REVISE:
                    // If they are revising, we set the form up for it
                    lblMode.Content = "Mode: Revise";

                    pgbTimer.Value = 0;
                    lblTime.Content = "Time Remaining: Unlimited";

                    break;
                case TestMode.TESTMODE_TAKE:
                    // If they are taking the test, it's more complicated. We set up the form, timer, and a few other things:
                    lblMode.Content = "Mode: Take Test";

                    // Initialise times
                    time_ = new TimeSpan(0, test_.TestTimeLength.Value, 0);
                    timeRemaining_ = new TimeSpan(0, test_.TestTimeLength.Value, 0);
                    
                    // Set up more form data
                    lblTime.Content = "Time Remaining: " + timeRemaining_.ToString(@"mm\:ss");
                    btnCheck.Content = "Submit"; // 'Check' seems a bit inappropriate for a real test

                    // And enable the timer
                    timer_ = new DispatcherTimer();
                    timer_.Interval = new TimeSpan(0, 0, 1);
                    timer_.Tick += timer__Tick;
                    timer_.Start();

                    // Set the progressbar value to 0 (0% time over)
                    pgbTimer.Value = 0;

                    break;
            }

            // Create a new scope to prevent pollution in the current scope
            {
                // Keep a list of definition labels and answers
                definitions = new List<Label>();
                answers = new List<TextBox>();

                // Iteration
                int i = 0;
                foreach (TestQuestion question in test_.TestQuestions)
                {
                    // Create new row
                    grdQuestions.RowDefinitions.Insert(i, new RowDefinition() { Height = new GridLength(40) });
                    
                    // Create definition label
                    Label lblDefinition = new Label()
                    {
                        Content = question.Question,
                        // Set style
                        Style = (Style)FindResource("StudentFormLabel")
                    };
                    Grid.SetRow(lblDefinition, i);

                    // Create answer box
                    TextBox txtAnswer = new TextBox()
                    {
                        // Set style
                        Style = (Style)FindResource("StudentTextBoxStyle"),
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        Width = 160,
                        Margin = new Thickness(5, 0, 0, 0),
                    };
                    Grid.SetRow(txtAnswer, i);
                    Grid.SetColumn(txtAnswer, 1);

                    // Add them to relevant lists & grid.
                    definitions.Add(lblDefinition);
                    answers.Add(txtAnswer);
                    grdQuestions.Children.Add(lblDefinition);
                    grdQuestions.Children.Add(txtAnswer);

                    // Increment counter
                    i++;
                }
            }
        }

        void timer__Tick(object sender, EventArgs e)
        {
            timeRemaining_ = timeRemaining_.Subtract(new TimeSpan(0, 0, 1));
            // Value is a percentage since it is between 0 and 100
            pgbTimer.Value = ((100 / time_.TotalSeconds) * timeRemaining_.TotalSeconds);

            // Update time label content
            lblTime.Content = "Time Remaining: " + timeRemaining_.ToString(@"mm\:ss");

            // Automatically submit when timer reaches 0
            if (timeRemaining_.TotalSeconds < 1)
            {
                check();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // If they close the window, stop the timer
            if (timer_ != null)
                timer_.Stop();
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            check();
        }

        private void check()
        {
            // Unhighlight any highlighted from last check
            foreach (TextBox answerBox in answers)
            {
                answerBox.BorderThickness = new Thickness(1);
            }


            int i = 0;
            int score = 0;
            int major = 0;
            int minor = 0;
            int correct = 0;

            // Create a list of questionresults and a result object
            TestResult result = null;
            List<TestQuestionResult> questionResults = new List<TestQuestionResult>();

            // If they're taking the test for real, we instance the result object
            if (mode_ == TestMode.TESTMODE_TAKE)
            {
                result = new TestResult()
                {
                    Student = student_,
                    Test = test_,
                };
            }

            foreach (TextBox answer in answers)
            {
                string check = answer.Text;
                // Get some helper variables which save repeatedly calling the methods getErrorStatus and ElementAt
                TestQuestion question = test_.TestQuestions.ElementAt(i);
                TestQuestion.ErrorType errorType = question.getErrorStatus(check);

                // If it's revision, we don't create results.
                // If it's a full test, we do.
                // Therefore we switch the type here.
                switch (mode_)
                {
                    case TestMode.TESTMODE_TAKE:
                        // Check their results, inserted into the list below the switch block
                        TestQuestionResult questionResult = new TestQuestionResult()
                        {
                            Answer = check,
                            TestQuestion = question,
                        };

                        // Check what type of error they made
                        switch (errorType)
                        {
                            case TestQuestion.ErrorType.CORRECT:
                                // +2 score
                                questionResult.Score = 2;
                                score += 2;
                                correct++;
                                break;
                            case TestQuestion.ErrorType.ERRORMAJOR:
                                // No score for a major error
                                questionResult.Score = 0;
                                major++;
                                break;
                            case TestQuestion.ErrorType.ERRORMINOR:
                                // +1 score for a minor error
                                questionResult.Score = 1;
                                score += 1;
                                minor++;
                                break;
                        }

                        questionResults.Add(questionResult);

                        break;
                    case TestMode.TESTMODE_REVISE:
                        // Check and highlight mistakes.
                        // In revision mode, we don't track anything but
                        //  which ones they had wrong.
                        // We class all errors as 'major'.
                        if (errorType != TestQuestion.ErrorType.CORRECT)
                        {
                            answers.ElementAt(i).BorderThickness = new Thickness(2);
                            major++;
                        }
                        break;
                }

                i++; // Increase interator for finding the questions
            }

            // Give feedback
            switch (mode_)
            {
                case TestMode.TESTMODE_TAKE:
                    int percent;
                    // Percent formula is 100 / max * num
                    percent = (int)((100.0f / test_.MaxScore) * score);

                    // Stop timer before showing modal dialog
                    timer_.Stop();

                    // Show score to user
                    App.Message("Score", "You had a score of " + score + " meaning you had a total of " + percent + "%.");

                    // Set the score and update the database entry
                    result.Score = score;
                    App.db.SubmitChanges();
                    
                    // Set the foreign key for results since it was submitted into the database by SubmitChanges, and given
                    //  a primary key.
                    foreach (TestQuestionResult questionResult_i in questionResults)
                    {
                        questionResult_i.ResultsID = result.ResultsID;
                    }

                    App.db.SubmitChanges();

                    // Now we want to close this window.
                    Close();
                    break;
                case TestMode.TESTMODE_REVISE:
                    // If revising, they get a boolean response - Either they get told how many were incorrect, or they get a well done message.
                    if (major == 0)
                    {
                        if (App.getBoolean("Well Done!", "You had them all right! Would you like to exit revision for this test now?"))
                        {
                            this.Close();
                        }
                    }
                    else
                    {
                        // Notify them how many they had wrong
                        App.Message("Feedback", "You had " + major + " incorrect. They have been highlighted.");
                    }
                    break;
            }
        }
    }
}