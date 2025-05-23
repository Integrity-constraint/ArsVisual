using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVisual.Helpers
{
    public class UndoState
    {
        public List<DesignerItemState> Items { get; set; } = new List<DesignerItemState>();
        public List<ConnectionState> Connections { get; set; } = new List<ConnectionState>();
    }
}
