using System;
using System.Globalization;
using MvvmCross.Platform.Converters;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Converters
{
    public class InverseBooleanValueConverter : MvxValueConverter<bool, bool>, IValueConverter
    {
        /// <summary>
        /// Converts the inverse of a boolean.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        protected override bool Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value;
        }

        /// <summary>
        /// Converts the inverse of a boolean back.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        protected override bool ConvertBack(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }
}