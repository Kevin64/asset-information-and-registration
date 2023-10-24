using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using AssetInformationAndRegistration.Properties;
using AssetInformationAndRegistration.Updater;
using ConstantsDLL;
using ConstantsDLL.Properties;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Microsoft.Win32;
using Octokit;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary> 
    /// Class for About box
    /// </summary>
    internal partial class AboutBox : Form, ITheming
    {
        private bool isSystemDarkModeEnabled;
        private readonly Program.Definitions definitions;
        private readonly LogGenerator log;
        private readonly GitHubClient ghc;
        private UserPreferenceChangedEventHandler UserPreferenceChanged;

        /// <summary>
        /// About form constructor
        /// </summary>
        /// <param name="ghc">GitHub client object</param>
        /// <param name="log">Log file object</param>
        /// <param name="definitions">Definition object</param>
        /// <param name="isSystemDarkModeEnabled">Theme mode</param>
        internal AboutBox(GitHubClient ghc, LogGenerator log, Program.Definitions definitions, bool isSystemDarkModeEnabled)
        {
            InitializeComponent();
            KeyDown += AboutBox_KeyDown;

            this.ghc = ghc;
            this.definitions = definitions;
            this.log = log;
            this.isSystemDarkModeEnabled = isSystemDarkModeEnabled;

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

            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OPENING_ABOUTBOX, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

            Text = string.Format("{0} {1}", labelFormTitle.Text, AssemblyTitle);
            labelProductName.Text = AssemblyProduct;
#if DEBUG
            labelVersion.Text = string.Format(labelVersion.Text + " {0}-{1}", AssemblyVersion, AirResources.DEV_STATUS);
#else
            labelVersion.Text = string.Format(labelVersion.Text + " {0}", AssemblyVersion);
#endif
            labelCopyright.Text = AssemblyCopyright;
            labelCompanyName.Text = AssemblyCompany;
            textBoxDescription.Text = AirStrings.DESCRIPTION;
            textBoxDescription.LinkClicked += TextBoxDescription_LinkClicked;
        }

        /// <summary> 
        /// Loads the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutBox_Load(object sender, EventArgs e)
        {
            UserPreferenceChanged = new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
            Disposed += new EventHandler(AboutBox_Disposed);
            FormClosing += AboutBox_Closing;
        }

        /// <summary> 
        /// Handles the closing of the current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutBox_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_ABOUTBOX, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
        }

        /// <summary>
        /// Free resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutBox_Disposed(object sender, EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
        }

        /// <summary> 
        /// Handles link clicks inside the Description box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxDescription_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            _ = System.Diagnostics.Process.Start(e.LinkText);
        }

        /// <summary> 
        /// Triggers an update check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckUpdateButton_Click(object sender, System.EventArgs e)
        {
            UpdateChecker.Check(ghc, log, definitions, true, true, false, isSystemDarkModeEnabled);
        }

        /// <summary>
        /// Closes the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            Close();
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
        private void AboutBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        #region Acessório de Atributos do Assembly

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != string.Empty)
                        return titleAttribute.Title;
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
