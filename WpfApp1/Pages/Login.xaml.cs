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
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Threading;
using WpfApp1.Services;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        int code = new Random().Next(100000, 999999);
        private readonly TelegramService _telegramService;
        private CancellationTokenSource _cancellationTokenSource;

        public Login()
        {
            InitializeComponent();
            _telegramService = new TelegramService("7607352593:AAFhuj0-E7YzAkP9MHulawDPe997RKNnXX4");
            _cancellationTokenSource = new CancellationTokenSource();
            StartTelegramService();
        }

        private async void StartTelegramService()
        {
            try
            {
                await _telegramService.StartAsync(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска Telegram бота: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string usernameOrEmail = UsernameOrEmailTextBox.Text;
            string password = PasswordBox.Password;
            var user = ConnectionClass.connect.User.FirstOrDefault(u => u.Username == usernameOrEmail || u.Telegram == usernameOrEmail);


            if (user != null && VerifyPassword(user, password))
            {
                
                if (user.TelegramChatID == null)
                {
                    MessageBox.Show("TelegramChatID не установлен. Пожалуйста, свяжитесь с администратором.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else {
                   
                }
                    if (fac.Text == code.ToString())
                    {
                        MessageBox.Show("Успешный вход!");
                    NavigationService.Navigate(new Menu(user.Balance, user.Username));
                    }
                    else
                    {
                        MessageBox.Show("Неверный код 2FA!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
            }
            else
            {
                MessageBox.Show("Неверные данные для входа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Send2FACodeToTelegram(long chatId, int code)
        {
            Task.Run(async () =>
            {
                try
                {
                    await _telegramService.SendTextMessageAsync(chatId, $"Ваш код для входа: {code}");
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Ошибка при отправке кода 2FA: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            });
        }
        private bool VerifyPassword(DB.User user, string password)
        {
            return user.PassHash == HashPassword(password);
        }
        public string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Reg());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string usernameOrEmail = UsernameOrEmailTextBox.Text;
            string password = PasswordBox.Password;
            var user = ConnectionClass.connect.User.FirstOrDefault(u => u.Username == usernameOrEmail || u.Telegram == usernameOrEmail );
            if (user.PassHash == HashPassword(password)) {
                Send2FACodeToTelegram(user.TelegramChatID.Value, code);
            }
            else{
                MessageBox.Show("Проверьте порол!");
            }
        }
    }
}
