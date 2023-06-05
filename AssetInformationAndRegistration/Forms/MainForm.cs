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

namespace AssetInformationAndRegistration.Forms
{
    ///<summary>Class for handling Main window tasks and UI</summary>
    internal partial class MainForm : Form, ITheming
    {
        private int percent, i = 0, leftBound, rightBound;
        private int xPosOS = 0, yPosOS = 0;
        private int xPosFwVersion = 0, yPosFwVersion = 0;
        private int xPosVideoCard = 0, yPosVideoCard = 0;
        private int xPosRam = 0, yPosRam = 0;
        private int xPosProcessor = 0, yPosProcessor = 0;
        private bool themeBool, serverOnline, pass = true, invertOSScroll, invertFwVersionScroll, invertVideoCardScroll, invertRamScroll, invertProcessorScroll;
        private readonly bool offlineMode;
        private readonly string serverIP, serverPort;
        private string serviceTypeURL, brand, model, serialNumber, processor, ram, storageSize, storageType, mediaOperationMode, videoCard, operatingSystem, hostname, macAddress, ipAddress, fwVersion, fwType, secureBoot, virtualizationTechnology, smartStatus, tpmVersion;
        private readonly string[] serverArgs = new string[34], agentData = new string[2];
        private readonly List<string[]> parametersList, jsonServerSettings;
        private readonly List<string> enforcementList, orgDataList;

        ///<summary>Main form constructor</summary>
        ///<param name="offlineMode">Offline mode set</param>
        ///<param name="agentData">Agent name and id gotten from the Login form</param>
        ///<param name="serverIP">Server IP address</param>
        ///<param name="serverPort">Server port</param>
        ///<param name="log">Log file object</param>
        ///<param name="parametersList">List containing data from [Parameters]</param>
        ///<param name="enforcementList">List containing data from [Enforcement]</param>
        ///<param name="orgDataList">List containing data from [OrgData]</param>
        internal MainForm(bool offlineMode, string[] agentData, string serverIP, string serverPort, LogGenerator log, List<string[]> parametersList, List<string> enforcementList, List<string> orgDataList)
        {
            //Inits WinForms components
            InitializeComponent();

            //Program version
#if DEBUG
            toolStripVersionText.Text = MiscMethods.Version(Resources.DEV_STATUS); //Debug/Beta version
#else
            toolStripVersionText.Text = MiscMethods.Version(); //Release/Final version
#endif
            //Define theming according to ini file provided info
            if (StringsAndConstants.LIST_THEME_GUI.Contains(parametersList[3][0].ToString()) && parametersList[3][0].ToString().Equals(StringsAndConstants.LIST_THEME_GUI[0]))
            {
                themeBool = MiscMethods.ThemeInit();
                if (themeBool)
                {
                    if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                    }
                    DarkTheme();
                }
                else
                {
                    if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                    }
                    LightTheme();
                }
            }
            else if (parametersList[3][0].ToString().Equals(StringsAndConstants.LIST_THEME_GUI[1]))
            {
                comboBoxThemeButton.Enabled = false;
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                }
                LightTheme();
                themeBool = false;
            }
            else if (parametersList[3][0].ToString().Equals(StringsAndConstants.LIST_THEME_GUI[2]))
            {
                comboBoxThemeButton.Enabled = false;
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
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
            ApcsButton = new System.Windows.Forms.Button();
            lblFixedFwType = new System.Windows.Forms.Label();
            lblFwType = new System.Windows.Forms.Label();
            groupBoxHwData = new System.Windows.Forms.GroupBox();
            loadingCircleTpmVersion = new MRG.Controls.UI.LoadingCircle();
            loadingCircleVirtualizationTechnology = new MRG.Controls.UI.LoadingCircle();
            loadingCircleSecureBoot = new MRG.Controls.UI.LoadingCircle();
            loadingCircleFwVersion = new MRG.Controls.UI.LoadingCircle();
            loadingCircleFwType = new MRG.Controls.UI.LoadingCircle();
            loadingCircleIpAddress = new MRG.Controls.UI.LoadingCircle();
            loadingCircleMacAddress = new MRG.Controls.UI.LoadingCircle();
            loadingCircleHostname = new MRG.Controls.UI.LoadingCircle();
            loadingCircleOperatingSystem = new MRG.Controls.UI.LoadingCircle();
            loadingCircleVideoCard = new MRG.Controls.UI.LoadingCircle();
            loadingCircleMediaOperationMode = new MRG.Controls.UI.LoadingCircle();
            loadingCircleStorageType = new MRG.Controls.UI.LoadingCircle();
            loadingCircleSmartStatus = new MRG.Controls.UI.LoadingCircle();
            loadingCircleStorageSize = new MRG.Controls.UI.LoadingCircle();
            loadingCircleRam = new MRG.Controls.UI.LoadingCircle();
            loadingCircleProcessor = new MRG.Controls.UI.LoadingCircle();
            loadingCircleSerialNumber = new MRG.Controls.UI.LoadingCircle();
            loadingCircleModel = new MRG.Controls.UI.LoadingCircle();
            loadingCircleBrand = new MRG.Controls.UI.LoadingCircle();
            separatorH = new System.Windows.Forms.Label();
            separatorV = new System.Windows.Forms.Label();
            iconImgTpmVersion = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            iconImgSmartStatus = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblSmartStatus = new System.Windows.Forms.Label();
            lblTpmVersion = new System.Windows.Forms.Label();
            lblFixedSmartStatus = new System.Windows.Forms.Label();
            iconImgVirtualizationTechnology = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            lblFixedTpmVersion = new System.Windows.Forms.Label();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            lblProgressBarPercent = new System.Windows.Forms.Label();
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
            comboBoxBatteryChange = new CustomFlatComboBox();
            comboBoxStandard = new CustomFlatComboBox();
            comboBoxActiveDirectory = new CustomFlatComboBox();
            comboBoxTag = new CustomFlatComboBox();
            comboBoxInUse = new CustomFlatComboBox();
            comboBoxHwType = new CustomFlatComboBox();
            comboBoxBuilding = new CustomFlatComboBox();
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
            groupBoxServiceType = new System.Windows.Forms.GroupBox();
            loadingCircleMaintenance = new MRG.Controls.UI.LoadingCircle();
            loadingCircleFormatting = new MRG.Controls.UI.LoadingCircle();
            lblMaintenanceSince = new System.Windows.Forms.Label();
            lblInstallSince = new System.Windows.Forms.Label();
            lblFixedMandatoryServiceType = new System.Windows.Forms.Label();
            textBoxFixedFormattingRadio = new System.Windows.Forms.TextBox();
            textBoxFixedMaintenanceRadio = new System.Windows.Forms.TextBox();
            radioButtonFormatting = new System.Windows.Forms.RadioButton();
            radioButtonMaintenance = new System.Windows.Forms.RadioButton();
            lblFixedAdRegistered = new System.Windows.Forms.Label();
            lblFixedStandard = new System.Windows.Forms.Label();
            lblAgentName = new System.Windows.Forms.Label();
            lblFixedAgentName = new System.Windows.Forms.Label();
            lblServerPort = new System.Windows.Forms.Label();
            lblServerIP = new System.Windows.Forms.Label();
            lblFixedServerIP = new System.Windows.Forms.Label();
            lblServerOperationalStatus = new System.Windows.Forms.Label();
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
            groupBoxHwData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)iconImgTpmVersion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconImgSmartStatus).BeginInit();
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
            // ApcsButton
            // 
            ApcsButton.BackColor = System.Drawing.SystemColors.Control;
            ApcsButton.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(ApcsButton, "ApcsButton");
            ApcsButton.Name = "ApcsButton";
            ApcsButton.UseVisualStyleBackColor = true;
            ApcsButton.Click += new System.EventHandler(ApcsButton_Click);
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
            groupBoxHwData.Controls.Add(loadingCircleTpmVersion);
            groupBoxHwData.Controls.Add(loadingCircleVirtualizationTechnology);
            groupBoxHwData.Controls.Add(loadingCircleSecureBoot);
            groupBoxHwData.Controls.Add(loadingCircleFwVersion);
            groupBoxHwData.Controls.Add(loadingCircleFwType);
            groupBoxHwData.Controls.Add(loadingCircleIpAddress);
            groupBoxHwData.Controls.Add(loadingCircleMacAddress);
            groupBoxHwData.Controls.Add(loadingCircleHostname);
            groupBoxHwData.Controls.Add(loadingCircleOperatingSystem);
            groupBoxHwData.Controls.Add(loadingCircleVideoCard);
            groupBoxHwData.Controls.Add(loadingCircleMediaOperationMode);
            groupBoxHwData.Controls.Add(loadingCircleStorageType);
            groupBoxHwData.Controls.Add(loadingCircleSmartStatus);
            groupBoxHwData.Controls.Add(loadingCircleStorageSize);
            groupBoxHwData.Controls.Add(loadingCircleRam);
            groupBoxHwData.Controls.Add(loadingCircleProcessor);
            groupBoxHwData.Controls.Add(loadingCircleSerialNumber);
            groupBoxHwData.Controls.Add(loadingCircleModel);
            groupBoxHwData.Controls.Add(loadingCircleBrand);
            groupBoxHwData.Controls.Add(separatorH);
            groupBoxHwData.Controls.Add(separatorV);
            groupBoxHwData.Controls.Add(iconImgTpmVersion);
            groupBoxHwData.Controls.Add(iconImgSmartStatus);
            groupBoxHwData.Controls.Add(lblSmartStatus);
            groupBoxHwData.Controls.Add(lblTpmVersion);
            groupBoxHwData.Controls.Add(lblFixedSmartStatus);
            groupBoxHwData.Controls.Add(iconImgVirtualizationTechnology);
            groupBoxHwData.Controls.Add(lblFixedTpmVersion);
            groupBoxHwData.Controls.Add(progressBar1);
            groupBoxHwData.Controls.Add(lblProgressBarPercent);
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
            groupBoxHwData.Controls.Add(lblOperatingSystem);
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
            groupBoxHwData.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(groupBoxHwData, "groupBoxHwData");
            groupBoxHwData.Name = "groupBoxHwData";
            groupBoxHwData.TabStop = false;
            // 
            // loadingCircleTpmVersion
            // 
            loadingCircleTpmVersion.Active = false;
            loadingCircleTpmVersion.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleTpmVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleTpmVersion.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleTpmVersion, "loadingCircleTpmVersion");
            loadingCircleTpmVersion.Name = "loadingCircleTpmVersion";
            loadingCircleTpmVersion.NumberSpoke = 12;
            loadingCircleTpmVersion.OuterCircleRadius = 11;
            loadingCircleTpmVersion.RotationSpeed = 1;
            loadingCircleTpmVersion.SpokeThickness = 2;
            loadingCircleTpmVersion.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleVirtualizationTechnology
            // 
            loadingCircleVirtualizationTechnology.Active = false;
            loadingCircleVirtualizationTechnology.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleVirtualizationTechnology.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleVirtualizationTechnology.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleVirtualizationTechnology, "loadingCircleVirtualizationTechnology");
            loadingCircleVirtualizationTechnology.Name = "loadingCircleVirtualizationTechnology";
            loadingCircleVirtualizationTechnology.NumberSpoke = 12;
            loadingCircleVirtualizationTechnology.OuterCircleRadius = 11;
            loadingCircleVirtualizationTechnology.RotationSpeed = 1;
            loadingCircleVirtualizationTechnology.SpokeThickness = 2;
            loadingCircleVirtualizationTechnology.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleSecureBoot
            // 
            loadingCircleSecureBoot.Active = false;
            loadingCircleSecureBoot.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleSecureBoot.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleSecureBoot.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleSecureBoot, "loadingCircleSecureBoot");
            loadingCircleSecureBoot.Name = "loadingCircleSecureBoot";
            loadingCircleSecureBoot.NumberSpoke = 12;
            loadingCircleSecureBoot.OuterCircleRadius = 11;
            loadingCircleSecureBoot.RotationSpeed = 1;
            loadingCircleSecureBoot.SpokeThickness = 2;
            loadingCircleSecureBoot.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleFwVersion
            // 
            loadingCircleFwVersion.Active = false;
            loadingCircleFwVersion.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleFwVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleFwVersion.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleFwVersion, "loadingCircleFwVersion");
            loadingCircleFwVersion.Name = "loadingCircleFwVersion";
            loadingCircleFwVersion.NumberSpoke = 12;
            loadingCircleFwVersion.OuterCircleRadius = 11;
            loadingCircleFwVersion.RotationSpeed = 1;
            loadingCircleFwVersion.SpokeThickness = 2;
            loadingCircleFwVersion.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleFwType
            // 
            loadingCircleFwType.Active = false;
            loadingCircleFwType.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleFwType.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleFwType.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleFwType, "loadingCircleFwType");
            loadingCircleFwType.Name = "loadingCircleFwType";
            loadingCircleFwType.NumberSpoke = 12;
            loadingCircleFwType.OuterCircleRadius = 11;
            loadingCircleFwType.RotationSpeed = 1;
            loadingCircleFwType.SpokeThickness = 2;
            loadingCircleFwType.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleIpAddress
            // 
            loadingCircleIpAddress.Active = false;
            loadingCircleIpAddress.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleIpAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleIpAddress.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleIpAddress, "loadingCircleIpAddress");
            loadingCircleIpAddress.Name = "loadingCircleIpAddress";
            loadingCircleIpAddress.NumberSpoke = 12;
            loadingCircleIpAddress.OuterCircleRadius = 11;
            loadingCircleIpAddress.RotationSpeed = 1;
            loadingCircleIpAddress.SpokeThickness = 2;
            loadingCircleIpAddress.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleMacAddress
            // 
            loadingCircleMacAddress.Active = false;
            loadingCircleMacAddress.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleMacAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleMacAddress.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleMacAddress, "loadingCircleMacAddress");
            loadingCircleMacAddress.Name = "loadingCircleMacAddress";
            loadingCircleMacAddress.NumberSpoke = 12;
            loadingCircleMacAddress.OuterCircleRadius = 11;
            loadingCircleMacAddress.RotationSpeed = 1;
            loadingCircleMacAddress.SpokeThickness = 2;
            loadingCircleMacAddress.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleHostname
            // 
            loadingCircleHostname.Active = false;
            loadingCircleHostname.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleHostname.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleHostname.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleHostname, "loadingCircleHostname");
            loadingCircleHostname.Name = "loadingCircleHostname";
            loadingCircleHostname.NumberSpoke = 12;
            loadingCircleHostname.OuterCircleRadius = 11;
            loadingCircleHostname.RotationSpeed = 1;
            loadingCircleHostname.SpokeThickness = 2;
            loadingCircleHostname.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleOperatingSystem
            // 
            loadingCircleOperatingSystem.Active = false;
            loadingCircleOperatingSystem.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleOperatingSystem.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleOperatingSystem.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleOperatingSystem, "loadingCircleOperatingSystem");
            loadingCircleOperatingSystem.Name = "loadingCircleOperatingSystem";
            loadingCircleOperatingSystem.NumberSpoke = 12;
            loadingCircleOperatingSystem.OuterCircleRadius = 11;
            loadingCircleOperatingSystem.RotationSpeed = 1;
            loadingCircleOperatingSystem.SpokeThickness = 2;
            loadingCircleOperatingSystem.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleVideoCard
            // 
            loadingCircleVideoCard.Active = false;
            loadingCircleVideoCard.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleVideoCard.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleVideoCard.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleVideoCard, "loadingCircleVideoCard");
            loadingCircleVideoCard.Name = "loadingCircleVideoCard";
            loadingCircleVideoCard.NumberSpoke = 12;
            loadingCircleVideoCard.OuterCircleRadius = 11;
            loadingCircleVideoCard.RotationSpeed = 1;
            loadingCircleVideoCard.SpokeThickness = 2;
            loadingCircleVideoCard.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleMediaOperationMode
            // 
            loadingCircleMediaOperationMode.Active = false;
            loadingCircleMediaOperationMode.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleMediaOperationMode.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleMediaOperationMode.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleMediaOperationMode, "loadingCircleMediaOperationMode");
            loadingCircleMediaOperationMode.Name = "loadingCircleMediaOperationMode";
            loadingCircleMediaOperationMode.NumberSpoke = 12;
            loadingCircleMediaOperationMode.OuterCircleRadius = 11;
            loadingCircleMediaOperationMode.RotationSpeed = 1;
            loadingCircleMediaOperationMode.SpokeThickness = 2;
            loadingCircleMediaOperationMode.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleStorageType
            // 
            loadingCircleStorageType.Active = false;
            loadingCircleStorageType.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleStorageType.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleStorageType.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleStorageType, "loadingCircleStorageType");
            loadingCircleStorageType.Name = "loadingCircleStorageType";
            loadingCircleStorageType.NumberSpoke = 12;
            loadingCircleStorageType.OuterCircleRadius = 11;
            loadingCircleStorageType.RotationSpeed = 1;
            loadingCircleStorageType.SpokeThickness = 2;
            loadingCircleStorageType.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleSmartStatus
            // 
            loadingCircleSmartStatus.Active = false;
            loadingCircleSmartStatus.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleSmartStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleSmartStatus.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleSmartStatus, "loadingCircleSmartStatus");
            loadingCircleSmartStatus.Name = "loadingCircleSmartStatus";
            loadingCircleSmartStatus.NumberSpoke = 12;
            loadingCircleSmartStatus.OuterCircleRadius = 11;
            loadingCircleSmartStatus.RotationSpeed = 1;
            loadingCircleSmartStatus.SpokeThickness = 2;
            loadingCircleSmartStatus.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleStorageSize
            // 
            loadingCircleStorageSize.Active = false;
            loadingCircleStorageSize.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleStorageSize.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleStorageSize.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleStorageSize, "loadingCircleStorageSize");
            loadingCircleStorageSize.Name = "loadingCircleStorageSize";
            loadingCircleStorageSize.NumberSpoke = 12;
            loadingCircleStorageSize.OuterCircleRadius = 11;
            loadingCircleStorageSize.RotationSpeed = 1;
            loadingCircleStorageSize.SpokeThickness = 2;
            loadingCircleStorageSize.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleRam
            // 
            loadingCircleRam.Active = false;
            loadingCircleRam.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleRam.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleRam.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleRam, "loadingCircleRam");
            loadingCircleRam.Name = "loadingCircleRam";
            loadingCircleRam.NumberSpoke = 12;
            loadingCircleRam.OuterCircleRadius = 11;
            loadingCircleRam.RotationSpeed = 1;
            loadingCircleRam.SpokeThickness = 2;
            loadingCircleRam.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleProcessor
            // 
            loadingCircleProcessor.Active = false;
            loadingCircleProcessor.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleProcessor.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleProcessor.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleProcessor, "loadingCircleProcessor");
            loadingCircleProcessor.Name = "loadingCircleProcessor";
            loadingCircleProcessor.NumberSpoke = 12;
            loadingCircleProcessor.OuterCircleRadius = 11;
            loadingCircleProcessor.RotationSpeed = 1;
            loadingCircleProcessor.SpokeThickness = 2;
            loadingCircleProcessor.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleSerialNumber
            // 
            loadingCircleSerialNumber.Active = false;
            loadingCircleSerialNumber.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleSerialNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleSerialNumber.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleSerialNumber, "loadingCircleSerialNumber");
            loadingCircleSerialNumber.Name = "loadingCircleSerialNumber";
            loadingCircleSerialNumber.NumberSpoke = 12;
            loadingCircleSerialNumber.OuterCircleRadius = 11;
            loadingCircleSerialNumber.RotationSpeed = 1;
            loadingCircleSerialNumber.SpokeThickness = 2;
            loadingCircleSerialNumber.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleModel
            // 
            loadingCircleModel.Active = false;
            loadingCircleModel.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleModel.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleModel.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleModel, "loadingCircleModel");
            loadingCircleModel.Name = "loadingCircleModel";
            loadingCircleModel.NumberSpoke = 12;
            loadingCircleModel.OuterCircleRadius = 11;
            loadingCircleModel.RotationSpeed = 1;
            loadingCircleModel.SpokeThickness = 2;
            loadingCircleModel.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleBrand
            // 
            loadingCircleBrand.Active = false;
            loadingCircleBrand.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleBrand.ForeColor = System.Drawing.SystemColors.ControlText;
            loadingCircleBrand.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleBrand, "loadingCircleBrand");
            loadingCircleBrand.Name = "loadingCircleBrand";
            loadingCircleBrand.NumberSpoke = 12;
            loadingCircleBrand.OuterCircleRadius = 11;
            loadingCircleBrand.RotationSpeed = 1;
            loadingCircleBrand.SpokeThickness = 2;
            loadingCircleBrand.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // separatorH
            // 
            separatorH.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(separatorH, "separatorH");
            separatorH.Name = "separatorH";
            // 
            // separatorV
            // 
            separatorV.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(separatorV, "separatorV");
            separatorV.Name = "separatorV";
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
            // iconImgSmartStatus
            // 
            iconImgSmartStatus.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(iconImgSmartStatus, "iconImgSmartStatus");
            iconImgSmartStatus.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            iconImgSmartStatus.Name = "iconImgSmartStatus";
            iconImgSmartStatus.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            iconImgSmartStatus.TabStop = false;
            // 
            // lblSmartStatus
            // 
            resources.ApplyResources(lblSmartStatus, "lblSmartStatus");
            lblSmartStatus.ForeColor = System.Drawing.Color.Silver;
            lblSmartStatus.Name = "lblSmartStatus";
            // 
            // lblTpmVersion
            // 
            resources.ApplyResources(lblTpmVersion, "lblTpmVersion");
            lblTpmVersion.ForeColor = System.Drawing.Color.Silver;
            lblTpmVersion.Name = "lblTpmVersion";
            // 
            // lblFixedSmartStatus
            // 
            resources.ApplyResources(lblFixedSmartStatus, "lblFixedSmartStatus");
            lblFixedSmartStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            lblFixedSmartStatus.Name = "lblFixedSmartStatus";
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
            // lblProgressBarPercent
            // 
            resources.ApplyResources(lblProgressBarPercent, "lblProgressBarPercent");
            lblProgressBarPercent.BackColor = System.Drawing.Color.Transparent;
            lblProgressBarPercent.ForeColor = System.Drawing.SystemColors.ControlText;
            lblProgressBarPercent.Name = "lblProgressBarPercent";
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
            groupBoxAssetData.Controls.Add(groupBoxServiceType);
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
            comboBoxActiveDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
            // groupBoxServiceType
            // 
            groupBoxServiceType.Controls.Add(loadingCircleMaintenance);
            groupBoxServiceType.Controls.Add(loadingCircleFormatting);
            groupBoxServiceType.Controls.Add(lblMaintenanceSince);
            groupBoxServiceType.Controls.Add(lblInstallSince);
            groupBoxServiceType.Controls.Add(lblFixedMandatoryServiceType);
            groupBoxServiceType.Controls.Add(textBoxFixedFormattingRadio);
            groupBoxServiceType.Controls.Add(textBoxFixedMaintenanceRadio);
            groupBoxServiceType.Controls.Add(radioButtonFormatting);
            groupBoxServiceType.Controls.Add(radioButtonMaintenance);
            groupBoxServiceType.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(groupBoxServiceType, "groupBoxServiceType");
            groupBoxServiceType.Name = "groupBoxServiceType";
            groupBoxServiceType.TabStop = false;
            // 
            // loadingCircleMaintenance
            // 
            loadingCircleMaintenance.Active = false;
            loadingCircleMaintenance.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleMaintenance.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleMaintenance, "loadingCircleMaintenance");
            loadingCircleMaintenance.Name = "loadingCircleMaintenance";
            loadingCircleMaintenance.NumberSpoke = 12;
            loadingCircleMaintenance.OuterCircleRadius = 11;
            loadingCircleMaintenance.RotationSpeed = 1;
            loadingCircleMaintenance.SpokeThickness = 2;
            loadingCircleMaintenance.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleFormatting
            // 
            loadingCircleFormatting.Active = false;
            loadingCircleFormatting.Color = System.Drawing.Color.LightSlateGray;
            loadingCircleFormatting.InnerCircleRadius = 5;
            resources.ApplyResources(loadingCircleFormatting, "loadingCircleFormatting");
            loadingCircleFormatting.Name = "loadingCircleFormatting";
            loadingCircleFormatting.NumberSpoke = 12;
            loadingCircleFormatting.OuterCircleRadius = 11;
            loadingCircleFormatting.RotationSpeed = 1;
            loadingCircleFormatting.SpokeThickness = 2;
            loadingCircleFormatting.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
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
            // lblFixedMandatoryServiceType
            // 
            resources.ApplyResources(lblFixedMandatoryServiceType, "lblFixedMandatoryServiceType");
            lblFixedMandatoryServiceType.ForeColor = System.Drawing.Color.Red;
            lblFixedMandatoryServiceType.Name = "lblFixedMandatoryServiceType";
            // 
            // textBoxFixedFormattingRadio
            // 
            textBoxFixedFormattingRadio.BackColor = System.Drawing.SystemColors.Control;
            textBoxFixedFormattingRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(textBoxFixedFormattingRadio, "textBoxFixedFormattingRadio");
            textBoxFixedFormattingRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxFixedFormattingRadio.Name = "textBoxFixedFormattingRadio";
            textBoxFixedFormattingRadio.ReadOnly = true;
            // 
            // textBoxFixedMaintenanceRadio
            // 
            textBoxFixedMaintenanceRadio.BackColor = System.Drawing.SystemColors.Control;
            textBoxFixedMaintenanceRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(textBoxFixedMaintenanceRadio, "textBoxFixedMaintenanceRadio");
            textBoxFixedMaintenanceRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            textBoxFixedMaintenanceRadio.Name = "textBoxFixedMaintenanceRadio";
            textBoxFixedMaintenanceRadio.ReadOnly = true;
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
            // lblServerOperationalStatus
            // 
            resources.ApplyResources(lblServerOperationalStatus, "lblServerOperationalStatus");
            lblServerOperationalStatus.BackColor = System.Drawing.Color.Transparent;
            lblServerOperationalStatus.ForeColor = System.Drawing.Color.Silver;
            lblServerOperationalStatus.Name = "lblServerOperationalStatus";
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
            loadingCircleCollectButton.BackColor = System.Drawing.SystemColors.Control;
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
            loadingCircleRegisterButton.BackColor = System.Drawing.SystemColors.Control;
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
            groupBoxServerStatus.Controls.Add(lblServerOperationalStatus);
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
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.Control;
            Controls.Add(groupBoxServerStatus);
            Controls.Add(loadingCircleRegisterButton);
            Controls.Add(loadingCircleCollectButton);
            Controls.Add(groupBoxRegistryStatus);
            Controls.Add(groupBoxAssetData);
            Controls.Add(groupBoxHwData);
            Controls.Add(imgTopBanner);
            Controls.Add(ApcsButton);
            Controls.Add(collectButton);
            Controls.Add(statusStrip1);
            Controls.Add(registerButton);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            Load += new System.EventHandler(MainForm_Load);
            groupBoxHwData.ResumeLayout(false);
            groupBoxHwData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)iconImgTpmVersion).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconImgSmartStatus).EndInit();
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

        #region Variables being declared
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
        private Timer timerFwVersionLabelScroll;
        private Timer timerVideoCardLabelScroll;
        private Timer timerRamLabelScroll;
        private Timer timerProcessorLabelScroll;
        private Timer timerOSLabelScroll;

        #endregion

        ///<summary>Sets service mode to format</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void FormatButton1_CheckedChanged(object sender, EventArgs e)
        {
            serviceTypeURL = ConstantsDLL.Properties.Resources.FORMAT_URL;
        }

        ///<summary>Sets service mode to maintenance</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void MaintenanceButton2_CheckedChanged(object sender, EventArgs e)
        {
            serviceTypeURL = ConstantsDLL.Properties.Resources.MAINTENANCE_URL;
        }

        ///<summary>Method for auto selecting the app theme</summary>
        private void ComboBoxThemeInit()
        {
            themeBool = MiscMethods.ThemeInit();
            if (themeBool)
            {
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10)) //If Windows 10/11
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark); //Sets context menus to dark
                }
                DarkTheme(); //Sets dark theme
            }
            else
            {
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10)) //If Windows 10/11
                {
                    DarkNet.Instance.SetCurrentProcessTheme(Theme.Light); //Sets context menus to light
                }
                LightTheme(); //Sets light theme
            }
        }

        ///<summary>Method for setting the auto theme via toolStrip</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_AUTOTHEME_CHANGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            ComboBoxThemeInit();
        }

        ///<summary>Method for setting the light theme via toolStrip</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_LIGHTMODE_CHANGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
            {
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
            }
            LightTheme();
            themeBool = false;
        }

        ///<summary>Method for setting the dark theme via toolStrip</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_DARKMODE_CHANGE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
            {
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
            }
            DarkTheme();
            themeBool = true;
        }

        public void LightTheme()
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
            iconImgSmartStatus.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SMART_LIGHT_PATH));
            iconImgTpmVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_TPM_LIGHT_PATH));
            iconImgBatteryChange.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_CMOS_BATTERY_LIGHT_PATH));
            iconImgTicketNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_TICKET_LIGHT_PATH));
        }

        public void DarkTheme()
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
            iconImgSmartStatus.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SMART_DARK_PATH));
            iconImgTpmVersion.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_TPM_DARK_PATH));
            iconImgBatteryChange.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_CMOS_BATTERY_DARK_PATH));
            iconImgTicketNumber.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_TICKET_DARK_PATH));
        }

        ///<summary>Sets highlight about label when hovering with the mouse</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void AboutLabel_MouseEnter(object sender, EventArgs e)
        {
            aboutLabelButton.ForeColor = StringsAndConstants.HIGHLIGHT_LABEL_COLOR;
        }

        ///<summary>Allow to OS label to slide left to right (and vice versa) if it is longer than its parent groupbox width</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
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

        ///<summary>Allow to firmware version label to slide left to right (and vice versa) if it is longer than its parent groupbox width</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
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

        ///<summary>Allow to video card label to slide left to right (and vice versa) if it is longer than its parent groupbox width</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
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

        ///<summary>Allow to RAM label to slide left to right (and vice versa) if it is longer than its parent groupbox width</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
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

        ///<summary>Allow to processor label to slide left to right (and vice versa) if it is longer than its parent groupbox width</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
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

        ///<summary>Resets highlight about label when hovering with the mouse</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void AboutLabel_MouseLeave(object sender, EventArgs e)
        {
            aboutLabelButton.ForeColor = !themeBool ? StringsAndConstants.LIGHT_FORECOLOR : StringsAndConstants.DARK_FORECOLOR;
        }

        ///<summary>Sets highlight log label when hovering with the mouse</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void LogLabel_MouseEnter(object sender, EventArgs e)
        {
            logLabelButton.ForeColor = StringsAndConstants.HIGHLIGHT_LABEL_COLOR;
        }

        ///<summary>Resets highlight log label when hovering with the mouse</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void LogLabel_MouseLeave(object sender, EventArgs e)
        {
            logLabelButton.ForeColor = !themeBool ? StringsAndConstants.LIGHT_FORECOLOR : StringsAndConstants.DARK_FORECOLOR;
        }

        ///<summary>Opens the log file</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void LogLabelButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_OPENING_LOG, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
#if DEBUG
            System.Diagnostics.Process.Start(parametersList[2][0] + ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.DEV_STATUS + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#else
            System.Diagnostics.Process.Start(parametersList[2][0] + ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#endif
        }

        ///<summary>Opens the About box</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void AboutLabelButton_Click(object sender, EventArgs e)
        {
            AboutBox aboutForm = new AboutBox(log, parametersList, themeBool);
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
            {
                DarkNet.Instance.SetWindowThemeForms(aboutForm, Theme.Auto);
            }
            _ = aboutForm.ShowDialog();
        }

        ///<summary>Opens the selected webpage, according to the IP and port specified in the comboboxes</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void ApcsButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_VIEW_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            _ = System.Diagnostics.Process.Start(ConstantsDLL.Properties.Resources.HTTP + serverIP + ":" + serverPort);
        }

        ///<summary>Handles the closing of the current form</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_CLOSING_MAINFORM, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Deletes downloaded json files
            File.Delete(StringsAndConstants.MODEL_FILE_PATH);
            File.Delete(StringsAndConstants.CREDENTIALS_FILE_PATH);
            File.Delete(StringsAndConstants.ASSET_FILE_PATH);
            File.Delete(StringsAndConstants.CONFIG_FILE_PATH);

            //Kills Webview2 instance
            webView2Control.Dispose();
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        ///<summary>Loads the form, sets some combobox values, create timers (1000 ms cadence), and triggers a hardware collection</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        ///<returns>Returns a asynchronous task</returns>
        private async void MainForm_Load(object sender, EventArgs e)
        {
            #region Define loading circle parameters

            switch (MiscMethods.GetWindowsScaling())
            {
                case 100:
                    //Init loading circles parameters for 100% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    loadingCircleBrand.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleModel.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleSerialNumber.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleProcessor.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleRam.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleStorageSize.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleSmartStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleStorageType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleMediaOperationMode.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleVideoCard.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleOperatingSystem.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleHostname.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleMacAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleIpAddress.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleFwType.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleFwVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleSecureBoot.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleVirtualizationTechnology.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleTpmVersion.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleFormatting.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleMaintenance.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleCollectButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleRegisterButton.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleServerOperationalStatus.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleBrand.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleModel.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleSerialNumber.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleProcessor.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleRam.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleStorageSize.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleSmartStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleStorageType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleMediaOperationMode.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleVideoCard.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleOperatingSystem.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleHostname.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleMacAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleIpAddress.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleFwType.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleFwVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleSecureBoot.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleVirtualizationTechnology.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleTpmVersion.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleFormatting.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleMaintenance.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleCollectButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleRegisterButton.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleServerOperationalStatus.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleBrand.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleModel.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleSerialNumber.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleProcessor.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleRam.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleStorageSize.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleSmartStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleStorageType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleMediaOperationMode.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleVideoCard.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleOperatingSystem.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleHostname.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleMacAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleIpAddress.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleFwType.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleFwVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleSecureBoot.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleVirtualizationTechnology.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleTpmVersion.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleFormatting.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleMaintenance.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleCollectButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleRegisterButton.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleServerOperationalStatus.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleBrand.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleModel.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleSerialNumber.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleProcessor.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleRam.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleStorageSize.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleSmartStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleStorageType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleMediaOperationMode.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleVideoCard.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleOperatingSystem.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleHostname.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleMacAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleIpAddress.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleFwType.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleFwVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleSecureBoot.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleVirtualizationTechnology.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleTpmVersion.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleFormatting.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleMaintenance.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleCollectButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleRegisterButton.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    loadingCircleServerOperationalStatus.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    break;
            }

            #region Sets loading circle color and rotation speed
            loadingCircleBrand.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleModel.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleSerialNumber.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleProcessor.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleRam.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleStorageSize.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleSmartStatus.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleStorageType.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleMediaOperationMode.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleVideoCard.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleOperatingSystem.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleHostname.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleMacAddress.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleIpAddress.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleFwType.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleFwVersion.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleSecureBoot.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleVirtualizationTechnology.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleTpmVersion.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleFormatting.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleMaintenance.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleCollectButton.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleRegisterButton.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleServerOperationalStatus.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleBrand.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleModel.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleSerialNumber.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleProcessor.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleRam.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleStorageSize.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleSmartStatus.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleStorageType.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleMediaOperationMode.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleVideoCard.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleOperatingSystem.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleHostname.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleMacAddress.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleIpAddress.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleFwType.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleFwVersion.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleSecureBoot.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleVirtualizationTechnology.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleTpmVersion.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleFormatting.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleMaintenance.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleCollectButton.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleRegisterButton.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            loadingCircleServerOperationalStatus.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            #endregion

            #endregion

            //Sets current and maximum values for the progressbar
            progressBar1.Maximum = 19;
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
                bw = new BusyForm(themeBool)
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

            separatorV.BringToFront();

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
                lblAgentName.Text = agentData[1].ToUpper(); //Prints agent name
            }
            lblServerIP.Text = serverIP; //Prints IP address
            lblServerPort.Text = serverPort; //Prints port number
            dateTimePickerServiceDate.MaxDate = DateTime.Today; //Define max date of datetimepicker to current day
            FormClosing += MainForm_Closing; //Handles Form closing
            tbProgMain = TaskbarManager.Instance; //Handles taskbar progress bar
            CollectButton_Click(sender, e); //Start collecting
        }

        ///<summary>Restricts textbox4 only with chars</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void TextBoxCharsOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        ///<summary>Restricts textbox only with numbers</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void TextBoxNumbersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
            {
                e.Handled = true;
            }
        }

        ///<summary>Sets the Hostname label to flash in red</summary>
        ///<param name="myObject"></param>
        ///<param name="myEventArgs"></param>
        private void AlertFlashTextHostname(object myObject, EventArgs myEventArgs)
        {
            lblHostname.ForeColor = lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblHostname.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        ///<summary>Sets the MediaOperations label to flash in red</summary>
        ///<param name="myObject"></param>
        ///<param name="myEventArgs"></param>
        private void AlertFlashTextMediaOperationMode(object myobject, EventArgs myEventArgs)
        {
            lblMediaOperationMode.ForeColor = lblMediaOperationMode.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblMediaOperationMode.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        ///<summary>Sets the Secure Boot label to flash in red</summary>
        ///<param name="myObject"></param>
        ///<param name="myEventArgs"></param>
        private void AlertFlashTextSecureBoot(object myobject, EventArgs myEventArgs)
        {
            lblSecureBoot.ForeColor = lblSecureBoot.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblSecureBoot.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        ///<summary>Sets the VT label to flash in red</summary>
        ///<param name="myObject"></param>
        ///<param name="myEventArgs"></param>
        private void AlertFlashTextVirtualizationTechnology(object myobject, EventArgs myEventArgs)
        {
            lblVirtualizationTechnology.ForeColor = lblVirtualizationTechnology.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblVirtualizationTechnology.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        ///<summary>Sets the SMART label to flash in red</summary>
        ///<param name="myObject"></param>
        ///<param name="myEventArgs"></param>
        private void AlertFlashTextSmartStatus(object myobject, EventArgs myEventArgs)
        {
            lblSmartStatus.ForeColor = lblSmartStatus.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblSmartStatus.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        ///<summary>Sets the BIOS Version label to flash in red</summary>
        ///<param name="myObject"></param>
        ///<param name="myEventArgs"></param>
        private void FlashTextFwVersion(object myobject, EventArgs myEventArgs)
        {
            lblFwVersion.ForeColor = lblFwVersion.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblFwVersion.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        ///<summary>Sets the Mac and IP labels to flash in red</summary>
        ///<param name="myObject"></param>
        ///<param name="myEventArgs"></param>
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

        ///<summary>Sets the Firmware Type label to flash in red</summary>
        ///<param name="myObject"></param>
        ///<param name="myEventArgs"></param>
        private void FlashTextBIOSType(object myobject, EventArgs myEventArgs)
        {
            lblFwType.ForeColor = lblFwType.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblFwType.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        ///<summary>Sets the Physical Memory label to flash in red</summary>
        ///<param name="myObject"></param>
        ///<param name="myEventArgs"></param>
        private void AlertFlashTextRamAmount(object myobject, EventArgs myEventArgs)
        {
            lblRam.ForeColor = lblRam.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblRam.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        ///<summary>Sets the TPM label to flash in red</summary>
        ///<param name="myObject"></param>
        ///<param name="myEventArgs"></param>
        private void AlertFlashTextTpmVersion(object myobject, EventArgs myEventArgs)
        {
            lblTpmVersion.ForeColor = lblTpmVersion.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == true
                ? StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR
                : lblTpmVersion.ForeColor == StringsAndConstants.ALERT_COLOR && themeBool == false
                ? StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR
                : StringsAndConstants.ALERT_COLOR;
        }

        ///<summary>Starts the collection process</summary>
        ///<returns>Returns a asynchronous task</returns>
        private async void Collecting()
        {
            #region Writes a dash in the labels, while scanning the hardware
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

            #region Show loading circles while scanning the hardware
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

                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_PINGGING_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                //Feches model info from server
                serverOnline = await JsonFileReaderDLL.ModelFileReader.CheckHostMT(serverIP, serverPort);

                if (serverOnline && serverPort != string.Empty)
                {
                    loadingCircleServerOperationalStatus.Visible = false;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_ONLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    lblServerOperationalStatus.Text = Strings.ONLINE;
                    lblServerOperationalStatus.ForeColor = StringsAndConstants.ONLINE_ALERT;
                }
                else
                {
                    loadingCircleServerOperationalStatus.Visible = false;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_OFFLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
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

        ///<summary>Auxiliary method for progress bar</summary>
        ///<param name="k">Progress bar step</param>
        ///<returns>Percentage</returns>
        private int ProgressAuxFunction(int k)
        {
            return k * 100 / progressBar1.Maximum;
        }

        ///<summary>Runs on a separate thread, calling methods from the HardwareInfo class, and setting the variables, while reporting the progress to the progressbar</summary>
        ///<param name="worker"></param>
        private void CollectThread(BackgroundWorker worker)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_START_COLLECTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            i = 0;

            //Scans for PC maker
            brand = HardwareInfo.GetBrand();
            if (brand == ConstantsDLL.Properties.Resources.TO_BE_FILLED_BY_OEM || brand == string.Empty)
            {
                brand = HardwareInfo.GetBrandAlt();
            }
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_BM, brand, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for PC model
            model = HardwareInfo.GetModel();
            if (model == ConstantsDLL.Properties.Resources.TO_BE_FILLED_BY_OEM || model == string.Empty)
            {
                model = HardwareInfo.GetModelAlt();
                if (model == ConstantsDLL.Properties.Resources.TO_BE_FILLED_BY_OEM || model == string.Empty)
                {
                    model = ConstantsDLL.Properties.Strings.UNKNOWN;
                }
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
            storageSize = HardwareInfo.GetStorageSize();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_HDSIZE, storageSize, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for SMART status
            smartStatus = HardwareInfo.GetSMARTStatus();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_SMART, smartStatus, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Scans for Storage type
            storageType = HardwareInfo.GetStorageType();
            i++;
            worker.ReportProgress(ProgressAuxFunction(i));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MEDIATYPE, storageType, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

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

        ///<summary>Prints the collected data into the form labels, warning the agent when there are forbidden modes</summary>
        ///<returns>Returns a asynchronous task</returns>
        private async Task PrintHardwareData()
        {
            #region Hides loading circles after scanning the hardware
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

            #region Prints fetched data into labels
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
            lblSecureBoot.Text = StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[9][Convert.ToInt32(secureBoot)])];
            lblVirtualizationTechnology.Text = StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[10][Convert.ToInt32(virtualizationTechnology)])];
            lblTpmVersion.Text = parametersList[7][Convert.ToInt32(tpmVersion)];

            lblInstallSince.Text = MiscMethods.SinceLabelUpdate(true);
            lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
            #endregion

            pass = true;

            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), lblInstallSince.Text, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), lblMaintenanceSince.Text, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            if (!offlineMode)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_FETCHING_BIOSFILE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            }

            try
            {
                //Feches model info from server
                string[] modelJsonStr = await JsonFileReaderDLL.ModelFileReader.FetchInfoMT(brand, model, fwType, tpmVersion, mediaOperationMode, serverIP, serverPort);

                //If hostname is the default one and its enforcement is enabled
                if (enforcementList[3] == ConstantsDLL.Properties.Resources.TRUE && hostname.Equals(Strings.DEFAULT_HOSTNAME))
                {
                    pass = false;
                    lblHostname.Text += Strings.HOSTNAME_ALERT;
                    timerAlertHostname.Enabled = true;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.HOSTNAME_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                //If model Json file does exist, mediaOpMode enforcement is enabled, and the mode is incorrect
                if (enforcementList[2] == ConstantsDLL.Properties.Resources.TRUE && modelJsonStr != null && modelJsonStr[3].Equals(ConstantsDLL.Properties.Resources.FALSE))
                {
                    pass = false;
                    lblMediaOperationMode.Text += Strings.MEDIA_OPERATION_ALERT;
                    timerAlertMediaOperationMode.Enabled = true;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.MEDIA_OPERATION_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                //The section below contains the exception cases for Secure Boot enforcement, if it is enabled
                if (enforcementList[6] == ConstantsDLL.Properties.Resources.TRUE && StringsAndConstants.LIST_STATES[Convert.ToInt32(secureBoot)] == ConstantsDLL.Properties.Strings.DEACTIVATED)
                {
                    pass = false;
                    lblSecureBoot.Text += Strings.SECURE_BOOT_ALERT;
                    timerAlertSecureBoot.Enabled = true;
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
                        lblFwVersion.Text += Strings.BIOS_VERSION_ALERT;
                        timerAlertFwVersion.Enabled = true;
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.BIOS_VERSION_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    }
                }
                //If model Json file does exist, firmware type enforcement is enabled, and the type is incorrect
                if (enforcementList[4] == ConstantsDLL.Properties.Resources.TRUE && modelJsonStr != null && modelJsonStr[1].Equals(ConstantsDLL.Properties.Resources.FALSE))
                {
                    pass = false;
                    lblFwType.Text += Strings.FIRMWARE_TYPE_ALERT;
                    timerAlertFwType.Enabled = true;
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
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.VT_ALERT, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                //If Smart status is not OK and its enforcement is enabled
                if (enforcementList[1] == ConstantsDLL.Properties.Resources.TRUE && !smartStatus.Contains(ConstantsDLL.Properties.Resources.OK))
                {
                    pass = false;
                    lblSmartStatus.Text += Strings.SMART_FAIL;
                    timerAlertSmartStatus.Enabled = true;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.SMART_FAIL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                //If model Json file does exist, TPM enforcement is enabled, and TPM version is incorrect
                if (enforcementList[8] == ConstantsDLL.Properties.Resources.TRUE && modelJsonStr != null && modelJsonStr[2].Equals(ConstantsDLL.Properties.Resources.FALSE))
                {
                    pass = false;
                    lblTpmVersion.Text += Strings.TPM_ERROR;
                    timerAlertTpmVersion.Enabled = true;
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
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.NOT_ENOUGH_MEMORY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                //If RAM is more than 4GB and OS is x86, and its limit enforcement is enabled, shows an alert
                if (enforcementList[0] == ConstantsDLL.Properties.Resources.TRUE && d > 4.0 && !Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    lblRam.Text += Strings.TOO_MUCH_MEMORY;
                    timerAlertRamAmount.Enabled = true;
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.TOO_MUCH_MEMORY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }
                if (pass && !offlineMode)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_HARDWARE_PASSED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                }

                if (!pass)
                {
                    progressBar1.SetState(2);
                    tbProgMain.SetProgressState(TaskbarProgressBarState.Error, Handle);
                }
            }
            catch (Exception e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), e.Message, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            }
        }

        ///<summary>Triggers when the form opens, and when the agent clicks to collect</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void CollectButton_Click(object sender, EventArgs e)
        {
            progressBar1.SetState(1);
            tbProgMain.SetProgressState(TaskbarProgressBarState.Normal, Handle);
            webView2Control.Visible = false;
            Collecting();
            ApcsButton.Enabled = false;
            registerButton.Enabled = false;
            collectButton.Enabled = false;
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_START_THREAD, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            StartAsync(sender, e);
        }

        ///<summary>Starts the worker for threading</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void StartAsync(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        ///<summary>Runs the collectThread method in a separate thread</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            CollectThread(worker);
        }

        ///<summary>Draws the collection progress on the screen</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            percent = e.ProgressPercentage * progressBar1.Maximum / 100;
            tbProgMain.SetProgressValue(percent, progressBar1.Maximum);
            progressBar1.Value = percent;
            lblProgressBarPercent.Text = e.ProgressPercentage.ToString() + "%";
        }

        ///<summary>Runs when the collection ends, ending the thread</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        ///<returns>Returns a asynchronous task</returns>
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

        ///<summary>Attributes the data collected previously to the variables which will inside the URL for registration</summary>
        private void AttrHardwareData()
        {
            serverArgs[12] = brand;
            serverArgs[13] = model;
            serverArgs[14] = serialNumber;
            serverArgs[15] = processor;
            serverArgs[16] = ram;
            serverArgs[17] = storageSize;
            serverArgs[18] = storageType;
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

        ///<summary>Runs the registration for the website</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        private async void RegisterButton_ClickAsync(object sender, EventArgs e)
        {
            webView2Control.Visible = false;
            tbProgMain.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            loadingCircleRegisterButton.Visible = true;
            loadingCircleRegisterButton.Active = true;
            registerButton.Text = ConstantsDLL.Properties.Resources.DASH;
            registerButton.Enabled = false;
            ApcsButton.Enabled = false;
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


                //Feches asset number data from server
                string[] assetJsonStr = await JsonFileReaderDLL.AssetFileReader.FetchInfoMT(serverArgs[2], serverArgs[0], serverArgs[1]);

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
                        try //Tries to get the laster register date from the asset number to check if the chosen date is adequate
                        {
                            DateTime registerDate = DateTime.ParseExact(serverArgs[5], ConstantsDLL.Properties.Resources.DATE_FORMAT, CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(assetJsonStr[10], ConstantsDLL.Properties.Resources.DATE_FORMAT, CultureInfo.InvariantCulture);

                            if (registerDate >= lastRegisterDate) //If chosen date is greater or equal than the last format/maintenance date of the PC, let proceed
                            {
                                webView2Control.Visible = true;
                                SendData.ServerSendInfo(serverArgs, log, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI), webView2Control); //Send info to server
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                                if (radioButtonFormatting.Checked) //If the format radio button is checked
                                {
                                    MiscMethods.RegCreate(true, serverArgs[5]); //Create reg entries for format and maintenance
                                    lblInstallSince.Text = MiscMethods.SinceLabelUpdate(true);
                                    lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_RESETTING_INSTALLDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                                }
                                else if (radioButtonMaintenance.Checked) //If the maintenance radio button is checked
                                {
                                    MiscMethods.RegCreate(false, serverArgs[5]); //Create reg entry just for maintenance
                                    lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                                }
                                await Task.Delay(Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL) * 3);
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

                            if (radioButtonFormatting.Checked) //If the format radio button is checked
                            {
                                MiscMethods.RegCreate(true, serverArgs[5]); //Create reg entries for format and maintenance
                                lblInstallSince.Text = MiscMethods.SinceLabelUpdate(true);
                                lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_RESETTING_INSTALLDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                            }
                            else if (radioButtonMaintenance.Checked) //If the maintenance radio button is checked
                            {
                                MiscMethods.RegCreate(false, serverArgs[5]); //Create reg entry just for maintenance
                                lblMaintenanceSince.Text = MiscMethods.SinceLabelUpdate(false);
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                            }
                            await Task.Delay(Convert.ToInt32(ConstantsDLL.Properties.Resources.TIMER_INTERVAL) * 3);
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

