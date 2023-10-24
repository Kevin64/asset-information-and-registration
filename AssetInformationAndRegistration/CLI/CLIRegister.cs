using AssetInformationAndRegistration.Properties;
using ConstantsDLL;
using ConstantsDLL.Properties;
using HardwareInfoDLL;
using LogGeneratorDLL;
using RestApiDLL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using static AssetInformationAndRegistration.Program;
using Resources = ConstantsDLL.Properties.Resources;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary> 
    /// Class for asset registering via CLI
    /// </summary>
    internal class CLIRegister
    {
        public bool pass, serverOnline;
        private bool[] serverAlertBool;
        private readonly string[] serverArgs;
        private string[] serverAlert;
        private readonly LogGenerator log;
        private readonly List<string[]> parametersList;
        private List<List<string>> videoCardDetailPrev, storageDetailPrev, ramDetailPrev, processorDetailPrev, videoCardDetail, storageDetail, ramDetail, processorDetail;
        private string processorSummary, ramSummary, storageSummary, videoCardSummary, operatingSystemSummary;
        private readonly string serverIP, serverPort;

        private HttpClient client;
        private readonly ConfigurationOptions configOptions;

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

        /// <summary> 
        /// CLI constructor
        /// </summary>
        /// <param name="argsArray">Arguments array</param>
        /// <param name="agent">Agent object</param>
        /// <param name="log">Log file object</param>
        /// <param name="parametersList">List containing data from [Parameters]</param>
        /// <param name="enforcementList">List containing data from [Enforcement]</param>
        internal CLIRegister(string[] argsArray, Agent agent, LogGenerator log, ConfigurationOptions configOptions)
        {
            //Creates a new Asset object and subobjects
            newFirmware = new firmware();
            newProcessor = new List<processor>();
            newRam = new List<ram>();
            newStorage = new List<storage>();
            newVideoCard = new List<videoCard>();
            newLocation = new location()
            {
                building = argsArray[3],
                roomNumber = argsArray[4],
            };
            newMaintenances = new List<maintenances>()
            {
                new maintenances()
                {
                    serviceDate = argsArray[5],
                    serviceType = argsArray[6],
                    batteryChange = argsArray[7],
                    ticketNumber = argsArray[8],
                    agentId = agent.id
                }
            };
            newNetwork = new network();
            newOperatingSystem = new operatingSystem();
            newHardware = new hardware()
            {
                type = argsArray[13],
                processor = newProcessor,
                ram = newRam,
                storage = newStorage,
                videoCard = newVideoCard
            };
            newAsset = new Asset()
            {
                assetNumber = argsArray[2],
                standard = argsArray[9],
                inUse = argsArray[10],
                sealNumber = argsArray[11],
                tag = argsArray[12],

                firmware = newFirmware,
                hardware = newHardware,
                location = newLocation,
                maintenances = newMaintenances,
                network = newNetwork,
                operatingSystem = newOperatingSystem
            };

            serverArgs = argsArray;

            serverIP = argsArray[0];
            serverPort = argsArray[1];

            this.log = log;
            this.configOptions = configOptions;

            InitProc(serverIP, serverPort, newAsset);
        }

        /// <summary> 
        /// Method that checks if args are compliant, then passes them to register method
        /// </summary>
        /// <param name="serverIP">Server IP address</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="newAsset">Asset object</param>
        private void InitProc(string serverIP, string serverPort, Asset newAsset)
        {
            serverAlert = new string[11];
            serverAlertBool = new bool[11];
            serverAlert[0] = AirStrings.CLI_HOSTNAME_ALERT;
            serverAlert[1] = AirStrings.CLI_MEDIA_OPERATION_ALERT;
            serverAlert[2] = AirStrings.CLI_SECURE_BOOT_ALERT;
            serverAlert[3] = AirStrings.CLI_DATABASE_REACH_ERROR;
            serverAlert[4] = AirStrings.CLI_FIRMWARE_VERSION_ALERT;
            serverAlert[5] = AirStrings.CLI_FIRMWARE_TYPE_ALERT;
            serverAlert[6] = AirStrings.CLI_NETWORK_IP_ERROR;
            serverAlert[7] = AirStrings.CLI_NETWORK_MAC_ERROR;
            serverAlert[8] = AirStrings.CLI_VT_ALERT;
            serverAlert[9] = AirStrings.CLI_TPM_ALERT;
            serverAlert[10] = AirStrings.CLI_MEMORY_ALERT;

            //Fetch building and hw types info from the specified server
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_SERVER_DATA, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            System.Threading.Tasks.Task<ServerParam> v1 = ParameterHandler.GetParameterAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_PARAMETERS_URL);
            v1.Wait();
            Parameters newParam = new Parameters()
            {
                Buildings = v1.Result.Parameters.Buildings,
                HardwareTypes = v1.Result.Parameters.HardwareTypes,
                FirmwareTypes = v1.Result.Parameters.FirmwareTypes,
                TpmTypes = v1.Result.Parameters.TpmTypes,
                MediaOperationTypes = v1.Result.Parameters.MediaOperationTypes,
                RamTypes = v1.Result.Parameters.RamTypes,
                SecureBootStates = v1.Result.Parameters.SecureBootStates,
                VirtualizationTechnologyStates = v1.Result.Parameters.VirtualizationTechnologyStates
            };

            string[] dateFormat = new string[] { Resources.DATE_FORMAT };

            if (serverIP.Length <= 15 && serverIP.Length > 6 && //serverIP
                serverPort.Length <= 5 && serverPort.All(char.IsDigit) && //serverPort
                newAsset.assetNumber.Length <= 6 && newAsset.assetNumber.Length >= 0 && newAsset.assetNumber.All(char.IsDigit) && //assetNumber
                (parametersList[4].Contains(newAsset.location.building) || newAsset.location.building.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //building
                ((newAsset.location.roomNumber.Length <= 4 && newAsset.location.roomNumber.Length > 0 && newAsset.location.roomNumber.All(char.IsDigit)) || newAsset.location.roomNumber.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //roomNumber
                ((newAsset.maintenances[0].serviceDate.Length == 10 && DateTime.TryParseExact(newAsset.maintenances[0].serviceDate, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out DateTime datetime)) || newAsset.maintenances[0].serviceDate.Equals(Resources.TODAY)) && //serviceDate
                StringsAndConstants.LIST_MODE_CLI.Contains(newAsset.maintenances[0].serviceType) && //serviceType
                StringsAndConstants.LIST_BATTERY_CLI.Contains(newAsset.maintenances[0].batteryChange) && //batteryChange
                newAsset.maintenances[0].ticketNumber.Length <= 6 && newAsset.maintenances[0].ticketNumber.All(char.IsDigit) && //ticketNumber
                (StringsAndConstants.LIST_STANDARD_CLI.Contains(newAsset.standard) || newAsset.standard.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //standard
                (StringsAndConstants.LIST_IN_USE_CLI.Contains(newAsset.inUse) || newAsset.inUse.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //inUse
                ((newAsset.sealNumber.Length <= 10 && newAsset.sealNumber.All(char.IsDigit)) || newAsset.sealNumber.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //sealNumber
                (StringsAndConstants.LIST_TAG_CLI.Contains(newAsset.tag) || newAsset.tag.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //tag
                (parametersList[5].Contains(newAsset.hardware.type) || newAsset.hardware.type.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))) //hwType
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PINGGING_SERVER, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                serverOnline = true; /* await JsonFileReaderDLL.ModelFileReader.CheckHostMT(serverIP, serverPort);*/
                if (serverOnline && serverPort != string.Empty)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SERVER_DATA, serverIP + ":" + serverPort, true);
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_ONLINE_SERVER, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));

                    CollectThread();

                    //assetNumber
                    if (newAsset.assetNumber.Equals(string.Empty))
                        newAsset.assetNumber = HardwareInfo.GetHostname().Substring(3);

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_ASSET_DATA, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_GUI));

                    //Feches asset number data from server
                    try
                    {
                        client = new HttpClient
                        {
                            BaseAddress = new Uri(Resources.HTTP + serverIP + ":" + serverPort)
                        };
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Resources.HTTP_CONTENT_TYPE_JSON));

                        System.Threading.Tasks.Task<Asset> v = AssetHandler.GetAssetAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_ASSET_URL + newAsset.assetNumber);
                        v.Wait();
                        existingAsset = v.Result;
                    }
                    catch
                    {

                    }


                    //If asset Json does not exist and there are some 'same' cmd switch word
                    if (existingAsset == null && serverArgs.Contains(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), LogStrings.LOG_SAMEWORD_NOFIRSTREGISTRY, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                        Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                    }
                    else if (existingAsset == null) //If asset Json does not exist
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Misc.MiscMethods.SinceLabelUpdate(string.Empty), string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));

                        //serviceType
                        if (newAsset.maintenances[0].serviceType.Equals(StringsAndConstants.CLI_SERVICE_TYPE_0))
                            newAsset.maintenances[0].serviceType = "0";
                        else if (newAsset.maintenances[0].serviceType.Equals(StringsAndConstants.CLI_SERVICE_TYPE_1))
                            newAsset.maintenances[0].serviceType = "1";
                        //building
                        newAsset.location.building = Array.IndexOf(parametersList[4], newAsset.location.building).ToString();
                        //standard
                        if (newAsset.standard.Equals(StringsAndConstants.CLI_EMPLOYEE_TYPE_0))
                            newAsset.standard = Convert.ToInt32(SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.standard.Equals(StringsAndConstants.CLI_EMPLOYEE_TYPE_1))
                            newAsset.standard = Convert.ToInt32(SpecBinaryStates.ENABLED).ToString();
                        //batteryChange
                        if (newAsset.maintenances[0].batteryChange.Equals(StringsAndConstants.LIST_NO_ABBREV))
                            newAsset.maintenances[0].batteryChange = Convert.ToInt32(SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.maintenances[0].batteryChange.Equals(StringsAndConstants.LIST_YES_ABBREV))
                            newAsset.maintenances[0].batteryChange = Convert.ToInt32(SpecBinaryStates.ENABLED).ToString();
                        //inUse
                        if (newAsset.inUse.Equals(StringsAndConstants.LIST_NO_ABBREV))
                            newAsset.inUse = Convert.ToInt32(SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.inUse.Equals(StringsAndConstants.LIST_YES_ABBREV))
                            newAsset.inUse = Convert.ToInt32(SpecBinaryStates.ENABLED).ToString();
                        //tag
                        if (newAsset.tag.Equals(StringsAndConstants.LIST_NO_ABBREV))
                            newAsset.tag = Convert.ToInt32(SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.tag.Equals(StringsAndConstants.LIST_YES_ABBREV))
                            newAsset.tag = Convert.ToInt32(SpecBinaryStates.ENABLED).ToString();
                        //hwType
                        newAsset.hardware.type = Array.IndexOf(parametersList[5], newAsset.hardware.type).ToString();
                    }
                    else //If asset Json does exist
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Misc.MiscMethods.SinceLabelUpdate(existingAsset.maintenances[0].serviceDate), string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));

                        //If asset is discarded
                        if (existingAsset.discarded == "1")
                        {
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.ASSET_DROPPED, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                            Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                        }
                        //serviceType
                        if (newAsset.maintenances[0].serviceType.Equals(StringsAndConstants.CLI_SERVICE_TYPE_0))
                            newAsset.maintenances[0].serviceType = "0";
                        else if (newAsset.maintenances[0].serviceType.Equals(StringsAndConstants.CLI_SERVICE_TYPE_1))
                            newAsset.maintenances[0].serviceType = "1";
                        //sealNumber
                        if (newAsset.sealNumber.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.sealNumber = existingAsset.sealNumber;
                        //roomNumber
                        if (newAsset.location.roomNumber.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.location.roomNumber = existingAsset.location.roomNumber;
                        //building
                        if (newAsset.location.building.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.location.building = existingAsset.location.building;
                        else
                            newAsset.location.building = Array.IndexOf(parametersList[4], newAsset.location.building).ToString();
                        //standard
                        if (newAsset.standard.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.standard = existingAsset.standard;
                        else if (newAsset.standard.Equals(StringsAndConstants.CLI_EMPLOYEE_TYPE_0))
                            newAsset.standard = Convert.ToInt32(SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.standard.Equals(StringsAndConstants.CLI_EMPLOYEE_TYPE_1))
                            newAsset.standard = Convert.ToInt32(SpecBinaryStates.ENABLED).ToString();
                        //batteryChange
                        if (newAsset.maintenances[0].batteryChange.Equals(StringsAndConstants.LIST_NO_ABBREV))
                            newAsset.maintenances[0].batteryChange = Convert.ToInt32(SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.maintenances[0].batteryChange.Equals(StringsAndConstants.LIST_YES_ABBREV))
                            newAsset.maintenances[0].batteryChange = Convert.ToInt32(SpecBinaryStates.ENABLED).ToString();
                        //inUse
                        if (newAsset.inUse.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.inUse = existingAsset.inUse;
                        else if (newAsset.inUse.Equals(StringsAndConstants.LIST_NO_ABBREV))
                            newAsset.inUse = Convert.ToInt32(SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.inUse.Equals(StringsAndConstants.LIST_YES_ABBREV))
                            newAsset.inUse = Convert.ToInt32(SpecBinaryStates.ENABLED).ToString();
                        //tag
                        if (newAsset.tag.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.tag = existingAsset.tag;
                        else if (newAsset.tag.Equals(StringsAndConstants.LIST_NO_ABBREV))
                            newAsset.tag = Convert.ToInt32(SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.tag.Equals(StringsAndConstants.LIST_YES_ABBREV))
                            newAsset.tag = Convert.ToInt32(SpecBinaryStates.ENABLED).ToString();
                        //hwType
                        if (newAsset.hardware.type.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.hardware.type = existingAsset.hardware.type;
                        else
                            newAsset.hardware.type = Array.IndexOf(parametersList[5], newAsset.hardware.type).ToString();
                    }
                    PrintHardwareData();

                    //If there are no pendencies
                    if (pass)
                    {
                        DateTime d = new DateTime();
                        DateTime todayDate = DateTime.Today;
                        bool tDay;

                        try
                        {
                            _ = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain();
                            newAsset.adRegistered = Convert.ToInt32(SpecBinaryStates.ENABLED).ToString();
                        }
                        catch
                        {
                            newAsset.adRegistered = Convert.ToInt32(SpecBinaryStates.DISABLED).ToString();
                        }

                        try //If there is database record of the asset number
                        {
                            //If chosen date is 'today'
                            if (newAsset.maintenances[0].serviceDate.Equals(Resources.TODAY))
                            {
                                newAsset.maintenances[0].serviceDate = DateTime.Today.ToString(Resources.DATE_FORMAT).Substring(0, 10);
                                tDay = true;
                            }
                            else //If chosen date is not 'today'
                            {
                                d = DateTime.Parse(newAsset.maintenances[0].serviceDate);
                                newAsset.maintenances[0].serviceDate = d.ToString(Resources.DATE_FORMAT);
                                tDay = false;
                            }

                            //Calculates last registered date with chosen date
                            DateTime registerDate = DateTime.ParseExact(newAsset.maintenances[0].serviceDate, Resources.DATE_FORMAT, CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(existingAsset.maintenances[0].serviceDate, Resources.DATE_FORMAT, CultureInfo.InvariantCulture);

                            //If chosen date is later than the registered date
                            if (registerDate >= lastRegisterDate)
                            {
                                if (tDay) //If today
                                {
                                    newAsset.maintenances[0].serviceDate = todayDate.ToString(Resources.DATE_FORMAT).Substring(0, 10);
                                }
                                else if (registerDate <= todayDate) //If today is greater or equal than registered date
                                {
                                    newAsset.maintenances[0].serviceDate = DateTime.Parse(newAsset.maintenances[0].serviceDate).ToString(Resources.DATE_FORMAT).Substring(0, 10);
                                }
                                else //Forbids future registering
                                {
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirStrings.INCORRECT_FUTURE_REGISTER_DATE, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                                    Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                                }

                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                                System.Threading.Tasks.Task<Uri> v = AssetHandler.SetAssetAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_ASSET_URL, newAsset); //Send info to server
                                v.Wait();
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                            }
                            else //If chosen date is earlier than the registered date, show an error
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirStrings.INCORRECT_REGISTER_DATE, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR)); //Exits
                            }
                        }
                        catch //If there is no database record of the asset number
                        {
                            if (newAsset.maintenances[0].serviceDate.Equals(Resources.TODAY))
                            {
                                newAsset.maintenances[0].serviceDate = todayDate.ToString(Resources.DATE_FORMAT).Substring(0, 10);
                            }
                            else
                            {
                                d = DateTime.Parse(newAsset.maintenances[0].serviceDate);
                                newAsset.maintenances[0].serviceDate = d.ToString(Resources.DATE_FORMAT).Substring(0, 10);
                            }

                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                            System.Threading.Tasks.Task<Uri> v = AssetHandler.SetAssetAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_ASSET_URL, newAsset); //Send info to server
                            v.Wait();
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                        }
                    }
                    else //If there are pendencies
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirStrings.FIX_PROBLEMS, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                        for (int i = 0; i < serverAlert.Length; i++)
                        {
                            if (serverAlertBool[i])
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), serverAlert[i], string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                        }
                        Environment.Exit(Convert.ToInt32(ExitCodes.WARNING)); //Exits
                    }
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), LogStrings.LOG_OFFLINE_SERVER, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                    Environment.Exit(Convert.ToInt32(ExitCodes.ERROR)); //Exits
                }
            }
            else
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirStrings.ARGS_ERROR, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR)); //Exits
            }
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_CLI, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));

            Environment.Exit(Convert.ToInt32(ExitCodes.SUCCESS)); //Exits
        }

        /// <summary> 
        /// Prints the collected data into the form labels, warning the agent when there are forbidden modes
        /// </summary>
        private void PrintHardwareData()
        {
            pass = true;

            try
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_MODEL_DATA, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Feches model info from server
                System.Threading.Tasks.Task<Model> v = ModelHandler.GetModelAsync(client, Resources.HTTP + serverIP + ":" + serverPort + Resources.API_MODEL_URL + newAsset.hardware.model);
                v.Wait();
                modelTemplate = v.Result;
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If hostname is the default one and its enforcement is enabled
                if (configOptions.Enforcement.Hostname.ToString() == Resources.TRUE && newAsset.network.hostname.Equals(AirStrings.DEFAULT_HOSTNAME))
                {
                    pass = false;
                    serverAlert[0] += newAsset.network.hostname + AirStrings.HOSTNAME_ALERT;
                    serverAlertBool[0] = true;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, mediaOpMode enforcement is enabled, and the mode is incorrect
                if (configOptions.Enforcement.MediaOperationMode.ToString() == Resources.TRUE && modelTemplate != null && modelTemplate.mediaOperationMode != newAsset.firmware.mediaOperationMode)
                {
                    pass = false;
                    serverAlert[1] += newAsset.firmware.mediaOperationMode + AirStrings.MEDIA_OPERATION_ALERT;
                    serverAlertBool[1] = true;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //The section below contains the exception cases for Secure Boot enforcement, if it is enabled
                if (configOptions.Enforcement.SecureBoot.ToString() == Resources.TRUE && newAsset.firmware.secureBoot.Equals(Strings.DEACTIVATED))
                {
                    pass = false;
                    serverAlert[2] += newAsset.firmware.secureBoot + AirStrings.SECURE_BOOT_ALERT;
                    serverAlertBool[2] = true;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does not exist and server is unreachable
                if (modelTemplate == null)
                {
                    pass = false;
                    serverAlert[3] += AirStrings.DATABASE_REACH_ERROR;
                    serverAlertBool[3] = true;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, firmware version enforcement is enabled, and the version is incorrect
                if (configOptions.Enforcement.FirmwareVersion.ToString() == Resources.TRUE && modelTemplate != null && !newAsset.firmware.version.Contains(modelTemplate.fwVersion))
                {
                    pass = false;
                    serverAlert[4] += newAsset.firmware.version + AirStrings.FIRMWARE_VERSION_ALERT;
                    serverAlertBool[4] = true;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, firmware type enforcement is enabled, and the type is incorrect
                if (configOptions.Enforcement.FirmwareType.ToString() == Resources.TRUE && modelTemplate != null && modelTemplate.fwType != newAsset.firmware.type)
                {
                    pass = false;
                    serverAlert[5] += newAsset.firmware.type + AirStrings.FIRMWARE_TYPE_ALERT;
                    serverAlertBool[5] = true;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If there is no MAC address assigned
                if (newAsset.network.macAddress == string.Empty)
                {
                    pass = false;
                    serverAlert[6] += newAsset.network.macAddress + AirStrings.NETWORK_ERROR; //Prints a network error
                    serverAlert[7] += newAsset.network.ipAddress + AirStrings.NETWORK_ERROR; //Prints a network error
                    serverAlertBool[6] = true;
                    serverAlertBool[7] = true;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If Virtualization Technology is disabled for UEFI and its enforcement is enabled
                if (configOptions.Enforcement.VirtualizationTechnology.ToString() == Resources.TRUE && newAsset.firmware.virtualizationTechnology == Strings.DEACTIVATED)
                {
                    pass = false;
                    serverAlert[8] += newAsset.firmware.virtualizationTechnology + AirStrings.VT_ALERT;
                    serverAlertBool[8] = true;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, TPM enforcement is enabled, and TPM version is incorrect
                if (configOptions.Enforcement.Tpm.ToString() == Resources.TRUE && modelTemplate != null && modelTemplate.tpmVersion != newAsset.firmware.tpmVersion)
                {
                    pass = false;
                    serverAlert[9] += newAsset.firmware.tpmVersion + AirStrings.TPM_ERROR;
                    serverAlertBool[9] = true;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetRamAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, and its limit enforcement is enabled, shows an alert
                if (configOptions.Enforcement.RamLimit.ToString() == Resources.TRUE && d < 4.0 && Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    serverAlert[10] += newAsset.hardware.ram + AirStrings.NOT_ENOUGH_MEMORY;
                    serverAlertBool[10] = true;
                }
                //If RAM is more than 4GB and OS is x86, and its limit enforcement is enabled, shows an alert
                if (configOptions.Enforcement.RamLimit.ToString() == Resources.TRUE && d > 4.0 && !Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    serverAlert[10] += newAsset.hardware.ram + AirStrings.TOO_MUCH_MEMORY;
                    serverAlertBool[10] = true;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                if (pass)
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_PASSED, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            }
            catch (Exception e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), e.Message, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            }
        }

        /// <summary> 
        /// Runs on a separate thread, calling methods from the HardwareInfo class, and setting the variables
        /// </summary>
        private void CollectThread()
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_START_COLLECTING, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));

            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for PC maker 
            newAsset.hardware.brand = HardwareInfo.GetBrand();
            if (newAsset.hardware.brand == Resources.TO_BE_FILLED_BY_OEM || newAsset.hardware.brand == string.Empty)
                newAsset.hardware.brand = HardwareInfo.GetBrandAlt();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_BM, newAsset.hardware.brand, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for PC model
            newAsset.hardware.model = HardwareInfo.GetModel();
            if (newAsset.hardware.model == Resources.TO_BE_FILLED_BY_OEM || newAsset.hardware.model == string.Empty)
            {
                newAsset.hardware.model = HardwareInfo.GetModelAlt();
                if (newAsset.hardware.model == Resources.TO_BE_FILLED_BY_OEM || newAsset.hardware.model == string.Empty)
                    newAsset.hardware.model = Strings.UNKNOWN;
            }
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_MODEL, newAsset.hardware.model, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for motherboard Serial number
            newAsset.hardware.serialNumber = HardwareInfo.GetSerialNumber();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SERIALNO, newAsset.hardware.serialNumber, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCNAME, processorSummary, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PM, ramSummary, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_MEDIATYPE, storageSummary, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Media Operation (IDE/AHCI/NVME)
            newAsset.firmware.mediaOperationMode = HardwareInfo.GetMediaOperationMode();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_MEDIAOP, parametersList[8][Convert.ToInt32(newAsset.firmware.mediaOperationMode)], Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for GPU information
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
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_GPUINFO, videoCardSummary, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for OS infomation
            newAsset.operatingSystem.arch = HardwareInfo.GetOSArchBinary();
            newAsset.operatingSystem.build = HardwareInfo.GetOSBuildAndRevision();
            newAsset.operatingSystem.name = HardwareInfo.GetOSName();
            newAsset.operatingSystem.version = HardwareInfo.GetOSVersion();
            operatingSystemSummary = HardwareInfo.GetOSSummary();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OS, operatingSystemSummary, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for hostname
            newAsset.network.hostname = HardwareInfo.GetHostname();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HOSTNAME, newAsset.network.hostname, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for MAC Address
            newAsset.network.macAddress = HardwareInfo.GetMacAddress();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_MAC, newAsset.network.macAddress, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for IP Address
            newAsset.network.ipAddress = HardwareInfo.GetIpAddress();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_IP, newAsset.network.ipAddress, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for firmware type
            newAsset.firmware.type = HardwareInfo.GetFwType();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_BIOSTYPE, parametersList[6][Convert.ToInt32(newAsset.firmware.type)], Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Secure Boot status
            newAsset.firmware.secureBoot = HardwareInfo.GetSecureBoot();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SECBOOT, StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[9][Convert.ToInt32(newAsset.firmware.secureBoot)])], Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for firmware version
            newAsset.firmware.version = HardwareInfo.GetFirmwareVersion();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_BIOS, newAsset.firmware.version, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for VT status
            newAsset.firmware.virtualizationTechnology = HardwareInfo.GetVirtualizationTechnology();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_VT, StringsAndConstants.LIST_STATES[Convert.ToInt32(parametersList[10][Convert.ToInt32(newAsset.firmware.virtualizationTechnology)])], Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for TPM status
            newAsset.firmware.tpmVersion = HardwareInfo.GetTPMStatus();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_TPM, parametersList[7][Convert.ToInt32(newAsset.firmware.tpmVersion)], Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_END_COLLECTING, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
        }
    }
}
