using AssetInformationAndRegistration.Properties;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AssetInformationAndRegistration
{
    ///<summary>Class that allows changing the progressbar color</summary>
    public static class ModifyProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            _ = SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }

    ///<summary>Class for miscelaneous methods</summary>
    internal static class MiscMethods
    {
        ///<summary>Check the registry for a installation/maintenance date</summary>
        ///<param name="mode">Service type, 'true' for formatting, 'false' for maintenance</param>
        ///<returns>The amount of days since the service date, or '-1' if an exception occur</returns>
        public static double RegCheck(bool mode)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(ConstantsDLL.Properties.Resources.HWINFO_REG_PATH);
                DateTime li = Convert.ToDateTime(rk.GetValue(ConstantsDLL.Properties.Resources.LAST_INSTALL).ToString());
                DateTime lm = Convert.ToDateTime(rk.GetValue(ConstantsDLL.Properties.Resources.LAST_MAINTENANCE).ToString());
                return mode ? (DateTime.Today - li).TotalDays : (DateTime.Today - lm).TotalDays;
            }
            catch
            {
                return -1;
            }
        }

        ///<summary>Creates a registry key when a register operation is made in GUI mode</summary>
        ///<param name="mode">Service type, 'true' for formatting, 'false' for maintenance</param>
        ///<param name="dateTimePicker">Desired date</param>
        public static void RegCreate(bool mode, DateTimePicker dateTimePicker)
        {
            RegistryKey rk = Registry.LocalMachine.CreateSubKey(ConstantsDLL.Properties.Resources.HWINFO_REG_PATH, true);
            if (mode)
            {
                rk.SetValue(ConstantsDLL.Properties.Resources.LAST_INSTALL, dateTimePicker.Value.ToString().Substring(0, 10), RegistryValueKind.String);
                rk.SetValue(ConstantsDLL.Properties.Resources.LAST_MAINTENANCE, dateTimePicker.Value.ToString().Substring(0, 10), RegistryValueKind.String);
            }
            else
            {
                rk.SetValue(ConstantsDLL.Properties.Resources.LAST_MAINTENANCE, dateTimePicker.Value.ToString().Substring(0, 10), RegistryValueKind.String);
            }
        }

        ///<summary>Creates a registry key when a register operation is made in CLI mode</summary>
        ///<param name="mode">Service type, 'true' for formatting, 'false' for maintenance</param>
        ///<param name="dateTime">Desired date</param>
        public static void RegCreate(bool mode, string dateTime)
        {
            RegistryKey rk = Registry.LocalMachine.CreateSubKey(ConstantsDLL.Properties.Resources.HWINFO_REG_PATH, true);
            if (mode)
            {
                rk.SetValue(ConstantsDLL.Properties.Resources.LAST_INSTALL, dateTime.Substring(0, 10), RegistryValueKind.String);
                rk.SetValue(ConstantsDLL.Properties.Resources.LAST_MAINTENANCE, dateTime.Substring(0, 10), RegistryValueKind.String);
            }
            else
            {
                rk.SetValue(ConstantsDLL.Properties.Resources.LAST_MAINTENANCE, dateTime.Substring(0, 10), RegistryValueKind.String);
            }
        }

        ///<summary>Fetches the WebView2 systemwide version</summary>
        ///<returns>The WebView2 runtime version, or an empty string if inexistent</returns>
        public static string GetWebView2Version()
        {
            RegistryKey rk = Environment.Is64BitOperatingSystem
                ? Registry.LocalMachine.CreateSubKey(ConstantsDLL.Properties.Resources.WEBVIEW2_REG_PATH_X64, true)
                : Registry.LocalMachine.CreateSubKey(ConstantsDLL.Properties.Resources.WEBVIEW2_REG_PATH_X86, true);
            if (rk != null)
            {
                object o = rk.GetValue("pv");
                return o != null ? o.ToString() : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        ///<summary>Checks if a log file exists and creates a directory if necessary</summary>
        ///<param name="path">File path</param>
        ///<returns>'true' if log exists, 'false' if not</returns>
        ///<exception cref="System.Exception">Thrown when there is a problem with the query</exception>
        public static string CheckIfLogExists(string path)
        {
            bool b;
            try
            {
#if DEBUG
                //Checks if log directory exists
                b = File.Exists(path + ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.DEV_STATUS + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#else
                //Checks if log directory exists
                b = File.Exists(path + ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#endif
                //If not, creates a new directory
                if (!b)
                {
                    Directory.CreateDirectory(path);
                    return ConstantsDLL.Properties.Resources.FALSE;
                }
                return ConstantsDLL.Properties.Resources.TRUE;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        ///<summary>Initializes the theme, according to the host theme</summary>
        ///<returns>'true' if system is using Dark theme, 'false' if otherwise</returns>
        public static bool ThemeInit()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(ConstantsDLL.Properties.Resources.THEME_REG_PATH))
                {
                    if (key != null)
                    {
                        object o = key.GetValue(ConstantsDLL.Properties.Resources.THEME_REG_KEY);
                        return o != null && o.Equals(0);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        ///<summary>Updates the 'last installed' or 'last maintenance' labels</summary>
        ///<param name="mode">Service type, 'true' for formatting, 'false' for maintenance</param>
        ///<returns>Text which will be shown inside the program, with number of days since the last formatting/maintenance</returns>
        public static string SinceLabelUpdate(bool mode)
        {
            string InstallLabel, MaintenanceLabel;
            if (mode)
            {
                InstallLabel = RegCheck(mode).ToString();
                return !InstallLabel.Equals("-1")
                    ? "(" + InstallLabel + Strings.DAYS_PASSED_TEXT + Strings.FORMAT_TEXT + ")"
                    : Strings.SINCE_UNKNOWN;
            }
            else
            {
                MaintenanceLabel = RegCheck(mode).ToString();
                return !MaintenanceLabel.Equals("-1")
                    ? "(" + MaintenanceLabel + Strings.DAYS_PASSED_TEXT + Strings.MAINTENANCE_TEXT + ")"
                    : Strings.SINCE_UNKNOWN;
            }
        }

        ///<summary>Fetches the screen scale</summary>
        ///<returns>The current window scaling</returns>
        public static int GetWindowsScaling()
        {
            return (int)(100 * Screen.PrimaryScreen.Bounds.Width / System.Windows.SystemParameters.PrimaryScreenWidth);
        }

        ///<summary>Fetches the program's binary version</summary>
        ///<returns>The current application version in the format 'v0.0.0.0'</returns>
        public static string Version()
        {
            return "v" + Application.ProductVersion;
        }

        ///<summary>Fetches the program's binary version (for unstable releases)</summary>
        ///<param name="testBranch">Test branch (alpha, beta, rc, etc)</param>
        ///<returns>The current application version in the format 'v0.0.0.0-testBranch'</returns>
        public static string Version(string testBranch)
        {
            return "v" + Application.ProductVersion + "-" + testBranch;
        }
    }
}
