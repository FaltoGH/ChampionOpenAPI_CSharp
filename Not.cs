using System;
using System.Globalization;
using System.Windows.Data;

namespace ChampionOpenAPI_CSharp
{
    public class Not : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool?)
            {
                var x = (bool?)value;
                return !x.GetValueOrDefault();
            }
            if(value is bool)
            {
                var x = (bool)value;
                return !x;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
