﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVisual.Helpers
{
    public class DesignerItemInfo
    {
        public Guid ID { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double RotationAngle { get; set; }
        public string ContentXaml { get; set; }
    }
}
