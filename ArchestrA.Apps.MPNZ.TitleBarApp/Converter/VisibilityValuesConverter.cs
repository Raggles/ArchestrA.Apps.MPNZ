// ------------------------------------------------------------------------
// <copyright file="VisibilityValuesConverter.cs" company="Schneider Electric Software, LLC">
// Copyright (C) 2017 Schneider Electric Software, LLC.  All rights reserved.
//
// USE OF THIS SOFTWARE IS SUBJECT TO THE TERMS OF THE LICENSE AGREEMENT
// PROVIDED AT THE TIME OF INSTALLATION OR DOWNLOAD, OR WHICH OTHERWISE
// ACCOMPANIES THIS SOFTWARE IN EITHER ELECTRONIC OR HARD COPY FORM.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Schneider Electric, LLC</author>
// ------------------------------------------------------------------------

namespace ArchestrA.Apps.MPNZ.TitleBarApp
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// This class is used in setting Visibility of Restore or maximize button
    /// </summary>
    public class VisibilityValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Length > 0)
            {
                var expectedItemVisibility = (Visibility)values[0];
                if (expectedItemVisibility != Visibility.Visible)
                {
                    return Visibility.Collapsed;
                }

                var itemsLength = values.Length;
                for (int index = 1; index < itemsLength; index++)
                {
                    var itemVisibility = (Visibility)values[index];
                    if (itemVisibility == Visibility.Visible)
                    {
                        return Visibility.Visible;
                    }
                }
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
