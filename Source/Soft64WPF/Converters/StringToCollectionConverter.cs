using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace Soft64WPF.Converters
{
    public sealed class StringToCollectionConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return new List<String>();
            }
            else
            {
                String str = value as String;
                return str.ToCharArray();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}