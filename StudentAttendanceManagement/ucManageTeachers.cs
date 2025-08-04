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
    public partial class ucManageTeachers : UserControl
    {
        string connectionString = "server=localhost;uid=root;pwd=;database=db_attendance;";

        public ucManageTeachers()
        {
            InitializeComponent();
            this.Load += ucManageTeachers_Load;
            dataGridView1.CellClick += dataGridView1_CellClick;
            btnUpdate.Click += btnUpdate_Click;
            btnCancel.Click += btnCancel_Click;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // Add the event handler for the refresh button
            btn_Refresh.Click += btn_Refresh_Click;
        }

        private void btnManageStudents_Click(object sender, EventArgs e)
        {
            // Clear current controls and show ucManageStudents
            this.Controls.Clear();
            ucManageStudents studentsControl = new ucManageStudents();
            studentsControl.Dock = DockStyle.Fill;
            this.Controls.Add(studentsControl);
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

        private void ucManageTeachers_Load(object sender, EventArgs e)
        {
            LoadTeachers();
            // Call the method to load classes into the ComboBox when the control loads
            LoadClassesIntoComboBox();
        }

        private void LoadTeachers(string search = "")
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT id, role, username,
                                    CONCAT_WS(' ', first_name, middle_name, last_name) AS full_name,
                                    gender, assigned_subject, handled_class
                                FROM users
                                WHERE role = 'Teacher' AND
                                    (username LIKE @search OR
                                     first_name LIKE @search OR
                                     middle_name LIKE @search OR
                                     last_name LIKE @search OR
                                     assigned_subject LIKE @search OR
                                     handled_class LIKE @search)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@search", $"%{search}%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    dataGridView1.DataSource = table;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.MultiSelect = false;
                    dataGridView1.ClearSelection();
                }
            }
        }

        /// <summary>
        /// Loads all available sections from the 'section_list' table into the cmbClass ComboBox.
        /// </summary>
        private void LoadClassesIntoComboBox()
        {
            cmbClass.Items.Clear(); // Clear existing items before loading new ones

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Select distinct section names from the section_list table
                    string query = "SELECT DISTINCT section FROM section_list ORDER BY section ASC";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Add each section name to the ComboBox
                                cmbClass.Items.Add(reader["section"].ToString());
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error loading classes: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                // This line sets the txtTeacher textbox to the full name from the selected row.
                txtTeacher.Text = row.Cells["full_name"].Value.ToString();

                cmbClass.Text = row.Cells["handled_class"].Value.ToString();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to update this teacher's information?", "Confirm Update", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    int userId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

                    string classHandled = cmbClass.Text;

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        // Removed 'assigned_subject = @subject' as there's no input for it.
                        string query = "UPDATE users SET handled_class = @class WHERE id = @id";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@class", classHandled);
                            cmd.Parameters.AddWithValue("@id", userId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Teacher's info updated successfully.");
                    LoadTeachers();
                    ClearSelection();
                }
            }
            else
            {
                MessageBox.Show("Please select a teacher to update.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to clear the selection?", "Confirm Clear", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                ClearSelection();
            }
        }

        private void ClearSelection()
        {
            txtTeacher.Clear();

            cmbClass.SelectedIndex = -1;
            dataGridView1.ClearSelection();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadTeachers(txtSearch.Text);
        }

        // Event handler for the refresh button
        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            LoadTeachers(); // Reload data into the DataGridView
            ClearSelection(); // Clear the textboxes and selection
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a teacher to edit.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get selected row
            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

            // Ensure column "id" exists and is used as the unique key
            if (!dataGridView1.Columns.Contains("id"))
            {
                MessageBox.Show("ID column not found in DataGridView.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            int selectedTeacherId = Convert.ToInt32(selectedRow.Cells["id"].Value);

            string connectionString = "Server=localhost;Port=3306;Database=db_attendance;Uid=root;Pwd=;";

            string firstName = "", middleName = "", lastName = "", username = "", password = "";

            // Fetch the full teacher data from the DB
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT first_name, middle_name, last_name, username, password FROM users WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedTeacherId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                firstName = reader["first_name"].ToString();
                                middleName = reader["middle_name"].ToString();
                                lastName = reader["last_name"].ToString();
                                username = reader["username"].ToString();
                                password = reader["password"].ToString(); // Plain text for now (hash in production)
                            }
                            else
                            {
                                MessageBox.Show("Selected teacher not found in the database.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error fetching teacher data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Open FormAddUser with fetched data
            FormAddUser editForm = new FormAddUser();
            editForm.TeacherId = selectedTeacherId;
            editForm.LoadTeacherData(firstName, middleName, lastName, username, password);

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadTeachers(); // Refresh DataGridView
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete the selected teacher?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Get the ID of the selected teacher
                    int teacherIdToDelete = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            string query = "DELETE FROM users WHERE id = @id AND role = 'Teacher'";
                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@id", teacherIdToDelete);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Teacher deleted successfully.", "Deletion Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadTeachers(); // Refresh the DataGridView
                                    ClearSelection(); // Clear textboxes and selection
                                }
                                else
                                {
                                    MessageBox.Show("Teacher not found or could not be deleted.", "Deletion Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("Error deleting teacher: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a teacher to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
