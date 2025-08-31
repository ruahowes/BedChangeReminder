using System.Globalization;


namespace BedChangeReminder
{
    class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isFlipActive && parameter is string buttonType)
            {
                bool isActive = (buttonType == "Flip") ? isFlipActive : !isFlipActive;
                return isActive ? Colors.DarkBlue : Colors.LightGray;
            }
            return Colors.LightGray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
