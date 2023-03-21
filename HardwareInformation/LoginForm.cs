using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using HardwareInformation.Properties;
using JsonFileReaderDLL;
using LogGeneratorDLL;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace HardwareInformation
{
    public partial class LoginForm : Form
    {
        private readonly bool themeBool;
        private string[] str = { };
        private readonly LogGenerator log;
        private readonly List<string[]> defList;
        private readonly List<string> orgList;

        #region
        private TaskbarManager tbProgLogin;
        private MainForm mForm;
        #endregion

        //Form constructor
        public LoginForm(LogGenerator l, List<string[]> definitionListSection, List<string> orgDataListSection)
        {
            //Inits WinForms components
            InitializeComponent();

            defList = definitionListSection;
            orgList = orgDataListSection;

            //Sets status bar text according to info provided in the ini file
            string[] oList = new string[6];
            for (int i = 0; i < orgList.Count; i++)
            {
                if (!orgList[i].Equals(string.Empty))
                {
                    oList[i] = orgList[i].ToString() + " - ";
                }
            }

            toolStripStatusBarText.Text = oList[3] + oList[1].Substring(0, oList[1].Length - 2);

            log = l;

            //Define theming according to ini file provided info
            if (StringsAndConstants.listThemeGUI.Contains(defList[5][0].ToString()) && defList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[0]))
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
            else if (defList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[1]))
            {
                if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                }

                LightTheme();
            }
            else if (defList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[2]))
            {
                if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                }

                DarkTheme();
            }

            log.LogWrite(StringsAndConstants.LOG_INFO, Strings.LOG_THEME, themeBool.ToString(), StringsAndConstants.consoleOutGUI);

            comboBoxServerIP.Items.AddRange(defList[0]);
            comboBoxServerPort.Items.AddRange(defList[1]);

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

            lblFixedUser.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedPassword.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedServerPort.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedServerIP.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            loadingCircle1.BackColor = StringsAndConstants.INACTIVE_SYSTEM_BUTTON_COLOR;

            textBoxUser.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxUser.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
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

            aboutLabel.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            aboutLabel.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripStatusBarText.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripVersionText.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripStatusBarText.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripVersionText.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            statusStrip1.Renderer = new ModifiedToolStripProfessionalLightTheme();

            aboutLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_about_light_path));

            topBannerImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.login_banner_light_path));
            userIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_user_light_path));
            passwordIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_password_light_path));
            serverIPIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_server_light_path));
            serverPortIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_port_light_path));
        }

        //Sets a dark theme for the login form
        public void DarkTheme()
        {
            BackColor = StringsAndConstants.DARK_BACKGROUND;

            lblFixedUser.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedPassword.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedServerPort.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedServerIP.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            loadingCircle1.BackColor = StringsAndConstants.DARK_BACKCOLOR;

            textBoxUser.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxUser.ForeColor = StringsAndConstants.DARK_FORECOLOR;
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

            aboutLabel.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            aboutLabel.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripStatusBarText.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripVersionText.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripStatusBarText.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripVersionText.BackColor = StringsAndConstants.DARK_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.DARK_BACKGROUND;

            statusStrip1.Renderer = new ModifiedToolStripProfessionalDarkTheme();

            aboutLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_about_dark_path));

            topBannerImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.login_banner_dark_path));
            userIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_user_dark_path));
            passwordIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_password_dark_path));
            serverIPIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_server_dark_path));
            serverPortIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_port_dark_path));
        }

        //Loads the form, sets some combobox values
        private void Form2_Load(object sender, EventArgs e)
        {
            // Define loading circle parameters
            #region

            switch (MiscMethods.GetWindowsScaling())
            {
                case 100:
                    //Init loading circles parameters for 100% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    break;
            }

            loadingCircle1.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle1.Color = StringsAndConstants.rotatingCircleColor;
            #endregion

            FormClosing += Form2_FormClosing;
            tbProgLogin = TaskbarManager.Instance;
        }

        //Handles the closing of the current form
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, Strings.LOG_CLOSING_LOGINFORM, string.Empty, StringsAndConstants.consoleOutGUI);
            log.LogWrite(StringsAndConstants.LOG_MISC, ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, StringsAndConstants.consoleOutGUI);

            //Deletes downloaded json files
            File.Delete(StringsAndConstants.biosPath);
            File.Delete(StringsAndConstants.loginPath);
            File.Delete(StringsAndConstants.pcPath);
            File.Delete(StringsAndConstants.configPath);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        //Checks the user/password and shows the main form
        private async void AuthButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, Strings.LOG_INIT_LOGIN, textBoxUser.Text, StringsAndConstants.consoleOutGUI);
            loadingCircle1.Visible = true;
            loadingCircle1.Active = true;
            if (checkBoxOfflineMode.Checked)
            {
                tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                mForm = new MainForm(true, Strings.OFFLINE_MODE_ACTIVATED, null, null, log, defList, orgList);
                if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                {
                    DarkNet.Instance.SetWindowThemeForms(mForm, Theme.Auto);
                }

                Hide();
                textBoxUser.Text = null;
                textBoxPassword.Text = null;
                textBoxUser.Select();
                _ = mForm.ShowDialog();
                mForm.Close();
                mForm.Dispose();
                Show();
            }
            else
            {
                textBoxUser.Enabled = false;
                textBoxPassword.Enabled = false;
                comboBoxServerIP.Enabled = false;
                comboBoxServerPort.Enabled = false;
                checkBoxOfflineMode.Enabled = false;
                tbProgLogin.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
                log.LogWrite(StringsAndConstants.LOG_INFO, Strings.LOG_SERVER_DETAIL, comboBoxServerIP.Text + ":" + comboBoxServerPort.Text, StringsAndConstants.consoleOutGUI);

                //Feches login data from server
                str = await LoginFileReader.FetchInfoMT(textBoxUser.Text, textBoxPassword.Text, comboBoxServerIP.Text, comboBoxServerPort.Text);

                //If all the mandatory fields are filled
                if (!string.IsNullOrWhiteSpace(textBoxUser.Text) && !string.IsNullOrWhiteSpace(textBoxPassword.Text))
                {
                    //If Login Json file does not exist, there is no internet connection
                    if (str == null)
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_NO_INTRANET, string.Empty, StringsAndConstants.consoleOutGUI);
                        _ = MessageBox.Show(StringsAndConstants.INTRANET_REQUIRED, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                    }
                    else if (str[0] == "false") //If Login Json file does exist, but the user do not exist
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, Strings.LOG_LOGIN_FAILED, string.Empty, StringsAndConstants.consoleOutGUI);
                        _ = MessageBox.Show(Strings.AUTH_INVALID, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                    }
                    else //If Login Json file does exist and user logs in
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, Strings.LOG_LOGIN_SUCCESS, string.Empty, StringsAndConstants.consoleOutGUI);
                        MainForm mForm = new MainForm(false, str[1], comboBoxServerIP.Text, comboBoxServerPort.Text, log, defList, orgList);
                        if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                        {
                            DarkNet.Instance.SetWindowThemeForms(mForm, Theme.Auto);
                        }

                        Hide();
                        textBoxUser.Text = null;
                        textBoxPassword.Text = null;
                        textBoxUser.Select();
                        _ = mForm.ShowDialog();
                        mForm.Close();
                        mForm.Dispose();
                        Show();
                    }
                }
                else //If all the mandatory fields are not filled
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_LOGIN_INCOMPLETE, string.Empty, StringsAndConstants.consoleOutGUI);
                    _ = MessageBox.Show(StringsAndConstants.NO_AUTH, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                }
            }

            //Enables controls if login is not successful
            loadingCircle1.Visible = false;
            loadingCircle1.Active = false;
            textBoxUser.Enabled = true;
            _ = textBoxUser.Focus();
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
                textBoxUser.Enabled = false;
                textBoxPassword.Enabled = false;
                comboBoxServerIP.Enabled = false;
                comboBoxServerPort.Enabled = false;
            }
            else
            {
                textBoxUser.Enabled = true;
                textBoxPassword.Enabled = true;
                comboBoxServerIP.Enabled = true;
                comboBoxServerPort.Enabled = true;
            }
        }

        //Opens the About box
        private void AboutLabel_Click(object sender, EventArgs e)
        {
            AboutBox aboutForm = new AboutBox(defList, themeBool);
            if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
            {
                DarkNet.Instance.SetWindowThemeForms(aboutForm, Theme.Auto);
            }

            _ = aboutForm.ShowDialog();
        }
    }
}
