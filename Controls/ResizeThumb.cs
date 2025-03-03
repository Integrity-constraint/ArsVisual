using DiagramDesigner.Adorners;
using System;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Linq;
using System.Collections.Generic;

namespace DiagramDesigner.Controls
{
    public class ResizeThumb : Thumb
    {
        private RotateTransform rotateTransform;
        private double angle;
        private Adorner adorner;
        private Point transformOrigin;
        private ContentControl designerItem;
        private Canvas canvas;

        public ResizeThumb()
        {
            DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
            DragCompleted += new DragCompletedEventHandler(this.ResizeThumb_DragCompleted);
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = this.DataContext as ContentControl;

            if (this.designerItem != null)
            {
                this.canvas = VisualTreeHelper.GetParent(this.designerItem) as Canvas;

                if (this.canvas != null)
                {
                    this.transformOrigin = this.designerItem.RenderTransformOrigin;
                    this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;

                    if (this.rotateTransform != null)
                    {
                        this.angle = this.rotateTransform.Angle * Math.PI / 180.0;
                    }
                    else
                    {
                        this.angle = 0.0d;
                    }

                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.canvas);
                    if (adornerLayer != null)
                    {
                        this.adorner = new SizeAdorner(this.designerItem);
                        adornerLayer.Add(this.adorner);
                    }
                }
            }
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null)
            {
                double deltaVertical, deltaHorizontal;

                IEnumerable<DesignerItem> selectedDesignerItems = GetSelectedDesignerItems();

                double minLeft, minTop, minDeltaHorizontal, minDeltaVertical;

                CalculateDragLimits(selectedDesignerItems, out minLeft, out minTop, out minDeltaHorizontal, out minDeltaVertical);

                foreach (var item in selectedDesignerItems)
                {
                    double newTop, newLeft;

                    switch (VerticalAlignment)
                    {
                        case System.Windows.VerticalAlignment.Bottom:
                            deltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                            newTop = Canvas.GetTop(item) + (this.transformOrigin.Y * deltaVertical * (1 - Math.Cos(-this.angle)));
                            newLeft = Canvas.GetLeft(item) - deltaVertical * this.transformOrigin.Y * Math.Sin(-this.angle);

                            if (IsWithinBounds(newTop, newLeft, item.Width, item.Height - deltaVertical))
                            {
                                Canvas.SetTop(item, newTop);
                                Canvas.SetLeft(item, newLeft);
                                item.Height -= deltaVertical;
                            }
                            break;
                        case System.Windows.VerticalAlignment.Top:
                            deltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                            newTop = Canvas.GetTop(item) + deltaVertical * Math.Cos(-this.angle) + (this.transformOrigin.Y * deltaVertical * (1 - Math.Cos(-this.angle)));
                            newLeft = Canvas.GetLeft(item) + deltaVertical * Math.Sin(-this.angle) - (this.transformOrigin.Y * deltaVertical * Math.Sin(-this.angle));

                            if (IsWithinBounds(newTop, newLeft, item.Width, item.Height - deltaVertical))
                            {
                                Canvas.SetTop(item, newTop);
                                Canvas.SetLeft(item, newLeft);
                                item.Height -= deltaVertical;
                            }
                            break;
                        default:
                            break;
                    }

                    switch (HorizontalAlignment)
                    {
                        case System.Windows.HorizontalAlignment.Left:
                            deltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                            newTop = Canvas.GetTop(item) + deltaHorizontal * Math.Sin(this.angle) - this.transformOrigin.X * deltaHorizontal * Math.Sin(this.angle);
                            newLeft = Canvas.GetLeft(item) + deltaHorizontal * Math.Cos(this.angle) + (this.transformOrigin.X * deltaHorizontal * (1 - Math.Cos(this.angle)));

                            if (IsWithinBounds(newTop, newLeft, item.Width - deltaHorizontal, item.Height))
                            {
                                Canvas.SetTop(item, newTop);
                                Canvas.SetLeft(item, newLeft);
                                item.Width -= deltaHorizontal;
                            }
                            break;
                        case System.Windows.HorizontalAlignment.Right:
                            deltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                            newTop = Canvas.GetTop(item) - this.transformOrigin.X * deltaHorizontal * Math.Sin(this.angle);
                            newLeft = Canvas.GetLeft(item) + (deltaHorizontal * this.transformOrigin.X * (1 - Math.Cos(this.angle)));

                            if (IsWithinBounds(newTop, newLeft, item.Width - deltaHorizontal, item.Height))
                            {
                                Canvas.SetTop(item, newTop);
                                Canvas.SetLeft(item, newLeft);
                                item.Width -= deltaHorizontal;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            e.Handled = true;
        }

        private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this.adorner != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.canvas);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this.adorner);
                }

                this.adorner = null;
            }
        }

        // Метод для получения выбранных элементов DesignerItem
        private IEnumerable<DesignerItem> GetSelectedDesignerItems()
        {
            DesignerCanvas designer = VisualTreeHelper.GetParent(this.designerItem) as DesignerCanvas;
            if (designer != null)
            {
                return designer.SelectionService.CurrentSelection.OfType<DesignerItem>();
            }
            return Enumerable.Empty<DesignerItem>();
        }

        // Метод для проверки границ контейнера
        private bool IsWithinBounds(double top, double left, double width, double height)
        {
            return top >= 0 && left >= 0 && (top + height) <= this.canvas.ActualHeight && (left + width) <= this.canvas.ActualWidth;
        }

        // Метод для вычисления ограничений перемещения
        private void CalculateDragLimits(IEnumerable<DesignerItem> selectedItems, out double minLeft, out double minTop, out double minDeltaHorizontal, out double minDeltaVertical)
        {
            minLeft = double.MaxValue;
            minTop = double.MaxValue;
            minDeltaHorizontal = double.MaxValue;
            minDeltaVertical = double.MaxValue;

            foreach (DesignerItem item in selectedItems)
            {
                double left = Canvas.GetLeft(item);
                double top = Canvas.GetTop(item);

                minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);

                minDeltaVertical = Math.Min(minDeltaVertical, item.ActualHeight - item.MinHeight);
                minDeltaHorizontal = Math.Min(minDeltaHorizontal, item.ActualWidth - item.MinWidth);
            }
        }

    }


}
