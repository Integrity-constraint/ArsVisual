using ArsVisual.NotifyComponents.MsgBox;
using AutoUpdaterDotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Логика взаимодействия для UpdatePage.xaml
    /// </summary>
    public partial class UpdatePage : Page
    {
        public UpdatePage()
        {
            InitializeComponent();

            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            Verstext.Text = $"Версия программы: {Assembly.GetExecutingAssembly().GetName().Version.ToString()}";

          
        }

        private void CheckUpdates(object sender, RoutedEventArgs e)
        {
              AutoUpdater.Start("https://raw.githubusercontent.com/Integrity-constraint/ArsVisual/master/Update.xml");

            
        }

        public static void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {

            if (args != null) 
            {
                if (args.IsUpdateAvailable)
                {
                   
                    MessageBoxResult messageBoxResult = NotifyBox.Show(
                        $"Доступна новая версия {args.CurrentVersion}. Хотите загрузить?",
                        "Внимание",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information
                    );

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        try
                        {
                            if (AutoUpdater.DownloadUpdate(args))
                            {
                                Application.Current.Shutdown();
                            }
                        }
                        catch (Exception exception)
                        {
                            NotifyBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                   
                }
                else if (!args.IsUpdateAvailable)
                {

                    NotifyBox.Show(
                        $"У вас уже установлена последняя версия: {Assembly.GetExecutingAssembly().GetName().Version}",
                        "Информация",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
            else
            {
                NotifyBox.Show("Не удалось проверить обновления. Проверьте подключение к интернету.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
    }

