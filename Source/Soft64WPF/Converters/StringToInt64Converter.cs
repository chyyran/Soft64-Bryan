using System;
using System.Globalization;
using System.Windows.Data;

namespace Soft64WPF.Converters
{
    public sealed class StringToInt64Converter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String str = value as String;

            if (str != null)
            {
                try
                {
                    if (str.Contains("0x"))
                    {
                        str = str.Remove(0, 2);
                    }

                    return Int64.Parse(str, NumberStyles.AllowHexSpecifier);
                }
                catch (FormatException)
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}