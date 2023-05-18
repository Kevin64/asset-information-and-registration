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

                    if (HardwareInfo.GetWinVersion() == ConstantsDLL.Properties.Resources.WINDOWS_7)
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    }
                    client.DownloadFile(ConstantsDLL.Properties.Resources.WEBVIEW2_URL, StringsAndConstants.WEBVIEW2_FILE_PATH);
                    Process process = Process.Start(StringsAndConstants.WEBVIEW2_FILE_PATH);
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
