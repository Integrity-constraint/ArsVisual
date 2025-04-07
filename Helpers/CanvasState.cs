using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVisual.Helpers
{
   public class CanvasState
    {
        public List<DesignerItem> Items { get; set; } = new List<DesignerItem>();
        public List<ConnectionInfo> Connections { get; set; } = new List<ConnectionInfo>();
    }
}
