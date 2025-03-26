using ArsVisual.pages;
using DiagramDesigner;
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
        private Window1 _window1;

        public SettingsMaster()
        {
            InitializeComponent();

            AppearanceMaster.LoadColors();

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            AppearanceMaster.SaveColors(); 
        }

        private void Palitra(object sender, RoutedEventArgs e)
        {
            var colorChangerPage = new pages.ColorChanger();
           
            colorChangerPage.ColorSizeChromeSelected += OnColorSizeChromeSelected;
           
            colorChangerPage.ColorSnapBrushSelected += OnColorSnapBrushSelected;
            OCHKO.Navigate(colorChangerPage);
        }

     
        private void OnColorSizeChromeSelected(Color selectedColor)
        {
           
            Application.Current.Resources["SizeChromeColor"] = new SolidColorBrush(selectedColor);
          
        }

      
        private void OnColorSnapBrushSelected(Color selectedColor)
        {

            Application.Current.Resources["SnapAdornerColor"] = new SolidColorBrush(selectedColor);

        }
        private void closesettings(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void Update_Selected(object sender, RoutedEventArgs e)
        {
            OCHKO.Navigate( new pages.UpdatePage());
        }
    }
}
