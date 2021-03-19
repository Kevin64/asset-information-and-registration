using Microsoft.Win32;
using System;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace HardwareInformation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lblBM = new System.Windows.Forms.Label();
            this.lblModel = new System.Windows.Forms.Label();
            this.lblSerialNo = new System.Windows.Forms.Label();
            this.lblProcName = new System.Windows.Forms.Label();
            this.lblPM = new System.Windows.Forms.Label();
            this.lblHDSize = new System.Windows.Forms.Label();
            this.lblOS = new System.Windows.Forms.Label();
            this.lblHostname = new System.Windows.Forms.Label();
            this.lblMac = new System.Windows.Forms.Label();
            this.lblIP = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.label17 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.cadastra = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.comboBox6 = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.comboBox7 = new System.Windows.Forms.ComboBox();
            this.comboBox8 = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.coleta = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.lblBIOS = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.lblBIOSType = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblMediaType = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.comboBoxTheme = new System.Windows.Forms.ComboBox();
            this.label26 = new System.Windows.Forms.Label();
            this.lblGPUInfo = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblBM
            // 
            this.lblBM.AutoSize = true;
            this.lblBM.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblBM.Location = new System.Drawing.Point(185, 25);
            this.lblBM.Name = "lblBM";
            this.lblBM.Size = new System.Drawing.Size(13, 13);
            this.lblBM.TabIndex = 7;
            this.lblBM.Text = "a";
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblModel.Location = new System.Drawing.Point(185, 50);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(13, 13);
            this.lblModel.TabIndex = 8;
            this.lblModel.Text = "b";
            // 
            // lblSerialNo
            // 
            this.lblSerialNo.AutoSize = true;
            this.lblSerialNo.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblSerialNo.Location = new System.Drawing.Point(185, 75);
            this.lblSerialNo.Name = "lblSerialNo";
            this.lblSerialNo.Size = new System.Drawing.Size(13, 13);
            this.lblSerialNo.TabIndex = 9;
            this.lblSerialNo.Text = "c";
            // 
            // lblProcName
            // 
            this.lblProcName.AutoSize = true;
            this.lblProcName.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblProcName.Location = new System.Drawing.Point(185, 100);
            this.lblProcName.Name = "lblProcName";
            this.lblProcName.Size = new System.Drawing.Size(13, 13);
            this.lblProcName.TabIndex = 10;
            this.lblProcName.Text = "d";
            // 
            // lblPM
            // 
            this.lblPM.AutoSize = true;
            this.lblPM.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblPM.Location = new System.Drawing.Point(185, 125);
            this.lblPM.Name = "lblPM";
            this.lblPM.Size = new System.Drawing.Size(13, 13);
            this.lblPM.TabIndex = 11;
            this.lblPM.Text = "e";
            // 
            // lblHDSize
            // 
            this.lblHDSize.AutoSize = true;
            this.lblHDSize.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblHDSize.Location = new System.Drawing.Point(185, 150);
            this.lblHDSize.Name = "lblHDSize";
            this.lblHDSize.Size = new System.Drawing.Size(10, 13);
            this.lblHDSize.TabIndex = 12;
            this.lblHDSize.Text = "f";
            // 
            // lblOS
            // 
            this.lblOS.AutoSize = true;
            this.lblOS.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblOS.Location = new System.Drawing.Point(185, 219);
            this.lblOS.Name = "lblOS";
            this.lblOS.Size = new System.Drawing.Size(13, 13);
            this.lblOS.TabIndex = 13;
            this.lblOS.Text = "g";
            // 
            // lblHostname
            // 
            this.lblHostname.AutoSize = true;
            this.lblHostname.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblHostname.Location = new System.Drawing.Point(185, 244);
            this.lblHostname.Name = "lblHostname";
            this.lblHostname.Size = new System.Drawing.Size(13, 13);
            this.lblHostname.TabIndex = 15;
            this.lblHostname.Text = "h";
            // 
            // lblMac
            // 
            this.lblMac.AutoSize = true;
            this.lblMac.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMac.Location = new System.Drawing.Point(185, 269);
            this.lblMac.Name = "lblMac";
            this.lblMac.Size = new System.Drawing.Size(9, 13);
            this.lblMac.TabIndex = 18;
            this.lblMac.Text = "i";
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblIP.Location = new System.Drawing.Point(185, 294);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(9, 13);
            this.lblIP.TabIndex = 19;
            this.lblIP.Text = "j";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(25, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Marca:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(25, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Modelo:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(25, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Número Serial:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(25, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(146, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Processador e nº de núcleos:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(25, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Memória RAM e nº de slots:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label6.Location = new System.Drawing.Point(25, 150);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(153, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Armazenamento (espaço total):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label7.Location = new System.Drawing.Point(25, 219);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Sistema Operacional:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label8.Location = new System.Drawing.Point(25, 244);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(113, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Nome do Computador:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label9.Location = new System.Drawing.Point(25, 269);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(118, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Endereço MAC do NIC:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label10.Location = new System.Drawing.Point(25, 294);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(105, 13);
            this.label10.TabIndex = 9;
            this.label10.Text = "Endereço IP do NIC:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label11.Location = new System.Drawing.Point(20, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Patrimônio:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label12.Location = new System.Drawing.Point(20, 50);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 13);
            this.label12.TabIndex = 11;
            this.label12.Text = "Lacre (se houver):";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label13.Location = new System.Drawing.Point(20, 100);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(40, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "Prédio:";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox1.Location = new System.Drawing.Point(164, 22);
            this.textBox1.MaxLength = 6;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(227, 20);
            this.textBox1.TabIndex = 34;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox2.Location = new System.Drawing.Point(164, 47);
            this.textBox2.MaxLength = 10;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(227, 20);
            this.textBox2.TabIndex = 35;
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox3.Location = new System.Drawing.Point(164, 72);
            this.textBox3.MaxLength = 4;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(92, 20);
            this.textBox3.TabIndex = 36;
            this.textBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox4.Location = new System.Drawing.Point(358, 72);
            this.textBox4.MaxLength = 1;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(33, 20);
            this.textBox4.TabIndex = 37;
            this.textBox4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox4_KeyPress);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label14.Location = new System.Drawing.Point(20, 75);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(135, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "Sala (0000 se não houver):";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label15.Location = new System.Drawing.Point(20, 125);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(137, 13);
            this.label15.TabIndex = 14;
            this.label15.Text = "Cadastrado no servidor AD:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label16.Location = new System.Drawing.Point(20, 250);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(95, 13);
            this.label16.TabIndex = 16;
            this.label16.Text = "Última formatação:";
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "21",
            "67",
            "74 - A",
            "74 - B",
            "74 - C",
            "ANTIGA REITORIA",
            "APOIO",
            "BIBLIOTECA SETORIAL"});
            this.comboBox1.Location = new System.Drawing.Point(164, 97);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(92, 21);
            this.comboBox1.Sorted = true;
            this.comboBox1.TabIndex = 38;
            // 
            // comboBox2
            // 
            this.comboBox2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "NAO",
            "SIM"});
            this.comboBox2.Location = new System.Drawing.Point(164, 122);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(92, 21);
            this.comboBox2.Sorted = true;
            this.comboBox2.TabIndex = 39;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(32, 527);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScrollBarsEnabled = false;
            this.webBrowser1.Size = new System.Drawing.Size(957, 45);
            this.webBrowser1.TabIndex = 19;
            this.webBrowser1.TabStop = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label17.Location = new System.Drawing.Point(20, 150);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(44, 13);
            this.label17.TabIndex = 15;
            this.label17.Text = "Padrão:";
            // 
            // comboBox3
            // 
            this.comboBox3.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "AD",
            "PCCLI"});
            this.comboBox3.Location = new System.Drawing.Point(164, 147);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(92, 21);
            this.comboBox3.TabIndex = 40;
            // 
            // cadastra
            // 
            this.cadastra.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.cadastra.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cadastra.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cadastra.Location = new System.Drawing.Point(348, 583);
            this.cadastra.Name = "cadastra";
            this.cadastra.Size = new System.Drawing.Size(167, 32);
            this.cadastra.TabIndex = 50;
            this.cadastra.Text = "Cadastrar / Atualizar dados";
            this.cadastra.UseVisualStyleBackColor = false;
            this.cadastra.Click += new System.EventHandler(this.cadastra_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label18.Location = new System.Drawing.Point(262, 100);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(45, 13);
            this.label18.TabIndex = 48;
            this.label18.Text = "Em uso:";
            // 
            // comboBox4
            // 
            this.comboBox4.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Items.AddRange(new object[] {
            "NAO",
            "SIM"});
            this.comboBox4.Location = new System.Drawing.Point(313, 97);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(78, 21);
            this.comboBox4.TabIndex = 41;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label19.Location = new System.Drawing.Point(262, 125);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(49, 13);
            this.label19.TabIndex = 50;
            this.label19.Text = "Etiqueta:";
            // 
            // comboBox5
            // 
            this.comboBox5.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox5.FormattingEnabled = true;
            this.comboBox5.Items.AddRange(new object[] {
            "NAO",
            "SIM"});
            this.comboBox5.Location = new System.Drawing.Point(313, 122);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(78, 21);
            this.comboBox5.TabIndex = 42;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label20.Location = new System.Drawing.Point(262, 150);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(31, 13);
            this.label20.TabIndex = 53;
            this.label20.Text = "Tipo:";
            // 
            // comboBox6
            // 
            this.comboBox6.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox6.FormattingEnabled = true;
            this.comboBox6.Items.AddRange(new object[] {
            "DESKTOP",
            "NOTEBOOK",
            "TABLET"});
            this.comboBox6.Location = new System.Drawing.Point(313, 147);
            this.comboBox6.Name = "comboBox6";
            this.comboBox6.Size = new System.Drawing.Size(78, 21);
            this.comboBox6.TabIndex = 43;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label21.Location = new System.Drawing.Point(595, 503);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(49, 13);
            this.label21.TabIndex = 17;
            this.label21.Text = "Servidor:";
            // 
            // comboBox7
            // 
            this.comboBox7.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox7.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox7.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox7.FormattingEnabled = true;
            this.comboBox7.Items.AddRange(new object[] {
            "192.168.76.103"});
            this.comboBox7.Location = new System.Drawing.Point(739, 500);
            this.comboBox7.Name = "comboBox7";
            this.comboBox7.Size = new System.Drawing.Size(115, 21);
            this.comboBox7.TabIndex = 45;
            // 
            // comboBox8
            // 
            this.comboBox8.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox8.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox8.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox8.FormattingEnabled = true;
            this.comboBox8.Items.AddRange(new object[] {
            "8081"});
            this.comboBox8.Location = new System.Drawing.Point(911, 500);
            this.comboBox8.Name = "comboBox8";
            this.comboBox8.Size = new System.Drawing.Size(78, 21);
            this.comboBox8.TabIndex = 46;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label22.Location = new System.Drawing.Point(860, 503);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(35, 13);
            this.label22.TabIndex = 18;
            this.label22.Text = "Porta:";
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.monthCalendar1.Location = new System.Drawing.Point(164, 172);
            this.monthCalendar1.MaxSelectionCount = 1;
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 44;
            this.monthCalendar1.TabStop = false;
            // 
            // coleta
            // 
            this.coleta.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.coleta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.coleta.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.coleta.Location = new System.Drawing.Point(220, 497);
            this.coleta.Name = "coleta";
            this.coleta.Size = new System.Drawing.Size(141, 25);
            this.coleta.TabIndex = 49;
            this.coleta.Text = "Coletar Novamente";
            this.coleta.UseVisualStyleBackColor = false;
            this.coleta.Click += new System.EventHandler(this.coleta_Click);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label23.Location = new System.Drawing.Point(262, 75);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(90, 13);
            this.label23.TabIndex = 55;
            this.label23.Text = "Letra (se houver):";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label24.Location = new System.Drawing.Point(25, 344);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(115, 13);
            this.label24.TabIndex = 56;
            this.label24.Text = "Versão da BIOS/UEFI:";
            // 
            // lblBIOS
            // 
            this.lblBIOS.AutoSize = true;
            this.lblBIOS.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblBIOS.Location = new System.Drawing.Point(185, 344);
            this.lblBIOS.Name = "lblBIOS";
            this.lblBIOS.Size = new System.Drawing.Size(9, 13);
            this.lblBIOS.TabIndex = 57;
            this.lblBIOS.Text = "l";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.button1.Location = new System.Drawing.Point(521, 583);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 32);
            this.button1.TabIndex = 51;
            this.button1.Text = "Acessar sistema de patrimônios";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label25.Location = new System.Drawing.Point(25, 319);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(88, 13);
            this.label25.TabIndex = 62;
            this.label25.Text = "Tipo de firmware:";
            // 
            // lblBIOSType
            // 
            this.lblBIOSType.AutoSize = true;
            this.lblBIOSType.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblBIOSType.Location = new System.Drawing.Point(185, 319);
            this.lblBIOSType.Name = "lblBIOSType";
            this.lblBIOSType.Size = new System.Drawing.Size(13, 13);
            this.lblBIOSType.TabIndex = 63;
            this.lblBIOSType.Text = "k";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Image = global::HardwareInformation.Properties.Resources.banner_dark;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(-7, -2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1032, 109);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 64;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblGPUInfo);
            this.groupBox1.Controls.Add(this.label29);
            this.groupBox1.Controls.Add(this.lblMediaType);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lblOS);
            this.groupBox1.Controls.Add(this.lblBIOSType);
            this.groupBox1.Controls.Add(this.lblHDSize);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.lblPM);
            this.groupBox1.Controls.Add(this.lblProcName);
            this.groupBox1.Controls.Add(this.lblSerialNo);
            this.groupBox1.Controls.Add(this.lblBIOS);
            this.groupBox1.Controls.Add(this.lblModel);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.lblBM);
            this.groupBox1.Controls.Add(this.lblHostname);
            this.groupBox1.Controls.Add(this.lblMac);
            this.groupBox1.Controls.Add(this.lblIP);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox1.Location = new System.Drawing.Point(32, 113);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(537, 378);
            this.groupBox1.TabIndex = 65;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dados do computador";
            // 
            // lblMediaType
            // 
            this.lblMediaType.AutoSize = true;
            this.lblMediaType.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMediaType.Location = new System.Drawing.Point(185, 172);
            this.lblMediaType.Name = "lblMediaType";
            this.lblMediaType.Size = new System.Drawing.Size(15, 13);
            this.lblMediaType.TabIndex = 65;
            this.lblMediaType.Text = "m";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label27.Location = new System.Drawing.Point(25, 172);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(124, 13);
            this.label27.TabIndex = 64;
            this.label27.Text = "Tipo de armazenamento:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.comboBox1);
            this.groupBox2.Controls.Add(this.comboBox2);
            this.groupBox2.Controls.Add(this.comboBox6);
            this.groupBox2.Controls.Add(this.monthCalendar1);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.textBox4);
            this.groupBox2.Controls.Add(this.comboBox3);
            this.groupBox2.Controls.Add(this.comboBox5);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.comboBox4);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox2.Location = new System.Drawing.Point(575, 113);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(414, 378);
            this.groupBox2.TabIndex = 66;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Dados do patrimônio, manutenção e de localização";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)));
            this.toolStripStatusLabel2.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripStatusLabel2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(56, 19);
            this.toolStripStatusLabel2.Text = "v" + System.Windows.Forms.Application.ProductVersion;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 627);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1025, 24);
            this.statusStrip1.TabIndex = 60;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Top | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(954, 19);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "Sistema desenvolvido pelo servidor Kevin Costa, SIAPE 1971957, para uso no serviç" +
    "o da Unidade de Tecnologia da Informação do CCSH - UFSM";
            // 
            // comboBoxTheme
            // 
            this.comboBoxTheme.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBoxTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTheme.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBoxTheme.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxTheme.FormattingEnabled = true;
            this.comboBoxTheme.Items.AddRange(new object[] {
            "Automático (Tema do sistema)",
            "Claro",
            "Escuro"});
            this.comboBoxTheme.Location = new System.Drawing.Point(810, 590);
            this.comboBoxTheme.Name = "comboBoxTheme";
            this.comboBoxTheme.Size = new System.Drawing.Size(179, 21);
            this.comboBoxTheme.TabIndex = 47;
            this.comboBoxTheme.SelectedIndexChanged += new System.EventHandler(this.comboBoxTheme_SelectedIndexChanged);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label26.Location = new System.Drawing.Point(767, 593);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(37, 13);
            this.label26.TabIndex = 68;
            this.label26.Text = "Tema:";
            // 
            // lblGPUInfo
            // 
            this.lblGPUInfo.AutoSize = true;
            this.lblGPUInfo.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblGPUInfo.Location = new System.Drawing.Point(185, 195);
            this.lblGPUInfo.Name = "lblGPUInfo";
            this.lblGPUInfo.Size = new System.Drawing.Size(13, 13);
            this.lblGPUInfo.TabIndex = 67;
            this.lblGPUInfo.Text = "n";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label29.Location = new System.Drawing.Point(25, 195);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(84, 13);
            this.label29.TabIndex = 66;
            this.label29.Text = "Placa de Vídeo:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(1025, 651);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.comboBoxTheme);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.coleta);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.comboBox8);
            this.Controls.Add(this.comboBox7);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.cadastra);
            this.Controls.Add(this.webBrowser1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Coleta de hardware e cadastro de patrimônio / Unidade de Tecnologia da Informação" +
    " do CCSH - UFSM";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Label lblBM;
        private Label lblModel;
        private Label lblSerialNo;
        private Label lblProcName;
        private Label lblPM;
        private Label lblHDSize;
        private Label lblHostname;
        private Label lblMac;
        private Label lblIP;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private Label label14;
        private Label label15;
        private Label label16;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private WebBrowser webBrowser1;
        private Label lblOS;
        private Label label17;
        private ComboBox comboBox3;
        private Label label18;
        private ComboBox comboBox4;
        private Label label19;
        private ComboBox comboBox5;
        private Button cadastra;
        private Label label20;
        private ComboBox comboBox6;
        private ComboBox comboBox7;
        private Label label21;
        private string servidor_web, porta;
        private string varPatrimonio, varLacre, varSala, varBoard, varModel,
           varSerial, varProc, varRAM, varHD, varHDType, varGPUInfo, varOS, varHostname,
           varMac, varIP, varPredio, varCadastrado, varPadrao, varCalend,
           varUso, varTag, varTipo, varBIOS, varBIOSType;
        private Label label25;
        private Label lblBIOSType;
        private PictureBox pictureBox1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label lblMediaType;
        private Label label27;
        private Label lblGPUInfo;
        private Label label29;
        private ToolStripStatusLabel toolStripStatusLabel2;

        public void lightTheme()
        {
            this.lblBM.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblModel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSerialNo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblProcName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPM.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblHDSize.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblMediaType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblOS.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblHostname.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblMac.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblIP.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label13.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textBox2.BackColor = System.Drawing.SystemColors.Control;
            this.textBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textBox3.BackColor = System.Drawing.SystemColors.Control;
            this.textBox3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textBox4.BackColor = System.Drawing.SystemColors.Control;
            this.textBox4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label15.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label16.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.comboBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBox2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.comboBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBox3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.comboBox3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cadastra.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cadastra.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label18.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBox4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.comboBox4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label19.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBox5.BackColor = System.Drawing.SystemColors.ControlLight;
            this.comboBox5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label20.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBox6.BackColor = System.Drawing.SystemColors.ControlLight;
            this.comboBox6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label21.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBox7.BackColor = System.Drawing.SystemColors.ControlLight;
            this.comboBox7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBox8.BackColor = System.Drawing.SystemColors.ControlLight;
            this.comboBox8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label22.ForeColor = System.Drawing.SystemColors.ControlText;
            this.monthCalendar1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.coleta.BackColor = System.Drawing.SystemColors.ControlLight;
            this.coleta.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label23.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label24.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblBIOS.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label25.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblBIOSType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripStatusLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBoxTheme.BackColor = System.Drawing.SystemColors.ControlLight;
            this.comboBoxTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label26.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label27.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label29.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblGPUInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.webBrowser1.DocumentText = "<html><body style='background-color: white;'></body></html>";
            this.pictureBox1.Image = global::HardwareInformation.Properties.Resources.banner_light;
        }

        public void darkTheme()
        {
            this.lblBM.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblModel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblSerialNo.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblProcName.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblPM.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblHDSize.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMediaType.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblOS.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblHostname.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMac.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblIP.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label12.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label13.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox3.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox4.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label14.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label15.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label16.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label17.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox3.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cadastra.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.cadastra.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label18.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox4.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label19.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox5.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label20.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox6.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label21.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox7.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox7.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBox8.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBox8.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label22.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.monthCalendar1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.coleta.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.coleta.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label23.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label24.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblBIOS.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.button1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.button1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label25.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblBIOSType.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripStatusLabel2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxTheme.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBoxTheme.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label26.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label27.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label29.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblGPUInfo.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.webBrowser1.DocumentText = "<html><body style='background-color: #404040;'></body></html>";
            this.pictureBox1.Image = global::HardwareInformation.Properties.Resources.banner_dark;
        }

        private void comboBoxTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxTheme.SelectedIndex.Equals(0))
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
            else if (this.comboBoxTheme.SelectedIndex.Equals(1))
            {
                lightTheme();
            }
            else
            {
                darkTheme();
            }
        }

        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ComboBox comboBoxTheme;
        private Label label26;
        private string coletando = "Coletando...";
                
        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(comboBox7.Text) && !string.IsNullOrWhiteSpace(comboBox8.Text))
                System.Diagnostics.Process.Start("http://" + comboBox7.Text + ":" + comboBox8.Text);
            else
                MessageBox.Show("Para acessar, selecione o servidor e a porta!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private ComboBox comboBox8;
        private MonthCalendar monthCalendar1;
        private Button coleta;
        private Label label23;
        private Label label24;
        private Label lblBIOS;
        private Button button1;
        private Label label22;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            coleta_Click(sender, e);
            comboBox1.SelectedIndex = 4;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 1;
            comboBox4.SelectedIndex = 1;
            comboBox5.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;
            comboBoxTheme.SelectedIndex = 0;
            this.FormClosing += Form1_FormClosing;
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void coleta_Click(object sender, EventArgs e)
        {
            varBoard = lblBM.Text = coletando;
            varModel = lblModel.Text = coletando;
            varSerial = lblSerialNo.Text = coletando;
            varProc = lblProcName.Text = coletando;
            varRAM = lblPM.Text = coletando;
            varHD = lblHDSize.Text = coletando;
            varHDType = lblMediaType.Text = coletando;
            varGPUInfo = lblGPUInfo.Text = coletando;
            varOS = lblOS.Text = coletando;
            varHostname = lblHostname.Text = coletando;
            varMac = lblMac.Text = coletando;
            varIP = lblIP.Text = coletando;
            varBIOS = lblBIOS.Text = coletando;
            varBIOSType = lblBIOSType.Text = coletando;

            varBoard = lblBM.Text = HardwareInfo.GetBoardMaker();
            varModel = lblModel.Text = HardwareInfo.GetModel();
            varSerial = lblSerialNo.Text = HardwareInfo.GetBoardProductId();
            varProc = lblProcName.Text = HardwareInfo.GetProcessorCores();
            varRAM = lblPM.Text = HardwareInfo.GetPhysicalMemory() + " (" + HardwareInfo.GetNumFreeRamSlots(Convert.ToInt32(HardwareInfo.GetNumRamSlots())) +
                " slots de " + HardwareInfo.GetNumRamSlots() + " ocupados" + ")";
            varHD = lblHDSize.Text = HardwareInfo.GetHDSize();
            varHDType = lblMediaType.Text = HardwareInfo.GetStorageType();
            varGPUInfo = lblGPUInfo.Text = HardwareInfo.GetGPUInfo();
            varOS = lblOS.Text = HardwareInfo.GetOSInformation();
            varHostname = lblHostname.Text = HardwareInfo.GetComputerName();
            varMac = lblMac.Text = HardwareInfo.GetMACAddress();
            varIP = lblIP.Text = HardwareInfo.GetIPAddress();
            varBIOS = lblBIOS.Text = HardwareInfo.GetComputerBIOS();
            varBIOSType = lblBIOSType.Text = HardwareInfo.GetBIOSType();
        }

        public static bool PingHost(string servidor_web)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            if (servidor_web == "")
                return false;
            try
            {
                PingReply reply = pinger.Send(servidor_web);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {

            }
            return pingable;
        }

        private void cadastra_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox3.Text) && comboBox6.SelectedItem != null)
            {
                varPatrimonio = textBox1.Text;
                varLacre = textBox2.Text;
                if (textBox4.Text != "")
                    varSala = textBox3.Text + " - " + textBox4.Text;
                else
                    varSala = textBox3.Text;
                varPredio = comboBox1.SelectedItem.ToString();
                varCadastrado = comboBox2.SelectedItem.ToString();
                varPadrao = comboBox3.SelectedItem.ToString();
                varUso = comboBox4.SelectedItem.ToString();
                varTag = comboBox5.SelectedItem.ToString();
                varTipo = comboBox6.SelectedItem.ToString();
                varCalend = monthCalendar1.SelectionRange.Start.ToString();
                servidor_web = comboBox7.Text;
                porta = comboBox8.Text;

                if (PingHost(servidor_web) == true && porta != "")
                    webBrowser1.Navigate("http://" + servidor_web + ":" + porta + "/recebeDados.php?patrimonio=" + varPatrimonio + "&lacre=" + varLacre +
                 "&sala=" + varSala + "&predio=" + varPredio + "&ad=" + varCadastrado + "&padrao=" + varPadrao + "&formatacao=" + varCalend + "&formatacoesAnteriores=" + varCalend +
                 "&marca=" + varBoard + "&modelo=" + varModel + "&numeroSerial=" + varSerial + "&processador=" + varProc + "&memoria=" + varRAM +
                 "&hd=" + varHD + "&sistemaOperacional=" + varOS + "&nomeDoComputador=" + varHostname + "&bios=" + varBIOS + "&mac=" + varMac + "&ip=" + varIP + "&emUso=" + varUso +
                 "&etiqueta=" + varTag + "&tipo=" + varTipo + "&tipoFW=" + varBIOSType + "&tipoArmaz=" + varHDType + "&gpu=" + varGPUInfo);
                else
                    MessageBox.Show("Servidor não encontrado. Selecione um servidor válido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Preencha os campos 'Patrimônio', 'Sala' e 'Tipo'", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
