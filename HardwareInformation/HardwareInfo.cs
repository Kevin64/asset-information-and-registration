using Microsoft.Win32;
using System;
using System.IO;
using System.Management;

public static class HardwareInfo
{
    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

    public static string GetProcessorInformation()
    {
        ManagementClass mc = new ManagementClass("win32_processor");
        ManagementObjectCollection moc = mc.GetInstances();
        String info = String.Empty;
        foreach (ManagementObject mo in moc)
        {
            string name = (string)mo["Name"];
            name = name.Replace("(TM)", "™").Replace("(tm)", "™").Replace("(R)", "®").Replace("(r)", "®").Replace("(C)", "©").Replace("(c)", "©").Replace("    ", " ").Replace("  ", " ");

            info = name + ", " + (string)mo["Caption"] + ", " + (string)mo["SocketDesignation"];
            //mo.Properties["Name"].Value.ToString();
            //break;
        }
        return info;
    }

    public static string GetProcessorInfo()
    {
        ManagementClass mc = new ManagementClass("win32_processor");
        ManagementObjectCollection moc = mc.GetInstances();
        String Id = String.Empty;
        foreach (ManagementObject mo in moc)
        {
            Id = mo.Properties["name"].Value.ToString() + " " + mo.Properties["CurrentClockSpeed"].Value.ToString()
               + " " + "MHz" + " (" + mo.Properties["NumberOfCores"].Value.ToString() + " cores)";
            break;
        }
        return Id;
    }

    public static string GetHDSize()
    {
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        double dresult = 0;

        foreach (DriveInfo d in allDrives)
        {
            if (d.IsReady == true && d.DriveType != DriveType.Network)
            {
                dresult += d.TotalSize;
            }
        }
        return Convert.ToString(Math.Round(dresult / 1000000000, 0)) + " GB";
    }

    public static string GetMACAddress()
    {
        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        ManagementObjectCollection moc = mc.GetInstances();
        string MACAddress = String.Empty;
        foreach (ManagementObject mo in moc)
        {
            if (MACAddress == String.Empty)
            {
                if ((bool)mo["IPEnabled"] == true)
                    MACAddress = mo["MacAddress"].ToString();
            }
            mo.Dispose();
        }

        return MACAddress;
    }

    public static string GetIPAddress()
    {
        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        ManagementObjectCollection moc = mc.GetInstances();
        string[] IPAddress = null;
        foreach (ManagementObject mo in moc)
        {
            if ((bool)mo["IPEnabled"] == true)
                IPAddress = (string[])mo["IPAddress"];
            mo.Dispose();
        }

        return IPAddress[0];
    }

    public static string GetBoardMaker()
    {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_ComputerSystem");
        foreach (ManagementObject wmi in searcher.Get())
        {
            try
            {
                return wmi.GetPropertyValue("Manufacturer").ToString();
            }
            catch { }
        }
        return "Unknown";
    }

    public static string GetModel()
    {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_ComputerSystem");
        foreach (ManagementObject wmi in searcher.Get())
        {
            try
            {
                return wmi.GetPropertyValue("Model").ToString();
            }
            catch { }
        }
        return "Unknown";
    }

    public static string GetBoardProductId()
    {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
        foreach (ManagementObject wmi in searcher.Get())
        {
            try
            {
                return wmi.GetPropertyValue("SerialNumber").ToString();
            }
            catch { }
        }
        return "Unknown";
    }

    public static string GetPhysicalMemory()
    {
        ManagementScope oMs = new ManagementScope();
        ObjectQuery oQuery = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
        ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
        ManagementObjectCollection oCollection = oSearcher.Get();

        long MemSize = 0;
        long mCap = 0;

        // In case more than one Memory sticks are installed
        foreach (ManagementObject obj in oCollection)
        {
            mCap = Convert.ToInt64(obj["Capacity"]);
            MemSize += mCap;
        }
        MemSize = (MemSize / 1024) / 1024;
        return MemSize.ToString() + " MB";
    }

    public static string GetNoRamSlots()
    {
        int MemSlots = 0;
        ManagementScope oMs = new ManagementScope();
        ObjectQuery oQuery2 = new ObjectQuery("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");
        ManagementObjectSearcher oSearcher2 = new ManagementObjectSearcher(oMs, oQuery2);
        ManagementObjectCollection oCollection2 = oSearcher2.Get();
        foreach (ManagementObject obj in oCollection2)
        {
            MemSlots = Convert.ToInt32(obj["MemoryDevices"]);
        }
        return MemSlots.ToString();
    }

    public static string GetDefaultIPGateway()
    {
        //create out management class object using the
        //Win32_NetworkAdapterConfiguration class to get the attributes
        //of the network adapter
        ManagementClass mgmt = new ManagementClass("Win32_NetworkAdapterConfiguration");
        //create our ManagementObjectCollection to get the attributes with
        ManagementObjectCollection objCol = mgmt.GetInstances();
        string gateway = String.Empty;
        //loop through all the objects we find
        foreach (ManagementObject obj in objCol)
        {
            if (gateway == String.Empty)  // only return MAC Address from first card
            {
                //grab the value from the first network adapter we find
                //you can change the string to an array and get all
                //network adapters found as well
                //check to see if the adapter's IPEnabled
                //equals true
                if ((bool)obj["IPEnabled"] == true)
                {
                    gateway = obj["DefaultIPGateway"].ToString();
                }
            }
            //dispose of our object
            obj.Dispose();
        }
        //replace the ":" with an empty space, this could also
        //be removed if you wish
        gateway = gateway.Replace(":", "");
        //return the mac address
        return gateway;
    }

    static string getOSInfoAux()
    {
        //Get Operating system information.
        OperatingSystem os = Environment.OSVersion;
        //Get version information about the os.
        Version vs = os.Version;

        //Variable to hold our return value
        string operatingSystem = "";

        //if (os.Platform == PlatformID.Win32Windows)
        //{
        //    //This is a pre-NT version of Windows
        //    switch (vs.Minor)
        //    {
        //        case 0:
        //            operatingSystem = "95";
        //            break;
        //        case 10:
        //            if (vs.Revision.ToString() == "2222A")
        //                operatingSystem = "98SE";
        //            else
        //                operatingSystem = "98";
        //            break;
        //        case 90:
        //            operatingSystem = "Me";
        //            break;
        //        default:
        //            break;
        //    }
        //}
        if (os.Platform == PlatformID.Win32NT)
        {
            switch (vs.Major)
            {
                //case 3:
                //    operatingSystem = "NT 3.51";
                //    break;
                //case 4:
                //    operatingSystem = "NT 4.0";
                //    break;
                //case 5:
                //    if (vs.Minor == 0)
                //        operatingSystem = "2000";
                //    else
                //        operatingSystem = "XP";
                //    break;
                case 6:
                    //if (vs.Minor == 0)
                    //    operatingSystem = "Vista";
                    if (vs.Minor == 1)
                        operatingSystem = "7";
                    else if (vs.Minor == 2)
                        operatingSystem = "8";
                    else
                        operatingSystem = "8.1";
                    break;
                case 10:
                    operatingSystem = "10";
                    break;
                default:
                    break;
            }
        }
        ////Make sure we actually got something in our OS check
        ////We don't want to just return " Service Pack 2" or " 32-bit"
        ////That information is useless without the OS version.
        //if (operatingSystem != "")
        //{
        //    //Got something.  Let's prepend "Windows" and get more info.
        //    operatingSystem = "Windows " + operatingSystem;
        //    //See if there's a service pack installed.
        //    if (os.ServicePack != "")
        //    {
        //        //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
        //        operatingSystem += " " + os.ServicePack;
        //    }
        //    //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
        //    //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
        //}
        ////Return the information we've gathered.
        return operatingSystem;
    }

    public static string GetOSInformation()
    {
        string getOSMajor = getOSInfoAux();
        string releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
        foreach (ManagementObject wmi in searcher.Get())
        {
            try
            {
                if (getOSInfoAux().Equals("10"))
                {
                    return ((string)wmi["Caption"]).Trim() + ", v" + releaseId + ", build " + (string)wmi["Version"] + ", " + (string)wmi["OSArchitecture"];
                }
                else
                {
                    return ((string)wmi["Caption"]).Trim() + ", build " + (string)wmi["Version"] + ", " + (string)wmi["OSArchitecture"];
                }

            }
            catch { }
        }
        return "BIOS Maker: Unknown";
    }

    public static string GetComputerName()
    {
        ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
        ManagementObjectCollection moc = mc.GetInstances();
        String info = String.Empty;
        foreach (ManagementObject mo in moc)
        {
            info = (string)mo["Name"];
            //mo.Properties["Name"].Value.ToString();
            //break;
        }
        return info;
    }

    public static string GetComputerBIOS()
    {
        String biosVersion = String.Empty;
        ManagementObjectSearcher mSearcher = new ManagementObjectSearcher("SELECT SerialNumber, SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS");
        ManagementObjectCollection collection = mSearcher.Get();
        foreach (ManagementObject obj in collection)
        {
            biosVersion = (string)obj["SMBIOSBIOSVersion"];
        }
        return biosVersion;
    }
}