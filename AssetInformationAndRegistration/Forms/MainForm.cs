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
        private readonly ProcessorDetailForm processorForm;
        private readonly RamDetailForm ramForm;
        private readonly StorageDetailForm storageForm;
        private readonly VideoCardDetailForm videoCardForm;

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

            processorForm.Hide();
            ramForm.Hide();
            storageForm.Hide();
            videoCardForm.Hide();

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
                //Fetch building and hw types info from the specified server
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_SERVER_DATA, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                serverParam = await ParameterHandler.GetParameterAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.V1_API_PARAMETERS_URL);
            }
            else
            {
                //Fetch building and hw types info from the local file
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_LOCAL_DATA, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                serverParam = ParameterHandler.GetOfflineModeConfigFile();
            }

            comboBoxBuilding.Items.AddRange(serverParam.Parameters.Buildings.ToArray());
            comboBoxHwType.Items.AddRange(serverParam.Parameters.HardwareTypes.ToArray());

            //Fills controls with provided info from ini file and constants dll
            comboBoxActiveDirectory.Items.AddRange(StringsAndConstants.LIST_ACTIVE_DIRECTORY_GUI.ToArray());
            comboBoxStandard.Items.AddRange(StringsAndConstants.LIST_STANDARD_GUI.ToArray());
            comboBoxInUse.Items.AddRange(StringsAndConstants.LIST_IN_USE_GUI.ToArray());
            comboBoxTag.Items.AddRange(StringsAndConstants.LIST_TAG_GUI.ToArray());
            comboBoxBatteryChange.Items.AddRange(StringsAndConstants.LIST_BATTERY_GUI.ToArray());
            if (HardwareInfo.GetHostname().Substring(0, 3).ToUpper().Equals(GenericResources.HOSTNAME_PATTERN))
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

            timerAlertHostname.Interval = Convert.ToInt32(GenericResources.TIMER_INTERVAL);
            timerAlertMediaOperationMode.Interval = Convert.ToInt32(GenericResources.TIMER_INTERVAL);
            timerAlertSecureBoot.Interval = Convert.ToInt32(GenericResources.TIMER_INTERVAL);
            timerAlertFwVersion.Interval = Convert.ToInt32(GenericResources.TIMER_INTERVAL);
            timerAlertNetConnectivity.Interval = Convert.ToInt32(GenericResources.TIMER_INTERVAL);
            timerAlertFwType.Interval = Convert.ToInt32(GenericResources.TIMER_INTERVAL);
            timerAlertVirtualizationTechnology.Interval = Convert.ToInt32(GenericResources.TIMER_INTERVAL);
            timerAlertSmartStatus.Interval = Convert.ToInt32(GenericResources.TIMER_INTERVAL);
            timerAlertTpmVersion.Interval = Convert.ToInt32(GenericResources.TIMER_INTERVAL);
            timerAlertRamAmount.Interval = Convert.ToInt32(GenericResources.TIMER_INTERVAL);
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_MAIN_FORM, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), GenericResources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_START_COLLECT_THREAD, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            StartAsync(sender, e);
        }

        /// <summary> 
        /// Prepares for the collection process
        /// </summary>
        private async void CollectPreparation()
        {
            #region Writes a dash in the labels, while scanning the hardware
            lblColorLastService.Text = GenericResources.DASH;
            lblBrand.Text = GenericResources.DASH;
            lblModel.Text = GenericResources.DASH;
            lblSerialNumber.Text = GenericResources.DASH;
            lblProcessor.Text = GenericResources.DASH;
            lblRam.Text = GenericResources.DASH;
            lblColorCompliant.Text = GenericResources.DASH;
            lblStorageType.Text = GenericResources.DASH;
            lblMediaOperationMode.Text = GenericResources.DASH;
            lblVideoCard.Text = GenericResources.DASH;
            lblOperatingSystem.Text = GenericResources.DASH;
            lblHostname.Text = GenericResources.DASH;
            lblIpAddress.Text = GenericResources.DASH;
            lblFwVersion.Text = GenericResources.DASH;
            lblFwType.Text = GenericResources.DASH;
            lblSecureBoot.Text = GenericResources.DASH;
            lblVirtualizationTechnology.Text = GenericResources.DASH;
            lblTpmVersion.Text = GenericResources.DASH;
            collectButton.Text = GenericResources.DASH;
            lblColorServerOperationalStatus.Text = GenericResources.DASH;
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

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    _ = MessageBox.Show(AirUIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_START_COLLECTING, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

            progressbarCount = 0;
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for PC maker
            newAsset.hardware.brand = HardwareInfo.GetBrand();
            if (newAsset.hardware.brand == GenericResources.TO_BE_FILLED_BY_OEM || newAsset.hardware.brand == string.Empty)
                newAsset.hardware.brand = HardwareInfo.GetBrandAlt();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_BRAND, newAsset.hardware.brand, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for PC model
            newAsset.hardware.model = HardwareInfo.GetModel();
            if (newAsset.hardware.model == GenericResources.TO_BE_FILLED_BY_OEM || newAsset.hardware.model == string.Empty)
            {
                newAsset.hardware.model = HardwareInfo.GetModelAlt();
                if (newAsset.hardware.model == GenericResources.TO_BE_FILLED_BY_OEM || newAsset.hardware.model == string.Empty)
                    newAsset.hardware.model = UIStrings.UNKNOWN;
            }
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_MODEL, newAsset.hardware.model, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for motherboard Serial number
            newAsset.hardware.serialNumber = HardwareInfo.GetSerialNumber();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_SERIAL_NUMBER, newAsset.hardware.serialNumber, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for CPU information
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
                        processorDetail[i][j] = UIStrings.UNKNOWN;
                }
                processorDetail[i][1] = processorDetail[i][1].Replace("(R)", string.Empty);
                processorDetail[i][1] = processorDetail[i][1].Replace("(TM)", string.Empty);
                processorDetail[i][1] = processorDetail[i][1].Replace("(tm)", string.Empty);
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
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_NAME + " [" + newHardware.processor[i].processorId + "]", newHardware.processor[i].name, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_FREQUENCY + " [" + newHardware.processor[i].processorId + "]", newHardware.processor[i].frequency, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_CORES + " [" + newHardware.processor[i].processorId + "]", newHardware.processor[i].numberOfCores, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_THREADS + " [" + newHardware.processor[i].processorId + "]", newHardware.processor[i].numberOfThreads, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_CACHE + " [" + newHardware.processor[i].processorId + "]", Misc.MiscMethods.FriendlySizeBinary(Convert.ToInt64(newHardware.processor[i].cache)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            }
            processorForm.TreatData(processorDetail);
            processorSummary = processorDetail[0][1];
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR, processorSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for RAM amount and total number of slots
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
                        ramDetail[i][j] = UIStrings.UNKNOWN;
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
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_TYPE + " [" + newHardware.ram[i].slot + "]", Enum.GetName(typeof(HardwareInfo.RamTypes), Convert.ToInt32(newHardware.ram[i].type)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_AMOUNT + " [" + newHardware.ram[i].slot + "]", Misc.MiscMethods.FriendlySizeBinary(Convert.ToInt64(newHardware.ram[i].amount)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_FREQUENCY + " [" + newHardware.ram[i].slot + "]", newHardware.ram[i].frequency, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_MANUFACTURER + " [" + newHardware.ram[i].slot + "]", newHardware.ram[i].manufacturer, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_SERIAL_NUMBER + " [" + newHardware.ram[i].slot + "]", newHardware.ram[i].serialNumber, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_PART_NUMBER + " [" + newHardware.ram[i].slot + "]", newHardware.ram[i].partNumber, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            }
            ramForm.TreatData(ramDetail, isSystemDarkModeEnabled);
            ramSummary = HardwareInfo.GetRamSummary();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM, ramSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Storage data
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
                        storageDetail[i][j] = UIStrings.UNKNOWN;
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
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_TYPE + " [" + newHardware.storage[i].storageId + "]", newHardware.storage[i].type, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_SIZE + " [" + newHardware.storage[i].storageId + "]", Misc.MiscMethods.FriendlySizeDecimal(Convert.ToInt64(newHardware.storage[i].size)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_CONNECTION + " [" + newHardware.storage[i].storageId + "]", newHardware.storage[i].connection, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_MODEL + " [" + newHardware.storage[i].storageId + "]", newHardware.storage[i].model, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_SERIAL_NUMBER + " [" + newHardware.storage[i].storageId + "]", newHardware.storage[i].serialNumber, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_SMART_STATUS + " [" + newHardware.storage[i].storageId + "]", newHardware.storage[i].smartStatus, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            }
            storageForm.TreatData(storageDetail);
            storageSummary = HardwareInfo.GetStorageSummary();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE, storageSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Media Operation (IDE/AHCI/NVME)
            newAsset.firmware.mediaOperationMode = HardwareInfo.GetMediaOperationMode();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_MEDIA_OPERATION_TYPE, serverParam.Parameters.MediaOperationTypes[Convert.ToInt32(newAsset.firmware.mediaOperationMode)], Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
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
                        videoCardDetail[i][j] = UIStrings.UNKNOWN;
                }
                videoCardDetail[i][1] = videoCardDetail[i][1].Replace("(R)", string.Empty);
                videoCardDetail[i][1] = videoCardDetail[i][1].Replace("(TM)", string.Empty);
                videoCardDetail[i][1] = videoCardDetail[i][1].Replace("(tm)", string.Empty);
                videoCard v = new videoCard
                {
                    gpuId = videoCardDetail[i][0],
                    name = videoCardDetail[i][1],
                    vRam = videoCardDetail[i][2],
                };
                newHardware.videoCard.Add(v);
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_VIDEO_CARD_NAME + " [" + newHardware.videoCard[i].gpuId + "]", newHardware.videoCard[i].name, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_VIDEO_CARD_RAM + " [" + newHardware.videoCard[i].gpuId + "]", Misc.MiscMethods.FriendlySizeBinary(Convert.ToInt64(newHardware.videoCard[i].vRam)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            }
            videoCardForm.TreatData(videoCardDetail);
            videoCardSummary = videoCardDetail[0][1];
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_VIDEO_CARD, videoCardSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for OS infomation
            newAsset.operatingSystem.arch = HardwareInfo.GetOSArchBinary();
            newAsset.operatingSystem.build = HardwareInfo.GetOSBuildAndRevision();
            newAsset.operatingSystem.name = HardwareInfo.GetOSName();
            newAsset.operatingSystem.version = HardwareInfo.GetOSVersion();
            operatingSystemSummary = HardwareInfo.GetOSSummary();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OPERATING_SYSTEM, operatingSystemSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Hostname
            newAsset.network.hostname = HardwareInfo.GetHostname();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_NETWORK_HOSTNAME, newAsset.network.hostname, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for MAC Address
            newAsset.network.macAddress = HardwareInfo.GetMacAddress();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_NETWORK_MAC_ADDRESS, newAsset.network.macAddress, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for IP Address
            newAsset.network.ipAddress = HardwareInfo.GetIpAddress();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_NETWORK_IP_ADDRESS, newAsset.network.ipAddress, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for firmware type
            newAsset.firmware.type = HardwareInfo.GetFwType();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_TYPE, serverParam.Parameters.FirmwareTypes[Convert.ToInt32(newAsset.firmware.type)], Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Secure Boot status
            newAsset.firmware.secureBoot = HardwareInfo.GetSecureBoot();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_SECURE_BOOT, StringsAndConstants.LIST_STATES[Convert.ToInt32(serverParam.Parameters.SecureBootStates[Convert.ToInt32(newAsset.firmware.secureBoot)])], Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for firmware version
            newAsset.firmware.version = HardwareInfo.GetFirmwareVersion();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_VERSION, newAsset.firmware.version, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for VT status
            newAsset.firmware.virtualizationTechnology = HardwareInfo.GetVirtualizationTechnology();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_VIRTUALIZATION_TECHNOLOGY, StringsAndConstants.LIST_STATES[Convert.ToInt32(serverParam.Parameters.VirtualizationTechnologyStates[Convert.ToInt32(newAsset.firmware.virtualizationTechnology)])], Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for TPM status
            newAsset.firmware.tpmVersion = HardwareInfo.GetTPMStatus();
            progressbarCount++;
            worker.ReportProgress(ProgressAuxFunction(progressbarCount));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_TPM, serverParam.Parameters.TpmTypes[Convert.ToInt32(newAsset.firmware.tpmVersion)], Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_END_COLLECTING, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
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
                    try
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_MODEL_DATA, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                        //Feches model info from server
                        modelTemplate = await ModelHandler.GetModelAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.V1_API_MODEL_URL + lblModel.Text);
                    }
                    catch (InvalidModelException)
                    {

                    }
                    catch (HttpRequestException)
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                        tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                        _ = MessageBox.Show(AirUIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    try
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_ASSET_DATA, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                        existingAsset = null;

                        //Fetches existing asset data from server
                        existingAsset = await AssetHandler.GetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.V1_API_ASSET_URL + textBoxAssetNumber.Text);

                        loadingCircleLastService.Visible = false;
                        loadingCircleLastService.Active = false;

                        radioButtonUpdateData.Enabled = true;
                        lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(existingAsset.maintenances[0].serviceDate);
                        lblColorLastService.ForeColor = StringsAndConstants.BLUE_FOREGROUND;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), lblColorLastService.Text, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SERVICES_MADE, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                        for (int i = 0; i < existingAsset.maintenances.Count; i++)
                        {
                            //Feches agent names from server
                            agentMaintenances = await AuthenticationHandler.GetAgentAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.V1_API_AGENTS_URL + existingAsset.maintenances[i].agentId);
                            if (agentMaintenances.id == existingAsset.maintenances[i].agentId)
                            {
                                _ = tableMaintenances.Rows.Add(DateTime.ParseExact(existingAsset.maintenances[i].serviceDate, GenericResources.DATE_FORMAT, CultureInfo.InvariantCulture).ToString(GenericResources.DATE_DISPLAY), StringsAndConstants.LIST_SERVICE_TYPE_GUI[Convert.ToInt32(existingAsset.maintenances[i].serviceType)], agentMaintenances.name + " " + agentMaintenances.surname);

                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), "[" + i + "]" , DateTime.ParseExact(existingAsset.maintenances[i].serviceDate, GenericResources.DATE_FORMAT, CultureInfo.InvariantCulture).ToString(GenericResources.DATE_DISPLAY) + " - " + StringsAndConstants.LIST_SERVICE_TYPE_GUI[Convert.ToInt32(existingAsset.maintenances[i].serviceType)] + " - " + agentMaintenances.name + " " + agentMaintenances.surname, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                            }
                        }
                        tableMaintenances.Visible = true;
                        tableMaintenances.Sort(tableMaintenances.Columns["serviceDate"], ListSortDirection.Descending);
                    }
                    //If asset does not exist on the database
                    catch (InvalidAssetException)
                    {
                        loadingCircleLastService.Visible = false;
                        loadingCircleLastService.Active = false;

                        radioButtonUpdateData.Enabled = false;
                        lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(string.Empty);
                        lblColorLastService.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), LogStrings.LOG_ASSET_NOT_EXIST, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        lblThereIsNothingHere.Visible = true;
                    }
                    //If server is unreachable
                    catch (HttpRequestException)
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                        tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                        _ = MessageBox.Show(AirUIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    loadingCircleTableMaintenances.Visible = false;
                    loadingCircleTableMaintenances.Active = false;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If hostname is the default one and its enforcement is enabled
                if (configOptions.Enforcement.Hostname.ToString() == GenericResources.TRUE && newAsset.network.hostname.Equals(AirUIStrings.DEFAULT_HOSTNAME) && !offlineMode)
                {
                    pass = false;
                    lblHostname.Text += AirUIStrings.HOSTNAME_ALERT;
                    timerAlertHostname.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.HOSTNAME_ALERT, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, mediaOpMode enforcement is enabled, and the mode is incorrect
                if (configOptions.Enforcement.MediaOperationMode.ToString() == GenericResources.TRUE && modelTemplate != null && modelTemplate.mediaOperationMode != newAsset.firmware.mediaOperationMode && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblMediaOperationMode.Text += AirUIStrings.MEDIA_OPERATION_ALERT;
                    timerAlertMediaOperationMode.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.MEDIA_OPERATION_ALERT, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //The section below contains the exception cases for Secure Boot enforcement, if it is enabled
                if (configOptions.Enforcement.SecureBoot.ToString() == GenericResources.TRUE && StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.secureBoot)] == UIStrings.DEACTIVATED && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblSecureBoot.Text += AirUIStrings.SECURE_BOOT_ALERT;
                    timerAlertSecureBoot.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.SECURE_BOOT_ALERT, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, firmware version enforcement is enabled, and the version is incorrect
                if (configOptions.Enforcement.FirmwareVersion.ToString() == GenericResources.TRUE && modelTemplate != null && !newAsset.firmware.version.Contains(modelTemplate.fwVersion) && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblFwVersion.Text += AirUIStrings.FIRMWARE_VERSION_ALERT;
                    timerAlertFwVersion.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.FIRMWARE_VERSION_ALERT, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, firmware type enforcement is enabled, and the type is incorrect
                if (configOptions.Enforcement.FirmwareType.ToString() == GenericResources.TRUE && modelTemplate != null && modelTemplate.fwType != newAsset.firmware.type && !offlineMode)
                {
                    pass = false;
                    lblFwType.Text += AirUIStrings.FIRMWARE_TYPE_ALERT;
                    timerAlertFwType.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.FIRMWARE_TYPE_ALERT, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If there is no MAC address assigned
                if (string.IsNullOrEmpty(newAsset.network.macAddress))
                {
                    if (!offlineMode) //If it's not in offline mode
                    {
                        pass = false;
                        lblIpAddress.Text = AirUIStrings.NETWORK_ERROR; //Prints a network error
                        timerAlertNetConnectivity.Enabled = true;
                        nonCompliantCount++;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.NETWORK_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    }
                    else //If it's in offline mode
                    {
                        lblIpAddress.Text = AirUIStrings.OFFLINE_MODE_ACTIVATED; //Specifies offline mode
                    }
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If Virtualization Technology is disabled for UEFI and its enforcement is enabled
                if (configOptions.Enforcement.VirtualizationTechnology.ToString() == GenericResources.TRUE && StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.virtualizationTechnology)] == UIStrings.DEACTIVATED && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblVirtualizationTechnology.Text += AirUIStrings.VT_ALERT;
                    timerAlertVirtualizationTechnology.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.VT_ALERT, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If Smart status is not OK and its enforcement is enabled
                if (configOptions.Enforcement.SmartStatus.ToString() == GenericResources.TRUE && storageDetailPrev[6].Contains(GenericResources.PRED_FAIL) && !offlineMode && serverOnline)
                {
                    pass = false;
                    //lblStorageType.Text += UIStrings.SMART_FAIL;
                    timerAlertSmartStatus.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.SMART_FAIL, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, TPM enforcement is enabled, and TPM version is incorrect
                if (configOptions.Enforcement.Tpm.ToString() == GenericResources.TRUE && modelTemplate != null && modelTemplate.tpmVersion != newAsset.firmware.tpmVersion && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblTpmVersion.Text += AirUIStrings.TPM_ERROR;
                    timerAlertTpmVersion.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.TPM_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetRamAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, and its limit enforcement is enabled, shows an alert
                if (configOptions.Enforcement.RamLimit.ToString() == GenericResources.TRUE && d < 4.0 && Environment.Is64BitOperatingSystem && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblRam.Text += AirUIStrings.NOT_ENOUGH_MEMORY;
                    timerAlertRamAmount.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.NOT_ENOUGH_MEMORY, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
                //If RAM is more than 4GB and OS is x86, and its limit enforcement is enabled, shows an alert
                if (configOptions.Enforcement.RamLimit.ToString() == GenericResources.TRUE && d > 4.0 && !Environment.Is64BitOperatingSystem && !offlineMode && serverOnline)
                {
                    pass = false;
                    lblRam.Text += AirUIStrings.TOO_MUCH_MEMORY;
                    timerAlertRamAmount.Enabled = true;
                    nonCompliantCount++;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.TOO_MUCH_MEMORY, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                }
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
                registerButton.Text = GenericResources.DASH;
                registerButton.Enabled = false;
                apcsButton.Enabled = false;
                collectButton.Enabled = false;

                //Attribute variables to a previously created new Asset, which will be sent to the server
                location l = new location
                {
                    building = Array.IndexOf(serverParam.Parameters.Buildings.ToArray(), comboBoxBuilding.SelectedItem.ToString()).ToString(),
                    roomNumber = textBoxRoomLetter.Text != string.Empty ? textBoxRoomNumber.Text + textBoxRoomLetter.Text : textBoxRoomNumber.Text,
                };

                maintenances m = new maintenances
                {
                    agentId = agent.id,
                    batteryChange = comboBoxBatteryChange.SelectedItem.ToString().Equals(UIStrings.LIST_YES_0) ? Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString(),
                    serviceDate = dateTimePickerServiceDate.Value.ToString(GenericResources.DATE_FORMAT).Substring(0, 10),
                    serviceType = serviceTypeRadio.ToString(),
                    ticketNumber = textBoxTicketNumber.Text
                };

                newAsset.assetNumber = textBoxAssetNumber.Text;
                newAsset.discarded = "0";
                newAsset.inUse = comboBoxInUse.SelectedItem.ToString().Equals(UIStrings.LIST_YES_0) ? Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                newAsset.sealNumber = textBoxSealNumber.Text;
                newAsset.standard = comboBoxStandard.SelectedItem.ToString().Equals(UIStrings.LIST_STANDARD_GUI_EMPLOYEE) ? Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString() : Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString();
                newAsset.tag = comboBoxTag.SelectedItem.ToString().Equals(UIStrings.LIST_YES_0) ? Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                newAsset.adRegistered = comboBoxActiveDirectory.SelectedItem.ToString().Equals(UIStrings.LIST_YES_0) ? Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString() : Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                newAsset.hardware.type = Array.IndexOf(serverParam.Parameters.HardwareTypes.ToArray(), comboBoxHwType.SelectedItem.ToString()).ToString();
                newAsset.location = l;

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
                            DateTime registerDate = DateTime.ParseExact(newAsset.maintenances[0].serviceDate, GenericResources.DATE_FORMAT, CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(existingAsset.maintenances[0].serviceDate, GenericResources.DATE_FORMAT, CultureInfo.InvariantCulture);

                            if (registerDate >= lastRegisterDate) //If chosen date is greater or equal than the last format/maintenance date of the PC, let proceed
                            {
                                try
                                {
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_APCS_REGISTERING, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                                    //Send info to server
                                    _ = await AssetHandler.SetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.V1_API_ASSET_URL, newAsset);

                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                                    _ = MessageBox.Show(UIStrings.ASSET_UPDATED, UIStrings.SUCCESS_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                catch (HttpRequestException)
                                {
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                                    _ = MessageBox.Show(AirUIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                _ = await AssetHandler.SetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.V1_API_ASSET_URL, newAsset);

                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                                _ = MessageBox.Show(UIStrings.ASSET_ADDED, UIStrings.SUCCESS_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (HttpRequestException)
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                                tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                                _ = MessageBox.Show(AirUIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    //Feches asset number data from server to update the label
                    loadingCircleLastService.Visible = true;
                    loadingCircleLastService.Active = true;
                    loadingCircleRegisterButton.Visible = true;
                    loadingCircleRegisterButton.Active = true;
                    lblThereIsNothingHere.Visible = false;
                    tableMaintenances.Visible = false;
                    loadingCircleTableMaintenances.Visible = true;
                    loadingCircleTableMaintenances.Active = true;
                    lblColorLastService.Text = GenericResources.DASH;

                    if (serverOnline)
                    {
                        try
                        {
                            existingAsset = await AssetHandler.GetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.V1_API_ASSET_URL + textBoxAssetNumber.Text);

                            radioButtonUpdateData.Enabled = true;
                            loadingCircleLastService.Visible = false;
                            loadingCircleLastService.Active = false;
                            lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(existingAsset.maintenances[0].serviceDate);
                            lblColorLastService.ForeColor = StringsAndConstants.BLUE_FOREGROUND;

                            tableMaintenances.Rows.Clear();
                            for (int i = 0; i < existingAsset.maintenances.Count; i++)
                            {
                                //Feches agent names from server
                                agentMaintenances = await AuthenticationHandler.GetAgentAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.V1_API_AGENTS_URL + existingAsset.maintenances[i].agentId);
                                if (agentMaintenances.id == existingAsset.maintenances[i].agentId)
                                {
                                    _ = tableMaintenances.Rows.Add(DateTime.ParseExact(existingAsset.maintenances[i].serviceDate, GenericResources.DATE_FORMAT, CultureInfo.InvariantCulture).ToString(GenericResources.DATE_DISPLAY), StringsAndConstants.LIST_SERVICE_TYPE_GUI[Convert.ToInt32(existingAsset.maintenances[i].serviceType)], agentMaintenances.name + " " + agentMaintenances.surname);
                                }
                            }
                            tableMaintenances.Visible = true;
                            tableMaintenances.Sort(tableMaintenances.Columns["serviceDate"], ListSortDirection.Descending);
                        }
                        //If asset does not exist on the database
                        catch (InvalidAssetException)
                        {
                            loadingCircleLastService.Visible = false;
                            loadingCircleLastService.Active = false;

                            radioButtonUpdateData.Enabled = false;
                            lblColorLastService.Text = Misc.MiscMethods.SinceLabelUpdate(string.Empty);
                            lblColorLastService.ForeColor = StringsAndConstants.OFFLINE_ALERT;
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), lblColorLastService.Text, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                            lblThereIsNothingHere.Visible = true;
                        }
                        //If server is unreachable
                        catch (HttpRequestException)
                        {
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                            tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);

                            _ = MessageBox.Show(AirUIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                        _ = MessageBox.Show(AirUIStrings.DATABASE_REACH_ERROR, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            registerButton.Text = AirUIStrings.REGISTER_AGAIN;
            registerButton.Enabled = true;
            apcsButton.Enabled = true;
            collectButton.Enabled = true;
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
            isSystemDarkModeEnabled = true;
        }

        /// <summary> 
        /// Method for opening the Storage list form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageDetailsButton_Click(object sender, EventArgs e)
        {
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WINDOWS_10))
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
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WINDOWS_10))
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
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WINDOWS_10))
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
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WINDOWS_10))
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
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WINDOWS_10))
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
            System.Diagnostics.Process.Start(configOptions.Definitions.LogLocation + GenericResources.LOG_FILENAME_AIR + "-v" + Application.ProductVersion + "-" + AirResources.DEV_STATUS + GenericResources.LOG_FILE_EXT);
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

