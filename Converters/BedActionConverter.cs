using BedChangeReminder.Models;
using System.Globalization;


namespace BedChangeReminder.Converters
{
    public class BedActionConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is BedAction selectedAction && parameter is string buttonType)
            {
                var buttonAction = buttonType switch
                {
                    "None" => BedAction.None,
                    "Flip" => BedAction.Flip,
                    "Rotate" => BedAction.Rotate,
                    _ => BedAction.None
                };

                return selectedAction == buttonAction ? Colors.DarkBlue : Colors.LightGray;
            }
            return Colors.LightGray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
