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
        private DesignerCanvas designerCanvas;

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
                this.designerCanvas = VisualTreeHelper.GetParent(this.designerItem) as DesignerCanvas;

                if (this.designerCanvas != null)
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

                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.designerCanvas);
                    if (adornerLayer != null)
                    {
                        this.adorner = new SizeAdorner(this.designerItem);
                        adornerLayer.Add(this.adorner);
                    }
                }
            }
        }

        void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            DesignerItem designerItem = this.DataContext as DesignerItem;
            DesignerCanvas designer = VisualTreeHelper.GetParent(designerItem) as DesignerCanvas;

            if (designerItem != null && designer != null && designerItem.IsSelected)
            {
                double minLeft, minTop, minDeltaHorizontal, minDeltaVertical;
                double dragDeltaVertical, dragDeltaHorizontal, scale;

                IEnumerable<DesignerItem> selectedDesignerItems = designer.SelectionService.CurrentSelection.OfType<DesignerItem>();

                CalculateDragLimits(selectedDesignerItems, out minLeft, out minTop,
                                    out minDeltaHorizontal, out minDeltaVertical);

                foreach (DesignerItem item in selectedDesignerItems)
                {
                    if (item != null && item.ParentID == Guid.Empty)
                    {
                        switch (base.VerticalAlignment)
                        {
                            case VerticalAlignment.Bottom:
                                dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                                scale = (item.ActualHeight - dragDeltaVertical) / item.ActualHeight;
                                DragBottom(scale, item, designer.SelectionService);
                                break;
                            case VerticalAlignment.Top:
                                double top = Canvas.GetTop(item);
                                dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                                scale = (item.ActualHeight - dragDeltaVertical) / item.ActualHeight;
                                DragTop(scale, item, designer.SelectionService);
                                break;
                            default:
                                break;
                        }

                        switch (base.HorizontalAlignment)
                        {
                            case HorizontalAlignment.Left:
                                double left = Canvas.GetLeft(item);
                                dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                                scale = (item.ActualWidth - dragDeltaHorizontal) / item.ActualWidth;
                                DragLeft(scale, item, designer.SelectionService);
                                break;
                            case HorizontalAlignment.Right:
                                dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                                scale = (item.ActualWidth - dragDeltaHorizontal) / item.ActualWidth;
                                DragRight(scale, item, designer.SelectionService);
                                break;
                            default:
                                break;
                        }
                    }
                }
                e.Handled = true;
            }
        }
        private void CalculateDragLimits(IEnumerable<DesignerItem> selectedItems, out double minLeft, out double minTop, out double minDeltaHorizontal, out double minDeltaVertical)
        {
            minLeft = double.MaxValue;
            minTop = double.MaxValue;
            minDeltaHorizontal = double.MaxValue;
            minDeltaVertical = double.MaxValue;

            // drag limits are set by these parameters: canvas top, canvas left, minHeight, minWidth
            // calculate min value for each parameter for each item
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
        private void DragTop(double scale, DesignerItem item, SelectionService selectionService)
        {
            IEnumerable<DesignerItem> groupItems = selectionService.GetGroupMembers(item).Cast<DesignerItem>();
            double groupBottom = Canvas.GetTop(item) + item.Height;
            foreach (DesignerItem groupItem in groupItems)
            {
                double groupItemTop = Canvas.GetTop(groupItem);
                double delta = (groupBottom - groupItemTop) * (scale - 1);
                Canvas.SetTop(groupItem, groupItemTop - delta);
                groupItem.Height = groupItem.ActualHeight * scale;
            }
        }

        private void DragRight(double scale, DesignerItem item, SelectionService selectionService)
        {
            IEnumerable<DesignerItem> groupItems = selectionService.GetGroupMembers(item).Cast<DesignerItem>();

            double groupLeft = Canvas.GetLeft(item);
            foreach (DesignerItem groupItem in groupItems)
            {
                double groupItemLeft = Canvas.GetLeft(groupItem);
                double delta = (groupItemLeft - groupLeft) * (scale - 1);

                Canvas.SetLeft(groupItem, groupItemLeft + delta);
                groupItem.Width = groupItem.ActualWidth * scale;
            }
        }

        private void DragBottom(double scale, DesignerItem item, SelectionService selectionService)
        {
            IEnumerable<DesignerItem> groupItems = selectionService.GetGroupMembers(item).Cast<DesignerItem>();
            double groupTop = Canvas.GetTop(item);
            foreach (DesignerItem groupItem in groupItems)
            {
                double groupItemTop = Canvas.GetTop(groupItem);
                double delta = (groupItemTop - groupTop) * (scale - 1);

                Canvas.SetTop(groupItem, groupItemTop + delta);
                groupItem.Height = groupItem.ActualHeight * scale;
            }
        }
        private void DragLeft(double scale, DesignerItem item, SelectionService selectionService)
        {
            IEnumerable<DesignerItem> groupItems = selectionService.GetGroupMembers(item).Cast<DesignerItem>();

            double groupLeft = Canvas.GetLeft(item) + item.Width;
            foreach (DesignerItem groupItem in groupItems)
            {
                double groupItemLeft = Canvas.GetLeft(groupItem);
                double delta = (groupLeft - groupItemLeft) * (scale - 1);
                Canvas.SetLeft(groupItem, groupItemLeft - delta);
                groupItem.Width = groupItem.ActualWidth * scale;
            }
        }
        private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this.adorner != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.designerCanvas);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this.adorner);
                }

                this.adorner = null;
            }
        }
    }
}
