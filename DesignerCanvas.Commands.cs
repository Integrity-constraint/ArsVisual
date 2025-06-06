﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

using ArsVisual.Settings;
using Microsoft.Win32;

using System.Windows.Controls.Primitives;
using static ArsVisual.Connection;
using System.Windows.Threading;
using ArsVisual.NotifyComponents.MsgBox;
using ArsVisual.NetService;
using ArsVisual.Helpers;
using System.Diagnostics;




namespace ArsVisual
{
    public partial class DesignerCanvas
    {
        public static RoutedCommand Group = new RoutedCommand();
        public static RoutedCommand Ungroup = new RoutedCommand();
        public static RoutedCommand BringForward = new RoutedCommand();
        public static RoutedCommand BringToFront = new RoutedCommand();
        public static RoutedCommand SendBackward = new RoutedCommand();
        public static RoutedCommand SendToBack = new RoutedCommand();
        public static RoutedCommand AlignTop = new RoutedCommand();
        public static RoutedCommand AlignVerticalCenters = new RoutedCommand();
        public static RoutedCommand AlignBottom = new RoutedCommand();
        public static RoutedCommand AlignLeft = new RoutedCommand();
        public static RoutedCommand AlignHorizontalCenters = new RoutedCommand();
        public static RoutedCommand AlignRight = new RoutedCommand();
        public static RoutedCommand DistributeHorizontal = new RoutedCommand();
        public static RoutedCommand DistributeVertical = new RoutedCommand();
        public static RoutedCommand SelectAll = new RoutedCommand();
        public static RoutedCommand Settings = new RoutedCommand();
        public static RoutedCommand SaveTOpng = new RoutedCommand();
        public static RoutedCommand ColorChange = new RoutedCommand();
        public static RoutedCommand UndoFunck = new RoutedCommand();
        public static RoutedCommand GridToggle = new RoutedCommand();
        public static RoutedCommand GridOff = new RoutedCommand();
        private Stack<UndoState> undoStack = new Stack<UndoState>(10);
        public DesignerCanvas()
        {
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, Print_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Executed, Cut_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Executed, Copy_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Paste_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, Delete_Executed, Delete_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.Group, Group_Executed, Group_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SaveTOpng, SaveTOPNG));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.Ungroup, Ungroup_Executed, Ungroup_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.BringForward, BringForward_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.BringToFront, BringToFront_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SendBackward, SendBackward_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SendToBack, SendToBack_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignTop, AlignTop_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignVerticalCenters, AlignVerticalCenters_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignBottom, AlignBottom_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignLeft, AlignLeft_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignHorizontalCenters, AlignHorizontalCenters_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignRight, AlignRight_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.DistributeHorizontal, DistributeHorizontal_Executed, Distribute_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.DistributeVertical, DistributeVertical_Executed, Distribute_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SelectAll, SelectAll_Executed));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.Settings, opensettings));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.UndoFunck, Undo));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.GridToggle, gridToggle));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.GridOff, gridToggleOff));
                 
            this.Focusable = true;
        SelectAll.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
        GridToggle.InputGestures.Add(new KeyGesture(Key.G, ModifierKeys.Control));
       GridOff.InputGestures.Add(new KeyGesture(Key.G, ModifierKeys.Alt));
            UndoFunck.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));

            this.AllowDrop = true;
            Clipboard.Clear();
        }

        private WorkWindow _workWindow;

        private WorkWindow WorkWindow
        {
            get
            {
                if (_workWindow == null)
                {
                    _workWindow = Application.Current.Windows.OfType<WorkWindow>().FirstOrDefault();
                }
                return _workWindow;
            }
        }




        #region New Command

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Children.Clear();
            this.SelectionService.ClearSelection();

            WorkWindow.FileName = "Новый проект";

        }

        #endregion
        #region SaveToPNG Command

        private void SaveTOPNG(object sender, ExecutedRoutedEventArgs e)
        {
            var surface = this; 

          
            Transform transform = surface.LayoutTransform;
            
            surface.LayoutTransform = null;

           
            Size size = new Size(surface.ActualWidth, surface.ActualHeight);
      
            surface.Measure(size);
            surface.Arrange(new Rect(size));

          
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

          
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg";
            saveFileDialog.DefaultExt = "png";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                
                using (FileStream outStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                 
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                   
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                  
                    encoder.Save(outStream);


                }

               App.NotifyUser(saveFileDialog.FileName.ToString(), "Изображение сохранено", "collage.gif",4000, PopupAnimation.Slide );

            }

           
            surface.LayoutTransform = transform;
        }
        #endregion

        private UndoState SaveCurrentState()
        {
            var state = new UndoState();

            foreach (var child in this.Children)
            {
                if (child is DesignerItem item)
                {
                    state.Items.Add(new DesignerItemState
                    {
                        ID = item.ID,
                        Left = Canvas.GetLeft(item),
                        Top = Canvas.GetTop(item),
                        Width = item.Width,
                        Height = item.Height,
                        ZIndex = Canvas.GetZIndex(item),
                        ContentXaml = XamlWriter.Save(item.Content),
                        RotationAngle = item.RotationAngle,
                        StrokeColor = (item.Stroke as SolidColorBrush)?.Color ?? Colors.Black,
                        FillColor = (item.Fill as SolidColorBrush)?.Color ?? Colors.White,
                        FontSize = item.FontSize,
                        FontFamily = item.FontFamily.ToString(),
                        ForegroundColor = (item.Foreground as SolidColorBrush)?.Color ?? Colors.Black
                    });
                }
                else if (child is Connection connection)
                {
                    var connState = new ConnectionState
                    {
                        SourceId = connection.Source?.ParentDesignerItem?.ID ?? Guid.Empty,
                        SinkId = connection.Sink?.ParentDesignerItem?.ID ?? Guid.Empty,
                        SourceConnectorName = connection.Source?.Name,
                        SinkConnectorName = connection.Sink?.Name,
                        SourceArrowSymbol = connection.SourceArrowSymbol,
                        SinkArrowSymbol = connection.SinkArrowSymbol,
                        StrokeDashArray = connection.StrokeDashArray,
                        Text = connection.Text,
                        ZIndex = Canvas.GetZIndex(connection),
                        LineType = connection._ConnectionLineType,
                        SourceOrientation = connection.Source?.Orientation ?? ConnectorOrientation.None,
                        SinkOrientation = connection.Sink?.Orientation ?? ConnectorOrientation.None
                    };
                    state.Connections.Add(connState);

                     }
            }

         
            return state;
        }
        private void RestoreState(UndoState state)
        {
            this.Children.Clear();
            var itemDict = new Dictionary<Guid, DesignerItem>();

          
            foreach (var itemState in state.Items)
            {
                var item = new DesignerItem(itemState.ID)
                {
                    Width = itemState.Width,
                    Height = itemState.Height,
                    RotationAngle = itemState.RotationAngle,
                    Stroke = new SolidColorBrush(itemState.StrokeColor),
                    Fill = new SolidColorBrush(itemState.FillColor),
                    FontSize = itemState.FontSize,
                    FontFamily = new FontFamily(itemState.FontFamily),
                    Foreground = new SolidColorBrush(itemState.ForegroundColor)
                };

                Canvas.SetLeft(item, itemState.Left);
                Canvas.SetTop(item, itemState.Top);
                Canvas.SetZIndex(item, itemState.ZIndex);

                using (var stringReader = new StringReader(itemState.ContentXaml))
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    item.Content = XamlReader.Load(xmlReader);
                }

              
                item.ApplyTemplate();
                this.Children.Add(item);
                itemDict[itemState.ID] = item;
              
            }

         
            foreach (var connState in state.Connections)
            {
                if (itemDict.TryGetValue(connState.SourceId, out var sourceItem) &&
                    itemDict.TryGetValue(connState.SinkId, out var sinkItem))
                {
                    var sourceConnector = FindConnector(sourceItem, connState.SourceConnectorName);
                    var sinkConnector = FindConnector(sinkItem, connState.SinkConnectorName);
                    if (sourceConnector != null && sinkConnector != null)
                    {
                        sourceConnector.Orientation = connState.SourceOrientation;
                        sinkConnector.Orientation = connState.SinkOrientation;

                        var connection = new Connection(sourceConnector, sinkConnector)
                        {
                            SourceArrowSymbol = connState.SourceArrowSymbol,
                            SinkArrowSymbol = connState.SinkArrowSymbol,
                            StrokeDashArray = connState.StrokeDashArray,
                            Text = connState.Text,
                            _ConnectionLineType = connState.LineType
                        };
                        Canvas.SetZIndex(connection, connState.ZIndex);
                        this.Children.Add(connection);
                    }
                    else
                    {
                    }
                }
                else
                {
                }
            }
        }
        private Connector FindConnector(DesignerItem item, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

     
            item.ApplyTemplate();

     
            var connectorDecorator = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
            if (connectorDecorator == null)
            {
                return null;
            }

    
            connectorDecorator.ApplyTemplate();

        
            var connector = connectorDecorator.Template.FindName(name, connectorDecorator) as Connector;
            if (connector == null)
            {
                Debug.WriteLine($"Connector {name} not found in PART_ConnectorDecorator of item {item.ID}");
              
                var templateContent = connectorDecorator.Template?.LoadContent();
                if (templateContent is FrameworkElement fe)
                {
                    var names = new List<string>();
                    foreach (var child in LogicalTreeHelper.GetChildren(fe))
                    {
                        if (child is FrameworkElement childFe && !string.IsNullOrEmpty(childFe.Name))
                        {
                            names.Add(childFe.Name);
                        }
                    }
                }
            }
            else
            {
            }
            return connector;
        }
        public void SaveUndoState()
        {
            var state = SaveCurrentState();
            undoStack.Push(state);
        }

        public void Undo(object sender, ExecutedRoutedEventArgs e)
        {
            if (undoStack.Count > 0)
            {
                var state = undoStack.Pop();
                RestoreState(state);
            }
        }
        #region settings Command

        private void opensettings(object sender, ExecutedRoutedEventArgs e)
        {
            Window settings = new  SettingsMaster();
          settings.Show();
        }

        #endregion

        #region Open Command
      
        public void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            XElement root = LoadSerializedDataFromFile(out string filePath);
            if (root == null) return;

            // var window = Application.Current.Windows.OfType<WorkWindow>().FirstOrDefault();
            //  window.FileName = $"{filePath}";
            WorkWindow.FileName = $"{filePath}";
            var tabsToRemove = WorkWindow.MainTabControlReference.Items
                .OfType<TabItem>()
                .Where(tab => tab.Header?.ToString() != "+")
                .ToList();

            foreach (var tab in tabsToRemove)
            {
                WorkWindow.MainTabControlReference.Items.Remove(tab);
                WorkWindow._pageCanvases.Remove(tab);
            }

          
            foreach (XElement tabXML in root.Elements("Tab"))
            {
                var newTabItem = new TabItem
                {
                    Header = tabXML.Element("Header")?.Value,
                    HeaderTemplate = (DataTemplate)WorkWindow.FindResource("TabHeaderTemplate")
                };

                var grid = new Grid();
                var scrollViewer = new ScrollViewer
                {
                    Background = Brushes.Transparent,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                };

                var designerCanvas = new DesignerCanvas
                {
                    MinHeight = 800,
                    MinWidth = 1000,
                    AllowDrop = true,
                    Background = Brushes.White
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

              
                var addTabButton = WorkWindow.MainTabControlReference.Items
                    .OfType<TabItem>()
                    .FirstOrDefault(tab => tab.Header?.ToString() == "+");

                int insertIndex = addTabButton != null
                    ? WorkWindow.MainTabControlReference.Items.IndexOf(addTabButton)
                    : WorkWindow.MainTabControlReference.Items.Count;

                WorkWindow.MainTabControlReference.Items.Insert(insertIndex, newTabItem);
                WorkWindow._pageCanvases[newTabItem] = designerCanvas;

          
                XElement canvasContent = tabXML.Element("CanvasContent");
                if (canvasContent != null)
                {
                    var itemsDict = new Dictionary<Guid, DesignerItem>();

                   
                    foreach (XElement itemXML in canvasContent.Element("DesignerItems").Elements("DesignerItem"))
                    {
                        Guid id = new Guid(itemXML.Element("ID").Value);
                        DesignerItem item = DeserializeDesignerItem(itemXML, id, 0, 0);
                        designerCanvas.Children.Add(item);
                        itemsDict[id] = item;
                    }

                   
                    Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            foreach (XElement connectionXML in canvasContent.Element("Connections").Elements("Connection"))
                            {
                                try
                                {
                                    Guid sourceID = new Guid(connectionXML.Element("SourceID").Value);
                                    Guid sinkID = new Guid(connectionXML.Element("SinkID").Value);

                                    if (itemsDict.TryGetValue(sourceID, out var sourceItem) &&
                                        itemsDict.TryGetValue(sinkID, out var sinkItem))
                                    {
                                       
                                        SetConnectorDecoratorTemplate(sourceItem);
                                        SetConnectorDecoratorTemplate(sinkItem);
                                        sourceItem.ApplyTemplate();
                                        sinkItem.ApplyTemplate();

                                        string sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                                        string sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                                        var sourceConn = WorkWindow.FindConnectorInItem(sourceItem, sourceConnectorName);
                                        var sinkConn = WorkWindow.FindConnectorInItem(sinkItem, sinkConnectorName);

                                        if (sourceConn != null && sinkConn != null)
                                        {
                                            var connection = new Connection(sourceConn, sinkConn)
                                            {
                                                SourceArrowSymbol = (ArrowSymbol)Enum.Parse(typeof(ArrowSymbol), connectionXML.Element("SourceArrowSymbol").Value),
                                                SinkArrowSymbol = (ArrowSymbol)Enum.Parse(typeof(ArrowSymbol), connectionXML.Element("SinkArrowSymbol").Value),
                                                StrokeDashArray = StringToDoubleCollection(connectionXML.Element("StrokeDashArray").Value),
                                                Text = connectionXML.Element("Text").Value,
                                                _ConnectionLineType = (ConnectionLineType)Enum.Parse(typeof(ConnectionLineType), connectionXML.Element("ConnectionLineType").Value)
                                            };
                                            Canvas.SetZIndex(connection, int.Parse(connectionXML.Element("zIndex").Value));
                                            designerCanvas.Children.Add(connection);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    NotifyBox.Show($"Ошибка восстановления соединения", "Ошибка", MessageBoxButton.OK,MessageBoxImage.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            NotifyBox.Show($"Ошибка восстановления соединений", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }, DispatcherPriority.ContextIdle);
                }
            }

            EnsureAddTabButtonExists();
        }
        #endregion
        #region FromCloudLoad Command
        public void Open_FromCloud(object sender, ExecutedRoutedEventArgs e, string selecteditem)
        {
            XElement root = LoadSerializedDataFromCloud(out string fileName, selecteditem);
            if (root == null) return;

            // var window = Application.Current.Windows.OfType<WorkWindow>().FirstOrDefault();
            //  window.FileName = $"{filePath}";
            WorkWindow.FileName = $"{fileName}";
            var tabsToRemove = WorkWindow.MainTabControlReference.Items
                .OfType<TabItem>()
                .Where(tab => tab.Header?.ToString() != "+")
                .ToList();

            foreach (var tab in tabsToRemove)
            {
                WorkWindow.MainTabControlReference.Items.Remove(tab);
                WorkWindow._pageCanvases.Remove(tab);
            }


            foreach (XElement tabXML in root.Elements("Tab"))
            {
                var newTabItem = new TabItem
                {
                    Header = tabXML.Element("Header")?.Value,
                    HeaderTemplate = (DataTemplate)WorkWindow.FindResource("TabHeaderTemplate")
                };

                var grid = new Grid();
                var scrollViewer = new ScrollViewer
                {
                    Background = Brushes.Transparent,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                };

                var designerCanvas = new DesignerCanvas
                {
                    MinHeight = 800,
                    MinWidth = 1000,
                    AllowDrop = true,
                    Background = Brushes.White
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


                var addTabButton = WorkWindow.MainTabControlReference.Items
                    .OfType<TabItem>()
                    .FirstOrDefault(tab => tab.Header?.ToString() == "+");

                int insertIndex = addTabButton != null
                    ? WorkWindow.MainTabControlReference.Items.IndexOf(addTabButton)
                    : WorkWindow.MainTabControlReference.Items.Count;

                WorkWindow.MainTabControlReference.Items.Insert(insertIndex, newTabItem);
                WorkWindow._pageCanvases[newTabItem] = designerCanvas;


                XElement canvasContent = tabXML.Element("CanvasContent");
                if (canvasContent != null)
                {
                    var itemsDict = new Dictionary<Guid, DesignerItem>();


                    foreach (XElement itemXML in canvasContent.Element("DesignerItems").Elements("DesignerItem"))
                    {
                        Guid id = new Guid(itemXML.Element("ID").Value);
                        DesignerItem item = DeserializeDesignerItem(itemXML, id, 0, 0);
                        designerCanvas.Children.Add(item);
                        itemsDict[id] = item;
                    }


                    Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            foreach (XElement connectionXML in canvasContent.Element("Connections").Elements("Connection"))
                            {
                                try
                                {
                                    Guid sourceID = new Guid(connectionXML.Element("SourceID").Value);
                                    Guid sinkID = new Guid(connectionXML.Element("SinkID").Value);

                                    if (itemsDict.TryGetValue(sourceID, out var sourceItem) &&
                                        itemsDict.TryGetValue(sinkID, out var sinkItem))
                                    {

                                        SetConnectorDecoratorTemplate(sourceItem);
                                        SetConnectorDecoratorTemplate(sinkItem);
                                        sourceItem.ApplyTemplate();
                                        sinkItem.ApplyTemplate();

                                        string sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                                        string sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                                        var sourceConn = WorkWindow.FindConnectorInItem(sourceItem, sourceConnectorName);
                                        var sinkConn = WorkWindow.FindConnectorInItem(sinkItem, sinkConnectorName);

                                        if (sourceConn != null && sinkConn != null)
                                        {
                                            var connection = new Connection(sourceConn, sinkConn)
                                            {
                                                SourceArrowSymbol = (ArrowSymbol)Enum.Parse(typeof(ArrowSymbol), connectionXML.Element("SourceArrowSymbol").Value),
                                                SinkArrowSymbol = (ArrowSymbol)Enum.Parse(typeof(ArrowSymbol), connectionXML.Element("SinkArrowSymbol").Value),
                                                StrokeDashArray = StringToDoubleCollection(connectionXML.Element("StrokeDashArray").Value),
                                                Text = connectionXML.Element("Text").Value,
                                                _ConnectionLineType = (ConnectionLineType)Enum.Parse(typeof(ConnectionLineType), connectionXML.Element("ConnectionLineType").Value)
                                            };
                                            Canvas.SetZIndex(connection, int.Parse(connectionXML.Element("zIndex").Value));
                                            designerCanvas.Children.Add(connection);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    NotifyBox.Show($"Ошибка восстановления соединения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            NotifyBox.Show($"Ошибка восстановления соединений", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }, DispatcherPriority.ContextIdle);
                }
            }

            EnsureAddTabButtonExists();
        }
        #endregion
   private void EnsureAddTabButtonExists()
{
    var addTabButton = WorkWindow.MainTabControlReference.Items
        .OfType<TabItem>()
        .FirstOrDefault(tab => tab.Header?.ToString() == "+");

    if (addTabButton == null)
    {
        
        addTabButton = new TabItem
        {
            Header = "+",
            HeaderTemplate = (DataTemplate)this.FindResource("TabHeaderTemplate")
        };

        WorkWindow.MainTabControlReference.Items.Add(addTabButton);
    }
}


      

        #region Save Command

        public void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            XElement root = new XElement("Root");

            foreach (var tabItem in WorkWindow._pageCanvases.Keys)
            {

                tabItem.IsSelected = true;
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));

                var canvas = WorkWindow._pageCanvases[tabItem];
                var designerItems = canvas.Children.OfType<DesignerItem>().ToList();
                var connections = canvas.Children.OfType<Connection>().ToList();

                XElement designerItemsXML = SerializeDesignerItems(designerItems);
                XElement connectionsXML = SerializeConnections(connections);
                XElement tabContent = new XElement("Tab",
                     new XElement("Header", tabItem.Header.ToString()),
                     new XElement("CanvasContent",
                         designerItemsXML,
                         connectionsXML)
                 );

                root.Add(tabContent);
            }
            
            MessageBoxResult messageBoxResult =  NotifyBox.Show("Сохранить проект локально \n локально - да \n В облаке - нет", "Внимание", MessageBoxButton.YesNoCancel);
            if (messageBoxResult == MessageBoxResult.Yes)
            {

                SaveFile(root);

            }
            else if (messageBoxResult == MessageBoxResult.No)
            {
               
                SaveFileToCloud(root);
            }
            else
            {
             
            }

           


        }


        #endregion

        #region Print Command

        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionService.ClearSelection();

            PrintDialog printDialog = new PrintDialog();

            if (true == printDialog.ShowDialog())
            {
                printDialog.PrintVisual(this, "WPF Diagram");
            }
        }

        #endregion

        #region Copy Command

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
        }

        private void Copy_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Paste Command

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            XElement root = LoadSerializedDataFromClipBoard();

            if (root == null)
                return;

          
            Dictionary<Guid, Guid> mappingOldToNewIDs = new Dictionary<Guid, Guid>();
            List<ISelectable> newItems = new List<ISelectable>();
            IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");

            double offsetX = Double.Parse(root.Attribute("OffsetX").Value, CultureInfo.InvariantCulture);
            double offsetY = Double.Parse(root.Attribute("OffsetY").Value, CultureInfo.InvariantCulture);

            foreach (XElement itemXML in itemsXML)
            {
                Guid oldID = new Guid(itemXML.Element("ID").Value);
                Guid newID = Guid.NewGuid();
                mappingOldToNewIDs.Add(oldID, newID);
                DesignerItem item = DeserializeDesignerItem(itemXML, newID, offsetX, offsetY);
                this.Children.Add(item);
                SetConnectorDecoratorTemplate(item);
                newItems.Add(item);
            }

            
            SelectionService.ClearSelection();
            foreach (DesignerItem el in newItems)
            {
                if (el.ParentID != Guid.Empty)
                    el.ParentID = mappingOldToNewIDs[el.ParentID];
            }


            foreach (DesignerItem item in newItems)
            {
                if (item.ParentID == Guid.Empty)
                {
                    SelectionService.AddToSelection(item);
                }
            }

           
            IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid oldSourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid oldSinkID = new Guid(connectionXML.Element("SinkID").Value);

                if (mappingOldToNewIDs.ContainsKey(oldSourceID) && mappingOldToNewIDs.ContainsKey(oldSinkID))
                {
                    Guid newSourceID = mappingOldToNewIDs[oldSourceID];
                    Guid newSinkID = mappingOldToNewIDs[oldSinkID];

                    String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                    String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                    Connector sourceConnector = GetConnector(newSourceID, sourceConnectorName);
                    Connector sinkConnector = GetConnector(newSinkID, sinkConnectorName);

                    Connection connection = new Connection(sourceConnector, sinkConnector);
                    Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                    this.Children.Add(connection);

                    SelectionService.AddToSelection(connection);
                }
            }

            DesignerCanvas.BringToFront.Execute(null, this);

            
            root.Attribute("OffsetX").Value = (offsetX + 10).ToString();
            root.Attribute("OffsetY").Value = (offsetY + 10).ToString();
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        private void Paste_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Xaml);
        }

        #endregion

        #region Delete Command

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        
            DeleteCurrentSelection();
        }

        private void Delete_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Cut Command

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
          
            CopyCurrentSelection();
            DeleteCurrentSelection();
        }

        private void Cut_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Group Command

        private void Group_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveUndoState();
            var items = from item in this.SelectionService.CurrentSelection.OfType<DesignerItem>()
                        where item.ParentID == Guid.Empty
                        select item;

            Rect rect = GetBoundingRectangle(items);

            DesignerItem groupItem = new DesignerItem();
            groupItem.IsGroup = true;
            groupItem.Width = rect.Width;
            groupItem.Height = rect.Height;
            Canvas.SetLeft(groupItem, rect.Left);
            Canvas.SetTop(groupItem, rect.Top);
            Canvas groupCanvas = new Canvas();
            groupItem.Content = groupCanvas;
            Canvas.SetZIndex(groupItem, this.Children.Count);
            this.Children.Add(groupItem);

            foreach (DesignerItem item in items)
                item.ParentID = groupItem.ID;

            this.SelectionService.SelectItem(groupItem);
        }

        private void Group_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            int count = (from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                         where item.ParentID == Guid.Empty
                         select item).Count();

            e.CanExecute = count > 1;
        }

        #endregion

        #region Ungroup Command

        private void Ungroup_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var groups = (from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                          where item.IsGroup && item.ParentID == Guid.Empty
                          select item).ToArray();

            foreach (DesignerItem groupRoot in groups)
            {
                var children = from child in SelectionService.CurrentSelection.OfType<DesignerItem>()
                               where child.ParentID == groupRoot.ID
                               select child;

                foreach (DesignerItem child in children)
                    child.ParentID = Guid.Empty;

                this.SelectionService.RemoveFromSelection(groupRoot);
                this.Children.Remove(groupRoot);
                UpdateZIndex();
            }
        }

        private void Ungroup_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var groupedItem = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                              where item.ParentID != Guid.Empty
                              select item;


            e.CanExecute = groupedItem.Count() > 0;
        }

        #endregion

        #region BringForward Command

        private void BringForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> ordered = (from item in SelectionService.CurrentSelection
                                       orderby Canvas.GetZIndex(item as UIElement) descending
                                       select item as UIElement).ToList();

            int count = this.Children.Count;

            for (int i = 0; i < ordered.Count; i++)
            {
                int currentIndex = Canvas.GetZIndex(ordered[i]);
                int newIndex = Math.Min(count - 1 - i, currentIndex + 1);
                if (currentIndex != newIndex)
                {
                    Canvas.SetZIndex(ordered[i], newIndex);
                    IEnumerable<UIElement> it = this.Children.OfType<UIElement>().Where(item => Canvas.GetZIndex(item) == newIndex);

                    foreach (UIElement elm in it)
                    {
                        if (elm != ordered[i])
                        {
                            Canvas.SetZIndex(elm, currentIndex);
                            break;
                        }
                    }
                }
            }
        }

        private void Order_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.CurrentSelection.Count() > 0;
            e.CanExecute = true;
        }

        #endregion

        #region BringToFront Command

        private void BringToFront_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> selectionSorted = (from item in SelectionService.CurrentSelection
                                               orderby Canvas.GetZIndex(item as UIElement) ascending
                                               select item as UIElement).ToList();

            List<UIElement> childrenSorted = (from UIElement item in this.Children
                                              orderby Canvas.GetZIndex(item as UIElement) ascending
                                              select item as UIElement).ToList();

            int i = 0;
            int j = 0;
            foreach (UIElement item in childrenSorted)
            {
                if (selectionSorted.Contains(item))
                {
                    int idx = Canvas.GetZIndex(item);
                    Canvas.SetZIndex(item, childrenSorted.Count - selectionSorted.Count + j++);
                }
                else
                {
                    Canvas.SetZIndex(item, i++);
                }
            }
        }

        #endregion

        #region SendBackward Command

        private void SendBackward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> ordered = (from item in SelectionService.CurrentSelection
                                       orderby Canvas.GetZIndex(item as UIElement) ascending
                                       select item as UIElement).ToList();

            int count = this.Children.Count;

            for (int i = 0; i < ordered.Count; i++)
            {
                int currentIndex = Canvas.GetZIndex(ordered[i]);
                int newIndex = Math.Max(i, currentIndex - 1);
                if (currentIndex != newIndex)
                {
                    Canvas.SetZIndex(ordered[i], newIndex);
                    IEnumerable<UIElement> it = this.Children.OfType<UIElement>().Where(item => Canvas.GetZIndex(item) == newIndex);

                    foreach (UIElement elm in it)
                    {
                        if (elm != ordered[i])
                        {
                            Canvas.SetZIndex(elm, currentIndex);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region SendToBack Command

        private void SendToBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> selectionSorted = (from item in SelectionService.CurrentSelection
                                               orderby Canvas.GetZIndex(item as UIElement) ascending
                                               select item as UIElement).ToList();

            List<UIElement> childrenSorted = (from UIElement item in this.Children
                                              orderby Canvas.GetZIndex(item as UIElement) ascending
                                              select item as UIElement).ToList();
            int i = 0;
            int j = 0;
            foreach (UIElement item in childrenSorted)
            {
                if (selectionSorted.Contains(item))
                {
                    int idx = Canvas.GetZIndex(item);
                    Canvas.SetZIndex(item, j++);

                }
                else
                {
                    Canvas.SetZIndex(item, selectionSorted.Count + i++);
                }
            }
        }        

        #endregion

        #region AlignTop Command

        private void AlignTop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double top = Canvas.GetTop(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = top - Canvas.GetTop(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                }
            }
        }

        private void Align_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var groupedItem = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                              where item.ParentID == Guid.Empty
                              select item;


            e.CanExecute = groupedItem.Count() > 1;
            e.CanExecute = true;
        }

        #endregion

        #region AlignVerticalCenters Command

        private void AlignVerticalCenters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height / 2;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = bottom - (Canvas.GetTop(item) + item.Height / 2);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignBottom Command

        private void AlignBottom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = bottom - (Canvas.GetTop(item) + item.Height);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignLeft Command

        private void AlignLeft_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double left = Canvas.GetLeft(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = left - Canvas.GetLeft(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignHorizontalCenters Command

        private void AlignHorizontalCenters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double center = Canvas.GetLeft(selectedItems.First()) + selectedItems.First().Width / 2;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = center - (Canvas.GetLeft(item) + item.Width / 2);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignRight Command

        private void AlignRight_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double right = Canvas.GetLeft(selectedItems.First()) + selectedItems.First().Width;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = right - (Canvas.GetLeft(item) + item.Width);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region DistributeHorizontal Command

        private void DistributeHorizontal_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                let itemLeft = Canvas.GetLeft(item)
                                orderby itemLeft
                                select item;

            if (selectedItems.Count() > 1)
            {
                double left = Double.MaxValue;
                double right = Double.MinValue;
                double sumWidth = 0;
                foreach (DesignerItem item in selectedItems)
                {
                    left = Math.Min(left, Canvas.GetLeft(item));
                    right = Math.Max(right, Canvas.GetLeft(item) + item.Width);
                    sumWidth += item.Width;
                }

                double distance = Math.Max(0, (right - left - sumWidth) / (selectedItems.Count() - 1));
                double offset = Canvas.GetLeft(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = offset - Canvas.GetLeft(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                    offset = offset + item.Width + distance;
                }
            }
        }

        private void Distribute_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var groupedItem = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                              where item.ParentID == Guid.Empty
                              select item;


            e.CanExecute = groupedItem.Count() > 1;
            e.CanExecute = true;
        }

        #endregion

        #region DistributeVertical Command

        private void DistributeVertical_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                let itemTop = Canvas.GetTop(item)
                                orderby itemTop
                                select item;

            if (selectedItems.Count() > 1)
            {
                double top = Double.MaxValue;
                double bottom = Double.MinValue;
                double sumHeight = 0;
                foreach (DesignerItem item in selectedItems)
                {
                    top = Math.Min(top, Canvas.GetTop(item));
                    bottom = Math.Max(bottom, Canvas.GetTop(item) + item.Height);
                    sumHeight += item.Height;
                }

                double distance = Math.Max(0, (bottom - top - sumHeight) / (selectedItems.Count() - 1));
                double offset = Canvas.GetTop(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = offset - Canvas.GetTop(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                    offset = offset + item.Height + distance;
                }
            }
        }

        #endregion

        #region SelectAll Command

        private void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionService.SelectAll();
        }

        #endregion

        #region Helper Methods

        private XElement LoadSerializedDataFromFile(out string fileName)
        {
            fileName = null;
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Designer Files (*.xml)|*.xml|All Files (*.*)|*.*";

            if (openFile.ShowDialog() == true)
            {
                try
                {
                    fileName = openFile.FileName;
                    return XElement.Load(openFile.FileName);
                }
                catch (Exception e)
                {
                    NotifyBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return null;
        }
        public static XElement LoadSerializedDataFromCloud(out string fileName, string filepath )
        {
            fileName = null;

            try
            {
                fileName = filepath;
                return XElement.Load(filepath);

            }
            catch(Exception e)
            {
                NotifyBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }


            return null;
        }
        //

        void SaveFile(XElement xElement)
        {
         

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (saveFile.ShowDialog() == true)
            {
                try
                {
                    xElement.Save(saveFile.FileName);


                    App.NotifyUser(saveFile.FileName.ToString(), "Файл проекта сохранён", "folder.gif", 4000, PopupAnimation.Slide);


                }
                catch (Exception ex)
                {
                    NotifyBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        void SaveFileToCloud(XElement xElement)
        {
            DigitalHiveLoginForm loginForm = new DigitalHiveLoginForm(xElement); 
            loginForm.ShowDialog(); 
                 
            
        }

        private XElement LoadSerializedDataFromClipBoard()
        {
            if (Clipboard.ContainsData(DataFormats.Xaml))
            {
                String clipboardData = Clipboard.GetData(DataFormats.Xaml) as String;

                if (String.IsNullOrEmpty(clipboardData))
                    return null;
                try
                {
                    return XElement.Load(new StringReader(clipboardData));
                }
                catch (Exception e)
                {
                    NotifyBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK);

                   
                }
            }

            return null;
        }

        private XElement SerializeDesignerItems(IEnumerable<DesignerItem> designerItems)
        {
            XElement serializedItems = new XElement("DesignerItems",
                                           from item in designerItems
                                           let contentXaml = XamlWriter.Save(((DesignerItem)item).Content)
                                           let rotateTransform = item.RenderTransform as RotateTransform
                                           select new XElement("DesignerItem",
                                                      new XElement("Left", Canvas.GetLeft(item)),
                                                      new XElement("Top", Canvas.GetTop(item)),
                                                      new XElement("Width", item.Width),
                                                      new XElement("Height", item.Height),
                                                      new XElement("ID", item.ID),
                                                      new XElement("zIndex", Canvas.GetZIndex(item)),
                                                      new XElement("IsGroup", item.IsGroup),
                                                      new XElement("ParentID", item.ParentID),
                                                      new XElement("RotationAngle", rotateTransform != null ? rotateTransform.Angle : 0),
                                                      new XElement("Text", item.Text),
                                                      new XElement("FontSize", item.FontSize),
                                                      new XElement("FontFamily", item.FontFamily),
                                                      new XElement("Foreground", item.Foreground),
                                                      new XElement("ItemFill", item.Fill),
                                                      new XElement("ItemStroke", item.Stroke),
                                                      new XElement("Content", contentXaml)
                                                  )
                                       );

            return serializedItems;
        }
        private static string DoubleCollectionToString(DoubleCollection collection)
        {
            if (collection == null || collection.Count == 0)
                return string.Empty;

            return string.Join(",", collection);
        }
        private static DoubleCollection StringToDoubleCollection(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var values = value.Split(',');
            var doubleCollection = new DoubleCollection();

            foreach (var val in values)
            {
                if (double.TryParse(val, out double result))
                {
                    doubleCollection.Add(result);
                }
            }

            return doubleCollection;
        }
        private XElement SerializeConnections(IEnumerable<Connection> connections)
        {
            return new XElement("Connections",
                from connection in connections
                let sourceId = connection.Source?.ParentDesignerItem?.ID ?? Guid.Empty
                let sinkId = connection.Sink?.ParentDesignerItem?.ID ?? Guid.Empty
                let sourceName = connection.Source?.Name ?? "Unknown"
                let sinkName = connection.Sink?.Name ?? "Unknown"
                where sourceId != Guid.Empty && sinkId != Guid.Empty 
                select new XElement("Connection",
                    new XElement("SourceID", sourceId),
                    new XElement("SinkID", sinkId),
                    new XElement("SourceConnectorName", sourceName),
                    new XElement("SinkConnectorName", sinkName),
                    new XElement("SourceArrowSymbol", connection.SourceArrowSymbol.ToString()),
                    new XElement("SinkArrowSymbol", connection.SinkArrowSymbol.ToString()),
                    new XElement("ConnectionLineType", connection._ConnectionLineType.ToString()),
                    new XElement("StrokeDashArray", DoubleCollectionToString(connection.StrokeDashArray)),
                    new XElement("Text", connection.Text ?? string.Empty),
                    new XElement("zIndex", Canvas.GetZIndex(connection))
            ));
        }

        private static DesignerItem DeserializeDesignerItem(XElement itemXML, Guid id, double OffsetX, double OffsetY)
        {
            DesignerItem item = new DesignerItem(id);
            item.Width = Double.Parse(itemXML.Element("Width").Value, CultureInfo.InvariantCulture);
            item.Height = Double.Parse(itemXML.Element("Height").Value, CultureInfo.InvariantCulture);
            item.ParentID = new Guid(itemXML.Element("ParentID").Value);
            item.IsGroup = Boolean.Parse(itemXML.Element("IsGroup").Value);
            item.Text = itemXML.Element("Text")?.Value; 
            item.FontSize = Double.Parse(itemXML.Element("FontSize").Value, CultureInfo.InvariantCulture); 
            item.FontFamily = new FontFamily(itemXML.Element("FontFamily").Value); 
            item.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(itemXML.Element("Foreground").Value)); 
            item.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(itemXML.Element("ItemStroke").Value)); 
            item.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(itemXML.Element("ItemFill").Value)); 

            Canvas.SetLeft(item, Double.Parse(itemXML.Element("Left").Value, CultureInfo.InvariantCulture) + OffsetX);
            Canvas.SetTop(item, Double.Parse(itemXML.Element("Top").Value, CultureInfo.InvariantCulture) + OffsetY);
            Canvas.SetZIndex(item, Int32.Parse(itemXML.Element("zIndex").Value));
            

            double rotationAngle = Double.Parse(itemXML.Element("RotationAngle").Value, CultureInfo.InvariantCulture);
            RotateTransform rotateTransform = new RotateTransform(rotationAngle);
            item.RenderTransform = rotateTransform;
            item.RenderTransformOrigin = new Point(0.5, 0.5);

            Object content = XamlReader.Load(XmlReader.Create(new StringReader(itemXML.Element("Content").Value)));
            item.Content = content;
            return item;
        }

        private void CopyCurrentSelection()
        {
            IEnumerable<DesignerItem> selectedDesignerItems =
                this.SelectionService.CurrentSelection.OfType<DesignerItem>();

            List<Connection> selectedConnections =
                this.SelectionService.CurrentSelection.OfType<Connection>().ToList();

            foreach (Connection connection in this.Children.OfType<Connection>())
            {
                if (!selectedConnections.Contains(connection))
                {
                    DesignerItem sourceItem = (from item in selectedDesignerItems
                                               where item.ID == connection.Source.ParentDesignerItem.ID
                                               select item).FirstOrDefault();

                    DesignerItem sinkItem = (from item in selectedDesignerItems
                                             where item.ID == connection.Sink.ParentDesignerItem.ID
                                             select item).FirstOrDefault();

                    if (sourceItem != null &&
                        sinkItem != null &&
                        BelongToSameGroup(sourceItem, sinkItem))
                    {
                        selectedConnections.Add(connection);
                    }
                }
            }

            XElement designerItemsXML = SerializeDesignerItems(selectedDesignerItems);
            XElement connectionsXML = SerializeConnections(selectedConnections);

            XElement root = new XElement("Root");
            root.Add(designerItemsXML);
            root.Add(connectionsXML);

            root.Add(new XAttribute("OffsetX", 10));
            root.Add(new XAttribute("OffsetY", 10));

            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        private void DeleteCurrentSelection()
        {
            foreach (Connection connection in SelectionService.CurrentSelection.OfType<Connection>())
            {
                this.Children.Remove(connection);
            }

            foreach (DesignerItem item in SelectionService.CurrentSelection.OfType<DesignerItem>())
            {
                Control cd = item.Template.FindName("PART_ConnectorDecorator", item) as Control;

                List<Connector> connectors = new List<Connector>();
                GetConnectors(cd, connectors);

                foreach (Connector connector in connectors)
                {
                    foreach (Connection con in connector.Connections)
                    {
                        this.Children.Remove(con);
                    }
                }
                this.Children.Remove(item);
            }

            SelectionService.ClearSelection();
            UpdateZIndex();
        }

        private void UpdateZIndex()
        {
            List<UIElement> ordered = (from UIElement item in this.Children
                                       orderby Canvas.GetZIndex(item as UIElement)
                                       select item as UIElement).ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                Canvas.SetZIndex(ordered[i], i);
            }
        }

        private static Rect GetBoundingRectangle(IEnumerable<DesignerItem> items)
        {
            double x1 = Double.MaxValue;
            double y1 = Double.MaxValue;
            double x2 = Double.MinValue;
            double y2 = Double.MinValue;

            foreach (DesignerItem item in items)
            {
                x1 = Math.Min(Canvas.GetLeft(item), x1);
                y1 = Math.Min(Canvas.GetTop(item), y1);

                x2 = Math.Max(Canvas.GetLeft(item) + item.Width, x2);
                y2 = Math.Max(Canvas.GetTop(item) + item.Height, y2);
            }

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }

        private void GetConnectors(DependencyObject parent, List<Connector> connectors)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is Connector)
                {
                    connectors.Add(child as Connector);
                }
                else
                    GetConnectors(child, connectors);
            }
        }

        private Connector GetConnector(Guid itemID, String connectorName)
        {
            DesignerItem designerItem = (from item in this.Children.OfType<DesignerItem>()
                                         where item.ID == itemID
                                         select item).FirstOrDefault();

            Control connectorDecorator = designerItem.Template.FindName("PART_ConnectorDecorator", designerItem) as Control;
            connectorDecorator.ApplyTemplate();

            return connectorDecorator.Template.FindName(connectorName, connectorDecorator) as Connector;
        }

        private bool BelongToSameGroup(IGroupable item1, IGroupable item2)
        {
            IGroupable root1 = SelectionService.GetGroupRoot(item1);
            IGroupable root2 = SelectionService.GetGroupRoot(item2);

            return (root1.ID == root2.ID);
        }

        #endregion



        private void gridToggle(object sender, ExecutedRoutedEventArgs e)
        {
            this.AddGridAdorner(this);
        }
        private void gridToggleOff(object sender, ExecutedRoutedEventArgs e)
        {
            this.RemoveGridAdorner(this);
        }
    }
}
