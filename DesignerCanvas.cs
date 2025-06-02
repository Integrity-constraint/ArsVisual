using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using ArsVisual.Adorners;
using ArsVisual.Helpers;
using System.Diagnostics;

namespace ArsVisual
{
    public partial class DesignerCanvas : Canvas
    {
       
        private const double SnapThreshold = 5.0; 
        private SnapAdorner snapAdorner;

        private Point? rubberbandSelectionStartPoint = null;

        private SelectionService selectionService;
        internal SelectionService SelectionService
        {
            get
            {
                if (selectionService == null)
                    selectionService = new SelectionService(this);

                return selectionService;
            }
        }
        public void InitializeSnapAdorner()
        {
            if (snapAdorner == null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    snapAdorner = new SnapAdorner(this);
                    adornerLayer.Add(snapAdorner);
                }
            }
        }

        public void AddGridAdorner(Canvas canvas)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(canvas);
            if (layer != null)
            {
             
                Adorner[] adorners = layer.GetAdorners(canvas);
                if (adorners != null)
                {
                    foreach (Adorner adorner in adorners)
                    {
                        if (adorner is GridAdorner)
                        {
                            return; 
                        }
                    }
                }

               
                GridAdorner gridAdorner = new GridAdorner(canvas, 20);
                layer.Add(gridAdorner);
                layer.Update();
            }
        }
        public void RemoveGridAdorner(Canvas canvas)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(canvas);
            if (layer != null)
            {
                Adorner[] adorners = layer.GetAdorners(canvas);
                if (adorners != null)
                {
                    foreach (Adorner adorner in adorners)
                    {
                        if (adorner is GridAdorner)
                        {
                            layer.Remove(adorner);
                            layer.Update();
                        }
                    }
                }
            }
        }
        public void ClearSnapLines()
        {
            snapAdorner?.ClearSnapLines();
        }

      
        public void DrawSnapLine(AlignmentType alignmentType, double position)
        {
            InitializeSnapAdorner();
            snapAdorner?.AddSnapLine(alignmentType, position, ActualWidth, ActualHeight);
        }

     
        public IEnumerable<UIElement> GetOtherElements(IEnumerable<UIElement> excludeElements)
        {
            return Children.Cast<UIElement>().Where(e => e is FrameworkElement && !excludeElements.Contains(e));
        }

      
        public double SnapThresholdValue => SnapThreshold;

      
        public enum AlignmentType
        {
            Horizontal,
            Vertical
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Source == this)
            {
               
                this.rubberbandSelectionStartPoint = new Point?(e.GetPosition(this));

                
                SelectionService.ClearSelection();
                Focus();
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            
            if (e.LeftButton != MouseButtonState.Pressed)
                this.rubberbandSelectionStartPoint = null;

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.rubberbandSelectionStartPoint = null;
                ClearSnapLines(); 
            }
            if (this.rubberbandSelectionStartPoint.HasValue)
            {
               
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(this, rubberbandSelectionStartPoint);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }
            }
            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            DragObject dragObject = e.Data.GetData(typeof(DragObject)) as DragObject;
            if (dragObject != null && !String.IsNullOrEmpty(dragObject.Xaml))
            {
                DesignerItem newItem = null;
                Object content = XamlReader.Load(XmlReader.Create(new StringReader(dragObject.Xaml)));

                if (content != null)
                {
                    SaveUndoState();
                    newItem = new DesignerItem();
                    newItem.Content = content;

                    Point position = e.GetPosition(this);

                    if (dragObject.DesiredSize.HasValue)
                    {
                        Size desiredSize = dragObject.DesiredSize.Value;
                        newItem.Width = desiredSize.Width;
                        newItem.Height = desiredSize.Height;

                        DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
                        DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));
                    }
                    else
                    {
                        DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X));
                        DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y));
                    }

                    Canvas.SetZIndex(newItem, this.Children.Count);
                    this.Children.Add(newItem);                    
                    SetConnectorDecoratorTemplate(newItem);

                   
                    this.SelectionService.SelectItem(newItem);
                    newItem.Focus();
                }

                e.Handled = true;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();

            foreach (UIElement element in this.InternalChildren)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

              
                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }
            size.Width += 10;
            size.Height += 10;
            return size;
        }
        public IEnumerable<UIElement> GetSelectedItems() { return this.SelectionService.CurrentSelection.OfType<UIElement>(); }
        private void SetConnectorDecoratorTemplate(DesignerItem item)
        {
            if (item.ApplyTemplate() && item.Content is UIElement)
            {
                ControlTemplate template = DesignerItem.GetConnectorDecoratorTemplate(item.Content as UIElement);
                Control decorator = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
                if (decorator != null && template != null)
                    decorator.Template = template;
            }
        }
    }


}
