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
    public partial class ucManageClass : UserControl
    {
        private MySqlConnection connection;

        public ucManageClass()
        {
            InitializeComponent();

            string server = "localhost";
            string database = "db_attendance";
            string uid = "root";
            string password = "";

            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
            connection = new MySqlConnection(connectionString);

            // Setup UI look and feel
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
        }

        private void ucManageClass_Load(object sender, EventArgs e)
        {

        }

        private bool OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

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
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void LoadClasses(string searchTerm = "", string sortBy = "")
        {
            if (OpenConnection())
            {
                try
                {
                    string query = @"
                    SELECT 
                        s.id,
                        s.grade_level,
                        s.track,
                        s.section,
                        (
                            SELECT COUNT(*) 
                            FROM users u 
                            WHERE u.role = 'Student' AND u.assigned_section = s.section
                        ) AS Students,
                        s.capacity,
                        s.room_assignment
                    FROM section_list s";

                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        query += " WHERE s.grade_level LIKE @searchTerm OR s.track LIKE @searchTerm OR s.section LIKE @searchTerm OR s.room_assignment LIKE @searchTerm";
                        parameters.Add(new MySqlParameter("@searchTerm", "%" + searchTerm + "%"));
                    }

                    switch (sortBy)
                    {
                        case "Grade Level (Alphabetical)": query += " ORDER BY s.grade_level ASC"; break;
                        case "Section": query += " ORDER BY s.section ASC"; break;
                        case "Grade Level (Asc)": query += " ORDER BY s.grade_level + 0 ASC"; break;
                        case "Grade Level (Desc)": query += " ORDER BY s.grade_level + 0 DESC"; break;
                        case "Capacity (Asc)": query += " ORDER BY s.capacity ASC"; break;
                        case "Capacity (Desc)": query += " ORDER BY s.capacity DESC"; break;
                        case "Students (Asc)": query += " ORDER BY Students ASC"; break;
                        case "Students (Desc)": query += " ORDER BY Students DESC"; break;
                        default: query += " ORDER BY s.id ASC"; break;
                    }

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddRange(parameters.ToArray());
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dt.Columns.Add("Status", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        int students = row["Students"] != DBNull.Value ? Convert.ToInt32(row["Students"]) : 0;
                        int capacity = row["capacity"] != DBNull.Value ? Convert.ToInt32(row["capacity"]) : 0;

                        if (capacity <= 0) row["Status"] = "N/A";
                        else if (students >= capacity) row["Status"] = "Full";
                        else if ((double)students >= capacity * 0.8) row["Status"] = "Almost Full";
                        else row["Status"] = "Available";
                    }

                    dataGridView1.DataSource = dt;

                    dataGridView1.Columns["id"].Visible = false;
                    dataGridView1.Columns["grade_level"].HeaderText = "Grade Level";
                    dataGridView1.Columns["track"].HeaderText = "Track";
                    dataGridView1.Columns["section"].HeaderText = "Section";
                    dataGridView1.Columns["Students"].HeaderText = "Students";
                    dataGridView1.Columns["capacity"].HeaderText = "Capacity";
                    dataGridView1.Columns["room_assignment"].HeaderText = "Room Assignment";
                    dataGridView1.Columns["Status"].HeaderText = "Status";

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string status = row.Cells["Status"].Value?.ToString();
                        if (status == "Full")
                            row.DefaultCellStyle.BackColor = Color.LightCoral;
                        else if (status == "Almost Full")
                            row.DefaultCellStyle.BackColor = Color.Khaki;
                        else if (status == "Available")
                            row.DefaultCellStyle.BackColor = Color.LightGreen;
                        else if (status == "N/A")
                            row.DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading classes: " + ex.Message);
                }
                finally { CloseConnection(); }
            }
            UpdateActionButtonStates();
        }

        private void UpdateActionButtonStates()
        {
            bool rowSelected = dataGridView1.SelectedRows.Count > 0;
            btnEdit.Enabled = rowSelected;
            btnDelete.Enabled = rowSelected;
        }

        private void OnClassDataChanged(object sender, EventArgs e)
        {
            LoadClasses(txtSearch.Text.ToString());
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            FormCreateClass f = new FormCreateClass();
            f.ClassDataChanged += OnClassDataChanged;
            f.ShowDialog();
            f.ClassDataChanged -= OnClassDataChanged;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow r = dataGridView1.SelectedRows[0];
                int id = Convert.ToInt32(r.Cells["id"].Value);
                string g = r.Cells["grade_level"].Value.ToString();
                string t = r.Cells["track"].Value.ToString();
                string s = r.Cells["section"].Value.ToString();
                int c = r.Cells["capacity"].Value != DBNull.Value ? Convert.ToInt32(r.Cells["capacity"].Value) : 0;
                string ra = r.Cells["room_assignment"].Value.ToString();

                // Get the current number of students assigned to this class
                int currentStudents = r.Cells["Students"].Value != DBNull.Value ? Convert.ToInt32(r.Cells["Students"].Value) : 0;

                // Check if the class is currently full or over capacity before opening the edit form
                if (currentStudents >= c && c > 0)
                {
                    MessageBox.Show(
                        "This class is currently at or over its capacity (" + c + " students). " +
                        "Please note that you cannot reduce the class capacity below the current number of students (" + currentStudents + "). " +
                        "The actual capacity validation should be handled in the 'FormCreateClass' when saving changes.",
                        "Warning: Class Capacity",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                FormCreateClass f = new FormCreateClass(id, g, t, s, c, ra);
                f.ClassDataChanged += OnClassDataChanged;
                f.ShowDialog();
                f.ClassDataChanged -= OnClassDataChanged;
            }
            else MessageBox.Show("Please select a class to edit.");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 &&
                MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
                if (OpenConnection())
                {
                    try
                    {
                        string q = "DELETE FROM section_list WHERE id=@id";
                        MySqlCommand cmd = new MySqlCommand(q, connection);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Class deleted.");
                        LoadClasses(txtSearch.Text.ToString());
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    finally { CloseConnection(); }
                }
            }
        }
        private void btnManageTeachers_Click(object sender, EventArgs e)
        {
            // TODO: Implement teacher management logic here
        }

        private void btnManageUsers_Click(object sender, EventArgs e)
        {
            // TODO: Implement user management logic here
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Optional: handle clicks inside specific cells
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // TODO: Save changes
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // TODO: Cancel logic
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadClasses(txtSearch.Text.ToString());
        }

        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadClasses(txtSearch.Text.ToString());
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            UpdateActionButtonStates();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";

            LoadClasses();
        }
    }
}
