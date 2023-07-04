using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using AssetInformationAndRegistration.Updater;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
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
        private readonly string currentVersion, newVersion, changelog, url;
        private readonly LogGenerator log;

        /// <summary> 
        /// Updater form constructor
        /// </summary>
        /// <param name="parametersList">List containing data from [Parameters]</param>
        /// <param name="themeBool">Theme mode</param>
        /// <param name="releases">GitHub release information</param>
        public UpdateCheckerForm(LogGenerator log, List<string[]> parametersList, bool themeBool, UpdateInfo ui)
        {
            InitializeComponent();

            if (ui != null)
            {
                newVersion = ui.TagName;
                changelog = ui.Body;
                url = ui.HtmlUrl;
            }
            currentVersion = MiscMethods.Version();
            this.log = log;

            if (StringsAndConstants.LIST_THEME_GUI.Contains(parametersList[3][0].ToString()) && parametersList[3][0].ToString().Equals(StringsAndConstants.LIST_THEME_GUI[0]))
            {
                if (themeBool)
                {
                    if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                    }
                    DarkTheme();
                }
                else
                {
                    if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                    }
                    LightTheme();
                }
            }
            else if (parametersList[3][0].ToString().Equals(StringsAndConstants.LIST_THEME_GUI[1]))
            {
                LightTheme();
            }
            else if (parametersList[3][0].ToString().Equals(StringsAndConstants.LIST_THEME_GUI[2]))
            {
                DarkTheme();
            }
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
        }
    }
}
