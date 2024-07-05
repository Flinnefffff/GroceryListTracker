using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Windows.Input;

namespace Grocery_List_Tracker
{
    /// <summary>
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class login : Window
    {
        private const string connectionString = "Server=mariadb.vamk.fi;Database=e2101098_Windows;Uid=e2101098;Pwd=cqgYeaFEN6A;";

        public static int LoggedInUserId { get; set; }

        public login()
        {
            InitializeComponent();

            // Handle the KeyDown event for the text boxes
            usernametxtbox.KeyDown += (sender, e) => { if (e.Key == Key.Enter) LoginButton_Click(sender, e); };
            pwdtxtbox.KeyDown += (sender, e) => { if (e.Key == Key.Enter) LoginButton_Click(sender, e); };
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernametxtbox.Text;
            string password = pwdtxtbox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Id FROM Users WHERE Name = @Username AND PasswordHash = @Password";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        LoggedInUserId = Convert.ToInt32(result);
                        MessageBox.Show("Login successful!", "Login", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Navigate to the home window
                        home homeWindow = new home();
                        homeWindow.Show();

                        // Close the login window
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.", "Login", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string CalculateHash(string input)
        {
            // Implement your password hashing function here
            throw new NotImplementedException();
        }
    }
}
