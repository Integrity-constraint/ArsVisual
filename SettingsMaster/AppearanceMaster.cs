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
using Path = System.IO.Path;

namespace ArsVisual.SettingsMaster
{
    public class AppearanceMaster
    {
        private const string ConfigFilePath = "ViewConfigs.txt";
        private static readonly HashSet<string> ValidColorKeys = new HashSet<string>
    {
        "SnapAdornerColor",
        "SizeChromeColor" 
       
    };

        public static void SaveColors()
        {
            try
            {
                var colorsToSave = new Dictionary<string, string>();

                foreach (var key in ValidColorKeys)
                {
                    if (Application.Current.Resources[key] is SolidColorBrush brush)
                    {
                        colorsToSave[key] = brush.Color.ToString();
                    }
                }

                var tempFilePath = Path.GetTempFileName();
                File.WriteAllLines(tempFilePath, colorsToSave.Select(kv => $"{kv.Key}={kv.Value}"));

               
                File.Replace(tempFilePath, ConfigFilePath, null);
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Ошибка сохранения цветов: {ex.Message}");
            }
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
                        var colorName = parts[0].Trim();
                        var colorValue = parts[1].Trim();

                     
                        if (!ValidColorKeys.Contains(colorName))
                        {
                            MessageBox.Show($"Неизвестный ключ цвета: {colorName}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            continue;
                        }

                        try
                        {
                            if (ColorConverter.ConvertFromString(colorValue) is Color color)
                            {
                                Application.Current.Resources[colorName] = new SolidColorBrush(color);
                            }
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show($"Некорректный формат цвета: {colorValue} для ключа {colorName}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки цветов: {ex.Message}","Ошибка",MessageBoxButton.OK, MessageBoxImage.Error);
               
                RestoreDefaultColors();
            }
        }

        private static void RestoreDefaultColors()
        {
           
            Application.Current.Resources["SnapAdornerColor"] = new SolidColorBrush(Colors.Red);
            Application.Current.Resources["SizeChromeColor"] = new SolidColorBrush(Colors.Blue);
           
        }
    }
}
