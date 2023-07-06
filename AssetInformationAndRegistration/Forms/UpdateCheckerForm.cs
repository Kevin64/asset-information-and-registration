using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using AssetInformationAndRegistration.Updater;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary> 
    /// Class for handling Updater window
    /// </summary>
    public partial class UpdateCheckerForm : Form, ITheming
    {
        private bool themeBool;
        private readonly string currentVersion, newVersion, changelog, url;
        private readonly List<string[]> parametersList;
        private readonly LogGenerator log;
        private readonly UserPreferenceChangedEventHandler UserPreferenceChanged;

        /// <summary> 
        /// Updater form constructor
        /// </summary>
        /// <param name="parametersList">List containing data from [Parameters]</param>
        /// <param name="themeBool">Theme mode</param>
        /// <param name="releases">GitHub release information</param>
        public UpdateCheckerForm(LogGenerator log, List<string[]> parametersList, UpdateInfo ui, bool themeBool)
        {
            InitializeComponent();

            int themeFileSet = MiscMethods.GetFileThemeMode(parametersList, themeBool);
            switch (themeFileSet)
            {
                case 0:
                    DarkTheme();
                    break;
                case 1:
                    LightTheme();
                    break;
                case 2:
                    LightTheme();
                    break;
                case 3:
                    DarkTheme();
                    break;
            }

            if (ui != null)
            {
                newVersion = ui.TagName;
                changelog = ui.Body;
                url = ui.HtmlUrl;
            }
            currentVersion = MiscMethods.Version();
            this.log = log;
            this.themeBool = themeBool;
            this.parametersList = parametersList;

            UserPreferenceChanged = new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
            Disposed += new EventHandler(UpdateCheckerForm_Disposed);
        }

        /// <summary> 
        /// Compares versions and sets labels if there is an update
        /// </summary>
        /// <returns>True is there is a new version, false otherwise</returns>
        public bool IsThereANewVersion()
        {
            lblOldVersion.Text = currentVersion;
            lblNewVersion.Text = newVersion;
            switch (newVersion.CompareTo(currentVersion))
            {
                case 1:
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), ConstantsDLL.Properties.Strings.LOG_CHECKING_FOR_UPDATES, ConstantsDLL.Properties.Strings.NEW_VERSION_AVAILABLE, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    lblUpdateAnnoucement.Text = ConstantsDLL.Properties.Strings.NEW_VERSION_AVAILABLE;
                    changelogTextBox.Text = changelog;
                    lblFixedNewVersion.Visible = true;
                    lblNewVersion.Visible = true;
                    downloadButton.Visible = true;
                    return true;
                default:
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), ConstantsDLL.Properties.Strings.LOG_CHECKING_FOR_UPDATES, ConstantsDLL.Properties.Strings.NO_VERSION_AVAILABLE, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    lblUpdateAnnoucement.Text = ConstantsDLL.Properties.Strings.NO_VERSION_AVAILABLE;
                    changelogTextBox.Text = changelog;
                    lblFixedNewVersion.Visible = false;
                    lblNewVersion.Visible = false;
                    downloadButton.Visible = false;
                    return false;
            }
        }

        /// <summary> 
        /// Opens the GitHub url in the browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_OPENING_GITHUB, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            _ = System.Diagnostics.Process.Start(url);
        }

        /// <summary> 
        /// Closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_CLOSING_UPDATER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            Close();
        }

        public void LightTheme()
        {
            BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            lblUpdateAnnoucement.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedOldVersion.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedNewVersion.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblOldVersion.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblNewVersion.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedChangelogLatestVersion.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            downloadButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;

            changelogTextBox.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            changelogTextBox.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
            {
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
            }
        }

        public void DarkTheme()
        {
            BackColor = StringsAndConstants.DARK_BACKGROUND;

            lblUpdateAnnoucement.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedOldVersion.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedNewVersion.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblOldVersion.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblNewVersion.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedChangelogLatestVersion.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            downloadButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            downloadButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            downloadButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            downloadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            closeButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            closeButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            closeButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            changelogTextBox.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            changelogTextBox.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
            {
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
            }
        }

        /// <summary> 
        /// Method for auto selecting the app theme
        /// </summary>
        private void ToggleTheme()
        {
            switch (MiscMethods.GetFileThemeMode(parametersList, MiscMethods.GetSystemThemeMode()))
            {
                case 0:
                    DarkTheme();
                    themeBool = true;
                    break;
                case 1:
                    LightTheme();
                    themeBool = false;
                    break;
                case 2:
                    LightTheme();
                    themeBool = false;
                    break;
                case 3:
                    DarkTheme();
                    themeBool = true;
                    break;
            }
        }

        /// <summary>
        /// Allows the theme to change automatically according to the system one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                ToggleTheme();
            }
        }

        /// <summary>
        /// Free resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCheckerForm_Disposed(object sender, EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
        }
    }
}
