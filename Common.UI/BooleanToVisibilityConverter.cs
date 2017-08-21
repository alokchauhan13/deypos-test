using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Common.UI
{
    /// <summary>
    /// Class represent the converter from boolean to visibility
    /// If value is true then visibility is visible.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        #region Properties

        /// <summary>
        /// Property that determines if the converter is going to collapse or hide visibility
        /// </summary>
        public bool Collapse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is invert.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is invert; otherwise, <c>false</c>.
        /// </value>
        public bool IsInvert { get; set; }

        #endregion Properties

        #region Constructor
        /// <summary>
        /// Constructor for the class represent to converter from Boolean to visibility
        /// </summary>
        public BooleanToVisibilityConverter()
        {
            Collapse = true;
            IsInvert = false;
        }
        #endregion


        #region IValueConverter

        /// <summary>
        /// Convert from boolean to Visibility(if true then Visibility= Visibility.Visible else Visibility.Hidden)
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (IsInvert) value = !(bool)value;

            if ((bool)value)
                return Visibility.Visible;
            if (Collapse) return Visibility.Collapsed;

            return Visibility.Hidden;
        }

        /// <summary>
        /// Convert back from Visibility to boolean
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = false;
            if ((Visibility)value == Visibility.Visible)
                result = true;
            if (IsInvert) result = !result;

            return result;
        }

        #endregion IValueConverter Members

    }
}
