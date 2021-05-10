using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace HardwareInformation
{
    public static class BIOSFileReader
    {
        private static string fileBios = "bios.json", fileBakBios = "bios.bak.json";
        private static string fileSha1 = "bios-checksum.txt", fileBakSha1 = "bios-checksum.bak.txt";
        private static string jsonFile, str, sha1, aux;
        private static WebClient wc;
        private static StreamReader fileB;

        //Reads a json file retrieved from the server and parses brand, model and BIOS versions, returning the latter
        [STAThread]
        public static string fetchInfo(string brd, string mod, string ip, string port)
        {            
            try
            {
                wc = new WebClient();
                wc.DownloadString("http://" + ip + ":" + port + "/forneceDadosBIOS.php");
                System.Threading.Thread.Sleep(1000);
                wc.DownloadFile("http://" + ip + ":" + port + "/" + fileBios, fileBios);
                System.Threading.Thread.Sleep(1000);
                sha1 = wc.DownloadString("http://" + ip + ":" + port + "/" + fileSha1);
                System.Threading.Thread.Sleep(1000);
                sha1 = sha1.ToUpper();
                fileB = new StreamReader(@fileBios);
                aux = fileBios;
            }
            catch
            {
                sha1 = wc.DownloadString(fileBakSha1);
                sha1 = sha1.ToUpper();
                fileB = new StreamReader(@fileBakBios);
                aux = fileBakBios;
            }            

            if (GetSha1Hash(aux).Equals(sha1))
            {
                jsonFile = fileB.ReadToEnd();
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
                                fileB.Close();
                                return obj["versao"];
                            }
                        }
                    }
                }
            }
            fileB.Close();
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
