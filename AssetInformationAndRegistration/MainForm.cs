using ConfigurableQualityPictureBoxDLL;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using AssetInformationAndRegistration.Properties;
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

namespace AssetInformationAndRegistration
{
    public partial class MainForm : Form
    {
        private int percent, i = 0;
        private bool themeBool, serverOnline, pass = true;
        private readonly bool offlineMode;
        private readonly string serverIP, serverPort;
        private string serviceTypeURL, brand, model, serialNumber, processor, ram, storageSize, storageType, mediaOperationMode, videoCard, operatingSystem, hostname, macAddress, ipAddress, fwVersion, fwType, secureBoot, virtualizationTechnology, smartStatus, tpmVersion;
        private readonly string[] serverArgs = new string[34], agentData = new string[2];
        private readonly List<string[]> parametersList, jsonServerSettings;
        private readonly List<string> enforcementList, orgDataList;

        //Form constructor
        public MainForm(bool offlineMode, string[] agentData, string serverIP, string serverPort, LogGenerator log, List<string[]> parametersList, List<string> enforcementList, List<string> orgDataList)
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
            if (StringsAndConstants.listThemeGUI.Contains(parametersList[3][0].ToString()) && parametersList[3][0].ToString().Equals(StringsAndConstants.listThemeGUI[0]))
            {
                themeBool = MiscMethods.ThemeInit();
                if (themeBool)
                {
                    if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.windows10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                    }
                    DarkTheme();
                }
                else
                {
                    if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.windows10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                    }
                    LightTheme();
                }
            }
            else if (parametersList[3][0].ToString().Equals(StringsAndConstants.listThemeGUI[1]))
            {
                comboBoxThemeButton.Enabled = false;
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.windows10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                }
                LightTheme();
                themeBool = false;
            }
            else if (parametersList[3][0].ToString().Equals(StringsAndConstants.listThemeGUI[2]))
            {
                comboBoxThemeButton.Enabled = false;
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.windows10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                }
                DarkTheme();
                themeBool = true;
            }

            this.serverIP = serverIP;
            this.serverPort = serverPort;
            this.log = log;
            this.offlineMode = offlineMode;
            this.parametersList = parametersList;
            this.enforcementList = enforcementList;
            this.orgDataList = orgDataList;
            this.agentData = agentData;

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_OFFLINE_MODE, offlineMode.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            if (!offlineMode)
            {
                //Fetch building and hw types info from the specified server
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_FETCHING_SERVER_DATA, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                jsonServerSettings = ConfigFileReader.FetchInfoST(serverIP, serverPort);
                parametersList[4] = jsonServerSettings[0]; //Buildings
                parametersList[5] = jsonServerSettings[1]; //Hw Types
                parametersList[6] = jsonServerSettings[2]; //Firmware Types
                parametersList[7] = jsonServerSettings[3]; //Tpm Types
                parametersList[8] = jsonServerSettings[4]; //Media Op Types
                parametersList[9] = jsonServerSettings[5]; //Secure Boot States
                parametersList[10] = jsonServerSettings[6]; //Virtualization Technology States
                comboBoxBuilding.Items.AddRange(parametersList[4]);
                comboBoxHwType.Items.AddRange(parametersList[5]);
            }
            else
            {
                //Fetch building and hw types info from the local file
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_FETCHING_LOCAL_DATA, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                jsonServerSettings = ConfigFileReader.GetOfflineModeConfigFile();
                parametersList[4] = jsonServerSettings[0];
                parametersList[5] = jsonServerSettings[1];
                parametersList[6] = jsonServerSettings[2];
                parametersList[7] = jsonServerSettings[3];
                parametersList[8] = jsonServerSettings[4];
                parametersList[9] = jsonServerSettings[5];
                parametersList[10] = jsonServerSettings[6];
                comboBoxBuilding.Items.AddRange(parametersList[4]);
                comboBoxHwType.Items.AddRange(parametersList[5]);
            }

            //Fills controls with provided info from ini file and constants dll
            comboBoxActiveDirectory.Items.AddRange(StringsAndConstants.listActiveDirectoryGUI.ToArray());
            comboBoxStandard.Items.AddRange(StringsAndConstants.listStandardGUI.ToArray());
            comboBoxInUse.Items.AddRange(StringsAndConstants.listInUseGUI.ToArray());
            comboBoxTag.Items.AddRange(StringsAndConstants.listTagGUI.ToArray());
            comboBoxBatteryChange.Items.AddRange(StringsAndConstants.listBatteryGUI.ToArray());
            textBoxAssetNumber.Text = HardwareInfo.GetHostname().Substring(0, 3).ToUpper().Equals(ConstantsDLL.Properties.Resources.HOSTNAME_PATTERN)
                ? HardwareInfo.GetHostname().Substring(3)
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
            for (int i = 0; i < this.orgDataList.Count; i++)
            {
                if (!this.orgDataList[i].Equals(string.Empty))
                {
                    oList[i] = this.orgDataList[i].ToString() + " - ";
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
            this.lblBrand = new System.Windows.Forms.Label();
            this.lblModel = new System.Windows.Forms.Label();
            this.lblSerialNumber = new System.Windows.Forms.Label();
            this.lblProcessor = new System.Windows.Forms.Label();
            this.lblRam = new System.Windows.Forms.Label();
            this.lblStorageSize = new System.Windows.Forms.Label();
            this.lblOperatingSystem = new System.Windows.Forms.Label();
            this.lblHostname = new System.Windows.Forms.Label();
            this.lblMacAddress = new System.Windows.Forms.Label();
            this.lblIpAddress = new System.Windows.Forms.Label();
            this.lblFixedBrand = new System.Windows.Forms.Label();
            this.lblFixedModel = new System.Windows.Forms.Label();
            this.lblFixedSerialNumber = new System.Windows.Forms.Label();
            this.lblFixedProcessor = new System.Windows.Forms.Label();
            this.lblFixedRam = new System.Windows.Forms.Label();
            this.lblFixedStorageSize = new System.Windows.Forms.Label();
            this.lblFixedOperatingSystem = new System.Windows.Forms.Label();
            this.lblFixedHostname = new System.Windows.Forms.Label();
            this.lblFixedMacAddress = new System.Windows.Forms.Label();
            this.lblFixedIpAddress = new System.Windows.Forms.Label();
            this.lblFixedAssetNumber = new System.Windows.Forms.Label();
            this.lblFixedSealNumber = new System.Windows.Forms.Label();
            this.lblFixedBuilding = new System.Windows.Forms.Label();
            this.textBoxAssetNumber = new System.Windows.Forms.TextBox();
            this.textBoxSealNumber = new System.Windows.Forms.TextBox();
            this.textBoxRoomNumber = new System.Windows.Forms.TextBox();
            this.textBoxRoomLetter = new System.Windows.Forms.TextBox();
            this.lblFixedRoomNumber = new System.Windows.Forms.Label();
            this.lblFixedServiceDate = new System.Windows.Forms.Label();
            this.registerButton = new System.Windows.Forms.Button();
            this.lblFixedInUse = new System.Windows.Forms.Label();
            this.lblFixedTag = new System.Windows.Forms.Label();
            this.lblFixedHwType = new System.Windows.Forms.Label();
            this.lblFixedServerOperationalStatus = new System.Windows.Forms.Label();
            this.lblFixedServerPort = new System.Windows.Forms.Label();
            this.collectButton = new System.Windows.Forms.Button();
            this.lblFixedRoomLetter = new System.Windows.Forms.Label();
            this.lblFixedFwVersion = new System.Windows.Forms.Label();
            this.lblFwVersion = new System.Windows.Forms.Label();
            this.ApcsButton = new System.Windows.Forms.Button();
            this.lblFixedFwType = new System.Windows.Forms.Label();
            this.lblFwType = new System.Windows.Forms.Label();
            this.groupBoxHwData = new System.Windows.Forms.GroupBox();
            this.loadingCircleTpmVersion = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleVirtualizationTechnology = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleSecureBoot = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleFwVersion = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleFwType = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleIpAddress = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleMacAddress = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleHostname = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleOperatingSystem = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleVideoCard = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleMediaOperationMode = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleStorageType = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleSmartStatus = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleStorageSize = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleRam = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleProcessor = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleSerialNumber = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleModel = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleBrand = new MRG.Controls.UI.LoadingCircle();
            this.separatorH = new System.Windows.Forms.Label();
            this.separatorV = new System.Windows.Forms.Label();
            this.iconImgTpmVersion = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgSmartStatus = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblSmartStatus = new System.Windows.Forms.Label();
            this.lblTpmVersion = new System.Windows.Forms.Label();
            this.lblFixedSmartStatus = new System.Windows.Forms.Label();
            this.iconImgVirtualizationTechnology = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblFixedTpmVersion = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblProgressBarPercent = new System.Windows.Forms.Label();
            this.lblVirtualizationTechnology = new System.Windows.Forms.Label();
            this.lblFixedVirtualizationTechnology = new System.Windows.Forms.Label();
            this.iconImgBrand = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgSecureBoot = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgFwVersion = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgFwType = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgIpAddress = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgMacAddress = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgHostname = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgOperatingSystem = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgVideoCard = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgMediaOperationMode = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgStorageType = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgStorageSize = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgRam = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgProcessor = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgSerialNumber = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgModel = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblSecureBoot = new System.Windows.Forms.Label();
            this.lblFixedSecureBoot = new System.Windows.Forms.Label();
            this.lblMediaOperationMode = new System.Windows.Forms.Label();
            this.lblFixedMediaOperationMode = new System.Windows.Forms.Label();
            this.lblVideoCard = new System.Windows.Forms.Label();
            this.lblFixedVideoCard = new System.Windows.Forms.Label();
            this.lblStorageType = new System.Windows.Forms.Label();
            this.lblFixedStorageType = new System.Windows.Forms.Label();
            this.groupBoxAssetData = new System.Windows.Forms.GroupBox();
            this.comboBoxBatteryChange = new CustomFlatComboBox();
            this.comboBoxStandard = new CustomFlatComboBox();
            this.comboBoxActiveDirectory = new CustomFlatComboBox();
            this.comboBoxTag = new CustomFlatComboBox();
            this.comboBoxInUse = new CustomFlatComboBox();
            this.comboBoxHwType = new CustomFlatComboBox();
            this.comboBoxBuilding = new CustomFlatComboBox();
            this.lblFixedMandatoryTicketNumber = new System.Windows.Forms.Label();
            this.lblFixedMandatoryBatteryChange = new System.Windows.Forms.Label();
            this.iconImgTicketNumber = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblFixedTicketNumber = new System.Windows.Forms.Label();
            this.textBoxTicketNumber = new System.Windows.Forms.TextBox();
            this.iconImgBatteryChange = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblFixedMandatoryWho = new System.Windows.Forms.Label();
            this.lblFixedMandatoryTag = new System.Windows.Forms.Label();
            this.lblFixedBatteryChange = new System.Windows.Forms.Label();
            this.lblFixedMandatoryHwType = new System.Windows.Forms.Label();
            this.lblFixedMandatoryInUse = new System.Windows.Forms.Label();
            this.lblFixedMandatoryBuilding = new System.Windows.Forms.Label();
            this.lblFixedMandatoryRoomNumber = new System.Windows.Forms.Label();
            this.lblFixedMandatoryAssetNumber = new System.Windows.Forms.Label();
            this.lblFixedMandatoryMain = new System.Windows.Forms.Label();
            this.radioButtonStudent = new System.Windows.Forms.RadioButton();
            this.radioButtonEmployee = new System.Windows.Forms.RadioButton();
            this.iconImgWho = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblFixedWho = new System.Windows.Forms.Label();
            this.iconImgRoomLetter = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgHwType = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgTag = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgInUse = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgServiceDate = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgStandard = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgAdRegistered = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgBuilding = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgRoomNumber = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgSealNumber = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgAssetNumber = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.dateTimePickerServiceDate = new System.Windows.Forms.DateTimePicker();
            this.groupBoxServiceType = new System.Windows.Forms.GroupBox();
            this.loadingCircleMaintenance = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleFormatting = new MRG.Controls.UI.LoadingCircle();
            this.lblMaintenanceSince = new System.Windows.Forms.Label();
            this.lblInstallSince = new System.Windows.Forms.Label();
            this.lblFixedMandatoryServiceType = new System.Windows.Forms.Label();
            this.textBoxFixedFormattingRadio = new System.Windows.Forms.TextBox();
            this.textBoxFixedMaintenanceRadio = new System.Windows.Forms.TextBox();
            this.radioButtonFormatting = new System.Windows.Forms.RadioButton();
            this.radioButtonMaintenance = new System.Windows.Forms.RadioButton();
            this.lblFixedAdRegistered = new System.Windows.Forms.Label();
            this.lblFixedStandard = new System.Windows.Forms.Label();
            this.lblAgentName = new System.Windows.Forms.Label();
            this.lblFixedAgentName = new System.Windows.Forms.Label();
            this.lblServerPort = new System.Windows.Forms.Label();
            this.lblServerIP = new System.Windows.Forms.Label();
            this.lblFixedServerIP = new System.Windows.Forms.Label();
            this.lblServerOperationalStatus = new System.Windows.Forms.Label();
            this.toolStripVersionText = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.comboBoxThemeButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripAutoTheme = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLightTheme = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDarkTheme = new System.Windows.Forms.ToolStripMenuItem();
            this.logLabelButton = new System.Windows.Forms.ToolStripStatusLabel();
            this.aboutLabelButton = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusBarText = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerAlertHostname = new System.Windows.Forms.Timer(this.components);
            this.timerAlertMediaOperationMode = new System.Windows.Forms.Timer(this.components);
            this.timerAlertSecureBoot = new System.Windows.Forms.Timer(this.components);
            this.timerAlertFwVersion = new System.Windows.Forms.Timer(this.components);
            this.timerAlertNetConnectivity = new System.Windows.Forms.Timer(this.components);
            this.timerAlertFwType = new System.Windows.Forms.Timer(this.components);
            this.timerAlertVirtualizationTechnology = new System.Windows.Forms.Timer(this.components);
            this.timerAlertSmartStatus = new System.Windows.Forms.Timer(this.components);
            this.groupBoxRegistryStatus = new System.Windows.Forms.GroupBox();
            this.webView2Control = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.timerAlertTpmVersion = new System.Windows.Forms.Timer(this.components);
            this.timerAlertRamAmount = new System.Windows.Forms.Timer(this.components);
            this.imgTopBanner = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.loadingCircleCollectButton = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleRegisterButton = new MRG.Controls.UI.LoadingCircle();
            this.groupBoxServerStatus = new System.Windows.Forms.GroupBox();
            this.loadingCircleServerOperationalStatus = new MRG.Controls.UI.LoadingCircle();
            this.groupBoxHwData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgTpmVersion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgSmartStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgVirtualizationTechnology)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgBrand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgSecureBoot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgFwVersion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgFwType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgIpAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgMacAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgHostname)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgOperatingSystem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgVideoCard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgMediaOperationMode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgStorageType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgStorageSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgRam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgProcessor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgSerialNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgModel)).BeginInit();
            this.groupBoxAssetData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgTicketNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgBatteryChange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgWho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgRoomLetter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgHwType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgTag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgInUse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgServiceDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgStandard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgAdRegistered)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgBuilding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgRoomNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgSealNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgAssetNumber)).BeginInit();
            this.groupBoxServiceType.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBoxRegistryStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2Control)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgTopBanner)).BeginInit();
            this.groupBoxServerStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblBrand
            // 
            resources.ApplyResources(this.lblBrand, "lblBrand");
            this.lblBrand.ForeColor = System.Drawing.Color.Silver;
            this.lblBrand.Name = "lblBrand";
            // 
            // lblModel
            // 
            resources.ApplyResources(this.lblModel, "lblModel");
            this.lblModel.ForeColor = System.Drawing.Color.Silver;
            this.lblModel.Name = "lblModel";
            // 
            // lblSerialNumber
            // 
            resources.ApplyResources(this.lblSerialNumber, "lblSerialNumber");
            this.lblSerialNumber.ForeColor = System.Drawing.Color.Silver;
            this.lblSerialNumber.Name = "lblSerialNumber";
            // 
            // lblProcessor
            // 
            resources.ApplyResources(this.lblProcessor, "lblProcessor");
            this.lblProcessor.ForeColor = System.Drawing.Color.Silver;
            this.lblProcessor.Name = "lblProcessor";
            // 
            // lblRam
            // 
            resources.ApplyResources(this.lblRam, "lblRam");
            this.lblRam.ForeColor = System.Drawing.Color.Silver;
            this.lblRam.Name = "lblRam";
            // 
            // lblStorageSize
            // 
            resources.ApplyResources(this.lblStorageSize, "lblStorageSize");
            this.lblStorageSize.ForeColor = System.Drawing.Color.Silver;
            this.lblStorageSize.Name = "lblStorageSize";
            // 
            // lblOperatingSystem
            // 
            resources.ApplyResources(this.lblOperatingSystem, "lblOperatingSystem");
            this.lblOperatingSystem.ForeColor = System.Drawing.Color.Silver;
            this.lblOperatingSystem.Name = "lblOperatingSystem";
            // 
            // lblHostname
            // 
            resources.ApplyResources(this.lblHostname, "lblHostname");
            this.lblHostname.ForeColor = System.Drawing.Color.Silver;
            this.lblHostname.Name = "lblHostname";
            // 
            // lblMacAddress
            // 
            resources.ApplyResources(this.lblMacAddress, "lblMacAddress");
            this.lblMacAddress.ForeColor = System.Drawing.Color.Silver;
            this.lblMacAddress.Name = "lblMacAddress";
            // 
            // lblIpAddress
            // 
            resources.ApplyResources(this.lblIpAddress, "lblIpAddress");
            this.lblIpAddress.ForeColor = System.Drawing.Color.Silver;
            this.lblIpAddress.Name = "lblIpAddress";
            // 
            // lblFixedBrand
            // 
            resources.ApplyResources(this.lblFixedBrand, "lblFixedBrand");
            this.lblFixedBrand.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedBrand.Name = "lblFixedBrand";
            // 
            // lblFixedModel
            // 
            resources.ApplyResources(this.lblFixedModel, "lblFixedModel");
            this.lblFixedModel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedModel.Name = "lblFixedModel";
            // 
            // lblFixedSerialNumber
            // 
            resources.ApplyResources(this.lblFixedSerialNumber, "lblFixedSerialNumber");
            this.lblFixedSerialNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedSerialNumber.Name = "lblFixedSerialNumber";
            // 
            // lblFixedProcessor
            // 
            resources.ApplyResources(this.lblFixedProcessor, "lblFixedProcessor");
            this.lblFixedProcessor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedProcessor.Name = "lblFixedProcessor";
            // 
            // lblFixedRam
            // 
            resources.ApplyResources(this.lblFixedRam, "lblFixedRam");
            this.lblFixedRam.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedRam.Name = "lblFixedRam";
            // 
            // lblFixedStorageSize
            // 
            resources.ApplyResources(this.lblFixedStorageSize, "lblFixedStorageSize");
            this.lblFixedStorageSize.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedStorageSize.Name = "lblFixedStorageSize";
            // 
            // lblFixedOperatingSystem
            // 
            resources.ApplyResources(this.lblFixedOperatingSystem, "lblFixedOperatingSystem");
            this.lblFixedOperatingSystem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedOperatingSystem.Name = "lblFixedOperatingSystem";
            // 
            // lblFixedHostname
            // 
            resources.ApplyResources(this.lblFixedHostname, "lblFixedHostname");
            this.lblFixedHostname.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedHostname.Name = "lblFixedHostname";
            // 
            // lblFixedMacAddress
            // 
            resources.ApplyResources(this.lblFixedMacAddress, "lblFixedMacAddress");
            this.lblFixedMacAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedMacAddress.Name = "lblFixedMacAddress";
            // 
            // lblFixedIpAddress
            // 
            resources.ApplyResources(this.lblFixedIpAddress, "lblFixedIpAddress");
            this.lblFixedIpAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedIpAddress.Name = "lblFixedIpAddress";
            // 
            // lblFixedAssetNumber
            // 
            resources.ApplyResources(this.lblFixedAssetNumber, "lblFixedAssetNumber");
            this.lblFixedAssetNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedAssetNumber.Name = "lblFixedAssetNumber";
            // 
            // lblFixedSealNumber
            // 
            resources.ApplyResources(this.lblFixedSealNumber, "lblFixedSealNumber");
            this.lblFixedSealNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedSealNumber.Name = "lblFixedSealNumber";
            // 
            // lblFixedBuilding
            // 
            resources.ApplyResources(this.lblFixedBuilding, "lblFixedBuilding");
            this.lblFixedBuilding.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedBuilding.Name = "lblFixedBuilding";
            // 
            // textBoxAssetNumber
            // 
            resources.ApplyResources(this.textBoxAssetNumber, "textBoxAssetNumber");
            this.textBoxAssetNumber.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxAssetNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxAssetNumber.Name = "textBoxAssetNumber";
            this.textBoxAssetNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxSealNumber
            // 
            resources.ApplyResources(this.textBoxSealNumber, "textBoxSealNumber");
            this.textBoxSealNumber.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxSealNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxSealNumber.Name = "textBoxSealNumber";
            this.textBoxSealNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxRoomNumber
            // 
            resources.ApplyResources(this.textBoxRoomNumber, "textBoxRoomNumber");
            this.textBoxRoomNumber.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxRoomNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxRoomNumber.Name = "textBoxRoomNumber";
            this.textBoxRoomNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxRoomLetter
            // 
            resources.ApplyResources(this.textBoxRoomLetter, "textBoxRoomLetter");
            this.textBoxRoomLetter.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxRoomLetter.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxRoomLetter.Name = "textBoxRoomLetter";
            this.textBoxRoomLetter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxCharsOnly_KeyPress);
            // 
            // lblFixedRoomNumber
            // 
            resources.ApplyResources(this.lblFixedRoomNumber, "lblFixedRoomNumber");
            this.lblFixedRoomNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedRoomNumber.Name = "lblFixedRoomNumber";
            // 
            // lblFixedServiceDate
            // 
            resources.ApplyResources(this.lblFixedServiceDate, "lblFixedServiceDate");
            this.lblFixedServiceDate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedServiceDate.Name = "lblFixedServiceDate";
            // 
            // registerButton
            // 
            resources.ApplyResources(this.registerButton, "registerButton");
            this.registerButton.BackColor = System.Drawing.SystemColors.Control;
            this.registerButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.registerButton.Name = "registerButton";
            this.registerButton.UseVisualStyleBackColor = true;
            this.registerButton.Click += new System.EventHandler(this.RegisterButton_ClickAsync);
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
            // lblFixedHwType
            // 
            resources.ApplyResources(this.lblFixedHwType, "lblFixedHwType");
            this.lblFixedHwType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedHwType.Name = "lblFixedHwType";
            // 
            // lblFixedServerOperationalStatus
            // 
            resources.ApplyResources(this.lblFixedServerOperationalStatus, "lblFixedServerOperationalStatus");
            this.lblFixedServerOperationalStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedServerOperationalStatus.Name = "lblFixedServerOperationalStatus";
            // 
            // lblFixedServerPort
            // 
            resources.ApplyResources(this.lblFixedServerPort, "lblFixedServerPort");
            this.lblFixedServerPort.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedServerPort.Name = "lblFixedServerPort";
            // 
            // collectButton
            // 
            resources.ApplyResources(this.collectButton, "collectButton");
            this.collectButton.BackColor = System.Drawing.SystemColors.Control;
            this.collectButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.collectButton.Name = "collectButton";
            this.collectButton.UseVisualStyleBackColor = true;
            this.collectButton.Click += new System.EventHandler(this.CollectButton_Click);
            // 
            // lblFixedRoomLetter
            // 
            resources.ApplyResources(this.lblFixedRoomLetter, "lblFixedRoomLetter");
            this.lblFixedRoomLetter.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedRoomLetter.Name = "lblFixedRoomLetter";
            // 
            // lblFixedFwVersion
            // 
            resources.ApplyResources(this.lblFixedFwVersion, "lblFixedFwVersion");
            this.lblFixedFwVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedFwVersion.Name = "lblFixedFwVersion";
            // 
            // lblFwVersion
            // 
            resources.ApplyResources(this.lblFwVersion, "lblFwVersion");
            this.lblFwVersion.ForeColor = System.Drawing.Color.Silver;
            this.lblFwVersion.Name = "lblFwVersion";
            // 
            // ApcsButton
            // 
            resources.ApplyResources(this.ApcsButton, "ApcsButton");
            this.ApcsButton.BackColor = System.Drawing.SystemColors.Control;
            this.ApcsButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ApcsButton.Name = "ApcsButton";
            this.ApcsButton.UseVisualStyleBackColor = true;
            this.ApcsButton.Click += new System.EventHandler(this.ApcsButton_Click);
            // 
            // lblFixedFwType
            // 
            resources.ApplyResources(this.lblFixedFwType, "lblFixedFwType");
            this.lblFixedFwType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedFwType.Name = "lblFixedFwType";
            // 
            // lblFwType
            // 
            resources.ApplyResources(this.lblFwType, "lblFwType");
            this.lblFwType.ForeColor = System.Drawing.Color.Silver;
            this.lblFwType.Name = "lblFwType";
            // 
            // groupBoxHwData
            // 
            resources.ApplyResources(this.groupBoxHwData, "groupBoxHwData");
            this.groupBoxHwData.Controls.Add(this.loadingCircleTpmVersion);
            this.groupBoxHwData.Controls.Add(this.loadingCircleVirtualizationTechnology);
            this.groupBoxHwData.Controls.Add(this.loadingCircleSecureBoot);
            this.groupBoxHwData.Controls.Add(this.loadingCircleFwVersion);
            this.groupBoxHwData.Controls.Add(this.loadingCircleFwType);
            this.groupBoxHwData.Controls.Add(this.loadingCircleIpAddress);
            this.groupBoxHwData.Controls.Add(this.loadingCircleMacAddress);
            this.groupBoxHwData.Controls.Add(this.loadingCircleHostname);
            this.groupBoxHwData.Controls.Add(this.loadingCircleOperatingSystem);
            this.groupBoxHwData.Controls.Add(this.loadingCircleVideoCard);
            this.groupBoxHwData.Controls.Add(this.loadingCircleMediaOperationMode);
            this.groupBoxHwData.Controls.Add(this.loadingCircleStorageType);
            this.groupBoxHwData.Controls.Add(this.loadingCircleSmartStatus);
            this.groupBoxHwData.Controls.Add(this.loadingCircleStorageSize);
            this.groupBoxHwData.Controls.Add(this.loadingCircleRam);
            this.groupBoxHwData.Controls.Add(this.loadingCircleProcessor);
            this.groupBoxHwData.Controls.Add(this.loadingCircleSerialNumber);
            this.groupBoxHwData.Controls.Add(this.loadingCircleModel);
            this.groupBoxHwData.Controls.Add(this.loadingCircleBrand);
            this.groupBoxHwData.Controls.Add(this.separatorH);
            this.groupBoxHwData.Controls.Add(this.separatorV);
            this.groupBoxHwData.Controls.Add(this.iconImgTpmVersion);
            this.groupBoxHwData.Controls.Add(this.iconImgSmartStatus);
            this.groupBoxHwData.Controls.Add(this.lblSmartStatus);
            this.groupBoxHwData.Controls.Add(this.lblTpmVersion);
            this.groupBoxHwData.Controls.Add(this.lblFixedSmartStatus);
            this.groupBoxHwData.Controls.Add(this.iconImgVirtualizationTechnology);
            this.groupBoxHwData.Controls.Add(this.lblFixedTpmVersion);
            this.groupBoxHwData.Controls.Add(this.progressBar1);
            this.groupBoxHwData.Controls.Add(this.lblProgressBarPercent);
            this.groupBoxHwData.Controls.Add(this.lblVirtualizationTechnology);
            this.groupBoxHwData.Controls.Add(this.lblFixedVirtualizationTechnology);
            this.groupBoxHwData.Controls.Add(this.iconImgBrand);
            this.groupBoxHwData.Controls.Add(this.iconImgSecureBoot);
            this.groupBoxHwData.Controls.Add(this.iconImgFwVersion);
            this.groupBoxHwData.Controls.Add(this.iconImgFwType);
            this.groupBoxHwData.Controls.Add(this.iconImgIpAddress);
            this.groupBoxHwData.Controls.Add(this.iconImgMacAddress);
            this.groupBoxHwData.Controls.Add(this.iconImgHostname);
            this.groupBoxHwData.Controls.Add(this.iconImgOperatingSystem);
            this.groupBoxHwData.Controls.Add(this.iconImgVideoCard);
            this.groupBoxHwData.Controls.Add(this.iconImgMediaOperationMode);
            this.groupBoxHwData.Controls.Add(this.iconImgStorageType);
            this.groupBoxHwData.Controls.Add(this.iconImgStorageSize);
            this.groupBoxHwData.Controls.Add(this.iconImgRam);
            this.groupBoxHwData.Controls.Add(this.iconImgProcessor);
            this.groupBoxHwData.Controls.Add(this.iconImgSerialNumber);
            this.groupBoxHwData.Controls.Add(this.iconImgModel);
            this.groupBoxHwData.Controls.Add(this.lblSecureBoot);
            this.groupBoxHwData.Controls.Add(this.lblFixedSecureBoot);
            this.groupBoxHwData.Controls.Add(this.lblMediaOperationMode);
            this.groupBoxHwData.Controls.Add(this.lblFixedMediaOperationMode);
            this.groupBoxHwData.Controls.Add(this.lblVideoCard);
            this.groupBoxHwData.Controls.Add(this.lblFixedVideoCard);
            this.groupBoxHwData.Controls.Add(this.lblStorageType);
            this.groupBoxHwData.Controls.Add(this.lblFixedStorageType);
            this.groupBoxHwData.Controls.Add(this.lblFixedBrand);
            this.groupBoxHwData.Controls.Add(this.lblOperatingSystem);
            this.groupBoxHwData.Controls.Add(this.lblFwType);
            this.groupBoxHwData.Controls.Add(this.lblStorageSize);
            this.groupBoxHwData.Controls.Add(this.lblFixedFwType);
            this.groupBoxHwData.Controls.Add(this.lblRam);
            this.groupBoxHwData.Controls.Add(this.lblProcessor);
            this.groupBoxHwData.Controls.Add(this.lblSerialNumber);
            this.groupBoxHwData.Controls.Add(this.lblFwVersion);
            this.groupBoxHwData.Controls.Add(this.lblModel);
            this.groupBoxHwData.Controls.Add(this.lblFixedFwVersion);
            this.groupBoxHwData.Controls.Add(this.lblBrand);
            this.groupBoxHwData.Controls.Add(this.lblHostname);
            this.groupBoxHwData.Controls.Add(this.lblMacAddress);
            this.groupBoxHwData.Controls.Add(this.lblIpAddress);
            this.groupBoxHwData.Controls.Add(this.lblFixedModel);
            this.groupBoxHwData.Controls.Add(this.lblFixedSerialNumber);
            this.groupBoxHwData.Controls.Add(this.lblFixedProcessor);
            this.groupBoxHwData.Controls.Add(this.lblFixedRam);
            this.groupBoxHwData.Controls.Add(this.lblFixedStorageSize);
            this.groupBoxHwData.Controls.Add(this.lblFixedOperatingSystem);
            this.groupBoxHwData.Controls.Add(this.lblFixedHostname);
            this.groupBoxHwData.Controls.Add(this.lblFixedMacAddress);
            this.groupBoxHwData.Controls.Add(this.lblFixedIpAddress);
            this.groupBoxHwData.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBoxHwData.Name = "groupBoxHwData";
            this.groupBoxHwData.TabStop = false;
            // 
            // loadingCircleTpmVersion
            // 
            resources.ApplyResources(this.loadingCircleTpmVersion, "loadingCircleTpmVersion");
            this.loadingCircleTpmVersion.Active = false;
            this.loadingCircleTpmVersion.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleTpmVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleTpmVersion.InnerCircleRadius = 5;
            this.loadingCircleTpmVersion.Name = "loadingCircleTpmVersion";
            this.loadingCircleTpmVersion.NumberSpoke = 12;
            this.loadingCircleTpmVersion.OuterCircleRadius = 11;
            this.loadingCircleTpmVersion.RotationSpeed = 1;
            this.loadingCircleTpmVersion.SpokeThickness = 2;
            this.loadingCircleTpmVersion.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleVirtualizationTechnology
            // 
            resources.ApplyResources(this.loadingCircleVirtualizationTechnology, "loadingCircleVirtualizationTechnology");
            this.loadingCircleVirtualizationTechnology.Active = false;
            this.loadingCircleVirtualizationTechnology.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleVirtualizationTechnology.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleVirtualizationTechnology.InnerCircleRadius = 5;
            this.loadingCircleVirtualizationTechnology.Name = "loadingCircleVirtualizationTechnology";
            this.loadingCircleVirtualizationTechnology.NumberSpoke = 12;
            this.loadingCircleVirtualizationTechnology.OuterCircleRadius = 11;
            this.loadingCircleVirtualizationTechnology.RotationSpeed = 1;
            this.loadingCircleVirtualizationTechnology.SpokeThickness = 2;
            this.loadingCircleVirtualizationTechnology.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleSecureBoot
            // 
            resources.ApplyResources(this.loadingCircleSecureBoot, "loadingCircleSecureBoot");
            this.loadingCircleSecureBoot.Active = false;
            this.loadingCircleSecureBoot.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleSecureBoot.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleSecureBoot.InnerCircleRadius = 5;
            this.loadingCircleSecureBoot.Name = "loadingCircleSecureBoot";
            this.loadingCircleSecureBoot.NumberSpoke = 12;
            this.loadingCircleSecureBoot.OuterCircleRadius = 11;
            this.loadingCircleSecureBoot.RotationSpeed = 1;
            this.loadingCircleSecureBoot.SpokeThickness = 2;
            this.loadingCircleSecureBoot.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleFwVersion
            // 
            resources.ApplyResources(this.loadingCircleFwVersion, "loadingCircleFwVersion");
            this.loadingCircleFwVersion.Active = false;
            this.loadingCircleFwVersion.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleFwVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleFwVersion.InnerCircleRadius = 5;
            this.loadingCircleFwVersion.Name = "loadingCircleFwVersion";
            this.loadingCircleFwVersion.NumberSpoke = 12;
            this.loadingCircleFwVersion.OuterCircleRadius = 11;
            this.loadingCircleFwVersion.RotationSpeed = 1;
            this.loadingCircleFwVersion.SpokeThickness = 2;
            this.loadingCircleFwVersion.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleFwType
            // 
            resources.ApplyResources(this.loadingCircleFwType, "loadingCircleFwType");
            this.loadingCircleFwType.Active = false;
            this.loadingCircleFwType.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleFwType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleFwType.InnerCircleRadius = 5;
            this.loadingCircleFwType.Name = "loadingCircleFwType";
            this.loadingCircleFwType.NumberSpoke = 12;
            this.loadingCircleFwType.OuterCircleRadius = 11;
            this.loadingCircleFwType.RotationSpeed = 1;
            this.loadingCircleFwType.SpokeThickness = 2;
            this.loadingCircleFwType.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleIpAddress
            // 
            resources.ApplyResources(this.loadingCircleIpAddress, "loadingCircleIpAddress");
            this.loadingCircleIpAddress.Active = false;
            this.loadingCircleIpAddress.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleIpAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleIpAddress.InnerCircleRadius = 5;
            this.loadingCircleIpAddress.Name = "loadingCircleIpAddress";
            this.loadingCircleIpAddress.NumberSpoke = 12;
            this.loadingCircleIpAddress.OuterCircleRadius = 11;
            this.loadingCircleIpAddress.RotationSpeed = 1;
            this.loadingCircleIpAddress.SpokeThickness = 2;
            this.loadingCircleIpAddress.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleMacAddress
            // 
            resources.ApplyResources(this.loadingCircleMacAddress, "loadingCircleMacAddress");
            this.loadingCircleMacAddress.Active = false;
            this.loadingCircleMacAddress.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleMacAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleMacAddress.InnerCircleRadius = 5;
            this.loadingCircleMacAddress.Name = "loadingCircleMacAddress";
            this.loadingCircleMacAddress.NumberSpoke = 12;
            this.loadingCircleMacAddress.OuterCircleRadius = 11;
            this.loadingCircleMacAddress.RotationSpeed = 1;
            this.loadingCircleMacAddress.SpokeThickness = 2;
            this.loadingCircleMacAddress.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleHostname
            // 
            resources.ApplyResources(this.loadingCircleHostname, "loadingCircleHostname");
            this.loadingCircleHostname.Active = false;
            this.loadingCircleHostname.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleHostname.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleHostname.InnerCircleRadius = 5;
            this.loadingCircleHostname.Name = "loadingCircleHostname";
            this.loadingCircleHostname.NumberSpoke = 12;
            this.loadingCircleHostname.OuterCircleRadius = 11;
            this.loadingCircleHostname.RotationSpeed = 1;
            this.loadingCircleHostname.SpokeThickness = 2;
            this.loadingCircleHostname.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleOperatingSystem
            // 
            resources.ApplyResources(this.loadingCircleOperatingSystem, "loadingCircleOperatingSystem");
            this.loadingCircleOperatingSystem.Active = false;
            this.loadingCircleOperatingSystem.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleOperatingSystem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleOperatingSystem.InnerCircleRadius = 5;
            this.loadingCircleOperatingSystem.Name = "loadingCircleOperatingSystem";
            this.loadingCircleOperatingSystem.NumberSpoke = 12;
            this.loadingCircleOperatingSystem.OuterCircleRadius = 11;
            this.loadingCircleOperatingSystem.RotationSpeed = 1;
            this.loadingCircleOperatingSystem.SpokeThickness = 2;
            this.loadingCircleOperatingSystem.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleVideoCard
            // 
            resources.ApplyResources(this.loadingCircleVideoCard, "loadingCircleVideoCard");
            this.loadingCircleVideoCard.Active = false;
            this.loadingCircleVideoCard.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleVideoCard.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleVideoCard.InnerCircleRadius = 5;
            this.loadingCircleVideoCard.Name = "loadingCircleVideoCard";
            this.loadingCircleVideoCard.NumberSpoke = 12;
            this.loadingCircleVideoCard.OuterCircleRadius = 11;
            this.loadingCircleVideoCard.RotationSpeed = 1;
            this.loadingCircleVideoCard.SpokeThickness = 2;
            this.loadingCircleVideoCard.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleMediaOperationMode
            // 
            resources.ApplyResources(this.loadingCircleMediaOperationMode, "loadingCircleMediaOperationMode");
            this.loadingCircleMediaOperationMode.Active = false;
            this.loadingCircleMediaOperationMode.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleMediaOperationMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleMediaOperationMode.InnerCircleRadius = 5;
            this.loadingCircleMediaOperationMode.Name = "loadingCircleMediaOperationMode";
            this.loadingCircleMediaOperationMode.NumberSpoke = 12;
            this.loadingCircleMediaOperationMode.OuterCircleRadius = 11;
            this.loadingCircleMediaOperationMode.RotationSpeed = 1;
            this.loadingCircleMediaOperationMode.SpokeThickness = 2;
            this.loadingCircleMediaOperationMode.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleStorageType
            // 
            resources.ApplyResources(this.loadingCircleStorageType, "loadingCircleStorageType");
            this.loadingCircleStorageType.Active = false;
            this.loadingCircleStorageType.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleStorageType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleStorageType.InnerCircleRadius = 5;
            this.loadingCircleStorageType.Name = "loadingCircleStorageType";
            this.loadingCircleStorageType.NumberSpoke = 12;
            this.loadingCircleStorageType.OuterCircleRadius = 11;
            this.loadingCircleStorageType.RotationSpeed = 1;
            this.loadingCircleStorageType.SpokeThickness = 2;
            this.loadingCircleStorageType.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleSmartStatus
            // 
            resources.ApplyResources(this.loadingCircleSmartStatus, "loadingCircleSmartStatus");
            this.loadingCircleSmartStatus.Active = false;
            this.loadingCircleSmartStatus.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleSmartStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleSmartStatus.InnerCircleRadius = 5;
            this.loadingCircleSmartStatus.Name = "loadingCircleSmartStatus";
            this.loadingCircleSmartStatus.NumberSpoke = 12;
            this.loadingCircleSmartStatus.OuterCircleRadius = 11;
            this.loadingCircleSmartStatus.RotationSpeed = 1;
            this.loadingCircleSmartStatus.SpokeThickness = 2;
            this.loadingCircleSmartStatus.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleStorageSize
            // 
            resources.ApplyResources(this.loadingCircleStorageSize, "loadingCircleStorageSize");
            this.loadingCircleStorageSize.Active = false;
            this.loadingCircleStorageSize.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleStorageSize.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleStorageSize.InnerCircleRadius = 5;
            this.loadingCircleStorageSize.Name = "loadingCircleStorageSize";
            this.loadingCircleStorageSize.NumberSpoke = 12;
            this.loadingCircleStorageSize.OuterCircleRadius = 11;
            this.loadingCircleStorageSize.RotationSpeed = 1;
            this.loadingCircleStorageSize.SpokeThickness = 2;
            this.loadingCircleStorageSize.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleRam
            // 
            resources.ApplyResources(this.loadingCircleRam, "loadingCircleRam");
            this.loadingCircleRam.Active = false;
            this.loadingCircleRam.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleRam.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleRam.InnerCircleRadius = 5;
            this.loadingCircleRam.Name = "loadingCircleRam";
            this.loadingCircleRam.NumberSpoke = 12;
            this.loadingCircleRam.OuterCircleRadius = 11;
            this.loadingCircleRam.RotationSpeed = 1;
            this.loadingCircleRam.SpokeThickness = 2;
            this.loadingCircleRam.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleProcessor
            // 
            resources.ApplyResources(this.loadingCircleProcessor, "loadingCircleProcessor");
            this.loadingCircleProcessor.Active = false;
            this.loadingCircleProcessor.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleProcessor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleProcessor.InnerCircleRadius = 5;
            this.loadingCircleProcessor.Name = "loadingCircleProcessor";
            this.loadingCircleProcessor.NumberSpoke = 12;
            this.loadingCircleProcessor.OuterCircleRadius = 11;
            this.loadingCircleProcessor.RotationSpeed = 1;
            this.loadingCircleProcessor.SpokeThickness = 2;
            this.loadingCircleProcessor.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleSerialNumber
            // 
            resources.ApplyResources(this.loadingCircleSerialNumber, "loadingCircleSerialNumber");
            this.loadingCircleSerialNumber.Active = false;
            this.loadingCircleSerialNumber.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleSerialNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleSerialNumber.InnerCircleRadius = 5;
            this.loadingCircleSerialNumber.Name = "loadingCircleSerialNumber";
            this.loadingCircleSerialNumber.NumberSpoke = 12;
            this.loadingCircleSerialNumber.OuterCircleRadius = 11;
            this.loadingCircleSerialNumber.RotationSpeed = 1;
            this.loadingCircleSerialNumber.SpokeThickness = 2;
            this.loadingCircleSerialNumber.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleModel
            // 
            resources.ApplyResources(this.loadingCircleModel, "loadingCircleModel");
            this.loadingCircleModel.Active = false;
            this.loadingCircleModel.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleModel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleModel.InnerCircleRadius = 5;
            this.loadingCircleModel.Name = "loadingCircleModel";
            this.loadingCircleModel.NumberSpoke = 12;
            this.loadingCircleModel.OuterCircleRadius = 11;
            this.loadingCircleModel.RotationSpeed = 1;
            this.loadingCircleModel.SpokeThickness = 2;
            this.loadingCircleModel.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleBrand
            // 
            resources.ApplyResources(this.loadingCircleBrand, "loadingCircleBrand");
            this.loadingCircleBrand.Active = false;
            this.loadingCircleBrand.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleBrand.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleBrand.InnerCircleRadius = 5;
            this.loadingCircleBrand.Name = "loadingCircleBrand";
            this.loadingCircleBrand.NumberSpoke = 12;
            this.loadingCircleBrand.OuterCircleRadius = 11;
            this.loadingCircleBrand.RotationSpeed = 1;
            this.loadingCircleBrand.SpokeThickness = 2;
            this.loadingCircleBrand.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // separatorH
            // 
            resources.ApplyResources(this.separatorH, "separatorH");
            this.separatorH.BackColor = System.Drawing.Color.DimGray;
            this.separatorH.Name = "separatorH";
            // 
            // separatorV
            // 
            resources.ApplyResources(this.separatorV, "separatorV");
            this.separatorV.BackColor = System.Drawing.Color.DimGray;
            this.separatorV.Name = "separatorV";
            // 
            // iconImgTpmVersion
            // 
            resources.ApplyResources(this.iconImgTpmVersion, "iconImgTpmVersion");
            this.iconImgTpmVersion.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgTpmVersion.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgTpmVersion.Name = "iconImgTpmVersion";
            this.iconImgTpmVersion.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgTpmVersion.TabStop = false;
            // 
            // iconImgSmartStatus
            // 
            resources.ApplyResources(this.iconImgSmartStatus, "iconImgSmartStatus");
            this.iconImgSmartStatus.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgSmartStatus.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgSmartStatus.Name = "iconImgSmartStatus";
            this.iconImgSmartStatus.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgSmartStatus.TabStop = false;
            // 
            // lblSmartStatus
            // 
            resources.ApplyResources(this.lblSmartStatus, "lblSmartStatus");
            this.lblSmartStatus.ForeColor = System.Drawing.Color.Silver;
            this.lblSmartStatus.Name = "lblSmartStatus";
            // 
            // lblTpmVersion
            // 
            resources.ApplyResources(this.lblTpmVersion, "lblTpmVersion");
            this.lblTpmVersion.ForeColor = System.Drawing.Color.Silver;
            this.lblTpmVersion.Name = "lblTpmVersion";
            // 
            // lblFixedSmartStatus
            // 
            resources.ApplyResources(this.lblFixedSmartStatus, "lblFixedSmartStatus");
            this.lblFixedSmartStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedSmartStatus.Name = "lblFixedSmartStatus";
            // 
            // iconImgVirtualizationTechnology
            // 
            resources.ApplyResources(this.iconImgVirtualizationTechnology, "iconImgVirtualizationTechnology");
            this.iconImgVirtualizationTechnology.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgVirtualizationTechnology.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgVirtualizationTechnology.Name = "iconImgVirtualizationTechnology";
            this.iconImgVirtualizationTechnology.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgVirtualizationTechnology.TabStop = false;
            // 
            // lblFixedTpmVersion
            // 
            resources.ApplyResources(this.lblFixedTpmVersion, "lblFixedTpmVersion");
            this.lblFixedTpmVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedTpmVersion.Name = "lblFixedTpmVersion";
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // lblProgressBarPercent
            // 
            resources.ApplyResources(this.lblProgressBarPercent, "lblProgressBarPercent");
            this.lblProgressBarPercent.BackColor = System.Drawing.Color.Transparent;
            this.lblProgressBarPercent.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblProgressBarPercent.Name = "lblProgressBarPercent";
            // 
            // lblVirtualizationTechnology
            // 
            resources.ApplyResources(this.lblVirtualizationTechnology, "lblVirtualizationTechnology");
            this.lblVirtualizationTechnology.ForeColor = System.Drawing.Color.Silver;
            this.lblVirtualizationTechnology.Name = "lblVirtualizationTechnology";
            // 
            // lblFixedVirtualizationTechnology
            // 
            resources.ApplyResources(this.lblFixedVirtualizationTechnology, "lblFixedVirtualizationTechnology");
            this.lblFixedVirtualizationTechnology.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedVirtualizationTechnology.Name = "lblFixedVirtualizationTechnology";
            // 
            // iconImgBrand
            // 
            resources.ApplyResources(this.iconImgBrand, "iconImgBrand");
            this.iconImgBrand.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgBrand.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgBrand.Name = "iconImgBrand";
            this.iconImgBrand.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgBrand.TabStop = false;
            // 
            // iconImgSecureBoot
            // 
            resources.ApplyResources(this.iconImgSecureBoot, "iconImgSecureBoot");
            this.iconImgSecureBoot.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgSecureBoot.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgSecureBoot.Name = "iconImgSecureBoot";
            this.iconImgSecureBoot.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgSecureBoot.TabStop = false;
            // 
            // iconImgFwVersion
            // 
            resources.ApplyResources(this.iconImgFwVersion, "iconImgFwVersion");
            this.iconImgFwVersion.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgFwVersion.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgFwVersion.Name = "iconImgFwVersion";
            this.iconImgFwVersion.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgFwVersion.TabStop = false;
            // 
            // iconImgFwType
            // 
            resources.ApplyResources(this.iconImgFwType, "iconImgFwType");
            this.iconImgFwType.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgFwType.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgFwType.Name = "iconImgFwType";
            this.iconImgFwType.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgFwType.TabStop = false;
            // 
            // iconImgIpAddress
            // 
            resources.ApplyResources(this.iconImgIpAddress, "iconImgIpAddress");
            this.iconImgIpAddress.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgIpAddress.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgIpAddress.Name = "iconImgIpAddress";
            this.iconImgIpAddress.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgIpAddress.TabStop = false;
            // 
            // iconImgMacAddress
            // 
            resources.ApplyResources(this.iconImgMacAddress, "iconImgMacAddress");
            this.iconImgMacAddress.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgMacAddress.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgMacAddress.Name = "iconImgMacAddress";
            this.iconImgMacAddress.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgMacAddress.TabStop = false;
            // 
            // iconImgHostname
            // 
            resources.ApplyResources(this.iconImgHostname, "iconImgHostname");
            this.iconImgHostname.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgHostname.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgHostname.Name = "iconImgHostname";
            this.iconImgHostname.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgHostname.TabStop = false;
            // 
            // iconImgOperatingSystem
            // 
            resources.ApplyResources(this.iconImgOperatingSystem, "iconImgOperatingSystem");
            this.iconImgOperatingSystem.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgOperatingSystem.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgOperatingSystem.Name = "iconImgOperatingSystem";
            this.iconImgOperatingSystem.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgOperatingSystem.TabStop = false;
            // 
            // iconImgVideoCard
            // 
            resources.ApplyResources(this.iconImgVideoCard, "iconImgVideoCard");
            this.iconImgVideoCard.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgVideoCard.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgVideoCard.Name = "iconImgVideoCard";
            this.iconImgVideoCard.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgVideoCard.TabStop = false;
            // 
            // iconImgMediaOperationMode
            // 
            resources.ApplyResources(this.iconImgMediaOperationMode, "iconImgMediaOperationMode");
            this.iconImgMediaOperationMode.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgMediaOperationMode.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgMediaOperationMode.Name = "iconImgMediaOperationMode";
            this.iconImgMediaOperationMode.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgMediaOperationMode.TabStop = false;
            // 
            // iconImgStorageType
            // 
            resources.ApplyResources(this.iconImgStorageType, "iconImgStorageType");
            this.iconImgStorageType.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgStorageType.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgStorageType.Name = "iconImgStorageType";
            this.iconImgStorageType.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgStorageType.TabStop = false;
            // 
            // iconImgStorageSize
            // 
            resources.ApplyResources(this.iconImgStorageSize, "iconImgStorageSize");
            this.iconImgStorageSize.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgStorageSize.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgStorageSize.Name = "iconImgStorageSize";
            this.iconImgStorageSize.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgStorageSize.TabStop = false;
            // 
            // iconImgRam
            // 
            resources.ApplyResources(this.iconImgRam, "iconImgRam");
            this.iconImgRam.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgRam.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgRam.Name = "iconImgRam";
            this.iconImgRam.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgRam.TabStop = false;
            // 
            // iconImgProcessor
            // 
            resources.ApplyResources(this.iconImgProcessor, "iconImgProcessor");
            this.iconImgProcessor.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgProcessor.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgProcessor.Name = "iconImgProcessor";
            this.iconImgProcessor.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgProcessor.TabStop = false;
            // 
            // iconImgSerialNumber
            // 
            resources.ApplyResources(this.iconImgSerialNumber, "iconImgSerialNumber");
            this.iconImgSerialNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgSerialNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgSerialNumber.Name = "iconImgSerialNumber";
            this.iconImgSerialNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgSerialNumber.TabStop = false;
            // 
            // iconImgModel
            // 
            resources.ApplyResources(this.iconImgModel, "iconImgModel");
            this.iconImgModel.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgModel.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgModel.Name = "iconImgModel";
            this.iconImgModel.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgModel.TabStop = false;
            // 
            // lblSecureBoot
            // 
            resources.ApplyResources(this.lblSecureBoot, "lblSecureBoot");
            this.lblSecureBoot.ForeColor = System.Drawing.Color.Silver;
            this.lblSecureBoot.Name = "lblSecureBoot";
            // 
            // lblFixedSecureBoot
            // 
            resources.ApplyResources(this.lblFixedSecureBoot, "lblFixedSecureBoot");
            this.lblFixedSecureBoot.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedSecureBoot.Name = "lblFixedSecureBoot";
            // 
            // lblMediaOperationMode
            // 
            resources.ApplyResources(this.lblMediaOperationMode, "lblMediaOperationMode");
            this.lblMediaOperationMode.ForeColor = System.Drawing.Color.Silver;
            this.lblMediaOperationMode.Name = "lblMediaOperationMode";
            // 
            // lblFixedMediaOperationMode
            // 
            resources.ApplyResources(this.lblFixedMediaOperationMode, "lblFixedMediaOperationMode");
            this.lblFixedMediaOperationMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedMediaOperationMode.Name = "lblFixedMediaOperationMode";
            // 
            // lblVideoCard
            // 
            resources.ApplyResources(this.lblVideoCard, "lblVideoCard");
            this.lblVideoCard.ForeColor = System.Drawing.Color.Silver;
            this.lblVideoCard.Name = "lblVideoCard";
            // 
            // lblFixedVideoCard
            // 
            resources.ApplyResources(this.lblFixedVideoCard, "lblFixedVideoCard");
            this.lblFixedVideoCard.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedVideoCard.Name = "lblFixedVideoCard";
            // 
            // lblStorageType
            // 
            resources.ApplyResources(this.lblStorageType, "lblStorageType");
            this.lblStorageType.ForeColor = System.Drawing.Color.Silver;
            this.lblStorageType.Name = "lblStorageType";
            // 
            // lblFixedStorageType
            // 
            resources.ApplyResources(this.lblFixedStorageType, "lblFixedStorageType");
            this.lblFixedStorageType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedStorageType.Name = "lblFixedStorageType";
            // 
            // groupBoxAssetData
            // 
            resources.ApplyResources(this.groupBoxAssetData, "groupBoxAssetData");
            this.groupBoxAssetData.Controls.Add(this.comboBoxBatteryChange);
            this.groupBoxAssetData.Controls.Add(this.comboBoxStandard);
            this.groupBoxAssetData.Controls.Add(this.comboBoxActiveDirectory);
            this.groupBoxAssetData.Controls.Add(this.comboBoxTag);
            this.groupBoxAssetData.Controls.Add(this.comboBoxInUse);
            this.groupBoxAssetData.Controls.Add(this.comboBoxHwType);
            this.groupBoxAssetData.Controls.Add(this.comboBoxBuilding);
            this.groupBoxAssetData.Controls.Add(this.lblFixedMandatoryTicketNumber);
            this.groupBoxAssetData.Controls.Add(this.lblFixedMandatoryBatteryChange);
            this.groupBoxAssetData.Controls.Add(this.iconImgTicketNumber);
            this.groupBoxAssetData.Controls.Add(this.lblFixedTicketNumber);
            this.groupBoxAssetData.Controls.Add(this.textBoxTicketNumber);
            this.groupBoxAssetData.Controls.Add(this.iconImgBatteryChange);
            this.groupBoxAssetData.Controls.Add(this.lblFixedMandatoryWho);
            this.groupBoxAssetData.Controls.Add(this.lblFixedMandatoryTag);
            this.groupBoxAssetData.Controls.Add(this.lblFixedBatteryChange);
            this.groupBoxAssetData.Controls.Add(this.lblFixedMandatoryHwType);
            this.groupBoxAssetData.Controls.Add(this.lblFixedMandatoryInUse);
            this.groupBoxAssetData.Controls.Add(this.lblFixedMandatoryBuilding);
            this.groupBoxAssetData.Controls.Add(this.lblFixedMandatoryRoomNumber);
            this.groupBoxAssetData.Controls.Add(this.lblFixedMandatoryAssetNumber);
            this.groupBoxAssetData.Controls.Add(this.lblFixedMandatoryMain);
            this.groupBoxAssetData.Controls.Add(this.radioButtonStudent);
            this.groupBoxAssetData.Controls.Add(this.radioButtonEmployee);
            this.groupBoxAssetData.Controls.Add(this.iconImgWho);
            this.groupBoxAssetData.Controls.Add(this.lblFixedWho);
            this.groupBoxAssetData.Controls.Add(this.iconImgRoomLetter);
            this.groupBoxAssetData.Controls.Add(this.iconImgHwType);
            this.groupBoxAssetData.Controls.Add(this.iconImgTag);
            this.groupBoxAssetData.Controls.Add(this.iconImgInUse);
            this.groupBoxAssetData.Controls.Add(this.iconImgServiceDate);
            this.groupBoxAssetData.Controls.Add(this.iconImgStandard);
            this.groupBoxAssetData.Controls.Add(this.iconImgAdRegistered);
            this.groupBoxAssetData.Controls.Add(this.iconImgBuilding);
            this.groupBoxAssetData.Controls.Add(this.iconImgRoomNumber);
            this.groupBoxAssetData.Controls.Add(this.iconImgSealNumber);
            this.groupBoxAssetData.Controls.Add(this.iconImgAssetNumber);
            this.groupBoxAssetData.Controls.Add(this.dateTimePickerServiceDate);
            this.groupBoxAssetData.Controls.Add(this.groupBoxServiceType);
            this.groupBoxAssetData.Controls.Add(this.lblFixedAssetNumber);
            this.groupBoxAssetData.Controls.Add(this.lblFixedSealNumber);
            this.groupBoxAssetData.Controls.Add(this.lblFixedBuilding);
            this.groupBoxAssetData.Controls.Add(this.textBoxAssetNumber);
            this.groupBoxAssetData.Controls.Add(this.textBoxSealNumber);
            this.groupBoxAssetData.Controls.Add(this.lblFixedRoomLetter);
            this.groupBoxAssetData.Controls.Add(this.textBoxRoomNumber);
            this.groupBoxAssetData.Controls.Add(this.lblFixedRoomNumber);
            this.groupBoxAssetData.Controls.Add(this.lblFixedAdRegistered);
            this.groupBoxAssetData.Controls.Add(this.lblFixedServiceDate);
            this.groupBoxAssetData.Controls.Add(this.lblFixedHwType);
            this.groupBoxAssetData.Controls.Add(this.lblFixedStandard);
            this.groupBoxAssetData.Controls.Add(this.textBoxRoomLetter);
            this.groupBoxAssetData.Controls.Add(this.lblFixedInUse);
            this.groupBoxAssetData.Controls.Add(this.lblFixedTag);
            this.groupBoxAssetData.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBoxAssetData.Name = "groupBoxAssetData";
            this.groupBoxAssetData.TabStop = false;
            // 
            // comboBoxBatteryChange
            // 
            resources.ApplyResources(this.comboBoxBatteryChange, "comboBoxBatteryChange");
            this.comboBoxBatteryChange.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxBatteryChange.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxBatteryChange.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxBatteryChange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBatteryChange.FormattingEnabled = true;
            this.comboBoxBatteryChange.Name = "comboBoxBatteryChange";
            // 
            // comboBoxStandard
            // 
            resources.ApplyResources(this.comboBoxStandard, "comboBoxStandard");
            this.comboBoxStandard.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxStandard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxStandard.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStandard.FormattingEnabled = true;
            this.comboBoxStandard.Name = "comboBoxStandard";
            // 
            // comboBoxActiveDirectory
            // 
            resources.ApplyResources(this.comboBoxActiveDirectory, "comboBoxActiveDirectory");
            this.comboBoxActiveDirectory.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxActiveDirectory.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxActiveDirectory.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxActiveDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxActiveDirectory.FormattingEnabled = true;
            this.comboBoxActiveDirectory.Name = "comboBoxActiveDirectory";
            // 
            // comboBoxTag
            // 
            resources.ApplyResources(this.comboBoxTag, "comboBoxTag");
            this.comboBoxTag.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxTag.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxTag.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTag.FormattingEnabled = true;
            this.comboBoxTag.Name = "comboBoxTag";
            // 
            // comboBoxInUse
            // 
            resources.ApplyResources(this.comboBoxInUse, "comboBoxInUse");
            this.comboBoxInUse.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxInUse.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxInUse.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxInUse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInUse.FormattingEnabled = true;
            this.comboBoxInUse.Name = "comboBoxInUse";
            // 
            // comboBoxHwType
            // 
            resources.ApplyResources(this.comboBoxHwType, "comboBoxHwType");
            this.comboBoxHwType.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxHwType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxHwType.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxHwType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHwType.FormattingEnabled = true;
            this.comboBoxHwType.Name = "comboBoxHwType";
            // 
            // comboBoxBuilding
            // 
            resources.ApplyResources(this.comboBoxBuilding, "comboBoxBuilding");
            this.comboBoxBuilding.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxBuilding.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxBuilding.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBuilding.FormattingEnabled = true;
            this.comboBoxBuilding.Name = "comboBoxBuilding";
            // 
            // lblFixedMandatoryTicketNumber
            // 
            resources.ApplyResources(this.lblFixedMandatoryTicketNumber, "lblFixedMandatoryTicketNumber");
            this.lblFixedMandatoryTicketNumber.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryTicketNumber.Name = "lblFixedMandatoryTicketNumber";
            // 
            // lblFixedMandatoryBatteryChange
            // 
            resources.ApplyResources(this.lblFixedMandatoryBatteryChange, "lblFixedMandatoryBatteryChange");
            this.lblFixedMandatoryBatteryChange.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryBatteryChange.Name = "lblFixedMandatoryBatteryChange";
            // 
            // iconImgTicketNumber
            // 
            resources.ApplyResources(this.iconImgTicketNumber, "iconImgTicketNumber");
            this.iconImgTicketNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgTicketNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgTicketNumber.Name = "iconImgTicketNumber";
            this.iconImgTicketNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgTicketNumber.TabStop = false;
            // 
            // lblFixedTicketNumber
            // 
            resources.ApplyResources(this.lblFixedTicketNumber, "lblFixedTicketNumber");
            this.lblFixedTicketNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedTicketNumber.Name = "lblFixedTicketNumber";
            // 
            // textBoxTicketNumber
            // 
            resources.ApplyResources(this.textBoxTicketNumber, "textBoxTicketNumber");
            this.textBoxTicketNumber.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxTicketNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxTicketNumber.Name = "textBoxTicketNumber";
            this.textBoxTicketNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxNumbersOnly_KeyPress);
            // 
            // iconImgBatteryChange
            // 
            resources.ApplyResources(this.iconImgBatteryChange, "iconImgBatteryChange");
            this.iconImgBatteryChange.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgBatteryChange.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgBatteryChange.Name = "iconImgBatteryChange";
            this.iconImgBatteryChange.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgBatteryChange.TabStop = false;
            // 
            // lblFixedMandatoryWho
            // 
            resources.ApplyResources(this.lblFixedMandatoryWho, "lblFixedMandatoryWho");
            this.lblFixedMandatoryWho.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryWho.Name = "lblFixedMandatoryWho";
            // 
            // lblFixedMandatoryTag
            // 
            resources.ApplyResources(this.lblFixedMandatoryTag, "lblFixedMandatoryTag");
            this.lblFixedMandatoryTag.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryTag.Name = "lblFixedMandatoryTag";
            // 
            // lblFixedBatteryChange
            // 
            resources.ApplyResources(this.lblFixedBatteryChange, "lblFixedBatteryChange");
            this.lblFixedBatteryChange.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedBatteryChange.Name = "lblFixedBatteryChange";
            // 
            // lblFixedMandatoryHwType
            // 
            resources.ApplyResources(this.lblFixedMandatoryHwType, "lblFixedMandatoryHwType");
            this.lblFixedMandatoryHwType.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryHwType.Name = "lblFixedMandatoryHwType";
            // 
            // lblFixedMandatoryInUse
            // 
            resources.ApplyResources(this.lblFixedMandatoryInUse, "lblFixedMandatoryInUse");
            this.lblFixedMandatoryInUse.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryInUse.Name = "lblFixedMandatoryInUse";
            // 
            // lblFixedMandatoryBuilding
            // 
            resources.ApplyResources(this.lblFixedMandatoryBuilding, "lblFixedMandatoryBuilding");
            this.lblFixedMandatoryBuilding.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryBuilding.Name = "lblFixedMandatoryBuilding";
            // 
            // lblFixedMandatoryRoomNumber
            // 
            resources.ApplyResources(this.lblFixedMandatoryRoomNumber, "lblFixedMandatoryRoomNumber");
            this.lblFixedMandatoryRoomNumber.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryRoomNumber.Name = "lblFixedMandatoryRoomNumber";
            // 
            // lblFixedMandatoryAssetNumber
            // 
            resources.ApplyResources(this.lblFixedMandatoryAssetNumber, "lblFixedMandatoryAssetNumber");
            this.lblFixedMandatoryAssetNumber.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryAssetNumber.Name = "lblFixedMandatoryAssetNumber";
            // 
            // lblFixedMandatoryMain
            // 
            resources.ApplyResources(this.lblFixedMandatoryMain, "lblFixedMandatoryMain");
            this.lblFixedMandatoryMain.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryMain.Name = "lblFixedMandatoryMain";
            // 
            // radioButtonStudent
            // 
            resources.ApplyResources(this.radioButtonStudent, "radioButtonStudent");
            this.radioButtonStudent.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radioButtonStudent.Name = "radioButtonStudent";
            this.radioButtonStudent.TabStop = true;
            this.radioButtonStudent.UseVisualStyleBackColor = true;
            this.radioButtonStudent.CheckedChanged += new System.EventHandler(this.StudentButton2_CheckedChanged);
            // 
            // radioButtonEmployee
            // 
            resources.ApplyResources(this.radioButtonEmployee, "radioButtonEmployee");
            this.radioButtonEmployee.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radioButtonEmployee.Name = "radioButtonEmployee";
            this.radioButtonEmployee.TabStop = true;
            this.radioButtonEmployee.UseVisualStyleBackColor = true;
            this.radioButtonEmployee.CheckedChanged += new System.EventHandler(this.EmployeeButton1_CheckedChanged);
            // 
            // iconImgWho
            // 
            resources.ApplyResources(this.iconImgWho, "iconImgWho");
            this.iconImgWho.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgWho.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgWho.Name = "iconImgWho";
            this.iconImgWho.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgWho.TabStop = false;
            // 
            // lblFixedWho
            // 
            resources.ApplyResources(this.lblFixedWho, "lblFixedWho");
            this.lblFixedWho.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedWho.Name = "lblFixedWho";
            // 
            // iconImgRoomLetter
            // 
            resources.ApplyResources(this.iconImgRoomLetter, "iconImgRoomLetter");
            this.iconImgRoomLetter.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgRoomLetter.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgRoomLetter.Name = "iconImgRoomLetter";
            this.iconImgRoomLetter.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgRoomLetter.TabStop = false;
            // 
            // iconImgHwType
            // 
            resources.ApplyResources(this.iconImgHwType, "iconImgHwType");
            this.iconImgHwType.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgHwType.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgHwType.Name = "iconImgHwType";
            this.iconImgHwType.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgHwType.TabStop = false;
            // 
            // iconImgTag
            // 
            resources.ApplyResources(this.iconImgTag, "iconImgTag");
            this.iconImgTag.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgTag.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgTag.Name = "iconImgTag";
            this.iconImgTag.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgTag.TabStop = false;
            // 
            // iconImgInUse
            // 
            resources.ApplyResources(this.iconImgInUse, "iconImgInUse");
            this.iconImgInUse.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgInUse.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgInUse.Name = "iconImgInUse";
            this.iconImgInUse.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgInUse.TabStop = false;
            // 
            // iconImgServiceDate
            // 
            resources.ApplyResources(this.iconImgServiceDate, "iconImgServiceDate");
            this.iconImgServiceDate.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgServiceDate.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgServiceDate.Name = "iconImgServiceDate";
            this.iconImgServiceDate.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgServiceDate.TabStop = false;
            // 
            // iconImgStandard
            // 
            resources.ApplyResources(this.iconImgStandard, "iconImgStandard");
            this.iconImgStandard.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgStandard.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgStandard.Name = "iconImgStandard";
            this.iconImgStandard.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgStandard.TabStop = false;
            // 
            // iconImgAdRegistered
            // 
            resources.ApplyResources(this.iconImgAdRegistered, "iconImgAdRegistered");
            this.iconImgAdRegistered.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgAdRegistered.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgAdRegistered.Name = "iconImgAdRegistered";
            this.iconImgAdRegistered.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgAdRegistered.TabStop = false;
            // 
            // iconImgBuilding
            // 
            resources.ApplyResources(this.iconImgBuilding, "iconImgBuilding");
            this.iconImgBuilding.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgBuilding.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgBuilding.Name = "iconImgBuilding";
            this.iconImgBuilding.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgBuilding.TabStop = false;
            // 
            // iconImgRoomNumber
            // 
            resources.ApplyResources(this.iconImgRoomNumber, "iconImgRoomNumber");
            this.iconImgRoomNumber.CompositingQuality = null;
            this.iconImgRoomNumber.InterpolationMode = null;
            this.iconImgRoomNumber.Name = "iconImgRoomNumber";
            this.iconImgRoomNumber.SmoothingMode = null;
            this.iconImgRoomNumber.TabStop = false;
            // 
            // iconImgSealNumber
            // 
            resources.ApplyResources(this.iconImgSealNumber, "iconImgSealNumber");
            this.iconImgSealNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgSealNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgSealNumber.Name = "iconImgSealNumber";
            this.iconImgSealNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgSealNumber.TabStop = false;
            // 
            // iconImgAssetNumber
            // 
            resources.ApplyResources(this.iconImgAssetNumber, "iconImgAssetNumber");
            this.iconImgAssetNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgAssetNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgAssetNumber.Name = "iconImgAssetNumber";
            this.iconImgAssetNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgAssetNumber.TabStop = false;
            // 
            // dateTimePickerServiceDate
            // 
            resources.ApplyResources(this.dateTimePickerServiceDate, "dateTimePickerServiceDate");
            this.dateTimePickerServiceDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePickerServiceDate.Name = "dateTimePickerServiceDate";
            // 
            // groupBoxServiceType
            // 
            resources.ApplyResources(this.groupBoxServiceType, "groupBoxServiceType");
            this.groupBoxServiceType.Controls.Add(this.loadingCircleMaintenance);
            this.groupBoxServiceType.Controls.Add(this.loadingCircleFormatting);
            this.groupBoxServiceType.Controls.Add(this.lblMaintenanceSince);
            this.groupBoxServiceType.Controls.Add(this.lblInstallSince);
            this.groupBoxServiceType.Controls.Add(this.lblFixedMandatoryServiceType);
            this.groupBoxServiceType.Controls.Add(this.textBoxFixedFormattingRadio);
            this.groupBoxServiceType.Controls.Add(this.textBoxFixedMaintenanceRadio);
            this.groupBoxServiceType.Controls.Add(this.radioButtonFormatting);
            this.groupBoxServiceType.Controls.Add(this.radioButtonMaintenance);
            this.groupBoxServiceType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBoxServiceType.Name = "groupBoxServiceType";
            this.groupBoxServiceType.TabStop = false;
            // 
            // loadingCircleMaintenance
            // 
            resources.ApplyResources(this.loadingCircleMaintenance, "loadingCircleMaintenance");
            this.loadingCircleMaintenance.Active = false;
            this.loadingCircleMaintenance.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleMaintenance.InnerCircleRadius = 5;
            this.loadingCircleMaintenance.Name = "loadingCircleMaintenance";
            this.loadingCircleMaintenance.NumberSpoke = 12;
            this.loadingCircleMaintenance.OuterCircleRadius = 11;
            this.loadingCircleMaintenance.RotationSpeed = 1;
            this.loadingCircleMaintenance.SpokeThickness = 2;
            this.loadingCircleMaintenance.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleFormatting
            // 
            resources.ApplyResources(this.loadingCircleFormatting, "loadingCircleFormatting");
            this.loadingCircleFormatting.Active = false;
            this.loadingCircleFormatting.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleFormatting.InnerCircleRadius = 5;
            this.loadingCircleFormatting.Name = "loadingCircleFormatting";
            this.loadingCircleFormatting.NumberSpoke = 12;
            this.loadingCircleFormatting.OuterCircleRadius = 11;
            this.loadingCircleFormatting.RotationSpeed = 1;
            this.loadingCircleFormatting.SpokeThickness = 2;
            this.loadingCircleFormatting.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
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
            // lblFixedMandatoryServiceType
            // 
            resources.ApplyResources(this.lblFixedMandatoryServiceType, "lblFixedMandatoryServiceType");
            this.lblFixedMandatoryServiceType.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryServiceType.Name = "lblFixedMandatoryServiceType";
            // 
            // textBoxFixedFormattingRadio
            // 
            resources.ApplyResources(this.textBoxFixedFormattingRadio, "textBoxFixedFormattingRadio");
            this.textBoxFixedFormattingRadio.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxFixedFormattingRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxFixedFormattingRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxFixedFormattingRadio.Name = "textBoxFixedFormattingRadio";
            this.textBoxFixedFormattingRadio.ReadOnly = true;
            // 
            // textBoxFixedMaintenanceRadio
            // 
            resources.ApplyResources(this.textBoxFixedMaintenanceRadio, "textBoxFixedMaintenanceRadio");
            this.textBoxFixedMaintenanceRadio.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxFixedMaintenanceRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxFixedMaintenanceRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxFixedMaintenanceRadio.Name = "textBoxFixedMaintenanceRadio";
            this.textBoxFixedMaintenanceRadio.ReadOnly = true;
            // 
            // radioButtonFormatting
            // 
            resources.ApplyResources(this.radioButtonFormatting, "radioButtonFormatting");
            this.radioButtonFormatting.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radioButtonFormatting.Name = "radioButtonFormatting";
            this.radioButtonFormatting.UseVisualStyleBackColor = true;
            this.radioButtonFormatting.CheckedChanged += new System.EventHandler(this.FormatButton1_CheckedChanged);
            // 
            // radioButtonMaintenance
            // 
            resources.ApplyResources(this.radioButtonMaintenance, "radioButtonMaintenance");
            this.radioButtonMaintenance.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radioButtonMaintenance.Name = "radioButtonMaintenance";
            this.radioButtonMaintenance.UseVisualStyleBackColor = true;
            this.radioButtonMaintenance.CheckedChanged += new System.EventHandler(this.MaintenanceButton2_CheckedChanged);
            // 
            // lblFixedAdRegistered
            // 
            resources.ApplyResources(this.lblFixedAdRegistered, "lblFixedAdRegistered");
            this.lblFixedAdRegistered.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedAdRegistered.Name = "lblFixedAdRegistered";
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
            // lblServerPort
            // 
            resources.ApplyResources(this.lblServerPort, "lblServerPort");
            this.lblServerPort.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblServerPort.Name = "lblServerPort";
            // 
            // lblServerIP
            // 
            resources.ApplyResources(this.lblServerIP, "lblServerIP");
            this.lblServerIP.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblServerIP.Name = "lblServerIP";
            // 
            // lblFixedServerIP
            // 
            resources.ApplyResources(this.lblFixedServerIP, "lblFixedServerIP");
            this.lblFixedServerIP.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedServerIP.Name = "lblFixedServerIP";
            // 
            // lblServerOperationalStatus
            // 
            resources.ApplyResources(this.lblServerOperationalStatus, "lblServerOperationalStatus");
            this.lblServerOperationalStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblServerOperationalStatus.ForeColor = System.Drawing.Color.Silver;
            this.lblServerOperationalStatus.Name = "lblServerOperationalStatus";
            // 
            // toolStripVersionText
            // 
            resources.ApplyResources(this.toolStripVersionText, "toolStripVersionText");
            this.toolStripVersionText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripVersionText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripVersionText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripVersionText.Name = "toolStripVersionText";
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comboBoxThemeButton,
            this.logLabelButton,
            this.aboutLabelButton,
            this.toolStripStatusBarText,
            this.toolStripVersionText});
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // comboBoxThemeButton
            // 
            resources.ApplyResources(this.comboBoxThemeButton, "comboBoxThemeButton");
            this.comboBoxThemeButton.BackColor = System.Drawing.SystemColors.Control;
            this.comboBoxThemeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.comboBoxThemeButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripAutoTheme,
            this.toolStripLightTheme,
            this.toolStripDarkTheme});
            this.comboBoxThemeButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBoxThemeButton.Name = "comboBoxThemeButton";
            // 
            // toolStripAutoTheme
            // 
            resources.ApplyResources(this.toolStripAutoTheme, "toolStripAutoTheme");
            this.toolStripAutoTheme.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripAutoTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripAutoTheme.Name = "toolStripAutoTheme";
            this.toolStripAutoTheme.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // toolStripLightTheme
            // 
            resources.ApplyResources(this.toolStripLightTheme, "toolStripLightTheme");
            this.toolStripLightTheme.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripLightTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripLightTheme.Name = "toolStripLightTheme";
            this.toolStripLightTheme.Click += new System.EventHandler(this.ToolStripMenuItem2_Click);
            // 
            // toolStripDarkTheme
            // 
            resources.ApplyResources(this.toolStripDarkTheme, "toolStripDarkTheme");
            this.toolStripDarkTheme.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripDarkTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripDarkTheme.Name = "toolStripDarkTheme";
            this.toolStripDarkTheme.Click += new System.EventHandler(this.ToolStripMenuItem3_Click);
            // 
            // logLabelButton
            // 
            resources.ApplyResources(this.logLabelButton, "logLabelButton");
            this.logLabelButton.BackColor = System.Drawing.SystemColors.Control;
            this.logLabelButton.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.logLabelButton.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.logLabelButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.logLabelButton.Name = "logLabelButton";
            this.logLabelButton.Click += new System.EventHandler(this.LogLabelButton_Click);
            this.logLabelButton.MouseEnter += new System.EventHandler(this.LogLabel_MouseEnter);
            this.logLabelButton.MouseLeave += new System.EventHandler(this.LogLabel_MouseLeave);
            // 
            // aboutLabelButton
            // 
            resources.ApplyResources(this.aboutLabelButton, "aboutLabelButton");
            this.aboutLabelButton.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.aboutLabelButton.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.aboutLabelButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.aboutLabelButton.Name = "aboutLabelButton";
            this.aboutLabelButton.Click += new System.EventHandler(this.AboutLabelButton_Click);
            this.aboutLabelButton.MouseEnter += new System.EventHandler(this.AboutLabel_MouseEnter);
            this.aboutLabelButton.MouseLeave += new System.EventHandler(this.AboutLabel_MouseLeave);
            // 
            // toolStripStatusBarText
            // 
            resources.ApplyResources(this.toolStripStatusBarText, "toolStripStatusBarText");
            this.toolStripStatusBarText.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripStatusBarText.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.toolStripStatusBarText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripStatusBarText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripStatusBarText.Name = "toolStripStatusBarText";
            this.toolStripStatusBarText.Spring = true;
            // 
            // timerAlertHostname
            // 
            this.timerAlertHostname.Interval = 500;
            // 
            // groupBoxRegistryStatus
            // 
            resources.ApplyResources(this.groupBoxRegistryStatus, "groupBoxRegistryStatus");
            this.groupBoxRegistryStatus.Controls.Add(this.webView2Control);
            this.groupBoxRegistryStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBoxRegistryStatus.Name = "groupBoxRegistryStatus";
            this.groupBoxRegistryStatus.TabStop = false;
            // 
            // webView2Control
            // 
            resources.ApplyResources(this.webView2Control, "webView2Control");
            this.webView2Control.AllowExternalDrop = true;
            this.webView2Control.CreationProperties = null;
            this.webView2Control.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView2Control.Name = "webView2Control";
            this.webView2Control.ZoomFactor = 1D;
            // 
            // imgTopBanner
            // 
            resources.ApplyResources(this.imgTopBanner, "imgTopBanner");
            this.imgTopBanner.CompositingQuality = null;
            this.imgTopBanner.InterpolationMode = null;
            this.imgTopBanner.Name = "imgTopBanner";
            this.imgTopBanner.SmoothingMode = null;
            this.imgTopBanner.TabStop = false;
            // 
            // loadingCircleCollectButton
            // 
            resources.ApplyResources(this.loadingCircleCollectButton, "loadingCircleCollectButton");
            this.loadingCircleCollectButton.Active = false;
            this.loadingCircleCollectButton.BackColor = System.Drawing.SystemColors.Control;
            this.loadingCircleCollectButton.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleCollectButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleCollectButton.InnerCircleRadius = 5;
            this.loadingCircleCollectButton.Name = "loadingCircleCollectButton";
            this.loadingCircleCollectButton.NumberSpoke = 12;
            this.loadingCircleCollectButton.OuterCircleRadius = 11;
            this.loadingCircleCollectButton.RotationSpeed = 1;
            this.loadingCircleCollectButton.SpokeThickness = 2;
            this.loadingCircleCollectButton.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircleCollectButton.UseWaitCursor = true;
            // 
            // loadingCircleRegisterButton
            // 
            resources.ApplyResources(this.loadingCircleRegisterButton, "loadingCircleRegisterButton");
            this.loadingCircleRegisterButton.Active = false;
            this.loadingCircleRegisterButton.BackColor = System.Drawing.SystemColors.Control;
            this.loadingCircleRegisterButton.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleRegisterButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleRegisterButton.InnerCircleRadius = 5;
            this.loadingCircleRegisterButton.Name = "loadingCircleRegisterButton";
            this.loadingCircleRegisterButton.NumberSpoke = 12;
            this.loadingCircleRegisterButton.OuterCircleRadius = 11;
            this.loadingCircleRegisterButton.RotationSpeed = 1;
            this.loadingCircleRegisterButton.SpokeThickness = 2;
            this.loadingCircleRegisterButton.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // groupBoxServerStatus
            // 
            resources.ApplyResources(this.groupBoxServerStatus, "groupBoxServerStatus");
            this.groupBoxServerStatus.Controls.Add(this.loadingCircleServerOperationalStatus);
            this.groupBoxServerStatus.Controls.Add(this.lblFixedServerIP);
            this.groupBoxServerStatus.Controls.Add(this.lblFixedServerOperationalStatus);
            this.groupBoxServerStatus.Controls.Add(this.lblFixedServerPort);
            this.groupBoxServerStatus.Controls.Add(this.lblServerOperationalStatus);
            this.groupBoxServerStatus.Controls.Add(this.lblServerIP);
            this.groupBoxServerStatus.Controls.Add(this.lblServerPort);
            this.groupBoxServerStatus.Controls.Add(this.lblFixedAgentName);
            this.groupBoxServerStatus.Controls.Add(this.lblAgentName);
            this.groupBoxServerStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBoxServerStatus.Name = "groupBoxServerStatus";
            this.groupBoxServerStatus.TabStop = false;
            // 
            // loadingCircleServerOperationalStatus
            // 
            resources.ApplyResources(this.loadingCircleServerOperationalStatus, "loadingCircleServerOperationalStatus");
            this.loadingCircleServerOperationalStatus.Active = false;
            this.loadingCircleServerOperationalStatus.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleServerOperationalStatus.InnerCircleRadius = 5;
            this.loadingCircleServerOperationalStatus.Name = "loadingCircleServerOperationalStatus";
            this.loadingCircleServerOperationalStatus.NumberSpoke = 12;
            this.loadingCircleServerOperationalStatus.OuterCircleRadius = 11;
            this.loadingCircleServerOperationalStatus.RotationSpeed = 1;
            this.loadingCircleServerOperationalStatus.SpokeThickness = 2;
            this.loadingCircleServerOperationalStatus.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.groupBoxServerStatus);
            this.Controls.Add(this.loadingCircleRegisterButton);
            this.Controls.Add(this.loadingCircleCollectButton);
            this.Controls.Add(this.groupBoxRegistryStatus);
            this.Controls.Add(this.groupBoxAssetData);
            this.Controls.Add(this.groupBoxHwData);
            this.Controls.Add(this.imgTopBanner);
            this.Controls.Add(this.ApcsButton);
            this.Controls.Add(this.collectButton);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.registerButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxHwData.ResumeLayout(false);
            this.groupBoxHwData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgTpmVersion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgSmartStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgVirtualizationTechnology)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgBrand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgSecureBoot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgFwVersion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgFwType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgIpAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgMacAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgHostname)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgOperatingSystem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgVideoCard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgMediaOperationMode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgStorageType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgStorageSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgRam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgProcessor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgSerialNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgModel)).EndInit();
            this.groupBoxAssetData.ResumeLayout(false);
            this.groupBoxAssetData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgTicketNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgBatteryChange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgWho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgRoomLetter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgHwType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgTag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgInUse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgServiceDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgStandard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgAdRegistered)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgBuilding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgRoomNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgSealNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgAssetNumber)).EndInit();
            this.groupBoxServiceType.ResumeLayout(false);
            this.groupBoxServiceType.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBoxRegistryStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView2Control)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgTopBanner)).EndInit();
            this.groupBoxServerStatus.ResumeLayout(false);
            this.groupBoxServerStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        //Variables being declared
        #region
        private Label lblBrand;
        private Label lblModel;
        private Label lblSerialNumber;
        private Label lblProcessor;
        private Label lblRam;
        private Label lblStorageSize;
        private Label lblHostname;
        private Label lblMacAddress;
        private Label lblIpAddress;
        private Label lblFixedBrand;
        private Label lblFixedModel;
        private Label lblFixedSerialNumber;
        private Label lblFixedProcessor;
        private Label lblFixedRam;
        private Label lblFixedStorageSize;
        private Label lblFixedOperatingSystem;
        private Label lblFixedHostname;
        private Label lblFixedMacAddress;
        private Label lblFixedIpAddress;
        private Label lblFixedAssetNumber;
        private Label lblFixedSealNumber;
        private Label lblFixedBuilding;
        private TextBox textBoxAssetNumber;
        private TextBox textBoxSealNumber;
        private TextBox textBoxRoomNumber;
        private TextBox textBoxRoomLetter;
        private Label lblFixedRoomNumber;
        private Label lblFixedServiceDate;
        private Label lblOperatingSystem;
        private Label lblFixedInUse;
        private Label lblFixedTag;
        private Button registerButton;
        private Label lblFixedHwType;
        private Label lblFixedServerOperationalStatus;
        private Label lblFixedFwType;
        private Label lblFwType;
        private GroupBox groupBoxHwData;
        private GroupBox groupBoxAssetData;
        private Label lblStorageType;
        private Label lblFixedStorageType;
        private Label lblVideoCard;
        private Label lblFixedVideoCard;
        private Timer timerAlertHostname, timerAlertMediaOperationMode, timerAlertSecureBoot, timerAlertFwVersion, timerAlertNetConnectivity, timerAlertFwType;
        private IContainer components;
        private Label lblMediaOperationMode;
        private Label lblFixedMediaOperationMode;
        private ToolStripStatusLabel toolStripVersionText;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusBarText;
        private Button collectButton;
        private Label lblFixedRoomLetter;
        private Label lblFixedFwVersion;
        private Label lblFwVersion;
        private Button ApcsButton;
        private ProgressBar progressBar1;
        private Label lblProgressBarPercent;
        private Label lblSecureBoot;
        private Label lblFixedSecureBoot;
        private WebView2 webView2Control;
        private RadioButton radioButtonMaintenance;
        private RadioButton radioButtonFormatting;
        private GroupBox groupBoxServiceType;
        private TextBox textBoxFixedFormattingRadio;
        private TextBox textBoxFixedMaintenanceRadio;
        private ToolStripDropDownButton comboBoxThemeButton;
        private ToolStripMenuItem toolStripAutoTheme;
        private ToolStripMenuItem toolStripLightTheme;
        private ToolStripMenuItem toolStripDarkTheme;
        private Label lblServerOperationalStatus;
        private DateTimePicker dateTimePickerServiceDate;
        private ConfigurableQualityPictureBox imgTopBanner;
        private ConfigurableQualityPictureBox iconImgBrand;
        private ConfigurableQualityPictureBox iconImgModel;
        private ConfigurableQualityPictureBox iconImgSerialNumber;
        private ConfigurableQualityPictureBox iconImgProcessor;
        private ConfigurableQualityPictureBox iconImgRam;
        private ConfigurableQualityPictureBox iconImgStorageSize;
        private ConfigurableQualityPictureBox iconImgStorageType;
        private ConfigurableQualityPictureBox iconImgMediaOperationMode;
        private ConfigurableQualityPictureBox iconImgVideoCard;
        private ConfigurableQualityPictureBox iconImgOperatingSystem;
        private ConfigurableQualityPictureBox iconImgHostname;
        private ConfigurableQualityPictureBox iconImgMacAddress;
        private ConfigurableQualityPictureBox iconImgIpAddress;
        private ConfigurableQualityPictureBox iconImgFwType;
        private ConfigurableQualityPictureBox iconImgFwVersion;
        private ConfigurableQualityPictureBox iconImgSecureBoot;
        private ConfigurableQualityPictureBox iconImgAssetNumber;
        private ConfigurableQualityPictureBox iconImgSealNumber;
        private ConfigurableQualityPictureBox iconImgRoomNumber;
        private ConfigurableQualityPictureBox iconImgBuilding;
        private ConfigurableQualityPictureBox iconImgServiceDate;
        private ConfigurableQualityPictureBox iconImgRoomLetter;
        private ConfigurableQualityPictureBox iconImgInUse;
        private ConfigurableQualityPictureBox iconImgTag;
        private ConfigurableQualityPictureBox iconImgHwType;
        private ConfigurableQualityPictureBox iconImgVirtualizationTechnology;
        private Label lblVirtualizationTechnology;
        private Label lblFixedVirtualizationTechnology;
        private ConfigurableQualityPictureBox iconImgWho;
        private Label lblFixedWho;
        private RadioButton radioButtonStudent;
        private RadioButton radioButtonEmployee;
        private Label lblFixedMandatoryWho;
        private Label lblFixedMandatoryTag;
        private Label lblFixedMandatoryHwType;
        private Label lblFixedMandatoryInUse;
        private Label lblFixedMandatoryBuilding;
        private Label lblFixedMandatoryRoomNumber;
        private Label lblFixedMandatoryAssetNumber;
        private Label lblFixedMandatoryMain;
        private Label lblFixedMandatoryServiceType;
        private Timer timerAlertVirtualizationTechnology;
        private ConfigurableQualityPictureBox iconImgSmartStatus;
        private Label lblSmartStatus;
        private Label lblFixedSmartStatus;
        private Timer timerAlertSmartStatus;
        private Label lblFixedServerPort;
        private ConfigurableQualityPictureBox iconImgTpmVersion;
        private Label lblTpmVersion;
        private Label lblFixedTpmVersion;
        private GroupBox groupBoxRegistryStatus;
        private ConfigurableQualityPictureBox iconImgBatteryChange;
        private Label lblFixedBatteryChange;
        private ConfigurableQualityPictureBox iconImgTicketNumber;
        private Label lblFixedTicketNumber;
        private TextBox textBoxTicketNumber;
        private Label lblFixedMandatoryTicketNumber;
        private Label lblFixedMandatoryBatteryChange;
        private Label lblFixedServerIP;
        private Label lblMaintenanceSince;
        private Label lblInstallSince;
        private BusyForm bw;
        private Label lblServerPort;
        private Label lblServerIP;
        private Label lblAgentName;
        private Label lblFixedAgentName;
        private Timer timerAlertTpmVersion;
        private Timer timerAlertRamAmount;
        private ConfigurableQualityPictureBox iconImgStandard;
        private ConfigurableQualityPictureBox iconImgAdRegistered;
        private Label lblFixedAdRegistered;
        private Label lblFixedStandard;
        private Label separatorV;
        private Label separatorH;
        private CustomFlatComboBox comboBoxBuilding;
        private CustomFlatComboBox comboBoxStandard;
        private CustomFlatComboBox comboBoxActiveDirectory;
        private CustomFlatComboBox comboBoxTag;
        private CustomFlatComboBox comboBoxInUse;
        private CustomFlatComboBox comboBoxHwType;
        private CustomFlatComboBox comboBoxBatteryChange;
        private LoadingCircle loadingCircleTpmVersion;
        private LoadingCircle loadingCircleVirtualizationTechnology;
        private LoadingCircle loadingCircleSecureBoot;
        private LoadingCircle loadingCircleFwVersion;
        private LoadingCircle loadingCircleFwType;
        private LoadingCircle loadingCircleIpAddress;
        private LoadingCircle loadingCircleMacAddress;
        private LoadingCircle loadingCircleHostname;
        private LoadingCircle loadingCircleOperatingSystem;
        private LoadingCircle loadingCircleVideoCard;
        private LoadingCircle loadingCircleMediaOperationMode;
        private LoadingCircle loadingCircleStorageType;
        private LoadingCircle loadingCircleSmartStatus;
        private LoadingCircle loadingCircleStorageSize;
        private LoadingCircle loadingCircleRam;
        private LoadingCircle loadingCircleProcessor;
        private LoadingCircle loadingCircleSerialNumber;
        private LoadingCircle loadingCircleModel;
        private LoadingCircle loadingCircleBrand;
        private LoadingCircle loadingCircleMaintenance;
        private LoadingCircle loadingCircleFormatting;
        private LoadingCircle loadingCircleCollectButton;
        private LoadingCircle loadingCircleRegisterButton;
        private ToolStripStatusLabel aboutLabelButton;
        private GroupBox groupBoxServerStatus;
        private LoadingCircle loadingCircleServerOperationalStatus;
        private readonly BackgroundWorker backgroundWorker1;
        private ToolStripStatusLabel logLabelButton;
        private readonly LogGenerator log;
        private TaskbarManager tbProgMain;

        #endregion

        //Sets service mode to format
        private void FormatButton1_CheckedChanged(object sender, EventArgs e)
        {
            serviceTypeURL = ConstantsDLL.Properties.Resources.formatURL;
        }

        //Sets service mode to maintenance
        private void MaintenanceButton2_CheckedChanged(object sender, EventArgs e)
        {
            serviceTypeURL = ConstantsDLL.Properties.Resources.maintenanceURL;
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
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.windows10)) //If Windows 10/11
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark); //Sets context menus to dark
                }
                DarkTheme(); //Sets dark theme
            }
            else
            {
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.windows10)) //If Windows 10/11
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
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.windows10))
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
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.windows10))
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

            lblBrand.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblModel.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblSerialNumber.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblProcessor.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblRam.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblStorageSize.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblStorageType.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblMediaOperationMode.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblOperatingSystem.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblHostname.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblMacAddress.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblIpAddress.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblFwVersion.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblFwType.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblTpmVersion.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblVirtualizationTechnology.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblSecureBoot.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblVideoCard.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblVirtualizationTechnology.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblSmartStatus.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblServerIP.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblServerPort.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            lblAgentName.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;

            lblInstallSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            lblMaintenanceSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            lblFixedBrand.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedModel.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedSerialNumber.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedProcessor.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedRam.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedStorageSize.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedOperatingSystem.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedHostname.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedMacAddress.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedIpAddress.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedAssetNumber.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedSealNumber.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedBuilding.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedRoomNumber.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedAdRegistered.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedServiceDate.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedStandard.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedInUse.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedTag.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedHwType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedServerOperationalStatus.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedServerPort.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedRoomLetter.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedFwVersion.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedFwType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedStorageType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedVideoCard.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblProgressBarPercent.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedMediaOperationMode.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedTicketNumber.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedSecureBoot.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedVirtualizationTechnology.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedWho.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedMandatoryMain.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatoryAssetNumber.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatoryRoomNumber.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatoryBuilding.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatoryInUse.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatoryHwType.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatoryTag.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatoryWho.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatoryServiceType.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedSmartStatus.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedTpmVersion.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedBatteryChange.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedMandatoryBatteryChange.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedMandatoryTicketNumber.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
            lblFixedServerIP.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            lblFixedAgentName.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            if (offlineMode)
            {
                lblServerIP.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblServerPort.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblAgentName.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }
            loadingCircleCollectButton.BackColor = StringsAndConstants.INACTIVE_SYSTEM_BUTTON_COLOR;
            loadingCircleRegisterButton.BackColor = StringsAndConstants.INACTIVE_SYSTEM_BUTTON_COLOR;

            textBoxAssetNumber.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxAssetNumber.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxSealNumber.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxSealNumber.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxRoomNumber.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxRoomNumber.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxRoomLetter.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxRoomLetter.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxFixedFormattingRadio.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxFixedFormattingRadio.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            textBoxFixedMaintenanceRadio.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxFixedMaintenanceRadio.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            textBoxTicketNumber.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            textBoxTicketNumber.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;

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
            comboBoxHwType.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxHwType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxHwType.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxHwType.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxHwType.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
            comboBoxBatteryChange.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxBatteryChange.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxBatteryChange.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
            comboBoxBatteryChange.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            comboBoxBatteryChange.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
            comboBoxThemeButton.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            comboBoxThemeButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            radioButtonEmployee.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            radioButtonStudent.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            radioButtonFormatting.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            radioButtonMaintenance.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            registerButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            registerButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            registerButton.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
            registerButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            collectButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            collectButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            collectButton.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
            collectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            ApcsButton.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            ApcsButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            ApcsButton.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
            ApcsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;

            groupBoxHwData.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            groupBoxAssetData.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            groupBoxServiceType.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            groupBoxRegistryStatus.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            groupBoxHwData.Paint += CustomColors.GroupBox_PaintLightTheme;
            groupBoxAssetData.Paint += CustomColors.GroupBox_PaintLightTheme;
            groupBoxServiceType.Paint += CustomColors.GroupBox_PaintLightTheme;
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
            logLabelButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            logLabelButton.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            aboutLabelButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            aboutLabelButton.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            statusStrip1.Renderer = new ModifiedToolStripProfessionalLightTheme();

            toolStripAutoTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_autotheme_light_path));
            toolStripLightTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_lighttheme_light_path));
            toolStripDarkTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_darktheme_light_path));

            comboBoxThemeButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_autotheme_light_path));
            logLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_log_light_path));
            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_about_light_path));

            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.main_banner_light_path));
            iconImgBrand.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_brand_light_path));
            iconImgModel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_model_light_path));
            iconImgSerialNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_serial_no_light_path));
            iconImgProcessor.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_cpu_light_path));
            iconImgRam.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ram_light_path));
            iconImgStorageSize.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_disk_size_light_path));
            iconImgStorageType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_hdd_light_path));
            iconImgMediaOperationMode.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ahci_light_path));
            iconImgVideoCard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_gpu_light_path));
            iconImgOperatingSystem.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_windows_light_path));
            iconImgHostname.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_hostname_light_path));
            iconImgMacAddress.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_mac_light_path));
            iconImgIpAddress.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ip_light_path));
            iconImgFwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_bios_light_path));
            iconImgFwVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_bios_version_light_path));
            iconImgSecureBoot.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_secure_boot_light_path));
            iconImgAssetNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_patr_light_path));
            iconImgSealNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_seal_light_path));
            iconImgRoomNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_room_light_path));
            iconImgBuilding.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_building_light_path));
            iconImgAdRegistered.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_server_light_path));
            iconImgStandard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_standard_light_path));
            iconImgServiceDate.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_service_light_path));
            iconImgRoomLetter.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_letter_light_path));
            iconImgInUse.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_in_use_light_path));
            iconImgTag.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_sticker_light_path));
            iconImgHwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_type_light_path));
            iconImgVirtualizationTechnology.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_VT_x_light_path));
            iconImgWho.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_who_light_path));
            iconImgSmartStatus.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_smart_light_path));
            iconImgTpmVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_tpm_light_path));
            iconImgBatteryChange.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_cmos_battery_light_path));
            iconImgTicketNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ticket_light_path));
        }

        //Sets a dark theme for the UI
        private void DarkTheme()
        {
            BackColor = StringsAndConstants.DARK_BACKGROUND;

            lblBrand.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblModel.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblSerialNumber.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblProcessor.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblRam.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblStorageSize.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblStorageType.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblMediaOperationMode.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblOperatingSystem.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblHostname.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblMacAddress.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblIpAddress.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblFwVersion.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblFwType.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblTpmVersion.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblVirtualizationTechnology.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblSecureBoot.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblVideoCard.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblVirtualizationTechnology.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblSmartStatus.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblServerIP.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblServerPort.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            lblAgentName.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;

            lblInstallSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            lblMaintenanceSince.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            lblFixedBrand.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedModel.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedSerialNumber.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedProcessor.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedRam.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedStorageSize.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedOperatingSystem.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedHostname.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedMacAddress.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedIpAddress.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedAssetNumber.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedSealNumber.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedBuilding.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedRoomNumber.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedAdRegistered.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedServiceDate.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedStandard.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedInUse.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedTag.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedHwType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedServerOperationalStatus.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedServerPort.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedRoomLetter.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedFwVersion.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedFwType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedStorageType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblProgressBarPercent.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedVideoCard.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedMediaOperationMode.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedTicketNumber.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedSecureBoot.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedVirtualizationTechnology.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedWho.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedMandatoryMain.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatoryAssetNumber.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatoryRoomNumber.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatoryBuilding.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatoryInUse.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatoryHwType.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatoryTag.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatoryWho.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatoryServiceType.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedSmartStatus.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedTpmVersion.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedBatteryChange.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedMandatoryBatteryChange.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedMandatoryTicketNumber.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
            lblFixedServerIP.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            lblFixedAgentName.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            if (offlineMode)
            {
                lblServerIP.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblServerPort.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblAgentName.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }
            loadingCircleCollectButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            loadingCircleRegisterButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;

            textBoxAssetNumber.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxAssetNumber.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxSealNumber.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxSealNumber.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxRoomNumber.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxRoomNumber.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxRoomLetter.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxRoomLetter.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxFixedFormattingRadio.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxFixedFormattingRadio.BackColor = StringsAndConstants.DARK_BACKGROUND;
            textBoxFixedMaintenanceRadio.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxFixedMaintenanceRadio.BackColor = StringsAndConstants.DARK_BACKGROUND;
            textBoxTicketNumber.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            textBoxTicketNumber.BackColor = StringsAndConstants.DARK_BACKCOLOR;

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
            comboBoxHwType.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxHwType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxHwType.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxHwType.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxBatteryChange.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxBatteryChange.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxBatteryChange.BorderColor = StringsAndConstants.DARK_FORECOLOR;
            comboBoxBatteryChange.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            comboBoxThemeButton.BackColor = StringsAndConstants.DARK_BACKGROUND;
            comboBoxThemeButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            radioButtonEmployee.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            radioButtonStudent.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            radioButtonFormatting.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            radioButtonMaintenance.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            registerButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            registerButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            registerButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            collectButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            collectButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            collectButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            collectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ApcsButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            ApcsButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            ApcsButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            ApcsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            groupBoxHwData.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            groupBoxAssetData.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            groupBoxServiceType.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            groupBoxRegistryStatus.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            groupBoxHwData.Paint += CustomColors.GroupBox_PaintDarkTheme;
            groupBoxAssetData.Paint += CustomColors.GroupBox_PaintDarkTheme;
            groupBoxServiceType.Paint += CustomColors.GroupBox_PaintDarkTheme;
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
            logLabelButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            logLabelButton.BackColor = StringsAndConstants.DARK_BACKGROUND;
            aboutLabelButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            aboutLabelButton.BackColor = StringsAndConstants.DARK_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.DARK_BACKGROUND;

            statusStrip1.Renderer = new ModifiedToolStripProfessionalDarkTheme();

            toolStripAutoTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_autotheme_dark_path));
            toolStripLightTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_lighttheme_dark_path));
            toolStripDarkTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_darktheme_dark_path));

            comboBoxThemeButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_autotheme_dark_path));
            logLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_log_dark_path));
            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_about_dark_path));

            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.main_banner_dark_path));
            iconImgBrand.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_brand_dark_path));
            iconImgModel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_model_dark_path));
            iconImgSerialNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_serial_no_dark_path));
            iconImgProcessor.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_cpu_dark_path));
            iconImgRam.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ram_dark_path));
            iconImgStorageSize.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_disk_size_dark_path));
            iconImgStorageType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_hdd_dark_path));
            iconImgMediaOperationMode.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ahci_dark_path));
            iconImgVideoCard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_gpu_dark_path));
            iconImgOperatingSystem.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_windows_dark_path));
            iconImgHostname.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_hostname_dark_path));
            iconImgMacAddress.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_mac_dark_path));
            iconImgIpAddress.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ip_dark_path));
            iconImgFwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_bios_dark_path));
            iconImgFwVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_bios_version_dark_path));
            iconImgSecureBoot.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_secure_boot_dark_path));
            iconImgAssetNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_patr_dark_path));
            iconImgSealNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_seal_dark_path));
            iconImgRoomNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_room_dark_path));
            iconImgBuilding.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_building_dark_path));
            iconImgAdRegistered.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_server_dark_path));
            iconImgStandard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_standard_dark_path));
            iconImgServiceDate.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_service_dark_path));
            iconImgRoomLetter.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_letter_dark_path));
            iconImgInUse.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_in_use_dark_path));
            iconImgTag.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_sticker_dark_path));
            iconImgHwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_type_dark_path));
            iconImgVirtualizationTechnology.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_VT_x_dark_path));
            iconImgWho.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_who_dark_path));
            iconImgSmartStatus.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_smart_dark_path));
            iconImgTpmVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_tpm_dark_path));
            iconImgBatteryChange.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_cmos_battery_dark_path));
            iconImgTicketNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.icon_ticket_dark_path));
        }

        //Sets highlight about label when hovering with the mouse
        private void AboutLabel_MouseEnter(object sender, EventArgs e)
        {
            aboutLabelButton.ForeColor = StringsAndConstants.HIGHLIGHT_LABEL_COLOR;
        }

        //Resets highlight about label when hovering with the mouse
        private void AboutLabel_MouseLeave(object sender, EventArgs e)
        {
            aboutLabelButton.ForeColor = !themeBool ? StringsAndConstants.LIGHT_FORECOLOR : StringsAndConstants.DARK_FORECOLOR;
        }

        //Sets highlight log label when hovering with the mouse
        private void LogLabel_MouseEnter(object sender, EventArgs e)
        {
            logLabelButton.ForeColor = StringsAndConstants.HIGHLIGHT_LABEL_COLOR;
        }

        //Resets highlight log label when hovering with the mouse
        private void LogLabel_MouseLeave(object sender, EventArgs e)
        {
            logLabelButton.ForeColor = !themeBool ? StringsAndConstants.LIGHT_FORECOLOR : StringsAndConstants.DARK_FORECOLOR;
        }

        //Opens the log file
        private void LogLabelButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_OPENING_LOG, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
#if DEBUG
            System.Diagnostics.Process.Start(parametersList[2][0] + ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.dev_status + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#else
            System.Diagnostics.Process.Start(parametersList[2][0] + ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#endif
        }

        //Opens the About box
        private void AboutLabelButton_Click(object sender, EventArgs e)
        {
            AboutBox aboutForm = new AboutBox(parametersList, themeBool);
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.windows10))
            {
                DarkNet.Instance.SetWindowThemeForms(aboutForm, Theme.Auto);
            }
            _ = aboutForm.ShowDialog();
        }

        //Opens the selected webpage, according to the IP and port specified in the comboboxes
        private void ApcsButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_VIEW_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            _ = System.Diagnostics.Process.Start(ConstantsDLL.Properties.Resources.HTTP + serverIP + ":" + serverPort);
        }

        //Handles the closing of the current form
        private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CLOSING_MAINFORM, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_MISC), ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Deletes downloaded json files
            File.Delete(StringsAndConstants.modelFilePath);
            File.Delete(StringsAndConstants.credentialsFilePath);
            File.Delete(StringsAndConstants.assetFilePath);
            File.Delete(StringsAndConstants.configFilePath);

            //Kills Webview2 instance
            webView2Control.Dispose();
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        //Loads the form, sets some combobox values, create timers (1000 ms cadence), and triggers a hardware collection
        private async void MainForm_Load(object sender, EventArgs e)
        {
            //Define loading circle parameters
            #region

            switch (MiscMethods.GetWindowsScaling())
            {
                case 100:
                    //Init loading circles parameters for 100% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    break;
            }

            //Sets loading circle color and rotation speed
            #region
            loadingCircleBrand.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleModel.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleSerialNumber.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleProcessor.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleRam.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleStorageSize.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleSmartStatus.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleStorageType.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleMediaOperationMode.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleVideoCard.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleOperatingSystem.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleHostname.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleMacAddress.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleIpAddress.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleFwType.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleFwVersion.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleSecureBoot.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleVirtualizationTechnology.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleTpmVersion.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleFormatting.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleMaintenance.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleCollectButton.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleRegisterButton.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleServerOperationalStatus.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircleBrand.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleModel.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleSerialNumber.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleProcessor.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleRam.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleStorageSize.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleSmartStatus.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleStorageType.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleMediaOperationMode.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleVideoCard.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleOperatingSystem.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleHostname.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleMacAddress.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleIpAddress.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleFwType.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleFwVersion.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleSecureBoot.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleVirtualizationTechnology.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleTpmVersion.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleFormatting.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleMaintenance.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleCollectButton.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleRegisterButton.Color = StringsAndConstants.rotatingCircleColor;
            loadingCircleServerOperationalStatus.Color = StringsAndConstants.rotatingCircleColor;
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
            timerAlertHostname.Tick += new EventHandler(AlertFlashTextHostname);
            timerAlertMediaOperationMode.Tick += new EventHandler(AlertFlashTextMediaOperationMode);
            timerAlertSecureBoot.Tick += new EventHandler(AlertFlashTextSecureBoot);
            timerAlertFwVersion.Tick += new EventHandler(FlashTextFwVersion);
            timerAlertNetConnectivity.Tick += new EventHandler(FlashTextNetConnectivity);
            timerAlertFwType.Tick += new EventHandler(FlashTextBIOSType);
            timerAlertVirtualizationTechnology.Tick += new EventHandler(AlertFlashTextVirtualizationTechnology);
            timerAlertSmartStatus.Tick += new EventHandler(AlertFlashTextSmartStatus);
            timerAlertTpmVersion.Tick += new EventHandler(AlertFlashTextTpmVersion);
            timerAlertRamAmount.Tick += new EventHandler(AlertFlashTextRamAmount);

            timerAlertHostname.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timerAlertMediaOperationMode.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timerAlertSecureBoot.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timerAlertFwVersion.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timerAlertNetConnectivity.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timerAlertFwType.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timerAlertVirtualizationTechnology.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timerAlertSmartStatus.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timerAlertTpmVersion.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            timerAlertRamAmount.Interval = Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL);
            #endregion

            if (!offlineMode)
            {
                lblAgentName.Text = agentData[1].ToUpper(); //Prints agent name
            }
            lblServerIP.Text = serverIP; //Prints IP address
            lblServerPort.Text = serverPort; //Prints port number
            dateTimePickerServiceDate.MaxDate = DateTime.Today; //Define max date of datetimepicker to current day
            FormClosing += MainForm_Closing; //Handles Form closing
            tbProgMain = TaskbarManager.Instance; //Handles taskbar progress bar
            CollectButton_Click(sender, e); //Start collecting
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
        private void AlertFlashTextHostname(object myObject, EventArgs myEventArgs)
        {
            lblHostname.ForeColor = lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the MediaOperations label to flash in red
        private void AlertFlashTextMediaOperationMode(object myobject, EventArgs myEventArgs)
        {
            lblMediaOperationMode.ForeColor = lblMediaOperationMode.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblMediaOperationMode.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the Secure Boot label to flash in red
        private void AlertFlashTextSecureBoot(object myobject, EventArgs myEventArgs)
        {
            lblSecureBoot.ForeColor = lblSecureBoot.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblSecureBoot.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the VT label to flash in red
        private void AlertFlashTextVirtualizationTechnology(object myobject, EventArgs myEventArgs)
        {
            lblVirtualizationTechnology.ForeColor = lblVirtualizationTechnology.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblVirtualizationTechnology.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the SMART label to flash in red
        private void AlertFlashTextSmartStatus(object myobject, EventArgs myEventArgs)
        {
            lblSmartStatus.ForeColor = lblSmartStatus.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblSmartStatus.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the BIOS Version label to flash in red
        private void FlashTextFwVersion(object myobject, EventArgs myEventArgs)
        {
            lblFwVersion.ForeColor = lblFwVersion.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblFwVersion.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the Mac and IP labels to flash in red
        private void FlashTextNetConnectivity(object myobject, EventArgs myEventArgs)
        {
            if (lblMacAddress.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true)
            {
                lblMacAddress.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblIpAddress.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            }
            else if (lblMacAddress.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false)
            {
                lblMacAddress.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblIpAddress.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            }
            else
            {
                lblMacAddress.ForeColor = StringsAndConstants.ALERT_COLOR;
                lblIpAddress.ForeColor = StringsAndConstants.ALERT_COLOR;
            }
        }

        //Sets the BIOS Firmware Type label to flash in red
        private void FlashTextBIOSType(object myobject, EventArgs myEventArgs)
        {
            lblFwType.ForeColor = lblFwType.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblFwType.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the Physical Memory label to flash in red
        private void AlertFlashTextRamAmount(object myobject, EventArgs myEventArgs)
        {
            lblRam.ForeColor = lblRam.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblRam.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        //Sets the TPM label to flash in red
        private void AlertFlashTextTpmVersion(object myobject, EventArgs myEventArgs)
        {
            lblTpmVersion.ForeColor = lblTpmVersion.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblTpmVersion.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
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
            lblBrand.Text = ConstantsDLL.Properties.Resources.DASH;
            lblModel.Text = ConstantsDLL.Properties.Resources.DASH;
            lblSerialNumber.Text = ConstantsDLL.Properties.Resources.DASH;
            lblProcessor.Text = ConstantsDLL.Properties.Resources.DASH;
            lblRam.Text = ConstantsDLL.Properties.Resources.DASH;
            lblStorageSize.Text = ConstantsDLL.Properties.Resources.DASH;
            lblSmartStatus.Text = ConstantsDLL.Properties.Resources.DASH;
            lblStorageType.Text = ConstantsDLL.Properties.Resources.DASH;
            lblMediaOperationMode.Text = ConstantsDLL.Properties.Resources.DASH;
            lblVideoCard.Text = ConstantsDLL.Properties.Resources.DASH;
            lblOperatingSystem.Text = ConstantsDLL.Properties.Resources.DASH;
            lblHostname.Text = ConstantsDLL.Properties.Resources.DASH;
            lblMacAddress.Text = ConstantsDLL.Properties.Resources.DASH;
            lblIpAddress.Text = ConstantsDLL.Properties.Resources.DASH;
            lblFwVersion.Text = ConstantsDLL.Properties.Resources.DASH;
            lblFwType.Text = ConstantsDLL.Properties.Resources.DASH;
            lblSecureBoot.Text = ConstantsDLL.Properties.Resources.DASH;
            lblVirtualizationTechnology.Text = ConstantsDLL.Properties.Resources.DASH;
            lblTpmVersion.Text = ConstantsDLL.Properties.Resources.DASH;
            collectButton.Text = ConstantsDLL.Properties.Resources.DASH;
            lblServerOperationalStatus.Text = ConstantsDLL.Properties.Resources.DASH;
            #endregion

            //Show loading circles while scanning the hardware
            #region
            loadingCircleBrand.Visible = true;
            loadingCircleModel.Visible = true;
            loadingCircleSerialNumber.Visible = true;
            loadingCircleProcessor.Visible = true;
            loadingCircleRam.Visible = true;
            loadingCircleStorageSize.Visible = true;
            loadingCircleSmartStatus.Visible = true;
            loadingCircleStorageType.Visible = true;
            loadingCircleMediaOperationMode.Visible = true;
            loadingCircleVideoCard.Visible = true;
            loadingCircleOperatingSystem.Visible = true;
            loadingCircleHostname.Visible = true;
            loadingCircleMacAddress.Visible = true;
            loadingCircleIpAddress.Visible = true;
            loadingCircleFwType.Visible = true;
            loadingCircleFwVersion.Visible = true;
            loadingCircleSecureBoot.Visible = true;
            loadingCircleVirtualizationTechnology.Visible = true;
            loadingCircleTpmVersion.Visible = true;
            loadingCircleFormatting.Visible = true;
            loadingCircleMaintenance.Visible = true;
            loadingCircleCollectButton.Visible = true;
            loadingCircleBrand.Active = true;
            loadingCircleModel.Active = true;
            loadingCircleSerialNumber.Active = true;
            loadingCircleProcessor.Active = true;
            loadingCircleRam.Active = true;
            loadingCircleStorageSize.Active = true;
            loadingCircleSmartStatus.Active = true;
            loadingCircleStorageType.Active = true;
            loadingCircleMediaOperationMode.Active = true;
            loadingCircleVideoCard.Active = true;
            loadingCircleOperatingSystem.Active = true;
            loadingCircleHostname.Active = true;
            loadingCircleMacAddress.Active = true;
            loadingCircleIpAddress.Active = true;
            loadingCircleFwType.Active = true;
            loadingCircleFwVersion.Active = true;
            loadingCircleSecureBoot.Active = true;
            loadingCircleVirtualizationTechnology.Active = true;
            loadingCircleTpmVersion.Active = true;
            loadingCircleFormatting.Active = true;
            loadingCircleMaintenance.Active = true;
            loadingCircleCollectButton.Active = true;
            #endregion

            if (!offlineMode)
            {
                loadingCircleServerOperationalStatus.Visible = true;
                loadingCircleServerOperationalStatus.Active = true;

                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PINGGING_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

                //Feches model info from server
                serverOnline = await ModelFileReader.CheckHostMT(serverIP, serverPort);

                if (serverOnline && serverPort != string.Empty)
                {
                    loadingCircleServerOperationalStatus.Visible = false;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_ONLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    lblServerOperationalStatus.Text = Strings.ONLINE;
                    lblServerOperationalStatus.ForeColor = StringsAndConstants.ONLINE_ALERT;
                }
                else
                {
                    loadingCircleServerOperationalStatus.Visible = false;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_OFFLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    lblServerOperationalStatus.Text = Strings.OFFLINE;
                    lblServerOperationalStatus.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                }
            }
            else
            {
                loadingCircleServerOperationalStatus.Visible = false;
                loadingCircleServerOperationalStatus.Active = false;
                lblServerIP.Text = lblServerPort.Text = lblAgentName.Text = lblServerOperationalStatus.Text = Strings.OFFLINE_MODE_ACTIVATED;
                lblServerIP.ForeColor = lblServerPort.ForeColor = lblAgentName.ForeColor = lblServerOperationalStatus.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }

            //Alerts stop blinking and resets red color
            timerAlertHostname.Enabled = false;
            timerAlertMediaOperationMode.Enabled = false;
            timerAlertSecureBoot.Enabled = false;
            timerAlertFwVersion.Enabled = false;
            timerAlertNetConnectivity.Enabled = false;
            timerAlertFwType.Enabled = false;
            timerAlertVirtualizationTechnology.Enabled = false;
            timerAlertSmartStatus.Enabled = false;
            timerAlertTpmVersion.Enabled = false;
            timerAlertRamAmount.Enabled = false;

            //Resets the colors while scanning the hardware
            if (themeBool)
            {
                lblHostname.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblMediaOperationMode.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblSecureBoot.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblFwVersion.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblFwType.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblVirtualizationTechnology.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblSmartStatus.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblRam.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblTpmVersion.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblMacAddress.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblIpAddress.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            }
            else
            {
                lblHostname.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblMediaOperationMode.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblSecureBoot.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblFwVersion.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblFwType.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblVirtualizationTechnology.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblSmartStatus.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblRam.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblTpmVersion.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblMacAddress.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblIpAddress.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
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
            brand = HardwareInfo.GetBrand();
            if (brand == ConstantsDLL.Properties.Resources.ToBeFilledByOEM || brand == string.Empty)
            {
                brand = HardwareInfo.GetBrandAlt();
            }

            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_BM, brand, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for PC model
            model = HardwareInfo.GetModel();
            if (model == ConstantsDLL.Properties.Resources.ToBeFilledByOEM || model == string.Empty)
            {
                model = HardwareInfo.GetModelAlt();
            }

            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MODEL, model, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for motherboard Serial number
            serialNumber = HardwareInfo.GetSerialNumber();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SERIALNO, serialNumber, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for CPU information
            processor = HardwareInfo.GetProcessorInfo();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PROCNAME, processor, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for RAM amount and total number of slots
            ram = HardwareInfo.GetRam() + " (" + HardwareInfo.GetNumFreeRamSlots(Convert.ToInt32(HardwareInfo.GetNumRamSlots())) +
                Strings.slots_of + HardwareInfo.GetNumRamSlots() + Strings.occupied + ")";
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PM, ram, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for Storage size
            storageSize = HardwareInfo.GetStorageSize();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_HDSIZE, storageSize, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for SMART status
            smartStatus = HardwareInfo.GetSMARTStatus();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SMART, smartStatus, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for Storage type
            storageType = HardwareInfo.GetStorageType();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MEDIATYPE, storageType, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for Media Operation (IDE/AHCI/NVME)
            mediaOperationMode = HardwareInfo.GetMediaOperationMode();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MEDIAOP, mediaOperationMode, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for GPU information
            videoCard = HardwareInfo.GetVideoCardInfo();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_GPUINFO, videoCard, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for OS infomation
            operatingSystem = HardwareInfo.GetOSString();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_OS, operatingSystem, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for Hostname
            hostname = HardwareInfo.GetHostname();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_HOSTNAME, hostname, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for MAC Address
            macAddress = HardwareInfo.GetMacAddress();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MAC, macAddress, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for IP Address
            ipAddress = HardwareInfo.GetIpAddress();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_IP, ipAddress, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for firmware type
            fwType = HardwareInfo.GetFwType();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_BIOSTYPE, fwType, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for Secure Boot status
            secureBoot = HardwareInfo.GetSecureBoot();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SECBOOT, secureBoot, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for BIOS version
            fwVersion = HardwareInfo.GetFirmwareVersion();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_BIOS, fwVersion, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for VT status
            virtualizationTechnology = HardwareInfo.GetVirtualizationTechnology();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_VT, virtualizationTechnology, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            //Scans for TPM status
            tpmVersion = HardwareInfo.GetTPMStatus();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_TPM, tpmVersion, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_END_COLLECTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
        }

        //Prints the collected data into the form labels, warning the agent when there are forbidden modes
        private async Task PrintHardwareData()
        {
            //Hides loading circles after scanning the hardware
            #region
            loadingCircleBrand.Visible = false;
            loadingCircleModel.Visible = false;
            loadingCircleSerialNumber.Visible = false;
            loadingCircleProcessor.Visible = false;
            loadingCircleRam.Visible = false;
            loadingCircleStorageSize.Visible = false;
            loadingCircleSmartStatus.Visible = false;
            loadingCircleStorageType.Visible = false;
            loadingCircleMediaOperationMode.Visible = false;
            loadingCircleVideoCard.Visible = false;
            loadingCircleOperatingSystem.Visible = false;
            loadingCircleHostname.Visible = false;
            loadingCircleMacAddress.Visible = false;
            loadingCircleIpAddress.Visible = false;
            loadingCircleFwType.Visible = false;
            loadingCircleFwVersion.Visible = false;
            loadingCircleSecureBoot.Visible = false;
            loadingCircleVirtualizationTechnology.Visible = false;
            loadingCircleTpmVersion.Visible = false;
            loadingCircleFormatting.Visible = false;
            loadingCircleMaintenance.Visible = false;
            loadingCircleBrand.Active = false;
            loadingCircleModel.Active = false;
            loadingCircleSerialNumber.Active = false;
            loadingCircleProcessor.Active = false;
            loadingCircleRam.Active = false;
            loadingCircleStorageSize.Active = false;
            loadingCircleSmartStatus.Active = false;
            loadingCircleStorageType.Active = false;
            loadingCircleMediaOperationMode.Active = false;
            loadingCircleVideoCard.Active = false;
            loadingCircleOperatingSystem.Active = false;
            loadingCircleHostname.Active = false;
            loadingCircleMacAddress.Active = false;
            loadingCircleIpAddress.Active = false;
            loadingCircleFwType.Active = false;
            loadingCircleFwVersion.Active = false;
            loadingCircleSecureBoot.Active = false;
            loadingCircleVirtualizationTechnology.Active = false;
            loadingCircleTpmVersion.Active = false;
            loadingCircleFormatting.Active = false;
            loadingCircleMaintenance.Active = false;
            #endregion

            //Prints fetched data into labels
            #region
            lblBrand.Text = brand;
            lblModel.Text = model;
            lblSerialNumber.Text = serialNumber;
            lblProcessor.Text = processor;
            lblRam.Text = ram;
            lblStorageSize.Text = storageSize;
            lblSmartStatus.Text = smartStatus;
            lblStorageType.Text = storageType;
            lblVideoCard.Text = videoCard;
            lblOperatingSystem.Text = operatingSystem;
            lblHostname.Text = hostname;
            lblMacAddress.Text = macAddress;
            lblIpAddress.Text = ipAddress;
            lblFwVersion.Text = fwVersion;

            lblMediaOperationMode.Text = parametersList[8][Convert.ToInt32(mediaOperationMode)];
            lblFwType.Text = parametersList[6][Convert.ToInt32(fwType)];
            lblSecureBoot.Text = StringsAndConstants.listStates[Convert.ToInt32(parametersList[9][Convert.ToInt32(secureBoot)])];
            lblVirtualizationTechnology.Text = StringsAndConstants.listStates[Convert.ToInt32(parametersList[10][Convert.ToInt32(virtualizationTechnology)])];
            lblTpmVersion.Text = parametersList[7][Convert.ToInt32(tpmVersion)];

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
                string[] modelJsonStr = await ModelFileReader.FetchInfoMT(brand, model, fwType, tpmVersion, mediaOperationMode, serverIP, serverPort);

                //If hostname is the default one and its enforcement is enabled
                if (enforcementList[3] == "true" && hostname.Equals(Strings.DEFAULT_HOSTNAME))
                {
                    pass = false;
                    lblHostname.Text += Strings.HOSTNAME_ALERT;
                    timerAlertHostname.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.HOSTNAME_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //If model Json file does exist, mediaOpMode enforcement is enabled, and the mode is incorrect
                if (enforcementList[2] == "true" && modelJsonStr != null && modelJsonStr[3].Equals("false"))
                {
                    pass = false;
                    lblMediaOperationMode.Text += Strings.MEDIA_OPERATION_ALERT;
                    timerAlertMediaOperationMode.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.MEDIA_OPERATION_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //The section below contains the exception cases for Secure Boot enforcement, if it is enabled
                if (enforcementList[6] == "true" && StringsAndConstants.listStates[Convert.ToInt32(secureBoot)] == ConstantsDLL.Properties.Strings.deactivated &&
                    !lblVideoCard.Text.Contains(ConstantsDLL.Properties.Resources.nonSecBootGPU1) &&
                    !lblVideoCard.Text.Contains(ConstantsDLL.Properties.Resources.nonSecBootGPU2))
                {
                    pass = false;
                    lblSecureBoot.Text += Strings.SECURE_BOOT_ALERT;
                    timerAlertSecureBoot.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.SECURE_BOOT_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //If model Json file does not exist and server is unreachable
                if (modelJsonStr == null)
                {
                    if (!offlineMode)
                    {
                        pass = false;
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                        _ = MessageBox.Show(Strings.DATABASE_REACH_ERROR, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                //If model Json file does exist, firmware version enforcement is enabled, and the version is incorrect
                if (enforcementList[5] == "true" && modelJsonStr != null && !fwVersion.Contains(modelJsonStr[0]))
                {
                    if (!modelJsonStr[0].Equals("-1"))
                    {
                        pass = false;
                        lblFwVersion.Text += Strings.BIOS_VERSION_ALERT;
                        timerAlertFwVersion.Enabled = true;
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.BIOS_VERSION_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    }
                }
                //If model Json file does exist, firmware type enforcement is enabled, and the type is incorrect
                if (enforcementList[4] == "true" && modelJsonStr != null && modelJsonStr[1].Equals("false"))
                {
                    pass = false;
                    lblFwType.Text += Strings.FIRMWARE_TYPE_ALERT;
                    timerAlertFwType.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.FIRMWARE_TYPE_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //If there is no MAC address assigned
                if (string.IsNullOrEmpty(macAddress))
                {
                    if (!offlineMode) //If it's not in offline mode
                    {
                        pass = false;
                        lblMacAddress.Text = Strings.NETWORK_ERROR; //Prints a network error
                        lblIpAddress.Text = Strings.NETWORK_ERROR; //Prints a network error
                        timerAlertNetConnectivity.Enabled = true;
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.NETWORK_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    }
                    else //If it's in offline mode
                    {
                        lblMacAddress.Text = Strings.OFFLINE_MODE_ACTIVATED; //Specifies offline mode
                        lblIpAddress.Text = Strings.OFFLINE_MODE_ACTIVATED; //Specifies offline mode
                    }
                }
                //If Virtualization Technology is disabled for UEFI and its enforcement is enabled
                if (enforcementList[7] == "true" && StringsAndConstants.listStates[Convert.ToInt32(virtualizationTechnology)] == ConstantsDLL.Properties.Strings.deactivated)
                {
                    pass = false;
                    lblVirtualizationTechnology.Text += Strings.VT_ALERT;
                    timerAlertVirtualizationTechnology.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.VT_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //If Smart status is not OK and its enforcement is enabled
                if (enforcementList[1] == "true" && !smartStatus.Contains(ConstantsDLL.Properties.Resources.ok))
                {
                    pass = false;
                    lblSmartStatus.Text += Strings.SMART_FAIL;
                    timerAlertSmartStatus.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.SMART_FAIL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //If model Json file does exist, TPM enforcement is enabled, and TPM version is incorrect
                if (enforcementList[8] == "true" && modelJsonStr != null && modelJsonStr[2].Equals("false"))
                {
                    pass = false;
                    lblTpmVersion.Text += Strings.TPM_ERROR;
                    timerAlertTpmVersion.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.TPM_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetRamAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, and its limit enforcement is enabled, shows an alert
                if (enforcementList[0] == "true" && d < 4.0 && Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    lblRam.Text += Strings.NOT_ENOUGH_MEMORY;
                    timerAlertRamAmount.Enabled = true;
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.NOT_ENOUGH_MEMORY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                }
                //If RAM is more than 4GB and OS is x86, and its limit enforcement is enabled, shows an alert
                if (enforcementList[0] == "true" && d > 4.0 && !Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    lblRam.Text += Strings.TOO_MUCH_MEMORY;
                    timerAlertRamAmount.Enabled = true;
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

        //Triggers when the form opens, and when the agent clicks to collect
        private void CollectButton_Click(object sender, EventArgs e)
        {
            progressBar1.SetState(1);
            tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
            webView2Control.Visible = false;
            Collecting();
            ApcsButton.Enabled = false;
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
            lblProgressBarPercent.Text = e.ProgressPercentage.ToString() + "%";
        }

        //Runs when the collection ends, ending the thread
        private async void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Task p = PrintHardwareData();
            await p;

            if (!offlineMode)
            {
                ApcsButton.Enabled = true; //Enables accessSystem button
                registerButton.Enabled = true; //Enables register button
            }
            loadingCircleCollectButton.Visible = false; //Hides loading circle
            collectButton.Enabled = true; //Enables collect button
            collectButton.Text = Strings.FETCH_AGAIN; //Updates collect button text
        }

        //Attributes the data collected previously to the variables which will inside the URL for registration
        private void AttrHardwareData()
        {
            serverArgs[10] = brand;
            serverArgs[11] = model;
            serverArgs[12] = serialNumber;
            serverArgs[13] = processor;
            serverArgs[14] = ram;
            serverArgs[15] = storageSize;
            serverArgs[16] = operatingSystem;
            serverArgs[17] = hostname;
            serverArgs[18] = fwVersion;
            serverArgs[19] = macAddress;
            serverArgs[20] = ipAddress;
            serverArgs[24] = fwType;
            serverArgs[25] = storageType;
            serverArgs[26] = videoCard;
            serverArgs[27] = mediaOperationMode;
            serverArgs[28] = Array.IndexOf(parametersList[9], secureBoot).ToString();
            serverArgs[29] = Array.IndexOf(parametersList[10], virtualizationTechnology).ToString();
            serverArgs[30] = tpmVersion;
        }

        //Loads webView2 component
        public async Task LoadWebView2()
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_START_LOADING_WEBVIEW2, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            CoreWebView2Environment webView2Environment = Environment.Is64BitOperatingSystem
                ? await CoreWebView2Environment.CreateAsync(ConstantsDLL.Properties.Resources.WEBVIEW2_SYSTEM_PATH_X64 + MiscMethods.GetWebView2Version(), System.IO.Path.GetTempPath())
                : await CoreWebView2Environment.CreateAsync(ConstantsDLL.Properties.Resources.WEBVIEW2_SYSTEM_PATH_X86 + MiscMethods.GetWebView2Version(), System.IO.Path.GetTempPath());
            await webView2Control.EnsureCoreWebView2Async(webView2Environment);
            webView2Control.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView2Control.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_END_LOADING_WEBVIEW2, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
        }

        //Sends hardware info to the specified server
        public void ServerSendInfo(string[] serverArgs)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTERING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            webView2Control.CoreWebView2.Navigate(ConstantsDLL.Properties.Resources.HTTP + serverArgs[0] + ":" + serverArgs[1] + "/" + serverArgs[2] + ".php"
                + ConstantsDLL.Properties.Resources.phpAssetNumber + serverArgs[3]
                + ConstantsDLL.Properties.Resources.phpSealNumber + serverArgs[4]
                + ConstantsDLL.Properties.Resources.phpRoom + serverArgs[5]
                + ConstantsDLL.Properties.Resources.phpBuilding + serverArgs[6]
                + ConstantsDLL.Properties.Resources.phpAdRegistered + serverArgs[7]
                + ConstantsDLL.Properties.Resources.phpStandard + serverArgs[8]
                + ConstantsDLL.Properties.Resources.phpServiceDate + serverArgs[9]
                + ConstantsDLL.Properties.Resources.phpPreviousServiceDates + serverArgs[9]
                + ConstantsDLL.Properties.Resources.phpBrand + serverArgs[10]
                + ConstantsDLL.Properties.Resources.phpModel + serverArgs[11]
                + ConstantsDLL.Properties.Resources.phpSerialNumber + serverArgs[12]
                + ConstantsDLL.Properties.Resources.phpProcessor + serverArgs[13]
                + ConstantsDLL.Properties.Resources.phpRam + serverArgs[14]
                + ConstantsDLL.Properties.Resources.phpStorageSize + serverArgs[15]
                + ConstantsDLL.Properties.Resources.phpOperatingSystem + serverArgs[16]
                + ConstantsDLL.Properties.Resources.phpHostname + serverArgs[17]
                + ConstantsDLL.Properties.Resources.phpFwVersion + serverArgs[18]
                + ConstantsDLL.Properties.Resources.phpMacAddress + serverArgs[19]
                + ConstantsDLL.Properties.Resources.phpIpAddress + serverArgs[20]
                + ConstantsDLL.Properties.Resources.phpInUse + serverArgs[21]
                + ConstantsDLL.Properties.Resources.phpTag + serverArgs[22]
                + ConstantsDLL.Properties.Resources.phpHwType + serverArgs[23]
                + ConstantsDLL.Properties.Resources.phpFwType + serverArgs[24]
                + ConstantsDLL.Properties.Resources.phpStorageType + serverArgs[25]
                + ConstantsDLL.Properties.Resources.phpVideoCard + serverArgs[26]
                + ConstantsDLL.Properties.Resources.phpMediaOperationMode + serverArgs[27]
                + ConstantsDLL.Properties.Resources.phpSecureBoot + serverArgs[28]
                + ConstantsDLL.Properties.Resources.phpVirtualizationTechnology + serverArgs[29]
                + ConstantsDLL.Properties.Resources.phpTpmVersion + serverArgs[30]
                + ConstantsDLL.Properties.Resources.phpBatteryChange + serverArgs[31]
                + ConstantsDLL.Properties.Resources.phpTicketNumber + serverArgs[32]
                + ConstantsDLL.Properties.Resources.phpAgent + serverArgs[33]);
        }

        //Runs the registration for the website
        private async void RegisterButton_ClickAsync(object sender, EventArgs e)
        {
            webView2Control.Visible = false;
            tbProgMain.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
            loadingCircleRegisterButton.Visible = true;
            loadingCircleRegisterButton.Active = true;
            registerButton.Text = ConstantsDLL.Properties.Resources.DASH;
            registerButton.Enabled = false;
            ApcsButton.Enabled = false;
            collectButton.Enabled = false;
            AttrHardwareData();

            //If all the mandatory fields are filled and there are no pendencies
            if (!string.IsNullOrWhiteSpace(textBoxAssetNumber.Text) && !string.IsNullOrWhiteSpace(textBoxRoomNumber.Text) && !string.IsNullOrWhiteSpace(textBoxTicketNumber.Text) && comboBoxHwType.SelectedItem != null && comboBoxBuilding.SelectedItem != null && comboBoxInUse.SelectedItem != null && comboBoxTag.SelectedItem != null && comboBoxBatteryChange.SelectedItem != null && (radioButtonEmployee.Checked || radioButtonStudent.Checked) && (radioButtonFormatting.Checked || radioButtonMaintenance.Checked) && pass == true)
            {
                //Attribute variables to an array which will be sent to the server
                serverArgs[0] = serverIP;
                serverArgs[1] = serverPort;
                serverArgs[2] = serviceTypeURL;
                serverArgs[3] = textBoxAssetNumber.Text;
                serverArgs[4] = textBoxSealNumber.Text;
                serverArgs[5] = textBoxRoomLetter.Text != string.Empty ? textBoxRoomNumber.Text + textBoxRoomLetter.Text : textBoxRoomNumber.Text;
                serverArgs[6] = Array.IndexOf(parametersList[4], comboBoxBuilding.SelectedItem.ToString()).ToString();
                serverArgs[7] = comboBoxActiveDirectory.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.listYes0) ? "1" : "0";
                serverArgs[8] = comboBoxStandard.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.listStandardGUIEmployee) ? "0" : "1";
                serverArgs[9] = dateTimePickerServiceDate.Value.ToString(ConstantsDLL.Properties.Resources.dateFormat).Substring(0, 10);
                serverArgs[21] = comboBoxInUse.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.listYes0) ? "1" : "0";
                serverArgs[22] = comboBoxTag.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.listYes0) ? "1" : "0";
                serverArgs[23] = Array.IndexOf(parametersList[5], comboBoxHwType.SelectedItem.ToString()).ToString();
                serverArgs[31] = comboBoxBatteryChange.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.listYes0) ? "1" : "0";
                serverArgs[32] = textBoxTicketNumber.Text;
                serverArgs[33] = agentData[0];

                //Feches asset number data from server
                string[] assetJsonStr = await AssetFileReader.FetchInfoMT(serverArgs[3], serverArgs[0], serverArgs[1]);

                //If patrinony is discarded
                if (assetJsonStr[0] != "false" && assetJsonStr[9] == "1")
                {
                    tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.PC_DROPPED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    _ = MessageBox.Show(ConstantsDLL.Properties.Strings.PC_DROPPED, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                }
                else //If not discarded
                {
                    if (serverOnline && serverPort != string.Empty) //If server is online and port is not null
                    {
                        try //Tries to get the laster register date from the asset number to check if the chosen date is adequate
                        {
                            DateTime registerDate = DateTime.ParseExact(serverArgs[9], ConstantsDLL.Properties.Resources.dateFormat, CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(assetJsonStr[10], ConstantsDLL.Properties.Resources.dateFormat, CultureInfo.InvariantCulture);

                            if (registerDate >= lastRegisterDate) //If chosen date is greater or equal than the last format/maintenance date of the PC, let proceed
                            {
                                webView2Control.Visible = true;
                                ServerSendInfo(serverArgs); //Send info to server
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

                                if (radioButtonFormatting.Checked) //If the format radio button is checked
                                {
                                    MiscMethods.RegCreate(true, serverArgs[9]); //Create reg entries for format and maintenance
                                    lblInstallSince.Text = MiscMethods.SinceLabelUpdate(true);
                                    lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_INSTALLDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                                }
                                else if (radioButtonMaintenance.Checked) //If the maintenance radio button is checked
                                {
                                    MiscMethods.RegCreate(false, serverArgs[9]); //Create reg entry just for maintenance
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
                        catch //If can't retrieve (asset number non existent in the database), register normally
                        {
                            webView2Control.Visible = true;
                            ServerSendInfo(serverArgs); //Send info to server
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

                            if (radioButtonFormatting.Checked) //If the format radio button is checked
                            {
                                MiscMethods.RegCreate(true, serverArgs[9]); //Create reg entries for format and maintenance
                                lblInstallSince.Text = MiscMethods.SinceLabelUpdate(true);
                                lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_INSTALLDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));

                            }
                            else if (radioButtonMaintenance.Checked) //If the maintenance radio button is checked
                            {
                                MiscMethods.RegCreate(false, serverArgs[9]); //Create reg entry just for maintenance
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
            loadingCircleRegisterButton.Visible = false;
            loadingCircleRegisterButton.Active = false;
            registerButton.Text = Strings.REGISTER_AGAIN;
            registerButton.Enabled = true;
            ApcsButton.Enabled = true;
            collectButton.Enabled = true;
        }
    }
}

