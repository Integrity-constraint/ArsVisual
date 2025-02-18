using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ArsVisual
{
    public static class DesignerItemCommands
    {
        public static readonly RoutedCommand ChangeFontSizeCommand = new RoutedCommand("ChangeFontSizeCommand", typeof(DesignerItemCommands));
        public static readonly RoutedCommand ChangeFontFamilyCommand = new RoutedCommand("ChangeFontFamilyCommand", typeof(DesignerItemCommands));
    }
}
