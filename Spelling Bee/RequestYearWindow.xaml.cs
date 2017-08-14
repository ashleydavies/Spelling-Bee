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
    public partial class RequestYearWindow 
    {
        public int year;

        public RequestYearWindow(string title)
        {
            InitializeComponent();

            // Set form information
            this.Title = title;
            lblTitle.Content = title;
        }

        /// <summary>
        /// Handles the submission of text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // If they selected a year
            if (cmbYear.SelectedIndex != -1)
            {
                // Parse the year (They are in a combobox, so input is safe.
                year = int.Parse(((ComboBoxItem)cmbYear.SelectedItem).Content.ToString().Substring(5));
                // And close the window
                this.Close();
            }
            else
            {
                // Otherwise, give an error
                App.Message("Validation Error", "Please select a year.");
            }
        }
    }
}
