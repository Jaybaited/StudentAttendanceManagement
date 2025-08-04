// File: DatabaseHelper.cs
using MySql.Data.MySqlClient;
using System.Windows.Forms; // For MessageBox

public class DatabaseHelper
{
    private static string connectionString = "Server=localhost;Port=3306;Database=db_attendance;Uid=root;Pwd=;"; // Adjust Uid/Pwd if needed

    public static MySqlConnection GetConnection()
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();
            return connection;
        }
        catch (MySqlException ex)
        {
            MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }

    public static void CloseConnection(MySqlConnection connection)
    {
        if (connection != null && connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
        }
    }
}