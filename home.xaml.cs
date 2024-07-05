using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Grocery_List_Tracker
{
    /// <summary>
    /// Interaction logic for home.xaml
    /// </summary>
    public partial class home : Window
    {
        private const string connectionString = "Server=mariadb.vamk.fi;Database=e2101098_Windows;Uid=e2101098;Pwd=cqgYeaFEN6A;";

        public home()
        {
            InitializeComponent();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the login window and show it
            login loginWindow = new login();
            loginWindow.Show();

            // Close the home window
            this.Close();
        }
        private void ChartButton_Click(object sender, RoutedEventArgs e)
        {
            chart chartWindow = new chart(login.LoggedInUserId);
            chartWindow.Show();
            this.Close();
        }
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the history_purchase window and show it
            history_purchase historyWindow = new history_purchase(login.LoggedInUserId);
            historyWindow.Show();
            this.Close();
        }

        private void AddPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            string itemName = ItemNameTextBox.Text;

            decimal price;
            if (!decimal.TryParse(PriceTextBox.Text, out price))
            {
                MessageBox.Show("Please enter a valid number for the price.");
                return;
            }

            decimal quantity;
            if (!decimal.TryParse(QuantityTextBox.Text, out quantity))
            {
                MessageBox.Show("Please enter a valid number for the quantity.");
                return;
            }

            DateTime date = DateDatePicker.SelectedDate ?? DateTime.Now;
            string storeName = StoreNameTextBox.Text;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO Purchase (UserId, ItemName, Price, Quantity, Date, StoreName) VALUES (@UserId, @ItemName, @Price, @Quantity, @Date, @StoreName)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserId", login.LoggedInUserId);
                    command.Parameters.AddWithValue("@ItemName", itemName);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@Date", date);
                    command.Parameters.AddWithValue("@StoreName", storeName);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Purchase added successfully:\n\nItem: {itemName}\nPrice: {price}\nQuantity: {quantity}\nDate: {date.ToShortDateString()}\nStore: {storeName}");
                    }
                    else
                    {
                        MessageBox.Show("Failed to add purchase.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
