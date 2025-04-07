using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace ArsVisual.Helpers
{
    public static class VisualTreeExtensions
    {
        public static IEnumerable<T> GetVisualChildren<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    yield return typedChild;
                }

                foreach (var grandChild in child.GetVisualChildren<T>())
                {
                    yield return grandChild;
                }
            }
        }
    }
}
