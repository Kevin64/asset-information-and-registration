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
using MRG.Controls.UI;
using LogGeneratorDLL;
using HardwareInformation.Properties;
using System.Globalization;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace HardwareInformation
{
    public partial class MainForm : Form
    {
        private BackgroundWorker backgroundWorker1;
        private ToolStripStatusLabel logLabel;
        private LogGenerator log;
        private TaskbarManager tbProgMain;
        private int percent;

        public MainForm(bool noConnection, string user, string ip, string port, LogGenerator l)
        {
            InitializeComponent();

            //Program version
#if DEBUG
            this.toolStripStatusLabel2.Text = MiscMethods.version(Resources.dev_status);
#else
            this.toolStripStatusLabel2.Text = MiscMethods.version();
#endif

            log = l;
            themeBool = MiscMethods.ThemeInit();
            offlineMode = noConnection;

            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_OFFLINE_MODE, offlineMode.ToString(), StringsAndConstants.consoleOutGUI);

            this.user = user;
            this.ip = ip;
            this.port = port;

            comboBoxBuilding.Items.AddRange(StringsAndConstants.listBuilding.ToArray());
            comboBoxActiveDirectory.Items.AddRange(StringsAndConstants.listActiveDirectory.ToArray());
            comboBoxStandard.Items.AddRange(StringsAndConstants.listStandardGUI.ToArray());
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
            this.toolStripStatusLabel1.Text = StringsAndConstants.statusBarTextForm1;
            this.Text = StringsAndConstants.formTitlebarText;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
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
            this.label16 = new System.Windows.Forms.Label();
            this.registerButton = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
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
            this.loadingCircle19 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle18 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle17 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle16 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle15 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle14 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle13 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle12 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle11 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle10 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle9 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle8 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle7 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle6 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle5 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle4 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle3 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle2 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle1 = new MRG.Controls.UI.LoadingCircle();
            this.separatorH = new System.Windows.Forms.Label();
            this.separatorV = new System.Windows.Forms.Label();
            this.configurableQualityPictureBox33 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox32 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblSmart = new System.Windows.Forms.Label();
            this.lblTPM = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.configurableQualityPictureBox30 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.label45 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label28 = new System.Windows.Forms.Label();
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
            this.lblSecBoot = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.lblMediaOperation = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.lblGPUInfo = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.lblMediaType = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxBattery = new CustomFlatComboBox();
            this.comboBoxStandard = new CustomFlatComboBox();
            this.comboBoxActiveDirectory = new CustomFlatComboBox();
            this.comboBoxTag = new CustomFlatComboBox();
            this.comboBoxInUse = new CustomFlatComboBox();
            this.comboBoxType = new CustomFlatComboBox();
            this.comboBoxBuilding = new CustomFlatComboBox();
            this.lblAgentName = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.lblPortServer = new System.Windows.Forms.Label();
            this.lblIPServer = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.configurableQualityPictureBox35 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.label31 = new System.Windows.Forms.Label();
            this.textBoxTicket = new System.Windows.Forms.TextBox();
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
            this.loadingCircle21 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle20 = new MRG.Controls.UI.LoadingCircle();
            this.lblMaintenanceSince = new System.Windows.Forms.Label();
            this.lblInstallSince = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.formatButton = new System.Windows.Forms.RadioButton();
            this.maintenanceButton = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.comboBoxTheme = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.logLabel = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.timer9 = new System.Windows.Forms.Timer(this.components);
            this.timer10 = new System.Windows.Forms.Timer(this.components);
            this.configurableQualityPictureBox1 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.loadingCircle22 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle23 = new MRG.Controls.UI.LoadingCircle();
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
            this.lblBM.ForeColor = System.Drawing.Color.Silver;
            this.lblBM.Location = new System.Drawing.Point(195, 20);
            this.lblBM.Name = "lblBM";
            this.lblBM.Size = new System.Drawing.Size(10, 13);
            this.lblBM.TabIndex = 7;
            this.lblBM.Text = "-";
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.ForeColor = System.Drawing.Color.Silver;
            this.lblModel.Location = new System.Drawing.Point(195, 46);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(10, 13);
            this.lblModel.TabIndex = 8;
            this.lblModel.Text = "-";
            // 
            // lblSerialNo
            // 
            this.lblSerialNo.AutoSize = true;
            this.lblSerialNo.ForeColor = System.Drawing.Color.Silver;
            this.lblSerialNo.Location = new System.Drawing.Point(195, 72);
            this.lblSerialNo.Name = "lblSerialNo";
            this.lblSerialNo.Size = new System.Drawing.Size(10, 13);
            this.lblSerialNo.TabIndex = 9;
            this.lblSerialNo.Text = "-";
            // 
            // lblProcName
            // 
            this.lblProcName.AutoSize = true;
            this.lblProcName.ForeColor = System.Drawing.Color.Silver;
            this.lblProcName.Location = new System.Drawing.Point(195, 98);
            this.lblProcName.Name = "lblProcName";
            this.lblProcName.Size = new System.Drawing.Size(10, 13);
            this.lblProcName.TabIndex = 10;
            this.lblProcName.Text = "-";
            // 
            // lblPM
            // 
            this.lblPM.AutoSize = true;
            this.lblPM.ForeColor = System.Drawing.Color.Silver;
            this.lblPM.Location = new System.Drawing.Point(195, 124);
            this.lblPM.Name = "lblPM";
            this.lblPM.Size = new System.Drawing.Size(10, 13);
            this.lblPM.TabIndex = 11;
            this.lblPM.Text = "-";
            // 
            // lblHDSize
            // 
            this.lblHDSize.AutoSize = true;
            this.lblHDSize.ForeColor = System.Drawing.Color.Silver;
            this.lblHDSize.Location = new System.Drawing.Point(195, 150);
            this.lblHDSize.Name = "lblHDSize";
            this.lblHDSize.Size = new System.Drawing.Size(10, 13);
            this.lblHDSize.TabIndex = 12;
            this.lblHDSize.Text = "-";
            // 
            // lblOS
            // 
            this.lblOS.AutoSize = true;
            this.lblOS.ForeColor = System.Drawing.Color.Silver;
            this.lblOS.Location = new System.Drawing.Point(195, 280);
            this.lblOS.Name = "lblOS";
            this.lblOS.Size = new System.Drawing.Size(10, 13);
            this.lblOS.TabIndex = 13;
            this.lblOS.Text = "-";
            // 
            // lblHostname
            // 
            this.lblHostname.AutoSize = true;
            this.lblHostname.ForeColor = System.Drawing.Color.Silver;
            this.lblHostname.Location = new System.Drawing.Point(195, 306);
            this.lblHostname.Name = "lblHostname";
            this.lblHostname.Size = new System.Drawing.Size(10, 13);
            this.lblHostname.TabIndex = 15;
            this.lblHostname.Text = "-";
            // 
            // lblMac
            // 
            this.lblMac.AutoSize = true;
            this.lblMac.ForeColor = System.Drawing.Color.Silver;
            this.lblMac.Location = new System.Drawing.Point(195, 332);
            this.lblMac.Name = "lblMac";
            this.lblMac.Size = new System.Drawing.Size(10, 13);
            this.lblMac.TabIndex = 18;
            this.lblMac.Text = "-";
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.ForeColor = System.Drawing.Color.Silver;
            this.lblIP.Location = new System.Drawing.Point(195, 358);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(10, 13);
            this.lblIP.TabIndex = 19;
            this.lblIP.Text = "-";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(37, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Marca:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(37, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Modelo:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(37, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Número Serial:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(37, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(146, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Processador e nº de núcleos:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(37, 124);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Memória RAM e nº de slots:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label6.Location = new System.Drawing.Point(37, 150);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(153, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Armazenamento (espaço total):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label7.Location = new System.Drawing.Point(37, 280);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Sistema Operacional:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label8.Location = new System.Drawing.Point(37, 306);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(113, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Nome do Computador:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label9.Location = new System.Drawing.Point(37, 332);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(118, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Endereço MAC do NIC:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label10.Location = new System.Drawing.Point(37, 358);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(105, 13);
            this.label10.TabIndex = 9;
            this.label10.Text = "Endereço IP do NIC:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label11.Location = new System.Drawing.Point(37, 20);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Patrimônio:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label12.Location = new System.Drawing.Point(37, 46);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 13);
            this.label12.TabIndex = 11;
            this.label12.Text = "Lacre (se houver):";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label13.Location = new System.Drawing.Point(37, 98);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(40, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "Prédio:";
            // 
            // textBoxPatrimony
            // 
            this.textBoxPatrimony.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.textBoxPatrimony.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxPatrimony.Location = new System.Drawing.Point(185, 17);
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
            this.textBoxSeal.Location = new System.Drawing.Point(185, 43);
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
            this.textBoxRoom.Location = new System.Drawing.Point(185, 69);
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
            this.textBoxLetter.Location = new System.Drawing.Point(419, 69);
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
            this.label14.Location = new System.Drawing.Point(37, 72);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(135, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "Sala (0000 se não houver):";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label16.Location = new System.Drawing.Point(37, 150);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(115, 13);
            this.label16.TabIndex = 16;
            this.label16.Text = "Data do último serviço:";
            // 
            // registerButton
            // 
            this.registerButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.registerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.registerButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.registerButton.Location = new System.Drawing.Point(760, 633);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(265, 56);
            this.registerButton.TabIndex = 53;
            this.registerButton.Text = "Cadastrar / Atualizar dados";
            this.registerButton.UseVisualStyleBackColor = false;
            this.registerButton.Click += new System.EventHandler(this.cadastra_ClickAsync);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label18.Location = new System.Drawing.Point(322, 98);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(45, 13);
            this.label18.TabIndex = 48;
            this.label18.Text = "Em uso:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label19.Location = new System.Drawing.Point(322, 124);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(49, 13);
            this.label19.TabIndex = 50;
            this.label19.Text = "Etiqueta:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label20.Location = new System.Drawing.Point(37, 124);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(31, 13);
            this.label20.TabIndex = 53;
            this.label20.Text = "Tipo:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label21.Location = new System.Drawing.Point(225, 403);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(95, 13);
            this.label21.TabIndex = 17;
            this.label21.Text = "Status do servidor:";
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
            this.collectButton.Location = new System.Drawing.Point(575, 633);
            this.collectButton.Name = "collectButton";
            this.collectButton.Size = new System.Drawing.Size(180, 25);
            this.collectButton.TabIndex = 51;
            this.collectButton.Text = "Coletar novamente";
            this.collectButton.UseVisualStyleBackColor = false;
            this.collectButton.Click += new System.EventHandler(this.coleta_Click);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label23.Location = new System.Drawing.Point(322, 72);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(90, 13);
            this.label23.TabIndex = 55;
            this.label23.Text = "Letra (se houver):";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label24.Location = new System.Drawing.Point(37, 410);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(115, 13);
            this.label24.TabIndex = 56;
            this.label24.Text = "Versão da BIOS/UEFI:";
            // 
            // lblBIOS
            // 
            this.lblBIOS.AutoSize = true;
            this.lblBIOS.ForeColor = System.Drawing.Color.Silver;
            this.lblBIOS.Location = new System.Drawing.Point(195, 410);
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
            this.accessSystemButton.Location = new System.Drawing.Point(575, 664);
            this.accessSystemButton.Name = "accessSystemButton";
            this.accessSystemButton.Size = new System.Drawing.Size(180, 25);
            this.accessSystemButton.TabIndex = 52;
            this.accessSystemButton.Text = "Acessar sistema de patrimônios";
            this.accessSystemButton.UseVisualStyleBackColor = false;
            this.accessSystemButton.Click += new System.EventHandler(this.accessButton_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label25.Location = new System.Drawing.Point(37, 384);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(88, 13);
            this.label25.TabIndex = 62;
            this.label25.Text = "Tipo de firmware:";
            // 
            // lblBIOSType
            // 
            this.lblBIOSType.AutoSize = true;
            this.lblBIOSType.ForeColor = System.Drawing.Color.Silver;
            this.lblBIOSType.Location = new System.Drawing.Point(195, 384);
            this.lblBIOSType.Name = "lblBIOSType";
            this.lblBIOSType.Size = new System.Drawing.Size(10, 13);
            this.lblBIOSType.TabIndex = 63;
            this.lblBIOSType.Text = "-";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.loadingCircle19);
            this.groupBox1.Controls.Add(this.loadingCircle18);
            this.groupBox1.Controls.Add(this.loadingCircle17);
            this.groupBox1.Controls.Add(this.loadingCircle16);
            this.groupBox1.Controls.Add(this.loadingCircle15);
            this.groupBox1.Controls.Add(this.loadingCircle14);
            this.groupBox1.Controls.Add(this.loadingCircle13);
            this.groupBox1.Controls.Add(this.loadingCircle12);
            this.groupBox1.Controls.Add(this.loadingCircle11);
            this.groupBox1.Controls.Add(this.loadingCircle10);
            this.groupBox1.Controls.Add(this.loadingCircle9);
            this.groupBox1.Controls.Add(this.loadingCircle8);
            this.groupBox1.Controls.Add(this.loadingCircle7);
            this.groupBox1.Controls.Add(this.loadingCircle6);
            this.groupBox1.Controls.Add(this.loadingCircle5);
            this.groupBox1.Controls.Add(this.loadingCircle4);
            this.groupBox1.Controls.Add(this.loadingCircle3);
            this.groupBox1.Controls.Add(this.loadingCircle2);
            this.groupBox1.Controls.Add(this.loadingCircle1);
            this.groupBox1.Controls.Add(this.separatorH);
            this.groupBox1.Controls.Add(this.separatorV);
            this.groupBox1.Controls.Add(this.configurableQualityPictureBox33);
            this.groupBox1.Controls.Add(this.configurableQualityPictureBox32);
            this.groupBox1.Controls.Add(this.lblSmart);
            this.groupBox1.Controls.Add(this.lblTPM);
            this.groupBox1.Controls.Add(this.label44);
            this.groupBox1.Controls.Add(this.configurableQualityPictureBox30);
            this.groupBox1.Controls.Add(this.label45);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.label28);
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
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox1.Location = new System.Drawing.Point(32, 113);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(537, 576);
            this.groupBox1.TabIndex = 65;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dados do computador";
            // 
            // loadingCircle19
            // 
            this.loadingCircle19.Active = false;
            this.loadingCircle19.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle19.InnerCircleRadius = 8;
            this.loadingCircle19.Location = new System.Drawing.Point(194, 482);
            this.loadingCircle19.Name = "loadingCircle19";
            this.loadingCircle19.NumberSpoke = 24;
            this.loadingCircle19.OuterCircleRadius = 9;
            this.loadingCircle19.RotationSpeed = 20;
            this.loadingCircle19.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle19.SpokeThickness = 4;
            this.loadingCircle19.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle19.TabIndex = 131;
            this.loadingCircle19.Text = "loadingCircle19";
            // 
            // loadingCircle18
            // 
            this.loadingCircle18.Active = false;
            this.loadingCircle18.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle18.InnerCircleRadius = 8;
            this.loadingCircle18.Location = new System.Drawing.Point(194, 456);
            this.loadingCircle18.Name = "loadingCircle18";
            this.loadingCircle18.NumberSpoke = 24;
            this.loadingCircle18.OuterCircleRadius = 9;
            this.loadingCircle18.RotationSpeed = 20;
            this.loadingCircle18.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle18.SpokeThickness = 4;
            this.loadingCircle18.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle18.TabIndex = 130;
            this.loadingCircle18.Text = "loadingCircle18";
            // 
            // loadingCircle17
            // 
            this.loadingCircle17.Active = false;
            this.loadingCircle17.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle17.InnerCircleRadius = 8;
            this.loadingCircle17.Location = new System.Drawing.Point(194, 430);
            this.loadingCircle17.Name = "loadingCircle17";
            this.loadingCircle17.NumberSpoke = 24;
            this.loadingCircle17.OuterCircleRadius = 9;
            this.loadingCircle17.RotationSpeed = 20;
            this.loadingCircle17.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle17.SpokeThickness = 4;
            this.loadingCircle17.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle17.TabIndex = 129;
            this.loadingCircle17.Text = "loadingCircle17";
            // 
            // loadingCircle16
            // 
            this.loadingCircle16.Active = false;
            this.loadingCircle16.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle16.InnerCircleRadius = 8;
            this.loadingCircle16.Location = new System.Drawing.Point(194, 404);
            this.loadingCircle16.Name = "loadingCircle16";
            this.loadingCircle16.NumberSpoke = 24;
            this.loadingCircle16.OuterCircleRadius = 9;
            this.loadingCircle16.RotationSpeed = 20;
            this.loadingCircle16.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle16.SpokeThickness = 4;
            this.loadingCircle16.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle16.TabIndex = 128;
            this.loadingCircle16.Text = "loadingCircle16";
            // 
            // loadingCircle15
            // 
            this.loadingCircle15.Active = false;
            this.loadingCircle15.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle15.InnerCircleRadius = 8;
            this.loadingCircle15.Location = new System.Drawing.Point(194, 378);
            this.loadingCircle15.Name = "loadingCircle15";
            this.loadingCircle15.NumberSpoke = 24;
            this.loadingCircle15.OuterCircleRadius = 9;
            this.loadingCircle15.RotationSpeed = 20;
            this.loadingCircle15.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle15.SpokeThickness = 4;
            this.loadingCircle15.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle15.TabIndex = 127;
            this.loadingCircle15.Text = "loadingCircle15";
            // 
            // loadingCircle14
            // 
            this.loadingCircle14.Active = false;
            this.loadingCircle14.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle14.InnerCircleRadius = 8;
            this.loadingCircle14.Location = new System.Drawing.Point(194, 352);
            this.loadingCircle14.Name = "loadingCircle14";
            this.loadingCircle14.NumberSpoke = 24;
            this.loadingCircle14.OuterCircleRadius = 9;
            this.loadingCircle14.RotationSpeed = 20;
            this.loadingCircle14.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle14.SpokeThickness = 4;
            this.loadingCircle14.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle14.TabIndex = 126;
            this.loadingCircle14.Text = "loadingCircle14";
            // 
            // loadingCircle13
            // 
            this.loadingCircle13.Active = false;
            this.loadingCircle13.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle13.InnerCircleRadius = 8;
            this.loadingCircle13.Location = new System.Drawing.Point(194, 326);
            this.loadingCircle13.Name = "loadingCircle13";
            this.loadingCircle13.NumberSpoke = 24;
            this.loadingCircle13.OuterCircleRadius = 9;
            this.loadingCircle13.RotationSpeed = 20;
            this.loadingCircle13.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle13.SpokeThickness = 4;
            this.loadingCircle13.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle13.TabIndex = 125;
            this.loadingCircle13.Text = "loadingCircle13";
            // 
            // loadingCircle12
            // 
            this.loadingCircle12.Active = false;
            this.loadingCircle12.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle12.InnerCircleRadius = 8;
            this.loadingCircle12.Location = new System.Drawing.Point(194, 300);
            this.loadingCircle12.Name = "loadingCircle12";
            this.loadingCircle12.NumberSpoke = 24;
            this.loadingCircle12.OuterCircleRadius = 9;
            this.loadingCircle12.RotationSpeed = 20;
            this.loadingCircle12.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle12.SpokeThickness = 4;
            this.loadingCircle12.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle12.TabIndex = 124;
            this.loadingCircle12.Text = "loadingCircle12";
            // 
            // loadingCircle11
            // 
            this.loadingCircle11.Active = false;
            this.loadingCircle11.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle11.InnerCircleRadius = 8;
            this.loadingCircle11.Location = new System.Drawing.Point(194, 274);
            this.loadingCircle11.Name = "loadingCircle11";
            this.loadingCircle11.NumberSpoke = 24;
            this.loadingCircle11.OuterCircleRadius = 9;
            this.loadingCircle11.RotationSpeed = 20;
            this.loadingCircle11.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle11.SpokeThickness = 4;
            this.loadingCircle11.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle11.TabIndex = 123;
            this.loadingCircle11.Text = "loadingCircle11";
            // 
            // loadingCircle10
            // 
            this.loadingCircle10.Active = false;
            this.loadingCircle10.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle10.InnerCircleRadius = 8;
            this.loadingCircle10.Location = new System.Drawing.Point(194, 248);
            this.loadingCircle10.Name = "loadingCircle10";
            this.loadingCircle10.NumberSpoke = 24;
            this.loadingCircle10.OuterCircleRadius = 9;
            this.loadingCircle10.RotationSpeed = 20;
            this.loadingCircle10.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle10.SpokeThickness = 4;
            this.loadingCircle10.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle10.TabIndex = 122;
            this.loadingCircle10.Text = "loadingCircle10";
            // 
            // loadingCircle9
            // 
            this.loadingCircle9.Active = false;
            this.loadingCircle9.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle9.InnerCircleRadius = 8;
            this.loadingCircle9.Location = new System.Drawing.Point(194, 222);
            this.loadingCircle9.Name = "loadingCircle9";
            this.loadingCircle9.NumberSpoke = 24;
            this.loadingCircle9.OuterCircleRadius = 9;
            this.loadingCircle9.RotationSpeed = 20;
            this.loadingCircle9.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle9.SpokeThickness = 4;
            this.loadingCircle9.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle9.TabIndex = 121;
            this.loadingCircle9.Text = "loadingCircle9";
            // 
            // loadingCircle8
            // 
            this.loadingCircle8.Active = false;
            this.loadingCircle8.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle8.InnerCircleRadius = 8;
            this.loadingCircle8.Location = new System.Drawing.Point(194, 196);
            this.loadingCircle8.Name = "loadingCircle8";
            this.loadingCircle8.NumberSpoke = 24;
            this.loadingCircle8.OuterCircleRadius = 9;
            this.loadingCircle8.RotationSpeed = 20;
            this.loadingCircle8.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle8.SpokeThickness = 4;
            this.loadingCircle8.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle8.TabIndex = 120;
            this.loadingCircle8.Text = "loadingCircle8";
            // 
            // loadingCircle7
            // 
            this.loadingCircle7.Active = false;
            this.loadingCircle7.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle7.InnerCircleRadius = 8;
            this.loadingCircle7.Location = new System.Drawing.Point(194, 170);
            this.loadingCircle7.Name = "loadingCircle7";
            this.loadingCircle7.NumberSpoke = 24;
            this.loadingCircle7.OuterCircleRadius = 9;
            this.loadingCircle7.RotationSpeed = 20;
            this.loadingCircle7.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle7.SpokeThickness = 4;
            this.loadingCircle7.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle7.TabIndex = 119;
            this.loadingCircle7.Text = "loadingCircle7";
            // 
            // loadingCircle6
            // 
            this.loadingCircle6.Active = false;
            this.loadingCircle6.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle6.InnerCircleRadius = 8;
            this.loadingCircle6.Location = new System.Drawing.Point(194, 144);
            this.loadingCircle6.Name = "loadingCircle6";
            this.loadingCircle6.NumberSpoke = 24;
            this.loadingCircle6.OuterCircleRadius = 9;
            this.loadingCircle6.RotationSpeed = 20;
            this.loadingCircle6.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle6.SpokeThickness = 4;
            this.loadingCircle6.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle6.TabIndex = 118;
            this.loadingCircle6.Text = "loadingCircle6";
            // 
            // loadingCircle5
            // 
            this.loadingCircle5.Active = false;
            this.loadingCircle5.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle5.InnerCircleRadius = 8;
            this.loadingCircle5.Location = new System.Drawing.Point(194, 118);
            this.loadingCircle5.Name = "loadingCircle5";
            this.loadingCircle5.NumberSpoke = 24;
            this.loadingCircle5.OuterCircleRadius = 9;
            this.loadingCircle5.RotationSpeed = 20;
            this.loadingCircle5.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle5.SpokeThickness = 4;
            this.loadingCircle5.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle5.TabIndex = 117;
            this.loadingCircle5.Text = "loadingCircle5";
            // 
            // loadingCircle4
            // 
            this.loadingCircle4.Active = false;
            this.loadingCircle4.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle4.InnerCircleRadius = 8;
            this.loadingCircle4.Location = new System.Drawing.Point(194, 92);
            this.loadingCircle4.Name = "loadingCircle4";
            this.loadingCircle4.NumberSpoke = 24;
            this.loadingCircle4.OuterCircleRadius = 9;
            this.loadingCircle4.RotationSpeed = 20;
            this.loadingCircle4.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle4.SpokeThickness = 4;
            this.loadingCircle4.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle4.TabIndex = 116;
            this.loadingCircle4.Text = "loadingCircle4";
            // 
            // loadingCircle3
            // 
            this.loadingCircle3.Active = false;
            this.loadingCircle3.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle3.InnerCircleRadius = 8;
            this.loadingCircle3.Location = new System.Drawing.Point(194, 66);
            this.loadingCircle3.Name = "loadingCircle3";
            this.loadingCircle3.NumberSpoke = 24;
            this.loadingCircle3.OuterCircleRadius = 9;
            this.loadingCircle3.RotationSpeed = 20;
            this.loadingCircle3.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle3.SpokeThickness = 4;
            this.loadingCircle3.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle3.TabIndex = 115;
            this.loadingCircle3.Text = "loadingCircle3";
            // 
            // loadingCircle2
            // 
            this.loadingCircle2.Active = false;
            this.loadingCircle2.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle2.InnerCircleRadius = 8;
            this.loadingCircle2.Location = new System.Drawing.Point(194, 40);
            this.loadingCircle2.Name = "loadingCircle2";
            this.loadingCircle2.NumberSpoke = 24;
            this.loadingCircle2.OuterCircleRadius = 9;
            this.loadingCircle2.RotationSpeed = 20;
            this.loadingCircle2.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle2.SpokeThickness = 4;
            this.loadingCircle2.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle2.TabIndex = 114;
            this.loadingCircle2.Text = "loadingCircle2";
            // 
            // loadingCircle1
            // 
            this.loadingCircle1.Active = false;
            this.loadingCircle1.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle1.InnerCircleRadius = 8;
            this.loadingCircle1.Location = new System.Drawing.Point(194, 14);
            this.loadingCircle1.Name = "loadingCircle1";
            this.loadingCircle1.NumberSpoke = 24;
            this.loadingCircle1.OuterCircleRadius = 9;
            this.loadingCircle1.RotationSpeed = 20;
            this.loadingCircle1.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle1.SpokeThickness = 4;
            this.loadingCircle1.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle1.TabIndex = 113;
            this.loadingCircle1.Text = "loadingCircle1";
            // 
            // separatorH
            // 
            this.separatorH.BackColor = System.Drawing.Color.DimGray;
            this.separatorH.Location = new System.Drawing.Point(6, 513);
            this.separatorH.Name = "separatorH";
            this.separatorH.Size = new System.Drawing.Size(525, 1);
            this.separatorH.TabIndex = 112;
            this.separatorH.Text = "label54";
            // 
            // separatorV
            // 
            this.separatorV.BackColor = System.Drawing.Color.DimGray;
            this.separatorV.Location = new System.Drawing.Point(192, 14);
            this.separatorV.Name = "separatorV";
            this.separatorV.Size = new System.Drawing.Size(1, 499);
            this.separatorV.TabIndex = 111;
            this.separatorV.Text = "label54";
            // 
            // configurableQualityPictureBox33
            // 
            this.configurableQualityPictureBox33.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox33.Image = global::HardwareInformation.Properties.Resources.tpm_white;
            this.configurableQualityPictureBox33.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox33.Location = new System.Drawing.Point(7, 482);
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
            this.configurableQualityPictureBox32.Location = new System.Drawing.Point(7, 170);
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
            this.lblSmart.ForeColor = System.Drawing.Color.Silver;
            this.lblSmart.Location = new System.Drawing.Point(195, 176);
            this.lblSmart.Name = "lblSmart";
            this.lblSmart.Size = new System.Drawing.Size(10, 13);
            this.lblSmart.TabIndex = 106;
            this.lblSmart.Text = "-";
            // 
            // lblTPM
            // 
            this.lblTPM.AutoSize = true;
            this.lblTPM.ForeColor = System.Drawing.Color.Silver;
            this.lblTPM.Location = new System.Drawing.Point(195, 488);
            this.lblTPM.Name = "lblTPM";
            this.lblTPM.Size = new System.Drawing.Size(10, 13);
            this.lblTPM.TabIndex = 109;
            this.lblTPM.Text = "-";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label44.Location = new System.Drawing.Point(37, 176);
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
            this.configurableQualityPictureBox30.Location = new System.Drawing.Point(7, 456);
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
            this.label45.Location = new System.Drawing.Point(37, 488);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(121, 13);
            this.label45.TabIndex = 108;
            this.label45.Text = "Versão do módulo TPM:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(6, 533);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(525, 37);
            this.progressBar1.TabIndex = 69;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.BackColor = System.Drawing.Color.Transparent;
            this.label28.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label28.Location = new System.Drawing.Point(260, 517);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(10, 13);
            this.label28.TabIndex = 70;
            this.label28.Text = "-";
            // 
            // lblVT
            // 
            this.lblVT.AutoSize = true;
            this.lblVT.ForeColor = System.Drawing.Color.Silver;
            this.lblVT.Location = new System.Drawing.Point(195, 462);
            this.lblVT.Name = "lblVT";
            this.lblVT.Size = new System.Drawing.Size(10, 13);
            this.lblVT.TabIndex = 103;
            this.lblVT.Text = "-";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label33.Location = new System.Drawing.Point(37, 462);
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
            this.configurableQualityPictureBox2.Location = new System.Drawing.Point(7, 14);
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
            this.configurableQualityPictureBox17.Location = new System.Drawing.Point(7, 430);
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
            this.configurableQualityPictureBox16.Location = new System.Drawing.Point(7, 404);
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
            this.configurableQualityPictureBox15.Location = new System.Drawing.Point(7, 378);
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
            this.configurableQualityPictureBox14.Location = new System.Drawing.Point(7, 352);
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
            this.configurableQualityPictureBox13.Location = new System.Drawing.Point(7, 326);
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
            this.configurableQualityPictureBox12.Location = new System.Drawing.Point(7, 300);
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
            this.configurableQualityPictureBox11.Location = new System.Drawing.Point(7, 274);
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
            this.configurableQualityPictureBox10.Location = new System.Drawing.Point(7, 248);
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
            this.configurableQualityPictureBox9.Location = new System.Drawing.Point(7, 222);
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
            this.configurableQualityPictureBox8.Location = new System.Drawing.Point(7, 196);
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
            this.configurableQualityPictureBox7.Location = new System.Drawing.Point(7, 144);
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
            this.configurableQualityPictureBox6.Location = new System.Drawing.Point(7, 118);
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
            this.configurableQualityPictureBox5.Location = new System.Drawing.Point(7, 92);
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
            this.configurableQualityPictureBox4.Location = new System.Drawing.Point(7, 66);
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
            this.configurableQualityPictureBox3.Location = new System.Drawing.Point(7, 40);
            this.configurableQualityPictureBox3.Name = "configurableQualityPictureBox3";
            this.configurableQualityPictureBox3.Size = new System.Drawing.Size(25, 25);
            this.configurableQualityPictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.configurableQualityPictureBox3.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.configurableQualityPictureBox3.TabIndex = 73;
            this.configurableQualityPictureBox3.TabStop = false;
            // 
            // lblSecBoot
            // 
            this.lblSecBoot.AutoSize = true;
            this.lblSecBoot.ForeColor = System.Drawing.Color.Silver;
            this.lblSecBoot.Location = new System.Drawing.Point(195, 436);
            this.lblSecBoot.Name = "lblSecBoot";
            this.lblSecBoot.Size = new System.Drawing.Size(10, 13);
            this.lblSecBoot.TabIndex = 71;
            this.lblSecBoot.Text = "-";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label32.Location = new System.Drawing.Point(37, 436);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(69, 13);
            this.label32.TabIndex = 70;
            this.label32.Text = "Secure Boot:";
            // 
            // lblMediaOperation
            // 
            this.lblMediaOperation.AutoSize = true;
            this.lblMediaOperation.ForeColor = System.Drawing.Color.Silver;
            this.lblMediaOperation.Location = new System.Drawing.Point(195, 228);
            this.lblMediaOperation.Name = "lblMediaOperation";
            this.lblMediaOperation.Size = new System.Drawing.Size(10, 13);
            this.lblMediaOperation.TabIndex = 69;
            this.lblMediaOperation.Text = "-";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label30.Location = new System.Drawing.Point(37, 228);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(154, 13);
            this.label30.TabIndex = 68;
            this.label30.Text = "Modo de operação SATA/M.2:";
            // 
            // lblGPUInfo
            // 
            this.lblGPUInfo.AutoSize = true;
            this.lblGPUInfo.ForeColor = System.Drawing.Color.Silver;
            this.lblGPUInfo.Location = new System.Drawing.Point(195, 254);
            this.lblGPUInfo.Name = "lblGPUInfo";
            this.lblGPUInfo.Size = new System.Drawing.Size(10, 13);
            this.lblGPUInfo.TabIndex = 67;
            this.lblGPUInfo.Text = "-";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label29.Location = new System.Drawing.Point(37, 254);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(126, 13);
            this.label29.TabIndex = 66;
            this.label29.Text = "Placa de Vídeo e vRAM:";
            // 
            // lblMediaType
            // 
            this.lblMediaType.AutoSize = true;
            this.lblMediaType.ForeColor = System.Drawing.Color.Silver;
            this.lblMediaType.Location = new System.Drawing.Point(195, 202);
            this.lblMediaType.Name = "lblMediaType";
            this.lblMediaType.Size = new System.Drawing.Size(10, 13);
            this.lblMediaType.TabIndex = 65;
            this.lblMediaType.Text = "-";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label27.Location = new System.Drawing.Point(37, 202);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(124, 13);
            this.label27.TabIndex = 64;
            this.label27.Text = "Tipo de armazenamento:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBoxBattery);
            this.groupBox2.Controls.Add(this.comboBoxStandard);
            this.groupBox2.Controls.Add(this.comboBoxActiveDirectory);
            this.groupBox2.Controls.Add(this.comboBoxTag);
            this.groupBox2.Controls.Add(this.comboBoxInUse);
            this.groupBox2.Controls.Add(this.comboBoxType);
            this.groupBox2.Controls.Add(this.comboBoxBuilding);
            this.groupBox2.Controls.Add(this.lblAgentName);
            this.groupBox2.Controls.Add(this.label53);
            this.groupBox2.Controls.Add(this.lblPortServer);
            this.groupBox2.Controls.Add(this.lblIPServer);
            this.groupBox2.Controls.Add(this.label49);
            this.groupBox2.Controls.Add(this.label48);
            this.groupBox2.Controls.Add(this.label47);
            this.groupBox2.Controls.Add(this.configurableQualityPictureBox35);
            this.groupBox2.Controls.Add(this.label31);
            this.groupBox2.Controls.Add(this.textBoxTicket);
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
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.textBoxLetter);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox2.Location = new System.Drawing.Point(575, 113);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(450, 447);
            this.groupBox2.TabIndex = 66;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Dados do patrimônio, manutenção e de localização";
            // 
            // comboBoxBattery
            // 
            this.comboBoxBattery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxBattery.BorderColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxBattery.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxBattery.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBattery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxBattery.FormattingEnabled = true;
            this.comboBoxBattery.Location = new System.Drawing.Point(185, 241);
            this.comboBoxBattery.Name = "comboBoxBattery";
            this.comboBoxBattery.Size = new System.Drawing.Size(84, 21);
            this.comboBoxBattery.TabIndex = 47;
            // 
            // comboBoxStandard
            // 
            this.comboBoxStandard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxStandard.BorderColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxStandard.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStandard.Enabled = false;
            this.comboBoxStandard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxStandard.FormattingEnabled = true;
            this.comboBoxStandard.Location = new System.Drawing.Point(348, 215);
            this.comboBoxStandard.Name = "comboBoxStandard";
            this.comboBoxStandard.Size = new System.Drawing.Size(96, 21);
            this.comboBoxStandard.TabIndex = 46;
            // 
            // comboBoxActiveDirectory
            // 
            this.comboBoxActiveDirectory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxActiveDirectory.BorderColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxActiveDirectory.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxActiveDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxActiveDirectory.Enabled = false;
            this.comboBoxActiveDirectory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxActiveDirectory.FormattingEnabled = true;
            this.comboBoxActiveDirectory.Location = new System.Drawing.Point(185, 215);
            this.comboBoxActiveDirectory.Name = "comboBoxActiveDirectory";
            this.comboBoxActiveDirectory.Size = new System.Drawing.Size(84, 21);
            this.comboBoxActiveDirectory.TabIndex = 45;
            // 
            // comboBoxTag
            // 
            this.comboBoxTag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxTag.BorderColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxTag.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTag.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxTag.FormattingEnabled = true;
            this.comboBoxTag.Location = new System.Drawing.Point(384, 121);
            this.comboBoxTag.Name = "comboBoxTag";
            this.comboBoxTag.Size = new System.Drawing.Size(60, 21);
            this.comboBoxTag.TabIndex = 41;
            // 
            // comboBoxInUse
            // 
            this.comboBoxInUse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxInUse.BorderColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxInUse.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxInUse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInUse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxInUse.FormattingEnabled = true;
            this.comboBoxInUse.Location = new System.Drawing.Point(384, 95);
            this.comboBoxInUse.Name = "comboBoxInUse";
            this.comboBoxInUse.Size = new System.Drawing.Size(60, 21);
            this.comboBoxInUse.TabIndex = 39;
            // 
            // comboBoxType
            // 
            this.comboBoxType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxType.BorderColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxType.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Location = new System.Drawing.Point(185, 121);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(101, 21);
            this.comboBoxType.TabIndex = 40;
            // 
            // comboBoxBuilding
            // 
            this.comboBoxBuilding.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxBuilding.BorderColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxBuilding.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.comboBoxBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBuilding.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxBuilding.FormattingEnabled = true;
            this.comboBoxBuilding.Location = new System.Drawing.Point(185, 95);
            this.comboBoxBuilding.Name = "comboBoxBuilding";
            this.comboBoxBuilding.Size = new System.Drawing.Size(101, 21);
            this.comboBoxBuilding.TabIndex = 38;
            // 
            // lblAgentName
            // 
            this.lblAgentName.AutoSize = true;
            this.lblAgentName.ForeColor = System.Drawing.Color.Silver;
            this.lblAgentName.Location = new System.Drawing.Point(335, 422);
            this.lblAgentName.Name = "lblAgentName";
            this.lblAgentName.Size = new System.Drawing.Size(10, 13);
            this.lblAgentName.TabIndex = 123;
            this.lblAgentName.Text = "-";
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label53.Location = new System.Drawing.Point(225, 422);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(104, 13);
            this.label53.TabIndex = 122;
            this.label53.Text = "Agente responsável:";
            // 
            // lblPortServer
            // 
            this.lblPortServer.AutoSize = true;
            this.lblPortServer.ForeColor = System.Drawing.Color.Silver;
            this.lblPortServer.Location = new System.Drawing.Point(99, 422);
            this.lblPortServer.Name = "lblPortServer";
            this.lblPortServer.Size = new System.Drawing.Size(10, 13);
            this.lblPortServer.TabIndex = 121;
            this.lblPortServer.Text = "-";
            // 
            // lblIPServer
            // 
            this.lblIPServer.AutoSize = true;
            this.lblIPServer.ForeColor = System.Drawing.Color.Silver;
            this.lblIPServer.Location = new System.Drawing.Point(99, 403);
            this.lblIPServer.Name = "lblIPServer";
            this.lblIPServer.Size = new System.Drawing.Size(10, 13);
            this.lblIPServer.TabIndex = 120;
            this.lblIPServer.Text = "-";
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
            this.label48.Location = new System.Drawing.Point(367, 244);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(17, 13);
            this.label48.TabIndex = 118;
            this.label48.Text = "✱";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.label47.Location = new System.Drawing.Point(146, 244);
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
            this.configurableQualityPictureBox35.Location = new System.Drawing.Point(273, 238);
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
            this.label31.Location = new System.Drawing.Point(303, 244);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(69, 13);
            this.label31.TabIndex = 116;
            this.label31.Text = "Nº chamado:";
            // 
            // textBoxTicket
            // 
            this.textBoxTicket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.textBoxTicket.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxTicket.Location = new System.Drawing.Point(384, 241);
            this.textBoxTicket.MaxLength = 6;
            this.textBoxTicket.Name = "textBoxTicket";
            this.textBoxTicket.Size = new System.Drawing.Size(60, 20);
            this.textBoxTicket.TabIndex = 48;
            this.textBoxTicket.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumbersOnly_KeyPress);
            // 
            // configurableQualityPictureBox34
            // 
            this.configurableQualityPictureBox34.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox34.Image = global::HardwareInformation.Properties.Resources.cmos_battery_white;
            this.configurableQualityPictureBox34.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox34.Location = new System.Drawing.Point(7, 238);
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
            this.label42.Location = new System.Drawing.Point(103, 176);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(17, 13);
            this.label42.TabIndex = 112;
            this.label42.Text = "✱";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.label41.Location = new System.Drawing.Point(367, 124);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(17, 13);
            this.label41.TabIndex = 111;
            this.label41.Text = "✱";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label46.Location = new System.Drawing.Point(37, 244);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(112, 13);
            this.label46.TabIndex = 111;
            this.label46.Text = "Houve troca de pilha?";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.label40.Location = new System.Drawing.Point(64, 124);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(17, 13);
            this.label40.TabIndex = 110;
            this.label40.Text = "✱";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.label39.Location = new System.Drawing.Point(363, 98);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(17, 13);
            this.label39.TabIndex = 109;
            this.label39.Text = "✱";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.label38.Location = new System.Drawing.Point(73, 98);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(17, 13);
            this.label38.TabIndex = 108;
            this.label38.Text = "✱";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.label37.Location = new System.Drawing.Point(167, 72);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(17, 13);
            this.label37.TabIndex = 107;
            this.label37.Text = "✱";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.label36.Location = new System.Drawing.Point(92, 20);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(17, 13);
            this.label36.TabIndex = 106;
            this.label36.Text = "✱";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.label35.Location = new System.Drawing.Point(258, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(152, 13);
            this.label35.TabIndex = 105;
            this.label35.Text = "✱ = Preenchimento obrigatório";
            // 
            // studentRadioButton
            // 
            this.studentRadioButton.AutoSize = true;
            this.studentRadioButton.Location = new System.Drawing.Point(185, 192);
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
            this.employeeRadioButton.Location = new System.Drawing.Point(185, 174);
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
            this.configurableQualityPictureBox31.Location = new System.Drawing.Point(7, 170);
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
            this.label34.Location = new System.Drawing.Point(37, 176);
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
            this.configurableQualityPictureBox25.Location = new System.Drawing.Point(292, 66);
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
            this.configurableQualityPictureBox29.Location = new System.Drawing.Point(7, 399);
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
            this.configurableQualityPictureBox28.Location = new System.Drawing.Point(7, 118);
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
            this.configurableQualityPictureBox27.Location = new System.Drawing.Point(292, 118);
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
            this.configurableQualityPictureBox26.Location = new System.Drawing.Point(292, 92);
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
            this.configurableQualityPictureBox24.Location = new System.Drawing.Point(7, 144);
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
            this.configurableQualityPictureBox23.Location = new System.Drawing.Point(273, 212);
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
            this.configurableQualityPictureBox22.Location = new System.Drawing.Point(7, 212);
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
            this.configurableQualityPictureBox21.Location = new System.Drawing.Point(7, 92);
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
            this.configurableQualityPictureBox20.Location = new System.Drawing.Point(7, 66);
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
            this.configurableQualityPictureBox19.Location = new System.Drawing.Point(7, 40);
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
            this.configurableQualityPictureBox18.Location = new System.Drawing.Point(7, 14);
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
            this.dateTimePicker1.Location = new System.Drawing.Point(185, 148);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(259, 20);
            this.dateTimePicker1.TabIndex = 42;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.BackColor = System.Drawing.Color.Transparent;
            this.label26.ForeColor = System.Drawing.Color.Silver;
            this.label26.Location = new System.Drawing.Point(335, 403);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(10, 13);
            this.label26.TabIndex = 72;
            this.label26.Text = "-";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.loadingCircle21);
            this.groupBox3.Controls.Add(this.loadingCircle20);
            this.groupBox3.Controls.Add(this.lblMaintenanceSince);
            this.groupBox3.Controls.Add(this.lblInstallSince);
            this.groupBox3.Controls.Add(this.label43);
            this.groupBox3.Controls.Add(this.textBox5);
            this.groupBox3.Controls.Add(this.textBox6);
            this.groupBox3.Controls.Add(this.formatButton);
            this.groupBox3.Controls.Add(this.maintenanceButton);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox3.Location = new System.Drawing.Point(6, 269);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(438, 124);
            this.groupBox3.TabIndex = 72;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tipo de serviço";
            // 
            // loadingCircle21
            // 
            this.loadingCircle21.Active = false;
            this.loadingCircle21.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle21.InnerCircleRadius = 8;
            this.loadingCircle21.Location = new System.Drawing.Point(89, 57);
            this.loadingCircle21.Name = "loadingCircle21";
            this.loadingCircle21.NumberSpoke = 24;
            this.loadingCircle21.OuterCircleRadius = 9;
            this.loadingCircle21.RotationSpeed = 20;
            this.loadingCircle21.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle21.SpokeThickness = 4;
            this.loadingCircle21.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle21.TabIndex = 133;
            this.loadingCircle21.Text = "loadingCircle21";
            // 
            // loadingCircle20
            // 
            this.loadingCircle20.Active = false;
            this.loadingCircle20.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle20.InnerCircleRadius = 8;
            this.loadingCircle20.Location = new System.Drawing.Point(89, 16);
            this.loadingCircle20.Name = "loadingCircle20";
            this.loadingCircle20.NumberSpoke = 24;
            this.loadingCircle20.OuterCircleRadius = 9;
            this.loadingCircle20.RotationSpeed = 20;
            this.loadingCircle20.Size = new System.Drawing.Size(37, 25);
            this.loadingCircle20.SpokeThickness = 4;
            this.loadingCircle20.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle20.TabIndex = 132;
            this.loadingCircle20.Text = "loadingCircle20";
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
            this.formatButton.TabIndex = 49;
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
            this.maintenanceButton.TabIndex = 50;
            this.maintenanceButton.Text = "Manutenção";
            this.maintenanceButton.UseVisualStyleBackColor = true;
            this.maintenanceButton.CheckedChanged += new System.EventHandler(this.maintenanceButton2_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label15.Location = new System.Drawing.Point(37, 218);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(137, 13);
            this.label15.TabIndex = 14;
            this.label15.Text = "Cadastrado no servidor AD:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label17.Location = new System.Drawing.Point(303, 218);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(44, 13);
            this.label17.TabIndex = 15;
            this.label17.Text = "Padrão:";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)));
            this.toolStripStatusLabel2.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripStatusLabel2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(4, 17);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comboBoxTheme,
            this.logLabel,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 693);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1056, 22);
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
            this.comboBoxTheme.Size = new System.Drawing.Size(48, 20);
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
            // logLabel
            // 
            this.logLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(27, 17);
            this.logLabel.Text = "Log";
            this.logLabel.Click += new System.EventHandler(this.logLabel_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.toolStripStatusLabel1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)(((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(931, 17);
            this.toolStripStatusLabel1.Spring = true;
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
            this.groupBox4.Size = new System.Drawing.Size(450, 65);
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
            this.webView2.Size = new System.Drawing.Size(448, 51);
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
            // loadingCircle22
            // 
            this.loadingCircle22.Active = false;
            this.loadingCircle22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.loadingCircle22.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle22.InnerCircleRadius = 8;
            this.loadingCircle22.Location = new System.Drawing.Point(576, 634);
            this.loadingCircle22.Name = "loadingCircle22";
            this.loadingCircle22.NumberSpoke = 24;
            this.loadingCircle22.OuterCircleRadius = 9;
            this.loadingCircle22.RotationSpeed = 20;
            this.loadingCircle22.Size = new System.Drawing.Size(178, 23);
            this.loadingCircle22.SpokeThickness = 4;
            this.loadingCircle22.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle22.TabIndex = 134;
            this.loadingCircle22.Text = "loadingCircle22";
            // 
            // loadingCircle23
            // 
            this.loadingCircle23.Active = false;
            this.loadingCircle23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.loadingCircle23.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle23.InnerCircleRadius = 8;
            this.loadingCircle23.Location = new System.Drawing.Point(761, 634);
            this.loadingCircle23.Name = "loadingCircle23";
            this.loadingCircle23.NumberSpoke = 24;
            this.loadingCircle23.OuterCircleRadius = 9;
            this.loadingCircle23.RotationSpeed = 20;
            this.loadingCircle23.Size = new System.Drawing.Size(263, 54);
            this.loadingCircle23.SpokeThickness = 4;
            this.loadingCircle23.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.loadingCircle23.TabIndex = 134;
            this.loadingCircle23.Text = "loadingCircle23";
            this.loadingCircle23.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(1056, 715);
            this.Controls.Add(this.loadingCircle23);
            this.Controls.Add(this.loadingCircle22);
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
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
        private Label label16;
        private Label lblOS;
        private Label label18;
        private Label label19;
        private Button registerButton;
        private Label label20;
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
        private BusyForm bw;

        private bool pass = true;
        private bool themeBool, serverOnline, offlineMode;
        private List<string> date;
        private string servidor_web, porta, modeURL, user, ip, port;
        private string BM, Model, SerialNo, ProcName, PM, HDSize, MediaType,
           MediaOperation, GPUInfo, OS, Hostname, Mac, IP, BIOS, BIOSType, SecBoot, VT, Smart, TPM;
        private string[] sArgs = new string[34];
        private Label lblPortServer;
        private Label lblIPServer;
        private Label lblAgentName;
        private Label label53;
        private Timer timer9;
        private Timer timer10;
        private ConfigurableQualityPictureBox configurableQualityPictureBox23;
        private ConfigurableQualityPictureBox configurableQualityPictureBox22;
        private Label label15;
        private Label label17;
        private Label separatorV;
        private Label separatorH;
        private CustomFlatComboBox comboBoxBuilding;
        private CustomFlatComboBox comboBoxStandard;
        private CustomFlatComboBox comboBoxActiveDirectory;
        private CustomFlatComboBox comboBoxTag;
        private CustomFlatComboBox comboBoxInUse;
        private CustomFlatComboBox comboBoxType;
        private CustomFlatComboBox comboBoxBattery;
        private LoadingCircle loadingCircle19;
        private LoadingCircle loadingCircle18;
        private LoadingCircle loadingCircle17;
        private LoadingCircle loadingCircle16;
        private LoadingCircle loadingCircle15;
        private LoadingCircle loadingCircle14;
        private LoadingCircle loadingCircle13;
        private LoadingCircle loadingCircle12;
        private LoadingCircle loadingCircle11;
        private LoadingCircle loadingCircle10;
        private LoadingCircle loadingCircle9;
        private LoadingCircle loadingCircle8;
        private LoadingCircle loadingCircle7;
        private LoadingCircle loadingCircle6;
        private LoadingCircle loadingCircle5;
        private LoadingCircle loadingCircle4;
        private LoadingCircle loadingCircle3;
        private LoadingCircle loadingCircle2;
        private LoadingCircle loadingCircle1;
        private LoadingCircle loadingCircle21;
        private LoadingCircle loadingCircle20;
        private LoadingCircle loadingCircle22;
        private LoadingCircle loadingCircle23;
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
            comboBoxActiveDirectory.SelectedIndex = 1;
            comboBoxStandard.SelectedIndex = 0;
        }

        //Sets service to student
        private void studentButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxActiveDirectory.SelectedIndex = 0;
            comboBoxStandard.SelectedIndex = 1;
        }

        //Method for auto selecting the app theme
        private void comboBoxThemeInit()
        {
            themeBool = MiscMethods.ThemeInit();
            if (themeBool)
                darkTheme();
            else
                lightTheme();
        }

        //Method for logging out
        private void logoutLabel_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_LOGOUT, string.Empty, StringsAndConstants.consoleOutGUI);
            this.DialogResult = DialogResult.OK;
        }

        //Method for setting the auto theme
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_AUTOTHEME_CHANGE, string.Empty, StringsAndConstants.consoleOutGUI);
            comboBoxThemeInit();
        }

        //Method for setting the light theme
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_LIGHTMODE_CHANGE, string.Empty, StringsAndConstants.consoleOutGUI);
            lightTheme();
            themeBool = false;
        }

        //Method for setting the dark theme
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_DARKMODE_CHANGE, string.Empty, StringsAndConstants.consoleOutGUI);
            darkTheme();
            themeBool = true;
        }

        private void logLabel_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_OPENING_LOG, string.Empty, StringsAndConstants.consoleOutGUI);
#if DEBUG
            System.Diagnostics.Process.Start(StringsAndConstants.LOGFILE_LOCATION + StringsAndConstants.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.dev_status + StringsAndConstants.LOG_FILE_EXT);
#else
            System.Diagnostics.Process.Start(StringsAndConstants.LOGFILE_LOCATION + StringsAndConstants.LOG_FILENAME_CP + "-v" + Application.ProductVersion + StringsAndConstants.LOG_FILE_EXT);
#endif
        }

        //Sets a light theme for the UI
        private void lightTheme()
        {
            this.BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            this.lblBM.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblModel.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblSerialNo.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblProcName.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblPM.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblHDSize.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblMediaType.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblMediaOperation.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblOS.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblHostname.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblMac.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblIP.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblBIOS.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblBIOSType.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblTPM.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblVT.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblSecBoot.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblGPUInfo.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblVT.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblSmart.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblIPServer.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblPortServer.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            this.lblAgentName.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;

            this.lblInstallSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            this.lblMaintenanceSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
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
            this.label14.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label15.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label16.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label17.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label18.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label19.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label20.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label21.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label22.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label23.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label24.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label25.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label27.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label29.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label28.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.label30.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
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
            this.label53.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            if (offlineMode)
            {
                this.lblIPServer.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                this.lblPortServer.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                this.lblAgentName.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }
            this.loadingCircle22.BackColor = StringsAndConstants.INACTIVE_SYSTEM_BUTTON_COLOR;
            this.loadingCircle23.BackColor = StringsAndConstants.INACTIVE_SYSTEM_BUTTON_COLOR;

            this.textBoxPatrimony.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxPatrimony.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBoxSeal.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxSeal.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBoxRoom.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxRoom.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBoxLetter.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxLetter.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBox5.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBox5.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.textBox6.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBox6.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.textBoxTicket.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.textBoxTicket.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;

            this.comboBoxBuilding.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxBuilding.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxBuilding.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxBuilding.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxActiveDirectory.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxActiveDirectory.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxActiveDirectory.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxActiveDirectory.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxStandard.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxStandard.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxStandard.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxStandard.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxInUse.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxInUse.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxInUse.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxInUse.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxTag.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxTag.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxTag.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxTag.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxType.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxType.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxType.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxBattery.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxBattery.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxBattery.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.comboBoxBattery.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.comboBoxTheme.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.comboBoxTheme.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            this.registerButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.registerButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.registerButton.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.registerButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.collectButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.collectButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.collectButton.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.collectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.accessSystemButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.accessSystemButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.accessSystemButton.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.accessSystemButton.FlatStyle = System.Windows.Forms.FlatStyle.System;

            this.groupBox1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.groupBox2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.groupBox3.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.groupBox4.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.groupBox1.Paint += CustomGroupBox.groupBox_PaintLightTheme;
            this.groupBox2.Paint += CustomGroupBox.groupBox_PaintLightTheme;
            this.groupBox3.Paint += CustomGroupBox.groupBox_PaintLightTheme;
            this.groupBox4.Paint += CustomGroupBox.groupBox_PaintLightTheme;
            this.separatorH.BackColor = StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR;
            this.separatorV.BackColor = StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR;

            this.toolStripStatusLabel1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripStatusLabel1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.toolStripStatusLabel2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripStatusLabel2.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.toolStripMenuItem1.BackgroundImage = global::HardwareInformation.Properties.Resources.lightback;
            this.toolStripMenuItem1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripMenuItem2.BackgroundImage = global::HardwareInformation.Properties.Resources.lightback;
            this.toolStripMenuItem2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.toolStripMenuItem3.BackgroundImage = global::HardwareInformation.Properties.Resources.lightback;
            this.toolStripMenuItem3.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.logLabel.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.logLabel.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            this.statusStrip1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;

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
        }

        //Sets a dark theme for the UI
        private void darkTheme()
        {
            this.BackColor = StringsAndConstants.DARK_BACKGROUND;

            this.lblBM.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblModel.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblSerialNo.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblProcName.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblPM.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblHDSize.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblMediaType.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblMediaOperation.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblOS.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblHostname.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblMac.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblIP.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblBIOS.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblBIOSType.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblTPM.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblVT.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblSecBoot.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblGPUInfo.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblVT.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblSmart.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblIPServer.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblPortServer.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            this.lblAgentName.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;

            this.lblInstallSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            this.lblMaintenanceSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
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
            this.label14.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label15.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label16.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label17.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label18.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label19.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label20.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label21.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label22.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label23.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label24.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label25.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label27.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label28.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label29.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.label30.ForeColor = StringsAndConstants.DARK_FORECOLOR;
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
            this.label53.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            if (offlineMode)
            {
                this.lblIPServer.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                this.lblPortServer.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                this.lblAgentName.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }
            this.loadingCircle22.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.loadingCircle23.BackColor = StringsAndConstants.DARK_BACKCOLOR;

            this.textBoxPatrimony.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.textBoxPatrimony.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBoxSeal.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.textBoxSeal.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBoxRoom.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.textBoxRoom.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBoxLetter.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.textBoxLetter.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBox5.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBox5.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.textBox6.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBox6.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.textBoxTicket.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.textBoxTicket.BackColor = StringsAndConstants.DARK_BACKCOLOR;

            this.comboBoxBuilding.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxBuilding.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxBuilding.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxBuilding.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxActiveDirectory.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxActiveDirectory.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxActiveDirectory.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxActiveDirectory.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxStandard.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxStandard.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxStandard.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxStandard.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxInUse.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxInUse.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxInUse.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxInUse.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxTag.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxTag.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxTag.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxTag.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxType.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxType.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxType.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxBattery.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxBattery.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxBattery.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            this.comboBoxBattery.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            this.comboBoxTheme.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.comboBoxTheme.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            this.registerButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.registerButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.registerButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            this.registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.collectButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.collectButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.collectButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            this.collectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.accessSystemButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.accessSystemButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.accessSystemButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            this.accessSystemButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            this.groupBox1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.groupBox2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.groupBox3.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.groupBox4.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.groupBox1.Paint += CustomGroupBox.groupBox_PaintDarkTheme;
            this.groupBox2.Paint += CustomGroupBox.groupBox_PaintDarkTheme;
            this.groupBox3.Paint += CustomGroupBox.groupBox_PaintDarkTheme;
            this.groupBox4.Paint += CustomGroupBox.groupBox_PaintDarkTheme;
            this.separatorH.BackColor = StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR;
            this.separatorV.BackColor = StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR;

            this.toolStripStatusLabel1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripStatusLabel1.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.toolStripStatusLabel2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripStatusLabel2.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.toolStripMenuItem1.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
            this.toolStripMenuItem1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripMenuItem2.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
            this.toolStripMenuItem2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.toolStripMenuItem3.BackgroundImage = global::HardwareInformation.Properties.Resources.darkback;
            this.toolStripMenuItem3.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.logLabel.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.logLabel.BackColor = StringsAndConstants.DARK_BACKGROUND;
            this.statusStrip1.BackColor = StringsAndConstants.DARK_BACKGROUND;

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
        }

        //Opens the selected webpage, according to the IP and port specified in the comboboxes
        private void accessButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_VIEW_SERVER, string.Empty, StringsAndConstants.consoleOutGUI);
            System.Diagnostics.Process.Start("http://" + ip + ":" + port);
        }

        //Handles the closing of the current form
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CLOSING_MAINFORM, string.Empty, StringsAndConstants.consoleOutGUI);
            log.LogWrite(StringsAndConstants.LOG_MISC, StringsAndConstants.LOG_SEPARATOR_SMALL, string.Empty, StringsAndConstants.consoleOutGUI);
            File.Delete(StringsAndConstants.biosPath);
            File.Delete(StringsAndConstants.loginPath);
            webView2.Dispose();
            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }

        //Loads the form, sets some combobox values, create timers (1000 ms cadence), and triggers a hardware collection
        private async void Form1_Load(object sender, EventArgs e)
        {
            comboBoxThemeInit();
            if (!offlineMode)
            {
                bw = new BusyForm();
                bw.Visible = true;
                await loadWebView2();
                bw.Visible = false;
            }
            timer1.Tick += new EventHandler(flashTextHostname);
            timer2.Tick += new EventHandler(flashTextMediaOp);
            timer3.Tick += new EventHandler(flashTextSecBoot);
            timer4.Tick += new EventHandler(flashTextBIOSVersion);
            timer5.Tick += new EventHandler(flashTextNetConnectivity);
            timer6.Tick += new EventHandler(flashTextBIOSType);
            timer7.Tick += new EventHandler(flashTextVT);
            timer8.Tick += new EventHandler(flashTextSmart);
            timer9.Tick += new EventHandler(flashTextTPM);
            timer10.Tick += new EventHandler(flashTextRAM);
            timer1.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer2.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer3.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer4.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer5.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer6.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer7.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer8.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer9.Interval = StringsAndConstants.TIMER_INTERVAL;
            timer10.Interval = StringsAndConstants.TIMER_INTERVAL;
            lblIPServer.Text = ip;
            lblPortServer.Text = port;
            lblAgentName.Text = this.user.ToUpper();
            dateTimePicker1.MaxDate = DateTime.Today;
            date = new List<string>();
            FormClosing += Form1_FormClosing;
            tbProgMain = TaskbarManager.Instance;
            coleta_Click(sender, e);
        }

        //Restricts textbox4 only with chars
        private void textBoxCharsOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back))
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
                lblHostname.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            else if (lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblHostname.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            else
                lblHostname.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the MediaOperations label to flash in red
        private void flashTextMediaOp(Object myobject, EventArgs myEventArgs)
        {
            if (lblMediaOperation.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblMediaOperation.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            else if (lblMediaOperation.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblMediaOperation.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            else
                lblMediaOperation.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the Secure Boot label to flash in red
        private void flashTextSecBoot(Object myobject, EventArgs myEventArgs)
        {
            if (lblSecBoot.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblSecBoot.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            else if (lblSecBoot.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblSecBoot.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            else
                lblSecBoot.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the VT label to flash in red
        private void flashTextVT(Object myobject, EventArgs myEventArgs)
        {
            if (lblVT.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblVT.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            else if (lblVT.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblVT.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            else
                lblVT.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the SMART label to flash in red
        private void flashTextSmart(Object myobject, EventArgs myEventArgs)
        {
            if (lblSmart.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblSmart.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            else if (lblSmart.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblSmart.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            else
                lblSmart.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the BIOS Version label to flash in red
        private void flashTextBIOSVersion(Object myobject, EventArgs myEventArgs)
        {
            if (lblBIOS.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblBIOS.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            else if (lblBIOS.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblBIOS.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            else
                lblBIOS.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the Mac and IP labels to flash in red
        private void flashTextNetConnectivity(Object myobject, EventArgs myEventArgs)
        {
            if (lblMac.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
            {
                lblMac.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblIP.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            }
            else if (lblMac.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
            {
                lblMac.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblIP.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
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
                lblBIOSType.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            else if (lblBIOSType.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblBIOSType.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            else
                lblBIOSType.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the Physical Memory label to flash in red
        private void flashTextRAM(Object myobject, EventArgs myEventArgs)
        {
            if (lblPM.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblPM.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            else if (lblPM.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblPM.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            else
                lblPM.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Sets the TPM label to flash in red
        private void flashTextTPM(Object myobject, EventArgs myEventArgs)
        {
            if (lblTPM.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
                lblTPM.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            else if (lblTPM.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
                lblTPM.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            else
                lblTPM.ForeColor = StringsAndConstants.ALERT_COLOR;
        }

        //Starts the collection process
        private async void collecting()
        {
            //Writes a dash in the labels, while collects data
            lblInstallSince.Text = StringsAndConstants.DASH;
            lblMaintenanceSince.Text = StringsAndConstants.DASH;
            lblBM.Text = StringsAndConstants.DASH;
            lblModel.Text = StringsAndConstants.DASH;
            lblSerialNo.Text = StringsAndConstants.DASH;
            lblProcName.Text = StringsAndConstants.DASH;
            lblPM.Text = StringsAndConstants.DASH;
            lblHDSize.Text = StringsAndConstants.DASH;
            lblSmart.Text = StringsAndConstants.DASH;
            lblMediaType.Text = StringsAndConstants.DASH;
            lblMediaOperation.Text = StringsAndConstants.DASH;
            lblGPUInfo.Text = StringsAndConstants.DASH;
            lblOS.Text = StringsAndConstants.DASH;
            lblHostname.Text = StringsAndConstants.DASH;
            lblMac.Text = StringsAndConstants.DASH;
            lblIP.Text = StringsAndConstants.DASH;
            lblBIOS.Text = StringsAndConstants.DASH;
            lblBIOSType.Text = StringsAndConstants.DASH;
            lblSecBoot.Text = StringsAndConstants.DASH;
            lblVT.Text = StringsAndConstants.DASH;
            lblTPM.Text = StringsAndConstants.DASH;
            collectButton.Text = StringsAndConstants.DASH;

            loadingCircle1.Visible = true;
            loadingCircle2.Visible = true;
            loadingCircle3.Visible = true;
            loadingCircle4.Visible = true;
            loadingCircle5.Visible = true;
            loadingCircle6.Visible = true;
            loadingCircle7.Visible = true;
            loadingCircle8.Visible = true;
            loadingCircle9.Visible = true;
            loadingCircle10.Visible = true;
            loadingCircle11.Visible = true;
            loadingCircle12.Visible = true;
            loadingCircle13.Visible = true;
            loadingCircle14.Visible = true;
            loadingCircle15.Visible = true;
            loadingCircle16.Visible = true;
            loadingCircle17.Visible = true;
            loadingCircle18.Visible = true;
            loadingCircle19.Visible = true;
            loadingCircle20.Visible = true;
            loadingCircle21.Visible = true;
            loadingCircle22.Visible = true;

            loadingCircle1.Active = true;
            loadingCircle2.Active = true;
            loadingCircle3.Active = true;
            loadingCircle4.Active = true;
            loadingCircle5.Active = true;
            loadingCircle6.Active = true;
            loadingCircle7.Active = true;
            loadingCircle8.Active = true;
            loadingCircle9.Active = true;
            loadingCircle10.Active = true;
            loadingCircle11.Active = true;
            loadingCircle12.Active = true;
            loadingCircle13.Active = true;
            loadingCircle14.Active = true;
            loadingCircle15.Active = true;
            loadingCircle16.Active = true;
            loadingCircle17.Active = true;
            loadingCircle18.Active = true;
            loadingCircle19.Active = true;
            loadingCircle20.Active = true;
            loadingCircle21.Active = true;
            loadingCircle22.Active = true;

            if (!offlineMode)
            {
                servidor_web = ip;
                porta = port;
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_PINGGING_SERVER, string.Empty, StringsAndConstants.consoleOutGUI);
                serverOnline = await BIOSFileReader.checkHostMT(servidor_web, porta);
                if (serverOnline && porta != "")
                {
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_ONLINE_SERVER, string.Empty, StringsAndConstants.consoleOutGUI);
                    label26.Text = StringsAndConstants.ONLINE;
                    label26.ForeColor = StringsAndConstants.ONLINE_ALERT;
                }
                else
                {
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_OFFLINE_SERVER, string.Empty, StringsAndConstants.consoleOutGUI);
                    label26.Text = StringsAndConstants.OFFLINE;
                    label26.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                }
            }
            else
            {
                lblIPServer.Text = lblPortServer.Text = lblAgentName.Text = label26.Text = StringsAndConstants.OFFLINE_MODE_ACTIVATED;
                lblIPServer.ForeColor = lblPortServer.ForeColor = lblAgentName.ForeColor = label26.ForeColor = StringsAndConstants.OFFLINE_ALERT;
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
            timer9.Enabled = false;
            timer10.Enabled = false;
            if (lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR || lblMediaOperation.ForeColor == StringsAndConstants.ALERT_COLOR || lblSecBoot.ForeColor == StringsAndConstants.ALERT_COLOR || lblBIOS.ForeColor == StringsAndConstants.ALERT_COLOR || lblVT.ForeColor == StringsAndConstants.ALERT_COLOR || lblSmart.ForeColor == StringsAndConstants.ALERT_COLOR || lblPM.ForeColor == StringsAndConstants.ALERT_COLOR || lblTPM.ForeColor == StringsAndConstants.ALERT_COLOR)
            {
                if (themeBool)
                {
                    lblHostname.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                    lblMediaOperation.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                    lblSecBoot.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                    lblBIOS.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                    lblVT.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                    lblSmart.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                    lblPM.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                    lblTPM.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                }
                else
                {
                    lblHostname.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                    lblMediaOperation.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                    lblSecBoot.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                    lblBIOS.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                    lblVT.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                    lblSmart.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                    lblPM.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                    lblTPM.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                }
            }

            //Sets current and maximum values for the progressbar
            progressBar1.Maximum = 19;
            progressBar1.Value = 0;
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
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_START_COLLECTING, string.Empty, StringsAndConstants.consoleOutGUI);

            i = 0;

            BM = HardwareInfo.GetBoardMaker();
            if (BM == StringsAndConstants.ToBeFilledByOEM || BM == "")
                BM = HardwareInfo.GetBoardMakerAlt();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_BM, BM, StringsAndConstants.consoleOutGUI);

            Model = HardwareInfo.GetModel();
            if (Model == StringsAndConstants.ToBeFilledByOEM || Model == "")
                Model = HardwareInfo.GetModelAlt();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_MODEL, Model, StringsAndConstants.consoleOutGUI);

            SerialNo = HardwareInfo.GetBoardProductId();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SERIALNO, SerialNo, StringsAndConstants.consoleOutGUI);

            ProcName = HardwareInfo.GetProcessorCores();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_PROCNAME, ProcName, StringsAndConstants.consoleOutGUI);

            PM = HardwareInfo.GetPhysicalMemory() + " (" + HardwareInfo.GetNumFreeRamSlots(Convert.ToInt32(HardwareInfo.GetNumRamSlots())) +
                " slots de " + HardwareInfo.GetNumRamSlots() + " ocupados" + ")";
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_PM, PM, StringsAndConstants.consoleOutGUI);

            HDSize = HardwareInfo.GetHDSize();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_HDSIZE, HDSize, StringsAndConstants.consoleOutGUI);

            Smart = HardwareInfo.GetSMARTStatus();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SMART, Smart, StringsAndConstants.consoleOutGUI);

            MediaType = HardwareInfo.GetStorageType();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_MEDIATYPE, MediaType, StringsAndConstants.consoleOutGUI);

            MediaOperation = HardwareInfo.GetStorageOperation();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_MEDIAOP, MediaOperation, StringsAndConstants.consoleOutGUI);

            GPUInfo = HardwareInfo.GetGPUInfo();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_GPUINFO, GPUInfo, StringsAndConstants.consoleOutGUI);

            OS = HardwareInfo.GetOSInformation();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_OS, OS, StringsAndConstants.consoleOutGUI);

            Hostname = HardwareInfo.GetComputerName();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_HOSTNAME, Hostname, StringsAndConstants.consoleOutGUI);

            Mac = HardwareInfo.GetMACAddress();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_MAC, Mac, StringsAndConstants.consoleOutGUI);

            IP = HardwareInfo.GetIPAddress();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_IP, IP, StringsAndConstants.consoleOutGUI);

            BIOSType = HardwareInfo.GetBIOSType();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_BIOSTYPE, BIOSType, StringsAndConstants.consoleOutGUI);

            SecBoot = HardwareInfo.GetSecureBoot();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SECBOOT, SecBoot, StringsAndConstants.consoleOutGUI);

            BIOS = HardwareInfo.GetComputerBIOS();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_BIOS, BIOS, StringsAndConstants.consoleOutGUI);

            VT = HardwareInfo.GetVirtualizationTechnology();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_VT, VT, StringsAndConstants.consoleOutGUI);

            TPM = HardwareInfo.GetTPMStatus();
            i++;
            worker.ReportProgress(progressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_TPM, TPM, StringsAndConstants.consoleOutGUI);

            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_END_COLLECTING, string.Empty, StringsAndConstants.consoleOutGUI);
        }

        //Prints the collected data into the form labels, warning the user when there are forbidden modes
        private async void printHardwareData()
        {
            loadingCircle1.Visible = false;
            loadingCircle2.Visible = false;
            loadingCircle3.Visible = false;
            loadingCircle4.Visible = false;
            loadingCircle5.Visible = false;
            loadingCircle6.Visible = false;
            loadingCircle7.Visible = false;
            loadingCircle8.Visible = false;
            loadingCircle9.Visible = false;
            loadingCircle10.Visible = false;
            loadingCircle11.Visible = false;
            loadingCircle12.Visible = false;
            loadingCircle13.Visible = false;
            loadingCircle14.Visible = false;
            loadingCircle15.Visible = false;
            loadingCircle16.Visible = false;
            loadingCircle17.Visible = false;
            loadingCircle18.Visible = false;
            loadingCircle19.Visible = false;
            loadingCircle20.Visible = false;
            loadingCircle21.Visible = false;
            loadingCircle22.Visible = false;

            loadingCircle1.Active = false;
            loadingCircle2.Active = false;
            loadingCircle3.Active = false;
            loadingCircle4.Active = false;
            loadingCircle5.Active = false;
            loadingCircle6.Active = false;
            loadingCircle7.Active = false;
            loadingCircle8.Active = false;
            loadingCircle9.Active = false;
            loadingCircle10.Active = false;
            loadingCircle11.Active = false;
            loadingCircle12.Active = false;
            loadingCircle13.Active = false;
            loadingCircle14.Active = false;
            loadingCircle15.Active = false;
            loadingCircle16.Active = false;
            loadingCircle17.Active = false;
            loadingCircle18.Active = false;
            loadingCircle19.Active = false;
            loadingCircle20.Active = false;
            loadingCircle21.Active = false;
            loadingCircle22.Active = false;

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
            lblInstallSince.Text = MiscMethods.sinceLabelUpdate(true);
            lblMaintenanceSince.Text = MiscMethods.sinceLabelUpdate(false);
            log.LogWrite(StringsAndConstants.LOG_INFO, lblInstallSince.Text, string.Empty, StringsAndConstants.consoleOutGUI);
            log.LogWrite(StringsAndConstants.LOG_INFO, lblMaintenanceSince.Text, string.Empty, StringsAndConstants.consoleOutGUI);

            if(!offlineMode)
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_FETCHING_BIOSFILE, string.Empty, StringsAndConstants.consoleOutGUI);
            try
            {
                string[] str = await BIOSFileReader.fetchInfoMT(lblBM.Text, lblModel.Text, lblBIOSType.Text, lblTPM.Text, lblMediaOperation.Text, ip, port);

                //Scan if hostname is the default one
                if (lblHostname.Text.Equals(StringsAndConstants.DEFAULT_HOSTNAME))
                {
                    pass = false;
                    lblHostname.Text += StringsAndConstants.HOSTNAME_ALERT;
                    timer1.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_HOSTNAME_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                //The section below contains the exception cases for AHCI enforcement
                if (str != null && str[3].Equals("false"))
                {
                    pass = false;
                    lblMediaOperation.Text += StringsAndConstants.MEDIA_OPERATION_ALERT;
                    timer2.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_MEDIAOP_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                //The section below contains the exception cases for Secure Boot enforcement
                if (lblSecBoot.Text.Equals(StringsAndConstants.deactivated) &&
                    !lblGPUInfo.Text.Contains(StringsAndConstants.nonSecBootGPU1) &&
                    !lblGPUInfo.Text.Contains(StringsAndConstants.nonSecBootGPU2))
                {
                    pass = false;
                    lblSecBoot.Text += StringsAndConstants.SECURE_BOOT_ALERT;
                    timer3.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_SECBOOT_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                if (str == null)
                {
                    if (!offlineMode)
                    {
                        pass = false;
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_OFFLINE_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
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
                        log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_BIOSVER_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                    }
                }
                if (str != null && str[1].Equals("false"))
                {
                    pass = false;
                    lblBIOSType.Text += StringsAndConstants.FIRMWARE_TYPE_ALERT;
                    timer6.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_FIRMWARE_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                if (lblMac.Text == "")
                {
                    if (!offlineMode)
                    {
                        pass = false;
                        lblMac.Text = StringsAndConstants.NETWORK_ERROR;
                        lblIP.Text = StringsAndConstants.NETWORK_ERROR;
                        timer5.Enabled = true;
                        log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_NETWORK_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
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
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_VT_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                if (!lblSmart.Text.Contains(StringsAndConstants.ok))
                {
                    pass = false;
                    lblSmart.Text += StringsAndConstants.SMART_FAIL;
                    timer8.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_SMART_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                if (str != null && str[2].Equals("false"))
                {
                    pass = false;
                    lblTPM.Text += StringsAndConstants.TPM_ERROR;
                    timer9.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_TPM_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                double d = Convert.ToDouble(HardwareInfo.GetPhysicalMemoryAlt(), CultureInfo.CurrentCulture.NumberFormat);
                if (d < 4.0 && Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    lblPM.Text += StringsAndConstants.NOT_ENOUGH_MEMORY;
                    timer10.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_MEMORYFEW_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                if (d > 4.0 && !Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    lblPM.Text += StringsAndConstants.TOO_MUCH_MEMORY;
                    timer10.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_MEMORYMUCH_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                if (pass && !offlineMode)
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_HARDWARE_PASSED, string.Empty, StringsAndConstants.consoleOutGUI);
                if (!pass)
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, this.Handle);
            }
            catch(Exception e)
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, e.Message, string.Empty, StringsAndConstants.consoleOutGUI);
            }
        }

        //Triggers when the form opens, and when the user clicks to collect
        private void coleta_Click(object sender, EventArgs e)
        {
            tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, this.Handle);
            webView2.Visible = false;
            collecting();
            accessSystemButton.Enabled = false;
            registerButton.Enabled = false;
            collectButton.Enabled = false;
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_START_THREAD, string.Empty, StringsAndConstants.consoleOutGUI);
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
            percent = e.ProgressPercentage * progressBar1.Maximum / 100;
            tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
            progressBar1.Value = percent;
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

        //Loads webView2 component
        public async Task loadWebView2()
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_START_LOADING_WEBVIEW2, string.Empty, StringsAndConstants.consoleOutGUI);
            CoreWebView2Environment webView2Environment;
            if (Environment.Is64BitOperatingSystem)
                webView2Environment = await CoreWebView2Environment.CreateAsync(StringsAndConstants.WEBVIEW2_SYSTEM_PATH_X64 + MiscMethods.getWebView2Version(), Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine));
            else
                webView2Environment = await CoreWebView2Environment.CreateAsync(StringsAndConstants.WEBVIEW2_SYSTEM_PATH_X86 + MiscMethods.getWebView2Version(), Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine));
            await webView2.EnsureCoreWebView2Async(webView2Environment);
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_END_LOADING_WEBVIEW2, string.Empty, StringsAndConstants.consoleOutGUI);
        }

        //Sends hardware info to the specified server
        public void serverSendInfo(string[] serverArgs)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTERING, string.Empty, StringsAndConstants.consoleOutGUI);
            webView2.CoreWebView2.Navigate("http://" + serverArgs[0] + ":" + serverArgs[1] + "/" + serverArgs[2] + ".php?patrimonio=" + serverArgs[3] + "&lacre=" + serverArgs[4] + "&sala=" + serverArgs[5] + "&predio=" + serverArgs[6] + "&ad=" + serverArgs[7] + "&padrao=" + serverArgs[8] + "&formatacao=" + serverArgs[9] + "&formatacoesAnteriores=" + serverArgs[9] + "&marca=" + serverArgs[10] + "&modelo=" + serverArgs[11] + "&numeroSerial=" + serverArgs[12] + "&processador=" + serverArgs[13] + "&memoria=" + serverArgs[14] + "&hd=" + serverArgs[15] + "&sistemaOperacional=" + serverArgs[16] + "&nomeDoComputador=" + serverArgs[17] + "&bios=" + serverArgs[18] + "&mac=" + serverArgs[19] + "&ip=" + serverArgs[20] + "&emUso=" + serverArgs[21] + "&etiqueta=" + serverArgs[22] + "&tipo=" + serverArgs[23] + "&tipoFW=" + serverArgs[24] + "&tipoArmaz=" + serverArgs[25] + "&gpu=" + serverArgs[26] + "&modoArmaz=" + serverArgs[27] + "&secBoot=" + serverArgs[28] + "&vt=" + serverArgs[29] + "&tpm=" + serverArgs[30] + "&trocaPilha=" + serverArgs[31] + "&ticketNum=" + serverArgs[32] + "&agent=" + serverArgs[33]);
        }

        //Runs the registration for the website
        private async void cadastra_ClickAsync(object sender, EventArgs e)
        {
            tbProgMain.SetProgressState(TaskbarProgressBarState.Indeterminate, this.Handle);
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_INIT_REGISTRY, string.Empty, StringsAndConstants.consoleOutGUI);
            loadingCircle23.Visible = true;
            loadingCircle23.Active = true;
            registerButton.Text = StringsAndConstants.DASH;
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
                if (comboBoxBattery.SelectedItem.ToString().Equals("Sim"))
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
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTRY_FINISHED, string.Empty, StringsAndConstants.consoleOutGUI);
                        date.Add(sArgs[9]);
                        if (formatButton.Checked)
                        {
                            MiscMethods.regCreate(true, dateTimePicker1);
                            lblInstallSince.Text = MiscMethods.sinceLabelUpdate(true);
                            lblMaintenanceSince.Text = MiscMethods.sinceLabelUpdate(false);
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_INSTALLDATE, string.Empty, StringsAndConstants.consoleOutGUI);
                            date.Add(sArgs[9]);
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_MAINTENANCEDATE, string.Empty, StringsAndConstants.consoleOutGUI);
                            date.Add(sArgs[9]);
                        }
                        else if (maintenanceButton.Checked)
                        {
                            MiscMethods.regCreate(false, dateTimePicker1);
                            lblMaintenanceSince.Text = MiscMethods.sinceLabelUpdate(false);
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_MAINTENANCEDATE, string.Empty, StringsAndConstants.consoleOutGUI);
                            date.Add(sArgs[9]);
                        }
                        await Task.Delay(StringsAndConstants.TIMER_INTERVAL * 3);
                        tbProgMain.SetProgressState(TaskbarProgressBarState.NoProgress, this.Handle);
                    }
                    else
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_ALREADY_REGISTERED_TODAY, string.Empty, StringsAndConstants.consoleOutGUI);
                        MessageBox.Show(StringsAndConstants.ALREADY_REGISTERED_TODAY, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                        tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, this.Handle);
                    }
                }
                else
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_SERVER_UNREACHABLE, string.Empty, StringsAndConstants.consoleOutGUI);
                    MessageBox.Show(StringsAndConstants.SERVER_NOT_FOUND_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, this.Handle);
                }
            }
            else if (!pass)
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_PENDENCY_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                MessageBox.Show(StringsAndConstants.PENDENCY_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                tbProgMain.SetProgressState(TaskbarProgressBarState.Error, this.Handle);
            }
            else
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_MANDATORY_FIELD_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                MessageBox.Show(StringsAndConstants.MANDATORY_FIELD, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, this.Handle);
            }
            loadingCircle23.Visible = false;
            loadingCircle23.Active = false;
            registerButton.Text = StringsAndConstants.REGISTER_AGAIN;
            registerButton.Enabled = true;
            accessSystemButton.Enabled = true;
            collectButton.Enabled = true;
        }
    }
}

