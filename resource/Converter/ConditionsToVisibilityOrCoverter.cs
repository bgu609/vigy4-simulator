using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Pa5455CmsResource.Converter
{
    public class ConditionsToVisibilityOrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // 두 개의 조건이 모두 true일 때만 Visible
            Visibility condition1 = (Visibility)values[0];
            Visibility condition2 = (Visibility)values[1];

            if (condition1 == Visibility.Visible || condition2 == Visibility.Visible)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed; // 또는 Visibility.Hidden
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
