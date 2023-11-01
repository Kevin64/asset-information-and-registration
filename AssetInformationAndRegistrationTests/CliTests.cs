using AssetInformationAndRegistration;
using ConstantsDLL;
using ConstantsDLL.Properties;
using RestApiDLL;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

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

    public class CliTests
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
        [InlineData("localhost", "8081", "666666", "Building2", "6666", "2020-07-27", "m", "y", "666666", "s", "y", "66666666", "n", "Desktop", "kevin", "123")]
        [InlineData("192.168.79.54", "8081", "888888", "same", "same", "today", "f", "y", "555555", "e", "y", "same", "y", "same", "kevin", "123")]
        
        public async void GivingAssetArgs_RegisterOnDb_ThenRetrieveDataToCompare(string serverIP, string serverPort, string assetNumber, string building, string roomNumber, string serviceDate, string serviceType, string batteryChange, string ticketNumber, string standard, string inUse, string sealNumber, string tag, string hwType, string username, string password)
        {
            try
            {
                client = AssetInformationAndRegistration.Misc.MiscMethods.SetHttpClient(serverIP, serverPort, GenericResources.HTTP_CONTENT_TYPE_JSON);
                
                sp = await ParameterHandler.GetParameterAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.V1_API_PARAMETERS_URL);

                vBefore = await AssetHandler.GetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.V1_API_ASSET_URL + assetNumber);

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

                vAfter = await AssetHandler.GetAssetAsync(client, GenericResources.HTTP + serverIP + ":" + serverPort + GenericResources.V1_API_ASSET_URL + assetNumber);
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

            Action act = () =>
            {
                Program.Bootstrap(argsArray);
            };
            var httpEx = Assert.Throws<UriFormatException>(act);
            Assert.Equal("URI Inválido: Porta especificada inválida.", httpEx.Message);
        }
    }
}