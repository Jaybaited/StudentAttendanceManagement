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
    public partial class LoginPage : Form
    {
        string connectionString = "server=localhost;uid=root;pwd=;database=db_attendance;";
        bool isPasswordShown = false;

        public LoginPage()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text.Trim();
            string password = textBoxPassword.Text.Trim();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Modify the query to select the role as well
                    string query = "SELECT role FROM users WHERE username = @username AND password = @password";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read(); // Read the first (and only) row
                        string role = reader["role"].ToString(); // Get the role from the database

                        MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide(); // Hide the login form

                        // Check the role and open the appropriate dashboard
                        if (role == "Admin")
                        {
                            AdminDashboard adminDashboard = new AdminDashboard(username, role);
                            adminDashboard.Show();
                        }
                        else if (role == "Teacher") // Assuming the role in the database is "Teacher"
                        {
                            

                            TeacherDashboard teacherDashboard = new TeacherDashboard(username, role);
                            teacherDashboard.Show();
                            this.Hide(); // Hide the login form
                        }
                        else
                        {
                            // Handle other roles or unrecognized roles if necessary
                            MessageBox.Show("Unknown user role. Please contact support.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.Show(); // Show login page again if role is unhandled
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

       
        private void pbClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pbShowPass_Click_1(object sender, EventArgs e)
        {
            if (isPasswordShown)
            {
                textBoxPassword.UseSystemPasswordChar = true;
                isPasswordShown = false;
            }
            else
            {
                textBoxPassword.UseSystemPasswordChar = false;
                isPasswordShown = true;
            }
        }
    }
}
