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
         public static readonly RoutedCommand ChangeFontForegroundCommand= new RoutedCommand("ChangeFontForegroundCommand", typeof(DesignerItemCommands));
         public static readonly RoutedCommand ChangeItemFillCommand= new RoutedCommand("ChangeItemFillCommand", typeof(DesignerItemCommands));
         public static readonly RoutedCommand ChangeItemStrokeCommand= new RoutedCommand("ChangeItemStrokeCommand", typeof(DesignerItemCommands));
    }
}
