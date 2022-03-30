using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for LoglnScreen.xaml
    /// </summary>
    public partial class Login : Window
    {
        private static SqlConnection sqlCon = new SqlConnection(@"Data Source = dewest.database.windows.net; Initial Catalog = blackjackgame; User ID = mish; Password = Shomiegotin1; Trusted_Connection = False; MultipleActiveResultSets = True");
        private static string username;
        private static int balance = 10;
        public Login()
        {
            InitializeComponent();
        }
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            username = txtUserName.Text;
            try
            {
                if ((GetBalance()) < 50)
                {
                    MessageBoxResult result = MessageBox.Show("You either do not have an account or have ran out of money! Do you want to register a new account?", "No Account", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (sqlCon.State == ConnectionState.Closed)
                            sqlCon.Open();
                        SqlCommand sqlRegCmd = new SqlCommand($"INSERT INTO Accounts (Username, Password, Balance) VALUES ('{txtUserName.Text}', '{PasswordBox.Password}', 2500)", sqlCon);
                        sqlRegCmd.ExecuteNonQuery();
                        if (GetBalance() < 50)
                        {
                            MessageBox.Show("An error occurred while creating your account!");
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                balance = GetBalance();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public int GetBalance()
        {
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();
            string query = $"SELECT Balance FROM Accounts WHERE Username='{txtUserName.Text}' AND Password='{PasswordBox.Password}'";
            SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
            int balance = Convert.ToInt32(sqlCmd.ExecuteScalar());
            return balance;
        }
        public string Username
        {
            get { return username; }
        }
        public int Balance
        {
            get { return balance; }
        }
    }
}
