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
    public partial class RequestStringWindow 
    {
        public string str;

        public RequestStringWindow(string title)
        {
            InitializeComponent();

            // Set form information
            this.Title = title;
            lblTitle.Content = title;
            txtInput.Focus();
        }

        /// <summary>
        /// Handles the submission of text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            str = txtInput.Text;

            this.Close();
        }
    }
}
