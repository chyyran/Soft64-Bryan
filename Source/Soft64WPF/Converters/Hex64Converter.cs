using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Soft64WPF.Converters
{
    public sealed class Hex64Converter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (typeof(String).Equals(value.GetType()))
            {
                UInt64 v = UInt64.Parse(value as String, NumberStyles.AllowHexSpecifier);
                return v;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (typeof(UInt64).Equals(value.GetType()))
            {
                return ((UInt64)value).ToString("X16");
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
