using ArsVisual.NotifyComponents.MsgBox;
using ArsVisual.Resources;
using ArsVisual.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace ArsVisual.pages
{
    /// <summary>
    /// Логика взаимодействия для ColorChanger.xaml
    /// </summary>
    public partial class ColorChanger : Page
    {

        public event Action<Color> ColorItemStrokeSelected;
        public event Action<Color> ColorSizeChromeSelected;
        public event Action<Color> ColorItemBrushSelected;
        public event Action<Color> ColorSnapBrushSelected;
        public event Action<Color> ColorGridSelected;

        public ColorChanger()
        {
            InitializeComponent();
           
        }

      



       

        private void ChangeResizeStyle(object sender, MouseButtonEventArgs e)
        {
            ChangeColor("SizeChromeColor");
        }
        private void ChangeSnapeStyle(object sender, MouseButtonEventArgs e)
        {
            ChangeColor("SnapAdornerColor");
        }
        private void ChangeGridLayer(object sender, MouseButtonEventArgs e)
        {
            ChangeColor("GridLayer");
        }

        private void ChangeColor(string resourceKey)
        {
            if (Application.Current.Resources[resourceKey] is SolidColorBrush currentBrush)
            {
               
                if (ColorPickerWindow.ShowColorPicker(out Color selectedColor, initialColor: currentBrush.Color))
                {

                   
                    var newBrush = new SolidColorBrush(selectedColor);
                    Application.Current.Resources[resourceKey] = newBrush;


                }
            }
        }

        private void RestoreColors(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = NotifyBox.Show(
                      "Сбросить цветовую схему?",
                      "Внимание",
                      MessageBoxButton.YesNo, MessageBoxImage.Warning
                  );

            if (messageBoxResult == MessageBoxResult.No)
            {

              
            }
            if (messageBoxResult == MessageBoxResult.Yes)
            {
              AppearanceMaster.RestoreDefaultColors();
               

            }
           

        }

       
    }
}
