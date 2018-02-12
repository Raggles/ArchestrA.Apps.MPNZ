// ------------------------------------------------------------------------
// <copyright file="DisplayAreaConverter.cs" company="Schneider Electric Software, LLC">
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

    public class DisplayAreaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DisplayArea displayArea = DisplayArea.None;

            DisplayArea region = DisplayArea.None;
            if (value != null && Enum.TryParse(value.ToString(), out displayArea) && Enum.TryParse(parameter.ToString(), out region))
            {
                switch (region)
                {
                    case DisplayArea.PrimaryLeft:
                        return displayArea == DisplayArea.PrimaryLeft;
                    case DisplayArea.PrimaryCenter:
                        return displayArea == DisplayArea.PrimaryCenter;
                    case DisplayArea.PrimaryRight:
                        return displayArea == DisplayArea.PrimaryRight;
                    case DisplayArea.SecondaryLeft:
                        return displayArea == DisplayArea.SecondaryLeft;
                    case DisplayArea.SecondaryCenter:
                        return displayArea == DisplayArea.SecondaryCenter;
                    case DisplayArea.SecondaryRight:
                        return displayArea == DisplayArea.SecondaryRight;

                    default:
                        return false;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
