using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Soft64Binding.WPF
{
    public class RegisterValue : IDataErrorInfo, INotifyPropertyChanged
    {
        private String m_Value = "0000000000000000";

        public String Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public string Error
        {
            get { return null; }
        }

        internal UInt64 RegValue
        {
            get
            {
                UInt64 v = 0;
                UInt64.TryParse(m_Value, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out v);
                return v;
            }

            set
            {
                m_Value = value.ToString("X16");
                OnPropertyChange("Value");
            }
        }

        public string this[string columnName]
        {
            get 
            {
                UInt64 v = 0;

                if (columnName.Equals("Value"))
                {
                    if (UInt64.TryParse(m_Value, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out v))
                    {
                        return null;
                    }
                    else
                    {
                        return "Value must be in valid hexadecimal value, not including 0x prefix";
                    }
                }

                return null;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChange(String name)
        {
            var e = PropertyChanged;

            if (e != null)
            {
                e(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
