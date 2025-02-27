using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace NetPrototype.Converters
{
    public class BoolToBrushButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // El parámetro es opcional; usar LightGray como color por defecto
            var trueColor = parameter?.ToString() ?? "#0861F2";

            if (value is bool && (bool)value)
            {
                // Intentar convertir el nombre del color a un Brush
                if (ColorConverter.ConvertFromString(trueColor) is Color color)
                {
                    return new SolidColorBrush(color);
                }
                else
                {
                    return Brushes.LightGray; // Color por defecto si la conversión falla
                }
            }

            return new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#e7e7e7")));    // Color por defecto cuando el valor booleano es false
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
