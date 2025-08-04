using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // Add this using statement for MySQL

namespace StudentAttendanceManagement
{
    public partial class ucAdminDashboard : UserControl
    {
        // Database connection string (Update with your actual connection details)
        private string connectionString = "Server=127.0.0.1;Port=3306;Database=db_attendance;Uid=root;Pwd=;";

        public ucAdminDashboard()
        {
            InitializeComponent();
            LoadDashboardCounts(); // Call this method when the dashboard loads
        }

        private void label4_Click(object sender, EventArgs e)
        {
            // This is your existing click event, no changes needed here unless specified
        }

        private void LoadDashboardCounts()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Count Classes/Sections
                    string queryClassesSections = "SELECT COUNT(*) FROM section_list";
                    using (MySqlCommand cmd = new MySqlCommand(queryClassesSections, connection))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        // Assuming label for Classes/Sections is named lblClass
                        if (Controls.Find("lblClass", true).FirstOrDefault() is Label lblClass)
                        {
                            lblClass.Text = count.ToString();
                        }
                    }

                    // Count Roles (Assuming 'Roles' refers to distinct roles in the users table, or a 'roles' table if you have one)
                    // If you have a separate 'roles' table, you would query that.
                    // For now, let's assume it refers to distinct roles in the 'users' table.
                    string queryRoles = "SELECT COUNT(DISTINCT role) FROM users";
                    using (MySqlCommand cmd = new MySqlCommand(queryRoles, connection))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        // Assuming label for Roles is named lblRole
                        if (Controls.Find("lblRole", true).FirstOrDefault() is Label lblRole)
                        {
                            lblRole.Text = count.ToString();
                        }
                    }

                    // Count Tracks/Strands
                    // Assuming Tracks/Strands are distinct values in the 'track' column of 'section_list' table
                    string queryTracksStrands = "SELECT COUNT(DISTINCT track) FROM section_list";
                    using (MySqlCommand cmd = new MySqlCommand(queryTracksStrands, connection))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        // Assuming label for Tracks/Strands is named lblTrack
                        if (Controls.Find("lblTrack", true).FirstOrDefault() is Label lblTrack)
                        {
                            lblTrack.Text = count.ToString();
                        }
                    }

                    // Count Admins
                    string queryAdmins = "SELECT COUNT(*) FROM users WHERE role = 'Admin'";
                    using (MySqlCommand cmd = new MySqlCommand(queryAdmins, connection))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        // Assuming label for Admins is named lblAdmin
                        if (Controls.Find("lblAdmin", true).FirstOrDefault() is Label lblAdmin)
                        {
                            lblAdmin.Text = count.ToString();
                        }
                    }

                    // Count Teachers
                    string queryTeachers = "SELECT COUNT(*) FROM users WHERE role = 'Teacher'";
                    using (MySqlCommand cmd = new MySqlCommand(queryTeachers, connection))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        // Assuming label for Teachers is named lblTeacher
                        if (Controls.Find("lblTeacher", true).FirstOrDefault() is Label lblTeacher)
                        {
                            lblTeacher.Text = count.ToString();
                        }
                    }

                    // Count Students
                    string queryStudents = "SELECT COUNT(*) FROM users WHERE role = 'Student'";
                    using (MySqlCommand cmd = new MySqlCommand(queryStudents, connection))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        // Assuming label for Students is named lblStudent
                        if (Controls.Find("lblStudent", true).FirstOrDefault() is Label lblStudent)
                        {
                            lblStudent.Text = count.ToString();
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Handle MySQL specific exceptions
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("MySQL Error: " + ex.ToString()); // Log full error for debugging
            }
            catch (Exception ex)
            {
                // Handle other general exceptions
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("General Error: " + ex.ToString()); // Log full error for debugging
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
