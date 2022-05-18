using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace HardwareInformation
{
	public class jFile
	{
		public string id { get; set; }
		public string marca { get; set; }
		public string modelo { get; set; }
		public string versao { get; set; }
		public string tipo { get; set; }
	}
	public static class BIOSFileReader
	{
		private static string fileBios = "bios.json";
		private static string fileSha1 = "bios-checksum.txt";
		private static string jsonFile, sha1, aux;
		private static WebClient wc;
		private static StreamReader fileB;

		public static bool checkHost(string ip, string port)
        {
			try
			{
				wc = new WebClient();
				wc.DownloadString("http://" + ip + ":" + port + "/forneceDadosBIOS.php");
				System.Threading.Thread.Sleep(300);
				wc.DownloadFile("http://" + ip + ":" + port + "/" + fileBios, fileBios);
				System.Threading.Thread.Sleep(300);
				sha1 = wc.DownloadString("http://" + ip + ":" + port + "/" + fileSha1);
				System.Threading.Thread.Sleep(300);
				sha1 = sha1.ToUpper();
				fileB = new StreamReader(@fileBios);
				aux = fileBios;
				fileB.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

		//Reads a json file retrieved from the server and parses brand, model and BIOS versions, returning the latter
		[STAThread]
		public static string[] fetchInfo(string brd, string mod, string type, string ip, string port)
		{
			if (!checkHost(ip, port))
				return null;

			string[] arr;
			fileB = new StreamReader(@fileBios);
			if (GetSha1Hash(aux).Equals(sha1))
			{
				jsonFile = fileB.ReadToEnd();
				jFile[] jsonParse = JsonConvert.DeserializeObject<jFile[]>(@jsonFile);

				for(int i = 0; i < jsonParse.Length; i++)
				{
					if (mod.Contains(jsonParse[i].modelo) && brd.Contains(jsonParse[i].marca))
					{
						
						if (!type.Equals(jsonParse[i].tipo))
						{
							arr = new String[] {jsonParse[i].versao, "false"};
							fileB.Close();
							return arr;
						}
						arr = new String[] {jsonParse[i].versao, "true"};
						fileB.Close();
						return arr;
					}
				}
			}
			arr = new String[] {"-1", "-1"};
			fileB.Close();
			return arr;
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
