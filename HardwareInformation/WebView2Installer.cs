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
                using (WebClient client = new WebClient())
                {

                    if (HardwareInfo.GetOSInfoAux() == ConstantsDLL.Properties.Resources.windows7)
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    }
                    client.DownloadFile(ConstantsDLL.Properties.Resources.webview2url, ConstantsDLL.Properties.Resources.webview2filePath);
                    Process process = Process.Start(ConstantsDLL.Properties.Resources.webview2filePath);
                    process.WaitForExit();
                    return process.ExitCode.ToString();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
