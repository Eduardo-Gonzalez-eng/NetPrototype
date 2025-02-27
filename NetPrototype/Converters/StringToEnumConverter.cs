using NetPrototype.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NetPrototype.Converters
{
    public class StringToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
            {
                Array enumValues = Enum.GetValues(enumValue.GetType());
                return Array.IndexOf(enumValues, enumValue);
            }
            return -1;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int position)
            {
                Array enumValues = Enum.GetValues(targetType);
                if (position >= 0 && position < enumValues.Length)
                {
                    return enumValues.GetValue(position);
                }
            }
            return BufferProccesorMethod.Disable;
        }
    }
}
