using System;
using System.Windows.Forms;

namespace HardwareInformation
{
	public partial class Form2 : Form
	{
		public Form2()
		{
			InitializeComponent();
			if (MiscMethods.ThemeInit())
				darkTheme();
			else
				lightTheme();
			comboBoxServerIP.Items.AddRange(StringsAndConstants.defaultServerIP.ToArray());
			comboBoxServerPort.Items.AddRange(StringsAndConstants.defaultServerPort.ToArray());
			comboBoxServerIP.SelectedIndex = 0;
			comboBoxServerPort.SelectedIndex = 0;
			this.toolStripStatusLabel2.Text = MiscMethods.version();
		}

		//Sets a light theme for the login form
		public void lightTheme()
		{
			this.label1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
			this.label2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
			this.label3.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
			this.label4.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
			this.textBoxUser.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
			this.textBoxUser.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
			this.textBoxPassword.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
			this.textBoxPassword.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
			this.comboBoxServerIP.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
			this.comboBoxServerIP.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
			this.comboBoxServerPort.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
			this.comboBoxServerPort.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
			this.AuthButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
			this.AuthButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
			this.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
			this.statusStrip1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
			this.toolStripStatusLabel1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
			this.toolStripStatusLabel2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
			this.configurableQualityPictureBox1.Image = global::HardwareInformation.Properties.Resources.uti_logo_light;
		}

		//Sets a dark theme for the login form
		public void darkTheme()
		{
			this.label1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
			this.label2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
			this.label3.ForeColor = StringsAndConstants.DARK_FORECOLOR;
			this.label4.ForeColor = StringsAndConstants.DARK_FORECOLOR;
			this.textBoxUser.BackColor = StringsAndConstants.DARK_BACKCOLOR;
			this.textBoxUser.ForeColor = StringsAndConstants.DARK_FORECOLOR;
			this.textBoxPassword.BackColor = StringsAndConstants.DARK_BACKCOLOR;
			this.textBoxPassword.ForeColor = StringsAndConstants.DARK_FORECOLOR;
			this.comboBoxServerIP.BackColor = StringsAndConstants.DARK_BACKCOLOR;
			this.comboBoxServerIP.ForeColor = StringsAndConstants.DARK_FORECOLOR;
			this.comboBoxServerPort.BackColor = StringsAndConstants.DARK_BACKCOLOR;
			this.comboBoxServerPort.ForeColor = StringsAndConstants.DARK_FORECOLOR;
			this.AuthButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
			this.AuthButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
			this.BackColor = StringsAndConstants.DARK_BACKGROUND;
			this.statusStrip1.BackColor = StringsAndConstants.DARK_BACKGROUND;
			this.toolStripStatusLabel1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
			this.toolStripStatusLabel2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
			this.configurableQualityPictureBox1.Image = global::HardwareInformation.Properties.Resources.uti_logo_dark;
		}

		//Checks the user/password and shows the main form
		private void button1_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(textBoxUser.Text) && !string.IsNullOrWhiteSpace(textBoxPassword.Text))
			{
				if(MiscMethods.offlineLogin(textBoxUser.Text, textBoxPassword.Text))
                {
					this.Visible = false;
					Form1 form = new Form1(true);
					form.Visible = true;
				}
                else
                {
					string[] str = LoginFileReader.fetchInfo(textBoxUser.Text, textBoxPassword.Text, comboBoxServerIP.Text, comboBoxServerPort.Text);

					if(str == null)
						MessageBox.Show(StringsAndConstants.INTRANET_REQUIRED, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
					else if (str[0] == "false")
						MessageBox.Show(StringsAndConstants.AUTH_INVALID, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
					else
					{
						this.Visible = false;
						Form1 form = new Form1(false);
						form.Visible = true;
					}
					textBoxPassword.SelectAll();
					textBoxPassword.Focus();
				}
			}
			else
			{
				MessageBox.Show(StringsAndConstants.NO_AUTH, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxPassword.SelectAll();
				textBoxPassword.Focus();
			}
		}
    }
}
