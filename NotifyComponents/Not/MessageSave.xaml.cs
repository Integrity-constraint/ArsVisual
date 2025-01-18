using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArsVisual.NotifyComponents.Not
{
   public partial class MessageSave: UserControl
    {
        private bool isClosing = false;

        #region BalloonText dependency property

        /// <summary>
        /// Description
        /// </summary>
        public static readonly DependencyProperty BalloonTextProperty =
            DependencyProperty.Register(nameof(BalloonText),
                typeof(string),
                typeof(MessageSave),
                new FrameworkPropertyMetadata(string.Empty));

        public static readonly DependencyProperty BalloonHeader =
             DependencyProperty.Register(nameof(Headersave),
                 typeof(string),
                 typeof(MessageSave),
                 new FrameworkPropertyMetadata(string.Empty));

        public static readonly DependencyProperty IconmsgProperty =
          DependencyProperty.Register(nameof(Iconmsg),
              typeof(ImageSource),
              typeof(MessageSave),
              new FrameworkPropertyMetadata(null));

        public ImageSource Iconmsg
        {
            get { return (ImageSource)GetValue(IconmsgProperty); }
            set { SetValue(IconmsgProperty, value); }
        }

        public string BalloonText
        {
            get { return (string)GetValue(BalloonTextProperty); }
            set { SetValue(BalloonTextProperty, value); }
        }
        public string Headersave
        {
            get { return (string)GetValue(BalloonHeader); }
            set { SetValue(BalloonHeader, value); }
        }
        #endregion

        public MessageSave()
        {
            InitializeComponent();
            TaskbarIcon.AddBalloonClosingHandler(this, OnBalloonClosing);
        }


       
        private void OnBalloonClosing(object sender, RoutedEventArgs e)
        {
            e.Handled = true; 
            isClosing = true;
        }


        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }

      
        private void grid_MouseEnter(object sender, MouseEventArgs e)
        {
          
            if (isClosing) return;

         
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.ResetBalloonCloseTimer();
        }


        private void OnFadeOutCompleted(object sender, EventArgs e)
        {
            Popup pp = (Popup)Parent;
            pp.IsOpen = false;
        }
    }
}
