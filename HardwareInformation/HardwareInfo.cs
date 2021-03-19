using Microsoft.Win32;
using System;
using System.Management;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;

public static class HardwareInfo
{
    private static string unknown = "Unknown";

    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

    public static string GetProcessorCores()
    {
        ManagementClass mc = new ManagementClass("win32_processor");
        ManagementObjectCollection moc = mc.GetInstances();
        String Id = String.Empty;
        string logical = "";
        foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
        {
            logical = item["NumberOfLogicalProcessors"].ToString();
        }
        foreach (ManagementObject mo in moc)
        {
            Id = mo.Properties["name"].Value.ToString() + " " + mo.Properties["CurrentClockSpeed"].Value.ToString()
               + " " + "MHz" + " (" + mo.Properties["NumberOfCores"].Value.ToString() + "C/" + logical + "T)";
            break;
        }
        Id = Id.Replace("(R)","");
        Id = Id.Replace("(TM)", "");
        Id = Id.Replace("(tm)", "");
        return Id;
    }

    public static string GetGPUInfo()
    {
        string gpuname = "", gpuramStr = "";
        double gpuram = 0;
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
        foreach(ManagementObject queryObj in searcher.Get())
        {      
            if(queryObj["DeviceID"].ToString().Equals("VideoController1"))
            {
                gpuram = Convert.ToInt64(queryObj["AdapterRAM"]);
                gpuram = Math.Round(gpuram / 1048576, 0);
                if (Math.Ceiling(Math.Log10(gpuram)) > 3)
                {
                    gpuramStr = Convert.ToString(Math.Round(gpuram / 1024, 1)) + " GB";
                }
                else
                {
                    gpuramStr = gpuram + " MB";
                }
                gpuname = queryObj["Caption"].ToString() + " (" + gpuramStr + ")";
            }            
        }
        return gpuname;
    }

    public static string GetStorageType()
    {
        int j = 0;
        if (getOSInfoAux().Equals("10") || getOSInfoAux().Equals("8.1") || getOSInfoAux().Equals("8"))
        {
            ManagementScope scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM MSFT_PhysicalDisk");
            int size = 10, i = 0;
            string[] type = new string[size];
            string concat = "", msftName = "Msft Virtual Disk";
            scope.Connect();
            searcher.Scope = scope;

            foreach (ManagementObject queryObj in searcher.Get())
            {
                if (!Convert.ToString(queryObj["FriendlyName"]).Equals(msftName))
                {
                    switch (Convert.ToInt16(queryObj["MediaType"]))
                    {
                        case 3:
                            type[i] = "HDD";
                            i++;
                            j++;
                            break;
                        case 4:
                            type[i] = "SSD";
                            i++;
                            j++;
                            break;
                    }
                }
            }
            if(j == 0)
            {
                type[i] = "HDD";
                i++;
            }
            var typeSliced = type.Take(i);
            searcher.Dispose();
            concat = countDistinct(typeSliced.ToArray());
            return concat;
        }
        else
        {
            return "Desconhecido (provavelmente HDD)";
        }        
    }

    public static string GetHDSize()
    {
        int i = 0;
        double dresult = 0;
        string dresultStr = "";
        if (getOSInfoAux().Equals("10") || getOSInfoAux().Equals("8.1") || getOSInfoAux().Equals("8"))
        {
            ManagementScope scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM MSFT_PhysicalDisk");
            scope.Connect();
            searcher.Scope = scope;

            foreach (ManagementObject queryObj in searcher.Get())
            {
                if (Convert.ToInt16(queryObj["MediaType"]).Equals(3) || Convert.ToInt16(queryObj["MediaType"]).Equals(4))
                {
                    dresult += Convert.ToInt64(queryObj.Properties["Size"].Value.ToString());
                    i++;
                }
            }           
            if(i == 0)
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {                    
                    dresult += Convert.ToInt64(queryObj.Properties["Size"].Value.ToString());                    
                }
            }
        }
        else
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady == true && d.DriveType != DriveType.Network)
                {
                    dresult += d.TotalSize;
                }
            }            
        }
        dresult = Math.Round(dresult / 1000000000, 0);
        if(Math.Ceiling(Math.Log10(dresult)) > 3)
        {
            dresultStr = Convert.ToString(Math.Round(dresult / 1000, 1)) + " TB";
            return dresultStr;
        }
        else
        {
            dresultStr = dresult + " GB";
            return dresultStr;
        }

    }

    public static string GetMACAddress()
    {
        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        ManagementObjectCollection moc = mc.GetInstances();
        string MACAddress = String.Empty;
        foreach (ManagementObject mo in moc)
        {
            string[] gat = (string[])mo["DefaultIPGateway"];
            if (MACAddress == String.Empty)
            {
                if ((bool)mo["IPEnabled"] == true && gat != null)
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
            string[] gat = (string[])mo["DefaultIPGateway"];
            if ((bool)mo["IPEnabled"] == true && gat != null)
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
        return unknown;
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
        return unknown;
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
        return unknown;
    }

    public static string GetPhysicalMemory()
    {
        ManagementScope oMs = new ManagementScope();
        ObjectQuery oQuery = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
        ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
        ManagementObjectCollection oCollection = oSearcher.Get();

        long MemSize = 0;
        long mCap = 0;

        foreach (ManagementObject obj in oCollection)
        {
            mCap = Convert.ToInt64(obj["Capacity"]);
            MemSize += mCap;
        }
        MemSize = (MemSize / 1024) / 1024 / 1024;
        return MemSize.ToString() + " GB";
    }

    public static string GetNumRamSlots()
    {
        int MemSlots = 0;
        ManagementScope oMs = new ManagementScope();
        ObjectQuery oQuery2 = new ObjectQuery("SELECT * FROM Win32_PhysicalMemoryArray");
        ManagementObjectSearcher oSearcher2 = new ManagementObjectSearcher(oMs, oQuery2);
        ManagementObjectCollection oCollection2 = oSearcher2.Get();
        foreach (ManagementObject obj in oCollection2)
        {
            if(Convert.ToString(obj["Tag"]).Equals("Physical Memory Array 0"))
            {
                MemSlots = Convert.ToInt32(obj["MemoryDevices"]);
            }            
        }
        return MemSlots.ToString();
    }

    public static int GetNumFreeRamSlots(int num)
    {
        int i = 0;
        string[] MemSlotsUsed = new string[num];
        ManagementScope oMs = new ManagementScope();
        ObjectQuery oQuery3 = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");
        ManagementObjectSearcher oSearcher3 = new ManagementObjectSearcher(oMs, oQuery3);
        ManagementObjectCollection oCollection3 = oSearcher3.Get();
        foreach (ManagementObject obj in oCollection3)
        {
            if(!Convert.ToString(obj["DeviceLocator"]).Contains("SYSTEM ROM"))
            {
                MemSlotsUsed[i] = Convert.ToString(obj["DeviceLocator"]);
                i++;
            }            
        }        
        return i;
    }

    public static string GetDefaultIPGateway()
    {
        ManagementClass mgmt = new ManagementClass("Win32_NetworkAdapterConfiguration");
        ManagementObjectCollection objCol = mgmt.GetInstances();
        string gateway = String.Empty;

        foreach (ManagementObject obj in objCol)
        {
            if (gateway == String.Empty)
            {
                if ((bool)obj["IPEnabled"] == true)
                {
                    gateway = obj["DefaultIPGateway"].ToString();
                }
            }
            obj.Dispose();
        }
        gateway = gateway.Replace(":", "");
        return gateway;
    }

    static string getOSInfoAux()
    {
        OperatingSystem os = Environment.OSVersion;
        Version vs = os.Version;

        string operatingSystem = "";

        if (os.Platform == PlatformID.Win32NT)
        {
            switch (vs.Major)
            {
                case 6:
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
                    return (((string)wmi["Caption"]).Trim() + ", v" + releaseId + ", build " + (string)wmi["Version"] + ", " + (string)wmi["OSArchitecture"]).Substring(10);
                }
                else
                {
                    return (((string)wmi["Caption"]).Trim() + ", build " + (string)wmi["Version"] + ", " + (string)wmi["OSArchitecture"]).Substring(10);
                }
            }
            catch { }
        }
        return unknown;
    }

    public static string GetComputerName()
    {
        ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
        ManagementObjectCollection moc = mc.GetInstances();
        String info = String.Empty;
        foreach (ManagementObject mo in moc)
        {
            info = (string)mo["Name"];
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

    public static string GetBIOSType()
    {
        try
        {
            PowerShell PowerShellInst = PowerShell.Create();        
            PowerShellInst.AddScript("Confirm-SecureBootUEFI");        
            Collection<PSObject> PSOutput = PowerShellInst.Invoke();
            foreach (PSObject obj in PSOutput)
            {
                return "UEFI";
            }
            return "BIOS";
        }
        catch
        {
            return "BIOS";
        }        
    }

    public static string countDistinct(string[] array)
    {
        string result = "";
        char[] comma = { ',' , ' ' };
        var groups = array.GroupBy(z => z);

        foreach (var group in groups)
            result += group.Count() + "x " + group.Key + ", ";
        return result.TrimEnd(comma);
    }
}