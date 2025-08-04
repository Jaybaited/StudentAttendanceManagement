using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace StudentAttendanceManagement
{
    public partial class ucManageStudents : UserControl
    {
        private readonly MySqlConnection connection;

        // ✅ Add this event so others can listen for section updates
        public event EventHandler StudentSectionUpdated;

        public ucManageStudents()
        {
            InitializeComponent();

            string connStr = "server=localhost;uid=root;pwd=;database=db_attendance;";
            connection = new MySqlConnection(connStr);

            dgvStudentList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStudentList.ReadOnly = true;
            dgvStudentList.AllowUserToAddRows = false;
            dgvStudentList.AllowUserToDeleteRows = false;
            dgvStudentList.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvStudentList.BorderStyle = BorderStyle.None;
            dgvStudentList.EnableHeadersVisualStyles = false;
            dgvStudentList.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dgvStudentList.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvStudentList.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            Load += ucManageStudents_Load;
            txtSearch.TextChanged += txtSearch_TextChanged;
            dgvStudentList.CellClick += dgvStudentList_CellClick;
            btnCreate.Click += btnCreate_Click;
            btnEdit.Click += btnEdit_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnCancel.Click += btnCancel_Click;
            // Added subscription for the new delete button
            btnDelete.Click += btnDelete_Click;
        }

        private void ucManageStudents_Load(object sender, EventArgs e)
        {
            LoadStudents();
            LoadSections();
            UpdateActionButtonStates();
        }

        /// <summary>
        /// Opens the database connection. Suppresses error messages here;
        /// calling methods are responsible for showing specific user messages.
        /// </summary>
        /// <returns>True if connection is open, false otherwise.</returns>
        private bool OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                return true;
            }
            catch (Exception)
            {
                // Suppress message box here to avoid multiple pop-ups for connection issues.
                // The calling methods will handle showing a single, relevant error message.
                return false;
            }
        }

        /// <summary>
        /// Closes the database connection. Suppresses error messages here.
        /// </summary>
        /// <returns>True if connection is closed, false otherwise.</returns>
        private bool CloseConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                return true;
            }
            catch (MySqlException)
            {
                // Suppress message box here as connection closing failures are less critical
                // and should not repeatedly interrupt the user.
                return false;
            }
        }

        private void LoadStudents(string search = "")
        {
            if (!OpenConnection())
            {
                MessageBox.Show("Could not connect to the database. Failed to load students.", "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string query = @"SELECT student_id, CONCAT(last_name, ', ', first_name, ' ', middle_name) AS full_name, gender, assigned_section FROM users WHERE role = 'Student'";

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query += " AND (student_id LIKE @search OR first_name LIKE @search OR middle_name LIKE @search OR last_name LIKE @search OR assigned_section LIKE @search)";
                }

                MySqlCommand cmd = new MySqlCommand(query, connection);
                if (!string.IsNullOrWhiteSpace(search))
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvStudentList.DataSource = dt;

                dgvStudentList.Columns["student_id"].HeaderText = "Student ID";
                dgvStudentList.Columns["full_name"].HeaderText = "Full Name";
                dgvStudentList.Columns["gender"].HeaderText = "Gender";
                dgvStudentList.Columns["assigned_section"].HeaderText = "Section";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load students: " + ex.Message, "Data Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { CloseConnection(); }

            UpdateActionButtonStates();
        }

        private void LoadSections()
        {
            if (!OpenConnection())
            {
                MessageBox.Show("Could not connect to the database. Failed to load sections.", "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string query = "SELECT section FROM section_list";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                cmbSection.Items.Clear();
                while (reader.Read())
                {
                    cmbSection.Items.Add(reader.GetString("section"));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load sections: " + ex.Message, "Data Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { CloseConnection(); }
        }

        /// <summary>
        /// Retrieves the current student count and capacity for a given section.
        /// This method now also returns a boolean indicating overall success.
        /// </summary>
        /// <param name="sectionName">The name of the section.</param>
        /// <returns>
        /// A tuple containing (currentStudents, capacity, success).
        /// Returns (0, 0, false) if the section is not found, connection fails, or another error occurs.
        /// Error messages for data retrieval issues are shown here. Connection errors are suppressed.
        /// </returns>
        private (int currentStudents, int capacity, bool success) GetSectionCapacityInfo(string sectionName)
        {
            // OpenConnection() now returns false silently on failure.
            // The caller (btnUpdate_Click) will handle the connection error message for the overall operation.
            if (!OpenConnection())
            {
                return (0, 0, false); // Connection failed, but no message shown here.
            }

            try
            {
                string query = @"
                    SELECT 
                        (SELECT COUNT(*) FROM users WHERE role = 'Student' AND assigned_section = @sectionName) AS Students,
                        capacity
                    FROM section_list
                    WHERE section = @sectionName";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@sectionName", sectionName);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int students = reader["Students"] != DBNull.Value ? Convert.ToInt32(reader["Students"]) : 0;
                        int capacity = reader["capacity"] != DBNull.Value ? Convert.ToInt32(reader["capacity"]) : 0;
                        return (students, capacity, true); // Successfully retrieved info
                    }
                }
            }
            catch (Exception ex)
            {
                // Only show message for SQL execution errors here, not connection errors handled by OpenConnection().
                MessageBox.Show("Error getting section capacity info: " + ex.Message, "Data Retrieval Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }
            return (0, 0, false); // Default if not found or error occurred in try block
        }


        private void UpdateActionButtonStates()
        {
            bool rowSelected = dgvStudentList.SelectedRows.Count > 0;
            btnEdit.Enabled = rowSelected;
            btnUpdate.Enabled = rowSelected;
            // Enable delete button only if a row is selected
            btnDelete.Enabled = rowSelected;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadStudents(txtSearch.Text);
        }

        private void dgvStudentList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            UpdateActionButtonStates();
            if (e.RowIndex >= 0)
            {
                var row = dgvStudentList.Rows[e.RowIndex];
                txtStudent.Text = row.Cells["full_name"].Value.ToString();
                cmbSection.Text = row.Cells["assigned_section"].Value?.ToString();
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            FormAddStudent f = new FormAddStudent();
            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadStudents(txtSearch.Text);
                LoadSections();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvStudentList.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student to edit.");
                return;
            }

            string studentId = dgvStudentList.SelectedRows[0].Cells["student_id"].Value.ToString();
            FormAddStudent f = new FormAddStudent(studentId);
            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadStudents(txtSearch.Text);
                LoadSections();
                ClearSelection();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvStudentList.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student to update.");
                return;
            }

            string studentId = dgvStudentList.SelectedRows[0].Cells["student_id"].Value.ToString();
            string oldSection = dgvStudentList.SelectedRows[0].Cells["assigned_section"].Value?.ToString();
            string newSection = cmbSection.Text;

            // Only perform capacity check if the section is actually changing
            if (oldSection != newSection)
            {
                if (string.IsNullOrWhiteSpace(newSection))
                {
                    // Allow unassigning a student from a section
                    // No capacity check needed as they are leaving a section (or were never in one)
                }
                else
                {
                    // Check capacity for the new section
                    (int currentStudents, int capacity, bool capacityInfoSuccess) = GetSectionCapacityInfo(newSection);

                    // If GetSectionCapacityInfo failed to retrieve info (e.g., connection error or SQL error during retrieval),
                    // it either showed its own message (for SQL error) or returned false silently (for connection error).
                    // In either case, we should stop the update operation as capacity info is unreliable.
                    if (!capacityInfoSuccess)
                    {
                        // A message box for the retrieval error (if not connection) was already shown by GetSectionCapacityInfo.
                        // If it was a connection error, the message will be shown by the main update logic below.
                        return; // Stop the update operation.
                    }
                    // Proceed with capacity check only if information was successfully retrieved.
                    else if (capacity > 0 && currentStudents >= capacity)
                    {
                        MessageBox.Show(
                            $"The selected section '{newSection}' is currently full ({currentStudents}/{capacity} students). " +
                            "A student cannot be assigned to a full section.",
                            "Capacity Full",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return; // Prevent the update due to capacity full
                    }
                }
            }

            // This is the primary connection check for the entire update operation.
            // If connection failed for GetSectionCapacityInfo, this will show the message.
            if (!OpenConnection())
            {
                MessageBox.Show("Could not connect to the database. Failed to update student.", "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string q = "UPDATE users SET assigned_section = @newSection WHERE student_id = @studentId";
                MySqlCommand cmd = new MySqlCommand(q, connection);
                cmd.Parameters.AddWithValue("@newSection", string.IsNullOrWhiteSpace(newSection) ? DBNull.Value : (object)newSection);
                cmd.Parameters.AddWithValue("@studentId", studentId);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Student updated successfully.");

                LoadStudents(txtSearch.Text);
                ClearSelection();

                // ✅ Raise event so ucManageClass can update student count
                StudentSectionUpdated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating student: " + ex.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { CloseConnection(); }
        }

        /// <summary>
        /// Handles the click event for the delete student button.
        /// Deletes the selected student record from the database.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStudentList.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the student ID from the selected row
            string studentIdToDelete = dgvStudentList.SelectedRows[0].Cells["student_id"].Value.ToString();
            string studentName = dgvStudentList.SelectedRows[0].Cells["full_name"].Value.ToString();

            // Confirmation dialog
            DialogResult confirmResult = MessageBox.Show(
                $"Are you sure you want to delete student: {studentName} (ID: {studentIdToDelete})?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult == DialogResult.Yes)
            {
                if (!OpenConnection())
                {
                    MessageBox.Show("Could not connect to the database. Failed to delete student.", "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    string deleteQuery = "DELETE FROM users WHERE student_id = @studentId AND role = 'Student'";
                    MySqlCommand cmd = new MySqlCommand(deleteQuery, connection);
                    cmd.Parameters.AddWithValue("@studentId", studentIdToDelete);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Student '{studentName}' deleted successfully.", "Delete Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadStudents(txtSearch.Text); // Refresh the student list
                        ClearSelection(); // Clear selection and input fields
                        StudentSectionUpdated?.Invoke(this, EventArgs.Empty); // Notify listeners of data change
                    }
                    else
                    {
                        MessageBox.Show("No student found with the selected ID, or student is not a 'Student' role.", "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting student: " + ex.Message, "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    CloseConnection();
                }
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear selection?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                ClearSelection();
        }

        private void ClearSelection()
        {
            txtStudent.Clear();
            cmbSection.SelectedIndex = -1;
            dgvStudentList.ClearSelection();
            UpdateActionButtonStates();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadStudents();
        }

        private void btnManageTeachers_Click(object sender, EventArgs e)
        {
            // Clear current controls and show ucManageTeachers
            this.Controls.Clear();
            ucManageTeachers teachersControl = new ucManageTeachers();
            teachersControl.Dock = DockStyle.Fill;
            this.Controls.Add(teachersControl);
        }

        private void btnManageUsers_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            ucManageAccounts usersControl = new ucManageAccounts();
            usersControl.Dock = DockStyle.Fill;
            this.Controls.Add(usersControl);
        }

        private void btnManageStudents_Click(object sender, EventArgs e)
        {
            // Clear current controls and show ucManageStudents
            this.Controls.Clear();
            ucManageStudents studentsControl = new ucManageStudents();
            studentsControl.Dock = DockStyle.Fill;
            this.Controls.Add(studentsControl);
        }
    }
}
