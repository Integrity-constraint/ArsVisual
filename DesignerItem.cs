using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ArsVisual;
using ArsVisual.Controls;
using ArsVisual.NotifyComponents.MsgBox;
using ArsVisual.Resources;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;


namespace ArsVisual
{
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(MoveThumb))]
    [TemplatePart(Name = "PART_RotateThumb", Type = typeof(RotateThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class DesignerItem : ContentControl, ISelectable, IGroupable, INotifyPropertyChanged
    {
        #region ID
        private Guid id;
        public Guid ID
        {
            get { return id; }
        }
        #endregion
            
        #region ParentID
        public Guid ParentID
        {
            get { return (Guid)GetValue(ParentIDProperty); }
            set { SetValue(ParentIDProperty, value); }
        }
        public static readonly DependencyProperty ParentIDProperty = DependencyProperty.Register("ParentID", typeof(Guid), typeof(DesignerItem));
        #endregion

        #region IsGroup
        public bool IsGroup
        {
            get { return (bool)GetValue(IsGroupProperty); }
            set { SetValue(IsGroupProperty, value); }
        }
        public static readonly DependencyProperty IsGroupProperty =
            DependencyProperty.Register("IsGroup", typeof(bool), typeof(DesignerItem));
        #endregion

        #region IsSelected Property

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected",
                                       typeof(bool),
                                       typeof(DesignerItem),
                                       new FrameworkPropertyMetadata(false));

        #endregion
        #region RotationAngle
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(DesignerItem), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region DragThumbTemplate Property

        
        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetDragThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }

        public static void SetDragThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(DragThumbTemplateProperty, value);
        }

        #endregion

        #region ConnectorDecoratorTemplate Property

       
        public static readonly DependencyProperty ConnectorDecoratorTemplateProperty =
            DependencyProperty.RegisterAttached("ConnectorDecoratorTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetConnectorDecoratorTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(ConnectorDecoratorTemplateProperty);
        }

        public static void SetConnectorDecoratorTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(ConnectorDecoratorTemplateProperty, value);
        }

        #endregion

        #region IsDragConnectionOver

        
        public bool IsDragConnectionOver
        {
            get { return (bool)GetValue(IsDragConnectionOverProperty); }
            set { SetValue(IsDragConnectionOverProperty, value); }
        }
        public static readonly DependencyProperty IsDragConnectionOverProperty =
            DependencyProperty.Register("IsDragConnectionOver",
                                         typeof(bool),
                                         typeof(DesignerItem),
                                         new FrameworkPropertyMetadata(false));

        #endregion

      
        public static readonly DependencyProperty TextProperty =
       DependencyProperty.Register("Text", typeof(string), typeof(DesignerItem), new PropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty FontSizeProperty =
     DependencyProperty.Register("FontSize", typeof(double), typeof(DesignerItem), new PropertyMetadata(12.0));

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(DesignerItem), new PropertyMetadata(new FontFamily("Segoe UI")));

        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static readonly DependencyProperty FontForeground =
           DependencyProperty.Register("Foreground", typeof(SolidColorBrush), typeof(DesignerItem), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
       
        public SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(FontForeground); }
            set { SetValue(FontForeground, value); }

        }
        public static readonly DependencyProperty ItemFill =
         DependencyProperty.Register(
             nameof(Fill),
             typeof(Brush),
             typeof(DesignerItem),
             new PropertyMetadata((Brush)Application.Current.Resources["FillElement"])
         );

        public static readonly DependencyProperty ItemStroke =
            DependencyProperty.Register(
                nameof(Stroke),
                typeof(Brush),
                typeof(DesignerItem),
                new PropertyMetadata((Brush)Application.Current.Resources["StrokeElement"])
            );

        public Brush Fill
        {
            get { return (Brush)GetValue(ItemFill); }
            set { SetValue(ItemFill, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(ItemStroke); }
            set { SetValue(ItemStroke, value); }
        }
        public static readonly DependencyProperty IsHitTestVisibleProperty =
        DependencyProperty.Register(
            "IsHitTestVisible",
            typeof(bool),
            typeof(DesignerItem),
            new PropertyMetadata(false, OnIsHitTestVisibleChanged));

      
        public bool IsHitTestVisible
        {
            get { return (bool)GetValue(IsHitTestVisibleProperty); }
            set { SetValue(IsHitTestVisibleProperty, value); }
        }
        private static void OnIsHitTestVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as DesignerItem;
            if (item != null)
            {
                
                item.UpdateHitTestVisibility();
            }
        }
        private void UpdateHitTestVisibility()
        {
            if (IsHitTestVisible)
            {
               
                var textBox = FindChild<TextBox>(this);
                textBox?.Focus();
            }
        }
        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;
                var descendant = FindChild<T>(child);
                if (descendant != null)
                    return descendant;
            }
            return null;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        static DesignerItem()
        {
           
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));


        }
        private void InitializeCommands()
        {
            CommandBindings.Add(new CommandBinding(DesignerItemCommands.ChangeFontSizeCommand, OnChangeFontSizeExecuted));
            CommandBindings.Add(new CommandBinding(DesignerItemCommands.ChangeFontFamilyCommand, OnChangeFontFamilyExecuted));
            CommandBindings.Add(new CommandBinding(DesignerItemCommands.ChangeFontForegroundCommand, OnChangeFontForegroubd));
           
            CommandBindings.Add(new CommandBinding(DesignerItemCommands.ChangeItemFillCommand, OnChangeItemFill));
            CommandBindings.Add(new CommandBinding(DesignerItemCommands.ChangeItemStrokeCommand, OnChangeItemStroke));
            this.MouseDoubleClick += DesignerItem_MouseDoubleClick;
        }

        public DesignerItem(Guid id)
        {
            this.id = id;
            this.Loaded += new RoutedEventHandler(DesignerItem_Loaded);
            InitializeCommands();

        }
        public void SaveUndofunc()
        {
            var canvas = VisualTreeHelper.GetParent(this) as DesignerCanvas;
            canvas?.SaveUndoState();
        }
        private void DesignerItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            IsHitTestVisible = !IsHitTestVisible;
        }

        private void OnChangeFontSizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveUndofunc();
            if (double.TryParse(e.Parameter.ToString(), out double fontSize))
            {
                
                if (fontSize > 0)
                {
                   
                    FontSize = fontSize;
                }
                else
                {
                    NotifyBox.Show("Размер шрифта должен быть больше 0.","Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private void OnChangeFontFamilyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveUndofunc();
            FontFamily = new FontFamily(e.Parameter.ToString());
        }
        private void OnChangeFontForegroubd(object sender, ExecutedRoutedEventArgs e)
        {
            SaveUndofunc();
            Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(e.Parameter.ToString()));
        }
        private void OnChangeItemFill(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SaveUndofunc();
                if (e.Parameter is string colorName)
                {
                    if (colorName == "Custom")
                    {
                       
                        if (ColorPickerWindow.ShowColorPicker(out System.Windows.Media.Color selectedColor))
                        {
                            
                            string hexColor = $"#{selectedColor.A:X2}{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                            this.Fill = new SolidColorBrush(selectedColor);
                        }
                        return;
                    }

                   
                    this.Fill = (Brush)new BrushConverter().ConvertFromString(colorName);
                }
            }
            catch (Exception ex)
            {
                NotifyBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnChangeItemStroke(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (e.Parameter is string colorName)
                {
                    if (colorName == "Custom")
                    {
                      
                        if (ColorPickerWindow.ShowColorPicker(out System.Windows.Media.Color selectedColor))
                        {
                            string hexColor = $"#{selectedColor.A:X2}{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                            this.Stroke = new SolidColorBrush(selectedColor);
                        }
                        return;
                    }

               
                    this.Stroke = (Brush)new BrushConverter().ConvertFromString(colorName);
                }
            }
            catch (Exception ex)
            {
                NotifyBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CanExecuteChangeItemFill(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true; 
        }
        public DesignerItem()
            : this(Guid.NewGuid())
        {
            CommandBindings.Add(new CommandBinding(DesignerItemCommands.ChangeFontSizeCommand, OnChangeFontSizeExecuted));
            CommandBindings.Add(new CommandBinding(DesignerItemCommands.ChangeFontFamilyCommand, OnChangeFontFamilyExecuted));
            CommandBindings.Add(new CommandBinding(DesignerItemCommands.ChangeFontForegroundCommand, OnChangeFontForegroubd));
            CommandBindings.Add(new CommandBinding( DesignerItemCommands.ChangeItemFillCommand,  OnChangeItemFill, CanExecuteChangeItemFill));
            CommandBindings.Add(new CommandBinding(DesignerItemCommands.ChangeItemStrokeCommand, OnChangeItemStroke));

        }


        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

           
            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    if (this.IsSelected)
                    {
                        designer.SelectionService.RemoveFromSelection(this);
                    }
                    else
                    {
                        designer.SelectionService.AddToSelection(this);
                    }
                else if (!this.IsSelected)
                {
                    designer.SelectionService.SelectItem(this);
                }
                Focus();
            }

            e.Handled = false;
        }

        void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (base.Template != null)
            {
                ContentPresenter contentPresenter =
                    this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
                if (contentPresenter != null && VisualTreeHelper.GetChildrenCount(contentPresenter) > 0) // Проверяем количество детей
                {
                    UIElement contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
                    if (contentVisual != null)
                    {
                        MoveThumb thumb = this.Template.FindName("PART_DragThumb", this) as MoveThumb;
                        if (thumb != null)
                        {
                            ControlTemplate template =
                                DesignerItem.GetDragThumbTemplate(contentVisual) as ControlTemplate;
                            if (template != null)
                                thumb.Template = template;
                        }

                        RotateThumb rotateThumb = this.Template.FindName("PART_RotateThumb", this) as RotateThumb;
                        if (rotateThumb != null)
                        {
                            ControlTemplate rotateTemplate =
                                DesignerItem.GetRotateThumbTemplate(contentVisual) as ControlTemplate;
                            if (rotateTemplate != null)
                                rotateThumb.Template = rotateTemplate;
                        }
                    }
                }
            }
        }
       

        #region RotateThumbTemplate Property

     
        public static readonly DependencyProperty RotateThumbTemplateProperty =
            DependencyProperty.RegisterAttached("RotateThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetRotateThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(RotateThumbTemplateProperty);
        }

        public static void SetRotateThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(RotateThumbTemplateProperty, value);
        }

        #endregion
    }
}