using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using AssetInformationAndRegistration.Properties;
using ConstantsDLL;
using ConstantsDLL.Properties;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using Octokit;
using RestApiDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

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
        private List<List<string>> videoCardDetail, storageDetail, ramDetail, processorDetail;
        private List<string> smartValueList;

        private HttpStatusCode finalHttpCode;
        private readonly HttpClient client;
        private readonly LogGenerator log;
        private readonly GitHubClient ghc;
        private readonly UserPreferenceChangedEventHandler UserPreferenceChanged;
        private readonly BackgroundWorker backgroundWorker1;

        private readonly Program.ConfigurationOptions configOptions;
        private Regex hostnamePattern;
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
        private readonly ProcessorDetailForm processorForm;
        private readonly RamDetailForm ramForm;
        private readonly StorageDetailForm storageForm;
        private readonly VideoCardDetailForm videoCardForm;
        private readonly HardwareChangeDetailForm hardwareChangeForm;

        /// <summary>
        /// Main form constructor
        /// </summary>
        /// <param name="client">HTTP client object</param>
        /// <param name="ghc">GitHub client object</param>
        /// <param name="log">Log file object</param>
        /// <param name="configOptions">Config file object</param>
        /// <param name="agent">Agent object</param>
        /// <param name="serverIP">Server IP address</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="offlineMode">Offline mode set</param>
        /// <param name="isSystemDarkModeEnabled">Theme mode</param>
        internal MainForm(HttpClient client, GitHubClient ghc, LogGenerator log, Program.ConfigurationOptions configOptions, Agent agent, string serverIP, string serverPort, bool offlineMode, bool isSystemDarkModeEnabled)
        {
            InitializeComponent();
            processorForm = new ProcessorDetailForm(log);
            ramForm = new RamDetailForm(log);
            storageForm = new StorageDetailForm(log);
            videoCardForm = new VideoCardDetailForm(log);
            hardwareChangeForm = new HardwareChangeDetailForm(log);
            processorForm.Hide();
            ramForm.Hide();
            storageForm.Hide();
            videoCardForm.Hide();
            hardwareChangeForm.Hide();

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
                    Misc.MiscMethods.LightThemeAllControls(processorForm);
                    processorForm.LightThemeSpecificControls();
                    Misc.MiscMethods.LightThemeAllControls(ramForm);
                    ramForm.LightThemeSpecificControls();
                    Misc.MiscMethods.LightThemeAllControls(storageForm);
                    storageForm.LightThemeSpecificControls();
                    Misc.MiscMethods.LightThemeAllControls(videoCardForm);
                    videoCardForm.LightThemeSpecificControls();
                    Misc.MiscMethods.LightThemeAllControls(hardwareChangeForm);
                    hardwareChangeForm.LightThemeSpecificControls();
                    if (themeEditable == false)
                    {
                        isSystemDarkModeEnabled = false;
                        comboBoxThemeButton.Enabled = false;
                    }
                    break;
                case 1:
                    Misc.MiscMethods.DarkThemeAllControls(this);
                    DarkThemeSpecificControls();
                    Misc.MiscMethods.DarkThemeAllControls(processorForm);
                    processorForm.DarkThemeSpecificControls();
                    Misc.MiscMethods.DarkThemeAllControls(ramForm);
                    ramForm.DarkThemeSpecificControls();
                    Misc.MiscMethods.DarkThemeAllControls(storageForm);
                    storageForm.DarkThemeSpecificControls();
                    Misc.MiscMethods.DarkThemeAllControls(videoCardForm);
                    videoCardForm.DarkThemeSpecificControls();
                    Misc.MiscMethods.DarkThemeAllControls(hardwareChangeForm);
                    hardwareChangeForm.DarkThemeSpecificControls();
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
            toolStripVersionText.Text = Misc.MiscMethods.Version(GenericResources.DEV_STATUS_BETA); //Debug/Beta version
#else
            toolStripVersionText.Text = Misc.MiscMethods.Version(); //Release/Final version
#endif

            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OFFLINE_MODE, offlineMode.ToString(), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

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

            //Attributes fetching data to an object
            serverParam = new ServerParam();
            if (!offlineMode)
            {
                try
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_SERVER_PARAMETERS, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                    //Fetch building and hw types info from the specified server
                    serverParam = await ParameterHandler.GetParameterAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_PARAMETERS_URL);
                }
                catch (InvalidRestApiCallException ex)
                {
                    //Shows a message about an error in the APCS web service
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    _ = MessageBox.Show(UIStrings.SERVER_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Misc.MiscMethods.CloseProgram(log, Convert.ToInt32(ExitCodes.ERROR), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                catch (InvalidAgentException ex)
                {
                    //Shows a message about an error of the agent credentials
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                    _ = MessageBox.Show(UIStrings.INVALID_CREDENTIALS, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Misc.MiscMethods.CloseProgram(log, Convert.ToInt32(ExitCodes.ERROR), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                catch (HttpRequestException ex)
                {
                    //Shows a message about an error with APCS connection
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                    _ = MessageBox.Show(UIStrings.INTRANET_REQUIRED, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Misc.MiscMethods.CloseProgram(log, Convert.ToInt32(ExitCodes.ERROR), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
            }
            else
            {
                //Fetch building and hw types info from the local file
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_SERVER_PARAMETERS, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                serverParam = ParameterHandler.GetOfflineModeConfigFile();
            }

            comboBoxBuilding.Items.AddRange(serverParam.Parameters.Buildings.ToArray());
            comboBoxHwType.Items.AddRange(serverParam.Parameters.HardwareTypes.ToArray());
            hostnamePattern = new Regex(serverParam.Parameters.HostnamePattern);
            textBoxAssetNumber.MaxLength = serverParam.Parameters.AssetNumberDigitLimit;
            textBoxSealNumber.MaxLength = serverParam.Parameters.SealNumberDigitLimit;
            textBoxRoomNumber.MaxLength = serverParam.Parameters.RoomNumberDigitLimit;
            textBoxTicketNumber.MaxLength = serverParam.Parameters.TicketNumberDigitLimit;

            //Fills controls with provided info from ini file and constants dll
            comboBoxActiveDirectory.Items.AddRange(StringsAndConstants.LIST_ACTIVE_DIRECTORY_GUI.ToArray());
            comboBoxStandard.Items.AddRange(StringsAndConstants.LIST_STANDARD_GUI.ToArray());
            comboBoxInUse.Items.AddRange(StringsAndConstants.LIST_IN_USE_GUI.ToArray());
            comboBoxTag.Items.AddRange(StringsAndConstants.LIST_TAG_GUI.ToArray());
            comboBoxBatteryChange.Items.AddRange(StringsAndConstants.LIST_BATTERY_GUI.ToArray());
            //if (HardwareInfo.GetHostname().Substring(0, 3).ToUpper().Equals(GenericResources.HOSTNAME_PATTERN))
            //    textBoxAssetNumber.Text = HardwareInfo.GetHostname().Substring(3);
            //else
            //    textBoxAssetNumber.Text = string.Empty;


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

            timerAlertHostname.Interval = Convert.ToInt32(GenericResources.ALERT_TIMER_INTERVAL);
            timerAlertMediaOperationMode.Interval = Convert.ToInt32(GenericResources.ALERT_TIMER_INTERVAL);
            timerAlertSecureBoot.Interval = Convert.ToInt32(GenericResources.ALERT_TIMER_INTERVAL);
            timerAlertFwVersion.Interval = Convert.ToInt32(GenericResources.ALERT_TIMER_INTERVAL);
            timerAlertNetConnectivity.Interval = Convert.ToInt32(GenericResources.ALERT_TIMER_INTERVAL);
            timerAlertFwType.Interval = Convert.ToInt32(GenericResources.ALERT_TIMER_INTERVAL);
            timerAlertVirtualizationTechnology.Interval = Convert.ToInt32(GenericResources.ALERT_TIMER_INTERVAL);
            timerAlertSmartStatus.Interval = Convert.ToInt32(GenericResources.ALERT_TIMER_INTERVAL);
            timerAlertTpmVersion.Interval = Convert.ToInt32(GenericResources.ALERT_TIMER_INTERVAL);
            timerAlertRamAmount.Interval = Convert.ToInt32(GenericResources.ALERT_TIMER_INTERVAL);
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
                comboBoxActiveDirectory.SelectedIndex = 1;
            }
            catch
            {
                comboBoxActiveDirectory.SelectedIndex = 0;
            }
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_AD_REGISTERED, comboBoxActiveDirectory.SelectedItem.ToString(), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

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
            Misc.MiscMethods.CloseProgram(log, Convert.ToInt32(ExitCodes.SUCCESS), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
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
            lblNoticeHardwareChanged.Visible = false;
            hardwareChangeButton.Visible = false;
            if (!offlineMode)
                lblThereIsNothingHere.Visible = false;
            lblNoticeHardwareChanged.MouseHover += new EventHandler(HwUidLabel_MouseHover);
            tableMaintenances.Visible = false;
            tableMaintenances.Rows.Clear();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_START_COLLECT_THREAD, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            StartAsync(sender, e);
        }

        /// <summary> 
        /// Prepares for the collection process
        /// </summary>
        private async void CollectPreparation()
        {
            #region Writes a dash in the labels, while scanning the hardware
            lblColorLastService.Text = GenericResources.DASH_SINGLE;
            lblBrand.Text = GenericResources.DASH_SINGLE;
            lblModel.Text = GenericResources.DASH_SINGLE;
            lblSerialNumber.Text = GenericResources.DASH_SINGLE;
            lblProcessor.Text = GenericResources.DASH_SINGLE;
            lblRam.Text = GenericResources.DASH_SINGLE;
            lblColorCompliant.Text = GenericResources.DASH_SINGLE;
            lblStorageType.Text = GenericResources.DASH_SINGLE;
            lblMediaOperationMode.Text = GenericResources.DASH_SINGLE;
            lblVideoCard.Text = GenericResources.DASH_SINGLE;
            lblOperatingSystem.Text = GenericResources.DASH_SINGLE;
            lblHostname.Text = GenericResources.DASH_SINGLE;
            lblIpAddress.Text = GenericResources.DASH_SINGLE;
            lblFwVersion.Text = GenericResources.DASH_SINGLE;
            lblFwType.Text = GenericResources.DASH_SINGLE;
            lblSecureBoot.Text = GenericResources.DASH_SINGLE;
            lblVirtualizationTechnology.Text = GenericResources.DASH_SINGLE;
            lblTpmVersion.Text = GenericResources.DASH_SINGLE;
            collectButton.Text = GenericResources.DASH_SINGLE;
            lblColorServerOperationalStatus.Text = GenericResources.DASH_SINGLE;
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

                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PINGGING_SERVER, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                //Checks if server is alive
                serverOnline = await ModelHandler.CheckHost(client, GenericResources.HTTP + serverIP + ":" + serverPort);

                loadingCircleServerOperationalStatus.Visible = false;
                loadingCircleServerOperationalStatus.Active = false;

                if (serverOnline && serverPort != string.Empty)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_ONLINE_SERVER, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    lblColorServerOperationalStatus.Text = AirUIStrings.ONLINE;
                    lblColorServerOperationalStatus.ForeColor = StringsAndConstants.ONLINE_ALERT;
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OFFLINE_SERVER, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    lblColorServerOperationalStatus.Text = AirUIStrings.OFFLINE;
                    lblColorServerOperationalStatus.ForeColor = StringsAndConstants.OFFLINE_ALERT;

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    _ = MessageBox.Show(UIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                lblServerIP.Text = lblServerPort.Text = lblAgentName.Text = lblColorServerOperationalStatus.Text = lblColorLastService.Text = lblColorCompliant.Text = AirUIStrings.OFFLINE_MODE_ACTIVATED;
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
            try
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_START_COLLECTING, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                progressbarCount = 0;
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for PC maker
                newAsset.hardware.hwBrand = HardwareInfo.GetBrand();
                if (newAsset.hardware.hwBrand == GenericResources.TO_BE_FILLED_BY_OEM || newAsset.hardware.hwBrand == string.Empty)
                    newAsset.hardware.hwBrand = HardwareInfo.GetBrandAlt();
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_BRAND, newAsset.hardware.hwBrand, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for PC model
                newAsset.hardware.hwModel = HardwareInfo.GetModel();
                if (newAsset.hardware.hwModel == GenericResources.TO_BE_FILLED_BY_OEM || newAsset.hardware.hwModel == string.Empty)
                {
                    newAsset.hardware.hwModel = HardwareInfo.GetModelAlt();
                    if (newAsset.hardware.hwModel == GenericResources.TO_BE_FILLED_BY_OEM || newAsset.hardware.hwModel == string.Empty)
                        newAsset.hardware.hwModel = UIStrings.UNKNOWN;
                }
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_MODEL, newAsset.hardware.hwModel, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for motherboard Serial number
                newAsset.hardware.hwSerialNumber = HardwareInfo.GetSerialNumber();
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_SERIAL_NUMBER, newAsset.hardware.hwSerialNumber, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for CPU information
                processorDetail = HardwareInfo.GetProcessorDetails();
                newHardware.processor.Clear();
                for (int i = 0; i < processorDetail.Count; i++)
                {
                    processorDetail[i][1] = processorDetail[i][1].Replace("(R)", string.Empty);
                    processorDetail[i][1] = processorDetail[i][1].Replace("(TM)", string.Empty);
                    processorDetail[i][1] = processorDetail[i][1].Replace("(tm)", string.Empty);
                    processor p = new processor
                    {
                        procId = processorDetail[i][0],
                        procName = processorDetail[i][1],
                        procFrequency = processorDetail[i][2],
                        procNumberOfCores = processorDetail[i][3],
                        procNumberOfThreads = processorDetail[i][4],
                        procCache = processorDetail[i][5]
                    };
                    newHardware.processor.Add(p);
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_NAME + " [" + newHardware.processor[i].procId + "]", newHardware.processor[i].procName, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_FREQUENCY + " [" + newHardware.processor[i].procId + "]", newHardware.processor[i].procFrequency + " " + GenericResources.FREQUENCY_MHZ, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_CORES + " [" + newHardware.processor[i].procId + "]", newHardware.processor[i].procNumberOfCores, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_THREADS + " [" + newHardware.processor[i].procId + "]", newHardware.processor[i].procNumberOfThreads, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_CACHE + " [" + newHardware.processor[i].procId + "]", Misc.MiscMethods.FriendlySizeBinary(Convert.ToInt64(newHardware.processor[i].procCache), false), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                processorForm.TreatData(processorDetail);
                processorSummary = processorDetail[0][1];
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR, processorSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for RAM amount and total number of slots
                ramDetail = HardwareInfo.GetRamDetails();
                newHardware.ram.Clear();
                for (int i = 0; i < ramDetail.Count; i++)
                {
                    ram r = new ram
                    {
                        ramSlot = ramDetail[i][0],
                        ramAmount = ramDetail[i][1],
                        ramType = ramDetail[i][2],
                        ramFrequency = ramDetail[i][3],
                        ramManufacturer = ramDetail[i][4],
                        ramSerialNumber = ramDetail[i][5],
                        ramPartNumber = ramDetail[i][6]
                    };
                    newHardware.ram.Add(r);
                    try
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_TYPE + " [" + newHardware.ram[i].ramSlot + "]", Enum.GetName(typeof(HardwareInfo.RamTypes), Convert.ToInt32(newHardware.ram[i].ramType)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_AMOUNT + " [" + newHardware.ram[i].ramSlot + "]", Misc.MiscMethods.FriendlySizeBinary(Convert.ToInt64(newHardware.ram[i].ramAmount), false), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    }
                    catch
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_TYPE + " [" + newHardware.ram[i].ramSlot + "]", newHardware.ram[i].ramType, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_AMOUNT + " [" + newHardware.ram[i].ramSlot + "]", newHardware.ram[i].ramAmount, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    }
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_FREQUENCY + " [" + newHardware.ram[i].ramSlot + "]", newHardware.ram[i].ramFrequency + " " + GenericResources.FREQUENCY_MHZ, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_MANUFACTURER + " [" + newHardware.ram[i].ramSlot + "]", newHardware.ram[i].ramManufacturer, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_SERIAL_NUMBER + " [" + newHardware.ram[i].ramSlot + "]", newHardware.ram[i].ramSerialNumber, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_PART_NUMBER + " [" + newHardware.ram[i].ramSlot + "]", newHardware.ram[i].ramPartNumber, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                ramForm.TreatData(ramDetail, isSystemDarkModeEnabled);
                ramSummary = HardwareInfo.GetRamSummary();
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM, ramSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for Storage data
                storageDetail = HardwareInfo.GetStorageDeviceDetails();
                smartValueList = new List<string>();
                newHardware.storage.Clear();
                for (int i = 0; i < storageDetail.Count; i++)
                {
                    smartValueList.Add(storageDetail[i][6]);
                    storage s = new storage
                    {
                        storId = storageDetail[i][0],
                        storType = storageDetail[i][1],
                        storSize = storageDetail[i][2],
                        storConnection = storageDetail[i][3],
                        storModel = storageDetail[i][4],
                        storSerialNumber = storageDetail[i][5],
                        storSmartStatus = storageDetail[i][6]
                    };
                    newHardware.storage.Add(s);
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_TYPE + " [" + newHardware.storage[i].storId + "]", Enum.GetName(typeof(HardwareInfo.StorageTypes), Convert.ToInt32(newHardware.storage[i].storType)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_SIZE + " [" + newHardware.storage[i].storId + "]", Misc.MiscMethods.FriendlySizeDecimal(Convert.ToInt64(newHardware.storage[i].storSize), false), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_CONNECTION + " [" + newHardware.storage[i].storId + "]", Enum.GetName(typeof(HardwareInfo.StorageConnectionTypes), Convert.ToInt32(newHardware.storage[i].storConnection)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_MODEL + " [" + newHardware.storage[i].storId + "]", newHardware.storage[i].storModel, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_SERIAL_NUMBER + " [" + newHardware.storage[i].storId + "]", newHardware.storage[i].storSerialNumber, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_SMART_STATUS + " [" + newHardware.storage[i].storId + "]", Enum.GetName(typeof(HardwareInfo.SmartStates), Convert.ToInt32(newHardware.storage[i].storSmartStatus)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                storageForm.TreatData(storageDetail);
                storageSummary = HardwareInfo.GetStorageSummary();
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE, storageSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for Media Operation (IDE/AHCI/NVME)
                newAsset.firmware.fwMediaOperationMode = HardwareInfo.GetMediaOperationMode();
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_MEDIA_OPERATION_TYPE, Enum.GetName(typeof(HardwareInfo.MediaOperationTypes), Convert.ToInt32(newAsset.firmware.fwMediaOperationMode)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for Video Card information
                videoCardDetail = HardwareInfo.GetVideoCardDetails();
                newHardware.videoCard.Clear();
                for (int i = 0; i < videoCardDetail.Count; i++)
                {
                    videoCardDetail[i][1] = videoCardDetail[i][1].Replace("(R)", string.Empty);
                    videoCardDetail[i][1] = videoCardDetail[i][1].Replace("(TM)", string.Empty);
                    videoCardDetail[i][1] = videoCardDetail[i][1].Replace("(tm)", string.Empty);
                    videoCard v = new videoCard
                    {
                        vcId = videoCardDetail[i][0],
                        vcName = videoCardDetail[i][1],
                        vcRam = videoCardDetail[i][2],
                    };
                    newHardware.videoCard.Add(v);
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_VIDEO_CARD_NAME + " [" + newHardware.videoCard[i].vcId + "]", newHardware.videoCard[i].vcName, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_VIDEO_CARD_RAM + " [" + newHardware.videoCard[i].vcId + "]", Misc.MiscMethods.FriendlySizeBinary(Convert.ToInt64(newHardware.videoCard[i].vcRam), false), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                videoCardForm.TreatData(videoCardDetail);
                videoCardSummary = videoCardDetail[0][1];
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_VIDEO_CARD, videoCardSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for OS infomation
                newAsset.operatingSystem.osArch = HardwareInfo.GetOSArchBinary();
                newAsset.operatingSystem.osBuild = HardwareInfo.GetOSBuildAndRevision();
                newAsset.operatingSystem.osName = HardwareInfo.GetOSName();
                newAsset.operatingSystem.osVersion = HardwareInfo.GetOSVersion();
                operatingSystemSummary = HardwareInfo.GetOSSummary();
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OPERATING_SYSTEM, operatingSystemSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for Hostname
                newAsset.network.netHostname = HardwareInfo.GetHostname();
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_NETWORK_HOSTNAME, newAsset.network.netHostname, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for MAC Address
                newAsset.network.netMacAddress = HardwareInfo.GetMacAddress();
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_NETWORK_MAC_ADDRESS, newAsset.network.netMacAddress, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for IP Address
                newAsset.network.netIpAddress = HardwareInfo.GetIpAddress();
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_NETWORK_IP_ADDRESS, newAsset.network.netIpAddress, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for firmware type
                newAsset.firmware.fwType = HardwareInfo.GetFirmwareType();
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_TYPE, Enum.GetName(typeof(HardwareInfo.FirmwareTypes), Convert.ToInt32(newAsset.firmware.fwType)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for Secure Boot status
                newAsset.firmware.fwSecureBoot = HardwareInfo.GetSecureBoot();
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_SECURE_BOOT, StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.fwSecureBoot)], Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for firmware version
                newAsset.firmware.fwVersion = HardwareInfo.GetFirmwareVersion();
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_VERSION, newAsset.firmware.fwVersion, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for VT status
                newAsset.firmware.fwVirtualizationTechnology = HardwareInfo.GetVirtualizationTechnology();
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_VIRTUALIZATION_TECHNOLOGY, StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.fwVirtualizationTechnology)], Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Scans for TPM status
                newAsset.firmware.fwTpmVersion = HardwareInfo.GetTPMStatus();
                progressbarCount++;
                worker.ReportProgress(ProgressAuxFunction(progressbarCount));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_TPM, StringsAndConstants.LIST_TPM_TYPES[Convert.ToInt32(newAsset.firmware.fwTpmVersion)], Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_END_COLLECTING, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            }
            catch
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), LogStrings.LOG_COLLECTION_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(UIStrings.COLLECTION_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
            }
        }

        /// <summary> 
        /// Prints the collected data into the form labels, warning the agent when there are forbidden modes
        /// </summary>
        /// <returns>Returns a asynchronous task</returns>
        private async Task ProcessCollectedData()
        {
            Misc.MiscMethods.HideLoadingCircles(this);

            #region Prints fetched data into labels
            lblBrand.Text = newAsset.hardware.hwBrand;
            lblModel.Text = newAsset.hardware.hwModel;
            lblSerialNumber.Text = newAsset.hardware.hwSerialNumber;
            lblProcessor.Text = processorSummary;
            lblRam.Text = ramSummary;
            lblStorageType.Text = storageSummary;
            lblVideoCard.Text = videoCardSummary;
            lblOperatingSystem.Text = operatingSystemSummary;
            lblHostname.Text = newAsset.network.netHostname;
            lblIpAddress.Text = newAsset.network.netIpAddress;
            lblFwVersion.Text = newAsset.firmware.fwVersion;

            lblMediaOperationMode.Text = Enum.GetName(typeof(HardwareInfo.MediaOperationTypes), Convert.ToInt32(newAsset.firmware.fwMediaOperationMode));
            lblFwType.Text = Enum.GetName(typeof(HardwareInfo.FirmwareTypes), Convert.ToInt32(newAsset.firmware.fwType));
            lblSecureBoot.Text = StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.fwSecureBoot)];
            lblVirtualizationTechnology.Text = StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.fwVirtualizationTechnology)];
            lblTpmVersion.Text = StringsAndConstants.LIST_TPM_TYPES[Convert.ToInt32(newAsset.firmware.fwTpmVersion)];

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
                    try
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_MODEL_DATA, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        //Feches model info from server
                        modelTemplate = await ModelHandler.GetModelAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_MODEL_URL + lblModel.Text);
                    }
                    catch (InvalidRestApiCallException ex)
                    {
                        //Shows a message about an error in the APCS web service
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                        _ = MessageBox.Show(UIStrings.SERVER_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Misc.MiscMethods.CloseProgram(log, Convert.ToInt32(ExitCodes.ERROR), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    }
                    catch (InvalidAgentException ex)
                    {
                        //Shows a message about an error of the agent credentials
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                        _ = MessageBox.Show(UIStrings.INVALID_CREDENTIALS, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Misc.MiscMethods.CloseProgram(log, Convert.ToInt32(ExitCodes.ERROR), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    }
                    catch (UnregisteredModelException ex)
                    {
                        //Shows a message about a unregistered model
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        _ = MessageBox.Show(UIStrings.UNREGISTERED_MODEL, UIStrings.WARNING_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (HttpRequestException)
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                        _ = MessageBox.Show(UIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    try
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_ASSET_DATA, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                        existingAsset = null;

                        //Fetches existing asset data from server using a hrdware unique ID (hash calculated from Brand, Model, Serial Number and MAC Address)
                        newAsset.assetHash = Misc.MiscMethods.HardwareSha256UniqueId(newAsset);
                        existingAsset = await AssetHandler.GetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_ASSET_HASH_URL + newAsset.assetHash);

                        //If hardware signature changed, alerts the agent
                        newAsset.hwHash = Misc.MiscMethods.HardwareSha256Hash(newAsset);
                        if (existingAsset.hwHash != string.Empty && existingAsset.hwHash != newAsset.hwHash)
                        {
                            lblNoticeHardwareChanged.Visible = true;
                            hardwareChangeButton.Visible = true;
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), LogStrings.LOG_ASSET_HARDWARE_MODIFIED, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                            _ = MessageBox.Show(UIStrings.ASSET_HARDWARE_MODIFIED, UIStrings.WARNING_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        loadingCircleLastService.Visible = false;
                        loadingCircleLastService.Active = false;

                        radioButtonUpdateData.Enabled = true;
                        lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(existingAsset.maintenances[0].mainServiceDate);
                        lblColorLastService.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), lblColorLastService.Text, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SERVICES_MADE, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                        TableMaintenancesFiller();

                        tableMaintenances.Visible = true;
                    }
                    //If asset does not exist on the database
                    catch (UnregisteredAssetException ex)
                    {
                        loadingCircleLastService.Visible = false;
                        loadingCircleLastService.Active = false;

                        radioButtonUpdateData.Enabled = false;
                        lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(string.Empty);
                        lblColorLastService.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        lblThereIsNothingHere.Visible = true;
                    }
                    catch (InvalidRestApiCallException ex)
                    {
                        //Shows a message about an error in the APCS web service
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                        _ = MessageBox.Show(UIStrings.SERVER_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Misc.MiscMethods.CloseProgram(log, Convert.ToInt32(ExitCodes.ERROR), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    }
                    catch (InvalidAgentException ex)
                    {
                        //Shows a message about an error of the agent credentials
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                        _ = MessageBox.Show(UIStrings.INVALID_CREDENTIALS, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Misc.MiscMethods.CloseProgram(log, Convert.ToInt32(ExitCodes.ERROR), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    }
                    //If server is unreachable
                    catch (HttpRequestException)
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                        tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                        _ = MessageBox.Show(UIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    loadingCircleTableMaintenances.Visible = false;
                    loadingCircleTableMaintenances.Active = false;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If hostname is the default one and its enforcement is enabled
                if (configOptions.Enforcement.Hostname.ToString() == GenericResources.TRUE && !hostnamePattern.IsMatch(newAsset.network.netHostname) && !offlineMode)
                {
                    pass = false;
                    lblHostname.Text += " (" + AirUIStrings.ALERT_HOSTNAME + ")";
                    timerAlertHostname.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.ALERT_HOSTNAME, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, mediaOpMode enforcement is enabled, and the mode is incorrect
                if (configOptions.Enforcement.MediaOperationMode.ToString() == GenericResources.TRUE && modelTemplate != null && modelTemplate.mediaOperationMode != newAsset.firmware.fwMediaOperationMode && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblMediaOperationMode.Text += " (" + AirUIStrings.ALERT_MEDIA_OPERATION_TO + " " + Enum.GetName(typeof(HardwareInfo.MediaOperationTypes), Convert.ToInt32(modelTemplate.mediaOperationMode)) + ")";
                    timerAlertMediaOperationMode.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.ALERT_MEDIA_OPERATION_TO, Enum.GetName(typeof(HardwareInfo.MediaOperationTypes), Convert.ToInt32(modelTemplate.mediaOperationMode)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //The section below contains the exception cases for Secure Boot enforcement, if it is enabled
                if (configOptions.Enforcement.SecureBoot.ToString() == GenericResources.TRUE && StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.fwSecureBoot)] == UIStrings.DEACTIVATED && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblSecureBoot.Text += " (" + AirUIStrings.ALERT_SECURE_BOOT + ")";
                    timerAlertSecureBoot.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.ALERT_SECURE_BOOT, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, firmware version enforcement is enabled, and the version is incorrect
                if (configOptions.Enforcement.FirmwareVersion.ToString() == GenericResources.TRUE && modelTemplate != null && !newAsset.firmware.fwVersion.Contains(modelTemplate.fwVersion) && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblFwVersion.Text += " (" + AirUIStrings.ALERT_FIRMWARE_VERSION_TO + " " + modelTemplate.fwVersion + ")";
                    timerAlertFwVersion.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.ALERT_FIRMWARE_VERSION_TO, modelTemplate.fwVersion, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, firmware type enforcement is enabled, and the type is incorrect
                if (configOptions.Enforcement.FirmwareType.ToString() == GenericResources.TRUE && modelTemplate != null && modelTemplate.fwType != newAsset.firmware.fwType && !offlineMode)
                {
                    pass = false;
                    lblFwType.Text += " (" + AirUIStrings.ALERT_FIRMWARE_TYPE_TO + " " + Enum.GetName(typeof(HardwareInfo.FirmwareTypes), Convert.ToInt32(modelTemplate.fwType)) + ")";
                    timerAlertFwType.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.ALERT_FIRMWARE_TYPE_TO, Enum.GetName(typeof(HardwareInfo.FirmwareTypes), Convert.ToInt32(modelTemplate.fwType)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If there is no MAC address assigned
                if (string.IsNullOrEmpty(newAsset.network.netMacAddress))
                {
                    if (!offlineMode) //If it's not in offline mode
                    {
                        pass = false;
                        lblIpAddress.Text = " (" + AirUIStrings.ALERT_NETWORK + ")"; //Prints a network error
                        timerAlertNetConnectivity.Enabled = true;
                        nonCompliantCount++;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.ALERT_NETWORK, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    }
                    else //If it's in offline mode
                    {
                        lblIpAddress.Text = AirUIStrings.OFFLINE_MODE_ACTIVATED; //Specifies offline mode
                    }
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If Virtualization Technology is disabled for UEFI and its enforcement is enabled
                if (configOptions.Enforcement.VirtualizationTechnology.ToString() == GenericResources.TRUE && StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.fwVirtualizationTechnology)] == UIStrings.DEACTIVATED && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblVirtualizationTechnology.Text += " (" + AirUIStrings.ALERT_VT + ")";
                    timerAlertVirtualizationTechnology.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.ALERT_VT, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If Smart status is not OK and its enforcement is enabled
                if (configOptions.Enforcement.SmartStatus.ToString() == GenericResources.TRUE && smartValueList.Contains(GenericResources.PRED_FAIL_CODE) && !offlineMode && serverOnline)
                {
                    pass = false;
                    timerAlertSmartStatus.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.SMART_FAIL, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, TPM enforcement is enabled, and TPM version is incorrect
                if (configOptions.Enforcement.Tpm.ToString() == GenericResources.TRUE && modelTemplate != null && modelTemplate.tpmVersion != newAsset.firmware.fwTpmVersion && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblTpmVersion.Text += " (" + AirUIStrings.ALERT_TPM_TO + " " + StringsAndConstants.LIST_TPM_TYPES[Convert.ToInt32(modelTemplate.tpmVersion)] + ")";
                    timerAlertTpmVersion.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.ALERT_TPM_TO, StringsAndConstants.LIST_TPM_TYPES[Convert.ToInt32(modelTemplate.tpmVersion)], Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetRamAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, and its limit enforcement is enabled, shows an alert
                if (configOptions.Enforcement.RamLimit.ToString() == GenericResources.TRUE && d < 4.0 && Environment.Is64BitOperatingSystem && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblRam.Text += " (" + AirUIStrings.ALERT_NOT_ENOUGH_MEMORY + ")";
                    timerAlertRamAmount.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.ALERT_NOT_ENOUGH_MEMORY, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                //If RAM is more than 4GB and OS is x86, and its limit enforcement is enabled, shows an alert
                if (configOptions.Enforcement.RamLimit.ToString() == GenericResources.TRUE && d > 4.0 && !Environment.Is64BitOperatingSystem && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblRam.Text += " (" + AirUIStrings.ALERT_TOO_MUCH_MEMORY + ")";
                    timerAlertRamAmount.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.ALERT_TOO_MUCH_MEMORY, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                if (nonCompliantCount > 0)
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.FIX_PROBLEMS + " [" + nonCompliantCount + "]", string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                if (pass && !offlineMode && serverOnline)
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_PASSED, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                //If there are compliance errors, colors taskbar's progress bar red
                if (!pass && !offlineMode)
                {
                    progressBar1.SetState(2);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                }

                if (!offlineMode && serverOnline)
                {
                    if (nonCompliantCount == 0)
                    {
                        lblColorCompliant.Text = UIStrings.NO_PENDENCIES;
                        lblColorCompliant.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
                    }
                    else
                    {
                        lblColorCompliant.Text = nonCompliantCount.ToString() + " " + UIStrings.PENDENCIES;
                        lblColorCompliant.ForeColor = StringsAndConstants.ALERT_COLOR;
                    }
                    loadingCircleCompliant.Visible = false;
                    loadingCircleCompliant.Active = false;
                }
            }
            catch (Exception e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), e.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            }
        }

        /// <summary> 
        /// Runs the registration process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RegisterButton_ClickAsync(object sender, EventArgs e)
        {
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
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                tbProgMain.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
                loadingCircleRegisterButton.Visible = true;
                loadingCircleRegisterButton.Active = true;
                registerButton.Text = GenericResources.DASH_SINGLE;
                registerButton.Enabled = false;
                apcsButton.Enabled = false;
                collectButton.Enabled = false;

                //Attribute variables to a previously created new Asset, which will be sent to the server
                location l = new location
                {
                    locBuilding = Array.IndexOf(serverParam.Parameters.Buildings.ToArray(), comboBoxBuilding.SelectedItem.ToString()).ToString(),
                    locRoomNumber = textBoxRoomLetter.Text != string.Empty ? textBoxRoomNumber.Text + textBoxRoomLetter.Text : textBoxRoomNumber.Text,
                };

                maintenances m = new maintenances
                {
                    mainAgentId = agent.id,
                    mainBatteryChange = comboBoxBatteryChange.SelectedItem.ToString().Equals(UIStrings.LIST_YES_0) ? Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString(),
                    mainServiceDate = dateTimePickerServiceDate.Value.ToString(GenericResources.DATE_FORMAT).Substring(0, 10),
                    mainServiceType = serviceTypeRadio.ToString(),
                    mainTicketNumber = textBoxTicketNumber.Text
                };

                newAsset.assetNumber = textBoxAssetNumber.Text;
                newAsset.discarded = "0";
                newAsset.inUse = comboBoxInUse.SelectedItem.ToString().Equals(UIStrings.LIST_YES_0) ? Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                newAsset.sealNumber = textBoxSealNumber.Text;
                newAsset.standard = comboBoxStandard.SelectedItem.ToString().Equals(UIStrings.LIST_STANDARD_GUI_EMPLOYEE) ? Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString() : Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString();
                newAsset.tag = comboBoxTag.SelectedItem.ToString().Equals(UIStrings.LIST_YES_0) ? Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                newAsset.adRegistered = comboBoxActiveDirectory.SelectedItem.ToString().Equals(UIStrings.LIST_YES_0) ? Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                newAsset.hardware.hwType = Array.IndexOf(serverParam.Parameters.HardwareTypes.ToArray(), comboBoxHwType.SelectedItem.ToString()).ToString();
                newAsset.location = l;
                if (newAsset.assetHash == null)
                    newAsset.assetHash = Misc.MiscMethods.HardwareSha256UniqueId(newAsset);
                if (newAsset.hwHash == null)
                    newAsset.hwHash = Misc.MiscMethods.HardwareSha256Hash(newAsset);

                newMaintenances.Clear();
                newMaintenances.Add(m);

                //If asset is discarded
                if (existingAsset != null && existingAsset.discarded == "1")
                {
                    tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.ASSET_DROPPED, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                    _ = MessageBox.Show(UIStrings.ASSET_DROPPED, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                }
                else //If not discarded
                {
                    if (serverOnline) //If server is online
                    {
                        try //Tries to get the latest register date from the asset number to check if the chosen date is adequate
                        {
                            DateTime registerDate = DateTime.ParseExact(newAsset.maintenances[0].mainServiceDate, GenericResources.DATE_FORMAT, CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(existingAsset.maintenances[0].mainServiceDate, GenericResources.DATE_FORMAT, CultureInfo.InvariantCulture);

                            if (registerDate >= lastRegisterDate) //If chosen date is greater or equal than the last format/maintenance date of the PC, let proceed
                            {
                                try
                                {
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_APCS_REGISTERING, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                                    //Send info to server
                                    finalHttpCode = await AssetHandler.SetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_ASSET_NUMBER_URL, newAsset);

                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                                    _ = MessageBox.Show(UIStrings.ASSET_UPDATED, UIStrings.SUCCESS_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                catch (HttpRequestException)
                                {
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                                    _ = MessageBox.Show(UIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _ = MessageBox.Show(UIStrings.ASSET_NOT_ADDED, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                tbProgMain.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                            }
                            else //If chosen date is before the last format/maintenance date of the PC, shows an error
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.INCORRECT_REGISTER_DATE, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                                tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);

                                _ = MessageBox.Show(AirUIStrings.INCORRECT_REGISTER_DATE, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                _ = MessageBox.Show(UIStrings.ASSET_NOT_UPDATED, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch //If can't retrieve (asset number non existent in the database), register normally
                        {
                            try
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_APCS_REGISTERING, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                                //Send info to server
                                finalHttpCode = await AssetHandler.SetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_ASSET_NUMBER_URL, newAsset);

                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                                _ = MessageBox.Show(UIStrings.ASSET_ADDED, UIStrings.SUCCESS_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (HttpRequestException)
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                                tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                                _ = MessageBox.Show(UIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                _ = MessageBox.Show(UIStrings.ASSET_NOT_ADDED, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            tbProgMain.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                        }
                    }
                    else //If the server is out of reach
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.SERVER_NOT_FOUND_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                        _ = MessageBox.Show(UIStrings.SERVER_NOT_FOUND_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                        tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                    }

                    if (Convert.ToInt32(finalHttpCode) == 200 || Convert.ToInt32(finalHttpCode) == 201 || Convert.ToInt32(finalHttpCode) == 204)
                    {
                        //Disables controls after the registry
                        textBoxAssetNumber.Enabled = false;
                        textBoxSealNumber.Text = string.Empty;
                        textBoxSealNumber.Enabled = false;
                        textBoxRoomNumber.Text = string.Empty;
                        textBoxRoomNumber.Enabled = false;
                        textBoxRoomLetter.Text = string.Empty;
                        textBoxRoomLetter.Enabled = false;
                        textBoxTicketNumber.Text = string.Empty;
                        textBoxTicketNumber.Enabled = false;
                        comboBoxBuilding.ResetText();
                        comboBoxBuilding.SelectedIndex = -1;
                        comboBoxBuilding.Enabled = false;
                        comboBoxInUse.ResetText();
                        comboBoxInUse.SelectedIndex = -1;
                        comboBoxInUse.Enabled = false;
                        comboBoxHwType.ResetText();
                        comboBoxHwType.SelectedIndex = -1;
                        comboBoxHwType.Enabled = false;
                        comboBoxTag.ResetText();
                        comboBoxTag.SelectedIndex = -1;
                        comboBoxTag.Enabled = false;
                        comboBoxActiveDirectory.ResetText();
                        comboBoxActiveDirectory.SelectedIndex = -1;
                        comboBoxActiveDirectory.Enabled = false;
                        comboBoxStandard.ResetText();
                        comboBoxStandard.SelectedIndex = -1;
                        comboBoxStandard.Enabled = false;
                        comboBoxBatteryChange.ResetText();
                        comboBoxBatteryChange.SelectedIndex = -1;
                        comboBoxBatteryChange.Enabled = false;
                        dateTimePickerServiceDate.Enabled = false;
                        radioButtonFormatting.Enabled = false;
                        radioButtonMaintenance.Enabled = false;
                        radioButtonUpdateData.Enabled = false;
                        collectButton.Enabled = false;
                        registerButton.Text = AirUIStrings.EXIT;
                        registerButton.Click -= RegisterButton_ClickAsync;
                        registerButton.Click += ExitButton_ClickAsync;
                        lblNoticeHardwareChanged.Visible = false;
                        hardwareChangeButton.Visible = false;
                        loadingCircleLastService.Visible = true;
                        loadingCircleLastService.Active = true;
                        loadingCircleRegisterButton.Visible = true;
                        loadingCircleRegisterButton.Active = true;
                        lblThereIsNothingHere.Visible = false;
                        tableMaintenances.Visible = false;
                        loadingCircleTableMaintenances.Visible = true;
                        loadingCircleTableMaintenances.Active = true;
                        lblColorLastService.Text = GenericResources.DASH_SINGLE;
                    }

                    if (serverOnline)
                    {
                        try
                        {
                            existingAsset = await AssetHandler.GetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_ASSET_NUMBER_URL + textBoxAssetNumber.Text);

                            //radioButtonUpdateData.Enabled = true;
                            loadingCircleLastService.Visible = false;
                            loadingCircleLastService.Active = false;
                            lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(existingAsset.maintenances[0].mainServiceDate);
                            lblColorLastService.ForeColor = StringsAndConstants.BLUE_FOREGROUND;

                            tableMaintenances.Rows.Clear();

                            TableMaintenancesFiller();

                            tableMaintenances.Visible = true;
                        }
                        //If asset does not exist on the database
                        catch (UnregisteredAssetException ex)
                        {
                            loadingCircleLastService.Visible = false;
                            loadingCircleLastService.Active = false;

                            radioButtonUpdateData.Enabled = false;
                            lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(string.Empty);
                            lblColorLastService.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                            lblThereIsNothingHere.Visible = true;
                        }
                        catch (InvalidRestApiCallException ex)
                        {
                            //Shows a message about an error in the APCS web service
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                            tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                            _ = MessageBox.Show(UIStrings.SERVER_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                        }
                        catch (InvalidAgentException ex)
                        {
                            //Shows a message about an error of the agent credentials
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ex.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                            tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                            _ = MessageBox.Show(UIStrings.INVALID_CREDENTIALS, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                        }
                        //If server is unreachable
                        catch (HttpRequestException)
                        {
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                            tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                            _ = MessageBox.Show(UIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        _ = MessageBox.Show(UIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (!pass) //If there are pendencies in the PC config
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.PENDENCY_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                _ = MessageBox.Show(AirUIStrings.PENDENCY_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
            }
            else //If all fields are not filled
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.MANDATORY_FIELD, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                _ = MessageBox.Show(AirUIStrings.MANDATORY_FIELD, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

                tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
                tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
            }

            //When finished registering, resets control states
            loadingCircleRegisterButton.Visible = false;
            loadingCircleRegisterButton.Active = false;
            loadingCircleTableMaintenances.Visible = false;
            loadingCircleTableMaintenances.Active = false;
            loadingCircleLastService.Visible = false;
            loadingCircleLastService.Active = false;
            registerButton.Enabled = true;
            apcsButton.Enabled = true;
        }

        /*-------------------------------------------------------------------------------------------------------------------------------------------*/
        // Passive functions
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

        private async void TableMaintenancesFiller()
        {
            for (int i = 0; i < existingAsset.maintenances.Count; i++)
            {
                //Feches agent names from server
                agentMaintenances = await AuthenticationHandler.GetAgentAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_AGENT_ID_URL + existingAsset.maintenances[i].mainAgentId);
                if (agentMaintenances.id == existingAsset.maintenances[i].mainAgentId)
                {
                    _ = tableMaintenances.Rows.Add(DateTime.ParseExact(existingAsset.maintenances[i].mainServiceDate, GenericResources.DATE_FORMAT, CultureInfo.InvariantCulture).ToString(GenericResources.DATE_DISPLAY), StringsAndConstants.LIST_SERVICE_TYPE_GUI[Convert.ToInt32(existingAsset.maintenances[i].mainServiceType)], agentMaintenances.name + " " + agentMaintenances.surname);

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), "[" + i + "]", DateTime.ParseExact(existingAsset.maintenances[i].mainServiceDate, GenericResources.DATE_FORMAT, CultureInfo.InvariantCulture).ToString(GenericResources.DATE_DISPLAY) + " - " + StringsAndConstants.LIST_SERVICE_TYPE_GUI[Convert.ToInt32(existingAsset.maintenances[i].mainServiceType)] + " - " + agentMaintenances.name + " " + agentMaintenances.surname, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
            }
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
            collectButton.Text = AirUIStrings.FETCH_AGAIN; //Updates collect button text
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_AUTOTHEME_CHANGE, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_LIGHT_THEME_CHANGE, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
            Misc.MiscMethods.LightThemeAllControls(this);
            LightThemeSpecificControls();
            Misc.MiscMethods.LightThemeAllControls(processorForm);
            processorForm.LightThemeSpecificControls();
            Misc.MiscMethods.LightThemeAllControls(ramForm);
            ramForm.LightThemeSpecificControls();
            Misc.MiscMethods.LightThemeAllControls(storageForm);
            storageForm.LightThemeSpecificControls();
            Misc.MiscMethods.LightThemeAllControls(videoCardForm);
            videoCardForm.LightThemeSpecificControls();
            Misc.MiscMethods.LightThemeAllControls(hardwareChangeForm);
            hardwareChangeForm.LightThemeSpecificControls();
            isSystemDarkModeEnabled = false;
        }

        /// <summary> 
        /// Method for setting the dark theme via toolStrip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuDarkTheme_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_DARK_THEME_CHANGE, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
            Misc.MiscMethods.DarkThemeAllControls(this);
            DarkThemeSpecificControls();
            Misc.MiscMethods.DarkThemeAllControls(processorForm);
            processorForm.DarkThemeSpecificControls();
            Misc.MiscMethods.DarkThemeAllControls(ramForm);
            ramForm.DarkThemeSpecificControls();
            Misc.MiscMethods.DarkThemeAllControls(storageForm);
            storageForm.DarkThemeSpecificControls();
            Misc.MiscMethods.DarkThemeAllControls(videoCardForm);
            videoCardForm.DarkThemeSpecificControls();
            Misc.MiscMethods.DarkThemeAllControls(hardwareChangeForm);
            hardwareChangeForm.DarkThemeSpecificControls();
            isSystemDarkModeEnabled = true;
        }

        /// <summary> 
        /// Method for opening the Storage list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageDetailsButton_Click(object sender, EventArgs e)
        {
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WIN_10_NAMENUM))
                DarkNet.Instance.SetWindowThemeForms(storageForm, Theme.Auto);
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OPENING_STORAGE_FORM, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            _ = storageForm.ShowDialog();
        }

        /// <summary> 
        /// Method for opening the Video Card list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoCardDetailsButton_Click(object sender, EventArgs e)
        {
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WIN_10_NAMENUM))
                DarkNet.Instance.SetWindowThemeForms(videoCardForm, Theme.Auto);
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OPENING_VIDEO_CARD_FORM, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            _ = videoCardForm.ShowDialog();
        }

        /// <summary> 
        /// Method for opening the RAM list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RamDetailsButton_Click(object sender, EventArgs e)
        {
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WIN_10_NAMENUM))
                DarkNet.Instance.SetWindowThemeForms(ramForm, Theme.Auto);
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OPENING_RAM_FORM, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            _ = ramForm.ShowDialog();
        }

        /// <summary> 
        /// Method for opening the Processor list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessorDetailsButton_Click(object sender, EventArgs e)
        {
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WIN_10_NAMENUM))
                DarkNet.Instance.SetWindowThemeForms(processorForm, Theme.Auto);
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OPENING_PROCESSOR_FORM, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
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
        /// Method for opening the Hardware Change comparison form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HardwareChangeButton_Click(object sender, EventArgs e)
        {
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WIN_10_NAMENUM))
                DarkNet.Instance.SetWindowThemeForms(hardwareChangeForm, Theme.Auto);
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OPENING_HARDWARE_CHANGE_FORM, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            hardwareChangeForm.FillData(existingAsset, newAsset);
            _ = hardwareChangeForm.ShowDialog();
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
                xPosOS -= Convert.ToInt32(GenericResources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertOSScroll = true;
            }

            if (xPosOS < leftBound && invertOSScroll == true)
            {
                lblOperatingSystem.Location = new Point(xPosOS, yPosOS);
                xPosOS += Convert.ToInt32(GenericResources.LABEL_SCROLL_SPEED);
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
                xPosFwVersion -= Convert.ToInt32(GenericResources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertFwVersionScroll = true;
            }

            if (xPosFwVersion < leftBound && invertFwVersionScroll == true)
            {
                lblFwVersion.Location = new Point(xPosFwVersion, yPosFwVersion);
                xPosFwVersion += Convert.ToInt32(GenericResources.LABEL_SCROLL_SPEED);
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
                lblVideoCard.Location = new Point(xPosVideoCard, yPosVideoCard);
                xPosVideoCard -= Convert.ToInt32(GenericResources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertVideoCardScroll = true;
            }

            if (xPosVideoCard < leftBound && invertVideoCardScroll == true)
            {
                lblVideoCard.Location = new Point(xPosVideoCard, yPosVideoCard);
                xPosVideoCard += Convert.ToInt32(GenericResources.LABEL_SCROLL_SPEED);
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
                xPosRam -= Convert.ToInt32(GenericResources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertRamScroll = true;
            }

            if (xPosRam < leftBound && invertRamScroll == true)
            {
                lblRam.Location = new Point(xPosRam, yPosRam);
                xPosRam += Convert.ToInt32(GenericResources.LABEL_SCROLL_SPEED);
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
                xPosProcessor -= Convert.ToInt32(GenericResources.LABEL_SCROLL_SPEED);
            }
            else
            {
                invertProcessorScroll = true;
            }

            if (xPosProcessor < leftBound && invertProcessorScroll == true)
            {
                lblProcessor.Location = new Point(xPosProcessor, yPosProcessor);
                xPosProcessor += Convert.ToInt32(GenericResources.LABEL_SCROLL_SPEED);
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
        /// Sets highlight HwUid label when hovering with the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HwUidLabel_MouseHover(object sender, EventArgs e)
        {
            hwUidToolTip.SetToolTip(lblNoticeHardwareChanged, AirUIStrings.DATABASE_HARDWARE_ID + ": " + existingAsset.hwHash + "\n" + AirUIStrings.CURRENT_HARDWARE_ID + ": " + newAsset.hwHash);
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
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WIN_10_NAMENUM))
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OPENING_LOG, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
#if DEBUG
            System.Diagnostics.Process.Start(configOptions.Definitions.LogLocation + GenericResources.LOG_FILENAME_AIR + "-v" + Application.ProductVersion + "-" + GenericResources.DEV_STATUS_BETA + GenericResources.LOG_FILE_EXT);
#else
            System.Diagnostics.Process.Start(configOptions.Definitions.LogLocation + GenericResources.LOG_FILENAME_AIR + "-v" + Application.ProductVersion + GenericResources.LOG_FILE_EXT);
#endif
        }

        /// <summary> 
        /// Opens the APCS homepage for the selected address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApcsButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_ACCESS_APCS, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            _ = System.Diagnostics.Process.Start(GenericResources.HTTP + serverIP + ":" + serverPort);
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
        /// Exits the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_ClickAsync(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_FORM, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            Environment.Exit(Convert.ToInt32(ExitCodes.SUCCESS));
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
                    Misc.MiscMethods.LightThemeAllControls(processorForm);
                    processorForm.LightThemeSpecificControls();
                    Misc.MiscMethods.LightThemeAllControls(ramForm);
                    ramForm.LightThemeSpecificControls();
                    Misc.MiscMethods.LightThemeAllControls(storageForm);
                    storageForm.LightThemeSpecificControls();
                    Misc.MiscMethods.LightThemeAllControls(videoCardForm);
                    videoCardForm.LightThemeSpecificControls();
                    Misc.MiscMethods.LightThemeAllControls(hardwareChangeForm);
                    hardwareChangeForm.LightThemeSpecificControls();
                    isSystemDarkModeEnabled = false;
                    break;
                case 1:
                    Misc.MiscMethods.DarkThemeAllControls(this);
                    DarkThemeSpecificControls();
                    Misc.MiscMethods.DarkThemeAllControls(processorForm);
                    processorForm.DarkThemeSpecificControls();
                    Misc.MiscMethods.DarkThemeAllControls(ramForm);
                    ramForm.DarkThemeSpecificControls();
                    Misc.MiscMethods.DarkThemeAllControls(storageForm);
                    storageForm.DarkThemeSpecificControls();
                    Misc.MiscMethods.DarkThemeAllControls(videoCardForm);
                    videoCardForm.DarkThemeSpecificControls();
                    Misc.MiscMethods.DarkThemeAllControls(hardwareChangeForm);
                    hardwareChangeForm.DarkThemeSpecificControls();
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

            toolStripAutoTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_AUTOTHEME_LIGHT_PATH));
            toolStripLightTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_LIGHTTHEME_LIGHT_PATH));
            toolStripDarkTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_DARKTHEME_LIGHT_PATH));
            comboBoxThemeButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_AUTOTHEME_LIGHT_PATH));
            logLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_LOG_LIGHT_PATH));
            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_ABOUT_LIGHT_PATH));
            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.MAIN_BANNER_LIGHT_PATH));
            iconImgBrand.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_BRAND_LIGHT_PATH));
            iconImgModel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_MODEL_LIGHT_PATH));
            iconImgSerialNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_SERIAL_NUMBER_LIGHT_PATH));
            iconImgProcessor.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_CPU_LIGHT_PATH));
            iconImgRam.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_RAM_LIGHT_PATH));
            iconImgStorageType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_HDD_LIGHT_PATH));
            iconImgMediaOperationMode.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_AHCI_LIGHT_PATH));
            iconImgVideoCard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_GPU_LIGHT_PATH));
            iconImgOperatingSystem.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_WINDOWS_LIGHT_PATH));
            iconImgHostname.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_HOSTNAME_LIGHT_PATH));
            iconImgIpAddress.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_IP_LIGHT_PATH));
            iconImgFwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_BIOS_LIGHT_PATH));
            iconImgFwVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_BIOS_VERSION_LIGHT_PATH));
            iconImgSecureBoot.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_SECURE_BOOT_LIGHT_PATH));
            iconImgAssetNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_ASSET_LIGHT_PATH));
            iconImgSealNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_SEAL_LIGHT_PATH));
            iconImgRoomNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_ROOM_LIGHT_PATH));
            iconImgBuilding.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_BUILDING_LIGHT_PATH));
            iconImgAdRegistered.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_SERVER_LIGHT_PATH));
            iconImgStandard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_STANDARD_LIGHT_PATH));
            iconImgServiceDate.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_SERVICE_LIGHT_PATH));
            iconImgRoomLetter.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_LETTER_LIGHT_PATH));
            iconImgInUse.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_IN_USE_LIGHT_PATH));
            iconImgTag.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_STICKER_LIGHT_PATH));
            iconImgHwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_TYPE_LIGHT_PATH));
            iconImgVirtualizationTechnology.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_VT_X_LIGHT_PATH));
            iconImgTpmVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_TPM_LIGHT_PATH));
            iconImgBatteryChange.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_CMOS_BATTERY_LIGHT_PATH));
            iconImgTicketNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_TICKET_LIGHT_PATH));
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

            toolStripAutoTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_AUTOTHEME_DARK_PATH));
            toolStripLightTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_LIGHTTHEME_DARK_PATH));
            toolStripDarkTheme.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_DARKTHEME_DARK_PATH));
            comboBoxThemeButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_AUTOTHEME_DARK_PATH));
            logLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_LOG_DARK_PATH));
            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_ABOUT_DARK_PATH));
            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.MAIN_BANNER_DARK_PATH));
            iconImgBrand.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_BRAND_DARK_PATH));
            iconImgModel.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_MODEL_DARK_PATH));
            iconImgSerialNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_SERIAL_NUMBER_DARK_PATH));
            iconImgProcessor.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_CPU_DARK_PATH));
            iconImgRam.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_RAM_DARK_PATH));
            iconImgStorageType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_HDD_DARK_PATH));
            iconImgMediaOperationMode.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_AHCI_DARK_PATH));
            iconImgVideoCard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_GPU_DARK_PATH));
            iconImgOperatingSystem.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_WINDOWS_DARK_PATH));
            iconImgHostname.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_HOSTNAME_DARK_PATH));
            iconImgIpAddress.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_IP_DARK_PATH));
            iconImgFwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_BIOS_DARK_PATH));
            iconImgFwVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_BIOS_VERSION_DARK_PATH));
            iconImgSecureBoot.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_SECURE_BOOT_DARK_PATH));
            iconImgAssetNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_ASSET_DARK_PATH));
            iconImgSealNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_SEAL_DARK_PATH));
            iconImgRoomNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_ROOM_DARK_PATH));
            iconImgBuilding.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_BUILDING_DARK_PATH));
            iconImgAdRegistered.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_SERVER_DARK_PATH));
            iconImgStandard.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_STARDARD_DARK_PATH));
            iconImgServiceDate.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_SERVICE_DARK_PATH));
            iconImgRoomLetter.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_LETTER_DARK_PATH));
            iconImgInUse.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_IN_USE_DARK_PATH));
            iconImgTag.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_STICKER_DARK_PATH));
            iconImgHwType.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_TYPE_DARK_PATH));
            iconImgVirtualizationTechnology.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_VT_X_DARK_PATH));
            iconImgTpmVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_TPM_DARK_PATH));
            iconImgBatteryChange.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_CMOS_BATTERY_DARK_PATH));
            iconImgTicketNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_TICKET_DARK_PATH));
        }
    }
}

