using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace DiagramDesigner.Controls
{
    public class MoveThumb : Thumb
    {
        private RotateTransform rotateTransform;
        private ContentControl designerItem;

        public MoveThumb()
        {
            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = DataContext as ContentControl;

            if (this.designerItem != null)
            {
                this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
            }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null)
            {
                // Получить все выбранные объекты
                var designerCanvas = VisualTreeHelper.GetParent(this.designerItem) as DesignerCanvas;
                if (designerCanvas != null)
                {
                    var selectedItems = designerCanvas.GetSelectedItems();
                    foreach (var item in selectedItems)
                    {
                        var control = item as ContentControl;
                        if (control != null)
                        {
                            Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

                            if (this.rotateTransform != null)
                            {
                                dragDelta = this.rotateTransform.Transform(dragDelta);
                            }

                            Canvas.SetLeft(control, Canvas.GetLeft(control) + dragDelta.X);
                            Canvas.SetTop(control, Canvas.GetTop(control) + dragDelta.Y);
                        }
                    }
                }
            }
        }

    }
}
