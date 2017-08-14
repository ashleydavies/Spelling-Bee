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
    /// Interaction logic for DEBUGWINDOW.xaml
    /// </summary>
    public partial class DEBUGWINDOW 
    {
        public DEBUGWINDOW()
        {
            InitializeComponent();

            int num = (from test in App.db.Tests select test.TestID).Count();
            lblNumber.Content = num + " entries";
            datagrid1.DataContext = (from staff in App.db.Staffs
                                     select staff);
            datagrid1_Copy.DataContext = (from student in App.db.Students
                                          select student);
            datagrid1_Copy1.DataContext = (from test in App.db.Tests
                                           select test);
            datagrid1_Copy2.DataContext = (from testQ in App.db.TestQuestions
                                           select testQ);
            datagrid1_Copy3.DataContext = (from class_ in App.db.Classes
                                           select class_);
            datagrid1_Copy4.DataContext = (from year in App.db.StudentClasses
                                           select year);
            datagrid1_Copy5.DataContext = (from result in App.db.TestResults
                                           select result);
            datagrid1_Copy6.DataContext = (from qResult in App.db.TestQuestionResults
                                           select qResult);
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            TestQuestion x = new TestQuestion()
            {
                Answer = CheckWord.Text,
            };

            App.Message("Answer Status", x.getErrorStatus(InWord.Text).ToString());
        }

        private void Check_Copy_Click(object sender, RoutedEventArgs e)
        {
            MeritAssemblyWindow maw = new MeritAssemblyWindow();
            maw.ShowDialog();
        }
    }
}
