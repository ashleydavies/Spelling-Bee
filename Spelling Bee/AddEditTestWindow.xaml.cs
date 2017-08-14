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
    /// Interaction logic for AddTestWindow.xaml
    /// </summary>
    public partial class AddEditTestWindow 
    {
        int words_ = 0;

        int words
        {
            get
            {
                return words_;
            }
            set
            {
                // Override the setter and change the words label whenever the words variable changes. Will save a lot of redundant code.
                words_ = value;
                lblWordCount.Content = "Words: " + words_;
            }
        }
        
        // The number of rows down the words begin
        const int baseWordRow = 4;

        // These two lists will be added/removed from respectively at the same time. .NET Lists preserve insertion order,
        //  so [i] in wordBoxes will always correspond to [i] in definitionBoxes, etc
        List<Label> wordLabels = new List<Label>();
        List<TextBox> wordBoxes = new List<TextBox>();
        List<Label> definitionLabels = new List<Label>();
        List<TextBox> definitionBoxes = new List<TextBox>();
        List<Button> deleteButtons = new List<Button>();


        // If they're editing, it will work somewhat differently to if they are adding a test from scratch, such as updating to database instead of inserting, so we keep track of this
        bool editing_ = false;
        Test editingTest_;

        public AddEditTestWindow(Staff teacher, Test test = null)
        {
            InitializeComponent();

            // Populate classes combobox
            List<Class> classList = (from classes in App.db.Classes
                                     // Only shows classes taught by this teacher.
                                     where classes.TeacherID == teacher.StaffID
                                     select classes).ToList();

            // Loop and put into combobox.
            foreach (Class cl in classList)
            {
                cmbClass.Items.Add(cl.ClassName);
            }

            // If they passed in a Test as the optional argument, their intention is to edit
            //  so set up the form to represent that.
            if (test != null)
            {
                txtName.Text = test.TestName;
                // Years are 3-based so we subtract 3 to get 0-4 instead of 3-6.
                cmbYear.SelectedIndex = test.TestYear.Value - 3;
                dtpDate.SelectedDate = test.TestBegin.Value;
                txtDuration.Text = test.TestTimeLength.ToString();
                cmbClass.SelectedItem = test.Class;
                
                // Set up checkboxes
                chkWholeYear.IsChecked = test.WholeYearTest.Value;
                chkEndOfTerm.IsChecked = test.LastTestOfTerm.Value;
                chkEndOfYear.IsChecked = test.LastTestOfYear.Value;

                // Finally, set up words
                foreach (TestQuestion question in test.TestQuestions)
                    newWord(question.Answer, question.Question);

                // Set the editing variables up
                editing_ = true;
                editingTest_ = test;
            }
            else
            {
                // The headteacher asked for 10 words, so by default, just create 10 empty words. The +/- word feature will almost definitely be a welcome addition, though.
                for (int i = 0; i < 10; i++)
                {
                    newWord();
                }
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            // Add a new word when they click new
            newWord();
        }

        /// <summary>
        /// Sets up a new word/definition row on the form
        /// </summary>
        /// <param name="word">Optional, provides a default word value</param>
        /// <param name="definition">Optional, provides a default definition value</param>
        public void newWord(string word = "", string definition = "")
        {
            // Increase word count
            words++;

            // Add a row to the grid for the word, with the height 40
            grdMain.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
            
            // Create the new row elements, with simple setup variables such as the style, and content
            Label wordLabel = new Label() { Content = "Word: ", Style = (Style)FindResource("StaffFormLabel") };
            // Set the row for the word label in the grid
            Grid.SetRow(wordLabel, baseWordRow + words);

            // Create the new row elements, with simple setup variables such as the style, and content
            TextBox wordBox = new TextBox() { Margin = new Thickness(5) };
            // Set column and row for the word box in the grid
            Grid.SetColumn(wordBox, 1);
            Grid.SetRow(wordBox, baseWordRow + words);

            // If there's a word provided as a parameter (When editing this will be the case), we put it into the form
            if (word != "")
                wordBox.Text = word;

            // Add a new label for the definition of the word
            Label definitionLabel = new Label() { Content = "Definition: ", Style = (Style)FindResource("StaffFormLabel") };
            // Additionally, setup it's grid position
            Grid.SetColumn(definitionLabel, 2);
            Grid.SetRow(definitionLabel, baseWordRow + words);

            // And also a textbox for definitions
            TextBox definitionBox = new TextBox() { Margin = new Thickness(5) };
            // As usual, set the grid position
            Grid.SetColumn(definitionBox, 3);
            Grid.SetRow(definitionBox, baseWordRow + words);

            // And once again, if editing, we put the content into the textbox
            if (definition != "")
                definitionBox.Text = definition;

            //Delete button setup, relatively simple right/center-aligned button which allows them to delete a word.
            Button deleteButton = new Button() { Content="x", HorizontalAlignment=HorizontalAlignment.Right, VerticalAlignment=VerticalAlignment.Center, Width=24.0, Margin=new Thickness(5) };
            // Set it's grid position
            Grid.SetColumn(deleteButton, 4);
            Grid.SetRow(deleteButton, baseWordRow + words);
            
            // Add all of the elements above to the grid
            grdMain.Children.Add(wordLabel);
            grdMain.Children.Add(wordBox);
            grdMain.Children.Add(definitionLabel);
            grdMain.Children.Add(definitionBox);
            grdMain.Children.Add(deleteButton);

            // And then add them to the lists
            wordLabels.Add(wordLabel);
            wordBoxes.Add(wordBox);
            definitionLabels.Add(definitionLabel);
            definitionBoxes.Add(definitionBox);
            deleteButtons.Add(deleteButton);

            // Finally, register an event handler for the new delete button's click event.
            deleteButton.Click += deleteButton_Click;

            // Move down the new button
            Grid.SetRow(btnNew, baseWordRow + words + 1);
        }

        void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Check the sender parameter to see which button was clicked
            Button btnSentFrom = (Button)sender;

            // Decrease word count
            words--;

            // Work out the index number that the button was at:
            int indxN = deleteButtons.IndexOf(btnSentFrom);
            int rowN = indxN + baseWordRow;

            // And now delete everything from that row by removing all references.
            grdMain.Children.Remove(wordLabels.ElementAt(indxN));
            grdMain.Children.Remove(wordBoxes.ElementAt(indxN));
            grdMain.Children.Remove(definitionLabels.ElementAt(indxN));
            grdMain.Children.Remove(definitionBoxes.ElementAt(indxN));
            grdMain.Children.Remove(deleteButtons.ElementAt(indxN));

            // Remove all of them from the lists, too
            wordLabels.RemoveAt(indxN);
            wordBoxes.RemoveAt(indxN);
            definitionLabels.RemoveAt(indxN);
            definitionBoxes.RemoveAt(indxN);
            deleteButtons.RemoveAt(indxN);
            
            // And remove the row they were in to shift everything else up
            grdMain.RowDefinitions.RemoveAt(rowN);

            fixRows();
        }

        /// <summary>
        /// Fixes all the rows of all form elements (Used when a row is removed)
        /// </summary>
        void fixRows()
        {
            for (int i = 0; i < words; i++)
            {
                // We make sure all elements are set to their correct row by setting their grid position to match where it should be (baseWordRow is the row for element #0, so we add one and the word number to it to get the correct row.
                Grid.SetRow(wordLabels.ElementAt(i), baseWordRow + i + 1);
                Grid.SetRow(wordBoxes.ElementAt(i), baseWordRow + i + 1);
                Grid.SetRow(definitionLabels.ElementAt(i), baseWordRow + i + 1);
                Grid.SetRow(definitionBoxes.ElementAt(i), baseWordRow + i + 1);
                Grid.SetRow(deleteButtons.ElementAt(i), baseWordRow + i + 1);
            }
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation Logic
            if (words == 0)
            {
                App.Message("Validation Error", "A minimum of 1 word must be provided for the test.");
                return;
            }

            // Check for empty boxes
            for (int i = 0; i < wordBoxes.Count; i++)
            {
                if (wordBoxes[i].Text.Trim().Length == 0 || definitionBoxes[i].Text.Trim().Length == 0)
                {
                    // i + 1 removes the 0-based aspect of the number for human consumption.
                    App.Message("Validation Error", "Word " + (i + 1) + " lacks a definition or word.");
                    return;
                }
            }

            // Check if cmbYear is selected
            if (cmbYear.SelectedIndex == -1)
            {
                App.Message("Validation Error", "You must select a year group.");
                return;
            }

            // Check if txtDuration is an integer
            int duration;
            if (!int.TryParse(txtDuration.Text, out duration) || duration < 1)
            {
                App.Message("Validation Error", "Duration must be a whole number that is one or more");
                return;
            }

            // Validate datepicker selection
            if (dtpDate.SelectedDate == null)
            {
                App.Message("Validation Error", "You must select a date for the test to begin at.");
                return;
            }

            Class testClass;
            // If it's not a whole-year test, ensure that the class is valid. If not, then it's not a problem.
            if (chkWholeYear.IsChecked.Value)
            {
                testClass = null;
            }
            else
            {
                testClass = getSelectedClass();

                // No class, so return and give an error
                if (testClass == null)
                {
                    App.Message("Validation Error", "If the test is not a whole-year test, you must select a class.");
                    return;
                }
            }

            // Validation complete, submit to database and close form.
            // If we're editing, we just update the current test, otherwise we create a new test.
            Test test;

            if (editing_)
                test = editingTest_;
            else
                test = new Test();

            // Set test variables from form
            test.TestName = txtName.Text;
            test.TestTimeLength = duration;
            test.TestBegin = dtpDate.SelectedDate.Value;
            test.WholeYearTest = chkWholeYear.IsChecked.Value;
            test.Class = testClass;
            test.LastTestOfYear = chkEndOfYear.IsChecked.Value;
            test.TestYear = getSelectedYear();
            test.LastTestOfTerm = (chkEndOfYear.IsChecked.Value ? true : chkEndOfTerm.IsChecked.Value);
            test.TestOpen = true;

            // Submit test if we're not editing
            if (!editing_)
                App.db.Tests.InsertOnSubmit(test);

            // Now, delete all existing questions if we're editing, then insert all.
            // The reason we don't attempt to change questions is purely because it would get very
            // Difficult to keep track of which questions were kept/changed and this is a much simpler approach.
            App.db.TestQuestions.DeleteAllOnSubmit(from testQuestions in App.db.TestQuestions
                                                   where testQuestions.TestID == test.TestID
                                                   select testQuestions);

            List<TestQuestion> questions = new List<TestQuestion>();

            for (int i = 0; i < wordBoxes.Count; i++)
            {
                // Add the new test question
                questions.Add(new TestQuestion()
                {
                    QuestionNumber = i,
                    Question = definitionBoxes[i].Text,
                    Answer = wordBoxes[i].Text,
                    Test = test,
                });
            }

            // Update database with the test
            App.db.TestQuestions.InsertAllOnSubmit(questions);

            App.db.SubmitChanges();

            this.Close();
            App.Message("Success", "Test created successfully.");
        }

        private void chkEndOfYear_Checked(object sender, RoutedEventArgs e)
        {
            // Hide the end of term checkbox (Since the end of the year is always end of term)
            // Value of end of term will be taken as true whenever the value of end of year is true.
            chkEndOfTerm.Visibility = Visibility.Hidden;

            // If it's the end of year, it must also be a whole year test
            chkWholeYear.IsChecked = true;
        }

        private void chkEndOfYear_Unchecked(object sender, RoutedEventArgs e)
        {
            // Make the end of term checkbox visible (See above method for more information)
            chkEndOfTerm.Visibility = Visibility.Visible;
        }

        private void cmbClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check which class has been selected, and force cmbYear to match it
            Class selectedClass = getSelectedClass();

            if (selectedClass != null)
            {
                // We subtract 3 since it (the combobox) is 0-based and years count from 3, so we go from 3-based to 0-based.
                cmbYear.SelectedIndex = selectedClass.ClassYear.Value - 3;
            }
        }

        private void cmbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If they change the year, blank out the class combobox if it's in a different year group.
            if (cmbClass.SelectedIndex != -1)
            {
                Class selectedClass = getSelectedClass();

                if (selectedClass.ClassYear != getSelectedYear())
                {
                    cmbClass.SelectedIndex = -1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Year selected or -1</returns>
        private int getSelectedYear()
        {
            if (cmbYear.SelectedIndex != -1)
            {
                return int.Parse(((ComboBoxItem)cmbYear.SelectedItem).Content.ToString().Substring(5));
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns class selected, or null</returns>
        private Class getSelectedClass()
        {
            if (cmbClass.SelectedIndex != -1)
            {
                return (from classes in App.db.Classes
                        where classes.ClassName == cmbClass.SelectedItem.ToString()
                        select classes).Single();
            }

            return null;
        }


        /// <summary>
        /// Disable and set value to -1 when the whole year is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkWholeYear_Checked(object sender, RoutedEventArgs e)
        {
            cmbClass.SelectedIndex = -1;
            cmbClass.IsEnabled = false;
        }

        /// <summary>
        /// When whole year is unchecked, re-enable the class combobox for them to select values for.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkWholeYear_Unchecked(object sender, RoutedEventArgs e)
        {
            cmbClass.IsEnabled = true;
        }
    }
}
