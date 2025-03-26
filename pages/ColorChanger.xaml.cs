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

            ColorSizeChromeComboBox.ItemsSource = colors;
            ColorSnapBrushComboBox.ItemsSource = colors;
           

            // Установка текущих значений из ресурсов
            SetCurrentColor(ColorSizeChromeComboBox, "SizeChromeColor");
            SetCurrentColor(ColorSnapBrushComboBox, "SnapAdornerColor");
           
        }



        private void SetCurrentColor(ComboBox comboBox, string resourceKey)
        {
            if (Application.Current.Resources[resourceKey] is SolidColorBrush brush)
            {
                comboBox.SelectedItem = brush.Color;
            }
        }

        private void ColorComboBoxSizeChrome_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorSizeChromeComboBox.SelectedItem is Color selectedColor)
            {

                ColorSizeChromeSelected?.Invoke(selectedColor);
            }

        }


        private void ColorSnapBrushComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorSnapBrushComboBox.SelectedItem is Color selectedColor)
            {

                ColorSnapBrushSelected?.Invoke(selectedColor);
            }
        }
    }
}
