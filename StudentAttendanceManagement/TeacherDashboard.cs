using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace StudentAttendanceManagement
{
    public partial class TeacherDashboard : Form
    {
        private Timer timer; // Added for live clock
        private Panel panelIndicator = new Panel(); // Panel for selection indicator

        // Add private fields to store the username and role
        private string _loggedInUsername;
        private string _loggedInRole;

        // Declare ucMarkAttendance as a class-level field
        private ucMarkAttendance _markAttendanceControl;

        // Declare ucTeacherDashboard as a class-level field so we can manage its instance
        private ucTeacherDashboard _teacherDashboardControl;

        // Declare ucAttendanceHistory as a class-level field
        private ucAttendanceHistory _attendanceHistoryControl; // <--- Declared here


        // Modified constructor to accept username and role
        public TeacherDashboard(string username, string role)
        {
            InitializeComponent();

            // Store the passed username and role
            _loggedInUsername = username;
            _loggedInRole = role;

            // Set the lblUser and lblRole text with the logged-in user's information
            lblUsername.Text = _loggedInUsername;
            lblRole.Text = _loggedInRole;

            // Initialize ucMarkAttendance ONCE in the constructor
            // Pass the logged-in username to its constructor
            _markAttendanceControl = new ucMarkAttendance(_loggedInUsername);
            _markAttendanceControl.Dock = DockStyle.Fill; // Make it fill the container

            // Initialize ucTeacherDashboard ONCE in the constructor
            _teacherDashboardControl = new ucTeacherDashboard();
            _teacherDashboardControl.TeacherUsername = _loggedInUsername; // <--- Set the TeacherUsername here!
            _teacherDashboardControl.Dock = DockStyle.Fill;

            // Initialize ucAttendanceHistory ONCE in the constructor
            _attendanceHistoryControl = new ucAttendanceHistory(); // <--- Initialized here
            _attendanceHistoryControl.CurrentUsername = _loggedInUsername; // <--- Pass the logged-in username
            _attendanceHistoryControl.Dock = DockStyle.Fill; // Make it fill the container

            // Add _teacherDashboardControl to your main content panel initially if the dashboard is the first view.
            // If MarkAttendance is the first view, add _markAttendanceControl instead.
            // For this example, let's assume the dashboard is the default view:
            this.panelDashboard.Controls.Add(_teacherDashboardControl);
            _teacherDashboardControl.BringToFront(); // Ensure it's on top if added directly

            lblDate.Text = DateTime.Now.ToString("MMMM dd,yyyy hh:mm:ss tt"); // Corrected format for clarity.

            // Initialize and start timer
            timer = new Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += Timer_Tick;
            timer.Start();

            // Panel indicator setup
            panelIndicator.Size = new Size(5, 45); // Adjust to match button height
            panelIndicator.BackColor = Color.White; // Customize color
            panelIndicator.Visible = false; // Hidden until button is clicked

            // Replace 'panelMenu' with the actual name of your sidebar panel
            panelSidebar.Controls.Add(panelIndicator);

            cmbLogout.Items.Add("Logout");
            cmbLogout.Items.Add("Exit");
            cmbLogout.DropDownStyle = ComboBoxStyle.DropDownList; // Make it non-editable
        }

        // Timer Tick: Update lblDate to current time
        private void Timer_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToString("MMMM dd,yyyy hh:mm:ss tt");
        }

        // Helper method to load UserControls into panelMain
        // This method will now ensure the existing _markAttendanceControl is used
        private void LoadUserControl(UserControl uc)
        {
            panelDashboard.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            panelDashboard.Controls.Add(uc);
            uc.BringToFront();
        }

        private void MoveIndicator(Button selectedButton)
        {
            panelIndicator.Top = selectedButton.Top;
            panelIndicator.Height = selectedButton.Height;
            panelIndicator.Visible = true;
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            // This event handler is now obsolete or might be a duplicate,
            // the main one is btnDashboard_Click_1
        }

        private void btnMarkAttendance_Click(object sender, EventArgs e)
        {
            MoveIndicator(btnMarkAttendance);
            // Now, instead of creating a new instance, use the existing _markAttendanceControl
            LoadUserControl(_markAttendanceControl);
            // You might want to explicitly call the refresh on this instance if needed after it's shown
            // For example: _markAttendanceControl.LoadSections(); if not automatically triggered by visibility/activation
        }

        private void btnViewAttendance_Click(object sender, EventArgs e)
        {
            
        }

        private void btnViewReport_Click(object sender, EventArgs e)
        {
            
        }

        private void pbClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnManageClasses_Click(object sender, EventArgs e)
        {
            MoveIndicator(btnAttendanceHistory); // Corrected to use btnAttendanceHistory for indicator
            // LoadUserControl(new ucManageClass(_loggedInUsername)); // Example: pass username if needed
            LoadUserControl(new ucManageClass());
        }

        private void panelSidebar_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblRole_Click(object sender, EventArgs e)
        {

        }

        private void lblUsername_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void lblDate_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pbProfileLogout_Click(object sender, EventArgs e)
        {
            cmbLogout.DroppedDown = true;

        }

        private void cmbLogout_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLogout.SelectedItem != null)
            {
                string selectedOption = cmbLogout.SelectedItem.ToString();

                if (selectedOption == "Logout")
                {
                    // Add confirmation message for logout
                    DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        // Assuming you have a Login form named 'LoginPage'
                        LoginPage loginForm = new LoginPage();
                        loginForm.Show(); // Show the login form
                        this.Hide();       // Hide the current TeacherDashboard form
                    }
                    else
                    {
                        cmbLogout.SelectedIndex = -1; // Deselect the item if user cancels
                    }
                }
                else if (selectedOption == "Exit")
                {
                    // Add confirmation message for exit
                    DialogResult result = MessageBox.Show("Are you sure you want to exit the application?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        Application.Exit(); // Terminate the application
                    }
                    else
                    {
                        cmbLogout.SelectedIndex = -1; // Deselect the item if user cancels
                    }
                }
            }
        }

        // MySQL Database connection helper class
        public class Database
        {
            private static string connectionString = "server=localhost;user id=root;password=;database=db_attendance;";
            public static MySqlConnection GetConnection()
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                return conn;
            }
        }

        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            MoveIndicator(btnDashboard);
            // Use the pre-initialized _teacherDashboardControl instance
            LoadUserControl(_teacherDashboardControl);
            // Explicitly call LoadDashboardCounts to refresh the data when the dashboard button is clicked
            _teacherDashboardControl.LoadDashboardCounts();
        }

        private void btnAttendanceHistory_Click(object sender, EventArgs e)
        {
            MoveIndicator(btnAttendanceHistory);
            // Use the pre-initialized _attendanceHistoryControl instance
            LoadUserControl(_attendanceHistoryControl);
            // Explicitly call LoadAttendanceData to refresh the data when the attendance history button is clicked
            _attendanceHistoryControl.LoadAttendanceData();
        }
    }
}
