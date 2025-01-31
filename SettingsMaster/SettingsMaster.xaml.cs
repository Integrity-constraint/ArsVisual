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

namespace ArsVisual.SettingsMaster
{
    /// <summary>
    /// Логика взаимодействия для SettingsMaster.xaml
    /// </summary>
    public partial class SettingsMaster : Window
    {
        public SettingsMaster()
        {
            InitializeComponent();
        }

        private void Palitra(object sender, RoutedEventArgs e)
        {
            var colorChangerPage = new pages.ColorChanger();
            colorChangerPage.ColorSelected += OnColorSelected;
            OCHKO.Navigate(colorChangerPage);
        }

        private void OnColorSelected(Color selectedColor)
        {
            // Обновляем ресурс приложения
            Application.Current.Resources["SizeChromeColor"] = new SolidColorBrush(selectedColor);
          //  Application.Current.Resources["ItemBrush"] = new SolidColorBrush(selectedColor);
            Application.Current.Resources["ItemStroke"] = new SolidColorBrush(selectedColor);
        }
    }
}
