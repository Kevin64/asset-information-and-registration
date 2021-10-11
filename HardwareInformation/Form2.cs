using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HardwareInformation
{
	public partial class Form2 : Form
	{
		private Authentication classA = new Authentication();

		private Color LIGHT_FORECOLOR = SystemColors.ControlText;
		private Color LIGHT_BACKCOLOR = SystemColors.ControlLight;
		private Color DARK_FORECOLOR = SystemColors.ControlLightLight;
		private Color DARK_BACKCOLOR = SystemColors.WindowFrame;
		private Color LIGHT_BACKGROUND = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
		private Color DARK_BACKGROUND = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
		private const string THEME_REG_PATH = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
		private const string THEME_REG_KEY = "AppsUseLightTheme";
		private const string AUTH_INVALID = "Credenciais inválidas. Tente novamente.";
		private const string NO_AUTH = "Preencha suas credenciais.";
		private const string ERROR_WINDOWTITLE = "Erro";

		public Form2()
		{
			InitializeComponent();
			ThemeInit();
			this.toolStripStatusLabel2.Text = version();
		}

		//Sets a light theme for the login form
		public void lightTheme()
		{
			this.label1.ForeColor = LIGHT_FORECOLOR;
			this.label2.ForeColor = LIGHT_FORECOLOR;
			this.textBox1.BackColor = LIGHT_BACKCOLOR;
			this.textBox1.ForeColor = LIGHT_FORECOLOR;
			this.textBox2.BackColor = LIGHT_BACKCOLOR;
			this.textBox2.ForeColor = LIGHT_FORECOLOR;
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
			this.textBox1.BackColor = DARK_BACKCOLOR;
			this.textBox1.ForeColor = DARK_FORECOLOR;
			this.textBox2.BackColor = DARK_BACKCOLOR;
			this.textBox2.ForeColor = DARK_FORECOLOR;
			this.button1.BackColor = DARK_BACKCOLOR;
			this.button1.ForeColor = DARK_FORECOLOR;
			this.BackColor = DARK_BACKGROUND;
			this.statusStrip1.BackColor = DARK_BACKGROUND;
			this.toolStripStatusLabel1.ForeColor = DARK_FORECOLOR;
			this.toolStripStatusLabel2.ForeColor = DARK_FORECOLOR;
			this.configurableQualityPictureBox1.Image = global::HardwareInformation.Properties.Resources.uti_logo_dark;
		}

		//Initializes the theme, according to the host theme
		public void ThemeInit()
		{
			try
			{
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(THEME_REG_PATH))
				{
					if (key != null)
					{
						Object o = key.GetValue(THEME_REG_KEY);
						if (o != null && o.Equals(0))
							darkTheme();
						else
							lightTheme();
					}
					else
						lightTheme();
				}
			}
			catch
			{
				lightTheme();
			}
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
				if (classA.Authenticate(textBox1.Text, textBox2.Text))
				{
					this.Visible = false;
					Form1 form = new Form1();
					form.Visible = true;
				}
				else
					MessageBox.Show(AUTH_INVALID, ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBox2.SelectAll();
				textBox2.Focus();
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
