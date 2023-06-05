using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using Octokit;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    ///<summary>Class for handling Updater window</summary>
    public partial class UpdateCheckerForm : Form, ITheming
    {
        private readonly string currentVersion, newVersion, changelog, url;

        ///<summary>Updater form constructor</summary>
        ///<param name="parametersList">List containing data from [Parameters]</param>
        ///<param name="themeBool">Theme mode</param>
        ///<param name="releases">GitHub release information</param>
        public UpdateCheckerForm(List<string[]> parametersList, bool themeBool, Release releases)
        {
            InitializeComponent();

            currentVersion = MiscMethods.Version();
            newVersion = releases.TagName;
            changelog = releases.Body;
            url = releases.HtmlUrl;

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

        ///<summary>Loads the form, sets labels if there is an update</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void UpdateChangelog_Load(object sender, EventArgs e)
        {
            switch (newVersion.CompareTo(currentVersion))
            {
                case 1:
                    lblUpdateAnnoucement.Text = ConstantsDLL.Properties.Strings.NEW_VERSION_AVAILABLE;
                    changelogTextBox.Text = changelog;
                    lblFixedNewVersion.Visible = true;
                    lblNewVersion.Visible = true;
                    downloadButton.Visible = true;
                    break;
                default:
                    lblUpdateAnnoucement.Text = ConstantsDLL.Properties.Strings.NO_VERSION_AVAILABLE;
                    changelogTextBox.Text = changelog;
                    lblFixedNewVersion.Visible = false;
                    lblNewVersion.Visible = false;
                    downloadButton.Visible = false;
                    break;
            }
            lblOldVersion.Text = currentVersion;
            lblNewVersion.Text = newVersion;
        }

        ///<summary>Opens the GitHub url in the browser</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void downloadButton_Click(object sender, EventArgs e)
        {
            //log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_VIEW_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            _ = System.Diagnostics.Process.Start(url);
        }

        ///<summary>Closes the window</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
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
