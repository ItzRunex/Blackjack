﻿using System;
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
        public Login()
        {
            InitializeComponent();
        }
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string DBPath = @"LABSCIFIPC20\LOCALHOST";
            SqlConnection sqlCon = new SqlConnection($@"Data Source ={DBPath}; Initial Catalog = Accounts; Integrated Security = True");
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                if (!CheckLogin(sqlCon))
                {
                    MessageBoxResult result = MessageBox.Show("No account matching! Do you want to register a new account?", "No Account", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        string regQuery = "INSERT INTO Accounts (Username, Password) VALUES (@Username, @Password)";
                        SqlCommand sqlRegCmd = new SqlCommand(regQuery, sqlCon);
                        sqlRegCmd.CommandType = CommandType.Text;
                        sqlRegCmd.Parameters.AddWithValue("@Username", txtUserName.Text);
                        sqlRegCmd.Parameters.AddWithValue("@Password", PasswordBox.Password);
                        sqlRegCmd.ExecuteNonQuery();
                        if (!CheckLogin(sqlCon))
                        {
                            MessageBox.Show("An error has occurred while creating your account!");
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                MainWindow dashboard = new MainWindow();
                dashboard.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }
        private bool CheckLogin(SqlConnection sqlCon)
        {
            string query = "SELECT COUNT(1) FROM Accounts WHERE Username=@Username AND Password=@Password";
            SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.Parameters.AddWithValue("@Username", txtUserName.Text);
            sqlCmd.Parameters.AddWithValue("@Password", PasswordBox.Password);
            int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
            return count == 1;
        }
    }
}
