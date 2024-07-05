using System;
using System.Windows;

namespace Grocery_List_Tracker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to signup.xaml page
            signup signUpPage = new signup();
            signUpPage.Show();
            this.Close(); // Close current window if needed
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to login.xaml page
            login loginPage = new login();
            loginPage.Show();
            this.Close(); // Close current window if needed
        }
    }
}
