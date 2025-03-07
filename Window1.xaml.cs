using ArsVisual.SettingsMaster;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace DiagramDesigner
{
    public partial class Window1 : Window
    {
        private DesignerCanvas canvas;

        public Window1()
        {
            InitializeComponent();

            
            canvas = this.FindName("MyDesignerCanvas") as DesignerCanvas;

           
            this.Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (canvas != null && canvas.Children.Count > 0)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "У вас не сохранён проект, сохранить?",
                    "Внимание",
                    MessageBoxButton.YesNo
                );

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                   
                    object fakeSender = null;
                    ExecutedRoutedEventArgs fakeEventArgs = null;

                   
                    canvas.Save_Executed(fakeSender, fakeEventArgs);

                  
                    e.Cancel = true;
                }
                else if (messageBoxResult == MessageBoxResult.No)
                {
                    
                    Application.Current.Shutdown();
                }
                else
                {
                   
                    e.Cancel = true;
                }
            }
        }
    }
}
