using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using HardwareInformation.Properties;
using JsonFileReaderDLL;
using LogGeneratorDLL;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace HardwareInformation
{
    public partial class LoginForm : Form
    {
        private bool themeBool;
        private string[] str = { };
        private LogGenerator log;
        private BackgroundWorker backgroundWorker1;
        private List<string[]> defList;
        private List<string> orgList;

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
                if (!orgList[i].Equals(string.Empty))
                    oList[i] = orgList[i].ToString() + " - ";
            this.toolStripStatusLabel1.Text = oList[3] + oList[1].Substring(0, oList[1].Length - 2);

            log = l;

            //Define theming according to ini file provided info
            if (StringsAndConstants.listThemeGUI.Contains(defList[5][0].ToString()) && defList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[0]))
            {
                themeBool = MiscMethods.ThemeInit();
                if (themeBool)
                {
                    if (HardwareInfo.getOSInfoAux().Equals(StringsAndConstants.windows10))
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                    darkTheme();
                }
                else
                {
                    if (HardwareInfo.getOSInfoAux().Equals(StringsAndConstants.windows10))
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                    lightTheme();
                }
            }
            else if (defList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[1]))
            {
                if (HardwareInfo.getOSInfoAux().Equals(StringsAndConstants.windows10))
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                lightTheme();
            }
            else if (defList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[2]))
            {
                if (HardwareInfo.getOSInfoAux().Equals(StringsAndConstants.windows10))
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                darkTheme();
            }

            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_THEME, themeBool.ToString(), StringsAndConstants.consoleOutGUI);

            comboBoxServerIP.Items.AddRange(defList[0]);
            comboBoxServerPort.Items.AddRange(defList[1]);

            //Program version
#if DEBUG
            this.toolStripStatusLabel2.Text = MiscMethods.version(Resources.dev_status);            
            comboBoxServerIP.SelectedIndex = 1;
            comboBoxServerPort.SelectedIndex = 0;
#else
            this.toolStripStatusLabel2.Text = MiscMethods.version();            
            comboBoxServerIP.SelectedIndex = 0;
			comboBoxServerPort.SelectedIndex = 0;
#endif

            //Inits thread worker for parallelism
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        //Sets a light theme for the login form
        public void lightTheme()
        {
            this.BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            this.label1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label3.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label4.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            this.loadingCircle1.BackColor = StringsAndConstants.INACTIVE_SYSTEM_BUTTON_COLOR;

            this.textBoxUser.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxUser.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBoxPassword.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxPassword.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            this.comboBoxServerIP.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxServerIP.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxServerIP.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxServerIP.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxServerIP.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
            this.comboBoxServerPort.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxServerPort.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxServerPort.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxServerPort.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxServerPort.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;

            this.AuthButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.AuthButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.AuthButton.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.AuthButton.FlatStyle = System.Windows.Forms.FlatStyle.System;

            this.checkBoxOfflineMode.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.checkBoxOfflineMode.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            this.aboutLabel.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.aboutLabel.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.toolStripStatusLabel1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripStatusLabel2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripStatusLabel1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.toolStripStatusLabel2.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.statusStrip1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            this.statusStrip1.Renderer = new ModifiedToolStripProfessionalLightTheme();

            this.aboutLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_about_light_path));

            configurableQualityPictureBox1.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.login_banner_light_path));
            configurableQualityPictureBox2.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_user_light_path));
            configurableQualityPictureBox3.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_password_light_path));
            configurableQualityPictureBox4.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_server_light_path));
            configurableQualityPictureBox5.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_port_light_path));
        }

        //Sets a dark theme for the login form
        public void darkTheme()
        {
            this.BackColor = StringsAndConstants.DARK_BACKGROUND;

            this.label1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label3.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label4.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            this.loadingCircle1.BackColor = StringsAndConstants.DARK_BACKCOLOR;

            this.textBoxUser.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.textBoxUser.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBoxPassword.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.textBoxPassword.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            this.comboBoxServerIP.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxServerIP.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxServerIP.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxServerIP.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxServerPort.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxServerPort.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxServerPort.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxServerPort.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;

            this.AuthButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.AuthButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.AuthButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;

            this.checkBoxOfflineMode.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.checkBoxOfflineMode.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            this.aboutLabel.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.aboutLabel.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.toolStripStatusLabel1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripStatusLabel2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripStatusLabel1.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.toolStripStatusLabel2.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.statusStrip1.BackColor = StringsAndConstants.DARK_BACKGROUND;

            this.statusStrip1.Renderer = new ModifiedToolStripProfessionalDarkTheme();

            this.aboutLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_about_dark_path));

            configurableQualityPictureBox1.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.login_banner_dark_path));
            configurableQualityPictureBox2.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_user_dark_path));
            configurableQualityPictureBox3.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_password_dark_path));
            configurableQualityPictureBox4.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_server_dark_path));
            configurableQualityPictureBox5.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_port_dark_path));
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
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CLOSING_LOGINFORM, string.Empty, StringsAndConstants.consoleOutGUI);
            log.LogWrite(StringsAndConstants.LOG_MISC, StringsAndConstants.LOG_SEPARATOR_SMALL, string.Empty, StringsAndConstants.consoleOutGUI);

            //Deletes downloaded json files
            File.Delete(StringsAndConstants.biosPath);
            File.Delete(StringsAndConstants.loginPath);
            File.Delete(StringsAndConstants.pcPath);
            File.Delete(StringsAndConstants.configPath);
            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }

        //Checks the user/password and shows the main form
        private async void authButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_INIT_LOGIN, textBoxUser.Text, StringsAndConstants.consoleOutGUI);
            loadingCircle1.Visible = true;
            loadingCircle1.Active = true;
            if (checkBoxOfflineMode.Checked)
            {
                tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, this.Handle);
                mForm = new MainForm(true, StringsAndConstants.OFFLINE_MODE_ACTIVATED, null, null, log, defList, orgList);
                if (HardwareInfo.getOSInfoAux().Equals(StringsAndConstants.windows10))
                    DarkNet.Instance.SetWindowThemeForms(mForm, Theme.Auto);
                this.Hide();
                textBoxUser.Text = null;
                textBoxPassword.Text = null;
                textBoxUser.Select();
                mForm.ShowDialog();
                mForm.Close();
                mForm.Dispose();
                this.Show();
            }
            else
            {
                textBoxUser.Enabled = false;
                textBoxPassword.Enabled = false;
                comboBoxServerIP.Enabled = false;
                comboBoxServerPort.Enabled = false;
                checkBoxOfflineMode.Enabled = false;
                tbProgLogin.SetProgressState(TaskbarProgressBarState.Indeterminate, this.Handle);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SERVER_DETAIL, comboBoxServerIP.Text + ":" + comboBoxServerPort.Text, StringsAndConstants.consoleOutGUI);
                
                //Feches login data from server
                str = await LoginFileReader.fetchInfoMT(textBoxUser.Text, textBoxPassword.Text, comboBoxServerIP.Text, comboBoxServerPort.Text);
                
                //If all the mandatory fields are filled
                if (!string.IsNullOrWhiteSpace(textBoxUser.Text) && !string.IsNullOrWhiteSpace(textBoxPassword.Text))
                {
                    //If Login Json file does not exist, there is no internet connection
                    if (str == null)
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_NO_INTRANET, string.Empty, StringsAndConstants.consoleOutGUI);
                        MessageBox.Show(StringsAndConstants.INTRANET_REQUIRED, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, this.Handle);
                    }
                    else if (str[0] == "false") //If Login Json file does exist, but the user do not exist
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_LOGIN_FAILED, string.Empty, StringsAndConstants.consoleOutGUI);
                        MessageBox.Show(StringsAndConstants.AUTH_INVALID, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, this.Handle);
                    }
                    else //If Login Json file does exist and user logs in
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_LOGIN_SUCCESS, string.Empty, StringsAndConstants.consoleOutGUI);
                        MainForm mForm = new MainForm(false, str[1], comboBoxServerIP.Text, comboBoxServerPort.Text, log, defList, orgList);
                        if (HardwareInfo.getOSInfoAux().Equals(StringsAndConstants.windows10))
                            DarkNet.Instance.SetWindowThemeForms(mForm, Theme.Auto);
                        this.Hide();
                        textBoxUser.Text = null;
                        textBoxPassword.Text = null;
                        textBoxUser.Select();
                        mForm.ShowDialog();
                        mForm.Close();
                        mForm.Dispose();
                        this.Show();
                    }
                }
                else //If all the mandatory fields are not filled
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_LOGIN_INCOMPLETE, string.Empty, StringsAndConstants.consoleOutGUI);
                    MessageBox.Show(StringsAndConstants.NO_AUTH, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, this.Handle);
                }
            }

            //Enables controls if login is not successful
            loadingCircle1.Visible = false;
            loadingCircle1.Active = false;
            textBoxUser.Enabled = true;
            textBoxUser.Focus();
            textBoxPassword.Enabled = true;
            comboBoxServerIP.Enabled = true;
            comboBoxServerPort.Enabled = true;
            checkBoxOfflineMode.Enabled = true;
            checkBoxOfflineMode.Checked = false;
        }

        //Handles the offline mode toggle 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
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
        private void aboutLabel_Click(object sender, EventArgs e)
        {
            AboutBox aboutForm = new AboutBox(defList, themeBool);
            if (HardwareInfo.getOSInfoAux().Equals(StringsAndConstants.windows10))
                DarkNet.Instance.SetWindowThemeForms(aboutForm, Theme.Auto);
            aboutForm.ShowDialog();
        }
    }
}
