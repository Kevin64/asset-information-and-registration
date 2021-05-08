using Newtonsoft.Json;
using System;
using System.Net;
using System.Windows.Forms;

namespace HardwareInformation
{
    public static class BIOSFileReader
    {
        private static string jsonFile, str;
        private static WebBrowser wb;

        //Reads a json file retrieved from the server and parses brand, model and BIOS versions, returning the latter
        [STAThread]
        public static string readLine(string brd, string mod)
        {
            wb = new WebBrowser();
            wb.Navigate("http://192.168.76.103:8081/forneceDadosBIOS.php");
            
            jsonFile = new WebClient().DownloadString("http://192.168.76.103:8081/bios.json");

            dynamic jsonParse = JsonConvert.DeserializeObject(jsonFile);
            var brand = jsonParse[0].marca;
            var model = jsonParse[0].modelo;
            var version = jsonParse[0].versao;

            foreach (var obj in jsonParse)
            {
                foreach(var obj2 in obj)
                {
                    foreach(var obj3 in obj2)
                    {
                        str = Convert.ToString(obj3);
                        if (mod.Contains(str))
                            return obj["versao"];
                    }   
                }
            }
            return null;
        }
    }
}
