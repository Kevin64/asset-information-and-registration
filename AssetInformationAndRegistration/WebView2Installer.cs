using ConstantsDLL;
using HardwareInfoDLL;
using System;
using System.Diagnostics;
using System.Net;

namespace AssetInformationAndRegistration
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
                    client.DownloadFile(ConstantsDLL.Properties.Resources.webview2url, StringsAndConstants.webView2filePath);
                    Process process = Process.Start(StringsAndConstants.webView2filePath);
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
