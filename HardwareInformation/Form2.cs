using Microsoft.Win32;
using System;
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

        public void lightTheme()
        {
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textBox2.BackColor = System.Drawing.SystemColors.Control;
            this.textBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        }

        public void darkTheme()
        {
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.button1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.button1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        }

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
                        {
                            darkTheme();
                        }
                        else
                        {
                            lightTheme();
                        }
                    }
                    else
                    {
                        lightTheme();
                    }
                }
            }
            catch (Exception ex)
            {
                lightTheme();
            }
        }

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
                {
                    MessageBox.Show("Credenciais inválidas. Tente novamente.", "Alerta!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
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
