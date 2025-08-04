using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // Required for MySQL database connection

namespace StudentAttendanceManagement
{
    public partial class ucTeacherDashboard : UserControl
    {
        // --- Database Connection String ---
        // IMPORTANT: Replace 'localhost', 'root', and '' with your actual XAMPP MySQL server details.
        // 'db_attendance' is your database name.
        private string connectionString = "Server=localhost;Port=3306;Database=db_attendance;Uid=root;Pwd=;";

        // --- Property to hold the current teacher's username ---
        // This username will be used to fetch the 'handled_class' for the current teacher.
        public string TeacherUsername { get; set; }


        public ucTeacherDashboard()
        {
            InitializeComponent();
            // InitializeComponent() is responsible for creating and
            // setting up the labels defined in ucTeacherDashboard.Designer.cs.
            // Manually initializing them here would create new instances,
            // leading to ambiguity errors.
        }

        // Event handler for when the UserControl loads
        private void ucTeacherDashboard_Load(object sender, EventArgs e)
        {
            // IMPORTANT: Call LoadDashboardCounts here.
            // Add a debug message to confirm TeacherUsername is set when the UC loads
            MessageBox.Show($"ucTeacherDashboard_Load: TeacherUsername is '{TeacherUsername}'", "Debug: ucTeacherDashboard Load", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadDashboardCounts();
        }

        // Method to load all dashboard counts from the database
        public void LoadDashboardCounts() // Made public so it can be called externally if data needs refreshing
        {
            try
            {
                // --- Debug: Confirm TeacherUsername at the start of LoadDashboardCounts ---
                MessageBox.Show($"LoadDashboardCounts: TeacherUsername is '{TeacherUsername}'", "Debug: LoadDashboardCounts Start", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // --- End Debug ---

                // Retrieve counts
                int totalStudentsCount = GetTotalStudentsCount();
                int totalClassesCount = GetTotalClassesCount();
                string handledClasses = GetTeacherHandledClass(TeacherUsername); // Get handled classes for the specific teacher username
                int handledStudentsCount = 0;
                int handledSectionsCount = 0;

                if (!string.IsNullOrEmpty(handledClasses))
                {
                    handledStudentsCount = GetHandledStudentsCount(handledClasses);
                    handledSectionsCount = GetHandledSectionsCount(handledClasses);
                }

              


                // Find and update lblTotalStudent
                if (Controls.Find("lblTotalStudent", true).FirstOrDefault() is Label lblTotalStudent)
                {
                    lblTotalStudent.Text = totalStudentsCount.ToString();
                }

                // Find and update lblTotalClass
                if (Controls.Find("lblTotalClass", true).FirstOrDefault() is Label lblTotalClass)
                {
                    lblTotalClass.Text = totalClassesCount.ToString();
                }

                // Find and update lblHandledStudent
                if (Controls.Find("lblHandledStudent", true).FirstOrDefault() is Label lblHandledStudent)
                {
                    lblHandledStudent.Text = handledStudentsCount.ToString();
                }

                // Find and update lblHandledSection
                if (Controls.Find("lblHandledSection", true).FirstOrDefault() is Label lblHandledSection)
                {
                    lblHandledSection.Text = handledSectionsCount.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Error loading dashboard data: " + ex.ToString()); // Log to console for detailed info
            }
        }

        // Method to get the total number of students in the entire system (role = 'Student')
        private int GetTotalStudentsCount()
        {
            int totalStudents = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM users WHERE role = 'Student'";
                MySqlCommand command = new MySqlCommand(query, connection);
                try
                {
                    connection.Open();
                    totalStudents = Convert.ToInt32(command.ExecuteScalar());
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Database error getting total students: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine("MySQL Error (GetTotalStudentsCount): " + ex.ToString());
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return totalStudents;
        }

        // Method to get the total number of classes (sections) from the section_list table
        private int GetTotalClassesCount()
        {
            int totalClasses = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM section_list";
                MySqlCommand command = new MySqlCommand(query, connection);
                try
                {
                    connection.Open();
                    totalClasses = Convert.ToInt32(command.ExecuteScalar());
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Database error getting total classes: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine("MySQL Error (GetTotalClassesCount): " + ex.ToString());
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return totalClasses;
        }

        // Method to get the 'handled_class' (section/s) for a specific teacher's username
        private string GetTeacherHandledClass(string username)
        {
            string handledClasses = string.Empty; // Use 'handledClasses' as it can be multiple sections
            if (string.IsNullOrEmpty(username))
            {
                return handledClasses; // Return empty if username is not set
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT handled_class FROM users WHERE username = @username AND role = 'Teacher'";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        handledClasses = result.ToString();
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Database error getting teacher's handled class: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine("MySQL Error (GetTeacherHandledClass): " + ex.ToString());
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return handledClasses;
        }

        // Method to get the total number of students handled by the teacher
        private int GetHandledStudentsCount(string handledClasses)
        {
            int handledStudents = 0;
            string[] classes = handledClasses.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (classes.Length > 0)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        foreach (string className in classes)
                        {
                            string query = "SELECT COUNT(*) FROM users WHERE role = 'Student' AND assigned_section = @className";
                            MySqlCommand command = new MySqlCommand(query, connection);
                            command.Parameters.AddWithValue("@className", className.Trim());
                            handledStudents += Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Database error getting handled students count: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Console.WriteLine("MySQL Error (GetHandledStudentsCount): " + ex.ToString());
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
            }
            return handledStudents;
        }

        // Method to get the number of unique sections (classes) handled by the teacher
        private int GetHandledSectionsCount(string handledClasses)
        {
            string[] classes = handledClasses.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return classes.Length;
        }
    }
}
