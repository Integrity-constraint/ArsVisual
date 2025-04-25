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
using System.Xml.Linq;
using ArsVisual.NetService;
using ArsVisual.NotifyComponents.MsgBox;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Windows.Controls.Primitives;
using System.Security.RightsManagement;
using System.Security;
using System.Runtime.InteropServices;



namespace ArsVisual.NetService
{
    /// <summary>
    /// Логика взаимодействия для DigitalHiveLoginForm.xaml
    /// </summary>
    public partial class DigitalHiveLoginForm : Window
    {
        FileUploader uploader = new FileUploader();
       
        private XElement XElement { get; set; }

        public DigitalHiveLoginForm(XElement xElement)
        {
            InitializeComponent();
            XElement = xElement;
        }

        private void CheckTextboxOutline(TextBox Text)
        {
            Text.BorderBrush = new SolidColorBrush(Colors.Red);
        }

        async public void SaveToCloud(object sender, RoutedEventArgs e)
        {
           
            string  email = EmailBox.Text;
            string fileName = NameBox.Text;
            string Password = SecureStringToString(PassBox.SecurePassword);



            if (string.IsNullOrEmpty(email))
            {
                EmailBox.Text = "Имя файла пустое";
                EmailBox.Foreground = new SolidColorBrush(Colors.Red);
                NotifyBox.Show("Почта не указана", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                CheckTextboxOutline(EmailBox);

            }
            if (string.IsNullOrEmpty(fileName))
            {
                NameBox.Text = "Имя файла пустое";
                NameBox.Foreground = new SolidColorBrush(Colors.Red);
             
                CheckTextboxOutline(NameBox);
            }
            if (string.IsNullOrEmpty(fileName))
            {              
                PassBox.Foreground = new SolidColorBrush(Colors.Red);               
            }
            else 
            {
                try
                {
                    string defaultPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{fileName}");
                    XElement.Save(defaultPath);
                    uploader.UploadFileAsync(defaultPath, email, Password);
                }
                catch(Exception ex) 
                {
                    NotifyBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {

                    App.NotifyUser(fileName, "Проект Сохранён в облаке", "cloud-computing.gif", 4000, PopupAnimation.Slide);
                }
           
            }
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string SecureStringToString(SecureString secureString)
        {
            if (secureString == null)
                return string.Empty;

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }
    }
}
