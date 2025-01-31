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
        public event Action<Color> ColorSelected;

        public ColorChanger()
        {
            InitializeComponent();
            LoadColors();
        }

        private void LoadColors()
        {
            // Добавляем предопределённые цвета
            var colors = new List<Color>
            {
                Colors.Red,
                Colors.Green,
                Colors.Blue,
                Colors.Yellow,
                Colors.Orange,
                Colors.Purple
            };

            ColorComboBox.ItemsSource = colors;
            ColorItemComboBox.ItemsSource = colors;
            ColorItemOutlineComboBox.ItemsSource = colors;
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorComboBox.SelectedItem is Color selectedColor)
            {
                // Вызываем событие при выборе цвета
                ColorSelected?.Invoke(selectedColor);
            }
        }

        private void ColorComboBoxItemOutline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorItemOutlineComboBox.SelectedItem is Color selectedColor)
            {
                // Вызываем событие при выборе цвета
                ColorSelected?.Invoke(selectedColor);
            }
        }

        private void ColorComboBoxItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorItemComboBox.SelectedItem is Color selectedColor)
            {
                // Вызываем событие при выборе цвета
                ColorSelected?.Invoke(selectedColor);
            }
        }
    }
}
