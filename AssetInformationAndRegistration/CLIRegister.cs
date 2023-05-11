﻿using AssetInformationAndRegistration.Properties;
using ConstantsDLL;
using HardwareInfoDLL;
using JsonFileReaderDLL;
using LogGeneratorDLL;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetInformationAndRegistration
{
    public class CLIRegister : Form
    {
        public bool pass, serverOnline;
        private bool[] serverAlertBool;
        private string[] serverArgs, serverAlert;
        private WebView2 webView2;
        private readonly LogGenerator log;
        private readonly List<string[]> parametersList;
        private List<string[]> jsonServerSettings;
        private readonly List<string> enforcementList;

        //Basic form for WebView2
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CLIRegister));
            webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)webView2).BeginInit();
            SuspendLayout();
            // 
            // webView2
            // 
            webView2.AllowExternalDrop = true;
            webView2.CreationProperties = null;
            webView2.DefaultBackgroundColor = System.Drawing.Color.White;
            webView2.Location = new System.Drawing.Point(12, 12);
            webView2.Name = "webView2";
            webView2.Size = new System.Drawing.Size(134, 29);
            webView2.TabIndex = 0;
            webView2.ZoomFactor = 1D;
            // 
            // CLIRegister
            // 
            ClientSize = new System.Drawing.Size(158, 53);
            ControlBox = false;
            Controls.Add(webView2);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CLIRegister";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            WindowState = System.Windows.Forms.FormWindowState.Minimized;
            ((System.ComponentModel.ISupportInitialize)webView2).EndInit();
            ResumeLayout(false);
        }

        //Form constructor
        public CLIRegister(string serverIP, string serverPort, string serviceType, string assetNumber, string sealNumber, string roomNumber, string building, string adRegistered, string standard, string serviceDate, string batteryChange, string ticketNumber, string inUse, string tag, string hwType, string[] agentData, LogGenerator log, List<string[]> parametersList, List<string> enforcementList)
        {
            //Inits WinForms components
            InitializeComponent();

            this.parametersList = parametersList;
            this.enforcementList = enforcementList;
            this.log = log;

            InitProc(serverIP, serverPort, serviceType, assetNumber, sealNumber, roomNumber, building, adRegistered, standard, serviceDate, batteryChange, ticketNumber, inUse, tag, hwType, agentData);
        }

        //Method that allocates a WebView2 instance and checks if args are within standard, then passes them to register method
        public async void InitProc(string serverIP, string serverPort, string serviceType, string assetNumber, string sealNumber, string roomNumber, string building, string adRegistered, string standard, string serviceDate, string batteryChange, string ticketNumber, string inUse, string tag, string hwType, string[] agentData)
        {
            #region

            /** !!Labels!!
             * serverArgs[0](serverIP)
             * serverArgs[1](serverPort)
             * serverArgs[2](serviceType)
             * serverArgs[3](assetNumber)
             * serverArgs[4](sealNumber)
             * serverArgs[5](roomNumber)
             * serverArgs[6](building)
             * serverArgs[7](adRegistered)
             * serverArgs[8](standard)
             * serverArgs[9](serviceDate)
             * serverArgs[10](batteryChange)
             * serverArgs[11](ticketNumber)
             * serverArgs[12](inUse)
             * serverArgs[13](tag)
             * serverArgs[14](hwType)
             * serverArgs[15]()
             * serverArgs[16]()
             * serverArgs[17](brand)
             * serverArgs[18](model)
             * serverArgs[19](serialNumber)
             * serverArgs[20](processor)
             * serverArgs[21](ram)
             * serverArgs[22](storageSize)
             * serverArgs[23](operatingSystem)
             * serverArgs[24](hostname)
             * serverArgs[25](fwVersion)
             * serverArgs[26](macAddress)
             * serverArgs[27](ipAddress)
             * serverArgs[28](fwType)
             * serverArgs[29](storageType)
             * serverArgs[30](videoCard)
             * serverArgs[31](mediaOperationMode)
             * serverArgs[32](secureBoot)
             * serverArgs[33](virtualizationTechnology)
             * serverArgs[34](tpmVersion)
             * serverArgs[35](agent)
             */
            serverArgs = new string[36];

            serverArgs[0] = serverIP;
            serverArgs[1] = serverPort;
            serverArgs[2] = serviceType;
            serverArgs[3] = assetNumber;
            serverArgs[4] = sealNumber;
            serverArgs[5] = roomNumber;
            serverArgs[6] = building;
            serverArgs[7] = adRegistered;
            serverArgs[8] = standard;
            serverArgs[9] = serviceDate;
            serverArgs[10] = batteryChange;
            serverArgs[11] = ticketNumber;
            serverArgs[12] = inUse;
            serverArgs[13] = tag;
            serverArgs[14] = hwType;
            serverArgs[35] = agentData[0];

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

            #endregion

            //Fetch building and hw types info from the specified server
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_FETCHING_SERVER_DATA, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            jsonServerSettings = ConfigFileReader.FetchInfoST(serverIP, serverPort);
            parametersList[2] = jsonServerSettings[0];
            parametersList[3] = jsonServerSettings[1];
            parametersList[4] = jsonServerSettings[0]; //Buildings
            parametersList[5] = jsonServerSettings[1]; //Hw Types
            parametersList[6] = jsonServerSettings[2]; //Firmware Types
            parametersList[7] = jsonServerSettings[3]; //Tpm Types
            parametersList[8] = jsonServerSettings[4]; //Media Op Types
            parametersList[9] = jsonServerSettings[5]; //Secure Boot States
            parametersList[10] = jsonServerSettings[6]; //Virtualization Technology States

            webView2 = new WebView2();

            await LoadWebView2();

            string[] dateFormat = new string[] { ConstantsDLL.Properties.Resources.dateFormat };

            if (serverArgs[0].Length <= 15 && serverArgs[0].Length > 6 && //serverIP
                serverArgs[1].Length <= 5 && serverArgs[1].All(char.IsDigit) && //serverPort
                StringsAndConstants.listModeCLI.Contains(serverArgs[2]) && //serviceType
                serverArgs[3].Length <= 6 && serverArgs[3].Length >= 0 && serverArgs[3].All(char.IsDigit) && //assetNumber
                ((serverArgs[4].Length <= 10 && serverArgs[4].All(char.IsDigit)) || serverArgs[4].Equals(StringsAndConstants.cliDefaultUnchanged)) && //sealNumber
                ((serverArgs[5].Length <= 4 && serverArgs[5].Length > 0 && serverArgs[5].All(char.IsDigit)) || serverArgs[5].Equals(StringsAndConstants.cliDefaultUnchanged)) && //roomNumber
                (parametersList[2].Contains(serverArgs[6]) || serverArgs[6].Equals(StringsAndConstants.cliDefaultUnchanged)) && //building
                (StringsAndConstants.listActiveDirectoryCLI.Contains(serverArgs[7]) || serverArgs[7].Equals(StringsAndConstants.cliDefaultUnchanged)) && //adRegistered
                (StringsAndConstants.listStandardCLI.Contains(serverArgs[8]) || serverArgs[8].Equals(StringsAndConstants.cliDefaultUnchanged)) && //standard
                ((serverArgs[9].Length == 10 && DateTime.TryParseExact(serverArgs[9], dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out DateTime datetime)) || serverArgs[9].Equals(ConstantsDLL.Properties.Resources.today)) && //Data
                StringsAndConstants.listBatteryCLI.Contains(serverArgs[10]) && //batteryChange
                serverArgs[11].Length <= 6 && serverArgs[11].All(char.IsDigit) && //Ticket
                (StringsAndConstants.listInUseCLI.Contains(serverArgs[12]) || serverArgs[12].Equals(StringsAndConstants.cliDefaultUnchanged)) && //inUse
                (StringsAndConstants.listTagCLI.Contains(serverArgs[13]) || serverArgs[13].Equals(StringsAndConstants.cliDefaultUnchanged)) && //tag
                (parametersList[3].Contains(serverArgs[14]) || serverArgs[14].Equals(StringsAndConstants.cliDefaultUnchanged))) //hwType
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PINGGING_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                serverOnline = await ModelFileReader.CheckHostMT(serverArgs[0], serverArgs[1]);
                if (serverOnline && serverArgs[1] != string.Empty)
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SERVER_DETAIL, serverArgs[0] + ":" + serverArgs[1], true);
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_ONLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

                    CollectThread();

                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), MiscMethods.SinceLabelUpdate(true), string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), MiscMethods.SinceLabelUpdate(false), string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    //assetNumber
                    if (serverArgs[3].Equals(string.Empty))
                    {
                        serverArgs[3] = HardwareInfo.GetHostname().Substring(3);
                    }

                    string[] assetJsonStr = AssetFileReader.FetchInfoST(serverArgs[3], serverArgs[0], serverArgs[1]);
                    //If PC Json does not exist and there are some 'mesmo' cmd switch word
                    if (assetJsonStr[0] == "false" && serverArgs.Contains(StringsAndConstants.cliDefaultUnchanged))
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.LOG_SAMEWORD_NOFIRSTREGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                        webView2.Dispose();
                        Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
                    }
                    else if (assetJsonStr[0] == "false") //If PC Json does not exist
                    {
                        //serviceType
                        if (serverArgs[2].Equals(StringsAndConstants.cliServiceType0))
                        {
                            serverArgs[2] = ConstantsDLL.Properties.Resources.formatURL;
                        }
                        else if (serverArgs[2].Equals(StringsAndConstants.cliServiceType1))
                        {
                            serverArgs[2] = ConstantsDLL.Properties.Resources.maintenanceURL;
                        }
                        //building
                        serverArgs[6] = Array.IndexOf(parametersList[2], serverArgs[6]).ToString();
                        //adRegistered
                        if (serverArgs[7].Equals(StringsAndConstants.listNoAbbrev))
                        {
                            serverArgs[7] = "0";
                        }
                        else if (serverArgs[7].Equals(StringsAndConstants.listYesAbbrev))
                        {
                            serverArgs[7] = "1";
                        }
                        //standard
                        if (serverArgs[8].Equals(StringsAndConstants.cliEmployeeType0))
                        {
                            serverArgs[8] = "0";
                        }
                        else if (serverArgs[8].Equals(StringsAndConstants.cliEmployeeType1))
                        {
                            serverArgs[8] = "1";
                        }
                        //batteryChange
                        if (serverArgs[10].Equals(StringsAndConstants.listNoAbbrev))
                        {
                            serverArgs[10] = "0";
                        }
                        else if (serverArgs[10].Equals(StringsAndConstants.listYesAbbrev))
                        {
                            serverArgs[10] = "1";
                        }
                        //inUse
                        if (serverArgs[12].Equals(StringsAndConstants.listNoAbbrev))
                        {
                            serverArgs[12] = "0";
                        }
                        else if (serverArgs[12].Equals(StringsAndConstants.listYesAbbrev))
                        {
                            serverArgs[12] = "1";
                        }
                        //tag
                        if (serverArgs[13].Equals(StringsAndConstants.listNoAbbrev))
                        {
                            serverArgs[13] = "0";
                        }
                        else if (serverArgs[13].Equals(StringsAndConstants.listYesAbbrev))
                        {
                            serverArgs[13] = "1";
                        }
                    }
                    else //If PC Json does exist
                    {
                        //If PC is discarded
                        if (assetJsonStr[9] == "1")
                        {
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.PC_DROPPED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                            webView2.Dispose();
                            Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
                        }
                        //serviceType
                        if (serverArgs[2].Equals(StringsAndConstants.cliServiceType0))
                        {
                            serverArgs[2] = ConstantsDLL.Properties.Resources.formatURL;
                        }
                        else if (serverArgs[2].Equals(StringsAndConstants.cliServiceType1))
                        {
                            serverArgs[2] = ConstantsDLL.Properties.Resources.maintenanceURL;
                        }
                        //sealNumber
                        if (serverArgs[4].Equals(StringsAndConstants.cliDefaultUnchanged))
                        {
                            serverArgs[4] = assetJsonStr[6];
                        }
                        //roomNumber
                        if (serverArgs[5].Equals(StringsAndConstants.cliDefaultUnchanged))
                        {
                            serverArgs[5] = assetJsonStr[2];
                        }
                        //building
                        serverArgs[6] = serverArgs[6].Equals(StringsAndConstants.cliDefaultUnchanged)
                            ? assetJsonStr[1]
                            : Array.IndexOf(parametersList[2], serverArgs[6]).ToString();
                        //adRegistered
                        if (serverArgs[7].Equals(StringsAndConstants.cliDefaultUnchanged))
                        {
                            serverArgs[7] = assetJsonStr[4];
                        }
                        else if (serverArgs[7].Equals(StringsAndConstants.listNoAbbrev))
                        {
                            serverArgs[7] = "0";
                        }
                        else if (serverArgs[7].Equals(StringsAndConstants.listYesAbbrev))
                        {
                            serverArgs[7] = "1";
                        }
                        //standard
                        if (serverArgs[8].Equals(StringsAndConstants.cliDefaultUnchanged))
                        {
                            serverArgs[8] = assetJsonStr[3];
                        }
                        else if (serverArgs[8].Equals(StringsAndConstants.cliEmployeeType0))
                        {
                            serverArgs[8] = "0";
                        }
                        else if (serverArgs[8].Equals(StringsAndConstants.cliEmployeeType1))
                        {
                            serverArgs[8] = "1";
                        }
                        //batteryChange
                        if (serverArgs[10].Equals(StringsAndConstants.listNoAbbrev))
                        {
                            serverArgs[10] = "0";
                        }
                        else if (serverArgs[10].Equals(StringsAndConstants.listYesAbbrev))
                        {
                            serverArgs[10] = "1";
                        }
                        //inUse
                        if (serverArgs[12].Equals(StringsAndConstants.cliDefaultUnchanged))
                        {
                            serverArgs[12] = assetJsonStr[5];
                        }
                        else if (serverArgs[12].Equals(StringsAndConstants.listNoAbbrev))
                        {
                            serverArgs[12] = "0";
                        }
                        else if (serverArgs[12].Equals(StringsAndConstants.listYesAbbrev))
                        {
                            serverArgs[12] = "1";
                        }
                        //tag
                        if (serverArgs[13].Equals(StringsAndConstants.cliDefaultUnchanged))
                        {
                            serverArgs[13] = assetJsonStr[7];
                        }
                        else if (serverArgs[13].Equals(StringsAndConstants.listNoAbbrev))
                        {
                            serverArgs[13] = "0";
                        }
                        else if (serverArgs[13].Equals(StringsAndConstants.listYesAbbrev))
                        {
                            serverArgs[13] = "1";
                        }
                        //hwType
                        serverArgs[14] = serverArgs[14].Equals(StringsAndConstants.cliDefaultUnchanged)
                            ? assetJsonStr[8]
                            : Array.IndexOf(parametersList[3], serverArgs[14]).ToString();
                    }
                    PrintHardwareData();

                    //If there are no pendencies
                    if (pass)
                    {
                        DateTime d = new DateTime();
                        DateTime todayDate = DateTime.Today;
                        bool tDay;

                        try //If there is database record of the asset number
                        {
                            //If chosen date is 'hoje'
                            if (serverArgs[9].Equals(ConstantsDLL.Properties.Resources.today))
                            {
                                serverArgs[9] = DateTime.Today.ToString(ConstantsDLL.Properties.Resources.dateFormat).Substring(0, 10);
                                tDay = true;
                            }
                            else //If chosen date is not 'hoje'
                            {
                                d = DateTime.Parse(serverArgs[9]);
                                serverArgs[9] = d.ToString(ConstantsDLL.Properties.Resources.dateFormat);
                                tDay = false;
                            }

                            //Calculates last registered date with chosen date
                            DateTime registerDate = DateTime.ParseExact(serverArgs[9], ConstantsDLL.Properties.Resources.dateFormat, CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(assetJsonStr[10], ConstantsDLL.Properties.Resources.dateFormat, CultureInfo.InvariantCulture);

                            //If chosen date is later than the registered date
                            if (registerDate >= lastRegisterDate)
                            {
                                if (tDay) //If today
                                {
                                    serverArgs[9] = todayDate.ToString(ConstantsDLL.Properties.Resources.dateFormat).Substring(0, 10);
                                }
                                else if (registerDate <= todayDate) //If today is greater or equal than registered date
                                {
                                    serverArgs[9] = DateTime.Parse(serverArgs[9]).ToString(ConstantsDLL.Properties.Resources.dateFormat).Substring(0, 10);
                                }
                                else //Forbids future registering
                                {
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.INCORRECT_FUTURE_REGISTER_DATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                    webView2.Dispose();
                                    Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
                                }

                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                ServerSendInfo(serverArgs); //Send info to server
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

                                //Resets host install date
                                if (serviceType == StringsAndConstants.cliServiceType0)
                                {
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_INSTALLDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                    MiscMethods.RegCreate(true, serverArgs[9]);
                                }
                                if (serviceType == StringsAndConstants.cliServiceType1) //Resets host maintenance date
                                {
                                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                    MiscMethods.RegCreate(false, serverArgs[9]);
                                }
                            }
                            else //If chosen date is earlier than the registered date, show an error
                            {
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.INCORRECT_REGISTER_DATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                webView2.Dispose(); //Kills WebView2 instance
                                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR)); //Exits
                            }
                        }
                        catch //If there is no database record of the asset number
                        {
                            if (serverArgs[9].Equals(ConstantsDLL.Properties.Resources.today))
                            {
                                serverArgs[9] = todayDate.ToString(ConstantsDLL.Properties.Resources.dateFormat).Substring(0, 10);
                            }
                            else
                            {
                                d = DateTime.Parse(serverArgs[9]);
                                serverArgs[9] = d.ToString(ConstantsDLL.Properties.Resources.dateFormat).Substring(0, 10);
                            }

                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                            ServerSendInfo(serverArgs); //Send info to server
                            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

                            //Resets host install date
                            if (serviceType == StringsAndConstants.cliServiceType0)
                            {
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_INSTALLDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                MiscMethods.RegCreate(true, serverArgs[9]);
                            }
                            else if (serviceType == StringsAndConstants.cliServiceType1) //Resets host maintenance date
                            {
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_RESETTING_MAINTENANCEDATE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                                MiscMethods.RegCreate(false, serverArgs[9]);
                            }
                        }
                    }
                    else //If there are pendencies
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.FIX_PROBLEMS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                        for (int i = 0; i < serverAlert.Length; i++)
                        {
                            if (serverAlertBool[i])
                            {
                                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), serverAlert[i], string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                            }
                        }
                        webView2.Dispose(); //Kills WebView2 instance
                        Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_WARNING)); //Exits
                    }
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.LOG_OFFLINE_SERVER, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    webView2.Dispose(); //Kills WebView2 instance
                    Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR)); //Exits
                }
            }
            else
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.ARGS_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                webView2.Dispose(); //Kills WebView2 instance
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR)); //Exits
            }
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CLOSING_CLI, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_MISC), ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Deletes downloaded json files
            File.Delete(StringsAndConstants.modelFilePath);
            File.Delete(StringsAndConstants.credentialsFilePath);
            File.Delete(StringsAndConstants.assetFilePath);
            File.Delete(StringsAndConstants.configFilePath);
            webView2.NavigationCompleted += WebView2_NavigationCompleted;
            Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_SUCCESS)); //Exits
        }

        //When WebView2 navigation is finished
        public void WebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webView2.Dispose(); //Kills instance
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_SUCCESS)); //Exits
            }
        }

        //Prints the collected data into the form labels, warning the agent when there are forbidden modes
        private void PrintHardwareData()
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_FETCHING_BIOSFILE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            pass = true;

            try
            {
                //Feches model info from server
                string[] modelJsonStr = ModelFileReader.FetchInfoST(serverArgs[17], serverArgs[18], serverArgs[28], serverArgs[34], serverArgs[31], serverArgs[0], serverArgs[1]);

                //If hostname is the default one and its enforcement is enabled
                if (enforcementList[3] == "true" && serverArgs[24].Equals(Strings.DEFAULT_HOSTNAME))
                {
                    pass = false;
                    serverAlert[0] += serverArgs[24] + Strings.HOSTNAME_ALERT;
                    serverAlertBool[0] = true;
                }
                //If model Json file does exist, mediaOpMode enforcement is enabled, and the mode is incorrect
                if (enforcementList[2] == "true" && modelJsonStr != null && modelJsonStr[3].Equals("false"))
                {
                    pass = false;
                    serverAlert[1] += serverArgs[31] + Strings.MEDIA_OPERATION_ALERT;
                    serverAlertBool[1] = true;
                }
                //The section below contains the exception cases for Secure Boot enforcement, if it is enabled
                if (enforcementList[6] == "true" && serverArgs[32].Equals(ConstantsDLL.Properties.Strings.deactivated) &&
                    !serverArgs[30].Contains(ConstantsDLL.Properties.Resources.nonSecBootGPU1) &&
                    !serverArgs[30].Contains(ConstantsDLL.Properties.Resources.nonSecBootGPU2))
                {
                    pass = false;
                    serverAlert[2] += serverArgs[32] + Strings.SECURE_BOOT_ALERT;
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
                if (enforcementList[5] == "true" && modelJsonStr != null && !serverArgs[25].Contains(modelJsonStr[0]))
                {
                    if (!modelJsonStr[0].Equals("-1"))
                    {
                        pass = false;
                        serverAlert[4] += serverArgs[25] + Strings.BIOS_VERSION_ALERT;
                        serverAlertBool[4] = true;
                    }
                }
                //If model Json file does exist, firmware type enforcement is enabled, and the type is incorrect
                if (enforcementList[4] == "true" && modelJsonStr != null && modelJsonStr[1].Equals("false"))
                {
                    pass = false;
                    serverAlert[5] += serverArgs[28] + Strings.FIRMWARE_TYPE_ALERT;
                    serverAlertBool[5] = true;
                }
                //If there is no MAC address assigned
                if (serverArgs[26] == string.Empty)
                {
                    pass = false;
                    serverAlert[6] += serverArgs[26] + Strings.NETWORK_ERROR; //Prints a network error
                    serverAlert[7] += serverArgs[27] + Strings.NETWORK_ERROR; //Prints a network error
                    serverAlertBool[6] = true;
                    serverAlertBool[7] = true;
                }
                //If Virtualization Technology is disabled for UEFI and its enforcement is enabled
                if (enforcementList[7] == "true" && serverArgs[33] == ConstantsDLL.Properties.Strings.deactivated)
                {
                    pass = false;
                    serverAlert[8] += serverArgs[33] + Strings.VT_ALERT;
                    serverAlertBool[8] = true;
                }
                //If model Json file does exist, TPM enforcement is enabled, and TPM version is incorrect
                if (enforcementList[8] == "true" && modelJsonStr != null && modelJsonStr[2].Equals("false"))
                {
                    pass = false;
                    serverAlert[9] += serverArgs[34] + Strings.TPM_ERROR;
                    serverAlertBool[9] = true;
                }
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetRamAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, and its limit enforcement is enabled, shows an alert
                if (enforcementList[0] == "true" && d < 4.0 && Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    serverAlert[10] += serverArgs[21] + Strings.NOT_ENOUGH_MEMORY;
                    serverAlertBool[10] = true;
                }
                //If RAM is more than 4GB and OS is x86, and its limit enforcement is enabled, shows an alert
                if (enforcementList[0] == "true" && d > 4.0 && !Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    serverAlert[10] += serverArgs[21] + Strings.TOO_MUCH_MEMORY;
                    serverAlertBool[10] = true;
                }
                if (pass)
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_HARDWARE_PASSED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }
            }
            catch (Exception e)
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), e.Message, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            }
        }

        //Runs on a separate thread, calling methods from the HardwareInfo class, and setting the variables,
        // while reporting the progress to the progressbar
        public void CollectThread()
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_START_COLLECTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for PC maker
            serverArgs[17] = HardwareInfo.GetBrand();
            if (serverArgs[17] == ConstantsDLL.Properties.Resources.ToBeFilledByOEM || serverArgs[17] == string.Empty)
            {
                serverArgs[17] = HardwareInfo.GetBrandAlt();
            }

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_BM, serverArgs[17], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for PC model
            serverArgs[18] = HardwareInfo.GetModel();
            if (serverArgs[18] == ConstantsDLL.Properties.Resources.ToBeFilledByOEM || serverArgs[18] == string.Empty)
            {
                serverArgs[18] = HardwareInfo.GetModelAlt();
            }

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MODEL, serverArgs[18], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for motherboard Serial number
            serverArgs[19] = HardwareInfo.GetSerialNumber();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SERIALNO, serverArgs[19], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for CPU information
            serverArgs[20] = HardwareInfo.GetProcessorInfo();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PROCNAME, serverArgs[20], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for RAM amount and total number of slots
            serverArgs[21] = HardwareInfo.GetRam() + " (" + HardwareInfo.GetNumFreeRamSlots(Convert.ToInt32(HardwareInfo.GetNumRamSlots())) +
                Strings.slots_of + HardwareInfo.GetNumRamSlots() + Strings.occupied + ")";
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_PM, serverArgs[21], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for Storage size
            serverArgs[22] = HardwareInfo.GetStorageSize();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_HDSIZE, serverArgs[22], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for Storage type
            serverArgs[29] = HardwareInfo.GetStorageType();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MEDIATYPE, serverArgs[29], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for Media Operation (IDE/AHCI/NVME)
            serverArgs[31] = HardwareInfo.GetMediaOperationMode();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MEDIAOP, parametersList[8][Convert.ToInt32(serverArgs[31])], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for GPU information
            serverArgs[30] = HardwareInfo.GetVideoCardInfo();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_GPUINFO, serverArgs[30], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for OS infomation
            serverArgs[23] = HardwareInfo.GetOSString();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_OS, serverArgs[23], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for hostname
            serverArgs[24] = HardwareInfo.GetHostname();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_HOSTNAME, serverArgs[24], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for MAC Address
            serverArgs[26] = HardwareInfo.GetMacAddress();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_MAC, serverArgs[26], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for IP Address
            serverArgs[27] = HardwareInfo.GetIpAddress();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_IP, serverArgs[27], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for firmware type
            serverArgs[28] = HardwareInfo.GetFwType();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_BIOSTYPE, parametersList[6][Convert.ToInt32(serverArgs[28])], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for Secure Boot status
            serverArgs[32] = HardwareInfo.GetSecureBoot();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SECBOOT, StringsAndConstants.listStates[Convert.ToInt32(parametersList[9][Convert.ToInt32(serverArgs[32])])], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for BIOS version
            serverArgs[25] = HardwareInfo.GetFirmwareVersion();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_BIOS, serverArgs[25], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for VT status
            serverArgs[33] = HardwareInfo.GetVirtualizationTechnology();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_VT, StringsAndConstants.listStates[Convert.ToInt32(parametersList[10][Convert.ToInt32(serverArgs[33])])], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            //Scans for TPM status
            serverArgs[34] = HardwareInfo.GetTPMStatus();
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_TPM, parametersList[7][Convert.ToInt32(serverArgs[34])], Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_END_COLLECTING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
        }

        //Loads webView2 component
        public async Task LoadWebView2()
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_START_LOADING_WEBVIEW2, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            CoreWebView2Environment webView2Environment = Environment.Is64BitOperatingSystem
                ? await CoreWebView2Environment.CreateAsync(ConstantsDLL.Properties.Resources.WEBVIEW2_SYSTEM_PATH_X64 + MiscMethods.GetWebView2Version(), Path.GetTempPath())
                : await CoreWebView2Environment.CreateAsync(ConstantsDLL.Properties.Resources.WEBVIEW2_SYSTEM_PATH_X86 + MiscMethods.GetWebView2Version(), Path.GetTempPath());
            await webView2.EnsureCoreWebView2Async(webView2Environment);
            webView2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView2.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_END_LOADING_WEBVIEW2, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
        }

        //Sends hardware info to the specified server
        public void ServerSendInfo(string[] serverArgs)
        {
            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_REGISTERING, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            webView2.CoreWebView2.Navigate(ConstantsDLL.Properties.Resources.HTTP + serverArgs[0] + ":" + serverArgs[1] + "/" + serverArgs[2] + ".php"
                + ConstantsDLL.Properties.Resources.phpAssetNumber + serverArgs[3]
                + ConstantsDLL.Properties.Resources.phpSealNumber + serverArgs[4]
                + ConstantsDLL.Properties.Resources.phpRoom + serverArgs[5]
                + ConstantsDLL.Properties.Resources.phpBuilding + serverArgs[6]
                + ConstantsDLL.Properties.Resources.phpAdRegistered + serverArgs[7]
                + ConstantsDLL.Properties.Resources.phpStandard + serverArgs[8]
                + ConstantsDLL.Properties.Resources.phpServiceDate + serverArgs[9]
                + ConstantsDLL.Properties.Resources.phpPreviousServiceDates + serverArgs[9]
                + ConstantsDLL.Properties.Resources.phpBrand + serverArgs[17]
                + ConstantsDLL.Properties.Resources.phpModel + serverArgs[18]
                + ConstantsDLL.Properties.Resources.phpSerialNumber + serverArgs[19]
                + ConstantsDLL.Properties.Resources.phpProcessor + serverArgs[20]
                + ConstantsDLL.Properties.Resources.phpRam + serverArgs[21]
                + ConstantsDLL.Properties.Resources.phpStorageSize + serverArgs[22]
                + ConstantsDLL.Properties.Resources.phpOperatingSystem + serverArgs[23]
                + ConstantsDLL.Properties.Resources.phpHostname + serverArgs[24]
                + ConstantsDLL.Properties.Resources.phpFwVersion + serverArgs[25]
                + ConstantsDLL.Properties.Resources.phpMacAddress + serverArgs[26]
                + ConstantsDLL.Properties.Resources.phpIpAddress + serverArgs[27]
                + ConstantsDLL.Properties.Resources.phpInUse + serverArgs[12]
                + ConstantsDLL.Properties.Resources.phpTag + serverArgs[13]
                + ConstantsDLL.Properties.Resources.phpHwType + serverArgs[14]
                + ConstantsDLL.Properties.Resources.phpFwType + serverArgs[28]
                + ConstantsDLL.Properties.Resources.phpStorageType + serverArgs[29]
                + ConstantsDLL.Properties.Resources.phpVideoCard + serverArgs[30]
                + ConstantsDLL.Properties.Resources.phpMediaOperationMode + serverArgs[31]
                + ConstantsDLL.Properties.Resources.phpSecureBoot + serverArgs[32]
                + ConstantsDLL.Properties.Resources.phpVirtualizationTechnology + serverArgs[33]
                + ConstantsDLL.Properties.Resources.phpTpmVersion + serverArgs[34]
                + ConstantsDLL.Properties.Resources.phpBatteryChange + serverArgs[10]
                + ConstantsDLL.Properties.Resources.phpTicketNumber + serverArgs[11]
                + ConstantsDLL.Properties.Resources.phpAgent + serverArgs[35]);
        }
    }
}