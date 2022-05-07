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
    /// Interaction logic for Leaderboard.xaml
    /// </summary>
    public partial class Leaderboard : Window
    {
        TextBlock[] places;
        public Leaderboard()
        {
            InitializeComponent();
            places = new TextBlock[] { FirstPlace, SecondPlace, ThirdPlace, FourthPlace, FifthPlace };
            SqlCommand retrievePlaceBalance = new SqlCommand($"SELECT TOP 5 Username, Balance FROM Accounts ORDER BY Balance DESC", MainWindow.sqlCon);
            SqlDataReader reader = retrievePlaceBalance.ExecuteReader();
            for (int i = 0; i < places.Length; i++)
            {
                reader.Read();
                IDataRecord columns = reader;
                if (i < 3)
                    places[i].Text = $"{columns[0]} - ${columns[1]}";
                else
                    places[i].Text = $"{i + 1}. {columns[0]} - ${columns[1]}";
            }
        }
        private void CloseLeaderboard(object sender, RoutedEventArgs e)
        {
            Close();
            Closed += (a, b) => { Owner.Activate(); };
        }
    }
}
