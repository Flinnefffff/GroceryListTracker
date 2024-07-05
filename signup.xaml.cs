using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Grocery_List_Tracker
{
    public partial class signup : Window
    {
        private const string connectionString = "Server=mariadb.vamk.fi;Database=e2101098_Windows;Uid=e2101098;Pwd=cqgYeaFEN6A;";

        public signup()
        {
            InitializeComponent();

            // Handle the KeyDown event for the text boxes
            emailtxtbox.KeyDown += (sender, e) => { if (e.Key == Key.Enter) CreateButton_Click(sender, e); };
            usernametxtbox.KeyDown += (sender, e) => { if (e.Key == Key.Enter) CreateButton_Click(sender, e); };
            pwdtxtbox.KeyDown += (sender, e) => { if (e.Key == Key.Enter) CreateButton_Click(sender, e); };
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string email = emailtxtbox.Text;
            string username = usernametxtbox.Text;
            string password = pwdtxtbox.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email.");
                return;
            }

            try
            {
                bool userCreated = await Task.Run(() =>
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        // Check if the username already exists
                        string checkQuery = "SELECT COUNT(*) FROM Users WHERE Name = @Name";
                        MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                        checkCommand.Parameters.AddWithValue("@Name", username);

                        object result = checkCommand.ExecuteScalar();
                        if (result != null && result is long && (long)result > 0)
                        {
                            return false;
                        }

                        string query = "INSERT INTO Users (Name, Email, PasswordHash) VALUES (@Name, @Email, @PasswordHash)";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Name", username);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PasswordHash", password);

                        // Execute the SQL query and get the number of affected rows
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                });

                if (userCreated)
                {
                    MessageBox.Show("User created successfully.");

                    // Show the login page
                    login loginPage = new login();
                    loginPage.Show();

                    // Close the current window
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to create user. This username may already be taken.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void AlreadyHaveAccountButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the login page
            login loginPage = new login();
            loginPage.Show();

            // Close the current window
            this.Close();
        }
    }
}
