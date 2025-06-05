using ArsVisual.Helpers;
using ArsVisual.NotifyComponents.MsgBox;
using ArsVisual.Settings;
using AutoUpdaterDotNET;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ArsVisual.Resources;
using ArsVisual.NetService;

namespace ArsVisual
{
    public partial class WorkWindow : Window, INotifyPropertyChanged
    {
        
        public static Dictionary<TabItem, DesignerCanvas> _pageCanvases = new Dictionary<TabItem, DesignerCanvas>();
        private static Dictionary<TabItem, object> _pageStates = new Dictionary<TabItem, object>();
        public static TabControl MainTabControlReference { get; private set; }

        public static RoutedCommand OpenFaq = new RoutedCommand();
        private string _fileName = "Новый проект";
         public string FileName
         {
             get => _fileName;
             set
             {
                 if (_fileName != value)
                 {
                     _fileName = value;
                     OnPropertyChanged(nameof(FileName)); 
                 }
             }
         }
        public event PropertyChangedEventHandler PropertyChanged;

         protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
         {
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
         }
        public static void SavePageState(TabItem tabItem, object state)
        {
            _pageStates[tabItem] = state;
        }

        public static Dictionary<TabItem, object> GetPageStates()
        {
            return _pageStates;
        }
        public WorkWindow()
        {
            InitializeComponent();
            AutoUpdater.HttpUserAgent = "AutoUpdater";
           
            AutoUpdater.Start("https://raw.githubusercontent.com/Integrity-constraint/ArsVisual/master/Update.xml");
            AppearanceMaster.LoadColors();
            OpenFaq.InputGestures.Add(new KeyGesture(Key.F1));
            DataContext = this;
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

            if (MainTabControl.Items.Count == 0)
            {
                MainTabControl.Items.Add(newTabItem); 
            }
            else
            {
                MainTabControl.Items.Insert(MainTabControl.Items.Count - 1, newTabItem); 
            }

      
            _pageCanvases[newTabItem] = designerCanvas;

        
            MainTabControl.SelectedItem = newTabItem;
        }


        public void CloseTabClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TabItem tabItem)
            {
                if (_pageCanvases.ContainsKey(tabItem) && _pageCanvases[tabItem].Children.Count > 0)
                {
                    MessageBoxResult messageBoxResult = NotifyBox.Show(
                        "На странице есть схема, закрыть?",
                        "Внимание",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning
                    );

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                      
                        MainTabControl.Items.Remove(tabItem);

                        
                        _pageCanvases.Remove(tabItem);
                        _pageStates.Remove(tabItem);
                    }
                    if (messageBoxResult == MessageBoxResult.No)
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
            if (e.RemovedItems.Count > 0 && e.RemovedItems[0] is TabItem previousTab)
            {
                if (_pageCanvases.TryGetValue(previousTab, out var prevCanvas))
                {
                    _pageStates[previousTab] = SaveCanvasState(prevCanvas);
                }
            }

            if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabItem currentTab && currentTab != AddTabButton)
            {
                if (_pageCanvases.TryGetValue(currentTab, out var currentCanvas))
                {
                    if (_pageStates.TryGetValue(currentTab, out var state))
                    {
                        RestoreCanvasState(currentCanvas, state);
                    }
                }
            }
        }


        public static object SaveCanvasState(DesignerCanvas canvas)
        {
            var state = new CanvasState
            {
                Items = canvas.Children.OfType<DesignerItem>().ToList(),
                Connections = canvas.Children.OfType<Connection>()
             .Where(c => c.Source != null && c.Sink != null)
             .Select(c => new ConnectionInfo
             {
                 SourceId = c.Source.ParentDesignerItem.ID,
                 SinkId = c.Sink.ParentDesignerItem.ID,
                 SourceConnectorName = c.Source.Name,
                 SinkConnectorName = c.Sink.Name,
                 SourceArrowSymbol = c.SourceArrowSymbol,
                 SinkArrowSymbol = c.SinkArrowSymbol,
                 StrokeDashArray = c.StrokeDashArray,
                 Text = c.Text,
                 ZIndex = Canvas.GetZIndex(c),
                 ConnectionLineType = c._ConnectionLineType 
             }).ToList()
            };
            return state;
        }

        private void RestoreCanvasState(DesignerCanvas canvas, object state)
        {
            if (state == null) return;

            var canvasState = state as CanvasState;
            if (canvasState == null) return;

            var itemsDict = canvas.Children.OfType<DesignerItem>().ToDictionary(item => item.ID);

           
            foreach (var item in canvasState.Items)
            {
                if (!itemsDict.ContainsKey(item.ID))
                {
                    canvas.Children.Add(item);
                    SetConnectorDecoratorTemplate(item);
                }
            }

            
            itemsDict = canvas.Children.OfType<DesignerItem>().ToDictionary(item => item.ID);

           
            var currentConnections = canvas.Children.OfType<Connection>().ToList();
            var connectionsToKeep = new List<Connection>();

            foreach (ConnectionInfo connInfo in canvasState.Connections)
            {
                if (itemsDict.TryGetValue(connInfo.SourceId, out var sourceItem) &&
                    itemsDict.TryGetValue(connInfo.SinkId, out var sinkItem))
                {
                    var sourceConn = FindConnectorInItem(sourceItem, connInfo.SourceConnectorName);
                    var sinkConn = FindConnectorInItem(sinkItem, connInfo.SinkConnectorName);

                    if (sourceConn != null && sinkConn != null)
                    {
                        var existingConn = currentConnections.FirstOrDefault(c =>
                            c.Source?.ParentDesignerItem?.ID == connInfo.SourceId &&
                            c.Sink?.ParentDesignerItem?.ID == connInfo.SinkId &&
                            c.Source?.Name == connInfo.SourceConnectorName &&
                            c.Sink?.Name == connInfo.SinkConnectorName);

                        if (existingConn != null)
                        {
                            
                            existingConn.SourceArrowSymbol = connInfo.SourceArrowSymbol;
                            existingConn.SinkArrowSymbol = connInfo.SinkArrowSymbol;
                            existingConn.StrokeDashArray = connInfo.StrokeDashArray;
                            existingConn.Text = connInfo.Text;
                            existingConn._ConnectionLineType = connInfo.ConnectionLineType;            
                            Canvas.SetZIndex(existingConn, connInfo.ZIndex);
                            existingConn.UpdatePathGeometry();
                            connectionsToKeep.Add(existingConn);
                        }
                        else
                        {
                            
                            var connection = new Connection(sourceConn, sinkConn)
                            {
                                SourceArrowSymbol = connInfo.SourceArrowSymbol,
                                SinkArrowSymbol = connInfo.SinkArrowSymbol,
                                StrokeDashArray = connInfo.StrokeDashArray,
                                Text = connInfo.Text,
                                _ConnectionLineType = connInfo.ConnectionLineType 
                            };
                            Canvas.SetZIndex(connection, connInfo.ZIndex);
                            canvas.Children.Add(connection);
                            connectionsToKeep.Add(connection);
                        }
                    }
                }
            }

            
            foreach (var conn in currentConnections.Except(connectionsToKeep))
            {
                canvas.Children.Remove(conn);
            }
        }
       
        public static Connector FindConnectorInItem(DesignerItem item, string connectorName)
        {
            if (item == null) return null;

           
            item.ApplyTemplate();
            item.UpdateLayout();

           
            var decorator = item.Template?.FindName("PART_ConnectorDecorator", item) as Control;
            if (decorator == null) return null;

            decorator.ApplyTemplate();
            decorator.UpdateLayout();

        
            return decorator.Template?.FindName(connectorName, decorator) as Connector;
        }

        private void SetConnectorDecoratorTemplate(DesignerItem item)
        {
            if (item.ApplyTemplate() && item.Content is UIElement content)
            {
                ControlTemplate template = DesignerItem.GetConnectorDecoratorTemplate(content);
                Control decorator = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
                if (decorator != null && template != null)
                {
                    decorator.Template = template;
                    decorator.ApplyTemplate();
                }
            }
        }



        private async void OnClosing(object sender, CancelEventArgs e)
        {
            if (_pageCanvases.Values.Any(canvas => canvas.Children.Count > 0))
            {
                MessageBoxResult messageBoxResult = NotifyBox.Show(
                    "У вас не сохранён проект, сохранить?",
                    "Внимание",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    e.Cancel = true;

                    var saveTasks = _pageCanvases.Values
                        .Where(c => c.Children.Count > 0)
                        .Select(c => Task.Run(() =>
                            Dispatcher.Invoke(() => c.Save_Executed(null, null))));

                    await Task.WhenAll(saveTasks);
                    Application.Current.Shutdown();
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
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
     
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            }
        }

        private void Clostw(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenPopup(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = true;
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void OpenFaqExecuted(object sender, ExecutedRoutedEventArgs e)
        {
          
            SettingsMaster settings = new SettingsMaster();

            if(settings.IsActive == true)
            {
                settings.Focus();
            }
            if (settings.IsActive == false) 
            {
                settings.Show();
                settings.Frame.NavigationService.Navigate(new pages.InfoPage());
            }

        }

        private void GetFilesFromCloud(object sender, RoutedEventArgs e)
        {
            DigitalHiveGetFilesLogin login = new DigitalHiveGetFilesLogin();

            login.Show();
        }
    }
}