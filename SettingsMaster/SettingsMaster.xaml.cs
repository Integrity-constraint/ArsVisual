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
            colorChangerPage.ColorItemStrokeSelected += OnColorItemStrokeSelected;
            colorChangerPage.ColorSizeChromeSelected += OnColorSizeChromeSelected;
            colorChangerPage.ColorItemBrushSelected += OnColorItemBrushSelected;
            OCHKO.Navigate(colorChangerPage);
        }

        private void OnColorItemStrokeSelected(Color selectedColor)
        {
           
            Application.Current.Resources["StrokeElement"] = new SolidColorBrush(selectedColor);
        }
        private void OnColorSizeChromeSelected(Color selectedColor)
        {
           
            Application.Current.Resources["SizeChromeColor"] = new SolidColorBrush(selectedColor);
          
        }

        private void OnColorItemBrushSelected(Color selectedColor)
        {
           
            Application.Current.Resources["FillElement"] = new SolidColorBrush(selectedColor);
           
        }
    }
}
