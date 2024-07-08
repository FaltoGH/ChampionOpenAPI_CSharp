using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace QuickZip.Tools
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        // source: https://www.codeproject.com/Articles/31947/WPF-AutoComplete-Folder-TextBox

        public static BoolToVisibilityConverter Instance = new BoolToVisibilityConverter();

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return Visibility.Visible;

            if ((bool)value)
                return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Visibility))
                return true;

            return (Visibility)value == Visibility.Visible;
        }

        #endregion
    }
}
