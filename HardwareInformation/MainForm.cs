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
            toolStripVersionText.Text = MiscMethods.Version(Resources.dev_status); //Debug/Beta version
#else
            toolStripVersionText.Text = MiscMethods.Version(); //Release/Final version
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

            toolStripStatusBarText.Text = oList[4] + oList[2] + oList[0].Substring(0, oList[0].Length - 2);
            Text = Application.ProductName + " / " + oList[5] + oList[3] + oList[1].Substring(0, oList[1].Length - 2);
        }

        //Form elements
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
            this.lblFixedBM = new System.Windows.Forms.Label();
            this.lblFixedModel = new System.Windows.Forms.Label();
            this.lblFixedSerialNo = new System.Windows.Forms.Label();
            this.lblFixedProcName = new System.Windows.Forms.Label();
            this.lblFixedPM = new System.Windows.Forms.Label();
            this.lblFixedHDSize = new System.Windows.Forms.Label();
            this.lblFixedOS = new System.Windows.Forms.Label();
            this.lblFixedHostname = new System.Windows.Forms.Label();
            this.lblFixedMac = new System.Windows.Forms.Label();
            this.lblFixedIP = new System.Windows.Forms.Label();
            this.lblFixedPatrimony = new System.Windows.Forms.Label();
            this.lblFixedSeal = new System.Windows.Forms.Label();
            this.lblFixedBuilding = new System.Windows.Forms.Label();
            this.textBoxPatrimony = new System.Windows.Forms.TextBox();
            this.textBoxSeal = new System.Windows.Forms.TextBox();
            this.textBoxRoom = new System.Windows.Forms.TextBox();
            this.textBoxLetter = new System.Windows.Forms.TextBox();
            this.lblFixedRoom = new System.Windows.Forms.Label();
            this.lblFixedDateTimePicker = new System.Windows.Forms.Label();
            this.registerButton = new System.Windows.Forms.Button();
            this.lblFixedInUse = new System.Windows.Forms.Label();
            this.lblFixedTag = new System.Windows.Forms.Label();
            this.lblFixedType = new System.Windows.Forms.Label();
            this.lblFixedServerOpState = new System.Windows.Forms.Label();
            this.lblFixedPortServer = new System.Windows.Forms.Label();
            this.collectButton = new System.Windows.Forms.Button();
            this.lblFixedLetter = new System.Windows.Forms.Label();
            this.lblFixedBIOS = new System.Windows.Forms.Label();
            this.lblBIOS = new System.Windows.Forms.Label();
            this.accessSystemButton = new System.Windows.Forms.Button();
            this.lblFixedBIOSType = new System.Windows.Forms.Label();
            this.lblBIOSType = new System.Windows.Forms.Label();
            this.groupBoxHWData = new System.Windows.Forms.GroupBox();
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
            this.tpmIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.smartIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblSmart = new System.Windows.Forms.Label();
            this.lblTPM = new System.Windows.Forms.Label();
            this.lblFixedSmart = new System.Windows.Forms.Label();
            this.vtIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblFixedTPM = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblProgressPercent = new System.Windows.Forms.Label();
            this.lblVT = new System.Windows.Forms.Label();
            this.lblFixedVT = new System.Windows.Forms.Label();
            this.bmIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.secBootIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.biosIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.biosTypeIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.ipIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.macIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.hostnameIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.osIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.gpuInfoIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.mediaOperationIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.mediaTypeIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.hdSizeIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.pmIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.procNameIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.serialNoIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.modelIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblSecBoot = new System.Windows.Forms.Label();
            this.lblFixedSecBoot = new System.Windows.Forms.Label();
            this.lblMediaOperation = new System.Windows.Forms.Label();
            this.lblFixedMediaOperation = new System.Windows.Forms.Label();
            this.lblGPUInfo = new System.Windows.Forms.Label();
            this.lblFixedGPUInfo = new System.Windows.Forms.Label();
            this.lblMediaType = new System.Windows.Forms.Label();
            this.lblFixedMediaType = new System.Windows.Forms.Label();
            this.groupBoxPatrData = new System.Windows.Forms.GroupBox();
            this.comboBoxBattery = new CustomFlatComboBox();
            this.comboBoxStandard = new CustomFlatComboBox();
            this.comboBoxActiveDirectory = new CustomFlatComboBox();
            this.comboBoxTag = new CustomFlatComboBox();
            this.comboBoxInUse = new CustomFlatComboBox();
            this.comboBoxType = new CustomFlatComboBox();
            this.comboBoxBuilding = new CustomFlatComboBox();
            this.lblFixedMandatory9 = new System.Windows.Forms.Label();
            this.lblFixedMandatory8 = new System.Windows.Forms.Label();
            this.ticketIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblFixedTicket = new System.Windows.Forms.Label();
            this.textBoxTicket = new System.Windows.Forms.TextBox();
            this.batteryIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblFixedMandatory7 = new System.Windows.Forms.Label();
            this.lblFixedMandatory6 = new System.Windows.Forms.Label();
            this.lblFixedBattery = new System.Windows.Forms.Label();
            this.lblFixedMandatory5 = new System.Windows.Forms.Label();
            this.lblFixedMandatory4 = new System.Windows.Forms.Label();
            this.lblFixedMandatory3 = new System.Windows.Forms.Label();
            this.lblFixedMandatory2 = new System.Windows.Forms.Label();
            this.lblFixedMandatory = new System.Windows.Forms.Label();
            this.lblFixedMandatoryMain = new System.Windows.Forms.Label();
            this.studentRadioButton = new System.Windows.Forms.RadioButton();
            this.employeeRadioButton = new System.Windows.Forms.RadioButton();
            this.whoIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblFixedWho = new System.Windows.Forms.Label();
            this.letterIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.typeIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.tagIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.inUseIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.datetimeIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.standardIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.activeDirectoryIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.buildingIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.roomIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.sealIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.patrimonyIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.groupBoxTypeOfService = new System.Windows.Forms.GroupBox();
            this.loadingCircle21 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle20 = new MRG.Controls.UI.LoadingCircle();
            this.lblMaintenanceSince = new System.Windows.Forms.Label();
            this.lblInstallSince = new System.Windows.Forms.Label();
            this.lblFixedMandatory10 = new System.Windows.Forms.Label();
            this.textBoxFixedFormatRadio = new System.Windows.Forms.TextBox();
            this.textBoxMaintenanceRadio = new System.Windows.Forms.TextBox();
            this.formatRadioButton = new System.Windows.Forms.RadioButton();
            this.maintenanceRadioButton = new System.Windows.Forms.RadioButton();
            this.lblFixedActiveDirectory = new System.Windows.Forms.Label();
            this.lblFixedStandard = new System.Windows.Forms.Label();
            this.lblAgentName = new System.Windows.Forms.Label();
            this.lblFixedAgentName = new System.Windows.Forms.Label();
            this.lblPortServer = new System.Windows.Forms.Label();
            this.lblIPServer = new System.Windows.Forms.Label();
            this.lblFixedIPServer = new System.Windows.Forms.Label();
            this.lblServerOpState = new System.Windows.Forms.Label();
            this.toolStripVersionText = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.comboBoxTheme = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripAutoTheme = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLightTheme = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDarkTheme = new System.Windows.Forms.ToolStripMenuItem();
            this.logLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.aboutLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusBarText = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.timer4 = new System.Windows.Forms.Timer(this.components);
            this.timer5 = new System.Windows.Forms.Timer(this.components);
            this.timer6 = new System.Windows.Forms.Timer(this.components);
            this.timer7 = new System.Windows.Forms.Timer(this.components);
            this.timer8 = new System.Windows.Forms.Timer(this.components);
            this.groupBoxRegistryStatus = new System.Windows.Forms.GroupBox();
            this.webView2Control = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.timer9 = new System.Windows.Forms.Timer(this.components);
            this.timer10 = new System.Windows.Forms.Timer(this.components);
            this.topBannerImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.loadingCircle22 = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircle23 = new MRG.Controls.UI.LoadingCircle();
            this.groupBoxServerStatus = new System.Windows.Forms.GroupBox();
            this.loadingCircle24 = new MRG.Controls.UI.LoadingCircle();
            this.groupBoxHWData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tpmIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.smartIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vtIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bmIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.secBootIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.biosIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.biosTypeIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ipIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.macIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hostnameIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.osIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gpuInfoIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mediaOperationIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mediaTypeIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hdSizeIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pmIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.procNameIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serialNoIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelIconImg)).BeginInit();
            this.groupBoxPatrData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ticketIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.batteryIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.whoIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.letterIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.typeIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tagIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inUseIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datetimeIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.standardIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.activeDirectoryIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildingIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.roomIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sealIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.patrimonyIconImg)).BeginInit();
            this.groupBoxTypeOfService.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBoxRegistryStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2Control)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topBannerImg)).BeginInit();
            this.groupBoxServerStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblBM
            // 
            resources.ApplyResources(this.lblBM, "lblBM");
            this.lblBM.ForeColor = System.Drawing.Color.Silver;
            this.lblBM.Name = "lblBM";
            // 
            // lblModel
            // 
            resources.ApplyResources(this.lblModel, "lblModel");
            this.lblModel.ForeColor = System.Drawing.Color.Silver;
            this.lblModel.Name = "lblModel";
            // 
            // lblSerialNo
            // 
            resources.ApplyResources(this.lblSerialNo, "lblSerialNo");
            this.lblSerialNo.ForeColor = System.Drawing.Color.Silver;
            this.lblSerialNo.Name = "lblSerialNo";
            // 
            // lblProcName
            // 
            resources.ApplyResources(this.lblProcName, "lblProcName");
            this.lblProcName.ForeColor = System.Drawing.Color.Silver;
            this.lblProcName.Name = "lblProcName";
            // 
            // lblPM
            // 
            resources.ApplyResources(this.lblPM, "lblPM");
            this.lblPM.ForeColor = System.Drawing.Color.Silver;
            this.lblPM.Name = "lblPM";
            // 
            // lblHDSize
            // 
            resources.ApplyResources(this.lblHDSize, "lblHDSize");
            this.lblHDSize.ForeColor = System.Drawing.Color.Silver;
            this.lblHDSize.Name = "lblHDSize";
            // 
            // lblOS
            // 
            resources.ApplyResources(this.lblOS, "lblOS");
            this.lblOS.ForeColor = System.Drawing.Color.Silver;
            this.lblOS.Name = "lblOS";
            // 
            // lblHostname
            // 
            resources.ApplyResources(this.lblHostname, "lblHostname");
            this.lblHostname.ForeColor = System.Drawing.Color.Silver;
            this.lblHostname.Name = "lblHostname";
            // 
            // lblMac
            // 
            resources.ApplyResources(this.lblMac, "lblMac");
            this.lblMac.ForeColor = System.Drawing.Color.Silver;
            this.lblMac.Name = "lblMac";
            // 
            // lblIP
            // 
            resources.ApplyResources(this.lblIP, "lblIP");
            this.lblIP.ForeColor = System.Drawing.Color.Silver;
            this.lblIP.Name = "lblIP";
            // 
            // lblFixedBM
            // 
            resources.ApplyResources(this.lblFixedBM, "lblFixedBM");
            this.lblFixedBM.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedBM.Name = "lblFixedBM";
            // 
            // lblFixedModel
            // 
            resources.ApplyResources(this.lblFixedModel, "lblFixedModel");
            this.lblFixedModel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedModel.Name = "lblFixedModel";
            // 
            // lblFixedSerialNo
            // 
            resources.ApplyResources(this.lblFixedSerialNo, "lblFixedSerialNo");
            this.lblFixedSerialNo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedSerialNo.Name = "lblFixedSerialNo";
            // 
            // lblFixedProcName
            // 
            resources.ApplyResources(this.lblFixedProcName, "lblFixedProcName");
            this.lblFixedProcName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedProcName.Name = "lblFixedProcName";
            // 
            // lblFixedPM
            // 
            resources.ApplyResources(this.lblFixedPM, "lblFixedPM");
            this.lblFixedPM.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedPM.Name = "lblFixedPM";
            // 
            // lblFixedHDSize
            // 
            resources.ApplyResources(this.lblFixedHDSize, "lblFixedHDSize");
            this.lblFixedHDSize.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedHDSize.Name = "lblFixedHDSize";
            // 
            // lblFixedOS
            // 
            resources.ApplyResources(this.lblFixedOS, "lblFixedOS");
            this.lblFixedOS.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedOS.Name = "lblFixedOS";
            // 
            // lblFixedHostname
            // 
            resources.ApplyResources(this.lblFixedHostname, "lblFixedHostname");
            this.lblFixedHostname.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedHostname.Name = "lblFixedHostname";
            // 
            // lblFixedMac
            // 
            resources.ApplyResources(this.lblFixedMac, "lblFixedMac");
            this.lblFixedMac.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedMac.Name = "lblFixedMac";
            // 
            // lblFixedIP
            // 
            resources.ApplyResources(this.lblFixedIP, "lblFixedIP");
            this.lblFixedIP.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedIP.Name = "lblFixedIP";
            // 
            // lblFixedPatrimony
            // 
            resources.ApplyResources(this.lblFixedPatrimony, "lblFixedPatrimony");
            this.lblFixedPatrimony.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedPatrimony.Name = "lblFixedPatrimony";
            // 
            // lblFixedSeal
            // 
            resources.ApplyResources(this.lblFixedSeal, "lblFixedSeal");
            this.lblFixedSeal.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedSeal.Name = "lblFixedSeal";
            // 
            // lblFixedBuilding
            // 
            resources.ApplyResources(this.lblFixedBuilding, "lblFixedBuilding");
            this.lblFixedBuilding.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedBuilding.Name = "lblFixedBuilding";
            // 
            // textBoxPatrimony
            // 
            this.textBoxPatrimony.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxPatrimony.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.textBoxPatrimony, "textBoxPatrimony");
            this.textBoxPatrimony.Name = "textBoxPatrimony";
            this.textBoxPatrimony.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxSeal
            // 
            this.textBoxSeal.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxSeal.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.textBoxSeal, "textBoxSeal");
            this.textBoxSeal.Name = "textBoxSeal";
            this.textBoxSeal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxRoom
            // 
            this.textBoxRoom.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxRoom.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.textBoxRoom, "textBoxRoom");
            this.textBoxRoom.Name = "textBoxRoom";
            this.textBoxRoom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxLetter
            // 
            this.textBoxLetter.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxLetter.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.textBoxLetter, "textBoxLetter");
            this.textBoxLetter.Name = "textBoxLetter";
            this.textBoxLetter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxCharsOnly_KeyPress);
            // 
            // lblFixedRoom
            // 
            resources.ApplyResources(this.lblFixedRoom, "lblFixedRoom");
            this.lblFixedRoom.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedRoom.Name = "lblFixedRoom";
            // 
            // lblFixedDateTimePicker
            // 
            resources.ApplyResources(this.lblFixedDateTimePicker, "lblFixedDateTimePicker");
            this.lblFixedDateTimePicker.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedDateTimePicker.Name = "lblFixedDateTimePicker";
            // 
            // registerButton
            // 
            this.registerButton.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.registerButton, "registerButton");
            this.registerButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.registerButton.Name = "registerButton";
            this.registerButton.UseVisualStyleBackColor = true;
            this.registerButton.Click += new System.EventHandler(this.Cadastra_ClickAsync);
            // 
            // lblFixedInUse
            // 
            resources.ApplyResources(this.lblFixedInUse, "lblFixedInUse");
            this.lblFixedInUse.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedInUse.Name = "lblFixedInUse";
            // 
            // lblFixedTag
            // 
            resources.ApplyResources(this.lblFixedTag, "lblFixedTag");
            this.lblFixedTag.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedTag.Name = "lblFixedTag";
            // 
            // lblFixedType
            // 
            resources.ApplyResources(this.lblFixedType, "lblFixedType");
            this.lblFixedType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedType.Name = "lblFixedType";
            // 
            // lblFixedServerOpState
            // 
            resources.ApplyResources(this.lblFixedServerOpState, "lblFixedServerOpState");
            this.lblFixedServerOpState.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedServerOpState.Name = "lblFixedServerOpState";
            // 
            // lblFixedPortServer
            // 
            resources.ApplyResources(this.lblFixedPortServer, "lblFixedPortServer");
            this.lblFixedPortServer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedPortServer.Name = "lblFixedPortServer";
            // 
            // collectButton
            // 
            this.collectButton.BackColor = System.Drawing.SystemColors.Control;
            this.collectButton.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.collectButton, "collectButton");
            this.collectButton.Name = "collectButton";
            this.collectButton.UseVisualStyleBackColor = true;
            this.collectButton.Click += new System.EventHandler(this.Coleta_Click);
            // 
            // lblFixedLetter
            // 
            resources.ApplyResources(this.lblFixedLetter, "lblFixedLetter");
            this.lblFixedLetter.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedLetter.Name = "lblFixedLetter";
            // 
            // lblFixedBIOS
            // 
            resources.ApplyResources(this.lblFixedBIOS, "lblFixedBIOS");
            this.lblFixedBIOS.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedBIOS.Name = "lblFixedBIOS";
            // 
            // lblBIOS
            // 
            resources.ApplyResources(this.lblBIOS, "lblBIOS");
            this.lblBIOS.ForeColor = System.Drawing.Color.Silver;
            this.lblBIOS.Name = "lblBIOS";
            // 
            // accessSystemButton
            // 
            this.accessSystemButton.BackColor = System.Drawing.SystemColors.Control;
            this.accessSystemButton.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.accessSystemButton, "accessSystemButton");
            this.accessSystemButton.Name = "accessSystemButton";
            this.accessSystemButton.UseVisualStyleBackColor = true;
            this.accessSystemButton.Click += new System.EventHandler(this.AccessButton_Click);
            // 
            // lblFixedBIOSType
            // 
            resources.ApplyResources(this.lblFixedBIOSType, "lblFixedBIOSType");
            this.lblFixedBIOSType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedBIOSType.Name = "lblFixedBIOSType";
            // 
            // lblBIOSType
            // 
            resources.ApplyResources(this.lblBIOSType, "lblBIOSType");
            this.lblBIOSType.ForeColor = System.Drawing.Color.Silver;
            this.lblBIOSType.Name = "lblBIOSType";
            // 
            // groupBoxHWData
            // 
            this.groupBoxHWData.Controls.Add(this.loadingCircle19);
            this.groupBoxHWData.Controls.Add(this.loadingCircle18);
            this.groupBoxHWData.Controls.Add(this.loadingCircle17);
            this.groupBoxHWData.Controls.Add(this.loadingCircle16);
            this.groupBoxHWData.Controls.Add(this.loadingCircle15);
            this.groupBoxHWData.Controls.Add(this.loadingCircle14);
            this.groupBoxHWData.Controls.Add(this.loadingCircle13);
            this.groupBoxHWData.Controls.Add(this.loadingCircle12);
            this.groupBoxHWData.Controls.Add(this.loadingCircle11);
            this.groupBoxHWData.Controls.Add(this.loadingCircle10);
            this.groupBoxHWData.Controls.Add(this.loadingCircle9);
            this.groupBoxHWData.Controls.Add(this.loadingCircle8);
            this.groupBoxHWData.Controls.Add(this.loadingCircle7);
            this.groupBoxHWData.Controls.Add(this.loadingCircle6);
            this.groupBoxHWData.Controls.Add(this.loadingCircle5);
            this.groupBoxHWData.Controls.Add(this.loadingCircle4);
            this.groupBoxHWData.Controls.Add(this.loadingCircle3);
            this.groupBoxHWData.Controls.Add(this.loadingCircle2);
            this.groupBoxHWData.Controls.Add(this.loadingCircle1);
            this.groupBoxHWData.Controls.Add(this.separatorH);
            this.groupBoxHWData.Controls.Add(this.separatorV);
            this.groupBoxHWData.Controls.Add(this.tpmIconImg);
            this.groupBoxHWData.Controls.Add(this.smartIconImg);
            this.groupBoxHWData.Controls.Add(this.lblSmart);
            this.groupBoxHWData.Controls.Add(this.lblTPM);
            this.groupBoxHWData.Controls.Add(this.lblFixedSmart);
            this.groupBoxHWData.Controls.Add(this.vtIconImg);
            this.groupBoxHWData.Controls.Add(this.lblFixedTPM);
            this.groupBoxHWData.Controls.Add(this.progressBar1);
            this.groupBoxHWData.Controls.Add(this.lblProgressPercent);
            this.groupBoxHWData.Controls.Add(this.lblVT);
            this.groupBoxHWData.Controls.Add(this.lblFixedVT);
            this.groupBoxHWData.Controls.Add(this.bmIconImg);
            this.groupBoxHWData.Controls.Add(this.secBootIconImg);
            this.groupBoxHWData.Controls.Add(this.biosIconImg);
            this.groupBoxHWData.Controls.Add(this.biosTypeIconImg);
            this.groupBoxHWData.Controls.Add(this.ipIconImg);
            this.groupBoxHWData.Controls.Add(this.macIconImg);
            this.groupBoxHWData.Controls.Add(this.hostnameIconImg);
            this.groupBoxHWData.Controls.Add(this.osIconImg);
            this.groupBoxHWData.Controls.Add(this.gpuInfoIconImg);
            this.groupBoxHWData.Controls.Add(this.mediaOperationIconImg);
            this.groupBoxHWData.Controls.Add(this.mediaTypeIconImg);
            this.groupBoxHWData.Controls.Add(this.hdSizeIconImg);
            this.groupBoxHWData.Controls.Add(this.pmIconImg);
            this.groupBoxHWData.Controls.Add(this.procNameIconImg);
            this.groupBoxHWData.Controls.Add(this.serialNoIconImg);
            this.groupBoxHWData.Controls.Add(this.modelIconImg);
            this.groupBoxHWData.Controls.Add(this.lblSecBoot);
            this.groupBoxHWData.Controls.Add(this.lblFixedSecBoot);
            this.groupBoxHWData.Controls.Add(this.lblMediaOperation);
            this.groupBoxHWData.Controls.Add(this.lblFixedMediaOperation);
            this.groupBoxHWData.Controls.Add(this.lblGPUInfo);
            this.groupBoxHWData.Controls.Add(this.lblFixedGPUInfo);
            this.groupBoxHWData.Controls.Add(this.lblMediaType);
            this.groupBoxHWData.Controls.Add(this.lblFixedMediaType);
            this.groupBoxHWData.Controls.Add(this.lblFixedBM);
            this.groupBoxHWData.Controls.Add(this.lblOS);
            this.groupBoxHWData.Controls.Add(this.lblBIOSType);
            this.groupBoxHWData.Controls.Add(this.lblHDSize);
            this.groupBoxHWData.Controls.Add(this.lblFixedBIOSType);
            this.groupBoxHWData.Controls.Add(this.lblPM);
            this.groupBoxHWData.Controls.Add(this.lblProcName);
            this.groupBoxHWData.Controls.Add(this.lblSerialNo);
            this.groupBoxHWData.Controls.Add(this.lblBIOS);
            this.groupBoxHWData.Controls.Add(this.lblModel);
            this.groupBoxHWData.Controls.Add(this.lblFixedBIOS);
            this.groupBoxHWData.Controls.Add(this.lblBM);
            this.groupBoxHWData.Controls.Add(this.lblHostname);
            this.groupBoxHWData.Controls.Add(this.lblMac);
            this.groupBoxHWData.Controls.Add(this.lblIP);
            this.groupBoxHWData.Controls.Add(this.lblFixedModel);
            this.groupBoxHWData.Controls.Add(this.lblFixedSerialNo);
            this.groupBoxHWData.Controls.Add(this.lblFixedProcName);
            this.groupBoxHWData.Controls.Add(this.lblFixedPM);
            this.groupBoxHWData.Controls.Add(this.lblFixedHDSize);
            this.groupBoxHWData.Controls.Add(this.lblFixedOS);
            this.groupBoxHWData.Controls.Add(this.lblFixedHostname);
            this.groupBoxHWData.Controls.Add(this.lblFixedMac);
            this.groupBoxHWData.Controls.Add(this.lblFixedIP);
            this.groupBoxHWData.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.groupBoxHWData, "groupBoxHWData");
            this.groupBoxHWData.Name = "groupBoxHWData";
            this.groupBoxHWData.TabStop = false;
            // 
            // loadingCircle19
            // 
            this.loadingCircle19.Active = false;
            this.loadingCircle19.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle19.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle19.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle19, "loadingCircle19");
            this.loadingCircle19.Name = "loadingCircle19";
            this.loadingCircle19.NumberSpoke = 12;
            this.loadingCircle19.OuterCircleRadius = 11;
            this.loadingCircle19.RotationSpeed = 1;
            this.loadingCircle19.SpokeThickness = 2;
            this.loadingCircle19.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle18
            // 
            this.loadingCircle18.Active = false;
            this.loadingCircle18.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle18.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle18.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle18, "loadingCircle18");
            this.loadingCircle18.Name = "loadingCircle18";
            this.loadingCircle18.NumberSpoke = 12;
            this.loadingCircle18.OuterCircleRadius = 11;
            this.loadingCircle18.RotationSpeed = 1;
            this.loadingCircle18.SpokeThickness = 2;
            this.loadingCircle18.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle17
            // 
            this.loadingCircle17.Active = false;
            this.loadingCircle17.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle17.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle17, "loadingCircle17");
            this.loadingCircle17.Name = "loadingCircle17";
            this.loadingCircle17.NumberSpoke = 12;
            this.loadingCircle17.OuterCircleRadius = 11;
            this.loadingCircle17.RotationSpeed = 1;
            this.loadingCircle17.SpokeThickness = 2;
            this.loadingCircle17.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle16
            // 
            this.loadingCircle16.Active = false;
            this.loadingCircle16.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle16.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle16.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle16, "loadingCircle16");
            this.loadingCircle16.Name = "loadingCircle16";
            this.loadingCircle16.NumberSpoke = 12;
            this.loadingCircle16.OuterCircleRadius = 11;
            this.loadingCircle16.RotationSpeed = 1;
            this.loadingCircle16.SpokeThickness = 2;
            this.loadingCircle16.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle15
            // 
            this.loadingCircle15.Active = false;
            this.loadingCircle15.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle15.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle15.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle15, "loadingCircle15");
            this.loadingCircle15.Name = "loadingCircle15";
            this.loadingCircle15.NumberSpoke = 12;
            this.loadingCircle15.OuterCircleRadius = 11;
            this.loadingCircle15.RotationSpeed = 1;
            this.loadingCircle15.SpokeThickness = 2;
            this.loadingCircle15.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle14
            // 
            this.loadingCircle14.Active = false;
            this.loadingCircle14.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle14.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle14, "loadingCircle14");
            this.loadingCircle14.Name = "loadingCircle14";
            this.loadingCircle14.NumberSpoke = 12;
            this.loadingCircle14.OuterCircleRadius = 11;
            this.loadingCircle14.RotationSpeed = 1;
            this.loadingCircle14.SpokeThickness = 2;
            this.loadingCircle14.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle13
            // 
            this.loadingCircle13.Active = false;
            this.loadingCircle13.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle13.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle13.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle13, "loadingCircle13");
            this.loadingCircle13.Name = "loadingCircle13";
            this.loadingCircle13.NumberSpoke = 12;
            this.loadingCircle13.OuterCircleRadius = 11;
            this.loadingCircle13.RotationSpeed = 1;
            this.loadingCircle13.SpokeThickness = 2;
            this.loadingCircle13.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle12
            // 
            this.loadingCircle12.Active = false;
            this.loadingCircle12.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle12.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle12, "loadingCircle12");
            this.loadingCircle12.Name = "loadingCircle12";
            this.loadingCircle12.NumberSpoke = 12;
            this.loadingCircle12.OuterCircleRadius = 11;
            this.loadingCircle12.RotationSpeed = 1;
            this.loadingCircle12.SpokeThickness = 2;
            this.loadingCircle12.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle11
            // 
            this.loadingCircle11.Active = false;
            this.loadingCircle11.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle11.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle11, "loadingCircle11");
            this.loadingCircle11.Name = "loadingCircle11";
            this.loadingCircle11.NumberSpoke = 12;
            this.loadingCircle11.OuterCircleRadius = 11;
            this.loadingCircle11.RotationSpeed = 1;
            this.loadingCircle11.SpokeThickness = 2;
            this.loadingCircle11.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle10
            // 
            this.loadingCircle10.Active = false;
            this.loadingCircle10.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle10.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle10, "loadingCircle10");
            this.loadingCircle10.Name = "loadingCircle10";
            this.loadingCircle10.NumberSpoke = 12;
            this.loadingCircle10.OuterCircleRadius = 11;
            this.loadingCircle10.RotationSpeed = 1;
            this.loadingCircle10.SpokeThickness = 2;
            this.loadingCircle10.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle9
            // 
            this.loadingCircle9.Active = false;
            this.loadingCircle9.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle9.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle9, "loadingCircle9");
            this.loadingCircle9.Name = "loadingCircle9";
            this.loadingCircle9.NumberSpoke = 12;
            this.loadingCircle9.OuterCircleRadius = 11;
            this.loadingCircle9.RotationSpeed = 1;
            this.loadingCircle9.SpokeThickness = 2;
            this.loadingCircle9.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle8
            // 
            this.loadingCircle8.Active = false;
            this.loadingCircle8.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle8.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle8, "loadingCircle8");
            this.loadingCircle8.Name = "loadingCircle8";
            this.loadingCircle8.NumberSpoke = 12;
            this.loadingCircle8.OuterCircleRadius = 11;
            this.loadingCircle8.RotationSpeed = 1;
            this.loadingCircle8.SpokeThickness = 2;
            this.loadingCircle8.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle7
            // 
            this.loadingCircle7.Active = false;
            this.loadingCircle7.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle7.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle7, "loadingCircle7");
            this.loadingCircle7.Name = "loadingCircle7";
            this.loadingCircle7.NumberSpoke = 12;
            this.loadingCircle7.OuterCircleRadius = 11;
            this.loadingCircle7.RotationSpeed = 1;
            this.loadingCircle7.SpokeThickness = 2;
            this.loadingCircle7.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle6
            // 
            this.loadingCircle6.Active = false;
            this.loadingCircle6.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle6.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle6, "loadingCircle6");
            this.loadingCircle6.Name = "loadingCircle6";
            this.loadingCircle6.NumberSpoke = 12;
            this.loadingCircle6.OuterCircleRadius = 11;
            this.loadingCircle6.RotationSpeed = 1;
            this.loadingCircle6.SpokeThickness = 2;
            this.loadingCircle6.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle5
            // 
            this.loadingCircle5.Active = false;
            this.loadingCircle5.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle5.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle5, "loadingCircle5");
            this.loadingCircle5.Name = "loadingCircle5";
            this.loadingCircle5.NumberSpoke = 12;
            this.loadingCircle5.OuterCircleRadius = 11;
            this.loadingCircle5.RotationSpeed = 1;
            this.loadingCircle5.SpokeThickness = 2;
            this.loadingCircle5.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle4
            // 
            this.loadingCircle4.Active = false;
            this.loadingCircle4.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle4.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle4, "loadingCircle4");
            this.loadingCircle4.Name = "loadingCircle4";
            this.loadingCircle4.NumberSpoke = 12;
            this.loadingCircle4.OuterCircleRadius = 11;
            this.loadingCircle4.RotationSpeed = 1;
            this.loadingCircle4.SpokeThickness = 2;
            this.loadingCircle4.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle3
            // 
            this.loadingCircle3.Active = false;
            this.loadingCircle3.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle3.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle3, "loadingCircle3");
            this.loadingCircle3.Name = "loadingCircle3";
            this.loadingCircle3.NumberSpoke = 12;
            this.loadingCircle3.OuterCircleRadius = 11;
            this.loadingCircle3.RotationSpeed = 1;
            this.loadingCircle3.SpokeThickness = 2;
            this.loadingCircle3.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle2
            // 
            this.loadingCircle2.Active = false;
            this.loadingCircle2.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle2.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle2, "loadingCircle2");
            this.loadingCircle2.Name = "loadingCircle2";
            this.loadingCircle2.NumberSpoke = 12;
            this.loadingCircle2.OuterCircleRadius = 11;
            this.loadingCircle2.RotationSpeed = 1;
            this.loadingCircle2.SpokeThickness = 2;
            this.loadingCircle2.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle1
            // 
            this.loadingCircle1.Active = false;
            this.loadingCircle1.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle1.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle1, "loadingCircle1");
            this.loadingCircle1.Name = "loadingCircle1";
            this.loadingCircle1.NumberSpoke = 12;
            this.loadingCircle1.OuterCircleRadius = 11;
            this.loadingCircle1.RotationSpeed = 1;
            this.loadingCircle1.SpokeThickness = 2;
            this.loadingCircle1.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // separatorH
            // 
            this.separatorH.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.separatorH, "separatorH");
            this.separatorH.Name = "separatorH";
            // 
            // separatorV
            // 
            this.separatorV.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.separatorV, "separatorV");
            this.separatorV.Name = "separatorV";
            // 
            // tpmIconImg
            // 
            this.tpmIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.tpmIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.tpmIconImg, "tpmIconImg");
            this.tpmIconImg.Name = "tpmIconImg";
            this.tpmIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.tpmIconImg.TabStop = false;
            // 
            // smartIconImg
            // 
            this.smartIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.smartIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.smartIconImg, "smartIconImg");
            this.smartIconImg.Name = "smartIconImg";
            this.smartIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.smartIconImg.TabStop = false;
            // 
            // lblSmart
            // 
            resources.ApplyResources(this.lblSmart, "lblSmart");
            this.lblSmart.ForeColor = System.Drawing.Color.Silver;
            this.lblSmart.Name = "lblSmart";
            // 
            // lblTPM
            // 
            resources.ApplyResources(this.lblTPM, "lblTPM");
            this.lblTPM.ForeColor = System.Drawing.Color.Silver;
            this.lblTPM.Name = "lblTPM";
            // 
            // lblFixedSmart
            // 
            resources.ApplyResources(this.lblFixedSmart, "lblFixedSmart");
            this.lblFixedSmart.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedSmart.Name = "lblFixedSmart";
            // 
            // vtIconImg
            // 
            this.vtIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.vtIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.vtIconImg, "vtIconImg");
            this.vtIconImg.Name = "vtIconImg";
            this.vtIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.vtIconImg.TabStop = false;
            // 
            // lblFixedTPM
            // 
            resources.ApplyResources(this.lblFixedTPM, "lblFixedTPM");
            this.lblFixedTPM.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedTPM.Name = "lblFixedTPM";
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // lblProgressPercent
            // 
            resources.ApplyResources(this.lblProgressPercent, "lblProgressPercent");
            this.lblProgressPercent.BackColor = System.Drawing.Color.Transparent;
            this.lblProgressPercent.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblProgressPercent.Name = "lblProgressPercent";
            // 
            // lblVT
            // 
            resources.ApplyResources(this.lblVT, "lblVT");
            this.lblVT.ForeColor = System.Drawing.Color.Silver;
            this.lblVT.Name = "lblVT";
            // 
            // lblFixedVT
            // 
            resources.ApplyResources(this.lblFixedVT, "lblFixedVT");
            this.lblFixedVT.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedVT.Name = "lblFixedVT";
            // 
            // bmIconImg
            // 
            this.bmIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.bmIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.bmIconImg, "bmIconImg");
            this.bmIconImg.Name = "bmIconImg";
            this.bmIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.bmIconImg.TabStop = false;
            // 
            // secBootIconImg
            // 
            this.secBootIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.secBootIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.secBootIconImg, "secBootIconImg");
            this.secBootIconImg.Name = "secBootIconImg";
            this.secBootIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.secBootIconImg.TabStop = false;
            // 
            // biosIconImg
            // 
            this.biosIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.biosIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.biosIconImg, "biosIconImg");
            this.biosIconImg.Name = "biosIconImg";
            this.biosIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.biosIconImg.TabStop = false;
            // 
            // biosTypeIconImg
            // 
            this.biosTypeIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.biosTypeIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.biosTypeIconImg, "biosTypeIconImg");
            this.biosTypeIconImg.Name = "biosTypeIconImg";
            this.biosTypeIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.biosTypeIconImg.TabStop = false;
            // 
            // ipIconImg
            // 
            this.ipIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.ipIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.ipIconImg, "ipIconImg");
            this.ipIconImg.Name = "ipIconImg";
            this.ipIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.ipIconImg.TabStop = false;
            // 
            // macIconImg
            // 
            this.macIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.macIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.macIconImg, "macIconImg");
            this.macIconImg.Name = "macIconImg";
            this.macIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.macIconImg.TabStop = false;
            // 
            // hostnameIconImg
            // 
            this.hostnameIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.hostnameIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.hostnameIconImg, "hostnameIconImg");
            this.hostnameIconImg.Name = "hostnameIconImg";
            this.hostnameIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.hostnameIconImg.TabStop = false;
            // 
            // osIconImg
            // 
            this.osIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.osIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.osIconImg, "osIconImg");
            this.osIconImg.Name = "osIconImg";
            this.osIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.osIconImg.TabStop = false;
            // 
            // gpuInfoIconImg
            // 
            this.gpuInfoIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.gpuInfoIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.gpuInfoIconImg, "gpuInfoIconImg");
            this.gpuInfoIconImg.Name = "gpuInfoIconImg";
            this.gpuInfoIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.gpuInfoIconImg.TabStop = false;
            // 
            // mediaOperationIconImg
            // 
            this.mediaOperationIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.mediaOperationIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.mediaOperationIconImg, "mediaOperationIconImg");
            this.mediaOperationIconImg.Name = "mediaOperationIconImg";
            this.mediaOperationIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.mediaOperationIconImg.TabStop = false;
            // 
            // mediaTypeIconImg
            // 
            this.mediaTypeIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.mediaTypeIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.mediaTypeIconImg, "mediaTypeIconImg");
            this.mediaTypeIconImg.Name = "mediaTypeIconImg";
            this.mediaTypeIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.mediaTypeIconImg.TabStop = false;
            // 
            // hdSizeIconImg
            // 
            this.hdSizeIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.hdSizeIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.hdSizeIconImg, "hdSizeIconImg");
            this.hdSizeIconImg.Name = "hdSizeIconImg";
            this.hdSizeIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.hdSizeIconImg.TabStop = false;
            // 
            // pmIconImg
            // 
            this.pmIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.pmIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.pmIconImg, "pmIconImg");
            this.pmIconImg.Name = "pmIconImg";
            this.pmIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.pmIconImg.TabStop = false;
            // 
            // procNameIconImg
            // 
            this.procNameIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.procNameIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.procNameIconImg, "procNameIconImg");
            this.procNameIconImg.Name = "procNameIconImg";
            this.procNameIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.procNameIconImg.TabStop = false;
            // 
            // serialNoIconImg
            // 
            this.serialNoIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.serialNoIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.serialNoIconImg, "serialNoIconImg");
            this.serialNoIconImg.Name = "serialNoIconImg";
            this.serialNoIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.serialNoIconImg.TabStop = false;
            // 
            // modelIconImg
            // 
            this.modelIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.modelIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.modelIconImg, "modelIconImg");
            this.modelIconImg.Name = "modelIconImg";
            this.modelIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.modelIconImg.TabStop = false;
            // 
            // lblSecBoot
            // 
            resources.ApplyResources(this.lblSecBoot, "lblSecBoot");
            this.lblSecBoot.ForeColor = System.Drawing.Color.Silver;
            this.lblSecBoot.Name = "lblSecBoot";
            // 
            // lblFixedSecBoot
            // 
            resources.ApplyResources(this.lblFixedSecBoot, "lblFixedSecBoot");
            this.lblFixedSecBoot.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedSecBoot.Name = "lblFixedSecBoot";
            // 
            // lblMediaOperation
            // 
            resources.ApplyResources(this.lblMediaOperation, "lblMediaOperation");
            this.lblMediaOperation.ForeColor = System.Drawing.Color.Silver;
            this.lblMediaOperation.Name = "lblMediaOperation";
            // 
            // lblFixedMediaOperation
            // 
            resources.ApplyResources(this.lblFixedMediaOperation, "lblFixedMediaOperation");
            this.lblFixedMediaOperation.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedMediaOperation.Name = "lblFixedMediaOperation";
            // 
            // lblGPUInfo
            // 
            resources.ApplyResources(this.lblGPUInfo, "lblGPUInfo");
            this.lblGPUInfo.ForeColor = System.Drawing.Color.Silver;
            this.lblGPUInfo.Name = "lblGPUInfo";
            // 
            // lblFixedGPUInfo
            // 
            resources.ApplyResources(this.lblFixedGPUInfo, "lblFixedGPUInfo");
            this.lblFixedGPUInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedGPUInfo.Name = "lblFixedGPUInfo";
            // 
            // lblMediaType
            // 
            resources.ApplyResources(this.lblMediaType, "lblMediaType");
            this.lblMediaType.ForeColor = System.Drawing.Color.Silver;
            this.lblMediaType.Name = "lblMediaType";
            // 
            // lblFixedMediaType
            // 
            resources.ApplyResources(this.lblFixedMediaType, "lblFixedMediaType");
            this.lblFixedMediaType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedMediaType.Name = "lblFixedMediaType";
            // 
            // groupBoxPatrData
            // 
            this.groupBoxPatrData.Controls.Add(this.comboBoxBattery);
            this.groupBoxPatrData.Controls.Add(this.comboBoxStandard);
            this.groupBoxPatrData.Controls.Add(this.comboBoxActiveDirectory);
            this.groupBoxPatrData.Controls.Add(this.comboBoxTag);
            this.groupBoxPatrData.Controls.Add(this.comboBoxInUse);
            this.groupBoxPatrData.Controls.Add(this.comboBoxType);
            this.groupBoxPatrData.Controls.Add(this.comboBoxBuilding);
            this.groupBoxPatrData.Controls.Add(this.lblFixedMandatory9);
            this.groupBoxPatrData.Controls.Add(this.lblFixedMandatory8);
            this.groupBoxPatrData.Controls.Add(this.ticketIconImg);
            this.groupBoxPatrData.Controls.Add(this.lblFixedTicket);
            this.groupBoxPatrData.Controls.Add(this.textBoxTicket);
            this.groupBoxPatrData.Controls.Add(this.batteryIconImg);
            this.groupBoxPatrData.Controls.Add(this.lblFixedMandatory7);
            this.groupBoxPatrData.Controls.Add(this.lblFixedMandatory6);
            this.groupBoxPatrData.Controls.Add(this.lblFixedBattery);
            this.groupBoxPatrData.Controls.Add(this.lblFixedMandatory5);
            this.groupBoxPatrData.Controls.Add(this.lblFixedMandatory4);
            this.groupBoxPatrData.Controls.Add(this.lblFixedMandatory3);
            this.groupBoxPatrData.Controls.Add(this.lblFixedMandatory2);
            this.groupBoxPatrData.Controls.Add(this.lblFixedMandatory);
            this.groupBoxPatrData.Controls.Add(this.lblFixedMandatoryMain);
            this.groupBoxPatrData.Controls.Add(this.studentRadioButton);
            this.groupBoxPatrData.Controls.Add(this.employeeRadioButton);
            this.groupBoxPatrData.Controls.Add(this.whoIconImg);
            this.groupBoxPatrData.Controls.Add(this.lblFixedWho);
            this.groupBoxPatrData.Controls.Add(this.letterIconImg);
            this.groupBoxPatrData.Controls.Add(this.typeIconImg);
            this.groupBoxPatrData.Controls.Add(this.tagIconImg);
            this.groupBoxPatrData.Controls.Add(this.inUseIconImg);
            this.groupBoxPatrData.Controls.Add(this.datetimeIconImg);
            this.groupBoxPatrData.Controls.Add(this.standardIconImg);
            this.groupBoxPatrData.Controls.Add(this.activeDirectoryIconImg);
            this.groupBoxPatrData.Controls.Add(this.buildingIconImg);
            this.groupBoxPatrData.Controls.Add(this.roomIconImg);
            this.groupBoxPatrData.Controls.Add(this.sealIconImg);
            this.groupBoxPatrData.Controls.Add(this.patrimonyIconImg);
            this.groupBoxPatrData.Controls.Add(this.dateTimePicker1);
            this.groupBoxPatrData.Controls.Add(this.groupBoxTypeOfService);
            this.groupBoxPatrData.Controls.Add(this.lblFixedPatrimony);
            this.groupBoxPatrData.Controls.Add(this.lblFixedSeal);
            this.groupBoxPatrData.Controls.Add(this.lblFixedBuilding);
            this.groupBoxPatrData.Controls.Add(this.textBoxPatrimony);
            this.groupBoxPatrData.Controls.Add(this.textBoxSeal);
            this.groupBoxPatrData.Controls.Add(this.lblFixedLetter);
            this.groupBoxPatrData.Controls.Add(this.textBoxRoom);
            this.groupBoxPatrData.Controls.Add(this.lblFixedRoom);
            this.groupBoxPatrData.Controls.Add(this.lblFixedActiveDirectory);
            this.groupBoxPatrData.Controls.Add(this.lblFixedDateTimePicker);
            this.groupBoxPatrData.Controls.Add(this.lblFixedType);
            this.groupBoxPatrData.Controls.Add(this.lblFixedStandard);
            this.groupBoxPatrData.Controls.Add(this.textBoxLetter);
            this.groupBoxPatrData.Controls.Add(this.lblFixedInUse);
            this.groupBoxPatrData.Controls.Add(this.lblFixedTag);
            this.groupBoxPatrData.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.groupBoxPatrData, "groupBoxPatrData");
            this.groupBoxPatrData.Name = "groupBoxPatrData";
            this.groupBoxPatrData.TabStop = false;
            // 
            // comboBoxBattery
            // 
            this.comboBoxBattery.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxBattery.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxBattery.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxBattery.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxBattery, "comboBoxBattery");
            this.comboBoxBattery.FormattingEnabled = true;
            this.comboBoxBattery.Name = "comboBoxBattery";
            // 
            // comboBoxStandard
            // 
            this.comboBoxStandard.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxStandard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxStandard.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxStandard, "comboBoxStandard");
            this.comboBoxStandard.FormattingEnabled = true;
            this.comboBoxStandard.Name = "comboBoxStandard";
            // 
            // comboBoxActiveDirectory
            // 
            this.comboBoxActiveDirectory.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxActiveDirectory.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxActiveDirectory.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxActiveDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxActiveDirectory, "comboBoxActiveDirectory");
            this.comboBoxActiveDirectory.FormattingEnabled = true;
            this.comboBoxActiveDirectory.Name = "comboBoxActiveDirectory";
            // 
            // comboBoxTag
            // 
            this.comboBoxTag.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxTag.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxTag.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxTag, "comboBoxTag");
            this.comboBoxTag.FormattingEnabled = true;
            this.comboBoxTag.Name = "comboBoxTag";
            // 
            // comboBoxInUse
            // 
            this.comboBoxInUse.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxInUse.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxInUse.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxInUse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxInUse, "comboBoxInUse");
            this.comboBoxInUse.FormattingEnabled = true;
            this.comboBoxInUse.Name = "comboBoxInUse";
            // 
            // comboBoxType
            // 
            this.comboBoxType.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxType.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxType, "comboBoxType");
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Name = "comboBoxType";
            // 
            // comboBoxBuilding
            // 
            this.comboBoxBuilding.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxBuilding.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxBuilding.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxBuilding, "comboBoxBuilding");
            this.comboBoxBuilding.FormattingEnabled = true;
            this.comboBoxBuilding.Name = "comboBoxBuilding";
            // 
            // lblFixedMandatory9
            // 
            resources.ApplyResources(this.lblFixedMandatory9, "lblFixedMandatory9");
            this.lblFixedMandatory9.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatory9.Name = "lblFixedMandatory9";
            // 
            // lblFixedMandatory8
            // 
            resources.ApplyResources(this.lblFixedMandatory8, "lblFixedMandatory8");
            this.lblFixedMandatory8.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatory8.Name = "lblFixedMandatory8";
            // 
            // ticketIconImg
            // 
            this.ticketIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.ticketIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.ticketIconImg, "ticketIconImg");
            this.ticketIconImg.Name = "ticketIconImg";
            this.ticketIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.ticketIconImg.TabStop = false;
            // 
            // lblFixedTicket
            // 
            resources.ApplyResources(this.lblFixedTicket, "lblFixedTicket");
            this.lblFixedTicket.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedTicket.Name = "lblFixedTicket";
            // 
            // textBoxTicket
            // 
            this.textBoxTicket.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxTicket.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.textBoxTicket, "textBoxTicket");
            this.textBoxTicket.Name = "textBoxTicket";
            this.textBoxTicket.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxNumbersOnly_KeyPress);
            // 
            // batteryIconImg
            // 
            this.batteryIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.batteryIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.batteryIconImg, "batteryIconImg");
            this.batteryIconImg.Name = "batteryIconImg";
            this.batteryIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.batteryIconImg.TabStop = false;
            // 
            // lblFixedMandatory7
            // 
            resources.ApplyResources(this.lblFixedMandatory7, "lblFixedMandatory7");
            this.lblFixedMandatory7.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatory7.Name = "lblFixedMandatory7";
            // 
            // lblFixedMandatory6
            // 
            resources.ApplyResources(this.lblFixedMandatory6, "lblFixedMandatory6");
            this.lblFixedMandatory6.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatory6.Name = "lblFixedMandatory6";
            // 
            // lblFixedBattery
            // 
            resources.ApplyResources(this.lblFixedBattery, "lblFixedBattery");
            this.lblFixedBattery.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedBattery.Name = "lblFixedBattery";
            // 
            // lblFixedMandatory5
            // 
            resources.ApplyResources(this.lblFixedMandatory5, "lblFixedMandatory5");
            this.lblFixedMandatory5.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatory5.Name = "lblFixedMandatory5";
            // 
            // lblFixedMandatory4
            // 
            resources.ApplyResources(this.lblFixedMandatory4, "lblFixedMandatory4");
            this.lblFixedMandatory4.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatory4.Name = "lblFixedMandatory4";
            // 
            // lblFixedMandatory3
            // 
            resources.ApplyResources(this.lblFixedMandatory3, "lblFixedMandatory3");
            this.lblFixedMandatory3.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatory3.Name = "lblFixedMandatory3";
            // 
            // lblFixedMandatory2
            // 
            resources.ApplyResources(this.lblFixedMandatory2, "lblFixedMandatory2");
            this.lblFixedMandatory2.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatory2.Name = "lblFixedMandatory2";
            // 
            // lblFixedMandatory
            // 
            resources.ApplyResources(this.lblFixedMandatory, "lblFixedMandatory");
            this.lblFixedMandatory.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatory.Name = "lblFixedMandatory";
            // 
            // lblFixedMandatoryMain
            // 
            resources.ApplyResources(this.lblFixedMandatoryMain, "lblFixedMandatoryMain");
            this.lblFixedMandatoryMain.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryMain.Name = "lblFixedMandatoryMain";
            // 
            // studentRadioButton
            // 
            resources.ApplyResources(this.studentRadioButton, "studentRadioButton");
            this.studentRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.studentRadioButton.Name = "studentRadioButton";
            this.studentRadioButton.TabStop = true;
            this.studentRadioButton.UseVisualStyleBackColor = true;
            this.studentRadioButton.CheckedChanged += new System.EventHandler(this.StudentButton2_CheckedChanged);
            // 
            // employeeRadioButton
            // 
            resources.ApplyResources(this.employeeRadioButton, "employeeRadioButton");
            this.employeeRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.employeeRadioButton.Name = "employeeRadioButton";
            this.employeeRadioButton.TabStop = true;
            this.employeeRadioButton.UseVisualStyleBackColor = true;
            this.employeeRadioButton.CheckedChanged += new System.EventHandler(this.EmployeeButton1_CheckedChanged);
            // 
            // whoIconImg
            // 
            this.whoIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.whoIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.whoIconImg, "whoIconImg");
            this.whoIconImg.Name = "whoIconImg";
            this.whoIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.whoIconImg.TabStop = false;
            // 
            // lblFixedWho
            // 
            resources.ApplyResources(this.lblFixedWho, "lblFixedWho");
            this.lblFixedWho.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedWho.Name = "lblFixedWho";
            // 
            // letterIconImg
            // 
            this.letterIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.letterIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.letterIconImg, "letterIconImg");
            this.letterIconImg.Name = "letterIconImg";
            this.letterIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.letterIconImg.TabStop = false;
            // 
            // typeIconImg
            // 
            this.typeIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.typeIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.typeIconImg, "typeIconImg");
            this.typeIconImg.Name = "typeIconImg";
            this.typeIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.typeIconImg.TabStop = false;
            // 
            // tagIconImg
            // 
            this.tagIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.tagIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.tagIconImg, "tagIconImg");
            this.tagIconImg.Name = "tagIconImg";
            this.tagIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.tagIconImg.TabStop = false;
            // 
            // inUseIconImg
            // 
            this.inUseIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.inUseIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.inUseIconImg, "inUseIconImg");
            this.inUseIconImg.Name = "inUseIconImg";
            this.inUseIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.inUseIconImg.TabStop = false;
            // 
            // datetimeIconImg
            // 
            this.datetimeIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.datetimeIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.datetimeIconImg, "datetimeIconImg");
            this.datetimeIconImg.Name = "datetimeIconImg";
            this.datetimeIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.datetimeIconImg.TabStop = false;
            // 
            // standardIconImg
            // 
            this.standardIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.standardIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.standardIconImg, "standardIconImg");
            this.standardIconImg.Name = "standardIconImg";
            this.standardIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.standardIconImg.TabStop = false;
            // 
            // activeDirectoryIconImg
            // 
            this.activeDirectoryIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.activeDirectoryIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.activeDirectoryIconImg, "activeDirectoryIconImg");
            this.activeDirectoryIconImg.Name = "activeDirectoryIconImg";
            this.activeDirectoryIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.activeDirectoryIconImg.TabStop = false;
            // 
            // buildingIconImg
            // 
            this.buildingIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.buildingIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.buildingIconImg, "buildingIconImg");
            this.buildingIconImg.Name = "buildingIconImg";
            this.buildingIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.buildingIconImg.TabStop = false;
            // 
            // roomIconImg
            // 
            this.roomIconImg.CompositingQuality = null;
            this.roomIconImg.InterpolationMode = null;
            resources.ApplyResources(this.roomIconImg, "roomIconImg");
            this.roomIconImg.Name = "roomIconImg";
            this.roomIconImg.SmoothingMode = null;
            this.roomIconImg.TabStop = false;
            // 
            // sealIconImg
            // 
            this.sealIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.sealIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.sealIconImg, "sealIconImg");
            this.sealIconImg.Name = "sealIconImg";
            this.sealIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.sealIconImg.TabStop = false;
            // 
            // patrimonyIconImg
            // 
            this.patrimonyIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.patrimonyIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.patrimonyIconImg, "patrimonyIconImg");
            this.patrimonyIconImg.Name = "patrimonyIconImg";
            this.patrimonyIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.patrimonyIconImg.TabStop = false;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.dateTimePicker1, "dateTimePicker1");
            this.dateTimePicker1.Name = "dateTimePicker1";
            // 
            // groupBoxTypeOfService
            // 
            this.groupBoxTypeOfService.Controls.Add(this.loadingCircle21);
            this.groupBoxTypeOfService.Controls.Add(this.loadingCircle20);
            this.groupBoxTypeOfService.Controls.Add(this.lblMaintenanceSince);
            this.groupBoxTypeOfService.Controls.Add(this.lblInstallSince);
            this.groupBoxTypeOfService.Controls.Add(this.lblFixedMandatory10);
            this.groupBoxTypeOfService.Controls.Add(this.textBoxFixedFormatRadio);
            this.groupBoxTypeOfService.Controls.Add(this.textBoxMaintenanceRadio);
            this.groupBoxTypeOfService.Controls.Add(this.formatRadioButton);
            this.groupBoxTypeOfService.Controls.Add(this.maintenanceRadioButton);
            this.groupBoxTypeOfService.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.groupBoxTypeOfService, "groupBoxTypeOfService");
            this.groupBoxTypeOfService.Name = "groupBoxTypeOfService";
            this.groupBoxTypeOfService.TabStop = false;
            // 
            // loadingCircle21
            // 
            this.loadingCircle21.Active = false;
            this.loadingCircle21.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle21.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle21, "loadingCircle21");
            this.loadingCircle21.Name = "loadingCircle21";
            this.loadingCircle21.NumberSpoke = 12;
            this.loadingCircle21.OuterCircleRadius = 11;
            this.loadingCircle21.RotationSpeed = 1;
            this.loadingCircle21.SpokeThickness = 2;
            this.loadingCircle21.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle20
            // 
            this.loadingCircle20.Active = false;
            this.loadingCircle20.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle20.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle20, "loadingCircle20");
            this.loadingCircle20.Name = "loadingCircle20";
            this.loadingCircle20.NumberSpoke = 12;
            this.loadingCircle20.OuterCircleRadius = 11;
            this.loadingCircle20.RotationSpeed = 1;
            this.loadingCircle20.SpokeThickness = 2;
            this.loadingCircle20.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // lblMaintenanceSince
            // 
            resources.ApplyResources(this.lblMaintenanceSince, "lblMaintenanceSince");
            this.lblMaintenanceSince.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblMaintenanceSince.Name = "lblMaintenanceSince";
            // 
            // lblInstallSince
            // 
            resources.ApplyResources(this.lblInstallSince, "lblInstallSince");
            this.lblInstallSince.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblInstallSince.Name = "lblInstallSince";
            // 
            // lblFixedMandatory10
            // 
            resources.ApplyResources(this.lblFixedMandatory10, "lblFixedMandatory10");
            this.lblFixedMandatory10.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatory10.Name = "lblFixedMandatory10";
            // 
            // textBoxFixedFormatRadio
            // 
            this.textBoxFixedFormatRadio.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxFixedFormatRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxFixedFormatRadio, "textBoxFixedFormatRadio");
            this.textBoxFixedFormatRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxFixedFormatRadio.Name = "textBoxFixedFormatRadio";
            this.textBoxFixedFormatRadio.ReadOnly = true;
            // 
            // textBoxMaintenanceRadio
            // 
            this.textBoxMaintenanceRadio.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxMaintenanceRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxMaintenanceRadio, "textBoxMaintenanceRadio");
            this.textBoxMaintenanceRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxMaintenanceRadio.Name = "textBoxMaintenanceRadio";
            this.textBoxMaintenanceRadio.ReadOnly = true;
            // 
            // formatRadioButton
            // 
            resources.ApplyResources(this.formatRadioButton, "formatRadioButton");
            this.formatRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.formatRadioButton.Name = "formatRadioButton";
            this.formatRadioButton.UseVisualStyleBackColor = true;
            this.formatRadioButton.CheckedChanged += new System.EventHandler(this.FormatButton1_CheckedChanged);
            // 
            // maintenanceRadioButton
            // 
            resources.ApplyResources(this.maintenanceRadioButton, "maintenanceRadioButton");
            this.maintenanceRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.maintenanceRadioButton.Name = "maintenanceRadioButton";
            this.maintenanceRadioButton.UseVisualStyleBackColor = true;
            this.maintenanceRadioButton.CheckedChanged += new System.EventHandler(this.MaintenanceButton2_CheckedChanged);
            // 
            // lblFixedActiveDirectory
            // 
            resources.ApplyResources(this.lblFixedActiveDirectory, "lblFixedActiveDirectory");
            this.lblFixedActiveDirectory.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedActiveDirectory.Name = "lblFixedActiveDirectory";
            // 
            // lblFixedStandard
            // 
            resources.ApplyResources(this.lblFixedStandard, "lblFixedStandard");
            this.lblFixedStandard.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedStandard.Name = "lblFixedStandard";
            // 
            // lblAgentName
            // 
            resources.ApplyResources(this.lblAgentName, "lblAgentName");
            this.lblAgentName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAgentName.Name = "lblAgentName";
            // 
            // lblFixedAgentName
            // 
            resources.ApplyResources(this.lblFixedAgentName, "lblFixedAgentName");
            this.lblFixedAgentName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedAgentName.Name = "lblFixedAgentName";
            // 
            // lblPortServer
            // 
            resources.ApplyResources(this.lblPortServer, "lblPortServer");
            this.lblPortServer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPortServer.Name = "lblPortServer";
            // 
            // lblIPServer
            // 
            resources.ApplyResources(this.lblIPServer, "lblIPServer");
            this.lblIPServer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblIPServer.Name = "lblIPServer";
            // 
            // lblFixedIPServer
            // 
            resources.ApplyResources(this.lblFixedIPServer, "lblFixedIPServer");
            this.lblFixedIPServer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedIPServer.Name = "lblFixedIPServer";
            // 
            // lblServerOpState
            // 
            resources.ApplyResources(this.lblServerOpState, "lblServerOpState");
            this.lblServerOpState.BackColor = System.Drawing.Color.Transparent;
            this.lblServerOpState.ForeColor = System.Drawing.Color.Silver;
            this.lblServerOpState.Name = "lblServerOpState";
            // 
            // toolStripVersionText
            // 
            this.toolStripVersionText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripVersionText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripVersionText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripVersionText.Name = "toolStripVersionText";
            resources.ApplyResources(this.toolStripVersionText, "toolStripVersionText");
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comboBoxTheme,
            this.logLabel,
            this.aboutLabel,
            this.toolStripStatusBarText,
            this.toolStripVersionText});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // comboBoxTheme
            // 
            this.comboBoxTheme.BackColor = System.Drawing.SystemColors.Control;
            this.comboBoxTheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.comboBoxTheme.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripAutoTheme,
            this.toolStripLightTheme,
            this.toolStripDarkTheme});
            this.comboBoxTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.comboBoxTheme, "comboBoxTheme");
            this.comboBoxTheme.Name = "comboBoxTheme";
            // 
            // toolStripAutoTheme
            // 
            this.toolStripAutoTheme.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripAutoTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripAutoTheme.Name = "toolStripAutoTheme";
            resources.ApplyResources(this.toolStripAutoTheme, "toolStripAutoTheme");
            this.toolStripAutoTheme.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // toolStripLightTheme
            // 
            this.toolStripLightTheme.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripLightTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripLightTheme.Name = "toolStripLightTheme";
            resources.ApplyResources(this.toolStripLightTheme, "toolStripLightTheme");
            this.toolStripLightTheme.Click += new System.EventHandler(this.ToolStripMenuItem2_Click);
            // 
            // toolStripDarkTheme
            // 
            this.toolStripDarkTheme.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripDarkTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripDarkTheme.Name = "toolStripDarkTheme";
            resources.ApplyResources(this.toolStripDarkTheme, "toolStripDarkTheme");
            this.toolStripDarkTheme.Click += new System.EventHandler(this.ToolStripMenuItem3_Click);
            // 
            // logLabel
            // 
            this.logLabel.BackColor = System.Drawing.SystemColors.Control;
            this.logLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.logLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.logLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.logLabel.Name = "logLabel";
            resources.ApplyResources(this.logLabel, "logLabel");
            this.logLabel.Click += new System.EventHandler(this.LogLabel_Click);
            this.logLabel.MouseEnter += new System.EventHandler(this.LogLabel_MouseEnter);
            this.logLabel.MouseLeave += new System.EventHandler(this.LogLabel_MouseLeave);
            // 
            // aboutLabel
            // 
            this.aboutLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.aboutLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.aboutLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.aboutLabel.Name = "aboutLabel";
            resources.ApplyResources(this.aboutLabel, "aboutLabel");
            this.aboutLabel.Click += new System.EventHandler(this.AboutLabel_Click);
            this.aboutLabel.MouseEnter += new System.EventHandler(this.AboutLabel_MouseEnter);
            this.aboutLabel.MouseLeave += new System.EventHandler(this.AboutLabel_MouseLeave);
            // 
            // toolStripStatusBarText
            // 
            this.toolStripStatusBarText.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripStatusBarText.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.toolStripStatusBarText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripStatusBarText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripStatusBarText.Name = "toolStripStatusBarText";
            resources.ApplyResources(this.toolStripStatusBarText, "toolStripStatusBarText");
            this.toolStripStatusBarText.Spring = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            // 
            // groupBoxRegistryStatus
            // 
            this.groupBoxRegistryStatus.Controls.Add(this.webView2Control);
            this.groupBoxRegistryStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.groupBoxRegistryStatus, "groupBoxRegistryStatus");
            this.groupBoxRegistryStatus.Name = "groupBoxRegistryStatus";
            this.groupBoxRegistryStatus.TabStop = false;
            // 
            // webView2Control
            // 
            this.webView2Control.AllowExternalDrop = true;
            this.webView2Control.CreationProperties = null;
            this.webView2Control.DefaultBackgroundColor = System.Drawing.Color.White;
            resources.ApplyResources(this.webView2Control, "webView2Control");
            this.webView2Control.Name = "webView2Control";
            this.webView2Control.ZoomFactor = 1D;
            // 
            // topBannerImg
            // 
            resources.ApplyResources(this.topBannerImg, "topBannerImg");
            this.topBannerImg.CompositingQuality = null;
            this.topBannerImg.InterpolationMode = null;
            this.topBannerImg.Name = "topBannerImg";
            this.topBannerImg.SmoothingMode = null;
            this.topBannerImg.TabStop = false;
            // 
            // loadingCircle22
            // 
            this.loadingCircle22.Active = false;
            this.loadingCircle22.BackColor = System.Drawing.SystemColors.Control;
            this.loadingCircle22.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle22.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle22.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle22, "loadingCircle22");
            this.loadingCircle22.Name = "loadingCircle22";
            this.loadingCircle22.NumberSpoke = 12;
            this.loadingCircle22.OuterCircleRadius = 11;
            this.loadingCircle22.RotationSpeed = 1;
            this.loadingCircle22.SpokeThickness = 2;
            this.loadingCircle22.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircle22.UseWaitCursor = true;
            // 
            // loadingCircle23
            // 
            this.loadingCircle23.Active = false;
            this.loadingCircle23.BackColor = System.Drawing.SystemColors.Control;
            this.loadingCircle23.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle23.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircle23.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle23, "loadingCircle23");
            this.loadingCircle23.Name = "loadingCircle23";
            this.loadingCircle23.NumberSpoke = 12;
            this.loadingCircle23.OuterCircleRadius = 11;
            this.loadingCircle23.RotationSpeed = 1;
            this.loadingCircle23.SpokeThickness = 2;
            this.loadingCircle23.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // groupBoxServerStatus
            // 
            this.groupBoxServerStatus.Controls.Add(this.loadingCircle24);
            this.groupBoxServerStatus.Controls.Add(this.lblFixedIPServer);
            this.groupBoxServerStatus.Controls.Add(this.lblFixedServerOpState);
            this.groupBoxServerStatus.Controls.Add(this.lblFixedPortServer);
            this.groupBoxServerStatus.Controls.Add(this.lblServerOpState);
            this.groupBoxServerStatus.Controls.Add(this.lblIPServer);
            this.groupBoxServerStatus.Controls.Add(this.lblPortServer);
            this.groupBoxServerStatus.Controls.Add(this.lblFixedAgentName);
            this.groupBoxServerStatus.Controls.Add(this.lblAgentName);
            this.groupBoxServerStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.groupBoxServerStatus, "groupBoxServerStatus");
            this.groupBoxServerStatus.Name = "groupBoxServerStatus";
            this.groupBoxServerStatus.TabStop = false;
            // 
            // loadingCircle24
            // 
            this.loadingCircle24.Active = false;
            this.loadingCircle24.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle24.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircle24, "loadingCircle24");
            this.loadingCircle24.Name = "loadingCircle24";
            this.loadingCircle24.NumberSpoke = 12;
            this.loadingCircle24.OuterCircleRadius = 11;
            this.loadingCircle24.RotationSpeed = 1;
            this.loadingCircle24.SpokeThickness = 2;
            this.loadingCircle24.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.groupBoxServerStatus);
            this.Controls.Add(this.loadingCircle23);
            this.Controls.Add(this.loadingCircle22);
            this.Controls.Add(this.groupBoxRegistryStatus);
            this.Controls.Add(this.groupBoxPatrData);
            this.Controls.Add(this.groupBoxHWData);
            this.Controls.Add(this.topBannerImg);
            this.Controls.Add(this.accessSystemButton);
            this.Controls.Add(this.collectButton);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.registerButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBoxHWData.ResumeLayout(false);
            this.groupBoxHWData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tpmIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.smartIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vtIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bmIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.secBootIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.biosIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.biosTypeIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ipIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.macIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hostnameIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.osIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gpuInfoIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mediaOperationIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mediaTypeIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hdSizeIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pmIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.procNameIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serialNoIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelIconImg)).EndInit();
            this.groupBoxPatrData.ResumeLayout(false);
            this.groupBoxPatrData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ticketIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.batteryIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.whoIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.letterIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.typeIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tagIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inUseIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datetimeIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.standardIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.activeDirectoryIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildingIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.roomIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sealIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.patrimonyIconImg)).EndInit();
            this.groupBoxTypeOfService.ResumeLayout(false);
            this.groupBoxTypeOfService.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBoxRegistryStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView2Control)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.topBannerImg)).EndInit();
            this.groupBoxServerStatus.ResumeLayout(false);
            this.groupBoxServerStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private Label lblFixedBM;
        private Label lblFixedModel;
        private Label lblFixedSerialNo;
        private Label lblFixedProcName;
        private Label lblFixedPM;
        private Label lblFixedHDSize;
        private Label lblFixedOS;
        private Label lblFixedHostname;
        private Label lblFixedMac;
        private Label lblFixedIP;
        private Label lblFixedPatrimony;
        private Label lblFixedSeal;
        private Label lblFixedBuilding;
        private TextBox textBoxPatrimony;
        private TextBox textBoxSeal;
        private TextBox textBoxRoom;
        private TextBox textBoxLetter;
        private Label lblFixedRoom;
        private Label lblFixedDateTimePicker;
        private Label lblOS;
        private Label lblFixedInUse;
        private Label lblFixedTag;
        private Button registerButton;
        private Label lblFixedType;
        private Label lblFixedServerOpState;
        private Label lblFixedBIOSType;
        private Label lblBIOSType;
        private GroupBox groupBoxHWData;
        private GroupBox groupBoxPatrData;
        private Label lblMediaType;
        private Label lblFixedMediaType;
        private Label lblGPUInfo;
        private Label lblFixedGPUInfo;
        private Timer timer1, timer2, timer3, timer4, timer5, timer6;
        private IContainer components;
        private Label lblMediaOperation;
        private Label lblFixedMediaOperation;
        private ToolStripStatusLabel toolStripVersionText;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusBarText;
        private Button collectButton;
        private Label lblFixedLetter;
        private Label lblFixedBIOS;
        private Label lblBIOS;
        private Button accessSystemButton;
        private ProgressBar progressBar1;
        private Label lblProgressPercent;
        private Label lblSecBoot;
        private Label lblFixedSecBoot;
        private WebView2 webView2Control;
        private RadioButton maintenanceRadioButton;
        private RadioButton formatRadioButton;
        private GroupBox groupBoxTypeOfService;
        private TextBox textBoxFixedFormatRadio;
        private TextBox textBoxMaintenanceRadio;
        private ToolStripDropDownButton comboBoxTheme;
        private ToolStripMenuItem toolStripAutoTheme;
        private ToolStripMenuItem toolStripLightTheme;
        private ToolStripMenuItem toolStripDarkTheme;
        private Label lblServerOpState;
        private DateTimePicker dateTimePicker1;
        private ConfigurableQualityPictureBox topBannerImg;
        private ConfigurableQualityPictureBox bmIconImg;
        private ConfigurableQualityPictureBox modelIconImg;
        private ConfigurableQualityPictureBox serialNoIconImg;
        private ConfigurableQualityPictureBox procNameIconImg;
        private ConfigurableQualityPictureBox pmIconImg;
        private ConfigurableQualityPictureBox hdSizeIconImg;
        private ConfigurableQualityPictureBox mediaTypeIconImg;
        private ConfigurableQualityPictureBox mediaOperationIconImg;
        private ConfigurableQualityPictureBox gpuInfoIconImg;
        private ConfigurableQualityPictureBox osIconImg;
        private ConfigurableQualityPictureBox hostnameIconImg;
        private ConfigurableQualityPictureBox macIconImg;
        private ConfigurableQualityPictureBox ipIconImg;
        private ConfigurableQualityPictureBox biosTypeIconImg;
        private ConfigurableQualityPictureBox biosIconImg;
        private ConfigurableQualityPictureBox secBootIconImg;
        private ConfigurableQualityPictureBox patrimonyIconImg;
        private ConfigurableQualityPictureBox sealIconImg;
        private ConfigurableQualityPictureBox roomIconImg;
        private ConfigurableQualityPictureBox buildingIconImg;
        private ConfigurableQualityPictureBox datetimeIconImg;
        private ConfigurableQualityPictureBox letterIconImg;
        private ConfigurableQualityPictureBox inUseIconImg;
        private ConfigurableQualityPictureBox tagIconImg;
        private ConfigurableQualityPictureBox typeIconImg;
        private ConfigurableQualityPictureBox vtIconImg;
        private Label lblVT;
        private Label lblFixedVT;
        private ConfigurableQualityPictureBox whoIconImg;
        private Label lblFixedWho;
        private RadioButton studentRadioButton;
        private RadioButton employeeRadioButton;
        private Label lblFixedMandatory7;
        private Label lblFixedMandatory6;
        private Label lblFixedMandatory5;
        private Label lblFixedMandatory4;
        private Label lblFixedMandatory3;
        private Label lblFixedMandatory2;
        private Label lblFixedMandatory;
        private Label lblFixedMandatoryMain;
        private Label lblFixedMandatory10;
        private Timer timer7;
        private ConfigurableQualityPictureBox smartIconImg;
        private Label lblSmart;
        private Label lblFixedSmart;
        private Timer timer8;
        private Label lblFixedPortServer;
        private ConfigurableQualityPictureBox tpmIconImg;
        private Label lblTPM;
        private Label lblFixedTPM;
        private GroupBox groupBoxRegistryStatus;
        private ConfigurableQualityPictureBox batteryIconImg;
        private Label lblFixedBattery;
        private ConfigurableQualityPictureBox ticketIconImg;
        private Label lblFixedTicket;
        private TextBox textBoxTicket;
        private Label lblFixedMandatory9;
        private Label lblFixedMandatory8;
        private Label lblFixedIPServer;
        private Label lblMaintenanceSince;
        private Label lblInstallSince;
        private BusyForm bw;
        private Label lblPortServer;
        private Label lblIPServer;
        private Label lblAgentName;
        private Label lblFixedAgentName;
        private Timer timer9;
        private Timer timer10;
        private ConfigurableQualityPictureBox standardIconImg;
        private ConfigurableQualityPictureBox activeDirectoryIconImg;
        private Label lblFixedActiveDirectory;
        private Label lblFixedStandard;
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
        private GroupBox groupBoxServerStatus;
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
            lblFixedBM.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedModel.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedSerialNo.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedProcName.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedPM.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedHDSize.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedOS.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedHostname.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedMac.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedIP.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedPatrimony.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedSeal.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedBuilding.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedRoom.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedActiveDirectory.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedDateTimePicker.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedStandard.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedInUse.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedTag.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedServerOpState.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedPortServer.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedLetter.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedBIOS.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedBIOSType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedMediaType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedGPUInfo.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblProgressPercent.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedMediaOperation.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedTicket.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedSecBoot.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedVT.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedWho.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedMandatoryMain.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatory.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatory2.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatory3.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatory4.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatory5.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatory6.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatory7.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatory10.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedSmart.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedTPM.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedBattery.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedMandatory8.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatory9.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedIPServer.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedAgentName.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
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
            textBoxFixedFormatRadio.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxFixedFormatRadio.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            textBoxMaintenanceRadio.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxMaintenanceRadio.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
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
            formatRadioButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            maintenanceRadioButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

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

            groupBoxHWData.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            groupBoxPatrData.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            groupBoxTypeOfService.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            groupBoxRegistryStatus.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            groupBoxHWData.Paint += CustomColors.GroupBox_PaintLightTheme;
            groupBoxPatrData.Paint += CustomColors.GroupBox_PaintLightTheme;
            groupBoxTypeOfService.Paint += CustomColors.GroupBox_PaintLightTheme;
            groupBoxRegistryStatus.Paint += CustomColors.GroupBox_PaintLightTheme;
            groupBoxServerStatus.Paint += CustomColors.GroupBox_PaintLightTheme;
            separatorH.BackColor = StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR;
            separatorV.BackColor = StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR;

            toolStripStatusBarText.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripStatusBarText.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripVersionText.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripVersionText.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripAutoTheme.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripAutoTheme.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripLightTheme.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripLightTheme.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripDarkTheme.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripDarkTheme.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            logLabel.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            logLabel.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            aboutLabel.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            aboutLabel.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            statusStrip1.Renderer = new ModifiedToolStripProfessionalLightTheme();

            toolStripAutoTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_autotheme_light_path));
            toolStripLightTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_lighttheme_light_path));
            toolStripDarkTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_darktheme_light_path));

            comboBoxTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_autotheme_light_path));
            logLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_log_light_path));
            aboutLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_about_light_path));

            topBannerImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.main_banner_light_path));
            bmIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_brand_light_path));
            modelIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_model_light_path));
            serialNoIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_serial_no_light_path));
            procNameIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_cpu_light_path));
            pmIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ram_light_path));
            hdSizeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_disk_size_light_path));
            mediaTypeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_hdd_light_path));
            mediaOperationIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ahci_light_path));
            gpuInfoIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_gpu_light_path));
            osIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_windows_light_path));
            hostnameIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_hostname_light_path));
            macIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_mac_light_path));
            ipIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ip_light_path));
            biosTypeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_bios_light_path));
            biosIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_bios_version_light_path));
            secBootIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_secure_boot_light_path));
            patrimonyIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_patr_light_path));
            sealIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_seal_light_path));
            roomIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_room_light_path));
            buildingIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_building_light_path));
            activeDirectoryIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_server_light_path));
            standardIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_standard_light_path));
            datetimeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_service_light_path));
            letterIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_letter_light_path));
            inUseIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_in_use_light_path));
            tagIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_sticker_light_path));
            typeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_type_light_path));
            vtIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_VT_x_light_path));
            whoIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_who_light_path));
            smartIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_smart_light_path));
            tpmIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_tpm_light_path));
            batteryIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_cmos_battery_light_path));
            ticketIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ticket_light_path));
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
            lblFixedBM.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedModel.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedSerialNo.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedProcName.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedPM.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedHDSize.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedOS.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedHostname.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedMac.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedIP.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedPatrimony.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedSeal.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedBuilding.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedRoom.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedActiveDirectory.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedDateTimePicker.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedStandard.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedInUse.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedTag.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedServerOpState.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedPortServer.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedLetter.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedBIOS.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedBIOSType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedMediaType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblProgressPercent.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedGPUInfo.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedMediaOperation.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedTicket.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedSecBoot.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedVT.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedWho.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedMandatoryMain.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatory.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatory2.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatory3.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatory4.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatory5.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatory6.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatory7.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatory10.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedSmart.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedTPM.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedBattery.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedMandatory8.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatory9.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedIPServer.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedAgentName.ForeColor = StringsAndConstants.DARK_FORECOLOR;
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
            textBoxFixedFormatRadio.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxFixedFormatRadio.BackColor = StringsAndConstants.DARK_BACKGROUND;
            textBoxMaintenanceRadio.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxMaintenanceRadio.BackColor = StringsAndConstants.DARK_BACKGROUND;
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
            formatRadioButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            maintenanceRadioButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;

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

            groupBoxHWData.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            groupBoxPatrData.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            groupBoxTypeOfService.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            groupBoxRegistryStatus.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            groupBoxHWData.Paint += CustomColors.GroupBox_PaintDarkTheme;
            groupBoxPatrData.Paint += CustomColors.GroupBox_PaintDarkTheme;
            groupBoxTypeOfService.Paint += CustomColors.GroupBox_PaintDarkTheme;
            groupBoxRegistryStatus.Paint += CustomColors.GroupBox_PaintDarkTheme;
            groupBoxServerStatus.Paint += CustomColors.GroupBox_PaintDarkTheme;
            separatorH.BackColor = StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR;
            separatorV.BackColor = StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR;

            toolStripStatusBarText.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripStatusBarText.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripVersionText.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripVersionText.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripAutoTheme.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripAutoTheme.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripLightTheme.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripLightTheme.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripDarkTheme.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripDarkTheme.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            logLabel.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            logLabel.BackColor = StringsAndConstants.DARK_BACKGROUND;
            aboutLabel.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            aboutLabel.BackColor = StringsAndConstants.DARK_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.DARK_BACKGROUND;

            statusStrip1.Renderer = new ModifiedToolStripProfessionalDarkTheme();

            toolStripAutoTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_autotheme_dark_path));
            toolStripLightTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_lighttheme_dark_path));
            toolStripDarkTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_darktheme_dark_path));

            comboBoxTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_autotheme_dark_path));
            logLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_log_dark_path));
            aboutLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_about_dark_path));

            topBannerImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.main_banner_dark_path));
            bmIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_brand_dark_path));
            modelIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_model_dark_path));
            serialNoIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_serial_no_dark_path));
            procNameIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_cpu_dark_path));
            pmIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ram_dark_path));
            hdSizeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_disk_size_dark_path));
            mediaTypeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_hdd_dark_path));
            mediaOperationIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ahci_dark_path));
            gpuInfoIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_gpu_dark_path));
            osIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_windows_dark_path));
            hostnameIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_hostname_dark_path));
            macIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_mac_dark_path));
            ipIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ip_dark_path));
            biosTypeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_bios_dark_path));
            biosIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_bios_version_dark_path));
            secBootIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_secure_boot_dark_path));
            patrimonyIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_patr_dark_path));
            sealIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_seal_dark_path));
            roomIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_room_dark_path));
            buildingIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_building_dark_path));
            activeDirectoryIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_server_dark_path));
            standardIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_standard_dark_path));
            datetimeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_service_dark_path));
            letterIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_letter_dark_path));
            inUseIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_in_use_dark_path));
            tagIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_sticker_dark_path));
            typeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_type_dark_path));
            vtIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_VT_x_dark_path));
            whoIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_who_dark_path));
            smartIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_smart_dark_path));
            tpmIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_tpm_dark_path));
            batteryIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_cmos_battery_dark_path));
            ticketIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), StringsAndConstants.icon_ticket_dark_path));
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
            webView2Control.Dispose();
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
            lblServerOpState.Text = StringsAndConstants.DASH;
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
                    lblServerOpState.Text = StringsAndConstants.ONLINE;
                    lblServerOpState.ForeColor = StringsAndConstants.ONLINE_ALERT;
                }
                else
                {
                    loadingCircle24.Visible = false;
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_OFFLINE_SERVER, string.Empty, StringsAndConstants.consoleOutGUI);
                    lblServerOpState.Text = StringsAndConstants.OFFLINE;
                    lblServerOpState.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                }
            }
            else
            {
                loadingCircle24.Visible = false;
                loadingCircle24.Active = false;
                lblIPServer.Text = lblPortServer.Text = lblAgentName.Text = lblServerOpState.Text = StringsAndConstants.OFFLINE_MODE_ACTIVATED;
                lblIPServer.ForeColor = lblPortServer.ForeColor = lblAgentName.ForeColor = lblServerOpState.ForeColor = StringsAndConstants.OFFLINE_ALERT;
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
            webView2Control.Visible = false;
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
            lblProgressPercent.Text = e.ProgressPercentage.ToString() + "%";
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
            await webView2Control.EnsureCoreWebView2Async(webView2Environment);
            webView2Control.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView2Control.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_END_LOADING_WEBVIEW2, string.Empty, StringsAndConstants.consoleOutGUI);
        }

        //Sends hardware info to the specified server
        public void ServerSendInfo(string[] serverArgs)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTERING, string.Empty, StringsAndConstants.consoleOutGUI);
            webView2Control.CoreWebView2.Navigate("http://" + serverArgs[0] + ":" + serverArgs[1] + "/" + serverArgs[2] + ".php?patrimonio=" + serverArgs[3] + "&lacre=" + serverArgs[4] + "&sala=" + serverArgs[5] + "&predio=" + serverArgs[6] + "&ad=" + serverArgs[7] + "&padrao=" + serverArgs[8] + "&formatacao=" + serverArgs[9] + "&formatacoesAnteriores=" + serverArgs[9] + "&marca=" + serverArgs[10] + "&modelo=" + serverArgs[11] + "&numeroSerial=" + serverArgs[12] + "&processador=" + serverArgs[13] + "&memoria=" + serverArgs[14] + "&hd=" + serverArgs[15] + "&sistemaOperacional=" + serverArgs[16] + "&nomeDoComputador=" + serverArgs[17] + "&bios=" + serverArgs[18] + "&mac=" + serverArgs[19] + "&ip=" + serverArgs[20] + "&emUso=" + serverArgs[21] + "&etiqueta=" + serverArgs[22] + "&tipo=" + serverArgs[23] + "&tipoFW=" + serverArgs[24] + "&tipoArmaz=" + serverArgs[25] + "&gpu=" + serverArgs[26] + "&modoArmaz=" + serverArgs[27] + "&secBoot=" + serverArgs[28] + "&vt=" + serverArgs[29] + "&tpm=" + serverArgs[30] + "&trocaPilha=" + serverArgs[31] + "&ticketNum=" + serverArgs[32] + "&agent=" + serverArgs[33]);
        }

        //Runs the registration for the website
        private async void Cadastra_ClickAsync(object sender, EventArgs e)
        {
            webView2Control.Visible = false;
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
            if (!string.IsNullOrWhiteSpace(textBoxPatrimony.Text) && !string.IsNullOrWhiteSpace(textBoxRoom.Text) && !string.IsNullOrWhiteSpace(textBoxTicket.Text) && comboBoxType.SelectedItem != null && comboBoxBuilding.SelectedItem != null && comboBoxInUse.SelectedItem != null && comboBoxTag.SelectedItem != null && comboBoxBattery.SelectedItem != null && (employeeRadioButton.Checked || studentRadioButton.Checked) && (formatRadioButton.Checked || maintenanceRadioButton.Checked) && pass == true)
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
                                webView2Control.Visible = true;
                                ServerSendInfo(sArgs); //Send info to server
                                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTRY_FINISHED, string.Empty, StringsAndConstants.consoleOutGUI);

                                if (formatRadioButton.Checked) //If the format radio button is checked
                                {
                                    MiscMethods.RegCreate(true, dateTimePicker1); //Create reg entries for format and maintenance
                                    lblInstallSince.Text = MiscMethods.SinceLabelUpdate(true);
                                    lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_INSTALLDATE, string.Empty, StringsAndConstants.consoleOutGUI);
                                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_MAINTENANCEDATE, string.Empty, StringsAndConstants.consoleOutGUI);
                                }
                                else if (maintenanceRadioButton.Checked) //If the maintenance radio button is checked
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
                            webView2Control.Visible = true;
                            ServerSendInfo(sArgs); //Send info to server
                            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_REGISTRY_FINISHED, string.Empty, StringsAndConstants.consoleOutGUI);

                            if (formatRadioButton.Checked) //If the format radio button is checked
                            {
                                MiscMethods.RegCreate(true, dateTimePicker1); //Create reg entries for format and maintenance
                                lblInstallSince.Text = MiscMethods.SinceLabelUpdate(true);
                                lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_INSTALLDATE, string.Empty, StringsAndConstants.consoleOutGUI);

                                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RESETING_MAINTENANCEDATE, string.Empty, StringsAndConstants.consoleOutGUI);

                            }
                            else if (maintenanceRadioButton.Checked) //If the maintenance radio button is checked
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

