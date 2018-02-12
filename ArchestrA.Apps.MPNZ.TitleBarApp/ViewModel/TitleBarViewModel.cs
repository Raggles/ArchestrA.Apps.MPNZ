// ------------------------------------------------------------------------
// <copyright file="TitleBarViewModel.cs" company="Schneider Electric Software, LLC">
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
    using System.Globalization;
    using System.Windows;
    using ArchestrA.Apps.MPNZ.TitleBarApp.Properties;
    using ArchestrA.Client.CommonUtil;
    using ArchestrA.Client.ViewApp;
    using ArchestrA.Diagnostics;
    
    /// <summary>
    /// Acts as a DataContext for TitleBar View providing implementation for commands and property changed handlers on user input
    /// </summary>
    public class TitleBarViewModel : ViewModelBase
    {
        #region Fields

        private DelegateCommand minimizeCommand;
        private DelegateCommand maximizeCommand;
        private DelegateCommand closeCommand;
        private DelegateCommand restoreCommand;
        private object attachedView;
        private DelegateCommand closeappCommand;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleBarViewModel" /> class.
        /// </summary>
        /// <param name="attachedView">Passing View for accessing Window state</param>
        public TitleBarViewModel(object attachedView)
        {
            if (attachedView == null)
            {
                // throw exception if IView is null
                throw new ArgumentNullException("attachedView"); 
            }

           this.attachedView = attachedView;
        }

        #region Properties
        /// <summary>
        /// Gets TitleBar control Maximize Command
        /// </summary>
        public DelegateCommand MaximizeCommand
        {
            get
            {
                return this.maximizeCommand ??
                        (this.maximizeCommand = new DelegateCommand
                        {
                            CommandAction = obj => { this.MaximizeWindow(); }
                        });
            }
        }

        /// <summary>
        /// Gets TitleBar control Minimize Command
        /// </summary>
        public DelegateCommand MinimizeCommand
        {
            get
            {
                return this.minimizeCommand ??
                        (this.minimizeCommand = new DelegateCommand
                        {
                            CommandAction = obj => { this.MinimizeWindow(); }
                        });
            }
        }

        /// <summary>
        /// Gets TitleBar control Close Command
        /// </summary>
        public DelegateCommand CloseCommand
        {
            get
            {
                return this.closeCommand ??
                        (this.closeCommand = new DelegateCommand
                        {
                            CommandAction = obj => { this.CloseWindowOrApplication(false); }
                        });
            }
        }

        /// <summary>
        /// Gets TitleBar control Restore Command
        /// </summary>
        public DelegateCommand RestoreCommand
        {
            get
            {
                return this.restoreCommand ??
                        (this.restoreCommand = new DelegateCommand
                        {
                            CommandAction = obj => { this.RestoreWindow(); }
                        });
            }
        }

        /// <summary>
        /// Gets TitleBar control Close Command
        /// </summary>
        public DelegateCommand CloseAppCommand
        {
            get
            {
                return this.closeappCommand ??
                        (this.closeappCommand = new DelegateCommand
                        {
                            CommandAction = obj => { this.CloseWindowOrApplication(true); }
                        });
            }
        }
        #endregion
        
        /// <summary>
        /// Disposing the resources.
        /// </summary>
        /// <param name="disposing">true or false</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.attachedView = null;
            }

            base.Dispose(disposing);
        }

        #region Private Methods

        private void MaximizeWindow()
        {
            try
            {
                var parentApp = ViewApplication.Application;
                if (parentApp != null)
                {
                    parentApp.SetLayoutState(this.attachedView, WindowState.Maximized);
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(() => string.Format(CultureInfo.CurrentCulture, Resources.Maximize_ErrorFormat, e.ToString()));
            }
        }

        private void CloseWindowOrApplication(bool closeApplication = false)
        {
            try
            {
                var parentApp = ViewApplication.Application;
                if (parentApp != null)
                {
                    if (closeApplication)
                    {
                        parentApp.CloseViewApplication();
                    }
                    else
                    {
                         parentApp.CloseLayout(this.attachedView);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(() => string.Format(CultureInfo.CurrentCulture, Resources.Close_ErrorFormat, e.ToString()));
            }
        }

        private void MinimizeWindow()
        {
            var parentApp = ViewApplication.Application;

            try
            {
                if (parentApp != null)
                {
                    parentApp.SetLayoutState(this.attachedView, WindowState.Minimized);
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(() => string.Format(CultureInfo.CurrentCulture, Resources.Minimize_ErrorFormat, e.ToString()));
            }
        }

        private void RestoreWindow()
        {
            try
            {
                var parentApp = ViewApplication.Application;
                if (parentApp != null)
                {
                    parentApp.SetLayoutState(this.attachedView, WindowState.Normal);
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(() => string.Format(CultureInfo.CurrentCulture, Resources.Restore_ErrorFormat, e.ToString()));
            }
        }
        #endregion
    }
}
