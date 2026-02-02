using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquaClient.Converters
{
    public class InverseBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !(bool)(value ?? false);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !(bool)(value ?? false);
        }
    }

    public class DeviceTypeFilenameConverter : IValueConverter
    {
        static Dictionary<string, string> types = new()
        {
            ["sensor"] = "sensor_icon.png",
            ["switch"] = "switch_icon.png",
        };
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var type = (string)value;
            if (types.ContainsKey(type))
                return types[type];
            return "";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NumberBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var number = (int)value;
            return number != 0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0;

        }
    }

    public class StatusToButtonTextConverter : IValueConverter
    {
            public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var number = (int)value;
            return number != 0 ? "Выключить" : "Включить";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (string)value == "Выключить" ? 1: 0;
        }
    }

    public class StatusToButtonColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var number = (int)value;
            return number != 0 ? Colors.Lime : Colors.DarkGray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
