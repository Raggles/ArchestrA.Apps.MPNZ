// ------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Schneider Electric Software, LLC">
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

// ================================================  IMPORTANT NOTES ==================================================================================
//
// This is a shared code linked to multiple projects. Changing this source code will impact all projects consuming this code.
// 
// New capabilities added to this version:
//    - All log APIs are optimized to log to ArchestrA Logger only if the corresponding log flag is enabled. Also the log message 
//      is fetched from the calling method using a delegate and the message delegate will be invoked only if the corresponding log flag is enabled. 
//      This allows to skip the log message formatting logic if corresponding log flag is not enabled.
// 
//    - Log flag states are updated automatically and responds to the latest log flag states when changing the log flag status from the ArchestrA 
//      Logger console. This allows to enable or disable log flags in the ArchestrA Logger SMC console. All Logger APIs respond to the current state
//      of a given log flag. Note that these are in-memory flags and checking these flags do not affect performance.
// 
//    - Added ability to append call stack with information such as the method name, line number and the source code file path. This information 
//      is appended to every log message when the custom log flag named "IncludeCallerInfo" is turned ON in the SMC console. Note that by default
//      the log flag "IncludeCallerInfo" is OFF. So, in order to see additional details about logged messages, the IncludeCallerInfo log flag has
//      to be manually turned on from the ArchestrA Logger SMC console. This flag can be turned ON for a given component or for all components globally.
//      In order to output the source code file name and line number, pdb files need to be present along with dlls. If pdb file is not present then only 
//      the fully qualified class name will be available in the call stack.
// 
// Best Practices on using these APIs:
//    - LogError(), LogWarning() and LogInfo(): Message logged by these API's are seen by users of the application. Ensure the message specified when calling 
//      these log APIs make sense to the users. Ensure to have proper context in the message that help the user to diagnose and fix any issues. 
//      Note that the log flags corresponding to these log APIs are enabled by default in the SMC logger console.  
// 
//    - LogTrace(), LogCustom(), LogEntry(), LogExit(), LogSQL() and LogConnection(): Message logged by these API's are typically used by R&D and technical support.
//      These messages can have more technical details and should have context information that help diagnose.
//      Note that the log flags corresponding to these log APIs are NOT enabled by default in the SMC logger console.  
// 
// Sample Code Snippets:
// 
//      // Formatted message
//      Logger.LogError(() => string.Format("Failed to deploy ViewApp {0} to node {1}. Could not reach the target machine {2}", viewAppName, nodeName, targetMachineName));
// 
//      // Multi-line statement to compute the message 
//      Logger.LogError(() =>
//      {
//          int number = 25;
//          return string.Format("Sample formatted string: {0}, {1}", str, number);
//      });
// 
//      // Message formatted in a separate method 
//      Logger.LogError(() => GetMessage());
// 
//      // Simple message
//      Logger.LogError(() => "An error message");

namespace ArchestrA.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Win32;

    internal static class Logger
    {
        #region Public API's
        /// <summary>
        /// Logs an error message. Use the getMessage callback to pass in the message to log. 
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled in the ArchestrA Logger Console.</param>
        /// <param name="exception">Exception if exception details need to to be logged.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        public static void LogError(Func<string> getMessage, Exception exception = null)
        {
            if (ArchestrALogger.IsInstalled)
            {
                ArchestrALogger.LogError(getMessage, exception);
            }
            else
            {
                // On applications that run standalone without ArchestrA, log the message to the windows event logger (ETW)
                WindowsLogger.WriteEntry(getMessage(), EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Logs a warning message. Use the getMessage callback to pass in the message to log. 
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled in the ArchestrA Logger Console.</param>
        /// <param name="exception">Exception if exception details need to to be logged.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        public static void LogWarning(Func<string> getMessage, Exception exception = null)
        {
            if (ArchestrALogger.IsInstalled)
            {
                ArchestrALogger.LogWarning(getMessage, exception);
            }
            else
            {
                // On applications that run standalone without ArchestrA, log the message to the windows event logger (ETW)
                WindowsLogger.WriteEntry(getMessage(), EventLogEntryType.Warning);
            }
        }

        /// <summary>
        /// Logs an info message. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled in the ArchestrA Logger Console.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        public static void LogInfo(Func<string> getMessage)
        {
            if (ArchestrALogger.IsInstalled)
            {
                ArchestrALogger.LogInfo(getMessage);
            }
            else
            {
                // On applications that run standalone without ArchestrA, log the message to the windows event logger (ETW)
                WindowsLogger.WriteEntry(getMessage(), EventLogEntryType.Information);
            }
        }

        /// <summary>
        /// Logs a trace message. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled in the ArchestrA Logger Console.</param>
        /// <param name="exception">Exception if exception details need to to be logged.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        public static void LogTrace(Func<string> getMessage, Exception exception = null)
        {
            if (ArchestrALogger.IsInstalled)
            {
                ArchestrALogger.LogTrace(getMessage, exception);
            }
            else
            {
                // On applications that run standalone without ArchestrA, log the message to the windows event logger (ETW)
                WindowsLogger.WriteEntry(getMessage(), EventLogEntryType.Information);
            }
        }

        /// <summary>
        /// Logs a custom message. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="customLogHandle">Handle to the custom log flag. Pass the handle returned from the call to RegisterCustomLogFlag().</param>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled in the ArchestrA Logger Console.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        public static void LogCustom(IntPtr customLogHandle, Func<string> getMessage)
        {
            if (ArchestrALogger.IsInstalled)
            {
                ArchestrALogger.LogCustom(customLogHandle, getMessage);
            }
            else
            {
                // On applications that run standalone without ArchestrA, log the message to the windows event logger (ETW)
                WindowsLogger.WriteEntry(getMessage(), EventLogEntryType.Information);
            }
        }

        /// <summary>
        /// Logs a message. Use this mehtod to log a message at the begining of a method. Use the getMessage callback to pass in contextual information that will be useful for diagnostics.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled in the ArchestrA Logger Console.</param>
        /// <param name="callerName">Leave callerName empty.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        public static void LogEntry(Func<string> getMessage, [CallerMemberName] string callerName = "")
        {
            if (ArchestrALogger.IsInstalled)
            {
                const string Prefix = "Entered";
                ArchestrALogger.LogEntryExit(getMessage, Prefix, callerName);
            }
            else
            {
                // On applications that run standalone without ArchestrA, log the message to the windows event logger (ETW)
                WindowsLogger.WriteEntry(getMessage(), EventLogEntryType.Information);
            }
        }

        /// <summary>
        /// Logs a message. Use this mehtod to log a message before existing a method. Use the getMessage callback to pass in contextual information that will be useful for diagnostics.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled in the ArchestrA Logger Console.</param>
        /// <param name="callerName">Leave callerName empty.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        public static void LogExit(Func<string> getMessage, [CallerMemberName] string callerName = "")
        {
            if (ArchestrALogger.IsInstalled)
            {
                const string Prefix = "Exiting";
                ArchestrALogger.LogEntryExit(getMessage, Prefix, callerName);
            }
            else
            {
                // On applications that run standalone without ArchestrA, log the message to the windows event logger (ETW)
                WindowsLogger.WriteEntry(getMessage(), EventLogEntryType.Information);
            }
        }

        /// <summary>
        /// Logs a message related to SQL. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled in the ArchestrA Logger Console.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        public static void LogSQL(Func<string> getMessage)
        {
            if (ArchestrALogger.IsInstalled)
            {
                ArchestrALogger.LogSQL(getMessage);
            }
            else
            {
                // On applications that run standalone without ArchestrA, log the message to the windows event logger (ETW)
                WindowsLogger.WriteEntry(getMessage(), EventLogEntryType.Information);
            }
        }

        /// <summary>
        /// Logs a message related to connection and disconnection of services. Use the getMessage callback to pass in the message to log.
        /// </summary>
        /// <param name="getMessage">Delegate that will be called to get a string to log. Note that this delegate is called only when the log flag corresponding to the calling component is enabled in the ArchestrA Logger Console.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        public static void LogConnection(Func<string> getMessage)
        {
            if (ArchestrALogger.IsInstalled)
            {
                ArchestrALogger.LogConnection(getMessage);
            }
            else
            {
                // On applications that run standalone without ArchestrA, log the message to the windows event logger (ETW)
                WindowsLogger.WriteEntry(getMessage(), EventLogEntryType.Information);
            }
        }

        /// <summary>
        /// Logs a message related to dispose related issues. This method is typically used from the dispose method of a class when an instance of that class is not properly disposed.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        public static void LogDispose(string className)
        {
            const string DisposeMessageFormat = "Failed to call Dispose() for class {0}. Clean up of unmanaged resource was deferred to garbage collector.";

            // Dispose issues are treated as warnings
            LogWarning(() => string.Format(CultureInfo.InvariantCulture, DisposeMessageFormat, className), null);
        }

        /// <summary>
        /// Registers a custom flag with ArchestrA logger. 
        /// </summary>
        /// <param name="customLogFlagName">Name of the custom flag.</param>
        /// <returns>Retruns the handle to the custom flag. Note that registering the same name multiple time will return the same handle.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        public static IntPtr RegisterCustomLogFlag(string customLogFlagName)
        {
            if (string.IsNullOrEmpty(customLogFlagName))
            {
                return IntPtr.Zero;
            }

            return ArchestrALogger.RegisterCustomLogFlag(customLogFlagName);
        }

        /// <summary>
        /// Sets the identity name for the calling component. Note that the identity name is automatically set at the time of initialization and generally it is 
        /// not required to call this API. Identity name shows as the component name in the ArchestrA logger.
        /// </summary>
        /// <param name="identityName">Identity name for the component</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        public static void SetIndentityName(string identityName)
        {
            if (!string.IsNullOrEmpty(identityName))
            {
                ArchestrALogger.LogSetIdentityName(identityName);
            }
        }

        #endregion

        #region Private methods

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
        private static string FormatMessageWithCallerInfo(string message)
        {
            StackTrace callstack = new StackTrace(3, true); // skip top 3 levels of frame which are in this logger class
            string callerInfo = string.Format(CultureInfo.InvariantCulture, " \nCall Stack: \n{0}", callstack);

            return string.IsNullOrEmpty(message) ? callerInfo : message + callerInfo;
        }

        #endregion

        #region Supporting classes

        /// <summary>
        ///  ArchestrA Logger Wrapper class. Important note: This class should not be used in consuming assemblies and hence marked as private.
        /// </summary>
        private static class ArchestrALogger
        {
            #region Fields

            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Avoid property for performance. This feild is visible only to the parent class")]
            internal static bool IsInstalled = IsLoggerInstalled();

            private static int identity = 0;

            private static bool checkRegistry = true;

            private static IntPtr[] logFlags = new IntPtr[11];

            private static IntPtr includeCallerInfoLogFlag;

            private static object lockObj = new object();

            private static bool isLoaded;

            private static bool domainUnloaded;

            #endregion

            #region Internal Properties

            /// <summary>
            ///     Gets ErrorCount.
            /// </summary>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
                Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static int ErrorCount
            {
                get
                {
                    if (InitLoggerDll())
                    {
                        int errorCount = 0, warningCount = 0;
                        long lastError = 0, lastWarning = 0;

                        int retVal = NativeMethods.GetLoggerStats(
                            string.Empty,
                            ref errorCount,
                            ref lastError,
                            ref warningCount,
                            ref lastWarning);

                        return (retVal > 0) ? errorCount : -1;
                    }

                    return -1;
                }
            }

            /// <summary>
            ///     Gets WarningCount.
            /// </summary>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static int WarningCount
            {
                get
                {
                    if (InitLoggerDll())
                    {
                        int errorCount = 0, warningCount = 0;
                        long lastError = 0, lastWarning = 0;

                        int retVal = NativeMethods.GetLoggerStats(
                            string.Empty,
                            ref errorCount,
                            ref lastError,
                            ref warningCount,
                            ref lastWarning);

                        return (retVal > 0) ? warningCount : -1;
                    }

                    return -1;
                }
            }

            #endregion

            #region Log Methods

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static void LogError(Func<string> getMessage, Exception exception)
            {
                if (identity == 0 && !Initialize())
                {
                    return;
                }

                if (NativeMethods.LogFlagLog(identity, logFlags[LogFlagCategory.LOG_ERROR], 0))
                {
                    string message = FormatMessageWithException(getMessage, exception);
                    NativeMethods.LogError(identity, NativeMethods.LogFlagLog(identity, includeCallerInfoLogFlag, 0) ? FormatMessageWithCallerInfo(message) : message);
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static void LogWarning(Func<string> getMessage, Exception exception)
            {
                if (identity == 0 && !Initialize())
                {
                    return;
                }

                if (NativeMethods.LogFlagLog(identity, logFlags[LogFlagCategory.LOG_WARNING], 0))
                {
                    string message = FormatMessageWithException(getMessage, exception);
                    NativeMethods.LogWarning(identity, NativeMethods.LogFlagLog(identity, includeCallerInfoLogFlag, 0) ? FormatMessageWithCallerInfo(message) : message);
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static void LogInfo(Func<string> getMessage)
            {
                if (identity == 0 && !Initialize())
                {
                    return;
                }

                if (getMessage != null && NativeMethods.LogFlagLog(identity, logFlags[LogFlagCategory.LOG_INFO], 0))
                {
                    NativeMethods.LogInfo(identity, NativeMethods.LogFlagLog(identity, includeCallerInfoLogFlag, 0) ? FormatMessageWithCallerInfo(getMessage()) : getMessage());
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static void LogTrace(Func<string> getMessage, Exception exception)
            {
                if (identity == 0 && !Initialize())
                {
                    return;
                }

                if (NativeMethods.LogFlagLog(identity, logFlags[LogFlagCategory.LOG_TRACE], 0))
                {
                    string message = FormatMessageWithException(getMessage, exception);
                    NativeMethods.LogTrace(identity, NativeMethods.LogFlagLog(identity, includeCallerInfoLogFlag, 0) ? FormatMessageWithCallerInfo(message) : message);
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static void LogConnection(Func<string> getMessage)
            {
                if (identity == 0 && !Initialize())
                {
                    return;
                }

                if (getMessage != null && NativeMethods.LogFlagLog(identity, logFlags[LogFlagCategory.LOG_CONNECTION], 0))
                {
                    NativeMethods.LogConnection(identity, NativeMethods.LogFlagLog(identity, includeCallerInfoLogFlag, 0) ? FormatMessageWithCallerInfo(getMessage()) : getMessage());
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static void LogCustom(IntPtr customFlag, Func<string> getMessage)
            {
                if (identity == 0 && !Initialize())
                {
                    return;
                }

                if (getMessage != null && NativeMethods.LogFlagLog(identity, customFlag, 0))
                {
                    NativeMethods.LogCustom(identity, customFlag, NativeMethods.LogFlagLog(identity, includeCallerInfoLogFlag, 0) ? FormatMessageWithCallerInfo(getMessage()) : getMessage());
                }
            }

            [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "ArchestrA.Diagnostics.Logger+NativeMethods.LogEntryExit(System.Int32,System.String)", Justification = "Log messages use invariant culture.")]
            [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "ArchestrA.Diagnostics.Logger.FormatMessageWithCallerInfo(System.String)", Justification = "Log messages use invariant culture.")]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static void LogEntryExit(Func<string> getMessage, string prefix, string callerName)
            {
                if (identity == 0 && !Initialize())
                {
                    return;
                }

                if (getMessage != null && NativeMethods.LogFlagLog(identity, logFlags[LogFlagCategory.LOG_ENTRYEXIT], 0))
                {
                    string message = getMessage();
                    const string FormatString = "{0} method {1}. {2}";

                    message = string.Format(CultureInfo.InvariantCulture, FormatString, prefix, callerName, string.IsNullOrEmpty(message) ? string.Empty : message);

                    NativeMethods.LogEntryExit(identity, NativeMethods.LogFlagLog(identity, includeCallerInfoLogFlag, 0) ? FormatMessageWithCallerInfo(message) : message);
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static void LogSQL(Func<string> getMessage)
            {
                if (identity == 0 && !Initialize())
                {
                    return;
                }

                if (getMessage != null && NativeMethods.LogFlagLog(identity, logFlags[LogFlagCategory.LOG_SQL], 0))
                {
                    NativeMethods.LogSQL(identity, NativeMethods.LogFlagLog(identity, includeCallerInfoLogFlag, 0) ? FormatMessageWithCallerInfo(getMessage()) : getMessage());
                }
            }

            #endregion

            #region Initialization Methods

            [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "ArchestrA.Diagnostics.Logger+NativeMethods.SetIdentityName(System.Int32,System.String)", Justification = "Method does not have to be used by all clients of the Logger.")]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static void LogSetIdentityName(string identityName)
            {
                if (identity == 0 && !Initialize())
                {
                    return;
                }

                int rc = NativeMethods.SetIdentityName(identity, identityName);
                if (rc != 0)
                {
                    // Nothing for now.
                }
            }

            [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag", Justification = "Can't rename because of legacy reasons")]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static IntPtr RegisterCustomLogFlag(string flagName)
            {
                return Initialize() ? NativeMethods.RegisterLogFlag(identity, LogFlagCategory.LOG_CUSTOM, flagName) : IntPtr.Zero;
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static void ResetLoggerCheck()
            {
                checkRegistry = true;
            }

            #endregion

            #region Private Methods

            [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            private static bool InitLoggerDll()
            {
                if (isLoaded)
                {
                    return true;
                }

                if (checkRegistry)
                {
                    IntPtr libHandle = IntPtr.Zero;

                    // Default behavior: we do not wish to repeatedly check registry if DLL is not found first time
                    // so we check only once. Thereafter, we assume the DLL will never be there.
                    checkRegistry = false;

                    // NOTE: the following try/finally block is required for assemblies running in  
                    // partial trust environments (ActiveFactory RWS IE download is an example)
                    // Keep the registry permission demand from walking up the call stack
                    new RegistryPermission(RegistryPermissionAccess.Read, @"HKEY_LOCAL_MACHINE\Software\ArchestrA\Framework\Logger").Assert();

                    try
                    {
                        RegistryKey loggerKey = Registry.LocalMachine.OpenSubKey("Software\\ArchestrA\\Framework\\Logger", false);

                        if (loggerKey != null)
                        {
                            string loggerInstallPath = Convert.ToString(loggerKey.GetValue("InstallPath", string.Empty), CultureInfo.InvariantCulture);

                            if (loggerInstallPath.Length > 0)
                            {
                                libHandle = NativeMethods.LoadLibraryW(Path.Combine(loggerInstallPath, "LoggerDll.dll"));
                            }

                            loggerKey.Close();
                        }
                        else
                        {
                            bool is32BitLoggerExists = false;
                            using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                            {
                                using (RegistryKey key = baseKey.OpenSubKey(@"SOFTWARE\ArchestrA\Framework\Logger", false))
                                {
                                    is32BitLoggerExists = key != null && key.GetValue("InstallPath") != null &&
                                                          !string.IsNullOrEmpty(key.GetValue("InstallPath").ToString());
                                }
                            }

                            if (is32BitLoggerExists)
                            {
                                string loggerInstallPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                                string[] filesList = Directory.GetFiles(loggerInstallPath, "LoggerDll.dll", SearchOption.AllDirectories);
                                if (filesList.Length > 0)
                                {
                                    libHandle = NativeMethods.LoadLibraryW(filesList[0]);
                                }
                            }
                        }
                    }
                    finally
                    {
                        CodeAccessPermission.RevertAssert();
                    }

                    isLoaded = libHandle != IntPtr.Zero;
                }

                return isLoaded;
            }

            [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "ArchestrA.Diagnostics.Logger+NativeMethods.SetIdentityName(System.Int32,System.String)", Justification = "Unused in the logic")]
            private static bool Initialize()
            {
                if (domainUnloaded)
                {
                    return false;
                }

                if (identity != 0)
                {
                    return true;
                }

                lock (lockObj)
                {
                    if (identity == 0 && InitLoggerDll())
                    {
                        int mh = 0;
                        int rc = NativeMethods.RegisterLoggerClient(ref mh);
                        identity = mh;

                        if (rc != 0 && identity != 0)
                        {
                            // NOTE: the following try/finally block is required for assemblies running in  
                            // partial trust environments (ActiveFactory RWS IE download is an example)
                            // Need to assert PathDiscovery, but need arbitrary path
                            // I'm asserting too much trust, but I have not idea where the
                            // path is....
                            new FileIOPermission(PermissionState.Unrestricted).Assert();

                            try
                            {
                                NativeMethods.SetIdentityName(identity, Assembly.GetExecutingAssembly().GetName().Name);

                                InitializeLogsFlags();
                            }
                            finally
                            {
                                CodeAccessPermission.RevertAssert();
                            }

                            AppDomain.CurrentDomain.DomainUnload += OnCurrentDomainUnload;
                            return true;
                        }
                    }
                }

                return false;
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            private static void InitializeLogsFlags()
            {
                if (NativeMethods.LogStart(identity) > 0)
                {
                    for (var i = 0; i < logFlags.Length; i++)
                    {
                        logFlags[i] = NativeMethods.GetLogFlag(identity, i);
                    }

                    // Initialize the custom log flag for logging additional caller info along with log messages
                    const string CallerInfoLogFlagName = "IncludeCallerInfo";
                    includeCallerInfoLogFlag = Logger.RegisterCustomLogFlag(CallerInfoLogFlagName);
                }
            }

            private static void OnCurrentDomainUnload(object sender, EventArgs e)
            {
                UnInitialize();
                domainUnloaded = true;
            }

            [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "ArchestrA.Diagnostics.Logger+NativeMethods.UnregisterLoggerClient(System.Int32)", Justification = "Method does not have to be used by all clients of the Logger.")]
            private static void UnInitialize()
            {
                if (identity == 0)
                {
                    return;
                }

                int rc = NativeMethods.UnregisterLoggerClient(identity);
                if (rc != 0)
                {
                    // Nothing for now.
                }

                identity = 0;
            }

            private static bool IsLoggerInstalled()
            {
                bool is64BitLoggerExists;
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\ArchestrA\Framework\Logger", false))
                {
                    is64BitLoggerExists = regKey != null && regKey.GetValue("InstallPath") != null &&
                                          !string.IsNullOrEmpty(regKey.GetValue("InstallPath").ToString());
                }

                if (!is64BitLoggerExists)
                {
                    bool is32BitLoggerExists;
                    RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                    using (RegistryKey loggerKey = baseKey.OpenSubKey(@"SOFTWARE\ArchestrA\Framework\Logger", false))
                    {
                        is32BitLoggerExists = loggerKey != null && loggerKey.GetValue("InstallPath") != null &&
                                              !string.IsNullOrEmpty(loggerKey.GetValue("InstallPath").ToString());
                    }

                    return is32BitLoggerExists;
                }

                return true;
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            private static string FormatMessageWithException(Func<string> getMessage, Exception exception)
            {
                string message = getMessage != null ? getMessage() : string.Empty;

                if (!string.IsNullOrEmpty(message))
                {
                    if (exception != null)
                    {
                        message = string.Format(CultureInfo.InvariantCulture, "{0} \n{1}", message, exception);
                    }
                }
                else if (exception != null)
                {
                    message = exception.ToString();
                }

                return message;
            }

            #endregion
        }

        /// <summary>
        /// ArchestrA log flags category.Private, not exposed to consumers.
        /// </summary>
        private static class LogFlagCategory
        {
            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Native code")]
            internal const int LOG_ERROR = 0;

            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Native code")]
            internal const int LOG_WARNING = 1;

            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Native code")]
            internal const int LOG_INFO = 2;

            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Native code")]
            internal const int LOG_TRACE = 3;

            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Native code")]
            internal const int LOG_STARTSTOP = 4;

            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Native code")]
            internal const int LOG_ENTRYEXIT = 5;

            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Native code")]
            internal const int LOG_THREADSTARTSTOP = 6;

            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Native code")]
            internal const int LOG_SQL = 7;

            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Native code")]
            internal const int LOG_CONNECTION = 8;

            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Native code")]
            internal const int LOG_CTORDTOR = 9;

            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Native code")]
            internal const int LOG_REFCOUNT = 10;

            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Native code")]
            internal const int LOG_CUSTOM = 11;
        }

        /// <summary>
        ///  The native methods. This class is private and cannnot be used consuming projects. Private, not exposed to consumers.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
        private static class NativeMethods
        {
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            [DllImport("kernel32")]
            internal static extern IntPtr LoadLibraryW([MarshalAs(UnmanagedType.LPWStr)] string lpMdoule);

            /// <summary>
            ///     The register logger client.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <returns>
            ///     int value denoting success or failure.
            /// </returns>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            [DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "REGISTERLOGGERCLIENT",
                ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int RegisterLoggerClient(ref int hIdentity);

            /// <summary>
            ///     The unregister logger client.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <returns>
            ///     int value denoting success or failure.
            /// </returns>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            [DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "UNREGISTERLOGGERCLIENT",
                ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int UnregisterLoggerClient(int hIdentity);

            /// <summary>
            ///     The set identity name.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="strIdentity">
            ///     The str identity.
            /// </param>
            /// <returns>
            ///     int value denoting success or failure.
            /// </returns>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            [DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "SETIDENTITYNAME",
                ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int SetIdentityName(int hIdentity, string strIdentity);

            /// <summary>
            ///     The internal log error.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="message">
            ///     The message.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            [DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGERROR",
                ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void LogError(
                int hIdentity,
                [MarshalAs(UnmanagedType.LPWStr)] string message);

            /// <summary>
            ///     The internal log warning.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="message">
            ///     The message.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            [DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGWARNING",
                ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void LogWarning(
                int hIdentity,
                [MarshalAs(UnmanagedType.LPWStr)] string message);

            /// <summary>
            ///     The internal log info.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="message">
            ///     The message.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            [DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGINFO",
                ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void LogInfo(
                int hIdentity,
                [MarshalAs(UnmanagedType.LPWStr)] string message);

            /// <summary>
            ///     The internal log trace.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="message">
            ///     The message.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is shared code, not all consumers use all methods."),
             DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGTRACE",
                 ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void LogTrace(
                int hIdentity,
                [MarshalAs(UnmanagedType.LPWStr)] string message);

            /// <summary>
            ///     The internal log start stop.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="message">
            ///     The message.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is shared code, not all consumers use all methods."),
             DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGSTARTSTOP",
                 ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void LogStartStop(
                int hIdentity,
                [MarshalAs(UnmanagedType.LPWStr)] string message);

            /// <summary>
            ///     The internal log entry exit.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="message">
            ///     The message.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is shared code, not all consumers use all methods."),
             DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGENTRYEXIT",
                 ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void LogEntryExit(
                int hIdentity,
                [MarshalAs(UnmanagedType.LPWStr)] string message);

            /// <summary>
            ///     The internal log thread start stop.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="message">
            ///     The message.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is shared code, not all consumers use all methods."),
             DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGTHREADSTARTSTOP",
                 ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void LogThreadStartStop(
                int hIdentity,
                [MarshalAs(UnmanagedType.LPWStr)] string message);

            /// <summary>
            ///     The internal log sql.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="message">
            ///     The message.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is shared code, not all consumers use all methods."),
             DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGSQL",
                 ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void LogSQL(int hIdentity, [MarshalAs(UnmanagedType.LPWStr)] string message);

            /// <summary>
            ///     The internal log connection.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="message">
            ///     The message.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is shared code, not all consumers use all methods."),
             DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGCONNECTION",
                 ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void LogConnection(
                int hIdentity,
                [MarshalAs(UnmanagedType.LPWStr)] string message);

            /// <summary>
            ///     The internal log ctor dtor.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="message">
            ///     The message.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is shared code, not all consumers use all methods."),
             DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGCTORDTOR",
                 ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void LogCtorDtor(
                int hIdentity,
                [MarshalAs(UnmanagedType.LPWStr)] string message);

            /// <summary>
            ///     The internal log ref count.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="message">
            ///     The message.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is shared code, not all consumers use all methods."),
             DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGREFCOUNT",
                 ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void LogRefCount(
                int hIdentity,
                [MarshalAs(UnmanagedType.LPWStr)] string message);

            /// <summary>
            ///     The register log flag.
            /// </summary>
            /// <param name="hIdentity">
            ///     The h identity.
            /// </param>
            /// <param name="nCustomFlag">
            ///     The n custom flag.
            /// </param>
            /// <param name="strFlag">
            ///     The str flag.
            /// </param>
            /// <returns>
            ///     int value denoting success or failure.
            /// </returns>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is shared code, not all consumers use all methods."),
             DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "REGISTERLOGFLAG",
                 ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr RegisterLogFlag(int hIdentity, int nCustomFlag, [MarshalAs(UnmanagedType.LPWStr)] string strFlag);

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is shared code, not all consumers use all methods."),
             DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGCUSTOM2",
                 ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void LogCustom(
                int hIdentity,
                IntPtr customFlag,
                [MarshalAs(UnmanagedType.LPWStr)] string message);

            /// <summary>
            /// The get log flag.
            /// </summary>
            /// <param name="hIdentity">
            /// The h identity.
            /// </param>
            /// <param name="nFlag">
            /// The n flag.
            /// </param>
            /// <returns>
            /// integer pointer
            /// </returns>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            [DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "GETLOGFLAG",
                ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr GetLogFlag(int hIdentity, int nFlag);

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            [DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGFLAGLOG", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool LogFlagLog(int hIdentity, IntPtr logflag, int nFlag);

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            [DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "LOGSTART", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int LogStart(int hIdentity);

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is shared code, not all consumers use all methods."),
             DllImport("LoggerDLL.dll", CharSet = CharSet.Unicode, EntryPoint = "GETLOGGERSTATS",
                 ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int GetLoggerStats(
                [MarshalAs(UnmanagedType.LPWStr)] string hostName,
                ref int errorCount,
                ref long ftLastError,
                ref int warningCount,
                ref long ftLastWarning);
        }

        /// <summary>
        /// Windows ETW logger. Placeholder for now, will be evolved in future. Private, not exposed to consumers.
        /// </summary>
        private static class WindowsLogger
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "entryType", Justification = "Unused for now")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "message", Justification = "Unused for now")]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method does not have to be used by all clients of the Logger.")]
            internal static void WriteEntry(string message, EventLogEntryType entryType)
            {
                // For Release 1, this functionality is disabled. This need to be tested before enabling in a future release.
                // Source = "TestApplication";

                //// Write an entry to the event log.
                // EventLog.WriteEntry(Source, message, entryType);
            }
        }

        #endregion
    }
}
