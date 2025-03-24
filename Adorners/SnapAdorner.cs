using DiagramDesigner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Documents;

namespace ArsVisual.Adorners
{
    public class SnapAdorner : Adorner
    {
        private readonly List<Line> snapLines = new List<Line>();

        public SnapAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        public void AddSnapLine(DesignerCanvas.AlignmentType alignmentType, double position, double canvasWidth, double canvasHeight)
        {
            Line line = new Line
            {
                Stroke = (Brush)Application.Current.Resources["SnapAdornerColor"],
                StrokeThickness = 1
            };

            if (alignmentType == DesignerCanvas.AlignmentType.Horizontal)
            {
                line.X1 = 0;
                line.X2 = canvasWidth;
                line.Y1 = position;
                line.Y2 = position;
            }
            else
            {
                line.X1 = position;
                line.X2 = position;
                line.Y1 = 0;
                line.Y2 = canvasHeight;
            }

            snapLines.Add(line);
            InvalidateVisual();
        }

        public void ClearSnapLines()
        {
            snapLines.Clear();
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            foreach (var line in snapLines)
            {
                drawingContext.DrawLine(new Pen(line.Stroke, line.StrokeThickness),
                    new Point(line.X1, line.Y1), new Point(line.X2, line.Y2));
            }
        }
    }
}
