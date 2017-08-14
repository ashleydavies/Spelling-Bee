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
    public partial class RequestBoolWindow 
    {
        public bool answer;

        public RequestBoolWindow(string title, string content)
        {
            InitializeComponent();

            // Set form information
            this.Title = title;
            lblTitle.Content = title;
            txtContent.Text = content;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            // If they clicked yes, set the answer to true (1, yes)
            answer = true;
            // And close
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            // If false, set the answer to false (0, no)
            answer = false;
            // Close form
            this.Close();
        }
    }
}
