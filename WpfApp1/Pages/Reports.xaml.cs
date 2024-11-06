using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для Reports.xaml
    /// </summary>
    public partial class Reports : Page
    {
        private string _name;
        public Reports(string name)
        {
            InitializeComponent();
            _name = name;
            var user = ConnectionClass.connect.User.FirstOrDefault(u => u.Username == _name);
            ListCactus.ItemsSource = ConnectionClass.connect.GameSessions.Where(cactus => cactus.UserID == user.UserID).ToList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
