using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace HardwareInformation
{
    public static class BIOSFileReader
    {
        private static string fileName = "bios.json", sha1, fileBakName = "bios.bak.json";
        private static string jsonFile, str;
        private static WebBrowser wb;
        private static WebClient wc;
        private static StreamReader file;

        //Reads a json file retrieved from the server and parses brand, model and BIOS versions, returning the latter
        [STAThread]
        public static string readLine(string brd, string mod, string ip, string port)
        {            
            try
            {
                wb = new WebBrowser();
                wc = new WebClient();
                wb.Navigate("http://" + ip + ":" + port + "/forneceDadosBIOS.php");
                System.Threading.Thread.Sleep(1000);
                wc.DownloadFile("http://" + ip + ":" + port + "/bios.json", fileName);
                System.Threading.Thread.Sleep(1000);
                sha1 = wc.DownloadString("http://" + ip + ":" + port + "/bios-checksum.txt");
                System.Threading.Thread.Sleep(1000);
                sha1 = sha1.ToUpper();
                file = new StreamReader(@fileName);
            }
            catch
            {
                file = new StreamReader(@fileBakName);
            }            

            if (GetSha1Hash(fileName).Equals(sha1))
            {
                jsonFile = file.ReadToEnd();
                dynamic jsonParse = JsonConvert.DeserializeObject(jsonFile);

                foreach (var obj in jsonParse)
                {
                    foreach (var obj2 in obj)
                    {
                        foreach (var obj3 in obj2)
                        {
                            str = Convert.ToString(obj3);
                            if (mod.Contains(str))
                            {
                                file.Close();
                                return obj["versao"];
                            }
                        }
                    }
                }
                file.Close();
                return null;
            }
            file.Close();
            return null;
        }
        public static string GetSha1Hash(string filePath)
        {
            using (FileStream fs = File.OpenRead(filePath))
            {
                SHA1 sha = new SHA1Managed();
                return BitConverter.ToString(sha.ComputeHash(fs)).Replace("-", string.Empty);
            }
        }
    }
}
