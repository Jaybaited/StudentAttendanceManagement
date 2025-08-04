using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms; // Keep this if dtpCalendar is a Guna.UI2.WinForms.Guna2DateTimePicker

namespace StudentAttendanceManagement
{
    public partial class ucMarkAttendance : UserControl
    {
        // Database connection string
        private string connectionString = "server=127.0.0.1;port=3306;database=db_attendance;uid=root;pwd=;";

        // Private field to store the logged-in user's username
        private string _loggedInUsername;

        // Public property to set the current user's username from the main application
        // When this property is set, it triggers the LoadSections method to update the ComboBox.
        public string CurrentLoggedInUser
        {
            get { return _loggedInUsername; }
            set
            {
                _loggedInUsername = value;
                // Prevent execution in design time
                if (this.DesignMode == false)
                {
                    LoadSections(); // <-- This calls the method to load sections
                }
            }
        }

        public ucMarkAttendance()
        {
            InitializeComponent();
            // Initial configuration of DataGridView and Date display
            ConfigureDataGridView();

            // dtpCalendar.MaxDate is commented out to allow selection of future dates if desired.
            // If you want to restrict to today or past, uncomment this line:
            dtpCalendar.MaxDate = DateTime.Today; // Restricts selection to today or past dates.

            // Removed: dtpCalendar.MinDate = new DateTime(2024, 1, 20); // Removed this line to allow selection of any past date.
            // If you want a specific minimum date, you can set it here.

            // Initialize dtpCalendar DateTimePicker to the current date
            dtpCalendar.Value = DateTime.Today;

            // Sections will be loaded once CurrentLoggedInUser is set (either via property or overloaded constructor)
        }

        // Overloaded constructor to allow passing the username directly upon instantiation
        public ucMarkAttendance(string currentUserId) : this()
        {
            this.CurrentLoggedInUser = currentUserId;
        }

        // Method to configure DataGridView properties
        private void ConfigureDataGridView()
        {
            dataGridView1.ReadOnly = true; // Make DataGridView read-only
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Select entire row on cell click
            dataGridView1.AllowUserToAddRows = false; // Prevent users from adding new rows directly in the UI
            dataGridView1.AllowUserToDeleteRows = false; // Prevent users from deleting rows directly in the UI
            dataGridView1.MultiSelect = true; // IMPORTANT: Changed to true to allow multiple row selection

            // Auto-resize columns to fill the available space
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // Set header row font to bold for better readability
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            // Apply alternating row colors for improved visual separation of rows
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
        }

        // Method to display the current date (no longer applicable as dtpCalendar is the DateTimePicker)
        private void DisplayCurrentDate()
        {
            // Removed previous logic as dtpCalendar is the DateTimePicker itself.
            // The DateTimePicker manages its own display and input properties.
        }

        // Method to load sections assigned to the current logged-in user into the cmbSections ComboBox
        private void LoadSections()
        {
            // Add a check to ensure CurrentLoggedInUser is not null or empty
            if (string.IsNullOrWhiteSpace(CurrentLoggedInUser))
            {
                comboBox1.BeginUpdate();
                comboBox1.Items.Clear(); // Clear items if username is not valid
                comboBox1.Text = ""; // Clear selection
                comboBox1.EndUpdate();
                comboBox1.Refresh();
                return; // Exit the method
            }

            try
            {
                // Establish a new connection for this method
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // Open the database connection

                    string query = @"
                        SELECT DISTINCT handled_class
                        FROM users
                        WHERE username = @currentUsername
                        ORDER BY handled_class ASC;";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@currentUsername", CurrentLoggedInUser);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    comboBox1.BeginUpdate();
                    comboBox1.Items.Clear(); // Clears existing items

                    while (reader.Read())
                    {
                        if (reader["handled_class"] != DBNull.Value)
                        {
                            string handledClass = reader["handled_class"].ToString();
                            comboBox1.Items.Add(handledClass); // <-- Adds handled_class to comboBox1
                        }
                    }
                    reader.Close();

                    // Select the first item if any exist, otherwise clear selection
                    if (comboBox1.Items.Count > 0)
                    {
                        comboBox1.SelectedIndex = 0;
                    }
                    else
                    {
                        comboBox1.Text = ""; // Clear selection if no sections are found
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sections: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                comboBox1.EndUpdate();
                comboBox1.Refresh();
                // After loading sections, attempt to load attendance data if a section is selected
                if (comboBox1.SelectedItem != null)
                {
                    LoadAttendanceData();
                }
            }
        }

        // Event handler for the "Load Attendance" button click
        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadAttendanceData(); // Call the method to load attendance data
        }

        // Method to load attendance data based on the selected section and current date
        private void LoadAttendanceData()
        {
            // Check if a section is selected in the ComboBox
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a class/section first.", "Selection Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Exit the method if no section is selected
            }

            string selectedSection = comboBox1.SelectedItem.ToString(); // Get the selected section
            string attendanceDate = dtpCalendar.Value.ToString("yyyy-MM-dd"); // Get the attendance date from the dtpCalendar DateTimePicker

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // Open the database connection

                    // SQL query to ensure an attendance record exists for each student in the selected section for the current date.
                    // If a record does not exist, it inserts one with a default status of 'Absent'.
                    // If a record exists, it updates its 'created_at' timestamp only, without changing the status.
                    // This ensures that when the date or section changes, existing attendance statuses are preserved.
                    string checkAndInsertOrUpdateQuery = @"
                        INSERT INTO attendance (student_id, attendance_date, section, status, created_at)
                        SELECT u.student_id, @attendanceDate, @selectedSection, 'Absent', NOW()
                        FROM users u
                        WHERE u.assigned_section = @selectedSection AND u.role = 'Student'
                        ON DUPLICATE KEY UPDATE created_at = NOW();"; // Removed 'status = Absent' from here

                    using (MySqlCommand cmdCheckInsert = new MySqlCommand(checkAndInsertOrUpdateQuery, connection))
                    {
                        cmdCheckInsert.Parameters.AddWithValue("@attendanceDate", attendanceDate); // Add date parameter
                        cmdCheckInsert.Parameters.AddWithValue("@selectedSection", selectedSection); // Add section parameter
                        cmdCheckInsert.ExecuteNonQuery(); // Execute the insert/check/update query
                    }

                    // SQL query to load all students from the selected section, along with their attendance status for the given date.
                    // A LEFT JOIN is used to include all students even if they don't have a specific attendance record yet.
                    // IFNULL is used to ensure a 'Absent' status is displayed if attendance.status is NULL for any reason.
                    string query = @"
                        SELECT u.student_id,
                               CONCAT(u.first_name, ' ', IFNULL(u.middle_name, ''), ' ', u.last_name) AS FullName,
                               IFNULL(a.status, 'Absent') AS status
                        FROM users u
                        LEFT JOIN attendance a ON u.student_id = a.student_id AND a.attendance_date = @attendanceDate
                        WHERE u.assigned_section = @selectedSection AND u.role = 'Student'
                        ORDER BY FullName ASC;"; // Order results by full name

                    MySqlCommand cmd = new MySqlCommand(query, connection); // Create a command object
                    cmd.Parameters.AddWithValue("@selectedSection", selectedSection); // Add section parameter
                    cmd.Parameters.AddWithValue("@attendanceDate", attendanceDate); // Add date parameter

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd); // Create a data adapter
                    DataTable dt = new DataTable(); // Create a new DataTable
                    da.Fill(dt); // Fill the DataTable with data from the database

                    dataGridView1.DataSource = dt; // Set the DataTable as the DataSource for the DataGridView

                    // Rename DataGridView columns for a more user-friendly display
                    if (dataGridView1.Columns.Contains("student_id"))
                        dataGridView1.Columns["student_id"].HeaderText = "Student ID";
                    if (dataGridView1.Columns.Contains("FullName"))
                        dataGridView1.Columns["FullName"].HeaderText = "Full Name";
                    if (dataGridView1.Columns.Contains("status"))
                        dataGridView1.Columns["status"].HeaderText = "Status";

                    CountAttendanceStatus(); // Update the attendance status counts displayed in labels
                }
            }
            catch (Exception ex)
            {
                // Display an error message if loading attendance data fails
                MessageBox.Show("Error loading attendance data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to count and display attendance status totals in respective labels
        private void CountAttendanceStatus()
        {
            int totalStudents = 0;
            int presentCount = 0;
            int absentCount = 0;
            int lateCount = 0;
            int excusedCount = 0;

            // Check if there are rows in the DataGridView
            if (dataGridView1.Rows.Count > 0)
            {
                totalStudents = dataGridView1.Rows.Count; // Total students is the number of rows
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // Ensure the 'Status' cell is not null before checking its value
                    if (row.Cells["Status"].Value != null)
                    {
                        string status = row.Cells["Status"].Value.ToString(); // Get the status value
                        switch (status) // Increment respective counters based on status
                        {
                            case "Present":
                                presentCount++;
                                break;
                            case "Absent":
                                absentCount++;
                                break;
                            case "Late":
                                lateCount++;
                                break;
                            case "Excused":
                                excusedCount++;
                                break;
                        }
                    }
                }
            }

            // Update the text of the status labels
            lblTotal.Text = totalStudents.ToString();
            lblPresent.Text = presentCount.ToString();
            lblAbsent.Text = absentCount.ToString();
            lblLate.Text = lateCount.ToString();
            lblExcused.Text = excusedCount.ToString();
        }

        // Method to update the status of the selected student in both DataGridView and database
        private void UpdateStudentStatus(string newStatus)
        {
            // Check if any row is selected in the DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // When multiple rows are selected, we need to iterate through them.
                foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
                {
                    string studentId = selectedRow.Cells["student_id"].Value.ToString(); // Get Student ID from the selected row
                    string attendanceDate = dtpCalendar.Value.ToString("yyyy-MM-dd"); // Get the attendance date from the dtpCalendar DateTimePicker
                    string selectedSection = comboBox1.SelectedItem.ToString(); // Get the selected section

                    try
                    {
                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            connection.Open(); // Open the database connection
                            // SQL query to update the status of the attendance record for the specific student, date, and section
                            string query = @"
                                UPDATE attendance
                                SET status = @newStatus
                                WHERE student_id = @studentId
                                    AND attendance_date = @attendanceDate
                                    AND section = @selectedSection;";

                            MySqlCommand cmd = new MySqlCommand(query, connection); // Create a command object
                            cmd.Parameters.AddWithValue("@newStatus", newStatus); // Add new status parameter
                            cmd.Parameters.AddWithValue("@studentId", studentId); // Add student ID parameter
                            cmd.Parameters.AddWithValue("@attendanceDate", attendanceDate); // Add attendance date parameter
                            cmd.Parameters.AddWithValue("@selectedSection", selectedSection); // Add section parameter

                            int rowsAffected = cmd.ExecuteNonQuery(); // Execute the update query and get number of rows affected

                            if (rowsAffected > 0)
                            {
                                // If update is successful, update the DataGridView cell immediately to reflect the change
                                selectedRow.Cells["Status"].Value = newStatus;
                            }
                            else
                            {
                                // Display a message if no record was found or updated (consider if this should be per-row or a single message)
                                // For multiple selection, a single consolidated message might be better for user experience.
                                // MessageBox.Show($"Failed to update status for student {studentId}.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Display an error message if updating status fails (consider if this should be per-row or a single message)
                        MessageBox.Show($"Error updating student status for {studentId}: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                // After all updates, recount statuses
                CountAttendanceStatus(); // Recount and update attendance statuses in labels
            }
            else
            {
                // Prompt the user to select a row if none is selected
                MessageBox.Show("Please select at least one student row to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Event handlers for PictureBox clicks to update student status
        private void pbPresent_Click(object sender, EventArgs e)
        {
            UpdateStudentStatus("Present"); // Update status to "Present"
        }

        private void pbAbsent_Click(object sender, EventArgs e)
        {
            UpdateStudentStatus("Absent"); // Update status to "Absent"
        }

        private void pbLate_Click(object sender, EventArgs e)
        {
            UpdateStudentStatus("Late"); // Update status to "Late"
        }

        private void pbExcused_Click(object sender, EventArgs e)
        {
            UpdateStudentStatus("Excused"); // Update status to "Excused"
        }

        // New refresh button click event handler
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // Reload sections (which in turn triggers LoadAttendanceData if a section is selected)
            LoadSections();
        }

        // Event handler for when the date in dtpCalendar (DateTimePicker) changes
        private void dtpCalendar_ValueChanged(object sender, EventArgs e)
        {
            // Automatically load attendance data for the new date, if a section is selected
            if (comboBox1.SelectedItem != null)
            {
                LoadAttendanceData();
            }
        }

        // --- Existing empty event handlers from your provided code.
        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        // Event handler for when the selected index of the ComboBox changes
        private void cmbSections_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Automatically load attendance data for the newly selected section
            if (comboBox1.SelectedItem != null)
            {
                LoadAttendanceData();
            }
        }
        // --- End of existing empty event handlers
    }
}
