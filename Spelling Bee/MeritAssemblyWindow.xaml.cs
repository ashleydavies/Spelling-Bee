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
    /// Interaction logic for MeritAssemblyWindow.xaml
    /// </summary>
    public partial class MeritAssemblyWindow
    {
        public MeritAssemblyWindow()
        {
            InitializeComponent();

            // Add years, select the first, and apply the algorithm as per defaults
            cmbYears.Items.Add("3");
            cmbYears.Items.Add("4");
            cmbYears.Items.Add("5");
            cmbYears.Items.Add("6");

            cmbYears.SelectedIndex = 0;
            // Setting the combobox index automatically runs the applying code, so we're done in this routine now.
            // We just disable the average button by default (since it's selected by default)
            btnBestAvg.IsEnabled = false;
        }

        enum OrderAlgorithm
        {
            BEST_AVERAGE,
            BEST_IMPROVEMENT,
            BEST_LOWEST
        }

        OrderAlgorithm algorithm_ = OrderAlgorithm.BEST_AVERAGE; // Set this to a default average value

        OrderAlgorithm algorithm
        {
            get
            {
                return algorithm_;
            }
            set
            {
                // Automatically run the applyAlgorithm routine when this variable is changed, to save redundant code.
                algorithm_ = value;
                applyAlgorithm(value);
                // Finally, blur out the button that is selected:

                switch (value)
                {
                    case OrderAlgorithm.BEST_AVERAGE:
                        btnBestAvg.IsEnabled = false;
                        btnBestImprv.IsEnabled = true;
                        btnBestLowest.IsEnabled = true;
                        break;
                    case OrderAlgorithm.BEST_IMPROVEMENT:
                        btnBestAvg.IsEnabled = true;
                        btnBestImprv.IsEnabled = false;
                        btnBestLowest.IsEnabled = true;
                        break;
                    case OrderAlgorithm.BEST_LOWEST:
                        btnBestAvg.IsEnabled = true;
                        btnBestImprv.IsEnabled = true;
                        btnBestLowest.IsEnabled = false;
                        break;
                }
            }
        }

        private void applyAlgorithm(OrderAlgorithm alg)
        {
            // Get the year from the combobox on the fly
            if (cmbYears.SelectedIndex != -1)
            {
                int year = int.Parse(cmbYears.SelectedItem.ToString()); // Parse (as opposed to TryParse) is acceptable in this case since it's pre-defined safe input.

                // Get all students in that year using a simple LINQ query
                List<Student> students = (from studentsdb in App.db.Students
                                         where studentsdb.StudentYear == year
                                         select studentsdb).ToList();

                // Now for the algorithms
                switch (alg)
                {
                    case OrderAlgorithm.BEST_AVERAGE:
                        // Get every student's average first of all. We do this by going through each pupil individually, calculating the average, and then putting them into a list of KVPs.
                        List<KeyValuePair<int, Student>> averages = new List<KeyValuePair<int, Student>>();

                        // Loop students
                        foreach (Student student in students)
                        {
                            // Calculate total & max totals
                            int total = 0;
                            int maxTotal = 0;

                            // Loop their results & add them to the totals.
                            // We ONLY want to check tests from their _CURRENT_ year (not their last year), so we filter that!
                            foreach (TestResult result in student.TestResults.Where(x => x.Test.TestYear == student.StudentYear))
                            {
                                total += result.Score.Value;
                                maxTotal += result.MaxScore;
                            }

                            // If a student only sat tests which were out of ten, and another sat one test out of 100, then the second may have an average 5 times higher while getting a score
                            //  which is much lower proportionately (i.e. 8/10 average is better than 50/100) so we convert it to a fraction of 100 to make it fairer.

                            // First, get the fraction of the maxTotal achieved:
                            // Being careful to avoid div 0 errors.
                            double frTotal;
                            if (maxTotal > 0)
                                frTotal = (double)(total) / (double)(maxTotal);
                            else
                                frTotal = 0;

                            int average = (int)(frTotal * 100);

                            // Insert into the list
                            averages.Add(new KeyValuePair<int, Student>(average, student));
                        }

                        // They are now fully loaded, so sort them:
                        averages.Sort((x, y) => y.Key.CompareTo(x.Key));

                        // And display them!
                        lstStudents.ItemsSource = averages;

                        break;
                    case OrderAlgorithm.BEST_IMPROVEMENT:

                        // The algorithm for most improved will compare the gradient of their improvement based on the first, and last, half of their tests.

                        // To do this, the following steps will be followed:

                        // 1. Get their relevant tests
                        // 2. Order by date
                        // 3. Get a percentage value for each one
                        // 4. Get the average percentage for the first and second halves
                        // 5. If the second percentage is bigger than the first, we take the difference

                        List<KeyValuePair<int, Student>> differences = new List<KeyValuePair<int, Student>>();

                        foreach (Student student in students)
                        {
                            List<int> studentAverages = new List<int>();
                            // Step 1 & Step 2 //
                            // First, filter by the year group they're in. We don't want to conserve bias from the last year.   We then order by the date it began.
                            foreach (TestResult result in student.TestResults.Where(x => x.Test.TestYear == student.StudentYear).OrderBy(x => x.Test.TestBegin))
                            {
                                // Step 3 //
                                // Get the percentage in this test, as usual being careful against division by 0 errors
                                if (result.MaxScore != 0)
                                    studentAverages.Add( result.Percentage );
                            }
                            
                            if (studentAverages.Count() > 1)
                            {
                                // Step 4 //
                                int half = (studentAverages.Count() / 2);
                                List<int> firstHalf = studentAverages.GetRange(0, half);
                                List<int> secondHalf = studentAverages.GetRange(half, studentAverages.Count() - half);

                                int firstHalfAvg = 0;
                                int secondHalfAvg = 0;

                                // Total them to the total percentages (i.e. in ten tests, between 0% and 1000%)
                                firstHalf.ForEach(x => firstHalfAvg += x);
                                secondHalf.ForEach(x => secondHalfAvg += x);

                                // And divide it down to the number of tests, so 0% -> 0% and 1000% -> 100%, and 500% -> 50% for ten tests.
                                firstHalfAvg /= firstHalf.Count;
                                secondHalfAvg /= secondHalf.Count;
                                
                                // Step 5 //
                                if (secondHalfAvg > firstHalfAvg)
                                    differences.Add(new KeyValuePair<int, Student>(secondHalfAvg - firstHalfAvg, student));
                            }
                        }

                        // Finally, sory and show in list
                        differences.Sort((x, y) => y.Key.CompareTo(x.Key));
                        lstStudents.ItemsSource = differences;

                        break;
                    case OrderAlgorithm.BEST_LOWEST:
                        // Get every student's lowest value. Similar to the implementation of average but with less math.
                        List<KeyValuePair<int, Student>> lowests = new List<KeyValuePair<int, Student>>();

                        // Loop students
                        foreach (Student student in students)
                        {
                            // Set it to the biggest number so the first number is lower no matter what (We'll assume there isn't more than 2^30 questions)
                            int lowest = int.MaxValue;

                            // Loop their results & add them to the totals. Once again, we filter the non-this year tests.
                            foreach (TestResult result in student.TestResults.Where(x => x.Test.TestYear == student.StudentYear))
                            {
                                if (result.Score < lowest)
                                    lowest = result.Score.Value;
                            }

                            // Insert into the list
                            // We don't insert if their max was still "int.MaxValue" since that implies they didn't sit any tests (?)
                            if (lowest != int.MaxValue)
                                lowests.Add(new KeyValuePair<int, Student>(lowest, student));
                        }

                        // They are now fully loaded, so sort them:
                        lowests.Sort((x, y) => y.Key.CompareTo(x.Key));

                        // And display them!
                        lstStudents.ItemsSource = lowests;

                        break;
                }
            }
            // Don't throw a message as since there is a selection by default it seems unlikely this can happen, so an error is unnecessary. It should be obvious enough, anyway.
        }

        private void btnBestAvg_Click(object sender, RoutedEventArgs e)
        {
            algorithm = OrderAlgorithm.BEST_AVERAGE;
        }

        private void btnBestImprv_Click(object sender, RoutedEventArgs e)
        {
            algorithm = OrderAlgorithm.BEST_IMPROVEMENT;
        }

        private void btnBestLowest_Click(object sender, RoutedEventArgs e)
        {
            algorithm = OrderAlgorithm.BEST_LOWEST;
        }

        private void cmbYears_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            applyAlgorithm(algorithm);
        }
    }
}
