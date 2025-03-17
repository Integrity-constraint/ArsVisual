using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DiagramDesigner
{
    public partial class Window1 : Window
    {
        private Dictionary<TabItem, DesignerCanvas> _pageCanvases = new Dictionary<TabItem, DesignerCanvas>();
        private Dictionary<TabItem, object> _pageStates = new Dictionary<TabItem, object>();
        DesignerCanvas _canvas;
        public Window1()
        {
            InitializeComponent();

            // Привязка первой вкладки к логике
            _pageCanvases[(TabItem)MainTabControl.Items[0]] = MyDesignerCanvas;

            
            MainTabControl.SelectionChanged += TabControl_SelectionChanged;
            this.Closing += OnClosing;
        }

        // Обработчик переключения вкладок
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem == AddTabButton)
            {
                // Добавляем новую страницу
                AddNewPage();
                MainTabControl.SelectedIndex = MainTabControl.Items.Count - 2; // Переключаемся на новую страницу
            }
            else
            {
                // Сохраняем и восстанавливаем состояние страниц
                PageSwitch(sender, e);
            }
        }

        // Метод для добавления новой страницы
        private void AddNewPage()
        {
            var newTabItem = new TabItem { Header = $"Страница {MainTabControl.Items.Count}" };

            // Устанавливаем шаблон заголовка с кнопкой закрытия
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
        private void CloseTabClick(object sender, RoutedEventArgs e)
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
                        // Удаляем вкладку
                        MainTabControl.Items.Remove(tabItem);

                        // Очищаем данные
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
                    // Если на странице нет схемы, просто удаляем вкладку
                    MainTabControl.Items.Remove(tabItem);
                    _pageCanvases.Remove(tabItem);
                    _pageStates.Remove(tabItem);
                }
            }
        }

        // Обработчик переключения страниц
        private void PageSwitch(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem currentTab && currentTab != AddTabButton)
            {
                // Сохраняем состояние предыдущей страницы
                if (MainTabControl.SelectedIndex > 0)
                {
                    var previousTab = MainTabControl.Items[MainTabControl.SelectedIndex - 1] as TabItem;
                    if (previousTab != null && _pageCanvases.ContainsKey(previousTab))
                    {
                        _pageStates[previousTab] = SaveCanvasState(_pageCanvases[previousTab]);
                    }
                }

                // Восстанавливаем состояние текущей страницы
                if (_pageStates.ContainsKey(currentTab))
                {
                    RestoreCanvasState(_pageCanvases[currentTab], _pageStates[currentTab]);
                }
            }
        }

        // Метод для сохранения состояния канваса
        private object SaveCanvasState(DesignerCanvas canvas)
        {
            // Здесь реализуйте логику сохранения состояния канваса
            // Например, сериализация элементов диаграммы
            return null; // Замените на реальные данные
        }

        // Метод для восстановления состояния канваса
        private void RestoreCanvasState(DesignerCanvas canvas, object state)
        {
            // Здесь реализуйте логику восстановления состояния канваса
            // Например, десериализация элементов диаграммы
        }

        // Обработчик закрытия окна
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
                    // Закрываем приложение
                    Application.Current.Shutdown();
                }
                else
                {
                    // Отменяем закрытие
                    e.Cancel = true;
                }
            }
        }
    }
}