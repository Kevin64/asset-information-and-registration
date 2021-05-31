﻿using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace HardwareInformation
{
    public partial class Form1 : Form
    {
        private BackgroundWorker backgroundWorker1;
        public Form1()
        {
            InitializeComponent();
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker1_ProgressChanged);
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            //Change this for alpha, beta and final releases - use alpha, beta and blank respectively
            this.toolStripStatusLabel2.Text = version();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
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
            this.label17 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.cadastraButton = new System.Windows.Forms.Button();
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
            this.coletaButton = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.lblBIOS = new System.Windows.Forms.Label();
            this.accessSystemButton = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.lblBIOSType = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label28 = new System.Windows.Forms.Label();
            this.lblSecBoot = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.lblMediaOperation = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.lblGPUInfo = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.lblMediaType = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.formatButton1 = new System.Windows.Forms.RadioButton();
            this.maintenanceButton2 = new System.Windows.Forms.RadioButton();
            this.webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.comboBoxTheme = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.timer4 = new System.Windows.Forms.Timer(this.components);
            this.timer5 = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer6 = new System.Windows.Forms.Timer(this.components);
            this.label26 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBM
            // 
            this.lblBM.AutoSize = true;
            this.lblBM.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblBM.Location = new System.Drawing.Point(370, 48);
            this.lblBM.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblBM.Name = "lblBM";
            this.lblBM.Size = new System.Drawing.Size(24, 25);
            this.lblBM.TabIndex = 7;
            this.lblBM.Text = "a";
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblModel.Location = new System.Drawing.Point(370, 96);
            this.lblModel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(24, 25);
            this.lblModel.TabIndex = 8;
            this.lblModel.Text = "b";
            // 
            // lblSerialNo
            // 
            this.lblSerialNo.AutoSize = true;
            this.lblSerialNo.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblSerialNo.Location = new System.Drawing.Point(370, 144);
            this.lblSerialNo.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblSerialNo.Name = "lblSerialNo";
            this.lblSerialNo.Size = new System.Drawing.Size(23, 25);
            this.lblSerialNo.TabIndex = 9;
            this.lblSerialNo.Text = "c";
            // 
            // lblProcName
            // 
            this.lblProcName.AutoSize = true;
            this.lblProcName.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblProcName.Location = new System.Drawing.Point(370, 192);
            this.lblProcName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblProcName.Name = "lblProcName";
            this.lblProcName.Size = new System.Drawing.Size(24, 25);
            this.lblProcName.TabIndex = 10;
            this.lblProcName.Text = "d";
            // 
            // lblPM
            // 
            this.lblPM.AutoSize = true;
            this.lblPM.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblPM.Location = new System.Drawing.Point(370, 240);
            this.lblPM.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPM.Name = "lblPM";
            this.lblPM.Size = new System.Drawing.Size(24, 25);
            this.lblPM.TabIndex = 11;
            this.lblPM.Text = "e";
            // 
            // lblHDSize
            // 
            this.lblHDSize.AutoSize = true;
            this.lblHDSize.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblHDSize.Location = new System.Drawing.Point(370, 288);
            this.lblHDSize.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblHDSize.Name = "lblHDSize";
            this.lblHDSize.Size = new System.Drawing.Size(18, 25);
            this.lblHDSize.TabIndex = 12;
            this.lblHDSize.Text = "f";
            // 
            // lblOS
            // 
            this.lblOS.AutoSize = true;
            this.lblOS.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblOS.Location = new System.Drawing.Point(370, 481);
            this.lblOS.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblOS.Name = "lblOS";
            this.lblOS.Size = new System.Drawing.Size(17, 25);
            this.lblOS.TabIndex = 13;
            this.lblOS.Text = "j";
            // 
            // lblHostname
            // 
            this.lblHostname.AutoSize = true;
            this.lblHostname.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblHostname.Location = new System.Drawing.Point(370, 529);
            this.lblHostname.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblHostname.Name = "lblHostname";
            this.lblHostname.Size = new System.Drawing.Size(23, 25);
            this.lblHostname.TabIndex = 15;
            this.lblHostname.Text = "k";
            // 
            // lblMac
            // 
            this.lblMac.AutoSize = true;
            this.lblMac.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMac.Location = new System.Drawing.Point(370, 577);
            this.lblMac.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMac.Name = "lblMac";
            this.lblMac.Size = new System.Drawing.Size(17, 25);
            this.lblMac.TabIndex = 18;
            this.lblMac.Text = "l";
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblIP.Location = new System.Drawing.Point(370, 623);
            this.lblIP.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(29, 25);
            this.lblIP.TabIndex = 19;
            this.lblIP.Text = "m";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(50, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Marca:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(50, 96);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Modelo:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(50, 144);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "Número Serial:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(50, 192);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(294, 25);
            this.label4.TabIndex = 3;
            this.label4.Text = "Processador e nº de núcleos:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(50, 240);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(279, 25);
            this.label5.TabIndex = 4;
            this.label5.Text = "Memória RAM e nº de slots:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label6.Location = new System.Drawing.Point(50, 288);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(311, 25);
            this.label6.TabIndex = 5;
            this.label6.Text = "Armazenamento (espaço total):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label7.Location = new System.Drawing.Point(50, 481);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(217, 25);
            this.label7.TabIndex = 6;
            this.label7.Text = "Sistema Operacional:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label8.Location = new System.Drawing.Point(50, 529);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(227, 25);
            this.label8.TabIndex = 7;
            this.label8.Text = "Nome do Computador:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label9.Location = new System.Drawing.Point(50, 577);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(234, 25);
            this.label9.TabIndex = 8;
            this.label9.Text = "Endereço MAC do NIC:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label10.Location = new System.Drawing.Point(50, 623);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(206, 25);
            this.label10.TabIndex = 9;
            this.label10.Text = "Endereço IP do NIC:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label11.Location = new System.Drawing.Point(40, 48);
            this.label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(120, 25);
            this.label11.TabIndex = 10;
            this.label11.Text = "Patrimônio:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label12.Location = new System.Drawing.Point(40, 96);
            this.label12.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(187, 25);
            this.label12.TabIndex = 11;
            this.label12.Text = "Lacre (se houver):";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label13.Location = new System.Drawing.Point(40, 192);
            this.label13.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(80, 25);
            this.label13.TabIndex = 13;
            this.label13.Text = "Prédio:";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox1.Location = new System.Drawing.Point(328, 42);
            this.textBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBox1.MaxLength = 6;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(450, 31);
            this.textBox1.TabIndex = 34;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox2.Location = new System.Drawing.Point(328, 90);
            this.textBox2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBox2.MaxLength = 10;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(450, 31);
            this.textBox2.TabIndex = 35;
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox3.Location = new System.Drawing.Point(328, 138);
            this.textBox3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBox3.MaxLength = 4;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(180, 31);
            this.textBox3.TabIndex = 36;
            this.textBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox4.Location = new System.Drawing.Point(716, 138);
            this.textBox4.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBox4.MaxLength = 1;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(62, 31);
            this.textBox4.TabIndex = 37;
            this.textBox4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox4_KeyPress);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label14.Location = new System.Drawing.Point(40, 144);
            this.label14.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(272, 25);
            this.label14.TabIndex = 12;
            this.label14.Text = "Sala (0000 se não houver):";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label15.Location = new System.Drawing.Point(40, 240);
            this.label15.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(277, 25);
            this.label15.TabIndex = 14;
            this.label15.Text = "Cadastrado no servidor AD:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label16.Location = new System.Drawing.Point(40, 337);
            this.label16.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(191, 25);
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
            this.comboBox1.Location = new System.Drawing.Point(328, 187);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(180, 33);
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
            this.comboBox2.Location = new System.Drawing.Point(328, 235);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(180, 33);
            this.comboBox2.Sorted = true;
            this.comboBox2.TabIndex = 39;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label17.Location = new System.Drawing.Point(40, 288);
            this.label17.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(87, 25);
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
            this.comboBox3.Location = new System.Drawing.Point(328, 283);
            this.comboBox3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(180, 33);
            this.comboBox3.TabIndex = 40;
            // 
            // cadastraButton
            // 
            this.cadastraButton.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.cadastraButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cadastraButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cadastraButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cadastraButton.Location = new System.Drawing.Point(1494, 1056);
            this.cadastraButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cadastraButton.Name = "cadastraButton";
            this.cadastraButton.Size = new System.Drawing.Size(484, 104);
            this.cadastraButton.TabIndex = 50;
            this.cadastraButton.Text = "Cadastrar / Atualizar dados";
            this.cadastraButton.UseVisualStyleBackColor = false;
            this.cadastraButton.Click += new System.EventHandler(this.cadastra_ClickAsync);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label18.Location = new System.Drawing.Point(524, 192);
            this.label18.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(90, 25);
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
            this.comboBox4.Location = new System.Drawing.Point(626, 187);
            this.comboBox4.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(152, 33);
            this.comboBox4.TabIndex = 41;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label19.Location = new System.Drawing.Point(524, 240);
            this.label19.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(97, 25);
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
            this.comboBox5.Location = new System.Drawing.Point(626, 235);
            this.comboBox5.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(152, 33);
            this.comboBox5.TabIndex = 42;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label20.Location = new System.Drawing.Point(524, 288);
            this.label20.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(60, 25);
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
            this.comboBox6.Location = new System.Drawing.Point(626, 283);
            this.comboBox6.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.comboBox6.Name = "comboBox6";
            this.comboBox6.Size = new System.Drawing.Size(152, 33);
            this.comboBox6.TabIndex = 43;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label21.Location = new System.Drawing.Point(40, 665);
            this.label21.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(98, 25);
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
            this.comboBox7.Location = new System.Drawing.Point(328, 660);
            this.comboBox7.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.comboBox7.Name = "comboBox7";
            this.comboBox7.Size = new System.Drawing.Size(236, 33);
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
            this.comboBox8.Location = new System.Drawing.Point(662, 660);
            this.comboBox8.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.comboBox8.Name = "comboBox8";
            this.comboBox8.Size = new System.Drawing.Size(116, 33);
            this.comboBox8.TabIndex = 46;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label22.Location = new System.Drawing.Point(580, 665);
            this.label22.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(69, 25);
            this.label22.TabIndex = 18;
            this.label22.Text = "Porta:";
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.monthCalendar1.Location = new System.Drawing.Point(328, 331);
            this.monthCalendar1.Margin = new System.Windows.Forms.Padding(18, 17, 18, 17);
            this.monthCalendar1.MaxSelectionCount = 1;
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 44;
            this.monthCalendar1.TabStop = false;
            // 
            // coletaButton
            // 
            this.coletaButton.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.coletaButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.coletaButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.coletaButton.Location = new System.Drawing.Point(1150, 1056);
            this.coletaButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.coletaButton.Name = "coletaButton";
            this.coletaButton.Size = new System.Drawing.Size(332, 44);
            this.coletaButton.TabIndex = 49;
            this.coletaButton.Text = "Coletar Novamente";
            this.coletaButton.UseVisualStyleBackColor = false;
            this.coletaButton.Click += new System.EventHandler(this.coleta_Click);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label23.Location = new System.Drawing.Point(524, 144);
            this.label23.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(182, 25);
            this.label23.TabIndex = 55;
            this.label23.Text = "Letra (se houver):";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label24.Location = new System.Drawing.Point(50, 719);
            this.label24.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(224, 25);
            this.label24.TabIndex = 56;
            this.label24.Text = "Versão da BIOS/UEFI:";
            // 
            // lblBIOS
            // 
            this.lblBIOS.AutoSize = true;
            this.lblBIOS.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblBIOS.Location = new System.Drawing.Point(370, 719);
            this.lblBIOS.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblBIOS.Name = "lblBIOS";
            this.lblBIOS.Size = new System.Drawing.Size(24, 25);
            this.lblBIOS.TabIndex = 57;
            this.lblBIOS.Text = "o";
            // 
            // accessSystemButton
            // 
            this.accessSystemButton.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.accessSystemButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.accessSystemButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.accessSystemButton.Location = new System.Drawing.Point(1150, 1112);
            this.accessSystemButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.accessSystemButton.Name = "accessSystemButton";
            this.accessSystemButton.Size = new System.Drawing.Size(332, 48);
            this.accessSystemButton.TabIndex = 51;
            this.accessSystemButton.Text = "Acessar sistema de patrimônios";
            this.accessSystemButton.UseVisualStyleBackColor = false;
            this.accessSystemButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label25.Location = new System.Drawing.Point(50, 671);
            this.label25.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(177, 25);
            this.label25.TabIndex = 62;
            this.label25.Text = "Tipo de firmware:";
            // 
            // lblBIOSType
            // 
            this.lblBIOSType.AutoSize = true;
            this.lblBIOSType.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblBIOSType.Location = new System.Drawing.Point(370, 671);
            this.lblBIOSType.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblBIOSType.Name = "lblBIOSType";
            this.lblBIOSType.Size = new System.Drawing.Size(24, 25);
            this.lblBIOSType.TabIndex = 63;
            this.lblBIOSType.Text = "n";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label28);
            this.groupBox1.Controls.Add(this.lblSecBoot);
            this.groupBox1.Controls.Add(this.label32);
            this.groupBox1.Controls.Add(this.lblMediaOperation);
            this.groupBox1.Controls.Add(this.label30);
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
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox1.Location = new System.Drawing.Point(64, 217);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Size = new System.Drawing.Size(1074, 942);
            this.groupBox1.TabIndex = 65;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dados do computador";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.BackColor = System.Drawing.Color.Transparent;
            this.label28.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label28.Location = new System.Drawing.Point(510, 823);
            this.label28.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(24, 25);
            this.label28.TabIndex = 70;
            this.label28.Text = "q";
            // 
            // lblSecBoot
            // 
            this.lblSecBoot.AutoSize = true;
            this.lblSecBoot.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblSecBoot.Location = new System.Drawing.Point(370, 767);
            this.lblSecBoot.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblSecBoot.Name = "lblSecBoot";
            this.lblSecBoot.Size = new System.Drawing.Size(24, 25);
            this.lblSecBoot.TabIndex = 71;
            this.lblSecBoot.Text = "p";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label32.Location = new System.Drawing.Point(50, 767);
            this.label32.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(136, 25);
            this.label32.TabIndex = 70;
            this.label32.Text = "Secure Boot:";
            // 
            // lblMediaOperation
            // 
            this.lblMediaOperation.AutoSize = true;
            this.lblMediaOperation.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMediaOperation.Location = new System.Drawing.Point(370, 385);
            this.lblMediaOperation.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMediaOperation.Name = "lblMediaOperation";
            this.lblMediaOperation.Size = new System.Drawing.Size(24, 25);
            this.lblMediaOperation.TabIndex = 69;
            this.lblMediaOperation.Text = "h";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label30.Location = new System.Drawing.Point(50, 385);
            this.label30.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(301, 25);
            this.label30.TabIndex = 68;
            this.label30.Text = "Modo de operação SATA/M.2:";
            // 
            // lblGPUInfo
            // 
            this.lblGPUInfo.AutoSize = true;
            this.lblGPUInfo.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblGPUInfo.Location = new System.Drawing.Point(370, 433);
            this.lblGPUInfo.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblGPUInfo.Name = "lblGPUInfo";
            this.lblGPUInfo.Size = new System.Drawing.Size(17, 25);
            this.lblGPUInfo.TabIndex = 67;
            this.lblGPUInfo.Text = "i";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label29.Location = new System.Drawing.Point(50, 433);
            this.label29.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(163, 25);
            this.label29.TabIndex = 66;
            this.label29.Text = "Placa de Vídeo:";
            // 
            // lblMediaType
            // 
            this.lblMediaType.AutoSize = true;
            this.lblMediaType.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMediaType.Location = new System.Drawing.Point(370, 337);
            this.lblMediaType.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMediaType.Name = "lblMediaType";
            this.lblMediaType.Size = new System.Drawing.Size(24, 25);
            this.lblMediaType.TabIndex = 65;
            this.lblMediaType.Text = "g";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label27.Location = new System.Drawing.Point(50, 337);
            this.label27.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(250, 25);
            this.label27.TabIndex = 64;
            this.label27.Text = "Tipo de armazenamento:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 854);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1050, 69);
            this.progressBar1.TabIndex = 69;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label26);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.webView2);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.comboBox8);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.comboBox7);
            this.groupBox2.Controls.Add(this.comboBox1);
            this.groupBox2.Controls.Add(this.label21);
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
            this.groupBox2.Location = new System.Drawing.Point(1150, 217);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox2.Size = new System.Drawing.Size(828, 829);
            this.groupBox2.TabIndex = 66;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Dados do patrimônio, manutenção e de localização";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox5);
            this.groupBox3.Controls.Add(this.textBox6);
            this.groupBox3.Controls.Add(this.formatButton1);
            this.groupBox3.Controls.Add(this.maintenanceButton2);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox3.Location = new System.Drawing.Point(12, 367);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox3.Size = new System.Drawing.Size(302, 281);
            this.groupBox3.TabIndex = 72;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tipo de serviço";
            // 
            // textBox5
            // 
            this.textBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox5.Location = new System.Drawing.Point(36, 65);
            this.textBox5.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(246, 77);
            this.textBox5.TabIndex = 76;
            this.textBox5.Text = "Opção para quando o PC passar por manutenção com formatação";
            // 
            // textBox6
            // 
            this.textBox6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox6.Location = new System.Drawing.Point(34, 185);
            this.textBox6.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBox6.Multiline = true;
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(252, 79);
            this.textBox6.TabIndex = 77;
            this.textBox6.Text = "Opção para quando o PC passar por manutenção sem formatação";
            // 
            // formatButton1
            // 
            this.formatButton1.AutoSize = true;
            this.formatButton1.Checked = true;
            this.formatButton1.Location = new System.Drawing.Point(36, 37);
            this.formatButton1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.formatButton1.Name = "formatButton1";
            this.formatButton1.Size = new System.Drawing.Size(157, 29);
            this.formatButton1.TabIndex = 73;
            this.formatButton1.TabStop = true;
            this.formatButton1.Text = "Formatação";
            this.formatButton1.UseVisualStyleBackColor = true;
            this.formatButton1.CheckedChanged += new System.EventHandler(this.formatButton1_CheckedChanged);
            // 
            // maintenanceButton2
            // 
            this.maintenanceButton2.AutoSize = true;
            this.maintenanceButton2.Location = new System.Drawing.Point(36, 154);
            this.maintenanceButton2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.maintenanceButton2.Name = "maintenanceButton2";
            this.maintenanceButton2.Size = new System.Drawing.Size(162, 29);
            this.maintenanceButton2.TabIndex = 74;
            this.maintenanceButton2.Text = "Manutenção";
            this.maintenanceButton2.UseVisualStyleBackColor = true;
            this.maintenanceButton2.CheckedChanged += new System.EventHandler(this.maintenanceButton2_CheckedChanged);
            // 
            // webView2
            // 
            this.webView2.CreationProperties = null;
            this.webView2.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView2.Location = new System.Drawing.Point(12, 712);
            this.webView2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.webView2.Name = "webView2";
            this.webView2.Size = new System.Drawing.Size(804, 100);
            this.webView2.TabIndex = 72;
            this.webView2.ZoomFactor = 1D;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)));
            this.toolStripStatusLabel2.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripStatusLabel2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(4, 36);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comboBoxTheme,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 1164);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 28, 0);
            this.statusStrip1.Size = new System.Drawing.Size(2046, 46);
            this.statusStrip1.TabIndex = 60;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // comboBoxTheme
            // 
            this.comboBoxTheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.comboBoxTheme.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3});
            this.comboBoxTheme.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxTheme.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.comboBoxTheme.Name = "comboBoxTheme";
            this.comboBoxTheme.Size = new System.Drawing.Size(94, 42);
            this.comboBoxTheme.Text = "Tema";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripMenuItem1.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
            this.toolStripMenuItem1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(472, 44);
            this.toolStripMenuItem1.Text = "Automático (Tema do sistema)";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
            this.toolStripMenuItem2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(472, 44);
            this.toolStripMenuItem2.Text = "Claro";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
            this.toolStripMenuItem3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(472, 44);
            this.toolStripMenuItem3.Text = "Escuro";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Top | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(1918, 36);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "Sistema desenvolvido pelo servidor Kevin Costa, SIAPE 1971957, para uso no serviç" +
    "o da Unidade de Tecnologia da Informação do CCSH - UFSM";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Image = global::HardwareInformation.Properties.Resources.banner_dark;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(-14, -4);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(2060, 210);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 64;
            this.pictureBox1.TabStop = false;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.BackColor = System.Drawing.Color.Transparent;
            this.label26.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label26.Location = new System.Drawing.Point(136, 665);
            this.label26.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(19, 25);
            this.label26.TabIndex = 72;
            this.label26.Text = "r";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(2046, 1210);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.accessSystemButton);
            this.Controls.Add(this.coletaButton);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.cadastraButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Coleta de hardware e cadastro de patrimônio / Unidade de Tecnologia da Informação" +
    " do CCSH - UFSM";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        //Variables being declared
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
        private Label lblOS;
        private Label label17;
        private ComboBox comboBox3;
        private Label label18;
        private ComboBox comboBox4;
        private Label label19;
        private ComboBox comboBox5;
        private Button cadastraButton;
        private Label label20;
        private ComboBox comboBox6;
        private ComboBox comboBox7;
        private Label label21;
        private bool themeBool;
        private string servidor_web, porta;
        private string varPatrimonio, varLacre, varSala, varBoard, varModel,
           varSerial, varProc, varRAM, varHD, varHDType, varHDOperation, varGPUInfo,
           varOS, varHostname, varMac, varIP, varPredio, varCadastrado, varPadrao,
           varCalend, varUso, varTag, varTipo, varBIOS, varBIOSType, varSecBoot;
        private string BM, Model, SerialNo, ProcName, PM, HDSize, MediaType,
           MediaOperation, GPUInfo, OS, Hostname, Mac, IP, BIOS, BIOSType, SecBoot;
        private int i = 0;
        private Label label25;
        private Label lblBIOSType;
        private PictureBox pictureBox1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label lblMediaType;
        private Label label27;
        private Label lblGPUInfo;
        private Label label29;
        private Timer timer1, timer2, timer3, timer4, timer5, timer6;
        private IContainer components;
        private Label lblMediaOperation;
        private Label label30;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private string coletando = "Coletando...";
        private string cadastrando = "Cadastrando, aguarde...";
        private string coletarNovamente = "Coletar Novamente";
        private string cadastrarNovamente = "Cadastrar / Atualizar dados";
        private bool pass = true, mode = true;
        private ComboBox comboBox8;
        private MonthCalendar monthCalendar1;
        private Button coletaButton;
        private Label label23;
        private Label label24;
        private Label lblBIOS;
        private Button accessSystemButton;
        private ProgressBar progressBar1;
        private Label label28;
        private Label lblSecBoot;
        private Label label32;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2;
        private RadioButton maintenanceButton2;
        private RadioButton formatButton1;
        private GroupBox groupBox3;
        private TextBox textBox5;
        private TextBox textBox6;
        private ToolStripDropDownButton comboBoxTheme;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private Label label26;
        private Label label22;

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

        //Sets service mode to format
        private void formatButton1_CheckedChanged(object sender, EventArgs e)
        {
            mode = true;
        }

        //Sets service mode to maintenance
        private void maintenanceButton2_CheckedChanged(object sender, EventArgs e)
        {
            mode = false;
        }

        //Method for setting the auto theme
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
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
                            themeBool = true;
                        }
                        else
                        {
                            lightTheme();
                            themeBool = false;
                        }
                    }
                    else
                    {
                        lightTheme();
                        themeBool = false;
                    }
                }
            }
            catch (Exception ex)
            {
                lightTheme();
                themeBool = false;
            }
        }

        //Method for setting the light theme
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            lightTheme();
            themeBool = false;
        }

        //Method for setting the dark theme
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            darkTheme();
            themeBool = true;
        }

        //Initializes the application theme
        private void comboBoxThemeInit()
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
                            themeBool = true;
                        }
                        else
                        {
                            lightTheme();
                            themeBool = false;
                        }
                    }
                    else
                    {
                        lightTheme();
                        themeBool = false;
                    }
                }
            }
            catch (Exception ex)
            {
                lightTheme();
                themeBool = false;
            }
        }

        //Sets a light theme for the UI
        private void lightTheme()
        {
            this.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lblBM.ForeColor = SystemColors.ControlText;
            this.lblModel.ForeColor = SystemColors.ControlText;
            this.lblSerialNo.ForeColor = SystemColors.ControlText;
            this.lblProcName.ForeColor = SystemColors.ControlText;
            this.lblPM.ForeColor = SystemColors.ControlText;
            this.lblHDSize.ForeColor = SystemColors.ControlText;
            this.lblMediaType.ForeColor = SystemColors.ControlText;
            this.lblMediaOperation.ForeColor = SystemColors.ControlText;
            this.lblOS.ForeColor = SystemColors.ControlText;
            this.lblHostname.ForeColor = SystemColors.ControlText;
            this.lblMac.ForeColor = SystemColors.ControlText;
            this.lblIP.ForeColor = SystemColors.ControlText;
            this.label1.ForeColor = SystemColors.ControlText;
            this.label2.ForeColor = SystemColors.ControlText;
            this.label3.ForeColor = SystemColors.ControlText;
            this.label4.ForeColor = SystemColors.ControlText;
            this.label5.ForeColor = SystemColors.ControlText;
            this.label6.ForeColor = SystemColors.ControlText;
            this.label7.ForeColor = SystemColors.ControlText;
            this.label8.ForeColor = SystemColors.ControlText;
            this.label9.ForeColor = SystemColors.ControlText;
            this.label10.ForeColor = SystemColors.ControlText;
            this.label11.ForeColor = SystemColors.ControlText;
            this.label12.ForeColor = SystemColors.ControlText;
            this.label13.ForeColor = SystemColors.ControlText;
            this.textBox1.BackColor = SystemColors.Control;
            this.textBox1.ForeColor = SystemColors.ControlText;
            this.textBox2.BackColor = SystemColors.Control;
            this.textBox2.ForeColor = SystemColors.ControlText;
            this.textBox3.BackColor = SystemColors.Control;
            this.textBox3.ForeColor = SystemColors.ControlText;
            this.textBox4.BackColor = SystemColors.Control;
            this.textBox4.ForeColor = SystemColors.ControlText;
            this.label14.ForeColor = SystemColors.ControlText;
            this.label15.ForeColor = SystemColors.ControlText;
            this.label16.ForeColor = SystemColors.ControlText;
            this.comboBox1.BackColor = SystemColors.ControlLight;
            this.comboBox1.ForeColor = SystemColors.ControlText;
            this.comboBox2.BackColor = SystemColors.ControlLight;
            this.comboBox2.ForeColor = SystemColors.ControlText;
            this.label17.ForeColor = SystemColors.ControlText;
            this.comboBox3.BackColor = SystemColors.ControlLight;
            this.comboBox3.ForeColor = SystemColors.ControlText;
            this.cadastraButton.BackColor = SystemColors.ControlLight;
            this.cadastraButton.ForeColor = SystemColors.ControlText;
            this.label18.ForeColor = SystemColors.ControlText;
            this.comboBox4.BackColor = SystemColors.ControlLight;
            this.comboBox4.ForeColor = SystemColors.ControlText;
            this.label19.ForeColor = SystemColors.ControlText;
            this.comboBox5.BackColor = SystemColors.ControlLight;
            this.comboBox5.ForeColor = SystemColors.ControlText;
            this.label20.ForeColor = SystemColors.ControlText;
            this.comboBox6.BackColor = SystemColors.ControlLight;
            this.comboBox6.ForeColor = SystemColors.ControlText;
            this.label21.ForeColor = SystemColors.ControlText;
            this.comboBox7.BackColor = SystemColors.ControlLight;
            this.comboBox7.ForeColor = SystemColors.ControlText;
            this.comboBox8.BackColor = SystemColors.ControlLight;
            this.comboBox8.ForeColor = SystemColors.ControlText;
            this.label22.ForeColor = SystemColors.ControlText;
            this.monthCalendar1.BackColor = SystemColors.ControlLight;
            this.coletaButton.BackColor = SystemColors.ControlLight;
            this.coletaButton.ForeColor = SystemColors.ControlText;
            this.label23.ForeColor = SystemColors.ControlText;
            this.label24.ForeColor = SystemColors.ControlText;
            this.lblBIOS.ForeColor = SystemColors.ControlText;
            this.accessSystemButton.BackColor = SystemColors.ControlLight;
            this.accessSystemButton.ForeColor = SystemColors.ControlText;
            this.label25.ForeColor = SystemColors.ControlText;
            this.lblBIOSType.ForeColor = SystemColors.ControlText;
            this.groupBox1.ForeColor = SystemColors.ControlText;
            this.groupBox2.ForeColor = SystemColors.ControlText;
            this.toolStripStatusLabel2.ForeColor = SystemColors.ControlText;
            this.statusStrip1.BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.toolStripStatusLabel1.ForeColor = SystemColors.ControlText;
            this.comboBoxTheme.BackColor = this.BackColor;
            this.comboBoxTheme.ForeColor = SystemColors.ControlText;
            this.label27.ForeColor = SystemColors.ControlText;
            this.label29.ForeColor = SystemColors.ControlText;
            this.label28.ForeColor = SystemColors.ControlText;
            this.label30.ForeColor = SystemColors.ControlText;
            this.lblSecBoot.ForeColor = SystemColors.ControlText;
            this.label32.ForeColor = SystemColors.ControlText;
            this.lblGPUInfo.ForeColor = SystemColors.ControlText;
            this.groupBox3.ForeColor = SystemColors.ControlText;
            this.textBox5.ForeColor = SystemColors.ControlText;
            this.textBox5.BackColor = this.BackColor;
            this.textBox6.ForeColor = SystemColors.ControlText;
            this.textBox6.BackColor = this.BackColor;            
            this.toolStripMenuItem1.BackgroundImage = null;
            this.toolStripMenuItem1.ForeColor = SystemColors.ControlText;
            this.toolStripMenuItem2.BackgroundImage = null;
            this.toolStripMenuItem2.ForeColor = SystemColors.ControlText;
            this.toolStripMenuItem3.BackgroundImage = null;
            this.toolStripMenuItem3.ForeColor = SystemColors.ControlText;
            this.pictureBox1.Image = global::HardwareInformation.Properties.Resources.banner_light;
        }

        //Sets a dark theme for the UI
        private void darkTheme()
        {
            this.BackColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblBM.ForeColor = SystemColors.ControlLightLight;
            this.lblModel.ForeColor = SystemColors.ControlLightLight;
            this.lblSerialNo.ForeColor = SystemColors.ControlLightLight;
            this.lblProcName.ForeColor = SystemColors.ControlLightLight;
            this.lblPM.ForeColor = SystemColors.ControlLightLight;
            this.lblHDSize.ForeColor = SystemColors.ControlLightLight;
            this.lblMediaType.ForeColor = SystemColors.ControlLightLight;
            this.lblMediaOperation.ForeColor = SystemColors.ControlLightLight;
            this.lblOS.ForeColor = SystemColors.ControlLightLight;
            this.lblHostname.ForeColor = SystemColors.ControlLightLight;
            this.lblMac.ForeColor = SystemColors.ControlLightLight;
            this.lblIP.ForeColor = SystemColors.ControlLightLight;
            this.label1.ForeColor = SystemColors.ControlLightLight;
            this.label2.ForeColor = SystemColors.ControlLightLight;
            this.label3.ForeColor = SystemColors.ControlLightLight;
            this.label4.ForeColor = SystemColors.ControlLightLight;
            this.label5.ForeColor = SystemColors.ControlLightLight;
            this.label6.ForeColor = SystemColors.ControlLightLight;
            this.label7.ForeColor = SystemColors.ControlLightLight;
            this.label8.ForeColor = SystemColors.ControlLightLight;
            this.label9.ForeColor = SystemColors.ControlLightLight;
            this.label10.ForeColor = SystemColors.ControlLightLight;
            this.label11.ForeColor = SystemColors.ControlLightLight;
            this.label12.ForeColor = SystemColors.ControlLightLight;
            this.label13.ForeColor = SystemColors.ControlLightLight;
            this.textBox1.BackColor = SystemColors.WindowFrame;
            this.textBox1.ForeColor = SystemColors.ControlLightLight;
            this.textBox2.BackColor = SystemColors.WindowFrame;
            this.textBox2.ForeColor = SystemColors.ControlLightLight;
            this.textBox3.BackColor = SystemColors.WindowFrame;
            this.textBox3.ForeColor = SystemColors.ControlLightLight;
            this.textBox4.BackColor = SystemColors.WindowFrame;
            this.textBox4.ForeColor = SystemColors.ControlLightLight;
            this.label14.ForeColor = SystemColors.ControlLightLight;
            this.label15.ForeColor = SystemColors.ControlLightLight;
            this.label16.ForeColor = SystemColors.ControlLightLight;
            this.comboBox1.BackColor = SystemColors.WindowFrame;
            this.comboBox1.ForeColor = SystemColors.ControlLightLight;
            this.comboBox2.BackColor = SystemColors.WindowFrame;
            this.comboBox2.ForeColor = SystemColors.ControlLightLight;
            this.label17.ForeColor = SystemColors.ControlLightLight;
            this.comboBox3.BackColor = SystemColors.WindowFrame;
            this.comboBox3.ForeColor = SystemColors.ControlLightLight;
            this.cadastraButton.BackColor = SystemColors.WindowFrame;
            this.cadastraButton.ForeColor = SystemColors.ControlLightLight;
            this.label18.ForeColor = SystemColors.ControlLightLight;
            this.comboBox4.BackColor = SystemColors.WindowFrame;
            this.comboBox4.ForeColor = SystemColors.ControlLightLight;
            this.label19.ForeColor = SystemColors.ControlLightLight;
            this.comboBox5.BackColor = SystemColors.WindowFrame;
            this.comboBox5.ForeColor = SystemColors.ControlLightLight;
            this.label20.ForeColor = SystemColors.ControlLightLight;
            this.comboBox6.BackColor = SystemColors.WindowFrame;
            this.comboBox6.ForeColor = SystemColors.ControlLightLight;
            this.label21.ForeColor = SystemColors.ControlLightLight;
            this.comboBox7.BackColor = SystemColors.WindowFrame;
            this.comboBox7.ForeColor = SystemColors.ControlLightLight;
            this.comboBox8.BackColor = SystemColors.WindowFrame;
            this.comboBox8.ForeColor = SystemColors.ControlLightLight;
            this.label22.ForeColor = SystemColors.ControlLightLight;
            this.monthCalendar1.BackColor = SystemColors.WindowFrame;
            this.coletaButton.BackColor = SystemColors.WindowFrame;
            this.coletaButton.ForeColor = SystemColors.ControlLightLight;
            this.label23.ForeColor = SystemColors.ControlLightLight;
            this.label24.ForeColor = SystemColors.ControlLightLight;
            this.lblBIOS.ForeColor = SystemColors.ControlLightLight;
            this.accessSystemButton.BackColor = SystemColors.WindowFrame;
            this.accessSystemButton.ForeColor = SystemColors.ControlLightLight;
            this.label25.ForeColor = SystemColors.ControlLightLight;
            this.lblBIOSType.ForeColor = SystemColors.ControlLightLight;
            this.groupBox1.ForeColor = SystemColors.ControlLightLight;
            this.groupBox2.ForeColor = SystemColors.ControlLightLight;
            this.toolStripStatusLabel2.ForeColor = SystemColors.ControlLightLight;
            this.statusStrip1.BackColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripStatusLabel1.ForeColor = SystemColors.ControlLightLight;
            this.comboBoxTheme.BackColor = this.BackColor;
            this.comboBoxTheme.ForeColor = SystemColors.ControlLightLight;
            this.label27.ForeColor = SystemColors.ControlLightLight;
            this.label28.ForeColor = SystemColors.ControlLightLight;
            this.label29.ForeColor = SystemColors.ControlLightLight;
            this.label30.ForeColor = SystemColors.ControlLightLight;
            this.lblSecBoot.ForeColor = SystemColors.ControlLightLight;
            this.label32.ForeColor = SystemColors.ControlLightLight;
            this.lblGPUInfo.ForeColor = SystemColors.ControlLightLight;
            this.groupBox3.ForeColor = SystemColors.ControlLightLight;
            this.textBox5.ForeColor = SystemColors.ControlLightLight;
            this.textBox5.BackColor = this.BackColor;
            this.textBox6.ForeColor = SystemColors.ControlLightLight;
            this.textBox6.BackColor = this.BackColor;
            this.toolStripMenuItem1.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
            this.toolStripMenuItem1.ForeColor = SystemColors.ControlLightLight;
            this.toolStripMenuItem2.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
            this.toolStripMenuItem2.ForeColor = SystemColors.ControlLightLight;
            this.toolStripMenuItem3.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
            this.toolStripMenuItem3.ForeColor = SystemColors.ControlLightLight;
            this.pictureBox1.Image = global::HardwareInformation.Properties.Resources.banner_dark;
        }

        //Opens the selected webpage, according to the IP and port specified in the comboboxes
        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(comboBox7.Text) && !string.IsNullOrWhiteSpace(comboBox8.Text))
                System.Diagnostics.Process.Start("http://" + comboBox7.Text + ":" + comboBox8.Text);
            else
                MessageBox.Show("Para acessar, selecione o servidor e a porta!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        //Handles the closing of the current form
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        //Loads the form, sets some combobox values, create two timers (500 ms cadence), and triggers a hardware collection
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Tick += new EventHandler(flashTextHostname);
            timer2.Tick += new EventHandler(flashTextMediaOp);
            timer3.Tick += new EventHandler(flashTextSecBoot);
            timer4.Tick += new EventHandler(flashTextBIOSVersion);
            timer5.Tick += new EventHandler(flashTextNetConnectivity);
            timer6.Tick += new EventHandler(flashTextBIOSType);
            timer1.Interval = 500;
            timer2.Interval = 500;
            timer3.Interval = 500;
            timer4.Interval = 500;
            timer5.Interval = 500;
            timer6.Interval = 500;
            comboBox1.SelectedIndex = 4;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 1;
            comboBox4.SelectedIndex = 1;
            comboBox5.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;
            comboBoxThemeInit();
            this.FormClosing += Form1_FormClosing;
            coleta_Click(sender, e);
        }

        //Restricts textbox4 only with chars
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        //Restricts textbox1 only with numbers
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        //Sets the Hostname label to flash in red
        private void flashTextHostname(Object myObject, EventArgs myEventArgs)
        {
            if (lblHostname.ForeColor == Color.Red && themeBool == true)
                lblHostname.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            else if(lblHostname.ForeColor == Color.Red && themeBool == false)
                lblHostname.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            else
                lblHostname.ForeColor = Color.Red;
        }

        //Sets the MediaOperations label to flash in red
        private void flashTextMediaOp(Object myobject, EventArgs myEventArgs)
        {
            if (lblMediaOperation.ForeColor == Color.Red && themeBool == true)
                lblMediaOperation.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            else if (lblMediaOperation.ForeColor == Color.Red && themeBool == false)
                lblMediaOperation.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            else
                lblMediaOperation.ForeColor = Color.Red;
        }

        //Sets the Secure Boot label to flash in red
        private void flashTextSecBoot(Object myobject, EventArgs myEventArgs)
        {
            if (lblSecBoot.ForeColor == Color.Red && themeBool == true)
                lblSecBoot.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            else if (lblSecBoot.ForeColor == Color.Red && themeBool == false)
                lblSecBoot.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            else
                lblSecBoot.ForeColor = Color.Red;
        }

        //Sets the BIOS Version label to flash in red
        private void flashTextBIOSVersion(Object myobject, EventArgs myEventArgs)
        {
            if (lblBIOS.ForeColor == Color.Red && themeBool == true)
                lblBIOS.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            else if (lblBIOS.ForeColor == Color.Red && themeBool == false)
                lblBIOS.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            else
                lblBIOS.ForeColor = Color.Red;
        }

        private void flashTextNetConnectivity(Object myobject, EventArgs myEventArgs)
        {
            if (lblMac.ForeColor == Color.Red && themeBool == true)
            {
                lblMac.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                lblIP.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            }
            else if (lblMac.ForeColor == Color.Red && themeBool == false)
            {
                lblMac.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                lblIP.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            }
            else
            {
                lblMac.ForeColor = Color.Red;
                lblIP.ForeColor = Color.Red;
            }
        }
        
        private void flashTextBIOSType(Object myobject, EventArgs myEventArgs)
        {
            if (lblBIOSType.ForeColor == Color.Red && themeBool == true)
                lblBIOSType.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            else if (lblBIOSType.ForeColor == Color.Red && themeBool == false)
                lblBIOSType.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            else
                lblBIOSType.ForeColor = Color.Red;
        }

        //Starts the collection process
        private void collecting()
        {
            servidor_web = comboBox7.Text;
            porta = comboBox8.Text;
            if (PingHost(servidor_web) == true && porta != "")
            {
                label26.Text = "(Online)";
                label26.ForeColor = Color.Lime;
            }
            else
            {
                label26.Text = "(Offline)";
                label26.ForeColor = Color.Red;
            }               

            //Stops blinking and resets red color
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
            timer4.Enabled = false;
            timer5.Enabled = false;
            timer6.Enabled = false;
            if (lblHostname.ForeColor == Color.Red && themeBool == true)
            {
                lblHostname.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                lblMediaOperation.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                lblSecBoot.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                lblBIOS.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            }
            else if (lblHostname.ForeColor == Color.Red && themeBool == false)
            {
                lblHostname.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                lblMediaOperation.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                lblSecBoot.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                lblBIOS.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            }

            //Sets current and maximum values for the progressbar
            progressBar1.Maximum = 16;
            progressBar1.Value = 0;

            //Writes 'coletando...' in the labels, while collects data
            lblBM.Text = coletando;
            lblModel.Text = coletando;
            lblSerialNo.Text = coletando;
            lblProcName.Text = coletando;
            lblPM.Text = coletando;
            lblHDSize.Text = coletando;
            lblMediaType.Text = coletando;
            lblMediaOperation.Text = coletando;
            lblGPUInfo.Text = coletando;
            lblOS.Text = coletando;
            lblHostname.Text = coletando;
            lblMac.Text = coletando;
            lblIP.Text = coletando;
            lblBIOS.Text = coletando;
            lblBIOSType.Text = coletando;
            lblSecBoot.Text = coletando;
            coletaButton.Text = coletando;
        }       
        
        //Auxiliary method
        private int progressAuxFunction(int k)
        {
            return (k * 100) / progressBar1.Maximum;
        }

        //Runs on a separate thread, calling methods from the HardwareInfo class, and setting the variables,
        // while reporting the progress to the progressbar
        private void collectThread(BackgroundWorker worker, DoWorkEventArgs e)
        {            
            i = 0;

            BM = HardwareInfo.GetBoardMaker();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            Model = HardwareInfo.GetModel();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            SerialNo = HardwareInfo.GetBoardProductId();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            ProcName = HardwareInfo.GetProcessorCores();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            PM = HardwareInfo.GetPhysicalMemory() + " (" + HardwareInfo.GetNumFreeRamSlots(Convert.ToInt32(HardwareInfo.GetNumRamSlots())) +
                " slots de " + HardwareInfo.GetNumRamSlots() + " ocupados" + ")";
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            HDSize = HardwareInfo.GetHDSize();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            MediaType = HardwareInfo.GetStorageType();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            MediaOperation = HardwareInfo.GetStorageOperation();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            GPUInfo = HardwareInfo.GetGPUInfo();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            OS = HardwareInfo.GetOSInformation();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            Hostname = HardwareInfo.GetComputerName();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            Mac = HardwareInfo.GetMACAddress();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            IP = HardwareInfo.GetIPAddress();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            BIOSType = HardwareInfo.GetBIOSType();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            SecBoot = HardwareInfo.GetSecureBoot();
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            i++;
            BIOS = HardwareInfo.GetComputerBIOS();
            worker.ReportProgress(progressAuxFunction(i));
        }

        //Prints the collected data into the form labels, warning the user when the hostname and/or MediaOp string are forbidden
        private void printHardwareData()
        {
            pass = true;
            lblBM.Text = BM;
            lblModel.Text = Model;
            lblSerialNo.Text = SerialNo;
            lblProcName.Text = ProcName;
            lblPM.Text = PM;
            lblHDSize.Text = HDSize;
            lblMediaType.Text = MediaType;
            lblMediaOperation.Text = MediaOperation;
            lblGPUInfo.Text = GPUInfo;
            lblOS.Text = OS;
            lblHostname.Text = Hostname;
            lblMac.Text = Mac;
            lblIP.Text = IP;
            lblBIOS.Text = BIOS;
            lblBIOSType.Text = BIOSType;
            lblSecBoot.Text = SecBoot;
            string[] str = BIOSFileReader.fetchInfo(lblBM.Text, lblModel.Text, lblBIOSType.Text, comboBox7.Text, comboBox8.Text);
            if (lblHostname.Text.Equals("MUDAR-NOME"))
            {
                pass = false;
                lblHostname.Text += " (Nome incorreto, alterar)";
                timer1.Enabled = true;
            }
            if (!lblModel.Text.Contains("7057") && !lblModel.Text.Contains("8814") && !lblModel.Text.Contains("6078") && lblMediaOperation.Text.Equals("IDE/Legacy"))
            {
                pass = false;
                lblMediaOperation.Text += " (Modo de operação incorreto, alterar)";
                timer2.Enabled = true;
            }
            if (lblSecBoot.Text.Equals("Desativado") && !lblGPUInfo.Text.Contains("210") && !lblGPUInfo.Text.Contains("430"))
            {
                pass = false;
                lblSecBoot.Text += " (Ativar boot seguro)";
                timer3.Enabled = true;
            }
            if(str == null)
            {
                pass = false;
                MessageBox.Show("Erro ao contatar o banco de dados, verifique a sua conexão com a intranet e se o servidor web está ativo!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (str != null && !lblBIOS.Text.Contains(str[0]))
            {
                pass = false;
                lblBIOS.Text += " (Atualizar BIOS/UEFI)";
                timer4.Enabled = true;
            }
            if (str != null && str[1].Equals("false"))
            {
                pass = false;
                lblBIOSType.Text += " (PC suporta UEFI, fazer a conversão do sistema)";
                timer6.Enabled = true;
            }
            if (lblMac.Text == "")
            {
                pass = false;
                lblMac.Text = "Computador sem conexão com a Intranet";
                lblIP.Text = "Computador sem conexão com a Intranet";
                timer5.Enabled = true;
            }
        }
        
        //Triggers when the form opens, and when the user clicks to collect
        private void coleta_Click(object sender, EventArgs e)
        {
            collecting();
            accessSystemButton.Enabled = false;
            cadastraButton.Enabled = false;
            coletaButton.Enabled = false;
            startAsync(sender, e);                
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
            collectThread(worker, e);
        }

        //Draws the collection progress on the screen
        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = (e.ProgressPercentage * progressBar1.Maximum) / 100;
            label28.Text = (e.ProgressPercentage.ToString() + "%");
        }

        //Runs when the collection ends, ending the thread
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            
            accessSystemButton.Enabled = true;
            cadastraButton.Enabled = true;
            coletaButton.Enabled = true;
            coletaButton.Text = coletarNovamente;
            printHardwareData();
        }

        //Pings the IP:port selected to know if it's reachable
        private static bool PingHost(string servidor_web)
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

        //Attributes the data collected previously to the variables which will inside the URL for registration
        private void attrHardwareData()
        {
            varBoard = lblBM.Text;
            varModel = lblModel.Text;
            varSerial = lblSerialNo.Text;
            varProc = lblProcName.Text;
            varRAM = lblPM.Text;
            varHD = lblHDSize.Text;
            varHDType = lblMediaType.Text;
            varHDOperation = lblMediaOperation.Text;
            varGPUInfo = lblGPUInfo.Text;
            varOS = lblOS.Text;
            varHostname = lblHostname.Text;
            varMac = lblMac.Text;
            varIP = lblIP.Text;
            varBIOS = lblBIOS.Text;
            varBIOSType = lblBIOSType.Text;
            varSecBoot = lblSecBoot.Text;
        }

        //Runs the registration for the website
        private async void cadastra_ClickAsync(object sender, EventArgs e)
        {
            cadastraButton.Text = cadastrando;
            cadastraButton.Enabled = false;
            accessSystemButton.Enabled = false;
            coletaButton.Enabled = false;
            attrHardwareData();
            if (!string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox3.Text) && comboBox6.SelectedItem != null && pass == true)
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

                var webView2Environment = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync("webview2.runtime", System.IO.Path.GetTempPath());
                await webView2.EnsureCoreWebView2Async(webView2Environment);

                if (PingHost(servidor_web) == true && porta != "")
                {
                    if (mode)
                        webView2.CoreWebView2.Navigate("http://" + servidor_web + ":" + porta + "/recebeDadosFormatacao.php?patrimonio=" + varPatrimonio + "&lacre=" + varLacre +
                     "&sala=" + varSala + "&predio=" + varPredio + "&ad=" + varCadastrado + "&padrao=" + varPadrao + "&formatacao=" + varCalend + "&formatacoesAnteriores=" + varCalend +
                     "&marca=" + varBoard + "&modelo=" + varModel + "&numeroSerial=" + varSerial + "&processador=" + varProc + "&memoria=" + varRAM +
                     "&hd=" + varHD + "&sistemaOperacional=" + varOS + "&nomeDoComputador=" + varHostname + "&bios=" + varBIOS + "&mac=" + varMac + "&ip=" + varIP + "&emUso=" + varUso +
                     "&etiqueta=" + varTag + "&tipo=" + varTipo + "&tipoFW=" + varBIOSType + "&tipoArmaz=" + varHDType + "&gpu=" + varGPUInfo + "&modoArmaz=" + varHDOperation +
                     "&secBoot=" + varSecBoot);
                    else
                        webView2.CoreWebView2.Navigate("http://" + servidor_web + ":" + porta + "/recebeDadosManutencao.php?patrimonio=" + varPatrimonio + "&lacre=" + varLacre +
                     "&sala=" + varSala + "&predio=" + varPredio + "&ad=" + varCadastrado + "&padrao=" + varPadrao + "&formatacao=" + varCalend + "&formatacoesAnteriores=" + varCalend +
                     "&marca=" + varBoard + "&modelo=" + varModel + "&numeroSerial=" + varSerial + "&processador=" + varProc + "&memoria=" + varRAM +
                     "&hd=" + varHD + "&nomeDoComputador=" + varHostname + "&bios=" + varBIOS + "&mac=" + varMac + "&ip=" + varIP + "&emUso=" + varUso +
                     "&etiqueta=" + varTag + "&tipo=" + varTipo + "&tipoFW=" + varBIOSType + "&tipoArmaz=" + varHDType + "&gpu=" + varGPUInfo + "&modoArmaz=" + varHDOperation +
                     "&secBoot=" + varSecBoot);
                }
                else
                    MessageBox.Show("Servidor não encontrado. Selecione um servidor válido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(!pass)
                MessageBox.Show("Resolva as pendencias exibidas em vermelho!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("Preencha os campos 'Patrimônio', 'Sala' e 'Tipo'", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            cadastraButton.Text = cadastrarNovamente;
            cadastraButton.Enabled = true;
            accessSystemButton.Enabled = true;
            coletaButton.Enabled = true;
        }
    }
}
