using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ArchestrA.Apps.MPNZ.AttributeBrowserApp
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class QualityToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public QualityToVisibilityConverter()
        {
            // set defaults
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int p = 0;
            if (!(value is int))
                return null;
            if (parameter is string)
            {
                if (!int.TryParse((string)parameter, out p))
                    return null;
            }
            else if (!(parameter is int))
            {
                return null;
            }
            else
            {
                p = (int)parameter;
            }
            return (int)value == p ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
