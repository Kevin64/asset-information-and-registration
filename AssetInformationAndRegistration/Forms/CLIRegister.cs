using AssetInformationAndRegistration.Misc;
using AssetInformationAndRegistration.Properties;
using AssetInformationAndRegistration.WebView;
using ConstantsDLL;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary> 
    /// Class for CLI asset registering
    /// </summary>
    internal class CLIRegister : Form
    {
        public bool pass, serverOnline;
        private bool[] serverAlertBool;
        private string[] serverArgs, serverAlert;
        private WebView2 webView2Control;
        private readonly LogGenerator log;
        private readonly List<string[]> parametersList;
        private List<string[]> jsonServerSettings;
        private readonly List<string> enforcementList;
        private readonly string serverIP, serverPort, assetNumber, building, roomNumber, serviceDate, serviceType, batteryChange, ticketNumber, standard, inUse, sealNumber, tag, hwType;
        private readonly string[] agentData;
        private string brand, model, serialNumber, processor, ram, storageSize, storageType, mediaOperationMode, videoCard, operatingSystem, hostname, fwType, fwVersion, secureBoot, virtualizationTechnology, tpmVersion, macAddress, ipAddress;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CLIRegister));
            webView2Control = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)webView2Control).BeginInit();
            SuspendLayout();
            // 
            // webView2Control
            // 
            webView2Control.AllowExternalDrop = true;
            webView2Control.CreationProperties = null;
            webView2Control.DefaultBackgroundColor = System.Drawing.Color.White;
            webView2Control.Location = new System.Drawing.Point(12, 12);
            webView2Control.Name = "webView2Control";
            webView2Control.Size = new System.Drawing.Size(134, 29);
            webView2Control.TabIndex = 0;
            webView2Control.ZoomFactor = 1D;
            // 
            // CLIRegister
            // 
            ClientSize = new System.Drawing.Size(158, 53);
            ControlBox = false;
            Controls.Add(webView2Control);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CLIRegister";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            WindowState = System.Windows.Forms.FormWindowState.Minimized;
            ((System.ComponentModel.ISupportInitialize)webView2Control).EndInit();
            ResumeLayout(false);

        }

        /// <summary> 
        /// CLI pseudo-form constructor
        /// </summary>
        /// <param name="argsArray">Arguments array</param>
        /// <param name="agentData">Agent username and ID</param>
        /// <param name="log">Log file object</param>
        /// <param name="parametersList">List containing data from [Parameters]</param>
        /// <param name="enforcementList">List containing data from [Enforcement]</param>
        internal CLIRegister(string[] argsArray, string[] agentData, LogGenerator log, List<string[]> parametersList, List<string> enforcementList)
        {
            InitializeComponent();

            serverArgs = argsArray;

            serverIP = argsArray[0];
            serverPort = argsArray[1];
            assetNumber = argsArray[2];
            building = argsArray[3];
            roomNumber = argsArray[4];
            serviceDate = argsArray[5];
            serviceType = argsArray[6];
            batteryChange = argsArray[7];
            ticketNumber = argsArray[8];
            standard = argsArray[9];
            inUse = argsArray[10];
            sealNumber = argsArray[11];
            tag = argsArray[12];
            hwType = argsArray[13];
            this.agentData = agentData;
            this.log = log;
            this.parametersList = parametersList;
            this.enforcementList = enforcementList;

            InitProc(serverIP, serverPort, assetNumber, building, roomNumber, serviceDate, serviceType, batteryChange, ticketNumber, this.agentData, standard, inUse, sealNumber, tag, hwType);
        }

        /// <summary> 
        /// Method that allocates a WebView2 instance and checks if args are within standard, then passes them to register method
        /// </summary>
        /// <param name="serverIP">Server IP address</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="assetNumber">Asset number</param>
        /// <param name="building">Building name</param>
        /// <param name="roomNumber">Room number</param>
        /// <param name="serviceDate">Date of service</param>
        /// <param name="serviceType">Type os service</param>
        /// <param name="batteryChange">If there was a CMOS battery change</param>
        /// <param name="ticketNumber">Helpdesk ticket number</param>
        /// <param name="agentData">Agent username and ID</param>
        /// <param name="standard">Image standard</param>
        /// <param name="inUse">If the asset is in use</param>
        /// <param name="sealNumber">Seal number</param>
        /// <param name="tag">If the asset has a tag</param>
        /// <param name="hwType">hardware type of the asset</param>
        private async void InitProc(string serverIP, string serverPort, string assetNumber, string building, string roomNumber, string serviceDate, string serviceType, string batteryChange, string ticketNumber, string[] agentData, string standard, string inUse, string sealNumber, string tag, string hwType)
        {
            serverAlert = new string[11];
            serverAlertBool = new bool[11];
            serverAlert[0] = Strings.CLI_HOSTNAME_ALERT;
            serverAlert[1] = Strings.CLI_MEDIA_OPERATION_ALERT;
            serverAlert[2] = Strings.CLI_SECURE_BOOT_ALERT;
            serverAlert[3] = Strings.CLI_DATABASE_REACH_ERROR;
            serverAlert[4] = Strings.CLI_BIOS_VERSION_ALERT;
            serverAlert[5] = Strings.CLI_FIRMWARE_TYPE_ALERT;
            serverAlert[6] = Strings.CLI_NETWORK_IP_ERROR;
            serverAlert[7] = Strings.CLI_NETWORK_MAC_ERROR;
            serverAlert[8] = Strings.CLI_VT_ALERT;
            serverAlert[9] = Strings.CLI_TPM_ALERT;
            serverAlert[10] = Strings.CLI_MEMORY_ALERT;

            //Fetch building and hw types info from the specified server
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_FETCHING_SERVER_DATA, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
            jsonServerSettings = JsonFileReaderDLL.ConfigFileReader.FetchInfoST(serverIP, serverPort);
            parametersList[4] = jsonServerSettings[0]; //Buildings
            parametersList[5] = jsonServerSettings[1]; //Hw Types
            parametersList[6] = jsonServerSettings[2]; //Firmware Types
            parametersList[7] = jsonServerSettings[3]; //Tpm Types
            parametersList[8] = jsonServerSettings[4]; //Media Op Types
            parametersList[9] = jsonServerSettings[5]; //Secure Boot States
            parametersList[10] = jsonServerSettings[6]; //Virtualization Technology States

            webView2Control = new WebView2();

            await SendData.LoadWebView2(log, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI), webView2Control);

            string[] dateFormat = new string[] { ConstantsDLL.Properties.Resources.DATE_FORMAT };

            if (serverIP.Length <= 15 && serverIP.Length > 6 && //serverIP
                serverPort.Length <= 5 && serverPort.All(char.IsDigit) && //serverPort
                assetNumber.Length <= 6 && assetNumber.Length >= 0 && assetNumber.All(char.IsDigit) && //assetNumber
                (parametersList[4].Contains(building) || building.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //building
                ((roomNumber.Length <= 4 && roomNumber.Length > 0 && roomNumber.All(char.IsDigit)) || roomNumber.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //roomNumber
                ((serviceDate.Length == 10 && DateTime.TryParseExact(serviceDate, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out DateTime datetime)) || serviceDate.Equals(ConstantsDLL.Properties.Resources.TODAY)) && //serviceDate
                StringsAndConstants.LIST_MODE_CLI.Contains(serviceType) && //serviceType
                StringsAndConstants.LIST_BATTERY_CLI.Contains(batteryChange) && //batteryChange
                ticketNumber.Length <= 6 && ticketNumber.All(char.IsDigit) && //ticketNumber
                (StringsAndConstants.LIST_STANDARD_CLI.Contains(standard) || standard.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //standard
                (StringsAndConstants.LIST_IN_USE_CLI.Contains(inUse) || inUse.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //inUse
                ((sealNumber.Length <= 10 && sealNumber.All(char.IsDigit)) || sealNumber.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //sealNumber
                (StringsAndConstants.LIST_TAG_CLI.Contains(tag) || tag.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //tag
                (parametersList[5].Contains(hwType) || hwType.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))) //hwType
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_PINGGING_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                serverOnline = await JsonFileReaderDLL.ModelFileReader.CheckHostMT(serverIP, serverPort);
                if (serverOnline && serverPort != string.Empty)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_SERVER_DETAIL, serverIP + ":" + serverPort, true);
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_ONLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

                    CollectThread();

                    //log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), MiscMethods.SinceLabelUpdate(true), string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                    //log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), MiscMethods.SinceLabelUpdate(false), string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                    //assetNumber
                    if (assetNumber.Equals(string.Empty))
                    {
                        assetNumber = HardwareInfo.GetHostname().Substring(3);
                    }

                    string[] assetJsonStr = JsonFileReaderDLL.AssetFileReader.FetchInfoST(assetNumber, serverIP, serverPort);
                    //If asset Json does not exist and there are some 'same' cmd switch word
                    if (assetJsonStr[0] == ConstantsDLL.Properties.Resources.FALSE && serverArgs.Contains(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.LOG_SAMEWORD_NOFIRSTREGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                        webView2Control.Dispose();
                        Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                    }
                    else if (assetJsonStr[0] == ConstantsDLL.Properties.Resources.FALSE) //If asset Json does not exist
                    {
                        //serviceType
                        if (serviceType.Equals(StringsAndConstants.CLI_SERVICE_TYPE_0))
                        {
                            serviceType = ConstantsDLL.Properties.Resources.FORMAT_URL;
                        }
                        else if (serviceType.Equals(StringsAndConstants.CLI_SERVICE_TYPE_1))
                        {
                            serviceType = ConstantsDLL.Properties.Resources.MAINTENANCE_URL;
                        }
                        //building
                        building = Array.IndexOf(parametersList[4], building).ToString();
                        //standard
                        if (standard.Equals(StringsAndConstants.CLI_EMPLOYEE_TYPE_0))
                        {
                            standard = Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                        }
                        else if (standard.Equals(StringsAndConstants.CLI_EMPLOYEE_TYPE_1))
                        {
                            standard = Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString();
                        }
                        //batteryChange
                        if (batteryChange.Equals(StringsAndConstants.LIST_NO_ABBREV))
                        {
                            batteryChange = Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                        }
                        else if (batteryChange.Equals(StringsAndConstants.LIST_YES_ABBREV))
                        {
                            batteryChange = Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString();
                        }
                        //inUse
                        if (inUse.Equals(StringsAndConstants.LIST_NO_ABBREV))
                        {
                            inUse = Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                        }
                        else if (inUse.Equals(StringsAndConstants.LIST_YES_ABBREV))
                        {
                            inUse = Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString();
                        }
                        //tag
                        if (tag.Equals(StringsAndConstants.LIST_NO_ABBREV))
                        {
                            tag = Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                        }
                        else if (tag.Equals(StringsAndConstants.LIST_YES_ABBREV))
                        {
                            tag = Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString();
                        }
                        //hwType
                        hwType = Array.IndexOf(parametersList[5], hwType).ToString();
                    }
                    else //If asset Json does exist
                    {
                        //If asset is discarded
                        if (assetJsonStr[9] == "1")
                        {
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ConstantsDLL.Properties.Strings.ASSET_DROPPED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                            webView2Control.Dispose();
                            Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                        }
                        //serviceType
                        if (serviceType.Equals(StringsAndConstants.CLI_SERVICE_TYPE_0))
                        {
                            serviceType = ConstantsDLL.Properties.Resources.FORMAT_URL;
                        }
                        else if (serviceType.Equals(StringsAndConstants.CLI_SERVICE_TYPE_1))
                        {
                            serviceType = ConstantsDLL.Properties.Resources.MAINTENANCE_URL;
                        }
                        //sealNumber
                        if (sealNumber.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                        {
                            sealNumber = assetJsonStr[6];
                        }
                        //roomNumber
                        if (roomNumber.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                        {
                            roomNumber = assetJsonStr[2];
                        }
                        //building
                        building = building.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)
                            ? assetJsonStr[1]
                            : Array.IndexOf(parametersList[4], building).ToString();
                        //standard
                        if (standard.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                        {
                            standard = assetJsonStr[3];
                        }
                        else if (standard.Equals(StringsAndConstants.CLI_EMPLOYEE_TYPE_0))
                        {
                            standard = Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                        }
                        else if (standard.Equals(StringsAndConstants.CLI_EMPLOYEE_TYPE_1))
                        {
                            standard = Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString();
                        }
                        //batteryChange
                        if (batteryChange.Equals(StringsAndConstants.LIST_NO_ABBREV))
                        {
                            batteryChange = Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                        }
                        else if (batteryChange.Equals(StringsAndConstants.LIST_YES_ABBREV))
                        {
                            batteryChange = Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString();
                        }
                        //inUse
                        if (inUse.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                        {
                            inUse = assetJsonStr[5];
                        }
                        else if (inUse.Equals(StringsAndConstants.LIST_NO_ABBREV))
                        {
                            inUse = Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                        }
                        else if (inUse.Equals(StringsAndConstants.LIST_YES_ABBREV))
                        {
                            inUse = Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString();
                        }
                        //tag
                        if (tag.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                        {
                            tag = assetJsonStr[7];
                        }
                        else if (tag.Equals(StringsAndConstants.LIST_NO_ABBREV))
                        {
                            tag = Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                        }
                        else if (tag.Equals(StringsAndConstants.LIST_YES_ABBREV))
                        {
                            tag = Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString();
                        }
                        //hwType
                        hwType = hwType.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)
                            ? assetJsonStr[8]
                            : Array.IndexOf(parametersList[5], hwType).ToString();
                    }
                    PrintHardwareData();

                    //If there are no pendencies
                    if (pass)
                    {
                        serverArgs = new string[34];

                        serverArgs[0] = serverIP;
                        serverArgs[1] = serverPort;
                        serverArgs[2] = assetNumber;
                        serverArgs[3] = building;
                        serverArgs[4] = roomNumber;
                        serverArgs[6] = serviceType;
                        serverArgs[7] = batteryChange;
                        serverArgs[8] = ticketNumber;
                        serverArgs[9] = agentData[0];
                        serverArgs[10] = standard;
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
                        serverArgs[25] = secureBoot;
                        serverArgs[26] = virtualizationTechnology;
                        serverArgs[27] = tpmVersion;
                        serverArgs[28] = macAddress;
                        serverArgs[29] = ipAddress;
                        serverArgs[30] = inUse;
                        serverArgs[31] = sealNumber;
                        serverArgs[32] = tag;
                        serverArgs[33] = hwType;

                        DateTime d = new DateTime();
                        DateTime todayDate = DateTime.Today;
                        bool tDay;

                        try
                        {
                            _ = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain();
                            serverArgs[11] = Convert.ToInt32(Program.SpecBinaryStates.ENABLED).ToString();
                        }
                        catch
                        {
                            serverArgs[11] = Convert.ToInt32(Program.SpecBinaryStates.DISABLED).ToString();
                        }

                        try //If there is database record of the asset number
                        {
                            //If chosen date is 'hoje'
                            if (serviceDate.Equals(ConstantsDLL.Properties.Resources.TODAY))
                            {
                                serviceDate = DateTime.Today.ToString(ConstantsDLL.Properties.Resources.DATE_FORMAT).Substring(0, 10);
                                tDay = true;
                            }
                            else //If chosen date is not 'hoje'
                            {
                                d = DateTime.Parse(serviceDate);
                                serviceDate = d.ToString(ConstantsDLL.Properties.Resources.DATE_FORMAT);
                                tDay = false;
                            }

                            //Calculates last registered date with chosen date
                            DateTime registerDate = DateTime.ParseExact(serviceDate, ConstantsDLL.Properties.Resources.DATE_FORMAT, CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(assetJsonStr[10], ConstantsDLL.Properties.Resources.DATE_FORMAT, CultureInfo.InvariantCulture);

                            //If chosen date is later than the registered date
                            if (registerDate >= lastRegisterDate)
                            {
                                if (tDay) //If today
                                {
                                    serviceDate = todayDate.ToString(ConstantsDLL.Properties.Resources.DATE_FORMAT).Substring(0, 10);
                                }
                                else if (registerDate <= todayDate) //If today is greater or equal than registered date
                                {
                                    serviceDate = DateTime.Parse(serviceDate).ToString(ConstantsDLL.Properties.Resources.DATE_FORMAT).Substring(0, 10);
                                }
                                else //Forbids future registering
                                {
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.INCORRECT_FUTURE_REGISTER_DATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                                    webView2Control.Dispose();
                                    Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                                }

                                serverArgs[5] = serviceDate;

                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                                SendData.ServerSendInfo(serverArgs, log, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI), webView2Control); //Send info to server
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                            }
                            else //If chosen date is earlier than the registered date, show an error
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.INCORRECT_REGISTER_DATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                                webView2Control.Dispose(); //Kills WebView2 instance
                                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR)); //Exits
                            }
                        }
                        catch //If there is no database record of the asset number
                        {
                            if (serviceDate.Equals(ConstantsDLL.Properties.Resources.TODAY))
                            {
                                serviceDate = todayDate.ToString(ConstantsDLL.Properties.Resources.DATE_FORMAT).Substring(0, 10);
                            }
                            else
                            {
                                d = DateTime.Parse(serviceDate);
                                serviceDate = d.ToString(ConstantsDLL.Properties.Resources.DATE_FORMAT).Substring(0, 10);
                            }

                            serverArgs[5] = serviceDate;

                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                            SendData.ServerSendInfo(serverArgs, log, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI), webView2Control); //Send info to server
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                        }
                    }
                    else //If there are pendencies
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Strings.FIX_PROBLEMS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                        for (int i = 0; i < serverAlert.Length; i++)
                        {
                            if (serverAlertBool[i])
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), serverAlert[i], string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                            }
                        }
                        webView2Control.Dispose(); //Kills WebView2 instance
                        Environment.Exit(Convert.ToInt32(ExitCodes.WARNING)); //Exits
                    }
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.LOG_OFFLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                    webView2Control.Dispose(); //Kills WebView2 instance
                    Environment.Exit(Convert.ToInt32(ExitCodes.ERROR)); //Exits
                }
            }
            else
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.ARGS_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                webView2Control.Dispose(); //Kills WebView2 instance
                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR)); //Exits
            }
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_CLOSING_CLI, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Deletes downloaded json files
            File.Delete(StringsAndConstants.MODEL_FILE_PATH);
            File.Delete(StringsAndConstants.CREDENTIALS_FILE_PATH);
            File.Delete(StringsAndConstants.ASSET_FILE_PATH);
            File.Delete(StringsAndConstants.CONFIG_FILE_PATH);
            webView2Control.NavigationCompleted += WebView2_NavigationCompleted;
            Environment.Exit(Convert.ToInt32(ExitCodes.SUCCESS)); //Exits
        }

        /// <summary> 
        /// Checks if WebView2 navigation is finished
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webView2Control.Dispose(); //Kills instance
                Environment.Exit(Convert.ToInt32(ExitCodes.SUCCESS)); //Exits
            }
        }

        /// <summary> 
        /// Prints the collected data into the form labels, warning the agent when there are forbidden modes
        /// </summary>
        private void PrintHardwareData()
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_FETCHING_BIOSFILE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            pass = true;

            try
            {
                //Feches model info from server
                string[] modelJsonStr = JsonFileReaderDLL.ModelFileReader.FetchInfoST(brand, model, fwType, tpmVersion, mediaOperationMode, serverIP, serverPort);

                //If hostname is the default one and its enforcement is enabled
                if (enforcementList[3] == ConstantsDLL.Properties.Resources.TRUE && hostname.Equals(Strings.DEFAULT_HOSTNAME))
                {
                    pass = false;
                    serverAlert[0] += hostname + Strings.HOSTNAME_ALERT;
                    serverAlertBool[0] = true;
                }
                //If model Json file does exist, mediaOpMode enforcement is enabled, and the mode is incorrect
                if (enforcementList[2] == ConstantsDLL.Properties.Resources.TRUE && modelJsonStr != null && modelJsonStr[3].Equals(ConstantsDLL.Properties.Resources.FALSE))
                {
                    pass = false;
                    serverAlert[1] += mediaOperationMode + Strings.MEDIA_OPERATION_ALERT;
                    serverAlertBool[1] = true;
                }
                //The section below contains the exception cases for Secure Boot enforcement, if it is enabled
                if (enforcementList[6] == ConstantsDLL.Properties.Resources.TRUE && secureBoot.Equals(ConstantsDLL.Properties.Strings.DEACTIVATED))
                {
                    pass = false;
                    serverAlert[2] += secureBoot + Strings.SECURE_BOOT_ALERT;
                    serverAlertBool[2] = true;
                }
                //If model Json file does not exist and server is unreachable
                if (modelJsonStr == null)
                {
                    pass = false;
                    serverAlert[3] += Strings.DATABASE_REACH_ERROR;
                    serverAlertBool[3] = true;
                }
                //If model Json file does exist, firmware version enforcement is enabled, and the version is incorrect
                if (enforcementList[5] == ConstantsDLL.Properties.Resources.TRUE && modelJsonStr != null && !fwVersion.Contains(modelJsonStr[0]))
                {
                    if (!modelJsonStr[0].Equals("-1"))
                    {
                        pass = false;
                        serverAlert[4] += fwVersion + Strings.BIOS_VERSION_ALERT;
                        serverAlertBool[4] = true;
                    }
                }
                //If model Json file does exist, firmware type enforcement is enabled, and the type is incorrect
                if (enforcementList[4] == ConstantsDLL.Properties.Resources.TRUE && modelJsonStr != null && modelJsonStr[1].Equals(ConstantsDLL.Properties.Resources.FALSE))
                {
                    pass = false;
                    serverAlert[5] += fwType + Strings.FIRMWARE_TYPE_ALERT;
                    serverAlertBool[5] = true;
                }
                //If there is no MAC address assigned
                if (macAddress == string.Empty)
                {
                    pass = false;
                    serverAlert[6] += macAddress + Strings.NETWORK_ERROR; //Prints a network error
                    serverAlert[7] += ipAddress + Strings.NETWORK_ERROR; //Prints a network error
                    serverAlertBool[6] = true;
                    serverAlertBool[7] = true;
                }
                //If Virtualization Technology is disabled for UEFI and its enforcement is enabled
                if (enforcementList[7] == ConstantsDLL.Properties.Resources.TRUE && virtualizationTechnology == ConstantsDLL.Properties.Strings.DEACTIVATED)
                {
                    pass = false;
                    serverAlert[8] += virtualizationTechnology + Strings.VT_ALERT;
                    serverAlertBool[8] = true;
                }
                //If model Json file does exist, TPM enforcement is enabled, and TPM version is incorrect
                if (enforcementList[8] == ConstantsDLL.Properties.Resources.TRUE && modelJsonStr != null && modelJsonStr[2].Equals(ConstantsDLL.Properties.Resources.FALSE))
                {
                    pass = false;
                    serverAlert[9] += tpmVersion + Strings.TPM_ERROR;
                    serverAlertBool[9] = true;
                }
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetRamAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, and its limit enforcement is enabled, shows an alert
                if (enforcementList[0] == ConstantsDLL.Properties.Resources.TRUE && d < 4.0 && Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    serverAlert[10] += ram + Strings.NOT_ENOUGH_MEMORY;
                    serverAlertBool[10] = true;
                }
                //If RAM is more than 4GB and OS is x86, and its limit enforcement is enabled, shows an alert
                if (enforcementList[0] == ConstantsDLL.Properties.Resources.TRUE && d > 4.0 && !Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    serverAlert[10] += ram + Strings.TOO_MUCH_MEMORY;
                    serverAlertBool[10] = true;
                }
                if (pass)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_HARDWARE_PASSED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                }
            }
            catch (Exception e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), e.Message, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
            }
        }

        /// <summary> 
        /// Runs on a separate thread, calling methods from the HardwareInfo class, and setting the variables, while reporting the progress to the progressbar
        /// </summary>
        private void CollectThread()
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_START_COLLECTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for PC maker
            brand = HardwareInfo.GetBrand();
            if (brand == ConstantsDLL.Properties.Resources.TO_BE_FILLED_BY_OEM || brand == string.Empty)
            {
                brand = HardwareInfo.GetBrandAlt();
            }
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_BM, brand, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MODEL, model, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for motherboard Serial number
            serialNumber = HardwareInfo.GetSerialNumber();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_SERIALNO, serialNumber, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for CPU information
            processor = HardwareInfo.GetProcessorInfo();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_PROCNAME, processor, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for RAM amount and total number of slots
            ram = HardwareInfo.GetRam() + " (" + HardwareInfo.GetNumFreeRamSlots() +
                Strings.SLOTS_OF + HardwareInfo.GetNumRamSlots() + Strings.OCCUPIED + ")";
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_PM, ram, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for Storage size
            storageSize = HardwareInfo.GetStorageSize();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_HDSIZE, storageSize, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for Storage type
            storageType = HardwareInfo.GetStorageType();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MEDIATYPE, storageType, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for Media Operation (IDE/AHCI/NVME)
            mediaOperationMode = HardwareInfo.GetMediaOperationMode();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MEDIAOP, parametersList[8][Convert.ToInt32(mediaOperationMode)], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for GPU information
            videoCard = HardwareInfo.GetVideoCardInfo();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_GPUINFO, videoCard, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for OS infomation
            operatingSystem = HardwareInfo.GetOSString();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_OS, operatingSystem, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for hostname
            hostname = HardwareInfo.GetHostname();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_HOSTNAME, hostname, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for MAC Address
            macAddress = HardwareInfo.GetMacAddress();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_MAC, macAddress, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for IP Address
            ipAddress = HardwareInfo.GetIpAddress();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_IP, ipAddress, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for firmware type
            fwType = HardwareInfo.GetFwType();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_BIOSTYPE, parametersList[6][Convert.ToInt32(fwType)], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for Secure Boot status
            secureBoot = HardwareInfo.GetSecureBoot();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_SECBOOT, StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[9][Convert.ToInt32(secureBoot)])], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for firmware version
            fwVersion = HardwareInfo.GetFirmwareVersion();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_BIOS, fwVersion, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for VT status
            virtualizationTechnology = HardwareInfo.GetVirtualizationTechnology();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_VT, StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[10][Convert.ToInt32(virtualizationTechnology)])], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            //Scans for TPM status
            tpmVersion = HardwareInfo.GetTPMStatus();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_TPM, parametersList[7][Convert.ToInt32(tpmVersion)], Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));

            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_END_COLLECTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
        }
    }
}
