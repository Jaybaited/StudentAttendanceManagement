using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // Ensure this is present for MySQL connectivity

namespace StudentAttendanceManagement
{
    public partial class AdminDashboard : Form
    {
        private Timer timer; // Added for live clock
        private Panel panelIndicator = new Panel(); // Panel for selection indicator

        private string _loggedInUsername;
        private string _loggedInRole;
        public AdminDashboard(string username, string role)
        {
            InitializeComponent();

            // Set up lblDate for live date and time display
            lblDate.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss tt");

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

            // Populate cmbLogout with options
            cmbLogout.Items.Add("Logout");
            cmbLogout.Items.Add("Exit");
            cmbLogout.DropDownStyle = ComboBoxStyle.DropDownList; // Make it non-editable

            // Store the passed username and role
            _loggedInUsername = username;
            _loggedInRole = role;

            // Set the lblUser and lblRole text with the logged-in user's information
            lblUsername.Text = _loggedInUsername;
            lblRole.Text = _loggedInRole;
        }

        // Timer Tick: Update lblDate to current time
        private void Timer_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss tt");
        }

        // Helper method to load UserControls into panelMain
        private void LoadUserControl(UserControl uc)
        {
            panelMain.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            panelMain.Controls.Add(uc);
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
            MoveIndicator(btnDashboard);
            LoadUserControl(new ucAdminDashboard());
        }

        private void btnManageUser_Click(object sender, EventArgs e)
        {
            MoveIndicator(btnManageUser);
            LoadUserControl(new ucManageAccounts());
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
            // This event handler is currently empty.
        }

        private void btnManageClasses_Click(object sender, EventArgs e)
        {
            MoveIndicator(btnManageClasses);
            LoadUserControl(new ucManageClass());
        }

        private void panelSidebar_Paint(object sender, PaintEventArgs e)
        {
            // This event handler is currently empty.
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // This event handler is currently empty.
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // This event handler is currently empty.
        }

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {
            // This event handler is currently empty.
        }

        private void lblRole_Click(object sender, EventArgs e)
        {
            // This event handler is currently empty.
        }

        private void lblUsername_Click(object sender, EventArgs e)
        {
            // This event handler is currently empty.
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // This event handler is currently empty.
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // This event handler is currently empty.
        }

        private void lblDate_Click(object sender, EventArgs e)
        {
            // This event handler is currently empty.
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // This event handler is currently empty.
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // This event handler is currently empty.
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // This event handler is currently empty.
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the cmbLogout ComboBox.
        /// Performs actions based on the selected item ("Logout" or "Exit").
        /// </summary>
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
                        // Assuming you have a Login form named 'LoginForm'
                        LoginPage loginForm = new LoginPage();
                        loginForm.Show(); // Show the login form
                        this.Hide();     // Hide the current AdminDashboard form
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

        /// <summary>
        /// Handles the Click event of the pbProfileLogout PictureBox.
        /// Activates the cmbLogout ComboBox to show its dropdown list.
        /// </summary>
        private void pbProfileLogout_Click(object sender, EventArgs e)
        {
            // This will programmatically open the dropdown list of the ComboBox.
            cmbLogout.DroppedDown = true;
        }
    }

    // MySQL Database connection helper class
    public class DatabaseHelper
    {
        // This connection string is correctly configured for XAMPP's default MySQL setup.
        // It connects to 'localhost', 'root' user with no password, and 'db_attendance' database.
        private static string connectionString = "server=localhost;user id=root;password=;database=db_attendance;";

        /// <summary>
        /// Gets a new MySqlConnection object.
        /// It is crucial to open and close this connection for each database operation.
        /// </summary>
        /// <returns>A new MySqlConnection instance.</returns>
        public static MySqlConnection GetConnection()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        /// <summary>
        /// Example method to test database connection.
        /// You can call this from your AdminDashboard constructor or a button click for testing.
        /// </summary>
        public static void TestConnection()
        {
            using (MySqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("Database connection successful!", "Connection Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Database connection failed: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
