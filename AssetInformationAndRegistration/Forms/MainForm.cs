using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using AssetInformationAndRegistration.Properties;
using ConfigurableQualityPictureBoxDLL;
using ConstantsDLL;
using ConstantsDLL.Properties;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using MRG.Controls.UI;
using Octokit;
using RestApiDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using Label = System.Windows.Forms.Label;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary> 
    /// Class for handling Main window tasks and UI
    /// </summary>
    internal partial class MainForm : Form, ITheming
    {
        private bool isSystemDarkModeEnabled, serverOnline, pass = true, invertOSScroll, invertFwVersionScroll, invertVideoCardScroll, invertRamScroll, invertProcessorScroll;
        private int percent, progressbarCount = 0, nonCompliantCount, leftBound, rightBound;
        private int xPosOS = 0, yPosOS = 0;
        private int xPosFwVersion = 0, yPosFwVersion = 0;
        private int xPosVideoCard = 0, yPosVideoCard = 0;
        private int xPosRam = 0, yPosRam = 0;
        private int xPosProcessor = 0, yPosProcessor = 0;
        private int serviceTypeRadio;
        private string processorSummary, ramSummary, storageSummary, videoCardSummary, operatingSystemSummary;
        private readonly bool offlineMode;
        private readonly string serverIP, serverPort;
        private List<List<string>> videoCardDetailPrev, storageDetailPrev, ramDetailPrev, processorDetailPrev, videoCardDetail, storageDetail, ramDetail, processorDetail;

        private readonly HttpClient client;
        private readonly LogGenerator log;
        private readonly GitHubClient ghc;
        private readonly UserPreferenceChangedEventHandler UserPreferenceChanged;
        private readonly BackgroundWorker backgroundWorker1;

        private readonly Program.ConfigurationOptions configOptions;
        private readonly Agent agent;
        private Agent agentMaintenances;
        private Model modelTemplate;
        private Asset existingAsset;
        private readonly Asset newAsset;
        private readonly firmware newFirmware;
        private readonly hardware newHardware;
        private readonly List<processor> newProcessor;
        private readonly List<ram> newRam;
        private readonly List<storage> newStorage;
        private readonly List<videoCard> newVideoCard;
        private readonly List<maintenances> newMaintenances;
        private readonly location newLocation;
        private readonly network newNetwork;
        private readonly operatingSystem newOperatingSystem;
        private ServerParam serverParam;

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
        /// <param name="ghc">GitHub client object</param>
        /// <param name="offlineMode">Offline mode set</param>
        /// <param name="agent">Agent object</param>
        /// <param name="serverIP">Server IP address</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="log">Log file object</param>
        /// <param name="parametersList">List containing data from [Parameters]</param>
        /// <param name="enforcementList">List containing data from [Enforcement]</param>
        /// <param name="orgDataList">List containing data from [OrgData]</param>
        /// <param name="isSystemDarkModeEnabled">Theme state</param>
        internal MainForm(HttpClient client, GitHubClient ghc, LogGenerator log, Program.ConfigurationOptions configOptions, Agent agent, string serverIP, string serverPort, bool offlineMode, bool isSystemDarkModeEnabled)
        {
            InitializeComponent();

            this.client = client;
            this.ghc = ghc;
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            this.log = log;
            this.offlineMode = offlineMode;
            this.configOptions = configOptions;
            this.agent = agent;
            this.isSystemDarkModeEnabled = isSystemDarkModeEnabled;

            //Define theming according to JSON file provided info
            (int themeFileSet, bool themeEditable) = Misc.MiscMethods.GetFileThemeMode(configOptions.Definitions, isSystemDarkModeEnabled);
            switch (themeFileSet)
            {
                case 0:
                    Misc.MiscMethods.LightThemeAllControls(this);
                    LightThemeSpecificControls();
                    if (themeEditable == false)
                    {
                        isSystemDarkModeEnabled = false;
                        comboBoxThemeButton.Enabled = false;
                    }
                    break;
                case 1:
                    Misc.MiscMethods.DarkThemeAllControls(this);
                    DarkThemeSpecificControls();
                    if (themeEditable == false)
                    {
                        isSystemDarkModeEnabled = true;
                        comboBoxThemeButton.Enabled = false;
                    }
                    break;
            }

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

            UserPreferenceChanged = new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
            Disposed += new EventHandler(MainForm_Disposed);

            //Program version
#if DEBUG
            toolStripVersionText.Text = Misc.MiscMethods.Version(AirResources.DEV_STATUS); //Debug/Beta version
#else
            toolStripVersionText.Text = MiscMethods.Version(); //Release/Final version
#endif

            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OFFLINE_MODE, offlineMode.ToString(), Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

            //Sets status bar text according to info provided in the ini file
            List<string> oList = new List<string>();
            foreach (PropertyInfo pi in configOptions.OrgData.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(configOptions.OrgData);
                    if (!string.IsNullOrEmpty(value))
                        oList.Add(value + " - ");
                }
            }

            toolStripStatusBarText.Text = oList[4] + oList[2] + oList[0].Substring(0, oList[0].Length - 2);
            Text = Application.ProductName + " / " + oList[5] + oList[3] + oList[1].Substring(0, oList[1].Length - 2);

            //Inits thread worker for parallelism
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker1_ProgressChanged);
            backgroundWorker1.DoWork += new DoWorkEventHandler(BackgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        /// <summary> 
        /// Loads the form, sets some combobox values, create timers (1000 ms cadence), initializes HTTP client, and triggers a hardware collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainForm_Load(object sender, EventArgs e)
        {
            Misc.MiscMethods.SetLoadingCircles(this);

            ServerParam sp = new ServerParam();
            if (!offlineMode)
            {
                //Fetch building and hw types info from the specified server
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_SERVER_DATA, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                sp = await ParameterHandler.GetParameterAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_PARAMETERS_URL);
            }
            else
            {
                //Fetch building and hw types info from the local file
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_LOCAL_DATA, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                sp = ParameterHandler.GetOfflineModeConfigFile();
            }
            //Attributes fetching data to an object
            Parameters newParam = new Parameters()
            {
                Buildings = sp.Parameters.Buildings,
                HardwareTypes = sp.Parameters.HardwareTypes,
                FirmwareTypes = sp.Parameters.FirmwareTypes,
                TpmTypes = sp.Parameters.TpmTypes,
                MediaOperationTypes = sp.Parameters.MediaOperationTypes,
                RamTypes = sp.Parameters.RamTypes,
                SecureBootStates = sp.Parameters.SecureBootStates,
                VirtualizationTechnologyStates = sp.Parameters.VirtualizationTechnologyStates
            };
            serverParam = new ServerParam()
            {
                Parameters = newParam,
            };
            comboBoxBuilding.Items.AddRange(serverParam.Parameters.Buildings.ToArray());
            comboBoxHwType.Items.AddRange(serverParam.Parameters.HardwareTypes.ToArray());

            //Fills controls with provided info from ini file and constants dll
            comboBoxActiveDirectory.Items.AddRange(StringsAndConstants.LIST_ACTIVE_DIRECTORY_GUI.ToArray());
            comboBoxStandard.Items.AddRange(StringsAndConstants.LIST_STANDARD_GUI.ToArray());
            comboBoxInUse.Items.AddRange(StringsAndConstants.LIST_IN_USE_GUI.ToArray());
            comboBoxTag.Items.AddRange(StringsAndConstants.LIST_TAG_GUI.ToArray());
            comboBoxBatteryChange.Items.AddRange(StringsAndConstants.LIST_BATTERY_GUI.ToArray());
            if (HardwareInfo.GetHostname().Substring(0, 3).ToUpper().Equals(Resources.HOSTNAME_PATTERN))
                textBoxAssetNumber.Text = HardwareInfo.GetHostname().Substring(3);
            else
                textBoxAssetNumber.Text = string.Empty;

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

            timerAlertHostname.Interval = Convert.ToInt32(Resources.TIMER_INTERVAL);
            timerAlertMediaOperationMode.Interval = Convert.ToInt32(Resources.TIMER_INTERVAL);
            timerAlertSecureBoot.Interval = Convert.ToInt32(Resources.TIMER_INTERVAL);
            timerAlertFwVersion.Interval = Convert.ToInt32(Resources.TIMER_INTERVAL);
            timerAlertNetConnectivity.Interval = Convert.ToInt32(Resources.TIMER_INTERVAL);
            timerAlertFwType.Interval = Convert.ToInt32(Resources.TIMER_INTERVAL);
            timerAlertVirtualizationTechnology.Interval = Convert.ToInt32(Resources.TIMER_INTERVAL);
            timerAlertSmartStatus.Interval = Convert.ToInt32(Resources.TIMER_INTERVAL);
            timerAlertTpmVersion.Interval = Convert.ToInt32(Resources.TIMER_INTERVAL);
            timerAlertRamAmount.Interval = Convert.ToInt32(Resources.TIMER_INTERVAL);
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_AD_REGISTERED, comboBoxActiveDirectory.SelectedItem.ToString(), Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

            if (!offlineMode)
            {
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
        /// Handles the closing of the current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_MAIN_FORM, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
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
        /// Triggers when the form opens, and when the agent clicks to collect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectButton_Click(object sender, EventArgs e)
        {
            progressBar1.SetState(1);
            tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
            CollectPreparation();
            apcsButton.Enabled = false;
            registerButton.Enabled = false;
            collectButton.Enabled = false;
            storageDetailsButton.Visible = false;
            videoCardDetailsButton.Visible = false;
            ramDetailsButton.Visible = false;
            processorDetailsButton.Visible = false;
            if (!offlineMode)
                lblThereIsNothingHere.Visible = false;
            tableMaintenances.Visible = false;
            tableMaintenances.Rows.Clear();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_START_COLLECT_THREAD, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            StartAsync(sender, e);
        }

        /// <summary> 
        /// Prepares for the collection process
        /// </summary>
        private async void CollectPreparation()
        {
            #region Writes a dash in the labels, while scanning the hardware
            lblColorLastService.Text = Resources.DASH;
            lblBrand.Text = Resources.DASH;
            lblModel.Text = Resources.DASH;
            lblSerialNumber.Text = Resources.DASH;
            lblProcessor.Text = Resources.DASH;
            lblRam.Text = Resources.DASH;
            lblColorCompliant.Text = Resources.DASH;
            lblStorageType.Text = Resources.DASH;
            lblMediaOperationMode.Text = Resources.DASH;
            lblVideoCard.Text = Resources.DASH;
            lblOperatingSystem.Text = Resources.DASH;
            lblHostname.Text = Resources.DASH;
            lblIpAddress.Text = Resources.DASH;
            lblFwVersion.Text = Resources.DASH;
            lblFwType.Text = Resources.DASH;
            lblSecureBoot.Text = Resources.DASH;
            lblVirtualizationTechnology.Text = Resources.DASH;
            lblTpmVersion.Text = Resources.DASH;
            collectButton.Text = Resources.DASH;
            lblColorServerOperationalStatus.Text = Resources.DASH;
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

                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PINGGING_SERVER, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                //Checks if server is alive
                serverOnline = await ModelHandler.CheckHost(client, Resources.HTTP + serverIP + ":" + serverPort);

                loadingCircleServerOperationalStatus.Visible = false;
                loadingCircleServerOperationalStatus.Active = false;

                if (serverOnline && serverPort != string.Empty)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_ONLINE_SERVER, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                    lblColorServerOperationalStatus.Text = AirStrings.ONLINE;
                    lblColorServerOperationalStatus.ForeColor = StringsAndConstants.ONLINE_ALERT;
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OFFLINE_SERVER, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                    lblColorServerOperationalStatus.Text = AirStrings.OFFLINE;
                    lblColorServerOperationalStatus.ForeColor = StringsAndConstants.OFFLINE_ALERT;

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                    _ = MessageBox.Show(AirStrings.DATABASE_REACH_ERROR, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    loadingCircleServerOperationalStatus.Visible = false;
                    loadingCircleServerOperationalStatus.Active = false;
                    loadingCircleLastService.Visible = false;
                    loadingCircleLastService.Active = false;
                    loadingCircleTableMaintenances.Visible = false;
                    loadingCircleTableMaintenances.Active = false;
                    loadingCircleCompliant.Visible = false;
                    loadingCircleCompliant.Active = false;
                    lblThereIsNothingHere.Visible = true;
                }
            }
            else
            {
                lblServerIP.Text = lblServerPort.Text = lblAgentName.Text = lblColorServerOperationalStatus.Text = lblColorLastService.Text = lblColorCompliant.Text = AirStrings.OFFLINE_MODE_ACTIVATED;
                lblServerIP.ForeColor = lblServerPort.ForeColor = lblAgentName.ForeColor = lblColorServerOperationalStatus.ForeColor = lblColorLastService.ForeColor = lblColorCompliant.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                loadingCircleServerOperationalStatus.Visible = false;
                loadingCircleServerOperationalStatus.Active = false;
                loadingCircleLastService.Visible = false;
                loadingCircleLastService.Active = false;
                loadingCircleTableMaintenances.Visible = false;
                loadingCircleTableMaintenances.Active = false;
                loadingCircleCompliant.Visible = false;
                loadingCircleCompliant.Active = false;
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
        /// Runs on a separate thread, calling methods from the HardwareInfo class, and setting the variables, while reporting the progress to the progressbar
        /// </summary>
        /// <param name="worker"></param>
        private void CollectThread(BackgroundWorker worker)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_START_COLLECTING, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

            progressbarCount = 0;
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for PC maker
            newAsset.hardware.brand = HardwareInfo.GetBrand();
            if (newAsset.hardware.brand == Resources.TO_BE_FILLED_BY_OEM || newAsset.hardware.brand == string.Empty)
                newAsset.hardware.brand = HardwareInfo.GetBrandAlt();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_BM, newAsset.hardware.brand, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for PC model
            newAsset.hardware.model = HardwareInfo.GetModel();
            if (newAsset.hardware.model == Resources.TO_BE_FILLED_BY_OEM || newAsset.hardware.model == string.Empty)
            {
                newAsset.hardware.model = HardwareInfo.GetModelAlt();
                if (newAsset.hardware.model == Resources.TO_BE_FILLED_BY_OEM || newAsset.hardware.model == string.Empty)
                    newAsset.hardware.model = Strings.UNKNOWN;
            }
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_MODEL, newAsset.hardware.model, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for motherboard Serial number
            newAsset.hardware.serialNumber = HardwareInfo.GetSerialNumber();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SERIALNO, newAsset.hardware.serialNumber, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
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
                        processorDetail[i][j] = Strings.UNKNOWN;
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCNAME, processorSummary, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for RAM amount and total number of slots
            ramSummary = HardwareInfo.GetRamSummary() + " (" + HardwareInfo.GetNumFreeRamSlots() +
                AirStrings.SLOTS_OF + HardwareInfo.GetNumRamSlots() + AirStrings.OCCUPIED + ")";
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
                        ramDetail[i][j] = Strings.UNKNOWN;
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PM, ramSummary, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
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
                        storageDetail[i][j] = Strings.UNKNOWN;
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_MEDIATYPE, storageSummary, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Media Operation (IDE/AHCI/NVME)
            newAsset.firmware.mediaOperationMode = HardwareInfo.GetMediaOperationMode();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_MEDIAOP, serverParam.Parameters.MediaOperationTypes[Convert.ToInt32(newAsset.firmware.mediaOperationMode)], Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
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
                        videoCardDetail[i][j] = Strings.UNKNOWN;
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_GPUINFO, videoCardSummary, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for OS infomation
            newAsset.operatingSystem.arch = HardwareInfo.GetOSArchBinary();
            newAsset.operatingSystem.build = HardwareInfo.GetOSBuildAndRevision();
            newAsset.operatingSystem.name = HardwareInfo.GetOSName();
            newAsset.operatingSystem.version = HardwareInfo.GetOSVersion();
            operatingSystemSummary = HardwareInfo.GetOSSummary();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OS, operatingSystemSummary, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Hostname
            newAsset.network.hostname = HardwareInfo.GetHostname();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HOSTNAME, newAsset.network.hostname, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for MAC Address
            newAsset.network.macAddress = HardwareInfo.GetMacAddress();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_MAC, newAsset.network.macAddress, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for IP Address
            newAsset.network.ipAddress = HardwareInfo.GetIpAddress();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_IP, newAsset.network.ipAddress, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for firmware type
            newAsset.firmware.type = HardwareInfo.GetFwType();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_BIOSTYPE, serverParam.Parameters.FirmwareTypes[Convert.ToInt32(newAsset.firmware.type)], Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Secure Boot status
            newAsset.firmware.secureBoot = HardwareInfo.GetSecureBoot();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SECBOOT, StringsAndConstants.LIST_STATES[Convert.ToInt32(serverParam.Parameters.SecureBootStates[Convert.ToInt32(newAsset.firmware.secureBoot)])], Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for firmware version
            newAsset.firmware.version = HardwareInfo.GetFirmwareVersion();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_BIOS, newAsset.firmware.version, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for VT status
            newAsset.firmware.virtualizationTechnology = HardwareInfo.GetVirtualizationTechnology();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_VT, StringsAndConstants.LIST_STATES[Convert.ToInt32(serverParam.Parameters.VirtualizationTechnologyStates[Convert.ToInt32(newAsset.firmware.virtualizationTechnology)])], Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for TPM status
            newAsset.firmware.tpmVersion = HardwareInfo.GetTPMStatus();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_TPM, serverParam.Parameters.TpmTypes[Convert.ToInt32(newAsset.firmware.tpmVersion)], Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_END_COLLECTING, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
        }

        /// <summary> 
        /// Prints the collected data into the form labels, warning the agent when there are forbidden modes
        /// </summary>
        /// <returns>Returns a asynchronous task</returns>
        private async Task ProcessCollectedData()
        {
            Misc.MiscMethods.HideLoadingCircles(this);

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

            lblMediaOperationMode.Text = serverParam.Parameters.MediaOperationTypes[Convert.ToInt32(newAsset.firmware.mediaOperationMode)];
            lblFwType.Text = serverParam.Parameters.FirmwareTypes[Convert.ToInt32(newAsset.firmware.type)];
            lblSecureBoot.Text = StringsAndConstants.LIST_STATES[Convert.ToInt32(serverParam.Parameters.SecureBootStates[Convert.ToInt32(newAsset.firmware.secureBoot)])];
            lblVirtualizationTechnology.Text = StringsAndConstants.LIST_STATES[Convert.ToInt32(serverParam.Parameters.VirtualizationTechnologyStates[Convert.ToInt32(newAsset.firmware.virtualizationTechnology)])];
            lblTpmVersion.Text = serverParam.Parameters.TpmTypes[Convert.ToInt32(newAsset.firmware.tpmVersion)];

            #endregion

            if (serverOnline)
                pass = true;
            else
                pass = false;

            try
            {
                nonCompliantCount = 0;
                if (!offlineMode && serverOnline)
                {
                    if (textBoxAssetNumber.Text != string.Empty)
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_ASSET_DATA, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                        //Feches asset data from server
                        existingAsset = await AssetHandler.GetAssetAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_ASSET_URL + textBoxAssetNumber.Text);
                    }
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_MODEL_DATA, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                    //Feches model info from server
                    modelTemplate = await ModelHandler.GetModelAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_MODEL_URL + lblModel.Text);

                    loadingCircleLastService.Visible = false;
                    loadingCircleLastService.Active = false;

                    //If asset exists on the database
                    if (existingAsset != null)
                    {
                        radioButtonUpdateData.Enabled = true;
                        lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(existingAsset.maintenances[0].serviceDate);
                        lblColorLastService.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), lblColorLastService.Text, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                        for (int i = 0; i < existingAsset.maintenances.Count; i++)
                        {
                            //Feches agent names from server
                            agentMaintenances = await AuthenticationHandler.GetAgentAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_AGENTS_URL + existingAsset.maintenances[i].agentId);
                            if (agentMaintenances.id == existingAsset.maintenances[i].agentId)
                            {
                                _ = tableMaintenances.Rows.Add(DateTime.ParseExact(existingAsset.maintenances[i].serviceDate, Resources.DATE_FORMAT, CultureInfo.InvariantCulture).ToString(Resources.DATE_DISPLAY), StringsAndConstants.LIST_MODE_GUI[Convert.ToInt32(existingAsset.maintenances[i].serviceType)], agentMaintenances.name + " " + agentMaintenances.surname);
                            }

                        }
                        tableMaintenances.Visible = true;
                        tableMaintenances.Sort(tableMaintenances.Columns["serviceDate"], ListSortDirection.Descending);
                    }
                    //If asset does not exist on the database
                    else
                    {
                        radioButtonUpdateData.Enabled = false;
                        lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(string.Empty);
                        lblColorLastService.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), lblColorLastService.Text, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                        lblThereIsNothingHere.Visible = true;
                    }
                    loadingCircleTableMaintenances.Visible = false;
                    loadingCircleTableMaintenances.Active = false;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If hostname is the default one and its enforcement is enabled
                if (configOptions.Enforcement.Hostname.ToString() == Resources.TRUE && newAsset.network.hostname.Equals(AirStrings.DEFAULT_HOSTNAME) && !offlineMode)
                {
                    pass = false;
                    lblHostname.Text += AirStrings.HOSTNAME_ALERT;
                    timerAlertHostname.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirStrings.HOSTNAME_ALERT, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, mediaOpMode enforcement is enabled, and the mode is incorrect
                if (configOptions.Enforcement.MediaOperationMode.ToString() == Resources.TRUE && modelTemplate != null && modelTemplate.mediaOperationMode != newAsset.firmware.mediaOperationMode && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblMediaOperationMode.Text += AirStrings.MEDIA_OPERATION_ALERT;
                    timerAlertMediaOperationMode.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirStrings.MEDIA_OPERATION_ALERT, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //The section below contains the exception cases for Secure Boot enforcement, if it is enabled
                if (configOptions.Enforcement.SecureBoot.ToString() == Resources.TRUE && StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.secureBoot)] == Strings.DEACTIVATED && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblSecureBoot.Text += AirStrings.SECURE_BOOT_ALERT;
                    timerAlertSecureBoot.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirStrings.SECURE_BOOT_ALERT, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, firmware version enforcement is enabled, and the version is incorrect
                if (configOptions.Enforcement.FirmwareVersion.ToString() == Resources.TRUE && modelTemplate != null && !newAsset.firmware.version.Contains(modelTemplate.fwVersion) && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblFwVersion.Text += AirStrings.FIRMWARE_VERSION_ALERT;
                    timerAlertFwVersion.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirStrings.FIRMWARE_VERSION_ALERT, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, firmware type enforcement is enabled, and the type is incorrect
                if (configOptions.Enforcement.FirmwareType.ToString() == Resources.TRUE && modelTemplate != null && modelTemplate.fwType != newAsset.firmware.type && !offlineMode)
                {
                    pass = false;
                    lblFwType.Text += AirStrings.FIRMWARE_TYPE_ALERT;
                    timerAlertFwType.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirStrings.FIRMWARE_TYPE_ALERT, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If there is no MAC address assigned
                if (string.IsNullOrEmpty(newAsset.network.macAddress))
                {
                    if (!offlineMode) //If it's not in offline mode
                    {
                        pass = false;
                        lblIpAddress.Text = AirStrings.NETWORK_ERROR; //Prints a network error
                        timerAlertNetConnectivity.Enabled = true;
                        nonCompliantCount++;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirStrings.NETWORK_ERROR, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                    }
                    else //If it's in offline mode
                    {
                        lblIpAddress.Text = AirStrings.OFFLINE_MODE_ACTIVATED; //Specifies offline mode
                    }
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If Virtualization Technology is disabled for UEFI and its enforcement is enabled
                if (configOptions.Enforcement.VirtualizationTechnology.ToString() == Resources.TRUE && StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.virtualizationTechnology)] == Strings.DEACTIVATED && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblVirtualizationTechnology.Text += AirStrings.VT_ALERT;
                    timerAlertVirtualizationTechnology.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirStrings.VT_ALERT, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If Smart status is not OK and its enforcement is enabled
                if (configOptions.Enforcement.SmartStatus.ToString() == Resources.TRUE && storageDetailPrev[6].Contains(Resources.PRED_FAIL) && !offlineMode && serverOnline)
                {
                    pass = false;
                    //lblStorageType.Text += Strings.SMART_FAIL;
                    timerAlertSmartStatus.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirStrings.SMART_FAIL, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, TPM enforcement is enabled, and TPM version is incorrect
                if (configOptions.Enforcement.Tpm.ToString() == Resources.TRUE && modelTemplate != null && modelTemplate.tpmVersion != newAsset.firmware.tpmVersion && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblTpmVersion.Text += AirStrings.TPM_ERROR;
                    timerAlertTpmVersion.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirStrings.TPM_ERROR, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetRamAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, and its limit enforcement is enabled, shows an alert
                if (configOptions.Enforcement.RamLimit.ToString() == Resources.TRUE && d < 4.0 && Environment.Is64BitOperatingSystem && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblRam.Text += AirStrings.NOT_ENOUGH_MEMORY;
                    timerAlertRamAmount.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirStrings.NOT_ENOUGH_MEMORY, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                }
                //If RAM is more than 4GB and OS is x86, and its limit enforcement is enabled, shows an alert
                if (configOptions.Enforcement.RamLimit.ToString() == Resources.TRUE && d > 4.0 && !Environment.Is64BitOperatingSystem && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblRam.Text += AirStrings.TOO_MUCH_MEMORY;
                    timerAlertRamAmount.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirStrings.TOO_MUCH_MEMORY, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                if (pass && !offlineMode && serverOnline)
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_PASSED, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                //If there are compliance errors, colors taskbar's progress bar red
                if (!pass)
                {
                    progressBar1.SetState(2);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                }

                if (!offlineMode && serverOnline)
                {
                    if (nonCompliantCount == 0)
                    {
                        lblColorCompliant.Text = Strings.NO_PENDENCIES;
                        lblColorCompliant.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
                    }
                    else
                    {
                        lblColorCompliant.Text = nonCompliantCount.ToString() + " " + Strings.PENDENCIES;
                        lblColorCompliant.ForeColor = StringsAndConstants.ALERT_COLOR;
                    }
                    loadingCircleCompliant.Visible = false;
                    loadingCircleCompliant.Active = false;
                }
            }
            catch (Exception e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), e.Message, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            }
        }

        /// <summary> 
        /// Runs the registration process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RegisterButton_ClickAsync(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            tbProgMain.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
            loadingCircleRegisterButton.Visible = true;
            loadingCircleRegisterButton.Active = true;
            registerButton.Text = Resources.DASH;
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
                    building = Array.IndexOf(serverParam.Parameters.Buildings.ToArray(), comboBoxBuilding.SelectedItem.ToString()).ToString(),
                    roomNumber = textBoxRoomLetter.Text != string.Empty ? textBoxRoomNumber.Text + textBoxRoomLetter.Text : textBoxRoomNumber.Text,
                };

                maintenances m = new maintenances
                {
                    agentId = agent.id,
                    batteryChange = comboBoxBatteryChange.SelectedItem.ToString().Equals(Strings.LIST_YES_0) ? Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString(),
                    serviceDate = dateTimePickerServiceDate.Value.ToString(Resources.DATE_FORMAT).Substring(0, 10),
                    serviceType = serviceTypeRadio.ToString(),
                    ticketNumber = textBoxTicketNumber.Text
                };

                newAsset.assetNumber = textBoxAssetNumber.Text;
                newAsset.discarded = "0";
                newAsset.inUse = comboBoxInUse.SelectedItem.ToString().Equals(Strings.LIST_YES_0) ? Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                newAsset.sealNumber = textBoxSealNumber.Text;
                newAsset.standard = comboBoxStandard.SelectedItem.ToString().Equals(Strings.LIST_STANDARD_GUI_EMPLOYEE) ? Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString();
                newAsset.tag = comboBoxTag.SelectedItem.ToString().Equals(Strings.LIST_YES_0) ? Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                newAsset.adRegistered = comboBoxActiveDirectory.SelectedItem.ToString().Equals(Strings.LIST_YES_0) ? Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                newAsset.hardware.type = Array.IndexOf(serverParam.Parameters.HardwareTypes.ToArray(), comboBoxHwType.SelectedItem.ToString()).ToString();
                newAsset.location = l;

                newMaintenances.Clear();
                newMaintenances.Add(m);

                //If asset is discarded
                if (existingAsset != null && existingAsset.discarded == "1")
                {
                    tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.ASSET_DROPPED, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                    _ = MessageBox.Show(Strings.ASSET_DROPPED, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                }
                else //If not discarded
                {
                    if (serverOnline && serverPort != string.Empty) //If server is online and port is not null
                    {
                        try //Tries to get the latest register date from the asset number to check if the chosen date is adequate
                        {
                            DateTime registerDate = DateTime.ParseExact(newAsset.maintenances[0].serviceDate, Resources.DATE_FORMAT, CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(existingAsset.maintenances[0].serviceDate, Resources.DATE_FORMAT, CultureInfo.InvariantCulture);

                            if (registerDate >= lastRegisterDate) //If chosen date is greater or equal than the last format/maintenance date of the PC, let proceed
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_APCS_REGISTERING, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                                Uri v = await AssetHandler.SetAssetAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_ASSET_URL, newAsset); //Send info to server

                                if (v != null)
                                {
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                                    _ = MessageBox.Show(Strings.ASSET_UPDATED, Strings.SUCCESS_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                                    _ = MessageBox.Show(AirStrings.DATABASE_REACH_ERROR, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _ = MessageBox.Show(Strings.ASSET_NOT_ADDED, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                                tbProgMain.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                            }
                            else //If chosen date is before the last format/maintenance date of the PC, shows an error
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirStrings.INCORRECT_REGISTER_DATE, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                                tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);

                                _ = MessageBox.Show(AirStrings.INCORRECT_REGISTER_DATE, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                _ = MessageBox.Show(Strings.ASSET_NOT_UPDATED, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch //If can't retrieve (asset number non existent in the database), register normally
                        {
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_APCS_REGISTERING, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                            Uri v = await AssetHandler.SetAssetAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_ASSET_URL, newAsset); //Send info to server

                            if (v != null)
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                                _ = MessageBox.Show(Strings.ASSET_ADDED, Strings.SUCCESS_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                                tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                                _ = MessageBox.Show(AirStrings.DATABASE_REACH_ERROR, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                _ = MessageBox.Show(Strings.ASSET_NOT_ADDED, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            tbProgMain.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                        }
                    }
                    else //If the server is out of reach
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.SERVER_NOT_FOUND_ERROR, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                        _ = MessageBox.Show(Strings.SERVER_NOT_FOUND_ERROR, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                        tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                    }
                }
            }
            else if (!pass) //If there are pendencies in the PC config
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirStrings.PENDENCY_ERROR, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                _ = MessageBox.Show(AirStrings.PENDENCY_ERROR, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
            }
            else //If all fields are not filled
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirStrings.MANDATORY_FIELD, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                _ = MessageBox.Show(AirStrings.MANDATORY_FIELD, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

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
            lblColorLastService.Text = Resources.DASH;

            if (serverOnline)
            {
                existingAsset = await AssetHandler.GetAssetAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_ASSET_URL + textBoxAssetNumber.Text);
            }
            else
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(AirStrings.DATABASE_REACH_ERROR, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (existingAsset != null)
            {
                radioButtonUpdateData.Enabled = true;
                loadingCircleLastService.Visible = false;
                loadingCircleLastService.Active = false;
                lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(existingAsset.maintenances[0].serviceDate);
                lblColorLastService.ForeColor = StringsAndConstants.BLUE_FOREGROUND;

                tableMaintenances.Rows.Clear();
                for (int i = 0; i < existingAsset.maintenances.Count; i++)
                {
                    //Feches agent names from server
                    agentMaintenances = await AuthenticationHandler.GetAgentAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_AGENTS_URL + existingAsset.maintenances[i].agentId);
                    if (agentMaintenances.id == existingAsset.maintenances[i].agentId)
                    {
                        _ = tableMaintenances.Rows.Add(DateTime.ParseExact(existingAsset.maintenances[i].serviceDate, Resources.DATE_FORMAT, CultureInfo.InvariantCulture).ToString(Resources.DATE_DISPLAY), StringsAndConstants.LIST_MODE_GUI[Convert.ToInt32(existingAsset.maintenances[i].serviceType)], agentMaintenances.name + " " + agentMaintenances.surname);
                    }
                }
                tableMaintenances.Visible = true;
                tableMaintenances.Sort(tableMaintenances.Columns["serviceDate"], ListSortDirection.Descending);
            }
            else
            {
                radioButtonUpdateData.Enabled = false;
                lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(string.Empty);
                lblColorLastService.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                lblThereIsNothingHere.Visible = true;
            }

            //When finished registering, resets control states
            loadingCircleRegisterButton.Visible = false;
            loadingCircleRegisterButton.Active = false;
            loadingCircleTableMaintenances.Visible = false;
            loadingCircleTableMaintenances.Active = false;
            loadingCircleLastService.Visible = false;
            loadingCircleLastService.Active = false;
            registerButton.Text = AirStrings.REGISTER_AGAIN;
            registerButton.Enabled = true;
            apcsButton.Enabled = true;
            collectButton.Enabled = true;
        }

        /*-------------------------------------------------------------------------------------------------------------------------------------------*/
        /*-------------------------------------------------------------------------------------------------------------------------------------------*/
        
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
        private async void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Task p = ProcessCollectedData();
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
            collectButton.Text = AirStrings.FETCH_AGAIN; //Updates collect button text
        }

        /// <summary> 
        /// Sets service mode to formatting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonFormatting_CheckedChanged(object sender, EventArgs e)
        {
            serviceTypeRadio = 0;
        }

        /// <summary> 
        /// Sets service mode to maintenance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonMaintenance_CheckedChanged(object sender, EventArgs e)
        {
            serviceTypeRadio = 1;
        }

        /// <summary> 
        /// Sets service mode to update data only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonUpdateData_CheckedChanged(object sender, EventArgs e)
        {
            serviceTypeRadio = 2;
        }

        /// <summary> 
        /// Method for setting the auto theme via toolStrip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuAutoTheme_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_AUTOTHEME_CHANGE, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
            ToggleTheme();
        }

        /// <summary> 
        /// Method for setting the light theme via toolStrip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuLightTheme_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_LIGHTMODE_CHANGE, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
            Misc.MiscMethods.LightThemeAllControls(this);
            LightThemeSpecificControls();
            isSystemDarkModeEnabled = false;
        }

        /// <summary> 
        /// Method for setting the dark theme via toolStrip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuDarkTheme_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_DARKMODE_CHANGE, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
            Misc.MiscMethods.DarkThemeAllControls(this);
            DarkThemeSpecificControls();
            isSystemDarkModeEnabled = true;
        }

        /// <summary> 
        /// Method for opening the Storage list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageDetailsButton_Click(object sender, EventArgs e)
        {
            StorageDetailForm storageForm = new StorageDetailForm(storageDetail, configOptions.Definitions, isSystemDarkModeEnabled);
            if (HardwareInfo.GetWinVersion().Equals(Resources.WINDOWS_10))
                DarkNet.Instance.SetWindowThemeForms(storageForm, Theme.Auto);
            _ = storageForm.ShowDialog();
        }

        /// <summary> 
        /// Method for opening the Video Card list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoCardDetailsButton_Click(object sender, EventArgs e)
        {
            VideoCardDetailForm videoCardForm = new VideoCardDetailForm(videoCardDetail, configOptions.Definitions, isSystemDarkModeEnabled);
            if (HardwareInfo.GetWinVersion().Equals(Resources.WINDOWS_10))
                DarkNet.Instance.SetWindowThemeForms(videoCardForm, Theme.Auto);
            _ = videoCardForm.ShowDialog();
        }

        /// <summary> 
        /// Method for opening the RAM list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RamDetailsButton_Click(object sender, EventArgs e)
        {
            RamDetailForm ramForm = new RamDetailForm(ramDetail, configOptions.Definitions, isSystemDarkModeEnabled);
            if (HardwareInfo.GetWinVersion().Equals(Resources.WINDOWS_10))
                DarkNet.Instance.SetWindowThemeForms(ramForm, Theme.Auto);
            _ = ramForm.ShowDialog();
        }

        /// <summary> 
        /// Method for opening the Processor list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessorDetailsButton_Click(object sender, EventArgs e)
        {
            ProcessorDetailForm processorForm = new ProcessorDetailForm(processorDetail, configOptions.Definitions, isSystemDarkModeEnabled);
            if (HardwareInfo.GetWinVersion().Equals(Resources.WINDOWS_10))
                DarkNet.Instance.SetWindowThemeForms(processorForm, Theme.Auto);
            _ = processorForm.ShowDialog();
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
        /// Allow to OS label to slide left to right (and vice versa) if it is longer than its parent groupbox width
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerOSLabelScroll_Tick(object sender, EventArgs e)
        {
            if (xPosOS + lblOperatingSystem.Width > rightBound && invertOSScroll == false)
            {
                lblOperatingSystem.Location = new Point(xPosOS, yPosOS);
                xPosOS -= Convert.ToInt32(Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertOSScroll = true;
            }

            if (xPosOS < leftBound && invertOSScroll == true)
            {
                lblOperatingSystem.Location = new Point(xPosOS, yPosOS);
                xPosOS += Convert.ToInt32(Resources.LABEL_SCROLL_SPEED);
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
                xPosFwVersion -= Convert.ToInt32(Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertFwVersionScroll = true;
            }

            if (xPosFwVersion < leftBound && invertFwVersionScroll == true)
            {
                lblFwVersion.Location = new Point(xPosFwVersion, yPosFwVersion);
                xPosFwVersion += Convert.ToInt32(Resources.LABEL_SCROLL_SPEED);
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
                xPosVideoCard -= Convert.ToInt32(Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertVideoCardScroll = true;
            }

            if (xPosVideoCard < leftBound && invertVideoCardScroll == true)
            {
                lblVideoCard.Location = new Point(xPosVideoCard, yPosFwVersion);
                xPosVideoCard += Convert.ToInt32(Resources.LABEL_SCROLL_SPEED);
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
                xPosRam -= Convert.ToInt32(Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertRamScroll = true;
            }

            if (xPosRam < leftBound && invertRamScroll == true)
            {
                lblRam.Location = new Point(xPosRam, yPosRam);
                xPosRam += Convert.ToInt32(Resources.LABEL_SCROLL_SPEED);
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
                xPosProcessor -= Convert.ToInt32(Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertProcessorScroll = true;
            }

            if (xPosProcessor < leftBound && invertProcessorScroll == true)
            {
                lblProcessor.Location = new Point(xPosProcessor, yPosProcessor);
                xPosProcessor += Convert.ToInt32(Resources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertProcessorScroll = false;
            }
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
        /// Resets highlight about label when hovering with the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutLabel_MouseLeave(object sender, EventArgs e)
        {
            aboutLabelButton.ForeColor = !isSystemDarkModeEnabled ? StringsAndConstants.LIGHT_FORECOLOR : StringsAndConstants.DARK_FORECOLOR;
        }

        /// <summary> 
        /// Opens the About box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutLabelButton_Click(object sender, EventArgs e)
        {
            AboutBox aboutForm = new AboutBox(ghc, log, configOptions.Definitions, isSystemDarkModeEnabled);
            if (HardwareInfo.GetWinVersion().Equals(Resources.WINDOWS_10))
                DarkNet.Instance.SetWindowThemeForms(aboutForm, Theme.Auto);
            _ = aboutForm.ShowDialog();
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OPENING_LOG, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
#if DEBUG
            System.Diagnostics.Process.Start(configOptions.Definitions.LogLocation + Resources.LOG_FILENAME_AIR + "-v" + Application.ProductVersion + "-" + AirResources.DEV_STATUS + Resources.LOG_FILE_EXT);
#else
            System.Diagnostics.Process.Start(parametersList[2][0] + Resources.LOG_FILENAME_AIR + "-v" + Application.ProductVersion + Resources.LOG_FILE_EXT);
#endif
        }

        /// <summary> 
        /// Opens the APCS homepage for the selected address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApcsButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_VIEW_SERVER, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));
            _ = System.Diagnostics.Process.Start(Resources.HTTP + serverIP + ":" + serverPort);
        }

        /// <summary> 
        /// Restricts textbox only with chars
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
        /// Sets the storage button to flash in red
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
        /// Sets the IP label to flash in red
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
        /// Method for auto selecting the app theme
        /// </summary>
        private void ToggleTheme()
        {
            (int themeFileSet, bool _) = Misc.MiscMethods.GetFileThemeMode(configOptions.Definitions, Misc.MiscMethods.GetSystemThemeMode());
            switch (themeFileSet)
            {
                case 0:
                    Misc.MiscMethods.LightThemeAllControls(this);
                    LightThemeSpecificControls();
                    isSystemDarkModeEnabled = false;
                    break;
                case 1:
                    Misc.MiscMethods.DarkThemeAllControls(this);
                    DarkThemeSpecificControls();
                    isSystemDarkModeEnabled = true;
                    break;
            }
        }

        public void LightThemeSpecificControls()
        {
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

            toolStripAutoTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_AUTOTHEME_LIGHT_PATH));
            toolStripLightTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_LIGHTTHEME_LIGHT_PATH));
            toolStripDarkTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_DARKTHEME_LIGHT_PATH));
            comboBoxThemeButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_AUTOTHEME_LIGHT_PATH));
            logLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_LOG_LIGHT_PATH));
            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_ABOUT_LIGHT_PATH));
            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.MAIN_BANNER_LIGHT_PATH));
            iconImgBrand.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_BRAND_LIGHT_PATH));
            iconImgModel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_MODEL_LIGHT_PATH));
            iconImgSerialNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_SERIAL_NUMBER_LIGHT_PATH));
            iconImgProcessor.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_CPU_LIGHT_PATH));
            iconImgRam.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_RAM_LIGHT_PATH));
            iconImgStorageType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_HDD_LIGHT_PATH));
            iconImgMediaOperationMode.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_AHCI_LIGHT_PATH));
            iconImgVideoCard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_GPU_LIGHT_PATH));
            iconImgOperatingSystem.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_WINDOWS_LIGHT_PATH));
            iconImgHostname.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_HOSTNAME_LIGHT_PATH));
            iconImgIpAddress.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_IP_LIGHT_PATH));
            iconImgFwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_BIOS_LIGHT_PATH));
            iconImgFwVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_BIOS_VERSION_LIGHT_PATH));
            iconImgSecureBoot.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_SECURE_BOOT_LIGHT_PATH));
            iconImgAssetNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_ASSET_LIGHT_PATH));
            iconImgSealNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_SEAL_LIGHT_PATH));
            iconImgRoomNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_ROOM_LIGHT_PATH));
            iconImgBuilding.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_BUILDING_LIGHT_PATH));
            iconImgAdRegistered.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_SERVER_LIGHT_PATH));
            iconImgStandard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_STANDARD_LIGHT_PATH));
            iconImgServiceDate.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_SERVICE_LIGHT_PATH));
            iconImgRoomLetter.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_LETTER_LIGHT_PATH));
            iconImgInUse.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_IN_USE_LIGHT_PATH));
            iconImgTag.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_STICKER_LIGHT_PATH));
            iconImgHwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_TYPE_LIGHT_PATH));
            iconImgVirtualizationTechnology.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_VT_X_LIGHT_PATH));
            iconImgTpmVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_TPM_LIGHT_PATH));
            iconImgBatteryChange.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_CMOS_BATTERY_LIGHT_PATH));
            iconImgTicketNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_TICKET_LIGHT_PATH));
        }

        public void DarkThemeSpecificControls()
        {
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

            toolStripAutoTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_AUTOTHEME_DARK_PATH));
            toolStripLightTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_LIGHTTHEME_DARK_PATH));
            toolStripDarkTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_DARKTHEME_DARK_PATH));
            comboBoxThemeButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_AUTOTHEME_DARK_PATH));
            logLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_LOG_DARK_PATH));
            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_ABOUT_DARK_PATH));
            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.MAIN_BANNER_DARK_PATH));
            iconImgBrand.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_BRAND_DARK_PATH));
            iconImgModel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_MODEL_DARK_PATH));
            iconImgSerialNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_SERIAL_NUMBER_DARK_PATH));
            iconImgProcessor.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_CPU_DARK_PATH));
            iconImgRam.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_RAM_DARK_PATH));
            iconImgStorageType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_HDD_DARK_PATH));
            iconImgMediaOperationMode.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_AHCI_DARK_PATH));
            iconImgVideoCard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_GPU_DARK_PATH));
            iconImgOperatingSystem.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_WINDOWS_DARK_PATH));
            iconImgHostname.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_HOSTNAME_DARK_PATH));
            iconImgIpAddress.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_IP_DARK_PATH));
            iconImgFwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_BIOS_DARK_PATH));
            iconImgFwVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_BIOS_VERSION_DARK_PATH));
            iconImgSecureBoot.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_SECURE_BOOT_DARK_PATH));
            iconImgAssetNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_ASSET_DARK_PATH));
            iconImgSealNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_SEAL_DARK_PATH));
            iconImgRoomNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_ROOM_DARK_PATH));
            iconImgBuilding.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_BUILDING_DARK_PATH));
            iconImgAdRegistered.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_SERVER_DARK_PATH));
            iconImgStandard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_STARDARD_DARK_PATH));
            iconImgServiceDate.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_SERVICE_DARK_PATH));
            iconImgRoomLetter.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_LETTER_DARK_PATH));
            iconImgInUse.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_IN_USE_DARK_PATH));
            iconImgTag.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_STICKER_DARK_PATH));
            iconImgHwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_TYPE_DARK_PATH));
            iconImgVirtualizationTechnology.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_VT_X_DARK_PATH));
            iconImgTpmVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_TPM_DARK_PATH));
            iconImgBatteryChange.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_CMOS_BATTERY_DARK_PATH));
            iconImgTicketNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_TICKET_DARK_PATH));
        }

        /*-------------------------------------------------------------------------------------------------------------------------------------------*/
        /*-------------------------------------------------------------------------------------------------------------------------------------------*/

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            lblBrand = new System.Windows.Forms.Label();
            lblModel = new System.Windows.Forms.Label();
            lblSerialNumber = new System.Windows.Forms.Label();
            lblProcessor = new System.Windows.Forms.Label();
            lblRam = new System.Windows.Forms.Label();
            lblOperatingSystem = new System.Windows.Forms.Label();
            lblHostname = new System.Windows.Forms.Label();
            lblIpAddress = new System.Windows.Forms.Label();
            lblFixedBrand = new System.Windows.Forms.Label();
            lblFixedModel = new System.Windows.Forms.Label();
            lblFixedSerialNumber = new System.Windows.Forms.Label();
            lblFixedProcessor = new System.Windows.Forms.Label();
            lblFixedRam = new System.Windows.Forms.Label();
            lblFixedOperatingSystem = new System.Windows.Forms.Label();
            lblFixedHostname = new System.Windows.Forms.Label();
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
            vSeparator5 = new System.Windows.Forms.Label();
            vSeparator4 = new System.Windows.Forms.Label();
            vSeparator3 = new System.Windows.Forms.Label();
            vSeparator2 = new System.Windows.Forms.Label();
            vSeparator1 = new System.Windows.Forms.Label();
            hSeparator5 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            hSeparator1 = new System.Windows.Forms.Label();
            hSeparator4 = new System.Windows.Forms.Label();
            lblInactiveFirmware = new System.Windows.Forms.Label();
            hSeparator3 = new System.Windows.Forms.Label();
            lblInactiveNetwork = new System.Windows.Forms.Label();
            hSeparator2 = new System.Windows.Forms.Label();
            lblInactiveHardware = new System.Windows.Forms.Label();
            lblFixedProgressBarPercent = new System.Windows.Forms.Label();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            processorDetailsButton = new System.Windows.Forms.Button();
            ramDetailsButton = new System.Windows.Forms.Button();
            videoCardDetailsButton = new System.Windows.Forms.Button();
            loadingCircleCompliant = new MRG.Controls.UI.LoadingCircle();
            lblColorCompliant = new System.Windows.Forms.Label();
            storageDetailsButton = new System.Windows.Forms.Button();
            loadingCircleScanTpmVersion = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanVirtualizationTechnology = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanSecureBoot = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanFwVersion = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanFwType = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanIpAddress = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanHostname = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanOperatingSystem = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanVideoCard = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanMediaOperationMode = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanStorageType = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanRam = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanProcessor = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanSerialNumber = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanModel = new MRG.Controls.UI.LoadingCircle();
            loadingCircleScanBrand = new MRG.Controls.UI.LoadingCircle();
            iconImgTpmVersion = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblTpmVersion = new System.Windows.Forms.Label();
            iconImgVirtualizationTechnology = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblFixedTpmVersion = new System.Windows.Forms.Label();
            lblVirtualizationTechnology = new System.Windows.Forms.Label();
            lblFixedVirtualizationTechnology = new System.Windows.Forms.Label();
            iconImgBrand = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgSecureBoot = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgFwVersion = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgFwType = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgIpAddress = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgHostname = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgOperatingSystem = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgVideoCard = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgMediaOperationMode = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgStorageType = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
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
            comboBoxBatteryChange = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            comboBoxStandard = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            comboBoxActiveDirectory = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            comboBoxTag = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            comboBoxInUse = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            comboBoxHwType = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            comboBoxBuilding = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
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
            textBoxInactiveUpdateDataRadio = new System.Windows.Forms.TextBox();
            radioButtonUpdateData = new System.Windows.Forms.RadioButton();
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
            groupBoxTableMaintenances = new System.Windows.Forms.GroupBox();
            loadingCircleTableMaintenances = new MRG.Controls.UI.LoadingCircle();
            tableMaintenances = new System.Windows.Forms.DataGridView();
            serviceDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            serviceType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            agentUsername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            lblThereIsNothingHere = new System.Windows.Forms.Label();
            groupBoxHwData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)iconImgTpmVersion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgVirtualizationTechnology).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgBrand).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgSecureBoot).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgFwVersion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgFwType).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgIpAddress).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgHostname).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgOperatingSystem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgVideoCard).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgMediaOperationMode).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgStorageType).BeginInit();
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
            ((System.ComponentModel.ISupportInitialize)imgTopBanner).BeginInit();
            groupBoxServerStatus.SuspendLayout();
            groupBoxTableMaintenances.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tableMaintenances).BeginInit();
            SuspendLayout();
            // 
            // lblBrand
            // 
            resources.ApplyResources(lblBrand, "lblBrand");
            lblBrand.ForeColor = Color.Silver;
            lblBrand.Name = "lblBrand";
            // 
            // lblModel
            // 
            resources.ApplyResources(lblModel, "lblModel");
            lblModel.ForeColor = Color.Silver;
            lblModel.Name = "lblModel";
            // 
            // lblSerialNumber
            // 
            resources.ApplyResources(lblSerialNumber, "lblSerialNumber");
            lblSerialNumber.ForeColor = Color.Silver;
            lblSerialNumber.Name = "lblSerialNumber";
            // 
            // lblProcessor
            // 
            resources.ApplyResources(lblProcessor, "lblProcessor");
            lblProcessor.ForeColor = Color.Silver;
            lblProcessor.Name = "lblProcessor";
            // 
            // lblRam
            // 
            resources.ApplyResources(lblRam, "lblRam");
            lblRam.ForeColor = Color.Silver;
            lblRam.Name = "lblRam";
            // 
            // lblOperatingSystem
            // 
            resources.ApplyResources(lblOperatingSystem, "lblOperatingSystem");
            lblOperatingSystem.ForeColor = Color.Silver;
            lblOperatingSystem.Name = "lblOperatingSystem";
            // 
            // lblHostname
            // 
            resources.ApplyResources(lblHostname, "lblHostname");
            lblHostname.ForeColor = Color.Silver;
            lblHostname.Name = "lblHostname";
            // 
            // lblIpAddress
            // 
            resources.ApplyResources(lblIpAddress, "lblIpAddress");
            lblIpAddress.ForeColor = Color.Silver;
            lblIpAddress.Name = "lblIpAddress";
            // 
            // lblFixedBrand
            // 
            resources.ApplyResources(lblFixedBrand, "lblFixedBrand");
            lblFixedBrand.ForeColor = SystemColors.ControlText;
            lblFixedBrand.Name = "lblFixedBrand";
            // 
            // lblFixedModel
            // 
            resources.ApplyResources(lblFixedModel, "lblFixedModel");
            lblFixedModel.ForeColor = SystemColors.ControlText;
            lblFixedModel.Name = "lblFixedModel";
            // 
            // lblFixedSerialNumber
            // 
            resources.ApplyResources(lblFixedSerialNumber, "lblFixedSerialNumber");
            lblFixedSerialNumber.ForeColor = SystemColors.ControlText;
            lblFixedSerialNumber.Name = "lblFixedSerialNumber";
            // 
            // lblFixedProcessor
            // 
            resources.ApplyResources(lblFixedProcessor, "lblFixedProcessor");
            lblFixedProcessor.ForeColor = SystemColors.ControlText;
            lblFixedProcessor.Name = "lblFixedProcessor";
            // 
            // lblFixedRam
            // 
            resources.ApplyResources(lblFixedRam, "lblFixedRam");
            lblFixedRam.ForeColor = SystemColors.ControlText;
            lblFixedRam.Name = "lblFixedRam";
            // 
            // lblFixedOperatingSystem
            // 
            resources.ApplyResources(lblFixedOperatingSystem, "lblFixedOperatingSystem");
            lblFixedOperatingSystem.ForeColor = SystemColors.ControlText;
            lblFixedOperatingSystem.Name = "lblFixedOperatingSystem";
            // 
            // lblFixedHostname
            // 
            resources.ApplyResources(lblFixedHostname, "lblFixedHostname");
            lblFixedHostname.ForeColor = SystemColors.ControlText;
            lblFixedHostname.Name = "lblFixedHostname";
            // 
            // lblFixedIpAddress
            // 
            resources.ApplyResources(lblFixedIpAddress, "lblFixedIpAddress");
            lblFixedIpAddress.ForeColor = SystemColors.ControlText;
            lblFixedIpAddress.Name = "lblFixedIpAddress";
            // 
            // lblFixedAssetNumber
            // 
            resources.ApplyResources(lblFixedAssetNumber, "lblFixedAssetNumber");
            lblFixedAssetNumber.ForeColor = SystemColors.ControlText;
            lblFixedAssetNumber.Name = "lblFixedAssetNumber";
            // 
            // lblFixedSealNumber
            // 
            resources.ApplyResources(lblFixedSealNumber, "lblFixedSealNumber");
            lblFixedSealNumber.ForeColor = SystemColors.ControlText;
            lblFixedSealNumber.Name = "lblFixedSealNumber";
            // 
            // lblFixedBuilding
            // 
            resources.ApplyResources(lblFixedBuilding, "lblFixedBuilding");
            lblFixedBuilding.ForeColor = SystemColors.ControlText;
            lblFixedBuilding.Name = "lblFixedBuilding";
            // 
            // textBoxAssetNumber
            // 
            resources.ApplyResources(textBoxAssetNumber, "textBoxAssetNumber");
            textBoxAssetNumber.BackColor = SystemColors.Window;
            textBoxAssetNumber.ForeColor = SystemColors.WindowText;
            textBoxAssetNumber.Name = "textBoxAssetNumber";
            textBoxAssetNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxSealNumber
            // 
            resources.ApplyResources(textBoxSealNumber, "textBoxSealNumber");
            textBoxSealNumber.BackColor = SystemColors.Window;
            textBoxSealNumber.ForeColor = SystemColors.WindowText;
            textBoxSealNumber.Name = "textBoxSealNumber";
            textBoxSealNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxRoomNumber
            // 
            resources.ApplyResources(textBoxRoomNumber, "textBoxRoomNumber");
            textBoxRoomNumber.BackColor = SystemColors.Window;
            textBoxRoomNumber.ForeColor = SystemColors.WindowText;
            textBoxRoomNumber.Name = "textBoxRoomNumber";
            textBoxRoomNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxRoomLetter
            // 
            resources.ApplyResources(textBoxRoomLetter, "textBoxRoomLetter");
            textBoxRoomLetter.BackColor = SystemColors.Window;
            textBoxRoomLetter.CharacterCasing = CharacterCasing.Upper;
            textBoxRoomLetter.ForeColor = SystemColors.WindowText;
            textBoxRoomLetter.Name = "textBoxRoomLetter";
            textBoxRoomLetter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxCharsOnly_KeyPress);
            // 
            // lblFixedRoomNumber
            // 
            resources.ApplyResources(lblFixedRoomNumber, "lblFixedRoomNumber");
            lblFixedRoomNumber.ForeColor = SystemColors.ControlText;
            lblFixedRoomNumber.Name = "lblFixedRoomNumber";
            // 
            // lblFixedServiceDate
            // 
            resources.ApplyResources(lblFixedServiceDate, "lblFixedServiceDate");
            lblFixedServiceDate.ForeColor = SystemColors.ControlText;
            lblFixedServiceDate.Name = "lblFixedServiceDate";
            // 
            // registerButton
            // 
            resources.ApplyResources(registerButton, "registerButton");
            registerButton.Name = "registerButton";
            registerButton.UseVisualStyleBackColor = true;
            registerButton.Click += new System.EventHandler(RegisterButton_ClickAsync);
            // 
            // lblFixedInUse
            // 
            resources.ApplyResources(lblFixedInUse, "lblFixedInUse");
            lblFixedInUse.ForeColor = SystemColors.ControlText;
            lblFixedInUse.Name = "lblFixedInUse";
            // 
            // lblFixedTag
            // 
            resources.ApplyResources(lblFixedTag, "lblFixedTag");
            lblFixedTag.ForeColor = SystemColors.ControlText;
            lblFixedTag.Name = "lblFixedTag";
            // 
            // lblFixedHwType
            // 
            resources.ApplyResources(lblFixedHwType, "lblFixedHwType");
            lblFixedHwType.ForeColor = SystemColors.ControlText;
            lblFixedHwType.Name = "lblFixedHwType";
            // 
            // lblFixedServerOperationalStatus
            // 
            resources.ApplyResources(lblFixedServerOperationalStatus, "lblFixedServerOperationalStatus");
            lblFixedServerOperationalStatus.ForeColor = SystemColors.ControlText;
            lblFixedServerOperationalStatus.Name = "lblFixedServerOperationalStatus";
            // 
            // lblFixedServerPort
            // 
            resources.ApplyResources(lblFixedServerPort, "lblFixedServerPort");
            lblFixedServerPort.ForeColor = SystemColors.ControlText;
            lblFixedServerPort.Name = "lblFixedServerPort";
            // 
            // collectButton
            // 
            resources.ApplyResources(collectButton, "collectButton");
            collectButton.Name = "collectButton";
            collectButton.UseVisualStyleBackColor = true;
            collectButton.Click += new System.EventHandler(CollectButton_Click);
            // 
            // lblFixedRoomLetter
            // 
            resources.ApplyResources(lblFixedRoomLetter, "lblFixedRoomLetter");
            lblFixedRoomLetter.ForeColor = SystemColors.ControlText;
            lblFixedRoomLetter.Name = "lblFixedRoomLetter";
            // 
            // lblFixedFwVersion
            // 
            resources.ApplyResources(lblFixedFwVersion, "lblFixedFwVersion");
            lblFixedFwVersion.ForeColor = SystemColors.ControlText;
            lblFixedFwVersion.Name = "lblFixedFwVersion";
            // 
            // lblFwVersion
            // 
            resources.ApplyResources(lblFwVersion, "lblFwVersion");
            lblFwVersion.ForeColor = Color.Silver;
            lblFwVersion.Name = "lblFwVersion";
            // 
            // apcsButton
            // 
            resources.ApplyResources(apcsButton, "apcsButton");
            apcsButton.Name = "apcsButton";
            apcsButton.UseVisualStyleBackColor = true;
            apcsButton.Click += new System.EventHandler(ApcsButton_Click);
            // 
            // lblFixedFwType
            // 
            resources.ApplyResources(lblFixedFwType, "lblFixedFwType");
            lblFixedFwType.ForeColor = SystemColors.ControlText;
            lblFixedFwType.Name = "lblFixedFwType";
            // 
            // lblFwType
            // 
            resources.ApplyResources(lblFwType, "lblFwType");
            lblFwType.ForeColor = Color.Silver;
            lblFwType.Name = "lblFwType";
            // 
            // groupBoxHwData
            // 
            resources.ApplyResources(groupBoxHwData, "groupBoxHwData");
            groupBoxHwData.Controls.Add(vSeparator5);
            groupBoxHwData.Controls.Add(vSeparator4);
            groupBoxHwData.Controls.Add(vSeparator3);
            groupBoxHwData.Controls.Add(vSeparator2);
            groupBoxHwData.Controls.Add(vSeparator1);
            groupBoxHwData.Controls.Add(hSeparator5);
            groupBoxHwData.Controls.Add(label5);
            groupBoxHwData.Controls.Add(hSeparator1);
            groupBoxHwData.Controls.Add(hSeparator4);
            groupBoxHwData.Controls.Add(lblInactiveFirmware);
            groupBoxHwData.Controls.Add(hSeparator3);
            groupBoxHwData.Controls.Add(lblInactiveNetwork);
            groupBoxHwData.Controls.Add(hSeparator2);
            groupBoxHwData.Controls.Add(lblInactiveHardware);
            groupBoxHwData.Controls.Add(lblFixedProgressBarPercent);
            groupBoxHwData.Controls.Add(progressBar1);
            groupBoxHwData.Controls.Add(processorDetailsButton);
            groupBoxHwData.Controls.Add(ramDetailsButton);
            groupBoxHwData.Controls.Add(videoCardDetailsButton);
            groupBoxHwData.Controls.Add(loadingCircleCompliant);
            groupBoxHwData.Controls.Add(lblColorCompliant);
            groupBoxHwData.Controls.Add(storageDetailsButton);
            groupBoxHwData.Controls.Add(loadingCircleScanTpmVersion);
            groupBoxHwData.Controls.Add(loadingCircleScanVirtualizationTechnology);
            groupBoxHwData.Controls.Add(loadingCircleScanSecureBoot);
            groupBoxHwData.Controls.Add(loadingCircleScanFwVersion);
            groupBoxHwData.Controls.Add(loadingCircleScanFwType);
            groupBoxHwData.Controls.Add(loadingCircleScanIpAddress);
            groupBoxHwData.Controls.Add(loadingCircleScanHostname);
            groupBoxHwData.Controls.Add(loadingCircleScanOperatingSystem);
            groupBoxHwData.Controls.Add(loadingCircleScanVideoCard);
            groupBoxHwData.Controls.Add(loadingCircleScanMediaOperationMode);
            groupBoxHwData.Controls.Add(loadingCircleScanStorageType);
            groupBoxHwData.Controls.Add(loadingCircleScanRam);
            groupBoxHwData.Controls.Add(loadingCircleScanProcessor);
            groupBoxHwData.Controls.Add(loadingCircleScanSerialNumber);
            groupBoxHwData.Controls.Add(loadingCircleScanModel);
            groupBoxHwData.Controls.Add(loadingCircleScanBrand);
            groupBoxHwData.Controls.Add(iconImgTpmVersion);
            groupBoxHwData.Controls.Add(lblTpmVersion);
            groupBoxHwData.Controls.Add(iconImgVirtualizationTechnology);
            groupBoxHwData.Controls.Add(lblFixedTpmVersion);
            groupBoxHwData.Controls.Add(lblVirtualizationTechnology);
            groupBoxHwData.Controls.Add(lblFixedVirtualizationTechnology);
            groupBoxHwData.Controls.Add(iconImgBrand);
            groupBoxHwData.Controls.Add(iconImgSecureBoot);
            groupBoxHwData.Controls.Add(iconImgFwVersion);
            groupBoxHwData.Controls.Add(iconImgFwType);
            groupBoxHwData.Controls.Add(iconImgIpAddress);
            groupBoxHwData.Controls.Add(iconImgHostname);
            groupBoxHwData.Controls.Add(iconImgOperatingSystem);
            groupBoxHwData.Controls.Add(iconImgVideoCard);
            groupBoxHwData.Controls.Add(iconImgMediaOperationMode);
            groupBoxHwData.Controls.Add(iconImgStorageType);
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
            groupBoxHwData.Controls.Add(lblFixedFwType);
            groupBoxHwData.Controls.Add(lblRam);
            groupBoxHwData.Controls.Add(lblProcessor);
            groupBoxHwData.Controls.Add(lblSerialNumber);
            groupBoxHwData.Controls.Add(lblFwVersion);
            groupBoxHwData.Controls.Add(lblModel);
            groupBoxHwData.Controls.Add(lblFixedFwVersion);
            groupBoxHwData.Controls.Add(lblBrand);
            groupBoxHwData.Controls.Add(lblHostname);
            groupBoxHwData.Controls.Add(lblIpAddress);
            groupBoxHwData.Controls.Add(lblFixedModel);
            groupBoxHwData.Controls.Add(lblFixedSerialNumber);
            groupBoxHwData.Controls.Add(lblFixedProcessor);
            groupBoxHwData.Controls.Add(lblFixedRam);
            groupBoxHwData.Controls.Add(lblFixedOperatingSystem);
            groupBoxHwData.Controls.Add(lblFixedHostname);
            groupBoxHwData.Controls.Add(lblFixedIpAddress);
            groupBoxHwData.Controls.Add(lblOperatingSystem);
            groupBoxHwData.ForeColor = SystemColors.ControlText;
            groupBoxHwData.Name = "groupBoxHwData";
            groupBoxHwData.TabStop = false;
            // 
            // vSeparator5
            // 
            resources.ApplyResources(vSeparator5, "vSeparator5");
            vSeparator5.BackColor = Color.DimGray;
            vSeparator5.Name = "vSeparator5";
            // 
            // vSeparator4
            // 
            resources.ApplyResources(vSeparator4, "vSeparator4");
            vSeparator4.BackColor = Color.DimGray;
            vSeparator4.Name = "vSeparator4";
            // 
            // vSeparator3
            // 
            resources.ApplyResources(vSeparator3, "vSeparator3");
            vSeparator3.BackColor = Color.DimGray;
            vSeparator3.Name = "vSeparator3";
            // 
            // vSeparator2
            // 
            resources.ApplyResources(vSeparator2, "vSeparator2");
            vSeparator2.BackColor = Color.DimGray;
            vSeparator2.Name = "vSeparator2";
            // 
            // vSeparator1
            // 
            resources.ApplyResources(vSeparator1, "vSeparator1");
            vSeparator1.BackColor = Color.DimGray;
            vSeparator1.Name = "vSeparator1";
            // 
            // hSeparator5
            // 
            resources.ApplyResources(hSeparator5, "hSeparator5");
            hSeparator5.BackColor = Color.DimGray;
            hSeparator5.Name = "hSeparator5";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.ForeColor = SystemColors.ControlText;
            label5.Name = "label5";
            // 
            // hSeparator1
            // 
            resources.ApplyResources(hSeparator1, "hSeparator1");
            hSeparator1.BackColor = Color.DimGray;
            hSeparator1.Name = "hSeparator1";
            // 
            // hSeparator4
            // 
            resources.ApplyResources(hSeparator4, "hSeparator4");
            hSeparator4.BackColor = Color.DimGray;
            hSeparator4.Name = "hSeparator4";
            // 
            // lblInactiveFirmware
            // 
            resources.ApplyResources(lblInactiveFirmware, "lblInactiveFirmware");
            lblInactiveFirmware.ForeColor = SystemColors.ControlText;
            lblInactiveFirmware.Name = "lblInactiveFirmware";
            // 
            // hSeparator3
            // 
            resources.ApplyResources(hSeparator3, "hSeparator3");
            hSeparator3.BackColor = Color.DimGray;
            hSeparator3.Name = "hSeparator3";
            // 
            // lblInactiveNetwork
            // 
            resources.ApplyResources(lblInactiveNetwork, "lblInactiveNetwork");
            lblInactiveNetwork.ForeColor = SystemColors.ControlText;
            lblInactiveNetwork.Name = "lblInactiveNetwork";
            // 
            // hSeparator2
            // 
            resources.ApplyResources(hSeparator2, "hSeparator2");
            hSeparator2.BackColor = Color.DimGray;
            hSeparator2.Name = "hSeparator2";
            // 
            // lblInactiveHardware
            // 
            resources.ApplyResources(lblInactiveHardware, "lblInactiveHardware");
            lblInactiveHardware.ForeColor = SystemColors.ControlText;
            lblInactiveHardware.Name = "lblInactiveHardware";
            // 
            // lblFixedProgressBarPercent
            // 
            resources.ApplyResources(lblFixedProgressBarPercent, "lblFixedProgressBarPercent");
            lblFixedProgressBarPercent.BackColor = Color.Transparent;
            lblFixedProgressBarPercent.ForeColor = SystemColors.ControlText;
            lblFixedProgressBarPercent.Name = "lblFixedProgressBarPercent";
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            // 
            // processorDetailsButton
            // 
            resources.ApplyResources(processorDetailsButton, "processorDetailsButton");
            processorDetailsButton.Name = "processorDetailsButton";
            processorDetailsButton.UseVisualStyleBackColor = true;
            processorDetailsButton.Click += new System.EventHandler(ProcessorDetailsButton_Click);
            // 
            // ramDetailsButton
            // 
            resources.ApplyResources(ramDetailsButton, "ramDetailsButton");
            ramDetailsButton.Name = "ramDetailsButton";
            ramDetailsButton.UseVisualStyleBackColor = true;
            ramDetailsButton.Click += new System.EventHandler(RamDetailsButton_Click);
            // 
            // videoCardDetailsButton
            // 
            resources.ApplyResources(videoCardDetailsButton, "videoCardDetailsButton");
            videoCardDetailsButton.Name = "videoCardDetailsButton";
            videoCardDetailsButton.UseVisualStyleBackColor = true;
            videoCardDetailsButton.Click += new System.EventHandler(VideoCardDetailsButton_Click);
            // 
            // loadingCircleCompliant
            // 
            resources.ApplyResources(loadingCircleCompliant, "loadingCircleCompliant");
            loadingCircleCompliant.Active = false;
            loadingCircleCompliant.Color = Color.LightSlateGray;
            loadingCircleCompliant.InnerCircleRadius = 5;
            loadingCircleCompliant.Name = "loadingCircleCompliant";
            loadingCircleCompliant.NumberSpoke = 12;
            loadingCircleCompliant.OuterCircleRadius = 11;
            loadingCircleCompliant.RotationSpeed = 1;
            loadingCircleCompliant.SpokeThickness = 2;
            loadingCircleCompliant.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // lblColorCompliant
            // 
            resources.ApplyResources(lblColorCompliant, "lblColorCompliant");
            lblColorCompliant.ForeColor = SystemColors.MenuHighlight;
            lblColorCompliant.Name = "lblColorCompliant";
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
            resources.ApplyResources(loadingCircleScanTpmVersion, "loadingCircleScanTpmVersion");
            loadingCircleScanTpmVersion.Active = false;
            loadingCircleScanTpmVersion.Color = Color.LightSlateGray;
            loadingCircleScanTpmVersion.ForeColor = SystemColors.ControlText;
            loadingCircleScanTpmVersion.InnerCircleRadius = 5;
            loadingCircleScanTpmVersion.Name = "loadingCircleScanTpmVersion";
            loadingCircleScanTpmVersion.NumberSpoke = 12;
            loadingCircleScanTpmVersion.OuterCircleRadius = 11;
            loadingCircleScanTpmVersion.RotationSpeed = 1;
            loadingCircleScanTpmVersion.SpokeThickness = 2;
            loadingCircleScanTpmVersion.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanVirtualizationTechnology
            // 
            resources.ApplyResources(loadingCircleScanVirtualizationTechnology, "loadingCircleScanVirtualizationTechnology");
            loadingCircleScanVirtualizationTechnology.Active = false;
            loadingCircleScanVirtualizationTechnology.Color = Color.LightSlateGray;
            loadingCircleScanVirtualizationTechnology.ForeColor = SystemColors.ControlText;
            loadingCircleScanVirtualizationTechnology.InnerCircleRadius = 5;
            loadingCircleScanVirtualizationTechnology.Name = "loadingCircleScanVirtualizationTechnology";
            loadingCircleScanVirtualizationTechnology.NumberSpoke = 12;
            loadingCircleScanVirtualizationTechnology.OuterCircleRadius = 11;
            loadingCircleScanVirtualizationTechnology.RotationSpeed = 1;
            loadingCircleScanVirtualizationTechnology.SpokeThickness = 2;
            loadingCircleScanVirtualizationTechnology.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanSecureBoot
            // 
            resources.ApplyResources(loadingCircleScanSecureBoot, "loadingCircleScanSecureBoot");
            loadingCircleScanSecureBoot.Active = false;
            loadingCircleScanSecureBoot.Color = Color.LightSlateGray;
            loadingCircleScanSecureBoot.ForeColor = SystemColors.ControlText;
            loadingCircleScanSecureBoot.InnerCircleRadius = 5;
            loadingCircleScanSecureBoot.Name = "loadingCircleScanSecureBoot";
            loadingCircleScanSecureBoot.NumberSpoke = 12;
            loadingCircleScanSecureBoot.OuterCircleRadius = 11;
            loadingCircleScanSecureBoot.RotationSpeed = 1;
            loadingCircleScanSecureBoot.SpokeThickness = 2;
            loadingCircleScanSecureBoot.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanFwVersion
            // 
            resources.ApplyResources(loadingCircleScanFwVersion, "loadingCircleScanFwVersion");
            loadingCircleScanFwVersion.Active = false;
            loadingCircleScanFwVersion.Color = Color.LightSlateGray;
            loadingCircleScanFwVersion.ForeColor = SystemColors.ControlText;
            loadingCircleScanFwVersion.InnerCircleRadius = 5;
            loadingCircleScanFwVersion.Name = "loadingCircleScanFwVersion";
            loadingCircleScanFwVersion.NumberSpoke = 12;
            loadingCircleScanFwVersion.OuterCircleRadius = 11;
            loadingCircleScanFwVersion.RotationSpeed = 1;
            loadingCircleScanFwVersion.SpokeThickness = 2;
            loadingCircleScanFwVersion.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanFwType
            // 
            resources.ApplyResources(loadingCircleScanFwType, "loadingCircleScanFwType");
            loadingCircleScanFwType.Active = false;
            loadingCircleScanFwType.Color = Color.LightSlateGray;
            loadingCircleScanFwType.ForeColor = SystemColors.ControlText;
            loadingCircleScanFwType.InnerCircleRadius = 5;
            loadingCircleScanFwType.Name = "loadingCircleScanFwType";
            loadingCircleScanFwType.NumberSpoke = 12;
            loadingCircleScanFwType.OuterCircleRadius = 11;
            loadingCircleScanFwType.RotationSpeed = 1;
            loadingCircleScanFwType.SpokeThickness = 2;
            loadingCircleScanFwType.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanIpAddress
            // 
            resources.ApplyResources(loadingCircleScanIpAddress, "loadingCircleScanIpAddress");
            loadingCircleScanIpAddress.Active = false;
            loadingCircleScanIpAddress.Color = Color.LightSlateGray;
            loadingCircleScanIpAddress.ForeColor = SystemColors.ControlText;
            loadingCircleScanIpAddress.InnerCircleRadius = 5;
            loadingCircleScanIpAddress.Name = "loadingCircleScanIpAddress";
            loadingCircleScanIpAddress.NumberSpoke = 12;
            loadingCircleScanIpAddress.OuterCircleRadius = 11;
            loadingCircleScanIpAddress.RotationSpeed = 1;
            loadingCircleScanIpAddress.SpokeThickness = 2;
            loadingCircleScanIpAddress.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanHostname
            // 
            resources.ApplyResources(loadingCircleScanHostname, "loadingCircleScanHostname");
            loadingCircleScanHostname.Active = false;
            loadingCircleScanHostname.Color = Color.LightSlateGray;
            loadingCircleScanHostname.ForeColor = SystemColors.ControlText;
            loadingCircleScanHostname.InnerCircleRadius = 5;
            loadingCircleScanHostname.Name = "loadingCircleScanHostname";
            loadingCircleScanHostname.NumberSpoke = 12;
            loadingCircleScanHostname.OuterCircleRadius = 11;
            loadingCircleScanHostname.RotationSpeed = 1;
            loadingCircleScanHostname.SpokeThickness = 2;
            loadingCircleScanHostname.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanOperatingSystem
            // 
            resources.ApplyResources(loadingCircleScanOperatingSystem, "loadingCircleScanOperatingSystem");
            loadingCircleScanOperatingSystem.Active = false;
            loadingCircleScanOperatingSystem.Color = Color.LightSlateGray;
            loadingCircleScanOperatingSystem.ForeColor = SystemColors.ControlText;
            loadingCircleScanOperatingSystem.InnerCircleRadius = 5;
            loadingCircleScanOperatingSystem.Name = "loadingCircleScanOperatingSystem";
            loadingCircleScanOperatingSystem.NumberSpoke = 12;
            loadingCircleScanOperatingSystem.OuterCircleRadius = 11;
            loadingCircleScanOperatingSystem.RotationSpeed = 1;
            loadingCircleScanOperatingSystem.SpokeThickness = 2;
            loadingCircleScanOperatingSystem.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanVideoCard
            // 
            resources.ApplyResources(loadingCircleScanVideoCard, "loadingCircleScanVideoCard");
            loadingCircleScanVideoCard.Active = false;
            loadingCircleScanVideoCard.Color = Color.LightSlateGray;
            loadingCircleScanVideoCard.ForeColor = SystemColors.ControlText;
            loadingCircleScanVideoCard.InnerCircleRadius = 5;
            loadingCircleScanVideoCard.Name = "loadingCircleScanVideoCard";
            loadingCircleScanVideoCard.NumberSpoke = 12;
            loadingCircleScanVideoCard.OuterCircleRadius = 11;
            loadingCircleScanVideoCard.RotationSpeed = 1;
            loadingCircleScanVideoCard.SpokeThickness = 2;
            loadingCircleScanVideoCard.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanMediaOperationMode
            // 
            resources.ApplyResources(loadingCircleScanMediaOperationMode, "loadingCircleScanMediaOperationMode");
            loadingCircleScanMediaOperationMode.Active = false;
            loadingCircleScanMediaOperationMode.Color = Color.LightSlateGray;
            loadingCircleScanMediaOperationMode.ForeColor = SystemColors.ControlText;
            loadingCircleScanMediaOperationMode.InnerCircleRadius = 5;
            loadingCircleScanMediaOperationMode.Name = "loadingCircleScanMediaOperationMode";
            loadingCircleScanMediaOperationMode.NumberSpoke = 12;
            loadingCircleScanMediaOperationMode.OuterCircleRadius = 11;
            loadingCircleScanMediaOperationMode.RotationSpeed = 1;
            loadingCircleScanMediaOperationMode.SpokeThickness = 2;
            loadingCircleScanMediaOperationMode.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanStorageType
            // 
            resources.ApplyResources(loadingCircleScanStorageType, "loadingCircleScanStorageType");
            loadingCircleScanStorageType.Active = false;
            loadingCircleScanStorageType.Color = Color.LightSlateGray;
            loadingCircleScanStorageType.ForeColor = SystemColors.ControlText;
            loadingCircleScanStorageType.InnerCircleRadius = 5;
            loadingCircleScanStorageType.Name = "loadingCircleScanStorageType";
            loadingCircleScanStorageType.NumberSpoke = 12;
            loadingCircleScanStorageType.OuterCircleRadius = 11;
            loadingCircleScanStorageType.RotationSpeed = 1;
            loadingCircleScanStorageType.SpokeThickness = 2;
            loadingCircleScanStorageType.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanRam
            // 
            resources.ApplyResources(loadingCircleScanRam, "loadingCircleScanRam");
            loadingCircleScanRam.Active = false;
            loadingCircleScanRam.Color = Color.LightSlateGray;
            loadingCircleScanRam.ForeColor = SystemColors.ControlText;
            loadingCircleScanRam.InnerCircleRadius = 5;
            loadingCircleScanRam.Name = "loadingCircleScanRam";
            loadingCircleScanRam.NumberSpoke = 12;
            loadingCircleScanRam.OuterCircleRadius = 11;
            loadingCircleScanRam.RotationSpeed = 1;
            loadingCircleScanRam.SpokeThickness = 2;
            loadingCircleScanRam.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanProcessor
            // 
            resources.ApplyResources(loadingCircleScanProcessor, "loadingCircleScanProcessor");
            loadingCircleScanProcessor.Active = false;
            loadingCircleScanProcessor.Color = Color.LightSlateGray;
            loadingCircleScanProcessor.ForeColor = SystemColors.ControlText;
            loadingCircleScanProcessor.InnerCircleRadius = 5;
            loadingCircleScanProcessor.Name = "loadingCircleScanProcessor";
            loadingCircleScanProcessor.NumberSpoke = 12;
            loadingCircleScanProcessor.OuterCircleRadius = 11;
            loadingCircleScanProcessor.RotationSpeed = 1;
            loadingCircleScanProcessor.SpokeThickness = 2;
            loadingCircleScanProcessor.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanSerialNumber
            // 
            resources.ApplyResources(loadingCircleScanSerialNumber, "loadingCircleScanSerialNumber");
            loadingCircleScanSerialNumber.Active = false;
            loadingCircleScanSerialNumber.Color = Color.LightSlateGray;
            loadingCircleScanSerialNumber.ForeColor = SystemColors.ControlText;
            loadingCircleScanSerialNumber.InnerCircleRadius = 5;
            loadingCircleScanSerialNumber.Name = "loadingCircleScanSerialNumber";
            loadingCircleScanSerialNumber.NumberSpoke = 12;
            loadingCircleScanSerialNumber.OuterCircleRadius = 11;
            loadingCircleScanSerialNumber.RotationSpeed = 1;
            loadingCircleScanSerialNumber.SpokeThickness = 2;
            loadingCircleScanSerialNumber.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanModel
            // 
            resources.ApplyResources(loadingCircleScanModel, "loadingCircleScanModel");
            loadingCircleScanModel.Active = false;
            loadingCircleScanModel.Color = Color.LightSlateGray;
            loadingCircleScanModel.ForeColor = SystemColors.ControlText;
            loadingCircleScanModel.InnerCircleRadius = 5;
            loadingCircleScanModel.Name = "loadingCircleScanModel";
            loadingCircleScanModel.NumberSpoke = 12;
            loadingCircleScanModel.OuterCircleRadius = 11;
            loadingCircleScanModel.RotationSpeed = 1;
            loadingCircleScanModel.SpokeThickness = 2;
            loadingCircleScanModel.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanBrand
            // 
            resources.ApplyResources(loadingCircleScanBrand, "loadingCircleScanBrand");
            loadingCircleScanBrand.Active = false;
            loadingCircleScanBrand.Color = Color.LightSlateGray;
            loadingCircleScanBrand.ForeColor = SystemColors.ControlText;
            loadingCircleScanBrand.InnerCircleRadius = 5;
            loadingCircleScanBrand.Name = "loadingCircleScanBrand";
            loadingCircleScanBrand.NumberSpoke = 12;
            loadingCircleScanBrand.OuterCircleRadius = 11;
            loadingCircleScanBrand.RotationSpeed = 1;
            loadingCircleScanBrand.SpokeThickness = 2;
            loadingCircleScanBrand.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // iconImgTpmVersion
            // 
            resources.ApplyResources(iconImgTpmVersion, "iconImgTpmVersion");
            iconImgTpmVersion.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgTpmVersion.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgTpmVersion.Name = "iconImgTpmVersion";
            iconImgTpmVersion.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgTpmVersion.TabStop = false;
            // 
            // lblTpmVersion
            // 
            resources.ApplyResources(lblTpmVersion, "lblTpmVersion");
            lblTpmVersion.ForeColor = Color.Silver;
            lblTpmVersion.Name = "lblTpmVersion";
            // 
            // iconImgVirtualizationTechnology
            // 
            resources.ApplyResources(iconImgVirtualizationTechnology, "iconImgVirtualizationTechnology");
            iconImgVirtualizationTechnology.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgVirtualizationTechnology.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgVirtualizationTechnology.Name = "iconImgVirtualizationTechnology";
            iconImgVirtualizationTechnology.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgVirtualizationTechnology.TabStop = false;
            // 
            // lblFixedTpmVersion
            // 
            resources.ApplyResources(lblFixedTpmVersion, "lblFixedTpmVersion");
            lblFixedTpmVersion.ForeColor = SystemColors.ControlText;
            lblFixedTpmVersion.Name = "lblFixedTpmVersion";
            // 
            // lblVirtualizationTechnology
            // 
            resources.ApplyResources(lblVirtualizationTechnology, "lblVirtualizationTechnology");
            lblVirtualizationTechnology.ForeColor = Color.Silver;
            lblVirtualizationTechnology.Name = "lblVirtualizationTechnology";
            // 
            // lblFixedVirtualizationTechnology
            // 
            resources.ApplyResources(lblFixedVirtualizationTechnology, "lblFixedVirtualizationTechnology");
            lblFixedVirtualizationTechnology.ForeColor = SystemColors.ControlText;
            lblFixedVirtualizationTechnology.Name = "lblFixedVirtualizationTechnology";
            // 
            // iconImgBrand
            // 
            resources.ApplyResources(iconImgBrand, "iconImgBrand");
            iconImgBrand.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgBrand.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgBrand.Name = "iconImgBrand";
            iconImgBrand.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgBrand.TabStop = false;
            // 
            // iconImgSecureBoot
            // 
            resources.ApplyResources(iconImgSecureBoot, "iconImgSecureBoot");
            iconImgSecureBoot.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgSecureBoot.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgSecureBoot.Name = "iconImgSecureBoot";
            iconImgSecureBoot.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgSecureBoot.TabStop = false;
            // 
            // iconImgFwVersion
            // 
            resources.ApplyResources(iconImgFwVersion, "iconImgFwVersion");
            iconImgFwVersion.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgFwVersion.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgFwVersion.Name = "iconImgFwVersion";
            iconImgFwVersion.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgFwVersion.TabStop = false;
            // 
            // iconImgFwType
            // 
            resources.ApplyResources(iconImgFwType, "iconImgFwType");
            iconImgFwType.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgFwType.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgFwType.Name = "iconImgFwType";
            iconImgFwType.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgFwType.TabStop = false;
            // 
            // iconImgIpAddress
            // 
            resources.ApplyResources(iconImgIpAddress, "iconImgIpAddress");
            iconImgIpAddress.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgIpAddress.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgIpAddress.Name = "iconImgIpAddress";
            iconImgIpAddress.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgIpAddress.TabStop = false;
            // 
            // iconImgHostname
            // 
            resources.ApplyResources(iconImgHostname, "iconImgHostname");
            iconImgHostname.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgHostname.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgHostname.Name = "iconImgHostname";
            iconImgHostname.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgHostname.TabStop = false;
            // 
            // iconImgOperatingSystem
            // 
            resources.ApplyResources(iconImgOperatingSystem, "iconImgOperatingSystem");
            iconImgOperatingSystem.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgOperatingSystem.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgOperatingSystem.Name = "iconImgOperatingSystem";
            iconImgOperatingSystem.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgOperatingSystem.TabStop = false;
            // 
            // iconImgVideoCard
            // 
            resources.ApplyResources(iconImgVideoCard, "iconImgVideoCard");
            iconImgVideoCard.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgVideoCard.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgVideoCard.Name = "iconImgVideoCard";
            iconImgVideoCard.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgVideoCard.TabStop = false;
            // 
            // iconImgMediaOperationMode
            // 
            resources.ApplyResources(iconImgMediaOperationMode, "iconImgMediaOperationMode");
            iconImgMediaOperationMode.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgMediaOperationMode.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgMediaOperationMode.Name = "iconImgMediaOperationMode";
            iconImgMediaOperationMode.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgMediaOperationMode.TabStop = false;
            // 
            // iconImgStorageType
            // 
            resources.ApplyResources(iconImgStorageType, "iconImgStorageType");
            iconImgStorageType.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgStorageType.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgStorageType.Name = "iconImgStorageType";
            iconImgStorageType.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgStorageType.TabStop = false;
            // 
            // iconImgRam
            // 
            resources.ApplyResources(iconImgRam, "iconImgRam");
            iconImgRam.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgRam.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgRam.Name = "iconImgRam";
            iconImgRam.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgRam.TabStop = false;
            // 
            // iconImgProcessor
            // 
            resources.ApplyResources(iconImgProcessor, "iconImgProcessor");
            iconImgProcessor.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgProcessor.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgProcessor.Name = "iconImgProcessor";
            iconImgProcessor.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgProcessor.TabStop = false;
            // 
            // iconImgSerialNumber
            // 
            resources.ApplyResources(iconImgSerialNumber, "iconImgSerialNumber");
            iconImgSerialNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgSerialNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgSerialNumber.Name = "iconImgSerialNumber";
            iconImgSerialNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgSerialNumber.TabStop = false;
            // 
            // iconImgModel
            // 
            resources.ApplyResources(iconImgModel, "iconImgModel");
            iconImgModel.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgModel.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgModel.Name = "iconImgModel";
            iconImgModel.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgModel.TabStop = false;
            // 
            // lblSecureBoot
            // 
            resources.ApplyResources(lblSecureBoot, "lblSecureBoot");
            lblSecureBoot.ForeColor = Color.Silver;
            lblSecureBoot.Name = "lblSecureBoot";
            // 
            // lblFixedSecureBoot
            // 
            resources.ApplyResources(lblFixedSecureBoot, "lblFixedSecureBoot");
            lblFixedSecureBoot.ForeColor = SystemColors.ControlText;
            lblFixedSecureBoot.Name = "lblFixedSecureBoot";
            // 
            // lblMediaOperationMode
            // 
            resources.ApplyResources(lblMediaOperationMode, "lblMediaOperationMode");
            lblMediaOperationMode.ForeColor = Color.Silver;
            lblMediaOperationMode.Name = "lblMediaOperationMode";
            // 
            // lblFixedMediaOperationMode
            // 
            resources.ApplyResources(lblFixedMediaOperationMode, "lblFixedMediaOperationMode");
            lblFixedMediaOperationMode.ForeColor = SystemColors.ControlText;
            lblFixedMediaOperationMode.Name = "lblFixedMediaOperationMode";
            // 
            // lblVideoCard
            // 
            resources.ApplyResources(lblVideoCard, "lblVideoCard");
            lblVideoCard.ForeColor = Color.Silver;
            lblVideoCard.Name = "lblVideoCard";
            // 
            // lblFixedVideoCard
            // 
            resources.ApplyResources(lblFixedVideoCard, "lblFixedVideoCard");
            lblFixedVideoCard.ForeColor = SystemColors.ControlText;
            lblFixedVideoCard.Name = "lblFixedVideoCard";
            // 
            // lblStorageType
            // 
            resources.ApplyResources(lblStorageType, "lblStorageType");
            lblStorageType.ForeColor = Color.Silver;
            lblStorageType.Name = "lblStorageType";
            // 
            // lblFixedStorageType
            // 
            resources.ApplyResources(lblFixedStorageType, "lblFixedStorageType");
            lblFixedStorageType.ForeColor = SystemColors.ControlText;
            lblFixedStorageType.Name = "lblFixedStorageType";
            // 
            // groupBoxAssetData
            // 
            resources.ApplyResources(groupBoxAssetData, "groupBoxAssetData");
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
            groupBoxAssetData.ForeColor = SystemColors.ControlText;
            groupBoxAssetData.Name = "groupBoxAssetData";
            groupBoxAssetData.TabStop = false;
            // 
            // comboBoxBatteryChange
            // 
            resources.ApplyResources(comboBoxBatteryChange, "comboBoxBatteryChange");
            comboBoxBatteryChange.BackColor = SystemColors.Window;
            comboBoxBatteryChange.BorderColor = Color.FromArgb(122, 122, 122);
            comboBoxBatteryChange.ButtonColor = SystemColors.Window;
            comboBoxBatteryChange.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxBatteryChange.FormattingEnabled = true;
            comboBoxBatteryChange.Name = "comboBoxBatteryChange";
            // 
            // comboBoxStandard
            // 
            resources.ApplyResources(comboBoxStandard, "comboBoxStandard");
            comboBoxStandard.BackColor = SystemColors.Window;
            comboBoxStandard.BorderColor = Color.FromArgb(122, 122, 122);
            comboBoxStandard.ButtonColor = SystemColors.Window;
            comboBoxStandard.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxStandard.FormattingEnabled = true;
            comboBoxStandard.Name = "comboBoxStandard";
            // 
            // comboBoxActiveDirectory
            // 
            resources.ApplyResources(comboBoxActiveDirectory, "comboBoxActiveDirectory");
            comboBoxActiveDirectory.BackColor = SystemColors.Window;
            comboBoxActiveDirectory.BorderColor = Color.FromArgb(122, 122, 122);
            comboBoxActiveDirectory.ButtonColor = SystemColors.Window;
            comboBoxActiveDirectory.FormattingEnabled = true;
            comboBoxActiveDirectory.Name = "comboBoxActiveDirectory";
            // 
            // comboBoxTag
            // 
            resources.ApplyResources(comboBoxTag, "comboBoxTag");
            comboBoxTag.BackColor = SystemColors.Window;
            comboBoxTag.BorderColor = Color.FromArgb(122, 122, 122);
            comboBoxTag.ButtonColor = SystemColors.Window;
            comboBoxTag.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxTag.FormattingEnabled = true;
            comboBoxTag.Name = "comboBoxTag";
            // 
            // comboBoxInUse
            // 
            resources.ApplyResources(comboBoxInUse, "comboBoxInUse");
            comboBoxInUse.BackColor = SystemColors.Window;
            comboBoxInUse.BorderColor = Color.FromArgb(122, 122, 122);
            comboBoxInUse.ButtonColor = SystemColors.Window;
            comboBoxInUse.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxInUse.FormattingEnabled = true;
            comboBoxInUse.Name = "comboBoxInUse";
            // 
            // comboBoxHwType
            // 
            resources.ApplyResources(comboBoxHwType, "comboBoxHwType");
            comboBoxHwType.BackColor = SystemColors.Window;
            comboBoxHwType.BorderColor = Color.FromArgb(122, 122, 122);
            comboBoxHwType.ButtonColor = SystemColors.Window;
            comboBoxHwType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxHwType.FormattingEnabled = true;
            comboBoxHwType.Name = "comboBoxHwType";
            // 
            // comboBoxBuilding
            // 
            resources.ApplyResources(comboBoxBuilding, "comboBoxBuilding");
            comboBoxBuilding.BackColor = SystemColors.Window;
            comboBoxBuilding.BorderColor = Color.FromArgb(122, 122, 122);
            comboBoxBuilding.ButtonColor = SystemColors.Window;
            comboBoxBuilding.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxBuilding.FormattingEnabled = true;
            comboBoxBuilding.Name = "comboBoxBuilding";
            // 
            // lblFixedMandatoryTicketNumber
            // 
            resources.ApplyResources(lblFixedMandatoryTicketNumber, "lblFixedMandatoryTicketNumber");
            lblFixedMandatoryTicketNumber.ForeColor = Color.Red;
            lblFixedMandatoryTicketNumber.Name = "lblFixedMandatoryTicketNumber";
            // 
            // lblFixedMandatoryBatteryChange
            // 
            resources.ApplyResources(lblFixedMandatoryBatteryChange, "lblFixedMandatoryBatteryChange");
            lblFixedMandatoryBatteryChange.ForeColor = Color.Red;
            lblFixedMandatoryBatteryChange.Name = "lblFixedMandatoryBatteryChange";
            // 
            // iconImgTicketNumber
            // 
            resources.ApplyResources(iconImgTicketNumber, "iconImgTicketNumber");
            iconImgTicketNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgTicketNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgTicketNumber.Name = "iconImgTicketNumber";
            iconImgTicketNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgTicketNumber.TabStop = false;
            // 
            // lblFixedTicketNumber
            // 
            resources.ApplyResources(lblFixedTicketNumber, "lblFixedTicketNumber");
            lblFixedTicketNumber.ForeColor = SystemColors.ControlText;
            lblFixedTicketNumber.Name = "lblFixedTicketNumber";
            // 
            // textBoxTicketNumber
            // 
            resources.ApplyResources(textBoxTicketNumber, "textBoxTicketNumber");
            textBoxTicketNumber.BackColor = SystemColors.Window;
            textBoxTicketNumber.ForeColor = SystemColors.WindowText;
            textBoxTicketNumber.Name = "textBoxTicketNumber";
            textBoxTicketNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNumbersOnly_KeyPress);
            // 
            // iconImgBatteryChange
            // 
            resources.ApplyResources(iconImgBatteryChange, "iconImgBatteryChange");
            iconImgBatteryChange.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgBatteryChange.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgBatteryChange.Name = "iconImgBatteryChange";
            iconImgBatteryChange.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgBatteryChange.TabStop = false;
            // 
            // lblFixedMandatoryWho
            // 
            resources.ApplyResources(lblFixedMandatoryWho, "lblFixedMandatoryWho");
            lblFixedMandatoryWho.ForeColor = Color.Red;
            lblFixedMandatoryWho.Name = "lblFixedMandatoryWho";
            // 
            // lblFixedMandatoryTag
            // 
            resources.ApplyResources(lblFixedMandatoryTag, "lblFixedMandatoryTag");
            lblFixedMandatoryTag.ForeColor = Color.Red;
            lblFixedMandatoryTag.Name = "lblFixedMandatoryTag";
            // 
            // lblFixedBatteryChange
            // 
            resources.ApplyResources(lblFixedBatteryChange, "lblFixedBatteryChange");
            lblFixedBatteryChange.ForeColor = SystemColors.ControlText;
            lblFixedBatteryChange.Name = "lblFixedBatteryChange";
            // 
            // lblFixedMandatoryHwType
            // 
            resources.ApplyResources(lblFixedMandatoryHwType, "lblFixedMandatoryHwType");
            lblFixedMandatoryHwType.ForeColor = Color.Red;
            lblFixedMandatoryHwType.Name = "lblFixedMandatoryHwType";
            // 
            // lblFixedMandatoryInUse
            // 
            resources.ApplyResources(lblFixedMandatoryInUse, "lblFixedMandatoryInUse");
            lblFixedMandatoryInUse.ForeColor = Color.Red;
            lblFixedMandatoryInUse.Name = "lblFixedMandatoryInUse";
            // 
            // lblFixedMandatoryBuilding
            // 
            resources.ApplyResources(lblFixedMandatoryBuilding, "lblFixedMandatoryBuilding");
            lblFixedMandatoryBuilding.ForeColor = Color.Red;
            lblFixedMandatoryBuilding.Name = "lblFixedMandatoryBuilding";
            // 
            // lblFixedMandatoryRoomNumber
            // 
            resources.ApplyResources(lblFixedMandatoryRoomNumber, "lblFixedMandatoryRoomNumber");
            lblFixedMandatoryRoomNumber.ForeColor = Color.Red;
            lblFixedMandatoryRoomNumber.Name = "lblFixedMandatoryRoomNumber";
            // 
            // lblFixedMandatoryAssetNumber
            // 
            resources.ApplyResources(lblFixedMandatoryAssetNumber, "lblFixedMandatoryAssetNumber");
            lblFixedMandatoryAssetNumber.ForeColor = Color.Red;
            lblFixedMandatoryAssetNumber.Name = "lblFixedMandatoryAssetNumber";
            // 
            // lblFixedMandatoryMain
            // 
            resources.ApplyResources(lblFixedMandatoryMain, "lblFixedMandatoryMain");
            lblFixedMandatoryMain.ForeColor = Color.Red;
            lblFixedMandatoryMain.Name = "lblFixedMandatoryMain";
            // 
            // iconImgRoomLetter
            // 
            resources.ApplyResources(iconImgRoomLetter, "iconImgRoomLetter");
            iconImgRoomLetter.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgRoomLetter.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgRoomLetter.Name = "iconImgRoomLetter";
            iconImgRoomLetter.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgRoomLetter.TabStop = false;
            // 
            // iconImgHwType
            // 
            resources.ApplyResources(iconImgHwType, "iconImgHwType");
            iconImgHwType.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgHwType.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgHwType.Name = "iconImgHwType";
            iconImgHwType.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgHwType.TabStop = false;
            // 
            // iconImgTag
            // 
            resources.ApplyResources(iconImgTag, "iconImgTag");
            iconImgTag.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgTag.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgTag.Name = "iconImgTag";
            iconImgTag.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgTag.TabStop = false;
            // 
            // iconImgInUse
            // 
            resources.ApplyResources(iconImgInUse, "iconImgInUse");
            iconImgInUse.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgInUse.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgInUse.Name = "iconImgInUse";
            iconImgInUse.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgInUse.TabStop = false;
            // 
            // iconImgServiceDate
            // 
            resources.ApplyResources(iconImgServiceDate, "iconImgServiceDate");
            iconImgServiceDate.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgServiceDate.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgServiceDate.Name = "iconImgServiceDate";
            iconImgServiceDate.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgServiceDate.TabStop = false;
            // 
            // iconImgStandard
            // 
            resources.ApplyResources(iconImgStandard, "iconImgStandard");
            iconImgStandard.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgStandard.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgStandard.Name = "iconImgStandard";
            iconImgStandard.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgStandard.TabStop = false;
            // 
            // iconImgAdRegistered
            // 
            resources.ApplyResources(iconImgAdRegistered, "iconImgAdRegistered");
            iconImgAdRegistered.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgAdRegistered.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgAdRegistered.Name = "iconImgAdRegistered";
            iconImgAdRegistered.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgAdRegistered.TabStop = false;
            // 
            // iconImgBuilding
            // 
            resources.ApplyResources(iconImgBuilding, "iconImgBuilding");
            iconImgBuilding.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgBuilding.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgBuilding.Name = "iconImgBuilding";
            iconImgBuilding.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgBuilding.TabStop = false;
            // 
            // iconImgRoomNumber
            // 
            resources.ApplyResources(iconImgRoomNumber, "iconImgRoomNumber");
            iconImgRoomNumber.CompositingQuality = null;
            iconImgRoomNumber.InterpolationMode = null;
            iconImgRoomNumber.Name = "iconImgRoomNumber";
            iconImgRoomNumber.SmoothingMode = null;
            iconImgRoomNumber.TabStop = false;
            // 
            // iconImgSealNumber
            // 
            resources.ApplyResources(iconImgSealNumber, "iconImgSealNumber");
            iconImgSealNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgSealNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgSealNumber.Name = "iconImgSealNumber";
            iconImgSealNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgSealNumber.TabStop = false;
            // 
            // iconImgAssetNumber
            // 
            resources.ApplyResources(iconImgAssetNumber, "iconImgAssetNumber");
            iconImgAssetNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            iconImgAssetNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgAssetNumber.Name = "iconImgAssetNumber";
            iconImgAssetNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgAssetNumber.TabStop = false;
            // 
            // dateTimePickerServiceDate
            // 
            resources.ApplyResources(dateTimePickerServiceDate, "dateTimePickerServiceDate");
            dateTimePickerServiceDate.CalendarTitleForeColor = SystemColors.ControlLightLight;
            dateTimePickerServiceDate.Name = "dateTimePickerServiceDate";
            // 
            // lblFixedAdRegistered
            // 
            resources.ApplyResources(lblFixedAdRegistered, "lblFixedAdRegistered");
            lblFixedAdRegistered.ForeColor = SystemColors.ControlText;
            lblFixedAdRegistered.Name = "lblFixedAdRegistered";
            // 
            // lblFixedStandard
            // 
            resources.ApplyResources(lblFixedStandard, "lblFixedStandard");
            lblFixedStandard.ForeColor = SystemColors.ControlText;
            lblFixedStandard.Name = "lblFixedStandard";
            // 
            // groupBoxServiceType
            // 
            resources.ApplyResources(groupBoxServiceType, "groupBoxServiceType");
            groupBoxServiceType.Controls.Add(textBoxInactiveUpdateDataRadio);
            groupBoxServiceType.Controls.Add(radioButtonUpdateData);
            groupBoxServiceType.Controls.Add(lblFixedMandatoryServiceType);
            groupBoxServiceType.Controls.Add(textBoxInactiveFormattingRadio);
            groupBoxServiceType.Controls.Add(textBoxInactiveMaintenanceRadio);
            groupBoxServiceType.Controls.Add(radioButtonFormatting);
            groupBoxServiceType.Controls.Add(radioButtonMaintenance);
            groupBoxServiceType.ForeColor = SystemColors.ControlText;
            groupBoxServiceType.Name = "groupBoxServiceType";
            groupBoxServiceType.TabStop = false;
            // 
            // textBoxInactiveUpdateDataRadio
            // 
            resources.ApplyResources(textBoxInactiveUpdateDataRadio, "textBoxInactiveUpdateDataRadio");
            textBoxInactiveUpdateDataRadio.BackColor = SystemColors.Control;
            textBoxInactiveUpdateDataRadio.BorderStyle = BorderStyle.None;
            textBoxInactiveUpdateDataRadio.ForeColor = SystemColors.WindowText;
            textBoxInactiveUpdateDataRadio.Name = "textBoxInactiveUpdateDataRadio";
            textBoxInactiveUpdateDataRadio.ReadOnly = true;
            // 
            // radioButtonUpdateData
            // 
            resources.ApplyResources(radioButtonUpdateData, "radioButtonUpdateData");
            radioButtonUpdateData.ForeColor = SystemColors.ControlText;
            radioButtonUpdateData.Name = "radioButtonUpdateData";
            radioButtonUpdateData.UseVisualStyleBackColor = true;
            radioButtonUpdateData.CheckedChanged += new System.EventHandler(RadioButtonUpdateData_CheckedChanged);
            // 
            // lblFixedMandatoryServiceType
            // 
            resources.ApplyResources(lblFixedMandatoryServiceType, "lblFixedMandatoryServiceType");
            lblFixedMandatoryServiceType.ForeColor = Color.Red;
            lblFixedMandatoryServiceType.Name = "lblFixedMandatoryServiceType";
            // 
            // textBoxInactiveFormattingRadio
            // 
            resources.ApplyResources(textBoxInactiveFormattingRadio, "textBoxInactiveFormattingRadio");
            textBoxInactiveFormattingRadio.BackColor = SystemColors.Control;
            textBoxInactiveFormattingRadio.BorderStyle = BorderStyle.None;
            textBoxInactiveFormattingRadio.ForeColor = SystemColors.WindowText;
            textBoxInactiveFormattingRadio.Name = "textBoxInactiveFormattingRadio";
            textBoxInactiveFormattingRadio.ReadOnly = true;
            // 
            // textBoxInactiveMaintenanceRadio
            // 
            resources.ApplyResources(textBoxInactiveMaintenanceRadio, "textBoxInactiveMaintenanceRadio");
            textBoxInactiveMaintenanceRadio.BackColor = SystemColors.Control;
            textBoxInactiveMaintenanceRadio.BorderStyle = BorderStyle.None;
            textBoxInactiveMaintenanceRadio.ForeColor = SystemColors.WindowText;
            textBoxInactiveMaintenanceRadio.Name = "textBoxInactiveMaintenanceRadio";
            textBoxInactiveMaintenanceRadio.ReadOnly = true;
            // 
            // radioButtonFormatting
            // 
            resources.ApplyResources(radioButtonFormatting, "radioButtonFormatting");
            radioButtonFormatting.ForeColor = SystemColors.ControlText;
            radioButtonFormatting.Name = "radioButtonFormatting";
            radioButtonFormatting.UseVisualStyleBackColor = true;
            radioButtonFormatting.CheckedChanged += new System.EventHandler(RadioButtonFormatting_CheckedChanged);
            // 
            // radioButtonMaintenance
            // 
            resources.ApplyResources(radioButtonMaintenance, "radioButtonMaintenance");
            radioButtonMaintenance.ForeColor = SystemColors.ControlText;
            radioButtonMaintenance.Name = "radioButtonMaintenance";
            radioButtonMaintenance.UseVisualStyleBackColor = true;
            radioButtonMaintenance.CheckedChanged += new System.EventHandler(RadioButtonMaintenance_CheckedChanged);
            // 
            // loadingCircleLastService
            // 
            resources.ApplyResources(loadingCircleLastService, "loadingCircleLastService");
            loadingCircleLastService.Active = false;
            loadingCircleLastService.Color = Color.LightSlateGray;
            loadingCircleLastService.InnerCircleRadius = 5;
            loadingCircleLastService.Name = "loadingCircleLastService";
            loadingCircleLastService.NumberSpoke = 12;
            loadingCircleLastService.OuterCircleRadius = 11;
            loadingCircleLastService.RotationSpeed = 1;
            loadingCircleLastService.SpokeThickness = 2;
            loadingCircleLastService.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // lblColorLastService
            // 
            resources.ApplyResources(lblColorLastService, "lblColorLastService");
            lblColorLastService.ForeColor = SystemColors.MenuHighlight;
            lblColorLastService.Name = "lblColorLastService";
            // 
            // lblAgentName
            // 
            resources.ApplyResources(lblAgentName, "lblAgentName");
            lblAgentName.ForeColor = SystemColors.ControlText;
            lblAgentName.Name = "lblAgentName";
            // 
            // lblFixedAgentName
            // 
            resources.ApplyResources(lblFixedAgentName, "lblFixedAgentName");
            lblFixedAgentName.ForeColor = SystemColors.ControlText;
            lblFixedAgentName.Name = "lblFixedAgentName";
            // 
            // lblServerPort
            // 
            resources.ApplyResources(lblServerPort, "lblServerPort");
            lblServerPort.ForeColor = SystemColors.ControlText;
            lblServerPort.Name = "lblServerPort";
            // 
            // lblServerIP
            // 
            resources.ApplyResources(lblServerIP, "lblServerIP");
            lblServerIP.ForeColor = SystemColors.ControlText;
            lblServerIP.Name = "lblServerIP";
            // 
            // lblFixedServerIP
            // 
            resources.ApplyResources(lblFixedServerIP, "lblFixedServerIP");
            lblFixedServerIP.ForeColor = SystemColors.ControlText;
            lblFixedServerIP.Name = "lblFixedServerIP";
            // 
            // lblColorServerOperationalStatus
            // 
            resources.ApplyResources(lblColorServerOperationalStatus, "lblColorServerOperationalStatus");
            lblColorServerOperationalStatus.BackColor = Color.Transparent;
            lblColorServerOperationalStatus.ForeColor = Color.Silver;
            lblColorServerOperationalStatus.Name = "lblColorServerOperationalStatus";
            // 
            // toolStripVersionText
            // 
            resources.ApplyResources(toolStripVersionText, "toolStripVersionText");
            toolStripVersionText.BorderSides = ToolStripStatusLabelBorderSides.Left;
            toolStripVersionText.BorderStyle = Border3DStyle.SunkenOuter;
            toolStripVersionText.ForeColor = SystemColors.ControlText;
            toolStripVersionText.Name = "toolStripVersionText";
            // 
            // statusStrip1
            // 
            resources.ApplyResources(statusStrip1, "statusStrip1");
            statusStrip1.BackColor = SystemColors.Control;
            statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            comboBoxThemeButton,
            logLabelButton,
            aboutLabelButton,
            toolStripStatusBarText,
            toolStripVersionText});
            statusStrip1.Name = "statusStrip1";
            statusStrip1.RenderMode = ToolStripRenderMode.Professional;
            // 
            // comboBoxThemeButton
            // 
            resources.ApplyResources(comboBoxThemeButton, "comboBoxThemeButton");
            comboBoxThemeButton.BackColor = SystemColors.Control;
            comboBoxThemeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            comboBoxThemeButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripAutoTheme,
            toolStripLightTheme,
            toolStripDarkTheme});
            comboBoxThemeButton.ForeColor = SystemColors.ControlText;
            comboBoxThemeButton.Name = "comboBoxThemeButton";
            // 
            // toolStripAutoTheme
            // 
            resources.ApplyResources(toolStripAutoTheme, "toolStripAutoTheme");
            toolStripAutoTheme.BackColor = SystemColors.Control;
            toolStripAutoTheme.ForeColor = SystemColors.ControlText;
            toolStripAutoTheme.Name = "toolStripAutoTheme";
            toolStripAutoTheme.Click += new System.EventHandler(ToolStripMenuAutoTheme_Click);
            // 
            // toolStripLightTheme
            // 
            resources.ApplyResources(toolStripLightTheme, "toolStripLightTheme");
            toolStripLightTheme.BackColor = SystemColors.Control;
            toolStripLightTheme.ForeColor = SystemColors.ControlText;
            toolStripLightTheme.Name = "toolStripLightTheme";
            toolStripLightTheme.Click += new System.EventHandler(ToolStripMenuLightTheme_Click);
            // 
            // toolStripDarkTheme
            // 
            resources.ApplyResources(toolStripDarkTheme, "toolStripDarkTheme");
            toolStripDarkTheme.BackColor = SystemColors.Control;
            toolStripDarkTheme.ForeColor = SystemColors.ControlText;
            toolStripDarkTheme.Name = "toolStripDarkTheme";
            toolStripDarkTheme.Click += new System.EventHandler(ToolStripMenuDarkTheme_Click);
            // 
            // logLabelButton
            // 
            resources.ApplyResources(logLabelButton, "logLabelButton");
            logLabelButton.BackColor = SystemColors.Control;
            logLabelButton.BorderSides = ToolStripStatusLabelBorderSides.Left;
            logLabelButton.BorderStyle = Border3DStyle.SunkenOuter;
            logLabelButton.ForeColor = SystemColors.ControlText;
            logLabelButton.Name = "logLabelButton";
            logLabelButton.Click += new System.EventHandler(LogLabelButton_Click);
            logLabelButton.MouseEnter += new System.EventHandler(LogLabel_MouseEnter);
            logLabelButton.MouseLeave += new System.EventHandler(LogLabel_MouseLeave);
            // 
            // aboutLabelButton
            // 
            resources.ApplyResources(aboutLabelButton, "aboutLabelButton");
            aboutLabelButton.BorderSides = ToolStripStatusLabelBorderSides.Left;
            aboutLabelButton.BorderStyle = Border3DStyle.SunkenOuter;
            aboutLabelButton.ForeColor = SystemColors.ControlText;
            aboutLabelButton.Name = "aboutLabelButton";
            aboutLabelButton.Click += new System.EventHandler(AboutLabelButton_Click);
            aboutLabelButton.MouseEnter += new System.EventHandler(AboutLabel_MouseEnter);
            aboutLabelButton.MouseLeave += new System.EventHandler(AboutLabel_MouseLeave);
            // 
            // toolStripStatusBarText
            // 
            resources.ApplyResources(toolStripStatusBarText, "toolStripStatusBarText");
            toolStripStatusBarText.BackColor = SystemColors.Control;
            toolStripStatusBarText.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
            toolStripStatusBarText.BorderStyle = Border3DStyle.SunkenOuter;
            toolStripStatusBarText.ForeColor = SystemColors.ControlText;
            toolStripStatusBarText.Name = "toolStripStatusBarText";
            toolStripStatusBarText.Spring = true;
            // 
            // timerAlertHostname
            // 
            timerAlertHostname.Interval = 500;
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
            resources.ApplyResources(loadingCircleCollectButton, "loadingCircleCollectButton");
            loadingCircleCollectButton.Active = false;
            loadingCircleCollectButton.BackColor = Color.Transparent;
            loadingCircleCollectButton.Color = Color.LightSlateGray;
            loadingCircleCollectButton.ForeColor = SystemColors.ControlText;
            loadingCircleCollectButton.InnerCircleRadius = 5;
            loadingCircleCollectButton.Name = "loadingCircleCollectButton";
            loadingCircleCollectButton.NumberSpoke = 12;
            loadingCircleCollectButton.OuterCircleRadius = 11;
            loadingCircleCollectButton.RotationSpeed = 1;
            loadingCircleCollectButton.SpokeThickness = 2;
            loadingCircleCollectButton.StylePreset = LoadingCircle.StylePresets.MacOSX;
            loadingCircleCollectButton.UseWaitCursor = true;
            // 
            // loadingCircleRegisterButton
            // 
            resources.ApplyResources(loadingCircleRegisterButton, "loadingCircleRegisterButton");
            loadingCircleRegisterButton.Active = false;
            loadingCircleRegisterButton.BackColor = Color.Transparent;
            loadingCircleRegisterButton.Color = Color.LightSlateGray;
            loadingCircleRegisterButton.ForeColor = SystemColors.ControlText;
            loadingCircleRegisterButton.InnerCircleRadius = 5;
            loadingCircleRegisterButton.Name = "loadingCircleRegisterButton";
            loadingCircleRegisterButton.NumberSpoke = 12;
            loadingCircleRegisterButton.OuterCircleRadius = 11;
            loadingCircleRegisterButton.RotationSpeed = 1;
            loadingCircleRegisterButton.SpokeThickness = 2;
            loadingCircleRegisterButton.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // groupBoxServerStatus
            // 
            resources.ApplyResources(groupBoxServerStatus, "groupBoxServerStatus");
            groupBoxServerStatus.Controls.Add(loadingCircleServerOperationalStatus);
            groupBoxServerStatus.Controls.Add(lblFixedServerIP);
            groupBoxServerStatus.Controls.Add(lblFixedServerOperationalStatus);
            groupBoxServerStatus.Controls.Add(lblFixedServerPort);
            groupBoxServerStatus.Controls.Add(lblColorServerOperationalStatus);
            groupBoxServerStatus.Controls.Add(lblServerIP);
            groupBoxServerStatus.Controls.Add(lblServerPort);
            groupBoxServerStatus.Controls.Add(lblFixedAgentName);
            groupBoxServerStatus.Controls.Add(lblAgentName);
            groupBoxServerStatus.ForeColor = SystemColors.ControlText;
            groupBoxServerStatus.Name = "groupBoxServerStatus";
            groupBoxServerStatus.TabStop = false;
            // 
            // loadingCircleServerOperationalStatus
            // 
            resources.ApplyResources(loadingCircleServerOperationalStatus, "loadingCircleServerOperationalStatus");
            loadingCircleServerOperationalStatus.Active = false;
            loadingCircleServerOperationalStatus.Color = Color.LightSlateGray;
            loadingCircleServerOperationalStatus.InnerCircleRadius = 5;
            loadingCircleServerOperationalStatus.Name = "loadingCircleServerOperationalStatus";
            loadingCircleServerOperationalStatus.NumberSpoke = 12;
            loadingCircleServerOperationalStatus.OuterCircleRadius = 11;
            loadingCircleServerOperationalStatus.RotationSpeed = 1;
            loadingCircleServerOperationalStatus.SpokeThickness = 2;
            loadingCircleServerOperationalStatus.StylePreset = LoadingCircle.StylePresets.MacOSX;
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
            // groupBoxTableMaintenances
            // 
            resources.ApplyResources(groupBoxTableMaintenances, "groupBoxTableMaintenances");
            groupBoxTableMaintenances.Controls.Add(loadingCircleTableMaintenances);
            groupBoxTableMaintenances.Controls.Add(tableMaintenances);
            groupBoxTableMaintenances.Controls.Add(lblThereIsNothingHere);
            groupBoxTableMaintenances.ForeColor = SystemColors.ControlText;
            groupBoxTableMaintenances.Name = "groupBoxTableMaintenances";
            groupBoxTableMaintenances.TabStop = false;
            // 
            // loadingCircleTableMaintenances
            // 
            resources.ApplyResources(loadingCircleTableMaintenances, "loadingCircleTableMaintenances");
            loadingCircleTableMaintenances.Active = false;
            loadingCircleTableMaintenances.Color = Color.LightSlateGray;
            loadingCircleTableMaintenances.InnerCircleRadius = 5;
            loadingCircleTableMaintenances.Name = "loadingCircleTableMaintenances";
            loadingCircleTableMaintenances.NumberSpoke = 12;
            loadingCircleTableMaintenances.OuterCircleRadius = 11;
            loadingCircleTableMaintenances.RotationSpeed = 1;
            loadingCircleTableMaintenances.SpokeThickness = 2;
            loadingCircleTableMaintenances.StylePreset = LoadingCircle.StylePresets.MacOSX;
            // 
            // tableMaintenances
            // 
            resources.ApplyResources(tableMaintenances, "tableMaintenances");
            tableMaintenances.AllowUserToAddRows = false;
            tableMaintenances.AllowUserToDeleteRows = false;
            tableMaintenances.AllowUserToResizeRows = false;
            tableMaintenances.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tableMaintenances.BackgroundColor = SystemColors.Control;
            tableMaintenances.BorderStyle = BorderStyle.None;
            tableMaintenances.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            tableMaintenances.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            tableMaintenances.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            tableMaintenances.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            serviceDate,
            serviceType,
            agentUsername});
            tableMaintenances.EnableHeadersVisualStyles = false;
            tableMaintenances.Name = "tableMaintenances";
            tableMaintenances.ReadOnly = true;
            tableMaintenances.RowHeadersVisible = false;
            tableMaintenances.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            tableMaintenances.SelectionMode = DataGridViewSelectionMode.CellSelect;
            // 
            // serviceDate
            // 
            serviceDate.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(serviceDate, "serviceDate");
            serviceDate.Name = "serviceDate";
            serviceDate.ReadOnly = true;
            // 
            // serviceType
            // 
            serviceType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(serviceType, "serviceType");
            serviceType.Name = "serviceType";
            serviceType.ReadOnly = true;
            // 
            // agentUsername
            // 
            agentUsername.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(agentUsername, "agentUsername");
            agentUsername.Name = "agentUsername";
            agentUsername.ReadOnly = true;
            // 
            // lblThereIsNothingHere
            // 
            resources.ApplyResources(lblThereIsNothingHere, "lblThereIsNothingHere");
            lblThereIsNothingHere.ForeColor = SystemColors.ControlLight;
            lblThereIsNothingHere.Name = "lblThereIsNothingHere";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.Control;
            Controls.Add(groupBoxTableMaintenances);
            Controls.Add(loadingCircleRegisterButton);
            Controls.Add(loadingCircleCollectButton);
            Controls.Add(loadingCircleLastService);
            Controls.Add(lblColorLastService);
            Controls.Add(groupBoxServerStatus);
            Controls.Add(groupBoxAssetData);
            Controls.Add(groupBoxHwData);
            Controls.Add(imgTopBanner);
            Controls.Add(apcsButton);
            Controls.Add(collectButton);
            Controls.Add(statusStrip1);
            Controls.Add(registerButton);
            Controls.Add(groupBoxServiceType);
            FormBorderStyle = FormBorderStyle.FixedDialog;
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
            ((System.ComponentModel.ISupportInitialize)iconImgHostname).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgOperatingSystem).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgVideoCard).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgMediaOperationMode).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgStorageType).EndInit();
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
            ((System.ComponentModel.ISupportInitialize)imgTopBanner).EndInit();
            groupBoxServerStatus.ResumeLayout(false);
            groupBoxServerStatus.PerformLayout();
            groupBoxTableMaintenances.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tableMaintenances).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }
    }
}

