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
                var designerCanvas = VisualTreeHelper.GetParent(this.designerItem) as DesignerCanvas;
                if (designerCanvas != null)
                {
                    var selectedItems = designerCanvas.GetSelectedItems().ToList();
                    designerCanvas.ClearSnapLines();

                    // Calculate base movement vector
                    Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);
                    if (this.rotateTransform != null)
                    {
                        dragDelta = this.rotateTransform.Transform(dragDelta);
                    }

                    // Get the bounds of the entire selection
                    Rect selectionBounds = GetSelectionBounds(selectedItems);

                    // Calculate proposed new position for the entire selection
                    double newLeft = selectionBounds.Left + dragDelta.X;
                    double newTop = selectionBounds.Top + dragDelta.Y;

                    // Snap the entire selection as a group
                    SnapSelectionToAlignment(designerCanvas, selectedItems, ref newLeft, ref newTop,
                                            selectionBounds.Width, selectionBounds.Height);

                    // Calculate final delta after snapping
                    double finalDeltaX = newLeft - selectionBounds.Left;
                    double finalDeltaY = newTop - selectionBounds.Top;

                    // Apply movement to all selected items
                    foreach (var item in selectedItems)
                    {
                        if (item is ContentControl control)
                        {
                            double itemLeft = Canvas.GetLeft(control);
                            double itemTop = Canvas.GetTop(control);

                            Canvas.SetLeft(control, itemLeft + finalDeltaX);
                            Canvas.SetTop(control, itemTop + finalDeltaY);
                        }
                    }
                }
            }
        }
        private Rect GetSelectionBounds(IEnumerable<UIElement> items)
        {
            Rect bounds = Rect.Empty;
            foreach (var item in items)
            {
                if (item is FrameworkElement element)
                {
                    double left = Canvas.GetLeft(element);
                    double top = Canvas.GetTop(element);
                    Rect itemRect = new Rect(left, top, element.ActualWidth, element.ActualHeight);

                    if (bounds.IsEmpty)
                        bounds = itemRect;
                    else
                        bounds.Union(itemRect);
                }
            }
            return bounds;
        }
        private void SnapSelectionToAlignment(DesignerCanvas canvas, List<UIElement> selectedItems,
                                    ref double newLeft, ref double newTop,
                                    double selectionWidth, double selectionHeight)
        {
            var otherElements = canvas.GetOtherElements(selectedItems);

            // Check horizontal alignment (top, bottom, center)
            CheckGroupAlignment(canvas, otherElements, ref newTop, selectionHeight,
                               DesignerCanvas.AlignmentType.Horizontal);

            // Check vertical alignment (left, right, center)
            CheckGroupAlignment(canvas, otherElements, ref newLeft, selectionWidth,
                               DesignerCanvas.AlignmentType.Vertical);
        }
      
        private void CheckGroupAlignment(DesignerCanvas canvas, IEnumerable<UIElement> otherElements,
                                 ref double newPos, double size,
                                 DesignerCanvas.AlignmentType alignmentType)
        {
            foreach (var other in otherElements)
            {
                double otherPos = alignmentType == DesignerCanvas.AlignmentType.Horizontal
                    ? Canvas.GetTop(other)
                    : Canvas.GetLeft(other);
                double otherSize = alignmentType == DesignerCanvas.AlignmentType.Horizontal
                    ? other.RenderSize.Height
                    : other.RenderSize.Width;

                // Варианты выравнивания: край к краю и центр к центру
                var alignmentOptions = new List<(double referencePos, double targetPos, string type)>
        {
            // Левый/верхний край с левым/верхним краем
            (newPos, otherPos, "edge"),
            
            // Правый/нижний край с правым/нижним краем
            (newPos + size, otherPos + otherSize, "edge"),
            
            // Центр с центром
            (newPos + size/2, otherPos + otherSize/2, "center"),
            
            // Левый/верхний край с правым/нижним краем
            (newPos, otherPos + otherSize, "opposite-edge"),
            
            // Правый/нижний край с левым/верхним краем
            (newPos + size, otherPos, "opposite-edge")
        };

                foreach (var option in alignmentOptions)
                {
                    double diff = Math.Abs(option.referencePos - option.targetPos);
                    if (diff < canvas.SnapThresholdValue)
                    {
                        switch (option.type)
                        {
                            case "edge":
                                // Выравнивание одинаковых краев
                                newPos = option.targetPos;
                                break;
                            case "opposite-edge":
                                // Выравнивание противоположных краев
                                if (option.referencePos == newPos) // Левый/верхний край
                                    newPos = option.targetPos;
                                else // Правый/нижний край
                                    newPos = option.targetPos - size;
                                break;
                            case "center":
                                // Центр с центром
                                newPos = option.targetPos - size / 2;
                                break;
                        }

                        canvas.DrawSnapLine(alignmentType, option.targetPos);
                        break;
                    }
                }
            }
        }
        private void CheckAlignment(DesignerCanvas canvas, ref double newLeft, ref double newTop,
            double draggedWidth, double draggedHeight, IEnumerable<UIElement> otherElements,
            DesignerCanvas.AlignmentType alignmentType)
        {
            double draggedPos = alignmentType == DesignerCanvas.AlignmentType.Horizontal ? newTop : newLeft;
            double draggedSize = alignmentType == DesignerCanvas.AlignmentType.Horizontal ? draggedHeight : draggedWidth;

            foreach (var other in otherElements)
            {
                double otherLeft = Canvas.GetLeft(other);
                double otherTop = Canvas.GetTop(other);
                double otherWidth = other.RenderSize.Width;
                double otherHeight = other.RenderSize.Height;

                List<(double target, string type)> targets = new List<(double, string)>();
                if (alignmentType == DesignerCanvas.AlignmentType.Horizontal)
                {
                    targets.Add((otherTop, "top"));
                    targets.Add((otherTop + otherHeight, "bottom"));
                    targets.Add((otherTop + otherHeight / 2, "center"));
                }
                else
                {
                    targets.Add((otherLeft, "left"));
                    targets.Add((otherLeft + otherWidth, "right"));
                    targets.Add((otherLeft + otherWidth / 2, "center"));
                }

                foreach (var (target, type) in targets)
                {
                    double currentPos = draggedPos;
                    if (type == "bottom") currentPos += draggedSize;
                    else if (type == "center") currentPos += draggedSize / 2;

                    double diff = Math.Abs(currentPos - target);
                    if (diff < canvas.SnapThresholdValue)
                    {
                        if (alignmentType == DesignerCanvas.AlignmentType.Horizontal)
                        {
                            newTop = type == "top" ? target
                                : type == "bottom" ? target - draggedSize
                                : target - draggedSize / 2;
                            canvas.DrawSnapLine(alignmentType, target);
                        }
                        else
                        {
                            newLeft = type == "left" ? target
                                : type == "right" ? target - draggedSize
                                : target - draggedSize / 2;
                            canvas.DrawSnapLine(alignmentType, target);
                        }
                        break;
                    }
                }
            }
        }
    }
}