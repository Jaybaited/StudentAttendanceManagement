using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace StudentAttendanceManagement
{
    public partial class FormAddUser : Form
    {
        private string connectionString = "Server=localhost;Port=3306;Database=db_attendance;Uid=root;Pwd=;";
        public int? TeacherId { get; set; } // Nullable int to indicate Add or Edit

        public FormAddUser()
        {
            InitializeComponent();
        }

        // Method to populate fields when editing
        public void LoadTeacherData(string firstName, string middleName, string lastName, string username, string password)
        {
            txtFirstName.Text = firstName;
            txtMiddleName.Text = middleName;
            txtLastName.Text = lastName;
            txtUsername.Text = username;
            txtPassword.Text = password;
        }

        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string middleName = txtMiddleName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all required fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query;

                if (TeacherId.HasValue)
                {
                    // Update existing user
                    query = @"UPDATE users 
                              SET first_name = @firstName,
                                  middle_name = @middleName,
                                  last_name = @lastName,
                                  username = @username,
                                  password = @password
                              WHERE id = @id";
                }
                else
                {
                    // Insert new user
                    query = @"INSERT INTO users 
                              (first_name, middle_name, last_name, username, password, role, gender)
                              VALUES (@firstName, @middleName, @lastName, @username, @password, 'Teacher', 'Male')";
                }

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@middleName", middleName);
                    command.Parameters.AddWithValue("@lastName", lastName);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    if (TeacherId.HasValue)
                        command.Parameters.AddWithValue("@id", TeacherId.Value);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show(TeacherId.HasValue ? "User updated successfully!" : "User added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Operation failed. No rows affected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
