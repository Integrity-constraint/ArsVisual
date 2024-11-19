using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System;

namespace DiagramDesigner.Controls
{
    //TODO доработать логику, но уже круто
    public class RotateThumb : Thumb
    {
        private DesignerItem designerItem;
        private DesignerCanvas designerCanvas;
        private Point groupCenter;
        private Dictionary<DesignerItem, Point> initialPositions = new Dictionary<DesignerItem, Point>();
        private Dictionary<DesignerItem, double> initialAngles = new Dictionary<DesignerItem, double>();
        private double initialGroupAngle;
        private double startAngle;

        public RotateThumb()
        {
            DragStarted += new DragStartedEventHandler(RotateThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(RotateThumb_DragDelta);
        }

        private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = this.DataContext as DesignerItem;

            if (this.designerItem != null)
            {
                this.designerCanvas = VisualTreeHelper.GetParent(this.designerItem) as DesignerCanvas;

                if (this.designerCanvas != null)
                {
                    var selectedItems = this.designerCanvas.SelectionService.CurrentSelection.OfType<DesignerItem>().ToList();
                    if (selectedItems.Any())
                    {
                        // Определяем центр группы
                        double sumX = 0;
                        double sumY = 0;
                        initialPositions.Clear();
                        initialAngles.Clear();

                        foreach (var item in selectedItems)
                        {
                            double left = Canvas.GetLeft(item);
                            double top = Canvas.GetTop(item);
                            double centerX = left + item.ActualWidth / 2;
                            double centerY = top + item.ActualHeight / 2;
                            sumX += centerX;
                            sumY += centerY;

                            initialPositions[item] = new Point(centerX, centerY);

                            RotateTransform rotateTransform = item.RenderTransform as RotateTransform ?? new RotateTransform();
                            initialAngles[item] = rotateTransform.Angle;
                        }

                        groupCenter = new Point(sumX / selectedItems.Count, sumY / selectedItems.Count);

                        Point currentMousePosition = Mouse.GetPosition(designerCanvas);
                        Vector centerToMouse = Point.Subtract(currentMousePosition, groupCenter);

                        initialGroupAngle = Math.Atan2(centerToMouse.Y, centerToMouse.X) * (180 / Math.PI);
                    }
                }
            }
        }

        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null && this.designerCanvas != null)
            {
                var selectedItems = this.designerCanvas.SelectionService.CurrentSelection.OfType<DesignerItem>().ToList();
                if (selectedItems.Any())
                {
                    Point currentMousePosition = Mouse.GetPosition(designerCanvas);
                    Vector centerToMouse = Point.Subtract(currentMousePosition, groupCenter);

                    double currentAngle = Math.Atan2(centerToMouse.Y, centerToMouse.X) * (180 / Math.PI);
                    double deltaAngle = currentAngle - initialGroupAngle;

                    foreach (var item in selectedItems)
                    {
                        RotateTransform rotateTransform = item.RenderTransform as RotateTransform ?? new RotateTransform();
                        rotateTransform.Angle = initialAngles[item] + deltaAngle;
                        item.RenderTransform = rotateTransform;
                        item.RenderTransformOrigin = new Point(0.5, 0.5);

                        // Обновление позиции элемента после поворота
                        Point initialPosition = initialPositions[item];
                        Point rotatedPosition = RotatePoint(initialPosition, groupCenter, deltaAngle);

                        Canvas.SetLeft(item, rotatedPosition.X - item.ActualWidth / 2);
                        Canvas.SetTop(item, rotatedPosition.Y - item.ActualHeight / 2);
                    }
                }
            }
        }

        private Point RotatePoint(Point point, Point center, double angle)
        {
            double radians = angle * (Math.PI / 180);
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            double deltaX = point.X - center.X;
            double deltaY = point.Y - center.Y;

            double newX = center.X + (deltaX * cos - deltaY * sin);
            double newY = center.Y + (deltaX * sin + deltaY * cos);

            return new Point(newX, newY);
        }
    }
}
