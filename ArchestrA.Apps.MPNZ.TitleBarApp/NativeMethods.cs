// ------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Schneider Electric Software, LLC">
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
    using System.Runtime.InteropServices;

    public static class NativeMethods
    {
        public const int LeftButtonDown = 0xA1;
        public const int WindowCaption = 0x2;

        /// <summary>
        /// This specific GUID below is required to invoke the TabTip
        /// </summary>
        [ComImport, Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface ITipInvocation
        {
            void Toggle(IntPtr hwnd);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1414:MarkBooleanPInvokeArgumentsWithMarshalAs", Justification = "Native method"), DllImport("User32.dll")]
        internal static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = false)]
        internal static extern IntPtr GetDesktopWindow();

        [ComImport, Guid("4ce576fa-83dc-4F88-951c-9d0782b4e376")]
        internal class UIHostNoLaunch
        {
        }
    }
}
