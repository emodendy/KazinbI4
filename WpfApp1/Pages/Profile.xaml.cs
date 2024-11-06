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
using WpfApp1.DB;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Profile.xaml
    /// </summary>
    public partial class Profile : Page
    {
        private string _name;
        public Profile(string name)
        {
            InitializeComponent();
            _name = name;
            var user = ConnectionClass.connect.User.FirstOrDefault(u => u.Username == _name);

            if (user != null)
            {
                UsernameTextBlock.Text = user.Username;
                RegistrationDateTextBlock.Text = user.RegistrationDate.ToString("dd.MM.yyyy");
                BalanceTextBlock.Text = $"{user.Balance:C}";
                TelegramTextBlock.Text = user.TelegramChatID.HasValue ? user.TelegramChatID.ToString() : "Не привязан";
                var totalBets = ConnectionClass.connect.GameSessions
    .Where(gs => gs.UserID == user.UserID && gs.BetAmount.HasValue)
    .Sum(gs => gs.BetAmount.Value);

                var totalWins = ConnectionClass.connect.GameSessions
                    .Where(gs => gs.UserID == user.UserID && gs.WinAmount.HasValue)
                    .Sum(gs => gs.WinAmount.Value);

                TotalBetsTextBlock.Text = $"{totalBets:C}";
                TotalWinsTextBlock.Text = $"{totalWins:C}";

            }
            else
            {
                MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void LoadUserProfile()
        {
            
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Reports(_name));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
