using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace StudentAttendanceManagement
{
    public partial class FormAddStudent : Form
    {
        string connectionStrings = "server=localhost;uid=root;pwd=;database=db_attendance;";
        private string _studentIdToEdit = string.Empty; // To store the ID if we are editing

        // Constructor for adding a new student (parameterless)
        public FormAddStudent()
        {
            InitializeComponent();
            this.Text = "Add New Student"; // Set form title for adding
        }

        // Constructor for editing an existing student (takes studentId as argument)
        public FormAddStudent(string studentId) : this() // Call the default constructor first
        {
            _studentIdToEdit = studentId;
            this.Text = "Edit Student Information"; // Set form title for editing mode
            btnCreateUser.Text = "Update Student"; // Change the text of your save/create button to indicate "Update"

            LoadStudentDataForEdit(studentId);
        }

        // Method to load existing student data into form controls
        private void LoadStudentDataForEdit(string studentId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionStrings))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT first_name, middle_name, last_name, gender, assigned_section
                                     FROM users
                                     WHERE student_id = @student_id AND role = 'Student'";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", studentId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtFirstName.Text = reader["first_name"].ToString();
                                txtMiddleName.Text = reader["middle_name"].ToString();
                                txtLastName.Text = reader["last_name"].ToString();

                                string gender = reader["gender"].ToString();
                                if (gender == "Male")
                                {
                                    rbMale.Checked = true;
                                }
                                else if (gender == "Female")
                                {
                                    rbFemale.Checked = true;
                                }
                                // If you have cmbAssignedSection, load it here.
                                // cmbAssignedSection.Text = reader["assigned_section"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Student with ID " + studentId + " not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.DialogResult = DialogResult.Cancel;
                                this.Close();
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error loading student data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
        }


        // This method will now handle both creating (INSERT) and updating (UPDATE) students.
        private void btnCreateUser_Click_2(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string middleName = txtMiddleName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string gender = rbMale.Checked ? "Male" : (rbFemale.Checked ? "Female" : "");
            // Assuming you do not have an assigned_section input on this form for student creation/edit
            // If you do, retrieve it here: string assignedSection = cmbAssignedSection.Text.Trim();


            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(gender))
            {
                MessageBox.Show("Please fill in required fields and select gender.");
                return;
            }

            string role = "Student";

            using (MySqlConnection conn = new MySqlConnection(connectionStrings))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                if (string.IsNullOrEmpty(_studentIdToEdit)) // This is an Add operation
                {
                    string studentId = GenerateNextStudentId(); // Generate new ID for new students
                    // Generate a unique username for the student
                    // You can customize this, e.g., "S" + studentId, or first initial + last name + studentId
                    string username = "S" + studentId; // Example: "S0300"
                    string defaultPassword = "password123"; // Set a default password for new students

                    string insertQuery = @"INSERT INTO users (student_id, first_name, middle_name, last_name, gender, role, username, password)
                                           VALUES (@student_id, @first_name, @middle_name, @last_name, @gender, @role, @username, @password)";
                    // If you also want to insert assigned_section during creation:
                    // string insertQuery = @"INSERT INTO users (student_id, first_name, middle_name, last_name, gender, role, username, password, assigned_section)
                    //                        VALUES (@student_id, @first_name, @middle_name, @last_name, @gender, @role, @username, @password, @assigned_section)";

                    cmd.CommandText = insertQuery;
                    cmd.Parameters.AddWithValue("@student_id", studentId);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", defaultPassword); // Add the password parameter
                    MessageBox.Show("Student created successfully with ID: " + studentId + " and Username: " + username);
                }
                else // This is an Update operation
                {
                    // For student updates, we typically don't change the username/password
                    // unless explicitly added controls for them in FormAddStudent.
                    // This query only updates the student's personal info and assigned_section if present.
                    string updateQuery = @"UPDATE users
                                           SET first_name = @first_name,
                                               middle_name = @middle_name,
                                               last_name = @last_name,
                                               gender = @gender
                                               -- If you have an assigned_section input on this form for update:
                                               -- , assigned_section = @assigned_section
                                           WHERE student_id = @student_id AND role = 'Student'";

                    cmd.CommandText = updateQuery;
                    cmd.Parameters.AddWithValue("@student_id", _studentIdToEdit); // Use the existing ID for update
                }

                // Add common parameters for both INSERT and UPDATE
                cmd.Parameters.AddWithValue("@first_name", firstName);
                cmd.Parameters.AddWithValue("@middle_name", middleName);
                cmd.Parameters.AddWithValue("@last_name", lastName);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@role", role); // Still passed, useful if WHERE role = 'Student' is used
                // If you have an assigned_section input:
                // cmd.Parameters.AddWithValue("@assigned_section", assignedSection);


                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Student information saved successfully.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (MySqlException ex)
                {
                    // This will now catch other potential database errors, but the username issue should be fixed.
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GenerateNextStudentId()
        {
            int startingId = 300;
            int maxId = startingId;

            using (MySqlConnection conn = new MySqlConnection(connectionStrings))
            {
                conn.Open();
                string query = "SELECT CAST(student_id AS UNSIGNED) FROM users WHERE role = 'Student' ORDER BY CAST(student_id AS UNSIGNED) DESC LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int lastId))
                    {
                        maxId = Math.Max(lastId + 1, startingId);
                    }
                }
            }
            return maxId.ToString("0000"); // Formats like "0300", "0301", ...
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FormAddStudent_Load(object sender, EventArgs e)
        {

        }
    }
}