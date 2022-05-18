using Microsoft.Win32;
using System;
using System.Security;
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
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"Software\HardwareInformation");
                DateTime li = Convert.ToDateTime(rk.GetValue("LastInstallation").ToString());
                DateTime lm = Convert.ToDateTime(rk.GetValue("LastMaintenance").ToString());
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
            RegistryKey rk = Registry.LocalMachine.CreateSubKey(@"Software\HardwareInformation", true);
            if (mode)
            {
                rk.SetValue("LastInstallation", dateTimePicker.Value.ToString().Substring(0, 10), RegistryValueKind.String);
                rk.SetValue("LastMaintenance", dateTimePicker.Value.ToString().Substring(0, 10), RegistryValueKind.String);
            }
            else
                rk.SetValue("LastMaintenance", dateTimePicker.Value.ToString().Substring(0, 10), RegistryValueKind.String);

        }

        //Creates a registry key when a register operation is made in CLI mode
        public static void regCreate(bool mode, string dateTime)
        {
            RegistryKey rk = Registry.LocalMachine.CreateSubKey(@"Software\HardwareInformation", true);
            if (mode)
            {
                rk.SetValue("LastInstallation", dateTime.Substring(0, 10), RegistryValueKind.String);
                rk.SetValue("LastMaintenance", dateTime.Substring(0, 10), RegistryValueKind.String);
            }
            else
                rk.SetValue("LastMaintenance", dateTime.Substring(0, 10), RegistryValueKind.String);
        }

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
            if (userName == "test" && password == "test")
                return true;
            return false;
        }
    }
}
