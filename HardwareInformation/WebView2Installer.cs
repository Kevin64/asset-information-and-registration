using ConstantsDLL;
using System.Diagnostics;
using System.Net;

namespace HardwareInformation
{
    internal static class WebView2Installer
    {
        public static int install()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(StringsAndConstants.webview2url, StringsAndConstants.webview2filePath);
                var process = Process.Start(StringsAndConstants.webview2filePath);
                process.WaitForExit();
                return process.ExitCode;
            }
        }
    }
}
