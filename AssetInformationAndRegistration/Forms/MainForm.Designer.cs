using AssetInformationAndRegistration.Misc;
using ConfigurableQualityPictureBoxDLL;
using Microsoft.WindowsAPICodePack.Taskbar;
using MRG.Controls.UI;
using System.ComponentModel;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    partial class MainForm
    {
        #region Código gerado pelo Windows Form Designer

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.hardwareChangeButton = new System.Windows.Forms.Button();
            this.lblNoticeHardwareChanged = new System.Windows.Forms.Label();
            this.vSeparator2 = new System.Windows.Forms.Label();
            this.processorDetailsButton = new System.Windows.Forms.Button();
            this.ramDetailsButton = new System.Windows.Forms.Button();
            this.videoCardDetailsButton = new System.Windows.Forms.Button();
            this.storageDetailsButton = new System.Windows.Forms.Button();
            this.vSeparator5 = new System.Windows.Forms.Label();
            this.vSeparator4 = new System.Windows.Forms.Label();
            this.vSeparator3 = new System.Windows.Forms.Label();
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
            this.loadingCircleCompliant = new MRG.Controls.UI.LoadingCircle();
            this.lblColorCompliant = new System.Windows.Forms.Label();
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
            this.lblStorageType = new System.Windows.Forms.Label();
            this.lblFixedStorageType = new System.Windows.Forms.Label();
            this.lblFixedVideoCard = new System.Windows.Forms.Label();
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
            this.hwUidToolTip = new System.Windows.Forms.ToolTip(this.components);
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
            this.textBoxAssetNumber.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxAssetNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.textBoxAssetNumber, "textBoxAssetNumber");
            this.textBoxAssetNumber.Name = "textBoxAssetNumber";
            this.textBoxAssetNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxSealNumber
            // 
            this.textBoxSealNumber.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxSealNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.textBoxSealNumber, "textBoxSealNumber");
            this.textBoxSealNumber.Name = "textBoxSealNumber";
            this.textBoxSealNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxRoomNumber
            // 
            this.textBoxRoomNumber.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxRoomNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.textBoxRoomNumber, "textBoxRoomNumber");
            this.textBoxRoomNumber.Name = "textBoxRoomNumber";
            this.textBoxRoomNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxNumbersOnly_KeyPress);
            // 
            // textBoxRoomLetter
            // 
            this.textBoxRoomLetter.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxRoomLetter.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxRoomLetter.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.textBoxRoomLetter, "textBoxRoomLetter");
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
            this.groupBoxHwData.Controls.Add(this.hardwareChangeButton);
            this.groupBoxHwData.Controls.Add(this.lblNoticeHardwareChanged);
            this.groupBoxHwData.Controls.Add(this.vSeparator2);
            this.groupBoxHwData.Controls.Add(this.processorDetailsButton);
            this.groupBoxHwData.Controls.Add(this.ramDetailsButton);
            this.groupBoxHwData.Controls.Add(this.videoCardDetailsButton);
            this.groupBoxHwData.Controls.Add(this.storageDetailsButton);
            this.groupBoxHwData.Controls.Add(this.vSeparator5);
            this.groupBoxHwData.Controls.Add(this.vSeparator4);
            this.groupBoxHwData.Controls.Add(this.vSeparator3);
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
            this.groupBoxHwData.Controls.Add(this.loadingCircleCompliant);
            this.groupBoxHwData.Controls.Add(this.lblColorCompliant);
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
            this.groupBoxHwData.Controls.Add(this.lblStorageType);
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
            this.groupBoxHwData.Controls.Add(this.lblFixedOperatingSystem);
            this.groupBoxHwData.Controls.Add(this.lblFixedHostname);
            this.groupBoxHwData.Controls.Add(this.lblFixedIpAddress);
            this.groupBoxHwData.Controls.Add(this.lblOperatingSystem);
            this.groupBoxHwData.Controls.Add(this.lblFixedStorageType);
            this.groupBoxHwData.Controls.Add(this.lblFixedVideoCard);
            this.groupBoxHwData.Controls.Add(this.lblFixedRam);
            this.groupBoxHwData.Controls.Add(this.lblFixedProcessor);
            this.groupBoxHwData.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.groupBoxHwData, "groupBoxHwData");
            this.groupBoxHwData.Name = "groupBoxHwData";
            this.groupBoxHwData.TabStop = false;
            // 
            // hardwareChangeButton
            // 
            resources.ApplyResources(this.hardwareChangeButton, "hardwareChangeButton");
            this.hardwareChangeButton.Name = "hardwareChangeButton";
            this.hardwareChangeButton.UseVisualStyleBackColor = true;
            this.hardwareChangeButton.Click += new System.EventHandler(this.HardwareChangeButton_Click);
            // 
            // lblNoticeHardwareChanged
            // 
            resources.ApplyResources(this.lblNoticeHardwareChanged, "lblNoticeHardwareChanged");
            this.lblNoticeHardwareChanged.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblNoticeHardwareChanged.Name = "lblNoticeHardwareChanged";
            // 
            // vSeparator2
            // 
            this.vSeparator2.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.vSeparator2, "vSeparator2");
            this.vSeparator2.Name = "vSeparator2";
            // 
            // processorDetailsButton
            // 
            resources.ApplyResources(this.processorDetailsButton, "processorDetailsButton");
            this.processorDetailsButton.Name = "processorDetailsButton";
            this.processorDetailsButton.UseVisualStyleBackColor = true;
            this.processorDetailsButton.Click += new System.EventHandler(this.ProcessorDetailsButton_Click);
            // 
            // ramDetailsButton
            // 
            resources.ApplyResources(this.ramDetailsButton, "ramDetailsButton");
            this.ramDetailsButton.Name = "ramDetailsButton";
            this.ramDetailsButton.UseVisualStyleBackColor = true;
            this.ramDetailsButton.Click += new System.EventHandler(this.RamDetailsButton_Click);
            // 
            // videoCardDetailsButton
            // 
            resources.ApplyResources(this.videoCardDetailsButton, "videoCardDetailsButton");
            this.videoCardDetailsButton.Name = "videoCardDetailsButton";
            this.videoCardDetailsButton.UseVisualStyleBackColor = true;
            this.videoCardDetailsButton.Click += new System.EventHandler(this.VideoCardDetailsButton_Click);
            // 
            // storageDetailsButton
            // 
            resources.ApplyResources(this.storageDetailsButton, "storageDetailsButton");
            this.storageDetailsButton.Name = "storageDetailsButton";
            this.storageDetailsButton.UseVisualStyleBackColor = true;
            this.storageDetailsButton.Click += new System.EventHandler(this.StorageDetailsButton_Click);
            // 
            // vSeparator5
            // 
            this.vSeparator5.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.vSeparator5, "vSeparator5");
            this.vSeparator5.Name = "vSeparator5";
            // 
            // vSeparator4
            // 
            this.vSeparator4.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.vSeparator4, "vSeparator4");
            this.vSeparator4.Name = "vSeparator4";
            // 
            // vSeparator3
            // 
            this.vSeparator3.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.vSeparator3, "vSeparator3");
            this.vSeparator3.Name = "vSeparator3";
            // 
            // vSeparator1
            // 
            this.vSeparator1.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.vSeparator1, "vSeparator1");
            this.vSeparator1.Name = "vSeparator1";
            // 
            // hSeparator5
            // 
            this.hSeparator5.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.hSeparator5, "hSeparator5");
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
            this.hSeparator1.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.hSeparator1, "hSeparator1");
            this.hSeparator1.Name = "hSeparator1";
            // 
            // hSeparator4
            // 
            this.hSeparator4.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.hSeparator4, "hSeparator4");
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
            this.hSeparator3.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.hSeparator3, "hSeparator3");
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
            this.hSeparator2.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.hSeparator2, "hSeparator2");
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
            this.lblFixedProgressBarPercent.BackColor = System.Drawing.Color.Transparent;
            this.lblFixedProgressBarPercent.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.lblFixedProgressBarPercent, "lblFixedProgressBarPercent");
            this.lblFixedProgressBarPercent.Name = "lblFixedProgressBarPercent";
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // loadingCircleCompliant
            // 
            this.loadingCircleCompliant.Active = false;
            this.loadingCircleCompliant.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleCompliant.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleCompliant, "loadingCircleCompliant");
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
            // loadingCircleScanTpmVersion
            // 
            this.loadingCircleScanTpmVersion.Active = false;
            this.loadingCircleScanTpmVersion.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanTpmVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanTpmVersion.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanTpmVersion, "loadingCircleScanTpmVersion");
            this.loadingCircleScanTpmVersion.Name = "loadingCircleScanTpmVersion";
            this.loadingCircleScanTpmVersion.NumberSpoke = 12;
            this.loadingCircleScanTpmVersion.OuterCircleRadius = 11;
            this.loadingCircleScanTpmVersion.RotationSpeed = 1;
            this.loadingCircleScanTpmVersion.SpokeThickness = 2;
            this.loadingCircleScanTpmVersion.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanVirtualizationTechnology
            // 
            this.loadingCircleScanVirtualizationTechnology.Active = false;
            this.loadingCircleScanVirtualizationTechnology.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanVirtualizationTechnology.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanVirtualizationTechnology.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanVirtualizationTechnology, "loadingCircleScanVirtualizationTechnology");
            this.loadingCircleScanVirtualizationTechnology.Name = "loadingCircleScanVirtualizationTechnology";
            this.loadingCircleScanVirtualizationTechnology.NumberSpoke = 12;
            this.loadingCircleScanVirtualizationTechnology.OuterCircleRadius = 11;
            this.loadingCircleScanVirtualizationTechnology.RotationSpeed = 1;
            this.loadingCircleScanVirtualizationTechnology.SpokeThickness = 2;
            this.loadingCircleScanVirtualizationTechnology.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanSecureBoot
            // 
            this.loadingCircleScanSecureBoot.Active = false;
            this.loadingCircleScanSecureBoot.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanSecureBoot.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanSecureBoot.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanSecureBoot, "loadingCircleScanSecureBoot");
            this.loadingCircleScanSecureBoot.Name = "loadingCircleScanSecureBoot";
            this.loadingCircleScanSecureBoot.NumberSpoke = 12;
            this.loadingCircleScanSecureBoot.OuterCircleRadius = 11;
            this.loadingCircleScanSecureBoot.RotationSpeed = 1;
            this.loadingCircleScanSecureBoot.SpokeThickness = 2;
            this.loadingCircleScanSecureBoot.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanFwVersion
            // 
            this.loadingCircleScanFwVersion.Active = false;
            this.loadingCircleScanFwVersion.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanFwVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanFwVersion.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanFwVersion, "loadingCircleScanFwVersion");
            this.loadingCircleScanFwVersion.Name = "loadingCircleScanFwVersion";
            this.loadingCircleScanFwVersion.NumberSpoke = 12;
            this.loadingCircleScanFwVersion.OuterCircleRadius = 11;
            this.loadingCircleScanFwVersion.RotationSpeed = 1;
            this.loadingCircleScanFwVersion.SpokeThickness = 2;
            this.loadingCircleScanFwVersion.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanFwType
            // 
            this.loadingCircleScanFwType.Active = false;
            this.loadingCircleScanFwType.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanFwType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanFwType.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanFwType, "loadingCircleScanFwType");
            this.loadingCircleScanFwType.Name = "loadingCircleScanFwType";
            this.loadingCircleScanFwType.NumberSpoke = 12;
            this.loadingCircleScanFwType.OuterCircleRadius = 11;
            this.loadingCircleScanFwType.RotationSpeed = 1;
            this.loadingCircleScanFwType.SpokeThickness = 2;
            this.loadingCircleScanFwType.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanIpAddress
            // 
            this.loadingCircleScanIpAddress.Active = false;
            this.loadingCircleScanIpAddress.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanIpAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanIpAddress.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanIpAddress, "loadingCircleScanIpAddress");
            this.loadingCircleScanIpAddress.Name = "loadingCircleScanIpAddress";
            this.loadingCircleScanIpAddress.NumberSpoke = 12;
            this.loadingCircleScanIpAddress.OuterCircleRadius = 11;
            this.loadingCircleScanIpAddress.RotationSpeed = 1;
            this.loadingCircleScanIpAddress.SpokeThickness = 2;
            this.loadingCircleScanIpAddress.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanHostname
            // 
            this.loadingCircleScanHostname.Active = false;
            this.loadingCircleScanHostname.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanHostname.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanHostname.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanHostname, "loadingCircleScanHostname");
            this.loadingCircleScanHostname.Name = "loadingCircleScanHostname";
            this.loadingCircleScanHostname.NumberSpoke = 12;
            this.loadingCircleScanHostname.OuterCircleRadius = 11;
            this.loadingCircleScanHostname.RotationSpeed = 1;
            this.loadingCircleScanHostname.SpokeThickness = 2;
            this.loadingCircleScanHostname.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanOperatingSystem
            // 
            this.loadingCircleScanOperatingSystem.Active = false;
            this.loadingCircleScanOperatingSystem.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanOperatingSystem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanOperatingSystem.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanOperatingSystem, "loadingCircleScanOperatingSystem");
            this.loadingCircleScanOperatingSystem.Name = "loadingCircleScanOperatingSystem";
            this.loadingCircleScanOperatingSystem.NumberSpoke = 12;
            this.loadingCircleScanOperatingSystem.OuterCircleRadius = 11;
            this.loadingCircleScanOperatingSystem.RotationSpeed = 1;
            this.loadingCircleScanOperatingSystem.SpokeThickness = 2;
            this.loadingCircleScanOperatingSystem.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanVideoCard
            // 
            this.loadingCircleScanVideoCard.Active = false;
            this.loadingCircleScanVideoCard.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanVideoCard.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanVideoCard.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanVideoCard, "loadingCircleScanVideoCard");
            this.loadingCircleScanVideoCard.Name = "loadingCircleScanVideoCard";
            this.loadingCircleScanVideoCard.NumberSpoke = 12;
            this.loadingCircleScanVideoCard.OuterCircleRadius = 11;
            this.loadingCircleScanVideoCard.RotationSpeed = 1;
            this.loadingCircleScanVideoCard.SpokeThickness = 2;
            this.loadingCircleScanVideoCard.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanMediaOperationMode
            // 
            this.loadingCircleScanMediaOperationMode.Active = false;
            this.loadingCircleScanMediaOperationMode.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanMediaOperationMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanMediaOperationMode.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanMediaOperationMode, "loadingCircleScanMediaOperationMode");
            this.loadingCircleScanMediaOperationMode.Name = "loadingCircleScanMediaOperationMode";
            this.loadingCircleScanMediaOperationMode.NumberSpoke = 12;
            this.loadingCircleScanMediaOperationMode.OuterCircleRadius = 11;
            this.loadingCircleScanMediaOperationMode.RotationSpeed = 1;
            this.loadingCircleScanMediaOperationMode.SpokeThickness = 2;
            this.loadingCircleScanMediaOperationMode.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanStorageType
            // 
            this.loadingCircleScanStorageType.Active = false;
            this.loadingCircleScanStorageType.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanStorageType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanStorageType.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanStorageType, "loadingCircleScanStorageType");
            this.loadingCircleScanStorageType.Name = "loadingCircleScanStorageType";
            this.loadingCircleScanStorageType.NumberSpoke = 12;
            this.loadingCircleScanStorageType.OuterCircleRadius = 11;
            this.loadingCircleScanStorageType.RotationSpeed = 1;
            this.loadingCircleScanStorageType.SpokeThickness = 2;
            this.loadingCircleScanStorageType.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanRam
            // 
            this.loadingCircleScanRam.Active = false;
            this.loadingCircleScanRam.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanRam.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanRam.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanRam, "loadingCircleScanRam");
            this.loadingCircleScanRam.Name = "loadingCircleScanRam";
            this.loadingCircleScanRam.NumberSpoke = 12;
            this.loadingCircleScanRam.OuterCircleRadius = 11;
            this.loadingCircleScanRam.RotationSpeed = 1;
            this.loadingCircleScanRam.SpokeThickness = 2;
            this.loadingCircleScanRam.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanProcessor
            // 
            this.loadingCircleScanProcessor.Active = false;
            this.loadingCircleScanProcessor.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanProcessor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanProcessor.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanProcessor, "loadingCircleScanProcessor");
            this.loadingCircleScanProcessor.Name = "loadingCircleScanProcessor";
            this.loadingCircleScanProcessor.NumberSpoke = 12;
            this.loadingCircleScanProcessor.OuterCircleRadius = 11;
            this.loadingCircleScanProcessor.RotationSpeed = 1;
            this.loadingCircleScanProcessor.SpokeThickness = 2;
            this.loadingCircleScanProcessor.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanSerialNumber
            // 
            this.loadingCircleScanSerialNumber.Active = false;
            this.loadingCircleScanSerialNumber.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanSerialNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanSerialNumber.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanSerialNumber, "loadingCircleScanSerialNumber");
            this.loadingCircleScanSerialNumber.Name = "loadingCircleScanSerialNumber";
            this.loadingCircleScanSerialNumber.NumberSpoke = 12;
            this.loadingCircleScanSerialNumber.OuterCircleRadius = 11;
            this.loadingCircleScanSerialNumber.RotationSpeed = 1;
            this.loadingCircleScanSerialNumber.SpokeThickness = 2;
            this.loadingCircleScanSerialNumber.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanModel
            // 
            this.loadingCircleScanModel.Active = false;
            this.loadingCircleScanModel.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanModel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanModel.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanModel, "loadingCircleScanModel");
            this.loadingCircleScanModel.Name = "loadingCircleScanModel";
            this.loadingCircleScanModel.NumberSpoke = 12;
            this.loadingCircleScanModel.OuterCircleRadius = 11;
            this.loadingCircleScanModel.RotationSpeed = 1;
            this.loadingCircleScanModel.SpokeThickness = 2;
            this.loadingCircleScanModel.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // loadingCircleScanBrand
            // 
            this.loadingCircleScanBrand.Active = false;
            this.loadingCircleScanBrand.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleScanBrand.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleScanBrand.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleScanBrand, "loadingCircleScanBrand");
            this.loadingCircleScanBrand.Name = "loadingCircleScanBrand";
            this.loadingCircleScanBrand.NumberSpoke = 12;
            this.loadingCircleScanBrand.OuterCircleRadius = 11;
            this.loadingCircleScanBrand.RotationSpeed = 1;
            this.loadingCircleScanBrand.SpokeThickness = 2;
            this.loadingCircleScanBrand.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // iconImgTpmVersion
            // 
            this.iconImgTpmVersion.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgTpmVersion, "iconImgTpmVersion");
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
            this.iconImgVirtualizationTechnology.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgVirtualizationTechnology, "iconImgVirtualizationTechnology");
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
            this.iconImgBrand.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgBrand, "iconImgBrand");
            this.iconImgBrand.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgBrand.Name = "iconImgBrand";
            this.iconImgBrand.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgBrand.TabStop = false;
            // 
            // iconImgSecureBoot
            // 
            this.iconImgSecureBoot.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgSecureBoot, "iconImgSecureBoot");
            this.iconImgSecureBoot.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgSecureBoot.Name = "iconImgSecureBoot";
            this.iconImgSecureBoot.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgSecureBoot.TabStop = false;
            // 
            // iconImgFwVersion
            // 
            this.iconImgFwVersion.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgFwVersion, "iconImgFwVersion");
            this.iconImgFwVersion.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgFwVersion.Name = "iconImgFwVersion";
            this.iconImgFwVersion.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgFwVersion.TabStop = false;
            // 
            // iconImgFwType
            // 
            this.iconImgFwType.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgFwType, "iconImgFwType");
            this.iconImgFwType.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgFwType.Name = "iconImgFwType";
            this.iconImgFwType.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgFwType.TabStop = false;
            // 
            // iconImgIpAddress
            // 
            this.iconImgIpAddress.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgIpAddress, "iconImgIpAddress");
            this.iconImgIpAddress.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgIpAddress.Name = "iconImgIpAddress";
            this.iconImgIpAddress.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgIpAddress.TabStop = false;
            // 
            // iconImgHostname
            // 
            this.iconImgHostname.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgHostname, "iconImgHostname");
            this.iconImgHostname.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgHostname.Name = "iconImgHostname";
            this.iconImgHostname.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgHostname.TabStop = false;
            // 
            // iconImgOperatingSystem
            // 
            this.iconImgOperatingSystem.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgOperatingSystem, "iconImgOperatingSystem");
            this.iconImgOperatingSystem.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgOperatingSystem.Name = "iconImgOperatingSystem";
            this.iconImgOperatingSystem.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgOperatingSystem.TabStop = false;
            // 
            // iconImgVideoCard
            // 
            this.iconImgVideoCard.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgVideoCard, "iconImgVideoCard");
            this.iconImgVideoCard.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgVideoCard.Name = "iconImgVideoCard";
            this.iconImgVideoCard.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgVideoCard.TabStop = false;
            // 
            // iconImgMediaOperationMode
            // 
            this.iconImgMediaOperationMode.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgMediaOperationMode, "iconImgMediaOperationMode");
            this.iconImgMediaOperationMode.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgMediaOperationMode.Name = "iconImgMediaOperationMode";
            this.iconImgMediaOperationMode.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgMediaOperationMode.TabStop = false;
            // 
            // iconImgStorageType
            // 
            this.iconImgStorageType.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgStorageType, "iconImgStorageType");
            this.iconImgStorageType.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgStorageType.Name = "iconImgStorageType";
            this.iconImgStorageType.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgStorageType.TabStop = false;
            // 
            // iconImgRam
            // 
            this.iconImgRam.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgRam, "iconImgRam");
            this.iconImgRam.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgRam.Name = "iconImgRam";
            this.iconImgRam.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgRam.TabStop = false;
            // 
            // iconImgProcessor
            // 
            this.iconImgProcessor.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgProcessor, "iconImgProcessor");
            this.iconImgProcessor.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgProcessor.Name = "iconImgProcessor";
            this.iconImgProcessor.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgProcessor.TabStop = false;
            // 
            // iconImgSerialNumber
            // 
            this.iconImgSerialNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgSerialNumber, "iconImgSerialNumber");
            this.iconImgSerialNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgSerialNumber.Name = "iconImgSerialNumber";
            this.iconImgSerialNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgSerialNumber.TabStop = false;
            // 
            // iconImgModel
            // 
            this.iconImgModel.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgModel, "iconImgModel");
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
            // lblFixedVideoCard
            // 
            resources.ApplyResources(this.lblFixedVideoCard, "lblFixedVideoCard");
            this.lblFixedVideoCard.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedVideoCard.Name = "lblFixedVideoCard";
            // 
            // groupBoxAssetData
            // 
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
            resources.ApplyResources(this.groupBoxAssetData, "groupBoxAssetData");
            this.groupBoxAssetData.Name = "groupBoxAssetData";
            this.groupBoxAssetData.TabStop = false;
            // 
            // comboBoxBatteryChange
            // 
            this.comboBoxBatteryChange.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxBatteryChange.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxBatteryChange.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxBatteryChange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxBatteryChange, "comboBoxBatteryChange");
            this.comboBoxBatteryChange.FormattingEnabled = true;
            this.comboBoxBatteryChange.Name = "comboBoxBatteryChange";
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
            // comboBoxHwType
            // 
            this.comboBoxHwType.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxHwType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxHwType.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxHwType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxHwType, "comboBoxHwType");
            this.comboBoxHwType.FormattingEnabled = true;
            this.comboBoxHwType.Name = "comboBoxHwType";
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
            this.comboBoxBuilding.Sorted = true;
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
            this.iconImgTicketNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgTicketNumber, "iconImgTicketNumber");
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
            this.textBoxTicketNumber.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxTicketNumber.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.textBoxTicketNumber, "textBoxTicketNumber");
            this.textBoxTicketNumber.Name = "textBoxTicketNumber";
            this.textBoxTicketNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxNumbersOnly_KeyPress);
            // 
            // iconImgBatteryChange
            // 
            this.iconImgBatteryChange.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgBatteryChange, "iconImgBatteryChange");
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
            this.iconImgRoomLetter.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgRoomLetter, "iconImgRoomLetter");
            this.iconImgRoomLetter.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgRoomLetter.Name = "iconImgRoomLetter";
            this.iconImgRoomLetter.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgRoomLetter.TabStop = false;
            // 
            // iconImgHwType
            // 
            this.iconImgHwType.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgHwType, "iconImgHwType");
            this.iconImgHwType.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgHwType.Name = "iconImgHwType";
            this.iconImgHwType.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgHwType.TabStop = false;
            // 
            // iconImgTag
            // 
            this.iconImgTag.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgTag, "iconImgTag");
            this.iconImgTag.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgTag.Name = "iconImgTag";
            this.iconImgTag.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgTag.TabStop = false;
            // 
            // iconImgInUse
            // 
            this.iconImgInUse.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgInUse, "iconImgInUse");
            this.iconImgInUse.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgInUse.Name = "iconImgInUse";
            this.iconImgInUse.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgInUse.TabStop = false;
            // 
            // iconImgServiceDate
            // 
            this.iconImgServiceDate.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgServiceDate, "iconImgServiceDate");
            this.iconImgServiceDate.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgServiceDate.Name = "iconImgServiceDate";
            this.iconImgServiceDate.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgServiceDate.TabStop = false;
            // 
            // iconImgStandard
            // 
            this.iconImgStandard.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgStandard, "iconImgStandard");
            this.iconImgStandard.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgStandard.Name = "iconImgStandard";
            this.iconImgStandard.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgStandard.TabStop = false;
            // 
            // iconImgAdRegistered
            // 
            this.iconImgAdRegistered.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgAdRegistered, "iconImgAdRegistered");
            this.iconImgAdRegistered.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgAdRegistered.Name = "iconImgAdRegistered";
            this.iconImgAdRegistered.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgAdRegistered.TabStop = false;
            // 
            // iconImgBuilding
            // 
            this.iconImgBuilding.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgBuilding, "iconImgBuilding");
            this.iconImgBuilding.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgBuilding.Name = "iconImgBuilding";
            this.iconImgBuilding.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgBuilding.TabStop = false;
            // 
            // iconImgRoomNumber
            // 
            this.iconImgRoomNumber.CompositingQuality = null;
            resources.ApplyResources(this.iconImgRoomNumber, "iconImgRoomNumber");
            this.iconImgRoomNumber.InterpolationMode = null;
            this.iconImgRoomNumber.Name = "iconImgRoomNumber";
            this.iconImgRoomNumber.SmoothingMode = null;
            this.iconImgRoomNumber.TabStop = false;
            // 
            // iconImgSealNumber
            // 
            this.iconImgSealNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgSealNumber, "iconImgSealNumber");
            this.iconImgSealNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgSealNumber.Name = "iconImgSealNumber";
            this.iconImgSealNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgSealNumber.TabStop = false;
            // 
            // iconImgAssetNumber
            // 
            this.iconImgAssetNumber.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgAssetNumber, "iconImgAssetNumber");
            this.iconImgAssetNumber.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgAssetNumber.Name = "iconImgAssetNumber";
            this.iconImgAssetNumber.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgAssetNumber.TabStop = false;
            // 
            // dateTimePickerServiceDate
            // 
            this.dateTimePickerServiceDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.dateTimePickerServiceDate, "dateTimePickerServiceDate");
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
            this.groupBoxServiceType.Controls.Add(this.textBoxInactiveUpdateDataRadio);
            this.groupBoxServiceType.Controls.Add(this.radioButtonUpdateData);
            this.groupBoxServiceType.Controls.Add(this.lblFixedMandatoryServiceType);
            this.groupBoxServiceType.Controls.Add(this.textBoxInactiveFormattingRadio);
            this.groupBoxServiceType.Controls.Add(this.textBoxInactiveMaintenanceRadio);
            this.groupBoxServiceType.Controls.Add(this.radioButtonFormatting);
            this.groupBoxServiceType.Controls.Add(this.radioButtonMaintenance);
            this.groupBoxServiceType.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.groupBoxServiceType, "groupBoxServiceType");
            this.groupBoxServiceType.Name = "groupBoxServiceType";
            this.groupBoxServiceType.TabStop = false;
            // 
            // textBoxInactiveUpdateDataRadio
            // 
            this.textBoxInactiveUpdateDataRadio.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxInactiveUpdateDataRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxInactiveUpdateDataRadio, "textBoxInactiveUpdateDataRadio");
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
            this.radioButtonUpdateData.CheckedChanged += new System.EventHandler(this.RadioButtonUpdateData_CheckedChanged);
            // 
            // lblFixedMandatoryServiceType
            // 
            resources.ApplyResources(this.lblFixedMandatoryServiceType, "lblFixedMandatoryServiceType");
            this.lblFixedMandatoryServiceType.ForeColor = System.Drawing.Color.Red;
            this.lblFixedMandatoryServiceType.Name = "lblFixedMandatoryServiceType";
            // 
            // textBoxInactiveFormattingRadio
            // 
            this.textBoxInactiveFormattingRadio.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxInactiveFormattingRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxInactiveFormattingRadio, "textBoxInactiveFormattingRadio");
            this.textBoxInactiveFormattingRadio.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxInactiveFormattingRadio.Name = "textBoxInactiveFormattingRadio";
            this.textBoxInactiveFormattingRadio.ReadOnly = true;
            // 
            // textBoxInactiveMaintenanceRadio
            // 
            this.textBoxInactiveMaintenanceRadio.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxInactiveMaintenanceRadio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxInactiveMaintenanceRadio, "textBoxInactiveMaintenanceRadio");
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
            this.radioButtonFormatting.CheckedChanged += new System.EventHandler(this.RadioButtonFormatting_CheckedChanged);
            // 
            // radioButtonMaintenance
            // 
            resources.ApplyResources(this.radioButtonMaintenance, "radioButtonMaintenance");
            this.radioButtonMaintenance.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radioButtonMaintenance.Name = "radioButtonMaintenance";
            this.radioButtonMaintenance.UseVisualStyleBackColor = true;
            this.radioButtonMaintenance.CheckedChanged += new System.EventHandler(this.RadioButtonMaintenance_CheckedChanged);
            // 
            // loadingCircleLastService
            // 
            this.loadingCircleLastService.Active = false;
            this.loadingCircleLastService.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleLastService.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleLastService, "loadingCircleLastService");
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
            this.comboBoxThemeButton,
            this.logLabelButton,
            this.aboutLabelButton,
            this.toolStripStatusBarText,
            this.toolStripVersionText});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // comboBoxThemeButton
            // 
            this.comboBoxThemeButton.BackColor = System.Drawing.SystemColors.Control;
            this.comboBoxThemeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.comboBoxThemeButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripAutoTheme,
            this.toolStripLightTheme,
            this.toolStripDarkTheme});
            this.comboBoxThemeButton.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.comboBoxThemeButton, "comboBoxThemeButton");
            this.comboBoxThemeButton.Name = "comboBoxThemeButton";
            // 
            // toolStripAutoTheme
            // 
            this.toolStripAutoTheme.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripAutoTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripAutoTheme.Name = "toolStripAutoTheme";
            resources.ApplyResources(this.toolStripAutoTheme, "toolStripAutoTheme");
            this.toolStripAutoTheme.Click += new System.EventHandler(this.ToolStripMenuAutoTheme_Click);
            // 
            // toolStripLightTheme
            // 
            this.toolStripLightTheme.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripLightTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripLightTheme.Name = "toolStripLightTheme";
            resources.ApplyResources(this.toolStripLightTheme, "toolStripLightTheme");
            this.toolStripLightTheme.Click += new System.EventHandler(this.ToolStripMenuLightTheme_Click);
            // 
            // toolStripDarkTheme
            // 
            this.toolStripDarkTheme.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripDarkTheme.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripDarkTheme.Name = "toolStripDarkTheme";
            resources.ApplyResources(this.toolStripDarkTheme, "toolStripDarkTheme");
            this.toolStripDarkTheme.Click += new System.EventHandler(this.ToolStripMenuDarkTheme_Click);
            // 
            // logLabelButton
            // 
            this.logLabelButton.BackColor = System.Drawing.SystemColors.Control;
            this.logLabelButton.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.logLabelButton.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.logLabelButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.logLabelButton.Name = "logLabelButton";
            resources.ApplyResources(this.logLabelButton, "logLabelButton");
            this.logLabelButton.Click += new System.EventHandler(this.LogLabelButton_Click);
            this.logLabelButton.MouseEnter += new System.EventHandler(this.LogLabel_MouseEnter);
            this.logLabelButton.MouseLeave += new System.EventHandler(this.LogLabel_MouseLeave);
            // 
            // aboutLabelButton
            // 
            this.aboutLabelButton.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.aboutLabelButton.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.aboutLabelButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.aboutLabelButton.Name = "aboutLabelButton";
            resources.ApplyResources(this.aboutLabelButton, "aboutLabelButton");
            this.aboutLabelButton.Click += new System.EventHandler(this.AboutLabelButton_Click);
            this.aboutLabelButton.MouseEnter += new System.EventHandler(this.AboutLabel_MouseEnter);
            this.aboutLabelButton.MouseLeave += new System.EventHandler(this.AboutLabel_MouseLeave);
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
            this.loadingCircleCollectButton.Active = false;
            this.loadingCircleCollectButton.BackColor = System.Drawing.Color.Transparent;
            this.loadingCircleCollectButton.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleCollectButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleCollectButton.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleCollectButton, "loadingCircleCollectButton");
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
            this.loadingCircleRegisterButton.Active = false;
            this.loadingCircleRegisterButton.BackColor = System.Drawing.Color.Transparent;
            this.loadingCircleRegisterButton.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleRegisterButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.loadingCircleRegisterButton.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleRegisterButton, "loadingCircleRegisterButton");
            this.loadingCircleRegisterButton.Name = "loadingCircleRegisterButton";
            this.loadingCircleRegisterButton.NumberSpoke = 12;
            this.loadingCircleRegisterButton.OuterCircleRadius = 11;
            this.loadingCircleRegisterButton.RotationSpeed = 1;
            this.loadingCircleRegisterButton.SpokeThickness = 2;
            this.loadingCircleRegisterButton.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // groupBoxServerStatus
            // 
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
            resources.ApplyResources(this.groupBoxServerStatus, "groupBoxServerStatus");
            this.groupBoxServerStatus.Name = "groupBoxServerStatus";
            this.groupBoxServerStatus.TabStop = false;
            // 
            // loadingCircleServerOperationalStatus
            // 
            this.loadingCircleServerOperationalStatus.Active = false;
            this.loadingCircleServerOperationalStatus.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleServerOperationalStatus.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleServerOperationalStatus, "loadingCircleServerOperationalStatus");
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
            this.groupBoxTableMaintenances.Controls.Add(this.loadingCircleTableMaintenances);
            this.groupBoxTableMaintenances.Controls.Add(this.tableMaintenances);
            this.groupBoxTableMaintenances.Controls.Add(this.lblThereIsNothingHere);
            this.groupBoxTableMaintenances.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.groupBoxTableMaintenances, "groupBoxTableMaintenances");
            this.groupBoxTableMaintenances.Name = "groupBoxTableMaintenances";
            this.groupBoxTableMaintenances.TabStop = false;
            // 
            // loadingCircleTableMaintenances
            // 
            this.loadingCircleTableMaintenances.Active = false;
            this.loadingCircleTableMaintenances.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleTableMaintenances.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleTableMaintenances, "loadingCircleTableMaintenances");
            this.loadingCircleTableMaintenances.Name = "loadingCircleTableMaintenances";
            this.loadingCircleTableMaintenances.NumberSpoke = 12;
            this.loadingCircleTableMaintenances.OuterCircleRadius = 11;
            this.loadingCircleTableMaintenances.RotationSpeed = 1;
            this.loadingCircleTableMaintenances.SpokeThickness = 2;
            this.loadingCircleTableMaintenances.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // tableMaintenances
            // 
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
            resources.ApplyResources(this.tableMaintenances, "tableMaintenances");
            this.tableMaintenances.Name = "tableMaintenances";
            this.tableMaintenances.ReadOnly = true;
            this.tableMaintenances.RowHeadersVisible = false;
            this.tableMaintenances.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.tableMaintenances.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            // 
            // serviceDate
            // 
            this.serviceDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Format = "d";
            dataGridViewCellStyle1.NullValue = null;
            this.serviceDate.DefaultCellStyle = dataGridViewCellStyle1;
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
            // hwUidToolTip
            // 
            this.hwUidToolTip.IsBalloon = true;
            this.hwUidToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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

        #endregion

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
        private Label lblNoticeHardwareChanged;
        private ToolTip hwUidToolTip;
        private Button hardwareChangeButton;
        private DataGridViewTextBoxColumn serviceDate;
        private DataGridViewTextBoxColumn serviceType;
        private DataGridViewTextBoxColumn agentUsername;
    }
}
