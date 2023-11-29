using AssetInformationAndRegistration;
using AssetInformationAndRegistration.Updater;
using ConstantsDLL;
using ConstantsDLL.Properties;
using LogGeneratorDLL;
using Newtonsoft.Json;
using Octokit;
using RestApiDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Xunit;
using static AssetInformationAndRegistration.Program;

[assembly: CollectionBehavior(DisableTestParallelization = true)]


namespace AssetInformationAndRegistrationTests
{

    public class Arguments
    {
        public string ServerIP { get; set; }
        public string ServerPort { get; set; }
        public string AssetNumber { get; set; }
        public string Building { get; set; }
        public string RoomNumber { get; set; }
        public string ServiceDate { get; set; }
        public string ServiceType { get; set; }
        public string BatteryChange { get; set; }
        public string TicketNumber { get; set; }
        public string Standard { get; set; }
        public string InUse { get; set; }
        public string SealNumber { get; set; }
        public string Tag { get; set; }
        public string HardwareType { get; set; }
    }

    public class Tests
    {
        private HttpClient client;
        private Arguments argsObj, existingAssetObj;
        private Asset vBefore, vAfter;
        private ServerParam sp;

        [Theory]
        [InlineData("localhost", "8081", "999999", "Building0", "9999", "2023-10-30", "f", "y", "999999", "e", "y", "99999999", "y", "Tablet", "kevin1", "123")]
        [InlineData("localhost", "8081", "999999", "Building0", "9999", "2023-10-30", "f", "y", "999999", "e", "y", "99999999", "y", "Tablet", "kevin", "123")]
        [InlineData("localhost", "8081", "888888", "Building1", "8888", "2022-09-29", "m", "n", "888888", "s", "n", "88888888", "n", "Notebook/Laptop", "kevin", "123")]
        [InlineData("localhost", "8081", "777777", "Building0", "7777", "2021-08-28", "f", "n", "777777", "e", "n", "77777777", "y", "All-In-One", "kevin", "123")]
        [InlineData("localhost", "8081", "666666", "Building2", "6666", "2023-11-06", "m", "y", "666666", "s", "y", "66666666", "n", "Desktop", "kevin", "1234")]
        [InlineData("192.168.79.54", "8081", "888888", "same", "same", "today", "f", "y", "555555", "e", "y", "same", "y", "same", "kevin", "123")]
        public async void GivingAssetArgs_RegisterOnDb_ThenRetrieveDataToCompare(string serverIP, string serverPort, string assetNumber, string building, string roomNumber, string serviceDate, string serviceType, string batteryChange, string ticketNumber, string standard, string inUse, string sealNumber, string tag, string hwType, string username, string password)
        {
            try
            {
                client = MiscMethods.SetHttpClient(serverIP, serverPort, GenericResources.HTTP_CONTENT_TYPE_JSON, username, password);

                sp = await ParameterHandler.GetParameterAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_PARAMETERS_URL);

                vBefore = await AssetHandler.GetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_ASSET_NUMBER_URL + assetNumber);

                existingAssetObj = new Arguments
                {
                    AssetNumber = vBefore.assetNumber,
                    Building = vBefore.location.locBuilding,
                    RoomNumber = vBefore.location.locRoomNumber,
                    ServiceDate = vBefore.maintenances[0].mainServiceDate,
                    ServiceType = vBefore.maintenances[0].mainServiceType,
                    BatteryChange = vBefore.maintenances[0].mainBatteryChange,
                    TicketNumber = vBefore.maintenances[0].mainTicketNumber,
                    Standard = vBefore.standard,
                    InUse = vBefore.inUse,
                    SealNumber = vBefore.sealNumber,
                    Tag = vBefore.tag,
                    HardwareType = vBefore.hardware.hwType
                };
                string[] argsArray = { "--serverIP=" + serverIP, "--serverPort=" + serverPort, "--assetNumber=" + assetNumber, "--building=" + building, "--roomNumber=" + roomNumber, "--serviceDate=" + serviceDate, "--serviceType=" + serviceType, "--batteryChanged=" + batteryChange, "--ticketNumber=" + ticketNumber, "--standard=" + standard, "--inUse=" + inUse, "--sealNumber=" + sealNumber, "--tag=" + tag, "--hwType=" + hwType, "--username=" + username, "--password=" + password };

                argsObj = new Arguments
                {
                    AssetNumber = assetNumber,
                    Building = building,
                    RoomNumber = roomNumber,
                    ServiceDate = serviceDate,
                    ServiceType = serviceType,
                    BatteryChange = batteryChange,
                    TicketNumber = ticketNumber,
                    Standard = standard,
                    InUse = inUse,
                    SealNumber = sealNumber,
                    Tag = tag,
                    HardwareType = hwType
                };

                Program.Bootstrap(argsArray);

                vAfter = await AssetHandler.GetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_ASSET_NUMBER_URL + assetNumber);
            }
            catch (HttpRequestException)
            {

            }

            if (!argsObj.Building.Contains("same"))
                Assert.Equal(argsObj.Building, sp.Parameters.Buildings[Convert.ToInt32(vAfter.location.locBuilding)]);
            else
                Assert.Equal(sp.Parameters.Buildings[Convert.ToInt32(existingAssetObj.Building)], sp.Parameters.Buildings[Convert.ToInt32(vAfter.location.locBuilding)]);

            if (!argsObj.RoomNumber.Contains("same"))
                Assert.Equal(argsObj.RoomNumber, vAfter.location.locRoomNumber);
            else
                Assert.Equal(existingAssetObj.RoomNumber, vAfter.location.locRoomNumber);

            if (!argsObj.ServiceDate.Contains("today"))
                Assert.Equal(argsObj.ServiceDate, vAfter.maintenances[0].mainServiceDate);

            if (!argsObj.Standard.Contains("same"))
                Assert.Equal(argsObj.Standard, StringsAndConstants.LIST_STANDARD_CLI[Convert.ToInt32(vAfter.standard)]);
            else
                Assert.Equal(StringsAndConstants.LIST_STANDARD_CLI[Convert.ToInt32(existingAssetObj.Standard)], StringsAndConstants.LIST_STANDARD_CLI[Convert.ToInt32(vAfter.standard)]);

            if (!argsObj.InUse.Contains("same"))
                Assert.Equal(argsObj.InUse, StringsAndConstants.LIST_IN_USE_CLI[Convert.ToInt32(vAfter.inUse)]);
            else
                Assert.Equal(StringsAndConstants.LIST_IN_USE_CLI[Convert.ToInt32(existingAssetObj.InUse)], StringsAndConstants.LIST_IN_USE_CLI[Convert.ToInt32(vAfter.inUse)]);

            if (!argsObj.SealNumber.Contains("same"))
                Assert.Equal(argsObj.SealNumber, vAfter.sealNumber);
            else
                Assert.Equal(existingAssetObj.SealNumber, vAfter.sealNumber);

            if (!argsObj.Tag.Contains("same"))
                Assert.Equal(argsObj.Tag, StringsAndConstants.LIST_IN_USE_CLI[Convert.ToInt32(vAfter.tag)]);
            else
                Assert.Equal(StringsAndConstants.LIST_IN_USE_CLI[Convert.ToInt32(existingAssetObj.Tag)], StringsAndConstants.LIST_IN_USE_CLI[Convert.ToInt32(vAfter.tag)]);

            if (!argsObj.HardwareType.Contains("same"))
                Assert.Equal(argsObj.HardwareType, sp.Parameters.HardwareTypes[Convert.ToInt32(vAfter.hardware.hwType)]);
            else
                Assert.Equal(sp.Parameters.HardwareTypes[Convert.ToInt32(existingAssetObj.HardwareType)], sp.Parameters.HardwareTypes[Convert.ToInt32(vAfter.hardware.hwType)]);

            Assert.Equal(argsObj.ServiceType, StringsAndConstants.LIST_SERVICE_TYPE_CLI[Convert.ToInt32(vAfter.maintenances[0].mainServiceType)]);
            Assert.Equal(argsObj.AssetNumber, vAfter.assetNumber);
            Assert.Equal(argsObj.BatteryChange, StringsAndConstants.LIST_BATTERY_CLI[Convert.ToInt32(vAfter.maintenances[0].mainBatteryChange)]);
            Assert.Equal(argsObj.TicketNumber, vAfter.maintenances[0].mainTicketNumber);

        }

        [Theory]
        [InlineData("localhost", "gtr", "999999", "Building0", "9999", "2023-10-30", "f", "y", "999999", "e", "y", "99999999", "y", "Tablet", "kevin", "123")]
        public void ThrowsExceptionForBadUri(string serverIP, string serverPort, string assetNumber, string building, string roomNumber, string serviceDate, string serviceType, string batteryChange, string ticketNumber, string standard, string inUse, string sealNumber, string tag, string hwType, string username, string password)
        {
            string[] argsArray = { "--serverIP=" + serverIP, "--serverPort=" + serverPort, "--assetNumber=" + assetNumber, "--building=" + building, "--roomNumber=" + roomNumber, "--serviceDate=" + serviceDate, "--serviceType=" + serviceType, "--batteryChanged=" + batteryChange, "--ticketNumber=" + ticketNumber, "--standard=" + standard, "--inUse=" + inUse, "--sealNumber=" + sealNumber, "--tag=" + tag, "--hwType=" + hwType, "--username=" + username, "--password=" + password };

            void act()
            {
                Program.Bootstrap(argsArray);
            }
            UriFormatException httpEx = Assert.Throws<UriFormatException>(act);
            Assert.Equal("URI Inválido: Porta especificada inválida.", httpEx.Message);
        }

        /*-------------------------------------------------------------------------------------------------------------------------------------------*/
        // Tests for RestApi.cs functions
        /*-------------------------------------------------------------------------------------------------------------------------------------------*/

        [Theory]
        [InlineData("localhost", "8081", "kevin", "123", "999999", "OptiPlex-7070")]
        public async void GivingServerAndAuthData_RetrieveServerObject_PassIfObjectsAreNotNull(string serverIP, string serverPort, string username, string password, string assetNumber, string mod)
        {
            Asset asset;
            Model model;
            Agent agent;
            ServerParam sp;
            client = MiscMethods.SetHttpClient(serverIP, serverPort, GenericResources.HTTP_CONTENT_TYPE_JSON, username, password);
            asset = await AssetHandler.GetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_ASSET_NUMBER_URL + assetNumber);
            model = await ModelHandler.GetModelAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_MODEL_URL + mod);
            agent = await AuthenticationHandler.GetAgentAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_AGENT_USERNAME_URL + username);
            sp = await ParameterHandler.GetParameterAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_PARAMETERS_URL);
            Assert.NotNull(asset);
            Assert.NotNull(model);
            Assert.NotNull(agent);
            Assert.NotNull(sp);
        }

        [Fact]
        public void RetrieveOfflineParams_PassIfIsNotNull()
        {
            ServerParam sp;
            sp = ParameterHandler.GetOfflineModeConfigFile();
            Assert.NotNull(sp);
        }

        [Fact]
        public async void CheckIfHostIsOnline_PassIfIsTrue()
        {
            string serverIP = "localhost", serverPort = "8081", username = "kevin", password = "123";
            client = MiscMethods.SetHttpClient(serverIP, serverPort, GenericResources.HTTP_CONTENT_TYPE_JSON, username, password);
            bool isOnline = await ModelHandler.CheckHost(client, GenericResources.HTTP + serverIP + ":" + serverPort);
            Assert.True(isOnline);
        }

        [Fact]
        public async void SetAssetAsync_PassIfSuccessStatusCode()
        {
            string serverIP = "localhost", serverPort = "8081", username = "kevin", password = "123";
            client = MiscMethods.SetHttpClient(serverIP, serverPort, GenericResources.HTTP_CONTENT_TYPE_JSON, username, password);
            Asset asset = new Asset()
            {
                assetNumber = "888888",
                adRegistered = "1",
                assetHash = "abcdef1234567890abcdef1234567890abcdef1234567890abcdef1234567890",
                discarded = "0",
                hwHash = "0987654321fedcba0987654321fedcba0987654321fedcba0987654321fedcba",
                inUse = "1",
                sealNumber = "888888",
                standard = "1",
                tag = "1",
                firmware = new firmware()
                {
                    fwMediaOperationMode = "1",
                    fwSecureBoot = "2",
                    fwTpmVersion = "1",
                    fwType = "1",
                    fwVersion = "1.1test",
                    fwVirtualizationTechnology = "1"
                },
                hardware = new hardware()
                {
                    hwBrand = "TestBrand",
                    hwModel = "TestModel",
                    hwType = "3",
                    hwSerialNumber = "888888",
                    processor = new List<processor>()
                    {
                        new processor()
                        {
                            procId = "0",
                            procName = "Intel Pentium 4 HT 3.06GHz",
                            procFrequency = "3060",
                            procNumberOfCores = "1",
                            procNumberOfThreads = "2",
                            procCache = "1048576"
                        },
                        new processor()
                        {
                            procId = "1",
                            procName = "Intel Core 2 Duo 2.26GHz",
                            procFrequency = "2260",
                            procNumberOfCores = "2",
                            procNumberOfThreads = "2",
                            procCache = "2097152"
                        }
                    },
                    ram = new List<ram>()
                    {
                        new ram()
                        {
                            ramSlot = "0",
                            ramType = "24",
                            ramFrequency = "800",
                            ramAmount = "1073741824",
                            ramManufacturer = "Samsung",
                            ramSerialNumber = "123987",
                            ramPartNumber = "ABCXYZ"
                        },
                        new ram()
                        {
                            ramSlot = "1",
                            ramType = "22",
                            ramFrequency = "667",
                            ramAmount = "2147483648",
                            ramManufacturer = "Hynix",
                            ramSerialNumber = "789321",
                            ramPartNumber = "CBAZYX"
                        },
                        new ram()
                        {
                            ramSlot = "-2",
                            ramType = "-2",
                            ramFrequency = "-2",
                            ramAmount = "-2",
                            ramManufacturer = "-2",
                            ramSerialNumber = "-2",
                            ramPartNumber = "-2"
                        },
                        new ram()
                        {
                            ramSlot = "-2",
                            ramType = "-2",
                            ramFrequency = "-2",
                            ramAmount = "-2",
                            ramManufacturer = "-2",
                            ramSerialNumber = "-2",
                            ramPartNumber = "-2"
                        }
                    },
                    storage = new List<storage>()
                    {
                        new storage()
                        {
                            storId = "0",
                            storType = "0",
                            storConnection = "1",
                            storModel = "Intel 520 HDD",
                            storSize = "250000000000",
                            storSerialNumber = "IntelSN123",
                            storSmartStatus = "0"
                        },
                        new storage()
                        {
                            storId = "1",
                            storType = "1",
                            storConnection = "2",
                            storModel = "Samsung 990 Pro NVMe SSD",
                            storSize = "1000000000000",
                            storSerialNumber = "SamsungSN987",
                            storSmartStatus = "1"
                        }
                    },
                    videoCard = new List<videoCard>()
                    {
                        new videoCard()
                        {
                            vcId = "0",
                            vcName = "NVIDIA Geforce RTX 4090 Ti",
                            vcRam = "34359738368"
                        },
                        new videoCard()
                        {
                            vcId = "1",
                            vcName = "NVIDIA Geforce 5200",
                            vcRam = "268435456"
                        }
                    },
                },
                location = new location()
                {
                    locBuilding = "0",
                    locRoomNumber = "8888"
                },
                network = new network()
                {
                    netHostname = "PC-002600",
                    netIpAddress = "169.254.140.20",
                    netMacAddress = "AA:BB:CC:DD:EE:FF"
                },
                operatingSystem = new operatingSystem()
                {
                    osArch = "0",
                    osBuild = "5.1.2600",
                    osName = "Windows XP Professional",
                    osVersion = "Service Pack 3"
                },
                maintenances = new List<maintenances>()
                {
                    new maintenances()
                    {
                        mainAgentId = "3",
                        mainBatteryChange = "0",
                        mainTicketNumber = "888888",
                        mainServiceType = "0",
                        mainServiceDate = "2015-07-29"
                    },
                    new maintenances()
                    {
                        mainAgentId = "62",
                        mainBatteryChange = "1",
                        mainTicketNumber = "888889",
                        mainServiceType = "1",
                        mainServiceDate = "2023-11-21"
                    }
                }
            };
            HttpStatusCode hsc = await AssetHandler.SetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.APCS_V1_API_ASSET_NUMBER_URL, asset);
            try
            {
                Assert.Equal(201, Convert.ToInt32(hsc));
            }
            catch
            {
                Assert.Equal(200, Convert.ToInt32(hsc));
            }
        }

        /*-------------------------------------------------------------------------------------------------------------------------------------------*/
        // Tests for UpdateChecker.cs functions
        /*-------------------------------------------------------------------------------------------------------------------------------------------*/

        [Fact]
        public void CheckForGitHubUpdate_PassIfSuccessful()
        {
            StreamReader fileC = new StreamReader(GenericResources.CONFIG_FILE);
            string jsonFile = fileC.ReadToEnd();
            ConfigurationOptions jsonParse = JsonConvert.DeserializeObject<ConfigurationOptions>(@jsonFile);
            fileC.Close();

            Definitions definitions = new Definitions()
            {
                LogLocation = jsonParse.Definitions.LogLocation,
                ServerIP = jsonParse.Definitions.ServerIP,
                ServerPort = jsonParse.Definitions.ServerPort,
                ThemeUI = jsonParse.Definitions.ThemeUI,
            };

            GitHubClient ghc = new GitHubClient(new Octokit.ProductHeaderValue(GenericResources.GITHUB_REPO_AIR));
            LogGenerator log = new LogGenerator("AIR-Tests", "C:\\AppLog\\", "AIR-Tests" + GenericResources.LOG_FILE_EXT, false);

            AssetInformationAndRegistration.Misc.MiscMethods.RegDeleteUpdateData();

            UpdateChecker.Check(ghc, log, definitions, false, true, false, false);

            UpdateInfo updateInfo = AssetInformationAndRegistration.Misc.MiscMethods.RegCheckUpdateData();

            Assert.NotNull(updateInfo);
        }
    }
}