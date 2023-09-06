using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using AssetInformationAndRegistration.Properties;
using AssetInformationAndRegistration.WebView;
using ConfigurableQualityPictureBoxDLL;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using MRG.Controls.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary> 
    /// Class for handling Main window tasks and UI
    /// </summary>
    internal partial class MainForm : Form, ITheming
    {
        private int percent, i = 0, nonCompliantCount, leftBound, rightBound;
        private int xPosOS = 0, yPosOS = 0;
        private int xPosFwVersion = 0, yPosFwVersion = 0;
        private int xPosVideoCard = 0, yPosVideoCard = 0;
        private int xPosRam = 0, yPosRam = 0;
        private int xPosProcessor = 0, yPosProcessor = 0;
        private bool isSystemDarkModeEnabled, serverOnline, pass = true, invertOSScroll, invertFwVersionScroll, invertVideoCardScroll, invertRamScroll, invertProcessorScroll;
        private readonly bool offlineMode;
        private readonly string serverIP, serverPort;
        private string serviceTypeURL, brand, model, serialNumber, processor, ram, storageSize, storageSummary, mediaOperationMode, videoCard, operatingSystem, hostname, macAddress, ipAddress, fwVersion, fwType, secureBoot, virtualizationTechnology, tpmVersion;
        private readonly string[] serverArgs = new string[34], agentData = new string[2];
        private string[] assetJsonStr, modelJsonStr;
        private readonly List<string[]> parametersList, jsonServerSettings;
        private readonly List<string> enforcementList, orgDataList;
        private List<List<string>> storageDetail;
        private readonly List<List<string>> storageType;
        private readonly Octokit.GitHubClient ghc;
        private Button storageDetailsButton;
        private LoadingCircle loadingCircleCompliant;
        private Label vSeparator2;
        private Label lblColorCompliant;
        private Label vSeparator1;
        private Label hSeparator2;
        private readonly UserPreferenceChangedEventHandler UserPreferenceChanged;

        /// <summary> 
        /// Main form constructor
        /// </summary>
        /// <param name="offlineMode">Offline mode set</param>
        /// <param name="agentData">Agent name and id gotten from the Login form</param>
        /// <param name="serverIP">Server IP address</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="log">Log file object</param>
        /// <param name="parametersList">List containing data from [Parameters]</param>
        /// <param name="enforcementList">List containing data from [Enforcement]</param>
        /// <param name="orgDataList">List containing data from [OrgData]</param>
        internal MainForm(Octokit.GitHubClient ghc, bool offlineMode, string[] agentData, string serverIP, string serverPort, LogGenerator log, List<string[]> parametersList, List<string> enforcementList, List<string> orgDataList, bool isSystemDarkModeEnabled)
        {
            //Inits WinForms components
            InitializeComponent();

            //Define theming according to ini file provided info
            (int themeFileSet, bool themeEditable) = MiscMethods.GetFileThemeMode(parametersList, isSystemDarkModeEnabled);
            switch (themeFileSet)
            {
                case 0:
                    LightTheme();
                    if (themeEditable == false)
                    {
                        isSystemDarkModeEnabled = false;
                        comboBoxThemeButton.Enabled = false;
                    }
                    break;
                case 1:
                    DarkTheme();
                    if (themeEditable == false)
                    {
                        isSystemDarkModeEnabled = true;
                        comboBoxThemeButton.Enabled = false;
                    }
                    break;
            }

            UserPreferenceChanged = new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
            Disposed += new EventHandler(MainForm_Disposed);

            //Program version
#if DEBUG
            toolStripVersionText.Text = MiscMethods.Version(Resources.DEV_STATUS); //Debug/Beta version
#else
            toolStripVersionText.Text = MiscMethods.Version(); //Release/Final version
#endif

            this.ghc = ghc;
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            this.log = log;
            this.offlineMode = offlineMode;
            this.parametersList = parametersList;
            this.enforcementList = enforcementList;
            this.orgDataList = orgDataList;
            this.agentData = agentData;
            this.isSystemDarkModeEnabled = isSystemDarkModeEnabled;

            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_OFFLINE_MODE, offlineMode.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            if (!offlineMode)
            {
                //Fetch building and hw types info from the specified server
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_FETCHING_SERVER_DATA, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                jsonServerSettings = JsonFileReaderDLL.ConfigFileReader.FetchInfoST(serverIP, serverPort);
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
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_FETCHING_LOCAL_DATA, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                jsonServerSettings = JsonFileReaderDLL.ConfigFileReader.GetOfflineModeConfigFile();
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

            //Fills controls with provided info from ini file and constants dll
            comboBoxActiveDirectory.Items.AddRange(StringsAndConstants.LIST_ACTIVE_DIRECTORY_GUI.ToArray());
            comboBoxStandard.Items.AddRange(StringsAndConstants.LIST_STANDARD_GUI.ToArray());
            comboBoxInUse.Items.AddRange(StringsAndConstants.LIST_IN_USE_GUI.ToArray());
            comboBoxTag.Items.AddRange(StringsAndConstants.LIST_TAG_GUI.ToArray());
            comboBoxBatteryChange.Items.AddRange(StringsAndConstants.LIST_BATTERY_GUI.ToArray());
            if (HardwareInfo.GetHostname().Substring(0, 3).ToUpper().Equals(ConstantsDLL.Properties.Resources.HOSTNAME_PATTERN))
                textBoxAssetNumber.Text = HardwareInfo.GetHostname().Substring(3);
            else
                textBoxAssetNumber.Text = string.Empty;

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
                    oList[i] = this.orgDataList[i].ToString() + " - ";
            }

            toolStripStatusBarText.Text = oList[4] + oList[2] + oList[0].Substring(0, oList[0].Length - 2);
            Text = Application.ProductName + " / " + oList[5] + oList[3] + oList[1].Substring(0, oList[1].Length - 2);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            lblBrand = new System.Windows.Forms.Label();
            lblModel = new System.Windows.Forms.Label();
            lblSerialNumber = new System.Windows.Forms.Label();
            lblProcessor = new System.Windows.Forms.Label();
            lblRam = new System.Windows.Forms.Label();
            lblStorageSize = new System.Windows.Forms.Label();
            lblOperatingSystem = new System.Windows.Forms.Label();
            lblHostname = new System.Windows.Forms.Label();
            lblMacAddress = new System.Windows.Forms.Label();
            lblIpAddress = new System.Windows.Forms.Label();
            lblFixedBrand = new System.Windows.Forms.Label();
            lblFixedModel = new System.Windows.Forms.Label();
            lblFixedSerialNumber = new System.Windows.Forms.Label();
            lblFixedProcessor = new System.Windows.Forms.Label();
            lblFixedRam = new System.Windows.Forms.Label();
            lblFixedStorageSize = new System.Windows.Forms.Label();
            lblFixedOperatingSystem = new System.Windows.Forms.Label();
            lblFixedHostname = new System.Windows.Forms.Label();
            lblFixedMacAddress = new System.Windows.Forms.Label();
            lblFixedIpAddress = new System.Windows.Forms.Label();
            lblFixedAssetNumber = new System.Windows.Forms.Label();
            lblFixedSealNumber = new System.Windows.Forms.Label();
            lblFixedBuilding = new System.Windows.Forms.Label();
            textBoxAssetNumber = new System.Windows.Forms.TextBox();
            textBoxSealNumber = new System.Windows.Forms.TextBox();
            textBoxRoomNumber = new System.Windows.Forms.TextBox();
            textBoxRoomLetter = new System.Windows.Forms.TextBox();
            lblFixedRoomNumber = new System.Windows.Forms.Label();
            lblFixedServiceDate = new System.Windows.Forms.Label();
            registerButton = new System.Windows.Forms.Button();
            lblFixedInUse = new System.Windows.Forms.Label();
            lblFixedTag = new System.Windows.Forms.Label();
            lblFixedHwType = new System.Windows.Forms.Label();
            lblFixedServerOperationalStatus = new System.Windows.Forms.Label();
            lblFixedServerPort = new System.Windows.Forms.Label();
            collectButton = new System.Windows.Forms.Button();
            lblFixedRoomLetter = new System.Windows.Forms.Label();
            lblFixedFwVersion = new System.Windows.Forms.Label();
            lblFwVersion = new System.Windows.Forms.Label();
            apcsButton = new System.Windows.Forms.Button();
            lblFixedFwType = new System.Windows.Forms.Label();
            lblFwType = new System.Windows.Forms.Label();
            groupBoxHwData = new System.Windows.Forms.GroupBox();
            loadingCircleCompliant = new MRG.Controls.UI.LoadingCircle();
            vSeparator2 = new System.Windows.Forms.Label();
            lblColorCompliant = new System.Windows.Forms.Label();
            vSeparator1 = new System.Windows.Forms.Label();
            hSeparator2 = new System.Windows.Forms.Label();
            storageDetailsButton = new System.Windows.Forms.Button();
            loadingCircleScanTpmVersion = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanVirtualizationTechnology = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanSecureBoot = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanFwVersion = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanFwType = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanIpAddress = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanMacAddress = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanHostname = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanOperatingSystem = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanVideoCard = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanMediaOperationMode = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanStorageType = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanStorageSize = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanRam = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanProcessor = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanSerialNumber = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanModel = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanBrand = new MRG.Controls.UI.LoadingCircle();
            hSeparator1 = new System.Windows.Forms.Label();
            vSeparator3 = new System.Windows.Forms.Label();
            iconImgTpmVersion = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblTpmVersion = new System.Windows.Forms.Label();
            iconImgVirtualizationTechnology = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblFixedTpmVersion = new System.Windows.Forms.Label();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            lblFixedProgressBarPercent = new System.Windows.Forms.Label();
            lblVirtualizationTechnology = new System.Windows.Forms.Label();
            lblFixedVirtualizationTechnology = new System.Windows.Forms.Label();
            iconImgBrand = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgSecureBoot = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgFwVersion = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgFwType = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgIpAddress = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgMacAddress = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgHostname = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgOperatingSystem = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgVideoCard = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgMediaOperationMode = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgStorageType = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgStorageSize = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgRam = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgProcessor = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgSerialNumber = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgModel = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblSecureBoot = new System.Windows.Forms.Label();
            lblFixedSecureBoot = new System.Windows.Forms.Label();
            lblMediaOperationMode = new System.Windows.Forms.Label();
            lblFixedMediaOperationMode = new System.Windows.Forms.Label();
            lblVideoCard = new System.Windows.Forms.Label();
            lblFixedVideoCard = new System.Windows.Forms.Label();
            lblStorageType = new System.Windows.Forms.Label();
            lblFixedStorageType = new System.Windows.Forms.Label();
            groupBoxAssetData = new System.Windows.Forms.GroupBox();
            lblFixedMandatoryTicketNumber = new System.Windows.Forms.Label();
            lblFixedMandatoryBatteryChange = new System.Windows.Forms.Label();
            iconImgTicketNumber = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblFixedTicketNumber = new System.Windows.Forms.Label();
            textBoxTicketNumber = new System.Windows.Forms.TextBox();
            iconImgBatteryChange = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblFixedMandatoryWho = new System.Windows.Forms.Label();
            lblFixedMandatoryTag = new System.Windows.Forms.Label();
            lblFixedBatteryChange = new System.Windows.Forms.Label();
            lblFixedMandatoryHwType = new System.Windows.Forms.Label();
            lblFixedMandatoryInUse = new System.Windows.Forms.Label();
            lblFixedMandatoryBuilding = new System.Windows.Forms.Label();
            lblFixedMandatoryRoomNumber = new System.Windows.Forms.Label();
            lblFixedMandatoryAssetNumber = new System.Windows.Forms.Label();
            lblFixedMandatoryMain = new System.Windows.Forms.Label();
            iconImgRoomLetter = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgHwType = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgTag = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgInUse = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgServiceDate = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgStandard = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgAdRegistered = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgBuilding = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgRoomNumber = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgSealNumber = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgAssetNumber = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            dateTimePickerServiceDate = new System.Windows.Forms.DateTimePicker();
            lblFixedAdRegistered = new System.Windows.Forms.Label();
            lblFixedStandard = new System.Windows.Forms.Label();
            groupBoxServiceType = new System.Windows.Forms.GroupBox();
            lblFixedMandatoryServiceType = new System.Windows.Forms.Label();
            textBoxInactiveFormattingRadio = new System.Windows.Forms.TextBox();
            textBoxInactiveMaintenanceRadio = new System.Windows.Forms.TextBox();
            radioButtonFormatting = new System.Windows.Forms.RadioButton();
            radioButtonMaintenance = new System.Windows.Forms.RadioButton();
            loadingCircleLastService = new MRG.Controls.UI.LoadingCircle();
            lblColorLastService = new System.Windows.Forms.Label();
            lblAgentName = new System.Windows.Forms.Label();
            lblFixedAgentName = new System.Windows.Forms.Label();
            lblServerPort = new System.Windows.Forms.Label();
            lblServerIP = new System.Windows.Forms.Label();
            lblFixedServerIP = new System.Windows.Forms.Label();
            lblColorServerOperationalStatus = new System.Windows.Forms.Label();
            toolStripVersionText = new System.Windows.Forms.ToolStripStatusLabel();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            comboBoxThemeButton = new System.Windows.Forms.ToolStripDropDownButton();
            toolStripAutoTheme = new System.Windows.Forms.ToolStripMenuItem();
            toolStripLightTheme = new System.Windows.Forms.ToolStripMenuItem();
            toolStripDarkTheme = new System.Windows.Forms.ToolStripMenuItem();
            logLabelButton = new System.Windows.Forms.ToolStripStatusLabel();
            aboutLabelButton = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusBarText = new System.Windows.Forms.ToolStripStatusLabel();
            timerAlertHostname = new System.Windows.Forms.Timer(components);
            timerAlertMediaOperationMode = new System.Windows.Forms.Timer(components);
            timerAlertSecureBoot = new System.Windows.Forms.Timer(components);
            timerAlertFwVersion = new System.Windows.Forms.Timer(components);
            timerAlertNetConnectivity = new System.Windows.Forms.Timer(components);
            timerAlertFwType = new System.Windows.Forms.Timer(components);
            timerAlertVirtualizationTechnology = new System.Windows.Forms.Timer(components);
            timerAlertSmartStatus = new System.Windows.Forms.Timer(components);
            groupBoxRegistryStatus = new System.Windows.Forms.GroupBox();
            webView2Control = new Microsoft.Web.WebView2.WinForms.WebView2();
            timerAlertTpmVersion = new System.Windows.Forms.Timer(components);
            timerAlertRamAmount = new System.Windows.Forms.Timer(components);
            imgTopBanner = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            loadingCircleCollectButton = new MRG.Controls.UI.LoadingCircle();
            loadingCircleRegisterButton = new MRG.Controls.UI.LoadingCircle();
            groupBoxServerStatus = new System.Windows.Forms.GroupBox();
            loadingCircleServerOperationalStatus = new MRG.Controls.UI.LoadingCircle();
            timerOSLabelScroll = new System.Windows.Forms.Timer(components);
            timerFwVersionLabelScroll = new System.Windows.Forms.Timer(components);
            timerVideoCardLabelScroll = new System.Windows.Forms.Timer(components);
            timerRamLabelScroll = new System.Windows.Forms.Timer(components);
            timerProcessorLabelScroll = new System.Windows.Forms.Timer(components);
            comboBoxBatteryChange = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            comboBoxStandard = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            comboBoxActiveDirectory = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            comboBoxTag = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            comboBoxInUse = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            comboBoxHwType = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            comboBoxBuilding = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            groupBoxHwData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)iconImgTpmVersion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgVirtualizationTechnology).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgBrand).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgSecureBoot).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgFwVersion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgFwType).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgIpAddress).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgMacAddress).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgHostname).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgOperatingSystem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgVideoCard).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgMediaOperationMode).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgStorageType).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgStorageSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgRam).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgProcessor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgSerialNumber).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgModel).BeginInit();
            groupBoxAssetData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)iconImgTicketNumber).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgBatteryChange).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgRoomLetter).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgHwType).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgTag).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgInUse).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgServiceDate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgStandard).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgAdRegistered).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgBuilding).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgRoomNumber).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgSealNumber).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgAssetNumber).BeginInit();
            groupBoxServiceType.SuspendLayout();
            statusStrip1.SuspendLayout();
            groupBoxRegistryStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView2Control).BeginInit();
            ((System.ComponentModel.ISupportInitialize)imgTopBanner).BeginInit();
            groupBoxServerStatus.SuspendLayout();
            SuspendLayout();
            // 
            // lblBrand
            // 
            resources.ApplyResources(lblBrand, "lblBrand");
            lblBrand.ForeColor = System.Drawing.Color.Silver;
            lblBrand.Name = "lblBrand";
            // 
            // lblModel
            // 
            resources.ApplyResources(lblModel, "lblModel");
            lblModel.ForeColor = System.Drawing.Color.Silver;
            lblModel.Name = "lblModel";
            // 
            // lblSerialNumber
            // 
            resources.ApplyResources(lblSerialNumber, "lblSerialNumber");
            lblSerialNumber.ForeColor = System.Drawing.Color.Silver;
            lblSerialNumber.Name = "lblSerialNumber";
            // 
            // lblProcessor
            // 
            resources.ApplyResources(lblProcessor, "lblProcessor");
            lblProcessor.ForeColor = System.Drawing.Color.Silver;
            lblProcessor.Name = "lblProcessor";
            // 
            // lblRam
            // 
            resources.ApplyResources(lblRam, "lblRam");
            lblRam.ForeColor = System.Drawing.Color.Silver;
            lblRam.Name = "lblRam";
            // 
            // lblStorageSize
            // 
            resources.ApplyResources(lblStorageSize, "lblStorageSize");
            lblStorageSize.ForeColor = System.Drawing.Color.Silver;
            lblStorageSize.Name = "lblStorageSize";
            // 
            // lblOperatingSystem
            // 
            resources.ApplyResources(lblOperatingSystem, "lblOperatingSystem");
            lblOperatingSystem.ForeColor = System.Drawing.Color.Silver;
            lblOperatingSystem.Name = "lblOperatingSystem";
            // 
            // lblHostname
            // 
            resources.ApplyResources(lblHostname, "lblHostname");
            lblHostname.ForeColor = System.Drawing.Color.Silver;
            lblHostname.Name = "lblHostname";
            // 
            // lblMacAddress
            // 
            resources.ApplyResources(lblMacAddress, "lblMacAddress");
            lblMacAddress.ForeColor = System.Drawing.Color.Silver;
            lblMacAddress.Name = "lblMacAddress";
            // 
            // lblIpAddress
            // 
            resources.ApplyResources(lblIpAddress, "lblIpAddress");
            lblIpAddress.ForeColor = System.Drawing.Color.Silver;
            lblIpAddress.Name = "lblIpAddress";
            // 
            // lblFixedBrand
            // 
            resources.ApplyResources(lblFixedBrand, "lblFixedBrand");
            lblFixedBrand.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedBrand.Name = "lblFixedBrand";
            // 
            // lblFixedModel
            // 
            resources.ApplyResources(lblFixedModel, "lblFixedModel");
            lblFixedModel.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedModel.Name = "lblFixedModel";
            // 
            // lblFixedSerialNumber
            // 
            resources.ApplyResources(lblFixedSerialNumber, "lblFixedSerialNumber");
            lblFixedSerialNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedSerialNumber.Name = "lblFixedSerialNumber";
            // 
            // lblFixedProcessor
            // 
            resources.ApplyResources(lblFixedProcessor, "lblFixedProcessor");
            lblFixedProcessor.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedProcessor.Name = "lblFixedProcessor";
            // 
            // lblFixedRam
            // 
            resources.ApplyResources(lblFixedRam, "lblFixedRam");
            lblFixedRam.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedRam.Name = "lblFixedRam";
            // 
            // lblFixedStorageSize
            // 
            resources.ApplyResources(lblFixedStorageSize, "lblFixedStorageSize");
            lblFixedStorageSize.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedStorageSize.Name = "lblFixedStorageSize";
            // 
            // lblFixedOperatingSystem
            // 
            resources.ApplyResources(lblFixedOperatingSystem, "lblFixedOperatingSystem");
            lblFixedOperatingSystem.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedOperatingSystem.Name = "lblFixedOperatingSystem";
            // 
            // lblFixedHostname
            // 
            resources.ApplyResources(lblFixedHostname, "lblFixedHostname");
            lblFixedHostname.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedHostname.Name = "lblFixedHostname";
            // 
            // lblFixedMacAddress
            // 
            resources.ApplyResources(lblFixedMacAddress, "lblFixedMacAddress");
            lblFixedMacAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedMacAddress.Name = "lblFixedMacAddress";
            // 
            // lblFixedIpAddress
            // 
            resources.ApplyResources(lblFixedIpAddress, "lblFixedIpAddress");
            lblFixedIpAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedIpAddress.Name = "lblFixedIpAddress";
            // 
            // lblFixedAssetNumber
            // 
            resources.ApplyResources(lblFixedAssetNumber, "lblFixedAssetNumber");
            lblFixedAssetNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedAssetNumber.Name = "lblFixedAssetNumber";
            // 
            // lblFixedSealNumber
            // 
            resources.ApplyResources(lblFixedSealNumber, "lblFixedSealNumber");
            lblFixedSealNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedSealNumber.Name = "lblFixedSealNumber";
            // 
            // lblFixedBuilding
            // 
            resources.ApplyResources(lblFixedBuilding, "lblFixedBuilding");
            lblFixedBuilding.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedBuilding.Name = "lblFixedBuilding";
            // 
            // textBoxAssetNumber
            // 
            textBoxAssetNumber.BackColor = System.Drawing.SystemColors.Window;
            textBoxAssetNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(textBoxAssetNumber, "textBoxAssetNumber");
            textBoxAssetNumber.Name = "textBoxAssetNumber";
            textBoxAssetNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxSealNumber
            // 
            textBoxSealNumber.BackColor = System.Drawing.SystemColors.Window;
            textBoxSealNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(textBoxSealNumber, "textBoxSealNumber");
            textBoxSealNumber.Name = "textBoxSealNumber";
            textBoxSealNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxRoomNumber
            // 
            textBoxRoomNumber.BackColor = System.Drawing.SystemColors.Window;
            textBoxRoomNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(textBoxRoomNumber, "textBoxRoomNumber");
            textBoxRoomNumber.Name = "textBoxRoomNumber";
            textBoxRoomNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxRoomLetter
            // 
            textBoxRoomLetter.BackColor = System.Drawing.SystemColors.Window;
            textBoxRoomLetter.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            textBoxRoomLetter.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(textBoxRoomLetter, "textBoxRoomLetter");
            textBoxRoomLetter.Name = "textBoxRoomLetter";
            textBoxRoomLetter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxCharsOnly_KeyPress);
            // 
            // lblFixedRoomNumber
            // 
            resources.ApplyResources(lblFixedRoomNumber, "lblFixedRoomNumber");
            lblFixedRoomNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedRoomNumber.Name = "lblFixedRoomNumber";
            // 
            // lblFixedServiceDate
            // 
            resources.ApplyResources(lblFixedServiceDate, "lblFixedServiceDate");
            lblFixedServiceDate.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedServiceDate.Name = "lblFixedServiceDate";
            // 
            // registerButton
            // 
            registerButton.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(registerButton, "registerButton");
            registerButton.ForeColor = System.Drawing.SystemColors.ControlText;
            registerButton.Name = "registerButton";
            registerButton.UseVisualStyleBackColor = true;
            registerButton.Click += new System.EventHandler(RegisterButton_ClickAsync);
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
            // lblFixedHwType
            // 
            resources.ApplyResources(lblFixedHwType, "lblFixedHwType");
            lblFixedHwType.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedHwType.Name = "lblFixedHwType";
            // 
            // lblFixedServerOperationalStatus
            // 
            resources.ApplyResources(lblFixedServerOperationalStatus, "lblFixedServerOperationalStatus");
            lblFixedServerOperationalStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedServerOperationalStatus.Name = "lblFixedServerOperationalStatus";
            // 
            // lblFixedServerPort
            // 
            resources.ApplyResources(lblFixedServerPort, "lblFixedServerPort");
            lblFixedServerPort.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedServerPort.Name = "lblFixedServerPort";
            // 
            // collectButton
            // 
            collectButton.BackColor = System.Drawing.SystemColors.Control;
            collectButton.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(collectButton, "collectButton");
            collectButton.Name = "collectButton";
            collectButton.UseVisualStyleBackColor = true;
            collectButton.Click += new System.EventHandler(CollectButton_Click);
            // 
            // lblFixedRoomLetter
            // 
            resources.ApplyResources(lblFixedRoomLetter, "lblFixedRoomLetter");
            lblFixedRoomLetter.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedRoomLetter.Name = "lblFixedRoomLetter";
            // 
            // lblFixedFwVersion
            // 
            resources.ApplyResources(lblFixedFwVersion, "lblFixedFwVersion");
            lblFixedFwVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedFwVersion.Name = "lblFixedFwVersion";
            // 
            // lblFwVersion
            // 
            resources.ApplyResources(lblFwVersion, "lblFwVersion");
            lblFwVersion.ForeColor = System.Drawing.Color.Silver;
            lblFwVersion.Name = "lblFwVersion";
            // 
            // apcsButton
            // 
            apcsButton.BackColor = System.Drawing.SystemColors.Control;
            apcsButton.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(apcsButton, "apcsButton");
            apcsButton.Name = "apcsButton";
            apcsButton.UseVisualStyleBackColor = true;
            apcsButton.Click += new System.EventHandler(ApcsButton_Click);
            // 
            // lblFixedFwType
            // 
            resources.ApplyResources(lblFixedFwType, "lblFixedFwType");
            lblFixedFwType.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedFwType.Name = "lblFixedFwType";
            // 
            // lblFwType
            // 
            resources.ApplyResources(lblFwType, "lblFwType");
            lblFwType.ForeColor = System.Drawing.Color.Silver;
            lblFwType.Name = "lblFwType";
            // 
            // groupBoxHwData
            // 
            groupBoxHwData.Controls.Add(loadingCircleCompliant);
            groupBoxHwData.Controls.Add(vSeparator2);
            groupBoxHwData.Controls.Add(lblColorCompliant);
            groupBoxHwData.Controls.Add(vSeparator1);
            groupBoxHwData.Controls.Add(hSeparator2);
            groupBoxHwData.Controls.Add(storageDetailsButton);
            groupBoxHwData.Controls.Add(loadingCircleScanTpmVersion);
            groupBoxHwData.Controls.Add(loadingCircleScanVirtualizationTechnology);
            groupBoxHwData.Controls.Add(loadingCircleScanSecureBoot);
            groupBoxHwData.Controls.Add(loadingCircleScanFwVersion);
            groupBoxHwData.Controls.Add(loadingCircleScanFwType);
            groupBoxHwData.Controls.Add(loadingCircleScanIpAddress);
            groupBoxHwData.Controls.Add(loadingCircleScanMacAddress);
            groupBoxHwData.Controls.Add(loadingCircleScanHostname);
            groupBoxHwData.Controls.Add(loadingCircleScanOperatingSystem);
            groupBoxHwData.Controls.Add(loadingCircleScanVideoCard);
            groupBoxHwData.Controls.Add(loadingCircleScanMediaOperationMode);
            groupBoxHwData.Controls.Add(loadingCircleScanStorageType);
            groupBoxHwData.Controls.Add(loadingCircleScanStorageSize);
            groupBoxHwData.Controls.Add(loadingCircleScanRam);
            groupBoxHwData.Controls.Add(loadingCircleScanProcessor);
            groupBoxHwData.Controls.Add(loadingCircleScanSerialNumber);
            groupBoxHwData.Controls.Add(loadingCircleScanModel);
            groupBoxHwData.Controls.Add(loadingCircleScanBrand);
            groupBoxHwData.Controls.Add(hSeparator1);
            groupBoxHwData.Controls.Add(vSeparator3);
            groupBoxHwData.Controls.Add(iconImgTpmVersion);
            groupBoxHwData.Controls.Add(lblTpmVersion);
            groupBoxHwData.Controls.Add(iconImgVirtualizationTechnology);
            groupBoxHwData.Controls.Add(lblFixedTpmVersion);
            groupBoxHwData.Controls.Add(progressBar1);
            groupBoxHwData.Controls.Add(lblFixedProgressBarPercent);
            groupBoxHwData.Controls.Add(lblVirtualizationTechnology);
            groupBoxHwData.Controls.Add(lblFixedVirtualizationTechnology);
            groupBoxHwData.Controls.Add(iconImgBrand);
            groupBoxHwData.Controls.Add(iconImgSecureBoot);
            groupBoxHwData.Controls.Add(iconImgFwVersion);
            groupBoxHwData.Controls.Add(iconImgFwType);
            groupBoxHwData.Controls.Add(iconImgIpAddress);
            groupBoxHwData.Controls.Add(iconImgMacAddress);
            groupBoxHwData.Controls.Add(iconImgHostname);
            groupBoxHwData.Controls.Add(iconImgOperatingSystem);
            groupBoxHwData.Controls.Add(iconImgVideoCard);
            groupBoxHwData.Controls.Add(iconImgMediaOperationMode);
            groupBoxHwData.Controls.Add(iconImgStorageType);
            groupBoxHwData.Controls.Add(iconImgStorageSize);
            groupBoxHwData.Controls.Add(iconImgRam);
            groupBoxHwData.Controls.Add(iconImgProcessor);
            groupBoxHwData.Controls.Add(iconImgSerialNumber);
            groupBoxHwData.Controls.Add(iconImgModel);
            groupBoxHwData.Controls.Add(lblSecureBoot);
            groupBoxHwData.Controls.Add(lblFixedSecureBoot);
            groupBoxHwData.Controls.Add(lblMediaOperationMode);
            groupBoxHwData.Controls.Add(lblFixedMediaOperationMode);
            groupBoxHwData.Controls.Add(lblVideoCard);
            groupBoxHwData.Controls.Add(lblFixedVideoCard);
            groupBoxHwData.Controls.Add(lblStorageType);
            groupBoxHwData.Controls.Add(lblFixedStorageType);
            groupBoxHwData.Controls.Add(lblFixedBrand);
            groupBoxHwData.Controls.Add(lblFwType);
            groupBoxHwData.Controls.Add(lblStorageSize);
            groupBoxHwData.Controls.Add(lblFixedFwType);
            groupBoxHwData.Controls.Add(lblRam);
            groupBoxHwData.Controls.Add(lblProcessor);
            groupBoxHwData.Controls.Add(lblSerialNumber);
            groupBoxHwData.Controls.Add(lblFwVersion);
            groupBoxHwData.Controls.Add(lblModel);
            groupBoxHwData.Controls.Add(lblFixedFwVersion);
            groupBoxHwData.Controls.Add(lblBrand);
            groupBoxHwData.Controls.Add(lblHostname);
            groupBoxHwData.Controls.Add(lblMacAddress);
            groupBoxHwData.Controls.Add(lblIpAddress);
            groupBoxHwData.Controls.Add(lblFixedModel);
            groupBoxHwData.Controls.Add(lblFixedSerialNumber);
            groupBoxHwData.Controls.Add(lblFixedProcessor);
            groupBoxHwData.Controls.Add(lblFixedRam);
            groupBoxHwData.Controls.Add(lblFixedStorageSize);
            groupBoxHwData.Controls.Add(lblFixedOperatingSystem);
            groupBoxHwData.Controls.Add(lblFixedHostname);
            groupBoxHwData.Controls.Add(lblFixedMacAddress);
            groupBoxHwData.Controls.Add(lblFixedIpAddress);
            groupBoxHwData.Controls.Add(lblOperatingSystem);
            groupBoxHwData.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(groupBoxHwData, "groupBoxHwData");
            groupBoxHwData.Name = "groupBoxHwData";
            groupBoxHwData.TabStop = false;
            // 
            // loadingCircleCompliant
            // 
            loadingCircleCompliant.Active = false;
            loadingCircleCompliant.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleCompliant.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleCompliant, "loadingCircleCompliant");
            loadingCircleCompliant.Name = "loadingCircleCompliant";
            loadingCircleCompliant.NumberSpoke = 12;
            loadingCircleCompliant.OuterCircleRadius = 11;
            loadingCircleCompliant.RotationSpeed = 1;
            loadingCircleCompliant.SpokeThickness = 2;
            loadingCircleCompliant.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // vSeparator2
            // 
            vSeparator2.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(vSeparator2, "vSeparator2");
            vSeparator2.Name = "vSeparator2";
            // 
            // lblColorCompliant
            // 
            resources.ApplyResources(lblColorCompliant, "lblColorCompliant");
            lblColorCompliant.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            lblColorCompliant.Name = "lblColorCompliant";
            // 
            // vSeparator1
            // 
            vSeparator1.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(vSeparator1, "vSeparator1");
            vSeparator1.Name = "vSeparator1";
            // 
            // hSeparator2
            // 
            hSeparator2.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(hSeparator2, "hSeparator2");
            hSeparator2.Name = "hSeparator2";
            // 
            // storageDetailsButton
            // 
            resources.ApplyResources(storageDetailsButton, "storageDetailsButton");
            storageDetailsButton.Name = "storageDetailsButton";
            storageDetailsButton.UseVisualStyleBackColor = true;
            storageDetailsButton.Click += new System.EventHandler(StorageDetailsButton_Click);
            // 
            // loadingCircleScanTpmVersion
            // 
            loadingCircleScanTpmVersion.Active = false;
            loadingCircleScanTpmVersion.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanTpmVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanTpmVersion.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanTpmVersion, "loadingCircleScanTpmVersion");
            loadingCircleScanTpmVersion.Name = "loadingCircleScanTpmVersion";
            loadingCircleScanTpmVersion.NumberSpoke = 12;
            loadingCircleScanTpmVersion.OuterCircleRadius = 11;
            loadingCircleScanTpmVersion.RotationSpeed = 1;
            loadingCircleScanTpmVersion.SpokeThickness = 2;
            loadingCircleScanTpmVersion.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanVirtualizationTechnology
            // 
            loadingCircleScanVirtualizationTechnology.Active = false;
            loadingCircleScanVirtualizationTechnology.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanVirtualizationTechnology.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanVirtualizationTechnology.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanVirtualizationTechnology, "loadingCircleScanVirtualizationTechnology");
            loadingCircleScanVirtualizationTechnology.Name = "loadingCircleScanVirtualizationTechnology";
            loadingCircleScanVirtualizationTechnology.NumberSpoke = 12;
            loadingCircleScanVirtualizationTechnology.OuterCircleRadius = 11;
            loadingCircleScanVirtualizationTechnology.RotationSpeed = 1;
            loadingCircleScanVirtualizationTechnology.SpokeThickness = 2;
            loadingCircleScanVirtualizationTechnology.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanSecureBoot
            // 
            loadingCircleScanSecureBoot.Active = false;
            loadingCircleScanSecureBoot.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanSecureBoot.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanSecureBoot.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanSecureBoot, "loadingCircleScanSecureBoot");
            loadingCircleScanSecureBoot.Name = "loadingCircleScanSecureBoot";
            loadingCircleScanSecureBoot.NumberSpoke = 12;
            loadingCircleScanSecureBoot.OuterCircleRadius = 11;
            loadingCircleScanSecureBoot.RotationSpeed = 1;
            loadingCircleScanSecureBoot.SpokeThickness = 2;
            loadingCircleScanSecureBoot.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanFwVersion
            // 
            loadingCircleScanFwVersion.Active = false;
            loadingCircleScanFwVersion.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanFwVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanFwVersion.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanFwVersion, "loadingCircleScanFwVersion");
            loadingCircleScanFwVersion.Name = "loadingCircleScanFwVersion";
            loadingCircleScanFwVersion.NumberSpoke = 12;
            loadingCircleScanFwVersion.OuterCircleRadius = 11;
            loadingCircleScanFwVersion.RotationSpeed = 1;
            loadingCircleScanFwVersion.SpokeThickness = 2;
            loadingCircleScanFwVersion.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanFwType
            // 
            loadingCircleScanFwType.Active = false;
            loadingCircleScanFwType.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanFwType.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanFwType.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanFwType, "loadingCircleScanFwType");
            loadingCircleScanFwType.Name = "loadingCircleScanFwType";
            loadingCircleScanFwType.NumberSpoke = 12;
            loadingCircleScanFwType.OuterCircleRadius = 11;
            loadingCircleScanFwType.RotationSpeed = 1;
            loadingCircleScanFwType.SpokeThickness = 2;
            loadingCircleScanFwType.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanIpAddress
            // 
            loadingCircleScanIpAddress.Active = false;
            loadingCircleScanIpAddress.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanIpAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanIpAddress.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanIpAddress, "loadingCircleScanIpAddress");
            loadingCircleScanIpAddress.Name = "loadingCircleScanIpAddress";
            loadingCircleScanIpAddress.NumberSpoke = 12;
            loadingCircleScanIpAddress.OuterCircleRadius = 11;
            loadingCircleScanIpAddress.RotationSpeed = 1;
            loadingCircleScanIpAddress.SpokeThickness = 2;
            loadingCircleScanIpAddress.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanMacAddress
            // 
            loadingCircleScanMacAddress.Active = false;
            loadingCircleScanMacAddress.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanMacAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanMacAddress.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanMacAddress, "loadingCircleScanMacAddress");
            loadingCircleScanMacAddress.Name = "loadingCircleScanMacAddress";
            loadingCircleScanMacAddress.NumberSpoke = 12;
            loadingCircleScanMacAddress.OuterCircleRadius = 11;
            loadingCircleScanMacAddress.RotationSpeed = 1;
            loadingCircleScanMacAddress.SpokeThickness = 2;
            loadingCircleScanMacAddress.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanHostname
            // 
            loadingCircleScanHostname.Active = false;
            loadingCircleScanHostname.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanHostname.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanHostname.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanHostname, "loadingCircleScanHostname");
            loadingCircleScanHostname.Name = "loadingCircleScanHostname";
            loadingCircleScanHostname.NumberSpoke = 12;
            loadingCircleScanHostname.OuterCircleRadius = 11;
            loadingCircleScanHostname.RotationSpeed = 1;
            loadingCircleScanHostname.SpokeThickness = 2;
            loadingCircleScanHostname.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanOperatingSystem
            // 
            loadingCircleScanOperatingSystem.Active = false;
            loadingCircleScanOperatingSystem.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanOperatingSystem.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanOperatingSystem.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanOperatingSystem, "loadingCircleScanOperatingSystem");
            loadingCircleScanOperatingSystem.Name = "loadingCircleScanOperatingSystem";
            loadingCircleScanOperatingSystem.NumberSpoke = 12;
            loadingCircleScanOperatingSystem.OuterCircleRadius = 11;
            loadingCircleScanOperatingSystem.RotationSpeed = 1;
            loadingCircleScanOperatingSystem.SpokeThickness = 2;
            loadingCircleScanOperatingSystem.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanVideoCard
            // 
            loadingCircleScanVideoCard.Active = false;
            loadingCircleScanVideoCard.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanVideoCard.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanVideoCard.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanVideoCard, "loadingCircleScanVideoCard");
            loadingCircleScanVideoCard.Name = "loadingCircleScanVideoCard";
            loadingCircleScanVideoCard.NumberSpoke = 12;
            loadingCircleScanVideoCard.OuterCircleRadius = 11;
            loadingCircleScanVideoCard.RotationSpeed = 1;
            loadingCircleScanVideoCard.SpokeThickness = 2;
            loadingCircleScanVideoCard.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanMediaOperationMode
            // 
            loadingCircleScanMediaOperationMode.Active = false;
            loadingCircleScanMediaOperationMode.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanMediaOperationMode.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanMediaOperationMode.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanMediaOperationMode, "loadingCircleScanMediaOperationMode");
            loadingCircleScanMediaOperationMode.Name = "loadingCircleScanMediaOperationMode";
            loadingCircleScanMediaOperationMode.NumberSpoke = 12;
            loadingCircleScanMediaOperationMode.OuterCircleRadius = 11;
            loadingCircleScanMediaOperationMode.RotationSpeed = 1;
            loadingCircleScanMediaOperationMode.SpokeThickness = 2;
            loadingCircleScanMediaOperationMode.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanStorageType
            // 
            loadingCircleScanStorageType.Active = false;
            loadingCircleScanStorageType.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanStorageType.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanStorageType.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanStorageType, "loadingCircleScanStorageType");
            loadingCircleScanStorageType.Name = "loadingCircleScanStorageType";
            loadingCircleScanStorageType.NumberSpoke = 12;
            loadingCircleScanStorageType.OuterCircleRadius = 11;
            loadingCircleScanStorageType.RotationSpeed = 1;
            loadingCircleScanStorageType.SpokeThickness = 2;
            loadingCircleScanStorageType.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanStorageSize
            // 
            loadingCircleScanStorageSize.Active = false;
            loadingCircleScanStorageSize.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanStorageSize.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanStorageSize.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanStorageSize, "loadingCircleScanStorageSize");
            loadingCircleScanStorageSize.Name = "loadingCircleScanStorageSize";
            loadingCircleScanStorageSize.NumberSpoke = 12;
            loadingCircleScanStorageSize.OuterCircleRadius = 11;
            loadingCircleScanStorageSize.RotationSpeed = 1;
            loadingCircleScanStorageSize.SpokeThickness = 2;
            loadingCircleScanStorageSize.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanRam
            // 
            loadingCircleScanRam.Active = false;
            loadingCircleScanRam.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanRam.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanRam.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanRam, "loadingCircleScanRam");
            loadingCircleScanRam.Name = "loadingCircleScanRam";
            loadingCircleScanRam.NumberSpoke = 12;
            loadingCircleScanRam.OuterCircleRadius = 11;
            loadingCircleScanRam.RotationSpeed = 1;
            loadingCircleScanRam.SpokeThickness = 2;
            loadingCircleScanRam.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanProcessor
            // 
            loadingCircleScanProcessor.Active = false;
            loadingCircleScanProcessor.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanProcessor.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanProcessor.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanProcessor, "loadingCircleScanProcessor");
            loadingCircleScanProcessor.Name = "loadingCircleScanProcessor";
            loadingCircleScanProcessor.NumberSpoke = 12;
            loadingCircleScanProcessor.OuterCircleRadius = 11;
            loadingCircleScanProcessor.RotationSpeed = 1;
            loadingCircleScanProcessor.SpokeThickness = 2;
            loadingCircleScanProcessor.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanSerialNumber
            // 
            loadingCircleScanSerialNumber.Active = false;
            loadingCircleScanSerialNumber.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanSerialNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanSerialNumber.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanSerialNumber, "loadingCircleScanSerialNumber");
            loadingCircleScanSerialNumber.Name = "loadingCircleScanSerialNumber";
            loadingCircleScanSerialNumber.NumberSpoke = 12;
            loadingCircleScanSerialNumber.OuterCircleRadius = 11;
            loadingCircleScanSerialNumber.RotationSpeed = 1;
            loadingCircleScanSerialNumber.SpokeThickness = 2;
            loadingCircleScanSerialNumber.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanModel
            // 
            loadingCircleScanModel.Active = false;
            loadingCircleScanModel.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanModel.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanModel.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanModel, "loadingCircleScanModel");
            loadingCircleScanModel.Name = "loadingCircleScanModel";
            loadingCircleScanModel.NumberSpoke = 12;
            loadingCircleScanModel.OuterCircleRadius = 11;
            loadingCircleScanModel.RotationSpeed = 1;
            loadingCircleScanModel.SpokeThickness = 2;
            loadingCircleScanModel.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanBrand
            // 
            loadingCircleScanBrand.Active = false;
            loadingCircleScanBrand.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleScanBrand.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleScanBrand.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleScanBrand, "loadingCircleScanBrand");
            loadingCircleScanBrand.Name = "loadingCircleScanBrand";
            loadingCircleScanBrand.NumberSpoke = 12;
            loadingCircleScanBrand.OuterCircleRadius = 11;
            loadingCircleScanBrand.RotationSpeed = 1;
            loadingCircleScanBrand.SpokeThickness = 2;
            loadingCircleScanBrand.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // hSeparator1
            // 
            hSeparator1.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(hSeparator1, "hSeparator1");
            hSeparator1.Name = "hSeparator1";
            // 
            // vSeparator3
            // 
            vSeparator3.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(vSeparator3, "vSeparator3");
            vSeparator3.Name = "vSeparator3";
            // 
            // iconImgTpmVersion
            // 
            iconImgTpmVersion.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgTpmVersion, "iconImgTpmVersion");
            iconImgTpmVersion.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgTpmVersion.Name = "iconImgTpmVersion";
            iconImgTpmVersion.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgTpmVersion.TabStop = false;
            // 
            // lblTpmVersion
            // 
            resources.ApplyResources(lblTpmVersion, "lblTpmVersion");
            lblTpmVersion.ForeColor = System.Drawing.Color.Silver;
            lblTpmVersion.Name = "lblTpmVersion";
            // 
            // iconImgVirtualizationTechnology
            // 
            iconImgVirtualizationTechnology.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgVirtualizationTechnology, "iconImgVirtualizationTechnology");
            iconImgVirtualizationTechnology.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgVirtualizationTechnology.Name = "iconImgVirtualizationTechnology";
            iconImgVirtualizationTechnology.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgVirtualizationTechnology.TabStop = false;
            // 
            // lblFixedTpmVersion
            // 
            resources.ApplyResources(lblFixedTpmVersion, "lblFixedTpmVersion");
            lblFixedTpmVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedTpmVersion.Name = "lblFixedTpmVersion";
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            // 
            // lblFixedProgressBarPercent
            // 
            lblFixedProgressBarPercent.BackColor = System.Drawing.Color.Transparent;
            lblFixedProgressBarPercent.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(lblFixedProgressBarPercent, "lblFixedProgressBarPercent");
            lblFixedProgressBarPercent.Name = "lblFixedProgressBarPercent";
            // 
            // lblVirtualizationTechnology
            // 
            resources.ApplyResources(lblVirtualizationTechnology, "lblVirtualizationTechnology");
            lblVirtualizationTechnology.ForeColor = System.Drawing.Color.Silver;
            lblVirtualizationTechnology.Name = "lblVirtualizationTechnology";
            // 
            // lblFixedVirtualizationTechnology
            // 
            resources.ApplyResources(lblFixedVirtualizationTechnology, "lblFixedVirtualizationTechnology");
            lblFixedVirtualizationTechnology.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedVirtualizationTechnology.Name = "lblFixedVirtualizationTechnology";
            // 
            // iconImgBrand
            // 
            iconImgBrand.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgBrand, "iconImgBrand");
            iconImgBrand.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgBrand.Name = "iconImgBrand";
            iconImgBrand.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgBrand.TabStop = false;
            // 
            // iconImgSecureBoot
            // 
            iconImgSecureBoot.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgSecureBoot, "iconImgSecureBoot");
            iconImgSecureBoot.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgSecureBoot.Name = "iconImgSecureBoot";
            iconImgSecureBoot.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgSecureBoot.TabStop = false;
            // 
            // iconImgFwVersion
            // 
            iconImgFwVersion.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgFwVersion, "iconImgFwVersion");
            iconImgFwVersion.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgFwVersion.Name = "iconImgFwVersion";
            iconImgFwVersion.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgFwVersion.TabStop = false;
            // 
            // iconImgFwType
            // 
            iconImgFwType.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgFwType, "iconImgFwType");
            iconImgFwType.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgFwType.Name = "iconImgFwType";
            iconImgFwType.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgFwType.TabStop = false;
            // 
            // iconImgIpAddress
            // 
            iconImgIpAddress.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgIpAddress, "iconImgIpAddress");
            iconImgIpAddress.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgIpAddress.Name = "iconImgIpAddress";
            iconImgIpAddress.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgIpAddress.TabStop = false;
            // 
            // iconImgMacAddress
            // 
            iconImgMacAddress.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgMacAddress, "iconImgMacAddress");
            iconImgMacAddress.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgMacAddress.Name = "iconImgMacAddress";
            iconImgMacAddress.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgMacAddress.TabStop = false;
            // 
            // iconImgHostname
            // 
            iconImgHostname.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgHostname, "iconImgHostname");
            iconImgHostname.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgHostname.Name = "iconImgHostname";
            iconImgHostname.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgHostname.TabStop = false;
            // 
            // iconImgOperatingSystem
            // 
            iconImgOperatingSystem.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgOperatingSystem, "iconImgOperatingSystem");
            iconImgOperatingSystem.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgOperatingSystem.Name = "iconImgOperatingSystem";
            iconImgOperatingSystem.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgOperatingSystem.TabStop = false;
            // 
            // iconImgVideoCard
            // 
            iconImgVideoCard.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgVideoCard, "iconImgVideoCard");
            iconImgVideoCard.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgVideoCard.Name = "iconImgVideoCard";
            iconImgVideoCard.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgVideoCard.TabStop = false;
            // 
            // iconImgMediaOperationMode
            // 
            iconImgMediaOperationMode.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgMediaOperationMode, "iconImgMediaOperationMode");
            iconImgMediaOperationMode.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgMediaOperationMode.Name = "iconImgMediaOperationMode";
            iconImgMediaOperationMode.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgMediaOperationMode.TabStop = false;
            // 
            // iconImgStorageType
            // 
            iconImgStorageType.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgStorageType, "iconImgStorageType");
            iconImgStorageType.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgStorageType.Name = "iconImgStorageType";
            iconImgStorageType.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgStorageType.TabStop = false;
            // 
            // iconImgStorageSize
            // 
            iconImgStorageSize.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgStorageSize, "iconImgStorageSize");
            iconImgStorageSize.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgStorageSize.Name = "iconImgStorageSize";
            iconImgStorageSize.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgStorageSize.TabStop = false;
            // 
            // iconImgRam
            // 
            iconImgRam.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgRam, "iconImgRam");
            iconImgRam.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgRam.Name = "iconImgRam";
            iconImgRam.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgRam.TabStop = false;
            // 
            // iconImgProcessor
            // 
            iconImgProcessor.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgProcessor, "iconImgProcessor");
            iconImgProcessor.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgProcessor.Name = "iconImgProcessor";
            iconImgProcessor.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgProcessor.TabStop = false;
            // 
            // iconImgSerialNumber
            // 
            iconImgSerialNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgSerialNumber, "iconImgSerialNumber");
            iconImgSerialNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgSerialNumber.Name = "iconImgSerialNumber";
            iconImgSerialNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgSerialNumber.TabStop = false;
            // 
            // iconImgModel
            // 
            iconImgModel.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgModel, "iconImgModel");
            iconImgModel.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgModel.Name = "iconImgModel";
            iconImgModel.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgModel.TabStop = false;
            // 
            // lblSecureBoot
            // 
            resources.ApplyResources(lblSecureBoot, "lblSecureBoot");
            lblSecureBoot.ForeColor = System.Drawing.Color.Silver;
            lblSecureBoot.Name = "lblSecureBoot";
            // 
            // lblFixedSecureBoot
            // 
            resources.ApplyResources(lblFixedSecureBoot, "lblFixedSecureBoot");
            lblFixedSecureBoot.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedSecureBoot.Name = "lblFixedSecureBoot";
            // 
            // lblMediaOperationMode
            // 
            resources.ApplyResources(lblMediaOperationMode, "lblMediaOperationMode");
            lblMediaOperationMode.ForeColor = System.Drawing.Color.Silver;
            lblMediaOperationMode.Name = "lblMediaOperationMode";
            // 
            // lblFixedMediaOperationMode
            // 
            resources.ApplyResources(lblFixedMediaOperationMode, "lblFixedMediaOperationMode");
            lblFixedMediaOperationMode.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedMediaOperationMode.Name = "lblFixedMediaOperationMode";
            // 
            // lblVideoCard
            // 
            resources.ApplyResources(lblVideoCard, "lblVideoCard");
            lblVideoCard.ForeColor = System.Drawing.Color.Silver;
            lblVideoCard.Name = "lblVideoCard";
            // 
            // lblFixedVideoCard
            // 
            resources.ApplyResources(lblFixedVideoCard, "lblFixedVideoCard");
            lblFixedVideoCard.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedVideoCard.Name = "lblFixedVideoCard";
            // 
            // lblStorageType
            // 
            resources.ApplyResources(lblStorageType, "lblStorageType");
            lblStorageType.ForeColor = System.Drawing.Color.Silver;
            lblStorageType.Name = "lblStorageType";
            // 
            // lblFixedStorageType
            // 
            resources.ApplyResources(lblFixedStorageType, "lblFixedStorageType");
            lblFixedStorageType.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedStorageType.Name = "lblFixedStorageType";
            // 
            // groupBoxAssetData
            // 
            groupBoxAssetData.Controls.Add(comboBoxBatteryChange);
            groupBoxAssetData.Controls.Add(comboBoxStandard);
            groupBoxAssetData.Controls.Add(comboBoxActiveDirectory);
            groupBoxAssetData.Controls.Add(comboBoxTag);
            groupBoxAssetData.Controls.Add(comboBoxInUse);
            groupBoxAssetData.Controls.Add(comboBoxHwType);
            groupBoxAssetData.Controls.Add(comboBoxBuilding);
            groupBoxAssetData.Controls.Add(lblFixedMandatoryTicketNumber);
            groupBoxAssetData.Controls.Add(lblFixedMandatoryBatteryChange);
            groupBoxAssetData.Controls.Add(iconImgTicketNumber);
            groupBoxAssetData.Controls.Add(lblFixedTicketNumber);
            groupBoxAssetData.Controls.Add(textBoxTicketNumber);
            groupBoxAssetData.Controls.Add(iconImgBatteryChange);
            groupBoxAssetData.Controls.Add(lblFixedMandatoryWho);
            groupBoxAssetData.Controls.Add(lblFixedMandatoryTag);
            groupBoxAssetData.Controls.Add(lblFixedBatteryChange);
            groupBoxAssetData.Controls.Add(lblFixedMandatoryHwType);
            groupBoxAssetData.Controls.Add(lblFixedMandatoryInUse);
            groupBoxAssetData.Controls.Add(lblFixedMandatoryBuilding);
            groupBoxAssetData.Controls.Add(lblFixedMandatoryRoomNumber);
            groupBoxAssetData.Controls.Add(lblFixedMandatoryAssetNumber);
            groupBoxAssetData.Controls.Add(lblFixedMandatoryMain);
            groupBoxAssetData.Controls.Add(iconImgRoomLetter);
            groupBoxAssetData.Controls.Add(iconImgHwType);
            groupBoxAssetData.Controls.Add(iconImgTag);
            groupBoxAssetData.Controls.Add(iconImgInUse);
            groupBoxAssetData.Controls.Add(iconImgServiceDate);
            groupBoxAssetData.Controls.Add(iconImgStandard);
            groupBoxAssetData.Controls.Add(iconImgAdRegistered);
            groupBoxAssetData.Controls.Add(iconImgBuilding);
            groupBoxAssetData.Controls.Add(iconImgRoomNumber);
            groupBoxAssetData.Controls.Add(iconImgSealNumber);
            groupBoxAssetData.Controls.Add(iconImgAssetNumber);
            groupBoxAssetData.Controls.Add(dateTimePickerServiceDate);
            groupBoxAssetData.Controls.Add(lblFixedAssetNumber);
            groupBoxAssetData.Controls.Add(lblFixedSealNumber);
            groupBoxAssetData.Controls.Add(lblFixedBuilding);
            groupBoxAssetData.Controls.Add(textBoxAssetNumber);
            groupBoxAssetData.Controls.Add(textBoxSealNumber);
            groupBoxAssetData.Controls.Add(lblFixedRoomLetter);
            groupBoxAssetData.Controls.Add(textBoxRoomNumber);
            groupBoxAssetData.Controls.Add(lblFixedRoomNumber);
            groupBoxAssetData.Controls.Add(lblFixedAdRegistered);
            groupBoxAssetData.Controls.Add(lblFixedServiceDate);
            groupBoxAssetData.Controls.Add(lblFixedHwType);
            groupBoxAssetData.Controls.Add(lblFixedStandard);
            groupBoxAssetData.Controls.Add(textBoxRoomLetter);
            groupBoxAssetData.Controls.Add(lblFixedInUse);
            groupBoxAssetData.Controls.Add(lblFixedTag);
            groupBoxAssetData.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(groupBoxAssetData, "groupBoxAssetData");
            groupBoxAssetData.Name = "groupBoxAssetData";
            groupBoxAssetData.TabStop = false;
            // 
            // lblFixedMandatoryTicketNumber
            // 
            resources.ApplyResources(lblFixedMandatoryTicketNumber, "lblFixedMandatoryTicketNumber");
            lblFixedMandatoryTicketNumber.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryTicketNumber.Name = "lblFixedMandatoryTicketNumber";
            // 
            // lblFixedMandatoryBatteryChange
            // 
            resources.ApplyResources(lblFixedMandatoryBatteryChange, "lblFixedMandatoryBatteryChange");
            lblFixedMandatoryBatteryChange.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryBatteryChange.Name = "lblFixedMandatoryBatteryChange";
            // 
            // iconImgTicketNumber
            // 
            iconImgTicketNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgTicketNumber, "iconImgTicketNumber");
            iconImgTicketNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgTicketNumber.Name = "iconImgTicketNumber";
            iconImgTicketNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgTicketNumber.TabStop = false;
            // 
            // lblFixedTicketNumber
            // 
            resources.ApplyResources(lblFixedTicketNumber, "lblFixedTicketNumber");
            lblFixedTicketNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedTicketNumber.Name = "lblFixedTicketNumber";
            // 
            // textBoxTicketNumber
            // 
            textBoxTicketNumber.BackColor = System.Drawing.SystemColors.Window;
            textBoxTicketNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(textBoxTicketNumber, "textBoxTicketNumber");
            textBoxTicketNumber.Name = "textBoxTicketNumber";
            textBoxTicketNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // iconImgBatteryChange
            // 
            iconImgBatteryChange.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgBatteryChange, "iconImgBatteryChange");
            iconImgBatteryChange.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgBatteryChange.Name = "iconImgBatteryChange";
            iconImgBatteryChange.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgBatteryChange.TabStop = false;
            // 
            // lblFixedMandatoryWho
            // 
            resources.ApplyResources(lblFixedMandatoryWho, "lblFixedMandatoryWho");
            lblFixedMandatoryWho.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryWho.Name = "lblFixedMandatoryWho";
            // 
            // lblFixedMandatoryTag
            // 
            resources.ApplyResources(lblFixedMandatoryTag, "lblFixedMandatoryTag");
            lblFixedMandatoryTag.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryTag.Name = "lblFixedMandatoryTag";
            // 
            // lblFixedBatteryChange
            // 
            resources.ApplyResources(lblFixedBatteryChange, "lblFixedBatteryChange");
            lblFixedBatteryChange.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedBatteryChange.Name = "lblFixedBatteryChange";
            // 
            // lblFixedMandatoryHwType
            // 
            resources.ApplyResources(lblFixedMandatoryHwType, "lblFixedMandatoryHwType");
            lblFixedMandatoryHwType.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryHwType.Name = "lblFixedMandatoryHwType";
            // 
            // lblFixedMandatoryInUse
            // 
            resources.ApplyResources(lblFixedMandatoryInUse, "lblFixedMandatoryInUse");
            lblFixedMandatoryInUse.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryInUse.Name = "lblFixedMandatoryInUse";
            // 
            // lblFixedMandatoryBuilding
            // 
            resources.ApplyResources(lblFixedMandatoryBuilding, "lblFixedMandatoryBuilding");
            lblFixedMandatoryBuilding.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryBuilding.Name = "lblFixedMandatoryBuilding";
            // 
            // lblFixedMandatoryRoomNumber
            // 
            resources.ApplyResources(lblFixedMandatoryRoomNumber, "lblFixedMandatoryRoomNumber");
            lblFixedMandatoryRoomNumber.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryRoomNumber.Name = "lblFixedMandatoryRoomNumber";
            // 
            // lblFixedMandatoryAssetNumber
            // 
            resources.ApplyResources(lblFixedMandatoryAssetNumber, "lblFixedMandatoryAssetNumber");
            lblFixedMandatoryAssetNumber.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryAssetNumber.Name = "lblFixedMandatoryAssetNumber";
            // 
            // lblFixedMandatoryMain
            // 
            resources.ApplyResources(lblFixedMandatoryMain, "lblFixedMandatoryMain");
            lblFixedMandatoryMain.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryMain.Name = "lblFixedMandatoryMain";
            // 
            // iconImgRoomLetter
            // 
            iconImgRoomLetter.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgRoomLetter, "iconImgRoomLetter");
            iconImgRoomLetter.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgRoomLetter.Name = "iconImgRoomLetter";
            iconImgRoomLetter.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgRoomLetter.TabStop = false;
            // 
            // iconImgHwType
            // 
            iconImgHwType.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgHwType, "iconImgHwType");
            iconImgHwType.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgHwType.Name = "iconImgHwType";
            iconImgHwType.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgHwType.TabStop = false;
            // 
            // iconImgTag
            // 
            iconImgTag.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgTag, "iconImgTag");
            iconImgTag.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgTag.Name = "iconImgTag";
            iconImgTag.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgTag.TabStop = false;
            // 
            // iconImgInUse
            // 
            iconImgInUse.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgInUse, "iconImgInUse");
            iconImgInUse.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgInUse.Name = "iconImgInUse";
            iconImgInUse.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgInUse.TabStop = false;
            // 
            // iconImgServiceDate
            // 
            iconImgServiceDate.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgServiceDate, "iconImgServiceDate");
            iconImgServiceDate.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgServiceDate.Name = "iconImgServiceDate";
            iconImgServiceDate.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgServiceDate.TabStop = false;
            // 
            // iconImgStandard
            // 
            iconImgStandard.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgStandard, "iconImgStandard");
            iconImgStandard.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgStandard.Name = "iconImgStandard";
            iconImgStandard.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgStandard.TabStop = false;
            // 
            // iconImgAdRegistered
            // 
            iconImgAdRegistered.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgAdRegistered, "iconImgAdRegistered");
            iconImgAdRegistered.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgAdRegistered.Name = "iconImgAdRegistered";
            iconImgAdRegistered.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgAdRegistered.TabStop = false;
            // 
            // iconImgBuilding
            // 
            iconImgBuilding.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgBuilding, "iconImgBuilding");
            iconImgBuilding.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgBuilding.Name = "iconImgBuilding";
            iconImgBuilding.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgBuilding.TabStop = false;
            // 
            // iconImgRoomNumber
            // 
            iconImgRoomNumber.CompositingQuality = null;
            resources.ApplyResources(iconImgRoomNumber, "iconImgRoomNumber");
            iconImgRoomNumber.InterpolationMode = null;
            iconImgRoomNumber.Name = "iconImgRoomNumber";
            iconImgRoomNumber.SmoothingMode = null;
            iconImgRoomNumber.TabStop = false;
            // 
            // iconImgSealNumber
            // 
            iconImgSealNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgSealNumber, "iconImgSealNumber");
            iconImgSealNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgSealNumber.Name = "iconImgSealNumber";
            iconImgSealNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgSealNumber.TabStop = false;
            // 
            // iconImgAssetNumber
            // 
            iconImgAssetNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgAssetNumber, "iconImgAssetNumber");
            iconImgAssetNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgAssetNumber.Name = "iconImgAssetNumber";
            iconImgAssetNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgAssetNumber.TabStop = false;
            // 
            // dateTimePickerServiceDate
            // 
            dateTimePickerServiceDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(dateTimePickerServiceDate, "dateTimePickerServiceDate");
            dateTimePickerServiceDate.Name = "dateTimePickerServiceDate";
            // 
            // lblFixedAdRegistered
            // 
            resources.ApplyResources(lblFixedAdRegistered, "lblFixedAdRegistered");
            lblFixedAdRegistered.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedAdRegistered.Name = "lblFixedAdRegistered";
            // 
            // lblFixedStandard
            // 
            resources.ApplyResources(lblFixedStandard, "lblFixedStandard");
            lblFixedStandard.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedStandard.Name = "lblFixedStandard";
            // 
            // groupBoxServiceType
            // 
            groupBoxServiceType.Controls.Add(lblFixedMandatoryServiceType);
            groupBoxServiceType.Controls.Add(textBoxInactiveFormattingRadio);
            groupBoxServiceType.Controls.Add(textBoxInactiveMaintenanceRadio);
            groupBoxServiceType.Controls.Add(radioButtonFormatting);
            groupBoxServiceType.Controls.Add(radioButtonMaintenance);
            groupBoxServiceType.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(groupBoxServiceType, "groupBoxServiceType");
            groupBoxServiceType.Name = "groupBoxServiceType";
            groupBoxServiceType.TabStop = false;
            // 
            // lblFixedMandatoryServiceType
            // 
            resources.ApplyResources(lblFixedMandatoryServiceType, "lblFixedMandatoryServiceType");
            lblFixedMandatoryServiceType.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryServiceType.Name = "lblFixedMandatoryServiceType";
            // 
            // textBoxInactiveFormattingRadio
            // 
            textBoxInactiveFormattingRadio.BackColor = System.Drawing.SystemColors.Control;
            textBoxInactiveFormattingRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(textBoxInactiveFormattingRadio, "textBoxInactiveFormattingRadio");
            textBoxInactiveFormattingRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxInactiveFormattingRadio.Name = "textBoxInactiveFormattingRadio";
            textBoxInactiveFormattingRadio.ReadOnly = true;
            // 
            // textBoxInactiveMaintenanceRadio
            // 
            textBoxInactiveMaintenanceRadio.BackColor = System.Drawing.SystemColors.Control;
            textBoxInactiveMaintenanceRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(textBoxInactiveMaintenanceRadio, "textBoxInactiveMaintenanceRadio");
            textBoxInactiveMaintenanceRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxInactiveMaintenanceRadio.Name = "textBoxInactiveMaintenanceRadio";
            textBoxInactiveMaintenanceRadio.ReadOnly = true;
            // 
            // radioButtonFormatting
            // 
            resources.ApplyResources(radioButtonFormatting, "radioButtonFormatting");
            radioButtonFormatting.ForeColor = System.Drawing.SystemColors.ControlText;
            radioButtonFormatting.Name = "radioButtonFormatting";
            radioButtonFormatting.UseVisualStyleBackColor = true;
            radioButtonFormatting.CheckedChanged += new System.EventHandler(FormatButton1_CheckedChanged);
            // 
            // radioButtonMaintenance
            // 
            resources.ApplyResources(radioButtonMaintenance, "radioButtonMaintenance");
            radioButtonMaintenance.ForeColor = System.Drawing.SystemColors.ControlText;
            radioButtonMaintenance.Name = "radioButtonMaintenance";
            radioButtonMaintenance.UseVisualStyleBackColor = true;
            radioButtonMaintenance.CheckedChanged += new System.EventHandler(MaintenanceButton2_CheckedChanged);
            // 
            // loadingCircleLastService
            // 
            loadingCircleLastService.Active = false;
            loadingCircleLastService.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleLastService.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleLastService, "loadingCircleLastService");
            loadingCircleLastService.Name = "loadingCircleLastService";
            loadingCircleLastService.NumberSpoke = 12;
            loadingCircleLastService.OuterCircleRadius = 11;
            loadingCircleLastService.RotationSpeed = 1;
            loadingCircleLastService.SpokeThickness = 2;
            loadingCircleLastService.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // lblColorLastService
            // 
            resources.ApplyResources(lblColorLastService, "lblColorLastService");
            lblColorLastService.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            lblColorLastService.Name = "lblColorLastService";
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
            // lblServerPort
            // 
            resources.ApplyResources(lblServerPort, "lblServerPort");
            lblServerPort.ForeColor = System.Drawing.SystemColors.ControlText;
            lblServerPort.Name = "lblServerPort";
            // 
            // lblServerIP
            // 
            resources.ApplyResources(lblServerIP, "lblServerIP");
            lblServerIP.ForeColor = System.Drawing.SystemColors.ControlText;
            lblServerIP.Name = "lblServerIP";
            // 
            // lblFixedServerIP
            // 
            resources.ApplyResources(lblFixedServerIP, "lblFixedServerIP");
            lblFixedServerIP.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedServerIP.Name = "lblFixedServerIP";
            // 
            // lblColorServerOperationalStatus
            // 
            resources.ApplyResources(lblColorServerOperationalStatus, "lblColorServerOperationalStatus");
            lblColorServerOperationalStatus.BackColor = System.Drawing.Color.Transparent;
            lblColorServerOperationalStatus.ForeColor = System.Drawing.Color.Silver;
            lblColorServerOperationalStatus.Name = "lblColorServerOperationalStatus";
            // 
            // toolStripVersionText
            // 
            toolStripVersionText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            toolStripVersionText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            toolStripVersionText.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripVersionText.Name = "toolStripVersionText";
            resources.ApplyResources(toolStripVersionText, "toolStripVersionText");
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            comboBoxThemeButton,
            logLabelButton,
            aboutLabelButton,
            toolStripStatusBarText,
            toolStripVersionText});
            resources.ApplyResources(statusStrip1, "statusStrip1");
            statusStrip1.Name = "statusStrip1";
            statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // comboBoxThemeButton
            // 
            comboBoxThemeButton.BackColor = System.Drawing.SystemColors.Control;
            comboBoxThemeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            comboBoxThemeButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripAutoTheme,
            toolStripLightTheme,
            toolStripDarkTheme});
            comboBoxThemeButton.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(comboBoxThemeButton, "comboBoxThemeButton");
            comboBoxThemeButton.Name = "comboBoxThemeButton";
            // 
            // toolStripAutoTheme
            // 
            toolStripAutoTheme.BackColor = System.Drawing.SystemColors.Control;
            toolStripAutoTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripAutoTheme.Name = "toolStripAutoTheme";
            resources.ApplyResources(toolStripAutoTheme, "toolStripAutoTheme");
            toolStripAutoTheme.Click += new System.EventHandler(ToolStripMenuItem1_Click);
            // 
            // toolStripLightTheme
            // 
            toolStripLightTheme.BackColor = System.Drawing.SystemColors.Control;
            toolStripLightTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripLightTheme.Name = "toolStripLightTheme";
            resources.ApplyResources(toolStripLightTheme, "toolStripLightTheme");
            toolStripLightTheme.Click += new System.EventHandler(ToolStripMenuItem2_Click);
            // 
            // toolStripDarkTheme
            // 
            toolStripDarkTheme.BackColor = System.Drawing.SystemColors.Control;
            toolStripDarkTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripDarkTheme.Name = "toolStripDarkTheme";
            resources.ApplyResources(toolStripDarkTheme, "toolStripDarkTheme");
            toolStripDarkTheme.Click += new System.EventHandler(ToolStripMenuItem3_Click);
            // 
            // logLabelButton
            // 
            logLabelButton.BackColor = System.Drawing.SystemColors.Control;
            logLabelButton.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            logLabelButton.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            logLabelButton.ForeColor = System.Drawing.SystemColors.ControlText;
            logLabelButton.Name = "logLabelButton";
            resources.ApplyResources(logLabelButton, "logLabelButton");
            logLabelButton.Click += new System.EventHandler(LogLabelButton_Click);
            logLabelButton.MouseEnter += new System.EventHandler(LogLabel_MouseEnter);
            logLabelButton.MouseLeave += new System.EventHandler(LogLabel_MouseLeave);
            // 
            // aboutLabelButton
            // 
            aboutLabelButton.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            aboutLabelButton.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            aboutLabelButton.ForeColor = System.Drawing.SystemColors.ControlText;
            aboutLabelButton.Name = "aboutLabelButton";
            resources.ApplyResources(aboutLabelButton, "aboutLabelButton");
            aboutLabelButton.Click += new System.EventHandler(AboutLabelButton_Click);
            aboutLabelButton.MouseEnter += new System.EventHandler(AboutLabel_MouseEnter);
            aboutLabelButton.MouseLeave += new System.EventHandler(AboutLabel_MouseLeave);
            // 
            // toolStripStatusBarText
            // 
            toolStripStatusBarText.BackColor = System.Drawing.SystemColors.Control;
            toolStripStatusBarText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            toolStripStatusBarText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            toolStripStatusBarText.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripStatusBarText.Name = "toolStripStatusBarText";
            resources.ApplyResources(toolStripStatusBarText, "toolStripStatusBarText");
            toolStripStatusBarText.Spring = true;
            // 
            // timerAlertHostname
            // 
            timerAlertHostname.Interval = 500;
            // 
            // groupBoxRegistryStatus
            // 
            groupBoxRegistryStatus.Controls.Add(webView2Control);
            groupBoxRegistryStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(groupBoxRegistryStatus, "groupBoxRegistryStatus");
            groupBoxRegistryStatus.Name = "groupBoxRegistryStatus";
            groupBoxRegistryStatus.TabStop = false;
            // 
            // webView2Control
            // 
            webView2Control.AllowExternalDrop = true;
            webView2Control.CreationProperties = null;
            webView2Control.DefaultBackgroundColor = System.Drawing.Color.White;
            resources.ApplyResources(webView2Control, "webView2Control");
            webView2Control.Name = "webView2Control";
            webView2Control.ZoomFactor = 1D;
            // 
            // imgTopBanner
            // 
            resources.ApplyResources(imgTopBanner, "imgTopBanner");
            imgTopBanner.CompositingQuality = null;
            imgTopBanner.InterpolationMode = null;
            imgTopBanner.Name = "imgTopBanner";
            imgTopBanner.SmoothingMode = null;
            imgTopBanner.TabStop = false;
            // 
            // loadingCircleCollectButton
            // 
            loadingCircleCollectButton.Active = false;
            loadingCircleCollectButton.BackColor = System.Drawing.Color.Transparent;
            loadingCircleCollectButton.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleCollectButton.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleCollectButton.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleCollectButton, "loadingCircleCollectButton");
            loadingCircleCollectButton.Name = "loadingCircleCollectButton";
            loadingCircleCollectButton.NumberSpoke = 12;
            loadingCircleCollectButton.OuterCircleRadius = 11;
            loadingCircleCollectButton.RotationSpeed = 1;
            loadingCircleCollectButton.SpokeThickness = 2;
            loadingCircleCollectButton.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            loadingCircleCollectButton.UseWaitCursor = true;
            // 
            // loadingCircleRegisterButton
            // 
            loadingCircleRegisterButton.Active = false;
            loadingCircleRegisterButton.BackColor = System.Drawing.Color.Transparent;
            loadingCircleRegisterButton.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleRegisterButton.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleRegisterButton.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleRegisterButton, "loadingCircleRegisterButton");
            loadingCircleRegisterButton.Name = "loadingCircleRegisterButton";
            loadingCircleRegisterButton.NumberSpoke = 12;
            loadingCircleRegisterButton.OuterCircleRadius = 11;
            loadingCircleRegisterButton.RotationSpeed = 1;
            loadingCircleRegisterButton.SpokeThickness = 2;
            loadingCircleRegisterButton.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // groupBoxServerStatus
            // 
            groupBoxServerStatus.Controls.Add(loadingCircleServerOperationalStatus);
            groupBoxServerStatus.Controls.Add(lblFixedServerIP);
            groupBoxServerStatus.Controls.Add(lblFixedServerOperationalStatus);
            groupBoxServerStatus.Controls.Add(lblFixedServerPort);
            groupBoxServerStatus.Controls.Add(lblColorServerOperationalStatus);
            groupBoxServerStatus.Controls.Add(lblServerIP);
            groupBoxServerStatus.Controls.Add(lblServerPort);
            groupBoxServerStatus.Controls.Add(lblFixedAgentName);
            groupBoxServerStatus.Controls.Add(lblAgentName);
            groupBoxServerStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(groupBoxServerStatus, "groupBoxServerStatus");
            groupBoxServerStatus.Name = "groupBoxServerStatus";
            groupBoxServerStatus.TabStop = false;
            // 
            // loadingCircleServerOperationalStatus
            // 
            loadingCircleServerOperationalStatus.Active = false;
            loadingCircleServerOperationalStatus.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleServerOperationalStatus.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleServerOperationalStatus, "loadingCircleServerOperationalStatus");
            loadingCircleServerOperationalStatus.Name = "loadingCircleServerOperationalStatus";
            loadingCircleServerOperationalStatus.NumberSpoke = 12;
            loadingCircleServerOperationalStatus.OuterCircleRadius = 11;
            loadingCircleServerOperationalStatus.RotationSpeed = 1;
            loadingCircleServerOperationalStatus.SpokeThickness = 2;
            loadingCircleServerOperationalStatus.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // timerOSLabelScroll
            // 
            timerOSLabelScroll.Tick += new System.EventHandler(TimerOSLabelScroll_Tick);
            // 
            // timerFwVersionLabelScroll
            // 
            timerFwVersionLabelScroll.Tick += new System.EventHandler(TimerFwVersionLabelScroll_Tick);
            // 
            // timerVideoCardLabelScroll
            // 
            timerVideoCardLabelScroll.Tick += new System.EventHandler(TimerVideoCardLabelScroll_Tick);
            // 
            // timerRamLabelScroll
            // 
            timerRamLabelScroll.Tick += new System.EventHandler(TimerRamLabelScroll_Tick);
            // 
            // timerProcessorLabelScroll
            // 
            timerProcessorLabelScroll.Tick += new System.EventHandler(TimerProcessorLabelScroll_Tick);
            // 
            // comboBoxBatteryChange
            // 
            comboBoxBatteryChange.BackColor = System.Drawing.SystemColors.Window;
            comboBoxBatteryChange.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxBatteryChange.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxBatteryChange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBoxBatteryChange, "comboBoxBatteryChange");
            comboBoxBatteryChange.FormattingEnabled = true;
            comboBoxBatteryChange.Name = "comboBoxBatteryChange";
            // 
            // comboBoxStandard
            // 
            comboBoxStandard.BackColor = System.Drawing.SystemColors.Window;
            comboBoxStandard.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxStandard.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBoxStandard, "comboBoxStandard");
            comboBoxStandard.FormattingEnabled = true;
            comboBoxStandard.Name = "comboBoxStandard";
            // 
            // comboBoxActiveDirectory
            // 
            comboBoxActiveDirectory.BackColor = System.Drawing.SystemColors.Window;
            comboBoxActiveDirectory.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxActiveDirectory.ButtonColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(comboBoxActiveDirectory, "comboBoxActiveDirectory");
            comboBoxActiveDirectory.FormattingEnabled = true;
            comboBoxActiveDirectory.Name = "comboBoxActiveDirectory";
            // 
            // comboBoxTag
            // 
            comboBoxTag.BackColor = System.Drawing.SystemColors.Window;
            comboBoxTag.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxTag.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxTag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBoxTag, "comboBoxTag");
            comboBoxTag.FormattingEnabled = true;
            comboBoxTag.Name = "comboBoxTag";
            // 
            // comboBoxInUse
            // 
            comboBoxInUse.BackColor = System.Drawing.SystemColors.Window;
            comboBoxInUse.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxInUse.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxInUse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBoxInUse, "comboBoxInUse");
            comboBoxInUse.FormattingEnabled = true;
            comboBoxInUse.Name = "comboBoxInUse";
            // 
            // comboBoxHwType
            // 
            comboBoxHwType.BackColor = System.Drawing.SystemColors.Window;
            comboBoxHwType.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxHwType.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxHwType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBoxHwType, "comboBoxHwType");
            comboBoxHwType.FormattingEnabled = true;
            comboBoxHwType.Name = "comboBoxHwType";
            // 
            // comboBoxBuilding
            // 
            comboBoxBuilding.BackColor = System.Drawing.SystemColors.Window;
            comboBoxBuilding.BorderColor = System.Drawing.Color.FromArgb(122, 122, 122);
            comboBoxBuilding.ButtonColor = System.Drawing.SystemColors.Window;
            comboBoxBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBoxBuilding, "comboBoxBuilding");
            comboBoxBuilding.FormattingEnabled = true;
            comboBoxBuilding.Name = "comboBoxBuilding";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Control;
            Controls.Add(loadingCircleRegisterButton);
            Controls.Add(loadingCircleCollectButton);
            Controls.Add(loadingCircleLastService);
            Controls.Add(lblColorLastService);
            Controls.Add(groupBoxServerStatus);
            Controls.Add(groupBoxRegistryStatus);
            Controls.Add(groupBoxAssetData);
            Controls.Add(groupBoxHwData);
            Controls.Add(imgTopBanner);
            Controls.Add(apcsButton);
            Controls.Add(collectButton);
            Controls.Add(statusStrip1);
            Controls.Add(registerButton);
            Controls.Add(groupBoxServiceType);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            Load += new System.EventHandler(MainForm_Load);
            groupBoxHwData.ResumeLayout(false);
            groupBoxHwData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)iconImgTpmVersion).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgVirtualizationTechnology).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgBrand).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgSecureBoot).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgFwVersion).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgFwType).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgIpAddress).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgMacAddress).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgHostname).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgOperatingSystem).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgVideoCard).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgMediaOperationMode).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgStorageType).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgStorageSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgRam).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgProcessor).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgSerialNumber).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgModel).EndInit();
            groupBoxAssetData.ResumeLayout(false);
            groupBoxAssetData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)iconImgTicketNumber).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgBatteryChange).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgRoomLetter).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgHwType).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgTag).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgInUse).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgServiceDate).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgStandard).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgAdRegistered).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgBuilding).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgRoomNumber).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgSealNumber).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgAssetNumber).EndInit();
            groupBoxServiceType.ResumeLayout(false);
            groupBoxServiceType.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBoxRegistryStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webView2Control).EndInit();
            ((System.ComponentModel.ISupportInitialize)imgTopBanner).EndInit();
            groupBoxServerStatus.ResumeLayout(false);
            groupBoxServerStatus.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #region Variable declaration
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
        private Button apcsButton;
        private ProgressBar progressBar1;
        private Label lblFixedProgressBarPercent;
        private Label lblSecureBoot;
        private Label lblFixedSecureBoot;
        private WebView2 webView2Control;
        private RadioButton radioButtonMaintenance;
        private RadioButton radioButtonFormatting;
        private GroupBox groupBoxServiceType;
        private TextBox textBoxInactiveFormattingRadio;
        private TextBox textBoxInactiveMaintenanceRadio;
        private ToolStripDropDownButton comboBoxThemeButton;
        private ToolStripMenuItem toolStripAutoTheme;
        private ToolStripMenuItem toolStripLightTheme;
        private ToolStripMenuItem toolStripDarkTheme;
        private Label lblColorServerOperationalStatus;
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
        private Label lblColorLastService;
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
        private Label vSeparator3;
        private Label hSeparator1;
        private CustomFlatComboBox comboBoxBuilding;
        private CustomFlatComboBox comboBoxStandard;
        private CustomFlatComboBox comboBoxActiveDirectory;
        private CustomFlatComboBox comboBoxTag;
        private CustomFlatComboBox comboBoxInUse;
        private CustomFlatComboBox comboBoxHwType;
        private CustomFlatComboBox comboBoxBatteryChange;
        private LoadingCircle loadingCircleScanTpmVersion;
        private LoadingCircle loadingCircleScanVirtualizationTechnology;
        private LoadingCircle loadingCircleScanSecureBoot;
        private LoadingCircle loadingCircleScanFwVersion;
        private LoadingCircle loadingCircleScanFwType;
        private LoadingCircle loadingCircleScanIpAddress;
        private LoadingCircle loadingCircleScanMacAddress;
        private LoadingCircle loadingCircleScanHostname;
        private LoadingCircle loadingCircleScanOperatingSystem;
        private LoadingCircle loadingCircleScanVideoCard;
        private LoadingCircle loadingCircleScanMediaOperationMode;
        private LoadingCircle loadingCircleScanStorageType;
        private LoadingCircle loadingCircleScanStorageSize;
        private LoadingCircle loadingCircleScanRam;
        private LoadingCircle loadingCircleScanProcessor;
        private LoadingCircle loadingCircleScanSerialNumber;
        private LoadingCircle loadingCircleScanModel;
        private LoadingCircle loadingCircleScanBrand;
        private LoadingCircle loadingCircleLastService;
        private LoadingCircle loadingCircleCollectButton;
        private LoadingCircle loadingCircleRegisterButton;
        private ToolStripStatusLabel aboutLabelButton;
        private GroupBox groupBoxServerStatus;
        private LoadingCircle loadingCircleServerOperationalStatus;
        private readonly BackgroundWorker backgroundWorker1;
        private ToolStripStatusLabel logLabelButton;
        private readonly LogGenerator log;
        private TaskbarManager tbProgMain;
        private Timer timerFwVersionLabelScroll;
        private Timer timerVideoCardLabelScroll;
        private Timer timerRamLabelScroll;
        private Timer timerProcessorLabelScroll;
        private Timer timerOSLabelScroll;

        #endregion

        /// <summary> 
        /// Sets service mode to format
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormatButton1_CheckedChanged(object sender, EventArgs e)
        {
            serviceTypeURL = ConstantsDLL.Properties.Resources.FORMAT_URL;
        }

        /// <summary> 
        /// Sets service mode to maintenance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaintenanceButton2_CheckedChanged(object sender, EventArgs e)
        {
            serviceTypeURL = ConstantsDLL.Properties.Resources.MAINTENANCE_URL;
        }

        /// <summary> 
        /// Method for setting the auto theme via toolStrip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_AUTOTHEME_CHANGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
            ToggleTheme();
        }

        /// <summary> 
        /// Method for opening the Storage list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageDetailsButton_Click(object sender, EventArgs e)
        {
            StorageDetailForm storageForm = new StorageDetailForm(storageDetail, parametersList, isSystemDarkModeEnabled);
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetWindowThemeForms(storageForm, Theme.Auto);
            _ = storageForm.ShowDialog();
        }

        /// <summary> 
        /// Method for setting the light theme via toolStrip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_LIGHTMODE_CHANGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
            LightTheme();
            isSystemDarkModeEnabled = false;
        }

        /// <summary> 
        /// Method for setting the dark theme via toolStrip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_DARKMODE_CHANGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
            DarkTheme();
            isSystemDarkModeEnabled = true;
        }

        public void LightTheme()
        {
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);

            BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            foreach (GroupBox gb in Controls.OfType<GroupBox>())
            {
                gb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                gb.Paint += CustomColors.GroupBox_PaintLightTheme;

                foreach (Button b in gb.Controls.OfType<Button>())
                {
                    b.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    b.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    b.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
                    b.FlatStyle = System.Windows.Forms.FlatStyle.System;
                }
                foreach (CheckBox cb in gb.Controls.OfType<CheckBox>())
                {
                    cb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                    cb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (CustomFlatComboBox cfcb in gb.Controls.OfType<CustomFlatComboBox>())
                {
                    cfcb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    cfcb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    cfcb.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
                    cfcb.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
                }
                foreach (DataGridView dgv in gb.Controls.OfType<DataGridView>())
                {
                    dgv.BackgroundColor = StringsAndConstants.LIGHT_BACKGROUND;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    dgv.DefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    dgv.DefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (Label l in gb.Controls.OfType<Label>())
                {
                    if (l.Name.Contains("Separator"))
                        l.BackColor = StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR;
                    else if (l.Name.Contains("Mandatory"))
                        l.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
                    else if (l.Name.Contains("Fixed"))
                        l.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    else if (!l.Name.Contains("Color"))
                        l.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                }
                foreach (RadioButton rb in gb.Controls.OfType<RadioButton>())
                {
                    rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (RichTextBox rtb in gb.Controls.OfType<RichTextBox>())
                {
                    rtb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    rtb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (TextBox tb in gb.Controls.OfType<TextBox>())
                {
                    tb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    tb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    if (tb.Name.Contains("Inactive"))
                        tb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                }
            }
            foreach (Button b in Controls.OfType<Button>())
            {
                b.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                b.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                b.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
                b.FlatStyle = System.Windows.Forms.FlatStyle.System;
            }
            foreach (CheckBox cb in Controls.OfType<CheckBox>())
            {
                cb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                cb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (CustomFlatComboBox cfcb in Controls.OfType<CustomFlatComboBox>())
            {
                cfcb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                cfcb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                cfcb.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
                cfcb.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            }
            foreach (DataGridView dgv in Controls.OfType<DataGridView>())
            {
                dgv.BackgroundColor = StringsAndConstants.LIGHT_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                dgv.DefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                dgv.DefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (Label l in Controls.OfType<Label>())
            {
                if (l.Name.Contains("Separator"))
                    l.BackColor = StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR;
                else if (l.Name.Contains("Mandatory"))
                    l.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
                else if (l.Name.Contains("Fixed"))
                    l.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                else if (!l.Name.Contains("Color"))
                    l.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            }
            foreach (RadioButton rb in Controls.OfType<RadioButton>())
            {
                rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (RichTextBox rtb in Controls.OfType<RichTextBox>())
            {
                rtb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                rtb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (TextBox tb in Controls.OfType<TextBox>())
            {
                tb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                tb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                if (tb.Name.Contains("Inactive"))
                    tb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            }

            if (offlineMode)
            {
                lblServerIP.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblServerPort.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblColorServerOperationalStatus.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblAgentName.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblColorLastService.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblColorCompliant.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }

            comboBoxThemeButton.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            comboBoxThemeButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
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

            toolStripAutoTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_AUTOTHEME_LIGHT_PATH));
            toolStripLightTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_LIGHTTHEME_LIGHT_PATH));
            toolStripDarkTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_DARKTHEME_LIGHT_PATH));
            comboBoxThemeButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_AUTOTHEME_LIGHT_PATH));
            logLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_LOG_LIGHT_PATH));
            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_ABOUT_LIGHT_PATH));
            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.MAIN_BANNER_LIGHT_PATH));
            iconImgBrand.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_BRAND_LIGHT_PATH));
            iconImgModel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_MODEL_LIGHT_PATH));
            iconImgSerialNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SERIAL_NUMBER_LIGHT_PATH));
            iconImgProcessor.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_CPU_LIGHT_PATH));
            iconImgRam.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_RAM_LIGHT_PATH));
            iconImgStorageSize.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_DISK_SIZE_LIGHT_PATH));
            iconImgStorageType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_HDD_LIGHT_PATH));
            iconImgMediaOperationMode.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_AHCI_LIGHT_PATH));
            iconImgVideoCard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_GPU_LIGHT_PATH));
            iconImgOperatingSystem.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_WINDOWS_LIGHT_PATH));
            iconImgHostname.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_HOSTNAME_LIGHT_PATH));
            iconImgMacAddress.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_MAC_LIGHT_PATH));
            iconImgIpAddress.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_IP_LIGHT_PATH));
            iconImgFwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_BIOS_LIGHT_PATH));
            iconImgFwVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_BIOS_VERSION_LIGHT_PATH));
            iconImgSecureBoot.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SECURE_BOOT_LIGHT_PATH));
            iconImgAssetNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_ASSET_LIGHT_PATH));
            iconImgSealNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SEAL_LIGHT_PATH));
            iconImgRoomNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_ROOM_LIGHT_PATH));
            iconImgBuilding.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_BUILDING_LIGHT_PATH));
            iconImgAdRegistered.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SERVER_LIGHT_PATH));
            iconImgStandard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_STANDARD_LIGHT_PATH));
            iconImgServiceDate.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SERVICE_LIGHT_PATH));
            iconImgRoomLetter.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_LETTER_LIGHT_PATH));
            iconImgInUse.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_IN_USE_LIGHT_PATH));
            iconImgTag.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_STICKER_LIGHT_PATH));
            iconImgHwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_TYPE_LIGHT_PATH));
            iconImgVirtualizationTechnology.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_VT_X_LIGHT_PATH));
            iconImgTpmVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_TPM_LIGHT_PATH));
            iconImgBatteryChange.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_CMOS_BATTERY_LIGHT_PATH));
            iconImgTicketNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_TICKET_LIGHT_PATH));
        }

        public void DarkTheme()
        {
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);

            BackColor = StringsAndConstants.DARK_BACKGROUND;

            foreach (GroupBox gb in Controls.OfType<GroupBox>())
            {
                gb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                gb.Paint += CustomColors.GroupBox_PaintDarkTheme;

                foreach (Button b in gb.Controls.OfType<Button>())
                {
                    b.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    b.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    b.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
                    b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                }
                foreach (CheckBox cb in gb.Controls.OfType<CheckBox>())
                {
                    cb.BackColor = StringsAndConstants.DARK_BACKGROUND;
                    cb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (CustomFlatComboBox cfcb in gb.Controls.OfType<CustomFlatComboBox>())
                {
                    cfcb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    cfcb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    cfcb.BorderColor = StringsAndConstants.DARK_FORECOLOR;
                    cfcb.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
                }
                foreach (DataGridView dgv in gb.Controls.OfType<DataGridView>())
                {
                    dgv.BackgroundColor = StringsAndConstants.DARK_BACKGROUND;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKGROUND;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    dgv.DefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    dgv.DefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (Label l in gb.Controls.OfType<Label>())
                {
                    if (l.Name.Contains("Separator"))
                        l.BackColor = StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR;
                    else if (l.Name.Contains("Mandatory"))
                        l.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
                    else if (l.Name.Contains("Fixed"))
                        l.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    else if (!l.Name.Contains("Color"))
                        l.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                }
                foreach (RadioButton rb in gb.Controls.OfType<RadioButton>())
                {
                    rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (RichTextBox rtb in gb.Controls.OfType<RichTextBox>())
                {
                    rtb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    rtb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (TextBox tb in gb.Controls.OfType<TextBox>())
                {
                    tb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    tb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    if (tb.Name.Contains("Inactive"))
                        tb.BackColor = StringsAndConstants.DARK_BACKGROUND;
                }
            }
            foreach (Button b in Controls.OfType<Button>())
            {
                b.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                b.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                b.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
                b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            }
            foreach (CheckBox cb in Controls.OfType<CheckBox>())
            {
                cb.BackColor = StringsAndConstants.DARK_BACKGROUND;
                cb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (CustomFlatComboBox cfcb in Controls.OfType<CustomFlatComboBox>())
            {
                cfcb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                cfcb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                cfcb.BorderColor = StringsAndConstants.DARK_FORECOLOR;
                cfcb.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            }
            foreach (DataGridView dgv in Controls.OfType<DataGridView>())
            {
                dgv.BackgroundColor = StringsAndConstants.DARK_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                dgv.DefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                dgv.DefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (Label l in Controls.OfType<Label>())
            {
                if (l.Name.Contains("Separator"))
                    l.BackColor = StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR;
                else if (l.Name.Contains("Mandatory"))
                    l.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
                else if (l.Name.Contains("Fixed"))
                    l.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                else if (!l.Name.Contains("Color"))
                    l.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            }
            foreach (RadioButton rb in Controls.OfType<RadioButton>())
            {
                rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (RichTextBox rtb in Controls.OfType<RichTextBox>())
            {
                rtb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                rtb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (TextBox tb in Controls.OfType<TextBox>())
            {
                tb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                tb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                if (tb.Name.Contains("Inactive"))
                    tb.BackColor = StringsAndConstants.DARK_BACKGROUND;
            }

            if (offlineMode)
            {
                lblServerIP.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblServerPort.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblColorServerOperationalStatus.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblAgentName.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblColorLastService.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblColorCompliant.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }

            comboBoxThemeButton.BackColor = StringsAndConstants.DARK_BACKGROUND;
            comboBoxThemeButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
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

            toolStripAutoTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_AUTOTHEME_DARK_PATH));
            toolStripLightTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_LIGHTTHEME_DARK_PATH));
            toolStripDarkTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_DARKTHEME_DARK_PATH));
            comboBoxThemeButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_AUTOTHEME_DARK_PATH));
            logLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_LOG_DARK_PATH));
            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_ABOUT_DARK_PATH));
            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.MAIN_BANNER_DARK_PATH));
            iconImgBrand.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_BRAND_DARK_PATH));
            iconImgModel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_MODEL_DARK_PATH));
            iconImgSerialNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SERIAL_NUMBER_DARK_PATH));
            iconImgProcessor.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_CPU_DARK_PATH));
            iconImgRam.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_RAM_DARK_PATH));
            iconImgStorageSize.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_DISK_SIZE_DARK_PATH));
            iconImgStorageType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_HDD_DARK_PATH));
            iconImgMediaOperationMode.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_AHCI_DARK_PATH));
            iconImgVideoCard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_GPU_DARK_PATH));
            iconImgOperatingSystem.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_WINDOWS_DARK_PATH));
            iconImgHostname.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_HOSTNAME_DARK_PATH));
            iconImgMacAddress.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_MAC_DARK_PATH));
            iconImgIpAddress.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_IP_DARK_PATH));
            iconImgFwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_BIOS_DARK_PATH));
            iconImgFwVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_BIOS_VERSION_DARK_PATH));
            iconImgSecureBoot.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SECURE_BOOT_DARK_PATH));
            iconImgAssetNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_ASSET_DARK_PATH));
            iconImgSealNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SEAL_DARK_PATH));
            iconImgRoomNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_ROOM_DARK_PATH));
            iconImgBuilding.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_BUILDING_DARK_PATH));
            iconImgAdRegistered.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SERVER_DARK_PATH));
            iconImgStandard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_STARDARD_DARK_PATH));
            iconImgServiceDate.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SERVICE_DARK_PATH));
            iconImgRoomLetter.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_LETTER_DARK_PATH));
            iconImgInUse.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_IN_USE_DARK_PATH));
            iconImgTag.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_STICKER_DARK_PATH));
            iconImgHwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_TYPE_DARK_PATH));
            iconImgVirtualizationTechnology.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_VT_X_DARK_PATH));
            iconImgTpmVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_TPM_DARK_PATH));
            iconImgBatteryChange.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_CMOS_BATTERY_DARK_PATH));
            iconImgTicketNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_TICKET_DARK_PATH));
        }

        /// <summary> 
        /// Method for auto selecting the app theme
        /// </summary>
        private void ToggleTheme()
        {
            (int themeFileSet, bool _) = MiscMethods.GetFileThemeMode(parametersList, MiscMethods.GetSystemThemeMode());
            switch (themeFileSet)
            {
                case 0:
                    LightTheme();
                    isSystemDarkModeEnabled = false;
                    break;
                case 1:
                    DarkTheme();
                    isSystemDarkModeEnabled = true;
                    break;
            }
        }

        /// <summary>
        /// Allows the theme to change automatically according to the system one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
                ToggleTheme();
        }

        /// <summary> 
        /// Sets highlight about label when hovering with the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutLabel_MouseEnter(object sender, EventArgs e)
        {
            aboutLabelButton.ForeColor = StringsAndConstants.HIGHLIGHT_LABEL_COLOR;
        }

        /// <summary> 
        /// Allow to OS label to slide left to right (and vice versa) if it is longer than its parent groupbox width
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerOSLabelScroll_Tick(object sender, EventArgs e)
        {
            if (xPosOS + lblOperatingSystem.Width > rightBound && invertOSScroll == false)
            {
                lblOperatingSystem.Location = new Point(xPosOS, yPosOS);
                xPosOS -= Convert.ToInt32(ConstantsDLL.Properties.Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertOSScroll = true;
            }

            if (xPosOS < leftBound && invertOSScroll == true)
            {
                lblOperatingSystem.Location = new Point(xPosOS, yPosOS);
                xPosOS += Convert.ToInt32(ConstantsDLL.Properties.Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertOSScroll = false;
            }
        }

        /// <summary> 
        /// Allow to firmware version label to slide left to right (and vice versa) if it is longer than its parent groupbox width
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerFwVersionLabelScroll_Tick(object sender, EventArgs e)
        {
            if (xPosFwVersion + lblFwVersion.Width > rightBound && invertFwVersionScroll == false)
            {
                lblFwVersion.Location = new Point(xPosFwVersion, yPosFwVersion);
                xPosFwVersion -= Convert.ToInt32(ConstantsDLL.Properties.Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertFwVersionScroll = true;
            }

            if (xPosFwVersion < leftBound && invertFwVersionScroll == true)
            {
                lblFwVersion.Location = new Point(xPosFwVersion, yPosFwVersion);
                xPosFwVersion += Convert.ToInt32(ConstantsDLL.Properties.Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertFwVersionScroll = false;
            }
        }

        /// <summary> 
        /// Allow to video card label to slide left to right (and vice versa) if it is longer than its parent groupbox width
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerVideoCardLabelScroll_Tick(object sender, EventArgs e)
        {
            if (xPosVideoCard + lblVideoCard.Width > rightBound && invertVideoCardScroll == false)
            {
                lblVideoCard.Location = new Point(xPosVideoCard, yPosFwVersion);
                xPosVideoCard -= Convert.ToInt32(ConstantsDLL.Properties.Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertVideoCardScroll = true;
            }

            if (xPosVideoCard < leftBound && invertVideoCardScroll == true)
            {
                lblVideoCard.Location = new Point(xPosVideoCard, yPosFwVersion);
                xPosVideoCard += Convert.ToInt32(ConstantsDLL.Properties.Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertVideoCardScroll = false;
            }
        }

        /// <summary> 
        /// Allow to RAM label to slide left to right (and vice versa) if it is longer than its parent groupbox width
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerRamLabelScroll_Tick(object sender, EventArgs e)
        {
            if (xPosRam + lblRam.Width > rightBound && invertRamScroll == false)
            {
                lblRam.Location = new Point(xPosRam, yPosRam);
                xPosRam -= Convert.ToInt32(ConstantsDLL.Properties.Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertRamScroll = true;
            }

            if (xPosRam < leftBound && invertRamScroll == true)
            {
                lblRam.Location = new Point(xPosRam, yPosRam);
                xPosRam += Convert.ToInt32(ConstantsDLL.Properties.Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertRamScroll = false;
            }
        }

        /// <summary> 
        /// Allow to processor label to slide left to right (and vice versa) if it is longer than its parent groupbox width
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerProcessorLabelScroll_Tick(object sender, EventArgs e)
        {
            if (xPosProcessor + lblProcessor.Width > rightBound && invertProcessorScroll == false)
            {
                lblProcessor.Location = new Point(xPosProcessor, yPosProcessor);
                xPosProcessor -= Convert.ToInt32(ConstantsDLL.Properties.Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertProcessorScroll = true;
            }

            if (xPosProcessor < leftBound && invertProcessorScroll == true)
            {
                lblProcessor.Location = new Point(xPosProcessor, yPosProcessor);
                xPosProcessor += Convert.ToInt32(ConstantsDLL.Properties.Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertProcessorScroll = false;
            }
        }

        /// <summary> 
        /// Resets highlight about label when hovering with the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutLabel_MouseLeave(object sender, EventArgs e)
        {
            aboutLabelButton.ForeColor = !isSystemDarkModeEnabled ? StringsAndConstants.LIGHT_FORECOLOR : StringsAndConstants.DARK_FORECOLOR;
        }

        /// <summary> 
        /// Sets highlight log label when hovering with the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogLabel_MouseEnter(object sender, EventArgs e)
        {
            logLabelButton.ForeColor = StringsAndConstants.HIGHLIGHT_LABEL_COLOR;
        }

        /// <summary> 
        /// Resets highlight log label when hovering with the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogLabel_MouseLeave(object sender, EventArgs e)
        {
            logLabelButton.ForeColor = !isSystemDarkModeEnabled ? StringsAndConstants.LIGHT_FORECOLOR : StringsAndConstants.DARK_FORECOLOR;
        }

        /// <summary> 
        /// Opens the log file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogLabelButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_OPENING_LOG, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
#if DEBUG
            System.Diagnostics.Process.Start(parametersList[2][0] + ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.DEV_STATUS + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#else
            System.Diagnostics.Process.Start(parametersList[2][0] + ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#endif
        }

        /// <summary> 
        /// Opens the About box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutLabelButton_Click(object sender, EventArgs e)
        {
            AboutBox aboutForm = new AboutBox(ghc, log, parametersList, isSystemDarkModeEnabled);
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetWindowThemeForms(aboutForm, Theme.Auto);
            _ = aboutForm.ShowDialog();
        }

        /// <summary> 
        /// Opens the selected webpage, according to the IP and port specified in the comboboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApcsButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_VIEW_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            _ = System.Diagnostics.Process.Start(ConstantsDLL.Properties.Resources.HTTP + serverIP + ":" + serverPort);
        }

        /// <summary> 
        /// Handles the closing of the current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_CLOSING_MAIN_FORM, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Deletes downloaded json files
            File.Delete(StringsAndConstants.MODEL_FILE_PATH);
            File.Delete(StringsAndConstants.CREDENTIALS_FILE_PATH);
            File.Delete(StringsAndConstants.ASSET_FILE_PATH);
            File.Delete(StringsAndConstants.CONFIG_FILE_PATH);

            //Kills Webview2 instance
            webView2Control.Dispose();
            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }

        /// <summary> 
        /// Loads the form, sets some combobox values, create timers (1000 ms cadence), and triggers a hardware collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>Returns a asynchronous task</returns>
        private async void MainForm_Load(object sender, EventArgs e)
        {
            #region Define loading circle parameters

            switch (MiscMethods.GetWindowsScaling())
            {
                case 100:
                    //Init loading circles parameters for 100% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    }

                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    }
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    }
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    }
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    }
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    }
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    }
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    }
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    }
                    break;
            }
            foreach (GroupBox gb in Controls.OfType<GroupBox>())
            {
                foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                {
                    lc.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
                    lc.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
                }
            }
            foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
            {
                lc.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
                lc.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            }

            #endregion

            //Sets current and maximum values for the progressbar
            progressBar1.Maximum = 18;
            progressBar1.Value = 0;

            //Sets label position for their default positions
            xPosOS = lblOperatingSystem.Location.X;
            yPosOS = lblOperatingSystem.Location.Y;
            xPosFwVersion = lblFwVersion.Location.X;
            yPosFwVersion = lblFwVersion.Location.Y;
            xPosVideoCard = lblVideoCard.Location.X;
            yPosVideoCard = lblVideoCard.Location.Y;
            xPosRam = lblRam.Location.X;
            yPosRam = lblRam.Location.Y;
            xPosProcessor = lblProcessor.Location.X;
            yPosProcessor = lblProcessor.Location.Y;

            //If stats in non-offline mode, instantiates WebView2 and show a Busy form until loading is complete
            if (!offlineMode)
            {
                bw = new BusyForm(parametersList, isSystemDarkModeEnabled)
                {
                    Visible = true,
                };
                await SendData.LoadWebView2(log, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI), webView2Control);
                bw.Visible = false;
            }

            #region Sets timer settings for respective alerts
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

            leftBound = lblOperatingSystem.Location.X;
            rightBound = groupBoxHwData.Width;

            lblFixedOperatingSystem.BringToFront();
            lblOperatingSystem.SendToBack();
            timerOSLabelScroll.Start();

            lblFixedFwVersion.BringToFront();
            lblFwVersion.SendToBack();
            timerFwVersionLabelScroll.Start();

            lblFixedVideoCard.BringToFront();
            lblVideoCard.SendToBack();
            timerVideoCardLabelScroll.Start();

            lblFixedRam.BringToFront();
            lblRam.SendToBack();
            timerRamLabelScroll.Start();

            lblFixedProcessor.BringToFront();
            lblProcessor.SendToBack();
            timerProcessorLabelScroll.Start();

            vSeparator3.BringToFront();

            try
            {
                _ = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain();
                comboBoxActiveDirectory.SelectedIndex = 0;
            }
            catch
            {
                comboBoxActiveDirectory.SelectedIndex = 1;
            }
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_AD_REGISTERED, comboBoxActiveDirectory.SelectedItem.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            if (!offlineMode)
                lblAgentName.Text = agentData[1].ToUpper(); //Prints agent name
            lblServerIP.Text = serverIP; //Prints IP address
            lblServerPort.Text = serverPort; //Prints port number
            dateTimePickerServiceDate.MaxDate = DateTime.Today; //Define max date of datetimepicker to current day
            FormClosing += MainForm_Closing; //Handles Form closing
            tbProgMain = TaskbarManager.Instance; //Handles taskbar progress bar
            CollectButton_Click(sender, e); //Start collecting
        }

        /// <summary>
        /// Free resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Disposed(object sender, EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
        }

        /// <summary> 
        /// Restricts textbox4 only with chars
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxCharsOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back))
                e.Handled = true;
        }

        /// <summary> 
        /// Restricts textbox only with numbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxNumbersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        /// <summary> 
        /// Sets the Hostname label to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void AlertFlashTextHostname(object myObject, EventArgs myEventArgs)
        {
            lblHostname.ForeColor = lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Sets the MediaOperations label to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void AlertFlashTextMediaOperationMode(object myobject, EventArgs myEventArgs)
        {
            lblMediaOperationMode.ForeColor = lblMediaOperationMode.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblMediaOperationMode.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Sets the Secure Boot label to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void AlertFlashTextSecureBoot(object myobject, EventArgs myEventArgs)
        {
            lblSecureBoot.ForeColor = lblSecureBoot.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblSecureBoot.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Sets the VT label to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void AlertFlashTextVirtualizationTechnology(object myobject, EventArgs myEventArgs)
        {
            lblVirtualizationTechnology.ForeColor = lblVirtualizationTechnology.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblVirtualizationTechnology.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Sets the SMART label to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void AlertFlashTextSmartStatus(object myobject, EventArgs myEventArgs)
        {
            lblStorageType.ForeColor = lblStorageType.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblStorageType.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Sets the BIOS Version label to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void FlashTextFwVersion(object myobject, EventArgs myEventArgs)
        {
            lblFwVersion.ForeColor = lblFwVersion.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblFwVersion.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Sets the Mac and IP labels to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void FlashTextNetConnectivity(object myobject, EventArgs myEventArgs)
        {
            if (lblMacAddress.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true)
            {
                lblMacAddress.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblIpAddress.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            }
            else if (lblMacAddress.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false)
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

        /// <summary> 
        /// Sets the Firmware Type label to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void FlashTextBIOSType(object myobject, EventArgs myEventArgs)
        {
            lblFwType.ForeColor = lblFwType.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblFwType.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Sets the Physical Memory label to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void AlertFlashTextRamAmount(object myobject, EventArgs myEventArgs)
        {
            lblRam.ForeColor = lblRam.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblRam.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Sets the TPM label to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void AlertFlashTextTpmVersion(object myobject, EventArgs myEventArgs)
        {
            lblTpmVersion.ForeColor = lblTpmVersion.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblTpmVersion.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Starts the collection process
        /// </summary>
        /// <returns>Returns a asynchronous task</returns>
        private async void Collecting()
        {
            #region Writes a dash in the labels, while scanning the hardware
            lblColorLastService.Text = ConstantsDLL.Properties.Resources.DASH;
            lblBrand.Text = ConstantsDLL.Properties.Resources.DASH;
            lblModel.Text = ConstantsDLL.Properties.Resources.DASH;
            lblSerialNumber.Text = ConstantsDLL.Properties.Resources.DASH;
            lblProcessor.Text = ConstantsDLL.Properties.Resources.DASH;
            lblRam.Text = ConstantsDLL.Properties.Resources.DASH;
            lblStorageSize.Text = ConstantsDLL.Properties.Resources.DASH;
            lblColorCompliant.Text = ConstantsDLL.Properties.Resources.DASH;
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
            lblColorServerOperationalStatus.Text = ConstantsDLL.Properties.Resources.DASH;
            #endregion

            #region Show loading circles while scanning the hardware
            loadingCircleScanBrand.Visible = true;
            loadingCircleScanModel.Visible = true;
            loadingCircleScanSerialNumber.Visible = true;
            loadingCircleScanProcessor.Visible = true;
            loadingCircleScanRam.Visible = true;
            loadingCircleScanStorageSize.Visible = true;
            loadingCircleCompliant.Visible = true;
            loadingCircleScanStorageType.Visible = true;
            loadingCircleScanMediaOperationMode.Visible = true;
            loadingCircleScanVideoCard.Visible = true;
            loadingCircleScanOperatingSystem.Visible = true;
            loadingCircleScanHostname.Visible = true;
            loadingCircleScanMacAddress.Visible = true;
            loadingCircleScanIpAddress.Visible = true;
            loadingCircleScanFwType.Visible = true;
            loadingCircleScanFwVersion.Visible = true;
            loadingCircleScanSecureBoot.Visible = true;
            loadingCircleScanVirtualizationTechnology.Visible = true;
            loadingCircleScanTpmVersion.Visible = true;
            loadingCircleLastService.Visible = true;
            loadingCircleCollectButton.Visible = true;
            loadingCircleScanBrand.Active = true;
            loadingCircleScanModel.Active = true;
            loadingCircleScanSerialNumber.Active = true;
            loadingCircleScanProcessor.Active = true;
            loadingCircleScanRam.Active = true;
            loadingCircleScanStorageSize.Active = true;
            loadingCircleCompliant.Active = true;
            loadingCircleScanStorageType.Active = true;
            loadingCircleScanMediaOperationMode.Active = true;
            loadingCircleScanVideoCard.Active = true;
            loadingCircleScanOperatingSystem.Active = true;
            loadingCircleScanHostname.Active = true;
            loadingCircleScanMacAddress.Active = true;
            loadingCircleScanIpAddress.Active = true;
            loadingCircleScanFwType.Active = true;
            loadingCircleScanFwVersion.Active = true;
            loadingCircleScanSecureBoot.Active = true;
            loadingCircleScanVirtualizationTechnology.Active = true;
            loadingCircleScanTpmVersion.Active = true;
            loadingCircleLastService.Active = true;
            loadingCircleCollectButton.Active = true;
            #endregion

            if (!offlineMode)
            {
                loadingCircleServerOperationalStatus.Visible = true;
                loadingCircleServerOperationalStatus.Active = true;

                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_PINGGING_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                //Feches model info from server
                serverOnline = await JsonFileReaderDLL.ModelFileReader.CheckHostMT(serverIP, serverPort);

                loadingCircleServerOperationalStatus.Visible = false;
                loadingCircleServerOperationalStatus.Active = false;

                if (serverOnline && serverPort != string.Empty)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_ONLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    lblColorServerOperationalStatus.Text = Strings.ONLINE;
                    lblColorServerOperationalStatus.ForeColor = StringsAndConstants.ONLINE_ALERT;
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_OFFLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    lblColorServerOperationalStatus.Text = Strings.OFFLINE;
                    lblColorServerOperationalStatus.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                }
            }
            else
            {
                loadingCircleServerOperationalStatus.Visible = false;
                loadingCircleServerOperationalStatus.Active = false;
                loadingCircleLastService.Visible = false;
                loadingCircleLastService.Active = false;
                loadingCircleCompliant.Visible = false;
                loadingCircleCompliant.Active = false;
                lblServerIP.Text = lblServerPort.Text = lblAgentName.Text = lblColorServerOperationalStatus.Text = lblColorLastService.Text = lblColorCompliant.Text = Strings.OFFLINE_MODE_ACTIVATED;
                lblServerIP.ForeColor = lblServerPort.ForeColor = lblAgentName.ForeColor = lblColorServerOperationalStatus.ForeColor = lblColorLastService.ForeColor = lblColorCompliant.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }

            //Alerts stop blinking and resets red color
            timerAlertHostname.Enabled = false;
            timerAlertMediaOperationMode.Enabled = false;
            timerAlertSecureBoot.Enabled = false;
            timerAlertFwVersion.Enabled = false;
            timerAlertNetConnectivity.Enabled = false;
            timerAlertFwType.Enabled = false;
            timerAlertVirtualizationTechnology.Enabled = false;
            timerAlertTpmVersion.Enabled = false;
            timerAlertRamAmount.Enabled = false;

            //Resets the colors while scanning the hardware
            if (isSystemDarkModeEnabled)
            {
                lblHostname.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblMediaOperationMode.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblSecureBoot.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblFwVersion.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblFwType.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblVirtualizationTechnology.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblRam.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblTpmVersion.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblMacAddress.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                lblIpAddress.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            }
            else
            {
                lblHostname.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblMediaOperationMode.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblSecureBoot.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblFwVersion.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblFwType.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblVirtualizationTechnology.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblRam.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblTpmVersion.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblMacAddress.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                lblIpAddress.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            }
        }

        /// <summary> 
        /// Auxiliary method for progress bar
        /// </summary>
        /// <param name="k">Progress bar step</param>
        /// <returns>Percentage</returns>
        private int ProgressAuxFunction(int k)
        {
            return k * 100 / progressBar1.Maximum;
        }

        /// <summary> 
        /// Runs on a separate thread, calling methods from the HardwareInfo class, and setting the variables, while reporting the progress to the progressbar
        /// </summary>
        /// <param name="worker"></param>
        private void CollectThread(BackgroundWorker worker)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_START_COLLECTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            i = 0;

            //Scans for PC maker
            brand = HardwareInfo.GetBrand();
            if (brand == ConstantsDLL.Properties.Resources.TO_BE_FILLED_BY_OEM || brand == string.Empty)
                brand = HardwareInfo.GetBrandAlt();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_BM, brand, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for PC model
            model = HardwareInfo.GetModel();
            if (model == ConstantsDLL.Properties.Resources.TO_BE_FILLED_BY_OEM || model == string.Empty)
            {
                model = HardwareInfo.GetModelAlt();
                if (model == ConstantsDLL.Properties.Resources.TO_BE_FILLED_BY_OEM || model == string.Empty)
                    model = ConstantsDLL.Properties.Strings.UNKNOWN;
            }
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MODEL, model, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for motherboard Serial number
            serialNumber = HardwareInfo.GetSerialNumber();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_SERIALNO, serialNumber, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for CPU information
            processor = HardwareInfo.GetProcessorInfo();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_PROCNAME, processor, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for RAM amount and total number of slots
            ram = HardwareInfo.GetRam() + " (" + HardwareInfo.GetNumFreeRamSlots() +
                Strings.SLOTS_OF + HardwareInfo.GetNumRamSlots() + Strings.OCCUPIED + ")";
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_PM, ram, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for Storage size
            storageSize = HardwareInfo.GetStorageTotalSize();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_HDSIZE, storageSize, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for Storage type
            storageSummary = HardwareInfo.GetStorageSummary();
            storageDetail = new List<List<string>>
            {
                HardwareInfo.GetStorageTypeList(),
                HardwareInfo.GetStorageSizeList(),
                HardwareInfo.GetStorageConnectionList(),
                HardwareInfo.GetStorageModelList(),
                HardwareInfo.GetStorageSerialNumberList(),
                HardwareInfo.GetStorageSmartList()
            };
            storageDetail = MiscMethods.Transpose(storageDetail);
            for (int i = 0; i < storageDetail.Count; i++)
            {
                for (int j = 0; j < storageDetail[i].Count; j++)
                {
                    if (storageDetail[i][j] == null)
                        storageDetail[i][j] = ConstantsDLL.Properties.Strings.UNKNOWN;
                }
            }

            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MEDIATYPE, storageSummary, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for Media Operation (IDE/AHCI/NVME)
            mediaOperationMode = HardwareInfo.GetMediaOperationMode();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MEDIAOP, parametersList[8][Convert.ToInt32(mediaOperationMode)], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for GPU information
            videoCard = HardwareInfo.GetVideoCardInfo();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_GPUINFO, videoCard, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for OS infomation
            operatingSystem = HardwareInfo.GetOSString();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_OS, operatingSystem, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for Hostname
            hostname = HardwareInfo.GetHostname();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_HOSTNAME, hostname, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for MAC Address
            macAddress = HardwareInfo.GetMacAddress();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MAC, macAddress, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for IP Address
            ipAddress = HardwareInfo.GetIpAddress();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_IP, ipAddress, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for firmware type
            fwType = HardwareInfo.GetFwType();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_BIOSTYPE, parametersList[6][Convert.ToInt32(fwType)], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for Secure Boot status
            secureBoot = HardwareInfo.GetSecureBoot();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_SECBOOT, StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[9][Convert.ToInt32(secureBoot)])], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for firmware version
            fwVersion = HardwareInfo.GetFirmwareVersion();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_BIOS, fwVersion, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for VT status
            virtualizationTechnology = HardwareInfo.GetVirtualizationTechnology();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_VT, StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[10][Convert.ToInt32(virtualizationTechnology)])], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for TPM status
            tpmVersion = HardwareInfo.GetTPMStatus();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_TPM, parametersList[7][Convert.ToInt32(tpmVersion)], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_END_COLLECTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
        }

        /// <summary> 
        /// Prints the collected data into the form labels, warning the agent when there are forbidden modes
        /// </summary>
        /// <returns>Returns a asynchronous task</returns>
        private async Task PrintHardwareData()
        {
            #region Hides loading circles after scanning the hardware

            foreach (GroupBox gb in Controls.OfType<GroupBox>())
            {
                foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                {
                    if (lc.Name.Contains("Scan"))
                    {
                        lc.Visible = false;
                        lc.Active = false;
                    }
                }
            }
            foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
            {
                if (lc.Name.Contains("Scan"))
                {
                    lc.Visible = false;
                    lc.Active = false;
                }
            }
            #endregion

            #region Prints fetched data into labels
            lblBrand.Text = brand;
            lblModel.Text = model;
            lblSerialNumber.Text = serialNumber;
            lblProcessor.Text = processor;
            lblRam.Text = ram;
            lblStorageSize.Text = storageSize;
            //lblStorageType.Text = storageType;
            lblStorageType.Text = storageSummary;
            lblVideoCard.Text = videoCard;
            lblOperatingSystem.Text = operatingSystem;
            lblHostname.Text = hostname;
            lblMacAddress.Text = macAddress;
            lblIpAddress.Text = ipAddress;
            lblFwVersion.Text = fwVersion;

            lblMediaOperationMode.Text = parametersList[8][Convert.ToInt32(mediaOperationMode)];
            lblFwType.Text = parametersList[6][Convert.ToInt32(fwType)];
            lblSecureBoot.Text = StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[9][Convert.ToInt32(secureBoot)])];
            lblVirtualizationTechnology.Text = StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[10][Convert.ToInt32(virtualizationTechnology)])];
            lblTpmVersion.Text = parametersList[7][Convert.ToInt32(tpmVersion)];

            #endregion

            pass = true;

            try
            {
                nonCompliantCount = 0;
                if (!offlineMode)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_FETCHING_MODEL_FILE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                    //Feches model info from server
                    modelJsonStr = await JsonFileReaderDLL.ModelFileReader.FetchInfoMT(brand, model, fwType, tpmVersion, mediaOperationMode, serverIP, serverPort);
                }

                //If hostname is the default one and its enforcement is enabled
                if (enforcementList[3] == ConstantsDLL.Properties.Resources.TRUE && hostname.Equals(Strings.DEFAULT_HOSTNAME))
                {
                    pass = false;
                    lblHostname.Text += Strings.HOSTNAME_ALERT;
                    timerAlertHostname.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.HOSTNAME_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                //If model Json file does exist, mediaOpMode enforcement is enabled, and the mode is incorrect
                if (enforcementList[2] == ConstantsDLL.Properties.Resources.TRUE && modelJsonStr != null && modelJsonStr[3].Equals(ConstantsDLL.Properties.Resources.FALSE))
                {
                    pass = false;
                    lblMediaOperationMode.Text += Strings.MEDIA_OPERATION_ALERT;
                    timerAlertMediaOperationMode.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.MEDIA_OPERATION_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                //The section below contains the exception cases for Secure Boot enforcement, if it is enabled
                if (enforcementList[6] == ConstantsDLL.Properties.Resources.TRUE && StringsAndConstants.LIST_STATES[Convert.ToInt32(secureBoot)] == ConstantsDLL.Properties.Strings.DEACTIVATED)
                {
                    pass = false;
                    lblSecureBoot.Text += Strings.SECURE_BOOT_ALERT;
                    timerAlertSecureBoot.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.SECURE_BOOT_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                //If model Json file does not exist and server is unreachable
                if (modelJsonStr == null)
                {
                    if (!offlineMode)
                    {
                        pass = false;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                        _ = MessageBox.Show(Strings.DATABASE_REACH_ERROR, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                //If model Json file does exist, firmware version enforcement is enabled, and the version is incorrect
                if (enforcementList[5] == ConstantsDLL.Properties.Resources.TRUE && modelJsonStr != null && !fwVersion.Contains(modelJsonStr[0]))
                {
                    if (!modelJsonStr[0].Equals("-1"))
                    {
                        pass = false;
                        lblFwVersion.Text += Strings.FIRMWARE_VERSION_ALERT;
                        timerAlertFwVersion.Enabled = true;
                        nonCompliantCount++;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.FIRMWARE_VERSION_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    }
                }
                //If model Json file does exist, firmware type enforcement is enabled, and the type is incorrect
                if (enforcementList[4] == ConstantsDLL.Properties.Resources.TRUE && modelJsonStr != null && modelJsonStr[1].Equals(ConstantsDLL.Properties.Resources.FALSE))
                {
                    pass = false;
                    lblFwType.Text += Strings.FIRMWARE_TYPE_ALERT;
                    timerAlertFwType.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.FIRMWARE_TYPE_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
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
                        nonCompliantCount++;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.NETWORK_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    }
                    else //If it's in offline mode
                    {
                        lblMacAddress.Text = Strings.OFFLINE_MODE_ACTIVATED; //Specifies offline mode
                        lblIpAddress.Text = Strings.OFFLINE_MODE_ACTIVATED; //Specifies offline mode
                    }
                }
                //If Virtualization Technology is disabled for UEFI and its enforcement is enabled
                if (enforcementList[7] == ConstantsDLL.Properties.Resources.TRUE && StringsAndConstants.LIST_STATES[Convert.ToInt32(virtualizationTechnology)] == ConstantsDLL.Properties.Strings.DEACTIVATED)
                {
                    pass = false;
                    lblVirtualizationTechnology.Text += Strings.VT_ALERT;
                    timerAlertVirtualizationTechnology.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.VT_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                //If Smart status is not OK and its enforcement is enabled
                //if (enforcementList[1] == ConstantsDLL.Properties.Resources.TRUE && storageDetail[5].Contains(ConstantsDLL.Properties.Resources.PRED_FAIL))
                //{
                //    pass = false;
                //    lblStorageType.Text += Strings.SMART_FAIL;
                //    timerAlertSmartStatus.Enabled = true;
                //    nonCompliantCount++;
                //    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.SMART_FAIL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                //}
                //If model Json file does exist, TPM enforcement is enabled, and TPM version is incorrect
                if (enforcementList[8] == ConstantsDLL.Properties.Resources.TRUE && modelJsonStr != null && modelJsonStr[2].Equals(ConstantsDLL.Properties.Resources.FALSE))
                {
                    pass = false;
                    lblTpmVersion.Text += Strings.TPM_ERROR;
                    timerAlertTpmVersion.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.TPM_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetRamAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, and its limit enforcement is enabled, shows an alert
                if (enforcementList[0] == ConstantsDLL.Properties.Resources.TRUE && d < 4.0 && Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    lblRam.Text += Strings.NOT_ENOUGH_MEMORY;
                    timerAlertRamAmount.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.NOT_ENOUGH_MEMORY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                //If RAM is more than 4GB and OS is x86, and its limit enforcement is enabled, shows an alert
                if (enforcementList[0] == ConstantsDLL.Properties.Resources.TRUE && d > 4.0 && !Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    lblRam.Text += Strings.TOO_MUCH_MEMORY;
                    timerAlertRamAmount.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.TOO_MUCH_MEMORY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                if (pass && !offlineMode)
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_HARDWARE_PASSED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                if (!pass)
                {
                    progressBar1.SetState(2);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                }

                if (!offlineMode)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_FETCHING_ASSET_FILE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                    //Feches asset number data from server
                    assetJsonStr = await JsonFileReaderDLL.AssetFileReader.FetchInfoMT(textBoxAssetNumber.Text, serverIP, serverPort);

                    //If asset exists, prints amount of days since last service
                    if (assetJsonStr[0] != ConstantsDLL.Properties.Resources.FALSE)
                    {
                        lblColorLastService.Text = MiscMethods.SinceLabelUpdate(assetJsonStr[10]);
                        lblColorLastService.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), lblColorLastService.Text, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    }
                    else //If asset doesn't exists, prints text that could not determine last service date
                    {
                        lblColorLastService.Text = MiscMethods.SinceLabelUpdate(string.Empty);
                        lblColorLastService.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), lblColorLastService.Text, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    }
                    loadingCircleLastService.Visible = false;
                    loadingCircleLastService.Active = false;

                    if (nonCompliantCount == 0)
                    {
                        lblColorCompliant.Text = ConstantsDLL.Properties.Strings.NO_PENDENCIES;
                        lblColorCompliant.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
                    }
                    else
                    {
                        lblColorCompliant.Text = nonCompliantCount.ToString() + " " + ConstantsDLL.Properties.Strings.PENDENCIES;
                        lblColorCompliant.ForeColor = StringsAndConstants.ALERT_COLOR;
                    }
                    loadingCircleCompliant.Visible = false;
                    loadingCircleCompliant.Active = false;
                }
            }
            catch (Exception e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), e.Message, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            }
        }

        /// <summary> 
        /// Triggers when the form opens, and when the agent clicks to collect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectButton_Click(object sender, EventArgs e)
        {
            progressBar1.SetState(1);
            tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
            webView2Control.Visible = false;
            Collecting();
            apcsButton.Enabled = false;
            registerButton.Enabled = false;
            collectButton.Enabled = false;
            storageDetailsButton.Visible = false;
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_START_COLLECT_THREAD, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            StartAsync(sender, e);
        }

        /// <summary> 
        /// Starts the worker for threading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartAsync(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
                backgroundWorker1.RunWorkerAsync();
        }

        /// <summary> 
        /// Runs the collectThread method in a separate thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            CollectThread(worker);
        }

        /// <summary> 
        /// Draws the collection progress on the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            percent = e.ProgressPercentage * progressBar1.Maximum / 100;
            tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
            progressBar1.Value = percent;
            lblFixedProgressBarPercent.Text = e.ProgressPercentage.ToString() + "%";
        }

        /// <summary> 
        /// Runs when the collection ends, ending the thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>Returns a asynchronous task</returns>
        private async void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Task p = PrintHardwareData();
            storageDetailsButton.Visible = true;
            await p;

            if (!offlineMode)
            {
                apcsButton.Enabled = true; //Enables accessSystem button
                registerButton.Enabled = true; //Enables register button
            }
            //Hides loading circle
            loadingCircleCollectButton.Visible = false;
            loadingCircleCollectButton.Active = false;
            collectButton.Enabled = true; //Enables collect button
            collectButton.Text = Strings.FETCH_AGAIN; //Updates collect button text
        }

        /// <summary> 
        /// Attributes the data collected previously to the variables which will inside the URL for registration
        /// </summary>
        private void AttrHardwareData()
        {
            serverArgs[12] = brand;
            serverArgs[13] = model;
            serverArgs[14] = serialNumber;
            serverArgs[15] = processor;
            serverArgs[16] = ram;
            serverArgs[17] = storageSize;
            serverArgs[18] = storageSummary;
            serverArgs[19] = mediaOperationMode;
            serverArgs[20] = videoCard;
            serverArgs[21] = operatingSystem;
            serverArgs[22] = hostname;
            serverArgs[23] = fwType;
            serverArgs[24] = fwVersion;
            serverArgs[25] = Array.IndexOf(parametersList[9], secureBoot).ToString();
            serverArgs[26] = Array.IndexOf(parametersList[10], virtualizationTechnology).ToString();
            serverArgs[27] = tpmVersion;
            serverArgs[28] = macAddress;
            serverArgs[29] = ipAddress;
        }

        /// <summary> 
        /// Runs the registration for the website
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RegisterButton_ClickAsync(object sender, EventArgs e)
        {
            webView2Control.Visible = false;
            tbProgMain.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            loadingCircleRegisterButton.Visible = true;
            loadingCircleRegisterButton.Active = true;
            registerButton.Text = ConstantsDLL.Properties.Resources.DASH;
            registerButton.Enabled = false;
            apcsButton.Enabled = false;
            collectButton.Enabled = false;
            AttrHardwareData();

            //If all the mandatory fields are filled and there are no pendencies
            if (!string.IsNullOrWhiteSpace(textBoxAssetNumber.Text) && !string.IsNullOrWhiteSpace(textBoxRoomNumber.Text) && !string.IsNullOrWhiteSpace(textBoxTicketNumber.Text) && comboBoxHwType.SelectedItem != null && comboBoxBuilding.SelectedItem != null && comboBoxInUse.SelectedItem != null && comboBoxTag.SelectedItem != null && comboBoxBatteryChange.SelectedItem != null && comboBoxStandard.SelectedItem != null && (radioButtonFormatting.Checked || radioButtonMaintenance.Checked) && pass == true)
            {
                //Attribute variables to an array which will be sent to the server
                serverArgs[0] = serverIP;
                serverArgs[1] = serverPort;
                serverArgs[2] = textBoxAssetNumber.Text;
                serverArgs[3] = Array.IndexOf(parametersList[4], comboBoxBuilding.SelectedItem.ToString()).ToString();
                serverArgs[4] = textBoxRoomLetter.Text != string.Empty ? textBoxRoomNumber.Text + textBoxRoomLetter.Text : textBoxRoomNumber.Text;
                serverArgs[5] = dateTimePickerServiceDate.Value.ToString(ConstantsDLL.Properties.Resources.DATE_FORMAT).Substring(0, 10);
                serverArgs[6] = serviceTypeURL;
                serverArgs[7] = comboBoxBatteryChange.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.LIST_YES_0) ? Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                serverArgs[8] = textBoxTicketNumber.Text;
                serverArgs[9] = agentData[0];
                serverArgs[10] = comboBoxStandard.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.LIST_STANDARD_GUI_EMPLOYEE) ? Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString();
                serverArgs[11] = comboBoxActiveDirectory.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.LIST_YES_0) ? Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                serverArgs[30] = comboBoxInUse.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.LIST_YES_0) ? Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                serverArgs[31] = textBoxSealNumber.Text;
                serverArgs[32] = comboBoxTag.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.LIST_YES_0) ? Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                serverArgs[33] = Array.IndexOf(parametersList[5], comboBoxHwType.SelectedItem.ToString()).ToString();

                //If asset is discarded
                if (assetJsonStr[0] != ConstantsDLL.Properties.Resources.FALSE && assetJsonStr[9] == "1")
                {
                    tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ConstantsDLL.Properties.Strings.ASSET_DROPPED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    _ = MessageBox.Show(ConstantsDLL.Properties.Strings.ASSET_DROPPED, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                }
                else //If not discarded
                {
                    if (serverOnline && serverPort != string.Empty) //If server is online and port is not null
                    {
                        try //Tries to get the latest register date from the asset number to check if the chosen date is adequate
                        {
                            DateTime registerDate = DateTime.ParseExact(serverArgs[5], ConstantsDLL.Properties.Resources.DATE_FORMAT, CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(assetJsonStr[10], ConstantsDLL.Properties.Resources.DATE_FORMAT, CultureInfo.InvariantCulture);

                            if (registerDate >= lastRegisterDate) //If chosen date is greater or equal than the last format/maintenance date of the PC, let proceed
                            {
                                webView2Control.Visible = true;
                                SendData.ServerSendInfo(serverArgs, log, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI), webView2Control); //Send info to server
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                                await Task.Delay(Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL) * 2);
                                tbProgMain.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                            }
                            else //If chosen date is before the last format/maintenance date of the PC, shows an error
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.INCORRECT_REGISTER_DATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                                _ = MessageBox.Show(Strings.INCORRECT_REGISTER_DATE, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                                tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                            }
                        }
                        catch //If can't retrieve (asset number non existent in the database), register normally
                        {
                            webView2Control.Visible = true;
                            SendData.ServerSendInfo(serverArgs, log, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI), webView2Control); //Send info to server
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                            await Task.Delay(Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL) * 2);
                            tbProgMain.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                        }
                    }
                    else //If the server is out of reach
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ConstantsDLL.Properties.Strings.SERVER_NOT_FOUND_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                        _ = MessageBox.Show(ConstantsDLL.Properties.Strings.SERVER_NOT_FOUND_ERROR, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                        tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                    }
                }
            }
            else if (!pass) //If there are pendencies in the PC config
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.PENDENCY_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(Strings.PENDENCY_ERROR, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
            }
            else //If all fields are not filled
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.MANDATORY_FIELD, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(Strings.MANDATORY_FIELD, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
            }

            //Feches asset number data from server to update the label
            loadingCircleLastService.Visible = true;
            loadingCircleLastService.Active = true;
            loadingCircleRegisterButton.Visible = true;
            loadingCircleRegisterButton.Active = true;
            lblColorLastService.Text = ConstantsDLL.Properties.Resources.DASH;
            assetJsonStr = await JsonFileReaderDLL.AssetFileReader.FetchInfoMT(textBoxAssetNumber.Text, serverIP, serverPort);
            if (assetJsonStr[0] != ConstantsDLL.Properties.Resources.FALSE)
            {
                lblColorLastService.Text = MiscMethods.SinceLabelUpdate(assetJsonStr[10]);
                lblColorLastService.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
            }
            else
            {
                lblColorLastService.Text = MiscMethods.SinceLabelUpdate(string.Empty);
                lblColorLastService.ForeColor = StringsAndConstants.OFFLINE_ALERT;
            }
            loadingCircleLastService.Visible = false;
            loadingCircleLastService.Active = false;
            loadingCircleRegisterButton.Visible = false;
            loadingCircleRegisterButton.Active = false;

            //When finished registering, resets control states
            loadingCircleRegisterButton.Visible = false;
            loadingCircleRegisterButton.Active = false;
            registerButton.Text = Strings.REGISTER_AGAIN;
            registerButton.Enabled = true;
            apcsButton.Enabled = true;
            collectButton.Enabled = true;
        }
    }
}

