using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace Soft64.Toolkits.WPF
{
    public sealed class ByteToAsciiConverter : IValueConverter
    {
        #region IValueConverter Members

        private static readonly HashSet<Char> s_PrintableHashSet = new HashSet<char>((
            from index in Enumerable.Range(0, 128)
            let ch = (Char)index
            where Char.IsLetterOrDigit(ch) || Char.IsSymbol(ch) || Char.IsPunctuation(ch)
            select ch));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                Byte b = (Byte)value;

                if (s_PrintableHashSet.Contains((char)b))
                    return (char)b;
                else
                    return '.';
            }
            else
                return ".";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}