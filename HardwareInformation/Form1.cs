using Microsoft.Win32;
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
			this.coletaButton = new System.Windows.Forms.Button();
			this.label23 = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.lblBIOS = new System.Windows.Forms.Label();
			this.accessSystemButton = new System.Windows.Forms.Button();
			this.label25 = new System.Windows.Forms.Label();
			this.lblBIOSType = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblVT = new System.Windows.Forms.Label();
			this.label33 = new System.Windows.Forms.Label();
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
			this.label42 = new System.Windows.Forms.Label();
			this.label41 = new System.Windows.Forms.Label();
			this.label40 = new System.Windows.Forms.Label();
			this.label39 = new System.Windows.Forms.Label();
			this.label38 = new System.Windows.Forms.Label();
			this.label37 = new System.Windows.Forms.Label();
			this.label36 = new System.Windows.Forms.Label();
			this.label35 = new System.Windows.Forms.Label();
			this.studentButton2 = new System.Windows.Forms.RadioButton();
			this.employeeButton1 = new System.Windows.Forms.RadioButton();
			this.label34 = new System.Windows.Forms.Label();
			this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
			this.label26 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label43 = new System.Windows.Forms.Label();
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
			this.timer6 = new System.Windows.Forms.Timer(this.components);
			this.timer7 = new System.Windows.Forms.Timer(this.components);
			this.configurableQualityPictureBox31 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox25 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox29 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox28 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox27 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox26 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox24 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox23 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox22 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox21 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox20 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox19 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox18 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox30 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox2 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox17 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox16 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox15 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox14 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox13 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox12 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox11 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox10 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox9 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox8 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox7 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox6 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox5 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox4 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox3 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.configurableQualityPictureBox1 = new HardwareInformation.ConfigurableQualityPictureBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.webView2)).BeginInit();
			this.statusStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox31)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox25)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox29)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox28)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox27)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox26)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox24)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox23)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox22)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox21)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox20)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox19)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox18)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox30)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox17)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox16)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox15)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox14)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox13)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox12)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox11)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// lblBM
			// 
			this.lblBM.AutoSize = true;
			this.lblBM.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblBM.Location = new System.Drawing.Point(197, 25);
			this.lblBM.Name = "lblBM";
			this.lblBM.Size = new System.Drawing.Size(13, 13);
			this.lblBM.TabIndex = 7;
			this.lblBM.Text = "a";
			// 
			// lblModel
			// 
			this.lblModel.AutoSize = true;
			this.lblModel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblModel.Location = new System.Drawing.Point(197, 52);
			this.lblModel.Name = "lblModel";
			this.lblModel.Size = new System.Drawing.Size(13, 13);
			this.lblModel.TabIndex = 8;
			this.lblModel.Text = "b";
			// 
			// lblSerialNo
			// 
			this.lblSerialNo.AutoSize = true;
			this.lblSerialNo.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblSerialNo.Location = new System.Drawing.Point(197, 79);
			this.lblSerialNo.Name = "lblSerialNo";
			this.lblSerialNo.Size = new System.Drawing.Size(13, 13);
			this.lblSerialNo.TabIndex = 9;
			this.lblSerialNo.Text = "c";
			// 
			// lblProcName
			// 
			this.lblProcName.AutoSize = true;
			this.lblProcName.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblProcName.Location = new System.Drawing.Point(197, 106);
			this.lblProcName.Name = "lblProcName";
			this.lblProcName.Size = new System.Drawing.Size(13, 13);
			this.lblProcName.TabIndex = 10;
			this.lblProcName.Text = "d";
			// 
			// lblPM
			// 
			this.lblPM.AutoSize = true;
			this.lblPM.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblPM.Location = new System.Drawing.Point(197, 133);
			this.lblPM.Name = "lblPM";
			this.lblPM.Size = new System.Drawing.Size(13, 13);
			this.lblPM.TabIndex = 11;
			this.lblPM.Text = "e";
			// 
			// lblHDSize
			// 
			this.lblHDSize.AutoSize = true;
			this.lblHDSize.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblHDSize.Location = new System.Drawing.Point(197, 160);
			this.lblHDSize.Name = "lblHDSize";
			this.lblHDSize.Size = new System.Drawing.Size(10, 13);
			this.lblHDSize.TabIndex = 12;
			this.lblHDSize.Text = "f";
			// 
			// lblOS
			// 
			this.lblOS.AutoSize = true;
			this.lblOS.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblOS.Location = new System.Drawing.Point(197, 268);
			this.lblOS.Name = "lblOS";
			this.lblOS.Size = new System.Drawing.Size(9, 13);
			this.lblOS.TabIndex = 13;
			this.lblOS.Text = "j";
			// 
			// lblHostname
			// 
			this.lblHostname.AutoSize = true;
			this.lblHostname.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblHostname.Location = new System.Drawing.Point(197, 295);
			this.lblHostname.Name = "lblHostname";
			this.lblHostname.Size = new System.Drawing.Size(13, 13);
			this.lblHostname.TabIndex = 15;
			this.lblHostname.Text = "k";
			// 
			// lblMac
			// 
			this.lblMac.AutoSize = true;
			this.lblMac.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblMac.Location = new System.Drawing.Point(197, 322);
			this.lblMac.Name = "lblMac";
			this.lblMac.Size = new System.Drawing.Size(9, 13);
			this.lblMac.TabIndex = 18;
			this.lblMac.Text = "l";
			// 
			// lblIP
			// 
			this.lblIP.AutoSize = true;
			this.lblIP.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblIP.Location = new System.Drawing.Point(197, 349);
			this.lblIP.Name = "lblIP";
			this.lblIP.Size = new System.Drawing.Size(15, 13);
			this.lblIP.TabIndex = 19;
			this.lblIP.Text = "m";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label1.Location = new System.Drawing.Point(37, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Marca:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label2.Location = new System.Drawing.Point(37, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(45, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Modelo:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label3.Location = new System.Drawing.Point(37, 79);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(76, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Número Serial:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label4.Location = new System.Drawing.Point(37, 106);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(146, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Processador e nº de núcleos:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label5.Location = new System.Drawing.Point(37, 133);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(138, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "Memória RAM e nº de slots:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label6.Location = new System.Drawing.Point(37, 160);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(153, 13);
			this.label6.TabIndex = 5;
			this.label6.Text = "Armazenamento (espaço total):";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label7.Location = new System.Drawing.Point(37, 268);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(107, 13);
			this.label7.TabIndex = 6;
			this.label7.Text = "Sistema Operacional:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label8.Location = new System.Drawing.Point(37, 295);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(113, 13);
			this.label8.TabIndex = 7;
			this.label8.Text = "Nome do Computador:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label9.Location = new System.Drawing.Point(37, 322);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(118, 13);
			this.label9.TabIndex = 8;
			this.label9.Text = "Endereço MAC do NIC:";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label10.Location = new System.Drawing.Point(37, 349);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(105, 13);
			this.label10.TabIndex = 9;
			this.label10.Text = "Endereço IP do NIC:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label11.Location = new System.Drawing.Point(35, 25);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(59, 13);
			this.label11.TabIndex = 10;
			this.label11.Text = "Patrimônio:";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label12.Location = new System.Drawing.Point(35, 52);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(93, 13);
			this.label12.TabIndex = 11;
			this.label12.Text = "Lacre (se houver):";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label13.Location = new System.Drawing.Point(35, 106);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(40, 13);
			this.label13.TabIndex = 13;
			this.label13.Text = "Prédio:";
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.textBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.textBox1.Location = new System.Drawing.Point(192, 22);
			this.textBox1.MaxLength = 6;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(235, 20);
			this.textBox1.TabIndex = 34;
			this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
			// 
			// textBox2
			// 
			this.textBox2.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.textBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.textBox2.Location = new System.Drawing.Point(192, 49);
			this.textBox2.MaxLength = 10;
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(235, 20);
			this.textBox2.TabIndex = 35;
			this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
			// 
			// textBox3
			// 
			this.textBox3.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.textBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.textBox3.Location = new System.Drawing.Point(192, 76);
			this.textBox3.MaxLength = 4;
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(77, 20);
			this.textBox3.TabIndex = 36;
			this.textBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
			// 
			// textBox4
			// 
			this.textBox4.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.textBox4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.textBox4.Location = new System.Drawing.Point(402, 76);
			this.textBox4.MaxLength = 1;
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(25, 20);
			this.textBox4.TabIndex = 37;
			this.textBox4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox4_KeyPress);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label14.Location = new System.Drawing.Point(35, 79);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(135, 13);
			this.label14.TabIndex = 12;
			this.label14.Text = "Sala (0000 se não houver):";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label15.Location = new System.Drawing.Point(35, 236);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(137, 13);
			this.label15.TabIndex = 14;
			this.label15.Text = "Cadastrado no servidor AD:";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label16.Location = new System.Drawing.Point(35, 160);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(115, 13);
			this.label16.TabIndex = 16;
			this.label16.Text = "Data do último serviço:";
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
			this.comboBox1.Location = new System.Drawing.Point(192, 103);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(77, 21);
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
			this.comboBox2.Location = new System.Drawing.Point(192, 233);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(77, 21);
			this.comboBox2.Sorted = true;
			this.comboBox2.TabIndex = 39;
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label17.Location = new System.Drawing.Point(303, 236);
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
			this.comboBox3.Location = new System.Drawing.Point(350, 233);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new System.Drawing.Size(77, 21);
			this.comboBox3.TabIndex = 40;
			// 
			// cadastraButton
			// 
			this.cadastraButton.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.cadastraButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cadastraButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cadastraButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.cadastraButton.Location = new System.Drawing.Point(767, 591);
			this.cadastraButton.Name = "cadastraButton";
			this.cadastraButton.Size = new System.Drawing.Size(258, 56);
			this.cadastraButton.TabIndex = 50;
			this.cadastraButton.Text = "Cadastrar / Atualizar dados";
			this.cadastraButton.UseVisualStyleBackColor = false;
			this.cadastraButton.Click += new System.EventHandler(this.cadastra_ClickAsync);
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label18.Location = new System.Drawing.Point(306, 106);
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
			this.comboBox4.Location = new System.Drawing.Point(367, 103);
			this.comboBox4.Name = "comboBox4";
			this.comboBox4.Size = new System.Drawing.Size(60, 21);
			this.comboBox4.TabIndex = 41;
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label19.Location = new System.Drawing.Point(306, 133);
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
			this.comboBox5.Location = new System.Drawing.Point(367, 130);
			this.comboBox5.Name = "comboBox5";
			this.comboBox5.Size = new System.Drawing.Size(60, 21);
			this.comboBox5.TabIndex = 42;
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label20.Location = new System.Drawing.Point(35, 133);
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
			this.comboBox6.Location = new System.Drawing.Point(192, 130);
			this.comboBox6.Name = "comboBox6";
			this.comboBox6.Size = new System.Drawing.Size(77, 21);
			this.comboBox6.TabIndex = 43;
			// 
			// label21
			// 
			this.label21.AutoSize = true;
			this.label21.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label21.Location = new System.Drawing.Point(35, 393);
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
			this.comboBox7.Location = new System.Drawing.Point(192, 390);
			this.comboBox7.Name = "comboBox7";
			this.comboBox7.Size = new System.Drawing.Size(108, 21);
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
			this.comboBox8.Location = new System.Drawing.Point(350, 390);
			this.comboBox8.Name = "comboBox8";
			this.comboBox8.Size = new System.Drawing.Size(77, 21);
			this.comboBox8.TabIndex = 46;
			// 
			// label22
			// 
			this.label22.AutoSize = true;
			this.label22.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label22.Location = new System.Drawing.Point(306, 393);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(35, 13);
			this.label22.TabIndex = 18;
			this.label22.Text = "Porta:";
			// 
			// coletaButton
			// 
			this.coletaButton.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.coletaButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.coletaButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.coletaButton.Location = new System.Drawing.Point(575, 591);
			this.coletaButton.Name = "coletaButton";
			this.coletaButton.Size = new System.Drawing.Size(186, 25);
			this.coletaButton.TabIndex = 49;
			this.coletaButton.Text = "Coletar Novamente";
			this.coletaButton.UseVisualStyleBackColor = false;
			this.coletaButton.Click += new System.EventHandler(this.coleta_Click);
			// 
			// label23
			// 
			this.label23.AutoSize = true;
			this.label23.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label23.Location = new System.Drawing.Point(306, 79);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(90, 13);
			this.label23.TabIndex = 55;
			this.label23.Text = "Letra (se houver):";
			// 
			// label24
			// 
			this.label24.AutoSize = true;
			this.label24.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label24.Location = new System.Drawing.Point(37, 403);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(115, 13);
			this.label24.TabIndex = 56;
			this.label24.Text = "Versão da BIOS/UEFI:";
			// 
			// lblBIOS
			// 
			this.lblBIOS.AutoSize = true;
			this.lblBIOS.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblBIOS.Location = new System.Drawing.Point(197, 403);
			this.lblBIOS.Name = "lblBIOS";
			this.lblBIOS.Size = new System.Drawing.Size(13, 13);
			this.lblBIOS.TabIndex = 57;
			this.lblBIOS.Text = "o";
			// 
			// accessSystemButton
			// 
			this.accessSystemButton.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.accessSystemButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.accessSystemButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.accessSystemButton.Location = new System.Drawing.Point(575, 622);
			this.accessSystemButton.Name = "accessSystemButton";
			this.accessSystemButton.Size = new System.Drawing.Size(186, 25);
			this.accessSystemButton.TabIndex = 51;
			this.accessSystemButton.Text = "Acessar sistema de patrimônios";
			this.accessSystemButton.UseVisualStyleBackColor = false;
			this.accessSystemButton.Click += new System.EventHandler(this.button1_Click);
			// 
			// label25
			// 
			this.label25.AutoSize = true;
			this.label25.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label25.Location = new System.Drawing.Point(37, 376);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(88, 13);
			this.label25.TabIndex = 62;
			this.label25.Text = "Tipo de firmware:";
			// 
			// lblBIOSType
			// 
			this.lblBIOSType.AutoSize = true;
			this.lblBIOSType.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblBIOSType.Location = new System.Drawing.Point(197, 376);
			this.lblBIOSType.Name = "lblBIOSType";
			this.lblBIOSType.Size = new System.Drawing.Size(13, 13);
			this.lblBIOSType.TabIndex = 63;
			this.lblBIOSType.Text = "n";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox30);
			this.groupBox1.Controls.Add(this.lblVT);
			this.groupBox1.Controls.Add(this.label33);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox2);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox17);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox16);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox15);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox14);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox13);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox12);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox11);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox10);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox9);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox8);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox7);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox6);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox5);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox4);
			this.groupBox1.Controls.Add(this.configurableQualityPictureBox3);
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
			this.groupBox1.Location = new System.Drawing.Point(32, 113);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(537, 534);
			this.groupBox1.TabIndex = 65;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Dados do computador";
			// 
			// lblVT
			// 
			this.lblVT.AutoSize = true;
			this.lblVT.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblVT.Location = new System.Drawing.Point(197, 457);
			this.lblVT.Name = "lblVT";
			this.lblVT.Size = new System.Drawing.Size(13, 13);
			this.lblVT.TabIndex = 103;
			this.lblVT.Text = "q";
			// 
			// label33
			// 
			this.label33.AutoSize = true;
			this.label33.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label33.Location = new System.Drawing.Point(37, 457);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(141, 13);
			this.label33.TabIndex = 102;
			this.label33.Text = "Tecnologia de Virtualização:";
			// 
			// label28
			// 
			this.label28.AutoSize = true;
			this.label28.BackColor = System.Drawing.Color.Transparent;
			this.label28.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label28.Location = new System.Drawing.Point(256, 474);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(10, 13);
			this.label28.TabIndex = 70;
			this.label28.Text = "r";
			// 
			// lblSecBoot
			// 
			this.lblSecBoot.AutoSize = true;
			this.lblSecBoot.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblSecBoot.Location = new System.Drawing.Point(197, 430);
			this.lblSecBoot.Name = "lblSecBoot";
			this.lblSecBoot.Size = new System.Drawing.Size(13, 13);
			this.lblSecBoot.TabIndex = 71;
			this.lblSecBoot.Text = "p";
			// 
			// label32
			// 
			this.label32.AutoSize = true;
			this.label32.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label32.Location = new System.Drawing.Point(37, 430);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(69, 13);
			this.label32.TabIndex = 70;
			this.label32.Text = "Secure Boot:";
			// 
			// lblMediaOperation
			// 
			this.lblMediaOperation.AutoSize = true;
			this.lblMediaOperation.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblMediaOperation.Location = new System.Drawing.Point(197, 214);
			this.lblMediaOperation.Name = "lblMediaOperation";
			this.lblMediaOperation.Size = new System.Drawing.Size(13, 13);
			this.lblMediaOperation.TabIndex = 69;
			this.lblMediaOperation.Text = "h";
			// 
			// label30
			// 
			this.label30.AutoSize = true;
			this.label30.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label30.Location = new System.Drawing.Point(37, 214);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(154, 13);
			this.label30.TabIndex = 68;
			this.label30.Text = "Modo de operação SATA/M.2:";
			// 
			// lblGPUInfo
			// 
			this.lblGPUInfo.AutoSize = true;
			this.lblGPUInfo.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblGPUInfo.Location = new System.Drawing.Point(197, 241);
			this.lblGPUInfo.Name = "lblGPUInfo";
			this.lblGPUInfo.Size = new System.Drawing.Size(9, 13);
			this.lblGPUInfo.TabIndex = 67;
			this.lblGPUInfo.Text = "i";
			// 
			// label29
			// 
			this.label29.AutoSize = true;
			this.label29.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label29.Location = new System.Drawing.Point(37, 241);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(84, 13);
			this.label29.TabIndex = 66;
			this.label29.Text = "Placa de Vídeo:";
			// 
			// lblMediaType
			// 
			this.lblMediaType.AutoSize = true;
			this.lblMediaType.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblMediaType.Location = new System.Drawing.Point(197, 187);
			this.lblMediaType.Name = "lblMediaType";
			this.lblMediaType.Size = new System.Drawing.Size(13, 13);
			this.lblMediaType.TabIndex = 65;
			this.lblMediaType.Text = "g";
			// 
			// label27
			// 
			this.label27.AutoSize = true;
			this.label27.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label27.Location = new System.Drawing.Point(37, 187);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(124, 13);
			this.label27.TabIndex = 64;
			this.label27.Text = "Tipo de armazenamento:";
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(7, 490);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(525, 36);
			this.progressBar1.TabIndex = 69;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label42);
			this.groupBox2.Controls.Add(this.label41);
			this.groupBox2.Controls.Add(this.label40);
			this.groupBox2.Controls.Add(this.label39);
			this.groupBox2.Controls.Add(this.label38);
			this.groupBox2.Controls.Add(this.label37);
			this.groupBox2.Controls.Add(this.label36);
			this.groupBox2.Controls.Add(this.label35);
			this.groupBox2.Controls.Add(this.studentButton2);
			this.groupBox2.Controls.Add(this.employeeButton1);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox31);
			this.groupBox2.Controls.Add(this.label34);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox25);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox29);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox28);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox27);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox26);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox24);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox23);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox22);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox21);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox20);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox19);
			this.groupBox2.Controls.Add(this.configurableQualityPictureBox18);
			this.groupBox2.Controls.Add(this.dateTimePicker1);
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
			this.groupBox2.Size = new System.Drawing.Size(450, 474);
			this.groupBox2.TabIndex = 66;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Dados do patrimônio, manutenção e de localização";
			// 
			// label42
			// 
			this.label42.AutoSize = true;
			this.label42.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label42.Location = new System.Drawing.Point(103, 195);
			this.label42.Name = "label42";
			this.label42.Size = new System.Drawing.Size(17, 13);
			this.label42.TabIndex = 112;
			this.label42.Text = "✱";
			// 
			// label41
			// 
			this.label41.AutoSize = true;
			this.label41.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label41.Location = new System.Drawing.Point(350, 133);
			this.label41.Name = "label41";
			this.label41.Size = new System.Drawing.Size(17, 13);
			this.label41.TabIndex = 111;
			this.label41.Text = "✱";
			// 
			// label40
			// 
			this.label40.AutoSize = true;
			this.label40.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label40.Location = new System.Drawing.Point(64, 133);
			this.label40.Name = "label40";
			this.label40.Size = new System.Drawing.Size(17, 13);
			this.label40.TabIndex = 110;
			this.label40.Text = "✱";
			// 
			// label39
			// 
			this.label39.AutoSize = true;
			this.label39.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label39.Location = new System.Drawing.Point(346, 106);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(17, 13);
			this.label39.TabIndex = 109;
			this.label39.Text = "✱";
			// 
			// label38
			// 
			this.label38.AutoSize = true;
			this.label38.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label38.Location = new System.Drawing.Point(73, 106);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(17, 13);
			this.label38.TabIndex = 108;
			this.label38.Text = "✱";
			// 
			// label37
			// 
			this.label37.AutoSize = true;
			this.label37.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label37.Location = new System.Drawing.Point(168, 79);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(17, 13);
			this.label37.TabIndex = 107;
			this.label37.Text = "✱";
			// 
			// label36
			// 
			this.label36.AutoSize = true;
			this.label36.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label36.Location = new System.Drawing.Point(92, 25);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(17, 13);
			this.label36.TabIndex = 106;
			this.label36.Text = "✱";
			// 
			// label35
			// 
			this.label35.AutoSize = true;
			this.label35.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label35.Location = new System.Drawing.Point(6, 268);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(152, 13);
			this.label35.TabIndex = 105;
			this.label35.Text = "✱ = Preenchimento obrigatório";
			// 
			// studentButton2
			// 
			this.studentButton2.AutoSize = true;
			this.studentButton2.Location = new System.Drawing.Point(192, 205);
			this.studentButton2.Name = "studentButton2";
			this.studentButton2.Size = new System.Drawing.Size(246, 17);
			this.studentButton2.TabIndex = 104;
			this.studentButton2.TabStop = true;
			this.studentButton2.Text = "Aluno (computador de laboratório/sala de aula)";
			this.studentButton2.UseVisualStyleBackColor = true;
			this.studentButton2.CheckedChanged += new System.EventHandler(this.studentButton2_CheckedChanged);
			// 
			// employeeButton1
			// 
			this.employeeButton1.AutoSize = true;
			this.employeeButton1.Location = new System.Drawing.Point(192, 187);
			this.employeeButton1.Name = "employeeButton1";
			this.employeeButton1.Size = new System.Drawing.Size(242, 17);
			this.employeeButton1.TabIndex = 103;
			this.employeeButton1.TabStop = true;
			this.employeeButton1.Text = "Funcionário/Bolsista (computador de trabalho)";
			this.employeeButton1.UseVisualStyleBackColor = true;
			this.employeeButton1.CheckedChanged += new System.EventHandler(this.employeeButton1_CheckedChanged);
			// 
			// label34
			// 
			this.label34.AutoSize = true;
			this.label34.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label34.Location = new System.Drawing.Point(35, 195);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(70, 13);
			this.label34.TabIndex = 101;
			this.label34.Text = "Quem usará?";
			// 
			// dateTimePicker1
			// 
			this.dateTimePicker1.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.dateTimePicker1.Location = new System.Drawing.Point(192, 157);
			this.dateTimePicker1.Name = "dateTimePicker1";
			this.dateTimePicker1.Size = new System.Drawing.Size(235, 20);
			this.dateTimePicker1.TabIndex = 73;
			// 
			// label26
			// 
			this.label26.AutoSize = true;
			this.label26.BackColor = System.Drawing.Color.Transparent;
			this.label26.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.label26.Location = new System.Drawing.Point(84, 393);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(12, 13);
			this.label26.TabIndex = 72;
			this.label26.Text = "s";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label43);
			this.groupBox3.Controls.Add(this.textBox5);
			this.groupBox3.Controls.Add(this.textBox6);
			this.groupBox3.Controls.Add(this.formatButton1);
			this.groupBox3.Controls.Add(this.maintenanceButton2);
			this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.groupBox3.Location = new System.Drawing.Point(192, 256);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(235, 128);
			this.groupBox3.TabIndex = 72;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Tipo de serviço";
			// 
			// label43
			// 
			this.label43.AutoSize = true;
			this.label43.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label43.Location = new System.Drawing.Point(82, 0);
			this.label43.Name = "label43";
			this.label43.Size = new System.Drawing.Size(17, 13);
			this.label43.TabIndex = 113;
			this.label43.Text = "✱";
			// 
			// textBox5
			// 
			this.textBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox5.Enabled = false;
			this.textBox5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.textBox5.Location = new System.Drawing.Point(18, 42);
			this.textBox5.Multiline = true;
			this.textBox5.Name = "textBox5";
			this.textBox5.ReadOnly = true;
			this.textBox5.Size = new System.Drawing.Size(81, 74);
			this.textBox5.TabIndex = 76;
			this.textBox5.Text = "Opção para quando o PC passar por manutenção com formatação";
			// 
			// textBox6
			// 
			this.textBox6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox6.Enabled = false;
			this.textBox6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.textBox6.Location = new System.Drawing.Point(121, 42);
			this.textBox6.Multiline = true;
			this.textBox6.Name = "textBox6";
			this.textBox6.ReadOnly = true;
			this.textBox6.Size = new System.Drawing.Size(92, 68);
			this.textBox6.TabIndex = 77;
			this.textBox6.Text = "Opção para quando o PC passar por manutenção sem formatação";
			// 
			// formatButton1
			// 
			this.formatButton1.AutoSize = true;
			this.formatButton1.Location = new System.Drawing.Point(18, 19);
			this.formatButton1.Name = "formatButton1";
			this.formatButton1.Size = new System.Drawing.Size(81, 17);
			this.formatButton1.TabIndex = 73;
			this.formatButton1.Text = "Formatação";
			this.formatButton1.UseVisualStyleBackColor = true;
			this.formatButton1.CheckedChanged += new System.EventHandler(this.formatButton1_CheckedChanged);
			// 
			// maintenanceButton2
			// 
			this.maintenanceButton2.AutoSize = true;
			this.maintenanceButton2.Location = new System.Drawing.Point(121, 19);
			this.maintenanceButton2.Name = "maintenanceButton2";
			this.maintenanceButton2.Size = new System.Drawing.Size(85, 17);
			this.maintenanceButton2.TabIndex = 74;
			this.maintenanceButton2.Text = "Manutenção";
			this.maintenanceButton2.UseVisualStyleBackColor = true;
			this.maintenanceButton2.CheckedChanged += new System.EventHandler(this.maintenanceButton2_CheckedChanged);
			// 
			// webView2
			// 
			this.webView2.CreationProperties = null;
			this.webView2.DefaultBackgroundColor = System.Drawing.Color.White;
			this.webView2.Location = new System.Drawing.Point(6, 416);
			this.webView2.Name = "webView2";
			this.webView2.Size = new System.Drawing.Size(438, 52);
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
			this.toolStripStatusLabel2.Size = new System.Drawing.Size(4, 19);
			// 
			// statusStrip1
			// 
			this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.comboBoxTheme,
			this.toolStripStatusLabel1,
			this.toolStripStatusLabel2});
			this.statusStrip1.Location = new System.Drawing.Point(0, 651);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(1058, 24);
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
			this.comboBoxTheme.Size = new System.Drawing.Size(48, 22);
			this.comboBoxTheme.Text = "Tema";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.BackColor = System.Drawing.SystemColors.Control;
			this.toolStripMenuItem1.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
			this.toolStripMenuItem1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(236, 22);
			this.toolStripMenuItem1.Text = "Automático (Tema do sistema)";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
			this.toolStripMenuItem2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(236, 22);
			this.toolStripMenuItem2.Text = "Claro";
			this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
			this.toolStripMenuItem3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(236, 22);
			this.toolStripMenuItem3.Text = "Escuro";
			this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)(((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
			| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
			this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(991, 19);
			this.toolStripStatusLabel1.Spring = true;
			this.toolStripStatusLabel1.Text = "Sistema desenvolvido pelo servidor Kevin Costa, SIAPE 1971957, para uso no serviç" +
	"o da Unidade de Tecnologia da Informação do CCSH - UFSM";
			// 
			// timer1
			// 
			this.timer1.Interval = 500;
			// 
			// configurableQualityPictureBox31
			// 
			this.configurableQualityPictureBox31.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox31.Image = global::HardwareInformation.Properties.Resources.who_white;
			this.configurableQualityPictureBox31.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox31.Location = new System.Drawing.Point(6, 187);
			this.configurableQualityPictureBox31.Name = "configurableQualityPictureBox31";
			this.configurableQualityPictureBox31.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox31.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox31.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox31.TabIndex = 102;
			this.configurableQualityPictureBox31.TabStop = false;
			// 
			// configurableQualityPictureBox25
			// 
			this.configurableQualityPictureBox25.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox25.Image = global::HardwareInformation.Properties.Resources.letter_white;
			this.configurableQualityPictureBox25.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox25.Location = new System.Drawing.Point(275, 71);
			this.configurableQualityPictureBox25.Name = "configurableQualityPictureBox25";
			this.configurableQualityPictureBox25.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox25.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox25.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox25.TabIndex = 100;
			this.configurableQualityPictureBox25.TabStop = false;
			// 
			// configurableQualityPictureBox29
			// 
			this.configurableQualityPictureBox29.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox29.Image = global::HardwareInformation.Properties.Resources.server_white;
			this.configurableQualityPictureBox29.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox29.Location = new System.Drawing.Point(6, 385);
			this.configurableQualityPictureBox29.Name = "configurableQualityPictureBox29";
			this.configurableQualityPictureBox29.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox29.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox29.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox29.TabIndex = 99;
			this.configurableQualityPictureBox29.TabStop = false;
			// 
			// configurableQualityPictureBox28
			// 
			this.configurableQualityPictureBox28.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox28.Image = global::HardwareInformation.Properties.Resources.type_white;
			this.configurableQualityPictureBox28.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox28.Location = new System.Drawing.Point(6, 125);
			this.configurableQualityPictureBox28.Name = "configurableQualityPictureBox28";
			this.configurableQualityPictureBox28.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox28.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox28.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox28.TabIndex = 98;
			this.configurableQualityPictureBox28.TabStop = false;
			// 
			// configurableQualityPictureBox27
			// 
			this.configurableQualityPictureBox27.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox27.Image = global::HardwareInformation.Properties.Resources.sticker_white;
			this.configurableQualityPictureBox27.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox27.Location = new System.Drawing.Point(275, 125);
			this.configurableQualityPictureBox27.Name = "configurableQualityPictureBox27";
			this.configurableQualityPictureBox27.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox27.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox27.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox27.TabIndex = 97;
			this.configurableQualityPictureBox27.TabStop = false;
			// 
			// configurableQualityPictureBox26
			// 
			this.configurableQualityPictureBox26.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox26.Image = global::HardwareInformation.Properties.Resources.in_use_white;
			this.configurableQualityPictureBox26.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox26.Location = new System.Drawing.Point(275, 98);
			this.configurableQualityPictureBox26.Name = "configurableQualityPictureBox26";
			this.configurableQualityPictureBox26.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox26.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox26.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox26.TabIndex = 96;
			this.configurableQualityPictureBox26.TabStop = false;
			// 
			// configurableQualityPictureBox24
			// 
			this.configurableQualityPictureBox24.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox24.Image = global::HardwareInformation.Properties.Resources.service_white;
			this.configurableQualityPictureBox24.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox24.Location = new System.Drawing.Point(6, 152);
			this.configurableQualityPictureBox24.Name = "configurableQualityPictureBox24";
			this.configurableQualityPictureBox24.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox24.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox24.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox24.TabIndex = 94;
			this.configurableQualityPictureBox24.TabStop = false;
			// 
			// configurableQualityPictureBox23
			// 
			this.configurableQualityPictureBox23.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox23.Image = global::HardwareInformation.Properties.Resources.standard_white;
			this.configurableQualityPictureBox23.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox23.Location = new System.Drawing.Point(274, 228);
			this.configurableQualityPictureBox23.Name = "configurableQualityPictureBox23";
			this.configurableQualityPictureBox23.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox23.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox23.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox23.TabIndex = 93;
			this.configurableQualityPictureBox23.TabStop = false;
			// 
			// configurableQualityPictureBox22
			// 
			this.configurableQualityPictureBox22.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox22.Image = global::HardwareInformation.Properties.Resources.server_white;
			this.configurableQualityPictureBox22.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox22.Location = new System.Drawing.Point(6, 228);
			this.configurableQualityPictureBox22.Name = "configurableQualityPictureBox22";
			this.configurableQualityPictureBox22.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox22.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox22.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox22.TabIndex = 92;
			this.configurableQualityPictureBox22.TabStop = false;
			// 
			// configurableQualityPictureBox21
			// 
			this.configurableQualityPictureBox21.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox21.Image = global::HardwareInformation.Properties.Resources.building_white;
			this.configurableQualityPictureBox21.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox21.Location = new System.Drawing.Point(6, 98);
			this.configurableQualityPictureBox21.Name = "configurableQualityPictureBox21";
			this.configurableQualityPictureBox21.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox21.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox21.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox21.TabIndex = 91;
			this.configurableQualityPictureBox21.TabStop = false;
			// 
			// configurableQualityPictureBox20
			// 
			this.configurableQualityPictureBox20.CompositingQuality = null;
			this.configurableQualityPictureBox20.Image = global::HardwareInformation.Properties.Resources.room_white;
			this.configurableQualityPictureBox20.InterpolationMode = null;
			this.configurableQualityPictureBox20.Location = new System.Drawing.Point(6, 71);
			this.configurableQualityPictureBox20.Name = "configurableQualityPictureBox20";
			this.configurableQualityPictureBox20.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox20.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox20.SmoothingMode = null;
			this.configurableQualityPictureBox20.TabIndex = 90;
			this.configurableQualityPictureBox20.TabStop = false;
			// 
			// configurableQualityPictureBox19
			// 
			this.configurableQualityPictureBox19.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox19.Image = global::HardwareInformation.Properties.Resources.seal_white;
			this.configurableQualityPictureBox19.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox19.Location = new System.Drawing.Point(6, 44);
			this.configurableQualityPictureBox19.Name = "configurableQualityPictureBox19";
			this.configurableQualityPictureBox19.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox19.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.configurableQualityPictureBox19.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox19.TabIndex = 89;
			this.configurableQualityPictureBox19.TabStop = false;
			// 
			// configurableQualityPictureBox18
			// 
			this.configurableQualityPictureBox18.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox18.Image = global::HardwareInformation.Properties.Resources.patr_white;
			this.configurableQualityPictureBox18.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox18.Location = new System.Drawing.Point(6, 17);
			this.configurableQualityPictureBox18.Name = "configurableQualityPictureBox18";
			this.configurableQualityPictureBox18.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox18.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox18.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox18.TabIndex = 88;
			this.configurableQualityPictureBox18.TabStop = false;
			// 
			// configurableQualityPictureBox30
			// 
			this.configurableQualityPictureBox30.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox30.Image = global::HardwareInformation.Properties.Resources.VT_x_white;
			this.configurableQualityPictureBox30.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox30.Location = new System.Drawing.Point(7, 449);
			this.configurableQualityPictureBox30.Name = "configurableQualityPictureBox30";
			this.configurableQualityPictureBox30.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox30.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox30.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox30.TabIndex = 104;
			this.configurableQualityPictureBox30.TabStop = false;
			// 
			// configurableQualityPictureBox2
			// 
			this.configurableQualityPictureBox2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox2.Image = global::HardwareInformation.Properties.Resources.brand_white;
			this.configurableQualityPictureBox2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox2.Location = new System.Drawing.Point(7, 17);
			this.configurableQualityPictureBox2.Name = "configurableQualityPictureBox2";
			this.configurableQualityPictureBox2.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox2.TabIndex = 101;
			this.configurableQualityPictureBox2.TabStop = false;
			// 
			// configurableQualityPictureBox17
			// 
			this.configurableQualityPictureBox17.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox17.Image = global::HardwareInformation.Properties.Resources.secure_boot_white;
			this.configurableQualityPictureBox17.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox17.Location = new System.Drawing.Point(7, 422);
			this.configurableQualityPictureBox17.Name = "configurableQualityPictureBox17";
			this.configurableQualityPictureBox17.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox17.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox17.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox17.TabIndex = 87;
			this.configurableQualityPictureBox17.TabStop = false;
			// 
			// configurableQualityPictureBox16
			// 
			this.configurableQualityPictureBox16.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox16.Image = global::HardwareInformation.Properties.Resources.bios_version_white;
			this.configurableQualityPictureBox16.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox16.Location = new System.Drawing.Point(7, 395);
			this.configurableQualityPictureBox16.Name = "configurableQualityPictureBox16";
			this.configurableQualityPictureBox16.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox16.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox16.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox16.TabIndex = 86;
			this.configurableQualityPictureBox16.TabStop = false;
			// 
			// configurableQualityPictureBox15
			// 
			this.configurableQualityPictureBox15.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox15.Image = global::HardwareInformation.Properties.Resources.bios_white;
			this.configurableQualityPictureBox15.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox15.Location = new System.Drawing.Point(7, 368);
			this.configurableQualityPictureBox15.Name = "configurableQualityPictureBox15";
			this.configurableQualityPictureBox15.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox15.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox15.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox15.TabIndex = 85;
			this.configurableQualityPictureBox15.TabStop = false;
			// 
			// configurableQualityPictureBox14
			// 
			this.configurableQualityPictureBox14.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox14.Image = global::HardwareInformation.Properties.Resources.ip_white;
			this.configurableQualityPictureBox14.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox14.Location = new System.Drawing.Point(7, 341);
			this.configurableQualityPictureBox14.Name = "configurableQualityPictureBox14";
			this.configurableQualityPictureBox14.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox14.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox14.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox14.TabIndex = 84;
			this.configurableQualityPictureBox14.TabStop = false;
			// 
			// configurableQualityPictureBox13
			// 
			this.configurableQualityPictureBox13.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox13.Image = global::HardwareInformation.Properties.Resources.mac_white;
			this.configurableQualityPictureBox13.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox13.Location = new System.Drawing.Point(7, 314);
			this.configurableQualityPictureBox13.Name = "configurableQualityPictureBox13";
			this.configurableQualityPictureBox13.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox13.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox13.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox13.TabIndex = 83;
			this.configurableQualityPictureBox13.TabStop = false;
			// 
			// configurableQualityPictureBox12
			// 
			this.configurableQualityPictureBox12.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox12.Image = global::HardwareInformation.Properties.Resources.hostname_white;
			this.configurableQualityPictureBox12.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox12.Location = new System.Drawing.Point(7, 287);
			this.configurableQualityPictureBox12.Name = "configurableQualityPictureBox12";
			this.configurableQualityPictureBox12.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox12.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox12.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox12.TabIndex = 82;
			this.configurableQualityPictureBox12.TabStop = false;
			// 
			// configurableQualityPictureBox11
			// 
			this.configurableQualityPictureBox11.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox11.Image = global::HardwareInformation.Properties.Resources.windows_white;
			this.configurableQualityPictureBox11.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox11.Location = new System.Drawing.Point(7, 260);
			this.configurableQualityPictureBox11.Name = "configurableQualityPictureBox11";
			this.configurableQualityPictureBox11.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox11.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox11.TabIndex = 81;
			this.configurableQualityPictureBox11.TabStop = false;
			// 
			// configurableQualityPictureBox10
			// 
			this.configurableQualityPictureBox10.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox10.Image = global::HardwareInformation.Properties.Resources.gpu_white;
			this.configurableQualityPictureBox10.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox10.Location = new System.Drawing.Point(7, 233);
			this.configurableQualityPictureBox10.Name = "configurableQualityPictureBox10";
			this.configurableQualityPictureBox10.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox10.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox10.TabIndex = 80;
			this.configurableQualityPictureBox10.TabStop = false;
			// 
			// configurableQualityPictureBox9
			// 
			this.configurableQualityPictureBox9.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox9.Image = global::HardwareInformation.Properties.Resources.ahci_white;
			this.configurableQualityPictureBox9.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox9.Location = new System.Drawing.Point(7, 206);
			this.configurableQualityPictureBox9.Name = "configurableQualityPictureBox9";
			this.configurableQualityPictureBox9.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox9.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox9.TabIndex = 79;
			this.configurableQualityPictureBox9.TabStop = false;
			// 
			// configurableQualityPictureBox8
			// 
			this.configurableQualityPictureBox8.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox8.Image = global::HardwareInformation.Properties.Resources.hdd_white;
			this.configurableQualityPictureBox8.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox8.Location = new System.Drawing.Point(7, 179);
			this.configurableQualityPictureBox8.Name = "configurableQualityPictureBox8";
			this.configurableQualityPictureBox8.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox8.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox8.TabIndex = 78;
			this.configurableQualityPictureBox8.TabStop = false;
			// 
			// configurableQualityPictureBox7
			// 
			this.configurableQualityPictureBox7.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox7.Image = global::HardwareInformation.Properties.Resources.disk_size_white;
			this.configurableQualityPictureBox7.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox7.Location = new System.Drawing.Point(7, 152);
			this.configurableQualityPictureBox7.Name = "configurableQualityPictureBox7";
			this.configurableQualityPictureBox7.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox7.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox7.TabIndex = 77;
			this.configurableQualityPictureBox7.TabStop = false;
			// 
			// configurableQualityPictureBox6
			// 
			this.configurableQualityPictureBox6.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox6.Image = global::HardwareInformation.Properties.Resources.ram_white;
			this.configurableQualityPictureBox6.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox6.Location = new System.Drawing.Point(7, 125);
			this.configurableQualityPictureBox6.Name = "configurableQualityPictureBox6";
			this.configurableQualityPictureBox6.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox6.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox6.TabIndex = 76;
			this.configurableQualityPictureBox6.TabStop = false;
			// 
			// configurableQualityPictureBox5
			// 
			this.configurableQualityPictureBox5.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox5.Image = global::HardwareInformation.Properties.Resources.cpu_white;
			this.configurableQualityPictureBox5.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox5.Location = new System.Drawing.Point(7, 98);
			this.configurableQualityPictureBox5.Name = "configurableQualityPictureBox5";
			this.configurableQualityPictureBox5.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox5.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox5.TabIndex = 75;
			this.configurableQualityPictureBox5.TabStop = false;
			// 
			// configurableQualityPictureBox4
			// 
			this.configurableQualityPictureBox4.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox4.Image = global::HardwareInformation.Properties.Resources.serial_no_white;
			this.configurableQualityPictureBox4.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox4.Location = new System.Drawing.Point(7, 71);
			this.configurableQualityPictureBox4.Name = "configurableQualityPictureBox4";
			this.configurableQualityPictureBox4.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox4.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox4.TabIndex = 74;
			this.configurableQualityPictureBox4.TabStop = false;
			// 
			// configurableQualityPictureBox3
			// 
			this.configurableQualityPictureBox3.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			this.configurableQualityPictureBox3.Image = global::HardwareInformation.Properties.Resources.model_white;
			this.configurableQualityPictureBox3.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			this.configurableQualityPictureBox3.Location = new System.Drawing.Point(7, 44);
			this.configurableQualityPictureBox3.Name = "configurableQualityPictureBox3";
			this.configurableQualityPictureBox3.Size = new System.Drawing.Size(25, 25);
			this.configurableQualityPictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox3.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.configurableQualityPictureBox3.TabIndex = 73;
			this.configurableQualityPictureBox3.TabStop = false;
			// 
			// configurableQualityPictureBox1
			// 
			this.configurableQualityPictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.configurableQualityPictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.configurableQualityPictureBox1.CompositingQuality = null;
			this.configurableQualityPictureBox1.Image = global::HardwareInformation.Properties.Resources.banner_dark;
			this.configurableQualityPictureBox1.InitialImage = null;
			this.configurableQualityPictureBox1.InterpolationMode = null;
			this.configurableQualityPictureBox1.Location = new System.Drawing.Point(-5, -2);
			this.configurableQualityPictureBox1.Name = "configurableQualityPictureBox1";
			this.configurableQualityPictureBox1.Size = new System.Drawing.Size(1063, 109);
			this.configurableQualityPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.configurableQualityPictureBox1.SmoothingMode = null;
			this.configurableQualityPictureBox1.TabIndex = 64;
			this.configurableQualityPictureBox1.TabStop = false;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.ClientSize = new System.Drawing.Size(1058, 675);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.configurableQualityPictureBox1);
			this.Controls.Add(this.accessSystemButton);
			this.Controls.Add(this.coletaButton);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.cadastraButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox31)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox25)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox29)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox28)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox27)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox26)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox24)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox23)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox22)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox21)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox20)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox19)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox18)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox30)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox17)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox16)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox15)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox14)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox13)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox12)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox11)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox1)).EndInit();
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
		   varCalend, varUso, varTag, varTipo, varBIOS, varBIOSType, varSecBoot, varVT;
		private string BM, Model, SerialNo, ProcName, PM, HDSize, MediaType,
		   MediaOperation, GPUInfo, OS, Hostname, Mac, IP, BIOS, BIOSType, SecBoot, VT;
		private int i = 0;
		private Label label25;
		private Label lblBIOSType;
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
		private DateTimePicker dateTimePicker1;
		private ConfigurableQualityPictureBox configurableQualityPictureBox1;
		private ConfigurableQualityPictureBox configurableQualityPictureBox2;
		private ConfigurableQualityPictureBox configurableQualityPictureBox3;
		private ConfigurableQualityPictureBox configurableQualityPictureBox4;
		private ConfigurableQualityPictureBox configurableQualityPictureBox5;
		private ConfigurableQualityPictureBox configurableQualityPictureBox6;
		private ConfigurableQualityPictureBox configurableQualityPictureBox7;
		private ConfigurableQualityPictureBox configurableQualityPictureBox8;
		private ConfigurableQualityPictureBox configurableQualityPictureBox9;
		private ConfigurableQualityPictureBox configurableQualityPictureBox10;
		private ConfigurableQualityPictureBox configurableQualityPictureBox11;
		private ConfigurableQualityPictureBox configurableQualityPictureBox12;
		private ConfigurableQualityPictureBox configurableQualityPictureBox13;
		private ConfigurableQualityPictureBox configurableQualityPictureBox14;
		private ConfigurableQualityPictureBox configurableQualityPictureBox15;
		private ConfigurableQualityPictureBox configurableQualityPictureBox16;
		private ConfigurableQualityPictureBox configurableQualityPictureBox17;
		private ConfigurableQualityPictureBox configurableQualityPictureBox18;
		private ConfigurableQualityPictureBox configurableQualityPictureBox19;
		private ConfigurableQualityPictureBox configurableQualityPictureBox20;
		private ConfigurableQualityPictureBox configurableQualityPictureBox21;
		private ConfigurableQualityPictureBox configurableQualityPictureBox22;
		private ConfigurableQualityPictureBox configurableQualityPictureBox23;
		private ConfigurableQualityPictureBox configurableQualityPictureBox24;
		private ConfigurableQualityPictureBox configurableQualityPictureBox25;
		private ConfigurableQualityPictureBox configurableQualityPictureBox26;
		private ConfigurableQualityPictureBox configurableQualityPictureBox27;
		private ConfigurableQualityPictureBox configurableQualityPictureBox28;
		private ConfigurableQualityPictureBox configurableQualityPictureBox29;
		private ConfigurableQualityPictureBox configurableQualityPictureBox30;
		private Label lblVT;
		private Label label33;
		private ConfigurableQualityPictureBox configurableQualityPictureBox31;
		private Label label34;
		private RadioButton studentButton2;
		private RadioButton employeeButton1;
		private Label label42;
		private Label label41;
		private Label label40;
		private Label label39;
		private Label label38;
		private Label label37;
		private Label label36;
		private Label label35;
		private Label label43;
		private Timer timer7;
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

		//Sets service to employee
		private void employeeButton1_CheckedChanged(object sender, EventArgs e)
		{
			comboBox2.SelectedIndex = 0;
			comboBox3.SelectedIndex = 1;
		}

		//Sets service to student
		private void studentButton2_CheckedChanged(object sender, EventArgs e)
		{
			comboBox2.SelectedIndex = 1;
			comboBox3.SelectedIndex = 0;
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
			catch
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
			catch
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
			//this.monthCalendar1.BackColor = SystemColors.ControlLight;
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
			this.lblVT.ForeColor = SystemColors.ControlText;
			this.lblSecBoot.ForeColor = SystemColors.ControlText;
			this.label32.ForeColor = SystemColors.ControlText;
			this.label33.ForeColor = SystemColors.ControlText;
			this.label34.ForeColor = SystemColors.ControlText;
			this.label35.ForeColor = Color.Red;
			this.label36.ForeColor = Color.Red;
			this.label37.ForeColor = Color.Red;
			this.label38.ForeColor = Color.Red;
			this.label39.ForeColor = Color.Red;
			this.label40.ForeColor = Color.Red;
			this.label41.ForeColor = Color.Red;
			this.label42.ForeColor = Color.Red;
			this.label43.ForeColor = Color.Red;
			this.lblGPUInfo.ForeColor = SystemColors.ControlText;
			this.lblVT.ForeColor = SystemColors.ControlText;
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
			this.configurableQualityPictureBox1.Image = global::HardwareInformation.Properties.Resources.banner_light;
			this.configurableQualityPictureBox2.Image = global::HardwareInformation.Properties.Resources.brand_black;
			this.configurableQualityPictureBox3.Image = global::HardwareInformation.Properties.Resources.model_black;
			this.configurableQualityPictureBox4.Image = global::HardwareInformation.Properties.Resources.serial_no_black;
			this.configurableQualityPictureBox5.Image = global::HardwareInformation.Properties.Resources.cpu_black;
			this.configurableQualityPictureBox6.Image = global::HardwareInformation.Properties.Resources.ram_black;
			this.configurableQualityPictureBox7.Image = global::HardwareInformation.Properties.Resources.disk_size_black;
			this.configurableQualityPictureBox8.Image = global::HardwareInformation.Properties.Resources.hdd_black;
			this.configurableQualityPictureBox9.Image = global::HardwareInformation.Properties.Resources.ahci_black;
			this.configurableQualityPictureBox10.Image = global::HardwareInformation.Properties.Resources.gpu_black;
			this.configurableQualityPictureBox11.Image = global::HardwareInformation.Properties.Resources.windows_black;
			this.configurableQualityPictureBox12.Image = global::HardwareInformation.Properties.Resources.hostname_black;
			this.configurableQualityPictureBox13.Image = global::HardwareInformation.Properties.Resources.mac_black;
			this.configurableQualityPictureBox14.Image = global::HardwareInformation.Properties.Resources.ip_black;
			this.configurableQualityPictureBox15.Image = global::HardwareInformation.Properties.Resources.bios_black;
			this.configurableQualityPictureBox16.Image = global::HardwareInformation.Properties.Resources.bios_version_black;
			this.configurableQualityPictureBox17.Image = global::HardwareInformation.Properties.Resources.secure_boot_black;
			this.configurableQualityPictureBox18.Image = global::HardwareInformation.Properties.Resources.patr_black;
			this.configurableQualityPictureBox19.Image = global::HardwareInformation.Properties.Resources.seal_black;
			this.configurableQualityPictureBox20.Image = global::HardwareInformation.Properties.Resources.room_black;
			this.configurableQualityPictureBox21.Image = global::HardwareInformation.Properties.Resources.building_black;
			this.configurableQualityPictureBox22.Image = global::HardwareInformation.Properties.Resources.server_black;
			this.configurableQualityPictureBox23.Image = global::HardwareInformation.Properties.Resources.standard_black;
			this.configurableQualityPictureBox24.Image = global::HardwareInformation.Properties.Resources.service_black;
			this.configurableQualityPictureBox25.Image = global::HardwareInformation.Properties.Resources.letter_black;
			this.configurableQualityPictureBox26.Image = global::HardwareInformation.Properties.Resources.in_use_black;
			this.configurableQualityPictureBox27.Image = global::HardwareInformation.Properties.Resources.sticker_black;
			this.configurableQualityPictureBox28.Image = global::HardwareInformation.Properties.Resources.type_black;
			this.configurableQualityPictureBox29.Image = global::HardwareInformation.Properties.Resources.server_black;
			this.configurableQualityPictureBox30.Image = global::HardwareInformation.Properties.Resources.VT_x_black;
			this.configurableQualityPictureBox31.Image = global::HardwareInformation.Properties.Resources.who_black;
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
			//this.monthCalendar1.BackColor = SystemColors.WindowFrame;
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
			this.lblVT.ForeColor = SystemColors.ControlLightLight;
			this.lblSecBoot.ForeColor = SystemColors.ControlLightLight;
			this.label32.ForeColor = SystemColors.ControlLightLight;
			this.label33.ForeColor = SystemColors.ControlLightLight;
			this.label34.ForeColor = SystemColors.ControlLightLight;
			this.label35.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label36.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label37.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label38.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label39.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label40.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label41.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label42.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.label43.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
			this.lblGPUInfo.ForeColor = SystemColors.ControlLightLight;
			this.lblVT.ForeColor = SystemColors.ControlLightLight;
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
			this.configurableQualityPictureBox1.Image = global::HardwareInformation.Properties.Resources.banner_dark;
			this.configurableQualityPictureBox2.Image = global::HardwareInformation.Properties.Resources.brand_white;
			this.configurableQualityPictureBox3.Image = global::HardwareInformation.Properties.Resources.model_white;
			this.configurableQualityPictureBox4.Image = global::HardwareInformation.Properties.Resources.serial_no_white;
			this.configurableQualityPictureBox5.Image = global::HardwareInformation.Properties.Resources.cpu_white;
			this.configurableQualityPictureBox6.Image = global::HardwareInformation.Properties.Resources.ram_white;
			this.configurableQualityPictureBox7.Image = global::HardwareInformation.Properties.Resources.disk_size_white;
			this.configurableQualityPictureBox8.Image = global::HardwareInformation.Properties.Resources.hdd_white;
			this.configurableQualityPictureBox9.Image = global::HardwareInformation.Properties.Resources.ahci_white;
			this.configurableQualityPictureBox10.Image = global::HardwareInformation.Properties.Resources.gpu_white;
			this.configurableQualityPictureBox11.Image = global::HardwareInformation.Properties.Resources.windows_white;
			this.configurableQualityPictureBox12.Image = global::HardwareInformation.Properties.Resources.hostname_white;
			this.configurableQualityPictureBox13.Image = global::HardwareInformation.Properties.Resources.mac_white;
			this.configurableQualityPictureBox14.Image = global::HardwareInformation.Properties.Resources.ip_white;
			this.configurableQualityPictureBox15.Image = global::HardwareInformation.Properties.Resources.bios_white;
			this.configurableQualityPictureBox16.Image = global::HardwareInformation.Properties.Resources.bios_version_white;
			this.configurableQualityPictureBox17.Image = global::HardwareInformation.Properties.Resources.secure_boot_white;
			this.configurableQualityPictureBox18.Image = global::HardwareInformation.Properties.Resources.patr_white;
			this.configurableQualityPictureBox19.Image = global::HardwareInformation.Properties.Resources.seal_white;
			this.configurableQualityPictureBox20.Image = global::HardwareInformation.Properties.Resources.room_white;
			this.configurableQualityPictureBox21.Image = global::HardwareInformation.Properties.Resources.building_white;
			this.configurableQualityPictureBox22.Image = global::HardwareInformation.Properties.Resources.server_white;
			this.configurableQualityPictureBox23.Image = global::HardwareInformation.Properties.Resources.standard_white;
			this.configurableQualityPictureBox24.Image = global::HardwareInformation.Properties.Resources.service_white;
			this.configurableQualityPictureBox25.Image = global::HardwareInformation.Properties.Resources.letter_white;
			this.configurableQualityPictureBox26.Image = global::HardwareInformation.Properties.Resources.in_use_white;
			this.configurableQualityPictureBox27.Image = global::HardwareInformation.Properties.Resources.sticker_white;
			this.configurableQualityPictureBox28.Image = global::HardwareInformation.Properties.Resources.type_white;
			this.configurableQualityPictureBox29.Image = global::HardwareInformation.Properties.Resources.server_white;
			this.configurableQualityPictureBox30.Image = global::HardwareInformation.Properties.Resources.VT_x_white;
			this.configurableQualityPictureBox31.Image = global::HardwareInformation.Properties.Resources.who_white;
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
			timer7.Tick += new EventHandler(flashTextVT);
			timer1.Interval = 500;
			timer2.Interval = 500;
			timer3.Interval = 500;
			timer4.Interval = 500;
			timer5.Interval = 500;
			timer6.Interval = 500;
			timer7.Interval = 500;
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
			else if (lblHostname.ForeColor == Color.Red && themeBool == false)
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

		//Sets the VT label to flash in red
		private void flashTextVT(Object myobject, EventArgs myEventArgs)
		{
			if (lblVT.ForeColor == Color.Red && themeBool == true)
				lblVT.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			else if (lblVT.ForeColor == Color.Red && themeBool == false)
				lblVT.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			else
				lblVT.ForeColor = Color.Red;
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

		//Sets the Mac and IP labels to flash in red
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

		//Sets the BIOS Firmware Type label to flash in red
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
			timer7.Enabled = false;
			if (lblHostname.ForeColor == Color.Red && themeBool == true)
			{
				lblHostname.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
				lblMediaOperation.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
				lblSecBoot.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
				lblBIOS.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
				lblVT.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			}
			else if (lblHostname.ForeColor == Color.Red && themeBool == false)
			{
				lblHostname.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
				lblMediaOperation.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
				lblSecBoot.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
				lblBIOS.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
				lblVT.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			}

			//Sets current and maximum values for the progressbar
			progressBar1.Maximum = 17;
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
			lblVT.Text = coletando;
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
			i++;
			VT = HardwareInfo.GetVirtualizationTechnology();
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
			lblVT.Text = VT;
			string[] str = BIOSFileReader.fetchInfo(lblBM.Text, lblModel.Text, lblBIOSType.Text, comboBox7.Text, comboBox8.Text);
			if (lblHostname.Text.Equals("MUDAR-NOME"))
			{
				pass = false;
				lblHostname.Text += " (Nome incorreto, alterar)";
				timer1.Enabled = true;
			}
			if (!lblModel.Text.Contains("7057") && !lblModel.Text.Contains("8814") && !lblModel.Text.Contains("6078") && lblMediaOperation.Text.Equals("IDE/Legacy ou RAID"))
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
			if (str == null)
			{
				pass = false;
				MessageBox.Show("Erro ao contatar o banco de dados, verifique a sua conexão com a intranet e se o servidor web está ativo!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			if (str != null && !lblBIOS.Text.Contains(str[0]))
			{
				if (!str[0].Equals("-1"))
				{
					pass = false;
					lblBIOS.Text += " (Atualizar BIOS/UEFI)";
					timer4.Enabled = true;
				}
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
			if (lblVT.Text == "Desativado")
			{
				pass = false;
				lblVT.Text += " (Ativar Tecnologia de Virtualização na BIOS/UEFI)";
				timer7.Enabled = true;
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
			varVT = lblVT.Text;
		}

		//Runs the registration for the website
		private async void cadastra_ClickAsync(object sender, EventArgs e)
		{
			cadastraButton.Text = cadastrando;
			cadastraButton.Enabled = false;
			accessSystemButton.Enabled = false;
			coletaButton.Enabled = false;
			attrHardwareData();
			if (!string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox3.Text) && comboBox6.SelectedItem != null && comboBox1.SelectedItem != null && comboBox4.SelectedItem != null && comboBox5.SelectedItem != null && (employeeButton1.Checked || studentButton2.Checked) && (formatButton1.Checked || maintenanceButton2.Checked) && pass == true)
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
				varCalend = dateTimePicker1.Value.ToString();
				servidor_web = comboBox7.Text;
				porta = comboBox8.Text;

				var webView2Environment = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync("runtimes\\win-x86", System.IO.Path.GetTempPath());
				await webView2.EnsureCoreWebView2Async(webView2Environment);

				if (PingHost(servidor_web) == true && porta != "")
				{
					if (mode)
						webView2.CoreWebView2.Navigate("http://" + servidor_web + ":" + porta + "/recebeDadosFormatacao.php?patrimonio=" + varPatrimonio + "&lacre=" + varLacre +
					 "&sala=" + varSala + "&predio=" + varPredio + "&ad=" + varCadastrado + "&padrao=" + varPadrao + "&formatacao=" + varCalend + "&formatacoesAnteriores=" + varCalend +
					 "&marca=" + varBoard + "&modelo=" + varModel + "&numeroSerial=" + varSerial + "&processador=" + varProc + "&memoria=" + varRAM +
					 "&hd=" + varHD + "&sistemaOperacional=" + varOS + "&nomeDoComputador=" + varHostname + "&bios=" + varBIOS + "&mac=" + varMac + "&ip=" + varIP + "&emUso=" + varUso +
					 "&etiqueta=" + varTag + "&tipo=" + varTipo + "&tipoFW=" + varBIOSType + "&tipoArmaz=" + varHDType + "&gpu=" + varGPUInfo + "&modoArmaz=" + varHDOperation +
					 "&secBoot=" + varSecBoot + "&vt=" + varVT);
					else
						webView2.CoreWebView2.Navigate("http://" + servidor_web + ":" + porta + "/recebeDadosManutencao.php?patrimonio=" + varPatrimonio + "&lacre=" + varLacre +
					 "&sala=" + varSala + "&predio=" + varPredio + "&ad=" + varCadastrado + "&padrao=" + varPadrao + "&formatacao=" + varCalend + "&formatacoesAnteriores=" + varCalend +
					 "&marca=" + varBoard + "&modelo=" + varModel + "&numeroSerial=" + varSerial + "&processador=" + varProc + "&memoria=" + varRAM +
					 "&hd=" + varHD + "&nomeDoComputador=" + varHostname + "&bios=" + varBIOS + "&mac=" + varMac + "&ip=" + varIP + "&emUso=" + varUso +
					 "&etiqueta=" + varTag + "&tipo=" + varTipo + "&tipoFW=" + varBIOSType + "&tipoArmaz=" + varHDType + "&gpu=" + varGPUInfo + "&modoArmaz=" + varHDOperation +
					 "&secBoot=" + varSecBoot + "&vt=" + varVT);
				}
				else
					MessageBox.Show("Servidor não encontrado. Selecione um servidor válido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else if (!pass)
				MessageBox.Show("Resolva as pendencias exibidas em vermelho!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
			else
				MessageBox.Show("Preencha os campos obrigatórios", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
			cadastraButton.Text = cadastrarNovamente;
			cadastraButton.Enabled = true;
			accessSystemButton.Enabled = true;
			coletaButton.Enabled = true;
		}
	}
}
