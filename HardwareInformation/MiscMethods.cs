using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        //Creates a registry key when a register operation is made
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
    }
}
