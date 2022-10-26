using Microsoft.Win32;
using System;
using System.Windows.Forms;
using ConstantsDLL;
using System.IO;
using HardwareInformation.Properties;

namespace HardwareInformation
{
    internal static class MiscMethods
    {
        //Check the registry for a installation/maintenance date
        public static double regCheck(bool mode)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(StringsAndConstants.HWINFO_REG_PATH);
                DateTime li = Convert.ToDateTime(rk.GetValue(StringsAndConstants.lastInstall).ToString());
                DateTime lm = Convert.ToDateTime(rk.GetValue(StringsAndConstants.lastMaintenance).ToString());
                if (mode)
                    return (DateTime.Today - li).TotalDays;
                else
                    return (DateTime.Today - lm).TotalDays;
            }
            catch
            {
                return -1;
            }
        }

        //Creates a registry key when a register operation is made in GUI mode
        public static void regCreate(bool mode, DateTimePicker dateTimePicker)
        {
            RegistryKey rk = Registry.LocalMachine.CreateSubKey(StringsAndConstants.HWINFO_REG_PATH, true);
            if (mode)
            {
                rk.SetValue(StringsAndConstants.lastInstall, dateTimePicker.Value.ToString().Substring(0, 10), RegistryValueKind.String);
                rk.SetValue(StringsAndConstants.lastMaintenance, dateTimePicker.Value.ToString().Substring(0, 10), RegistryValueKind.String);
            }
            else
                rk.SetValue(StringsAndConstants.lastMaintenance, dateTimePicker.Value.ToString().Substring(0, 10), RegistryValueKind.String);
        }

        //Creates a registry key when a register operation is made in CLI mode
        public static void regCreate(bool mode, string dateTime)
        {
            RegistryKey rk = Registry.LocalMachine.CreateSubKey(StringsAndConstants.HWINFO_REG_PATH, true);
            if (mode)
            {
                rk.SetValue(StringsAndConstants.lastInstall, dateTime.Substring(0, 10), RegistryValueKind.String);
                rk.SetValue(StringsAndConstants.lastMaintenance, dateTime.Substring(0, 10), RegistryValueKind.String);
            }
            else
                rk.SetValue(StringsAndConstants.lastMaintenance, dateTime.Substring(0, 10), RegistryValueKind.String);
        }

        //Fetches the WebView2 systemwide version
        public static string getWebView2Version()
        {
            RegistryKey rk;
            if (Environment.Is64BitOperatingSystem)
                rk = Registry.LocalMachine.CreateSubKey(StringsAndConstants.WEBVIEW2_REG_PATH_X64, true);
            else
                rk = Registry.LocalMachine.CreateSubKey(StringsAndConstants.WEBVIEW2_REG_PATH_X86, true);
            if (rk != null)
            {
                Object o = rk.GetValue("pv");
                if (o != null)
                    return o.ToString();
                else return "";
            }
            else
                return "";
        }

        public static string checkIfLogExists(string path)
        {
            bool b;
            try
            {
#if DEBUG
                //Checks if log directory exists
                b = File.Exists(path + StringsAndConstants.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + "-" + Resources.dev_status + StringsAndConstants.LOG_FILE_EXT);
#else
                //Checks if log file exists
                b = File.Exists(path + StringsAndConstants.LOG_FILENAME_OOBE + "-v" + Application.ProductVersion + StringsAndConstants.LOG_FILE_EXT);
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
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StringsAndConstants.THEME_REG_PATH))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue(StringsAndConstants.THEME_REG_KEY);
                        if (o != null && o.Equals(0))
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        //Updates the 'last installed' or 'last maintenance' labels
        public static string sinceLabelUpdate(bool mode)
        {
            string InstallLabel, MaintenanceLabel;
            if (mode)
            {
                InstallLabel = regCheck(mode).ToString();
                if (!InstallLabel.Equals("-1"))
                    return "(" + InstallLabel + StringsAndConstants.DAYS_PASSED_TEXT + StringsAndConstants.FORMAT_TEXT + ")";
                else
                    return StringsAndConstants.SINCE_UNKNOWN;
            }
            else
            {
                MaintenanceLabel = regCheck(mode).ToString();
                if (!MaintenanceLabel.Equals("-1"))
                    return "(" + MaintenanceLabel + StringsAndConstants.DAYS_PASSED_TEXT + StringsAndConstants.MAINTENANCE_TEXT + ")";
                else
                    return StringsAndConstants.SINCE_UNKNOWN;
            }
        }

        //Fetches the program's binary version
        public static string version()
        {
            return "v" + Application.ProductVersion;
        }

        //Fetches the program's binary version (for unstable releases)
        public static string version(string testBranch)
        {
            return "v" + Application.ProductVersion + "-" + testBranch;
        }
    }
}
