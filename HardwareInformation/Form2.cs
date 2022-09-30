using System;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Windows.Forms;
using ConstantsDLL;
using JsonFileReaderDLL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HardwareInformation
{
    public partial class Form2 : Form
    {
        private BackgroundWorker backgroundWorker1;
        private string[] str = { };
        public Form2()
        {
            InitializeComponent();
            if (MiscMethods.ThemeInit())
                darkTheme();
            else
                lightTheme();
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

            this.toolStripStatusLabel1.Text = StringsAndConstants.statusBarTextForm2;
            //Change this for alpha, beta and final releases - uncomment the appropriate line
            //this.toolStripStatusLabel2.Text = MiscMethods.version();
            this.toolStripStatusLabel2.Text = MiscMethods.version("beta");
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

        //Checks the user/password and shows the main form
        private void button1_Click(object sender, EventArgs e)
        {
            loadingCircle1.Visible = true;
            loadingCircle1.Active = true;
            if (checkBoxOfflineMode.Checked)
            {
                Form1 form = new Form1(true, StringsAndConstants.OFFLINE_MODE_ACTIVATED, null, null);
                this.Hide();
                form.ShowDialog();
                form.Close();
                this.Show();
            }
            else
            {
                startAsync(sender, e);
                if (!string.IsNullOrWhiteSpace(textBoxUser.Text) && !string.IsNullOrWhiteSpace(textBoxPassword.Text))
                {
                    if (str == null)
                        MessageBox.Show(StringsAndConstants.INTRANET_REQUIRED, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (str[0] == "false")
                        MessageBox.Show(StringsAndConstants.AUTH_INVALID, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        Form1 form = new Form1(false, str[1], comboBoxServerIP.Text, comboBoxServerPort.Text);
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
                    MessageBox.Show(StringsAndConstants.NO_AUTH, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxPassword.SelectAll();
                    textBoxPassword.Focus();
                }
            }
            loadingCircle1.Visible = false;
            loadingCircle1.Active = false;
        }

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

        //Starts the worker for threading
        private void startAsync(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
                backgroundWorker1.RunWorkerAsync();
        }

        //Runs the collectThread method in a separate thread
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            str = LoginFileReader.fetchInfo(textBoxUser.Text, textBoxPassword.Text, comboBoxServerIP.Text, comboBoxServerPort.Text, worker, e);
        }
    }
}
