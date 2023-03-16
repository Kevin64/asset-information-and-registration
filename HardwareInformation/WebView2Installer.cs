using ConstantsDLL;
using HardwareInfoDLL;
using System;
using System.Diagnostics;
using System.Net;

namespace HardwareInformation
{
    internal static class WebView2Installer
    {
        public static string Install()
        {
            try
            {
                using (var client = new WebClient())
                {

                    if (HardwareInfo.GetOSInfoAux() == StringsAndConstants.windows7)
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    }
                    client.DownloadFile(StringsAndConstants.webview2url, StringsAndConstants.webview2filePath);
                    var process = Process.Start(StringsAndConstants.webview2filePath);
                    process.WaitForExit();
                    return process.ExitCode.ToString();
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
    }
}
