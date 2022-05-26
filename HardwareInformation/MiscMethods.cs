using Microsoft.Win32;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

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

        //Generates a MD5 hash from an input
        public static string HashMd5Generator(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        //Authenticates the user
        public static bool offlineLogin(string userName, string password)
        {
            if (userName == StringsAndConstants.offlineModeUser && password == StringsAndConstants.offlineModePassword)
                return true;
            return false;
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
