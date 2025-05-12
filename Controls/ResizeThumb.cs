using ArsVisual.Adorners;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;
using static ArsVisual.DesignerCanvas;

namespace ArsVisual.Controls
{
    public class ResizeThumb : Thumb
    {
        private RotateTransform rotateTransform;
        private double angle;
        private Adorner adorner;
        private Point transformOrigin;
        private ContentControl designerItem;
        private DesignerCanvas designerCanvas;
        private List<DesignerItem> selectedItems;
        private bool isSingleItemResizing;

        public ResizeThumb()
        {
            DragStarted += ResizeThumb_DragStarted;
            DragDelta += ResizeThumb_DragDelta;
            DragCompleted += ResizeThumb_DragCompleted;
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            designerItem = DataContext as ContentControl;
            if (designerItem != null)
            {
                designerCanvas = VisualTreeHelper.GetParent(designerItem) as DesignerCanvas;
                if (designerCanvas != null)
                {
                    transformOrigin = designerItem.RenderTransformOrigin;
                    rotateTransform = designerItem.RenderTransform as RotateTransform;
                    angle = rotateTransform?.Angle * Math.PI / 180.0 ?? 0.0;

                    selectedItems = designerCanvas.SelectionService.CurrentSelection
                        .OfType<DesignerItem>()
                        .ToList();

                   
                    isSingleItemResizing = selectedItems.Count == 1;

                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(designerCanvas);
                    if (adornerLayer != null)
                    {
                        adorner = new SizeAdorner(designerItem);
                        adornerLayer.Add(adorner);
                    }
                }
            }
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (designerItem == null || designerCanvas == null) return;

            designerCanvas.ClearSnapLines();

            double minLeft, minTop, minDeltaHorizontal, minDeltaVertical;
            CalculateDragLimits(selectedItems, out minLeft, out minTop, out minDeltaHorizontal, out minDeltaVertical);

            foreach (var item in selectedItems)
            {
                double originalWidth = item.Width;
                double originalHeight = item.Height;
                double originalLeft = Canvas.GetLeft(item);
                double originalTop = Canvas.GetTop(item);

               
                if (VerticalAlignment == VerticalAlignment.Top || VerticalAlignment == VerticalAlignment.Bottom)
                {
                    double deltaVertical = 0;
                    double newTop = originalTop;
                    double newHeight = originalHeight;

                    if (VerticalAlignment == VerticalAlignment.Bottom)
                    {
                        deltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                        newTop = originalTop + (transformOrigin.Y * deltaVertical * (1 - Math.Cos(-angle)));
                        newHeight = originalHeight - deltaVertical;

                    
                        if (isSingleItemResizing)
                        {
                            SnapBottomEdge(designerCanvas, item, newTop + newHeight, ref newHeight);
                        }
                    }
                    else if (VerticalAlignment == VerticalAlignment.Top)
                    {
                        deltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                        newTop = originalTop + deltaVertical * Math.Cos(-angle) +
                                (transformOrigin.Y * deltaVertical * (1 - Math.Cos(-angle)));
                        newHeight = originalHeight - deltaVertical;

                    
                        if (isSingleItemResizing)
                        {
                            SnapTopEdge(designerCanvas, item, newTop, ref newTop, ref newHeight);
                        }
                    }

                    if (IsWithinBounds(newTop, originalLeft, originalWidth, newHeight))
                    {
                        item.Height = newHeight;
                        Canvas.SetTop(item, newTop);
                    }
                }

            
                if (HorizontalAlignment == HorizontalAlignment.Left || HorizontalAlignment == HorizontalAlignment.Right)
                {
                    double deltaHorizontal = 0;
                    double newLeft = originalLeft;
                    double newWidth = originalWidth;

                    if (HorizontalAlignment == HorizontalAlignment.Left)
                    {
                        deltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                        newLeft = originalLeft + deltaHorizontal * Math.Cos(angle) +
                                 (transformOrigin.X * deltaHorizontal * (1 - Math.Cos(angle)));
                        newWidth = originalWidth - deltaHorizontal;

                      
                        if (isSingleItemResizing)
                        {
                            SnapLeftEdge(designerCanvas, item, newLeft, ref newLeft, ref newWidth);
                        }
                    }
                    else if (HorizontalAlignment == HorizontalAlignment.Right)
                    {
                        deltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                        newWidth = originalWidth - deltaHorizontal;

                       
                        if (isSingleItemResizing)
                        {
                            SnapRightEdge(designerCanvas, item, newLeft + newWidth, ref newWidth);
                        }
                    }

                    if (IsWithinBounds(originalTop, newLeft, newWidth, originalHeight))
                    {
                        item.Width = newWidth;
                        Canvas.SetLeft(item, newLeft);
                    }
                }
            }
        }

        #region Snap Methods (for single item only)
        private void SnapBottomEdge(DesignerCanvas canvas, DesignerItem item, double bottomEdge, ref double height)
        {
            var otherElements = canvas.GetOtherElements(selectedItems);
            double bestDiff = canvas.SnapThresholdValue;
            double bestTarget = bottomEdge;
            bool foundSnap = false;

            foreach (var other in otherElements)
            {
                double otherTop = Canvas.GetTop(other);
                double otherBottom = otherTop + other.RenderSize.Height;

                foreach (var target in new[] { otherTop, otherBottom })
                {
                    double diff = Math.Abs(bottomEdge - target);
                    if (diff < bestDiff)
                    {
                        bestDiff = diff;
                        bestTarget = target;
                        foundSnap = true;
                    }
                }
            }

            if (foundSnap)
            {
                canvas.DrawSnapLine(AlignmentType.Horizontal, bestTarget);
                height = bestTarget - Canvas.GetTop(item);
            }
        }

        private void SnapTopEdge(DesignerCanvas canvas, DesignerItem item, double topEdge, ref double top, ref double height)
        {
            var otherElements = canvas.GetOtherElements(selectedItems);
            double bestDiff = canvas.SnapThresholdValue;
            double bestTarget = topEdge;
            bool foundSnap = false;

            foreach (var other in otherElements)
            {
                double otherTop = Canvas.GetTop(other);
                double otherBottom = otherTop + other.RenderSize.Height;

                foreach (var target in new[] { otherTop, otherBottom })
                {
                    double diff = Math.Abs(topEdge - target);
                    if (diff < bestDiff)
                    {
                        bestDiff = diff;
                        bestTarget = target;
                        foundSnap = true;
                    }
                }
            }

            if (foundSnap)
            {
                canvas.DrawSnapLine(AlignmentType.Horizontal, bestTarget);
                height += top - bestTarget;
                top = bestTarget;
            }
        }

        private void SnapLeftEdge(DesignerCanvas canvas, DesignerItem item, double leftEdge, ref double left, ref double width)
        {
            var otherElements = canvas.GetOtherElements(selectedItems);
            double bestDiff = canvas.SnapThresholdValue;
            double bestTarget = leftEdge;
            bool foundSnap = false;

            foreach (var other in otherElements)
            {
                double otherLeft = Canvas.GetLeft(other);
                double otherRight = otherLeft + other.RenderSize.Width;

                foreach (var target in new[] { otherLeft, otherRight })
                {
                    double diff = Math.Abs(leftEdge - target);
                    if (diff < bestDiff)
                    {
                        bestDiff = diff;
                        bestTarget = target;
                        foundSnap = true;
                    }
                }
            }

            if (foundSnap)
            {
                canvas.DrawSnapLine(AlignmentType.Vertical, bestTarget);
                width += left - bestTarget;
                left = bestTarget;
            }
        }

        private void SnapRightEdge(DesignerCanvas canvas, DesignerItem item, double rightEdge, ref double width)
        {
            var otherElements = canvas.GetOtherElements(selectedItems);
            double bestDiff = canvas.SnapThresholdValue;
            double bestTarget = rightEdge;
            bool foundSnap = false;

            foreach (var other in otherElements)
            {
                double otherLeft = Canvas.GetLeft(other);
                double otherRight = otherLeft + other.RenderSize.Width;

                foreach (var target in new[] { otherLeft, otherRight })
                {
                    double diff = Math.Abs(rightEdge - target);
                    if (diff < bestDiff)
                    {
                        bestDiff = diff;
                        bestTarget = target;
                        foundSnap = true;
                    }
                }
            }

            if (foundSnap)
            {
                canvas.DrawSnapLine(AlignmentType.Vertical, bestTarget);
                width = bestTarget - Canvas.GetLeft(item);
            }
        }
        #endregion

        private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (adorner != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(designerCanvas);
                adornerLayer?.Remove(adorner);
                adorner = null;
            }
        }

        private bool IsWithinBounds(double top, double left, double width, double height)
        {
            return top >= 0 && left >= 0 &&
                   (top + height) <= designerCanvas.ActualHeight &&
                   (left + width) <= designerCanvas.ActualWidth;
        }

        private void CalculateDragLimits(IEnumerable<DesignerItem> items,
                                       out double minLeft, out double minTop,
                                       out double minDeltaHorizontal, out double minDeltaVertical)
        {
            minLeft = minTop = minDeltaHorizontal = minDeltaVertical = double.MaxValue;

            foreach (var item in items)
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