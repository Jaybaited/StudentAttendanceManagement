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
    public partial class ucManageAccounts : UserControl
    {
        string connectionString = "server=localhost;user id=root;password=;database=db_attendance;";

        public ucManageAccounts()
        {
            InitializeComponent();
            LoadAccounts();

            // Initialize Gender ComboBox
            cmbGender.Items.AddRange(new string[] { "Male", "Female" });
            cmbGender.SelectedIndex = 0; // Default to "Male"
        }

        private void btnManageUsers_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            ucManageAccounts usersControl = new ucManageAccounts();
            usersControl.Dock = DockStyle.Fill;
            this.Controls.Add(usersControl);
        }

        private void btnManageTeachers_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            ucManageTeachers teachersControl = new ucManageTeachers();
            teachersControl.Dock = DockStyle.Fill;
            this.Controls.Add(teachersControl);
        }

        private void btnManageStudents_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            ucManageStudents studentsControl = new ucManageStudents();
            studentsControl.Dock = DockStyle.Fill;
            this.Controls.Add(studentsControl);
        }

        private void LoadAccounts(string search = "")
        {
            // Implementation for loading accounts can be added here
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text.Trim();
            string password = txtPass.Text.Trim();
            string confirmPassword = txtCPass.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string middleName = txtMiddleName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string gender = cmbGender.SelectedItem?.ToString() ?? "";

            string role = rbAdmin.Checked ? "Admin" :
                          rbTeacher.Checked ? "Teacher" : "";
                          

            // Validate all required fields
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Please fill out all required fields.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to save this account?",
                "Confirm Save",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = @"INSERT INTO users 
                                        (username, password, first_name, middle_name, last_name, gender, role) 
                                        VALUES 
                                        (@username, @password, @first_name, @middle_name, @last_name, @gender, @role)";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.Parameters.AddWithValue("@password", password); // Note: Consider hashing the password
                            cmd.Parameters.AddWithValue("@first_name", firstName);
                            cmd.Parameters.AddWithValue("@middle_name", string.IsNullOrEmpty(middleName) ? (object)DBNull.Value : middleName);
                            cmd.Parameters.AddWithValue("@last_name", lastName);
                            cmd.Parameters.AddWithValue("@gender", gender);
                            cmd.Parameters.AddWithValue("@role", role);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Account successfully saved.");
                            LoadAccounts();
                            ClearForm();
                        }
                    }
                    catch (MySqlException ex)
                    {
                        if (ex.Number == 1062) // MySQL error number for duplicate entry
                        {
                            MessageBox.Show("Username already exists. Please choose a different username.");
                        }
                        else
                        {
                            MessageBox.Show("Error saving account: " + ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Save canceled.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtUser.Clear();
            txtPass.Clear();
            txtCPass.Clear();
            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtLastName.Clear();
            rbAdmin.Checked = false;
            rbTeacher.Checked = false;
       
            cmbGender.SelectedIndex = 0; // Reset to default
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // This can be left empty or used for specific gender radio button logic
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            // This can be left empty or used for specific gender radio button logic
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            // This can be left empty or used for specific role radio button logic
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Label click event - can be left empty
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            using (FormAddUser addUserForm = new FormAddUser())
            {
                // ShowDialog() opens the form as a modal dialog, meaning the user must interact with it
                // before returning to the parent form.
                DialogResult result = addUserForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // If the user was added successfully, you might want to refresh your user list
                    // or perform other actions on the parent form.
                    MessageBox.Show("User creation process completed.");
                    // Example: Refresh your DataGridView showing users
                    // LoadUsersData(); // You would need a method like this in your main form
                }
                else if (result == DialogResult.Cancel)
                {
                    MessageBox.Show("User creation cancelled.");
                }
            }
        }
    }
}
