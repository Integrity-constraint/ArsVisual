using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ArsVisual.Helpers
{
    public class ConnectionState
    {
        public Guid SourceId { get; set; }
        public Guid SinkId { get; set; }
        public string SourceConnectorName { get; set; }
        public string SinkConnectorName { get; set; }
        public ArrowSymbol SourceArrowSymbol { get; set; }
        public ArrowSymbol SinkArrowSymbol { get; set; }
        public DoubleCollection StrokeDashArray { get; set; }
        public string Text { get; set; }
        public int ZIndex { get; set; }
        public Color StrokeColor { get; set; }
    }
}
