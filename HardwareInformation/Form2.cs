using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HardwareInformation
{
    public partial class Form2 : Form
    {
        private Authentication classA = new Authentication();

        public Form2()
        {
            InitializeComponent();
            ThemeInit();
        }

        //Sets a light theme for the login form
        public void lightTheme()
        {
            this.label1.ForeColor = SystemColors.ControlText;
            this.label2.ForeColor = SystemColors.ControlText;
            this.textBox1.BackColor = SystemColors.Control;
            this.textBox1.ForeColor = SystemColors.ControlText;
            this.textBox2.BackColor = SystemColors.Control;
            this.textBox2.ForeColor = SystemColors.ControlText;
            this.button1.BackColor = SystemColors.ControlLight;
            this.button1.ForeColor = SystemColors.ControlText;
            this.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.configurableQualityPictureBox1.Image = global::HardwareInformation.Properties.Resources.uti_logo_light;
        }

        //Sets a dark theme for the login form
        public void darkTheme()
        {
            this.label1.ForeColor = SystemColors.ControlLightLight;
            this.label2.ForeColor = SystemColors.ControlLightLight;
            this.textBox1.BackColor = SystemColors.WindowFrame;
            this.textBox1.ForeColor = SystemColors.ControlLightLight;
            this.textBox2.BackColor = SystemColors.WindowFrame;
            this.textBox2.ForeColor = SystemColors.ControlLightLight;
            this.button1.BackColor = SystemColors.WindowFrame;
            this.button1.ForeColor = SystemColors.ControlLightLight;
            this.BackColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.configurableQualityPictureBox1.Image = global::HardwareInformation.Properties.Resources.uti_logo_dark;
        }

        //Initializes the theme, according to the host theme
        public void ThemeInit()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue("AppsUseLightTheme");
                        if (o != null && o.Equals(0))
                            darkTheme();
                        else
                            lightTheme();
                    }
                    else
                        lightTheme();
                }
            }
            catch (Exception ex)
            {
                lightTheme();
            }
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
                    MessageBox.Show("Credenciais inválidas. Tente novamente.", "Alerta!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox2.SelectAll();
                textBox2.Focus();
            }
            else
            {
                MessageBox.Show("Preencha suas credenciais.", "Erro!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.SelectAll();
                textBox2.Focus();
            }
        }
    }
}
