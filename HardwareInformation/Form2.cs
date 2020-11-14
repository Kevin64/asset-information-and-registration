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
