using ConfigurableQualityPictureBoxDLL;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using HardwareInformation.Properties;
using JsonFileReaderDLL;
using LogGeneratorDLL;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.WindowsAPICodePack.Taskbar;
using MRG.Controls.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardwareInformation
{
    public partial class MainForm : Form
    {
        private int percent, i = 0;
        private bool themeBool, serverOnline;
        private bool pass = true;
        private readonly bool offlineMode;
        private string servidor_web, porta, modeURL;
        private string BM, Model, SerialNo, ProcName, PM, HDSize, MediaType, MediaOperation, GPUInfo, OS, Hostname, Mac, IP, BIOS, BIOSType, SecBoot, VT, Smart, TPM;
        private readonly string user, ip, port;
        private readonly string[] sArgs = new string[34];
        private readonly List<string[]> defList;
        private readonly List<string> orgList;

        //Form constructor
        public MainForm(bool noConnection, string user, string ip, string port, LogGenerator l, List<string[]> definitionList, List<string> orgDataList)
        {
            //Inits WinForms components
            InitializeComponent();

            //Program version
#if DEBUG
            toolStripStatusLabel2.Text = MiscMethods.Version(Resources.dev_status); //Debug/Beta version
#else
            toolStripStatusLabel2.Text = MiscMethods.Version(); //Release/Final version
#endif
            //Define theming according to ini file provided info
            if (StringsAndConstants.listThemeGUI.Contains(definitionList[5][0].ToString()) && definitionList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[0]))
            {
                themeBool = MiscMethods.ThemeInit();
                if (themeBool)
                {
                    if (HardwareInfo.GetOSInfoAux().Equals(StringsAndConstants.windows10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                    }

                    DarkTheme();
                }
                else
                {
                    if (HardwareInfo.GetOSInfoAux().Equals(StringsAndConstants.windows10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                    }

                    LightTheme();
                }
            }
            else if (definitionList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[1]))
            {
                comboBoxTheme.Enabled = false;
                if (HardwareInfo.GetOSInfoAux().Equals(StringsAndConstants.windows10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                }

                LightTheme();
            }
            else if (definitionList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[2]))
            {
                comboBoxTheme.Enabled = false;
                if (HardwareInfo.GetOSInfoAux().Equals(StringsAndConstants.windows10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                }

                DarkTheme();
            }

            log = l;
            offlineMode = noConnection;
            defList = definitionList;
            orgList = orgDataList;



            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_OFFLINE_MODE, offlineMode.ToString(), StringsAndConstants.consoleOutGUI);

            this.user = user;
            this.ip = ip;
            this.port = port;

            if (!offlineMode)
            {
                //Fetch building and hw types info from the specified server
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_FETCHING_SERVER_DATA, string.Empty, StringsAndConstants.consoleOutGUI);
                List<string[]> jsonServerSettings = ConfigFileReader.FetchInfoST(ip, port);
                defList[2] = jsonServerSettings[0];
                defList[3] = jsonServerSettings[1];
                comboBoxBuilding.Items.AddRange(defList[2]);
                comboBoxType.Items.AddRange(defList[3]);
            }

            //Fills controls with provided info from ini file and constants dll
            comboBoxActiveDirectory.Items.AddRange(StringsAndConstants.listActiveDirectoryGUI.ToArray());
            comboBoxStandard.Items.AddRange(StringsAndConstants.listStandardGUI.ToArray());
            comboBoxInUse.Items.AddRange(StringsAndConstants.listInUseGUI.ToArray());
            comboBoxTag.Items.AddRange(StringsAndConstants.listTagGUI.ToArray());
            comboBoxBattery.Items.AddRange(StringsAndConstants.listBatteryGUI.ToArray());
            textBoxPatrimony.Text = System.Net.Dns.GetHostName().Substring(0, 3).ToUpper().Equals(StringsAndConstants.HOSTNAME_PATTERN)
                ? System.Net.Dns.GetHostName().Substring(3)
                : "";

            //Inits thread worker for parallelism
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker1_ProgressChanged);
            backgroundWorker1.DoWork += new DoWorkEventHandler(BackgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;

            //Sets status bar text according to info provided in the ini file
            string[] oList = new string[6];
            for (int i = 0; i < orgList.Count; i++)
            {
                if (!orgList[i].Equals(string.Empty))
                {
                    oList[i] = orgList[i].ToString() + " - ";
                }
            }

            toolStripStatusLabel1.Text = oList[4] + oList[2] + oList[0].Substring(0, oList[0].Length - 2);
            Text = Application.ProductName + " / " + oList[5] + oList[3] + oList[1].Substring(0, oList[1].Length - 2);
        }

        //Form elements
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            lblBM = new System.Windows.Forms.Label();
            lblModel = new System.Windows.Forms.Label();
            lblSerialNo = new System.Windows.Forms.Label();
            lblProcName = new System.Windows.Forms.Label();
            lblPM = new System.Windows.Forms.Label();
            lblHDSize = new System.Windows.Forms.Label();
            lblOS = new System.Windows.Forms.Label();
            lblHostname = new System.Windows.Forms.Label();
            lblMac = new System.Windows.Forms.Label();
            lblIP = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            textBoxPatrimony = new System.Windows.Forms.TextBox();
            textBoxSeal = new System.Windows.Forms.TextBox();
            textBoxRoom = new System.Windows.Forms.TextBox();
            textBoxLetter = new System.Windows.Forms.TextBox();
            label14 = new System.Windows.Forms.Label();
            label16 = new System.Windows.Forms.Label();
            registerButton = new System.Windows.Forms.Button();
            label18 = new System.Windows.Forms.Label();
            label19 = new System.Windows.Forms.Label();
            label20 = new System.Windows.Forms.Label();
            label21 = new System.Windows.Forms.Label();
            label22 = new System.Windows.Forms.Label();
            collectButton = new System.Windows.Forms.Button();
            label23 = new System.Windows.Forms.Label();
            label24 = new System.Windows.Forms.Label();
            lblBIOS = new System.Windows.Forms.Label();
            accessSystemButton = new System.Windows.Forms.Button();
            label25 = new System.Windows.Forms.Label();
            lblBIOSType = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            loadingCircle19 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle18 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle17 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle16 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle15 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle14 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle13 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle12 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle11 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle10 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle9 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle8 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle7 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle6 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle5 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle4 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle3 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle2 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle1 = new MRG.Controls.UI.LoadingCircle();
            separatorH = new System.Windows.Forms.Label();
            separatorV = new System.Windows.Forms.Label();
            configurableQualityPictureBox33 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox32 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblSmart = new System.Windows.Forms.Label();
            lblTPM = new System.Windows.Forms.Label();
            label44 = new System.Windows.Forms.Label();
            configurableQualityPictureBox30 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            label45 = new System.Windows.Forms.Label();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            label28 = new System.Windows.Forms.Label();
            lblVT = new System.Windows.Forms.Label();
            label33 = new System.Windows.Forms.Label();
            configurableQualityPictureBox2 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox17 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox16 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox15 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox14 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox13 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox12 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox11 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox10 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox9 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox8 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox7 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox6 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox5 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox4 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox3 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblSecBoot = new System.Windows.Forms.Label();
            label32 = new System.Windows.Forms.Label();
            lblMediaOperation = new System.Windows.Forms.Label();
            label30 = new System.Windows.Forms.Label();
            lblGPUInfo = new System.Windows.Forms.Label();
            label29 = new System.Windows.Forms.Label();
            lblMediaType = new System.Windows.Forms.Label();
            label27 = new System.Windows.Forms.Label();
            groupBox2 = new System.Windows.Forms.GroupBox();
            comboBoxBattery = new CustomFlatComboBox();
            comboBoxStandard = new CustomFlatComboBox();
            comboBoxActiveDirectory = new CustomFlatComboBox();
            comboBoxTag = new CustomFlatComboBox();
            comboBoxInUse = new CustomFlatComboBox();
            comboBoxType = new CustomFlatComboBox();
            comboBoxBuilding = new CustomFlatComboBox();
            label48 = new System.Windows.Forms.Label();
            label47 = new System.Windows.Forms.Label();
            configurableQualityPictureBox35 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            label31 = new System.Windows.Forms.Label();
            textBoxTicket = new System.Windows.Forms.TextBox();
            configurableQualityPictureBox34 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            label42 = new System.Windows.Forms.Label();
            label41 = new System.Windows.Forms.Label();
            label46 = new System.Windows.Forms.Label();
            label40 = new System.Windows.Forms.Label();
            label39 = new System.Windows.Forms.Label();
            label38 = new System.Windows.Forms.Label();
            label37 = new System.Windows.Forms.Label();
            label36 = new System.Windows.Forms.Label();
            label35 = new System.Windows.Forms.Label();
            studentRadioButton = new System.Windows.Forms.RadioButton();
            employeeRadioButton = new System.Windows.Forms.RadioButton();
            configurableQualityPictureBox31 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            label34 = new System.Windows.Forms.Label();
            configurableQualityPictureBox25 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox28 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox27 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox26 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox24 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox23 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox22 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox21 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox20 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox19 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            configurableQualityPictureBox18 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            groupBox3 = new System.Windows.Forms.GroupBox();
            loadingCircle21 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle20 = new MRG.Controls.UI.LoadingCircle();
            lblMaintenanceSince = new System.Windows.Forms.Label();
            lblInstallSince = new System.Windows.Forms.Label();
            label43 = new System.Windows.Forms.Label();
            textBox5 = new System.Windows.Forms.TextBox();
            textBox6 = new System.Windows.Forms.TextBox();
            formatButton = new System.Windows.Forms.RadioButton();
            maintenanceButton = new System.Windows.Forms.RadioButton();
            label15 = new System.Windows.Forms.Label();
            label17 = new System.Windows.Forms.Label();
            lblAgentName = new System.Windows.Forms.Label();
            label53 = new System.Windows.Forms.Label();
            lblPortServer = new System.Windows.Forms.Label();
            lblIPServer = new System.Windows.Forms.Label();
            label49 = new System.Windows.Forms.Label();
            label26 = new System.Windows.Forms.Label();
            toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            comboBoxTheme = new System.Windows.Forms.ToolStripDropDownButton();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            logLabel = new System.Windows.Forms.ToolStripStatusLabel();
            aboutLabel = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            timer1 = new System.Windows.Forms.Timer(components);
            timer2 = new System.Windows.Forms.Timer(components);
            timer3 = new System.Windows.Forms.Timer(components);
            timer4 = new System.Windows.Forms.Timer(components);
            timer5 = new System.Windows.Forms.Timer(components);
            timer6 = new System.Windows.Forms.Timer(components);
            timer7 = new System.Windows.Forms.Timer(components);
            timer8 = new System.Windows.Forms.Timer(components);
            groupBox4 = new System.Windows.Forms.GroupBox();
            webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            timer9 = new System.Windows.Forms.Timer(components);
            timer10 = new System.Windows.Forms.Timer(components);
            configurableQualityPictureBox1 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            loadingCircle22 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle23 = new MRG.Controls.UI.LoadingCircle();
            groupBox5 = new System.Windows.Forms.GroupBox();
            loadingCircle24 = new MRG.Controls.UI.LoadingCircle();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox33).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox32).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox30).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox17).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox16).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox15).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox14).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox13).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox12).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox11).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox10).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox9).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox8).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox3).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox35).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox34).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox31).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox25).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox28).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox27).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox26).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox24).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox23).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox22).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox21).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox20).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox19).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox18).BeginInit();
            groupBox3.SuspendLayout();
            statusStrip1.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox1).BeginInit();
            groupBox5.SuspendLayout();
            SuspendLayout();
            // 
            // lblBM
            // 
            lblBM.AutoSize = true;
            lblBM.ForeColor = System.Drawing.Color.Silver;
            lblBM.Location = new System.Drawing.Point(203, 20);
            lblBM.Name = "lblBM";
            lblBM.Size = new System.Drawing.Size(10, 13);
            lblBM.TabIndex = 7;
            lblBM.Text = "-";
            // 
            // lblModel
            // 
            lblModel.AutoSize = true;
            lblModel.ForeColor = System.Drawing.Color.Silver;
            lblModel.Location = new System.Drawing.Point(203, 46);
            lblModel.Name = "lblModel";
            lblModel.Size = new System.Drawing.Size(10, 13);
            lblModel.TabIndex = 8;
            lblModel.Text = "-";
            // 
            // lblSerialNo
            // 
            lblSerialNo.AutoSize = true;
            lblSerialNo.ForeColor = System.Drawing.Color.Silver;
            lblSerialNo.Location = new System.Drawing.Point(203, 72);
            lblSerialNo.Name = "lblSerialNo";
            lblSerialNo.Size = new System.Drawing.Size(10, 13);
            lblSerialNo.TabIndex = 9;
            lblSerialNo.Text = "-";
            // 
            // lblProcName
            // 
            lblProcName.AutoSize = true;
            lblProcName.ForeColor = System.Drawing.Color.Silver;
            lblProcName.Location = new System.Drawing.Point(203, 98);
            lblProcName.Name = "lblProcName";
            lblProcName.Size = new System.Drawing.Size(10, 13);
            lblProcName.TabIndex = 10;
            lblProcName.Text = "-";
            // 
            // lblPM
            // 
            lblPM.AutoSize = true;
            lblPM.ForeColor = System.Drawing.Color.Silver;
            lblPM.Location = new System.Drawing.Point(203, 124);
            lblPM.Name = "lblPM";
            lblPM.Size = new System.Drawing.Size(10, 13);
            lblPM.TabIndex = 11;
            lblPM.Text = "-";
            // 
            // lblHDSize
            // 
            lblHDSize.AutoSize = true;
            lblHDSize.ForeColor = System.Drawing.Color.Silver;
            lblHDSize.Location = new System.Drawing.Point(203, 150);
            lblHDSize.Name = "lblHDSize";
            lblHDSize.Size = new System.Drawing.Size(10, 13);
            lblHDSize.TabIndex = 12;
            lblHDSize.Text = "-";
            // 
            // lblOS
            // 
            lblOS.AutoSize = true;
            lblOS.ForeColor = System.Drawing.Color.Silver;
            lblOS.Location = new System.Drawing.Point(203, 280);
            lblOS.Name = "lblOS";
            lblOS.Size = new System.Drawing.Size(10, 13);
            lblOS.TabIndex = 13;
            lblOS.Text = "-";
            // 
            // lblHostname
            // 
            lblHostname.AutoSize = true;
            lblHostname.ForeColor = System.Drawing.Color.Silver;
            lblHostname.Location = new System.Drawing.Point(203, 306);
            lblHostname.Name = "lblHostname";
            lblHostname.Size = new System.Drawing.Size(10, 13);
            lblHostname.TabIndex = 15;
            lblHostname.Text = "-";
            // 
            // lblMac
            // 
            lblMac.AutoSize = true;
            lblMac.ForeColor = System.Drawing.Color.Silver;
            lblMac.Location = new System.Drawing.Point(203, 332);
            lblMac.Name = "lblMac";
            lblMac.Size = new System.Drawing.Size(10, 13);
            lblMac.TabIndex = 18;
            lblMac.Text = "-";
            // 
            // lblIP
            // 
            lblIP.AutoSize = true;
            lblIP.ForeColor = System.Drawing.Color.Silver;
            lblIP.Location = new System.Drawing.Point(203, 358);
            lblIP.Name = "lblIP";
            lblIP.Size = new System.Drawing.Size(10, 13);
            lblIP.TabIndex = 19;
            lblIP.Text = "-";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = System.Drawing.SystemColors.ControlText;
            label1.Location = new System.Drawing.Point(37, 20);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(40, 13);
            label1.TabIndex = 0;
            label1.Text = "Marca:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = System.Drawing.SystemColors.ControlText;
            label2.Location = new System.Drawing.Point(37, 46);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(45, 13);
            label2.TabIndex = 1;
            label2.Text = "Modelo:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = System.Drawing.SystemColors.ControlText;
            label3.Location = new System.Drawing.Point(37, 72);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(76, 13);
            label3.TabIndex = 2;
            label3.Text = "Número Serial:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = System.Drawing.SystemColors.ControlText;
            label4.Location = new System.Drawing.Point(37, 98);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(146, 13);
            label4.TabIndex = 3;
            label4.Text = "Processador e nº de núcleos:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = System.Drawing.SystemColors.ControlText;
            label5.Location = new System.Drawing.Point(37, 124);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(138, 13);
            label5.TabIndex = 4;
            label5.Text = "Memória RAM e nº de slots:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = System.Drawing.SystemColors.ControlText;
            label6.Location = new System.Drawing.Point(37, 150);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(153, 13);
            label6.TabIndex = 5;
            label6.Text = "Armazenamento (espaço total):";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = System.Drawing.SystemColors.ControlText;
            label7.Location = new System.Drawing.Point(37, 280);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(107, 13);
            label7.TabIndex = 6;
            label7.Text = "Sistema Operacional:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = System.Drawing.SystemColors.ControlText;
            label8.Location = new System.Drawing.Point(37, 306);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(113, 13);
            label8.TabIndex = 7;
            label8.Text = "Nome do Computador:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.ForeColor = System.Drawing.SystemColors.ControlText;
            label9.Location = new System.Drawing.Point(37, 332);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(118, 13);
            label9.TabIndex = 8;
            label9.Text = "Endereço MAC do NIC:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = System.Drawing.SystemColors.ControlText;
            label10.Location = new System.Drawing.Point(37, 358);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(105, 13);
            label10.TabIndex = 9;
            label10.Text = "Endereço IP do NIC:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.ForeColor = System.Drawing.SystemColors.ControlText;
            label11.Location = new System.Drawing.Point(37, 20);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(59, 13);
            label11.TabIndex = 10;
            label11.Text = "Patrimônio:";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.ForeColor = System.Drawing.SystemColors.ControlText;
            label12.Location = new System.Drawing.Point(37, 46);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(93, 13);
            label12.TabIndex = 11;
            label12.Text = "Lacre (se houver):";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.ForeColor = System.Drawing.SystemColors.ControlText;
            label13.Location = new System.Drawing.Point(37, 98);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(40, 13);
            label13.TabIndex = 13;
            label13.Text = "Prédio:";
            // 
            // textBoxPatrimony
            // 
            textBoxPatrimony.BackColor = System.Drawing.SystemColors.Window;
            textBoxPatrimony.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxPatrimony.Location = new System.Drawing.Point(185, 17);
            textBoxPatrimony.MaxLength = 6;
            textBoxPatrimony.Name = "textBoxPatrimony";
            textBoxPatrimony.Size = new System.Drawing.Size(259, 20);
            textBoxPatrimony.TabIndex = 34;
            textBoxPatrimony.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxSeal
            // 
            textBoxSeal.BackColor = System.Drawing.SystemColors.Window;
            textBoxSeal.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxSeal.Location = new System.Drawing.Point(185, 43);
            textBoxSeal.MaxLength = 10;
            textBoxSeal.Name = "textBoxSeal";
            textBoxSeal.Size = new System.Drawing.Size(259, 20);
            textBoxSeal.TabIndex = 35;
            textBoxSeal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxRoom
            // 
            textBoxRoom.BackColor = System.Drawing.SystemColors.Window;
            textBoxRoom.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxRoom.Location = new System.Drawing.Point(185, 69);
            textBoxRoom.MaxLength = 4;
            textBoxRoom.Name = "textBoxRoom";
            textBoxRoom.Size = new System.Drawing.Size(101, 20);
            textBoxRoom.TabIndex = 36;
            textBoxRoom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxLetter
            // 
            textBoxLetter.BackColor = System.Drawing.SystemColors.Window;
            textBoxLetter.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxLetter.Location = new System.Drawing.Point(419, 69);
            textBoxLetter.MaxLength = 1;
            textBoxLetter.Name = "textBoxLetter";
            textBoxLetter.Size = new System.Drawing.Size(25, 20);
            textBoxLetter.TabIndex = 37;
            textBoxLetter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxCharsOnly_KeyPress);
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.ForeColor = System.Drawing.SystemColors.ControlText;
            label14.Location = new System.Drawing.Point(37, 72);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(135, 13);
            label14.TabIndex = 12;
            label14.Text = "Sala (0000 se não houver):";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.ForeColor = System.Drawing.SystemColors.ControlText;
            label16.Location = new System.Drawing.Point(37, 150);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(115, 13);
            label16.TabIndex = 16;
            label16.Text = "Data do último serviço:";
            // 
            // registerButton
            // 
            registerButton.BackColor = System.Drawing.SystemColors.Control;
            registerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            registerButton.ForeColor = System.Drawing.SystemColors.ControlText;
            registerButton.Location = new System.Drawing.Point(760, 630);
            registerButton.Name = "registerButton";
            registerButton.Size = new System.Drawing.Size(265, 56);
            registerButton.TabIndex = 53;
            registerButton.Text = "Cadastrar / Atualizar dados";
            registerButton.UseVisualStyleBackColor = true;
            registerButton.Click += new System.EventHandler(Cadastra_ClickAsync);
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.ForeColor = System.Drawing.SystemColors.ControlText;
            label18.Location = new System.Drawing.Point(322, 98);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(45, 13);
            label18.TabIndex = 48;
            label18.Text = "Em uso:";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.ForeColor = System.Drawing.SystemColors.ControlText;
            label19.Location = new System.Drawing.Point(322, 124);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(49, 13);
            label19.TabIndex = 50;
            label19.Text = "Etiqueta:";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.ForeColor = System.Drawing.SystemColors.ControlText;
            label20.Location = new System.Drawing.Point(37, 124);
            label20.Name = "label20";
            label20.Size = new System.Drawing.Size(31, 13);
            label20.TabIndex = 53;
            label20.Text = "Tipo:";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.ForeColor = System.Drawing.SystemColors.ControlText;
            label21.Location = new System.Drawing.Point(187, 16);
            label21.Name = "label21";
            label21.Size = new System.Drawing.Size(98, 13);
            label21.TabIndex = 17;
            label21.Text = "Status operacional:";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.ForeColor = System.Drawing.SystemColors.ControlText;
            label22.Location = new System.Drawing.Point(7, 35);
            label22.Name = "label22";
            label22.Size = new System.Drawing.Size(35, 13);
            label22.TabIndex = 18;
            label22.Text = "Porta:";
            // 
            // collectButton
            // 
            collectButton.BackColor = System.Drawing.SystemColors.Control;
            collectButton.ForeColor = System.Drawing.SystemColors.ControlText;
            collectButton.Location = new System.Drawing.Point(575, 630);
            collectButton.Name = "collectButton";
            collectButton.Size = new System.Drawing.Size(180, 25);
            collectButton.TabIndex = 51;
            collectButton.Text = "Coletar novamente";
            collectButton.UseVisualStyleBackColor = true;
            collectButton.Click += new System.EventHandler(Coleta_Click);
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.ForeColor = System.Drawing.SystemColors.ControlText;
            label23.Location = new System.Drawing.Point(322, 72);
            label23.Name = "label23";
            label23.Size = new System.Drawing.Size(90, 13);
            label23.TabIndex = 55;
            label23.Text = "Letra (se houver):";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.ForeColor = System.Drawing.SystemColors.ControlText;
            label24.Location = new System.Drawing.Point(37, 410);
            label24.Name = "label24";
            label24.Size = new System.Drawing.Size(100, 13);
            label24.TabIndex = 56;
            label24.Text = "Versão do firmware:";
            // 
            // lblBIOS
            // 
            lblBIOS.AutoSize = true;
            lblBIOS.ForeColor = System.Drawing.Color.Silver;
            lblBIOS.Location = new System.Drawing.Point(203, 410);
            lblBIOS.Name = "lblBIOS";
            lblBIOS.Size = new System.Drawing.Size(10, 13);
            lblBIOS.TabIndex = 57;
            lblBIOS.Text = "-";
            // 
            // accessSystemButton
            // 
            accessSystemButton.BackColor = System.Drawing.SystemColors.Control;
            accessSystemButton.ForeColor = System.Drawing.SystemColors.ControlText;
            accessSystemButton.Location = new System.Drawing.Point(575, 661);
            accessSystemButton.Name = "accessSystemButton";
            accessSystemButton.Size = new System.Drawing.Size(180, 25);
            accessSystemButton.TabIndex = 52;
            accessSystemButton.Text = "Acessar sistema de patrimônios";
            accessSystemButton.UseVisualStyleBackColor = true;
            accessSystemButton.Click += new System.EventHandler(AccessButton_Click);
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.ForeColor = System.Drawing.SystemColors.ControlText;
            label25.Location = new System.Drawing.Point(37, 384);
            label25.Name = "label25";
            label25.Size = new System.Drawing.Size(88, 13);
            label25.TabIndex = 62;
            label25.Text = "Tipo de firmware:";
            // 
            // lblBIOSType
            // 
            lblBIOSType.AutoSize = true;
            lblBIOSType.ForeColor = System.Drawing.Color.Silver;
            lblBIOSType.Location = new System.Drawing.Point(203, 384);
            lblBIOSType.Name = "lblBIOSType";
            lblBIOSType.Size = new System.Drawing.Size(10, 13);
            lblBIOSType.TabIndex = 63;
            lblBIOSType.Text = "-";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(loadingCircle19);
            groupBox1.Controls.Add(loadingCircle18);
            groupBox1.Controls.Add(loadingCircle17);
            groupBox1.Controls.Add(loadingCircle16);
            groupBox1.Controls.Add(loadingCircle15);
            groupBox1.Controls.Add(loadingCircle14);
            groupBox1.Controls.Add(loadingCircle13);
            groupBox1.Controls.Add(loadingCircle12);
            groupBox1.Controls.Add(loadingCircle11);
            groupBox1.Controls.Add(loadingCircle10);
            groupBox1.Controls.Add(loadingCircle9);
            groupBox1.Controls.Add(loadingCircle8);
            groupBox1.Controls.Add(loadingCircle7);
            groupBox1.Controls.Add(loadingCircle6);
            groupBox1.Controls.Add(loadingCircle5);
            groupBox1.Controls.Add(loadingCircle4);
            groupBox1.Controls.Add(loadingCircle3);
            groupBox1.Controls.Add(loadingCircle2);
            groupBox1.Controls.Add(loadingCircle1);
            groupBox1.Controls.Add(separatorH);
            groupBox1.Controls.Add(separatorV);
            groupBox1.Controls.Add(configurableQualityPictureBox33);
            groupBox1.Controls.Add(configurableQualityPictureBox32);
            groupBox1.Controls.Add(lblSmart);
            groupBox1.Controls.Add(lblTPM);
            groupBox1.Controls.Add(label44);
            groupBox1.Controls.Add(configurableQualityPictureBox30);
            groupBox1.Controls.Add(label45);
            groupBox1.Controls.Add(progressBar1);
            groupBox1.Controls.Add(label28);
            groupBox1.Controls.Add(lblVT);
            groupBox1.Controls.Add(label33);
            groupBox1.Controls.Add(configurableQualityPictureBox2);
            groupBox1.Controls.Add(configurableQualityPictureBox17);
            groupBox1.Controls.Add(configurableQualityPictureBox16);
            groupBox1.Controls.Add(configurableQualityPictureBox15);
            groupBox1.Controls.Add(configurableQualityPictureBox14);
            groupBox1.Controls.Add(configurableQualityPictureBox13);
            groupBox1.Controls.Add(configurableQualityPictureBox12);
            groupBox1.Controls.Add(configurableQualityPictureBox11);
            groupBox1.Controls.Add(configurableQualityPictureBox10);
            groupBox1.Controls.Add(configurableQualityPictureBox9);
            groupBox1.Controls.Add(configurableQualityPictureBox8);
            groupBox1.Controls.Add(configurableQualityPictureBox7);
            groupBox1.Controls.Add(configurableQualityPictureBox6);
            groupBox1.Controls.Add(configurableQualityPictureBox5);
            groupBox1.Controls.Add(configurableQualityPictureBox4);
            groupBox1.Controls.Add(configurableQualityPictureBox3);
            groupBox1.Controls.Add(lblSecBoot);
            groupBox1.Controls.Add(label32);
            groupBox1.Controls.Add(lblMediaOperation);
            groupBox1.Controls.Add(label30);
            groupBox1.Controls.Add(lblGPUInfo);
            groupBox1.Controls.Add(label29);
            groupBox1.Controls.Add(lblMediaType);
            groupBox1.Controls.Add(label27);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(lblOS);
            groupBox1.Controls.Add(lblBIOSType);
            groupBox1.Controls.Add(lblHDSize);
            groupBox1.Controls.Add(label25);
            groupBox1.Controls.Add(lblPM);
            groupBox1.Controls.Add(lblProcName);
            groupBox1.Controls.Add(lblSerialNo);
            groupBox1.Controls.Add(lblBIOS);
            groupBox1.Controls.Add(lblModel);
            groupBox1.Controls.Add(label24);
            groupBox1.Controls.Add(lblBM);
            groupBox1.Controls.Add(lblHostname);
            groupBox1.Controls.Add(lblMac);
            groupBox1.Controls.Add(lblIP);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(label10);
            groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            groupBox1.Location = new System.Drawing.Point(32, 113);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(537, 573);
            groupBox1.TabIndex = 65;
            groupBox1.TabStop = false;
            groupBox1.Text = "Dados do computador";
            // 
            // loadingCircle19
            // 
            loadingCircle19.Active = false;
            loadingCircle19.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle19.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle19.InnerCircleRadius = 5;
            loadingCircle19.Location = new System.Drawing.Point(202, 482);
            loadingCircle19.Name = "loadingCircle19";
            loadingCircle19.NumberSpoke = 12;
            loadingCircle19.OuterCircleRadius = 11;
            loadingCircle19.RotationSpeed = 1;
            loadingCircle19.Size = new System.Drawing.Size(37, 25);
            loadingCircle19.SpokeThickness = 2;
            loadingCircle19.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle19.TabIndex = 131;
            loadingCircle19.Text = "loadingCircle19";
            // 
            // loadingCircle18
            // 
            loadingCircle18.Active = false;
            loadingCircle18.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle18.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle18.InnerCircleRadius = 5;
            loadingCircle18.Location = new System.Drawing.Point(202, 456);
            loadingCircle18.Name = "loadingCircle18";
            loadingCircle18.NumberSpoke = 12;
            loadingCircle18.OuterCircleRadius = 11;
            loadingCircle18.RotationSpeed = 1;
            loadingCircle18.Size = new System.Drawing.Size(37, 25);
            loadingCircle18.SpokeThickness = 2;
            loadingCircle18.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle18.TabIndex = 130;
            loadingCircle18.Text = "loadingCircle18";
            // 
            // loadingCircle17
            // 
            loadingCircle17.Active = false;
            loadingCircle17.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle17.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle17.InnerCircleRadius = 5;
            loadingCircle17.Location = new System.Drawing.Point(202, 430);
            loadingCircle17.Name = "loadingCircle17";
            loadingCircle17.NumberSpoke = 12;
            loadingCircle17.OuterCircleRadius = 11;
            loadingCircle17.RotationSpeed = 1;
            loadingCircle17.Size = new System.Drawing.Size(37, 25);
            loadingCircle17.SpokeThickness = 2;
            loadingCircle17.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle17.TabIndex = 129;
            loadingCircle17.Text = "loadingCircle17";
            // 
            // loadingCircle16
            // 
            loadingCircle16.Active = false;
            loadingCircle16.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle16.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle16.InnerCircleRadius = 5;
            loadingCircle16.Location = new System.Drawing.Point(202, 404);
            loadingCircle16.Name = "loadingCircle16";
            loadingCircle16.NumberSpoke = 12;
            loadingCircle16.OuterCircleRadius = 11;
            loadingCircle16.RotationSpeed = 1;
            loadingCircle16.Size = new System.Drawing.Size(37, 25);
            loadingCircle16.SpokeThickness = 2;
            loadingCircle16.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle16.TabIndex = 128;
            loadingCircle16.Text = "loadingCircle16";
            // 
            // loadingCircle15
            // 
            loadingCircle15.Active = false;
            loadingCircle15.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle15.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle15.InnerCircleRadius = 5;
            loadingCircle15.Location = new System.Drawing.Point(202, 378);
            loadingCircle15.Name = "loadingCircle15";
            loadingCircle15.NumberSpoke = 12;
            loadingCircle15.OuterCircleRadius = 11;
            loadingCircle15.RotationSpeed = 1;
            loadingCircle15.Size = new System.Drawing.Size(37, 25);
            loadingCircle15.SpokeThickness = 2;
            loadingCircle15.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle15.TabIndex = 127;
            loadingCircle15.Text = "loadingCircle15";
            // 
            // loadingCircle14
            // 
            loadingCircle14.Active = false;
            loadingCircle14.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle14.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle14.InnerCircleRadius = 5;
            loadingCircle14.Location = new System.Drawing.Point(202, 352);
            loadingCircle14.Name = "loadingCircle14";
            loadingCircle14.NumberSpoke = 12;
            loadingCircle14.OuterCircleRadius = 11;
            loadingCircle14.RotationSpeed = 1;
            loadingCircle14.Size = new System.Drawing.Size(37, 25);
            loadingCircle14.SpokeThickness = 2;
            loadingCircle14.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle14.TabIndex = 126;
            loadingCircle14.Text = "loadingCircle14";
            // 
            // loadingCircle13
            // 
            loadingCircle13.Active = false;
            loadingCircle13.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle13.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle13.InnerCircleRadius = 5;
            loadingCircle13.Location = new System.Drawing.Point(202, 326);
            loadingCircle13.Name = "loadingCircle13";
            loadingCircle13.NumberSpoke = 12;
            loadingCircle13.OuterCircleRadius = 11;
            loadingCircle13.RotationSpeed = 1;
            loadingCircle13.Size = new System.Drawing.Size(37, 25);
            loadingCircle13.SpokeThickness = 2;
            loadingCircle13.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle13.TabIndex = 125;
            loadingCircle13.Text = "loadingCircle13";
            // 
            // loadingCircle12
            // 
            loadingCircle12.Active = false;
            loadingCircle12.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle12.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle12.InnerCircleRadius = 5;
            loadingCircle12.Location = new System.Drawing.Point(202, 300);
            loadingCircle12.Name = "loadingCircle12";
            loadingCircle12.NumberSpoke = 12;
            loadingCircle12.OuterCircleRadius = 11;
            loadingCircle12.RotationSpeed = 1;
            loadingCircle12.Size = new System.Drawing.Size(37, 25);
            loadingCircle12.SpokeThickness = 2;
            loadingCircle12.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle12.TabIndex = 124;
            loadingCircle12.Text = "loadingCircle12";
            // 
            // loadingCircle11
            // 
            loadingCircle11.Active = false;
            loadingCircle11.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle11.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle11.InnerCircleRadius = 5;
            loadingCircle11.Location = new System.Drawing.Point(202, 274);
            loadingCircle11.Name = "loadingCircle11";
            loadingCircle11.NumberSpoke = 12;
            loadingCircle11.OuterCircleRadius = 11;
            loadingCircle11.RotationSpeed = 1;
            loadingCircle11.Size = new System.Drawing.Size(37, 25);
            loadingCircle11.SpokeThickness = 2;
            loadingCircle11.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle11.TabIndex = 123;
            loadingCircle11.Text = "loadingCircle11";
            // 
            // loadingCircle10
            // 
            loadingCircle10.Active = false;
            loadingCircle10.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle10.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle10.InnerCircleRadius = 5;
            loadingCircle10.Location = new System.Drawing.Point(202, 248);
            loadingCircle10.Name = "loadingCircle10";
            loadingCircle10.NumberSpoke = 12;
            loadingCircle10.OuterCircleRadius = 11;
            loadingCircle10.RotationSpeed = 1;
            loadingCircle10.Size = new System.Drawing.Size(37, 25);
            loadingCircle10.SpokeThickness = 2;
            loadingCircle10.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle10.TabIndex = 122;
            loadingCircle10.Text = "loadingCircle10";
            // 
            // loadingCircle9
            // 
            loadingCircle9.Active = false;
            loadingCircle9.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle9.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle9.InnerCircleRadius = 5;
            loadingCircle9.Location = new System.Drawing.Point(202, 222);
            loadingCircle9.Name = "loadingCircle9";
            loadingCircle9.NumberSpoke = 12;
            loadingCircle9.OuterCircleRadius = 11;
            loadingCircle9.RotationSpeed = 1;
            loadingCircle9.Size = new System.Drawing.Size(37, 25);
            loadingCircle9.SpokeThickness = 2;
            loadingCircle9.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle9.TabIndex = 121;
            loadingCircle9.Text = "loadingCircle9";
            // 
            // loadingCircle8
            // 
            loadingCircle8.Active = false;
            loadingCircle8.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle8.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle8.InnerCircleRadius = 5;
            loadingCircle8.Location = new System.Drawing.Point(202, 196);
            loadingCircle8.Name = "loadingCircle8";
            loadingCircle8.NumberSpoke = 12;
            loadingCircle8.OuterCircleRadius = 11;
            loadingCircle8.RotationSpeed = 1;
            loadingCircle8.Size = new System.Drawing.Size(37, 25);
            loadingCircle8.SpokeThickness = 2;
            loadingCircle8.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle8.TabIndex = 120;
            loadingCircle8.Text = "loadingCircle8";
            // 
            // loadingCircle7
            // 
            loadingCircle7.Active = false;
            loadingCircle7.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle7.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle7.InnerCircleRadius = 5;
            loadingCircle7.Location = new System.Drawing.Point(202, 170);
            loadingCircle7.Name = "loadingCircle7";
            loadingCircle7.NumberSpoke = 12;
            loadingCircle7.OuterCircleRadius = 11;
            loadingCircle7.RotationSpeed = 1;
            loadingCircle7.Size = new System.Drawing.Size(37, 25);
            loadingCircle7.SpokeThickness = 2;
            loadingCircle7.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle7.TabIndex = 119;
            loadingCircle7.Text = "loadingCircle7";
            // 
            // loadingCircle6
            // 
            loadingCircle6.Active = false;
            loadingCircle6.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle6.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle6.InnerCircleRadius = 5;
            loadingCircle6.Location = new System.Drawing.Point(202, 144);
            loadingCircle6.Name = "loadingCircle6";
            loadingCircle6.NumberSpoke = 12;
            loadingCircle6.OuterCircleRadius = 11;
            loadingCircle6.RotationSpeed = 1;
            loadingCircle6.Size = new System.Drawing.Size(37, 25);
            loadingCircle6.SpokeThickness = 2;
            loadingCircle6.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle6.TabIndex = 118;
            loadingCircle6.Text = "loadingCircle6";
            // 
            // loadingCircle5
            // 
            loadingCircle5.Active = false;
            loadingCircle5.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle5.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle5.InnerCircleRadius = 5;
            loadingCircle5.Location = new System.Drawing.Point(202, 118);
            loadingCircle5.Name = "loadingCircle5";
            loadingCircle5.NumberSpoke = 12;
            loadingCircle5.OuterCircleRadius = 11;
            loadingCircle5.RotationSpeed = 1;
            loadingCircle5.Size = new System.Drawing.Size(37, 25);
            loadingCircle5.SpokeThickness = 2;
            loadingCircle5.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle5.TabIndex = 117;
            loadingCircle5.Text = "loadingCircle5";
            // 
            // loadingCircle4
            // 
            loadingCircle4.Active = false;
            loadingCircle4.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle4.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle4.InnerCircleRadius = 5;
            loadingCircle4.Location = new System.Drawing.Point(202, 92);
            loadingCircle4.Name = "loadingCircle4";
            loadingCircle4.NumberSpoke = 12;
            loadingCircle4.OuterCircleRadius = 11;
            loadingCircle4.RotationSpeed = 1;
            loadingCircle4.Size = new System.Drawing.Size(37, 25);
            loadingCircle4.SpokeThickness = 2;
            loadingCircle4.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle4.TabIndex = 116;
            loadingCircle4.Text = "loadingCircle4";
            // 
            // loadingCircle3
            // 
            loadingCircle3.Active = false;
            loadingCircle3.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle3.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle3.InnerCircleRadius = 5;
            loadingCircle3.Location = new System.Drawing.Point(202, 66);
            loadingCircle3.Name = "loadingCircle3";
            loadingCircle3.NumberSpoke = 12;
            loadingCircle3.OuterCircleRadius = 11;
            loadingCircle3.RotationSpeed = 1;
            loadingCircle3.Size = new System.Drawing.Size(37, 25);
            loadingCircle3.SpokeThickness = 2;
            loadingCircle3.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle3.TabIndex = 115;
            loadingCircle3.Text = "loadingCircle3";
            // 
            // loadingCircle2
            // 
            loadingCircle2.Active = false;
            loadingCircle2.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle2.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle2.InnerCircleRadius = 5;
            loadingCircle2.Location = new System.Drawing.Point(202, 40);
            loadingCircle2.Name = "loadingCircle2";
            loadingCircle2.NumberSpoke = 12;
            loadingCircle2.OuterCircleRadius = 11;
            loadingCircle2.RotationSpeed = 1;
            loadingCircle2.Size = new System.Drawing.Size(37, 25);
            loadingCircle2.SpokeThickness = 2;
            loadingCircle2.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle2.TabIndex = 114;
            loadingCircle2.Text = "loadingCircle2";
            // 
            // loadingCircle1
            // 
            loadingCircle1.Active = false;
            loadingCircle1.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle1.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle1.InnerCircleRadius = 5;
            loadingCircle1.Location = new System.Drawing.Point(202, 14);
            loadingCircle1.Name = "loadingCircle1";
            loadingCircle1.NumberSpoke = 12;
            loadingCircle1.OuterCircleRadius = 11;
            loadingCircle1.RotationSpeed = 1;
            loadingCircle1.Size = new System.Drawing.Size(37, 25);
            loadingCircle1.SpokeThickness = 2;
            loadingCircle1.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle1.TabIndex = 113;
            loadingCircle1.Text = "loadingCircle1";
            // 
            // separatorH
            // 
            separatorH.BackColor = System.Drawing.Color.DimGray;
            separatorH.Location = new System.Drawing.Point(6, 513);
            separatorH.Name = "separatorH";
            separatorH.Size = new System.Drawing.Size(525, 1);
            separatorH.TabIndex = 112;
            separatorH.Text = "label54";
            // 
            // separatorV
            // 
            separatorV.BackColor = System.Drawing.Color.DimGray;
            separatorV.Location = new System.Drawing.Point(200, 14);
            separatorV.Name = "separatorV";
            separatorV.Size = new System.Drawing.Size(1, 499);
            separatorV.TabIndex = 111;
            separatorV.Text = "label54";
            // 
            // configurableQualityPictureBox33
            // 
            configurableQualityPictureBox33.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox33.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox33.Location = new System.Drawing.Point(7, 482);
            configurableQualityPictureBox33.Name = "configurableQualityPictureBox33";
            configurableQualityPictureBox33.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox33.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox33.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox33.TabIndex = 110;
            configurableQualityPictureBox33.TabStop = false;
            // 
            // configurableQualityPictureBox32
            // 
            configurableQualityPictureBox32.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox32.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox32.Location = new System.Drawing.Point(7, 170);
            configurableQualityPictureBox32.Name = "configurableQualityPictureBox32";
            configurableQualityPictureBox32.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox32.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox32.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox32.TabIndex = 107;
            configurableQualityPictureBox32.TabStop = false;
            // 
            // lblSmart
            // 
            lblSmart.AutoSize = true;
            lblSmart.ForeColor = System.Drawing.Color.Silver;
            lblSmart.Location = new System.Drawing.Point(203, 176);
            lblSmart.Name = "lblSmart";
            lblSmart.Size = new System.Drawing.Size(10, 13);
            lblSmart.TabIndex = 106;
            lblSmart.Text = "-";
            // 
            // lblTPM
            // 
            lblTPM.AutoSize = true;
            lblTPM.ForeColor = System.Drawing.Color.Silver;
            lblTPM.Location = new System.Drawing.Point(203, 488);
            lblTPM.Name = "lblTPM";
            lblTPM.Size = new System.Drawing.Size(10, 13);
            lblTPM.TabIndex = 109;
            lblTPM.Text = "-";
            // 
            // label44
            // 
            label44.AutoSize = true;
            label44.ForeColor = System.Drawing.SystemColors.ControlText;
            label44.Location = new System.Drawing.Point(37, 176);
            label44.Name = "label44";
            label44.Size = new System.Drawing.Size(96, 13);
            label44.TabIndex = 105;
            label44.Text = "Status S.M.A.R.T.:";
            // 
            // configurableQualityPictureBox30
            // 
            configurableQualityPictureBox30.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox30.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox30.Location = new System.Drawing.Point(7, 456);
            configurableQualityPictureBox30.Name = "configurableQualityPictureBox30";
            configurableQualityPictureBox30.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox30.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox30.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox30.TabIndex = 104;
            configurableQualityPictureBox30.TabStop = false;
            // 
            // label45
            // 
            label45.AutoSize = true;
            label45.ForeColor = System.Drawing.SystemColors.ControlText;
            label45.Location = new System.Drawing.Point(37, 488);
            label45.Name = "label45";
            label45.Size = new System.Drawing.Size(121, 13);
            label45.TabIndex = 108;
            label45.Text = "Versão do módulo TPM:";
            // 
            // progressBar1
            // 
            progressBar1.Location = new System.Drawing.Point(6, 533);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(525, 33);
            progressBar1.TabIndex = 69;
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.BackColor = System.Drawing.Color.Transparent;
            label28.ForeColor = System.Drawing.SystemColors.ControlText;
            label28.Location = new System.Drawing.Point(260, 515);
            label28.Name = "label28";
            label28.Size = new System.Drawing.Size(10, 13);
            label28.TabIndex = 70;
            label28.Text = "-";
            // 
            // lblVT
            // 
            lblVT.AutoSize = true;
            lblVT.ForeColor = System.Drawing.Color.Silver;
            lblVT.Location = new System.Drawing.Point(203, 462);
            lblVT.Name = "lblVT";
            lblVT.Size = new System.Drawing.Size(10, 13);
            lblVT.TabIndex = 103;
            lblVT.Text = "-";
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.ForeColor = System.Drawing.SystemColors.ControlText;
            label33.Location = new System.Drawing.Point(37, 462);
            label33.Name = "label33";
            label33.Size = new System.Drawing.Size(141, 13);
            label33.TabIndex = 102;
            label33.Text = "Tecnologia de Virtualização:";
            // 
            // configurableQualityPictureBox2
            // 
            configurableQualityPictureBox2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox2.Location = new System.Drawing.Point(7, 14);
            configurableQualityPictureBox2.Name = "configurableQualityPictureBox2";
            configurableQualityPictureBox2.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox2.TabIndex = 101;
            configurableQualityPictureBox2.TabStop = false;
            // 
            // configurableQualityPictureBox17
            // 
            configurableQualityPictureBox17.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox17.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox17.Location = new System.Drawing.Point(7, 430);
            configurableQualityPictureBox17.Name = "configurableQualityPictureBox17";
            configurableQualityPictureBox17.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox17.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox17.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox17.TabIndex = 87;
            configurableQualityPictureBox17.TabStop = false;
            // 
            // configurableQualityPictureBox16
            // 
            configurableQualityPictureBox16.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox16.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox16.Location = new System.Drawing.Point(7, 404);
            configurableQualityPictureBox16.Name = "configurableQualityPictureBox16";
            configurableQualityPictureBox16.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox16.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox16.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox16.TabIndex = 86;
            configurableQualityPictureBox16.TabStop = false;
            // 
            // configurableQualityPictureBox15
            // 
            configurableQualityPictureBox15.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox15.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox15.Location = new System.Drawing.Point(7, 378);
            configurableQualityPictureBox15.Name = "configurableQualityPictureBox15";
            configurableQualityPictureBox15.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox15.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox15.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox15.TabIndex = 85;
            configurableQualityPictureBox15.TabStop = false;
            // 
            // configurableQualityPictureBox14
            // 
            configurableQualityPictureBox14.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox14.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox14.Location = new System.Drawing.Point(7, 352);
            configurableQualityPictureBox14.Name = "configurableQualityPictureBox14";
            configurableQualityPictureBox14.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox14.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox14.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox14.TabIndex = 84;
            configurableQualityPictureBox14.TabStop = false;
            // 
            // configurableQualityPictureBox13
            // 
            configurableQualityPictureBox13.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox13.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox13.Location = new System.Drawing.Point(7, 326);
            configurableQualityPictureBox13.Name = "configurableQualityPictureBox13";
            configurableQualityPictureBox13.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox13.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox13.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox13.TabIndex = 83;
            configurableQualityPictureBox13.TabStop = false;
            // 
            // configurableQualityPictureBox12
            // 
            configurableQualityPictureBox12.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox12.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox12.Location = new System.Drawing.Point(7, 300);
            configurableQualityPictureBox12.Name = "configurableQualityPictureBox12";
            configurableQualityPictureBox12.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox12.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox12.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox12.TabIndex = 82;
            configurableQualityPictureBox12.TabStop = false;
            // 
            // configurableQualityPictureBox11
            // 
            configurableQualityPictureBox11.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox11.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox11.Location = new System.Drawing.Point(7, 274);
            configurableQualityPictureBox11.Name = "configurableQualityPictureBox11";
            configurableQualityPictureBox11.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox11.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox11.TabIndex = 81;
            configurableQualityPictureBox11.TabStop = false;
            // 
            // configurableQualityPictureBox10
            // 
            configurableQualityPictureBox10.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox10.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox10.Location = new System.Drawing.Point(7, 248);
            configurableQualityPictureBox10.Name = "configurableQualityPictureBox10";
            configurableQualityPictureBox10.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox10.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox10.TabIndex = 80;
            configurableQualityPictureBox10.TabStop = false;
            // 
            // configurableQualityPictureBox9
            // 
            configurableQualityPictureBox9.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox9.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox9.Location = new System.Drawing.Point(7, 222);
            configurableQualityPictureBox9.Name = "configurableQualityPictureBox9";
            configurableQualityPictureBox9.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox9.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox9.TabIndex = 79;
            configurableQualityPictureBox9.TabStop = false;
            // 
            // configurableQualityPictureBox8
            // 
            configurableQualityPictureBox8.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox8.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox8.Location = new System.Drawing.Point(7, 196);
            configurableQualityPictureBox8.Name = "configurableQualityPictureBox8";
            configurableQualityPictureBox8.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox8.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox8.TabIndex = 78;
            configurableQualityPictureBox8.TabStop = false;
            // 
            // configurableQualityPictureBox7
            // 
            configurableQualityPictureBox7.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox7.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox7.Location = new System.Drawing.Point(7, 144);
            configurableQualityPictureBox7.Name = "configurableQualityPictureBox7";
            configurableQualityPictureBox7.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox7.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox7.TabIndex = 77;
            configurableQualityPictureBox7.TabStop = false;
            // 
            // configurableQualityPictureBox6
            // 
            configurableQualityPictureBox6.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox6.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox6.Location = new System.Drawing.Point(7, 118);
            configurableQualityPictureBox6.Name = "configurableQualityPictureBox6";
            configurableQualityPictureBox6.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox6.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox6.TabIndex = 76;
            configurableQualityPictureBox6.TabStop = false;
            // 
            // configurableQualityPictureBox5
            // 
            configurableQualityPictureBox5.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox5.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox5.Location = new System.Drawing.Point(7, 92);
            configurableQualityPictureBox5.Name = "configurableQualityPictureBox5";
            configurableQualityPictureBox5.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox5.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox5.TabIndex = 75;
            configurableQualityPictureBox5.TabStop = false;
            // 
            // configurableQualityPictureBox4
            // 
            configurableQualityPictureBox4.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox4.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox4.Location = new System.Drawing.Point(7, 66);
            configurableQualityPictureBox4.Name = "configurableQualityPictureBox4";
            configurableQualityPictureBox4.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox4.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox4.TabIndex = 74;
            configurableQualityPictureBox4.TabStop = false;
            // 
            // configurableQualityPictureBox3
            // 
            configurableQualityPictureBox3.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox3.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox3.Location = new System.Drawing.Point(7, 40);
            configurableQualityPictureBox3.Name = "configurableQualityPictureBox3";
            configurableQualityPictureBox3.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox3.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox3.TabIndex = 73;
            configurableQualityPictureBox3.TabStop = false;
            // 
            // lblSecBoot
            // 
            lblSecBoot.AutoSize = true;
            lblSecBoot.ForeColor = System.Drawing.Color.Silver;
            lblSecBoot.Location = new System.Drawing.Point(203, 436);
            lblSecBoot.Name = "lblSecBoot";
            lblSecBoot.Size = new System.Drawing.Size(10, 13);
            lblSecBoot.TabIndex = 71;
            lblSecBoot.Text = "-";
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.ForeColor = System.Drawing.SystemColors.ControlText;
            label32.Location = new System.Drawing.Point(37, 436);
            label32.Name = "label32";
            label32.Size = new System.Drawing.Size(69, 13);
            label32.TabIndex = 70;
            label32.Text = "Secure Boot:";
            // 
            // lblMediaOperation
            // 
            lblMediaOperation.AutoSize = true;
            lblMediaOperation.ForeColor = System.Drawing.Color.Silver;
            lblMediaOperation.Location = new System.Drawing.Point(203, 228);
            lblMediaOperation.Name = "lblMediaOperation";
            lblMediaOperation.Size = new System.Drawing.Size(10, 13);
            lblMediaOperation.TabIndex = 69;
            lblMediaOperation.Text = "-";
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.ForeColor = System.Drawing.SystemColors.ControlText;
            label30.Location = new System.Drawing.Point(37, 228);
            label30.Name = "label30";
            label30.Size = new System.Drawing.Size(154, 13);
            label30.TabIndex = 68;
            label30.Text = "Modo de operação SATA/M.2:";
            // 
            // lblGPUInfo
            // 
            lblGPUInfo.AutoSize = true;
            lblGPUInfo.ForeColor = System.Drawing.Color.Silver;
            lblGPUInfo.Location = new System.Drawing.Point(203, 254);
            lblGPUInfo.Name = "lblGPUInfo";
            lblGPUInfo.Size = new System.Drawing.Size(10, 13);
            lblGPUInfo.TabIndex = 67;
            lblGPUInfo.Text = "-";
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.ForeColor = System.Drawing.SystemColors.ControlText;
            label29.Location = new System.Drawing.Point(37, 254);
            label29.Name = "label29";
            label29.Size = new System.Drawing.Size(126, 13);
            label29.TabIndex = 66;
            label29.Text = "Placa de Vídeo e vRAM:";
            // 
            // lblMediaType
            // 
            lblMediaType.AutoSize = true;
            lblMediaType.ForeColor = System.Drawing.Color.Silver;
            lblMediaType.Location = new System.Drawing.Point(203, 202);
            lblMediaType.Name = "lblMediaType";
            lblMediaType.Size = new System.Drawing.Size(10, 13);
            lblMediaType.TabIndex = 65;
            lblMediaType.Text = "-";
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.ForeColor = System.Drawing.SystemColors.ControlText;
            label27.Location = new System.Drawing.Point(37, 202);
            label27.Name = "label27";
            label27.Size = new System.Drawing.Size(124, 13);
            label27.TabIndex = 64;
            label27.Text = "Tipo de armazenamento:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(comboBoxBattery);
            groupBox2.Controls.Add(comboBoxStandard);
            groupBox2.Controls.Add(comboBoxActiveDirectory);
            groupBox2.Controls.Add(comboBoxTag);
            groupBox2.Controls.Add(comboBoxInUse);
            groupBox2.Controls.Add(comboBoxType);
            groupBox2.Controls.Add(comboBoxBuilding);
            groupBox2.Controls.Add(label48);
            groupBox2.Controls.Add(label47);
            groupBox2.Controls.Add(configurableQualityPictureBox35);
            groupBox2.Controls.Add(label31);
            groupBox2.Controls.Add(textBoxTicket);
            groupBox2.Controls.Add(configurableQualityPictureBox34);
            groupBox2.Controls.Add(label42);
            groupBox2.Controls.Add(label41);
            groupBox2.Controls.Add(label46);
            groupBox2.Controls.Add(label40);
            groupBox2.Controls.Add(label39);
            groupBox2.Controls.Add(label38);
            groupBox2.Controls.Add(label37);
            groupBox2.Controls.Add(label36);
            groupBox2.Controls.Add(label35);
            groupBox2.Controls.Add(studentRadioButton);
            groupBox2.Controls.Add(employeeRadioButton);
            groupBox2.Controls.Add(configurableQualityPictureBox31);
            groupBox2.Controls.Add(label34);
            groupBox2.Controls.Add(configurableQualityPictureBox25);
            groupBox2.Controls.Add(configurableQualityPictureBox28);
            groupBox2.Controls.Add(configurableQualityPictureBox27);
            groupBox2.Controls.Add(configurableQualityPictureBox26);
            groupBox2.Controls.Add(configurableQualityPictureBox24);
            groupBox2.Controls.Add(configurableQualityPictureBox23);
            groupBox2.Controls.Add(configurableQualityPictureBox22);
            groupBox2.Controls.Add(configurableQualityPictureBox21);
            groupBox2.Controls.Add(configurableQualityPictureBox20);
            groupBox2.Controls.Add(configurableQualityPictureBox19);
            groupBox2.Controls.Add(configurableQualityPictureBox18);
            groupBox2.Controls.Add(dateTimePicker1);
            groupBox2.Controls.Add(groupBox3);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(label12);
            groupBox2.Controls.Add(label13);
            groupBox2.Controls.Add(textBoxPatrimony);
            groupBox2.Controls.Add(textBoxSeal);
            groupBox2.Controls.Add(label23);
            groupBox2.Controls.Add(textBoxRoom);
            groupBox2.Controls.Add(label14);
            groupBox2.Controls.Add(label15);
            groupBox2.Controls.Add(label16);
            groupBox2.Controls.Add(label20);
            groupBox2.Controls.Add(label17);
            groupBox2.Controls.Add(textBoxLetter);
            groupBox2.Controls.Add(label18);
            groupBox2.Controls.Add(label19);
            groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            groupBox2.Location = new System.Drawing.Point(575, 113);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(450, 390);
            groupBox2.TabIndex = 66;
            groupBox2.TabStop = false;
            groupBox2.Text = "Dados do patrimônio, manutenção e de localização";
            // 
            // comboBoxBattery
            // 
            comboBoxBattery.BackColor = System.Drawing.SystemColors.Window;
            comboBoxBattery.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxBattery.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxBattery.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxBattery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBoxBattery.FormattingEnabled = true;
            comboBoxBattery.Location = new System.Drawing.Point(185, 241);
            comboBoxBattery.Name = "comboBoxBattery";
            comboBoxBattery.Size = new System.Drawing.Size(84, 21);
            comboBoxBattery.TabIndex = 47;
            // 
            // comboBoxStandard
            // 
            comboBoxStandard.BackColor = System.Drawing.SystemColors.Window;
            comboBoxStandard.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxStandard.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxStandard.Enabled = false;
            comboBoxStandard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBoxStandard.FormattingEnabled = true;
            comboBoxStandard.Location = new System.Drawing.Point(348, 215);
            comboBoxStandard.Name = "comboBoxStandard";
            comboBoxStandard.Size = new System.Drawing.Size(96, 21);
            comboBoxStandard.TabIndex = 46;
            // 
            // comboBoxActiveDirectory
            // 
            comboBoxActiveDirectory.BackColor = System.Drawing.SystemColors.Window;
            comboBoxActiveDirectory.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxActiveDirectory.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxActiveDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxActiveDirectory.Enabled = false;
            comboBoxActiveDirectory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBoxActiveDirectory.FormattingEnabled = true;
            comboBoxActiveDirectory.Location = new System.Drawing.Point(185, 215);
            comboBoxActiveDirectory.Name = "comboBoxActiveDirectory";
            comboBoxActiveDirectory.Size = new System.Drawing.Size(84, 21);
            comboBoxActiveDirectory.TabIndex = 45;
            // 
            // comboBoxTag
            // 
            comboBoxTag.BackColor = System.Drawing.SystemColors.Window;
            comboBoxTag.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxTag.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxTag.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBoxTag.FormattingEnabled = true;
            comboBoxTag.Location = new System.Drawing.Point(384, 121);
            comboBoxTag.Name = "comboBoxTag";
            comboBoxTag.Size = new System.Drawing.Size(60, 21);
            comboBoxTag.TabIndex = 41;
            // 
            // comboBoxInUse
            // 
            comboBoxInUse.BackColor = System.Drawing.SystemColors.Window;
            comboBoxInUse.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxInUse.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxInUse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxInUse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBoxInUse.FormattingEnabled = true;
            comboBoxInUse.Location = new System.Drawing.Point(384, 95);
            comboBoxInUse.Name = "comboBoxInUse";
            comboBoxInUse.Size = new System.Drawing.Size(60, 21);
            comboBoxInUse.TabIndex = 39;
            // 
            // comboBoxType
            // 
            comboBoxType.BackColor = System.Drawing.SystemColors.Window;
            comboBoxType.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxType.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBoxType.FormattingEnabled = true;
            comboBoxType.Location = new System.Drawing.Point(185, 121);
            comboBoxType.Name = "comboBoxType";
            comboBoxType.Size = new System.Drawing.Size(101, 21);
            comboBoxType.TabIndex = 40;
            // 
            // comboBoxBuilding
            // 
            comboBoxBuilding.BackColor = System.Drawing.SystemColors.Window;
            comboBoxBuilding.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxBuilding.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxBuilding.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBoxBuilding.FormattingEnabled = true;
            comboBoxBuilding.Location = new System.Drawing.Point(185, 95);
            comboBoxBuilding.Name = "comboBoxBuilding";
            comboBoxBuilding.Size = new System.Drawing.Size(101, 21);
            comboBoxBuilding.TabIndex = 38;
            // 
            // label48
            // 
            label48.AutoSize = true;
            label48.ForeColor = System.Drawing.Color.Red;
            label48.Location = new System.Drawing.Point(367, 244);
            label48.Name = "label48";
            label48.Size = new System.Drawing.Size(17, 13);
            label48.TabIndex = 118;
            label48.Text = "✱";
            // 
            // label47
            // 
            label47.AutoSize = true;
            label47.ForeColor = System.Drawing.Color.Red;
            label47.Location = new System.Drawing.Point(146, 244);
            label47.Name = "label47";
            label47.Size = new System.Drawing.Size(17, 13);
            label47.TabIndex = 114;
            label47.Text = "✱";
            // 
            // configurableQualityPictureBox35
            // 
            configurableQualityPictureBox35.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox35.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox35.Location = new System.Drawing.Point(273, 238);
            configurableQualityPictureBox35.Name = "configurableQualityPictureBox35";
            configurableQualityPictureBox35.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox35.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox35.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox35.TabIndex = 117;
            configurableQualityPictureBox35.TabStop = false;
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.ForeColor = System.Drawing.SystemColors.ControlText;
            label31.Location = new System.Drawing.Point(303, 244);
            label31.Name = "label31";
            label31.Size = new System.Drawing.Size(69, 13);
            label31.TabIndex = 116;
            label31.Text = "Nº chamado:";
            // 
            // textBoxTicket
            // 
            textBoxTicket.BackColor = System.Drawing.SystemColors.Window;
            textBoxTicket.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxTicket.Location = new System.Drawing.Point(384, 241);
            textBoxTicket.MaxLength = 6;
            textBoxTicket.Name = "textBoxTicket";
            textBoxTicket.Size = new System.Drawing.Size(60, 20);
            textBoxTicket.TabIndex = 48;
            textBoxTicket.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // configurableQualityPictureBox34
            // 
            configurableQualityPictureBox34.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox34.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox34.Location = new System.Drawing.Point(7, 238);
            configurableQualityPictureBox34.Name = "configurableQualityPictureBox34";
            configurableQualityPictureBox34.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox34.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox34.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox34.TabIndex = 113;
            configurableQualityPictureBox34.TabStop = false;
            // 
            // label42
            // 
            label42.AutoSize = true;
            label42.ForeColor = System.Drawing.Color.Red;
            label42.Location = new System.Drawing.Point(103, 176);
            label42.Name = "label42";
            label42.Size = new System.Drawing.Size(17, 13);
            label42.TabIndex = 112;
            label42.Text = "✱";
            // 
            // label41
            // 
            label41.AutoSize = true;
            label41.ForeColor = System.Drawing.Color.Red;
            label41.Location = new System.Drawing.Point(367, 124);
            label41.Name = "label41";
            label41.Size = new System.Drawing.Size(17, 13);
            label41.TabIndex = 111;
            label41.Text = "✱";
            // 
            // label46
            // 
            label46.AutoSize = true;
            label46.ForeColor = System.Drawing.SystemColors.ControlText;
            label46.Location = new System.Drawing.Point(37, 244);
            label46.Name = "label46";
            label46.Size = new System.Drawing.Size(112, 13);
            label46.TabIndex = 111;
            label46.Text = "Houve troca de pilha?";
            // 
            // label40
            // 
            label40.AutoSize = true;
            label40.ForeColor = System.Drawing.Color.Red;
            label40.Location = new System.Drawing.Point(64, 124);
            label40.Name = "label40";
            label40.Size = new System.Drawing.Size(17, 13);
            label40.TabIndex = 110;
            label40.Text = "✱";
            // 
            // label39
            // 
            label39.AutoSize = true;
            label39.ForeColor = System.Drawing.Color.Red;
            label39.Location = new System.Drawing.Point(363, 98);
            label39.Name = "label39";
            label39.Size = new System.Drawing.Size(17, 13);
            label39.TabIndex = 109;
            label39.Text = "✱";
            // 
            // label38
            // 
            label38.AutoSize = true;
            label38.ForeColor = System.Drawing.Color.Red;
            label38.Location = new System.Drawing.Point(73, 98);
            label38.Name = "label38";
            label38.Size = new System.Drawing.Size(17, 13);
            label38.TabIndex = 108;
            label38.Text = "✱";
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.ForeColor = System.Drawing.Color.Red;
            label37.Location = new System.Drawing.Point(167, 72);
            label37.Name = "label37";
            label37.Size = new System.Drawing.Size(17, 13);
            label37.TabIndex = 107;
            label37.Text = "✱";
            // 
            // label36
            // 
            label36.AutoSize = true;
            label36.ForeColor = System.Drawing.Color.Red;
            label36.Location = new System.Drawing.Point(92, 20);
            label36.Name = "label36";
            label36.Size = new System.Drawing.Size(17, 13);
            label36.TabIndex = 106;
            label36.Text = "✱";
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.ForeColor = System.Drawing.Color.Red;
            label35.Location = new System.Drawing.Point(258, 0);
            label35.Name = "label35";
            label35.Size = new System.Drawing.Size(152, 13);
            label35.TabIndex = 105;
            label35.Text = "✱ = Preenchimento obrigatório";
            // 
            // studentRadioButton
            // 
            studentRadioButton.AutoSize = true;
            studentRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            studentRadioButton.Location = new System.Drawing.Point(185, 192);
            studentRadioButton.Name = "studentRadioButton";
            studentRadioButton.Size = new System.Drawing.Size(246, 17);
            studentRadioButton.TabIndex = 44;
            studentRadioButton.TabStop = true;
            studentRadioButton.Text = "Aluno (computador de laboratório/sala de aula)";
            studentRadioButton.UseVisualStyleBackColor = true;
            studentRadioButton.CheckedChanged += new System.EventHandler(StudentButton2_CheckedChanged);
            // 
            // employeeRadioButton
            // 
            employeeRadioButton.AutoSize = true;
            employeeRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            employeeRadioButton.Location = new System.Drawing.Point(185, 174);
            employeeRadioButton.Name = "employeeRadioButton";
            employeeRadioButton.Size = new System.Drawing.Size(242, 17);
            employeeRadioButton.TabIndex = 43;
            employeeRadioButton.TabStop = true;
            employeeRadioButton.Text = "Funcionário/Bolsista (computador de trabalho)";
            employeeRadioButton.UseVisualStyleBackColor = true;
            employeeRadioButton.CheckedChanged += new System.EventHandler(EmployeeButton1_CheckedChanged);
            // 
            // configurableQualityPictureBox31
            // 
            configurableQualityPictureBox31.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox31.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox31.Location = new System.Drawing.Point(7, 170);
            configurableQualityPictureBox31.Name = "configurableQualityPictureBox31";
            configurableQualityPictureBox31.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox31.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox31.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox31.TabIndex = 102;
            configurableQualityPictureBox31.TabStop = false;
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.ForeColor = System.Drawing.SystemColors.ControlText;
            label34.Location = new System.Drawing.Point(37, 176);
            label34.Name = "label34";
            label34.Size = new System.Drawing.Size(70, 13);
            label34.TabIndex = 101;
            label34.Text = "Quem usará?";
            // 
            // configurableQualityPictureBox25
            // 
            configurableQualityPictureBox25.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox25.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox25.Location = new System.Drawing.Point(292, 66);
            configurableQualityPictureBox25.Name = "configurableQualityPictureBox25";
            configurableQualityPictureBox25.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox25.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox25.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox25.TabIndex = 100;
            configurableQualityPictureBox25.TabStop = false;
            // 
            // configurableQualityPictureBox28
            // 
            configurableQualityPictureBox28.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox28.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox28.Location = new System.Drawing.Point(7, 118);
            configurableQualityPictureBox28.Name = "configurableQualityPictureBox28";
            configurableQualityPictureBox28.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox28.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox28.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox28.TabIndex = 98;
            configurableQualityPictureBox28.TabStop = false;
            // 
            // configurableQualityPictureBox27
            // 
            configurableQualityPictureBox27.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox27.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox27.Location = new System.Drawing.Point(292, 118);
            configurableQualityPictureBox27.Name = "configurableQualityPictureBox27";
            configurableQualityPictureBox27.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox27.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox27.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox27.TabIndex = 97;
            configurableQualityPictureBox27.TabStop = false;
            // 
            // configurableQualityPictureBox26
            // 
            configurableQualityPictureBox26.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox26.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox26.Location = new System.Drawing.Point(292, 92);
            configurableQualityPictureBox26.Name = "configurableQualityPictureBox26";
            configurableQualityPictureBox26.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox26.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox26.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox26.TabIndex = 96;
            configurableQualityPictureBox26.TabStop = false;
            // 
            // configurableQualityPictureBox24
            // 
            configurableQualityPictureBox24.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox24.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox24.Location = new System.Drawing.Point(7, 144);
            configurableQualityPictureBox24.Name = "configurableQualityPictureBox24";
            configurableQualityPictureBox24.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox24.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox24.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox24.TabIndex = 94;
            configurableQualityPictureBox24.TabStop = false;
            // 
            // configurableQualityPictureBox23
            // 
            configurableQualityPictureBox23.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox23.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox23.Location = new System.Drawing.Point(273, 212);
            configurableQualityPictureBox23.Name = "configurableQualityPictureBox23";
            configurableQualityPictureBox23.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox23.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox23.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox23.TabIndex = 93;
            configurableQualityPictureBox23.TabStop = false;
            // 
            // configurableQualityPictureBox22
            // 
            configurableQualityPictureBox22.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox22.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox22.Location = new System.Drawing.Point(7, 212);
            configurableQualityPictureBox22.Name = "configurableQualityPictureBox22";
            configurableQualityPictureBox22.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox22.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox22.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox22.TabIndex = 92;
            configurableQualityPictureBox22.TabStop = false;
            // 
            // configurableQualityPictureBox21
            // 
            configurableQualityPictureBox21.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox21.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox21.Location = new System.Drawing.Point(7, 92);
            configurableQualityPictureBox21.Name = "configurableQualityPictureBox21";
            configurableQualityPictureBox21.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox21.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox21.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox21.TabIndex = 91;
            configurableQualityPictureBox21.TabStop = false;
            // 
            // configurableQualityPictureBox20
            // 
            configurableQualityPictureBox20.CompositingQuality = null;
            configurableQualityPictureBox20.InterpolationMode = null;
            configurableQualityPictureBox20.Location = new System.Drawing.Point(7, 66);
            configurableQualityPictureBox20.Name = "configurableQualityPictureBox20";
            configurableQualityPictureBox20.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox20.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox20.SmoothingMode = null;
            configurableQualityPictureBox20.TabIndex = 90;
            configurableQualityPictureBox20.TabStop = false;
            // 
            // configurableQualityPictureBox19
            // 
            configurableQualityPictureBox19.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox19.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox19.Location = new System.Drawing.Point(7, 40);
            configurableQualityPictureBox19.Name = "configurableQualityPictureBox19";
            configurableQualityPictureBox19.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox19.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            configurableQualityPictureBox19.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox19.TabIndex = 89;
            configurableQualityPictureBox19.TabStop = false;
            // 
            // configurableQualityPictureBox18
            // 
            configurableQualityPictureBox18.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            configurableQualityPictureBox18.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            configurableQualityPictureBox18.Location = new System.Drawing.Point(7, 14);
            configurableQualityPictureBox18.Name = "configurableQualityPictureBox18";
            configurableQualityPictureBox18.Size = new System.Drawing.Size(25, 25);
            configurableQualityPictureBox18.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox18.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            configurableQualityPictureBox18.TabIndex = 88;
            configurableQualityPictureBox18.TabStop = false;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            dateTimePicker1.Location = new System.Drawing.Point(185, 148);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new System.Drawing.Size(259, 20);
            dateTimePicker1.TabIndex = 42;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(loadingCircle21);
            groupBox3.Controls.Add(loadingCircle20);
            groupBox3.Controls.Add(lblMaintenanceSince);
            groupBox3.Controls.Add(lblInstallSince);
            groupBox3.Controls.Add(label43);
            groupBox3.Controls.Add(textBox5);
            groupBox3.Controls.Add(textBox6);
            groupBox3.Controls.Add(formatButton);
            groupBox3.Controls.Add(maintenanceButton);
            groupBox3.ForeColor = System.Drawing.SystemColors.ControlText;
            groupBox3.Location = new System.Drawing.Point(6, 266);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(438, 115);
            groupBox3.TabIndex = 72;
            groupBox3.TabStop = false;
            groupBox3.Text = "Tipo de serviço";
            // 
            // loadingCircle21
            // 
            loadingCircle21.Active = false;
            loadingCircle21.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle21.InnerCircleRadius = 5;
            loadingCircle21.Location = new System.Drawing.Point(89, 57);
            loadingCircle21.Name = "loadingCircle21";
            loadingCircle21.NumberSpoke = 12;
            loadingCircle21.OuterCircleRadius = 11;
            loadingCircle21.RotationSpeed = 1;
            loadingCircle21.Size = new System.Drawing.Size(37, 25);
            loadingCircle21.SpokeThickness = 2;
            loadingCircle21.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle21.TabIndex = 133;
            loadingCircle21.Text = "loadingCircle21";
            // 
            // loadingCircle20
            // 
            loadingCircle20.Active = false;
            loadingCircle20.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle20.InnerCircleRadius = 5;
            loadingCircle20.Location = new System.Drawing.Point(89, 16);
            loadingCircle20.Name = "loadingCircle20";
            loadingCircle20.NumberSpoke = 12;
            loadingCircle20.OuterCircleRadius = 11;
            loadingCircle20.RotationSpeed = 1;
            loadingCircle20.Size = new System.Drawing.Size(37, 25);
            loadingCircle20.SpokeThickness = 2;
            loadingCircle20.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle20.TabIndex = 132;
            loadingCircle20.Text = "loadingCircle20";
            // 
            // lblMaintenanceSince
            // 
            lblMaintenanceSince.AutoSize = true;
            lblMaintenanceSince.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            lblMaintenanceSince.Location = new System.Drawing.Point(101, 61);
            lblMaintenanceSince.Name = "lblMaintenanceSince";
            lblMaintenanceSince.Size = new System.Drawing.Size(10, 13);
            lblMaintenanceSince.TabIndex = 121;
            lblMaintenanceSince.Text = "-";
            // 
            // lblInstallSince
            // 
            lblInstallSince.AutoSize = true;
            lblInstallSince.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            lblInstallSince.Location = new System.Drawing.Point(101, 22);
            lblInstallSince.Name = "lblInstallSince";
            lblInstallSince.Size = new System.Drawing.Size(10, 13);
            lblInstallSince.TabIndex = 120;
            lblInstallSince.Text = "-";
            // 
            // label43
            // 
            label43.AutoSize = true;
            label43.ForeColor = System.Drawing.Color.Red;
            label43.Location = new System.Drawing.Point(82, 0);
            label43.Name = "label43";
            label43.Size = new System.Drawing.Size(17, 13);
            label43.TabIndex = 113;
            label43.Text = "✱";
            // 
            // textBox5
            // 
            textBox5.BackColor = System.Drawing.SystemColors.Control;
            textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBox5.Enabled = false;
            textBox5.ForeColor = System.Drawing.SystemColors.WindowText;
            textBox5.Location = new System.Drawing.Point(29, 38);
            textBox5.Multiline = true;
            textBox5.Name = "textBox5";
            textBox5.ReadOnly = true;
            textBox5.Size = new System.Drawing.Size(391, 19);
            textBox5.TabIndex = 76;
            textBox5.Text = "Opção para quando o PC passar por formatação ou reset";
            // 
            // textBox6
            // 
            textBox6.BackColor = System.Drawing.SystemColors.Control;
            textBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBox6.Enabled = false;
            textBox6.ForeColor = System.Drawing.SystemColors.WindowText;
            textBox6.Location = new System.Drawing.Point(29, 78);
            textBox6.Multiline = true;
            textBox6.Name = "textBox6";
            textBox6.ReadOnly = true;
            textBox6.Size = new System.Drawing.Size(391, 25);
            textBox6.TabIndex = 77;
            textBox6.Text = "Opção para quando o PC passar por manutenção preventiva, sem a necessidade de for" +
    "matação ou reset";
            // 
            // formatButton
            // 
            formatButton.AutoSize = true;
            formatButton.ForeColor = System.Drawing.SystemColors.ControlText;
            formatButton.Location = new System.Drawing.Point(10, 20);
            formatButton.Name = "formatButton";
            formatButton.Size = new System.Drawing.Size(81, 17);
            formatButton.TabIndex = 49;
            formatButton.Text = "Formatação";
            formatButton.UseVisualStyleBackColor = true;
            formatButton.CheckedChanged += new System.EventHandler(FormatButton1_CheckedChanged);
            // 
            // maintenanceButton
            // 
            maintenanceButton.AutoSize = true;
            maintenanceButton.ForeColor = System.Drawing.SystemColors.ControlText;
            maintenanceButton.Location = new System.Drawing.Point(10, 59);
            maintenanceButton.Name = "maintenanceButton";
            maintenanceButton.Size = new System.Drawing.Size(85, 17);
            maintenanceButton.TabIndex = 50;
            maintenanceButton.Text = "Manutenção";
            maintenanceButton.UseVisualStyleBackColor = true;
            maintenanceButton.CheckedChanged += new System.EventHandler(MaintenanceButton2_CheckedChanged);
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.ForeColor = System.Drawing.SystemColors.ControlText;
            label15.Location = new System.Drawing.Point(37, 218);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(137, 13);
            label15.TabIndex = 14;
            label15.Text = "Cadastrado no servidor AD:";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.ForeColor = System.Drawing.SystemColors.ControlText;
            label17.Location = new System.Drawing.Point(303, 218);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(44, 13);
            label17.TabIndex = 15;
            label17.Text = "Padrão:";
            // 
            // lblAgentName
            // 
            lblAgentName.AutoSize = true;
            lblAgentName.ForeColor = System.Drawing.SystemColors.ControlText;
            lblAgentName.Location = new System.Drawing.Point(299, 35);
            lblAgentName.Name = "lblAgentName";
            lblAgentName.Size = new System.Drawing.Size(10, 13);
            lblAgentName.TabIndex = 123;
            lblAgentName.Text = "-";
            // 
            // label53
            // 
            label53.AutoSize = true;
            label53.ForeColor = System.Drawing.SystemColors.ControlText;
            label53.Location = new System.Drawing.Point(187, 35);
            label53.Name = "label53";
            label53.Size = new System.Drawing.Size(104, 13);
            label53.TabIndex = 122;
            label53.Text = "Agente responsável:";
            // 
            // lblPortServer
            // 
            lblPortServer.AutoSize = true;
            lblPortServer.ForeColor = System.Drawing.SystemColors.ControlText;
            lblPortServer.Location = new System.Drawing.Point(50, 35);
            lblPortServer.Name = "lblPortServer";
            lblPortServer.Size = new System.Drawing.Size(10, 13);
            lblPortServer.TabIndex = 121;
            lblPortServer.Text = "-";
            // 
            // lblIPServer
            // 
            lblIPServer.AutoSize = true;
            lblIPServer.ForeColor = System.Drawing.SystemColors.ControlText;
            lblIPServer.Location = new System.Drawing.Point(50, 16);
            lblIPServer.Name = "lblIPServer";
            lblIPServer.Size = new System.Drawing.Size(10, 13);
            lblIPServer.TabIndex = 120;
            lblIPServer.Text = "-";
            // 
            // label49
            // 
            label49.AutoSize = true;
            label49.ForeColor = System.Drawing.SystemColors.ControlText;
            label49.Location = new System.Drawing.Point(7, 16);
            label49.Name = "label49";
            label49.Size = new System.Drawing.Size(20, 13);
            label49.TabIndex = 119;
            label49.Text = "IP:";
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.BackColor = System.Drawing.Color.Transparent;
            label26.ForeColor = System.Drawing.Color.Silver;
            label26.Location = new System.Drawing.Point(299, 16);
            label26.Name = "label26";
            label26.Size = new System.Drawing.Size(10, 13);
            label26.TabIndex = 72;
            label26.Text = "-";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            toolStripStatusLabel2.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            toolStripStatusLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            toolStripStatusLabel2.Size = new System.Drawing.Size(4, 19);
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            comboBoxTheme,
            logLabel,
            aboutLabel,
            toolStripStatusLabel1,
            toolStripStatusLabel2});
            statusStrip1.Location = new System.Drawing.Point(0, 702);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            statusStrip1.Size = new System.Drawing.Size(1056, 24);
            statusStrip1.TabIndex = 60;
            statusStrip1.Text = "statusStrip1";
            // 
            // comboBoxTheme
            // 
            comboBoxTheme.BackColor = System.Drawing.SystemColors.Control;
            comboBoxTheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            comboBoxTheme.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem1,
            toolStripMenuItem2,
            toolStripMenuItem3});
            comboBoxTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            comboBoxTheme.ImageTransparentColor = System.Drawing.Color.Magenta;
            comboBoxTheme.Name = "comboBoxTheme";
            comboBoxTheme.Size = new System.Drawing.Size(48, 22);
            comboBoxTheme.Text = "Tema";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.BackColor = System.Drawing.SystemColors.Control;
            toolStripMenuItem1.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(236, 22);
            toolStripMenuItem1.Text = "Automático (Tema do sistema)";
            toolStripMenuItem1.Click += new System.EventHandler(ToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.BackColor = System.Drawing.SystemColors.Control;
            toolStripMenuItem2.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(236, 22);
            toolStripMenuItem2.Text = "Claro";
            toolStripMenuItem2.Click += new System.EventHandler(ToolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.BackColor = System.Drawing.SystemColors.Control;
            toolStripMenuItem3.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new System.Drawing.Size(236, 22);
            toolStripMenuItem3.Text = "Escuro";
            toolStripMenuItem3.Click += new System.EventHandler(ToolStripMenuItem3_Click);
            // 
            // logLabel
            // 
            logLabel.BackColor = System.Drawing.SystemColors.Control;
            logLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            logLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            logLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            logLabel.Name = "logLabel";
            logLabel.Size = new System.Drawing.Size(31, 19);
            logLabel.Text = "Log";
            logLabel.Click += new System.EventHandler(LogLabel_Click);
            logLabel.MouseEnter += new System.EventHandler(LogLabel_MouseEnter);
            logLabel.MouseLeave += new System.EventHandler(LogLabel_MouseLeave);
            // 
            // aboutLabel
            // 
            aboutLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            aboutLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            aboutLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            aboutLabel.Name = "aboutLabel";
            aboutLabel.Size = new System.Drawing.Size(41, 19);
            aboutLabel.Text = "Sobre";
            aboutLabel.Click += new System.EventHandler(AboutLabel_Click);
            aboutLabel.MouseEnter += new System.EventHandler(AboutLabel_MouseEnter);
            aboutLabel.MouseLeave += new System.EventHandler(AboutLabel_MouseLeave);
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.BackColor = System.Drawing.SystemColors.Control;
            toolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            toolStripStatusLabel1.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(917, 19);
            toolStripStatusLabel1.Spring = true;
            // 
            // timer1
            // 
            timer1.Interval = 500;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(webView2);
            groupBox4.ForeColor = System.Drawing.SystemColors.ControlText;
            groupBox4.Location = new System.Drawing.Point(575, 559);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(450, 65);
            groupBox4.TabIndex = 73;
            groupBox4.TabStop = false;
            groupBox4.Text = "Status do cadastro";
            // 
            // webView2
            // 
            webView2.AllowExternalDrop = true;
            webView2.CreationProperties = null;
            webView2.DefaultBackgroundColor = System.Drawing.Color.White;
            webView2.Location = new System.Drawing.Point(1, 13);
            webView2.Name = "webView2";
            webView2.Size = new System.Drawing.Size(448, 51);
            webView2.TabIndex = 0;
            webView2.ZoomFactor = 1D;
            // 
            // configurableQualityPictureBox1
            // 
            configurableQualityPictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
            | System.Windows.Forms.AnchorStyles.Right;
            configurableQualityPictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            configurableQualityPictureBox1.CompositingQuality = null;
            configurableQualityPictureBox1.InitialImage = null;
            configurableQualityPictureBox1.InterpolationMode = null;
            configurableQualityPictureBox1.Location = new System.Drawing.Point(0, 0);
            configurableQualityPictureBox1.Name = "configurableQualityPictureBox1";
            configurableQualityPictureBox1.Size = new System.Drawing.Size(1056, 105);
            configurableQualityPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            configurableQualityPictureBox1.SmoothingMode = null;
            configurableQualityPictureBox1.TabIndex = 64;
            configurableQualityPictureBox1.TabStop = false;
            // 
            // loadingCircle22
            // 
            loadingCircle22.Active = false;
            loadingCircle22.BackColor = System.Drawing.SystemColors.Control;
            loadingCircle22.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle22.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle22.InnerCircleRadius = 5;
            loadingCircle22.Location = new System.Drawing.Point(577, 632);
            loadingCircle22.Name = "loadingCircle22";
            loadingCircle22.NumberSpoke = 12;
            loadingCircle22.OuterCircleRadius = 11;
            loadingCircle22.RightToLeft = System.Windows.Forms.RightToLeft.No;
            loadingCircle22.RotationSpeed = 1;
            loadingCircle22.Size = new System.Drawing.Size(176, 21);
            loadingCircle22.SpokeThickness = 2;
            loadingCircle22.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle22.TabIndex = 134;
            loadingCircle22.Text = "loadingCircle22";
            loadingCircle22.UseWaitCursor = true;
            // 
            // loadingCircle23
            // 
            loadingCircle23.Active = false;
            loadingCircle23.BackColor = System.Drawing.SystemColors.Control;
            loadingCircle23.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle23.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle23.InnerCircleRadius = 5;
            loadingCircle23.Location = new System.Drawing.Point(762, 632);
            loadingCircle23.Name = "loadingCircle23";
            loadingCircle23.NumberSpoke = 12;
            loadingCircle23.OuterCircleRadius = 11;
            loadingCircle23.RotationSpeed = 1;
            loadingCircle23.Size = new System.Drawing.Size(261, 52);
            loadingCircle23.SpokeThickness = 2;
            loadingCircle23.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle23.TabIndex = 134;
            loadingCircle23.Text = "loadingCircle23";
            loadingCircle23.Visible = false;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(loadingCircle24);
            groupBox5.Controls.Add(label49);
            groupBox5.Controls.Add(label21);
            groupBox5.Controls.Add(label22);
            groupBox5.Controls.Add(label26);
            groupBox5.Controls.Add(lblIPServer);
            groupBox5.Controls.Add(lblPortServer);
            groupBox5.Controls.Add(label53);
            groupBox5.Controls.Add(lblAgentName);
            groupBox5.ForeColor = System.Drawing.SystemColors.ControlText;
            groupBox5.Location = new System.Drawing.Point(575, 503);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new System.Drawing.Size(450, 56);
            groupBox5.TabIndex = 132;
            groupBox5.TabStop = false;
            groupBox5.Text = "Servidor SCPD";
            // 
            // loadingCircle24
            // 
            loadingCircle24.Active = false;
            loadingCircle24.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle24.InnerCircleRadius = 5;
            loadingCircle24.Location = new System.Drawing.Point(293, 9);
            loadingCircle24.Name = "loadingCircle24";
            loadingCircle24.NumberSpoke = 12;
            loadingCircle24.OuterCircleRadius = 11;
            loadingCircle24.RotationSpeed = 1;
            loadingCircle24.Size = new System.Drawing.Size(37, 25);
            loadingCircle24.SpokeThickness = 2;
            loadingCircle24.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle24.TabIndex = 134;
            loadingCircle24.Text = "loadingCircle24";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoScroll = true;
            AutoSize = true;
            BackColor = System.Drawing.SystemColors.Control;
            ClientSize = new System.Drawing.Size(1056, 726);
            Controls.Add(groupBox5);
            Controls.Add(loadingCircle23);
            Controls.Add(loadingCircle22);
            Controls.Add(groupBox4);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(configurableQualityPictureBox1);
            Controls.Add(accessSystemButton);
            Controls.Add(collectButton);
            Controls.Add(statusStrip1);
            Controls.Add(registerButton);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Load += new System.EventHandler(Form1_Load);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox33).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox32).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox30).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox17).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox16).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox15).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox14).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox13).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox12).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox11).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox10).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox9).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox8).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox7).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox6).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox3).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox35).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox34).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox31).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox25).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox28).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox27).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox26).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox24).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox23).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox22).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox21).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox20).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox19).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox18).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webView2).EndInit();
            ((System.ComponentModel.ISupportInitialize)configurableQualityPictureBox1).EndInit();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        //Variables being declared
        #region
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
        private ToolStripStatusLabel aboutLabel;
        private GroupBox groupBox5;
        private LoadingCircle loadingCircle24;
        private readonly BackgroundWorker backgroundWorker1;
        private ToolStripStatusLabel logLabel;
        private readonly LogGenerator log;
        private TaskbarManager tbProgMain;

        #endregion

        //Sets service mode to format
        private void FormatButton1_CheckedChanged(object sender, EventArgs e)
        {
            modeURL = StringsAndConstants.formatURL;
        }

        //Sets service mode to maintenance
        private void MaintenanceButton2_CheckedChanged(object sender, EventArgs e)
        {
            modeURL = StringsAndConstants.maintenanceURL;
        }

        //Sets service to employee
        private void EmployeeButton1_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxActiveDirectory.SelectedIndex = 1;
            comboBoxStandard.SelectedIndex = 0;
        }

        //Sets service to student
        private void StudentButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxActiveDirectory.SelectedIndex = 0;
            comboBoxStandard.SelectedIndex = 1;
        }

        //Method for auto selecting the app theme
        private void ComboBoxThemeInit()
        {
            themeBool = MiscMethods.ThemeInit();
            if (themeBool)
            {
                if (HardwareInfo.GetOSInfoAux().Equals(StringsAndConstants.windows10)) //If Windows 10/11
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark); //Sets context menus to dark
                }

                DarkTheme(); //Sets dark theme
            }
            else
            {
                if (HardwareInfo.GetOSInfoAux().Equals(StringsAndConstants.windows10)) //If Windows 10/11
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Light); //Sets context menus to light
                }

                LightTheme(); //Sets light theme
            }
        }

        //Method for setting the auto theme via toolStrip 
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_AUTOTHEME_CHANGE, string.Empty, StringsAndConstants.consoleOutGUI);
            ComboBoxThemeInit();
        }

        //Method for setting the light theme via toolStrip
        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_LIGHTMODE_CHANGE, string.Empty, StringsAndConstants.consoleOutGUI);
            if (HardwareInfo.GetOSInfoAux().Equals(StringsAndConstants.windows10))
            {
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
            }

            LightTheme();
            themeBool = false;
        }

        //Method for setting the dark theme via toolStrip
        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_DARKMODE_CHANGE, string.Empty, StringsAndConstants.consoleOutGUI);
            if (HardwareInfo.GetOSInfoAux().Equals(StringsAndConstants.windows10))
            {
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
            }

            DarkTheme();
            themeBool = true;
        }

        //Sets a light theme for the UI
        private void LightTheme()
        {
            BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            lblBM.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblModel.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblSerialNo.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblProcName.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblPM.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblHDSize.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblMediaType.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblMediaOperation.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblOS.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblHostname.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblMac.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblIP.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblBIOS.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblBIOSType.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblTPM.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblVT.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblSecBoot.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblGPUInfo.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblVT.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblSmart.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblIPServer.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblPortServer.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblAgentName.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;

            lblInstallSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            lblMaintenanceSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            label1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label3.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label4.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label5.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label6.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label7.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label8.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label9.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label10.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label11.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label12.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label13.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label14.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label15.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label16.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label17.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label18.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label19.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label20.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label21.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label22.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label23.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label24.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label25.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label27.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label29.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label28.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label30.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label31.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label32.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label33.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label34.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label35.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            label36.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            label37.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            label38.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            label39.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            label40.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            label41.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            label42.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            label43.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            label44.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label45.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label46.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label47.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            label48.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            label49.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            label53.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            if (offlineMode)
            {
                lblIPServer.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblPortServer.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblAgentName.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }
            loadingCircle22.BackColor = StringsAndConstants.INACTIVE_SYSTEM_BUTTON_COLOR;
            loadingCircle23.BackColor = StringsAndConstants.INACTIVE_SYSTEM_BUTTON_COLOR;

            textBoxPatrimony.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxPatrimony.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxSeal.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxSeal.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxRoom.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxRoom.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxLetter.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxLetter.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBox5.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBox5.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            textBox6.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBox6.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            textBoxTicket.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxTicket.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;

            comboBoxBuilding.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxBuilding.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxBuilding.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxBuilding.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxBuilding.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
            comboBoxActiveDirectory.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxActiveDirectory.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxActiveDirectory.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxActiveDirectory.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxActiveDirectory.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
            comboBoxStandard.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxStandard.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxStandard.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxStandard.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxStandard.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
            comboBoxInUse.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxInUse.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxInUse.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxInUse.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxInUse.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
            comboBoxTag.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxTag.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxTag.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxTag.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxTag.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
            comboBoxType.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxType.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxType.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxType.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
            comboBoxBattery.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxBattery.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxBattery.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxBattery.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxBattery.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
            comboBoxTheme.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            comboBoxTheme.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            employeeRadioButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            studentRadioButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            formatButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            maintenanceButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            registerButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            registerButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            registerButton.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
            registerButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            collectButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            collectButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            collectButton.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
            collectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            accessSystemButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            accessSystemButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            accessSystemButton.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
            accessSystemButton.FlatStyle = System.Windows.Forms.FlatStyle.System;

            groupBox1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            groupBox2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            groupBox3.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            groupBox4.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            groupBox1.Paint += CustomColors.GroupBox_PaintLightTheme;
            groupBox2.Paint += CustomColors.GroupBox_PaintLightTheme;
            groupBox3.Paint += CustomColors.GroupBox_PaintLightTheme;
            groupBox4.Paint += CustomColors.GroupBox_PaintLightTheme;
            groupBox5.Paint += CustomColors.GroupBox_PaintLightTheme;
            separatorH.BackColor = StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR;
            separatorV.BackColor = StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR;

            toolStripStatusLabel1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripStatusLabel1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripStatusLabel2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripStatusLabel2.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripMenuItem1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripMenuItem1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripMenuItem2.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripMenuItem2.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripMenuItem3.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripMenuItem3.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            logLabel.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            logLabel.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            aboutLabel.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            aboutLabel.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            statusStrip1.Renderer = new ModifiedToolStripProfessionalLightTheme();

            toolStripMenuItem1.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_autotheme_light_path));
            toolStripMenuItem2.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_lighttheme_light_path));
            toolStripMenuItem3.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_darktheme_light_path));

            comboBoxTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_autotheme_light_path));
            logLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_log_light_path));
            aboutLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_about_light_path));

            configurableQualityPictureBox1.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.main_banner_light_path));
            configurableQualityPictureBox2.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_brand_light_path));
            configurableQualityPictureBox3.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_model_light_path));
            configurableQualityPictureBox4.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_serial_no_light_path));
            configurableQualityPictureBox5.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_cpu_light_path));
            configurableQualityPictureBox6.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ram_light_path));
            configurableQualityPictureBox7.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_disk_size_light_path));
            configurableQualityPictureBox8.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_hdd_light_path));
            configurableQualityPictureBox9.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ahci_light_path));
            configurableQualityPictureBox10.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_gpu_light_path));
            configurableQualityPictureBox11.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_windows_light_path));
            configurableQualityPictureBox12.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_hostname_light_path));
            configurableQualityPictureBox13.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_mac_light_path));
            configurableQualityPictureBox14.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ip_light_path));
            configurableQualityPictureBox15.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_bios_light_path));
            configurableQualityPictureBox16.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_bios_version_light_path));
            configurableQualityPictureBox17.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_secure_boot_light_path));
            configurableQualityPictureBox18.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_patr_light_path));
            configurableQualityPictureBox19.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_seal_light_path));
            configurableQualityPictureBox20.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_room_light_path));
            configurableQualityPictureBox21.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_building_light_path));
            configurableQualityPictureBox22.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_server_light_path));
            configurableQualityPictureBox23.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_standard_light_path));
            configurableQualityPictureBox24.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_service_light_path));
            configurableQualityPictureBox25.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_letter_light_path));
            configurableQualityPictureBox26.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_in_use_light_path));
            configurableQualityPictureBox27.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_sticker_light_path));
            configurableQualityPictureBox28.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_type_light_path));
            configurableQualityPictureBox30.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_VT_x_light_path));
            configurableQualityPictureBox31.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_who_light_path));
            configurableQualityPictureBox32.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_smart_light_path));
            configurableQualityPictureBox33.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_tpm_light_path));
            configurableQualityPictureBox34.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_cmos_battery_light_path));
            configurableQualityPictureBox35.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ticket_light_path));
        }

        //Sets a dark theme for the UI
        private void DarkTheme()
        {
            BackColor = StringsAndConstants.DARK_BACKGROUND;

            lblBM.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblModel.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblSerialNo.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblProcName.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblPM.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblHDSize.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblMediaType.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblMediaOperation.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblOS.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblHostname.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblMac.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblIP.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblBIOS.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblBIOSType.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblTPM.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblVT.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblSecBoot.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblGPUInfo.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblVT.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblSmart.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblIPServer.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblPortServer.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblAgentName.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;

            lblInstallSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            lblMaintenanceSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            label1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label3.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label4.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label5.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label6.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label7.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label8.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label9.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label10.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label11.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label12.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label13.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label14.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label15.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label16.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label17.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label18.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label19.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label20.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label21.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label22.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label23.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label24.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label25.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label27.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label28.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label29.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label30.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label31.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label32.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label33.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label34.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label35.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            label36.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            label37.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            label38.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            label39.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            label40.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            label41.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            label42.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            label43.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            label44.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label45.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label46.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label47.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            label48.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            label49.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            label53.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            if (offlineMode)
            {
                lblIPServer.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblPortServer.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblAgentName.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }
            loadingCircle22.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            loadingCircle23.BackColor = StringsAndConstants.DARK_BACKCOLOR;

            textBoxPatrimony.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxPatrimony.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxSeal.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxSeal.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxRoom.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxRoom.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxLetter.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxLetter.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBox5.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBox5.BackColor = StringsAndConstants.DARK_BACKGROUND;
            textBox6.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBox6.BackColor = StringsAndConstants.DARK_BACKGROUND;
            textBoxTicket.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxTicket.BackColor = StringsAndConstants.DARK_BACKCOLOR;

            comboBoxBuilding.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxBuilding.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxBuilding.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxBuilding.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxActiveDirectory.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxActiveDirectory.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxActiveDirectory.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxActiveDirectory.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxStandard.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxStandard.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxStandard.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxStandard.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxInUse.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxInUse.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxInUse.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxInUse.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxTag.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxTag.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxTag.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxTag.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxType.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxType.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxType.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxBattery.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxBattery.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxBattery.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxBattery.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxTheme.BackColor = StringsAndConstants.DARK_BACKGROUND;
            comboBoxTheme.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            employeeRadioButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            studentRadioButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            formatButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            maintenanceButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            registerButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            registerButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            registerButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            collectButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            collectButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            collectButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            collectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            accessSystemButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            accessSystemButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            accessSystemButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            accessSystemButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            groupBox1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            groupBox2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            groupBox3.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            groupBox4.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            groupBox1.Paint += CustomColors.GroupBox_PaintDarkTheme;
            groupBox2.Paint += CustomColors.GroupBox_PaintDarkTheme;
            groupBox3.Paint += CustomColors.GroupBox_PaintDarkTheme;
            groupBox4.Paint += CustomColors.GroupBox_PaintDarkTheme;
            groupBox5.Paint += CustomColors.GroupBox_PaintDarkTheme;
            separatorH.BackColor = StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR;
            separatorV.BackColor = StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR;

            toolStripStatusLabel1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripStatusLabel1.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripStatusLabel2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripStatusLabel2.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripMenuItem1.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripMenuItem1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripMenuItem2.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripMenuItem2.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripMenuItem3.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripMenuItem3.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            logLabel.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            logLabel.BackColor = StringsAndConstants.DARK_BACKGROUND;
            aboutLabel.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            aboutLabel.BackColor = StringsAndConstants.DARK_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.DARK_BACKGROUND;

            statusStrip1.Renderer = new ModifiedToolStripProfessionalDarkTheme();

            toolStripMenuItem1.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_autotheme_dark_path));
            toolStripMenuItem2.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_lighttheme_dark_path));
            toolStripMenuItem3.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_darktheme_dark_path));

            comboBoxTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_autotheme_dark_path));
            logLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_log_dark_path));
            aboutLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_about_dark_path));

            configurableQualityPictureBox1.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.main_banner_dark_path));
            configurableQualityPictureBox2.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_brand_dark_path));
            configurableQualityPictureBox3.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_model_dark_path));
            configurableQualityPictureBox4.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_serial_no_dark_path));
            configurableQualityPictureBox5.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_cpu_dark_path));
            configurableQualityPictureBox6.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ram_dark_path));
            configurableQualityPictureBox7.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_disk_size_dark_path));
            configurableQualityPictureBox8.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_hdd_dark_path));
            configurableQualityPictureBox9.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ahci_dark_path));
            configurableQualityPictureBox10.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_gpu_dark_path));
            configurableQualityPictureBox11.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_windows_dark_path));
            configurableQualityPictureBox12.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_hostname_dark_path));
            configurableQualityPictureBox13.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_mac_dark_path));
            configurableQualityPictureBox14.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ip_dark_path));
            configurableQualityPictureBox15.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_bios_dark_path));
            configurableQualityPictureBox16.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_bios_version_dark_path));
            configurableQualityPictureBox17.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_secure_boot_dark_path));
            configurableQualityPictureBox18.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_patr_dark_path));
            configurableQualityPictureBox19.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_seal_dark_path));
            configurableQualityPictureBox20.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_room_dark_path));
            configurableQualityPictureBox21.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_building_dark_path));
            configurableQualityPictureBox22.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_server_dark_path));
            configurableQualityPictureBox23.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_standard_dark_path));
            configurableQualityPictureBox24.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_service_dark_path));
            configurableQualityPictureBox25.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_letter_dark_path));
            configurableQualityPictureBox26.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_in_use_dark_path));
            configurableQualityPictureBox27.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_sticker_dark_path));
            configurableQualityPictureBox28.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_type_dark_path));
            configurableQualityPictureBox30.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_VT_x_dark_path));
            configurableQualityPictureBox31.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_who_dark_path));
            configurableQualityPictureBox32.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_smart_dark_path));
            configurableQualityPictureBox33.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_tpm_dark_path));
            configurableQualityPictureBox34.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_cmos_battery_dark_path));
            configurableQualityPictureBox35.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ticket_dark_path));
        }

        //Sets highlight about label when hovering with the mouse
        private void AboutLabel_MouseEnter(object sender, EventArgs e)
        {
            aboutLabel.ForeColor = StringsAndConstants.HIGHLIGHT_LABEL_COLOR;
        }

        //Resets highlight about label when hovering with the mouse
        private void AboutLabel_MouseLeave(object sender, EventArgs e)
        {
            aboutLabel.ForeColor = !themeBool ? StringsAndConstants.LIGHT_FORECOLOR : StringsAndConstants.DARK_FORECOLOR;
        }

        //Sets highlight log label when hovering with the mouse
        private void LogLabel_MouseEnter(object sender, EventArgs e)
        {
            logLabel.ForeColor = StringsAndConstants.HIGHLIGHT_LABEL_COLOR;
        }

        //Resets highlight log label when hovering with the mouse
        private void LogLabel_MouseLeave(object sender, EventArgs e)
        {
            logLabel.ForeColor = !themeBool ? StringsAndConstants.LIGHT_FORECOLOR : StringsAndConstants.DARK_FORECOLOR;
        }

        //Opens the log file
        private void LogLabel_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_OPENING_LOG, string.Empty, StringsAndConstants.consoleOutGUI);
#if DEBUG
            System.Diagnostics.Process.Start(defList[4][0] + StringsAndConstants.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.dev_status + StringsAndConstants.LOG_FILE_EXT);
#else
            System.Diagnostics.Process.Start(defList[4][0] + StringsAndConstants.LOG_FILENAME_CP + "-v" + Application.ProductVersion + StringsAndConstants.LOG_FILE_EXT);
#endif
        }

        //Opens the About box
        private void AboutLabel_Click(object sender, EventArgs e)
        {
            AboutBox aboutForm = new AboutBox(defList, themeBool);
            if (HardwareInfo.GetOSInfoAux().Equals(StringsAndConstants.windows10))
            {
                DarkNet.Instance.SetWindowThemeForms(aboutForm, Theme.Auto);
            }

            _ = aboutForm.ShowDialog();
        }

        //Opens the selected webpage, according to the IP and port specified in the comboboxes
        private void AccessButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_VIEW_SERVER, string.Empty, StringsAndConstants.consoleOutGUI);
            _ = System.Diagnostics.Process.Start("http://" + ip + ":" + port);
        }

        //Handles the closing of the current form
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CLOSING_MAINFORM, string.Empty, StringsAndConstants.consoleOutGUI);
            log.LogWrite(StringsAndConstants.LOG_MISC, StringsAndConstants.LOG_SEPARATOR_SMALL, string.Empty, StringsAndConstants.consoleOutGUI);

            //Deletes downloaded json files
            File.Delete(StringsAndConstants.biosPath);
            File.Delete(StringsAndConstants.loginPath);
            File.Delete(StringsAndConstants.pcPath);
            File.Delete(StringsAndConstants.configPath);

            //Kills Webview2 instance
            webView2.Dispose();
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        //Loads the form, sets some combobox values, create timers (1000 ms cadence), and triggers a hardware collection
        private async void Form1_Load(object sender, EventArgs e)
        {
            //Define loading circle parameters
            #region

            switch (MiscMethods.GetWindowsScaling())
            {
                case 100:
                    //Init loading circles parameters for 100% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle2.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle3.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle4.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle5.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle6.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle7.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle8.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle9.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle10.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle11.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle12.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle13.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle14.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle15.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle16.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle17.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle18.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle19.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle20.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle21.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle22.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle23.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle24.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle2.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle3.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle4.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle5.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle6.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle7.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle8.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle9.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle10.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle11.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle12.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle13.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle14.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle15.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle16.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle17.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle18.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle19.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle20.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle21.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle22.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle23.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle24.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle2.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle3.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle4.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle5.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle6.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle7.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle8.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle9.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle10.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle11.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle12.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle13.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle14.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle15.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle16.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle17.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle18.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle19.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle20.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle21.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle22.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle23.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle24.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle2.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle3.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle4.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle5.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle6.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle7.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle8.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle9.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle10.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle11.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle12.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle13.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle14.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle15.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle16.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle17.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle18.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle19.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle20.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle21.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle22.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle23.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    loadingCircle24.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle2.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle3.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle4.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle5.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle6.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle7.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle8.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle9.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle10.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle11.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle12.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle13.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle14.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle15.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle16.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle17.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle18.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle19.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle20.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle21.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle22.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle23.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle24.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle2.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle3.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle4.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle5.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle6.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle7.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle8.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle9.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle10.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle11.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle12.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle13.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle14.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle15.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle16.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle17.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle18.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle19.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle20.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle21.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle22.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle23.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle24.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle2.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle3.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle4.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle5.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle6.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle7.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle8.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle9.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle10.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle11.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle12.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle13.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle14.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle15.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle16.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle17.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle18.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle19.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle20.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle21.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle22.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle23.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle24.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle2.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle3.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle4.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle5.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle6.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle7.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle8.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle9.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle10.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle11.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle12.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle13.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle14.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle15.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle16.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle17.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle18.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle19.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle20.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle21.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle22.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle23.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    loadingCircle24.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle2.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle3.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle4.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle5.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle6.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle7.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle8.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle9.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle10.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle11.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle12.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle13.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle14.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle15.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle16.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle17.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle18.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle19.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle20.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle21.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle22.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle23.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle24.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle2.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle3.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle4.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle5.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle6.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle7.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle8.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle9.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle10.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle11.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle12.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle13.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle14.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle15.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle16.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle17.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle18.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle19.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle20.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle21.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle22.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle23.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle24.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle2.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle3.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle4.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle5.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle6.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle7.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle8.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle9.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle10.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle11.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle12.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle13.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle14.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle15.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle16.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle17.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle18.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle19.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle20.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle21.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle22.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle23.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle24.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle2.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle3.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle4.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle5.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle6.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle7.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle8.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle9.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle10.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle11.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle12.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle13.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle14.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle15.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle16.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle17.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle18.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle19.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle20.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle21.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle22.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle23.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    loadingCircle24.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle2.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle3.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle4.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle5.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle6.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle7.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle8.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle9.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle10.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle11.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle12.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle13.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle14.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle15.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle16.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle17.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle18.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle19.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle20.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle21.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle22.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle23.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle24.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle2.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle3.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle4.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle5.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle6.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle7.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle8.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle9.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle10.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle11.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle12.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle13.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle14.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle15.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle16.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle17.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle18.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle19.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle20.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle21.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle22.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle23.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle24.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle2.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle3.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle4.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle5.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle6.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle7.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle8.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle9.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle10.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle11.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle12.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle13.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle14.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle15.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle16.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle17.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle18.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle19.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle20.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle21.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle22.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle23.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle24.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle2.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle3.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle4.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle5.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle6.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle7.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle8.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle9.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle10.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle11.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle12.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle13.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle14.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle15.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle16.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle17.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle18.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle19.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle20.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle21.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle22.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle23.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    loadingCircle24.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle2.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle3.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle4.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle5.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle6.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle7.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle8.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle9.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle10.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle11.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle12.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle13.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle14.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle15.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle16.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle17.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle18.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle19.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle20.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle21.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle22.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle23.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle24.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle2.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle3.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle4.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle5.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle6.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle7.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle8.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle9.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle10.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle11.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle12.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle13.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle14.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle15.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle16.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle17.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle18.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle19.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle20.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle21.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle22.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle23.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle24.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle2.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle3.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle4.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle5.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle6.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle7.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle8.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle9.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle10.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle11.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle12.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle13.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle14.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle15.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle16.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle17.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle18.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle19.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle20.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle21.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle22.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle23.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle24.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle2.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle3.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle4.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle5.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle6.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle7.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle8.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle9.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle10.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle11.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle12.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle13.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle14.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle15.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle16.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle17.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle18.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle19.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle20.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle21.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle22.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle23.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    loadingCircle24.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle2.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle3.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle4.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle5.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle6.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle7.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle8.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle9.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle10.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle11.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle12.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle13.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle14.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle15.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle16.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle17.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle18.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle19.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle20.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle21.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle22.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle23.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle24.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle2.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle3.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle4.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle5.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle6.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle7.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle8.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle9.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle10.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle11.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle12.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle13.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle14.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle15.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle16.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle17.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle18.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle19.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle20.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle21.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle22.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle23.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle24.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle2.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle3.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle4.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle5.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle6.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle7.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle8.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle9.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle10.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle11.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle12.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle13.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle14.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle15.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle16.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle17.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle18.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle19.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle20.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle21.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle22.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle23.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle24.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle2.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle3.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle4.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle5.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle6.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle7.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle8.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle9.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle10.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle11.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle12.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle13.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle14.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle15.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle16.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle17.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle18.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle19.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle20.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle21.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle22.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle23.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    loadingCircle24.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle2.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle3.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle4.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle5.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle6.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle7.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle8.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle9.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle10.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle11.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle12.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle13.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle14.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle15.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle16.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle17.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle18.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle19.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle20.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle21.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle22.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle23.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle24.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle2.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle3.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle4.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle5.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle6.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle7.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle8.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle9.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle10.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle11.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle12.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle13.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle14.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle15.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle16.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle17.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle18.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle19.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle20.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle21.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle22.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle23.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle24.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle2.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle3.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle4.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle5.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle6.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle7.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle8.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle9.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle10.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle11.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle12.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle13.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle14.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle15.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle16.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle17.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle18.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle19.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle20.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle21.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle22.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle23.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle24.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle2.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle3.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle4.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle5.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle6.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle7.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle8.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle9.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle10.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle11.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle12.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle13.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle14.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle15.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle16.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle17.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle18.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle19.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle20.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle21.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle22.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle23.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    loadingCircle24.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle2.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle3.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle4.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle5.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle6.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle7.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle8.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle9.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle10.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle11.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle12.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle13.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle14.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle15.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle16.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle17.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle18.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle19.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle20.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle21.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle22.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle23.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle24.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle2.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle3.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle4.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle5.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle6.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle7.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle8.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle9.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle10.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle11.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle12.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle13.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle14.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle15.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle16.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle17.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle18.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle19.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle20.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle21.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle22.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle23.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle24.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle2.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle3.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle4.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle5.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle6.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle7.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle8.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle9.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle10.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle11.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle12.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle13.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle14.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle15.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle16.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle17.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle18.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle19.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle20.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle21.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle22.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle23.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle24.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle2.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle3.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle4.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle5.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle6.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle7.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle8.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle9.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle10.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle11.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle12.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle13.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle14.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle15.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle16.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle17.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle18.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle19.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle20.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle21.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle22.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle23.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    loadingCircle24.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle2.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle3.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle4.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle5.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle6.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle7.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle8.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle9.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle10.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle11.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle12.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle13.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle14.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle15.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle16.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle17.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle18.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle19.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle20.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle21.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle22.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle23.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle24.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle2.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle3.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle4.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle5.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle6.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle7.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle8.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle9.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle10.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle11.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle12.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle13.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle14.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle15.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle16.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle17.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle18.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle19.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle20.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle21.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle22.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle23.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle24.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle2.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle3.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle4.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle5.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle6.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle7.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle8.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle9.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle10.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle11.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle12.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle13.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle14.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle15.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle16.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle17.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle18.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle19.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle20.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle21.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle22.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle23.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle24.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle2.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle3.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle4.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle5.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle6.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle7.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle8.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle9.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle10.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle11.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle12.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle13.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle14.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle15.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle16.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle17.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle18.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle19.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle20.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle21.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle22.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle23.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    loadingCircle24.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    break;
            }

            //Sets loading circle color and rotation speed
            #region
            loadingCircle1.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle2.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle3.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle4.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle5.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle6.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle7.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle8.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle9.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle10.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle11.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle12.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle13.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle14.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle15.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle16.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle17.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle18.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle19.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle20.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle21.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle22.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle23.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle24.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle1.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle2.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle3.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle4.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle5.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle6.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle7.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle8.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle9.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle10.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle11.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle12.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle13.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle14.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle15.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle16.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle17.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle18.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle19.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle20.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle21.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle22.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle23.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircle24.Color = StringsAndConstants.rotatingCircleColor;
            #endregion

            #endregion

            //Sets current and maximum values for the progressbar
            progressBar1.Maximum = 19;
            progressBar1.Value = 0;

            //If stats in non-offline mode, instantiates WebView2 and show a Busy form until loading is complete
            if (!offlineMode)
            {
                bw = new BusyForm
                {
                    Visible = true
                };
                await LoadWebView2();
                bw.Visible = false;
            }
            //Sets timer settings for respective alerts
            #region
            timer1.Tick += new EventHandler(FlashTextHostname);
            timer2.Tick += new EventHandler(FlashTextMediaOp);
            timer3.Tick += new EventHandler(FlashTextSecBoot);
            timer4.Tick += new EventHandler(FlashTextBIOSVersion);
            timer5.Tick += new EventHandler(FlashTextNetConnectivity);
            timer6.Tick += new EventHandler(FlashTextBIOSType);
            timer7.Tick += new EventHandler(FlashTextVT);
            timer8.Tick += new EventHandler(FlashTextSmart);
            timer9.Tick += new EventHandler(FlashTextTPM);
            timer10.Tick += new EventHandler(FlashTextRAM);
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
            #endregion

            lblIPServer.Text = ip; //Prints IP address
            lblPortServer.Text = port; //Prints port number
            lblAgentName.Text = user.ToUpper(); //Prints agent name
            dateTimePicker1.MaxDate = DateTime.Today; //Define max date of datetimepicker to current day
            FormClosing += Form1_FormClosing; //Handles Form closing
            tbProgMain = TaskbarManager.Instance; //Handles taskbar progress bar
            Coleta_Click(sender, e); //Start collecting
        }

        //Restricts textbox4 only with chars
        private void TextBoxCharsOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        //Restricts textbox only with numbers
        private void TextBoxNumbersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            {
                e.Handled = true;
            }
        }

        //Sets the Hostname label to flash in red
        private void FlashTextHostname(object myObject, EventArgs myEventArgs)
        {
            lblHostname.ForeColor = lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the MediaOperations label to flash in red
        private void FlashTextMediaOp(object myobject, EventArgs myEventArgs)
        {
            lblMediaOperation.ForeColor = lblMediaOperation.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblMediaOperation.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the Secure Boot label to flash in red
        private void FlashTextSecBoot(object myobject, EventArgs myEventArgs)
        {
            lblSecBoot.ForeColor = lblSecBoot.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblSecBoot.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the VT label to flash in red
        private void FlashTextVT(object myobject, EventArgs myEventArgs)
        {
            lblVT.ForeColor = lblVT.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblVT.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the SMART label to flash in red
        private void FlashTextSmart(object myobject, EventArgs myEventArgs)
        {
            lblSmart.ForeColor = lblSmart.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblSmart.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the BIOS Version label to flash in red
        private void FlashTextBIOSVersion(object myobject, EventArgs myEventArgs)
        {
            lblBIOS.ForeColor = lblBIOS.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblBIOS.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the Mac and IP labels to flash in red
        private void FlashTextNetConnectivity(object myobject, EventArgs myEventArgs)
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
        private void FlashTextBIOSType(object myobject, EventArgs myEventArgs)
        {
            lblBIOSType.ForeColor = lblBIOSType.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblBIOSType.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the Physical Memory label to flash in red
        private void FlashTextRAM(object myobject, EventArgs myEventArgs)
        {
            lblPM.ForeColor = lblPM.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblPM.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the TPM label to flash in red
        private void FlashTextTPM(object myobject, EventArgs myEventArgs)
        {
            lblTPM.ForeColor = lblTPM.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblTPM.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Starts the collection process
        private async void Collecting()
        {
            //Writes a dash in the labels, while scanning the hardware
            #region
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
            label26.Text = StringsAndConstants.DASH;
            #endregion

            //Show loading circles while scanning the hardware
            #region
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
            #endregion

            if (!offlineMode)
            {
                loadingCircle24.Visible = true;
                loadingCircle24.Active = true;
                servidor_web = ip;
                porta = port;

                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_PINGGING_SERVER, string.Empty, StringsAndConstants.consoleOutGUI);

                //Feches model info from server
                serverOnline = await BIOSFileReader.CheckHostMT(servidor_web, porta);

                if (serverOnline && porta != "")
                {
                    loadingCircle24.Visible = false;
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_ONLINE_SERVER, string.Empty, StringsAndConstants.consoleOutGUI);
                    label26.Text = StringsAndConstants.ONLINE;
                    label26.ForeColor = StringsAndConstants.ONLINE_ALERT;
                }
                else
                {
                    loadingCircle24.Visible = false;
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_OFFLINE_SERVER, string.Empty, StringsAndConstants.consoleOutGUI);
                    label26.Text = StringsAndConstants.OFFLINE;
                    label26.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                }
            }
            else
            {
                loadingCircle24.Visible = false;
                loadingCircle24.Active = false;
                lblIPServer.Text = lblPortServer.Text = lblAgentName.Text = label26.Text = StringsAndConstants.OFFLINE_MODE_ACTIVATED;
                lblIPServer.ForeColor = lblPortServer.ForeColor = lblAgentName.ForeColor = label26.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }

            //Alerts stop blinking and resets red color
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

            //Resets the colors while scanning the hardware
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
        }

        //Auxiliary method for progress bar
        private int ProgressAuxFunction(int k)
        {
            return k * 100 / progressBar1.Maximum;
        }

        //Runs on a separate thread, calling methods from the HardwareInfo class, and setting the variables,
        // while reporting the progress to the progressbar
        private void CollectThread(BackgroundWorker worker)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_START_COLLECTING, string.Empty, StringsAndConstants.consoleOutGUI);

            i = 0;

            //Scans for PC maker
            BM = HardwareInfo.GetBoardMaker();
            if (BM == StringsAndConstants.ToBeFilledByOEM || BM == "")
            {
                BM = HardwareInfo.GetBoardMakerAlt();
            }

            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_BM, BM, StringsAndConstants.consoleOutGUI);

            //Scans for PC model
            Model = HardwareInfo.GetModel();
            if (Model == StringsAndConstants.ToBeFilledByOEM || Model == "")
            {
                Model = HardwareInfo.GetModelAlt();
            }

            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_MODEL, Model, StringsAndConstants.consoleOutGUI);

            //Scans for motherboard Serial number
            SerialNo = HardwareInfo.GetBoardProductId();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SERIALNO, SerialNo, StringsAndConstants.consoleOutGUI);

            //Scans for CPU information
            ProcName = HardwareInfo.GetProcessorCores();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_PROCNAME, ProcName, StringsAndConstants.consoleOutGUI);

            //Scans for RAM amount and total number of slots
            PM = HardwareInfo.GetPhysicalMemory() + " (" + HardwareInfo.GetNumFreeRamSlots(Convert.ToInt32(HardwareInfo.GetNumRamSlots())) +
                " slots de " + HardwareInfo.GetNumRamSlots() + " ocupados" + ")";
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_PM, PM, StringsAndConstants.consoleOutGUI);

            //Scans for Storage size
            HDSize = HardwareInfo.GetHDSize();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_HDSIZE, HDSize, StringsAndConstants.consoleOutGUI);

            //Scans for SMART status
            Smart = HardwareInfo.GetSMARTStatus();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SMART, Smart, StringsAndConstants.consoleOutGUI);

            //Scans for Storage type
            MediaType = HardwareInfo.GetStorageType();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_MEDIATYPE, MediaType, StringsAndConstants.consoleOutGUI);

            //Scans for Media Operation (IDE/AHCI/NVME)
            MediaOperation = HardwareInfo.GetStorageOperation();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_MEDIAOP, MediaOperation, StringsAndConstants.consoleOutGUI);

            //Scans for GPU information
            GPUInfo = HardwareInfo.GetGPUInfo();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_GPUINFO, GPUInfo, StringsAndConstants.consoleOutGUI);

            //Scans for OS infomation
            OS = HardwareInfo.GetOSInformation();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_OS, OS, StringsAndConstants.consoleOutGUI);

            //Scans for Hostname
            Hostname = HardwareInfo.GetComputerName();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_HOSTNAME, Hostname, StringsAndConstants.consoleOutGUI);

            //Scans for MAC Address
            Mac = HardwareInfo.GetMACAddress();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_MAC, Mac, StringsAndConstants.consoleOutGUI);

            //Scans for IP Address
            IP = HardwareInfo.GetIPAddress();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_IP, IP, StringsAndConstants.consoleOutGUI);

            //Scans for firmware type
            BIOSType = HardwareInfo.GetBIOSType();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_BIOSTYPE, BIOSType, StringsAndConstants.consoleOutGUI);

            //Scans for Secure Boot status
            SecBoot = HardwareInfo.GetSecureBoot();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SECBOOT, SecBoot, StringsAndConstants.consoleOutGUI);

            //Scans for BIOS version
            BIOS = HardwareInfo.GetComputerBIOS();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_BIOS, BIOS, StringsAndConstants.consoleOutGUI);

            //Scans for VT status
            VT = HardwareInfo.GetVirtualizationTechnology();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_VT, VT, StringsAndConstants.consoleOutGUI);

            //Scans for TPM status
            TPM = HardwareInfo.GetTPMStatus();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_TPM, TPM, StringsAndConstants.consoleOutGUI);

            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_END_COLLECTING, string.Empty, StringsAndConstants.consoleOutGUI);
        }

        //Prints the collected data into the form labels, warning the user when there are forbidden modes
        private async Task PrintHardwareData()
        {
            //Hides loading circles after scanning the hardware
            #region
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
            #endregion

            //Prints fetched data into labels
            #region
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
            lblInstallSince.Text = MiscMethods.SinceLabelUpdate(true);
            lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
            #endregion

            pass = true;

            log.LogWrite(StringsAndConstants.LOG_INFO, lblInstallSince.Text, string.Empty, StringsAndConstants.consoleOutGUI);
            log.LogWrite(StringsAndConstants.LOG_INFO, lblMaintenanceSince.Text, string.Empty, StringsAndConstants.consoleOutGUI);

            if (!offlineMode)
            {
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_FETCHING_BIOSFILE, string.Empty, StringsAndConstants.consoleOutGUI);
            }

            try
            {
                //Feches model info from server
                string[] biosJsonStr = await BIOSFileReader.FetchInfoMT(lblBM.Text, lblModel.Text, lblBIOSType.Text, lblTPM.Text, lblMediaOperation.Text, ip, port);

                //Scan if hostname is the default one
                if (lblHostname.Text.Equals(StringsAndConstants.DEFAULT_HOSTNAME))
                {
                    pass = false;
                    lblHostname.Text += StringsAndConstants.HOSTNAME_ALERT;
                    timer1.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_HOSTNAME_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                //If model Json file does exist and the Media Operation is incorrect
                if (biosJsonStr != null && biosJsonStr[3].Equals("false"))
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
                //If model Json file does not exist and server is unreachable
                if (biosJsonStr == null)
                {
                    if (!offlineMode)
                    {
                        pass = false;
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_OFFLINE_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                        _ = MessageBox.Show(StringsAndConstants.DATABASE_REACH_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                //If model Json file does exist and BIOS/UEFI version is incorrect
                if (biosJsonStr != null && !lblBIOS.Text.Contains(biosJsonStr[0]))
                {
                    if (!biosJsonStr[0].Equals("-1"))
                    {
                        pass = false;
                        lblBIOS.Text += StringsAndConstants.BIOS_VERSION_ALERT;
                        timer4.Enabled = true;
                        log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_BIOSVER_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                    }
                }
                //If model Json file does exist and firmware type is incorrect
                if (biosJsonStr != null && biosJsonStr[1].Equals("false"))
                {
                    pass = false;
                    lblBIOSType.Text += StringsAndConstants.FIRMWARE_TYPE_ALERT;
                    timer6.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_FIRMWARE_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                //If there is no MAC address assigned
                if (lblMac.Text == "")
                {
                    if (!offlineMode) //If it's not in offline mode
                    {
                        pass = false;
                        lblMac.Text = StringsAndConstants.NETWORK_ERROR; //Prints a network error
                        lblIP.Text = StringsAndConstants.NETWORK_ERROR; //Prints a network error
                        timer5.Enabled = true;
                        log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_NETWORK_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                    }
                    else //If it's in offline mode
                    {
                        lblMac.Text = StringsAndConstants.OFFLINE_MODE_ACTIVATED; //Specifies offline mode
                        lblIP.Text = StringsAndConstants.OFFLINE_MODE_ACTIVATED; //Specifies offline mode
                    }
                }
                //If Virtualization Technology is disabled
                if (lblVT.Text == StringsAndConstants.deactivated)
                {
                    pass = false;
                    lblVT.Text += StringsAndConstants.VT_ALERT;
                    timer7.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_VT_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                //If Smart status is no OK
                if (!lblSmart.Text.Contains(StringsAndConstants.ok))
                {
                    pass = false;
                    lblSmart.Text += StringsAndConstants.SMART_FAIL;
                    timer8.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_SMART_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                //If model Json file does exist and TPM is not enabled
                if (biosJsonStr != null && biosJsonStr[2].Equals("false"))
                {
                    pass = false;
                    lblTPM.Text += StringsAndConstants.TPM_ERROR;
                    timer9.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_TPM_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetPhysicalMemoryAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, shows an alert
                if (d < 4.0 && Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    lblPM.Text += StringsAndConstants.NOT_ENOUGH_MEMORY;
                    timer10.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_MEMORYFEW_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                //If RAM is more than 4GB and OS is x86, shows an alert
                if (d > 4.0 && !Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    lblPM.Text += StringsAndConstants.TOO_MUCH_MEMORY;
                    timer10.Enabled = true;
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_MEMORYMUCH_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                }
                if (pass && !offlineMode)
                {
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_HARDWARE_PASSED, string.Empty, StringsAndConstants.consoleOutGUI);
                }

                if (!pass)
                {
                    progressBar1.SetState(2);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                }
            }
            catch (Exception e)
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, e.Message, string.Empty, StringsAndConstants.consoleOutGUI);
            }
        }

        //Triggers when the form opens, and when the user clicks to collect
        private void Coleta_Click(object sender, EventArgs e)
        {
            progressBar1.SetState(1);
            tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
            webView2.Visible = false;
            Collecting();
            accessSystemButton.Enabled = false;
            registerButton.Enabled = false;
            collectButton.Enabled = false;
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_START_THREAD, string.Empty, StringsAndConstants.consoleOutGUI);
            StartAsync(sender, e);
        }

        //Starts the worker for threading
        private void StartAsync(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        //Runs the collectThread method in a separate thread
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            CollectThread(worker);
        }

        //Draws the collection progress on the screen
        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            percent = e.ProgressPercentage * progressBar1.Maximum / 100;
            tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
            progressBar1.Value = percent;
            label28.Text = e.ProgressPercentage.ToString() + "%";
        }

        //Runs when the collection ends, ending the thread
        private async void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Task p = PrintHardwareData();
            await p;

            if (!offlineMode)
            {
                accessSystemButton.Enabled = true; //Enables accessSystem button
                registerButton.Enabled = true; //Enables register button
            }
            loadingCircle22.Visible = false; //Hides loading circle
            collectButton.Enabled = true; //Enables collect button
            collectButton.Text = StringsAndConstants.FETCH_AGAIN; //Updates collect button text
        }

        //Attributes the data collected previously to the variables which will inside the URL for registration
        private void AttrHardwareData()
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
        public async Task LoadWebView2()
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_START_LOADING_WEBVIEW2, string.Empty, StringsAndConstants.consoleOutGUI);
            CoreWebView2Environment webView2Environment = Environment.Is64BitOperatingSystem
                ? await CoreWebView2Environment.CreateAsync(StringsAndConstants.WEBVIEW2_SYSTEM_PATH_X64 + MiscMethods.GetWebView2Version(), Environment.GetEnvironmentVariable("TEMP"))
                : await CoreWebView2Environment.CreateAsync(StringsAndConstants.WEBVIEW2_SYSTEM_PATH_X86 + MiscMethods.GetWebView2Version(), Environment.GetEnvironmentVariable("TEMP"));
            await webView2.EnsureCoreWebView2Async(webView2Environment);
            webView2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView2.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_END_LOADING_WEBVIEW2, string.Empty, StringsAndConstants.consoleOutGUI);
        }

        //Sends hardware info to the specified server
        public void ServerSendInfo(string[] serverArgs)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTERING, string.Empty, StringsAndConstants.consoleOutGUI);
            webView2.CoreWebView2.Navigate("http://" + serverArgs[0] + ":" + serverArgs[1] + "/" + serverArgs[2] + ".php?patrimonio=" + serverArgs[3] + "&lacre=" + serverArgs[4] + "&sala=" + serverArgs[5] + "&predio=" + serverArgs[6] + "&ad=" + serverArgs[7] + "&padrao=" + serverArgs[8] + "&formatacao=" + serverArgs[9] + "&formatacoesAnteriores=" + serverArgs[9] + "&marca=" + serverArgs[10] + "&modelo=" + serverArgs[11] + "&numeroSerial=" + serverArgs[12] + "&processador=" + serverArgs[13] + "&memoria=" + serverArgs[14] + "&hd=" + serverArgs[15] + "&sistemaOperacional=" + serverArgs[16] + "&nomeDoComputador=" + serverArgs[17] + "&bios=" + serverArgs[18] + "&mac=" + serverArgs[19] + "&ip=" + serverArgs[20] + "&emUso=" + serverArgs[21] + "&etiqueta=" + serverArgs[22] + "&tipo=" + serverArgs[23] + "&tipoFW=" + serverArgs[24] + "&tipoArmaz=" + serverArgs[25] + "&gpu=" + serverArgs[26] + "&modoArmaz=" + serverArgs[27] + "&secBoot=" + serverArgs[28] + "&vt=" + serverArgs[29] + "&tpm=" + serverArgs[30] + "&trocaPilha=" + serverArgs[31] + "&ticketNum=" + serverArgs[32] + "&agent=" + serverArgs[33]);
        }

        //Runs the registration for the website
        private async void Cadastra_ClickAsync(object sender, EventArgs e)
        {
            webView2.Visible = false;
            tbProgMain.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_INIT_REGISTRY, string.Empty, StringsAndConstants.consoleOutGUI);
            loadingCircle23.Visible = true;
            loadingCircle23.Active = true;
            registerButton.Text = StringsAndConstants.DASH;
            registerButton.Enabled = false;
            accessSystemButton.Enabled = false;
            collectButton.Enabled = false;
            AttrHardwareData();

            //If all the mandatory fields are filled and there are no pendencies
            if (!string.IsNullOrWhiteSpace(textBoxPatrimony.Text) && !string.IsNullOrWhiteSpace(textBoxRoom.Text) && !string.IsNullOrWhiteSpace(textBoxTicket.Text) && comboBoxType.SelectedItem != null && comboBoxBuilding.SelectedItem != null && comboBoxInUse.SelectedItem != null && comboBoxTag.SelectedItem != null && comboBoxBattery.SelectedItem != null && (employeeRadioButton.Checked || studentRadioButton.Checked) && (formatButton.Checked || maintenanceButton.Checked) && pass == true)
            {
                //Attribute variables to an array which will be sent to the server
                sArgs[0] = ip;
                sArgs[1] = port;
                sArgs[2] = modeURL;
                sArgs[3] = textBoxPatrimony.Text;
                sArgs[4] = textBoxSeal.Text;
                sArgs[5] = textBoxLetter.Text != "" ? textBoxRoom.Text + textBoxLetter.Text : textBoxRoom.Text;
                sArgs[6] = comboBoxBuilding.SelectedItem.ToString();
                sArgs[7] = comboBoxActiveDirectory.SelectedItem.ToString();
                sArgs[8] = comboBoxStandard.SelectedItem.ToString();
                sArgs[9] = dateTimePicker1.Value.ToString("yyyy-MM-dd").Substring(0, 10);
                sArgs[21] = comboBoxInUse.SelectedItem.ToString();
                sArgs[22] = comboBoxTag.SelectedItem.ToString();
                sArgs[23] = comboBoxType.SelectedItem.ToString();
                sArgs[31] = comboBoxBattery.SelectedItem.ToString().Equals("Sim") ? StringsAndConstants.replacedBattery : StringsAndConstants.sameBattery;
                sArgs[32] = textBoxTicket.Text;
                sArgs[33] = user;

                //Feches patrimony data from server
                string[] pcJsonStr = await PCFileReader.FetchInfoMT(sArgs[3], sArgs[0], sArgs[1]);

                //If patrinony is discarded
                if (pcJsonStr[0] != "false" && pcJsonStr[9] == "1")
                {
                    tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_PC_DROPPED, string.Empty, StringsAndConstants.consoleOutGUI);
                    _ = MessageBox.Show(StringsAndConstants.PC_DROPPED, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                }
                else //If not discarded
                {
                    if (serverOnline && porta != "") //If server is online and port is not null
                    {
                        try //Tries to get the laster register date from the patrimony to check if the chosen date is adequate
                        {
                            DateTime registerDate = DateTime.ParseExact(sArgs[9], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(pcJsonStr[10], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            if (registerDate >= lastRegisterDate) //If chosen date is greater or equal than the last format/maintenance date of the PC, let proceed
                            {
                                sArgs[9] = dateTimePicker1.Value.ToString().Substring(0, 10);
                                webView2.Visible = true;
                                ServerSendInfo(sArgs); //Send info to server
                                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTRY_FINISHED, string.Empty, StringsAndConstants.consoleOutGUI);

                                if (formatButton.Checked) //If the format radio button is checked
                                {
                                    MiscMethods.RegCreate(true, dateTimePicker1); //Create reg entries for format and maintenance
                                    lblInstallSince.Text = MiscMethods.SinceLabelUpdate(true);
                                    lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_INSTALLDATE, string.Empty, StringsAndConstants.consoleOutGUI);
                                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_MAINTENANCEDATE, string.Empty, StringsAndConstants.consoleOutGUI);
                                }
                                else if (maintenanceButton.Checked) //If the maintenance radio button is checked
                                {
                                    MiscMethods.RegCreate(false, dateTimePicker1); //Create reg entry just for maintenance
                                    lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_MAINTENANCEDATE, string.Empty, StringsAndConstants.consoleOutGUI);
                                }
                                await Task.Delay(StringsAndConstants.TIMER_INTERVAL * 3);
                                tbProgMain.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                            }
                            else //If chosen date is before the last format/maintenance date of the PC, shows an error
                            {
                                log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_INCORRECT_REGISTER_DATE, string.Empty, StringsAndConstants.consoleOutGUI);
                                _ = MessageBox.Show(StringsAndConstants.INCORRECT_REGISTER_DATE, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                                tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                            }
                        }
                        catch //If can't retrieve (patrimony non existent in the database), register normally
                        {
                            sArgs[9] = dateTimePicker1.Value.ToString().Substring(0, 10);
                            webView2.Visible = true;
                            ServerSendInfo(sArgs); //Send info to server
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTRY_FINISHED, string.Empty, StringsAndConstants.consoleOutGUI);

                            if (formatButton.Checked) //If the format radio button is checked
                            {
                                MiscMethods.RegCreate(true, dateTimePicker1); //Create reg entries for format and maintenance
                                lblInstallSince.Text = MiscMethods.SinceLabelUpdate(true);
                                lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_INSTALLDATE, string.Empty, StringsAndConstants.consoleOutGUI);

                                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_MAINTENANCEDATE, string.Empty, StringsAndConstants.consoleOutGUI);

                            }
                            else if (maintenanceButton.Checked) //If the maintenance radio button is checked
                            {
                                MiscMethods.RegCreate(false, dateTimePicker1); //Create reg entry just for maintenance
                                lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_MAINTENANCEDATE, string.Empty, StringsAndConstants.consoleOutGUI);

                            }
                            await Task.Delay(StringsAndConstants.TIMER_INTERVAL * 3);
                            tbProgMain.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                        }
                    }
                    else //If the server is out of reach
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_SERVER_UNREACHABLE, string.Empty, StringsAndConstants.consoleOutGUI);
                        _ = MessageBox.Show(StringsAndConstants.SERVER_NOT_FOUND_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                        tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                    }
                }
            }
            else if (!pass) //If there are pendencies in the PC config
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_PENDENCY_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                _ = MessageBox.Show(StringsAndConstants.PENDENCY_ERROR, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
            }
            else //If all fields are not filled
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_MANDATORY_FIELD_ERROR, string.Empty, StringsAndConstants.consoleOutGUI);
                _ = MessageBox.Show(StringsAndConstants.MANDATORY_FIELD, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
            }

            //When finished registering, resets control states
            loadingCircle23.Visible = false;
            loadingCircle23.Active = false;
            registerButton.Text = StringsAndConstants.REGISTER_AGAIN;
            registerButton.Enabled = true;
            accessSystemButton.Enabled = true;
            collectButton.Enabled = true;
        }
    }
}

