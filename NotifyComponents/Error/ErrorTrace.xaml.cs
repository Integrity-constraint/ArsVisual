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

namespace ArsVisual.NotifyComponents.Error
{
    /// <summary>
    /// Логика взаимодействия для ErrorTrace.xaml
    /// </summary>
    public partial class ErrorTrace : Window
    {
       
        public ErrorTrace(string ex)
        {
            InitializeComponent();
            

            errortext.Text = ex.ToString();
        }

        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }





    }
}
