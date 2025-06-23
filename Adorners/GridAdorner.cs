using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ArsVisual.Adorners
{
    public class GridAdorner: Adorner
    {
        private double gridSize;

        public GridAdorner(UIElement adornedElement, double gridSize = 20)
            : base(adornedElement)
        {
            this.gridSize = gridSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect bounds = new Rect(new Point(0, 0), AdornedElement.RenderSize);
            Pen pen = new Pen((Brush)Application.Current.Resources["GridLayer"], 1);

            for (double x = 0; x < bounds.Width; x += gridSize)
            {
                drawingContext.DrawLine(pen, new Point(x, 0), new Point(x, bounds.Height));
            }

            for (double y = 0; y < bounds.Height; y += gridSize)
            {
                drawingContext.DrawLine(pen, new Point(0, y), new Point(bounds.Width, y));
            }
           
        }
    }
}
