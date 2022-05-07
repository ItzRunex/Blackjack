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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private static SqlConnection sqlCon = new SqlConnection(@"Data Source = dewest.database.windows.net; Initial Catalog = blackjackgame; User ID = mish; Password = Shomiegotin1; Trusted_Connection = False; MultipleActiveResultSets = True");
        private MainWindow mainWindow = null;
        public Settings(MainWindow owner)
        {
            InitializeComponent();
            mainWindow = owner;
            userTextbox.Text = mainWindow.Player.Username;
            bgDisplay.Source = ((ImageBrush)mainWindow.Background).ImageSource;
        }
        private void SaveUser(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(mainWindow.Player.Username);
            if (userTextbox.Text.Equals(null) || userTextbox.Text.Equals(""))
                MessageBox.Show("Your username cannot be null!");
            else
            {
                SqlCommand changeUser = new SqlCommand($"UPDATE Accounts SET Username='{userTextbox.Text}' WHERE Username='{mainWindow.Player.Username}'", MainWindow.sqlCon);
                changeUser.ExecuteNonQuery();
                mainWindow.Player.Username = userTextbox.Text;
                MessageBox.Show($"Username successfully changed to \"{userTextbox.Text}\"");
            }
        }
        private void SavePw(object sender, RoutedEventArgs e)
        {
            if (pwTextbox.Password.Equals(null) || pwTextbox.Password.Equals(""))
                MessageBox.Show("Your password cannot be null!");
            else
            {
                SqlCommand changePass = new SqlCommand($"UPDATE Accounts SET Password='{pwTextbox.Password}' WHERE Username='{mainWindow.Player.Username}'", MainWindow.sqlCon);
                changePass.ExecuteNonQuery();
                MessageBox.Show($"Password successfully changed.");
            }
        }
        private string[] imgs = { "default.png", "red.png", "yellow.png" };
        private int GetImageIndex()
        {
            int imgIndex = -5;
            for (int i = 0; i < imgs.Length; i++)
                if (bgDisplay.Source.ToString().Contains(imgs[i]))
                    imgIndex = i;
            return imgIndex;
        }
        private void SwitchBg(object sender, RoutedEventArgs e)
        {
            int imgIndex = GetImageIndex();
            Console.WriteLine(imgIndex);
            Button button = sender as Button;
            if (button.Name.Equals("previousBg"))
            {
                imgIndex--;
                if (imgIndex < 0)
                    imgIndex = 2;
            }
            else if (button.Name.Equals("nextBg"))
            {
                imgIndex++;
                if (imgIndex > 2)
                    imgIndex = 0;
            }
            bgDisplay.Source = new BitmapImage(new Uri($"Assets\\Backgrounds\\{imgs[imgIndex]}", UriKind.Relative));
        }
        private void SaveBg(object sender, RoutedEventArgs e)
        {
            int imgIndex = GetImageIndex();
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();
            SqlCommand changeBg = new SqlCommand($"UPDATE Accounts SET Background={imgIndex} WHERE Username='{mainWindow.Player.Username}'", sqlCon);
            changeBg.ExecuteNonQuery();
            mainWindow.CheckBackground();
        }
        private void CloseSettings(object sender, RoutedEventArgs e)
        {
            Close();
            Closed += (a, b) => { Owner.Activate(); };
        }
    }
}
