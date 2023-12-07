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
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary>
    /// Class for CLI alerts
    /// </summary>
    public class ServerAlert
    {
        public string DatabaseReachError { get; set; }
        public string FirmwareMediaOperationModeAlert { get; set; }
        public string FirmwareSecureBootAlert { get; set; }
        public string FirmwareTpmAlert { get; set; }
        public string FirmwareTypeAlert { get; set; }
        public string FirmwareVersionAlert { get; set; }
        public string FirmwareVirtualizationTechnologyAlert { get; set; }
        public string NetworkIpError { get; set; }
        public string NetworkMacError { get; set; }
        public string NetworkHostnameAlert { get; set; }
        public string RamAlert { get; set; }
        public string StorageSmartStatusAlert { get; set; }
    }

    /// <summary> 
    /// Class for asset registering via CLI
    /// </summary>
    internal class CLIRegister
    {
        public bool pass, serverOnline;
        private readonly string[] serverArgs;
        private readonly LogGenerator log;
        private List<List<string>> videoCardDetail, storageDetail, ramDetail, processorDetail;
        private string processorSummary, ramSummary, storageSummary, videoCardSummary, operatingSystemSummary;
        private List<string> smartValueList;
        private readonly string serverIP, serverPort;

        private readonly HttpClient client;
        private readonly Program.ConfigurationOptions configOptions;

        private Regex hostnamePattern;
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
        private readonly ServerAlert sA;
        private ServerParam serverParam;

        /// <summary>
        /// CLI constructor
        /// </summary>
        /// <param name="client">HTTP client object</param>
        /// <param name="agent">Agent object</param>
        /// <param name="log">Log file object</param>
        /// <param name="configOptions">Config file object</param>
        /// <param name="argsArray">CLI args passed by the entry point</param>
        internal CLIRegister(HttpClient client, Agent agent, LogGenerator log, Program.ConfigurationOptions configOptions, string[] argsArray)
        {
            this.log = log;
            this.configOptions = configOptions;
            this.client = client;

            //Creates a new Asset object and subobjects
            newFirmware = new firmware();
            newProcessor = new List<processor>();
            newRam = new List<ram>();
            newStorage = new List<storage>();
            newVideoCard = new List<videoCard>();
            newLocation = new location()
            {
                locBuilding = argsArray[3],
                locRoomNumber = argsArray[4],
            };
            newMaintenances = new List<maintenances>()
            {
                new maintenances()
                {
                    mainServiceDate = argsArray[5],
                    mainServiceType = argsArray[6],
                    mainBatteryChange = argsArray[7],
                    mainTicketNumber = argsArray[8],
                    mainAgentId = agent.id
                }
            };
            newNetwork = new network();
            newOperatingSystem = new operatingSystem();
            newHardware = new hardware()
            {
                hwType = argsArray[13],
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

            sA = new ServerAlert();

            serverArgs = argsArray;

            serverIP = argsArray[0];
            serverPort = argsArray[1];

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
            sA.NetworkHostnameAlert = AirUIStrings.ALERT_HOSTNAME_CLI_TITLE;
            sA.FirmwareMediaOperationModeAlert = AirUIStrings.ALERT_MEDIA_OPERATION_CLI_TITLE;
            sA.FirmwareSecureBootAlert = AirUIStrings.ALERT_SECURE_BOOT_CLI_TITLE;
            sA.DatabaseReachError = AirUIStrings.ALERT_DATABASE_REACH_CLI_TITLE;
            sA.FirmwareVersionAlert = AirUIStrings.ALERT_FIRMWARE_VERSION_CLI_TITLE;
            sA.FirmwareTypeAlert = AirUIStrings.ALERT_FIRMWARE_TYPE_CLI_TITLE;
            sA.NetworkIpError = AirUIStrings.ALERT_NETWORK_IP_ERROR_CLI_TITLE;
            sA.NetworkMacError = AirUIStrings.ALERT_NETWORK_MAC_ERROR_CLI_TITLE;
            sA.FirmwareVirtualizationTechnologyAlert = AirUIStrings.ALERT_VT_CLI_TITLE;
            sA.FirmwareTpmAlert = AirUIStrings.ALERT_TPM_CLI_TITLE;
            sA.RamAlert = AirUIStrings.ALERT_MEMORY_CLI_TITLE;
            sA.StorageSmartStatusAlert = AirUIStrings.ALERT_SMART_CLI_TITLE;

            //Fetch building and hw types info from the specified server
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_SERVER_PARAMETERS, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            System.Threading.Tasks.Task<ServerParam> sp = ParameterHandler.GetParameterAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_PARAMETERS_URL);
            sp.Wait();

            serverParam = sp.Result;
            hostnamePattern = new Regex(serverParam.Parameters.HostnamePattern);

            string[] dateFormat = new string[] { GenericResources.DATE_FORMAT };

            if (serverIP.Length <= 15 && serverIP.Length > 6 && //serverIP
                serverPort.Length <= 5 && serverPort.All(char.IsDigit) && //serverPort
                newAsset.assetNumber.Length <= serverParam.Parameters.AssetNumberDigitLimit && newAsset.assetNumber.Length >= 0 && newAsset.assetNumber.All(char.IsDigit) && //assetNumber
                (serverParam.Parameters.Buildings.Contains(newAsset.location.locBuilding) || newAsset.location.locBuilding.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //building
                ((newAsset.location.locRoomNumber.Length <= serverParam.Parameters.RoomNumberDigitLimit && newAsset.location.locRoomNumber.Length > 0 && newAsset.location.locRoomNumber.All(char.IsDigit)) || newAsset.location.locRoomNumber.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //roomNumber
                ((newAsset.maintenances[0].mainServiceDate.Length == 10 && DateTime.TryParseExact(newAsset.maintenances[0].mainServiceDate, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out DateTime datetime)) || newAsset.maintenances[0].mainServiceDate.Equals(GenericResources.TODAY)) && //serviceDate
                StringsAndConstants.LIST_SERVICE_TYPE_CLI.Contains(newAsset.maintenances[0].mainServiceType) && //serviceType
                StringsAndConstants.LIST_BATTERY_CLI.Contains(newAsset.maintenances[0].mainBatteryChange) && //batteryChange
                newAsset.maintenances[0].mainTicketNumber.Length <= serverParam.Parameters.TicketNumberDigitLimit && newAsset.maintenances[0].mainTicketNumber.All(char.IsDigit) && //ticketNumber
                (StringsAndConstants.LIST_STANDARD_CLI.Contains(newAsset.standard) || newAsset.standard.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //standard
                (StringsAndConstants.LIST_IN_USE_CLI.Contains(newAsset.inUse) || newAsset.inUse.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //inUse
                ((newAsset.sealNumber.Length <= serverParam.Parameters.SealNumberDigitLimit && newAsset.sealNumber.All(char.IsDigit)) || newAsset.sealNumber.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //sealNumber
                (StringsAndConstants.LIST_TAG_CLI.Contains(newAsset.tag) || newAsset.tag.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED)) && //tag
                (serverParam.Parameters.HardwareTypes.Contains(newAsset.hardware.hwType) || newAsset.hardware.hwType.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))) //hwType
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PINGGING_SERVER, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                System.Threading.Tasks.Task<bool> c = ModelHandler.CheckHost(client, GenericResources.HTTP + serverIP + ":" + serverPort);
                c.Wait();
                serverOnline = c.Result;

                if (serverOnline && serverPort != string.Empty)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SERVER_DATA, serverIP + ":" + serverPort, true);
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_ONLINE_SERVER, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));

                    CollectThread();

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_ASSET_DATA, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));

                    //Feches asset number data from server
                    try
                    {
                        System.Threading.Tasks.Task<Asset> v = AssetHandler.GetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_ASSET_NUMBER_URL + newAsset.assetNumber);
                        v.Wait();
                        existingAsset = v.Result;
                    }
                    //If asset does not exist on the database
                    catch (AggregateException)
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), LogStrings.LOG_ASSET_NOT_EXIST, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
                    }
                    //If server is unreachable
                    catch (HttpRequestException)
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.DATABASE_REACH_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                    }

                    //If asset Json does not exist and there are some 'same' cmd switch word
                    if (existingAsset == null && serverArgs.Contains(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), LogStrings.LOG_SAMEWORD_NOFIRSTREGISTRY, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                        Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                    }
                    else if (existingAsset == null) //If asset Json does not exist
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), Misc.MiscMethods.SinceLabelUpdate(string.Empty), string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));

                        //serviceType
                        if (newAsset.maintenances[0].mainServiceType.Equals(StringsAndConstants.CLI_SERVICE_TYPE_0))
                            newAsset.maintenances[0].mainServiceType = "0";
                        else if (newAsset.maintenances[0].mainServiceType.Equals(StringsAndConstants.CLI_SERVICE_TYPE_1))
                            newAsset.maintenances[0].mainServiceType = "1";
                        //building
                        newAsset.location.locBuilding = Array.IndexOf(serverParam.Parameters.Buildings.ToArray(), newAsset.location.locBuilding).ToString();
                        //standard
                        if (newAsset.standard.Equals(StringsAndConstants.CLI_EMPLOYEE_TYPE_0))
                            newAsset.standard = Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.standard.Equals(StringsAndConstants.CLI_EMPLOYEE_TYPE_1))
                            newAsset.standard = Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString();
                        //batteryChange
                        if (newAsset.maintenances[0].mainBatteryChange.Equals(StringsAndConstants.LIST_NO_ABBREV))
                            newAsset.maintenances[0].mainBatteryChange = Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.maintenances[0].mainBatteryChange.Equals(StringsAndConstants.LIST_YES_ABBREV))
                            newAsset.maintenances[0].mainBatteryChange = Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString();
                        //inUse
                        if (newAsset.inUse.Equals(StringsAndConstants.LIST_NO_ABBREV))
                            newAsset.inUse = Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.inUse.Equals(StringsAndConstants.LIST_YES_ABBREV))
                            newAsset.inUse = Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString();
                        //tag
                        if (newAsset.tag.Equals(StringsAndConstants.LIST_NO_ABBREV))
                            newAsset.tag = Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.tag.Equals(StringsAndConstants.LIST_YES_ABBREV))
                            newAsset.tag = Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString();
                        //hwType
                        newAsset.hardware.hwType = Array.IndexOf(serverParam.Parameters.HardwareTypes.ToArray(), newAsset.hardware.hwType).ToString();
                    }
                    else //If asset Json does exist
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Misc.MiscMethods.SinceLabelUpdate(existingAsset.maintenances[0].mainServiceDate), string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));

                        //If asset is discarded
                        if (existingAsset.discarded == "1")
                        {
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), UIStrings.ASSET_DROPPED, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                            Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                        }
                        //serviceType
                        if (newAsset.maintenances[0].mainServiceType.Equals(StringsAndConstants.CLI_SERVICE_TYPE_0))
                            newAsset.maintenances[0].mainServiceType = "0";
                        else if (newAsset.maintenances[0].mainServiceType.Equals(StringsAndConstants.CLI_SERVICE_TYPE_1))
                            newAsset.maintenances[0].mainServiceType = "1";
                        //sealNumber
                        if (newAsset.sealNumber.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.sealNumber = existingAsset.sealNumber;
                        //roomNumber
                        if (newAsset.location.locRoomNumber.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.location.locRoomNumber = existingAsset.location.locRoomNumber;
                        //building
                        if (newAsset.location.locBuilding.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.location.locBuilding = existingAsset.location.locBuilding;
                        else
                            newAsset.location.locBuilding = Array.IndexOf(serverParam.Parameters.Buildings.ToArray(), newAsset.location.locBuilding).ToString();
                        //standard
                        if (newAsset.standard.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.standard = existingAsset.standard;
                        else if (newAsset.standard.Equals(StringsAndConstants.CLI_EMPLOYEE_TYPE_0))
                            newAsset.standard = Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.standard.Equals(StringsAndConstants.CLI_EMPLOYEE_TYPE_1))
                            newAsset.standard = Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString();
                        //batteryChange
                        if (newAsset.maintenances[0].mainBatteryChange.Equals(StringsAndConstants.LIST_NO_ABBREV))
                            newAsset.maintenances[0].mainBatteryChange = Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.maintenances[0].mainBatteryChange.Equals(StringsAndConstants.LIST_YES_ABBREV))
                            newAsset.maintenances[0].mainBatteryChange = Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString();
                        //inUse
                        if (newAsset.inUse.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.inUse = existingAsset.inUse;
                        else if (newAsset.inUse.Equals(StringsAndConstants.LIST_NO_ABBREV))
                            newAsset.inUse = Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.inUse.Equals(StringsAndConstants.LIST_YES_ABBREV))
                            newAsset.inUse = Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString();
                        //tag
                        if (newAsset.tag.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.tag = existingAsset.tag;
                        else if (newAsset.tag.Equals(StringsAndConstants.LIST_NO_ABBREV))
                            newAsset.tag = Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                        else if (newAsset.tag.Equals(StringsAndConstants.LIST_YES_ABBREV))
                            newAsset.tag = Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString();
                        //hwType
                        if (newAsset.hardware.hwType.Equals(StringsAndConstants.CLI_DEFAULT_UNCHANGED))
                            newAsset.hardware.hwType = existingAsset.hardware.hwType;
                        else
                            newAsset.hardware.hwType = Array.IndexOf(serverParam.Parameters.HardwareTypes.ToArray(), newAsset.hardware.hwType).ToString();
                    }

                    ProcessCollectedData();

                    if (newAsset.assetHash == null)
                        newAsset.assetHash = Misc.MiscMethods.HardwareSha256UniqueId(newAsset);
                    if (newAsset.hwHash == null)
                        newAsset.hwHash = Misc.MiscMethods.HardwareSha256Hash(newAsset);

                    //If there are no pendencies
                    if (pass)
                    {
                        DateTime d = new DateTime();
                        DateTime todayDate = DateTime.Today;
                        bool tDay;

                        try
                        {
                            _ = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain();
                            newAsset.adRegistered = Convert.ToInt32(HardwareInfo.SpecBinaryStates.ENABLED).ToString();
                        }
                        catch
                        {
                            newAsset.adRegistered = Convert.ToInt32(HardwareInfo.SpecBinaryStates.DISABLED).ToString();
                        }

                        try //If there is database record of the asset number
                        {
                            //If chosen date is 'today'
                            if (newAsset.maintenances[0].mainServiceDate.Equals(GenericResources.TODAY))
                            {
                                newAsset.maintenances[0].mainServiceDate = DateTime.Today.ToString(GenericResources.DATE_FORMAT).Substring(0, 10);
                                tDay = true;
                            }
                            else //If chosen date is not 'today'
                            {
                                d = DateTime.Parse(newAsset.maintenances[0].mainServiceDate);
                                newAsset.maintenances[0].mainServiceDate = d.ToString(GenericResources.DATE_FORMAT);
                                tDay = false;
                            }

                            //Calculates last registered date with chosen date
                            DateTime registerDate = DateTime.ParseExact(newAsset.maintenances[0].mainServiceDate, GenericResources.DATE_FORMAT, CultureInfo.InvariantCulture);
                            DateTime lastRegisterDate = DateTime.ParseExact(existingAsset.maintenances[0].mainServiceDate, GenericResources.DATE_FORMAT, CultureInfo.InvariantCulture);

                            //If chosen date is later than the registered date
                            if (registerDate >= lastRegisterDate)
                            {
                                if (tDay) //If today
                                {
                                    newAsset.maintenances[0].mainServiceDate = todayDate.ToString(GenericResources.DATE_FORMAT).Substring(0, 10);
                                }
                                else if (registerDate <= todayDate) //If today is greater or equal than registered date
                                {
                                    newAsset.maintenances[0].mainServiceDate = DateTime.Parse(newAsset.maintenances[0].mainServiceDate).ToString(GenericResources.DATE_FORMAT).Substring(0, 10);
                                }
                                else //Forbids future registering
                                {
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.INCORRECT_FUTURE_REGISTER_DATE, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                                    Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                                }

                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                                System.Threading.Tasks.Task<HttpStatusCode> v = AssetHandler.SetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_ASSET_NUMBER_URL, newAsset); //Send info to server
                                v.Wait();
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                            }
                            else //If chosen date is earlier than the registered date, show an error
                            {
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.INCORRECT_REGISTER_DATE, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR)); //Exits
                            }
                        }
                        catch //If there is no database record of the asset number
                        {
                            if (newAsset.maintenances[0].mainServiceDate.Equals(GenericResources.TODAY))
                            {
                                newAsset.maintenances[0].mainServiceDate = todayDate.ToString(GenericResources.DATE_FORMAT).Substring(0, 10);
                            }
                            else
                            {
                                d = DateTime.Parse(newAsset.maintenances[0].mainServiceDate);
                                newAsset.maintenances[0].mainServiceDate = d.ToString(GenericResources.DATE_FORMAT).Substring(0, 10);
                            }

                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_INIT_REGISTRY, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                            System.Threading.Tasks.Task<HttpStatusCode> v = AssetHandler.SetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_ASSET_NUMBER_URL, newAsset); //Send info to server
                            v.Wait();
                            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_REGISTRY_FINISHED, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                        }
                    }
                    else //If there are pendencies
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), AirUIStrings.FIX_PROBLEMS, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                        foreach (PropertyInfo pi in sA.GetType().GetProperties())
                        {
                            if (pi.PropertyType == typeof(string))
                            {
                                string value = (string)pi.GetValue(sA);
                                if (value.Substring(value.IndexOf(":")).Length > 2)
                                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), value, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                            }
                        }
                        Environment.Exit(Convert.ToInt32(ExitCodes.WARNING)); //Exits
                    }
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), LogStrings.LOG_OFFLINE_SERVER, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                    Environment.Exit(Convert.ToInt32(ExitCodes.ERROR)); //Exits
                }
            }
            else
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirUIStrings.ARGS_ERROR, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR)); //Exits
            }
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_CLI, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), GenericResources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));

            Environment.Exit(Convert.ToInt32(ExitCodes.SUCCESS)); //Exits
        }

        /// <summary> 
        /// Calls methods from the HardwareInfo class, and setting the variables
        /// </summary>
        private void CollectThread()
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_START_COLLECTING, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));

            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for PC maker 
            newAsset.hardware.hwBrand = HardwareInfo.GetBrand();
            if (newAsset.hardware.hwBrand == GenericResources.TO_BE_FILLED_BY_OEM || newAsset.hardware.hwBrand == string.Empty)
                newAsset.hardware.hwBrand = HardwareInfo.GetBrandAlt();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_BRAND, newAsset.hardware.hwBrand, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for PC model
            newAsset.hardware.hwModel = HardwareInfo.GetModel();
            if (newAsset.hardware.hwModel == GenericResources.TO_BE_FILLED_BY_OEM || newAsset.hardware.hwModel == string.Empty)
            {
                newAsset.hardware.hwModel = HardwareInfo.GetModelAlt();
                if (newAsset.hardware.hwModel == GenericResources.TO_BE_FILLED_BY_OEM || newAsset.hardware.hwModel == string.Empty)
                    newAsset.hardware.hwModel = UIStrings.UNKNOWN;
            }
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_MODEL, newAsset.hardware.hwModel, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for motherboard Serial number
            newAsset.hardware.hwSerialNumber = HardwareInfo.GetSerialNumber();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_SERIAL_NUMBER, newAsset.hardware.hwSerialNumber, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
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
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_NAME + " [" + newHardware.processor[i].procId + "]", newHardware.processor[i].procName, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_FREQUENCY + " [" + newHardware.processor[i].procId + "]", newHardware.processor[i].procFrequency + " " + GenericResources.FREQUENCY_MHZ, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_CORES + " [" + newHardware.processor[i].procId + "]", newHardware.processor[i].procNumberOfCores, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_THREADS + " [" + newHardware.processor[i].procId + "]", newHardware.processor[i].procNumberOfThreads, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_CACHE + " [" + newHardware.processor[i].procId + "]", Misc.MiscMethods.FriendlySizeBinary(Convert.ToInt64(newHardware.processor[i].procCache), false), Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            }
            processorSummary = processorDetail[0][1];
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_PROCESSOR_NAME, processorSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
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
                    ramSerialNumber = ramDetail[i][4],
                    ramPartNumber = ramDetail[i][5],
                    ramManufacturer = ramDetail[i][6]
                };
                newHardware.ram.Add(r);
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_TYPE + " [" + newHardware.ram[i].ramSlot + "]", Enum.GetName(typeof(HardwareInfo.RamTypes), Convert.ToInt32(newHardware.ram[i].ramType)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_AMOUNT + " [" + newHardware.ram[i].ramSlot + "]", Misc.MiscMethods.FriendlySizeBinary(Convert.ToInt64(newHardware.ram[i].ramAmount), false), Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_FREQUENCY + " [" + newHardware.ram[i].ramSlot + "]", newHardware.ram[i].ramFrequency + " " + GenericResources.FREQUENCY_MHZ, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_MANUFACTURER + " [" + newHardware.ram[i].ramSlot + "]", newHardware.ram[i].ramManufacturer, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_SERIAL_NUMBER + " [" + newHardware.ram[i].ramSlot + "]", newHardware.ram[i].ramSerialNumber, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM_PART_NUMBER + " [" + newHardware.ram[i].ramSlot + "]", newHardware.ram[i].ramPartNumber, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            }
            ramSummary = HardwareInfo.GetRamSummary();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RAM, ramSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
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
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_TYPE + " [" + newHardware.storage[i].storId + "]", Enum.GetName(typeof(HardwareInfo.StorageTypes), Convert.ToInt32(newHardware.storage[i].storType)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_SIZE + " [" + newHardware.storage[i].storId + "]", Misc.MiscMethods.FriendlySizeDecimal(Convert.ToInt64(newHardware.storage[i].storSize), false), Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_CONNECTION + " [" + newHardware.storage[i].storId + "]", Enum.GetName(typeof(HardwareInfo.StorageConnectionTypes), Convert.ToInt32(newHardware.storage[i].storConnection)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_MODEL + " [" + newHardware.storage[i].storId + "]", newHardware.storage[i].storModel, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_SERIAL_NUMBER + " [" + newHardware.storage[i].storId + "]", newHardware.storage[i].storSerialNumber, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE_SMART_STATUS + " [" + newHardware.storage[i].storId + "]", Enum.GetName(typeof(HardwareInfo.SmartStates), Convert.ToInt32(newHardware.storage[i].storSmartStatus)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            }
            storageSummary = HardwareInfo.GetStorageSummary();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_STORAGE, storageSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Media Operation (IDE/AHCI/NVME)
            newAsset.firmware.fwMediaOperationMode = HardwareInfo.GetMediaOperationMode();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_MEDIA_OPERATION_TYPE, Enum.GetName(typeof(HardwareInfo.MediaOperationTypes), Convert.ToInt32(newAsset.firmware.fwMediaOperationMode)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for GPU information
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
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_VIDEO_CARD_NAME + " [" + newHardware.videoCard[i].vcId + "]", newHardware.videoCard[i].vcName, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_VIDEO_CARD_RAM + " [" + newHardware.videoCard[i].vcId + "]", Misc.MiscMethods.FriendlySizeBinary(Convert.ToInt64(newHardware.videoCard[i].vcRam), false), Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            }
            videoCardSummary = videoCardDetail[0][1];
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_VIDEO_CARD, videoCardSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for OS infomation
            newAsset.operatingSystem.osArch = HardwareInfo.GetOSArchBinary();
            newAsset.operatingSystem.osBuild = HardwareInfo.GetOSBuildAndRevision();
            newAsset.operatingSystem.osName = HardwareInfo.GetOSName();
            newAsset.operatingSystem.osVersion = HardwareInfo.GetOSVersion();
            operatingSystemSummary = HardwareInfo.GetOSSummary();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_OPERATING_SYSTEM, operatingSystemSummary, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for hostname
            newAsset.network.netHostname = HardwareInfo.GetHostname();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_NETWORK_HOSTNAME, newAsset.network.netHostname, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for MAC Address
            newAsset.network.netMacAddress = HardwareInfo.GetMacAddress();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_NETWORK_MAC_ADDRESS, newAsset.network.netMacAddress, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for IP Address
            newAsset.network.netIpAddress = HardwareInfo.GetIpAddress();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_NETWORK_IP_ADDRESS, newAsset.network.netIpAddress, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for firmware type
            newAsset.firmware.fwType = HardwareInfo.GetFirmwareType();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_TYPE, Enum.GetName(typeof(HardwareInfo.FirmwareTypes), Convert.ToInt32(newAsset.firmware.fwType)), Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for Secure Boot status
            newAsset.firmware.fwSecureBoot = HardwareInfo.GetSecureBoot();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_SECURE_BOOT, StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.fwSecureBoot)], Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for firmware version
            newAsset.firmware.fwVersion = HardwareInfo.GetFirmwareVersion();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_VERSION, newAsset.firmware.fwVersion, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for VT status
            newAsset.firmware.fwVirtualizationTechnology = HardwareInfo.GetVirtualizationTechnology();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_VIRTUALIZATION_TECHNOLOGY, StringsAndConstants.LIST_STATES[Convert.ToInt32(newAsset.firmware.fwVirtualizationTechnology)], Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            //Scans for TPM status
            newAsset.firmware.fwTpmVersion = HardwareInfo.GetTPMStatus();
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FIRMWARE_TPM, StringsAndConstants.LIST_TPM_TYPES[Convert.ToInt32(newAsset.firmware.fwTpmVersion)], Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            /*-------------------------------------------------------------------------------------------------------------------------------------------*/
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_END_COLLECTING, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
        }

        /// <summary> 
        /// Prints the collected data into the console, warning the agent when there are forbidden modes
        /// </summary>
        private void ProcessCollectedData()
        {
            pass = true;

            //If hardware signature changed, alerts the agent
            newAsset.hwHash = Misc.MiscMethods.HardwareSha256Hash(newAsset);
            if (existingAsset != null && existingAsset.hwHash != string.Empty && existingAsset.hwHash != newAsset.hwHash)
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), LogStrings.LOG_ASSET_HARDWARE_MODIFIED, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));

            try
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_FETCHING_MODEL_DATA, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Feches model info from server
                System.Threading.Tasks.Task<Model> v = ModelHandler.GetModelAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_MODEL_URL + newAsset.hardware.hwModel);
                v.Wait();
                modelTemplate = v.Result;
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If hostname is the default one and its enforcement is enabled
                if (configOptions.Enforcement.Hostname.ToString() == GenericResources.TRUE && !hostnamePattern.IsMatch(newAsset.network.netHostname))
                {
                    pass = false;
                    sA.NetworkHostnameAlert += newAsset.network.netHostname + AirUIStrings.ALERT_HOSTNAME;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, mediaOpMode enforcement is enabled, and the mode is incorrect
                if (configOptions.Enforcement.MediaOperationMode.ToString() == GenericResources.TRUE && modelTemplate != null && modelTemplate.mediaOperationMode != newAsset.firmware.fwMediaOperationMode)
                {
                    pass = false;
                    sA.FirmwareMediaOperationModeAlert += Enum.GetName(typeof(HardwareInfo.MediaOperationTypes), Convert.ToInt32(newAsset.firmware.fwMediaOperationMode)) + AirUIStrings.ALERT_MEDIA_OPERATION_TO;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //The section below contains the exception cases for Secure Boot enforcement, if it is enabled
                if (configOptions.Enforcement.SecureBoot.ToString() == GenericResources.TRUE && newAsset.firmware.fwSecureBoot.Equals(UIStrings.DEACTIVATED))
                {
                    pass = false;
                    sA.FirmwareSecureBootAlert += newAsset.firmware.fwSecureBoot + AirUIStrings.ALERT_SECURE_BOOT;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does not exist and server is unreachable
                if (modelTemplate == null)
                {
                    pass = false;
                    sA.DatabaseReachError += UIStrings.DATABASE_REACH_ERROR;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, firmware version enforcement is enabled, and the version is incorrect
                if (configOptions.Enforcement.FirmwareVersion.ToString() == GenericResources.TRUE && modelTemplate != null && !newAsset.firmware.fwVersion.Contains(modelTemplate.fwVersion))
                {
                    pass = false;
                    sA.FirmwareVersionAlert += newAsset.firmware.fwVersion + AirUIStrings.ALERT_FIRMWARE_VERSION_TO;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, firmware type enforcement is enabled, and the type is incorrect
                if (configOptions.Enforcement.FirmwareType.ToString() == GenericResources.TRUE && modelTemplate != null && modelTemplate.fwType != newAsset.firmware.fwType)
                {
                    pass = false;
                    sA.FirmwareTypeAlert += Enum.GetName(typeof(HardwareInfo.FirmwareTypes), Convert.ToInt32(newAsset.firmware.fwType)) + AirUIStrings.ALERT_FIRMWARE_TYPE_TO;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If there is no MAC address assigned
                if (newAsset.network.netMacAddress == string.Empty)
                {
                    pass = false;
                    sA.NetworkMacError += newAsset.network.netMacAddress + AirUIStrings.ALERT_NETWORK; //Prints a network error
                    sA.NetworkIpError += newAsset.network.netIpAddress + AirUIStrings.ALERT_NETWORK; //Prints a network error
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If Virtualization Technology is disabled for UEFI and its enforcement is enabled
                if (configOptions.Enforcement.VirtualizationTechnology.ToString() == GenericResources.TRUE && newAsset.firmware.fwVirtualizationTechnology == UIStrings.DEACTIVATED)
                {
                    pass = false;
                    sA.FirmwareVirtualizationTechnologyAlert += newAsset.firmware.fwVirtualizationTechnology + AirUIStrings.ALERT_VT;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If Smart status is not OK and its enforcement is enabled
                if (configOptions.Enforcement.SmartStatus.ToString() == GenericResources.TRUE && smartValueList.Contains(GenericResources.PRED_FAIL_CODE))
                {
                    pass = false;
                    sA.StorageSmartStatusAlert += AirUIStrings.SMART_FAIL + newAsset.hardware.storage[smartValueList.IndexOf(GenericResources.PRED_FAIL_CODE)].storSmartStatus;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //If model Json file does exist, TPM enforcement is enabled, and TPM version is incorrect
                if (configOptions.Enforcement.Tpm.ToString() == GenericResources.TRUE && modelTemplate != null && modelTemplate.tpmVersion != newAsset.firmware.fwTpmVersion)
                {
                    pass = false;
                    sA.FirmwareTpmAlert += StringsAndConstants.LIST_TPM_TYPES[Convert.ToInt32(newAsset.firmware.fwTpmVersion)] + AirUIStrings.ALERT_TPM_TO;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                //Checks for RAM amount
                double d = Convert.ToDouble(HardwareInfo.GetRamAlt(), CultureInfo.CurrentCulture.NumberFormat);
                //If RAM is less than 4GB and OS is x64, and its limit enforcement is enabled, shows an alert
                if (configOptions.Enforcement.RamLimit.ToString() == GenericResources.TRUE && d < 4.0 && Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    sA.RamAlert += newAsset.hardware.ram + AirUIStrings.ALERT_NOT_ENOUGH_MEMORY;
                }
                //If RAM is more than 4GB and OS is x86, and its limit enforcement is enabled, shows an alert
                if (configOptions.Enforcement.RamLimit.ToString() == GenericResources.TRUE && d > 4.0 && !Environment.Is64BitOperatingSystem)
                {
                    pass = false;
                    sA.RamAlert += newAsset.hardware.ram + AirUIStrings.ALERT_TOO_MUCH_MEMORY;
                }
                /*-------------------------------------------------------------------------------------------------------------------------------------------*/
                if (pass)
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_HARDWARE_PASSED, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            }
            catch (Exception e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), e.Message, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
            }
        }
    }
}
