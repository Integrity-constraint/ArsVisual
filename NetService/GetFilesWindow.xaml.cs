using ArsVisual.NotifyComponents.MsgBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArsVisual.NetService
{
    /// <summary>
    /// Логика взаимодействия для GetFilesWindow.xaml
    /// </summary>
    public partial class GetFilesWindow : Window
    {
        public List<FileInfo> Files { get; set; }
        public string Filepath { get; set; } 
        private static string DefaultPath { get; set; } 

        public GetFilesWindow(List<FileInfo> filesList)
        {
            InitializeComponent();
            Files = filesList;
            FileList.ItemsSource = Files;
            FileList.Items.Refresh();
            DataContext = this;
         
            if (string.IsNullOrEmpty(DefaultPath))
            {
                DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            Filepath = DefaultPath;
            pathcurrent.Text = Filepath;
        }

        private  async void loadfile()
        {
            if (FileList.SelectedItem != null)
            {
                FileInfo selectedfile = FileList.SelectedItem as FileInfo;
                string id = selectedfile.idApi;
                string name = selectedfile.fileName;


                string filePath = await FileLoader.GetFileFromCloud(id, Filepath);

                if (!string.IsNullOrEmpty(filePath))
                {
                    DesignerCanvas canvas = new DesignerCanvas();
                    bool cloudload = true;

                    if (CheckToLoad.IsChecked == true)
                    {
                        canvas.Open_FromCloud(null, null, filePath);
                    }

                }
            }
            else
            {
                NotifyBox.Show("Файл не выбран", "Ошибка", MessageBoxButton.OK);
            }
        }
        private async void LoadSelectedFile(object sender, MouseButtonEventArgs e)
        {
          loadfile();
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ChoosePath(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите папку для сохранения файлов";
                dialog.SelectedPath = Filepath; 

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Filepath = dialog.SelectedPath;
                    DefaultPath = Filepath;

                   pathcurrent.Text = Filepath;


                }
            }
        }

        private void Clostw(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

       async private void DeleteFile(object sender, RoutedEventArgs e)
        {
          
            var userData = UserData.GetInstance();
            var selecteditem = FileList.SelectedItem as FileInfo;
            if (selecteditem != null)
            {
                MessageBoxResult result = NotifyBox.Show("Вы хотите удалить файл?", "Внимание", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {

                    FileRemover.DeleteFileAsync(selecteditem.idApi, userData.Email, userData.Password);
                  FileList.Items.Refresh();
                    FileList.ItemsSource = Files;
                }
                if (result == MessageBoxResult.No) { }
            }
            else 
            {
                NotifyBox.Show("Файл не выбран", "Ошибка", MessageBoxButton.OK);
            }
          
           
          
        }

        private async void LoadFilebtn(object sender, RoutedEventArgs e)
        {
            loadfile();
        }
    }
}
