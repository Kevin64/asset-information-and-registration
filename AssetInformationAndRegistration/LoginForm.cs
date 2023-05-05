using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using AssetInformationAndRegistration.Properties;
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

        //Form constructor
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
            if (StringsAndConstants.listThemeGUI.Contains(parametersList[3][0].ToString()) && parametersList[3][0].ToString().Equals(StringsAndConstants.listThemeGUI[0]))
            {
                themeBool = MiscMethods.ThemeInit();
                if (themeBool)
                {
                    if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                    }
                    DarkTheme();
                }
                else
                {
                    if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                    }
                    LightTheme();
                }
            }
            else if (parametersList[3][0].ToString().Equals(StringsAndConstants.listThemeGUI[1]))
            {
                if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                }
                LightTheme();
                themeBool = false;
            }
            else if (parametersList[3][0].ToString().Equals(StringsAndConstants.listThemeGUI[2]))
            {
                if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                }
                DarkTheme();
                themeBool = true;
            }

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_THEME, themeBool.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            comboBoxServerIP.Items.AddRange(parametersList[0]);
            comboBoxServerPort.Items.AddRange(parametersList[1]);

            //Program version
#if DEBUG
            toolStripVersionText.Text = MiscMethods.Version(Resources.dev_status);
            comboBoxServerIP.SelectedIndex = 1;
            comboBoxServerPort.SelectedIndex = 0;
#else
            toolStripVersionText.Text = MiscMethods.Version();
            comboBoxServerIP.SelectedIndex = 0;
            comboBoxServerPort.SelectedIndex = 0;
#endif
        }

        //Sets a light theme for the login form
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

            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_about_light_path));

            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.login_banner_light_path));
            iconImgUsername.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_agent_light_path));
            iconImgPassword.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_password_light_path));
            iconImgServerIP.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_server_light_path));
            iconImgServerPort.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_port_light_path));
        }

        //Sets a dark theme for the login form
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

            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_about_dark_path));

            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.login_banner_dark_path));
            iconImgUsername.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_agent_dark_path));
            iconImgPassword.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_password_dark_path));
            iconImgServerIP.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_server_dark_path));
            iconImgServerPort.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_port_dark_path));
        }

        //Loads the form, sets some combobox values
        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Define loading circle parameters
            #region

            switch (MiscMethods.GetWindowsScaling())
            {
                case 100:
                    //Init loading circles parameters for 100% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    loadingCircleAuthButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleAuthButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleAuthButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleAuthButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    break;
            }

            loadingCircleAuthButton.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleAuthButton.Color = StringsAndConstants.rotatingCircleColor;
            #endregion

            FormClosing += LoginForm_Closing;
            tbProgLogin = TaskbarManager.Instance;
        }

        //Handles the closing of the current form
        private void LoginForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CLOSING_LOGINFORM, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_MISC), ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Deletes downloaded json files
            File.Delete(StringsAndConstants.modelFilePath);
            File.Delete(StringsAndConstants.credentialsFilePath);
            File.Delete(StringsAndConstants.assetFilePath);
            File.Delete(StringsAndConstants.configFilePath);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        //Checks the username/password and shows the main form
        private async void AuthButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_INIT_LOGIN, textBoxUsername.Text, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            loadingCircleAuthButton.Visible = true;
            loadingCircleAuthButton.Active = true;
            aboutLabelButton.Enabled = false;
            if (checkBoxOfflineMode.Checked)
            {
                string[] offStr = { Strings.OFFLINE_MODE_ACTIVATED };
                tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                mForm = new MainForm(true, offStr, null, null, log, parametersList, enforcementList, orgDataList);
                if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
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
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SERVER_DETAIL, comboBoxServerIP.Text + ":" + comboBoxServerPort.Text, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

                //Feches login data from server
                agentsJsonStr = await CredentialsFileReader.FetchInfoMT(textBoxUsername.Text, textBoxPassword.Text, comboBoxServerIP.Text, comboBoxServerPort.Text);

                //If all the mandatory fields are filled
                if (!string.IsNullOrWhiteSpace(textBoxUsername.Text) && !string.IsNullOrWhiteSpace(textBoxPassword.Text))
                {
                    //If Login Json file does not exist, there is no internet connection
                    if (agentsJsonStr == null)
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.INTRANET_REQUIRED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                        _ = MessageBox.Show(ConstantsDLL.Properties.Strings.INTRANET_REQUIRED, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                    }
                    else if (agentsJsonStr[0] == "false") //If Login Json file does exist, but the agent do not exist
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.LOG_LOGIN_FAILED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                        _ = MessageBox.Show(Strings.AUTH_INVALID, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                    }
                    else //If Login Json file does exist and agent logs in
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_LOGIN_SUCCESS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                        MainForm mForm = new MainForm(false, agentsJsonStr, comboBoxServerIP.Text, comboBoxServerPort.Text, log, parametersList, enforcementList, orgDataList);
                        if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
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
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.NO_AUTH, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
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

        //Handles the offline mode toggle 
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

        //Opens the About box
        private void AboutLabelButton_Click(object sender, EventArgs e)
        {
            AboutBox aboutForm = new AboutBox(parametersList, themeBool);
            if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
            {
                DarkNet.Instance.SetWindowThemeForms(aboutForm, Theme.Auto);
            }
            _ = aboutForm.ShowDialog();
        }
    }
}
