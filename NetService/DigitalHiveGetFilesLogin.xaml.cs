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
using System.Diagnostics;
using static System.Windows.Forms.DataFormats;



namespace ArsVisual.NetService
{
    /// <summary>
    /// Логика взаимодействия для DigitalHiveLoginForm.xaml
    /// </summary>
    public partial class DigitalHiveGetFilesLogin : Window
    {
        

        public DigitalHiveGetFilesLogin()
        {
            InitializeComponent();
            var userData = UserData.GetInstance();
            if (userData != null && userData.IsInitialized)
            {
                EmailBox.Text = userData.Email;
             
               LoginGetFilesAsync(userData.Email, userData.Password);
            }

        }

        private void CheckTextboxOutline(TextBox Text)
        {
            Text.BorderBrush = new SolidColorBrush(Colors.Red);
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


         private void HyperlinkDigitalHive_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
         {
             Process.Start(new ProcessStartInfo
             {
                 FileName = e.Uri.AbsoluteUri,
                 UseShellExecute = true
             });
             e.Handled = true;
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
        private async void LoginGetFilesAsync(string email, string password)
        {

            try
            {
                string format = null;
                List<FileInfo> files = await FileInfo.GetFilesByFormatAsync(email, password, format);

                if (files != null)
                {
                    GetFilesWindow fileWindow = new GetFilesWindow(files);
                    this.Close();
                    fileWindow.Show();
                }
              
            }
            catch (Exception ex)
            {
                NotifyBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void LoginGetFiles(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text;
            string password = SecureStringToString(PassBox.SecurePassword);

            if (string.IsNullOrEmpty(email))
            {
                EmailBox.Text = "Почта не указана";
                EmailBox.Foreground = new SolidColorBrush(Colors.Red);
                NotifyBox.Show("Почта не указана", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                CheckTextboxOutline(EmailBox);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                PassBox.Foreground = new SolidColorBrush(Colors.Red);
                NotifyBox.Show("Пароль не указан", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

           
           // UserData.GetInstance(password, email);
            LoginGetFilesAsync(email, password);
        }
    }
}
