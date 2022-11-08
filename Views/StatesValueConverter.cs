using System;
using System.Globalization;
using System.Windows.Data;

namespace MyEmmControl.Views
{
    public class StatesValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isConnected = System.Convert.ToBoolean(value);
            var flag = System.Convert.ToBoolean(parameter);//确是否转换
            return flag ? (isConnected ? "已连接" : "未连接")
                        : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
