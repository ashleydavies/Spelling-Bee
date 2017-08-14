using System;
using System.Collections.Generic;
using System.Text;
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
	/// Interaction logic for SysAdminRoot.xaml
	/// </summary>
	public partial class SysAdminRoot
	{
        Staff currentUser;

		public SysAdminRoot(Staff login)
		{
            this.InitializeComponent();

            // Capture & store the logged in user
            currentUser = login;
        }

        // Almost all events in this window are purely to link to other windows

        private void mnuClassAdd_Click(object sender, RoutedEventArgs e)
        {
            // Open the add class window
            AddClassWindow addClass = new AddClassWindow();
            addClass.ShowDialog();
        }

        private void mnuClassView_Click(object sender, RoutedEventArgs e)
        {
            // Open the edit class window
            EditClassWindow editClass = new EditClassWindow();
            editClass.ShowDialog();
        }



        private void mnuStudentAdd_Click(object sender, RoutedEventArgs e)
        {
            // Open the add student window
            AddStudentWindow addStudent = new AddStudentWindow();
            addStudent.ShowDialog();
        }

        private void mnuStudentView_Click(object sender, RoutedEventArgs e)
        {
            // Open the edit student window
            EditStudentWindow editStudent = new EditStudentWindow();
            editStudent.ShowDialog();
        }



        private void mnuStaffAdd_Click(object sender, RoutedEventArgs e)
        {
            // Open the add teacher window
            AddTeacherWindow addTeacher = new AddTeacherWindow();
            addTeacher.ShowDialog();
        }

        private void mnuStaffView_Click(object sender, RoutedEventArgs e)
        {
            // Open the edit staff window
            EditTeacherWindow editStaff = new EditTeacherWindow(currentUser);
            editStaff.ShowDialog();
        }

        private void menuStudentAddToClass_Click(object sender, RoutedEventArgs e)
        {
            // Open the add student to class window
            AddStudentToClassWindow addToClass = new AddStudentToClassWindow();
            addToClass.ShowDialog();
        }

        private void btnOpenTeacherRoot_Click(object sender, RoutedEventArgs e)
        {
            // They want to work as a teacher, so open the teacherroot.
            TeacherRoot tRoot = new TeacherRoot(currentUser);
            tRoot.ShowDialog();
        }
	}
}