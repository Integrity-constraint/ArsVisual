using ArsVisual.Adorners;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace ArsVisual.Controls
{
    public class MoveThumb : Thumb
    {
        private RotateTransform rotateTransform;
        private ContentControl designerItem;

        public MoveThumb()
        {
            DragStarted += MoveThumb_DragStarted;
            DragDelta += MoveThumb_DragDelta;
        }

        public void SaveUndofunc()
        {
            var canvas = VisualTreeHelper.GetParent(this.designerItem) as DesignerCanvas;
            canvas?.SaveUndoState();
        }
        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = DataContext as ContentControl;
            if (this.designerItem != null)
            {
                this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
              SaveUndofunc();
            }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null)
            {
                var designerCanvas = VisualTreeHelper.GetParent(this.designerItem) as DesignerCanvas;
                if (designerCanvas != null)
                {
            
                    var selectedElements = designerCanvas.GetSelectedItems().OfType<ContentControl>().ToList();
                    if (selectedElements.Count == 0) return;

                  
                    designerCanvas.ClearSnapLines();

               
                    Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);
                    if (this.rotateTransform != null)
                    {
                        dragDelta = this.rotateTransform.Transform(dragDelta);
                    }

             
                    Rect selectionBounds = GetSelectionBounds(selectedElements);

                    double newLeft = selectionBounds.Left + dragDelta.X;
                    double newTop = selectionBounds.Top + dragDelta.Y;

       
                    SnapToAlignment(designerCanvas, selectedElements, ref newLeft, ref newTop, selectionBounds.Width, selectionBounds.Height);

                    double finalDeltaX = newLeft - selectionBounds.Left;
                    double finalDeltaY = newTop - selectionBounds.Top;

                    foreach (var control in selectedElements)
                    {
                        double itemLeft = Canvas.GetLeft(control);
                        double itemTop = Canvas.GetTop(control);
                        Canvas.SetLeft(control, itemLeft + finalDeltaX);
                        Canvas.SetTop(control, itemTop + finalDeltaY);
                    }
                }
            }
        }
        private void SnapToAlignment(DesignerCanvas canvas, List<ContentControl> selectedElements,
                             ref double newLeft, ref double newTop, double width, double height)
        {
            const double SnapThreshold = 5.0; 
                                             
            var otherElements = canvas.Children.OfType<ContentControl>().Except(selectedElements);

          
            foreach (var element in otherElements)
            {
                double otherTop = Canvas.GetTop(element);
                if (double.IsNaN(otherTop)) continue;
                double otherHeight = element.ActualHeight;

                if (Math.Abs(newTop - otherTop) < SnapThreshold)
                {
                    newTop = otherTop;
                    canvas.DrawSnapLine(DesignerCanvas.AlignmentType.Horizontal, newTop);
                }
                else if (Math.Abs(newTop + height - (otherTop + otherHeight)) < SnapThreshold)
                {
                    newTop = otherTop + otherHeight - height;
                    canvas.DrawSnapLine(DesignerCanvas.AlignmentType.Horizontal, otherTop + otherHeight);
                }

            
                double otherLeft = Canvas.GetLeft(element);
                if (double.IsNaN(otherLeft)) continue;
                double otherWidth = element.ActualWidth;

                if (Math.Abs(newLeft - otherLeft) < SnapThreshold)
                {
                    newLeft = otherLeft;
                    canvas.DrawSnapLine(DesignerCanvas.AlignmentType.Vertical, newLeft);
                }
                else if (Math.Abs(newLeft + width - (otherLeft + otherWidth)) < SnapThreshold)
                {
                    newLeft = otherLeft + otherWidth - width;
                    canvas.DrawSnapLine(DesignerCanvas.AlignmentType.Vertical, otherLeft + otherWidth);
                }
            }
        }
      
        private Rect GetSelectionBounds(IEnumerable<ContentControl> items)
        {
            Rect bounds = Rect.Empty;
            foreach (var item in items)
            {
                double left = Canvas.GetLeft(item);
                double top = Canvas.GetTop(item);
            
                if (double.IsNaN(left) || double.IsNaN(top)) continue;
                Rect itemRect = new Rect(left, top, item.ActualWidth, item.ActualHeight);
                bounds = bounds.IsEmpty ? itemRect : Rect.Union(bounds, itemRect);
            }
            return bounds;
        }

       
        private void SnapToAlignment(DesignerCanvas canvas, SnapAdorner adorner, IEnumerable<DependencyObject> selectedItems,
                                    ref double newLeft, ref double newTop, double width, double height)
        {
            const double SnapThreshold = 5.0; // Порог привязки
            var otherElements = canvas.Children.Cast<UIElement>().Except(selectedItems.OfType<UIElement>());

            // Проверка горизонтальной привязки
            foreach (var element in otherElements)
            {
                double otherTop = Canvas.GetTop(element);
                double otherHeight = element.RenderSize.Height;

                if (Math.Abs(newTop - otherTop) < SnapThreshold)
                {
                    newTop = otherTop;
                    adorner?.AddSnapLine(DesignerCanvas.AlignmentType.Horizontal, newTop, canvas.ActualWidth, canvas.ActualHeight);
                }
                else if (Math.Abs(newTop + height - (otherTop + otherHeight)) < SnapThreshold)
                {
                    newTop = otherTop + otherHeight - height;
                    adorner?.AddSnapLine(DesignerCanvas.AlignmentType.Horizontal, otherTop + otherHeight, canvas.ActualWidth, canvas.ActualHeight);
                }

               
                double otherLeft = Canvas.GetLeft(element);
                double otherWidth = element.RenderSize.Width;

                if (Math.Abs(newLeft - otherLeft) < SnapThreshold)
                {
                    newLeft = otherLeft;
                    adorner?.AddSnapLine(DesignerCanvas.AlignmentType.Vertical, newLeft, canvas.ActualWidth, canvas.ActualHeight);
                }
                else if (Math.Abs(newLeft + width - (otherLeft + otherWidth)) < SnapThreshold)
                {
                    newLeft = otherLeft + otherWidth - width;
                    adorner?.AddSnapLine(DesignerCanvas.AlignmentType.Vertical, otherLeft + otherWidth, canvas.ActualWidth, canvas.ActualHeight);
                }
            }
        }
    }
}