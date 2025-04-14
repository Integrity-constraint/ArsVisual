using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ArsVisual;

namespace ArsVisual.NotifyComponents.MsgBox
{
   public partial class NotifyBox: Window
    {
     
        public NotifyBox()
        {
            InitializeComponent();
           
        }

        void AddButtons(MessageBoxButton buttons)
        {
            switch (buttons)
            {
                case MessageBoxButton.OK:
                    AddButton("OK", MessageBoxResult.OK);
                    break;
                case MessageBoxButton.OKCancel:
                    AddButton("OK", MessageBoxResult.OK);
                    AddButton("Отмена", MessageBoxResult.Cancel, isCancel: true);
                    break;
                case MessageBoxButton.YesNo:
                    AddButton("Да", MessageBoxResult.Yes);
                    AddButton("Нет", MessageBoxResult.No);
                    break;
                case MessageBoxButton.YesNoCancel:
                    AddButton("Да", MessageBoxResult.Yes);
                    AddButton("Нет", MessageBoxResult.No);
                    AddButton("Отмена", MessageBoxResult.Cancel, isCancel: true);
                    break;
                default:
                    throw new ArgumentException("Неизвестное значение", "Кнопучки");
            }
        }
        void AddImages(MessageBoxImage images)
        {
            switch (images)
            {
                case MessageBoxImage.Information:
                    AddImage("pack://application:,,,/icons/info_icon.png");
                    break;
                case MessageBoxImage.Error:
                    AddImage("pack://application:,,,/icons/critical_icon.png");
                    break;
                case MessageBoxImage.Warning:
                    AddImage("pack://application:,,,/icons/attention_icon.png");
                    break;
                case MessageBoxImage.Question:
                    AddImage("pack://application:,,,/icons/exception_icon.png");
                    break;
                default:
                    throw new ArgumentException("Неизвестное значение", "Кнопучки");
            }
        }
        void AddButton(string text, MessageBoxResult result, bool isCancel = false)
        {
            var button = new Button() { Content = text, IsCancel = isCancel };
            button.Click += (o, args) => { Result = result; DialogResult = true; };
            ButtonContainer.Children.Add(button);
        }
        void AddImage(string path)
        {
            ImageControl.Source = new BitmapImage(new Uri($"{path}", UriKind.RelativeOrAbsolute));
        }
        MessageBoxResult Result = MessageBoxResult.None;

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton buttons)
        {
            var dialog = new NotifyBox()
            {
                Title = caption,  
            };

            dialog.MessageContainer.Text = message;  
           
            dialog.AddButtons(buttons);
            dialog.ShowDialog();

            return dialog.Result;
        }
        public static MessageBoxResult Show(string message, string caption, MessageBoxButton buttons, MessageBoxImage Image)
        {
            var dialog = new NotifyBox()
            {
                Title = caption,
            };

            dialog.MessageContainer.Text = message;
            dialog.AddImages(Image);
            dialog.AddButtons(buttons);
            dialog.ShowDialog();

            return dialog.Result;
        }
        private void Drag(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
