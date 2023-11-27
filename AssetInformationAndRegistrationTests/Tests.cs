using AssetInformationAndRegistration;
using AssetInformationAndRegistration.Updater;
using ConstantsDLL;
using ConstantsDLL.Properties;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Newtonsoft.Json;
using Octokit;
using RestApiDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
                    Building = vBefore.location.building,
                    RoomNumber = vBefore.location.roomNumber,
                    ServiceDate = vBefore.maintenances[0].serviceDate,
                    ServiceType = vBefore.maintenances[0].serviceType,
                    BatteryChange = vBefore.maintenances[0].batteryChange,
                    TicketNumber = vBefore.maintenances[0].ticketNumber,
                    Standard = vBefore.standard,
                    InUse = vBefore.inUse,
                    SealNumber = vBefore.sealNumber,
                    Tag = vBefore.tag,
                    HardwareType = vBefore.hardware.type
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
                Assert.Equal(argsObj.Building, sp.Parameters.Buildings[Convert.ToInt32(vAfter.location.building)]);
            else
                Assert.Equal(sp.Parameters.Buildings[Convert.ToInt32(existingAssetObj.Building)], sp.Parameters.Buildings[Convert.ToInt32(vAfter.location.building)]);

            if (!argsObj.RoomNumber.Contains("same"))
                Assert.Equal(argsObj.RoomNumber, vAfter.location.roomNumber);
            else
                Assert.Equal(existingAssetObj.RoomNumber, vAfter.location.roomNumber);

            if (!argsObj.ServiceDate.Contains("today"))
                Assert.Equal(argsObj.ServiceDate, vAfter.maintenances[0].serviceDate);

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
                Assert.Equal(argsObj.HardwareType, sp.Parameters.HardwareTypes[Convert.ToInt32(vAfter.hardware.type)]);
            else
                Assert.Equal(sp.Parameters.HardwareTypes[Convert.ToInt32(existingAssetObj.HardwareType)], sp.Parameters.HardwareTypes[Convert.ToInt32(vAfter.hardware.type)]);

            Assert.Equal(argsObj.ServiceType, StringsAndConstants.LIST_SERVICE_TYPE_CLI[Convert.ToInt32(vAfter.maintenances[0].serviceType)]);
            Assert.Equal(argsObj.AssetNumber, vAfter.assetNumber);
            Assert.Equal(argsObj.BatteryChange, StringsAndConstants.LIST_BATTERY_CLI[Convert.ToInt32(vAfter.maintenances[0].batteryChange)]);
            Assert.Equal(argsObj.TicketNumber, vAfter.maintenances[0].ticketNumber);

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
                    mediaOperationMode = "1",
                    secureBoot = "2",
                    tpmVersion = "1",
                    type = "1",
                    version = "1.1test",
                    virtualizationTechnology = "1"
                },
                hardware = new hardware()
                {
                    brand = "TestBrand",
                    model = "TestModel",
                    type = "3",
                    serialNumber = "888888",
                    processor = new List<processor>()
                    {
                        new processor()
                        {
                            processorId = "0",
                            name = "Intel Pentium 4 HT 3.06GHz",
                            frequency = "3060",
                            numberOfCores = "1",
                            numberOfThreads = "2",
                            cache = "1048576"
                        },
                        new processor()
                        {
                            processorId = "1",
                            name = "Intel Core 2 Duo 2.26GHz",
                            frequency = "2260",
                            numberOfCores = "2",
                            numberOfThreads = "2",
                            cache = "2097152"
                        }
                    },
                    ram = new List<ram>()
                    {
                        new ram()
                        {
                            slot = "0",
                            type = "24",
                            frequency = "800",
                            amount = "1073741824",
                            manufacturer = "Samsung",
                            serialNumber = "123987",
                            partNumber = "ABCXYZ"
                        },
                        new ram()
                        {
                            slot = "1",
                            type = "22",
                            frequency = "667",
                            amount = "2147483648",
                            manufacturer = "Hynix",
                            serialNumber = "789321",
                            partNumber = "CBAZYX"
                        },
                        new ram()
                        {
                            slot = "-2",
                            type = "-2",
                            frequency = "-2",
                            amount = "-2",
                            manufacturer = "-2",
                            serialNumber = "-2",
                            partNumber = "-2"
                        },
                        new ram()
                        {
                            slot = "-2",
                            type = "-2",
                            frequency = "-2",
                            amount = "-2",
                            manufacturer = "-2",
                            serialNumber = "-2",
                            partNumber = "-2"
                        }
                    },
                    storage = new List<storage>()
                    {
                        new storage()
                        {
                            storageId = "0",
                            type = "0",
                            connection = "1",
                            model = "Intel 520 HDD",
                            size = "250000000000",
                            serialNumber = "IntelSN123",
                            smartStatus = "0"
                        },
                        new storage()
                        {
                            storageId = "1",
                            type = "1",
                            connection = "2",
                            model = "Samsung 990 Pro NVMe SSD",
                            size = "1000000000000",
                            serialNumber = "SamsungSN987",
                            smartStatus = "1"
                        }
                    },
                    videoCard = new List<videoCard>()
                    {
                        new videoCard()
                        {
                            videoCardId = "0",
                            name = "NVIDIA Geforce RTX 4090 Ti",
                            vRam = "34359738368"
                        },
                        new videoCard()
                        {
                            videoCardId = "1",
                            name = "NVIDIA Geforce 5200",
                            vRam = "268435456"
                        }
                    },
                },
                location = new location()
                {
                    building = "0",
                    roomNumber = "8888"
                },
                network = new network()
                {
                    hostname = "PC-002600",
                    ipAddress = "169.254.140.20",
                    macAddress = "AA:BB:CC:DD:EE:FF"
                },
                operatingSystem = new operatingSystem()
                {
                    arch = "0",
                    build = "5.1.2600",
                    name = "Windows XP Professional",
                    version = "Service Pack 3"
                },
                maintenances = new List<maintenances>()
                {
                    new maintenances()
                    {
                        agentId = "3",
                        batteryChange = "0",
                        ticketNumber = "888888",
                        serviceType = "0",
                        serviceDate = "2015-07-29"
                    },
                    new maintenances()
                    {
                        agentId = "62",
                        batteryChange = "1",
                        ticketNumber = "888889",
                        serviceType = "1",
                        serviceDate = "2023-11-21"
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