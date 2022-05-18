using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HardwareInformation
{
	public partial class Form2 : Form
	{
		private Color LIGHT_FORECOLOR = SystemColors.ControlText;
		private Color LIGHT_BACKCOLOR = SystemColors.ControlLight;
		private Color DARK_FORECOLOR = SystemColors.ControlLightLight;
		private Color DARK_BACKCOLOR = SystemColors.WindowFrame;
		private Color LIGHT_BACKGROUND = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
		private Color DARK_BACKGROUND = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
		private const string AUTH_INVALID = "Credenciais inválidas. Tente novamente.";
		private const string INTRANET_REQUIRED = "É necessário conexão com a intranet.";
		private const string NO_AUTH = "Preencha suas credenciais.";
		private const string ERROR_WINDOWTITLE = "Erro";
		private bool themeDark;

		public Form2()
		{
			InitializeComponent();
			themeDark = MiscMethods.ThemeInit();
			if (themeDark)
				darkTheme();
			else
				lightTheme();
			comboBoxServer.Items.Add("192.168.76.103");
			comboBoxPorta.Items.Add("8081");
			comboBoxServer.SelectedIndex = 0;
			comboBoxPorta.SelectedIndex = 0;
			this.toolStripStatusLabel2.Text = version();
		}

		//Sets a light theme for the login form
		public void lightTheme()
		{
			this.label1.ForeColor = LIGHT_FORECOLOR;
			this.label2.ForeColor = LIGHT_FORECOLOR;
			this.label3.ForeColor = LIGHT_FORECOLOR;
			this.label4.ForeColor = LIGHT_FORECOLOR;
			this.textBox1.BackColor = LIGHT_BACKCOLOR;
			this.textBox1.ForeColor = LIGHT_FORECOLOR;
			this.textBox2.BackColor = LIGHT_BACKCOLOR;
			this.textBox2.ForeColor = LIGHT_FORECOLOR;
			this.comboBoxServer.BackColor = LIGHT_BACKCOLOR;
			this.comboBoxServer.ForeColor = LIGHT_FORECOLOR;
			this.comboBoxPorta.BackColor = LIGHT_BACKCOLOR;
			this.comboBoxPorta.ForeColor = LIGHT_FORECOLOR;
			this.button1.BackColor = LIGHT_BACKCOLOR;
			this.button1.ForeColor = LIGHT_FORECOLOR;
			this.BackColor = LIGHT_BACKGROUND;
			this.statusStrip1.BackColor = LIGHT_BACKGROUND;
			this.toolStripStatusLabel1.ForeColor = LIGHT_FORECOLOR;
			this.toolStripStatusLabel2.ForeColor = LIGHT_FORECOLOR;
			this.configurableQualityPictureBox1.Image = global::HardwareInformation.Properties.Resources.uti_logo_light;
		}

		//Sets a dark theme for the login form
		public void darkTheme()
		{
			this.label1.ForeColor = DARK_FORECOLOR;
			this.label2.ForeColor = DARK_FORECOLOR;
			this.label3.ForeColor = DARK_FORECOLOR;
			this.label4.ForeColor = DARK_FORECOLOR;
			this.textBox1.BackColor = DARK_BACKCOLOR;
			this.textBox1.ForeColor = DARK_FORECOLOR;
			this.textBox2.BackColor = DARK_BACKCOLOR;
			this.textBox2.ForeColor = DARK_FORECOLOR;
			this.comboBoxServer.BackColor = DARK_BACKCOLOR;
			this.comboBoxServer.ForeColor = DARK_FORECOLOR;
			this.comboBoxPorta.BackColor = DARK_BACKCOLOR;
			this.comboBoxPorta.ForeColor = DARK_FORECOLOR;
			this.button1.BackColor = DARK_BACKCOLOR;
			this.button1.ForeColor = DARK_FORECOLOR;
			this.BackColor = DARK_BACKGROUND;
			this.statusStrip1.BackColor = DARK_BACKGROUND;
			this.toolStripStatusLabel1.ForeColor = DARK_FORECOLOR;
			this.toolStripStatusLabel2.ForeColor = DARK_FORECOLOR;
			this.configurableQualityPictureBox1.Image = global::HardwareInformation.Properties.Resources.uti_logo_dark;
		}

		//Fetches the program's binary version
		private string version()
		{
			return "v" + Application.ProductVersion;
		}

		//Fetches the program's binary version (for unstable releases)
		private string version(string testBranch)
		{
			return "v" + Application.ProductVersion + "-" + testBranch;
		}

		//Checks the user/password and shows the main form
		private void button1_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox2.Text))
			{
				if(MiscMethods.offlineLogin(textBox1.Text, textBox2.Text))
                {
					this.Visible = false;
					Form1 form = new Form1(true);
					form.Visible = true;
				}
                else
                {
					string[] str = LoginFileReader.fetchInfo(textBox1.Text, textBox2.Text, comboBoxServer.Text, comboBoxPorta.Text);

					if(str == null)
						MessageBox.Show(INTRANET_REQUIRED, ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
					else if (str[0] == "false")
						MessageBox.Show(AUTH_INVALID, ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
					else
					{
						this.Visible = false;
						Form1 form = new Form1(false);
						form.Visible = true;
					}
					textBox2.SelectAll();
					textBox2.Focus();
				}
			}
			else
			{
				MessageBox.Show(NO_AUTH, ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBox2.SelectAll();
				textBox2.Focus();
			}
		}
    }
}
