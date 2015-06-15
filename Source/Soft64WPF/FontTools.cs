using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Soft64WPF
{
    public static class FontTools
    {
        public static void RegisterFontChangeEvents(Type sourceType, EventHandler e)
        {
            DependencyProperty[] properties = new[]
                {
                    UserControl.FontFamilyProperty,
                    UserControl.FontSizeProperty,
                    UserControl.FontStretchProperty,
                    UserControl.FontStyleProperty,
                    UserControl.FontWeightProperty
                };

            foreach (var prop in properties)
            {
                prop.AddOwner(sourceType, new FrameworkPropertyMetadata((a, b) =>
                {
                    if (e != null)
                        e(a, new EventArgs());
                }));
            }
        }
    }
}
