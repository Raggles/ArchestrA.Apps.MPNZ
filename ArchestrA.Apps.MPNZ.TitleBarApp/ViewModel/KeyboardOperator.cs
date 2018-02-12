// ------------------------------------------------------------------------
// <copyright file="KeyboardOperator.cs" company="Schneider Electric Software, LLC">
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
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using Diagnostics;

    /// <summary>
    /// The class handles the show and hide the touch keyboard
    /// </summary>
    internal static class KeyboardOperator
    {
        public static void Toggle()
        {
            try
            {
                StartKeyboardProcess();
            }
            catch (Exception e)
            {
                Logger.LogInfo(() => e.ToString());
            }
        }

        private static void StartKeyboardProcess()
        {
            string tabTipApplicationPath = Path.Combine(Environment.ExpandEnvironmentVariables("%ProgramW6432%"), @"Common Files\microsoft shared\ink", "TabTip.exe");
            if (File.Exists(tabTipApplicationPath))
            {
                LaunchProcessUsingShellExecute(tabTipApplicationPath);
                var tipInvocation = (NativeMethods.ITipInvocation)new NativeMethods.UIHostNoLaunch();

                if (tipInvocation != null)
                {
                    tipInvocation.Toggle(NativeMethods.GetDesktopWindow());
                    Marshal.ReleaseComObject(tipInvocation);
                }
            }
            else
            {
                var keyboardpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "osk.exe");
                LaunchProcessUsingShellExecute(keyboardpath);
            }
        }

        private static void LaunchProcessUsingShellExecute(string keyboardpath)
        {
            if (File.Exists(keyboardpath))
            {
                var procStartInfo = new ProcessStartInfo(keyboardpath)
                                    {
                                        UseShellExecute = true,
                                        CreateNoWindow = true
                                    };

                using (Process proc = new Process())
                {
                    proc.StartInfo = procStartInfo;
                    proc.Start();
                }
            }
        }
    }
}
