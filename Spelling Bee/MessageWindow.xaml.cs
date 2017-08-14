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
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow 
    {
        public MessageWindow(string title, string content, bool sound)
        {
            // Play the sound that message boxes play in WinForms. Allows for a more familiar interface to the users.
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            InitializeComponent();

            // Set the contents
            this.Title = title;
            lblTitle.Content = title;
            txtContent.Text = content;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
