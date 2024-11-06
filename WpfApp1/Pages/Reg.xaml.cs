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
    /// Логика взаимодействия для Reg.xaml
    /// </summary>
    public partial class Reg : Page
    {
        public Reg()
        {
            InitializeComponent();
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Пожалуйста, введите имя пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Пожалуйста, введите электронную почту.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);


            ConnectionClass.connect.User.Add(new User
            {
                Username = username,
                IsBanned = false,
                RegistrationDate = DateTime.Now,
                PassHash = HashPassword(password),
                Telegram = email,
                Balance = 0
            });
            
            ConnectionClass.connect.SaveChanges();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }
    }
}
