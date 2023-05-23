using AssetInformationAndRegistration.Properties;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using JsonFileReaderDLL;
using LogGeneratorDLL;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AssetInformationAndRegistration
{
    ///<summary>Class for handling Login tasks and UI</summary>
    public partial class LoginForm : Form
    {
        private readonly bool themeBool;
        private string[] agentsJsonStr = { };
        private readonly LogGenerator log;
        private readonly List<string[]> parametersList;
        private readonly List<string> enforcementList, orgDataList;

        #region
        private TaskbarManager tbProgLogin;
        private MainForm mForm;
        #endregion

        ///<summary>Login form constructor</summary>
        ///<param name="log">Log file object</param>
        ///<param name="parametersList">List containing data from [Parameters]</param>
        ///<param name="enforcementList">List containing data from [Enforcement]</param>
        ///<param name="orgDataList">List containing data from [OrgData]</param>
        public LoginForm(LogGenerator log, List<string[]> parametersList, List<string> enforcementList, List<string> orgDataList)
        {
            //Inits WinForms components
            InitializeComponent();

            this.parametersList = parametersList;
            this.enforcementList = enforcementList;
            this.orgDataList = orgDataList;
            this.log = log;

            //Sets status bar text according to info provided in the ini file
            string[] oList = new string[6];
            for (int i = 0; i < orgDataList.Count; i++)
            {
                if (!orgDataList[i].Equals(string.Empty))
                {
                    oList[i] = orgDataList[i].ToString() + " - ";
                }
            }

            toolStripStatusBarText.Text = oList[3] + oList[1].Substring(0, oList[1].Length - 2);

            //Define theming according to ini file provided info
            if (StringsAndConstants.LIST_THEME_GUI.Contains(parametersList[3][0].ToString()) && parametersList[3][0].ToString().Equals(StringsAndConstants.LIST_THEME_GUI[0]))
            {
                themeBool = MiscMethods.ThemeInit();
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
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                }
                LightTheme();
                themeBool = false;
            }
            else if (parametersList[3][0].ToString().Equals(StringsAndConstants.LIST_THEME_GUI[2]))
            {
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                }
                DarkTheme();
                themeBool = true;
            }

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_THEME, themeBool.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            comboBoxServerIP.Items.AddRange(parametersList[0]);
            comboBoxServerPort.Items.AddRange(parametersList[1]);

            //Program version
#if DEBUG
            toolStripVersionText.Text = MiscMethods.Version(Resources.DEV_STATUS);
            comboBoxServerIP.SelectedIndex = 1;
            comboBoxServerPort.SelectedIndex = 0;
#else
            toolStripVersionText.Text = MiscMethods.Version();
            comboBoxServerIP.SelectedIndex = 0;
            comboBoxServerPort.SelectedIndex = 0;
#endif
        }

        ///<summary>Sets a light theme for the login form</summary>
        public void LightTheme()
        {
            BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            lblFixedUsername.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedPassword.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedServerPort.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedServerIP.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            loadingCircleAuthButton.BackColor = StringsAndConstants.INACTIVE_SYSTEM_BUTTON_COLOR;

            textBoxFixedLoginIntro.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            textBoxFixedLoginIntro.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxUsername.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxUsername.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxPassword.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxPassword.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            comboBoxServerIP.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxServerIP.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxServerIP.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxServerIP.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxServerIP.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
            comboBoxServerPort.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxServerPort.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxServerPort.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxServerPort.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxServerPort.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;

            AuthButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            AuthButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            AuthButton.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
            AuthButton.FlatStyle = System.Windows.Forms.FlatStyle.System;

            checkBoxOfflineMode.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            checkBoxOfflineMode.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            aboutLabelButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            aboutLabelButton.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripStatusBarText.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripVersionText.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripStatusBarText.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripVersionText.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            statusStrip1.Renderer = new ModifiedToolStripProfessionalLightTheme();

            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_ABOUT_LIGHT_PATH));

            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.LOGIN_BANNER_LIGHT_PATH));
            iconImgUsername.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_AGENT_LIGHT_PATH));
            iconImgPassword.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_PASSWORD_LIGHT_PATH));
            iconImgServerIP.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SERVER_LIGHT_PATH));
            iconImgServerPort.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_PORT_LIGHT_PATH));
        }

        ///<summary>Sets a dark theme for the login form</summary>
        public void DarkTheme()
        {
            BackColor = StringsAndConstants.DARK_BACKGROUND;

            lblFixedUsername.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedPassword.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedServerPort.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedServerIP.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            loadingCircleAuthButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;

            textBoxFixedLoginIntro.BackColor = StringsAndConstants.DARK_BACKGROUND;
            textBoxFixedLoginIntro.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxUsername.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxUsername.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxPassword.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxPassword.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            comboBoxServerIP.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxServerIP.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxServerIP.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxServerIP.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxServerPort.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxServerPort.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxServerPort.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxServerPort.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;

            AuthButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            AuthButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            AuthButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;

            checkBoxOfflineMode.BackColor = StringsAndConstants.DARK_BACKGROUND;
            checkBoxOfflineMode.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            aboutLabelButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            aboutLabelButton.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripStatusBarText.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripVersionText.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripStatusBarText.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripVersionText.BackColor = StringsAndConstants.DARK_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.DARK_BACKGROUND;

            statusStrip1.Renderer = new ModifiedToolStripProfessionalDarkTheme();

            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_ABOUT_DARK_PATH));

            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.LOGIN_BANNER_DARK_PATH));
            iconImgUsername.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_AGENT_DARK_PATH));
            iconImgPassword.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_PASSWORD_DARK_PATH));
            iconImgServerIP.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SERVER_DARK_PATH));
            iconImgServerPort.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_PORT_DARK_PATH));
        }

        ///<summary>Loads the form, sets some combobox values</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Define loading circle parameters
            #region

            switch (MiscMethods.GetWindowsScaling())
            {
                case 100:
                    //Init loading circles parameters for 100% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    break;
            }

            loadingCircleAuthButton.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleAuthButton.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            #endregion

            FormClosing += LoginForm_Closing;
            tbProgLogin = TaskbarManager.Instance;
        }

        ///<summary>Handles the closing of the current form</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void LoginForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CLOSING_LOGINFORM, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_MISC), ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Deletes downloaded json files
            File.Delete(StringsAndConstants.MODEL_FILE_PATH);
            File.Delete(StringsAndConstants.CREDENTIALS_FILE_PATH);
            File.Delete(StringsAndConstants.ASSET_FILE_PATH);
            File.Delete(StringsAndConstants.CONFIG_FILE_PATH);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        ///<summary>Checks the username/password and shows the main form</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private async void AuthButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_INIT_LOGIN, textBoxUsername.Text, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            loadingCircleAuthButton.Visible = true;
            loadingCircleAuthButton.Active = true;
            aboutLabelButton.Enabled = false;
            if (checkBoxOfflineMode.Checked)
            {
                string[] offStr = { Strings.OFFLINE_MODE_ACTIVATED };
                tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                mForm = new MainForm(true, offStr, null, null, log, parametersList, enforcementList, orgDataList);
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                {
                    DarkNet.Instance.SetWindowThemeForms(mForm, Theme.Auto);
                }

                Hide();
                textBoxUsername.Text = null;
                textBoxPassword.Text = null;
                textBoxUsername.Select();
                _ = mForm.ShowDialog();
                mForm.Close();
                mForm.Dispose();
                Show();
            }
            else
            {
                textBoxUsername.Enabled = false;
                textBoxPassword.Enabled = false;
                comboBoxServerIP.Enabled = false;
                comboBoxServerPort.Enabled = false;
                checkBoxOfflineMode.Enabled = false;
                tbProgLogin.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SERVER_DETAIL, comboBoxServerIP.Text + ":" + comboBoxServerPort.Text, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                //Feches login data from server
                agentsJsonStr = await CredentialsFileReader.FetchInfoMT(textBoxUsername.Text, textBoxPassword.Text, comboBoxServerIP.Text, comboBoxServerPort.Text);

                //If all the mandatory fields are filled
                if (!string.IsNullOrWhiteSpace(textBoxUsername.Text) && !string.IsNullOrWhiteSpace(textBoxPassword.Text))
                {
                    //If Login Json file does not exist, there is no internet connection
                    if (agentsJsonStr == null)
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.INTRANET_REQUIRED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                        _ = MessageBox.Show(ConstantsDLL.Properties.Strings.INTRANET_REQUIRED, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                    }
                    else if (agentsJsonStr[0] == ConstantsDLL.Properties.Resources.FALSE) //If Login Json file does exist, but the agent do not exist
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.LOG_LOGIN_FAILED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                        _ = MessageBox.Show(Strings.AUTH_INVALID, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                    }
                    else //If Login Json file does exist and agent logs in
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_LOGIN_SUCCESS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                        MainForm mForm = new MainForm(false, agentsJsonStr, comboBoxServerIP.Text, comboBoxServerPort.Text, log, parametersList, enforcementList, orgDataList);
                        if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                        {
                            DarkNet.Instance.SetWindowThemeForms(mForm, Theme.Auto);
                        }

                        Hide();
                        textBoxUsername.Text = null;
                        textBoxPassword.Text = null;
                        textBoxUsername.Select();
                        _ = mForm.ShowDialog();
                        mForm.Close();
                        mForm.Dispose();
                        Show();
                    }
                }
                else //If all the mandatory fields are not filled
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.NO_AUTH, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    _ = MessageBox.Show(ConstantsDLL.Properties.Strings.NO_AUTH, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                }
                aboutLabelButton.Enabled = true;
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
        }

        ///<summary>Handles the offline mode toggle</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
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

        ///<summary>Opens the About box</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void AboutLabelButton_Click(object sender, EventArgs e)
        {
            AboutBox aboutForm = new AboutBox(parametersList, themeBool);
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
            {
                DarkNet.Instance.SetWindowThemeForms(aboutForm, Theme.Auto);
            }
            _ = aboutForm.ShowDialog();
        }
    }
}
