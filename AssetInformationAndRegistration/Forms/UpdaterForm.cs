using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using AssetInformationAndRegistration.Updater;
using ConstantsDLL;
using ConstantsDLL.Properties;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary> 
    /// Class for handling Updater window
    /// </summary>
    public partial class UpdaterForm : Form, ITheming
    {
        private bool isSystemDarkModeEnabled;
        private readonly string currentVersion, newVersion, changelog, url;
        private readonly Program.Definitions definitions;
        private readonly LogGenerator log;
        private UserPreferenceChangedEventHandler UserPreferenceChanged;

        /// <summary>
        /// Updater form constructor
        /// </summary>
        /// <param name="log">Log file object</param>
        /// <param name="definitions">Definition object</param>
        /// <param name="ui">Update details object</param>
        /// <param name="isSystemDarkModeEnabled">Theme mode</param>
        public UpdaterForm(LogGenerator log, Program.Definitions definitions, UpdateInfo ui, bool isSystemDarkModeEnabled)
        {
            InitializeComponent();
            KeyDown += UpdaterForm_KeyDown;

            currentVersion = MiscMethods.Version();
            this.log = log;
            this.isSystemDarkModeEnabled = isSystemDarkModeEnabled;
            this.definitions = definitions;

            //Define theming according to JSON file provided info
            (int themeFileSet, bool _) = MiscMethods.GetFileThemeMode(definitions, isSystemDarkModeEnabled);
            switch (themeFileSet)
            {
                case 0:
                    MiscMethods.LightThemeAllControls(this);
                    LightThemeSpecificControls();
                    break;
                case 1:
                    MiscMethods.DarkThemeAllControls(this);
                    DarkThemeSpecificControls();
                    break;
            }

            if (ui != null)
            {
                newVersion = ui.TagName;
                changelog = ui.Body;
                url = ui.HtmlUrl;
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
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), UIStrings.NEW_VERSION_AVAILABLE, newVersion, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                    lblUpdateAnnoucement.Text = UIStrings.NEW_VERSION_AVAILABLE;
                    changelogTextBox.Text = changelog;
                    lblFixedNewVersion.Visible = true;
                    lblNewVersion.Visible = true;
                    downloadButton.Visible = true;
                    return true;
                default:
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), UIStrings.NO_NEW_VERSION_AVAILABLE, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                    lblUpdateAnnoucement.Text = UIStrings.NO_NEW_VERSION_AVAILABLE;
                    changelogTextBox.Text = changelog;
                    lblFixedNewVersion.Visible = false;
                    lblNewVersion.Visible = false;
                    downloadButton.Visible = false;
                    return false;
            }
        }

        /// <summary> 
        /// Loads the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdaterForm_Load(object sender, EventArgs e)
        {
            UserPreferenceChanged = new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
            Disposed += new EventHandler(UpdaterForm_Disposed);
            FormClosing += UpdaterForm_Closing;
        }

        /// <summary> 
        /// Handles the closing of the current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdaterForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_UPDATER_FORM, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
        }

        /// <summary>
        /// Free resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdaterForm_Disposed(object sender, EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
        }

        /// <summary> 
        /// Closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary> 
        /// Opens the GitHub url in the browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OPENING_GITHUB, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            _ = System.Diagnostics.Process.Start(url);
        }

        /// <summary> 
        /// Method for auto selecting the app theme
        /// </summary>
        private void ToggleTheme()
        {
            (int themeFileSet, bool _) = MiscMethods.GetFileThemeMode(definitions, MiscMethods.GetSystemThemeMode());
            switch (themeFileSet)
            {
                case 0:
                    MiscMethods.LightThemeAllControls(this);
                    LightThemeSpecificControls();
                    isSystemDarkModeEnabled = false;
                    break;
                case 1:
                    MiscMethods.DarkThemeAllControls(this);
                    DarkThemeSpecificControls();
                    isSystemDarkModeEnabled = true;
                    break;
            }
        }

        public void LightThemeSpecificControls()
        {

        }

        public void DarkThemeSpecificControls()
        {

        }

        /// <summary>
        /// Allows the theme to change automatically according to the system one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
                ToggleTheme();
        }

        /// <summary>
        /// Closes the form when Escape is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdaterForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
