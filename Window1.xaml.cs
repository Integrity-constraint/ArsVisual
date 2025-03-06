using ArsVisual.SettingsMaster;
using System.ComponentModel;
using System.Windows;

namespace DiagramDesigner
{
    public partial class Window1 : Window
    {
       
        public Window1()
        {
            InitializeComponent();

            this.Closed += OnClosing;

        }

        private void OnClosing(object sender, EventArgs e)
        {
           
            Application.Current.Shutdown();
        }
    }
}
