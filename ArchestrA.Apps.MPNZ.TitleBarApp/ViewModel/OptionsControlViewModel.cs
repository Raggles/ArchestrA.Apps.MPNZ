// ------------------------------------------------------------------------
// <copyright file="OptionsControlViewModel.cs" company="Schneider Electric Software, LLC">
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using Client.CommonUtil;
    using Client.MyViewApp;
    using Properties;

    public class OptionsControlViewModel : ViewModelBase
    {
        private DelegateCommand changeLanguageCommand;
        private DelegateCommand navigateHomeCommand;
        private DelegateCommand logInCommand;
        private DelegateCommand resetSelectedLoginOptionCommand;
        private IEnumerable<string> languages;
        private string selectedOption;
        private string logInStatus;
        private bool isLoggedIn;

        public OptionsControlViewModel()
        {
            this.HookupCommands();

            this.InitData();
            this.ResetSelectedLoginOption();
        }

        public DelegateCommand LaunchKeyBoardCommand { get; set; }

        public DateTime CurrentDateAndTime
        {
            get
            {
                return ArchestrA.Client.MyViewApp.System.DateTime;
            }
        }

        public string NavigationTitle
        {
            get
            {
                return ArchestrA.Client.MyViewApp.Navigation.CurrentTitle;
            }
        }

        /// <summary>
        /// Gets all available languages
        /// </summary>
        public IEnumerable<string> Languages
        {
            get
            {
                return this.languages;
            }

            private set
            {
                this.languages = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current language
        /// </summary>
        public string CurrentLanguage
        {
            get
            {
                return Language.CurrentLanguage;
            }

            set
            {
                if (string.Compare(Language.CurrentLanguage, value, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    Language.CurrentLanguage = value;
                }

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets TitleBar control ChangeLanguage Command
        /// </summary>
        public DelegateCommand ChangeLanguageCommand
        {
            get
            {
                return this.changeLanguageCommand ??
                        (this.changeLanguageCommand = new DelegateCommand
                        {
                            CommandAction = obj => { this.ChangeLanguage(obj); }
                        });
            }
        }

        /// <summary>
        /// Gets a value indicating whether user is logged in
        /// </summary>
        public bool IsLoggedIn
        {
            get
            {
                return this.isLoggedIn;
            }

            private set
            {
                this.isLoggedIn = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Login/Logout is selected in Login control
        /// </summary>
        public string SelectedOption
        {
            get
            {
                return this.selectedOption;
            }

            set
            {
                this.selectedOption = value;
                this.OnPropertyChanged();
                this.HandleLogInOptionChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether user is logged in which shows name of the logged in user if logged in
        /// </summary>
        public string LogInStatus
        {
            get
            {
                return this.logInStatus;
            }

            set
            {
                this.logInStatus = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets TitleBar control LogInCommand
        /// </summary>
        public DelegateCommand LogInCommand
        {
            get
            {
                return this.logInCommand ??
                        (this.logInCommand = new DelegateCommand
                        {
                            CommandAction = obj =>
                            {
                                if (!IsLoggedIn)
                                {
                                    this.SelectedOption = Resources.LogIn_Description;
                                }
                            }
                        });
            }
        }

        /// <summary>
        /// Gets TitleBar control ChangeLanguage Command
        /// </summary>
        public DelegateCommand NavigateToHomeCommand
        {
            get
            {
                return this.navigateHomeCommand ??
                        (this.navigateHomeCommand = new DelegateCommand
                        {
                            CommandAction = obj => { Navigation.CurrentPath = Navigation.RootPath; }
                        });
            }
        }

        /// <summary>
        /// Gets TitleBar control ResetComboBoxSelection Command
        /// </summary>
        public DelegateCommand ResetSelectedLoginOptionCommand
        {
            get
            {
                return this.resetSelectedLoginOptionCommand ??
                        (this.resetSelectedLoginOptionCommand = new DelegateCommand
                        {
                            CommandAction = obj =>
                            {
                                this.ResetSelectedLoginOption();
                            }
                        });
            }
        }

        /// <summary>
        /// Disposing the resources.
        /// </summary>
        /// <param name="disposing">true or false</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.UnHookCommands();
            }

            base.Dispose(disposing);
        }

        private void Language_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentLanguage")
            {
                this.CurrentLanguage = Language.CurrentLanguage;
            }
        }

        private void ChangeLanguage(object language)
        {
            var selectedLanguage = (string)language;
            if (!string.IsNullOrEmpty(selectedLanguage) && !string.Equals(this.CurrentLanguage, selectedLanguage))
            {
                this.CurrentLanguage = selectedLanguage;
            }
        }

        private void HookupCommands()
        {
            this.Languages = Language.Languages;
            Language.PropertyChanged += this.Language_PropertyChanged;
            Navigation.PropertyChanged += this.NavigationOnPropertyChanged;
            Security.PropertyChanged += this.Security_PropertyChanged;
            System.PropertyChanged += this.SystemOnPropertyChanged;

            this.LaunchKeyBoardCommand = new DelegateCommand()
                                         {
                                             CommandAction = commandParameter => { KeyboardOperator.Toggle(); }
                                         };
        }

        private void SystemOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "DateTime")
            {
                this.OnPropertyChanged("CurrentDateAndTime");
            }
        }

        private void UnHookCommands()
        {
            Language.PropertyChanged -= this.Language_PropertyChanged;
            Navigation.PropertyChanged -= this.NavigationOnPropertyChanged;
            Security.PropertyChanged -= this.Security_PropertyChanged;
            System.PropertyChanged -= this.SystemOnPropertyChanged;
        }

        private void NavigationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "CurrentTitle")
            {
                this.OnPropertyChanged("NavigationTitle");
            }
        }

        private void HandleLogInOptionChanged()
        {
            if (this.SelectedOption == null)
            {
                return;
            }

            if (this.SelectedOption.Equals(Resources.LogIn_Description) || this.SelectedOption.Equals(Resources.SwitchUser_Description))
            {
                Security.ShowLoginDialog();

                // Resetting Login ComboBox selection such that user can re-Login with other credentials.
                this.ResetSelectedLoginOption();
            }
            else if (this.SelectedOption.Equals(Resources.LogOut_Description) && this.IsLoggedIn)
            {
                Security.Logout();
            }
        }

        private void Security_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "LoggedIn":
                    this.IsLoggedIn = Security.LoggedIn;
                    break;
                case "LoggedInUserName":
                    this.SetLoginName();
                    break;
            }
        }

        private void ResetSelectedLoginOption()
        {
            // Resetting Login ComboBox selection on load as well as during selection changed such that user can re-Login with other credentials.
            this.SelectedOption = null;
        }
        
        private void InitData()
        {
            this.IsLoggedIn = Security.LoggedIn;
            this.SetLoginName();

            this.OnPropertyChanged("CurrentLanguage");
            this.OnPropertyChanged("NavigationTitle");
            this.OnPropertyChanged("CurrentDateAndTime");
        }

        private void SetLoginName()
        {
            this.LogInStatus = this.IsLoggedIn
                                   ? Security.LoggedInUserName
                                   : Resources.LogIn_Description;
        }
    }
}
