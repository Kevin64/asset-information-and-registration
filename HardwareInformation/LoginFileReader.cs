using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace HardwareInformation
{
	public class lFile
	{
		public string id { get; set; }
		public string usuario { get; set; }
		public string senha { get; set; }
		public string nivel { get; set; }
		public string status { get; set; }
	}
	public static class LoginFileReader
	{
		private static string fileLogin = "login.json";
		private static string fileSha1 = "login-checksum.txt";
		private static string jsonFile, sha1, aux;
		private static WebClient wc;
		private static StreamReader fileL;

		public static bool checkHost(string ip, string port)
        {
			try
			{
				wc = new WebClient();
				wc.DownloadString("http://" + ip + ":" + port + "/forneceDadosUsuario.php");
				System.Threading.Thread.Sleep(300);
				wc.DownloadFile("http://" + ip + ":" + port + "/" + fileLogin, fileLogin);
				System.Threading.Thread.Sleep(300);
				sha1 = wc.DownloadString("http://" + ip + ":" + port + "/" + fileSha1);
				System.Threading.Thread.Sleep(300);
				sha1 = sha1.ToUpper();
				fileL = new StreamReader(@fileLogin);
				aux = fileLogin;
				fileL.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

		//Reads a json file retrieved from the server and parses username and encoded password, returning the latter
		[STAThread]
		public static string[] fetchInfo(string nome, string senha, string ip, string port)
		{
			if (!checkHost(ip, port))
				return null;

			string[] arr;
			fileL = new StreamReader(@fileLogin);
			if (GetSha1Hash(aux).Equals(sha1))
			{
				jsonFile = fileL.ReadToEnd();
				lFile[] jsonParse = JsonConvert.DeserializeObject<lFile[]>(@jsonFile);

				for(int i = 0; i < jsonParse.Length; i++)
				{
					if (nome.Equals(jsonParse[i].usuario) && MiscMethods.HashMd5Generator(senha).Equals(jsonParse[i].senha))
					{
						arr = new string[] { "true" };
						fileL.Close();
						return arr;
					}
				}
			}
			arr = new string[] { "false" }; ;
			fileL.Close();
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
