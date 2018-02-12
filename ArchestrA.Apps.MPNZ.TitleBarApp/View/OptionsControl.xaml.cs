// ------------------------------------------------------------------------
// <copyright file="OptionsControl.xaml.cs" company="Schneider Electric Software, LLC">
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
    using ArchestrA.Diagnostics;

    /// <summary>
    /// Interaction logic for OptionsControl.xaml
    /// </summary>
    public partial class OptionsControl : IDisposable
    {
        private static readonly DependencyProperty CustomTextProperty = DependencyProperty.Register("CustomText", typeof(string), typeof(OptionsControl), new FrameworkPropertyMetadata(null));
        private static readonly DependencyProperty ShowLanguageSwitchingProperty = DependencyProperty.Register("ShowLanguageSwitching", typeof(bool), typeof(OptionsControl), new FrameworkPropertyMetadata(true));
        private static readonly DependencyProperty ShowLogonProperty = DependencyProperty.Register("ShowLogOn", typeof(bool), typeof(OptionsControl), new FrameworkPropertyMetadata(true));
        private static readonly DependencyProperty ShowHomeProperty = DependencyProperty.Register("ShowHome", typeof(bool), typeof(OptionsControl), new FrameworkPropertyMetadata(true));
        private static readonly DependencyProperty ShowNavigationTitleProperty = DependencyProperty.Register("ShowNavigationTitle", typeof(bool), typeof(OptionsControl), new FrameworkPropertyMetadata(true));
        private static readonly DependencyProperty ShowDateTimeProperty = DependencyProperty.Register("ShowDateTime", typeof(bool), typeof(OptionsControl), new FrameworkPropertyMetadata(false));
        private static readonly DependencyProperty ShowDateProperty = DependencyProperty.Register("ShowDate", typeof(bool), typeof(OptionsControl), new FrameworkPropertyMetadata(false));
        private static readonly DependencyProperty ShowTimeProperty = DependencyProperty.Register("ShowTime", typeof(bool), typeof(OptionsControl), new FrameworkPropertyMetadata(false));
        private static readonly DependencyProperty ShowKeyboardProperty = DependencyProperty.Register("ShowKeyboard", typeof(bool), typeof(OptionsControl), new FrameworkPropertyMetadata(true));

        #region Fields
        private OptionsControlViewModel viewModel;
        #endregion

        #region Constructor, Finalizer

        public OptionsControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="OptionsControl" /> class.
        /// </summary>
        ~OptionsControl()
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

        public string CustomText
        {
            get { return (string)this.GetValue(OptionsControl.CustomTextProperty); }

            set { this.SetValue(OptionsControl.CustomTextProperty, value); }
        }

        public bool ShowNavigationTitle
        {
            get { return (bool)this.GetValue(OptionsControl.ShowNavigationTitleProperty); }

            set { this.SetValue(OptionsControl.ShowNavigationTitleProperty, value); }
        }

        public bool ShowLanguageSwitching
        {
            get { return (bool)this.GetValue(OptionsControl.ShowLanguageSwitchingProperty); }

            set { this.SetValue(OptionsControl.ShowLanguageSwitchingProperty, value); }
        }

        public bool ShowKeyboard
        {
            get { return (bool)this.GetValue(OptionsControl.ShowKeyboardProperty); }

            set { this.SetValue(OptionsControl.ShowKeyboardProperty, value); }
        }

        public bool ShowLogOn
        {
            get { return (bool)this.GetValue(OptionsControl.ShowLogonProperty); }

            set { this.SetValue(OptionsControl.ShowLogonProperty, value); }
        }

        public bool ShowHome
        {
            get { return (bool)this.GetValue(OptionsControl.ShowHomeProperty); }

            set { this.SetValue(OptionsControl.ShowHomeProperty, value); }
        }

        public bool ShowDateTime
        {
            get { return (bool)this.GetValue(OptionsControl.ShowDateTimeProperty); }

            set { this.SetValue(OptionsControl.ShowDateTimeProperty, value); }
        }

        public bool ShowDate
        {
            get { return (bool)this.GetValue(OptionsControl.ShowDateProperty); }

            set { this.SetValue(OptionsControl.ShowDateProperty, value); }
        }

        public bool ShowTime
        {
            get { return (bool)this.GetValue(OptionsControl.ShowTimeProperty); }

            set { this.SetValue(OptionsControl.ShowTimeProperty, value); }
        }

        /// <summary>
        /// Disposes the TitleBarControl
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OptionsControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.DataContext = this.viewModel = new OptionsControlViewModel();
            }
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
            }
        }
    }
}
