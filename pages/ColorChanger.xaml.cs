using ArsVisual.Resources;
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

        public ColorChanger()
        {
            InitializeComponent();
            LoadColors();
        }

        private void LoadColors()
        {
            var colors = new List<Color>
        {
            Colors.Red,
            Colors.Green,
            Colors.Blue,
            Colors.Yellow,
            Colors.Orange,
            Colors.Purple,
            Colors.White,
            Colors.Black
        };

         

            
        }



        private void SetCurrentColor(ComboBox comboBox, string resourceKey)
        {
            if (Application.Current.Resources[resourceKey] is SolidColorBrush brush)
            {
                comboBox.SelectedItem = brush.Color;
            }
        }

       

        private void ChangeResizeStyle(object sender, MouseButtonEventArgs e)
        {
            ChangeColor("SizeChromeColor");
        }
        private void ChangeSnapeStyle(object sender, MouseButtonEventArgs e)
        {
            ChangeColor("SnapAdornerColor");
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
      
    }
}
