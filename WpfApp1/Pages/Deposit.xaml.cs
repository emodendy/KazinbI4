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
    /// Логика взаимодействия для Deposit.xaml
    /// </summary>
    public partial class Deposit : Page
    {
        string _name;
        public Deposit(string name)
        {
            InitializeComponent();
            _name = name;
        }

        private void TopUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AmountTextBox.Text, out int amount) && amount > 0)
            {
                MessageBox.Show($"Пополнение на сумму {amount} выполнено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateBalance(amount, isTopUp: true);

            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректную сумму для пополнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AmountTextBox.Text, out int amount) && amount > 0)
            {
                User Log = ConnectionClass.connect.User.FirstOrDefault(log => log.Username == _name);
                if (Log.Balance >= amount)
                {
                    MessageBox.Show($"Запрос на вывод средств на сумму {amount} отправлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                   UpdateBalance(amount, isTopUp: false);
                }
                else{
                    MessageBox.Show($"Недостаточна средств", "ок", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректную сумму для вывода.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateBalance(int amount, bool isTopUp)
        {

            User Log = ConnectionClass.connect.User.FirstOrDefault(log => log.Username == _name);
            if (isTopUp)
            {
                Log.Balance += amount;
            }
            if(!isTopUp)
            {
                
                    Log.Balance -= amount;
                
                
            }

            Transactions transaction = new Transactions
            {
                UserID = Log.UserID,
                TransactionType = isTopUp ? "Пополнение" : "Вывод",
                Amount = amount,
                TransactionDate = DateTime.Now
            };

            ConnectionClass.connect.Transactions.Add(transaction);

            ConnectionClass.connect.SaveChanges();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            User Log = ConnectionClass.connect.User.FirstOrDefault(log => log.Username == _name);
            int id = Convert.ToInt32(Log.Balance);
            string name = Log.Username;
            NavigationService.Navigate(new Menu(id, name));
        }

        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
