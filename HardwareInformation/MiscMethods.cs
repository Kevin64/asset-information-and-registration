using Microsoft.Win32;
using System;
using System.Windows.Forms;
using ConstantsDLL;
using System.Windows.Input;

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

        public static string getWebView2Version()
        {
            RegistryKey rk = Registry.LocalMachine.CreateSubKey(StringsAndConstants.WEBVIEW2_REG_PATH, true);
            if(rk != null)
            {
                Object o = rk.GetValue("pv");
                if (o != null)
                    return o.ToString();
                else return "";
            }
            else
                return "";
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
