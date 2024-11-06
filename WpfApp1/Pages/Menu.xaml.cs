using System;
using WpfApp1.DB;
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
using QRCoder;
using System.IO;
using Telegram.Bot.Types;
using System.Drawing;
using System.Drawing.Imaging;

namespace WpfApp1.Pages
{

    public partial class Menu : Page
    {
        string _name;
        public Menu(int balance, string name)
        {
            InitializeComponent();
            qweq.Text = balance.ToString();
            _name = name;
            imgg.Source = GeneratorQrCodeBitmapImage(name);
        }

        private void TopUpButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Deposit(_name));
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private BitmapImage GeneratorQrCodeBitmapImage(string text)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q); using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrBitmap.Save(ms, ImageFormat.Png);
                            ms.Position = 0;
                            BitmapImage bitmapImage = new BitmapImage(); bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad; bitmapImage.StreamSource = ms;
                            bitmapImage.EndInit();
                            return bitmapImage;
                        }
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var user = ConnectionClass.connect.User.FirstOrDefault(u => u.Username == _name);
            NavigationService.Navigate(new Crash(user.Username, user.UserID));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Profile(_name));
        }
    }
}
