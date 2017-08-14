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
    public partial class SelectClassWindow 
    {
        public bool selected = false;
        public Class selectedClass;

        public SelectClassWindow()
        {
            InitializeComponent();

            // Set the list view to display all of the classes
            lstClasses.ItemsSource = from classes in App.db.Classes
                                     select classes;
        }

        private void lstClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If they select one
            if (lstClasses.SelectedIndex != -1)
            {
                // Enable the select button
                btnSelect.IsEnabled = true;
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // If one is selected
            if (lstClasses.SelectedIndex != -1)
            {
                // Set selected to true and the selectedClass to the relevant class
                selected = true;
                selectedClass = (Class)lstClasses.SelectedItem;

                // Successfully chosen class, so close form
                Close();
            }
            else
            {
                // Notify them to select one
                App.Message("Validation Error", "Please select a class");
            }
        }
    }
}
