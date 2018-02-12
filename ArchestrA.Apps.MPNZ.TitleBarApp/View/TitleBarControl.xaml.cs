// ------------------------------------------------------------------------
// <copyright file="TitleBarControl.xaml.cs" company="Schneider Electric Software, LLC">
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
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using Diagnostics;

    /// <summary>
    /// Interaction logic for TitleBarControl.xaml
    /// </summary>
    public partial class TitleBarControl : UserControl, IDisposable
    {
        #region Fields
        private TitleBarViewModel viewModel;
        #endregion

        #region Constructor, Finalizer

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleBarControl" /> class.
        /// </summary>
        public TitleBarControl()
        {
            this.InitializeComponent();

            this.Background = Brushes.Black;
            this.Foreground = Brushes.White;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TitleBarControl" /> class.
        /// </summary>
        ~TitleBarControl()
        {
            try
            {
                this.Dispose(false);
                Logger.LogDispose(this.GetType().FullName);
            }
            catch
            {
                //// Ensure we don't throw out of the finalizer
            }
        }

        #endregion

        public override void OnApplyTemplate()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            this.DataContext = this.viewModel = new TitleBarViewModel(this);
        }

        /// <summary>
        /// Disposes the TitleBarControl
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposing the resources.
        /// </summary>
        /// <param name="disposing">true or false</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.viewModel != null)
                {
                    this.viewModel.Dispose();
                    this.viewModel = null;
                }

                if (this.primaryLeft != null)
                {
                    this.primaryLeft.Dispose();
                    this.primaryLeft = null;
                }

                if (this.primaryCenter != null)
                {
                    this.primaryCenter.Dispose();
                    this.primaryCenter = null;
                }

                if (this.primaryRight != null)
                {
                    this.primaryRight.Dispose();
                    this.primaryRight = null;
                }

                if (this.SecLeft != null)
                {
                    this.SecLeft.Dispose();
                    this.SecLeft = null;
                }

                if (this.SecCenter != null)
                {
                    this.SecCenter.Dispose();
                    this.SecCenter = null;
                }

                if (this.SecRight != null)
                {
                    this.SecRight.Dispose();
                    this.SecRight = null;
                }
            }
        }

        /// <summary>
        /// This handles the MouseDouble click event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">param</param>
        private void TitleBar_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // block maximizing on double click anywhere on the titlebut except the main horizontal title bar area
                if (e.OriginalSource.GetType() != typeof(Border))
                {
                    return;
                }

                if (this.viewModel != null && !(this.CanClose || this.CanCloseViewApp || this.CanMaximize || this.CanMinimize))
                {
                    return;
                }

                var parentWindow = Window.GetWindow(this);

                if (parentWindow != null)
                {
                    parentWindow.WindowState = parentWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                }
            }
            catch (Exception exception)
            {
                Logger.LogWarning(() => exception.ToString());
            }
        }

        /// <summary>
        /// This handles the MouseLeftButtonDown click event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event arguments</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "ArchestrA.Apps.MPNZ.TitleBarApp.NativeMethods.SendMessage(System.IntPtr,System.Int32,System.Int32,System.Int32)", Justification = "Native method")]
        private void TitleBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    var parentWindow = Window.GetWindow(this);
                    NativeMethods.ReleaseCapture();

                    if (parentWindow != null)
                    {
                        var windowInterOperHelper = new WindowInteropHelper(parentWindow);
                        NativeMethods.SendMessage(windowInterOperHelper.Handle, NativeMethods.LeftButtonDown, NativeMethods.WindowCaption, 0);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogWarning(() => exception.ToString());
            }
        }
    }
}
