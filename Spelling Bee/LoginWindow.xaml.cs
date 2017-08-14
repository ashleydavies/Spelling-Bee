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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Spelling_Bee
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // If there is no staff account, add admin account.
            if ((from staff in App.db.Staffs where staff.IsAdministrative == true select staff).Count() == 0)
            {
                // Create the account object
                Staff admin = new Staff()
                {
                    IsAdministrative = true,
                    StaffPrefix = "TEMP",
                    StaffForename = "Admin",
                    StaffSurname = "admin",
                    StaffPassword = "adm1n"
                };
                // Add it to the database
                App.db.Staffs.InsertOnSubmit(admin);
                App.db.SubmitChanges();
            }
        }

        private void badLogin()
        {
            // Make the bad login label visible
            lblBadLogin.Visibility = Visibility.Visible;

            // Create a simple animation for transition
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                BeginTime = TimeSpan.FromSeconds(3),
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                FillBehavior = FillBehavior.Stop
            };

            // Create a storyboard to host our animation
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            
            // Set it's target to the bad login label and make it change opacity.
            Storyboard.SetTarget(animation, lblBadLogin);
            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));

            // Hide it when the animation is completed
            storyboard.Completed += delegate { lblBadLogin.Visibility = Visibility.Hidden;  };
            // Start animation
            storyboard.Begin();

            return;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // TODO: REMOVE AFTER PROTOTYPING ----- DEBUG PURPOSES ONLY -------- \\
                        if (txtUsername.Text == "DEBUG")
                        {
                            DEBUGWINDOW dbw = new DEBUGWINDOW();
                            dbw.Show();
                        }

            // Get login details from form
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            // Clear password field to stop people carelessly leaving it on the login form with the password typed later
            txtPassword.Clear();

            bool isStaff = chkStaff.IsChecked.Value;

            // Depending on if they are trying to login as staff, run attemptLogin_Staff or attemptLogin_Pupil methods.
            if (isStaff)
            {
                if (!attemptLogin_Staff(username, password))
                    badLogin();
            }
            else
            {
                if (!attemptLogin_Student(username, password))
                    badLogin();
            }
        }

        private bool attemptLogin_Student(string username, string password)
        {
            // Check if any students in the database match the provided parameters
            List<Student> login = (from student in App.db.Students
                                   where student.StudentID.ToString() == username
                                    && student.StudentPassword == password
                                   select student).ToList();

            // If there aren't any, cancel
            if (login.Count() == 0)
                return false;

            // If there are, open a studentroot
            StudentRoot sr = new StudentRoot(login.First());
            this.Hide();
            sr.ShowDialog();
            this.Show();

            // Notify the login routine that it was successful
            return true;
        }

        private bool attemptLogin_Staff(string username, string password)
        {
            // Check if there's any staff that meet the parameters provided
            List<Staff> login = (from staff in App.db.Staffs
                                 where staff.StaffID.ToString() == username
                                    && staff.StaffPassword == password
                                 select staff).ToList();

            // If none, return false (cancel)
            if (login.Count() == 0)
                return false;

            // If there are, grab the first (Should be the only, but just to be safe we use First)
            Staff loginStaff = login.First();

            // Now we check if they're an admin or not and open either the sysadminroot or the teacherroot depending on whether they are admin or not respectively.
            if (loginStaff.IsAdministrative.Value)
            {
                SysAdminRoot sar = new SysAdminRoot(loginStaff);
                this.Hide();
                sar.ShowDialog();
                this.Show();
            }
            else
            {
                TeacherRoot tr = new TeacherRoot(loginStaff);
                this.Hide();
                //tr.ShowDialog();
                this.Show();
            }

            // Notify that login was successful after finished
            return true;
        }
    }
}
