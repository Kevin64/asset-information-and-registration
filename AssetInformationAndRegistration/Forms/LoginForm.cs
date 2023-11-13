using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using AssetInformationAndRegistration.Properties;
using ConstantsDLL;
using ConstantsDLL.Properties;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using Octokit;
using RestApiDLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows.Forms;
using MiscMethods = AssetInformationAndRegistration.Misc.MiscMethods;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary> 
    /// Class for handling Login tasks and UI
    /// </summary>
    internal partial class LoginForm : Form, ITheming
    {
        private bool isSystemDarkModeEnabled;
        private Agent agent;
        private readonly Program.ConfigurationOptions configOptions;
        private readonly GitHubClient ghc;
        private HttpClient client;
        private readonly LogGenerator log;
        private MainForm mForm;
        private TaskbarManager tbProgLogin;
        private UserPreferenceChangedEventHandler UserPreferenceChanged;

        /// <summary>
        /// Login form constructor
        /// </summary>
        /// <param name="ghc">GitHub client object</param>
        /// <param name="log">Log file object</param>
        /// <param name="configOptions">Config file object</param>
        /// <param name="isSystemDarkModeEnabled">Theme mode</param>
        internal LoginForm(GitHubClient ghc, LogGenerator log, Program.ConfigurationOptions configOptions, bool isSystemDarkModeEnabled)
        {
            //Inits WinForms components
            InitializeComponent();

            this.ghc = ghc;
            this.log = log;
            this.configOptions = configOptions;
            this.isSystemDarkModeEnabled = isSystemDarkModeEnabled;

            comboBoxServerIP.Items.AddRange(this.configOptions.Definitions.ServerIP.ToArray());
            comboBoxServerPort.Items.AddRange(this.configOptions.Definitions.ServerPort.ToArray());

            //Define theming according to JSON file provided info
            (int themeFileSet, bool _) = MiscMethods.GetFileThemeMode(this.configOptions.Definitions, this.isSystemDarkModeEnabled);
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

            //Sets status bar text according to info provided in the JSON file
            List<string> oList = new List<string>();
            foreach (PropertyInfo pi in configOptions.OrgData.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(configOptions.OrgData);
                    if (!string.IsNullOrEmpty(value))
                        oList.Add(value + " - ");
                }
            }
            toolStripStatusBarText.Text = oList[3] + oList[1].Substring(0, oList[1].Length - 2);

            //Program version
#if DEBUG
            toolStripVersionText.Text = MiscMethods.Version(GenericResources.DEV_STATUS_BETA);
            comboBoxServerIP.SelectedIndex = 1;
            comboBoxServerPort.SelectedIndex = 0;
#else
            toolStripVersionText.Text = MiscMethods.Version();
            comboBoxServerIP.SelectedIndex = 0;
            comboBoxServerPort.SelectedIndex = 0;
#endif
            if (isSystemDarkModeEnabled)
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_THEME, LogStrings.LOG_THEME_DARK, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            else
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_THEME, LogStrings.LOG_THEME_LIGHT, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
        }

        /// <summary> 
        /// Loads the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginForm_Load(object sender, EventArgs e)
        {
            MiscMethods.SetLoadingCircles(this);

            FormClosing += LoginForm_Closing;
            Disposed += new EventHandler(LoginForm_Disposed);
            tbProgLogin = TaskbarManager.Instance;
            UserPreferenceChanged = new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
        }

        /// <summary> 
        /// Handles the closing of the current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginForm_Closing(object sender, FormClosingEventArgs e)
        {
            MiscMethods.CloseProgram(log, Convert.ToInt32(ExitCodes.SUCCESS), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
        }

        /// <summary>
        /// Free resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginForm_Disposed(object sender, EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
        }

        /// <summary> 
        /// Checks the username/password and shows the main form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AuthButton_Click(object sender, EventArgs e)
        {
            try
            {
                authButton.Enabled = false;
                //If offline mode is checked
                if (checkBoxOfflineMode.Checked)
                {
                    tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                    mForm = new MainForm(null, ghc, log, configOptions, null, null, null, true, isSystemDarkModeEnabled);
                    if (HardwareInfo.GetWinVersion().Equals(GenericResources.WINDOWS_10))
                        DarkNet.Instance.SetWindowThemeForms(mForm, Theme.Auto);
                    Hide();
                    textBoxUsername.Text = null;
                    textBoxPassword.Text = null;
                    textBoxUsername.Select();
                    _ = mForm.ShowDialog();
                    mForm.Close();
                    mForm.Dispose();
                    Show();
                }
                //If not in offline mode and all the mandatory fields are filled
                else if (!string.IsNullOrWhiteSpace(textBoxUsername.Text) && !string.IsNullOrWhiteSpace(textBoxPassword.Text))
                {
                    tbProgLogin.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
                    loadingCircleAuthButton.Visible = true;
                    loadingCircleAuthButton.Active = true;
                    aboutLabelButton.Enabled = false;
                    textBoxUsername.Enabled = false;
                    textBoxPassword.Enabled = false;
                    comboBoxServerIP.Enabled = false;
                    comboBoxServerPort.Enabled = false;
                    checkBoxOfflineMode.Enabled = false;

                    client = RestApiDLL.MiscMethods.SetHttpClient(comboBoxServerIP.Text, comboBoxServerPort.Text, GenericResources.HTTP_CONTENT_TYPE_JSON, textBoxUsername.Text, textBoxPassword.Text);

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_AUTH_USER, textBoxUsername.Text, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SERVER_DATA, comboBoxServerIP.Text + ":" + comboBoxServerPort.Text, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                    //Feches login data from server
                    agent = await AuthenticationHandler.GetAgentAsync(client, GenericResources.HTTP + comboBoxServerIP.Text + ":" + comboBoxServerPort.Text + GenericResources.V1_API_AGENT_URL + textBoxUsername.Text);

                    //If Agent does exist and is retrieved
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_LOGIN_SUCCESS, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    MainForm mForm = new MainForm(client, ghc, log, configOptions, agent, comboBoxServerIP.Text, comboBoxServerPort.Text, false, isSystemDarkModeEnabled);
                    if (HardwareInfo.GetWinVersion().Equals(GenericResources.WINDOWS_10))
                        DarkNet.Instance.SetWindowThemeForms(mForm, Theme.Auto);

                    Hide();
                    textBoxUsername.Text = null;
                    textBoxPassword.Text = null;
                    textBoxUsername.Select();
                    _ = mForm.ShowDialog();
                    mForm.Close();
                    mForm.Dispose();
                    Show();
                }
                else //If all the mandatory fields are not filled
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.FILL_IN_YOUR_CREDENTIALS, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    _ = MessageBox.Show(UIStrings.FILL_IN_YOUR_CREDENTIALS, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                }
                aboutLabelButton.Enabled = true;
            }
            //If URI is invalid
            catch (UriFormatException ex)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(UIStrings.FILL_IN_SERVER_DETAILS, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
            }
            //If Agent does not exist because there is no internet connection
            catch (HttpRequestException ex)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(UIStrings.INTRANET_REQUIRED, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
            }
            //If Agent does not exist, but the connection succeeded
            catch (InvalidAgentException ex)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(UIStrings.INVALID_CREDENTIALS, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
            }
            //If Rest call is invalid
            catch (InvalidRestApiCallException ex)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(UIStrings.SERVER_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
            }

            //Enables controls if login is not successful
            loadingCircleAuthButton.Visible = false;
            loadingCircleAuthButton.Active = false;
            textBoxUsername.Enabled = true;
            _ = textBoxUsername.Focus();
            textBoxPassword.Enabled = true;
            comboBoxServerIP.Enabled = true;
            comboBoxServerPort.Enabled = true;
            checkBoxOfflineMode.Enabled = true;
            checkBoxOfflineMode.Checked = false;
            authButton.Enabled = true;
        }

        /// <summary> 
        /// Handles the offline mode toggle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OfflineModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOfflineMode.Checked)
            {
                textBoxUsername.Enabled = false;
                textBoxPassword.Enabled = false;
                comboBoxServerIP.Enabled = false;
                comboBoxServerPort.Enabled = false;
            }
            else
            {
                textBoxUsername.Enabled = true;
                textBoxPassword.Enabled = true;
                comboBoxServerIP.Enabled = true;
                comboBoxServerPort.Enabled = true;
            }
        }

        /// <summary> 
        /// Opens the About box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutLabelButton_Click(object sender, EventArgs e)
        {
            AboutBox aForm = new AboutBox(ghc, log, configOptions.Definitions, isSystemDarkModeEnabled);
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WINDOWS_10))
                DarkNet.Instance.SetWindowThemeForms(aForm, Theme.Auto);
            _ = aForm.ShowDialog();
        }

        /// <summary> 
        /// Method for auto selecting the app theme
        /// </summary>
        private void ToggleTheme()
        {
            (int themeFileSet, bool _) = MiscMethods.GetFileThemeMode(configOptions.Definitions, MiscMethods.GetSystemThemeMode());
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
            aboutLabelButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            aboutLabelButton.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripStatusBarText.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripVersionText.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripStatusBarText.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripVersionText.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            statusStrip1.Renderer = new ModifiedToolStripProfessionalLightTheme();

            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_ABOUT_LIGHT_PATH));
            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.LOGIN_BANNER_LIGHT_PATH));
            iconImgUsername.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_AGENT_LIGHT_PATH));
            iconImgPassword.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_PASSWORD_LIGHT_PATH));
            iconImgServerIP.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_SERVER_LIGHT_PATH));
            iconImgServerPort.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_PORT_LIGHT_PATH));
        }

        public void DarkThemeSpecificControls()
        {
            aboutLabelButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            aboutLabelButton.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripStatusBarText.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripVersionText.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripStatusBarText.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripVersionText.BackColor = StringsAndConstants.DARK_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.DARK_BACKGROUND;
            statusStrip1.Renderer = new ModifiedToolStripProfessionalDarkTheme();

            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_ABOUT_DARK_PATH));
            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.LOGIN_BANNER_DARK_PATH));
            iconImgUsername.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_AGENT_DARK_PATH));
            iconImgPassword.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_PASSWORD_DARK_PATH));
            iconImgServerIP.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_SERVER_DARK_PATH));
            iconImgServerPort.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_PORT_DARK_PATH));
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
    }
}
