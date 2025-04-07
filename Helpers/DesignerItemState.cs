using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ArsVisual.Helpers
{
    public class DesignerItemState
    {
        public Guid ID { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int ZIndex { get; set; }
        public string ContentXaml { get; set; }
        public double RotationAngle { get; set; }
        public Color StrokeColor { get; set; }
        public Color FillColor { get; set; }
        public double FontSize { get; set; }
        public string FontFamily { get; set; }
        public Color ForegroundColor { get; set; }
    }
}
