using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ArsVisual.NotifyComponents.Not;
using System.Reflection;
using ArsVisual.NotifyComponents.MsgBox;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace ArsVisual 

{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static TaskbarIcon ts;
      
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ts = (TaskbarIcon)FindResource("MyNotifyIcon");

           
        }

        private void CloseTabClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is WorkWindow mainWindow)
            {
                mainWindow.CloseTabClick(sender, e);
            }
        }


       public static void NotifyUser(string balloontext, string header, string imgtext, int time, PopupAnimation popup)
        {
            try
            {
                MessageSave messageSave = new MessageSave();
                messageSave.BalloonText = balloontext;
                messageSave.Iconmsg = new BitmapImage(new Uri($"pack://application:,,,/icons/{imgtext}"));
                messageSave.Headersave = header;
                App.ts.ShowCustomBalloon(messageSave, popup, time);
            }
            catch (Exception ex)
            {
                NotifyBox.Show(ex.Message, ex.StackTrace, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
