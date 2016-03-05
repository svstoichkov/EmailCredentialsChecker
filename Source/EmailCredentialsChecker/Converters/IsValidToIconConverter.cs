namespace EmailCredentialsChecker.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using MaterialDesignThemes.Wpf;

    public class IsValidToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return PackIconKind.HelpCircle;
            }

            if ((bool) value)
            {
                return PackIconKind.CheckAll;
            }

            return PackIconKind.CloseCircleOutline;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
