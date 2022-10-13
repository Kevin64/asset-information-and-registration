﻿using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ConstantsDLL;
using HardwareInformation.Properties;
using JsonFileReaderDLL;
using LogGeneratorDLL;

namespace HardwareInformation
{
    public partial class LoginForm : Form
    {
        private LogGenerator log;
        private BackgroundWorker backgroundWorker1;
        private string[] str = { };
        bool themeBool;

        public LoginForm(LogGenerator l)
        {
            InitializeComponent();

            this.toolStripStatusLabel1.Text = StringsAndConstants.statusBarTextForm2;

            //Program version
            this.toolStripStatusLabel2.Text = MiscMethods.version(Resources.dev_status);

            log = l;
            themeBool = MiscMethods.ThemeInit();
            if (themeBool)
                darkTheme();
            else
                lightTheme();

            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_THEME, themeBool.ToString(), StringsAndConstants.consoleOutGUI);

            comboBoxServerIP.Items.AddRange(StringsAndConstants.defaultServerIP.ToArray());
            comboBoxServerPort.Items.AddRange(StringsAndConstants.defaultServerPort.ToArray());
#if DEBUG
            comboBoxServerIP.SelectedIndex = 1;
            comboBoxServerPort.SelectedIndex = 0;
#else
            comboBoxServerIP.SelectedIndex = 0;
			comboBoxServerPort.SelectedIndex = 0;
#endif
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

            this.loadingCircle1.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;

            this.textBoxUser.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxUser.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBoxPassword.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxPassword.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            this.comboBoxServerIP.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxServerIP.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxServerIP.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxServerIP.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxServerPort.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxServerPort.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxServerPort.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxServerPort.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;

            this.AuthButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.AuthButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.AuthButton.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.AuthButton.FlatStyle = System.Windows.Forms.FlatStyle.System;

            this.checkBoxOfflineMode.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.checkBoxOfflineMode.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            this.toolStripStatusLabel1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripStatusLabel2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripStatusLabel1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.toolStripStatusLabel2.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.statusStrip1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            this.configurableQualityPictureBox1.Image = global::HardwareInformation.Properties.Resources.uti_logo_light;
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

            this.toolStripStatusLabel1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripStatusLabel2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripStatusLabel1.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.toolStripStatusLabel2.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.statusStrip1.BackColor = StringsAndConstants.DARK_BACKGROUND;

            this.configurableQualityPictureBox1.Image = global::HardwareInformation.Properties.Resources.uti_logo_dark;
        }

        //Loads the form, sets some combobox values
        private void Form2_Load(object sender, EventArgs e)
        {
            FormClosing += Form2_FormClosing;
        }

        //Handles the closing of the current form
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CLOSING_LOGINFORM, string.Empty, StringsAndConstants.consoleOutGUI);
            log.LogWrite(StringsAndConstants.LOG_MISC, StringsAndConstants.LOG_SEPARATOR_SMALL, string.Empty, StringsAndConstants.consoleOutGUI);
            File.Delete(StringsAndConstants.biosPath);
            File.Delete(StringsAndConstants.loginPath);
            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }

        //Checks the user/password and shows the main form
        private async void button1_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_INIT_LOGIN, textBoxUser.Text, StringsAndConstants.consoleOutGUI);
            loadingCircle1.Visible = true;
            loadingCircle1.Active = true;
            if (checkBoxOfflineMode.Checked)
            {
                MainForm form = new MainForm(true, StringsAndConstants.OFFLINE_MODE_ACTIVATED, null, null, log);
                this.Hide();
                form.ShowDialog();
                form.Close();
                this.Show();
            }
            else
            {
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SERVER_DETAIL, comboBoxServerIP.Text + ":" + comboBoxServerPort.Text, StringsAndConstants.consoleOutGUI);
                str = await LoginFileReader.fetchInfo(textBoxUser.Text, textBoxPassword.Text, comboBoxServerIP.Text, comboBoxServerPort.Text);
                if (!string.IsNullOrWhiteSpace(textBoxUser.Text) && !string.IsNullOrWhiteSpace(textBoxPassword.Text))
                {
                    if (str == null)
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_NO_INTRANET, string.Empty, StringsAndConstants.consoleOutGUI);
                        MessageBox.Show(StringsAndConstants.INTRANET_REQUIRED, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (str[0] == "false")
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_LOGIN_FAILED, string.Empty, StringsAndConstants.consoleOutGUI);
                        MessageBox.Show(StringsAndConstants.AUTH_INVALID, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_LOGIN_SUCCESS, string.Empty, StringsAndConstants.consoleOutGUI);
                        MainForm form = new MainForm(false, str[1], comboBoxServerIP.Text, comboBoxServerPort.Text, log);
                        this.Hide();
                        form.ShowDialog();
                        form.Close();
                        this.Show();
                    }
                    textBoxPassword.SelectAll();
                    textBoxPassword.Focus();
                }
                else
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_LOGIN_INCOMPLETE, string.Empty, StringsAndConstants.consoleOutGUI);
                    MessageBox.Show(StringsAndConstants.NO_AUTH, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxPassword.SelectAll();
                    textBoxPassword.Focus();
                }
            }
            loadingCircle1.Visible = false;
            loadingCircle1.Active = false;
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
    }
}
