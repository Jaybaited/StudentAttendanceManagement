using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;

namespace StudentAttendanceManagement
{
    public partial class FormCreateClass : Form
    {
        private int _classId = 0;
        private bool _isEditMode = false;

        public event EventHandler ClassDataChanged;

        public FormCreateClass()
        {
            InitializeComponent();
            this.Text = "Create New Class";

            // Populate cmbGrade with options
            cmbGrade.Items.AddRange(new string[] { "1st Year", "2nd Year", "3rd Year", "4th Year" });
            // Make cmbGrade non-editable
            cmbGrade.DropDownStyle = ComboBoxStyle.DropDownList; // TWEAKED

            // Populate cmbTrack with options
            cmbTrack.Items.AddRange(new string[] { "BSHM", "BSTM", "BSBA", "BSIT" });
            // Make cmbTrack non-editable
            cmbTrack.DropDownStyle = ComboBoxStyle.DropDownList; // TWEAKED

            // Add click event handlers to clear textboxes
            txtSection.Click += new EventHandler(TextBox_Click); // TWEAKED
            txtCapacity.Click += new EventHandler(TextBox_Click); // TWEAKED
            txtRoom.Click += new EventHandler(TextBox_Click);     // TWEAKED
        }

        // CORRECTED: Removed 'status' parameter from the constructor for editing
        public FormCreateClass(int id, string gradeLevel, string track, string section, int capacity, string roomAssignment) : this()
        {
            _classId = id;
            _isEditMode = true;

            // Updated to use ComboBoxes
            cmbGrade.Text = gradeLevel;
            cmbTrack.Text = track;
            txtSection.Text = section;
            txtCapacity.Text = capacity.ToString();
            txtRoom.Text = roomAssignment;

            this.Text = "Edit Class";
        }

        string connectionString = "server=localhost;user id=root;password=;database=db_attendance;";

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            // Updated validation to check ComboBoxes
            if (string.IsNullOrWhiteSpace(cmbGrade.Text) ||
                string.IsNullOrWhiteSpace(cmbTrack.Text) ||
                string.IsNullOrWhiteSpace(txtSection.Text) ||
                string.IsNullOrWhiteSpace(txtCapacity.Text) ||
                string.IsNullOrWhiteSpace(txtRoom.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity))
            {
                MessageBox.Show("Capacity must be a number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // TWEAKED: Added validation for minimum and maximum capacity
            if (capacity < 15 || capacity > 50)
            {
                MessageBox.Show("Capacity must be between 15 and 50.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd;
                    string query;

                    if (_isEditMode)
                    {
                        // Removed 'status' from the UPDATE query
                        query = "UPDATE section_list SET grade_level=@grade, track=@track, section=@section, capacity=@capacity, room_assignment=@room WHERE id=@id";
                        cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", _classId);
                    }
                    else
                    {
                        // Removed 'status' from the INSERT query
                        query = "INSERT INTO section_list " +
                                "(grade_level, track, section, capacity, room_assignment) " +
                                "VALUES (@grade, @track, @section, @capacity, @room)";
                        cmd = new MySqlCommand(query, conn);
                    }

                    // Updated to get values from ComboBoxes
                    cmd.Parameters.AddWithValue("@grade", cmbGrade.Text.Trim());
                    cmd.Parameters.AddWithValue("@track", cmbTrack.Text.Trim());
                    cmd.Parameters.AddWithValue("@section", txtSection.Text.Trim());
                    cmd.Parameters.AddWithValue("@capacity", capacity);
                    cmd.Parameters.AddWithValue("@room", txtRoom.Text.Trim());

                    cmd.ExecuteNonQuery();
                    MessageBox.Show(_isEditMode ? "Class/Section updated successfully." : "Class/Section created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClassDataChanged?.Invoke(this, EventArgs.Empty);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data:\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }

        // TWEAKED: Generic event handler to clear textboxes
        private void TextBox_Click(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Clear();
            }
        }

        private void txtSection_MouseEnter(object sender, EventArgs e)
        {
            // This event handler was empty and is not directly related to the request,
            // but it's kept as per your instruction to only change requested code.
        }
    }
}
