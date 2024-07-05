using LiveCharts;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Grocery_List_Tracker
{
    public partial class chart : Window
    {
        public class SpendingHistoryItem
        {
            public string Month { get; set; }
            public decimal TotalSpending { get; set; }
        }

        private const string connectionString = "Server=mariadb.vamk.fi;Database=e2101098_Windows;Uid=e2101098;Pwd=cqgYeaFEN6A;";

        public chart(int userId)
        {
            InitializeComponent();

            List<SpendingHistoryItem> spendingHistory = FetchSpendingHistoryForUser(userId);

            // Sort the spending history by month
            spendingHistory = spendingHistory.OrderBy(item => DateTime.ParseExact(item.Month, "yyyy-MM", CultureInfo.InvariantCulture)).ToList();

            SpendingHistoryChart.Series[0].Values = new ChartValues<decimal>(spendingHistory.Select(item => item.TotalSpending).ToList());
            SpendingHistoryChart.AxisX[0].Labels = spendingHistory.Select(item => item.Month).ToList();
        }

        private List<SpendingHistoryItem> FetchSpendingHistoryForUser(int userId)
        {
            var spendingHistory = new List<SpendingHistoryItem>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Check if there is data available in the Purchase table for the specified userId
                string checkQuery = "SELECT COUNT(*) FROM Purchase WHERE UserId = @UserId";

                using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@UserId", userId);
                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        // Proceed with inserting data into the Summary table
                        string insertQuery = @"
                    INSERT INTO Summary (Month, TotalSpending, UserId)
                    SELECT DATE_FORMAT(Date, '%Y-%m') AS Month, SUM(Price * Quantity) AS TotalSpending, UserId
                    FROM Purchase
                    WHERE UserId = @UserId
                    AND DATE_FORMAT(Date, '%Y-%m') NOT IN (SELECT Month FROM Summary WHERE UserId = @UserId)
                    GROUP BY Month, UserId";

                        using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@UserId", userId);
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Handle the case when there is no data available in the Purchase table
                        // You can display a message or take any other appropriate action
                    }
                }

                // Select to get the Summary
                string selectQuery = "SELECT Month, TotalSpending FROM Summary WHERE UserId = @UserId";

                using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@UserId", userId);

                    using (MySqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new SpendingHistoryItem
                            {
                                Month = reader.GetString("Month"),
                                TotalSpending = reader.GetDecimal("TotalSpending")
                            };

                            spendingHistory.Add(item);
                        }
                    }
                }
            }

            return spendingHistory;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            home homeWindow = new home();
            homeWindow.Show();
            this.Close();
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the history_purchase window and show it
            history_purchase historyWindow = new history_purchase(login.LoggedInUserId);
            historyWindow.Show();
            this.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the login window and show it
            login loginWindow = new login();
            loginWindow.Show();

            // Close the home window
            this.Close();
        }
    }
}
