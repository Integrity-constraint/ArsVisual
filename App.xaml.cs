using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ArsVisual.NotifyComponents.Not;

namespace DiagramDesigner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon ts;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ts = (TaskbarIcon)FindResource("MyNotifyIcon");
        }
    }
}
