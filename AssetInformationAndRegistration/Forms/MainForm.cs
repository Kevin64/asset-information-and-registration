using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using AssetInformationAndRegistration.Properties;
using ConfigurableQualityPictureBoxDLL;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using JsonFileReaderDLL;
using LogGeneratorDLL;
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
using System.Net.Http;
using System.Net.Http.Headers;
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
        private int percent, progressbarCount = 0, nonCompliantCount, leftBound, rightBound;
        private int xPosOS = 0, yPosOS = 0;
        private int xPosFwVersion = 0, yPosFwVersion = 0;
        private int xPosVideoCard = 0, yPosVideoCard = 0;
        private int xPosRam = 0, yPosRam = 0;
        private int xPosProcessor = 0, yPosProcessor = 0;
        private bool isSystemDarkModeEnabled, serverOnline, pass = true, invertOSScroll, invertFwVersionScroll, invertVideoCardScroll, invertRamScroll, invertProcessorScroll;
        private readonly bool offlineMode;
        private readonly string serverIP, serverPort;
        private string processorSummary, ramSummary, storageSummary, videoCardSummary, operatingSystemSummary;
        private readonly string[] serverArgs = new string[34];
        private readonly List<string[]> parametersList, jsonServerSettings;
        private readonly List<string> enforcementList, orgDataList;
        private List<List<string>> videoCardDetailPrev, storageDetailPrev, ramDetailPrev, processorDetailPrev, videoCardDetail, storageDetail, ramDetail, processorDetail;

        private HttpClient client;
        private readonly LogGenerator log;
        private readonly Octokit.GitHubClient ghc;
        private readonly UserPreferenceChangedEventHandler UserPreferenceChanged;
        private readonly BackgroundWorker backgroundWorker1;

        private Agent agent, agentMaintenances;
        private Model modelTemplate;

        private Asset existingAsset, newAsset;
        private readonly firmware existingFirmware, newFirmware;
        private readonly hardware existingHardware, newHardware;
        private readonly List<processor> existingProcessor, newProcessor;
        private readonly List<ram> existingRam, newRam;
        private readonly List<storage> existingStorage, newStorage;
        private readonly List<videoCard> existingVideoCard, newVideoCard;
        private readonly List<maintenances> existingMaintenances, newMaintenances;
        private readonly location existingLocation, newLocation;
        private readonly network existingNetwork, newNetwork;
        private readonly operatingSystem existingOperatingSystem, newOperatingSystem;

        #region Form variable declaration

        private Button storageDetailsButton;
        private LoadingCircle loadingCircleCompliant;
        private TextBox textBoxInactiveUpdateDataRadio;
        private RadioButton radioButtonUpdateData;
        private Button videoCardDetailsButton;
        private GroupBox groupBoxTableMaintenances;
        private DataGridView tableMaintenances;
        private Label lblColorCompliant;
        private Label lblThereIsNothingHere;
        private Label lblBrand;
        private Label lblModel;
        private Label lblSerialNumber;
        private Label lblProcessor;
        private Label lblRam;
        private Label lblHostname;
        private Label lblIpAddress;
        private Label lblFixedBrand;
        private Label lblFixedModel;
        private Label lblFixedSerialNumber;
        private Label lblFixedProcessor;
        private Label lblFixedRam;
        private Label lblFixedOperatingSystem;
        private Label lblFixedHostname;
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
        private ToolStripStatusLabel toolStripVersionText;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusBarText;
        private Button collectButton;
        private Label lblFixedRoomLetter;
        private Label lblFixedFwVersion;
        private Label lblFwVersion;
        private Button apcsButton;
        private ProgressBar progressBar1;
        private Label lblSecureBoot;
        private Label lblFixedSecureBoot;
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
        private ConfigurableQualityPictureBox iconImgStorageType;
        private ConfigurableQualityPictureBox iconImgVideoCard;
        private ConfigurableQualityPictureBox iconImgOperatingSystem;
        private ConfigurableQualityPictureBox iconImgHostname;
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
        private ConfigurableQualityPictureBox iconImgBatteryChange;
        private Label lblFixedBatteryChange;
        private ConfigurableQualityPictureBox iconImgTicketNumber;
        private Label lblFixedTicketNumber;
        private TextBox textBoxTicketNumber;
        private Label lblFixedMandatoryTicketNumber;
        private Label lblFixedMandatoryBatteryChange;
        private Label lblFixedServerIP;
        private Label lblColorLastService;
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
        private Label vSeparator1;
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
        private LoadingCircle loadingCircleScanHostname;
        private LoadingCircle loadingCircleScanOperatingSystem;
        private LoadingCircle loadingCircleScanVideoCard;
        private LoadingCircle loadingCircleScanMediaOperationMode;
        private LoadingCircle loadingCircleScanStorageType;
        private LoadingCircle loadingCircleScanRam;
        private LoadingCircle loadingCircleScanProcessor;
        private LoadingCircle loadingCircleScanSerialNumber;
        private LoadingCircle loadingCircleScanModel;
        private LoadingCircle loadingCircleScanBrand;
        private LoadingCircle loadingCircleLastService;
        private LoadingCircle loadingCircleCollectButton;
        private LoadingCircle loadingCircleRegisterButton;
        private LoadingCircle loadingCircleTableMaintenances;
        private ToolStripStatusLabel aboutLabelButton;
        private GroupBox groupBoxServerStatus;
        private LoadingCircle loadingCircleServerOperationalStatus;
        private ToolStripStatusLabel logLabelButton;
        private TaskbarManager tbProgMain;
        private Timer timerFwVersionLabelScroll;
        private Timer timerVideoCardLabelScroll;
        private Timer timerRamLabelScroll;
        private Timer timerProcessorLabelScroll;
        private Timer timerOSLabelScroll;
        private DataGridViewTextBoxColumn serviceDate;
        private DataGridViewTextBoxColumn serviceType;
        private Button processorDetailsButton;
        private Button ramDetailsButton;
        private ConfigurableQualityPictureBox iconImgMediaOperationMode;
        private Label lblMediaOperationMode;
        private Label lblFixedMediaOperationMode;
        private Label hSeparator2;
        private Label lblInactiveHardware;
        private Label hSeparator4;
        private Label lblInactiveFirmware;
        private Label hSeparator3;
        private Label lblInactiveNetwork;
        private Label vSeparator4;
        private Label vSeparator3;
        private Label vSeparator2;
        private Label hSeparator5;
        private Label label5;
        private Label hSeparator1;
        private Label lblFixedProgressBarPercent;
        private Label vSeparator5;
        private DataGridViewTextBoxColumn agentUsername;

        #endregion

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
        internal MainForm(Octokit.GitHubClient ghc, bool offlineMode, Agent agent, string serverIP, string serverPort, LogGenerator log, List<string[]> parametersList, List<string> enforcementList, List<string> orgDataList, bool isSystemDarkModeEnabled)
        {
            //Inits WinForms components
            InitializeComponent();
            //Creates a new Asset object and subobjects
            newFirmware = new firmware();
            newProcessor = new List<processor>();
            newRam = new List<ram>();
            newStorage = new List<storage>();
            newVideoCard = new List<videoCard>();
            newLocation = new location();
            newMaintenances = new List<maintenances>();
            newNetwork = new network();
            newOperatingSystem = new operatingSystem();
            newHardware = new hardware()
            {
                processor = newProcessor,
                ram = newRam,
                storage = newStorage,
                videoCard = newVideoCard
            };
            newAsset = new Asset()
            {
                firmware = newFirmware,
                hardware = newHardware,
                location = newLocation,
                maintenances = newMaintenances,
                network = newNetwork,
                operatingSystem = newOperatingSystem
            };

            //Define theming according to ini file provided info
            (int themeFileSet, bool themeEditable) = Misc.MiscMethods.GetFileThemeMode(parametersList, isSystemDarkModeEnabled);
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
            toolStripVersionText.Text = Misc.MiscMethods.Version(Resources.DEV_STATUS); //Debug/Beta version
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
            this.agent = agent;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblBrand = new System.Windows.Forms.Label();
            this.lblModel = new System.Windows.Forms.Label();
            this.lblSerialNumber = new System.Windows.Forms.Label();
            this.lblProcessor = new System.Windows.Forms.Label();
            this.lblRam = new System.Windows.Forms.Label();
            this.lblOperatingSystem = new System.Windows.Forms.Label();
            this.lblHostname = new System.Windows.Forms.Label();
            this.lblIpAddress = new System.Windows.Forms.Label();
            this.lblFixedBrand = new System.Windows.Forms.Label();
            this.lblFixedModel = new System.Windows.Forms.Label();
            this.lblFixedSerialNumber = new System.Windows.Forms.Label();
            this.lblFixedProcessor = new System.Windows.Forms.Label();
            this.lblFixedRam = new System.Windows.Forms.Label();
            this.lblFixedOperatingSystem = new System.Windows.Forms.Label();
            this.lblFixedHostname = new System.Windows.Forms.Label();
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
            this.apcsButton = new System.Windows.Forms.Button();
            this.lblFixedFwType = new System.Windows.Forms.Label();
            this.lblFwType = new System.Windows.Forms.Label();
            this.groupBoxHwData = new System.Windows.Forms.GroupBox();
            this.vSeparator5 = new System.Windows.Forms.Label();
            this.vSeparator4 = new System.Windows.Forms.Label();
            this.vSeparator3 = new System.Windows.Forms.Label();
            this.vSeparator2 = new System.Windows.Forms.Label();
            this.vSeparator1 = new System.Windows.Forms.Label();
            this.hSeparator5 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.hSeparator1 = new System.Windows.Forms.Label();
            this.hSeparator4 = new System.Windows.Forms.Label();
            this.lblInactiveFirmware = new System.Windows.Forms.Label();
            this.hSeparator3 = new System.Windows.Forms.Label();
            this.lblInactiveNetwork = new System.Windows.Forms.Label();
            this.hSeparator2 = new System.Windows.Forms.Label();
            this.lblInactiveHardware = new System.Windows.Forms.Label();
            this.lblFixedProgressBarPercent = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.processorDetailsButton = new System.Windows.Forms.Button();
            this.ramDetailsButton = new System.Windows.Forms.Button();
            this.videoCardDetailsButton = new System.Windows.Forms.Button();
            this.loadingCircleCompliant = new MRG.Controls.UI.LoadingCircle();
            this.lblColorCompliant = new System.Windows.Forms.Label();
            this.storageDetailsButton = new System.Windows.Forms.Button();
            this.loadingCircleScanTpmVersion = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanVirtualizationTechnology = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanSecureBoot = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanFwVersion = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanFwType = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanIpAddress = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanHostname = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanOperatingSystem = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanVideoCard = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanMediaOperationMode = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanStorageType = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanRam = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanProcessor = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanSerialNumber = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanModel = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleScanBrand = new MRG.Controls.UI.LoadingCircle();
            this.iconImgTpmVersion = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblTpmVersion = new System.Windows.Forms.Label();
            this.iconImgVirtualizationTechnology = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblFixedTpmVersion = new System.Windows.Forms.Label();
            this.lblVirtualizationTechnology = new System.Windows.Forms.Label();
            this.lblFixedVirtualizationTechnology = new System.Windows.Forms.Label();
            this.iconImgBrand = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgSecureBoot = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgFwVersion = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgFwType = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgIpAddress = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgHostname = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgOperatingSystem = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgVideoCard = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgMediaOperationMode = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgStorageType = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
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
            this.comboBoxBatteryChange = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            this.comboBoxStandard = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            this.comboBoxActiveDirectory = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            this.comboBoxTag = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            this.comboBoxInUse = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            this.comboBoxHwType = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            this.comboBoxBuilding = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
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
            this.lblFixedAdRegistered = new System.Windows.Forms.Label();
            this.lblFixedStandard = new System.Windows.Forms.Label();
            this.groupBoxServiceType = new System.Windows.Forms.GroupBox();
            this.textBoxInactiveUpdateDataRadio = new System.Windows.Forms.TextBox();
            this.radioButtonUpdateData = new System.Windows.Forms.RadioButton();
            this.lblFixedMandatoryServiceType = new System.Windows.Forms.Label();
            this.textBoxInactiveFormattingRadio = new System.Windows.Forms.TextBox();
            this.textBoxInactiveMaintenanceRadio = new System.Windows.Forms.TextBox();
            this.radioButtonFormatting = new System.Windows.Forms.RadioButton();
            this.radioButtonMaintenance = new System.Windows.Forms.RadioButton();
            this.loadingCircleLastService = new MRG.Controls.UI.LoadingCircle();
            this.lblColorLastService = new System.Windows.Forms.Label();
            this.lblAgentName = new System.Windows.Forms.Label();
            this.lblFixedAgentName = new System.Windows.Forms.Label();
            this.lblServerPort = new System.Windows.Forms.Label();
            this.lblServerIP = new System.Windows.Forms.Label();
            this.lblFixedServerIP = new System.Windows.Forms.Label();
            this.lblColorServerOperationalStatus = new System.Windows.Forms.Label();
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
            this.timerAlertTpmVersion = new System.Windows.Forms.Timer(this.components);
            this.timerAlertRamAmount = new System.Windows.Forms.Timer(this.components);
            this.imgTopBanner = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.loadingCircleCollectButton = new MRG.Controls.UI.LoadingCircle();
            this.loadingCircleRegisterButton = new MRG.Controls.UI.LoadingCircle();
            this.groupBoxServerStatus = new System.Windows.Forms.GroupBox();
            this.loadingCircleServerOperationalStatus = new MRG.Controls.UI.LoadingCircle();
            this.timerOSLabelScroll = new System.Windows.Forms.Timer(this.components);
            this.timerFwVersionLabelScroll = new System.Windows.Forms.Timer(this.components);
            this.timerVideoCardLabelScroll = new System.Windows.Forms.Timer(this.components);
            this.timerRamLabelScroll = new System.Windows.Forms.Timer(this.components);
            this.timerProcessorLabelScroll = new System.Windows.Forms.Timer(this.components);
            this.groupBoxTableMaintenances = new System.Windows.Forms.GroupBox();
            this.loadingCircleTableMaintenances = new MRG.Controls.UI.LoadingCircle();
            this.tableMaintenances = new System.Windows.Forms.DataGridView();
            this.serviceDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.serviceType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.agentUsername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblThereIsNothingHere = new System.Windows.Forms.Label();
            this.groupBoxHwData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgTpmVersion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgVirtualizationTechnology)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgBrand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgSecureBoot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgFwVersion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgFwType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgIpAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgHostname)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgOperatingSystem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgVideoCard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgMediaOperationMode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgStorageType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgRam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgProcessor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgSerialNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgModel)).BeginInit();
            this.groupBoxAssetData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgTicketNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgBatteryChange)).BeginInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.imgTopBanner)).BeginInit();
            this.groupBoxServerStatus.SuspendLayout();
            this.groupBoxTableMaintenances.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tableMaintenances)).BeginInit();
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
            this.textBoxRoomLetter.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
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
            // apcsButton
            // 
            resources.ApplyResources(this.apcsButton, "apcsButton");
            this.apcsButton.Name = "apcsButton";
            this.apcsButton.UseVisualStyleBackColor = true;
            this.apcsButton.Click += new System.EventHandler(this.ApcsButton_Click);
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
            this.groupBoxHwData.Controls.Add(this.vSeparator5);
            this.groupBoxHwData.Controls.Add(this.vSeparator4);
            this.groupBoxHwData.Controls.Add(this.vSeparator3);
            this.groupBoxHwData.Controls.Add(this.vSeparator2);
            this.groupBoxHwData.Controls.Add(this.vSeparator1);
            this.groupBoxHwData.Controls.Add(this.hSeparator5);
            this.groupBoxHwData.Controls.Add(this.label5);
            this.groupBoxHwData.Controls.Add(this.hSeparator1);
            this.groupBoxHwData.Controls.Add(this.hSeparator4);
            this.groupBoxHwData.Controls.Add(this.lblInactiveFirmware);
            this.groupBoxHwData.Controls.Add(this.hSeparator3);
            this.groupBoxHwData.Controls.Add(this.lblInactiveNetwork);
            this.groupBoxHwData.Controls.Add(this.hSeparator2);
            this.groupBoxHwData.Controls.Add(this.lblInactiveHardware);
            this.groupBoxHwData.Controls.Add(this.lblFixedProgressBarPercent);
            this.groupBoxHwData.Controls.Add(this.progressBar1);
            this.groupBoxHwData.Controls.Add(this.processorDetailsButton);
            this.groupBoxHwData.Controls.Add(this.ramDetailsButton);
            this.groupBoxHwData.Controls.Add(this.videoCardDetailsButton);
            this.groupBoxHwData.Controls.Add(this.loadingCircleCompliant);
            this.groupBoxHwData.Controls.Add(this.lblColorCompliant);
            this.groupBoxHwData.Controls.Add(this.storageDetailsButton);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanTpmVersion);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanVirtualizationTechnology);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanSecureBoot);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanFwVersion);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanFwType);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanIpAddress);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanHostname);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanOperatingSystem);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanVideoCard);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanMediaOperationMode);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanStorageType);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanRam);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanProcessor);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanSerialNumber);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanModel);
            this.groupBoxHwData.Controls.Add(this.loadingCircleScanBrand);
            this.groupBoxHwData.Controls.Add(this.iconImgTpmVersion);
            this.groupBoxHwData.Controls.Add(this.lblTpmVersion);
            this.groupBoxHwData.Controls.Add(this.iconImgVirtualizationTechnology);
            this.groupBoxHwData.Controls.Add(this.lblFixedTpmVersion);
            this.groupBoxHwData.Controls.Add(this.lblVirtualizationTechnology);
            this.groupBoxHwData.Controls.Add(this.lblFixedVirtualizationTechnology);
            this.groupBoxHwData.Controls.Add(this.iconImgBrand);
            this.groupBoxHwData.Controls.Add(this.iconImgSecureBoot);
            this.groupBoxHwData.Controls.Add(this.iconImgFwVersion);
            this.groupBoxHwData.Controls.Add(this.iconImgFwType);
            this.groupBoxHwData.Controls.Add(this.iconImgIpAddress);
            this.groupBoxHwData.Controls.Add(this.iconImgHostname);
            this.groupBoxHwData.Controls.Add(this.iconImgOperatingSystem);
            this.groupBoxHwData.Controls.Add(this.iconImgVideoCard);
            this.groupBoxHwData.Controls.Add(this.iconImgMediaOperationMode);
            this.groupBoxHwData.Controls.Add(this.iconImgStorageType);
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
            this.groupBoxHwData.Controls.Add(this.lblFwType);
            this.groupBoxHwData.Controls.Add(this.lblFixedFwType);
            this.groupBoxHwData.Controls.Add(this.lblRam);
            this.groupBoxHwData.Controls.Add(this.lblProcessor);
            this.groupBoxHwData.Controls.Add(this.lblSerialNumber);
            this.groupBoxHwData.Controls.Add(this.lblFwVersion);
            this.groupBoxHwData.Controls.Add(this.lblModel);
            this.groupBoxHwData.Controls.Add(this.lblFixedFwVersion);
            this.groupBoxHwData.Controls.Add(this.lblBrand);
            this.groupBoxHwData.Controls.Add(this.lblHostname);
            this.groupBoxHwData.Controls.Add(this.lblIpAddress);
            this.groupBoxHwData.Controls.Add(this.lblFixedModel);
            this.groupBoxHwData.Controls.Add(this.lblFixedSerialNumber);
            this.groupBoxHwData.Controls.Add(this.lblFixedProcessor);
            this.groupBoxHwData.Controls.Add(this.lblFixedRam);
            this.groupBoxHwData.Controls.Add(this.lblFixedOperatingSystem);
            this.groupBoxHwData.Controls.Add(this.lblFixedHostname);
            this.groupBoxHwData.Controls.Add(this.lblFixedIpAddress);
            this.groupBoxHwData.Controls.Add(this.lblOperatingSystem);
            this.groupBoxHwData.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBoxHwData.Name = "groupBoxHwData";
            this.groupBoxHwData.TabStop = false;
            // 
            // vSeparator5
            // 
            resources.ApplyResources(this.vSeparator5, "vSeparator5");
            this.vSeparator5.BackColor = System.Drawing.Color.DimGray;
            this.vSeparator5.Name = "vSeparator5";
            // 
            // vSeparator4
            // 
            resources.ApplyResources(this.vSeparator4, "vSeparator4");
            this.vSeparator4.BackColor = System.Drawing.Color.DimGray;
            this.vSeparator4.Name = "vSeparator4";
            // 
            // vSeparator3
            // 
            resources.ApplyResources(this.vSeparator3, "vSeparator3");
            this.vSeparator3.BackColor = System.Drawing.Color.DimGray;
            this.vSeparator3.Name = "vSeparator3";
            // 
            // vSeparator2
            // 
            resources.ApplyResources(this.vSeparator2, "vSeparator2");
            this.vSeparator2.BackColor = System.Drawing.Color.DimGray;
            this.vSeparator2.Name = "vSeparator2";
            // 
            // vSeparator1
            // 
            resources.ApplyResources(this.vSeparator1, "vSeparator1");
            this.vSeparator1.BackColor = System.Drawing.Color.DimGray;
            this.vSeparator1.Name = "vSeparator1";
            // 
            // hSeparator5
            // 
            resources.ApplyResources(this.hSeparator5, "hSeparator5");
            this.hSeparator5.BackColor = System.Drawing.Color.DimGray;
            this.hSeparator5.Name = "hSeparator5";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Name = "label5";
            // 
            // hSeparator1
            // 
            resources.ApplyResources(this.hSeparator1, "hSeparator1");
            this.hSeparator1.BackColor = System.Drawing.Color.DimGray;
            this.hSeparator1.Name = "hSeparator1";
            // 
            // hSeparator4
            // 
            resources.ApplyResources(this.hSeparator4, "hSeparator4");
            this.hSeparator4.BackColor = System.Drawing.Color.DimGray;
            this.hSeparator4.Name = "hSeparator4";
            // 
            // lblInactiveFirmware
            // 
            resources.ApplyResources(this.lblInactiveFirmware, "lblInactiveFirmware");
            this.lblInactiveFirmware.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInactiveFirmware.Name = "lblInactiveFirmware";
            // 
            // hSeparator3
            // 
            resources.ApplyResources(this.hSeparator3, "hSeparator3");
            this.hSeparator3.BackColor = System.Drawing.Color.DimGray;
            this.hSeparator3.Name = "hSeparator3";
            // 
            // lblInactiveNetwork
            // 
            resources.ApplyResources(this.lblInactiveNetwork, "lblInactiveNetwork");
            this.lblInactiveNetwork.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInactiveNetwork.Name = "lblInactiveNetwork";
            // 
            // hSeparator2
            // 
            resources.ApplyResources(this.hSeparator2, "hSeparator2");
            this.hSeparator2.BackColor = System.Drawing.Color.DimGray;
            this.hSeparator2.Name = "hSeparator2";
            // 
            // lblInactiveHardware
            // 
            resources.ApplyResources(this.lblInactiveHardware, "lblInactiveHardware");
            this.lblInactiveHardware.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInactiveHardware.Name = "lblInactiveHardware";
            // 
            // lblFixedProgressBarPercent
            // 
            resources.ApplyResources(this.lblFixedProgressBarPercent, "lblFixedProgressBarPercent");
            this.lblFixedProgressBarPercent.BackColor = System.Drawing.Color.Transparent;
            this.lblFixedProgressBarPercent.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedProgressBarPercent.Name = "lblFixedProgressBarPercent";
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // processorDetailsButton
            // 
            resources.ApplyResources(this.processorDetailsButton, "processorDetailsButton");
            this.processorDetailsButton.Name = "processorDetailsButton";
            this.processorDetailsButton.UseVisualStyleBackColor = true;
            this.processorDetailsButton.Click += new System.EventHandler(this.processorDetailsButton_Click);
            // 
            // ramDetailsButton
            // 
            resources.ApplyResources(this.ramDetailsButton, "ramDetailsButton");
            this.ramDetailsButton.Name = "ramDetailsButton";
            this.ramDetailsButton.UseVisualStyleBackColor = true;
            this.ramDetailsButton.Click += new System.EventHandler(this.ramDetailsButton_Click);
            // 
            // videoCardDetailsButton
            // 
            resources.ApplyResources(this.videoCardDetailsButton, "videoCardDetailsButton");
            this.videoCardDetailsButton.Name = "videoCardDetailsButton";
            this.videoCardDetailsButton.UseVisualStyleBackColor = true;
            this.videoCardDetailsButton.Click += new System.EventHandler(this.videoCardDetailsButton_Click);
            // 
            // loadingCircleCompliant
            // 
            resources.ApplyResources(this.loadingCircleCompliant, "loadingCircleCompliant");
            this.loadingCircleCompliant.Active = false;
            this.loadingCircleCompliant.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleCompliant.InnerCircleRadius = 5;
            this.loadingCircleCompliant.Name = "loadingCircleCompliant";
            this.loadingCircleCompliant.NumberSpoke = 12;
            this.loadingCircleCompliant.OuterCircleRadius = 11;
            this.loadingCircleCompliant.RotationSpeed = 1;
            this.loadingCircleCompliant.SpokeThickness = 2;
            this.loadingCircleCompliant.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // lblColorCompliant
            // 
            resources.ApplyResources(this.lblColorCompliant, "lblColorCompliant");
            this.lblColorCompliant.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblColorCompliant.Name = "lblColorCompliant";
            // 
            // storageDetailsButton
            // 
            resources.ApplyResources(this.storageDetailsButton, "storageDetailsButton");
            this.storageDetailsButton.Name = "storageDetailsButton";
            this.storageDetailsButton.UseVisualStyleBackColor = true;
            this.storageDetailsButton.Click += new System.EventHandler(this.StorageDetailsButton_Click);
            // 
            // loadingCircleScanTpmVersion
            // 
            resources.ApplyResources(this.loadingCircleScanTpmVersion, "loadingCircleScanTpmVersion");
            this.loadingCircleScanTpmVersion.Active = false;
            this.loadingCircleScanTpmVersion.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanTpmVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanTpmVersion.InnerCircleRadius = 5;
            this.loadingCircleScanTpmVersion.Name = "loadingCircleScanTpmVersion";
            this.loadingCircleScanTpmVersion.NumberSpoke = 12;
            this.loadingCircleScanTpmVersion.OuterCircleRadius = 11;
            this.loadingCircleScanTpmVersion.RotationSpeed = 1;
            this.loadingCircleScanTpmVersion.SpokeThickness = 2;
            this.loadingCircleScanTpmVersion.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanVirtualizationTechnology
            // 
            resources.ApplyResources(this.loadingCircleScanVirtualizationTechnology, "loadingCircleScanVirtualizationTechnology");
            this.loadingCircleScanVirtualizationTechnology.Active = false;
            this.loadingCircleScanVirtualizationTechnology.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanVirtualizationTechnology.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanVirtualizationTechnology.InnerCircleRadius = 5;
            this.loadingCircleScanVirtualizationTechnology.Name = "loadingCircleScanVirtualizationTechnology";
            this.loadingCircleScanVirtualizationTechnology.NumberSpoke = 12;
            this.loadingCircleScanVirtualizationTechnology.OuterCircleRadius = 11;
            this.loadingCircleScanVirtualizationTechnology.RotationSpeed = 1;
            this.loadingCircleScanVirtualizationTechnology.SpokeThickness = 2;
            this.loadingCircleScanVirtualizationTechnology.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanSecureBoot
            // 
            resources.ApplyResources(this.loadingCircleScanSecureBoot, "loadingCircleScanSecureBoot");
            this.loadingCircleScanSecureBoot.Active = false;
            this.loadingCircleScanSecureBoot.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanSecureBoot.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanSecureBoot.InnerCircleRadius = 5;
            this.loadingCircleScanSecureBoot.Name = "loadingCircleScanSecureBoot";
            this.loadingCircleScanSecureBoot.NumberSpoke = 12;
            this.loadingCircleScanSecureBoot.OuterCircleRadius = 11;
            this.loadingCircleScanSecureBoot.RotationSpeed = 1;
            this.loadingCircleScanSecureBoot.SpokeThickness = 2;
            this.loadingCircleScanSecureBoot.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanFwVersion
            // 
            resources.ApplyResources(this.loadingCircleScanFwVersion, "loadingCircleScanFwVersion");
            this.loadingCircleScanFwVersion.Active = false;
            this.loadingCircleScanFwVersion.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanFwVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanFwVersion.InnerCircleRadius = 5;
            this.loadingCircleScanFwVersion.Name = "loadingCircleScanFwVersion";
            this.loadingCircleScanFwVersion.NumberSpoke = 12;
            this.loadingCircleScanFwVersion.OuterCircleRadius = 11;
            this.loadingCircleScanFwVersion.RotationSpeed = 1;
            this.loadingCircleScanFwVersion.SpokeThickness = 2;
            this.loadingCircleScanFwVersion.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanFwType
            // 
            resources.ApplyResources(this.loadingCircleScanFwType, "loadingCircleScanFwType");
            this.loadingCircleScanFwType.Active = false;
            this.loadingCircleScanFwType.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanFwType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanFwType.InnerCircleRadius = 5;
            this.loadingCircleScanFwType.Name = "loadingCircleScanFwType";
            this.loadingCircleScanFwType.NumberSpoke = 12;
            this.loadingCircleScanFwType.OuterCircleRadius = 11;
            this.loadingCircleScanFwType.RotationSpeed = 1;
            this.loadingCircleScanFwType.SpokeThickness = 2;
            this.loadingCircleScanFwType.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanIpAddress
            // 
            resources.ApplyResources(this.loadingCircleScanIpAddress, "loadingCircleScanIpAddress");
            this.loadingCircleScanIpAddress.Active = false;
            this.loadingCircleScanIpAddress.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanIpAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanIpAddress.InnerCircleRadius = 5;
            this.loadingCircleScanIpAddress.Name = "loadingCircleScanIpAddress";
            this.loadingCircleScanIpAddress.NumberSpoke = 12;
            this.loadingCircleScanIpAddress.OuterCircleRadius = 11;
            this.loadingCircleScanIpAddress.RotationSpeed = 1;
            this.loadingCircleScanIpAddress.SpokeThickness = 2;
            this.loadingCircleScanIpAddress.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanHostname
            // 
            resources.ApplyResources(this.loadingCircleScanHostname, "loadingCircleScanHostname");
            this.loadingCircleScanHostname.Active = false;
            this.loadingCircleScanHostname.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanHostname.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanHostname.InnerCircleRadius = 5;
            this.loadingCircleScanHostname.Name = "loadingCircleScanHostname";
            this.loadingCircleScanHostname.NumberSpoke = 12;
            this.loadingCircleScanHostname.OuterCircleRadius = 11;
            this.loadingCircleScanHostname.RotationSpeed = 1;
            this.loadingCircleScanHostname.SpokeThickness = 2;
            this.loadingCircleScanHostname.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanOperatingSystem
            // 
            resources.ApplyResources(this.loadingCircleScanOperatingSystem, "loadingCircleScanOperatingSystem");
            this.loadingCircleScanOperatingSystem.Active = false;
            this.loadingCircleScanOperatingSystem.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanOperatingSystem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanOperatingSystem.InnerCircleRadius = 5;
            this.loadingCircleScanOperatingSystem.Name = "loadingCircleScanOperatingSystem";
            this.loadingCircleScanOperatingSystem.NumberSpoke = 12;
            this.loadingCircleScanOperatingSystem.OuterCircleRadius = 11;
            this.loadingCircleScanOperatingSystem.RotationSpeed = 1;
            this.loadingCircleScanOperatingSystem.SpokeThickness = 2;
            this.loadingCircleScanOperatingSystem.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanVideoCard
            // 
            resources.ApplyResources(this.loadingCircleScanVideoCard, "loadingCircleScanVideoCard");
            this.loadingCircleScanVideoCard.Active = false;
            this.loadingCircleScanVideoCard.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanVideoCard.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanVideoCard.InnerCircleRadius = 5;
            this.loadingCircleScanVideoCard.Name = "loadingCircleScanVideoCard";
            this.loadingCircleScanVideoCard.NumberSpoke = 12;
            this.loadingCircleScanVideoCard.OuterCircleRadius = 11;
            this.loadingCircleScanVideoCard.RotationSpeed = 1;
            this.loadingCircleScanVideoCard.SpokeThickness = 2;
            this.loadingCircleScanVideoCard.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanMediaOperationMode
            // 
            resources.ApplyResources(this.loadingCircleScanMediaOperationMode, "loadingCircleScanMediaOperationMode");
            this.loadingCircleScanMediaOperationMode.Active = false;
            this.loadingCircleScanMediaOperationMode.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanMediaOperationMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanMediaOperationMode.InnerCircleRadius = 5;
            this.loadingCircleScanMediaOperationMode.Name = "loadingCircleScanMediaOperationMode";
            this.loadingCircleScanMediaOperationMode.NumberSpoke = 12;
            this.loadingCircleScanMediaOperationMode.OuterCircleRadius = 11;
            this.loadingCircleScanMediaOperationMode.RotationSpeed = 1;
            this.loadingCircleScanMediaOperationMode.SpokeThickness = 2;
            this.loadingCircleScanMediaOperationMode.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanStorageType
            // 
            resources.ApplyResources(this.loadingCircleScanStorageType, "loadingCircleScanStorageType");
            this.loadingCircleScanStorageType.Active = false;
            this.loadingCircleScanStorageType.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanStorageType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanStorageType.InnerCircleRadius = 5;
            this.loadingCircleScanStorageType.Name = "loadingCircleScanStorageType";
            this.loadingCircleScanStorageType.NumberSpoke = 12;
            this.loadingCircleScanStorageType.OuterCircleRadius = 11;
            this.loadingCircleScanStorageType.RotationSpeed = 1;
            this.loadingCircleScanStorageType.SpokeThickness = 2;
            this.loadingCircleScanStorageType.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanRam
            // 
            resources.ApplyResources(this.loadingCircleScanRam, "loadingCircleScanRam");
            this.loadingCircleScanRam.Active = false;
            this.loadingCircleScanRam.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanRam.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanRam.InnerCircleRadius = 5;
            this.loadingCircleScanRam.Name = "loadingCircleScanRam";
            this.loadingCircleScanRam.NumberSpoke = 12;
            this.loadingCircleScanRam.OuterCircleRadius = 11;
            this.loadingCircleScanRam.RotationSpeed = 1;
            this.loadingCircleScanRam.SpokeThickness = 2;
            this.loadingCircleScanRam.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanProcessor
            // 
            resources.ApplyResources(this.loadingCircleScanProcessor, "loadingCircleScanProcessor");
            this.loadingCircleScanProcessor.Active = false;
            this.loadingCircleScanProcessor.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanProcessor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanProcessor.InnerCircleRadius = 5;
            this.loadingCircleScanProcessor.Name = "loadingCircleScanProcessor";
            this.loadingCircleScanProcessor.NumberSpoke = 12;
            this.loadingCircleScanProcessor.OuterCircleRadius = 11;
            this.loadingCircleScanProcessor.RotationSpeed = 1;
            this.loadingCircleScanProcessor.SpokeThickness = 2;
            this.loadingCircleScanProcessor.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanSerialNumber
            // 
            resources.ApplyResources(this.loadingCircleScanSerialNumber, "loadingCircleScanSerialNumber");
            this.loadingCircleScanSerialNumber.Active = false;
            this.loadingCircleScanSerialNumber.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanSerialNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanSerialNumber.InnerCircleRadius = 5;
            this.loadingCircleScanSerialNumber.Name = "loadingCircleScanSerialNumber";
            this.loadingCircleScanSerialNumber.NumberSpoke = 12;
            this.loadingCircleScanSerialNumber.OuterCircleRadius = 11;
            this.loadingCircleScanSerialNumber.RotationSpeed = 1;
            this.loadingCircleScanSerialNumber.SpokeThickness = 2;
            this.loadingCircleScanSerialNumber.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanModel
            // 
            resources.ApplyResources(this.loadingCircleScanModel, "loadingCircleScanModel");
            this.loadingCircleScanModel.Active = false;
            this.loadingCircleScanModel.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanModel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanModel.InnerCircleRadius = 5;
            this.loadingCircleScanModel.Name = "loadingCircleScanModel";
            this.loadingCircleScanModel.NumberSpoke = 12;
            this.loadingCircleScanModel.OuterCircleRadius = 11;
            this.loadingCircleScanModel.RotationSpeed = 1;
            this.loadingCircleScanModel.SpokeThickness = 2;
            this.loadingCircleScanModel.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanBrand
            // 
            resources.ApplyResources(this.loadingCircleScanBrand, "loadingCircleScanBrand");
            this.loadingCircleScanBrand.Active = false;
            this.loadingCircleScanBrand.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanBrand.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanBrand.InnerCircleRadius = 5;
            this.loadingCircleScanBrand.Name = "loadingCircleScanBrand";
            this.loadingCircleScanBrand.NumberSpoke = 12;
            this.loadingCircleScanBrand.OuterCircleRadius = 11;
            this.loadingCircleScanBrand.RotationSpeed = 1;
            this.loadingCircleScanBrand.SpokeThickness = 2;
            this.loadingCircleScanBrand.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
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
            // lblTpmVersion
            // 
            resources.ApplyResources(this.lblTpmVersion, "lblTpmVersion");
            this.lblTpmVersion.ForeColor = System.Drawing.Color.Silver;
            this.lblTpmVersion.Name = "lblTpmVersion";
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
            // groupBoxServiceType
            // 
            resources.ApplyResources(this.groupBoxServiceType, "groupBoxServiceType");
            this.groupBoxServiceType.Controls.Add(this.textBoxInactiveUpdateDataRadio);
            this.groupBoxServiceType.Controls.Add(this.radioButtonUpdateData);
            this.groupBoxServiceType.Controls.Add(this.lblFixedMandatoryServiceType);
            this.groupBoxServiceType.Controls.Add(this.textBoxInactiveFormattingRadio);
            this.groupBoxServiceType.Controls.Add(this.textBoxInactiveMaintenanceRadio);
            this.groupBoxServiceType.Controls.Add(this.radioButtonFormatting);
            this.groupBoxServiceType.Controls.Add(this.radioButtonMaintenance);
            this.groupBoxServiceType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBoxServiceType.Name = "groupBoxServiceType";
            this.groupBoxServiceType.TabStop = false;
            // 
            // textBoxInactiveUpdateDataRadio
            // 
            resources.ApplyResources(this.textBoxInactiveUpdateDataRadio, "textBoxInactiveUpdateDataRadio");
            this.textBoxInactiveUpdateDataRadio.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxInactiveUpdateDataRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxInactiveUpdateDataRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxInactiveUpdateDataRadio.Name = "textBoxInactiveUpdateDataRadio";
            this.textBoxInactiveUpdateDataRadio.ReadOnly = true;
            // 
            // radioButtonUpdateData
            // 
            resources.ApplyResources(this.radioButtonUpdateData, "radioButtonUpdateData");
            this.radioButtonUpdateData.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radioButtonUpdateData.Name = "radioButtonUpdateData";
            this.radioButtonUpdateData.UseVisualStyleBackColor = true;
            // 
            // lblFixedMandatoryServiceType
            // 
            resources.ApplyResources(this.lblFixedMandatoryServiceType, "lblFixedMandatoryServiceType");
            this.lblFixedMandatoryServiceType.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryServiceType.Name = "lblFixedMandatoryServiceType";
            // 
            // textBoxInactiveFormattingRadio
            // 
            resources.ApplyResources(this.textBoxInactiveFormattingRadio, "textBoxInactiveFormattingRadio");
            this.textBoxInactiveFormattingRadio.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxInactiveFormattingRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxInactiveFormattingRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxInactiveFormattingRadio.Name = "textBoxInactiveFormattingRadio";
            this.textBoxInactiveFormattingRadio.ReadOnly = true;
            // 
            // textBoxInactiveMaintenanceRadio
            // 
            resources.ApplyResources(this.textBoxInactiveMaintenanceRadio, "textBoxInactiveMaintenanceRadio");
            this.textBoxInactiveMaintenanceRadio.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxInactiveMaintenanceRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxInactiveMaintenanceRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxInactiveMaintenanceRadio.Name = "textBoxInactiveMaintenanceRadio";
            this.textBoxInactiveMaintenanceRadio.ReadOnly = true;
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
            // loadingCircleLastService
            // 
            resources.ApplyResources(this.loadingCircleLastService, "loadingCircleLastService");
            this.loadingCircleLastService.Active = false;
            this.loadingCircleLastService.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleLastService.InnerCircleRadius = 5;
            this.loadingCircleLastService.Name = "loadingCircleLastService";
            this.loadingCircleLastService.NumberSpoke = 12;
            this.loadingCircleLastService.OuterCircleRadius = 11;
            this.loadingCircleLastService.RotationSpeed = 1;
            this.loadingCircleLastService.SpokeThickness = 2;
            this.loadingCircleLastService.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // lblColorLastService
            // 
            resources.ApplyResources(this.lblColorLastService, "lblColorLastService");
            this.lblColorLastService.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblColorLastService.Name = "lblColorLastService";
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
            // lblColorServerOperationalStatus
            // 
            resources.ApplyResources(this.lblColorServerOperationalStatus, "lblColorServerOperationalStatus");
            this.lblColorServerOperationalStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblColorServerOperationalStatus.ForeColor = System.Drawing.Color.Silver;
            this.lblColorServerOperationalStatus.Name = "lblColorServerOperationalStatus";
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
            this.loadingCircleCollectButton.BackColor = System.Drawing.Color.Transparent;
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
            this.loadingCircleRegisterButton.BackColor = System.Drawing.Color.Transparent;
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
            this.groupBoxServerStatus.Controls.Add(this.lblColorServerOperationalStatus);
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
            // timerOSLabelScroll
            // 
            this.timerOSLabelScroll.Tick += new System.EventHandler(this.TimerOSLabelScroll_Tick);
            // 
            // timerFwVersionLabelScroll
            // 
            this.timerFwVersionLabelScroll.Tick += new System.EventHandler(this.TimerFwVersionLabelScroll_Tick);
            // 
            // timerVideoCardLabelScroll
            // 
            this.timerVideoCardLabelScroll.Tick += new System.EventHandler(this.TimerVideoCardLabelScroll_Tick);
            // 
            // timerRamLabelScroll
            // 
            this.timerRamLabelScroll.Tick += new System.EventHandler(this.TimerRamLabelScroll_Tick);
            // 
            // timerProcessorLabelScroll
            // 
            this.timerProcessorLabelScroll.Tick += new System.EventHandler(this.TimerProcessorLabelScroll_Tick);
            // 
            // groupBoxTableMaintenances
            // 
            resources.ApplyResources(this.groupBoxTableMaintenances, "groupBoxTableMaintenances");
            this.groupBoxTableMaintenances.Controls.Add(this.loadingCircleTableMaintenances);
            this.groupBoxTableMaintenances.Controls.Add(this.tableMaintenances);
            this.groupBoxTableMaintenances.Controls.Add(this.lblThereIsNothingHere);
            this.groupBoxTableMaintenances.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBoxTableMaintenances.Name = "groupBoxTableMaintenances";
            this.groupBoxTableMaintenances.TabStop = false;
            // 
            // loadingCircleTableMaintenances
            // 
            resources.ApplyResources(this.loadingCircleTableMaintenances, "loadingCircleTableMaintenances");
            this.loadingCircleTableMaintenances.Active = false;
            this.loadingCircleTableMaintenances.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleTableMaintenances.InnerCircleRadius = 5;
            this.loadingCircleTableMaintenances.Name = "loadingCircleTableMaintenances";
            this.loadingCircleTableMaintenances.NumberSpoke = 12;
            this.loadingCircleTableMaintenances.OuterCircleRadius = 11;
            this.loadingCircleTableMaintenances.RotationSpeed = 1;
            this.loadingCircleTableMaintenances.SpokeThickness = 2;
            this.loadingCircleTableMaintenances.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // tableMaintenances
            // 
            resources.ApplyResources(this.tableMaintenances, "tableMaintenances");
            this.tableMaintenances.AllowUserToAddRows = false;
            this.tableMaintenances.AllowUserToDeleteRows = false;
            this.tableMaintenances.AllowUserToResizeRows = false;
            this.tableMaintenances.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tableMaintenances.BackgroundColor = System.Drawing.SystemColors.Control;
            this.tableMaintenances.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableMaintenances.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.tableMaintenances.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.tableMaintenances.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.tableMaintenances.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.serviceDate,
            this.serviceType,
            this.agentUsername});
            this.tableMaintenances.EnableHeadersVisualStyles = false;
            this.tableMaintenances.Name = "tableMaintenances";
            this.tableMaintenances.ReadOnly = true;
            this.tableMaintenances.RowHeadersVisible = false;
            this.tableMaintenances.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.tableMaintenances.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            // 
            // serviceDate
            // 
            this.serviceDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.serviceDate, "serviceDate");
            this.serviceDate.Name = "serviceDate";
            this.serviceDate.ReadOnly = true;
            // 
            // serviceType
            // 
            this.serviceType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.serviceType, "serviceType");
            this.serviceType.Name = "serviceType";
            this.serviceType.ReadOnly = true;
            // 
            // agentUsername
            // 
            this.agentUsername.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.agentUsername, "agentUsername");
            this.agentUsername.Name = "agentUsername";
            this.agentUsername.ReadOnly = true;
            // 
            // lblThereIsNothingHere
            // 
            resources.ApplyResources(this.lblThereIsNothingHere, "lblThereIsNothingHere");
            this.lblThereIsNothingHere.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.lblThereIsNothingHere.Name = "lblThereIsNothingHere";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.groupBoxTableMaintenances);
            this.Controls.Add(this.loadingCircleRegisterButton);
            this.Controls.Add(this.loadingCircleCollectButton);
            this.Controls.Add(this.loadingCircleLastService);
            this.Controls.Add(this.lblColorLastService);
            this.Controls.Add(this.groupBoxServerStatus);
            this.Controls.Add(this.groupBoxAssetData);
            this.Controls.Add(this.groupBoxHwData);
            this.Controls.Add(this.imgTopBanner);
            this.Controls.Add(this.apcsButton);
            this.Controls.Add(this.collectButton);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.registerButton);
            this.Controls.Add(this.groupBoxServiceType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxHwData.ResumeLayout(false);
            this.groupBoxHwData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgTpmVersion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgVirtualizationTechnology)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgBrand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgSecureBoot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgFwVersion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgFwType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgIpAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgHostname)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgOperatingSystem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgVideoCard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgMediaOperationMode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgStorageType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgRam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgProcessor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgSerialNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgModel)).EndInit();
            this.groupBoxAssetData.ResumeLayout(false);
            this.groupBoxAssetData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgTicketNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgBatteryChange)).EndInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.imgTopBanner)).EndInit();
            this.groupBoxServerStatus.ResumeLayout(false);
            this.groupBoxServerStatus.PerformLayout();
            this.groupBoxTableMaintenances.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tableMaintenances)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /// <summary> 
        /// Sets service mode to format
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormatButton1_CheckedChanged(object sender, EventArgs e)
        {
            //serviceTypeURL = ConstantsDLL.Properties.Resources.FORMAT_URL;
        }

        /// <summary> 
        /// Sets service mode to maintenance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaintenanceButton2_CheckedChanged(object sender, EventArgs e)
        {
            //serviceTypeURL = ConstantsDLL.Properties.Resources.MAINTENANCE_URL;
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
        /// Method for opening the Video Card list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void videoCardDetailsButton_Click(object sender, EventArgs e)
        {
            VideoCardDetailForm videoCardForm = new VideoCardDetailForm(videoCardDetail, parametersList, isSystemDarkModeEnabled);
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetWindowThemeForms(videoCardForm, Theme.Auto);
            _ = videoCardForm.ShowDialog();
        }

        /// <summary> 
        /// Method for opening the RAM list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ramDetailsButton_Click(object sender, EventArgs e)
        {
            RamDetailForm ramForm = new RamDetailForm(ramDetail, parametersList, isSystemDarkModeEnabled);
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetWindowThemeForms(ramForm, Theme.Auto);
            _ = ramForm.ShowDialog();
        }

        /// <summary> 
        /// Method for opening the Processor list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processorDetailsButton_Click(object sender, EventArgs e)
        {
            ProcessorDetailForm processorForm = new ProcessorDetailForm(processorDetail, parametersList, isSystemDarkModeEnabled);
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetWindowThemeForms(processorForm, Theme.Auto);
            _ = processorForm.ShowDialog();
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

        /// <summary> 
        /// Method for auto selecting the app theme
        /// </summary>
        private void ToggleTheme()
        {
            (int themeFileSet, bool _) = Misc.MiscMethods.GetFileThemeMode(parametersList, Misc.MiscMethods.GetSystemThemeMode());
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

            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }

        /// <summary> 
        /// Loads the form, sets some combobox values, create timers (1000 ms cadence), and triggers a hardware collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>Returns a asynchronous task</returns>
        private void MainForm_Load(object sender, EventArgs e)
        {
            #region Define loading circle parameters

            switch (Misc.MiscMethods.GetWindowsScaling())
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
            progressBar1.Maximum = 17;
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

            #region Sets timer settings for respective alerts
            timerAlertHostname.Tick += new EventHandler(AlertFlashTextHostname);
            timerAlertMediaOperationMode.Tick += new EventHandler(AlertFlashTextMediaOperationMode);
            timerAlertSecureBoot.Tick += new EventHandler(AlertFlashTextSecureBoot);
            timerAlertFwVersion.Tick += new EventHandler(AlertFlashTextFirmwareVersion);
            timerAlertNetConnectivity.Tick += new EventHandler(AlertFlashTextNetConnectivity);
            timerAlertFwType.Tick += new EventHandler(AlertFlashTextFirmwareType);
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

            vSeparator1.BringToFront();

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
            {
                //Initializes HTTP client, estabilishing a connection with the remote server
                client = new HttpClient();
                client.BaseAddress = new Uri(ConstantsDLL.Properties.Resources.HTTP + serverIP + ":" + serverPort);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                agentMaintenances = new Agent();
                lblAgentName.Text = agent.name + " " + agent.surname; //Prints agent name
            }

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
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
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
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : lblMediaOperationMode.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
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
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : lblSecureBoot.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
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
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : lblVirtualizationTechnology.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Sets the SMART label to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void AlertFlashTextSmartStatus(object myobject, EventArgs myEventArgs)
        {
            storageDetailsButton.ForeColor = storageDetailsButton.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true
                ? StringsAndConstants.DARK_FORECOLOR
                : storageDetailsButton.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.LIGHT_FORECOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Sets the Firmware Version label to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void AlertFlashTextFirmwareVersion(object myobject, EventArgs myEventArgs)
        {
            lblFwVersion.ForeColor = lblFwVersion.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : lblFwVersion.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Sets the Mac and IP labels to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void AlertFlashTextNetConnectivity(object myobject, EventArgs myEventArgs)
        {
            if (lblIpAddress.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true)
            {
                lblIpAddress.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            }
            else if (lblIpAddress.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false)
            {
                lblIpAddress.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            }
            else
            {

                lblIpAddress.ForeColor = StringsAndConstants.ALERT_COLOR;
            }
        }

        /// <summary> 
        /// Sets the Firmware Type label to flash in red
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void AlertFlashTextFirmwareType(object myobject, EventArgs myEventArgs)
        {
            lblFwType.ForeColor = lblFwType.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == true
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : lblFwType.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
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
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : lblRam.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
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
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : lblTpmVersion.ForeColor == StringsAndConstants.ALERT_COLOR && isSystemDarkModeEnabled == false
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        /// <summary> 
        /// Starts the collection process
        /// </summary>
        /// <returns>Returns a asynchronous task</returns>
        private void Collecting()
        {
            #region Writes a dash in the labels, while scanning the hardware
            lblColorLastService.Text = ConstantsDLL.Properties.Resources.DASH;
            lblBrand.Text = ConstantsDLL.Properties.Resources.DASH;
            lblModel.Text = ConstantsDLL.Properties.Resources.DASH;
            lblSerialNumber.Text = ConstantsDLL.Properties.Resources.DASH;
            lblProcessor.Text = ConstantsDLL.Properties.Resources.DASH;
            lblRam.Text = ConstantsDLL.Properties.Resources.DASH;
            lblColorCompliant.Text = ConstantsDLL.Properties.Resources.DASH;
            lblStorageType.Text = ConstantsDLL.Properties.Resources.DASH;
            lblMediaOperationMode.Text = ConstantsDLL.Properties.Resources.DASH;
            lblVideoCard.Text = ConstantsDLL.Properties.Resources.DASH;
            lblOperatingSystem.Text = ConstantsDLL.Properties.Resources.DASH;
            lblHostname.Text = ConstantsDLL.Properties.Resources.DASH;
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
            loadingCircleCompliant.Visible = true;
            loadingCircleScanStorageType.Visible = true;
            loadingCircleScanMediaOperationMode.Visible = true;
            loadingCircleScanVideoCard.Visible = true;
            loadingCircleScanOperatingSystem.Visible = true;
            loadingCircleScanHostname.Visible = true;
            loadingCircleScanIpAddress.Visible = true;
            loadingCircleScanFwType.Visible = true;
            loadingCircleScanFwVersion.Visible = true;
            loadingCircleScanSecureBoot.Visible = true;
            loadingCircleScanVirtualizationTechnology.Visible = true;
            loadingCircleScanTpmVersion.Visible = true;
            loadingCircleLastService.Visible = true;
            loadingCircleTableMaintenances.Visible = true;
            loadingCircleCollectButton.Visible = true;

            loadingCircleScanBrand.Active = true;
            loadingCircleScanModel.Active = true;
            loadingCircleScanSerialNumber.Active = true;
            loadingCircleScanProcessor.Active = true;
            loadingCircleScanRam.Active = true;
            loadingCircleCompliant.Active = true;
            loadingCircleScanStorageType.Active = true;
            loadingCircleScanMediaOperationMode.Active = true;
            loadingCircleScanVideoCard.Active = true;
            loadingCircleScanOperatingSystem.Active = true;
            loadingCircleScanHostname.Active = true;
            loadingCircleScanIpAddress.Active = true;
            loadingCircleScanFwType.Active = true;
            loadingCircleScanFwVersion.Active = true;
            loadingCircleScanSecureBoot.Active = true;
            loadingCircleScanVirtualizationTechnology.Active = true;
            loadingCircleScanTpmVersion.Active = true;
            loadingCircleLastService.Active = true;
            loadingCircleTableMaintenances.Active = true;
            loadingCircleCollectButton.Active = true;
            #endregion

            if (!offlineMode)
            {
                loadingCircleServerOperationalStatus.Visible = true;
                loadingCircleServerOperationalStatus.Active = true;

                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_PINGGING_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                //Feches model info from server
                serverOnline = JsonFileReaderDLL.ModelHandler.CheckHost(client, ConstantsDLL.Properties.Resources.HTTP + serverIP + ":" + serverPort + ConstantsDLL.Properties.Resources.GET_MODEL_URL);

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
                loadingCircleTableMaintenances.Visible = false;
                loadingCircleTableMaintenances.Active = false;
                loadingCircleCompliant.Visible = false;
                loadingCircleCompliant.Active = false;
                lblServerIP.Text = lblServerPort.Text = lblAgentName.Text = lblColorServerOperationalStatus.Text = lblColorLastService.Text = lblColorCompliant.Text = Strings.OFFLINE_MODE_ACTIVATED;
                lblServerIP.ForeColor = lblServerPort.ForeColor = lblAgentName.ForeColor = lblColorServerOperationalStatus.ForeColor = lblColorLastService.ForeColor = lblColorCompliant.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblThereIsNothingHere.Visible = true;
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
            timerAlertSmartStatus.Enabled = false;

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
                lblIpAddress.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                storageDetailsButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
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
                lblIpAddress.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                storageDetailsButton.ForeColor = SystemColors.ControlText;
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

            progressbarCount = 0;
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for PC maker
            newAsset.hardware.brand = HardwareInfo.GetBrand();
            if (newAsset.hardware.brand == ConstantsDLL.Properties.Resources.TO_BE_FILLED_BY_OEM || newAsset.hardware.brand == string.Empty)
                newAsset.hardware.brand = HardwareInfo.GetBrandAlt();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_BM, newAsset.hardware.brand, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for PC model
            newAsset.hardware.model = HardwareInfo.GetModel();
            if (newAsset.hardware.model == ConstantsDLL.Properties.Resources.TO_BE_FILLED_BY_OEM || newAsset.hardware.model == string.Empty)
            {
                newAsset.hardware.model = HardwareInfo.GetModelAlt();
                if (newAsset.hardware.model == ConstantsDLL.Properties.Resources.TO_BE_FILLED_BY_OEM || newAsset.hardware.model == string.Empty)
                    newAsset.hardware.model = ConstantsDLL.Properties.Strings.UNKNOWN;
            }
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MODEL, newAsset.hardware.model, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for motherboard Serial number
            newAsset.hardware.serialNumber = HardwareInfo.GetSerialNumber();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_SERIALNO, newAsset.hardware.serialNumber, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for CPU information
            processorSummary = HardwareInfo.GetProcessorSummary();
            processorDetailPrev = new List<List<string>>()
            {
                HardwareInfo.GetProcessorIdList(),
                HardwareInfo.GetProcessorNameList(),
                HardwareInfo.GetProcessorFrequencyList(),
                HardwareInfo.GetProcessorCoresList(),
                HardwareInfo.GetProcessorThreadsList(),
                HardwareInfo.GetProcessorCacheList()
            };
            processorDetail = Misc.MiscMethods.Transpose(processorDetailPrev);
            newHardware.processor.Clear();
            for (int i = 0; i < processorDetail.Count; i++)
            {
                for (int j = 0; j < processorDetail[i].Count; j++)
                {
                    if (processorDetail[i][j] == null)
                        processorDetail[i][j] = ConstantsDLL.Properties.Strings.UNKNOWN;
                }
                processor p = new processor
                {
                    processorId = processorDetail[i][0],
                    name = processorDetail[i][1],
                    frequency = processorDetail[i][2],
                    numberOfCores = processorDetail[i][3],
                    numberOfThreads = processorDetail[i][4],
                    cache = processorDetail[i][5]
                };
                newHardware.processor.Add(p);
            }
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_PROCNAME, processorSummary, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for RAM amount and total number of slots
            ramSummary = HardwareInfo.GetRamSummary() + " (" + HardwareInfo.GetNumFreeRamSlots() +
                Strings.SLOTS_OF + HardwareInfo.GetNumRamSlots() + Strings.OCCUPIED + ")";
            ramDetailPrev = new List<List<string>>()
            {
                HardwareInfo.GetRamSlotList(),
                HardwareInfo.GetRamAmountList(),
                HardwareInfo.GetRamTypeList(),
                HardwareInfo.GetRamFrequencyList(),
                HardwareInfo.GetRamSerialNumberList(),
                HardwareInfo.GetRamPartNumberList(),
                HardwareInfo.GetRamManufacturerList()
            };
            ramDetail = Misc.MiscMethods.Transpose(ramDetailPrev);
            newHardware.ram.Clear();
            for (int i = 0; i < ramDetail.Count; i++)
            {
                for (int j = 0; j < ramDetail[i].Count; j++)
                {
                    if (ramDetail[i][j] == null)
                        ramDetail[i][j] = ConstantsDLL.Properties.Strings.UNKNOWN;
                }
                ram r = new ram
                {
                    slot = ramDetail[i][0],
                    amount = ramDetail[i][1],
                    type = ramDetail[i][2],
                    frequency = ramDetail[i][3],
                    serialNumber = ramDetail[i][4],
                    partNumber = ramDetail[i][5],
                    manufacturer = ramDetail[i][6]
                };
                newHardware.ram.Add(r);
            }
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_PM, ramSummary, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Storage data
            storageSummary = HardwareInfo.GetStorageSummary();
            storageDetailPrev = new List<List<string>>
            {
                HardwareInfo.GetStorageIdsList(),
                HardwareInfo.GetStorageTypeList(),
                HardwareInfo.GetStorageSizeList(),
                HardwareInfo.GetStorageConnectionList(),
                HardwareInfo.GetStorageModelList(),
                HardwareInfo.GetStorageSerialNumberList(),
                HardwareInfo.GetStorageSmartList()
            };
            storageDetail = Misc.MiscMethods.Transpose(storageDetailPrev);
            newHardware.storage.Clear();
            for (int i = 0; i < storageDetail.Count; i++)
            {
                for (int j = 0; j < storageDetail[i].Count; j++)
                {
                    if (storageDetail[i][j] == null)
                        storageDetail[i][j] = ConstantsDLL.Properties.Strings.UNKNOWN;
                }
                storage s = new storage
                {
                    storageId = storageDetail[i][0],
                    type = storageDetail[i][1],
                    size = storageDetail[i][2],
                    connection = storageDetail[i][3],
                    model = storageDetail[i][4],
                    serialNumber = storageDetail[i][5],
                    smartStatus = storageDetail[i][6]
                };
                newHardware.storage.Add(s);
            }
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MEDIATYPE, storageSummary, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Media Operation (IDE/AHCI/NVME)
            newAsset.firmware.mediaOperationMode = HardwareInfo.GetMediaOperationMode();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MEDIAOP, parametersList[8][Convert.ToInt32(newAsset.firmware.mediaOperationMode)], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Video Card information
            videoCardDetailPrev = new List<List<string>>
            {
                HardwareInfo.GetVideoCardIdList(),
                HardwareInfo.GetVideoCardNameList(),
                HardwareInfo.GetVideoCardRamList()
            };
            videoCardDetail = Misc.MiscMethods.Transpose(videoCardDetailPrev);
            newHardware.videoCard.Clear();
            for (int i = 0; i < videoCardDetail.Count; i++)
            {
                for (int j = 0; j < videoCardDetail[i].Count; j++)
                {
                    if (videoCardDetail[i][j] == null)
                        videoCardDetail[i][j] = ConstantsDLL.Properties.Strings.UNKNOWN;
                }
                videoCard v = new videoCard
                {
                    gpuId = videoCardDetail[i][0],
                    name = videoCardDetail[i][1],
                    vRam = videoCardDetail[i][2],
                };
                newHardware.videoCard.Add(v);
                newHardware.videoCard[i].name = newHardware.videoCard[i].name.Replace("(R)", string.Empty);
                newHardware.videoCard[i].name = newHardware.videoCard[i].name.Replace("(TM)", string.Empty);
                newHardware.videoCard[i].name = newHardware.videoCard[i].name.Replace("(tm)", string.Empty);
            }
            videoCardSummary = HardwareInfo.GetVideoCardSummary();

            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_GPUINFO, videoCardSummary, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for OS infomation
            newAsset.operatingSystem.arch = HardwareInfo.GetOSArchBinary();
            newAsset.operatingSystem.build = HardwareInfo.GetOSBuildAndRevision();
            newAsset.operatingSystem.name = HardwareInfo.GetOSName();
            newAsset.operatingSystem.version = HardwareInfo.GetOSVersion();
            operatingSystemSummary = HardwareInfo.GetOSSummary();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_OS, operatingSystemSummary, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Hostname
            newAsset.network.hostname = HardwareInfo.GetHostname();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_HOSTNAME, newAsset.network.hostname, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for MAC Address
            newAsset.network.macAddress = HardwareInfo.GetMacAddress();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MAC, newAsset.network.macAddress, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for IP Address
            newAsset.network.ipAddress = HardwareInfo.GetIpAddress();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_IP, newAsset.network.ipAddress, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for firmware type
            newAsset.firmware.type = HardwareInfo.GetFwType();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_BIOSTYPE, parametersList[6][Convert.ToInt32(newAsset.firmware.type)], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Secure Boot status
            newAsset.firmware.secureBoot = HardwareInfo.GetSecureBoot();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_SECBOOT, StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[9][Convert.ToInt32(newAsset.firmware.secureBoot)])], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for firmware version
            newAsset.firmware.version = HardwareInfo.GetFirmwareVersion();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_BIOS, newAsset.firmware.version, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for VT status
            newAsset.firmware.virtualizationTechnology = HardwareInfo.GetVirtualizationTechnology();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_VT, StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[10][Convert.ToInt32(newAsset.firmware.virtualizationTechnology)])], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for TPM status
            newAsset.firmware.tpmVersion = HardwareInfo.GetTPMStatus();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_TPM, parametersList[7][Convert.ToInt32(newAsset.firmware.tpmVersion)], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
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
            lblBrand.Text = newAsset.hardware.brand;
            lblModel.Text = newAsset.hardware.model;
            lblSerialNumber.Text = newAsset.hardware.serialNumber;
            lblProcessor.Text = processorSummary;
            lblRam.Text = ramSummary;
            lblStorageType.Text = storageSummary;
            lblVideoCard.Text = videoCardSummary;
            lblOperatingSystem.Text = operatingSystemSummary;
            lblHostname.Text = newAsset.network.hostname;
            lblIpAddress.Text = newAsset.network.ipAddress;
            lblFwVersion.Text = newAsset.firmware.version;

            lblMediaOperationMode.Text = parametersList[8][Convert.ToInt32(newAsset.firmware.mediaOperationMode)];
            lblFwType.Text = parametersList[6][Convert.ToInt32(newAsset.firmware.type)];
            lblSecureBoot.Text = StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[9][Convert.ToInt32(newAsset.firmware.secureBoot)])];
            lblVirtualizationTechnology.Text = StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[10][Convert.ToInt32(newAsset.firmware.virtualizationTechnology)])];
            lblTpmVersion.Text = parametersList[7][Convert.ToInt32(newAsset.firmware.tpmVersion)];

            #endregion

            pass = true;

            try
            {
                nonCompliantCount = 0;
                if (!offlineMode)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_FETCHING_ASSET_DATA, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                    //Feches asset data from server
                    existingAsset = await JsonFileReaderDLL.AssetHandler.GetAssetAsync(client, ConstantsDLL.Properties.Resources.HTTP + serverIP + ":" + serverPort + ConstantsDLL.Properties.Resources.GET_ASSET_URL + textBoxAssetNumber.Text);

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_FETCHING_MODEL_DATA, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                    //Feches model info from server
                    modelTemplate = await JsonFileReaderDLL.ModelHandler.GetModelAsync(client, ConstantsDLL.Properties.Resources.HTTP + serverIP + ":" + serverPort + ConstantsDLL.Properties.Resources.GET_MODEL_URL + lblModel.Text);

                    loadingCircleLastService.Visible = false;
                    loadingCircleLastService.Active = false;
                    //If asset exists on the database
                    if (existingAsset != null)
                    {
                        lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(existingAsset.maintenances[0].serviceDate);
                        lblColorLastService.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), lblColorLastService.Text, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                        for (int i = 0; i < existingAsset.maintenances.Count; i++)
                        {
                            //Feches agent names from server
                            agentMaintenances = await JsonFileReaderDLL.AuthenticationHandler.GetAgentAsync(client, ConstantsDLL.Properties.Resources.HTTP + serverIP + ":" + serverPort + "/api/getAgentName/" + existingAsset.maintenances[i].agentId);
                            if (agentMaintenances.id == existingAsset.maintenances[i].agentId)
                                _ = tableMaintenances.Rows.Add(existingAsset.maintenances[i].serviceDate, StringsAndConstants.LIST_MODE_GUI[Convert.ToInt32(existingAsset.maintenances[i].serviceType)], agentMaintenances.name + " " + agentMaintenances.surname);
                        }
                        tableMaintenances.Visible = true;
                        tableMaintenances.Sort(tableMaintenances.Columns["serviceDate"], ListSortDirection.Descending);
                    }
                    //If asset does not exist on the database
                    else
                    {
                        lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(string.Empty);
                        lblColorLastService.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), lblColorLastService.Text, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                        lblThereIsNothingHere.Visible = true;
                    }
                    loadingCircleTableMaintenances.Visible = false;
                    loadingCircleTableMaintenances.Active = false;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If hostname is the default one and its enforcement is enabled
                if (enforcementList[3] == ConstantsDLL.Properties.Resources.TRUE && newAsset.network.hostname.Equals(Strings.DEFAULT_HOSTNAME) && !offlineMode)
                {
                    pass = false;
                    lblHostname.Text += Strings.HOSTNAME_ALERT;
                    timerAlertHostname.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.HOSTNAME_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, mediaOpMode enforcement is enabled, and the mode is incorrect
                if (enforcementList[2] == ConstantsDLL.Properties.Resources.TRUE && modelTemplate != null && modelTemplate.mediaOperationMode != newAsset.firmware.mediaOperationMode && !offlineMode)
                {
                    pass = false;
                    lblMediaOperationMode.Text += Strings.MEDIA_OPERATION_ALERT;
                    timerAlertMediaOperationMode.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.MEDIA_OPERATION_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //The section below contains the exception cases for Secure Boot enforcement, if it is enabled
                if (enforcementList[6] == ConstantsDLL.Properties.Resources.TRUE && StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.secureBoot)] == ConstantsDLL.Properties.Strings.DEACTIVATED && !offlineMode)
                {
                    pass = false;
                    lblSecureBoot.Text += Strings.SECURE_BOOT_ALERT;
                    timerAlertSecureBoot.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.SECURE_BOOT_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does not exist and server is unreachable
                if (modelTemplate == null)
                {
                    if (!offlineMode)
                    {
                        pass = false;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                        _ = MessageBox.Show(Strings.DATABASE_REACH_ERROR, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, firmware version enforcement is enabled, and the version is incorrect
                if (enforcementList[5] == ConstantsDLL.Properties.Resources.TRUE && modelTemplate != null && !newAsset.firmware.version.Contains(modelTemplate.fwVersion) && !offlineMode)
                {
                    pass = false;
                    lblFwVersion.Text += Strings.FIRMWARE_VERSION_ALERT;
                    timerAlertFwVersion.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.FIRMWARE_VERSION_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, firmware type enforcement is enabled, and the type is incorrect
                if (enforcementList[4] == ConstantsDLL.Properties.Resources.TRUE && modelTemplate != null && modelTemplate.fwType != newAsset.firmware.type && !offlineMode)
                {
                    pass = false;
                    lblFwType.Text += Strings.FIRMWARE_TYPE_ALERT;
                    timerAlertFwType.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.FIRMWARE_TYPE_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If there is no MAC address assigned
                if (string.IsNullOrEmpty(newAsset.network.macAddress))
                {
                    if (!offlineMode) //If it's not in offline mode
                    {
                        pass = false;
                        lblIpAddress.Text = Strings.NETWORK_ERROR; //Prints a network error
                        timerAlertNetConnectivity.Enabled = true;
                        nonCompliantCount++;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.NETWORK_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    }
                    else //If it's in offline mode
                    {
                        lblIpAddress.Text = Strings.OFFLINE_MODE_ACTIVATED; //Specifies offline mode
                    }
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If Virtualization Technology is disabled for UEFI and its enforcement is enabled
                if (enforcementList[7] == ConstantsDLL.Properties.Resources.TRUE && StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.virtualizationTechnology)] == ConstantsDLL.Properties.Strings.DEACTIVATED && !offlineMode)
                {
                    pass = false;
                    lblVirtualizationTechnology.Text += Strings.VT_ALERT;
                    timerAlertVirtualizationTechnology.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.VT_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If Smart status is not OK and its enforcement is enabled
                if (enforcementList[1] == ConstantsDLL.Properties.Resources.TRUE && storageDetailPrev[6].Contains(ConstantsDLL.Properties.Resources.PRED_FAIL) && !offlineMode)
                {
                    pass = false;
                    //lblStorageType.Text += Strings.SMART_FAIL;
                    timerAlertSmartStatus.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.SMART_FAIL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, TPM enforcement is enabled, and TPM version is incorrect
                if (enforcementList[8] == ConstantsDLL.Properties.Resources.TRUE && modelTemplate != null && modelTemplate.tpmVersion != newAsset.firmware.tpmVersion && !offlineMode)
                {
                    pass = false;
                    lblTpmVersion.Text += Strings.TPM_ERROR;
                    timerAlertTpmVersion.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.TPM_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetRamAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, and its limit enforcement is enabled, shows an alert
                if (enforcementList[0] == ConstantsDLL.Properties.Resources.TRUE && d < 4.0 && Environment.Is64BitOperatingSystem && !offlineMode)
                {
                    pass = false;
                    lblRam.Text += Strings.NOT_ENOUGH_MEMORY;
                    timerAlertRamAmount.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.NOT_ENOUGH_MEMORY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                //If RAM is more than 4GB and OS is x86, and its limit enforcement is enabled, shows an alert
                if (enforcementList[0] == ConstantsDLL.Properties.Resources.TRUE && d > 4.0 && !Environment.Is64BitOperatingSystem && !offlineMode)
                {
                    pass = false;
                    lblRam.Text += Strings.TOO_MUCH_MEMORY;
                    timerAlertRamAmount.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.TOO_MUCH_MEMORY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                if (pass && !offlineMode)
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_HARDWARE_PASSED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                //If there are compliance errors, colors taskbar's progress bar red
                if (!pass)
                {
                    progressBar1.SetState(2);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                }

                if (!offlineMode)
                {
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
            Collecting();
            apcsButton.Enabled = false;
            registerButton.Enabled = false;
            collectButton.Enabled = false;
            storageDetailsButton.Visible = false;
            videoCardDetailsButton.Visible = false;
            ramDetailsButton.Visible = false;
            processorDetailsButton.Visible = false;
            if (!offlineMode)
            {
                lblThereIsNothingHere.Visible = false;
            }
            tableMaintenances.Visible = false;
            tableMaintenances.Rows.Clear();
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
            videoCardDetailsButton.Visible = true;
            ramDetailsButton.Visible = true;
            processorDetailsButton.Visible = true;
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
        /// Runs the registration for the website
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RegisterButton_ClickAsync(object sender, EventArgs e)
        {
            tbProgMain.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            loadingCircleRegisterButton.Visible = true;
            loadingCircleRegisterButton.Active = true;
            registerButton.Text = ConstantsDLL.Properties.Resources.DASH;
            registerButton.Enabled = false;
            apcsButton.Enabled = false;
            collectButton.Enabled = false;

            //If all the mandatory fields are filled and there are no pendencies
            if (!string.IsNullOrWhiteSpace(textBoxAssetNumber.Text) &&
                !string.IsNullOrWhiteSpace(textBoxRoomNumber.Text) &&
                !string.IsNullOrWhiteSpace(textBoxTicketNumber.Text) &&
                comboBoxHwType.SelectedItem != null &&
                comboBoxBuilding.SelectedItem != null &&
                comboBoxInUse.SelectedItem != null &&
                comboBoxTag.SelectedItem != null &&
                comboBoxBatteryChange.SelectedItem != null &&
                comboBoxStandard.SelectedItem != null &&
                (radioButtonFormatting.Checked || radioButtonMaintenance.Checked || radioButtonUpdateData.Checked) &&
                pass == true)
            {
                //Attribute variables to a previously created new Asset, which will be sent to the server
                location l = new location
                {
                    building = Array.IndexOf(parametersList[4], comboBoxBuilding.SelectedItem.ToString()).ToString(),
                    roomNumber = textBoxRoomLetter.Text != string.Empty ? textBoxRoomNumber.Text + textBoxRoomLetter.Text : textBoxRoomNumber.Text,
                };

                newAsset.assetNumber = textBoxAssetNumber.Text;
                newAsset.discarded = "0";
                newAsset.inUse = comboBoxInUse.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.LIST_YES_0) ? Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                newAsset.sealNumber = textBoxSealNumber.Text;
                newAsset.standard = comboBoxStandard.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.LIST_STANDARD_GUI_EMPLOYEE) ? Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString();
                newAsset.tag = comboBoxTag.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.LIST_YES_0) ? Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                newAsset.adRegistered = comboBoxActiveDirectory.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.LIST_YES_0) ? Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                newAsset.hardware.type = Array.IndexOf(parametersList[5], comboBoxHwType.SelectedItem.ToString()).ToString();
                newAsset.location = l;
                maintenances m = new maintenances();
                m.agentId = agent.id;
                m.batteryChange = comboBoxBatteryChange.SelectedItem.ToString().Equals(ConstantsDLL.Properties.Strings.LIST_YES_0) ? Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                m.serviceDate = dateTimePickerServiceDate.Value.ToString(ConstantsDLL.Properties.Resources.DATE_FORMAT).Substring(0, 10);
                m.serviceType = null;
                m.ticketNumber = textBoxTicketNumber.Text;
                newMaintenances.Clear();
                newMaintenances.Add(m);

                serverArgs[0] = serverIP;
                serverArgs[1] = serverPort;

                //If asset is discarded
                if (existingAsset != null && existingAsset.discarded == "1")
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
                            DateTime registerDate = DateTime.ParseExact(m.serviceDate, ConstantsDLL.Properties.Resources.DATE_FORMAT, CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(existingAsset.maintenances[0].serviceDate, ConstantsDLL.Properties.Resources.DATE_FORMAT, CultureInfo.InvariantCulture);

                            if (registerDate >= lastRegisterDate) //If chosen date is greater or equal than the last format/maintenance date of the PC, let proceed
                            {
                                await JsonFileReaderDLL.AssetHandler.SetAssetAsync(client, ConstantsDLL.Properties.Resources.HTTP + serverIP + ":" + serverPort + ConstantsDLL.Properties.Resources.SET_ASSET_URL, newAsset); //Send info to server

                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                                _ = MessageBox.Show(ConstantsDLL.Properties.Strings.ASSET_ADDED, ConstantsDLL.Properties.Strings.SUCCESS_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);

                                tbProgMain.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                            }
                            else //If chosen date is before the last format/maintenance date of the PC, shows an error
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.INCORRECT_REGISTER_DATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                                _ = MessageBox.Show(Strings.INCORRECT_REGISTER_DATE, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                
                                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                                tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);

                                _ = MessageBox.Show(ConstantsDLL.Properties.Strings.ASSET_NOT_ADDED, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch //If can't retrieve (asset number non existent in the database), register normally
                        {
                            await JsonFileReaderDLL.AssetHandler.SetAssetAsync(client, ConstantsDLL.Properties.Resources.HTTP + serverIP + ":" + serverPort + ConstantsDLL.Properties.Resources.SET_ASSET_URL, newAsset); //Send info to server

                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                            _ = MessageBox.Show(ConstantsDLL.Properties.Strings.ASSET_ADDED, ConstantsDLL.Properties.Strings.SUCCESS_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            lblThereIsNothingHere.Visible = false;
            tableMaintenances.Visible = false;
            loadingCircleTableMaintenances.Visible = true;
            loadingCircleTableMaintenances.Active = true;
            lblColorLastService.Text = ConstantsDLL.Properties.Resources.DASH;
            existingAsset = await JsonFileReaderDLL.AssetHandler.GetAssetAsync(client, ConstantsDLL.Properties.Resources.HTTP + serverIP + ":" + serverPort + ConstantsDLL.Properties.Resources.GET_ASSET_URL + textBoxAssetNumber.Text);
            if (existingAsset != null)
            {
                loadingCircleLastService.Visible = false;
                loadingCircleLastService.Active = false;
                lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(existingAsset.maintenances[0].serviceDate);
                lblColorLastService.ForeColor = StringsAndConstants.BLUE_FOREGROUND;

                tableMaintenances.Rows.Clear();
                for (int i = 0; i < existingAsset.maintenances.Count; i++)
                {
                    //Feches agent names from server
                    agentMaintenances = await JsonFileReaderDLL.AuthenticationHandler.GetAgentAsync(client, ConstantsDLL.Properties.Resources.HTTP + serverIP + ":" + serverPort + "/api/getAgentName/" + existingAsset.maintenances[i].agentId);
                    if (agentMaintenances.id == existingAsset.maintenances[i].agentId)
                        _ = tableMaintenances.Rows.Add(existingAsset.maintenances[i].serviceDate, StringsAndConstants.LIST_MODE_GUI[Convert.ToInt32(existingAsset.maintenances[i].serviceType)], agentMaintenances.name + " " + agentMaintenances.surname);
                }
                tableMaintenances.Visible = true;
                tableMaintenances.Sort(tableMaintenances.Columns["serviceDate"], ListSortDirection.Descending);
            }
            else
            {
                lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(string.Empty);
                lblColorLastService.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblThereIsNothingHere.Visible = true;
            }

            //When finished registering, resets control states
            loadingCircleRegisterButton.Visible = false;
            loadingCircleRegisterButton.Active = false;
            loadingCircleTableMaintenances.Visible = false;
            loadingCircleTableMaintenances.Active = false;
            registerButton.Text = Strings.REGISTER_AGAIN;
            registerButton.Enabled = true;
            apcsButton.Enabled = true;
            collectButton.Enabled = true;
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
                    b.BackColor = SystemColors.Control;
                    b.ForeColor = SystemColors.ControlText;
                    b.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
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

            lblThereIsNothingHere.ForeColor = StringsAndConstants.LIGHT_INACTIVE_CAPTION_COLOR;
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
            iconImgStorageType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_HDD_LIGHT_PATH));
            iconImgMediaOperationMode.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_AHCI_LIGHT_PATH));
            iconImgVideoCard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_GPU_LIGHT_PATH));
            iconImgOperatingSystem.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_WINDOWS_LIGHT_PATH));
            iconImgHostname.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_HOSTNAME_LIGHT_PATH));
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

            lblThereIsNothingHere.ForeColor = StringsAndConstants.DARK_INACTIVE_CAPTION_COLOR;
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
            iconImgStorageType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_HDD_DARK_PATH));
            iconImgMediaOperationMode.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_AHCI_DARK_PATH));
            iconImgVideoCard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_GPU_DARK_PATH));
            iconImgOperatingSystem.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_WINDOWS_DARK_PATH));
            iconImgHostname.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_HOSTNAME_DARK_PATH));
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
    }
}

