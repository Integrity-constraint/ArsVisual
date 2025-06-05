using ArsVisual.NotifyComponents.MsgBox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArsVisual.Settings
{
    public class AppearanceMaster
    {
      
        private const string ConfigFilePath = "ViewConfigs.txt";

        public static void SaveColors()
        {
            var colorsToSave = new Dictionary<string, string>
        {
            {"SnapAdornerColor", ((SolidColorBrush)Application.Current.Resources["SnapAdornerColor"]).Color.ToString()},
          
            {"SizeChromeColor", ((SolidColorBrush)Application.Current.Resources["SizeChromeColor"]).Color.ToString()},
            {"GridLayer", ((SolidColorBrush)Application.Current.Resources["GridLayer"]).Color.ToString()},
          
        };

            File.WriteAllLines(ConfigFilePath,
                colorsToSave.Select(kv => $"{kv.Key}={kv.Value}"));
        }

        public static void LoadColors()
        {
            if (!File.Exists(ConfigFilePath)) return;

            try
            {
                var lines = File.ReadAllLines(ConfigFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        var colorName = parts[0];
                        var colorValue = parts[1];

                        if (ColorConverter.ConvertFromString(colorValue) is Color color)
                        {
                            Application.Current.Resources[colorName] = new SolidColorBrush(color);
                        }
                    }
                }
            }
            catch(Exception ex) 
            {
                NotifyBox.Show($"Ошибка загрузки цветовых схем: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
              

                RestoreDefaultColors();
            }
          
        }

        public static void RestoreDefaultColors()
        {
            
            
                Application.Current.Resources["SnapAdornerColor"] = new SolidColorBrush(Colors.Red);
                Application.Current.Resources["SizeChromeColor"] = new SolidColorBrush(Colors.Blue);
                Application.Current.Resources["GridLayer"] = new SolidColorBrush(Colors.LightBlue);
            
          
           
        }
    }
}
