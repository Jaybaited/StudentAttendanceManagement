using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // Required for MySQL database connectivity

namespace StudentAttendanceManagement
{
    public partial class ucAttendanceHistory : UserControl
    {
        // Database connection string for XAMPP (phpMyAdmin) MySQL.
        // Make sure the 'uid' (user ID) and 'password' match your MySQL setup.
        private const string ConnectionString = "server=localhost;database=db_attendance;uid=root;password=;";
        private string _currentUsername; // Stores the username of the currently logged-in user

        public ucAttendanceHistory()
        {
            InitializeComponent();
            // Configure the DataGridView properties to ensure it's read-only and not interactive.
            ConfigureDataGridView();
            // Attach the Load event handler for the UserControl.
            // This ensures initial data loading happens when the control is displayed.
            this.Load += new EventHandler(ucAttendanceHistory_Load);
        }

        /// <summary>
        /// Public property to set the username of the current logged-in user.
        /// This property should be set by the parent form (e.g., your main dashboard form)
        /// after a successful user authentication.
        /// </summary>
        public string CurrentUsername
        {
            get { return _currentUsername; }
            set
            {
                _currentUsername = value;
                // If the username is successfully set, proceed to load the sections
                // handled by this user into the Class/Section ComboBox.
                if (!string.IsNullOrEmpty(_currentUsername))
                {
                    LoadSectionsComboBox();
                }
            }
        }

        /// <summary>
        /// Configures the DataGridView to match the specified format and behavior.
        /// It sets properties to make the grid non-editable, non-clickable,
        /// and defines the columns required for displaying attendance history.
        /// </summary>
        private void ConfigureDataGridView()
        {
            // Set DataGridView properties to make it read-only and prevent user interaction.
            dataGridView1.ReadOnly = true;                 // All cells are read-only.
            dataGridView1.AllowUserToAddRows = false;      // Prevents users from adding new rows.
            dataGridView1.AllowUserToDeleteRows = false;   // Prevents users from deleting rows.
            dataGridView1.AllowUserToResizeRows = false;   // Prevents users from resizing row heights.
            dataGridView1.AllowUserToResizeColumns = false; // Prevents users from reordering columns.
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Selects the entire row when clicked.
            dataGridView1.MultiSelect = false;             // Only one row can be selected at a time.
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize; // Automatically adjusts header height.

            // Apply basic visual styling to the DataGridView for better appearance.
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; // Draws horizontal lines between cells.
            dataGridView1.EnableHeadersVisualStyles = false; // Allows custom styling of column headers.
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray; // Sets header background color.
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;     // Sets header text color.
            dataGridView1.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight; // Background color for selected cells.
            dataGridView1.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText; // Text color for selected cells.

            // Clear any existing columns to ensure the DataGridView is set up fresh.
            dataGridView1.Columns.Clear();

            // Add the required columns based on the 2nd attached image:
            // "Date", "Student ID", "Student Name", "Section", "Status", "Time In", "Marked By".
            dataGridView1.Columns.Add("colDate", "Date");
            dataGridView1.Columns.Add("colStudentID", "Student ID");
            dataGridView1.Columns.Add("colStudentName", "Student Name");
            dataGridView1.Columns.Add("colSection", "Section");
            dataGridView1.Columns.Add("colStatus", "Status");
            dataGridView1.Columns.Add("colTimeIn", "Time In");
            // The "Marked By" column will now be automatically set to the current user.
            dataGridView1.Columns.Add("colMarkedBy", "Marked By");

            // Set column auto-size mode to 'Fill' to distribute column widths evenly,
            // ensuring the DataGridView fills its allocated space.
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        /// <summary>
        /// Event handler for when the UserControl loads.
        /// Initializes the DateTimePicker and provides a warning if the current username is not set.
        /// </summary>
        private void ucAttendanceHistory_Load(object sender, EventArgs e)
        {
            // Set the DateTimePicker to display the current date by default.
            dtpCalendar.Value = DateTime.Today;

            // Check if the CurrentUsername property has been set by the parent form.
            // If not, it means the user's context is missing, which is necessary for loading sections.
            if (string.IsNullOrEmpty(_currentUsername))
            {
                MessageBox.Show("User authentication information is missing. Please ensure the 'CurrentUsername' property is set before loading this control.",
                                "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // If _currentUsername is set, LoadSectionsComboBox() will be called automatically
            // via the CurrentUsername property's setter.
        }

        /// <summary>
        /// Loads the 'handled_class' values for the current logged-in user
        /// into the `comboBoxSection`. This populates the Class/Section dropdown.
        /// </summary>
        private void LoadSectionsComboBox()
        {
            comboBoxSection.Items.Clear(); // Clear any previously loaded sections.
            comboBoxStudent.Items.Clear(); // Also clear students, as section change invalidates student list.
            comboBoxStudent.Text = "";     // Clear any selected text in the student combo box.

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open(); // Open the database connection
                    // Query to retrieve the 'handled_class' for the current user.
                    // This assumes the 'users' table contains the teacher's username and their handled sections.
                    string query = "SELECT handled_class FROM users WHERE username = @username"; // Corrected column name
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@username", _currentUsername); // Parameterize to prevent SQL injection.

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // If a record is found for the current user.
                        {
                            string handledClasses = reader["handled_class"].ToString(); // Corrected column name
                            if (!string.IsNullOrEmpty(handledClasses))
                            {
                                // Split the comma-separated sections (e.g., "Grade 11 - ABM A, Grade 12 - STEM B")
                                // and add each section as a separate item to the ComboBox.
                                string[] sections = handledClasses.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string section in sections)
                                {
                                    comboBoxSection.Items.Add(section.Trim()); // Trim whitespace from section names.
                                }
                                if (comboBoxSection.Items.Count > 0)
                                {
                                    comboBoxSection.SelectedIndex = 0; // Automatically select the first section in the list.
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException mex)
            {
                // Handle specific MySQL database errors.
                MessageBox.Show("Database error loading sections: " + mex.Message + "\n\nEnsure the 'users' table and 'handled_class' column exist.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Updated error message
            }
            catch (Exception ex)
            {
                // Handle any other general exceptions.
                MessageBox.Show("An unexpected error occurred while loading sections: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads students belonging to the specified `section` into the `comboBoxStudent`.
        /// Students are fetched from the 'users' table where their role is 'Student'
        /// and their 'assigned_section' matches the selected class section.
        /// </summary>
        /// <param name="section">The specific section for which to retrieve students.</param>
        private void LoadStudentsComboBox(string section)
        {
            comboBoxStudent.Items.Clear(); // Clear any previously loaded students.
            comboBoxStudent.Text = "";     // Clear any selected text.

            // Add the "All Students" option at the beginning
            comboBoxStudent.Items.Add(new ComboBoxItem("", "All Students"));

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open(); // Open the database connection.
                    // Query to select student ID, first name, and last name from the 'users' table.
                    // Filters for users with 'Student' role whose 'assigned_section' matches the provided section.
                    string query = "SELECT student_id, first_name, last_name, middle_name FROM users WHERE role = 'Student' AND assigned_section = @section ORDER BY first_name, last_name"; // Corrected column name to assigned_section, added middle_name
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@section", section); // Parameterize the section.

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) // Iterate through each student record found.
                        {
                            string studentId = reader["student_id"].ToString();
                            string firstName = reader["first_name"].ToString();
                            string lastName = reader["last_name"].ToString();
                            string middleName = reader["middle_name"] != DBNull.Value ? reader["middle_name"].ToString() : "";
                            string fullName = $"{firstName} {(string.IsNullOrEmpty(middleName) ? "" : middleName + " ")}{lastName}";
                            // Add a new ComboBoxItem object to the ComboBox.
                            // This allows storing the student ID internally while displaying their full name.
                            comboBoxStudent.Items.Add(new ComboBoxItem(studentId, fullName));
                        }
                        // Always set SelectedIndex to 0 to select "All Students" initially.
                        if (comboBoxStudent.Items.Count > 0)
                        {
                            comboBoxStudent.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (MySqlException mex)
            {
                // Handle specific MySQL database errors.
                MessageBox.Show("Database error loading students: " + mex.Message + "\n\nEnsure the 'users' table, 'student_id', 'first_name', 'last_name', 'role', and 'assigned_section' columns exist.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Updated error message
            }
            catch (Exception ex)
            {
                // Handle any other general exceptions.
                MessageBox.Show("An unexpected error occurred while loading students: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// A helper class used to store both the Student ID (as 'Id') and
        /// the Student's full name (as 'Name') within the ComboBox.
        /// The `ToString()` method is overridden to ensure the full name is displayed.
        /// </summary>
        private class ComboBoxItem
        {
            public string Id { get; set; }   // Represents the student's unique ID. Can be empty for "All Students".
            public string Name { get; set; } // Represents the student's full name (for display).

            public ComboBoxItem(string id, string name)
            {
                Id = id;
                Name = name;
            }

            public override string ToString()
            {
                return Name; // This is what will be shown in the ComboBox dropdown.
            }
        }

        /// <summary>
        /// Event handler for the "Load Attendance" button click.
        /// This method simply calls the `LoadAttendanceData` function to refresh the display.
        /// </summary>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadAttendanceData(); // Trigger LoadAttendanceData when "Load Attendance" button is clicked.
        }

        /// <summary>
        /// Event handler for the "Refresh" button click.
        /// Clears all displayed attendance data, resets the summary labels,
        /// and reloads the sections and students based on the current user.
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();       // Clear all rows from the DataGridView.
            ResetAttendanceSummaryLabels();   // Reset the Present, Absent, Late, Excused, and Rate labels.
            comboBoxStudent.Items.Clear();    // Clear all items from the Student ComboBox.
            comboBoxStudent.Text = "";        // Clear any text currently displayed in the Student ComboBox.
            dtpCalendar.Value = DateTime.Today; // Reset the date picker to today's date.

            LoadSectionsComboBox(); // Reload the sections for the current user, which will in turn load students.
        }

        /// <summary>
        /// Event handler for when the selected item in the Class/Section ComboBox changes.
        /// When a new section is selected, it triggers the loading of students
        /// who belong to that specific section into the Student ComboBox.
        /// It also reloads attendance data.
        /// </summary>
        private void comboBoxSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSection.SelectedItem != null)
            {
                string selectedSection = comboBoxSection.SelectedItem.ToString();
                LoadStudentsComboBox(selectedSection); // Load students for the newly selected section.
                // LoadAttendanceData will be called by comboBoxStudent_SelectedIndexChanged after LoadStudentsComboBox populates.
            }
        }

        /// <summary>
        /// Event handler for when the selected item in the Student ComboBox changes.
        /// This now triggers the loading of attendance data for the specific student.
        /// </summary>
        private void comboBoxStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load attendance data automatically when the selected student changes.
            if (comboBoxSection.SelectedItem != null) // Ensure a section is selected before loading data
            {
                LoadAttendanceData();
            }
        }

        /// <summary>
        /// Event handler for when the date in the DateTimePicker changes.
        /// This now triggers the loading of attendance data for the new date.
        /// </summary>
        private void dtpCalendar_ValueChanged(object sender, EventArgs e)
        {
            // Load attendance data automatically when the selected date changes.
            if (comboBoxSection.SelectedItem != null) // Ensure a section is selected before loading data
            {
                LoadAttendanceData();
            }
        }

        /// <summary>
        /// This is the core method for loading attendance data.
        /// It fetches records from the database based on the selected section, student (optional), and date.
        /// It populates the DataGridView and updates the attendance summary labels.
        /// </summary>
        public void LoadAttendanceData() // Changed from private to public
        {
            dataGridView1.Rows.Clear(); // Clear all existing rows from the DataGridView.
            ResetAttendanceSummaryLabels(); // Reset the summary counts to zero before loading new data.

            // Basic validation: Ensure a section is selected before attempting to load data.
            if (comboBoxSection.SelectedItem == null)
            {
                MessageBox.Show("Please select a Class/Section to load attendance data.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Exit the method if no section is selected.
            }

            string selectedSection = comboBoxSection.SelectedItem.ToString();
            string selectedStudentId = null; // Initialize student ID to null.

            // If a student is selected in the ComboBox, retrieve their actual student ID.
            // This is crucial for filtering the DataGridView to a single student.
            if (comboBoxStudent.SelectedItem is ComboBoxItem selectedItem)
            {
                // Only filter by student_id if "All Students" is NOT selected (i.e., Id is not empty)
                if (!string.IsNullOrEmpty(selectedItem.Id))
                {
                    selectedStudentId = selectedItem.Id;
                }
            }

            DateTime selectedDate = dtpCalendar.Value.Date; // Get only the date part (time component is ignored for attendance date).

            // Initialize counters for various attendance statuses.
            int presentCount = 0;
            int absentCount = 0;
            int lateCount = 0;
            int excusedCount = 0;
            int totalRecords = 0; // Total attendance records retrieved for calculations.

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open(); // Establish connection to the database.

                    // Build the SQL query dynamically using StringBuilder for better performance and readability.
                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append("SELECT a.attendance_date, a.student_id, u.first_name, u.last_name, a.section, a.status, a.created_at ");
                    queryBuilder.Append("FROM attendance a ");
                    // Join with the 'users' table to fetch student names based on student_id.
                    queryBuilder.Append("JOIN users u ON a.student_id = u.student_id ");
                    queryBuilder.Append("WHERE a.section = @section ");

                    // Conditional filtering based on student selection
                    if (!string.IsNullOrEmpty(selectedStudentId))
                    {
                        // If a specific student is selected, filter by student ID, and show all dates for them.
                        queryBuilder.Append("AND a.student_id = @student_id ");
                        // Order by date for a single student view
                        queryBuilder.Append("ORDER BY a.attendance_date DESC;");
                    }
                    else
                    {
                        // If "All Students" is selected, filter by date, and show all students for that date.
                        queryBuilder.Append("AND a.attendance_date = @attendance_date ");
                        // Order by student name for a single date view
                        queryBuilder.Append("ORDER BY u.first_name, u.last_name;");
                    }

                    MySqlCommand cmd = new MySqlCommand(queryBuilder.ToString(), connection);
                    // Add parameters to the SQL command to prevent SQL injection and properly handle values.
                    cmd.Parameters.AddWithValue("@section", selectedSection);

                    // Add parameters conditionally based on selectedStudentId
                    if (!string.IsNullOrEmpty(selectedStudentId))
                    {
                        cmd.Parameters.AddWithValue("@student_id", selectedStudentId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@attendance_date", selectedDate);
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) // Loop through each row returned by the query.
                        {
                            // Concatenate first, middle (if exists), and last name for display
                            string studentName = $"{reader["first_name"]} {reader["last_name"]}";
                            // You might need to adjust this if middle_name is present in the select statement
                            // string studentName = $"{reader["first_name"]} {(reader["middle_name"] != DBNull.Value ? reader["middle_name"].ToString() + " " : "")}{reader["last_name"]}";

                            string status = reader["status"].ToString();
                            DateTime createdAt = (DateTime)reader["created_at"];
                            string timeIn = createdAt.ToString("hh:mm tt"); // Format time as "08:30 AM/PM".

                            // Set "Marked By" to the current logged-in username.
                            string markedBy = _currentUsername;

                            // Add the retrieved data as a new row to the DataGridView.
                            dataGridView1.Rows.Add(
                                ((DateTime)reader["attendance_date"]).ToShortDateString(), // Format date to a short string (e.g., 1/15/2024).
                                reader["student_id"].ToString(),
                                studentName,
                                reader["section"].ToString(),
                                status,
                                timeIn,
                                markedBy
                            );

                            // Update the attendance status counters based on the retrieved status.
                            switch (status.ToLower())
                            {
                                case "present":
                                    presentCount++;
                                    break;
                                case "absent":
                                    absentCount++;
                                    break;
                                case "late":
                                    lateCount++;
                                    break;
                                case "excused":
                                    excusedCount++;
                                    break;
                            }
                            totalRecords++; // Increment the total count of records processed.
                        }
                    }
                }
            }
            catch (MySqlException mex)
            {
                // Catch specific MySQL database errors and display a user-friendly message.
                MessageBox.Show("Database error loading attendance data: " + mex.Message + "\n\nVerify database connection, table names, and column names.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Stop execution if a database error occurs.
            }
            catch (Exception ex)
            {
                // Catch any other general exceptions.
                MessageBox.Show("An unexpected error occurred while loading attendance data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Stop execution for other errors.
            }

            // Update the UI labels with the calculated attendance summaries.
            lblPresent.Text = presentCount.ToString();
            lblAbsent.Text = absentCount.ToString();
            lblLate.Text = lateCount.ToString();
            lblExcused.Text = excusedCount.ToString(); // Assuming lblExcused is for total excused.

            // Calculate and display the attendance rate.
            if (totalRecords > 0)
            {
                // Attendance rate is calculated as (Present + Late) / Total Records.
                double attendanceRate = ((double)(presentCount + lateCount)) / totalRecords * 100;
                lblAttendanceRate.Text = $"{attendanceRate:F2}%"; // Format to two decimal places (e.g., "95.50%").
            }
            else
            {
                lblAttendanceRate.Text = "0%"; // If no records, attendance rate is 0%.
            }
        }

        /// <summary>
        /// Resets all the attendance summary labels (Present, Absent, Late, Excused, Attendance Rate)
        /// back to their default "0" or "0%" values.
        /// </summary>
        private void ResetAttendanceSummaryLabels()
        {
            lblPresent.Text = "0";
            lblAbsent.Text = "0";
            lblLate.Text = "0";
            lblExcused.Text = "0";
            lblAttendanceRate.Text = "0%";
        }

        // Original empty event handler, left as is per user instruction.
        private void label7_Click(object sender, EventArgs e)
        {
            // This event handler was empty in the original code and remains so.
        }
    }
}
