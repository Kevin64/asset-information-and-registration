using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using HardwareInfoDLL;
using ConstantsDLL;
using JsonFileReaderDLL;
using ConfigurableQualityPictureBoxDLL;

namespace HardwareInformation
{
    public partial class Form1 : Form
	{
		private BackgroundWorker backgroundWorker1;
		public Form1(bool noConnection, string user, string ip, string port)
		{
            InitializeComponent();

            themeBool = MiscMethods.ThemeInit();
            offlineMode = noConnection;
            this.user = user;
            this.ip = ip;
            this.port = port;

            comboBoxBuilding.Items.AddRange(StringsAndConstants.listBuilding.ToArray());
            comboBoxActiveDirectory.Items.AddRange(StringsAndConstants.listActiveDirectory.ToArray());
            comboBoxStandard.Items.AddRange(StringsAndConstants.listStandard.ToArray());
            comboBoxInUse.Items.AddRange(StringsAndConstants.listInUse.ToArray());
            comboBoxTag.Items.AddRange(StringsAndConstants.listTag.ToArray());
            comboBoxType.Items.AddRange(StringsAndConstants.listType.ToArray());
            comboBoxBattery.Items.AddRange(StringsAndConstants.listBattery.ToArray());

            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker1_ProgressChanged);
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            //Change this for alpha, beta and final releases - use alpha, beta and blank respectively
            this.toolStripStatusLabel2.Text = MiscMethods.version();
            //this.toolStripStatusLabel2.Text = MiscMethods.version("beta");
            //this.toolStripStatusLabel2.Text = MiscMethods.version("alpha");
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
            this.textBoxPatrimony = new System.Windows.Forms.TextBox();
            this.textBoxSeal = new System.Windows.Forms.TextBox();
            this.textBoxRoom = new System.Windows.Forms.TextBox();
            this.textBoxLetter = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBoxBuilding = new System.Windows.Forms.ComboBox();
            this.comboBoxActiveDirectory = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.comboBoxStandard = new System.Windows.Forms.ComboBox();
            this.registerButton = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.comboBoxInUse = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.comboBoxTag = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.collectButton = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.lblBIOS = new System.Windows.Forms.Label();
            this.accessSystemButton = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.lblBIOSType = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.configurableQualityPictureBox33 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox32 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblSmart = new System.Windows.Forms.Label();
            this.lblTPM = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.configurableQualityPictureBox30 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.label45 = new System.Windows.Forms.Label();
            this.lblVT = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.configurableQualityPictureBox2 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox17 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox16 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox15 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox14 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox13 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox12 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox11 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox10 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox9 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox8 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox7 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox6 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox5 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox4 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox3 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
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
            this.label52 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.configurableQualityPictureBox35 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.label31 = new System.Windows.Forms.Label();
            this.textBoxTicket = new System.Windows.Forms.TextBox();
            this.comboBoxBattery = new System.Windows.Forms.ComboBox();
            this.configurableQualityPictureBox34 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.label42 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.studentRadioButton = new System.Windows.Forms.RadioButton();
            this.employeeRadioButton = new System.Windows.Forms.RadioButton();
            this.configurableQualityPictureBox31 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.label34 = new System.Windows.Forms.Label();
            this.configurableQualityPictureBox25 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox29 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox28 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox27 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox26 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox24 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox23 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox22 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox21 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox20 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox19 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox18 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label26 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblMaintenanceSince = new System.Windows.Forms.Label();
            this.lblInstallSince = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.formatButton = new System.Windows.Forms.RadioButton();
            this.maintenanceButton = new System.Windows.Forms.RadioButton();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.comboBoxTheme = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.timer4 = new System.Windows.Forms.Timer(this.components);
            this.timer5 = new System.Windows.Forms.Timer(this.components);
            this.timer6 = new System.Windows.Forms.Timer(this.components);
            this.timer7 = new System.Windows.Forms.Timer(this.components);
            this.timer8 = new System.Windows.Forms.Timer(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.configurableQualityPictureBox1 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox33)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox32)).BeginInit();
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
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox35)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox34)).BeginInit();
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
            this.groupBox3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBM
            // 
            this.lblBM.AutoSize = true;
            this.lblBM.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblBM.Location = new System.Drawing.Point(197, 25);
            this.lblBM.Name = "lblBM";
            this.lblBM.Size = new System.Drawing.Size(10, 13);
            this.lblBM.TabIndex = 7;
            this.lblBM.Text = "-";
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblModel.Location = new System.Drawing.Point(197, 52);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(10, 13);
            this.lblModel.TabIndex = 8;
            this.lblModel.Text = "-";
            // 
            // lblSerialNo
            // 
            this.lblSerialNo.AutoSize = true;
            this.lblSerialNo.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblSerialNo.Location = new System.Drawing.Point(197, 79);
            this.lblSerialNo.Name = "lblSerialNo";
            this.lblSerialNo.Size = new System.Drawing.Size(10, 13);
            this.lblSerialNo.TabIndex = 9;
            this.lblSerialNo.Text = "-";
            // 
            // lblProcName
            // 
            this.lblProcName.AutoSize = true;
            this.lblProcName.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblProcName.Location = new System.Drawing.Point(197, 106);
            this.lblProcName.Name = "lblProcName";
            this.lblProcName.Size = new System.Drawing.Size(10, 13);
            this.lblProcName.TabIndex = 10;
            this.lblProcName.Text = "-";
            // 
            // lblPM
            // 
            this.lblPM.AutoSize = true;
            this.lblPM.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblPM.Location = new System.Drawing.Point(197, 133);
            this.lblPM.Name = "lblPM";
            this.lblPM.Size = new System.Drawing.Size(10, 13);
            this.lblPM.TabIndex = 11;
            this.lblPM.Text = "-";
            // 
            // lblHDSize
            // 
            this.lblHDSize.AutoSize = true;
            this.lblHDSize.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblHDSize.Location = new System.Drawing.Point(197, 160);
            this.lblHDSize.Name = "lblHDSize";
            this.lblHDSize.Size = new System.Drawing.Size(10, 13);
            this.lblHDSize.TabIndex = 12;
            this.lblHDSize.Text = "-";
            // 
            // lblOS
            // 
            this.lblOS.AutoSize = true;
            this.lblOS.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblOS.Location = new System.Drawing.Point(197, 295);
            this.lblOS.Name = "lblOS";
            this.lblOS.Size = new System.Drawing.Size(10, 13);
            this.lblOS.TabIndex = 13;
            this.lblOS.Text = "-";
            // 
            // lblHostname
            // 
            this.lblHostname.AutoSize = true;
            this.lblHostname.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblHostname.Location = new System.Drawing.Point(197, 322);
            this.lblHostname.Name = "lblHostname";
            this.lblHostname.Size = new System.Drawing.Size(10, 13);
            this.lblHostname.TabIndex = 15;
            this.lblHostname.Text = "-";
            // 
            // lblMac
            // 
            this.lblMac.AutoSize = true;
            this.lblMac.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMac.Location = new System.Drawing.Point(197, 349);
            this.lblMac.Name = "lblMac";
            this.lblMac.Size = new System.Drawing.Size(10, 13);
            this.lblMac.TabIndex = 18;
            this.lblMac.Text = "-";
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblIP.Location = new System.Drawing.Point(197, 376);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(10, 13);
            this.lblIP.TabIndex = 19;
            this.lblIP.Text = "-";
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
            this.label7.Location = new System.Drawing.Point(37, 295);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Sistema Operacional:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label8.Location = new System.Drawing.Point(37, 322);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(113, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Nome do Computador:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label9.Location = new System.Drawing.Point(37, 349);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(118, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Endereço MAC do NIC:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label10.Location = new System.Drawing.Point(37, 376);
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
            // textBoxPatrimony
            // 
            this.textBoxPatrimony.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.textBoxPatrimony.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxPatrimony.Location = new System.Drawing.Point(185, 22);
            this.textBoxPatrimony.MaxLength = 6;
            this.textBoxPatrimony.Name = "textBoxPatrimony";
            this.textBoxPatrimony.Size = new System.Drawing.Size(259, 20);
            this.textBoxPatrimony.TabIndex = 34;
            this.textBoxPatrimony.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumbersOnly_KeyPress);
            // 
            // textBoxSeal
            // 
            this.textBoxSeal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.textBoxSeal.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxSeal.Location = new System.Drawing.Point(185, 49);
            this.textBoxSeal.MaxLength = 10;
            this.textBoxSeal.Name = "textBoxSeal";
            this.textBoxSeal.Size = new System.Drawing.Size(259, 20);
            this.textBoxSeal.TabIndex = 35;
            this.textBoxSeal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumbersOnly_KeyPress);
            // 
            // textBoxRoom
            // 
            this.textBoxRoom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.textBoxRoom.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxRoom.Location = new System.Drawing.Point(185, 76);
            this.textBoxRoom.MaxLength = 4;
            this.textBoxRoom.Name = "textBoxRoom";
            this.textBoxRoom.Size = new System.Drawing.Size(101, 20);
            this.textBoxRoom.TabIndex = 36;
            this.textBoxRoom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumbersOnly_KeyPress);
            // 
            // textBoxLetter
            // 
            this.textBoxLetter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.textBoxLetter.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxLetter.Location = new System.Drawing.Point(419, 76);
            this.textBoxLetter.MaxLength = 1;
            this.textBoxLetter.Name = "textBoxLetter";
            this.textBoxLetter.Size = new System.Drawing.Size(25, 20);
            this.textBoxLetter.TabIndex = 37;
            this.textBoxLetter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxCharsOnly_KeyPress);
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
            this.label15.Location = new System.Drawing.Point(35, 224);
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
            // comboBoxBuilding
            // 
            this.comboBoxBuilding.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBuilding.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBoxBuilding.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxBuilding.FormattingEnabled = true;
            this.comboBoxBuilding.Location = new System.Drawing.Point(185, 103);
            this.comboBoxBuilding.Name = "comboBoxBuilding";
            this.comboBoxBuilding.Size = new System.Drawing.Size(101, 21);
            this.comboBoxBuilding.Sorted = true;
            this.comboBoxBuilding.TabIndex = 38;
            // 
            // comboBoxActiveDirectory
            // 
            this.comboBoxActiveDirectory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxActiveDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxActiveDirectory.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBoxActiveDirectory.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxActiveDirectory.FormattingEnabled = true;
            this.comboBoxActiveDirectory.Location = new System.Drawing.Point(185, 221);
            this.comboBoxActiveDirectory.Name = "comboBoxActiveDirectory";
            this.comboBoxActiveDirectory.Size = new System.Drawing.Size(84, 21);
            this.comboBoxActiveDirectory.Sorted = true;
            this.comboBoxActiveDirectory.TabIndex = 45;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label17.Location = new System.Drawing.Point(303, 224);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(44, 13);
            this.label17.TabIndex = 15;
            this.label17.Text = "Padrão:";
            // 
            // comboBoxStandard
            // 
            this.comboBoxStandard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStandard.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBoxStandard.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxStandard.FormattingEnabled = true;
            this.comboBoxStandard.Location = new System.Drawing.Point(384, 221);
            this.comboBoxStandard.Name = "comboBoxStandard";
            this.comboBoxStandard.Size = new System.Drawing.Size(60, 21);
            this.comboBoxStandard.Sorted = true;
            this.comboBoxStandard.TabIndex = 46;
            // 
            // registerButton
            // 
            this.registerButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.registerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.registerButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.registerButton.Location = new System.Drawing.Point(767, 643);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(258, 56);
            this.registerButton.TabIndex = 53;
            this.registerButton.Text = "Cadastrar / Atualizar dados";
            this.registerButton.UseVisualStyleBackColor = false;
            this.registerButton.Click += new System.EventHandler(this.cadastra_ClickAsync);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label18.Location = new System.Drawing.Point(323, 106);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(45, 13);
            this.label18.TabIndex = 48;
            this.label18.Text = "Em uso:";
            // 
            // comboBoxInUse
            // 
            this.comboBoxInUse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxInUse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInUse.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBoxInUse.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxInUse.FormattingEnabled = true;
            this.comboBoxInUse.Location = new System.Drawing.Point(384, 103);
            this.comboBoxInUse.Name = "comboBoxInUse";
            this.comboBoxInUse.Size = new System.Drawing.Size(60, 21);
            this.comboBoxInUse.Sorted = true;
            this.comboBoxInUse.TabIndex = 39;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label19.Location = new System.Drawing.Point(323, 133);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(49, 13);
            this.label19.TabIndex = 50;
            this.label19.Text = "Etiqueta:";
            // 
            // comboBoxTag
            // 
            this.comboBoxTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTag.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBoxTag.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxTag.FormattingEnabled = true;
            this.comboBoxTag.Location = new System.Drawing.Point(384, 130);
            this.comboBoxTag.Name = "comboBoxTag";
            this.comboBoxTag.Size = new System.Drawing.Size(60, 21);
            this.comboBoxTag.Sorted = true;
            this.comboBoxTag.TabIndex = 41;
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
            // comboBoxType
            // 
            this.comboBoxType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBoxType.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Location = new System.Drawing.Point(185, 130);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(101, 21);
            this.comboBoxType.Sorted = true;
            this.comboBoxType.TabIndex = 40;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label21.Location = new System.Drawing.Point(225, 403);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(49, 13);
            this.label21.TabIndex = 17;
            this.label21.Text = "Servidor:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label22.Location = new System.Drawing.Point(64, 422);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(35, 13);
            this.label22.TabIndex = 18;
            this.label22.Text = "Porta:";
            // 
            // collectButton
            // 
            this.collectButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.collectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.collectButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.collectButton.Location = new System.Drawing.Point(575, 643);
            this.collectButton.Name = "collectButton";
            this.collectButton.Size = new System.Drawing.Size(186, 25);
            this.collectButton.TabIndex = 51;
            this.collectButton.Text = "Coletar Novamente";
            this.collectButton.UseVisualStyleBackColor = false;
            this.collectButton.Click += new System.EventHandler(this.coleta_Click);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label23.Location = new System.Drawing.Point(323, 79);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(90, 13);
            this.label23.TabIndex = 55;
            this.label23.Text = "Letra (se houver):";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label24.Location = new System.Drawing.Point(37, 430);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(115, 13);
            this.label24.TabIndex = 56;
            this.label24.Text = "Versão da BIOS/UEFI:";
            // 
            // lblBIOS
            // 
            this.lblBIOS.AutoSize = true;
            this.lblBIOS.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblBIOS.Location = new System.Drawing.Point(197, 430);
            this.lblBIOS.Name = "lblBIOS";
            this.lblBIOS.Size = new System.Drawing.Size(10, 13);
            this.lblBIOS.TabIndex = 57;
            this.lblBIOS.Text = "-";
            // 
            // accessSystemButton
            // 
            this.accessSystemButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.accessSystemButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.accessSystemButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.accessSystemButton.Location = new System.Drawing.Point(575, 674);
            this.accessSystemButton.Name = "accessSystemButton";
            this.accessSystemButton.Size = new System.Drawing.Size(186, 25);
            this.accessSystemButton.TabIndex = 52;
            this.accessSystemButton.Text = "Acessar sistema de patrimônios";
            this.accessSystemButton.UseVisualStyleBackColor = false;
            this.accessSystemButton.Click += new System.EventHandler(this.accessButton_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label25.Location = new System.Drawing.Point(37, 403);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(88, 13);
            this.label25.TabIndex = 62;
            this.label25.Text = "Tipo de firmware:";
            // 
            // lblBIOSType
            // 
            this.lblBIOSType.AutoSize = true;
            this.lblBIOSType.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblBIOSType.Location = new System.Drawing.Point(197, 403);
            this.lblBIOSType.Name = "lblBIOSType";
            this.lblBIOSType.Size = new System.Drawing.Size(10, 13);
            this.lblBIOSType.TabIndex = 63;
            this.lblBIOSType.Text = "-";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.configurableQualityPictureBox33);
            this.groupBox1.Controls.Add(this.configurableQualityPictureBox32);
            this.groupBox1.Controls.Add(this.lblSmart);
            this.groupBox1.Controls.Add(this.lblTPM);
            this.groupBox1.Controls.Add(this.label44);
            this.groupBox1.Controls.Add(this.configurableQualityPictureBox30);
            this.groupBox1.Controls.Add(this.label45);
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
            this.groupBox1.Size = new System.Drawing.Size(537, 586);
            this.groupBox1.TabIndex = 65;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dados do computador";
            // 
            // configurableQualityPictureBox33
            // 
            this.configurableQualityPictureBox33.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox33.Image = global::HardwareInformation.Properties.Resources.tpm_white;
            this.configurableQualityPictureBox33.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox33.Location = new System.Drawing.Point(7, 503);
            this.configurableQualityPictureBox33.Name = "configurableQualityPictureBox33";
            this.configurableQualityPictureBox33.Size = new System.Drawing.Size(25, 25);
            this.configurableQualityPictureBox33.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.configurableQualityPictureBox33.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.configurableQualityPictureBox33.TabIndex = 110;
            this.configurableQualityPictureBox33.TabStop = false;
            // 
            // configurableQualityPictureBox32
            // 
            this.configurableQualityPictureBox32.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox32.Image = global::HardwareInformation.Properties.Resources.smart_white;
            this.configurableQualityPictureBox32.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox32.Location = new System.Drawing.Point(7, 179);
            this.configurableQualityPictureBox32.Name = "configurableQualityPictureBox32";
            this.configurableQualityPictureBox32.Size = new System.Drawing.Size(25, 25);
            this.configurableQualityPictureBox32.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.configurableQualityPictureBox32.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.configurableQualityPictureBox32.TabIndex = 107;
            this.configurableQualityPictureBox32.TabStop = false;
            // 
            // lblSmart
            // 
            this.lblSmart.AutoSize = true;
            this.lblSmart.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblSmart.Location = new System.Drawing.Point(197, 187);
            this.lblSmart.Name = "lblSmart";
            this.lblSmart.Size = new System.Drawing.Size(10, 13);
            this.lblSmart.TabIndex = 106;
            this.lblSmart.Text = "-";
            // 
            // lblTPM
            // 
            this.lblTPM.AutoSize = true;
            this.lblTPM.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTPM.Location = new System.Drawing.Point(197, 511);
            this.lblTPM.Name = "lblTPM";
            this.lblTPM.Size = new System.Drawing.Size(10, 13);
            this.lblTPM.TabIndex = 109;
            this.lblTPM.Text = "-";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label44.Location = new System.Drawing.Point(37, 187);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(96, 13);
            this.label44.TabIndex = 105;
            this.label44.Text = "Status S.M.A.R.T.:";
            // 
            // configurableQualityPictureBox30
            // 
            this.configurableQualityPictureBox30.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox30.Image = global::HardwareInformation.Properties.Resources.VT_x_white;
            this.configurableQualityPictureBox30.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox30.Location = new System.Drawing.Point(7, 476);
            this.configurableQualityPictureBox30.Name = "configurableQualityPictureBox30";
            this.configurableQualityPictureBox30.Size = new System.Drawing.Size(25, 25);
            this.configurableQualityPictureBox30.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.configurableQualityPictureBox30.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.configurableQualityPictureBox30.TabIndex = 104;
            this.configurableQualityPictureBox30.TabStop = false;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label45.Location = new System.Drawing.Point(37, 511);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(121, 13);
            this.label45.TabIndex = 108;
            this.label45.Text = "Versão do módulo TPM:";
            // 
            // lblVT
            // 
            this.lblVT.AutoSize = true;
            this.lblVT.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblVT.Location = new System.Drawing.Point(197, 484);
            this.lblVT.Name = "lblVT";
            this.lblVT.Size = new System.Drawing.Size(10, 13);
            this.lblVT.TabIndex = 103;
            this.lblVT.Text = "-";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label33.Location = new System.Drawing.Point(37, 484);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(141, 13);
            this.label33.TabIndex = 102;
            this.label33.Text = "Tecnologia de Virtualização:";
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
            this.configurableQualityPictureBox17.Location = new System.Drawing.Point(7, 449);
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
            this.configurableQualityPictureBox16.Location = new System.Drawing.Point(7, 422);
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
            this.configurableQualityPictureBox15.Location = new System.Drawing.Point(7, 395);
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
            this.configurableQualityPictureBox14.Location = new System.Drawing.Point(7, 368);
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
            this.configurableQualityPictureBox13.Location = new System.Drawing.Point(7, 341);
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
            this.configurableQualityPictureBox12.Location = new System.Drawing.Point(7, 314);
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
            this.configurableQualityPictureBox11.Location = new System.Drawing.Point(7, 287);
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
            this.configurableQualityPictureBox10.Location = new System.Drawing.Point(7, 260);
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
            this.configurableQualityPictureBox9.Location = new System.Drawing.Point(7, 233);
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
            this.configurableQualityPictureBox8.Location = new System.Drawing.Point(7, 206);
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
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.BackColor = System.Drawing.Color.Transparent;
            this.label28.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label28.Location = new System.Drawing.Point(255, 528);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(10, 13);
            this.label28.TabIndex = 70;
            this.label28.Text = "-";
            // 
            // lblSecBoot
            // 
            this.lblSecBoot.AutoSize = true;
            this.lblSecBoot.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblSecBoot.Location = new System.Drawing.Point(197, 457);
            this.lblSecBoot.Name = "lblSecBoot";
            this.lblSecBoot.Size = new System.Drawing.Size(10, 13);
            this.lblSecBoot.TabIndex = 71;
            this.lblSecBoot.Text = "-";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label32.Location = new System.Drawing.Point(37, 457);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(69, 13);
            this.label32.TabIndex = 70;
            this.label32.Text = "Secure Boot:";
            // 
            // lblMediaOperation
            // 
            this.lblMediaOperation.AutoSize = true;
            this.lblMediaOperation.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMediaOperation.Location = new System.Drawing.Point(197, 241);
            this.lblMediaOperation.Name = "lblMediaOperation";
            this.lblMediaOperation.Size = new System.Drawing.Size(10, 13);
            this.lblMediaOperation.TabIndex = 69;
            this.lblMediaOperation.Text = "-";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label30.Location = new System.Drawing.Point(37, 241);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(154, 13);
            this.label30.TabIndex = 68;
            this.label30.Text = "Modo de operação SATA/M.2:";
            // 
            // lblGPUInfo
            // 
            this.lblGPUInfo.AutoSize = true;
            this.lblGPUInfo.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblGPUInfo.Location = new System.Drawing.Point(197, 268);
            this.lblGPUInfo.Name = "lblGPUInfo";
            this.lblGPUInfo.Size = new System.Drawing.Size(10, 13);
            this.lblGPUInfo.TabIndex = 67;
            this.lblGPUInfo.Text = "-";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label29.Location = new System.Drawing.Point(37, 268);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(84, 13);
            this.label29.TabIndex = 66;
            this.label29.Text = "Placa de Vídeo:";
            // 
            // lblMediaType
            // 
            this.lblMediaType.AutoSize = true;
            this.lblMediaType.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMediaType.Location = new System.Drawing.Point(197, 214);
            this.lblMediaType.Name = "lblMediaType";
            this.lblMediaType.Size = new System.Drawing.Size(10, 13);
            this.lblMediaType.TabIndex = 65;
            this.lblMediaType.Text = "-";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label27.Location = new System.Drawing.Point(37, 214);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(124, 13);
            this.label27.TabIndex = 64;
            this.label27.Text = "Tipo de armazenamento:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(6, 544);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(525, 36);
            this.progressBar1.TabIndex = 69;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label52);
            this.groupBox2.Controls.Add(this.label53);
            this.groupBox2.Controls.Add(this.label51);
            this.groupBox2.Controls.Add(this.label50);
            this.groupBox2.Controls.Add(this.label49);
            this.groupBox2.Controls.Add(this.label48);
            this.groupBox2.Controls.Add(this.label47);
            this.groupBox2.Controls.Add(this.configurableQualityPictureBox35);
            this.groupBox2.Controls.Add(this.label31);
            this.groupBox2.Controls.Add(this.textBoxTicket);
            this.groupBox2.Controls.Add(this.comboBoxBattery);
            this.groupBox2.Controls.Add(this.configurableQualityPictureBox34);
            this.groupBox2.Controls.Add(this.label42);
            this.groupBox2.Controls.Add(this.label41);
            this.groupBox2.Controls.Add(this.label46);
            this.groupBox2.Controls.Add(this.label40);
            this.groupBox2.Controls.Add(this.label39);
            this.groupBox2.Controls.Add(this.label38);
            this.groupBox2.Controls.Add(this.label37);
            this.groupBox2.Controls.Add(this.label36);
            this.groupBox2.Controls.Add(this.label35);
            this.groupBox2.Controls.Add(this.studentRadioButton);
            this.groupBox2.Controls.Add(this.employeeRadioButton);
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
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.textBoxPatrimony);
            this.groupBox2.Controls.Add(this.textBoxSeal);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.textBoxRoom);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.comboBoxBuilding);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.comboBoxActiveDirectory);
            this.groupBox2.Controls.Add(this.comboBoxType);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.textBoxLetter);
            this.groupBox2.Controls.Add(this.comboBoxStandard);
            this.groupBox2.Controls.Add(this.comboBoxTag);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.comboBoxInUse);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox2.Location = new System.Drawing.Point(575, 113);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(450, 447);
            this.groupBox2.TabIndex = 66;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Dados do patrimônio, manutenção e de localização";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label52.Location = new System.Drawing.Point(275, 422);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(10, 13);
            this.label52.TabIndex = 123;
            this.label52.Text = "-";
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label53.Location = new System.Drawing.Point(225, 422);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(44, 13);
            this.label53.TabIndex = 122;
            this.label53.Text = "Agente:";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label51.Location = new System.Drawing.Point(99, 422);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(10, 13);
            this.label51.TabIndex = 121;
            this.label51.Text = "-";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label50.Location = new System.Drawing.Point(99, 403);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(10, 13);
            this.label50.TabIndex = 120;
            this.label50.Text = "-";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label49.Location = new System.Drawing.Point(64, 403);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(20, 13);
            this.label49.TabIndex = 119;
            this.label49.Text = "IP:";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.label48.Location = new System.Drawing.Point(367, 252);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(17, 13);
            this.label48.TabIndex = 118;
            this.label48.Text = "✱";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.label47.Location = new System.Drawing.Point(143, 252);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(17, 13);
            this.label47.TabIndex = 114;
            this.label47.Text = "✱";
            // 
            // configurableQualityPictureBox35
            // 
            this.configurableQualityPictureBox35.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox35.Image = global::HardwareInformation.Properties.Resources.ticket_white;
            this.configurableQualityPictureBox35.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox35.Location = new System.Drawing.Point(274, 244);
            this.configurableQualityPictureBox35.Name = "configurableQualityPictureBox35";
            this.configurableQualityPictureBox35.Size = new System.Drawing.Size(25, 25);
            this.configurableQualityPictureBox35.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.configurableQualityPictureBox35.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.configurableQualityPictureBox35.TabIndex = 117;
            this.configurableQualityPictureBox35.TabStop = false;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label31.Location = new System.Drawing.Point(303, 251);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(69, 13);
            this.label31.TabIndex = 116;
            this.label31.Text = "Nº chamado:";
            // 
            // textBoxTicket
            // 
            this.textBoxTicket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.textBoxTicket.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxTicket.Location = new System.Drawing.Point(384, 249);
            this.textBoxTicket.MaxLength = 6;
            this.textBoxTicket.Name = "textBoxTicket";
            this.textBoxTicket.Size = new System.Drawing.Size(60, 20);
            this.textBoxTicket.TabIndex = 115;
            this.textBoxTicket.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumbersOnly_KeyPress);
            // 
            // comboBoxBattery
            // 
            this.comboBoxBattery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxBattery.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBattery.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBoxBattery.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxBattery.FormattingEnabled = true;
            this.comboBoxBattery.Location = new System.Drawing.Point(185, 248);
            this.comboBoxBattery.Name = "comboBoxBattery";
            this.comboBoxBattery.Size = new System.Drawing.Size(84, 21);
            this.comboBoxBattery.Sorted = true;
            this.comboBoxBattery.TabIndex = 114;
            // 
            // configurableQualityPictureBox34
            // 
            this.configurableQualityPictureBox34.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox34.Image = global::HardwareInformation.Properties.Resources.cmos_battery_white;
            this.configurableQualityPictureBox34.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox34.Location = new System.Drawing.Point(6, 244);
            this.configurableQualityPictureBox34.Name = "configurableQualityPictureBox34";
            this.configurableQualityPictureBox34.Size = new System.Drawing.Size(25, 25);
            this.configurableQualityPictureBox34.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.configurableQualityPictureBox34.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.configurableQualityPictureBox34.TabIndex = 113;
            this.configurableQualityPictureBox34.TabStop = false;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.label42.Location = new System.Drawing.Point(103, 187);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(17, 13);
            this.label42.TabIndex = 112;
            this.label42.Text = "✱";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.label41.Location = new System.Drawing.Point(367, 133);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(17, 13);
            this.label41.TabIndex = 111;
            this.label41.Text = "✱";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label46.Location = new System.Drawing.Point(35, 251);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(112, 13);
            this.label46.TabIndex = 111;
            this.label46.Text = "Houve troca de pilha?";
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
            this.label39.Location = new System.Drawing.Point(363, 106);
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
            this.label35.Location = new System.Drawing.Point(253, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(152, 13);
            this.label35.TabIndex = 105;
            this.label35.Text = "✱ = Preenchimento obrigatório";
            // 
            // studentRadioButton
            // 
            this.studentRadioButton.AutoSize = true;
            this.studentRadioButton.Location = new System.Drawing.Point(185, 197);
            this.studentRadioButton.Name = "studentRadioButton";
            this.studentRadioButton.Size = new System.Drawing.Size(246, 17);
            this.studentRadioButton.TabIndex = 44;
            this.studentRadioButton.TabStop = true;
            this.studentRadioButton.Text = "Aluno (computador de laboratório/sala de aula)";
            this.studentRadioButton.UseVisualStyleBackColor = true;
            this.studentRadioButton.CheckedChanged += new System.EventHandler(this.studentButton2_CheckedChanged);
            // 
            // employeeRadioButton
            // 
            this.employeeRadioButton.AutoSize = true;
            this.employeeRadioButton.Location = new System.Drawing.Point(185, 179);
            this.employeeRadioButton.Name = "employeeRadioButton";
            this.employeeRadioButton.Size = new System.Drawing.Size(242, 17);
            this.employeeRadioButton.TabIndex = 43;
            this.employeeRadioButton.TabStop = true;
            this.employeeRadioButton.Text = "Funcionário/Bolsista (computador de trabalho)";
            this.employeeRadioButton.UseVisualStyleBackColor = true;
            this.employeeRadioButton.CheckedChanged += new System.EventHandler(this.employeeButton1_CheckedChanged);
            // 
            // configurableQualityPictureBox31
            // 
            this.configurableQualityPictureBox31.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox31.Image = global::HardwareInformation.Properties.Resources.who_white;
            this.configurableQualityPictureBox31.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox31.Location = new System.Drawing.Point(6, 179);
            this.configurableQualityPictureBox31.Name = "configurableQualityPictureBox31";
            this.configurableQualityPictureBox31.Size = new System.Drawing.Size(25, 25);
            this.configurableQualityPictureBox31.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.configurableQualityPictureBox31.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.configurableQualityPictureBox31.TabIndex = 102;
            this.configurableQualityPictureBox31.TabStop = false;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label34.Location = new System.Drawing.Point(35, 187);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(70, 13);
            this.label34.TabIndex = 101;
            this.label34.Text = "Quem usará?";
            // 
            // configurableQualityPictureBox25
            // 
            this.configurableQualityPictureBox25.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox25.Image = global::HardwareInformation.Properties.Resources.letter_white;
            this.configurableQualityPictureBox25.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox25.Location = new System.Drawing.Point(292, 71);
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
            this.configurableQualityPictureBox29.Location = new System.Drawing.Point(6, 399);
            this.configurableQualityPictureBox29.Name = "configurableQualityPictureBox29";
            this.configurableQualityPictureBox29.Size = new System.Drawing.Size(43, 40);
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
            this.configurableQualityPictureBox27.Location = new System.Drawing.Point(292, 125);
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
            this.configurableQualityPictureBox26.Location = new System.Drawing.Point(292, 98);
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
            this.configurableQualityPictureBox23.Location = new System.Drawing.Point(274, 216);
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
            this.configurableQualityPictureBox22.Location = new System.Drawing.Point(6, 216);
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
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePicker1.Location = new System.Drawing.Point(185, 157);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(259, 20);
            this.dateTimePicker1.TabIndex = 42;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.BackColor = System.Drawing.Color.Transparent;
            this.label26.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label26.Location = new System.Drawing.Point(275, 403);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(10, 13);
            this.label26.TabIndex = 72;
            this.label26.Text = "-";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblMaintenanceSince);
            this.groupBox3.Controls.Add(this.lblInstallSince);
            this.groupBox3.Controls.Add(this.label43);
            this.groupBox3.Controls.Add(this.textBox5);
            this.groupBox3.Controls.Add(this.textBox6);
            this.groupBox3.Controls.Add(this.formatButton);
            this.groupBox3.Controls.Add(this.maintenanceButton);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox3.Location = new System.Drawing.Point(6, 275);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(438, 118);
            this.groupBox3.TabIndex = 72;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tipo de serviço";
            // 
            // lblMaintenanceSince
            // 
            this.lblMaintenanceSince.AutoSize = true;
            this.lblMaintenanceSince.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblMaintenanceSince.Location = new System.Drawing.Point(101, 61);
            this.lblMaintenanceSince.Name = "lblMaintenanceSince";
            this.lblMaintenanceSince.Size = new System.Drawing.Size(10, 13);
            this.lblMaintenanceSince.TabIndex = 121;
            this.lblMaintenanceSince.Text = "-";
            // 
            // lblInstallSince
            // 
            this.lblInstallSince.AutoSize = true;
            this.lblInstallSince.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblInstallSince.Location = new System.Drawing.Point(101, 22);
            this.lblInstallSince.Name = "lblInstallSince";
            this.lblInstallSince.Size = new System.Drawing.Size(10, 13);
            this.lblInstallSince.TabIndex = 120;
            this.lblInstallSince.Text = "-";
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
            this.textBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox5.Enabled = false;
            this.textBox5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox5.Location = new System.Drawing.Point(29, 38);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(391, 19);
            this.textBox5.TabIndex = 76;
            this.textBox5.Text = "Opção para quando o PC passar por formatação ou reset";
            // 
            // textBox6
            // 
            this.textBox6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox6.Enabled = false;
            this.textBox6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox6.Location = new System.Drawing.Point(29, 78);
            this.textBox6.Multiline = true;
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(391, 25);
            this.textBox6.TabIndex = 77;
            this.textBox6.Text = "Opção para quando o PC passar por manutenção preventiva, sem a necessidade de for" +
    "matação ou reset";
            // 
            // formatButton
            // 
            this.formatButton.AutoSize = true;
            this.formatButton.Location = new System.Drawing.Point(10, 20);
            this.formatButton.Name = "formatButton";
            this.formatButton.Size = new System.Drawing.Size(81, 17);
            this.formatButton.TabIndex = 47;
            this.formatButton.Text = "Formatação";
            this.formatButton.UseVisualStyleBackColor = true;
            this.formatButton.CheckedChanged += new System.EventHandler(this.formatButton1_CheckedChanged);
            // 
            // maintenanceButton
            // 
            this.maintenanceButton.AutoSize = true;
            this.maintenanceButton.Location = new System.Drawing.Point(10, 59);
            this.maintenanceButton.Name = "maintenanceButton";
            this.maintenanceButton.Size = new System.Drawing.Size(85, 17);
            this.maintenanceButton.TabIndex = 48;
            this.maintenanceButton.Text = "Manutenção";
            this.maintenanceButton.UseVisualStyleBackColor = true;
            this.maintenanceButton.CheckedChanged += new System.EventHandler(this.maintenanceButton2_CheckedChanged);
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
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comboBoxTheme,
            this.logoutLabel,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 701);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1056, 24);
            this.statusStrip1.TabIndex = 60;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // comboBoxTheme
            // 
            this.comboBoxTheme.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
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
            // logoutLabel
            // 
            this.logoutLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.logoutLabel.Name = "logoutLabel";
            this.logoutLabel.Size = new System.Drawing.Size(45, 19);
            this.logoutLabel.Text = "Logout";
            this.logoutLabel.Click += new System.EventHandler(this.logoutLabel_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.toolStripStatusLabel1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)(((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(944, 19);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "Sistema desenvolvido pelo servidor Kevin Costa, SIAPE 1971957, para uso no serviç" +
    "o da Subdivisão de Tecnologia da Informação do CCSH - UFSM";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.webView2);
            this.groupBox4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox4.Location = new System.Drawing.Point(575, 562);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(450, 77);
            this.groupBox4.TabIndex = 73;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Status do cadastro";
            // 
            // webView2
            // 
            this.webView2.AllowExternalDrop = true;
            this.webView2.CreationProperties = null;
            this.webView2.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView2.Location = new System.Drawing.Point(1, 13);
            this.webView2.Name = "webView2";
            this.webView2.Size = new System.Drawing.Size(448, 62);
            this.webView2.TabIndex = 0;
            this.webView2.ZoomFactor = 1D;
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
            this.configurableQualityPictureBox1.Size = new System.Drawing.Size(1061, 109);
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
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(1056, 725);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.configurableQualityPictureBox1);
            this.Controls.Add(this.accessSystemButton);
            this.Controls.Add(this.collectButton);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.registerButton);
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
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox33)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox32)).EndInit();
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
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox35)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox34)).EndInit();
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
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).EndInit();
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
        private TextBox textBoxPatrimony;
        private TextBox textBoxSeal;
        private TextBox textBoxRoom;
        private TextBox textBoxLetter;
        private Label label14;
        private Label label15;
        private Label label16;
        private ComboBox comboBoxBuilding;
        private ComboBox comboBoxActiveDirectory;
        private Label lblOS;
        private Label label17;
        private ComboBox comboBoxStandard;
        private Label label18;
        private ComboBox comboBoxInUse;
        private Label label19;
        private ComboBox comboBoxTag;
        private Button registerButton;
        private Label label20;
        private ComboBox comboBoxType;
        private Label label21;
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
        private Button collectButton;
        private Label label23;
        private Label label24;
        private Label lblBIOS;
        private Button accessSystemButton;
        private ProgressBar progressBar1;
        private Label label28;
        private Label lblSecBoot;
        private Label label32;
        private WebView2 webView2;
        private RadioButton maintenanceButton;
        private RadioButton formatButton;
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
        private RadioButton studentRadioButton;
        private RadioButton employeeRadioButton;
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
        private ConfigurableQualityPictureBox configurableQualityPictureBox32;
        private Label lblSmart;
        private Label label44;
        private Timer timer8;
        private Label label22;
        private ConfigurableQualityPictureBox configurableQualityPictureBox33;
        private Label lblTPM;
        private Label label45;
        private GroupBox groupBox4;
        private ComboBox comboBoxBattery;
        private ConfigurableQualityPictureBox configurableQualityPictureBox34;
        private Label label46;
        private ConfigurableQualityPictureBox configurableQualityPictureBox35;
        private Label label31;
        private TextBox textBoxTicket;
        private Label label48;
        private Label label47;
        private Label label49;
        private Label lblMaintenanceSince;
        private Label lblInstallSince;
        private BusyWindow bw;

        private bool pass = true;
        private bool themeBool, serverOnline, offlineMode;
        private List<string> date;
        private string servidor_web, porta, modeURL, user, ip, port;
        private string BM, Model, SerialNo, ProcName, PM, HDSize, MediaType,
           MediaOperation, GPUInfo, OS, Hostname, Mac, IP, BIOS, BIOSType, SecBoot, VT, Smart, TPM,
            InstallLabel, MaintenanceLabel;
        private string[] sArgs = new string[34];
        private Label label51;
        private Label label50;
        private Label label52;
        private Label label53;
        private ToolStripStatusLabel logoutLabel;
        private int i = 0;

        //Sets service mode to format
        private void formatButton1_CheckedChanged(object sender, EventArgs e)
        {
            modeURL = StringsAndConstants.formatURL;
        }

        //Sets service mode to maintenance
        private void maintenanceButton2_CheckedChanged(object sender, EventArgs e)
        {
            modeURL = StringsAndConstants.maintenanceURL;
        }

        //Sets service to employee
        private void employeeButton1_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxActiveDirectory.SelectedIndex = 0;
            comboBoxStandard.SelectedIndex = 1;
        }

        //Sets service to student
        private void studentButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxActiveDirectory.SelectedIndex = 1;
            comboBoxStandard.SelectedIndex = 0;
        }

        private void comboBoxThemeInit()
        {
            themeBool = MiscMethods.ThemeInit();
            if (themeBool)
                darkTheme();
            else
                lightTheme();
        }

        private void logoutLabel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        //Method for setting the auto theme
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            comboBoxThemeInit();
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

        //Sets a light theme for the UI
        private void lightTheme()
        {
            this.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.lblBM.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblModel.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblSerialNo.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblProcName.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblPM.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblHDSize.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblMediaType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblMediaOperation.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblOS.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblHostname.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblMac.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblIP.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label3.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label4.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label5.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label6.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label7.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label8.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label9.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label10.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label11.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label12.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label13.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBoxPatrimony.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxPatrimony.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBoxSeal.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxSeal.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBoxRoom.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxRoom.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBoxLetter.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxLetter.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label14.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label15.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label16.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxBuilding.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxBuilding.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxActiveDirectory.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxActiveDirectory.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label17.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxStandard.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxStandard.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.registerButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.registerButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label18.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxInUse.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxInUse.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label19.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxTag.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxTag.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label20.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxType.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label21.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxBattery.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxBattery.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label22.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.collectButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.collectButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label23.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label24.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblBIOS.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.accessSystemButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.accessSystemButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label25.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblBIOSType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.groupBox1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.groupBox2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripStatusLabel2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripStatusLabel2.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.statusStrip1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.toolStripStatusLabel1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripStatusLabel1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.logoutLabel.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.logoutLabel.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.comboBoxTheme.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.comboBoxTheme.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label27.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label29.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label28.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label30.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblTPM.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblVT.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblSecBoot.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label31.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label32.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label33.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label34.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label35.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            this.label36.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            this.label37.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            this.label38.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            this.label39.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            this.label40.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            this.label41.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            this.label42.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            this.label43.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            this.label44.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label45.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label46.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label47.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            this.label48.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            this.label49.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label50.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label51.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label52.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label53.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblInstallSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            this.lblMaintenanceSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            this.lblGPUInfo.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblVT.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.lblSmart.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.groupBox3.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.groupBox4.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBox5.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBox5.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.textBox6.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBox6.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.textBoxTicket.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBoxTicket.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.toolStripMenuItem1.BackgroundImage = global::HardwareInformation.Properties.Resources.lightback;
            this.toolStripMenuItem1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripMenuItem2.BackgroundImage = global::HardwareInformation.Properties.Resources.lightback;
            this.toolStripMenuItem2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripMenuItem3.BackgroundImage = global::HardwareInformation.Properties.Resources.lightback;
            this.toolStripMenuItem3.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
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
            this.configurableQualityPictureBox32.Image = global::HardwareInformation.Properties.Resources.smart_black;
            this.configurableQualityPictureBox33.Image = global::HardwareInformation.Properties.Resources.tpm_black;
            this.configurableQualityPictureBox34.Image = global::HardwareInformation.Properties.Resources.cmos_battery_black;
            this.configurableQualityPictureBox35.Image = global::HardwareInformation.Properties.Resources.ticket_black;
            if (offlineMode)
            {
                this.label50.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                this.label51.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                this.label52.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }
        }

        //Sets a dark theme for the UI
        private void darkTheme()
        {
            this.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.lblBM.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblModel.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblSerialNo.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblProcName.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblPM.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblHDSize.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblMediaType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblMediaOperation.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblOS.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblHostname.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblMac.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblIP.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label3.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label4.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label5.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label6.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label7.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label8.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label9.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label10.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label11.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label12.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label13.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBoxPatrimony.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.textBoxPatrimony.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBoxSeal.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.textBoxSeal.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBoxRoom.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.textBoxRoom.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBoxLetter.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.textBoxLetter.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label14.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label15.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label16.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxBuilding.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxBuilding.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxActiveDirectory.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxActiveDirectory.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label17.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxStandard.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxStandard.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.registerButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.registerButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label18.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxInUse.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxInUse.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label19.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxTag.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxTag.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label20.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxType.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label21.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxBattery.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxBattery.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label22.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.collectButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.collectButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label23.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label24.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblBIOS.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.accessSystemButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.accessSystemButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label25.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblBIOSType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.groupBox1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.groupBox2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripStatusLabel2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripStatusLabel2.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.statusStrip1.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.toolStripStatusLabel1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripStatusLabel1.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.logoutLabel.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.logoutLabel.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.comboBoxTheme.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.comboBoxTheme.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label27.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label28.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label29.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label30.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblTPM.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblVT.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblSecBoot.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label31.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label32.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label33.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label34.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label35.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            this.label36.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            this.label37.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            this.label38.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            this.label39.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            this.label40.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            this.label41.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            this.label42.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            this.label43.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            this.label44.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label45.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label46.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label47.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            this.label48.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            this.label49.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label50.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label51.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label52.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label53.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblInstallSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            this.lblMaintenanceSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            this.lblGPUInfo.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblVT.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.lblSmart.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.groupBox3.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.groupBox4.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBox5.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBox5.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.textBox6.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBox6.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.textBoxTicket.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBoxTicket.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.toolStripMenuItem1.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
            this.toolStripMenuItem1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripMenuItem2.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
            this.toolStripMenuItem2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripMenuItem3.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
            this.toolStripMenuItem3.ForeColor = StringsAndConstants.DARK_FORECOLOR;
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
            this.configurableQualityPictureBox32.Image = global::HardwareInformation.Properties.Resources.smart_white;
            this.configurableQualityPictureBox33.Image = global::HardwareInformation.Properties.Resources.tpm_white;
            this.configurableQualityPictureBox34.Image = global::HardwareInformation.Properties.Resources.cmos_battery_white;
            this.configurableQualityPictureBox35.Image = global::HardwareInformation.Properties.Resources.ticket_white;
            if (offlineMode)
            {
                this.label50.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                this.label51.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                this.label52.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }
        }

        //Opens the selected webpage, according to the IP and port specified in the comboboxes
        private void accessButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://" + ip + ":" + port);
        }

        //Handles the closing of the current form
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.Delete(StringsAndConstants.biosPath);
            File.Delete(StringsAndConstants.loginPath);
            webView2.Dispose();
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        //Loads the form, sets some combobox values, create two timers (1000 ms cadence), and triggers a hardware collection
        private async void Form1_Load(object sender, EventArgs e)
        {
            comboBoxThemeInit();
            if (!offlineMode)
            {
                bw = new BusyWindow();
                logoutLabel.Enabled = false;
                bw.Visible = true;
                await loadWebView2();
                bw.Visible = false;
                logoutLabel.Enabled = true;
            }            
            timer1.Tick += new EventHandler(flashTextHostname);
            timer2.Tick += new EventHandler(flashTextMediaOp);
            timer3.Tick += new EventHandler(flashTextSecBoot);
            timer4.Tick += new EventHandler(flashTextBIOSVersion);
            timer5.Tick += new EventHandler(flashTextNetConnectivity);
            timer6.Tick += new EventHandler(flashTextBIOSType);
            timer7.Tick += new EventHandler(flashTextVT);
            timer8.Tick += new EventHandler(flashTextSmart);
            timer1.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer2.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer3.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer4.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer5.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer6.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer7.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer8.Interval = StringsAndConstants.TIMER_INTERVAL;
            label50.Text = ip;
            label51.Text = port;
            label52.Text = this.user.ToUpper();
            dateTimePicker1.MaxDate = DateTime.Today;
            date = new List<string>();
            FormClosing += Form1_FormClosing;
            coleta_Click(sender, e);            
        }

        //Restricts textbox4 only with chars
        private void textBoxCharsOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back))
                e.Handled = true;
        }

        //Restricts textbox only with numbers
        private void textBoxNumbersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        //Sets the Hostname label to flash in red
        private void flashTextHostname(Object myObject, EventArgs myEventArgs)
        {
            if (lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblHostname.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            else if (lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblHostname.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            else
                lblHostname.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the MediaOperations label to flash in red
        private void flashTextMediaOp(Object myobject, EventArgs myEventArgs)
        {
            if (lblMediaOperation.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblMediaOperation.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            else if (lblMediaOperation.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblMediaOperation.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            else
                lblMediaOperation.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the Secure Boot label to flash in red
        private void flashTextSecBoot(Object myobject, EventArgs myEventArgs)
        {
            if (lblSecBoot.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblSecBoot.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            else if (lblSecBoot.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblSecBoot.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            else
                lblSecBoot.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the VT label to flash in red
        private void flashTextVT(Object myobject, EventArgs myEventArgs)
        {
            if (lblVT.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblVT.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            else if (lblVT.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblVT.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            else
                lblVT.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the SMART label to flash in red
        private void flashTextSmart(Object myobject, EventArgs myEventArgs)
        {
            if (lblSmart.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblSmart.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            else if (lblSmart.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblSmart.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            else
                lblSmart.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the BIOS Version label to flash in red
        private void flashTextBIOSVersion(Object myobject, EventArgs myEventArgs)
        {
            if (lblBIOS.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblBIOS.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            else if (lblBIOS.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblBIOS.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            else
                lblBIOS.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the Mac and IP labels to flash in red
        private void flashTextNetConnectivity(Object myobject, EventArgs myEventArgs)
        {
            if (lblMac.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
            {
                lblMac.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                lblIP.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            else if (lblMac.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
            {
                lblMac.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                lblIP.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            else
            {
                lblMac.ForeColor = StringsAndConstants.ALERT_COLOR;
                lblIP.ForeColor = StringsAndConstants.ALERT_COLOR;
            }
        }

        //Sets the BIOS Firmware Type label to flash in red
        private void flashTextBIOSType(Object myobject, EventArgs myEventArgs)
        {
            if (lblBIOSType.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblBIOSType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            else if (lblBIOSType.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblBIOSType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            else
                lblBIOSType.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Starts the collection process
        private void collecting()
        {
            if (!offlineMode)
            {
                servidor_web = ip;
                porta = port;
                serverOnline = BIOSFileReader.checkHost(servidor_web, porta);
                if (serverOnline && porta != "")
                {
                    label26.Text = StringsAndConstants.ONLINE;
                    label26.ForeColor = StringsAndConstants.ONLINE_ALERT;
                }
                else
                {
                    label26.Text = StringsAndConstants.OFFLINE;
                    label26.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                }
            }
            else
            {
                label50.Text = label51.Text = label52.Text = label26.Text = StringsAndConstants.OFFLINE_MODE_ACTIVATED;
                label50.ForeColor = label51.ForeColor = label52.ForeColor = label26.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }

            //Stops blinking and resets red color
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
            timer4.Enabled = false;
            timer5.Enabled = false;
            timer6.Enabled = false;
            timer7.Enabled = false;
            timer8.Enabled = false;
            if (lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR || lblMediaOperation.ForeColor == StringsAndConstants.ALERT_COLOR || lblSecBoot.ForeColor == StringsAndConstants.ALERT_COLOR || lblBIOS.ForeColor == StringsAndConstants.ALERT_COLOR || lblVT.ForeColor == StringsAndConstants.ALERT_COLOR || lblSmart.ForeColor == StringsAndConstants.ALERT_COLOR)
            {
                if (themeBool)
                {
                    lblHostname.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    lblMediaOperation.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    lblSecBoot.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    lblBIOS.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    lblVT.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    lblSmart.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                else
                {
                    lblHostname.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    lblMediaOperation.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    lblSecBoot.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    lblBIOS.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    lblVT.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    lblSmart.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
            }

            //Sets current and maximum values for the progressbar
            progressBar1.Maximum = 19;
            progressBar1.Value = 0;

            //Writes 'Coletando...' in the labels, while collects data
            lblInstallSince.Text = StringsAndConstants.FETCHING;
            lblMaintenanceSince.Text = StringsAndConstants.FETCHING;
            lblBM.Text = StringsAndConstants.FETCHING;
            lblModel.Text = StringsAndConstants.FETCHING;
            lblSerialNo.Text = StringsAndConstants.FETCHING;
            lblProcName.Text = StringsAndConstants.FETCHING;
            lblPM.Text = StringsAndConstants.FETCHING;
            lblHDSize.Text = StringsAndConstants.FETCHING;
            lblSmart.Text = StringsAndConstants.FETCHING;
            lblMediaType.Text = StringsAndConstants.FETCHING;
            lblMediaOperation.Text = StringsAndConstants.FETCHING;
            lblGPUInfo.Text = StringsAndConstants.FETCHING;
            lblOS.Text = StringsAndConstants.FETCHING;
            lblHostname.Text = StringsAndConstants.FETCHING;
            lblMac.Text = StringsAndConstants.FETCHING;
            lblIP.Text = StringsAndConstants.FETCHING;
            lblBIOS.Text = StringsAndConstants.FETCHING;
            lblBIOSType.Text = StringsAndConstants.FETCHING;
            lblSecBoot.Text = StringsAndConstants.FETCHING;
            lblVT.Text = StringsAndConstants.FETCHING;
            lblTPM.Text = StringsAndConstants.FETCHING;
            collectButton.Text = StringsAndConstants.FETCHING;
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
            if (BM == StringsAndConstants.ToBeFilledByOEM || BM == "")
                BM = HardwareInfo.GetBoardMakerAlt();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            Model = HardwareInfo.GetModel();
            if (Model == StringsAndConstants.ToBeFilledByOEM || Model == "")
                Model = HardwareInfo.GetModelAlt();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            SerialNo = HardwareInfo.GetBoardProductId();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            ProcName = HardwareInfo.GetProcessorCores();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            PM = HardwareInfo.GetPhysicalMemory() + " (" + HardwareInfo.GetNumFreeRamSlots(Convert.ToInt32(HardwareInfo.GetNumRamSlots())) +
                " slots de " + HardwareInfo.GetNumRamSlots() + " ocupados" + ")";
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            HDSize = HardwareInfo.GetHDSize();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            Smart = HardwareInfo.GetSMARTStatus();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            MediaType = HardwareInfo.GetStorageType();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            MediaOperation = HardwareInfo.GetStorageOperation();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            GPUInfo = HardwareInfo.GetGPUInfo();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            OS = HardwareInfo.GetOSInformation();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            Hostname = HardwareInfo.GetComputerName();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            Mac = HardwareInfo.GetMACAddress();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            IP = HardwareInfo.GetIPAddress();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            BIOSType = HardwareInfo.GetBIOSType();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            SecBoot = HardwareInfo.GetSecureBoot();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            BIOS = HardwareInfo.GetComputerBIOS();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            VT = HardwareInfo.GetVirtualizationTechnology();
            i++;
            worker.ReportProgress(progressAuxFunction(i));

            TPM = HardwareInfo.GetTPMStatus();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
        }

        //Prints the collected data into the form labels, warning the user when there are forbidden modes
        private void printHardwareData()
        {
            pass = true;
            lblBM.Text = BM;
            lblModel.Text = Model;
            lblSerialNo.Text = SerialNo;
            lblProcName.Text = ProcName;
            lblPM.Text = PM;
            lblHDSize.Text = HDSize;
            lblSmart.Text = Smart;
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
            lblTPM.Text = TPM;
            sinceLabelUpdate(true);
            sinceLabelUpdate(false);

            string[] str = BIOSFileReader.fetchInfo(lblBM.Text, lblModel.Text, lblBIOSType.Text, ip, port);

            if (lblHostname.Text.Equals(StringsAndConstants.DEFAULT_HOSTNAME))
            {
                pass = false;
                lblHostname.Text += StringsAndConstants.HOSTNAME_ALERT;
                timer1.Enabled = true;
            }
            //The section below contains the exception cases for AHCI enforcement
            if (!lblModel.Text.Contains(StringsAndConstants.nonAHCImodel1) &&
                !lblModel.Text.Contains(StringsAndConstants.nonAHCImodel2) &&
                !lblModel.Text.Contains(StringsAndConstants.nonAHCImodel3) &&
                !lblModel.Text.Contains(StringsAndConstants.nonAHCImodel4) &&
                !lblModel.Text.Contains(StringsAndConstants.nonAHCImodel5) &&
                !lblModel.Text.Contains(StringsAndConstants.nonAHCImodel6) &&
                Environment.Is64BitOperatingSystem &&
                lblMediaOperation.Text.Equals(StringsAndConstants.MEDIA_OPERATION_IDE_RAID))
            {
                if (lblModel.Text.Contains(StringsAndConstants.nvmeModel1))
                {
                    lblMediaOperation.Text = StringsAndConstants.MEDIA_OPERATION_NVME;
                }
                else
                {
                    pass = false;
                    lblMediaOperation.Text += StringsAndConstants.MEDIA_OPERATION_ALERT;
                    timer2.Enabled = true;
                }
            }
            //The section below contains the exception cases for Secure Boot enforcement
            if (lblSecBoot.Text.Equals(StringsAndConstants.deactivated) &&
                !lblGPUInfo.Text.Contains(StringsAndConstants.nonSecBootGPU1) &&
                !lblGPUInfo.Text.Contains(StringsAndConstants.nonSecBootGPU2))
            {
                pass = false;
                lblSecBoot.Text += StringsAndConstants.SECURE_BOOT_ALERT;
                timer3.Enabled = true;
            }
            if (str == null)
            {
                if(!offlineMode)
                {
                    pass = false;
                    MessageBox.Show(StringsAndConstants.DATABASE_REACH_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }                
            }
            if (str != null && !lblBIOS.Text.Contains(str[0]))
            {
                if (!str[0].Equals("-1"))
                {
                    pass = false;
                    lblBIOS.Text += StringsAndConstants.BIOS_VERSION_ALERT;
                    timer4.Enabled = true;
                }
            }
            if (str != null && str[1].Equals("false"))
            {
                pass = false;
                lblBIOSType.Text += StringsAndConstants.FIRMWARE_TYPE_ALERT;
                timer6.Enabled = true;
            }
            if (lblMac.Text == "")
            {
                if(!offlineMode)
                {
                    pass = false;
                    lblMac.Text = StringsAndConstants.NETWORK_ERROR;
                    lblIP.Text = StringsAndConstants.NETWORK_ERROR;
                    timer5.Enabled = true;
                }
                else
                {
                    lblMac.Text = StringsAndConstants.OFFLINE_MODE_ACTIVATED;
                    lblIP.Text = StringsAndConstants.OFFLINE_MODE_ACTIVATED;
                }
                
            }
            if (lblVT.Text == StringsAndConstants.deactivated)
            {
                pass = false;
                lblVT.Text += StringsAndConstants.VT_ALERT;
                timer7.Enabled = true;
            }
            if (!lblSmart.Text.Contains(StringsAndConstants.ok))
            {
                pass = false;
                lblSmart.Text += StringsAndConstants.SMART_FAIL;
                timer8.Enabled = true;
            }
        }

        //Triggers when the form opens, and when the user clicks to collect
        private void coleta_Click(object sender, EventArgs e)
        {
            webView2.Visible = false;
            collecting();
            accessSystemButton.Enabled = false;
            registerButton.Enabled = false;
            collectButton.Enabled = false;
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
            if (!offlineMode)
            {
                accessSystemButton.Enabled = true;
                registerButton.Enabled = true;
            }
            collectButton.Enabled = true;
            collectButton.Text = StringsAndConstants.FETCH_AGAIN;
            printHardwareData();
        }

        //Attributes the data collected previously to the variables which will inside the URL for registration
        private void attrHardwareData()
        {
            sArgs[10] = lblBM.Text;
            sArgs[11] = lblModel.Text;
            sArgs[12] = lblSerialNo.Text;
            sArgs[13] = lblProcName.Text;
            sArgs[14] = lblPM.Text;
            sArgs[15] = lblHDSize.Text;
            sArgs[16] = lblOS.Text;
            sArgs[17] = lblHostname.Text;
            sArgs[18] = lblBIOS.Text;
            sArgs[19] = lblMac.Text;
            sArgs[20] = lblIP.Text;
            sArgs[24] = lblBIOSType.Text;
            sArgs[25] = lblMediaType.Text;
            sArgs[26] = lblGPUInfo.Text;
            sArgs[27] = lblMediaOperation.Text;
            sArgs[28] = lblSecBoot.Text;
            sArgs[29] = lblVT.Text;
            sArgs[30] = lblTPM.Text;
        }

        //Updates the 'last installed' or 'last maintenance' labels
        private void sinceLabelUpdate(bool mode)
        {
            if (mode)
            {
                InstallLabel = MiscMethods.regCheck(mode).ToString();
                if (!InstallLabel.Equals("-1"))
                    lblInstallSince.Text = "(" + InstallLabel + StringsAndConstants.DAYS_PASSED_TEXT + StringsAndConstants.FORMAT_TEXT + ")";
                else
                    lblInstallSince.Text = StringsAndConstants.SINCE_UNKNOWN;
            }
            else
            {
                MaintenanceLabel = MiscMethods.regCheck(mode).ToString();
                if (!MaintenanceLabel.Equals("-1"))
                    lblMaintenanceSince.Text = "(" + MaintenanceLabel + StringsAndConstants.DAYS_PASSED_TEXT + StringsAndConstants.MAINTENANCE_TEXT + ")";
                else
                    lblMaintenanceSince.Text = StringsAndConstants.SINCE_UNKNOWN;
            }
        }

        //Loads webView2 component
        public async Task loadWebView2()
        {
            CoreWebView2Environment webView2Environment = await CoreWebView2Environment.CreateAsync(StringsAndConstants.WEBVIEW2_PATH, Path.GetTempPath());
            await webView2.EnsureCoreWebView2Async(webView2Environment);
        }

        //Sends hardware info to the specified server
        public void serverSendInfo(string[] serverArgs)
        {
            webView2.CoreWebView2.Navigate("http://" + serverArgs[0] + ":" + serverArgs[1] + "/" + serverArgs[2] + ".php?patrimonio=" + serverArgs[3] + "&lacre=" + serverArgs[4] + "&sala=" + serverArgs[5] + "&predio=" + serverArgs[6] + "&ad=" + serverArgs[7] + "&padrao=" + serverArgs[8] + "&formatacao=" + serverArgs[9] + "&formatacoesAnteriores=" + serverArgs[9] + "&marca=" + serverArgs[10] + "&modelo=" + serverArgs[11] + "&numeroSerial=" + serverArgs[12] + "&processador=" + serverArgs[13] + "&memoria=" + serverArgs[14] + "&hd=" + serverArgs[15] + "&sistemaOperacional=" + serverArgs[16] + "&nomeDoComputador=" + serverArgs[17] + "&bios=" + serverArgs[18] + "&mac=" + serverArgs[19] + "&ip=" + serverArgs[20] + "&emUso=" + serverArgs[21] + "&etiqueta=" + serverArgs[22] + "&tipo=" + serverArgs[23] + "&tipoFW=" + serverArgs[24] + "&tipoArmaz=" + serverArgs[25] + "&gpu=" + serverArgs[26] + "&modoArmaz=" + serverArgs[27] + "&secBoot=" + serverArgs[28] + "&vt=" + serverArgs[29] + "&tpm=" + serverArgs[30] + "&trocaPilha=" + serverArgs[31] + "&ticketNum=" + serverArgs[32] + "&agent=" + serverArgs[33]);
        }

        //Runs the registration for the website
        private void cadastra_ClickAsync(object sender, EventArgs e)
        {
            registerButton.Text = StringsAndConstants.REGISTERING;
            registerButton.Enabled = false;
            accessSystemButton.Enabled = false;
            collectButton.Enabled = false;
            attrHardwareData();
            if (!string.IsNullOrWhiteSpace(textBoxPatrimony.Text) && !string.IsNullOrWhiteSpace(textBoxRoom.Text) && !string.IsNullOrWhiteSpace(textBoxTicket.Text) && comboBoxType.SelectedItem != null && comboBoxBuilding.SelectedItem != null && comboBoxInUse.SelectedItem != null && comboBoxTag.SelectedItem != null && comboBoxBattery.SelectedItem != null && (employeeRadioButton.Checked || studentRadioButton.Checked) && (formatButton.Checked || maintenanceButton.Checked) && pass == true)
            {
                sArgs[0] = ip;
                sArgs[1] = port;
                sArgs[2] = modeURL;
                sArgs[3] = textBoxPatrimony.Text;
                sArgs[4] = textBoxSeal.Text;
                if (textBoxLetter.Text != "")
                    sArgs[5] = textBoxRoom.Text + textBoxLetter.Text;
                else
                    sArgs[5] = textBoxRoom.Text;
                sArgs[6] = comboBoxBuilding.SelectedItem.ToString();
                sArgs[7] = comboBoxActiveDirectory.SelectedItem.ToString();
                sArgs[8] = comboBoxStandard.SelectedItem.ToString();
                sArgs[9] = dateTimePicker1.Value.ToString();
                sArgs[21] = comboBoxInUse.SelectedItem.ToString();
                sArgs[22] = comboBoxTag.SelectedItem.ToString();
                sArgs[23] = comboBoxType.SelectedItem.ToString();
                if(comboBoxBattery.SelectedItem.ToString().Equals("Sim"))
                    sArgs[31] = StringsAndConstants.replacedBattery;
                else
                    sArgs[31] = StringsAndConstants.sameBattery;
                sArgs[32] = textBoxTicket.Text;
                sArgs[33] = this.user;

                if (serverOnline && porta != "")
                {
                    if (!date.Contains(sArgs[9]))
                    {
                        webView2.Visible = true;
                        serverSendInfo(sArgs);
                        date.Add(sArgs[9]);
                        if(formatButton.Checked)
                        {
                            MiscMethods.regCreate(true, dateTimePicker1);
                            sinceLabelUpdate(true);
                            sinceLabelUpdate(false);
                        }
                        else if (maintenanceButton.Checked)
                        {
                            MiscMethods.regCreate(false, dateTimePicker1);
                            sinceLabelUpdate(false);
                        }
                    }
                    else
                        MessageBox.Show(StringsAndConstants.ALREADY_REGISTERED_TODAY, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show(StringsAndConstants.SERVER_NOT_FOUND_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!pass)
                MessageBox.Show(StringsAndConstants.PENDENCY_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show(StringsAndConstants.MANDATORY_FIELD, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            registerButton.Text = StringsAndConstants.REGISTER_AGAIN;
            registerButton.Enabled = true;
            accessSystemButton.Enabled = true;
            collectButton.Enabled = true;
        }
    }
}

