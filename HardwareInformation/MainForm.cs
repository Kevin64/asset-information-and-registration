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
                    if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                    }

                    DarkTheme();
                }
                else
                {
                    if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                    }

                    LightTheme();
                }
            }
            else if (definitionList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[1]))
            {
                comboBoxTheme.Enabled = false;
                if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                }

                LightTheme();
            }
            else if (definitionList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[2]))
            {
                comboBoxTheme.Enabled = false;
                if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                }

                DarkTheme();
            }

            log = l;
            offlineMode = noConnection;
            defList = definitionList;
            orgList = orgDataList;

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_OFFLINE_MODE, offlineMode.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            this.user = user;
            this.ip = ip;
            this.port = port;

            if (!offlineMode)
            {
                //Fetch building and hw types info from the specified server
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_FETCHING_SERVER_DATA, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
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
            textBoxPatrimony.Text = System.Net.Dns.GetHostName().Substring(0, 3).ToUpper().Equals(ConstantsDLL.Properties.Resources.HOSTNAME_PATTERN)
                ? System.Net.Dns.GetHostName().Substring(3)
                : string.Empty;

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
            lblFixedBM = new System.Windows.Forms.Label();
            lblFixedModel = new System.Windows.Forms.Label();
            lblFixedSerialNo = new System.Windows.Forms.Label();
            lblFixedProcName = new System.Windows.Forms.Label();
            lblFixedPM = new System.Windows.Forms.Label();
            lblFixedHDSize = new System.Windows.Forms.Label();
            lblFixedOS = new System.Windows.Forms.Label();
            lblFixedHostname = new System.Windows.Forms.Label();
            lblFixedMac = new System.Windows.Forms.Label();
            lblFixedIP = new System.Windows.Forms.Label();
            lblFixedPatrimony = new System.Windows.Forms.Label();
            lblFixedSeal = new System.Windows.Forms.Label();
            lblFixedBuilding = new System.Windows.Forms.Label();
            textBoxPatrimony = new System.Windows.Forms.TextBox();
            textBoxSeal = new System.Windows.Forms.TextBox();
            textBoxRoom = new System.Windows.Forms.TextBox();
            textBoxLetter = new System.Windows.Forms.TextBox();
            lblFixedRoom = new System.Windows.Forms.Label();
            lblFixedDateTimePicker = new System.Windows.Forms.Label();
            registerButton = new System.Windows.Forms.Button();
            lblFixedInUse = new System.Windows.Forms.Label();
            lblFixedTag = new System.Windows.Forms.Label();
            lblFixedType = new System.Windows.Forms.Label();
            lblFixedServerOpState = new System.Windows.Forms.Label();
            lblFixedPortServer = new System.Windows.Forms.Label();
            collectButton = new System.Windows.Forms.Button();
            lblFixedLetter = new System.Windows.Forms.Label();
            lblFixedBIOS = new System.Windows.Forms.Label();
            lblBIOS = new System.Windows.Forms.Label();
            accessSystemButton = new System.Windows.Forms.Button();
            lblFixedBIOSType = new System.Windows.Forms.Label();
            lblBIOSType = new System.Windows.Forms.Label();
            groupBoxHWData = new System.Windows.Forms.GroupBox();
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
            tpmIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            smartIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblSmart = new System.Windows.Forms.Label();
            lblTPM = new System.Windows.Forms.Label();
            lblFixedSmart = new System.Windows.Forms.Label();
            vtIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblFixedTPM = new System.Windows.Forms.Label();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            lblProgressPercent = new System.Windows.Forms.Label();
            lblVT = new System.Windows.Forms.Label();
            lblFixedVT = new System.Windows.Forms.Label();
            bmIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            secBootIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            biosIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            biosTypeIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            ipIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            macIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            hostnameIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            osIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            gpuInfoIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            mediaOperationIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            mediaTypeIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            hdSizeIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            pmIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            procNameIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            serialNoIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            modelIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblSecBoot = new System.Windows.Forms.Label();
            lblFixedSecBoot = new System.Windows.Forms.Label();
            lblMediaOperation = new System.Windows.Forms.Label();
            lblFixedMediaOperation = new System.Windows.Forms.Label();
            lblGPUInfo = new System.Windows.Forms.Label();
            lblFixedGPUInfo = new System.Windows.Forms.Label();
            lblMediaType = new System.Windows.Forms.Label();
            lblFixedMediaType = new System.Windows.Forms.Label();
            groupBoxPatrData = new System.Windows.Forms.GroupBox();
            lblFixedMandatory9 = new System.Windows.Forms.Label();
            lblFixedMandatory8 = new System.Windows.Forms.Label();
            ticketIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblFixedTicket = new System.Windows.Forms.Label();
            textBoxTicket = new System.Windows.Forms.TextBox();
            batteryIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblFixedMandatory7 = new System.Windows.Forms.Label();
            lblFixedMandatory6 = new System.Windows.Forms.Label();
            lblFixedBattery = new System.Windows.Forms.Label();
            lblFixedMandatory5 = new System.Windows.Forms.Label();
            lblFixedMandatory4 = new System.Windows.Forms.Label();
            lblFixedMandatory3 = new System.Windows.Forms.Label();
            lblFixedMandatory2 = new System.Windows.Forms.Label();
            lblFixedMandatory = new System.Windows.Forms.Label();
            lblFixedMandatoryMain = new System.Windows.Forms.Label();
            studentRadioButton = new System.Windows.Forms.RadioButton();
            employeeRadioButton = new System.Windows.Forms.RadioButton();
            whoIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblFixedWho = new System.Windows.Forms.Label();
            letterIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            typeIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            tagIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            inUseIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            datetimeIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            standardIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            activeDirectoryIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            buildingIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            roomIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            sealIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            patrimonyIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            groupBoxTypeOfService = new System.Windows.Forms.GroupBox();
            loadingCircle21 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle20 = new MRG.Controls.UI.LoadingCircle();
            lblMaintenanceSince = new System.Windows.Forms.Label();
            lblInstallSince = new System.Windows.Forms.Label();
            lblFixedMandatory10 = new System.Windows.Forms.Label();
            textBoxFixedFormatRadio = new System.Windows.Forms.TextBox();
            textBoxMaintenanceRadio = new System.Windows.Forms.TextBox();
            formatRadioButton = new System.Windows.Forms.RadioButton();
            maintenanceRadioButton = new System.Windows.Forms.RadioButton();
            lblFixedActiveDirectory = new System.Windows.Forms.Label();
            lblFixedStandard = new System.Windows.Forms.Label();
            lblAgentName = new System.Windows.Forms.Label();
            lblFixedAgentName = new System.Windows.Forms.Label();
            lblPortServer = new System.Windows.Forms.Label();
            lblIPServer = new System.Windows.Forms.Label();
            lblFixedIPServer = new System.Windows.Forms.Label();
            lblServerOpState = new System.Windows.Forms.Label();
            toolStripVersionText = new System.Windows.Forms.ToolStripStatusLabel();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            comboBoxTheme = new System.Windows.Forms.ToolStripDropDownButton();
            toolStripAutoTheme = new System.Windows.Forms.ToolStripMenuItem();
            toolStripLightTheme = new System.Windows.Forms.ToolStripMenuItem();
            toolStripDarkTheme = new System.Windows.Forms.ToolStripMenuItem();
            logLabel = new System.Windows.Forms.ToolStripStatusLabel();
            aboutLabel = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusBarText = new System.Windows.Forms.ToolStripStatusLabel();
            timer1 = new System.Windows.Forms.Timer(components);
            timer2 = new System.Windows.Forms.Timer(components);
            timer3 = new System.Windows.Forms.Timer(components);
            timer4 = new System.Windows.Forms.Timer(components);
            timer5 = new System.Windows.Forms.Timer(components);
            timer6 = new System.Windows.Forms.Timer(components);
            timer7 = new System.Windows.Forms.Timer(components);
            timer8 = new System.Windows.Forms.Timer(components);
            groupBoxRegistryStatus = new System.Windows.Forms.GroupBox();
            webView2Control = new Microsoft.Web.WebView2.WinForms.WebView2();
            timer9 = new System.Windows.Forms.Timer(components);
            timer10 = new System.Windows.Forms.Timer(components);
            topBannerImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            loadingCircle22 = new MRG.Controls.UI.LoadingCircle();
            loadingCircle23 = new MRG.Controls.UI.LoadingCircle();
            groupBoxServerStatus = new System.Windows.Forms.GroupBox();
            loadingCircle24 = new MRG.Controls.UI.LoadingCircle();
            comboBoxBattery = new CustomFlatComboBox();
            comboBoxStandard = new CustomFlatComboBox();
            comboBoxActiveDirectory = new CustomFlatComboBox();
            comboBoxTag = new CustomFlatComboBox();
            comboBoxInUse = new CustomFlatComboBox();
            comboBoxType = new CustomFlatComboBox();
            comboBoxBuilding = new CustomFlatComboBox();
            groupBoxHWData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tpmIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)smartIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)vtIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bmIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)secBootIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)biosIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)biosTypeIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ipIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)macIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)hostnameIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)osIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gpuInfoIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)mediaOperationIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)mediaTypeIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)hdSizeIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pmIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)procNameIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)serialNoIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)modelIconImg).BeginInit();
            groupBoxPatrData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ticketIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)batteryIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)whoIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)letterIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)typeIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tagIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inUseIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)datetimeIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)standardIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)activeDirectoryIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)buildingIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)roomIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sealIconImg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)patrimonyIconImg).BeginInit();
            groupBoxTypeOfService.SuspendLayout();
            statusStrip1.SuspendLayout();
            groupBoxRegistryStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView2Control).BeginInit();
            ((System.ComponentModel.ISupportInitialize)topBannerImg).BeginInit();
            groupBoxServerStatus.SuspendLayout();
            SuspendLayout();
            // 
            // lblBM
            // 
            resources.ApplyResources(lblBM, "lblBM");
            lblBM.ForeColor = System.Drawing.Color.Silver;
            lblBM.Name = "lblBM";
            // 
            // lblModel
            // 
            resources.ApplyResources(lblModel, "lblModel");
            lblModel.ForeColor = System.Drawing.Color.Silver;
            lblModel.Name = "lblModel";
            // 
            // lblSerialNo
            // 
            resources.ApplyResources(lblSerialNo, "lblSerialNo");
            lblSerialNo.ForeColor = System.Drawing.Color.Silver;
            lblSerialNo.Name = "lblSerialNo";
            // 
            // lblProcName
            // 
            resources.ApplyResources(lblProcName, "lblProcName");
            lblProcName.ForeColor = System.Drawing.Color.Silver;
            lblProcName.Name = "lblProcName";
            // 
            // lblPM
            // 
            resources.ApplyResources(lblPM, "lblPM");
            lblPM.ForeColor = System.Drawing.Color.Silver;
            lblPM.Name = "lblPM";
            // 
            // lblHDSize
            // 
            resources.ApplyResources(lblHDSize, "lblHDSize");
            lblHDSize.ForeColor = System.Drawing.Color.Silver;
            lblHDSize.Name = "lblHDSize";
            // 
            // lblOS
            // 
            resources.ApplyResources(lblOS, "lblOS");
            lblOS.ForeColor = System.Drawing.Color.Silver;
            lblOS.Name = "lblOS";
            // 
            // lblHostname
            // 
            resources.ApplyResources(lblHostname, "lblHostname");
            lblHostname.ForeColor = System.Drawing.Color.Silver;
            lblHostname.Name = "lblHostname";
            // 
            // lblMac
            // 
            resources.ApplyResources(lblMac, "lblMac");
            lblMac.ForeColor = System.Drawing.Color.Silver;
            lblMac.Name = "lblMac";
            // 
            // lblIP
            // 
            resources.ApplyResources(lblIP, "lblIP");
            lblIP.ForeColor = System.Drawing.Color.Silver;
            lblIP.Name = "lblIP";
            // 
            // lblFixedBM
            // 
            resources.ApplyResources(lblFixedBM, "lblFixedBM");
            lblFixedBM.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedBM.Name = "lblFixedBM";
            // 
            // lblFixedModel
            // 
            resources.ApplyResources(lblFixedModel, "lblFixedModel");
            lblFixedModel.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedModel.Name = "lblFixedModel";
            // 
            // lblFixedSerialNo
            // 
            resources.ApplyResources(lblFixedSerialNo, "lblFixedSerialNo");
            lblFixedSerialNo.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedSerialNo.Name = "lblFixedSerialNo";
            // 
            // lblFixedProcName
            // 
            resources.ApplyResources(lblFixedProcName, "lblFixedProcName");
            lblFixedProcName.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedProcName.Name = "lblFixedProcName";
            // 
            // lblFixedPM
            // 
            resources.ApplyResources(lblFixedPM, "lblFixedPM");
            lblFixedPM.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedPM.Name = "lblFixedPM";
            // 
            // lblFixedHDSize
            // 
            resources.ApplyResources(lblFixedHDSize, "lblFixedHDSize");
            lblFixedHDSize.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedHDSize.Name = "lblFixedHDSize";
            // 
            // lblFixedOS
            // 
            resources.ApplyResources(lblFixedOS, "lblFixedOS");
            lblFixedOS.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedOS.Name = "lblFixedOS";
            // 
            // lblFixedHostname
            // 
            resources.ApplyResources(lblFixedHostname, "lblFixedHostname");
            lblFixedHostname.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedHostname.Name = "lblFixedHostname";
            // 
            // lblFixedMac
            // 
            resources.ApplyResources(lblFixedMac, "lblFixedMac");
            lblFixedMac.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedMac.Name = "lblFixedMac";
            // 
            // lblFixedIP
            // 
            resources.ApplyResources(lblFixedIP, "lblFixedIP");
            lblFixedIP.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedIP.Name = "lblFixedIP";
            // 
            // lblFixedPatrimony
            // 
            resources.ApplyResources(lblFixedPatrimony, "lblFixedPatrimony");
            lblFixedPatrimony.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedPatrimony.Name = "lblFixedPatrimony";
            // 
            // lblFixedSeal
            // 
            resources.ApplyResources(lblFixedSeal, "lblFixedSeal");
            lblFixedSeal.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedSeal.Name = "lblFixedSeal";
            // 
            // lblFixedBuilding
            // 
            resources.ApplyResources(lblFixedBuilding, "lblFixedBuilding");
            lblFixedBuilding.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedBuilding.Name = "lblFixedBuilding";
            // 
            // textBoxPatrimony
            // 
            resources.ApplyResources(textBoxPatrimony, "textBoxPatrimony");
            textBoxPatrimony.BackColor = System.Drawing.SystemColors.Window;
            textBoxPatrimony.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxPatrimony.Name = "textBoxPatrimony";
            textBoxPatrimony.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxSeal
            // 
            resources.ApplyResources(textBoxSeal, "textBoxSeal");
            textBoxSeal.BackColor = System.Drawing.SystemColors.Window;
            textBoxSeal.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxSeal.Name = "textBoxSeal";
            textBoxSeal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxRoom
            // 
            resources.ApplyResources(textBoxRoom, "textBoxRoom");
            textBoxRoom.BackColor = System.Drawing.SystemColors.Window;
            textBoxRoom.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxRoom.Name = "textBoxRoom";
            textBoxRoom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxLetter
            // 
            resources.ApplyResources(textBoxLetter, "textBoxLetter");
            textBoxLetter.BackColor = System.Drawing.SystemColors.Window;
            textBoxLetter.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxLetter.Name = "textBoxLetter";
            textBoxLetter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxCharsOnly_KeyPress);
            // 
            // lblFixedRoom
            // 
            resources.ApplyResources(lblFixedRoom, "lblFixedRoom");
            lblFixedRoom.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedRoom.Name = "lblFixedRoom";
            // 
            // lblFixedDateTimePicker
            // 
            resources.ApplyResources(lblFixedDateTimePicker, "lblFixedDateTimePicker");
            lblFixedDateTimePicker.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedDateTimePicker.Name = "lblFixedDateTimePicker";
            // 
            // registerButton
            // 
            resources.ApplyResources(registerButton, "registerButton");
            registerButton.BackColor = System.Drawing.SystemColors.Control;
            registerButton.ForeColor = System.Drawing.SystemColors.ControlText;
            registerButton.Name = "registerButton";
            registerButton.UseVisualStyleBackColor = true;
            registerButton.Click += new System.EventHandler(Cadastra_ClickAsync);
            // 
            // lblFixedInUse
            // 
            resources.ApplyResources(lblFixedInUse, "lblFixedInUse");
            lblFixedInUse.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedInUse.Name = "lblFixedInUse";
            // 
            // lblFixedTag
            // 
            resources.ApplyResources(lblFixedTag, "lblFixedTag");
            lblFixedTag.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedTag.Name = "lblFixedTag";
            // 
            // lblFixedType
            // 
            resources.ApplyResources(lblFixedType, "lblFixedType");
            lblFixedType.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedType.Name = "lblFixedType";
            // 
            // lblFixedServerOpState
            // 
            resources.ApplyResources(lblFixedServerOpState, "lblFixedServerOpState");
            lblFixedServerOpState.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedServerOpState.Name = "lblFixedServerOpState";
            // 
            // lblFixedPortServer
            // 
            resources.ApplyResources(lblFixedPortServer, "lblFixedPortServer");
            lblFixedPortServer.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedPortServer.Name = "lblFixedPortServer";
            // 
            // collectButton
            // 
            resources.ApplyResources(collectButton, "collectButton");
            collectButton.BackColor = System.Drawing.SystemColors.Control;
            collectButton.ForeColor = System.Drawing.SystemColors.ControlText;
            collectButton.Name = "collectButton";
            collectButton.UseVisualStyleBackColor = true;
            collectButton.Click += new System.EventHandler(Coleta_Click);
            // 
            // lblFixedLetter
            // 
            resources.ApplyResources(lblFixedLetter, "lblFixedLetter");
            lblFixedLetter.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedLetter.Name = "lblFixedLetter";
            // 
            // lblFixedBIOS
            // 
            resources.ApplyResources(lblFixedBIOS, "lblFixedBIOS");
            lblFixedBIOS.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedBIOS.Name = "lblFixedBIOS";
            // 
            // lblBIOS
            // 
            resources.ApplyResources(lblBIOS, "lblBIOS");
            lblBIOS.ForeColor = System.Drawing.Color.Silver;
            lblBIOS.Name = "lblBIOS";
            // 
            // accessSystemButton
            // 
            resources.ApplyResources(accessSystemButton, "accessSystemButton");
            accessSystemButton.BackColor = System.Drawing.SystemColors.Control;
            accessSystemButton.ForeColor = System.Drawing.SystemColors.ControlText;
            accessSystemButton.Name = "accessSystemButton";
            accessSystemButton.UseVisualStyleBackColor = true;
            accessSystemButton.Click += new System.EventHandler(AccessButton_Click);
            // 
            // lblFixedBIOSType
            // 
            resources.ApplyResources(lblFixedBIOSType, "lblFixedBIOSType");
            lblFixedBIOSType.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedBIOSType.Name = "lblFixedBIOSType";
            // 
            // lblBIOSType
            // 
            resources.ApplyResources(lblBIOSType, "lblBIOSType");
            lblBIOSType.ForeColor = System.Drawing.Color.Silver;
            lblBIOSType.Name = "lblBIOSType";
            // 
            // groupBoxHWData
            // 
            resources.ApplyResources(groupBoxHWData, "groupBoxHWData");
            groupBoxHWData.Controls.Add(loadingCircle19);
            groupBoxHWData.Controls.Add(loadingCircle18);
            groupBoxHWData.Controls.Add(loadingCircle17);
            groupBoxHWData.Controls.Add(loadingCircle16);
            groupBoxHWData.Controls.Add(loadingCircle15);
            groupBoxHWData.Controls.Add(loadingCircle14);
            groupBoxHWData.Controls.Add(loadingCircle13);
            groupBoxHWData.Controls.Add(loadingCircle12);
            groupBoxHWData.Controls.Add(loadingCircle11);
            groupBoxHWData.Controls.Add(loadingCircle10);
            groupBoxHWData.Controls.Add(loadingCircle9);
            groupBoxHWData.Controls.Add(loadingCircle8);
            groupBoxHWData.Controls.Add(loadingCircle7);
            groupBoxHWData.Controls.Add(loadingCircle6);
            groupBoxHWData.Controls.Add(loadingCircle5);
            groupBoxHWData.Controls.Add(loadingCircle4);
            groupBoxHWData.Controls.Add(loadingCircle3);
            groupBoxHWData.Controls.Add(loadingCircle2);
            groupBoxHWData.Controls.Add(loadingCircle1);
            groupBoxHWData.Controls.Add(separatorH);
            groupBoxHWData.Controls.Add(separatorV);
            groupBoxHWData.Controls.Add(tpmIconImg);
            groupBoxHWData.Controls.Add(smartIconImg);
            groupBoxHWData.Controls.Add(lblSmart);
            groupBoxHWData.Controls.Add(lblTPM);
            groupBoxHWData.Controls.Add(lblFixedSmart);
            groupBoxHWData.Controls.Add(vtIconImg);
            groupBoxHWData.Controls.Add(lblFixedTPM);
            groupBoxHWData.Controls.Add(progressBar1);
            groupBoxHWData.Controls.Add(lblProgressPercent);
            groupBoxHWData.Controls.Add(lblVT);
            groupBoxHWData.Controls.Add(lblFixedVT);
            groupBoxHWData.Controls.Add(bmIconImg);
            groupBoxHWData.Controls.Add(secBootIconImg);
            groupBoxHWData.Controls.Add(biosIconImg);
            groupBoxHWData.Controls.Add(biosTypeIconImg);
            groupBoxHWData.Controls.Add(ipIconImg);
            groupBoxHWData.Controls.Add(macIconImg);
            groupBoxHWData.Controls.Add(hostnameIconImg);
            groupBoxHWData.Controls.Add(osIconImg);
            groupBoxHWData.Controls.Add(gpuInfoIconImg);
            groupBoxHWData.Controls.Add(mediaOperationIconImg);
            groupBoxHWData.Controls.Add(mediaTypeIconImg);
            groupBoxHWData.Controls.Add(hdSizeIconImg);
            groupBoxHWData.Controls.Add(pmIconImg);
            groupBoxHWData.Controls.Add(procNameIconImg);
            groupBoxHWData.Controls.Add(serialNoIconImg);
            groupBoxHWData.Controls.Add(modelIconImg);
            groupBoxHWData.Controls.Add(lblSecBoot);
            groupBoxHWData.Controls.Add(lblFixedSecBoot);
            groupBoxHWData.Controls.Add(lblMediaOperation);
            groupBoxHWData.Controls.Add(lblFixedMediaOperation);
            groupBoxHWData.Controls.Add(lblGPUInfo);
            groupBoxHWData.Controls.Add(lblFixedGPUInfo);
            groupBoxHWData.Controls.Add(lblMediaType);
            groupBoxHWData.Controls.Add(lblFixedMediaType);
            groupBoxHWData.Controls.Add(lblFixedBM);
            groupBoxHWData.Controls.Add(lblOS);
            groupBoxHWData.Controls.Add(lblBIOSType);
            groupBoxHWData.Controls.Add(lblHDSize);
            groupBoxHWData.Controls.Add(lblFixedBIOSType);
            groupBoxHWData.Controls.Add(lblPM);
            groupBoxHWData.Controls.Add(lblProcName);
            groupBoxHWData.Controls.Add(lblSerialNo);
            groupBoxHWData.Controls.Add(lblBIOS);
            groupBoxHWData.Controls.Add(lblModel);
            groupBoxHWData.Controls.Add(lblFixedBIOS);
            groupBoxHWData.Controls.Add(lblBM);
            groupBoxHWData.Controls.Add(lblHostname);
            groupBoxHWData.Controls.Add(lblMac);
            groupBoxHWData.Controls.Add(lblIP);
            groupBoxHWData.Controls.Add(lblFixedModel);
            groupBoxHWData.Controls.Add(lblFixedSerialNo);
            groupBoxHWData.Controls.Add(lblFixedProcName);
            groupBoxHWData.Controls.Add(lblFixedPM);
            groupBoxHWData.Controls.Add(lblFixedHDSize);
            groupBoxHWData.Controls.Add(lblFixedOS);
            groupBoxHWData.Controls.Add(lblFixedHostname);
            groupBoxHWData.Controls.Add(lblFixedMac);
            groupBoxHWData.Controls.Add(lblFixedIP);
            groupBoxHWData.ForeColor = System.Drawing.SystemColors.ControlText;
            groupBoxHWData.Name = "groupBoxHWData";
            groupBoxHWData.TabStop = false;
            // 
            // loadingCircle19
            // 
            resources.ApplyResources(loadingCircle19, "loadingCircle19");
            loadingCircle19.Active = false;
            loadingCircle19.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle19.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle19.InnerCircleRadius = 5;
            loadingCircle19.Name = "loadingCircle19";
            loadingCircle19.NumberSpoke = 12;
            loadingCircle19.OuterCircleRadius = 11;
            loadingCircle19.RotationSpeed = 1;
            loadingCircle19.SpokeThickness = 2;
            loadingCircle19.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle18
            // 
            resources.ApplyResources(loadingCircle18, "loadingCircle18");
            loadingCircle18.Active = false;
            loadingCircle18.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle18.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle18.InnerCircleRadius = 5;
            loadingCircle18.Name = "loadingCircle18";
            loadingCircle18.NumberSpoke = 12;
            loadingCircle18.OuterCircleRadius = 11;
            loadingCircle18.RotationSpeed = 1;
            loadingCircle18.SpokeThickness = 2;
            loadingCircle18.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle17
            // 
            resources.ApplyResources(loadingCircle17, "loadingCircle17");
            loadingCircle17.Active = false;
            loadingCircle17.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle17.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle17.InnerCircleRadius = 5;
            loadingCircle17.Name = "loadingCircle17";
            loadingCircle17.NumberSpoke = 12;
            loadingCircle17.OuterCircleRadius = 11;
            loadingCircle17.RotationSpeed = 1;
            loadingCircle17.SpokeThickness = 2;
            loadingCircle17.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle16
            // 
            resources.ApplyResources(loadingCircle16, "loadingCircle16");
            loadingCircle16.Active = false;
            loadingCircle16.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle16.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle16.InnerCircleRadius = 5;
            loadingCircle16.Name = "loadingCircle16";
            loadingCircle16.NumberSpoke = 12;
            loadingCircle16.OuterCircleRadius = 11;
            loadingCircle16.RotationSpeed = 1;
            loadingCircle16.SpokeThickness = 2;
            loadingCircle16.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle15
            // 
            resources.ApplyResources(loadingCircle15, "loadingCircle15");
            loadingCircle15.Active = false;
            loadingCircle15.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle15.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle15.InnerCircleRadius = 5;
            loadingCircle15.Name = "loadingCircle15";
            loadingCircle15.NumberSpoke = 12;
            loadingCircle15.OuterCircleRadius = 11;
            loadingCircle15.RotationSpeed = 1;
            loadingCircle15.SpokeThickness = 2;
            loadingCircle15.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle14
            // 
            resources.ApplyResources(loadingCircle14, "loadingCircle14");
            loadingCircle14.Active = false;
            loadingCircle14.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle14.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle14.InnerCircleRadius = 5;
            loadingCircle14.Name = "loadingCircle14";
            loadingCircle14.NumberSpoke = 12;
            loadingCircle14.OuterCircleRadius = 11;
            loadingCircle14.RotationSpeed = 1;
            loadingCircle14.SpokeThickness = 2;
            loadingCircle14.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle13
            // 
            resources.ApplyResources(loadingCircle13, "loadingCircle13");
            loadingCircle13.Active = false;
            loadingCircle13.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle13.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle13.InnerCircleRadius = 5;
            loadingCircle13.Name = "loadingCircle13";
            loadingCircle13.NumberSpoke = 12;
            loadingCircle13.OuterCircleRadius = 11;
            loadingCircle13.RotationSpeed = 1;
            loadingCircle13.SpokeThickness = 2;
            loadingCircle13.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle12
            // 
            resources.ApplyResources(loadingCircle12, "loadingCircle12");
            loadingCircle12.Active = false;
            loadingCircle12.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle12.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle12.InnerCircleRadius = 5;
            loadingCircle12.Name = "loadingCircle12";
            loadingCircle12.NumberSpoke = 12;
            loadingCircle12.OuterCircleRadius = 11;
            loadingCircle12.RotationSpeed = 1;
            loadingCircle12.SpokeThickness = 2;
            loadingCircle12.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle11
            // 
            resources.ApplyResources(loadingCircle11, "loadingCircle11");
            loadingCircle11.Active = false;
            loadingCircle11.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle11.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle11.InnerCircleRadius = 5;
            loadingCircle11.Name = "loadingCircle11";
            loadingCircle11.NumberSpoke = 12;
            loadingCircle11.OuterCircleRadius = 11;
            loadingCircle11.RotationSpeed = 1;
            loadingCircle11.SpokeThickness = 2;
            loadingCircle11.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle10
            // 
            resources.ApplyResources(loadingCircle10, "loadingCircle10");
            loadingCircle10.Active = false;
            loadingCircle10.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle10.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle10.InnerCircleRadius = 5;
            loadingCircle10.Name = "loadingCircle10";
            loadingCircle10.NumberSpoke = 12;
            loadingCircle10.OuterCircleRadius = 11;
            loadingCircle10.RotationSpeed = 1;
            loadingCircle10.SpokeThickness = 2;
            loadingCircle10.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle9
            // 
            resources.ApplyResources(loadingCircle9, "loadingCircle9");
            loadingCircle9.Active = false;
            loadingCircle9.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle9.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle9.InnerCircleRadius = 5;
            loadingCircle9.Name = "loadingCircle9";
            loadingCircle9.NumberSpoke = 12;
            loadingCircle9.OuterCircleRadius = 11;
            loadingCircle9.RotationSpeed = 1;
            loadingCircle9.SpokeThickness = 2;
            loadingCircle9.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle8
            // 
            resources.ApplyResources(loadingCircle8, "loadingCircle8");
            loadingCircle8.Active = false;
            loadingCircle8.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle8.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle8.InnerCircleRadius = 5;
            loadingCircle8.Name = "loadingCircle8";
            loadingCircle8.NumberSpoke = 12;
            loadingCircle8.OuterCircleRadius = 11;
            loadingCircle8.RotationSpeed = 1;
            loadingCircle8.SpokeThickness = 2;
            loadingCircle8.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle7
            // 
            resources.ApplyResources(loadingCircle7, "loadingCircle7");
            loadingCircle7.Active = false;
            loadingCircle7.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle7.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle7.InnerCircleRadius = 5;
            loadingCircle7.Name = "loadingCircle7";
            loadingCircle7.NumberSpoke = 12;
            loadingCircle7.OuterCircleRadius = 11;
            loadingCircle7.RotationSpeed = 1;
            loadingCircle7.SpokeThickness = 2;
            loadingCircle7.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle6
            // 
            resources.ApplyResources(loadingCircle6, "loadingCircle6");
            loadingCircle6.Active = false;
            loadingCircle6.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle6.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle6.InnerCircleRadius = 5;
            loadingCircle6.Name = "loadingCircle6";
            loadingCircle6.NumberSpoke = 12;
            loadingCircle6.OuterCircleRadius = 11;
            loadingCircle6.RotationSpeed = 1;
            loadingCircle6.SpokeThickness = 2;
            loadingCircle6.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle5
            // 
            resources.ApplyResources(loadingCircle5, "loadingCircle5");
            loadingCircle5.Active = false;
            loadingCircle5.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle5.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle5.InnerCircleRadius = 5;
            loadingCircle5.Name = "loadingCircle5";
            loadingCircle5.NumberSpoke = 12;
            loadingCircle5.OuterCircleRadius = 11;
            loadingCircle5.RotationSpeed = 1;
            loadingCircle5.SpokeThickness = 2;
            loadingCircle5.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle4
            // 
            resources.ApplyResources(loadingCircle4, "loadingCircle4");
            loadingCircle4.Active = false;
            loadingCircle4.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle4.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle4.InnerCircleRadius = 5;
            loadingCircle4.Name = "loadingCircle4";
            loadingCircle4.NumberSpoke = 12;
            loadingCircle4.OuterCircleRadius = 11;
            loadingCircle4.RotationSpeed = 1;
            loadingCircle4.SpokeThickness = 2;
            loadingCircle4.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle3
            // 
            resources.ApplyResources(loadingCircle3, "loadingCircle3");
            loadingCircle3.Active = false;
            loadingCircle3.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle3.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle3.InnerCircleRadius = 5;
            loadingCircle3.Name = "loadingCircle3";
            loadingCircle3.NumberSpoke = 12;
            loadingCircle3.OuterCircleRadius = 11;
            loadingCircle3.RotationSpeed = 1;
            loadingCircle3.SpokeThickness = 2;
            loadingCircle3.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle2
            // 
            resources.ApplyResources(loadingCircle2, "loadingCircle2");
            loadingCircle2.Active = false;
            loadingCircle2.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle2.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle2.InnerCircleRadius = 5;
            loadingCircle2.Name = "loadingCircle2";
            loadingCircle2.NumberSpoke = 12;
            loadingCircle2.OuterCircleRadius = 11;
            loadingCircle2.RotationSpeed = 1;
            loadingCircle2.SpokeThickness = 2;
            loadingCircle2.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle1
            // 
            resources.ApplyResources(loadingCircle1, "loadingCircle1");
            loadingCircle1.Active = false;
            loadingCircle1.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle1.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle1.InnerCircleRadius = 5;
            loadingCircle1.Name = "loadingCircle1";
            loadingCircle1.NumberSpoke = 12;
            loadingCircle1.OuterCircleRadius = 11;
            loadingCircle1.RotationSpeed = 1;
            loadingCircle1.SpokeThickness = 2;
            loadingCircle1.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // separatorH
            // 
            resources.ApplyResources(separatorH, "separatorH");
            separatorH.BackColor = System.Drawing.Color.DimGray;
            separatorH.Name = "separatorH";
            // 
            // separatorV
            // 
            resources.ApplyResources(separatorV, "separatorV");
            separatorV.BackColor = System.Drawing.Color.DimGray;
            separatorV.Name = "separatorV";
            // 
            // tpmIconImg
            // 
            resources.ApplyResources(tpmIconImg, "tpmIconImg");
            tpmIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            tpmIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            tpmIconImg.Name = "tpmIconImg";
            tpmIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            tpmIconImg.TabStop = false;
            // 
            // smartIconImg
            // 
            resources.ApplyResources(smartIconImg, "smartIconImg");
            smartIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            smartIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            smartIconImg.Name = "smartIconImg";
            smartIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            smartIconImg.TabStop = false;
            // 
            // lblSmart
            // 
            resources.ApplyResources(lblSmart, "lblSmart");
            lblSmart.ForeColor = System.Drawing.Color.Silver;
            lblSmart.Name = "lblSmart";
            // 
            // lblTPM
            // 
            resources.ApplyResources(lblTPM, "lblTPM");
            lblTPM.ForeColor = System.Drawing.Color.Silver;
            lblTPM.Name = "lblTPM";
            // 
            // lblFixedSmart
            // 
            resources.ApplyResources(lblFixedSmart, "lblFixedSmart");
            lblFixedSmart.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedSmart.Name = "lblFixedSmart";
            // 
            // vtIconImg
            // 
            resources.ApplyResources(vtIconImg, "vtIconImg");
            vtIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            vtIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            vtIconImg.Name = "vtIconImg";
            vtIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            vtIconImg.TabStop = false;
            // 
            // lblFixedTPM
            // 
            resources.ApplyResources(lblFixedTPM, "lblFixedTPM");
            lblFixedTPM.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedTPM.Name = "lblFixedTPM";
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            // 
            // lblProgressPercent
            // 
            resources.ApplyResources(lblProgressPercent, "lblProgressPercent");
            lblProgressPercent.BackColor = System.Drawing.Color.Transparent;
            lblProgressPercent.ForeColor = System.Drawing.SystemColors.ControlText;
            lblProgressPercent.Name = "lblProgressPercent";
            // 
            // lblVT
            // 
            resources.ApplyResources(lblVT, "lblVT");
            lblVT.ForeColor = System.Drawing.Color.Silver;
            lblVT.Name = "lblVT";
            // 
            // lblFixedVT
            // 
            resources.ApplyResources(lblFixedVT, "lblFixedVT");
            lblFixedVT.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedVT.Name = "lblFixedVT";
            // 
            // bmIconImg
            // 
            resources.ApplyResources(bmIconImg, "bmIconImg");
            bmIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            bmIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            bmIconImg.Name = "bmIconImg";
            bmIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            bmIconImg.TabStop = false;
            // 
            // secBootIconImg
            // 
            resources.ApplyResources(secBootIconImg, "secBootIconImg");
            secBootIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            secBootIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            secBootIconImg.Name = "secBootIconImg";
            secBootIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            secBootIconImg.TabStop = false;
            // 
            // biosIconImg
            // 
            resources.ApplyResources(biosIconImg, "biosIconImg");
            biosIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            biosIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            biosIconImg.Name = "biosIconImg";
            biosIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            biosIconImg.TabStop = false;
            // 
            // biosTypeIconImg
            // 
            resources.ApplyResources(biosTypeIconImg, "biosTypeIconImg");
            biosTypeIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            biosTypeIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            biosTypeIconImg.Name = "biosTypeIconImg";
            biosTypeIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            biosTypeIconImg.TabStop = false;
            // 
            // ipIconImg
            // 
            resources.ApplyResources(ipIconImg, "ipIconImg");
            ipIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            ipIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            ipIconImg.Name = "ipIconImg";
            ipIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            ipIconImg.TabStop = false;
            // 
            // macIconImg
            // 
            resources.ApplyResources(macIconImg, "macIconImg");
            macIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            macIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            macIconImg.Name = "macIconImg";
            macIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            macIconImg.TabStop = false;
            // 
            // hostnameIconImg
            // 
            resources.ApplyResources(hostnameIconImg, "hostnameIconImg");
            hostnameIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            hostnameIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            hostnameIconImg.Name = "hostnameIconImg";
            hostnameIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            hostnameIconImg.TabStop = false;
            // 
            // osIconImg
            // 
            resources.ApplyResources(osIconImg, "osIconImg");
            osIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            osIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            osIconImg.Name = "osIconImg";
            osIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            osIconImg.TabStop = false;
            // 
            // gpuInfoIconImg
            // 
            resources.ApplyResources(gpuInfoIconImg, "gpuInfoIconImg");
            gpuInfoIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gpuInfoIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            gpuInfoIconImg.Name = "gpuInfoIconImg";
            gpuInfoIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gpuInfoIconImg.TabStop = false;
            // 
            // mediaOperationIconImg
            // 
            resources.ApplyResources(mediaOperationIconImg, "mediaOperationIconImg");
            mediaOperationIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            mediaOperationIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            mediaOperationIconImg.Name = "mediaOperationIconImg";
            mediaOperationIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            mediaOperationIconImg.TabStop = false;
            // 
            // mediaTypeIconImg
            // 
            resources.ApplyResources(mediaTypeIconImg, "mediaTypeIconImg");
            mediaTypeIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            mediaTypeIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            mediaTypeIconImg.Name = "mediaTypeIconImg";
            mediaTypeIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            mediaTypeIconImg.TabStop = false;
            // 
            // hdSizeIconImg
            // 
            resources.ApplyResources(hdSizeIconImg, "hdSizeIconImg");
            hdSizeIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            hdSizeIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            hdSizeIconImg.Name = "hdSizeIconImg";
            hdSizeIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            hdSizeIconImg.TabStop = false;
            // 
            // pmIconImg
            // 
            resources.ApplyResources(pmIconImg, "pmIconImg");
            pmIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            pmIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            pmIconImg.Name = "pmIconImg";
            pmIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            pmIconImg.TabStop = false;
            // 
            // procNameIconImg
            // 
            resources.ApplyResources(procNameIconImg, "procNameIconImg");
            procNameIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            procNameIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            procNameIconImg.Name = "procNameIconImg";
            procNameIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            procNameIconImg.TabStop = false;
            // 
            // serialNoIconImg
            // 
            resources.ApplyResources(serialNoIconImg, "serialNoIconImg");
            serialNoIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            serialNoIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            serialNoIconImg.Name = "serialNoIconImg";
            serialNoIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            serialNoIconImg.TabStop = false;
            // 
            // modelIconImg
            // 
            resources.ApplyResources(modelIconImg, "modelIconImg");
            modelIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            modelIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            modelIconImg.Name = "modelIconImg";
            modelIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            modelIconImg.TabStop = false;
            // 
            // lblSecBoot
            // 
            resources.ApplyResources(lblSecBoot, "lblSecBoot");
            lblSecBoot.ForeColor = System.Drawing.Color.Silver;
            lblSecBoot.Name = "lblSecBoot";
            // 
            // lblFixedSecBoot
            // 
            resources.ApplyResources(lblFixedSecBoot, "lblFixedSecBoot");
            lblFixedSecBoot.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedSecBoot.Name = "lblFixedSecBoot";
            // 
            // lblMediaOperation
            // 
            resources.ApplyResources(lblMediaOperation, "lblMediaOperation");
            lblMediaOperation.ForeColor = System.Drawing.Color.Silver;
            lblMediaOperation.Name = "lblMediaOperation";
            // 
            // lblFixedMediaOperation
            // 
            resources.ApplyResources(lblFixedMediaOperation, "lblFixedMediaOperation");
            lblFixedMediaOperation.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedMediaOperation.Name = "lblFixedMediaOperation";
            // 
            // lblGPUInfo
            // 
            resources.ApplyResources(lblGPUInfo, "lblGPUInfo");
            lblGPUInfo.ForeColor = System.Drawing.Color.Silver;
            lblGPUInfo.Name = "lblGPUInfo";
            // 
            // lblFixedGPUInfo
            // 
            resources.ApplyResources(lblFixedGPUInfo, "lblFixedGPUInfo");
            lblFixedGPUInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedGPUInfo.Name = "lblFixedGPUInfo";
            // 
            // lblMediaType
            // 
            resources.ApplyResources(lblMediaType, "lblMediaType");
            lblMediaType.ForeColor = System.Drawing.Color.Silver;
            lblMediaType.Name = "lblMediaType";
            // 
            // lblFixedMediaType
            // 
            resources.ApplyResources(lblFixedMediaType, "lblFixedMediaType");
            lblFixedMediaType.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedMediaType.Name = "lblFixedMediaType";
            // 
            // groupBoxPatrData
            // 
            resources.ApplyResources(groupBoxPatrData, "groupBoxPatrData");
            groupBoxPatrData.Controls.Add(comboBoxBattery);
            groupBoxPatrData.Controls.Add(comboBoxStandard);
            groupBoxPatrData.Controls.Add(comboBoxActiveDirectory);
            groupBoxPatrData.Controls.Add(comboBoxTag);
            groupBoxPatrData.Controls.Add(comboBoxInUse);
            groupBoxPatrData.Controls.Add(comboBoxType);
            groupBoxPatrData.Controls.Add(comboBoxBuilding);
            groupBoxPatrData.Controls.Add(lblFixedMandatory9);
            groupBoxPatrData.Controls.Add(lblFixedMandatory8);
            groupBoxPatrData.Controls.Add(ticketIconImg);
            groupBoxPatrData.Controls.Add(lblFixedTicket);
            groupBoxPatrData.Controls.Add(textBoxTicket);
            groupBoxPatrData.Controls.Add(batteryIconImg);
            groupBoxPatrData.Controls.Add(lblFixedMandatory7);
            groupBoxPatrData.Controls.Add(lblFixedMandatory6);
            groupBoxPatrData.Controls.Add(lblFixedBattery);
            groupBoxPatrData.Controls.Add(lblFixedMandatory5);
            groupBoxPatrData.Controls.Add(lblFixedMandatory4);
            groupBoxPatrData.Controls.Add(lblFixedMandatory3);
            groupBoxPatrData.Controls.Add(lblFixedMandatory2);
            groupBoxPatrData.Controls.Add(lblFixedMandatory);
            groupBoxPatrData.Controls.Add(lblFixedMandatoryMain);
            groupBoxPatrData.Controls.Add(studentRadioButton);
            groupBoxPatrData.Controls.Add(employeeRadioButton);
            groupBoxPatrData.Controls.Add(whoIconImg);
            groupBoxPatrData.Controls.Add(lblFixedWho);
            groupBoxPatrData.Controls.Add(letterIconImg);
            groupBoxPatrData.Controls.Add(typeIconImg);
            groupBoxPatrData.Controls.Add(tagIconImg);
            groupBoxPatrData.Controls.Add(inUseIconImg);
            groupBoxPatrData.Controls.Add(datetimeIconImg);
            groupBoxPatrData.Controls.Add(standardIconImg);
            groupBoxPatrData.Controls.Add(activeDirectoryIconImg);
            groupBoxPatrData.Controls.Add(buildingIconImg);
            groupBoxPatrData.Controls.Add(roomIconImg);
            groupBoxPatrData.Controls.Add(sealIconImg);
            groupBoxPatrData.Controls.Add(patrimonyIconImg);
            groupBoxPatrData.Controls.Add(dateTimePicker1);
            groupBoxPatrData.Controls.Add(groupBoxTypeOfService);
            groupBoxPatrData.Controls.Add(lblFixedPatrimony);
            groupBoxPatrData.Controls.Add(lblFixedSeal);
            groupBoxPatrData.Controls.Add(lblFixedBuilding);
            groupBoxPatrData.Controls.Add(textBoxPatrimony);
            groupBoxPatrData.Controls.Add(textBoxSeal);
            groupBoxPatrData.Controls.Add(lblFixedLetter);
            groupBoxPatrData.Controls.Add(textBoxRoom);
            groupBoxPatrData.Controls.Add(lblFixedRoom);
            groupBoxPatrData.Controls.Add(lblFixedActiveDirectory);
            groupBoxPatrData.Controls.Add(lblFixedDateTimePicker);
            groupBoxPatrData.Controls.Add(lblFixedType);
            groupBoxPatrData.Controls.Add(lblFixedStandard);
            groupBoxPatrData.Controls.Add(textBoxLetter);
            groupBoxPatrData.Controls.Add(lblFixedInUse);
            groupBoxPatrData.Controls.Add(lblFixedTag);
            groupBoxPatrData.ForeColor = System.Drawing.SystemColors.ControlText;
            groupBoxPatrData.Name = "groupBoxPatrData";
            groupBoxPatrData.TabStop = false;
            // 
            // lblFixedMandatory9
            // 
            resources.ApplyResources(lblFixedMandatory9, "lblFixedMandatory9");
            lblFixedMandatory9.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatory9.Name = "lblFixedMandatory9";
            // 
            // lblFixedMandatory8
            // 
            resources.ApplyResources(lblFixedMandatory8, "lblFixedMandatory8");
            lblFixedMandatory8.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatory8.Name = "lblFixedMandatory8";
            // 
            // ticketIconImg
            // 
            resources.ApplyResources(ticketIconImg, "ticketIconImg");
            ticketIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            ticketIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            ticketIconImg.Name = "ticketIconImg";
            ticketIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            ticketIconImg.TabStop = false;
            // 
            // lblFixedTicket
            // 
            resources.ApplyResources(lblFixedTicket, "lblFixedTicket");
            lblFixedTicket.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedTicket.Name = "lblFixedTicket";
            // 
            // textBoxTicket
            // 
            resources.ApplyResources(textBoxTicket, "textBoxTicket");
            textBoxTicket.BackColor = System.Drawing.SystemColors.Window;
            textBoxTicket.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxTicket.Name = "textBoxTicket";
            textBoxTicket.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // batteryIconImg
            // 
            resources.ApplyResources(batteryIconImg, "batteryIconImg");
            batteryIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            batteryIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            batteryIconImg.Name = "batteryIconImg";
            batteryIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            batteryIconImg.TabStop = false;
            // 
            // lblFixedMandatory7
            // 
            resources.ApplyResources(lblFixedMandatory7, "lblFixedMandatory7");
            lblFixedMandatory7.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatory7.Name = "lblFixedMandatory7";
            // 
            // lblFixedMandatory6
            // 
            resources.ApplyResources(lblFixedMandatory6, "lblFixedMandatory6");
            lblFixedMandatory6.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatory6.Name = "lblFixedMandatory6";
            // 
            // lblFixedBattery
            // 
            resources.ApplyResources(lblFixedBattery, "lblFixedBattery");
            lblFixedBattery.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedBattery.Name = "lblFixedBattery";
            // 
            // lblFixedMandatory5
            // 
            resources.ApplyResources(lblFixedMandatory5, "lblFixedMandatory5");
            lblFixedMandatory5.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatory5.Name = "lblFixedMandatory5";
            // 
            // lblFixedMandatory4
            // 
            resources.ApplyResources(lblFixedMandatory4, "lblFixedMandatory4");
            lblFixedMandatory4.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatory4.Name = "lblFixedMandatory4";
            // 
            // lblFixedMandatory3
            // 
            resources.ApplyResources(lblFixedMandatory3, "lblFixedMandatory3");
            lblFixedMandatory3.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatory3.Name = "lblFixedMandatory3";
            // 
            // lblFixedMandatory2
            // 
            resources.ApplyResources(lblFixedMandatory2, "lblFixedMandatory2");
            lblFixedMandatory2.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatory2.Name = "lblFixedMandatory2";
            // 
            // lblFixedMandatory
            // 
            resources.ApplyResources(lblFixedMandatory, "lblFixedMandatory");
            lblFixedMandatory.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatory.Name = "lblFixedMandatory";
            // 
            // lblFixedMandatoryMain
            // 
            resources.ApplyResources(lblFixedMandatoryMain, "lblFixedMandatoryMain");
            lblFixedMandatoryMain.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryMain.Name = "lblFixedMandatoryMain";
            // 
            // studentRadioButton
            // 
            resources.ApplyResources(studentRadioButton, "studentRadioButton");
            studentRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            studentRadioButton.Name = "studentRadioButton";
            studentRadioButton.TabStop = true;
            studentRadioButton.UseVisualStyleBackColor = true;
            studentRadioButton.CheckedChanged += new System.EventHandler(StudentButton2_CheckedChanged);
            // 
            // employeeRadioButton
            // 
            resources.ApplyResources(employeeRadioButton, "employeeRadioButton");
            employeeRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            employeeRadioButton.Name = "employeeRadioButton";
            employeeRadioButton.TabStop = true;
            employeeRadioButton.UseVisualStyleBackColor = true;
            employeeRadioButton.CheckedChanged += new System.EventHandler(EmployeeButton1_CheckedChanged);
            // 
            // whoIconImg
            // 
            resources.ApplyResources(whoIconImg, "whoIconImg");
            whoIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            whoIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            whoIconImg.Name = "whoIconImg";
            whoIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            whoIconImg.TabStop = false;
            // 
            // lblFixedWho
            // 
            resources.ApplyResources(lblFixedWho, "lblFixedWho");
            lblFixedWho.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedWho.Name = "lblFixedWho";
            // 
            // letterIconImg
            // 
            resources.ApplyResources(letterIconImg, "letterIconImg");
            letterIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            letterIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            letterIconImg.Name = "letterIconImg";
            letterIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            letterIconImg.TabStop = false;
            // 
            // typeIconImg
            // 
            resources.ApplyResources(typeIconImg, "typeIconImg");
            typeIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            typeIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            typeIconImg.Name = "typeIconImg";
            typeIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            typeIconImg.TabStop = false;
            // 
            // tagIconImg
            // 
            resources.ApplyResources(tagIconImg, "tagIconImg");
            tagIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            tagIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            tagIconImg.Name = "tagIconImg";
            tagIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            tagIconImg.TabStop = false;
            // 
            // inUseIconImg
            // 
            resources.ApplyResources(inUseIconImg, "inUseIconImg");
            inUseIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            inUseIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            inUseIconImg.Name = "inUseIconImg";
            inUseIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            inUseIconImg.TabStop = false;
            // 
            // datetimeIconImg
            // 
            resources.ApplyResources(datetimeIconImg, "datetimeIconImg");
            datetimeIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            datetimeIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            datetimeIconImg.Name = "datetimeIconImg";
            datetimeIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            datetimeIconImg.TabStop = false;
            // 
            // standardIconImg
            // 
            resources.ApplyResources(standardIconImg, "standardIconImg");
            standardIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            standardIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            standardIconImg.Name = "standardIconImg";
            standardIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            standardIconImg.TabStop = false;
            // 
            // activeDirectoryIconImg
            // 
            resources.ApplyResources(activeDirectoryIconImg, "activeDirectoryIconImg");
            activeDirectoryIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            activeDirectoryIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            activeDirectoryIconImg.Name = "activeDirectoryIconImg";
            activeDirectoryIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            activeDirectoryIconImg.TabStop = false;
            // 
            // buildingIconImg
            // 
            resources.ApplyResources(buildingIconImg, "buildingIconImg");
            buildingIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            buildingIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            buildingIconImg.Name = "buildingIconImg";
            buildingIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            buildingIconImg.TabStop = false;
            // 
            // roomIconImg
            // 
            resources.ApplyResources(roomIconImg, "roomIconImg");
            roomIconImg.CompositingQuality = null;
            roomIconImg.InterpolationMode = null;
            roomIconImg.Name = "roomIconImg";
            roomIconImg.SmoothingMode = null;
            roomIconImg.TabStop = false;
            // 
            // sealIconImg
            // 
            resources.ApplyResources(sealIconImg, "sealIconImg");
            sealIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            sealIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            sealIconImg.Name = "sealIconImg";
            sealIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            sealIconImg.TabStop = false;
            // 
            // patrimonyIconImg
            // 
            resources.ApplyResources(patrimonyIconImg, "patrimonyIconImg");
            patrimonyIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            patrimonyIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            patrimonyIconImg.Name = "patrimonyIconImg";
            patrimonyIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            patrimonyIconImg.TabStop = false;
            // 
            // dateTimePicker1
            // 
            resources.ApplyResources(dateTimePicker1, "dateTimePicker1");
            dateTimePicker1.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            dateTimePicker1.Name = "dateTimePicker1";
            // 
            // groupBoxTypeOfService
            // 
            resources.ApplyResources(groupBoxTypeOfService, "groupBoxTypeOfService");
            groupBoxTypeOfService.Controls.Add(loadingCircle21);
            groupBoxTypeOfService.Controls.Add(loadingCircle20);
            groupBoxTypeOfService.Controls.Add(lblMaintenanceSince);
            groupBoxTypeOfService.Controls.Add(lblInstallSince);
            groupBoxTypeOfService.Controls.Add(lblFixedMandatory10);
            groupBoxTypeOfService.Controls.Add(textBoxFixedFormatRadio);
            groupBoxTypeOfService.Controls.Add(textBoxMaintenanceRadio);
            groupBoxTypeOfService.Controls.Add(formatRadioButton);
            groupBoxTypeOfService.Controls.Add(maintenanceRadioButton);
            groupBoxTypeOfService.ForeColor = System.Drawing.SystemColors.ControlText;
            groupBoxTypeOfService.Name = "groupBoxTypeOfService";
            groupBoxTypeOfService.TabStop = false;
            // 
            // loadingCircle21
            // 
            resources.ApplyResources(loadingCircle21, "loadingCircle21");
            loadingCircle21.Active = false;
            loadingCircle21.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle21.InnerCircleRadius = 5;
            loadingCircle21.Name = "loadingCircle21";
            loadingCircle21.NumberSpoke = 12;
            loadingCircle21.OuterCircleRadius = 11;
            loadingCircle21.RotationSpeed = 1;
            loadingCircle21.SpokeThickness = 2;
            loadingCircle21.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircle20
            // 
            resources.ApplyResources(loadingCircle20, "loadingCircle20");
            loadingCircle20.Active = false;
            loadingCircle20.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle20.InnerCircleRadius = 5;
            loadingCircle20.Name = "loadingCircle20";
            loadingCircle20.NumberSpoke = 12;
            loadingCircle20.OuterCircleRadius = 11;
            loadingCircle20.RotationSpeed = 1;
            loadingCircle20.SpokeThickness = 2;
            loadingCircle20.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // lblMaintenanceSince
            // 
            resources.ApplyResources(lblMaintenanceSince, "lblMaintenanceSince");
            lblMaintenanceSince.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            lblMaintenanceSince.Name = "lblMaintenanceSince";
            // 
            // lblInstallSince
            // 
            resources.ApplyResources(lblInstallSince, "lblInstallSince");
            lblInstallSince.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            lblInstallSince.Name = "lblInstallSince";
            // 
            // lblFixedMandatory10
            // 
            resources.ApplyResources(lblFixedMandatory10, "lblFixedMandatory10");
            lblFixedMandatory10.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatory10.Name = "lblFixedMandatory10";
            // 
            // textBoxFixedFormatRadio
            // 
            resources.ApplyResources(textBoxFixedFormatRadio, "textBoxFixedFormatRadio");
            textBoxFixedFormatRadio.BackColor = System.Drawing.SystemColors.Control;
            textBoxFixedFormatRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBoxFixedFormatRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxFixedFormatRadio.Name = "textBoxFixedFormatRadio";
            textBoxFixedFormatRadio.ReadOnly = true;
            // 
            // textBoxMaintenanceRadio
            // 
            resources.ApplyResources(textBoxMaintenanceRadio, "textBoxMaintenanceRadio");
            textBoxMaintenanceRadio.BackColor = System.Drawing.SystemColors.Control;
            textBoxMaintenanceRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBoxMaintenanceRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxMaintenanceRadio.Name = "textBoxMaintenanceRadio";
            textBoxMaintenanceRadio.ReadOnly = true;
            // 
            // formatRadioButton
            // 
            resources.ApplyResources(formatRadioButton, "formatRadioButton");
            formatRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            formatRadioButton.Name = "formatRadioButton";
            formatRadioButton.UseVisualStyleBackColor = true;
            formatRadioButton.CheckedChanged += new System.EventHandler(FormatButton1_CheckedChanged);
            // 
            // maintenanceRadioButton
            // 
            resources.ApplyResources(maintenanceRadioButton, "maintenanceRadioButton");
            maintenanceRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            maintenanceRadioButton.Name = "maintenanceRadioButton";
            maintenanceRadioButton.UseVisualStyleBackColor = true;
            maintenanceRadioButton.CheckedChanged += new System.EventHandler(MaintenanceButton2_CheckedChanged);
            // 
            // lblFixedActiveDirectory
            // 
            resources.ApplyResources(lblFixedActiveDirectory, "lblFixedActiveDirectory");
            lblFixedActiveDirectory.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedActiveDirectory.Name = "lblFixedActiveDirectory";
            // 
            // lblFixedStandard
            // 
            resources.ApplyResources(lblFixedStandard, "lblFixedStandard");
            lblFixedStandard.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedStandard.Name = "lblFixedStandard";
            // 
            // lblAgentName
            // 
            resources.ApplyResources(lblAgentName, "lblAgentName");
            lblAgentName.ForeColor = System.Drawing.SystemColors.ControlText;
            lblAgentName.Name = "lblAgentName";
            // 
            // lblFixedAgentName
            // 
            resources.ApplyResources(lblFixedAgentName, "lblFixedAgentName");
            lblFixedAgentName.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedAgentName.Name = "lblFixedAgentName";
            // 
            // lblPortServer
            // 
            resources.ApplyResources(lblPortServer, "lblPortServer");
            lblPortServer.ForeColor = System.Drawing.SystemColors.ControlText;
            lblPortServer.Name = "lblPortServer";
            // 
            // lblIPServer
            // 
            resources.ApplyResources(lblIPServer, "lblIPServer");
            lblIPServer.ForeColor = System.Drawing.SystemColors.ControlText;
            lblIPServer.Name = "lblIPServer";
            // 
            // lblFixedIPServer
            // 
            resources.ApplyResources(lblFixedIPServer, "lblFixedIPServer");
            lblFixedIPServer.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedIPServer.Name = "lblFixedIPServer";
            // 
            // lblServerOpState
            // 
            resources.ApplyResources(lblServerOpState, "lblServerOpState");
            lblServerOpState.BackColor = System.Drawing.Color.Transparent;
            lblServerOpState.ForeColor = System.Drawing.Color.Silver;
            lblServerOpState.Name = "lblServerOpState";
            // 
            // toolStripVersionText
            // 
            resources.ApplyResources(toolStripVersionText, "toolStripVersionText");
            toolStripVersionText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            toolStripVersionText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            toolStripVersionText.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripVersionText.Name = "toolStripVersionText";
            // 
            // statusStrip1
            // 
            resources.ApplyResources(statusStrip1, "statusStrip1");
            statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            comboBoxTheme,
            logLabel,
            aboutLabel,
            toolStripStatusBarText,
            toolStripVersionText});
            statusStrip1.Name = "statusStrip1";
            statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // comboBoxTheme
            // 
            resources.ApplyResources(comboBoxTheme, "comboBoxTheme");
            comboBoxTheme.BackColor = System.Drawing.SystemColors.Control;
            comboBoxTheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            comboBoxTheme.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripAutoTheme,
            toolStripLightTheme,
            toolStripDarkTheme});
            comboBoxTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            comboBoxTheme.Name = "comboBoxTheme";
            // 
            // toolStripAutoTheme
            // 
            resources.ApplyResources(toolStripAutoTheme, "toolStripAutoTheme");
            toolStripAutoTheme.BackColor = System.Drawing.SystemColors.Control;
            toolStripAutoTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripAutoTheme.Name = "toolStripAutoTheme";
            toolStripAutoTheme.Click += new System.EventHandler(ToolStripMenuItem1_Click);
            // 
            // toolStripLightTheme
            // 
            resources.ApplyResources(toolStripLightTheme, "toolStripLightTheme");
            toolStripLightTheme.BackColor = System.Drawing.SystemColors.Control;
            toolStripLightTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripLightTheme.Name = "toolStripLightTheme";
            toolStripLightTheme.Click += new System.EventHandler(ToolStripMenuItem2_Click);
            // 
            // toolStripDarkTheme
            // 
            resources.ApplyResources(toolStripDarkTheme, "toolStripDarkTheme");
            toolStripDarkTheme.BackColor = System.Drawing.SystemColors.Control;
            toolStripDarkTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripDarkTheme.Name = "toolStripDarkTheme";
            toolStripDarkTheme.Click += new System.EventHandler(ToolStripMenuItem3_Click);
            // 
            // logLabel
            // 
            resources.ApplyResources(logLabel, "logLabel");
            logLabel.BackColor = System.Drawing.SystemColors.Control;
            logLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            logLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            logLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            logLabel.Name = "logLabel";
            logLabel.Click += new System.EventHandler(LogLabel_Click);
            logLabel.MouseEnter += new System.EventHandler(LogLabel_MouseEnter);
            logLabel.MouseLeave += new System.EventHandler(LogLabel_MouseLeave);
            // 
            // aboutLabel
            // 
            resources.ApplyResources(aboutLabel, "aboutLabel");
            aboutLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            aboutLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            aboutLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            aboutLabel.Name = "aboutLabel";
            aboutLabel.Click += new System.EventHandler(AboutLabel_Click);
            aboutLabel.MouseEnter += new System.EventHandler(AboutLabel_MouseEnter);
            aboutLabel.MouseLeave += new System.EventHandler(AboutLabel_MouseLeave);
            // 
            // toolStripStatusBarText
            // 
            resources.ApplyResources(toolStripStatusBarText, "toolStripStatusBarText");
            toolStripStatusBarText.BackColor = System.Drawing.SystemColors.Control;
            toolStripStatusBarText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            toolStripStatusBarText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            toolStripStatusBarText.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripStatusBarText.Name = "toolStripStatusBarText";
            toolStripStatusBarText.Spring = true;
            // 
            // timer1
            // 
            timer1.Interval = 500;
            // 
            // groupBoxRegistryStatus
            // 
            resources.ApplyResources(groupBoxRegistryStatus, "groupBoxRegistryStatus");
            groupBoxRegistryStatus.Controls.Add(webView2Control);
            groupBoxRegistryStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            groupBoxRegistryStatus.Name = "groupBoxRegistryStatus";
            groupBoxRegistryStatus.TabStop = false;
            // 
            // webView2Control
            // 
            resources.ApplyResources(webView2Control, "webView2Control");
            webView2Control.AllowExternalDrop = true;
            webView2Control.CreationProperties = null;
            webView2Control.DefaultBackgroundColor = System.Drawing.Color.White;
            webView2Control.Name = "webView2Control";
            webView2Control.ZoomFactor = 1D;
            // 
            // topBannerImg
            // 
            resources.ApplyResources(topBannerImg, "topBannerImg");
            topBannerImg.CompositingQuality = null;
            topBannerImg.InterpolationMode = null;
            topBannerImg.Name = "topBannerImg";
            topBannerImg.SmoothingMode = null;
            topBannerImg.TabStop = false;
            // 
            // loadingCircle22
            // 
            resources.ApplyResources(loadingCircle22, "loadingCircle22");
            loadingCircle22.Active = false;
            loadingCircle22.BackColor = System.Drawing.SystemColors.Control;
            loadingCircle22.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle22.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle22.InnerCircleRadius = 5;
            loadingCircle22.Name = "loadingCircle22";
            loadingCircle22.NumberSpoke = 12;
            loadingCircle22.OuterCircleRadius = 11;
            loadingCircle22.RotationSpeed = 1;
            loadingCircle22.SpokeThickness = 2;
            loadingCircle22.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircle22.UseWaitCursor = true;
            // 
            // loadingCircle23
            // 
            resources.ApplyResources(loadingCircle23, "loadingCircle23");
            loadingCircle23.Active = false;
            loadingCircle23.BackColor = System.Drawing.SystemColors.Control;
            loadingCircle23.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle23.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircle23.InnerCircleRadius = 5;
            loadingCircle23.Name = "loadingCircle23";
            loadingCircle23.NumberSpoke = 12;
            loadingCircle23.OuterCircleRadius = 11;
            loadingCircle23.RotationSpeed = 1;
            loadingCircle23.SpokeThickness = 2;
            loadingCircle23.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // groupBoxServerStatus
            // 
            resources.ApplyResources(groupBoxServerStatus, "groupBoxServerStatus");
            groupBoxServerStatus.Controls.Add(loadingCircle24);
            groupBoxServerStatus.Controls.Add(lblFixedIPServer);
            groupBoxServerStatus.Controls.Add(lblFixedServerOpState);
            groupBoxServerStatus.Controls.Add(lblFixedPortServer);
            groupBoxServerStatus.Controls.Add(lblServerOpState);
            groupBoxServerStatus.Controls.Add(lblIPServer);
            groupBoxServerStatus.Controls.Add(lblPortServer);
            groupBoxServerStatus.Controls.Add(lblFixedAgentName);
            groupBoxServerStatus.Controls.Add(lblAgentName);
            groupBoxServerStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            groupBoxServerStatus.Name = "groupBoxServerStatus";
            groupBoxServerStatus.TabStop = false;
            // 
            // loadingCircle24
            // 
            resources.ApplyResources(loadingCircle24, "loadingCircle24");
            loadingCircle24.Active = false;
            loadingCircle24.Color = System.Drawing.Color.LightSlateGray;
            loadingCircle24.InnerCircleRadius = 5;
            loadingCircle24.Name = "loadingCircle24";
            loadingCircle24.NumberSpoke = 12;
            loadingCircle24.OuterCircleRadius = 11;
            loadingCircle24.RotationSpeed = 1;
            loadingCircle24.SpokeThickness = 2;
            loadingCircle24.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // comboBoxBattery
            // 
            resources.ApplyResources(comboBoxBattery, "comboBoxBattery");
            comboBoxBattery.BackColor = System.Drawing.SystemColors.Window;
            comboBoxBattery.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxBattery.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxBattery.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxBattery.FormattingEnabled = true;
            comboBoxBattery.Name = "comboBoxBattery";
            // 
            // comboBoxStandard
            // 
            resources.ApplyResources(comboBoxStandard, "comboBoxStandard");
            comboBoxStandard.BackColor = System.Drawing.SystemColors.Window;
            comboBoxStandard.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxStandard.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxStandard.FormattingEnabled = true;
            comboBoxStandard.Name = "comboBoxStandard";
            // 
            // comboBoxActiveDirectory
            // 
            resources.ApplyResources(comboBoxActiveDirectory, "comboBoxActiveDirectory");
            comboBoxActiveDirectory.BackColor = System.Drawing.SystemColors.Window;
            comboBoxActiveDirectory.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxActiveDirectory.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxActiveDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxActiveDirectory.FormattingEnabled = true;
            comboBoxActiveDirectory.Name = "comboBoxActiveDirectory";
            // 
            // comboBoxTag
            // 
            resources.ApplyResources(comboBoxTag, "comboBoxTag");
            comboBoxTag.BackColor = System.Drawing.SystemColors.Window;
            comboBoxTag.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxTag.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxTag.FormattingEnabled = true;
            comboBoxTag.Name = "comboBoxTag";
            // 
            // comboBoxInUse
            // 
            resources.ApplyResources(comboBoxInUse, "comboBoxInUse");
            comboBoxInUse.BackColor = System.Drawing.SystemColors.Window;
            comboBoxInUse.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxInUse.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxInUse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxInUse.FormattingEnabled = true;
            comboBoxInUse.Name = "comboBoxInUse";
            // 
            // comboBoxType
            // 
            resources.ApplyResources(comboBoxType, "comboBoxType");
            comboBoxType.BackColor = System.Drawing.SystemColors.Window;
            comboBoxType.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxType.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxType.FormattingEnabled = true;
            comboBoxType.Name = "comboBoxType";
            // 
            // comboBoxBuilding
            // 
            resources.ApplyResources(comboBoxBuilding, "comboBoxBuilding");
            comboBoxBuilding.BackColor = System.Drawing.SystemColors.Window;
            comboBoxBuilding.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxBuilding.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxBuilding.FormattingEnabled = true;
            comboBoxBuilding.Name = "comboBoxBuilding";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Control;
            Controls.Add(groupBoxServerStatus);
            Controls.Add(loadingCircle23);
            Controls.Add(loadingCircle22);
            Controls.Add(groupBoxRegistryStatus);
            Controls.Add(groupBoxPatrData);
            Controls.Add(groupBoxHWData);
            Controls.Add(topBannerImg);
            Controls.Add(accessSystemButton);
            Controls.Add(collectButton);
            Controls.Add(statusStrip1);
            Controls.Add(registerButton);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            Load += new System.EventHandler(Form1_Load);
            groupBoxHWData.ResumeLayout(false);
            groupBoxHWData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tpmIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)smartIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)vtIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)bmIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)secBootIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)biosIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)biosTypeIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)ipIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)macIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)hostnameIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)osIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)gpuInfoIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)mediaOperationIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)mediaTypeIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)hdSizeIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)pmIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)procNameIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)serialNoIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)modelIconImg).EndInit();
            groupBoxPatrData.ResumeLayout(false);
            groupBoxPatrData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ticketIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)batteryIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)whoIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)letterIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)typeIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)tagIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)inUseIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)datetimeIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)standardIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)activeDirectoryIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)buildingIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)roomIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)sealIconImg).EndInit();
            ((System.ComponentModel.ISupportInitialize)patrimonyIconImg).EndInit();
            groupBoxTypeOfService.ResumeLayout(false);
            groupBoxTypeOfService.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBoxRegistryStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webView2Control).EndInit();
            ((System.ComponentModel.ISupportInitialize)topBannerImg).EndInit();
            groupBoxServerStatus.ResumeLayout(false);
            groupBoxServerStatus.PerformLayout();
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
            modeURL = ConstantsDLL.Properties.Resources.formatURL;
        }

        //Sets service mode to maintenance
        private void MaintenanceButton2_CheckedChanged(object sender, EventArgs e)
        {
            modeURL = ConstantsDLL.Properties.Resources.maintenanceURL;
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
                if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10)) //If Windows 10/11
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark); //Sets context menus to dark
                }

                DarkTheme(); //Sets dark theme
            }
            else
            {
                if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10)) //If Windows 10/11
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Light); //Sets context menus to light
                }

                LightTheme(); //Sets light theme
            }
        }

        //Method for setting the auto theme via toolStrip 
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_AUTOTHEME_CHANGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            ComboBoxThemeInit();
        }

        //Method for setting the light theme via toolStrip
        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_LIGHTMODE_CHANGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
            {
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
            }

            LightTheme();
            themeBool = false;
        }

        //Method for setting the dark theme via toolStrip
        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_DARKMODE_CHANGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
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

            toolStripAutoTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_autotheme_light_path));
            toolStripLightTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_lighttheme_light_path));
            toolStripDarkTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_darktheme_light_path));

            comboBoxTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_autotheme_light_path));
            logLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_log_light_path));
            aboutLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_about_light_path));

            topBannerImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.main_banner_light_path));
            bmIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_brand_light_path));
            modelIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_model_light_path));
            serialNoIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_serial_no_light_path));
            procNameIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_cpu_light_path));
            pmIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ram_light_path));
            hdSizeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_disk_size_light_path));
            mediaTypeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_hdd_light_path));
            mediaOperationIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ahci_light_path));
            gpuInfoIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_gpu_light_path));
            osIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_windows_light_path));
            hostnameIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_hostname_light_path));
            macIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_mac_light_path));
            ipIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ip_light_path));
            biosTypeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_bios_light_path));
            biosIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_bios_version_light_path));
            secBootIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_secure_boot_light_path));
            patrimonyIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_patr_light_path));
            sealIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_seal_light_path));
            roomIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_room_light_path));
            buildingIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_building_light_path));
            activeDirectoryIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_server_light_path));
            standardIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_standard_light_path));
            datetimeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_service_light_path));
            letterIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_letter_light_path));
            inUseIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_in_use_light_path));
            tagIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_sticker_light_path));
            typeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_type_light_path));
            vtIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_VT_x_light_path));
            whoIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_who_light_path));
            smartIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_smart_light_path));
            tpmIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_tpm_light_path));
            batteryIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_cmos_battery_light_path));
            ticketIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ticket_light_path));
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

            toolStripAutoTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_autotheme_dark_path));
            toolStripLightTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_lighttheme_dark_path));
            toolStripDarkTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_darktheme_dark_path));

            comboBoxTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_autotheme_dark_path));
            logLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_log_dark_path));
            aboutLabel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_about_dark_path));

            topBannerImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.main_banner_dark_path));
            bmIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_brand_dark_path));
            modelIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_model_dark_path));
            serialNoIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_serial_no_dark_path));
            procNameIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_cpu_dark_path));
            pmIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ram_dark_path));
            hdSizeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_disk_size_dark_path));
            mediaTypeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_hdd_dark_path));
            mediaOperationIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ahci_dark_path));
            gpuInfoIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_gpu_dark_path));
            osIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_windows_dark_path));
            hostnameIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_hostname_dark_path));
            macIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_mac_dark_path));
            ipIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ip_dark_path));
            biosTypeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_bios_dark_path));
            biosIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_bios_version_dark_path));
            secBootIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_secure_boot_dark_path));
            patrimonyIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_patr_dark_path));
            sealIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_seal_dark_path));
            roomIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_room_dark_path));
            buildingIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_building_dark_path));
            activeDirectoryIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_server_dark_path));
            standardIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_standard_dark_path));
            datetimeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_service_dark_path));
            letterIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_letter_dark_path));
            inUseIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_in_use_dark_path));
            tagIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_sticker_dark_path));
            typeIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_type_dark_path));
            vtIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_VT_x_dark_path));
            whoIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_who_dark_path));
            smartIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_smart_dark_path));
            tpmIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_tpm_dark_path));
            batteryIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_cmos_battery_dark_path));
            ticketIconImg.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ticket_dark_path));
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
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_OPENING_LOG, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
#if DEBUG
            System.Diagnostics.Process.Start(defList[4][0] + ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.dev_status + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#else
            System.Diagnostics.Process.Start(defList[4][0] + ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#endif
        }

        //Opens the About box
        private void AboutLabel_Click(object sender, EventArgs e)
        {
            AboutBox aboutForm = new AboutBox(defList, themeBool);
            if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
            {
                DarkNet.Instance.SetWindowThemeForms(aboutForm, Theme.Auto);
            }

            _ = aboutForm.ShowDialog();
        }

        //Opens the selected webpage, according to the IP and port specified in the comboboxes
        private void AccessButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_VIEW_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            _ = System.Diagnostics.Process.Start("http://" + ip + ":" + port);
        }

        //Handles the closing of the current form
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CLOSING_MAINFORM, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_MISC), ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Deletes downloaded json files
            File.Delete(ConstantsDLL.Properties.Resources.biosPath);
            File.Delete(ConstantsDLL.Properties.Resources.loginPath);
            File.Delete(ConstantsDLL.Properties.Resources.pcPath);
            File.Delete(ConstantsDLL.Properties.Resources.configPath);

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
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle2.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle3.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle4.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle5.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle6.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle7.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle8.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle9.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle10.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle11.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle12.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle13.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle14.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle15.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle16.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle17.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle18.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle19.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle20.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle21.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle22.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle23.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle24.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle2.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle3.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle4.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle5.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle6.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle7.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle8.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle9.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle10.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle11.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle12.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle13.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle14.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle15.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle16.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle17.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle18.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle19.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle20.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle21.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle22.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle23.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle24.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle2.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle3.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle4.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle5.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle6.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle7.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle8.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle9.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle10.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle11.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle12.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle13.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle14.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle15.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle16.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle17.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle18.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle19.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle20.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle21.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle22.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle23.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle24.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle2.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle3.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle4.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle5.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle6.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle7.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle8.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle9.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle10.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle11.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle12.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle13.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle14.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle15.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle16.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle17.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle18.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle19.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle20.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle21.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle22.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle23.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircle24.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle2.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle3.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle4.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle5.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle6.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle7.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle8.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle9.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle10.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle11.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle12.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle13.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle14.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle15.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle16.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle17.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle18.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle19.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle20.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle21.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle22.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle23.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle24.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle2.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle3.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle4.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle5.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle6.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle7.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle8.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle9.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle10.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle11.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle12.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle13.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle14.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle15.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle16.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle17.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle18.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle19.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle20.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle21.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle22.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle23.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle24.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle2.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle3.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle4.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle5.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle6.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle7.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle8.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle9.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle10.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle11.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle12.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle13.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle14.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle15.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle16.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle17.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle18.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle19.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle20.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle21.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle22.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle23.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle24.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle2.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle3.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle4.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle5.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle6.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle7.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle8.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle9.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle10.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle11.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle12.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle13.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle14.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle15.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle16.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle17.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle18.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle19.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle20.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle21.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle22.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle23.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircle24.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle2.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle3.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle4.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle5.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle6.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle7.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle8.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle9.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle10.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle11.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle12.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle13.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle14.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle15.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle16.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle17.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle18.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle19.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle20.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle21.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle22.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle23.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle24.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle2.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle3.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle4.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle5.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle6.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle7.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle8.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle9.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle10.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle11.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle12.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle13.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle14.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle15.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle16.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle17.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle18.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle19.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle20.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle21.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle22.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle23.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle24.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle2.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle3.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle4.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle5.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle6.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle7.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle8.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle9.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle10.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle11.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle12.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle13.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle14.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle15.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle16.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle17.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle18.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle19.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle20.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle21.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle22.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle23.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle24.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle2.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle3.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle4.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle5.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle6.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle7.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle8.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle9.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle10.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle11.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle12.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle13.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle14.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle15.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle16.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle17.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle18.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle19.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle20.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle21.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle22.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle23.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircle24.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle2.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle3.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle4.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle5.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle6.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle7.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle8.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle9.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle10.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle11.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle12.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle13.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle14.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle15.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle16.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle17.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle18.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle19.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle20.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle21.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle22.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle23.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle24.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle2.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle3.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle4.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle5.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle6.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle7.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle8.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle9.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle10.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle11.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle12.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle13.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle14.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle15.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle16.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle17.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle18.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle19.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle20.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle21.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle22.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle23.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle24.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle2.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle3.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle4.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle5.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle6.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle7.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle8.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle9.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle10.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle11.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle12.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle13.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle14.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle15.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle16.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle17.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle18.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle19.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle20.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle21.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle22.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle23.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle24.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle2.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle3.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle4.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle5.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle6.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle7.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle8.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle9.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle10.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle11.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle12.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle13.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle14.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle15.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle16.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle17.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle18.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle19.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle20.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle21.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle22.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle23.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircle24.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle2.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle3.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle4.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle5.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle6.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle7.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle8.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle9.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle10.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle11.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle12.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle13.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle14.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle15.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle16.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle17.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle18.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle19.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle20.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle21.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle22.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle23.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle24.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle2.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle3.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle4.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle5.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle6.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle7.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle8.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle9.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle10.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle11.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle12.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle13.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle14.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle15.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle16.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle17.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle18.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle19.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle20.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle21.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle22.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle23.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle24.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle2.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle3.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle4.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle5.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle6.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle7.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle8.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle9.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle10.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle11.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle12.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle13.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle14.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle15.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle16.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle17.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle18.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle19.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle20.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle21.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle22.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle23.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle24.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle2.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle3.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle4.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle5.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle6.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle7.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle8.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle9.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle10.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle11.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle12.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle13.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle14.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle15.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle16.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle17.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle18.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle19.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle20.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle21.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle22.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle23.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircle24.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle2.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle3.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle4.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle5.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle6.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle7.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle8.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle9.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle10.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle11.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle12.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle13.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle14.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle15.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle16.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle17.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle18.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle19.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle20.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle21.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle22.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle23.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle24.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle2.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle3.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle4.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle5.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle6.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle7.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle8.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle9.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle10.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle11.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle12.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle13.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle14.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle15.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle16.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle17.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle18.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle19.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle20.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle21.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle22.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle23.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle24.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle2.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle3.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle4.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle5.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle6.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle7.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle8.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle9.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle10.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle11.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle12.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle13.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle14.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle15.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle16.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle17.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle18.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle19.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle20.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle21.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle22.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle23.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle24.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle2.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle3.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle4.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle5.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle6.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle7.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle8.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle9.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle10.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle11.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle12.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle13.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle14.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle15.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle16.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle17.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle18.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle19.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle20.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle21.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle22.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle23.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircle24.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle2.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle3.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle4.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle5.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle6.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle7.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle8.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle9.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle10.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle11.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle12.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle13.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle14.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle15.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle16.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle17.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle18.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle19.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle20.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle21.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle22.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle23.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle24.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle2.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle3.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle4.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle5.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle6.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle7.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle8.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle9.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle10.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle11.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle12.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle13.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle14.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle15.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle16.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle17.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle18.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle19.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle20.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle21.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle22.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle23.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle24.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle2.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle3.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle4.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle5.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle6.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle7.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle8.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle9.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle10.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle11.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle12.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle13.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle14.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle15.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle16.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle17.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle18.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle19.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle20.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle21.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle22.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle23.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle24.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle2.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle3.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle4.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle5.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle6.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle7.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle8.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle9.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle10.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle11.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle12.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle13.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle14.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle15.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle16.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle17.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle18.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle19.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle20.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle21.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle22.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle23.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircle24.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle2.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle3.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle4.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle5.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle6.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle7.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle8.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle9.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle10.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle11.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle12.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle13.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle14.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle15.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle16.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle17.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle18.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle19.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle20.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle21.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle22.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle23.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle24.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle2.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle3.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle4.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle5.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle6.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle7.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle8.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle9.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle10.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle11.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle12.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle13.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle14.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle15.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle16.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle17.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle18.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle19.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle20.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle21.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle22.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle23.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle24.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle2.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle3.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle4.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle5.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle6.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle7.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle8.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle9.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle10.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle11.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle12.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle13.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle14.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle15.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle16.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle17.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle18.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle19.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle20.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle21.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle22.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle23.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle24.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle2.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle3.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle4.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle5.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle6.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle7.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle8.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle9.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle10.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle11.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle12.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle13.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle14.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle15.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle16.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle17.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle18.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle19.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle20.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle21.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle22.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle23.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircle24.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle2.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle3.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle4.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle5.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle6.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle7.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle8.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle9.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle10.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle11.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle12.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle13.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle14.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle15.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle16.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle17.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle18.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle19.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle20.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle21.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle22.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle23.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle24.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle2.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle3.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle4.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle5.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle6.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle7.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle8.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle9.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle10.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle11.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle12.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle13.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle14.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle15.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle16.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle17.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle18.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle19.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle20.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle21.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle22.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle23.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle24.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle2.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle3.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle4.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle5.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle6.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle7.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle8.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle9.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle10.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle11.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle12.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle13.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle14.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle15.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle16.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle17.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle18.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle19.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle20.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle21.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle22.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle23.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle24.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle2.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle3.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle4.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle5.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle6.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle7.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle8.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle9.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle10.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle11.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle12.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle13.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle14.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle15.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle16.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle17.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle18.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle19.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle20.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle21.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle22.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle23.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircle24.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    break;
            }

            //Sets loading circle color and rotation speed
            #region
            loadingCircle1.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle2.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle3.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle4.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle5.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle6.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle7.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle8.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle9.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle10.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle11.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle12.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle13.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle14.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle15.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle16.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle17.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle18.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle19.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle20.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle21.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle22.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle23.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle24.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
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
            timer1.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timer2.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timer3.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timer4.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timer5.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timer6.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timer7.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timer8.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timer9.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timer10.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
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
            lblInstallSince.Text = ConstantsDLL.Properties.Resources.DASH;
            lblMaintenanceSince.Text = ConstantsDLL.Properties.Resources.DASH;
            lblBM.Text = ConstantsDLL.Properties.Resources.DASH;
            lblModel.Text = ConstantsDLL.Properties.Resources.DASH;
            lblSerialNo.Text = ConstantsDLL.Properties.Resources.DASH;
            lblProcName.Text = ConstantsDLL.Properties.Resources.DASH;
            lblPM.Text = ConstantsDLL.Properties.Resources.DASH;
            lblHDSize.Text = ConstantsDLL.Properties.Resources.DASH;
            lblSmart.Text = ConstantsDLL.Properties.Resources.DASH;
            lblMediaType.Text = ConstantsDLL.Properties.Resources.DASH;
            lblMediaOperation.Text = ConstantsDLL.Properties.Resources.DASH;
            lblGPUInfo.Text = ConstantsDLL.Properties.Resources.DASH;
            lblOS.Text = ConstantsDLL.Properties.Resources.DASH;
            lblHostname.Text = ConstantsDLL.Properties.Resources.DASH;
            lblMac.Text = ConstantsDLL.Properties.Resources.DASH;
            lblIP.Text = ConstantsDLL.Properties.Resources.DASH;
            lblBIOS.Text = ConstantsDLL.Properties.Resources.DASH;
            lblBIOSType.Text = ConstantsDLL.Properties.Resources.DASH;
            lblSecBoot.Text = ConstantsDLL.Properties.Resources.DASH;
            lblVT.Text = ConstantsDLL.Properties.Resources.DASH;
            lblTPM.Text = ConstantsDLL.Properties.Resources.DASH;
            collectButton.Text = ConstantsDLL.Properties.Resources.DASH;
            lblServerOpState.Text = ConstantsDLL.Properties.Resources.DASH;
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

                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PINGGING_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

                //Feches model info from server
                serverOnline = await BIOSFileReader.CheckHostMT(servidor_web, porta);

                if (serverOnline && porta != string.Empty)
                {
                    loadingCircle24.Visible = false;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_ONLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    lblServerOpState.Text = Strings.ONLINE;
                    lblServerOpState.ForeColor = StringsAndConstants.ONLINE_ALERT;
                }
                else
                {
                    loadingCircle24.Visible = false;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_OFFLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    lblServerOpState.Text = Strings.OFFLINE;
                    lblServerOpState.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                }
            }
            else
            {
                loadingCircle24.Visible = false;
                loadingCircle24.Active = false;
                lblIPServer.Text = lblPortServer.Text = lblAgentName.Text = lblServerOpState.Text = Strings.OFFLINE_MODE_ACTIVATED;
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
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_START_COLLECTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            i = 0;

            //Scans for PC maker
            BM = HardwareInfo.GetBoardMaker();
            if (BM == ConstantsDLL.Properties.Resources.ToBeFilledByOEM || BM == string.Empty)
            {
                BM = HardwareInfo.GetBoardMakerAlt();
            }

            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_BM, BM, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for PC model
            Model = HardwareInfo.GetModel();
            if (Model == ConstantsDLL.Properties.Resources.ToBeFilledByOEM || Model == string.Empty)
            {
                Model = HardwareInfo.GetModelAlt();
            }

            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MODEL, Model, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for motherboard Serial number
            SerialNo = HardwareInfo.GetBoardProductId();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SERIALNO, SerialNo, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for CPU information
            ProcName = HardwareInfo.GetProcessorCores();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PROCNAME, ProcName, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for RAM amount and total number of slots
            PM = HardwareInfo.GetPhysicalMemory() + " (" + HardwareInfo.GetNumFreeRamSlots(Convert.ToInt32(HardwareInfo.GetNumRamSlots())) +
                " slots de " + HardwareInfo.GetNumRamSlots() + " ocupados" + ")";
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PM, PM, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for Storage size
            HDSize = HardwareInfo.GetHDSize();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_HDSIZE, HDSize, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for SMART status
            Smart = HardwareInfo.GetSMARTStatus();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SMART, Smart, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for Storage type
            MediaType = HardwareInfo.GetStorageType();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MEDIATYPE, MediaType, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for Media Operation (IDE/AHCI/NVME)
            MediaOperation = HardwareInfo.GetStorageOperation();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MEDIAOP, MediaOperation, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for GPU information
            GPUInfo = HardwareInfo.GetGPUInfo();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_GPUINFO, GPUInfo, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for OS infomation
            OS = HardwareInfo.GetOSInformation();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_OS, OS, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for Hostname
            Hostname = HardwareInfo.GetComputerName();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_HOSTNAME, Hostname, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for MAC Address
            Mac = HardwareInfo.GetMACAddress();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MAC, Mac, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for IP Address
            IP = HardwareInfo.GetIPAddress();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_IP, IP, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for firmware type
            BIOSType = HardwareInfo.GetBIOSType();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_BIOSTYPE, BIOSType, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for Secure Boot status
            SecBoot = HardwareInfo.GetSecureBoot();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SECBOOT, SecBoot, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for BIOS version
            BIOS = HardwareInfo.GetComputerBIOS();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_BIOS, BIOS, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for VT status
            VT = HardwareInfo.GetVirtualizationTechnology();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_VT, VT, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for TPM status
            TPM = HardwareInfo.GetTPMStatus();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_TPM, TPM, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_END_COLLECTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
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

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), lblInstallSince.Text, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), lblMaintenanceSince.Text, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            if (!offlineMode)
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_FETCHING_BIOSFILE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            }

            try
            {
                //Feches model info from server
                string[] biosJsonStr = await BIOSFileReader.FetchInfoMT(lblBM.Text, lblModel.Text, lblBIOSType.Text, lblTPM.Text, lblMediaOperation.Text, ip, port);

                //Scan if hostname is the default one
                if (lblHostname.Text.Equals(Strings.DEFAULT_HOSTNAME))
                {
                    pass = false;
                    lblHostname.Text += Strings.HOSTNAME_ALERT;
                    timer1.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.HOSTNAME_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //If model Json file does exist and the Media Operation is incorrect
                if (biosJsonStr != null && biosJsonStr[3].Equals("false"))
                {
                    pass = false;
                    lblMediaOperation.Text += Strings.MEDIA_OPERATION_ALERT;
                    timer2.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.MEDIA_OPERATION_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //The section below contains the exception cases for Secure Boot enforcement
                if (lblSecBoot.Text.Equals(ConstantsDLL.Properties.Strings.deactivated) &&
                    !lblGPUInfo.Text.Contains(ConstantsDLL.Properties.Resources.nonSecBootGPU1) &&
                    !lblGPUInfo.Text.Contains(ConstantsDLL.Properties.Resources.nonSecBootGPU2))
                {
                    pass = false;
                    lblSecBoot.Text += Strings.SECURE_BOOT_ALERT;
                    timer3.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.SECURE_BOOT_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //If model Json file does not exist and server is unreachable
                if (biosJsonStr == null)
                {
                    if (!offlineMode)
                    {
                        pass = false;
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                        _ = MessageBox.Show(Strings.DATABASE_REACH_ERROR, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                //If model Json file does exist and BIOS/UEFI version is incorrect
                if (biosJsonStr != null && !lblBIOS.Text.Contains(biosJsonStr[0]))
                {
                    if (!biosJsonStr[0].Equals("-1"))
                    {
                        pass = false;
                        lblBIOS.Text += Strings.BIOS_VERSION_ALERT;
                        timer4.Enabled = true;
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.BIOS_VERSION_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    }
                }
                //If model Json file does exist and firmware type is incorrect
                if (biosJsonStr != null && biosJsonStr[1].Equals("false"))
                {
                    pass = false;
                    lblBIOSType.Text += Strings.FIRMWARE_TYPE_ALERT;
                    timer6.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.FIRMWARE_TYPE_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //If there is no MAC address assigned
                if (lblMac.Text == string.Empty)
                {
                    if (!offlineMode) //If it's not in offline mode
                    {
                        pass = false;
                        lblMac.Text = Strings.NETWORK_ERROR; //Prints a network error
                        lblIP.Text = Strings.NETWORK_ERROR; //Prints a network error
                        timer5.Enabled = true;
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.NETWORK_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    }
                    else //If it's in offline mode
                    {
                        lblMac.Text = Strings.OFFLINE_MODE_ACTIVATED; //Specifies offline mode
                        lblIP.Text = Strings.OFFLINE_MODE_ACTIVATED; //Specifies offline mode
                    }
                }
                //If Virtualization Technology is disabled
                if (lblVT.Text == ConstantsDLL.Properties.Strings.deactivated)
                {
                    pass = false;
                    lblVT.Text += Strings.VT_ALERT;
                    timer7.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.VT_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //If Smart status is no OK
                if (!lblSmart.Text.Contains(ConstantsDLL.Properties.Resources.ok))
                {
                    pass = false;
                    lblSmart.Text += Strings.SMART_FAIL;
                    timer8.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.SMART_FAIL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //If model Json file does exist and TPM is not enabled
                if (biosJsonStr != null && biosJsonStr[2].Equals("false"))
                {
                    pass = false;
                    lblTPM.Text += Strings.TPM_ERROR;
                    timer9.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.TPM_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetPhysicalMemoryAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, shows an alert
                if (d < 4.0 && Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    lblPM.Text += Strings.NOT_ENOUGH_MEMORY;
                    timer10.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.NOT_ENOUGH_MEMORY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //If RAM is more than 4GB and OS is x86, shows an alert
                if (d > 4.0 && !Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    lblPM.Text += Strings.TOO_MUCH_MEMORY;
                    timer10.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.TOO_MUCH_MEMORY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                if (pass && !offlineMode)
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_HARDWARE_PASSED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }

                if (!pass)
                {
                    progressBar1.SetState(2);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                }
            }
            catch (Exception e)
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), e.Message, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
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
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_START_THREAD, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
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
            collectButton.Text = Strings.FETCH_AGAIN; //Updates collect button text
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
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_START_LOADING_WEBVIEW2, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            CoreWebView2Environment webView2Environment = Environment.Is64BitOperatingSystem
                ? await CoreWebView2Environment.CreateAsync(ConstantsDLL.Properties.Resources.WEBVIEW2_SYSTEM_PATH_X64 + MiscMethods.GetWebView2Version(), Environment.GetEnvironmentVariable("TEMP"))
                : await CoreWebView2Environment.CreateAsync(ConstantsDLL.Properties.Resources.WEBVIEW2_SYSTEM_PATH_X86 + MiscMethods.GetWebView2Version(), Environment.GetEnvironmentVariable("TEMP"));
            await webView2Control.EnsureCoreWebView2Async(webView2Environment);
            webView2Control.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView2Control.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_END_LOADING_WEBVIEW2, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
        }

        //Sends hardware info to the specified server
        public void ServerSendInfo(string[] serverArgs)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTERING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            webView2Control.CoreWebView2.Navigate("http://" + serverArgs[0] + ":" + serverArgs[1] + "/" + serverArgs[2] + ".php?patrimonio=" + serverArgs[3] + "&lacre=" + serverArgs[4] + "&sala=" + serverArgs[5] + "&predio=" + serverArgs[6] + "&ad=" + serverArgs[7] + "&padrao=" + serverArgs[8] + "&formatacao=" + serverArgs[9] + "&formatacoesAnteriores=" + serverArgs[9] + "&marca=" + serverArgs[10] + "&modelo=" + serverArgs[11] + "&numeroSerial=" + serverArgs[12] + "&processador=" + serverArgs[13] + "&memoria=" + serverArgs[14] + "&hd=" + serverArgs[15] + "&sistemaOperacional=" + serverArgs[16] + "&nomeDoComputador=" + serverArgs[17] + "&bios=" + serverArgs[18] + "&mac=" + serverArgs[19] + "&ip=" + serverArgs[20] + "&emUso=" + serverArgs[21] + "&etiqueta=" + serverArgs[22] + "&tipo=" + serverArgs[23] + "&tipoFW=" + serverArgs[24] + "&tipoArmaz=" + serverArgs[25] + "&gpu=" + serverArgs[26] + "&modoArmaz=" + serverArgs[27] + "&secBoot=" + serverArgs[28] + "&vt=" + serverArgs[29] + "&tpm=" + serverArgs[30] + "&trocaPilha=" + serverArgs[31] + "&ticketNum=" + serverArgs[32] + "&agent=" + serverArgs[33]);
        }

        //Runs the registration for the website
        private async void Cadastra_ClickAsync(object sender, EventArgs e)
        {
            webView2Control.Visible = false;
            tbProgMain.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            loadingCircle23.Visible = true;
            loadingCircle23.Active = true;
            registerButton.Text = ConstantsDLL.Properties.Resources.DASH;
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
                sArgs[5] = textBoxLetter.Text != string.Empty ? textBoxRoom.Text + textBoxLetter.Text : textBoxRoom.Text;
                sArgs[6] = comboBoxBuilding.SelectedItem.ToString();
                sArgs[7] = comboBoxActiveDirectory.SelectedItem.ToString();
                sArgs[8] = comboBoxStandard.SelectedItem.ToString();
                sArgs[9] = dateTimePicker1.Value.ToString("yyyy-MM-dd").Substring(0, 10);
                sArgs[21] = comboBoxInUse.SelectedItem.ToString();
                sArgs[22] = comboBoxTag.SelectedItem.ToString();
                sArgs[23] = comboBoxType.SelectedItem.ToString();
                sArgs[31] = comboBoxBattery.SelectedItem.ToString().Equals("Sim") ? Strings.replacedBattery : Strings.sameBattery;
                sArgs[32] = textBoxTicket.Text;
                sArgs[33] = user;

                //Feches patrimony data from server
                string[] pcJsonStr = await PCFileReader.FetchInfoMT(sArgs[3], sArgs[0], sArgs[1]);

                //If patrinony is discarded
                if (pcJsonStr[0] != "false" && pcJsonStr[9] == "1")
                {
                    tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.PC_DROPPED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    _ = MessageBox.Show(ConstantsDLL.Properties.Strings.PC_DROPPED, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                }
                else //If not discarded
                {
                    if (serverOnline && porta != string.Empty) //If server is online and port is not null
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
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

                                if (formatRadioButton.Checked) //If the format radio button is checked
                                {
                                    MiscMethods.RegCreate(true, dateTimePicker1); //Create reg entries for format and maintenance
                                    lblInstallSince.Text = MiscMethods.SinceLabelUpdate(true);
                                    lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_INSTALLDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                }
                                else if (maintenanceRadioButton.Checked) //If the maintenance radio button is checked
                                {
                                    MiscMethods.RegCreate(false, dateTimePicker1); //Create reg entry just for maintenance
                                    lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                }
                                await Task.Delay(Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL) * 3);
                                tbProgMain.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                            }
                            else //If chosen date is before the last format/maintenance date of the PC, shows an error
                            {
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.INCORRECT_REGISTER_DATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                _ = MessageBox.Show(Strings.INCORRECT_REGISTER_DATE, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                                tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                            }
                        }
                        catch //If can't retrieve (patrimony non existent in the database), register normally
                        {
                            sArgs[9] = dateTimePicker1.Value.ToString().Substring(0, 10);
                            webView2Control.Visible = true;
                            ServerSendInfo(sArgs); //Send info to server
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

                            if (formatRadioButton.Checked) //If the format radio button is checked
                            {
                                MiscMethods.RegCreate(true, dateTimePicker1); //Create reg entries for format and maintenance
                                lblInstallSince.Text = MiscMethods.SinceLabelUpdate(true);
                                lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_INSTALLDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

                            }
                            else if (maintenanceRadioButton.Checked) //If the maintenance radio button is checked
                            {
                                MiscMethods.RegCreate(false, dateTimePicker1); //Create reg entry just for maintenance
                                lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

                            }
                            await Task.Delay(Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL) * 3);
                            tbProgMain.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                        }
                    }
                    else //If the server is out of reach
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.SERVER_NOT_FOUND_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                        _ = MessageBox.Show(ConstantsDLL.Properties.Strings.SERVER_NOT_FOUND_ERROR, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                        tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                    }
                }
            }
            else if (!pass) //If there are pendencies in the PC config
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.PENDENCY_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                _ = MessageBox.Show(Strings.PENDENCY_ERROR, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
            }
            else //If all fields are not filled
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.MANDATORY_FIELD, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                _ = MessageBox.Show(Strings.MANDATORY_FIELD, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
            }

            //When finished registering, resets control states
            loadingCircle23.Visible = false;
            loadingCircle23.Active = false;
            registerButton.Text = Strings.REGISTER_AGAIN;
            registerButton.Enabled = true;
            accessSystemButton.Enabled = true;
            collectButton.Enabled = true;
        }
    }
}

