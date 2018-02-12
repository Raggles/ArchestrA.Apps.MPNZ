// ------------------------------------------------------------------------
// <copyright file="WindowStateButtonVisibilityConverter.cs" company="Schneider Electric Software, LLC">
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
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// This class is used in setting Visibility of Restore or maximize button
    /// </summary>
    public class WindowStateButtonVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var returnValue = Visibility.Collapsed;

            // Initializing the parentWindowState to normal as WindowState will not be available when TitleBar is placed in Slide-in Pane
            WindowState parentWindowState = WindowState.Normal;

            // values[0] will be WindowState based on which button visibility is changed dynamically
            if (values[0].GetType() == typeof(WindowState))
            {
                parentWindowState = (WindowState)values[0];
            }

            // values[1] will be boolean values specifying whether button to be displayed or not
            var canMaximize = (bool)values[1];
            if (canMaximize)
            {
                if (parameter.Equals(WindowState.Normal))
                {
                    returnValue = (parentWindowState == WindowState.Maximized) ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (parameter.Equals(WindowState.Maximized))
                {
                    returnValue = (parentWindowState == WindowState.Normal) ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            return returnValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
