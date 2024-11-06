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
using System.Windows.Threading;
using Telegram.Bot.Types;
using WpfApp1.DB;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Crash.xaml
    /// </summary>
    public partial class Crash : Page
    {
        private bool _isGameRunning = false;
        private double _currentMultiplier = 1.0;
        private double _crashPoint;
        private DispatcherTimer _timer;
        string _name;
        private int _userId;
        int _betAmount;
        
        public Crash(string name, int id)
        {
            InitializeComponent();
            _name = name;
            _userId = id;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.03);
            _timer.Tick += Timer_Tick;
            
        }
        
        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            _betAmount = Convert.ToInt32(StatusTextBlock.Text);
            var user = ConnectionClass.connect.User.FirstOrDefault(u => u.Username == _name);
            if (user.Balance < _betAmount)
            {
                MessageBox.Show("Недостаточно средств для ставки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            user.Balance -= _betAmount;
            ConnectionClass.connect.SaveChanges();

            var gameSession = new GameSessions
            {
                UserID = user.UserID,
                GameID = 1,
                Date = DateTime.Now,
                BetAmount = _betAmount,
                WinAmount = 0
            };
            ConnectionClass.connect.GameSessions.Add(gameSession);
            ConnectionClass.connect.SaveChanges();

            _crashPoint = GenerateCrashPoint();
            _currentMultiplier = 1.0;
            MultiplierTextBlock.Text = $"{_currentMultiplier:F2}x";
            MultiplierProgressBar.Value = _currentMultiplier;

            StatusTextBlock.Text = "Игра началась!";
            PlayButton.IsEnabled = false;
            CashOutButton.IsEnabled = true;
            _isGameRunning = true;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_isGameRunning)
            {
                _currentMultiplier += 0.01;
                MultiplierTextBlock.Text = $"{_currentMultiplier:F2}x";
                MultiplierProgressBar.Value = _currentMultiplier;

                if (_currentMultiplier >= _crashPoint)
                {
                    var user = ConnectionClass.connect.User.Find(_userId);
                    _timer.Stop();
                    _isGameRunning = false;
                    CashOutButton.IsEnabled = false;
                    PlayButton.IsEnabled = true;
                    StatusTextBlock.Text = $"Краш! Коэффициент достиг {_crashPoint:F2}x. Вы потеряли ставку.";
                    user.Balance -= Convert.ToInt32(_betAmount);
                    var gameSession = ConnectionClass.connect.GameSessions.FirstOrDefault(gs => gs.UserID == _userId && gs.WinAmount == 0);
                    if (gameSession != null)
                    {
                        gameSession.WinAmount = -_betAmount;
                        ConnectionClass.connect.SaveChanges();
                    }
                    var transaction = new Transactions
                    {
                        UserID = _userId,
                        TransactionType = "Ставка",
                        Amount = -_betAmount,
                        TransactionDate = DateTime.Now
                    };

                    ConnectionClass.connect.Transactions.Add(transaction);
                    ConnectionClass.connect.SaveChanges();
                }
            }
        }

        private async void CashOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isGameRunning)
            {
                _timer.Stop();
                _isGameRunning = false;
                CashOutButton.IsEnabled = false;
                PlayButton.IsEnabled = true;
                StatusTextBlock.Text = $"Вы кэшировали при коэффициенте {_currentMultiplier:F2}x. Ваш выигрыш: {_betAmount * (decimal)_currentMultiplier}";

                decimal winAmount = _betAmount * (decimal)_currentMultiplier;

                var user = ConnectionClass.connect.User.Find(_userId);
                user.Balance += Convert.ToInt32(winAmount);
                ConnectionClass.connect.SaveChanges();
                var gameSession = ConnectionClass.connect.GameSessions.FirstOrDefault(gs => gs.UserID == _userId && gs.WinAmount == 0);
                if (gameSession != null)
                {
                    gameSession.WinAmount = winAmount;
                    ConnectionClass.connect.SaveChanges();
                }

                var transaction = new Transactions
                {
                    UserID = _userId,
                    TransactionType = "Выигрыш",
                    Amount = winAmount,
                    TransactionDate = DateTime.Now
                };
                ConnectionClass.connect.Transactions.Add(transaction);
                ConnectionClass.connect.SaveChanges();
            }
        }

        private double GenerateCrashPoint()
        {
            Random rand = new Random();
            return rand.NextDouble() * (10.0 - 1.5) + 1.5;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var user = ConnectionClass.connect.User.Find(_userId);
            NavigationService.Navigate(new Menu(user.Balance, user.Username));
        }

        private void StatusTextBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
