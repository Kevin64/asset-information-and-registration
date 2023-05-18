using AssetInformationAndRegistration.Properties;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AssetInformationAndRegistration
{
    //Class that allows changing the progressbar color
    public static class ModifyProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            _ = SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }

    //Class with auxiliary methods for minor functionality
    internal static class MiscMethods
    {
        //Check the registry for a installation/maintenance date
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

        //Creates a registry key when a register operation is made in GUI mode
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

        //Creates a registry key when a register operation is made in CLI mode
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

        //Fetches the WebView2 systemwide version
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

        public static string CheckIfLogExists(string path)
        {
            bool b;
            try
            {
#if DEBUG
                //Checks if log directory exists
                b = File.Exists(path + ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.DEV_STATUS + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#else
                //Checks if log file exists
                b = File.Exists(path + ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + ConstantsDLL.Properties.Resources.LOG_FILE_EXT);
#endif
                //If not, creates a new directory
                if (!b)
                {
                    Directory.CreateDirectory(path);
                    return "false";
                }
                return "true";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        //Initializes the theme, according to the host theme
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

        //Updates the 'last installed' or 'last maintenance' labels
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

        //Fetches the screen scale
        public static int GetWindowsScaling()
        {
            return (int)(100 * Screen.PrimaryScreen.Bounds.Width / System.Windows.SystemParameters.PrimaryScreenWidth);
        }

        //Fetches the program's binary version
        public static string Version()
        {
            return "v" + Application.ProductVersion;
        }

        //Fetches the program's binary version (for unstable releases)
        public static string Version(string testBranch)
        {
            return "v" + Application.ProductVersion + "-" + testBranch;
        }
    }
}
