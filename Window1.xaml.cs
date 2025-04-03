using ArsVisual.Settings;

using AutoUpdaterDotNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArsVisual
{
    public partial class Window1 : Window
    {
        AppearanceMaster app = new();
        public static Dictionary<TabItem, DesignerCanvas> _pageCanvases = new Dictionary<TabItem, DesignerCanvas>();
        public Dictionary<TabItem, object> _pageStates = new Dictionary<TabItem, object>();
        public static TabControl MainTabControlReference { get; private set; }
      
        public Window1()
        {
            InitializeComponent();
            AppearanceMaster.LoadColors();
            AutoUpdater.Start("https://raw.githubusercontent.com/Integrity-constraint/Lazar/master/Update.xml");
          
          
            _pageCanvases[(TabItem)MainTabControl.Items[0]] = MyDesignerCanvas;

            
            MainTabControl.SelectionChanged += TabControl_SelectionChanged;
          
            this.Closing += OnClosing;
           
            MainTabControlReference = MainTabControl;
        }

       
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem == AddTabButton)
            {
              
                AddNewPage();
                MainTabControl.SelectedIndex = MainTabControl.Items.Count - 2; 
            }
            else
            {
                
                PageSwitch(sender, e);
            }
        }

        
        public void AddNewPage()
        {
            var newTabItem = new TabItem { Header = $"Страница {MainTabControl.Items.Count}" };

           
            newTabItem.HeaderTemplate = (DataTemplate)FindResource("TabHeaderTemplate");

            var grid = new Grid();
            var scrollViewer = new ScrollViewer
            {
                Background = System.Windows.Media.Brushes.Transparent,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            var designerCanvas = new DesignerCanvas
            {
                MinHeight = 800,
                MinWidth = 1000,
                AllowDrop = true,
                Background = System.Windows.Media.Brushes.White
            };

            scrollViewer.Content = designerCanvas;

            var zoomBox = new ZoomBox
            {
                Width = 180,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                ScrollViewer = scrollViewer,
                Margin = new Thickness(0, 10, 30, 0)
            };

            grid.Children.Add(scrollViewer);
            grid.Children.Add(zoomBox);

            newTabItem.Content = grid;
            MainTabControl.Items.Insert(MainTabControl.Items.Count - 1, newTabItem); // Вставляем перед вкладкой с плюсом

            _pageCanvases[newTabItem] = designerCanvas;
        }

        // Обработчик закрытия вкладки
        public void CloseTabClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TabItem tabItem)
            {
                if (_pageCanvases.ContainsKey(tabItem) && _pageCanvases[tabItem].Children.Count > 0)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show(
                        "На странице есть схема, сохранить схему?",
                        "Внимание",
                        MessageBoxButton.YesNoCancel
                    );

                    if (messageBoxResult == MessageBoxResult.No)
                    {
                      
                        MainTabControl.Items.Remove(tabItem);

                        
                        _pageCanvases.Remove(tabItem);
                        _pageStates.Remove(tabItem);
                    }
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                       
                            if (_pageCanvases.ContainsKey(tabItem))
                            {
                                var canvas = _pageCanvases[tabItem];
                                canvas.Save_Executed(null, null); 
                        }
                        
                    }
                    if(messageBoxResult == MessageBoxResult.Cancel)
                    {

                    }
                   

                    
                }
                else
                {
                   
                    MainTabControl.Items.Remove(tabItem);
                    _pageCanvases.Remove(tabItem);
                    _pageStates.Remove(tabItem);
                }
            }
        }

      
        private void PageSwitch(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem currentTab && currentTab != AddTabButton)
            {
               
                if (MainTabControl.SelectedIndex > 0)
                {
                    var previousTab = MainTabControl.Items[MainTabControl.SelectedIndex - 1] as TabItem;
                    if (previousTab != null && _pageCanvases.ContainsKey(previousTab))
                    {
                        _pageStates[previousTab] = SaveCanvasState(_pageCanvases[previousTab]);
                    }
                }

               
                if (_pageStates.ContainsKey(currentTab))
                {
                    RestoreCanvasState(_pageCanvases[currentTab], _pageStates[currentTab]);
                }
            }
        }

     
        private object SaveCanvasState(DesignerCanvas canvas)
        {
           
            return null; 
        }

      
        private void RestoreCanvasState(DesignerCanvas canvas, object state)
        {
           
        }

    
        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (_pageCanvases.Values.Any(canvas => canvas.Children.Count > 0))
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "У вас не сохранён проект, сохранить?",
                    "Внимание",
                    MessageBoxButton.YesNoCancel
                );

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    // Сохраняем проект
                    foreach (var canvas in _pageCanvases.Values)
                    {
                        canvas.Save_Executed(null, null);
                    }
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

        private void Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized
               ? WindowState.Normal
               : WindowState.Maximized;
         }

        private void Clostw(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenPopup(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = true;
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