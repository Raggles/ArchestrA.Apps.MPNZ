// ------------------------------------------------------------------------
// <copyright file="TitleBarProperties.cs" company="Schneider Electric Software, LLC">
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
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media;
    using Client.CommonUtil;

    /// <summary>
    /// This partial class contains all the public properties which can be configured
    /// </summary>
    public partial class TitleBarControl : INotifyPropertyChanged
    {
        #region Private Instance Fields

        private FontFamily primaryFontFamily = new FontFamily("Roboto");
        private int primaryFontSize = 14;
        private FontStyle primaryFontStyle;
        private FontWeight primaryFontWeight;
        private bool hasSecondary = true;
        private FontFamily secondaryFontFamily = new FontFamily("Roboto");
        private int secondaryFontSize = 10;
        private FontStyle secondaryFontStyle;
        private FontWeight secondaryFontWeight;
        private string title = "InTouch OMI";
        private DisplayArea homeArea = DisplayArea.SecondaryLeft;
        private DisplayArea navigationTitleArea = DisplayArea.SecondaryLeft;
        private DisplayArea keyboardArea = DisplayArea.PrimaryRight;
        private DisplayArea loginArea = DisplayArea.PrimaryRight;
        private DisplayArea languageArea;
        private DisplayArea dateArea;
        private DisplayArea timeArea;
        private DisplayArea dateTimeArea = DisplayArea.SecondaryRight;
        private DisplayArea breadcrumbArea;
        private bool canClose = true;
        private bool canMinimize = true;
        private bool canMaximize = true;
        private bool cancloseViewApp;

        #endregion

        #region Public Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Public Instance Properties
        [Category(Constants.Appearance)]
        [DisplayName("Primary Font Family")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "PrimaryFontFamily_Description")]
        public FontFamily PrimaryFontFamily
        {
            get
            {
                return this.primaryFontFamily;
            }

            set
            {
                this.primaryFontFamily = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Appearance)]
        [DisplayName("Primary Font Size")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "PrimaryFontSize_Description")]
        public int PrimaryFontSize
        {
            get
            {
                return this.primaryFontSize;
            }

            set
            {
                this.primaryFontSize = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Appearance)]
        [DisplayName("Primary Font Style")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "PrimaryFontStyle_Description")]
        public FontStyle PrimaryFontStyle
        {
            get
            {
                return this.primaryFontStyle;
            }

            set
            {
                this.primaryFontStyle = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Appearance)]
        [DisplayName("Primary Font Weight")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "PrimaryFontWeight_Description")]
        public FontWeight PrimaryFontWeight
        {
            get
            {
                return this.primaryFontWeight;
            }

            set
            {
                this.primaryFontWeight = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Appearance)]
        [DisplayName("Has Secondary Row")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "HasSecondary_Description")]
        [DefaultValue(true)]
        public bool HasSecondary
        {
            get
            {
                return this.hasSecondary;
            }

            set
            {
                this.hasSecondary = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Appearance)]
        [DisplayName("Secondary Font Family")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "SecondaryFontFamily_Description")]
        public FontFamily SecondaryFontFamily
        {
            get
            {
                return this.secondaryFontFamily;
            }

            set
            {
                this.secondaryFontFamily = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Appearance)]
        [DisplayName("Secondary Font Size")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "SecondaryFontSize_Description")]
        public int SecondaryFontSize
        {
            get
            {
                return this.secondaryFontSize;
            }

            set
            {
                this.secondaryFontSize = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Appearance)]
        [DisplayName("Secondary Font Style")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "SecondaryFontStyle_Description")]
        public FontStyle SecondaryFontStyle
        {
            get
            {
                return this.secondaryFontStyle;
            }

            set
            {
                this.secondaryFontStyle = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Appearance)]
        [DisplayName("Secondary Font Weight")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "SecondaryFontWeight_Description")]
        public FontWeight SecondaryFontWeight
        {
            get
            {
                return this.secondaryFontWeight;
            }

            set
            {
                this.secondaryFontWeight = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Content)]
        [DisplayName("Title")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "Title_Description")]
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Content)]
        [DisplayName("Home")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "HomeArea_Description")]
        public DisplayArea HomeArea
        {
            get
            {
                return this.homeArea;
            }

            set
            {
                this.homeArea = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Content)]
        [DisplayName("Navigation Title")]
        [DefaultValue(DisplayArea.SecondaryLeft)]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "NavigationTitleArea_Description")]
        public DisplayArea NavigationTitleArea
        {
            get
            {
                return this.navigationTitleArea;
            }

            set
            {
                this.navigationTitleArea = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Content)]
        [DisplayName("Keyboard")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "KeyboardArea_Description")]
        public DisplayArea KeyboardArea
        {
            get
            {
                return this.keyboardArea;
            }

            set
            {
                this.keyboardArea = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Content)]
        [DisplayName("Login")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "LoginArea_Description")]
        public DisplayArea LoginArea
        {
            get
            {
                return this.loginArea;
            }

            set
            {
                this.loginArea = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Content)]
        [DisplayName("Language")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "LanguageArea_Description")]
        public DisplayArea LanguageArea
        {
            get
            {
                return this.languageArea;
            }

            set
            {
                this.languageArea = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Content)]
        [DisplayName("Date")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "DateArea_Description")]
        public DisplayArea DateArea
        {
            get
            {
                return this.dateArea;
            }

            set
            {
                this.dateArea = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Content)]
        [DisplayName("Time")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "TimeArea_Description")]
        public DisplayArea TimeArea
        {
            get
            {
                return this.timeArea;
            }

            set
            {
                this.timeArea = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Content)]
        [DisplayName("Date and Time")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "DateTimeArea_Description")]
        public DisplayArea DateTimeArea
        {
            get
            {
                return this.dateTimeArea;
            }

            set
            {
                this.dateTimeArea = value;
                this.OnPropertyChanged();
            }
        }

        [Browsable(false)]
        [Category(Constants.Content)]
        [DisplayName("Navigation Breadcrumb")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "BreadcumbArea_Description")]
        public DisplayArea BreadcrumbArea
        {
            get
            {
                return this.breadcrumbArea;
            }

            set
            {
                this.breadcrumbArea = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Display)]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "CanMinimize_Description")]
        [DisplayName("Show Minimize")]
        public bool CanMinimize
        {
            get
            {
                return this.canMinimize;
            }

            set
            {
                this.canMinimize = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Display)]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "CanMaximize_Description")]
        [DisplayName("Show Maximize")]
        [DefaultValue(true)]
        public bool CanMaximize
        {
            get
            {
                return this.canMaximize;
            }

            set
            {
                this.canMaximize = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Display)]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "CanClose_Description")]
        [DisplayName("Show Close Layout")]
        public bool CanClose
        {
            get
            {
                return this.canClose;
            }

            set
            {
                this.canClose = value;
                this.OnPropertyChanged();
            }
        }

        [Category(Constants.Display)]
        [DisplayName("Show Close ViewApp")]
        [LocalizedDescription(typeof(TitleBarControl), Constants.TitleBarAppResources, "CanCloseViewApp_Description")]
        public bool CanCloseViewApp
        {
            get
            {
                return this.cancloseViewApp;
            }

            set
            {
                this.cancloseViewApp = value;
                this.OnPropertyChanged();
            }
        }
        #endregion

        #region Private Instance Methods
        /// <summary>
        ///     Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
