using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Grocery_List_Tracker
{
    public partial class history_purchase : Window
    {
        public class PurchaseHistoryItem
        {
            public int UserId { get; set; }
            public string ItemName { get; set; }
            public decimal Price { get; set; }
            public decimal Quantity { get; set; }
            public DateTime Date { get; set; }
            public string StoreName { get; set; }
        }

        private const string connectionString = "Server=mariadb.vamk.fi;Database=e2101098_Windows;Uid=e2101098;Pwd=cqgYeaFEN6A;";

        public history_purchase(int userId)
        {
            InitializeComponent();

            // Fetch the purchase history from the database
            List<PurchaseHistoryItem> purchaseHistory = FetchPurchaseHistoryForUser(userId);

            // Bind the purchase history to the DataGrid
            PurchaseHistoryDataGrid.ItemsSource = purchaseHistory;
        }

        private List<PurchaseHistoryItem> FetchPurchaseHistoryForUser(int userId)
        {
            var purchaseHistory = new List<PurchaseHistoryItem>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand("SELECT * FROM Purchase WHERE UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new PurchaseHistoryItem
                            {
                                UserId = reader.GetInt32("UserId"),
                                ItemName = reader.GetString("ItemName"),
                                Price = reader.GetDecimal("Price"),
                                Quantity = reader.GetDecimal("Quantity"),
                                Date = reader.GetDateTime("Date"),
                                StoreName = reader.GetString("StoreName")
                            };

                            purchaseHistory.Add(item);
                        }
                    }
                }
            }

            return purchaseHistory;
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
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the home window and show it
            home homeWindow = new home();
            homeWindow.Show();

            // Close the history_purchase window
            this.Close();
        }
    }
}
