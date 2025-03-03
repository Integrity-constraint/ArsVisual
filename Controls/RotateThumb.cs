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
        // Для группового вращения сохраняем информацию об избранных элементах
        private Canvas designerCanvas;
        private List<DesignerItem> selectedItems;
        private Point groupCenter;
        private Vector startVector; // вектор от центра группы до точки нажатия мыши
        private Dictionary<DesignerItem, ItemRotationInfo> itemsOriginalInfo = new Dictionary<DesignerItem, ItemRotationInfo>();

        private class ItemRotationInfo
        {
            public Point OriginalCenter { get; set; }
            public double OriginalAngle { get; set; }
        }

        public RotateThumb()
        {
            DragStarted += RotateThumb_DragStarted;
            DragDelta += RotateThumb_DragDelta;
        }

        private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            // Используем DataContext элемента, на котором происходит вращение, как ссылку
            DesignerItem referenceItem = DataContext as DesignerItem;
            if (referenceItem == null) return;
            designerCanvas = VisualTreeHelper.GetParent(referenceItem) as Canvas;
            if (designerCanvas == null) return;

            // Получаем список всех выбранных элементов
            selectedItems = GetSelectedDesignerItems().ToList();
            if (selectedItems.Count == 0) return;

            // Вычисляем объединённый прямоугольник выбранных элементов
            double left = double.PositiveInfinity, top = double.PositiveInfinity;
            double right = double.NegativeInfinity, bottom = double.NegativeInfinity;
            itemsOriginalInfo.Clear();

            foreach (var item in selectedItems)
            {
                double itemLeft = Canvas.GetLeft(item);
                double itemTop = Canvas.GetTop(item);
                double itemRight = itemLeft + item.Width;
                double itemBottom = itemTop + item.Height;
                if (itemLeft < left) left = itemLeft;
                if (itemTop < top) top = itemTop;
                if (itemRight > right) right = itemRight;
                if (itemBottom > bottom) bottom = itemBottom;

                // Сохраняем исходный центр и угол поворота для каждого элемента
                Point origCenter = new Point(itemLeft + item.Width / 2, itemTop + item.Height / 2);
                double origAngle = 0.0;
                if (item.RenderTransform is RotateTransform rt)
                    origAngle = rt.Angle;
                itemsOriginalInfo[item] = new ItemRotationInfo { OriginalCenter = origCenter, OriginalAngle = origAngle };
            }

            // Группа: объединённый прямоугольник и его центр
            Rect groupBounds = new Rect(new Point(left, top), new Point(right, bottom));
            groupCenter = new Point(groupBounds.Left + groupBounds.Width / 2, groupBounds.Top + groupBounds.Height / 2);

            // Сохраняем начальный вектор от центра группы до точки нажатия мыши
            Point mousePos = Mouse.GetPosition(designerCanvas);
            startVector = Point.Subtract(mousePos, groupCenter);
        }

        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (designerCanvas == null || selectedItems == null || selectedItems.Count == 0)
                return;

            // Получаем текущую позицию мыши относительно контейнера
            Point currentMouse = Mouse.GetPosition(designerCanvas);
            Vector currentVector = Point.Subtract(currentMouse, groupCenter);
            // Определяем величину поворота (deltaAngle) в градусах для группы
            double deltaAngle = Vector.AngleBetween(startVector, currentVector);

            // Для каждого элемента вычисляем его новую позицию и угол
            Dictionary<DesignerItem, Rect> newItemBounds = new Dictionary<DesignerItem, Rect>();
            foreach (var item in selectedItems)
            {
                var info = itemsOriginalInfo[item];
                // Новая позиция центра = поворот исходного центра вокруг groupCenter на deltaAngle
                Point rotatedCenter = RotatePoint(info.OriginalCenter, groupCenter, deltaAngle);
                double newItemAngle = info.OriginalAngle + deltaAngle;
                // Новый верхний левый угол, если центр должен оставаться в центре элемента
                double newLeft = rotatedCenter.X - (item.Width / 2);
                double newTop = rotatedCenter.Y - (item.Height / 2);
                Rect newBounds = new Rect(newLeft, newTop, item.Width, item.Height);
                newItemBounds[item] = newBounds;
            }

            // Вычисляем объединённый прямоугольник для новой позиции группы
            Rect unionBounds = GetUnionRectangle(newItemBounds.Values);
            Rect containerRect = new Rect(0, 0, designerCanvas.ActualWidth, designerCanvas.ActualHeight);

            // Если новый объединённый прямоугольник полностью внутри контейнера, то применяем изменения
            if (containerRect.Contains(unionBounds))
            {
                foreach (var item in selectedItems)
                {
                    var info = itemsOriginalInfo[item];
                    Point rotatedCenter = RotatePoint(info.OriginalCenter, groupCenter, deltaAngle);
                    double newLeft = rotatedCenter.X - (item.Width / 2);
                    double newTop = rotatedCenter.Y - (item.Height / 2);
                    Canvas.SetLeft(item, newLeft);
                    Canvas.SetTop(item, newTop);
                    // Обновляем угол поворота элемента
                    RotateTransform rt = item.RenderTransform as RotateTransform;
                    if (rt == null)
                    {
                        rt = new RotateTransform();
                        item.RenderTransform = rt;
                        // Центр вращения для каждого элемента – его центр
                        item.RenderTransformOrigin = new Point(0.5, 0.5);
                    }
                    rt.Angle = info.OriginalAngle + deltaAngle;
                }
            }
            // Если новый поворот выходит за пределы, можно просто не обновлять позицию.
            // (Если нужно, можно вычислять максимально допустимый deltaAngle.)
        }

        // Вспомогательная функция для поворота точки вокруг опорной точки на заданный угол (в градусах)
        private Point RotatePoint(Point point, Point pivot, double angle)
        {
            double rad = angle * Math.PI / 180.0;
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);
            double dx = point.X - pivot.X;
            double dy = point.Y - pivot.Y;
            double rx = dx * cos - dy * sin;
            double ry = dx * sin + dy * cos;
            return new Point(pivot.X + rx, pivot.Y + ry);
        }

        // Вычисляет объединённый прямоугольник для набора прямоугольников
        private Rect GetUnionRectangle(IEnumerable<Rect> rects)
        {
            double left = double.PositiveInfinity;
            double top = double.PositiveInfinity;
            double right = double.NegativeInfinity;
            double bottom = double.NegativeInfinity;
            foreach (var rect in rects)
            {
                if (rect.Left < left) left = rect.Left;
                if (rect.Top < top) top = rect.Top;
                if (rect.Right > right) right = rect.Right;
                if (rect.Bottom > bottom) bottom = rect.Bottom;
            }
            return new Rect(new Point(left, top), new Point(right, bottom));
        }

        // Получаем выбранные элементы через SelectionService контейнера
        private IEnumerable<DesignerItem> GetSelectedDesignerItems()
        {
            DesignerCanvas designer = VisualTreeHelper.GetParent(DataContext as FrameworkElement) as DesignerCanvas;
            if (designer != null)
            {
                return designer.SelectionService.CurrentSelection.OfType<DesignerItem>();
            }
            return Enumerable.Empty<DesignerItem>();
        }
    }

}
