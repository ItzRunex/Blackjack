using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Shapes;

namespace BlackjackGame
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
            RefreshTable();
        }
        private void RefreshTable()
        {
            InitializeComponent();
            SqlCommand selectAll = new SqlCommand("SELECT Username, Balance, Background, IsAdmin FROM Accounts", MainWindow.sqlCon);
            selectAll.ExecuteNonQuery();
            SqlDataAdapter adapter = new SqlDataAdapter(selectAll);
            DataTable dt = new DataTable("Users");
            adapter.Fill(dt);
            AllUsersAdminDisplay.ItemsSource = dt.DefaultView;
            adapter.Update(dt);
        }
        public void SaveUserBalance(object sender, RoutedEventArgs e)
        {
            if (VerifyUser())
            {
                if (int.TryParse(BalanceTb.Text, out int balance))
                {
                    if (balance >= 50)
                    {
                        SqlCommand updateBalance = new SqlCommand($"UPDATE Accounts SET Balance = {balance} WHERE Username = '{UserTb.Text}'", MainWindow.sqlCon);
                        if (updateBalance.ExecuteNonQuery() == 1)
                            MessageBox.Show($"Successfully updated {UserTb.Text}'s balance!");
                        else
                            MessageBox.Show($"Failed to update user's balance.");
                        RefreshTable();
                    }
                    else
                    {
                        MessageBox.Show("Balance cannot be a value smaller than 50!");
                    }
                }
                else
                {
                    MessageBox.Show("Balance value is invalid!");
                }
            }
        }
        public void AddToAdmin(object sender, RoutedEventArgs e)
        {
            if (VerifyUser())
            {
                SqlCommand addUserToAdmin = new SqlCommand($"UPDATE Accounts SET IsAdmin = 1 WHERE Username = '{UserTb.Text}'", MainWindow.sqlCon);
                if (addUserToAdmin.ExecuteNonQuery() == 1)
                    MessageBox.Show($"Successfully set {UserTb.Text} as admin!");
                else
                    MessageBox.Show($"Failed to set user as admin.");
                RefreshTable();
            }
        }
        public void RemoveFromAdmin(object sender, RoutedEventArgs e)
        {
            if (VerifyUser())
            {
                SqlCommand removeUserFromAdmin = new SqlCommand($"UPDATE Accounts SET IsAdmin = 0 WHERE Username = '{UserTb.Text}'", MainWindow.sqlCon);
                if (removeUserFromAdmin.ExecuteNonQuery() == 1)
                    MessageBox.Show($"Successfully removed {UserTb.Text}'s admin rights!");
                else
                    MessageBox.Show($"Failed to remove user's admin rights.");
                RefreshTable();
            }
        }
        public void DeleteUser(object sender, RoutedEventArgs e)
        {
            if (VerifyUser())
            {
                SqlCommand deleteUser = new SqlCommand($"DELETE FROM Accounts WHERE Username = '{UserTb.Text}'", MainWindow.sqlCon);
                if (deleteUser.ExecuteNonQuery() == 1)
                    MessageBox.Show($"Successfully deleted user {UserTb.Text}.");
                else
                    MessageBox.Show("Failed to delete user.");
                RefreshTable();
            }
        }
        private bool VerifyUser()
        {
            if (UserTb.Text == "" || UserTb.Text == null)
            {
                MessageBox.Show("Username cannot be empty!");
                return false;
            }
            SqlCommand selectUser = new SqlCommand($"SELECT COUNT(Username) FROM Accounts WHERE Username = '{UserTb.Text}'", MainWindow.sqlCon);
            if (!selectUser.ExecuteScalar().Equals(1))
            {
                MessageBox.Show("There is no such user!");
                return false;
            }
            return true;
        }
        private void CloseAdmin(object sender, RoutedEventArgs e)
        {
            Close();
            Closed += (a, b) => { Owner.Activate(); };
        }
    }
}
