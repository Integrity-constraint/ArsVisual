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
using System.Windows.Shapes;

namespace ArsVisual.Resources
{
    /// <summary>
    /// Логика взаимодействия для ColorPickerWindow.xaml
    /// </summary>
    public partial class ColorPickerWindow : Window
    {
       
        public Color SelectedColor { get; private set; }
        public SolidColorBrush PreviewColorBrush { get; set; } 

      
        private readonly SolidColorBrush _previewColorBrush = new SolidColorBrush();

        public ColorPickerWindow(Color initialColor)
        {
            InitializeComponent();

           
            SelectedColor = initialColor;
            _previewColorBrush.Color = initialColor;
            PreviewColorBrush = _previewColorBrush;

         
            AlphaSlider.Value = initialColor.A;
            RedSlider.Value = initialColor.R;
            GreenSlider.Value = initialColor.G;
            BlueSlider.Value = initialColor.B;

          
            UpdateTextBoxes();
            UpdatePreview();

           
            SubscribeToEvents();
        }

        
        private void SubscribeToEvents()
        {
            AlphaSlider.ValueChanged += Slider_ValueChanged;
            RedSlider.ValueChanged += Slider_ValueChanged;
            GreenSlider.ValueChanged += Slider_ValueChanged;
            BlueSlider.ValueChanged += Slider_ValueChanged;

            AlphaTextBox.TextChanged += TextBox_TextChanged;
            RedTextBox.TextChanged += TextBox_TextChanged;
            GreenTextBox.TextChanged += TextBox_TextChanged;
            BlueTextBox.TextChanged += TextBox_TextChanged;

            HexColorTextBox.TextChanged += HexTextBox_TextChanged;
        }

       
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateColorFromSliders();
            UpdateTextBoxes();
            UpdatePreview();
        }

       
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox)
            {
                if (byte.TryParse(textBox.Text, out byte value))
                {
                    if (textBox == AlphaTextBox) AlphaSlider.Value = value;
                    else if (textBox == RedTextBox) RedSlider.Value = value;
                    else if (textBox == GreenTextBox) GreenSlider.Value = value;
                    else if (textBox == BlueTextBox) BlueSlider.Value = value;
                }
            }
        }

        
        private void HexTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (HexColorTextBox.Text.Length == 7 || HexColorTextBox.Text.Length == 9)
                {
                    var color = (Color)ColorConverter.ConvertFromString(HexColorTextBox.Text);
                    AlphaSlider.Value = color.A;
                    RedSlider.Value = color.R;
                    GreenSlider.Value = color.G;
                    BlueSlider.Value = color.B;
                }
            }
            catch {  }
        }

     
        private void UpdateColorFromSliders()
        {
            SelectedColor = Color.FromArgb(
                (byte)AlphaSlider.Value,
                (byte)RedSlider.Value,
                (byte)GreenSlider.Value,
                (byte)BlueSlider.Value);
        }

      
        private void UpdateTextBoxes()
        {
            AlphaTextBox.Text = ((byte)AlphaSlider.Value).ToString();
            RedTextBox.Text = ((byte)RedSlider.Value).ToString();
            GreenTextBox.Text = ((byte)GreenSlider.Value).ToString();
            BlueTextBox.Text = ((byte)BlueSlider.Value).ToString();
            HexColorTextBox.Text = $"#{SelectedColor.A:X2}{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2}";
        }

    
        private void UpdatePreview()
        {
            var brush = new SolidColorBrush(SelectedColor);
            PreviewBorder.Background = brush;
            PreviewText.Foreground = GetContrastColor(SelectedColor);
        }

     
        private SolidColorBrush GetContrastColor(Color bgColor)
        {
            double brightness = (bgColor.R * 299 + bgColor.G * 587 + bgColor.B * 114) / 1000;
            return brightness > 128 ? Brushes.Black : Brushes.White;
        }

       
        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

      
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

      
        public static bool ShowColorPicker(out Color selectedColor, Color? initialColor = null)
        {
            var picker = new ColorPickerWindow(initialColor ?? Colors.White);
            bool result = picker.ShowDialog() ?? false;
            selectedColor = picker.SelectedColor;
            return result;
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
    }
}

