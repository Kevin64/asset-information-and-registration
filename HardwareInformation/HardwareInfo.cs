﻿using Microsoft.Win32;
using System;
using System.Management;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;

public static class HardwareInfo
{
    private static string unknown = "Unknown";

    //Fetches the CPU information, including the number of cores/threads
    public static string GetProcessorCores()
    {
        string Id = "", logical = "";

        ManagementClass mc = new ManagementClass("win32_processor");
        ManagementObjectCollection moc = mc.GetInstances();
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
        
        foreach (var item in searcher.Get())
            logical = item["NumberOfLogicalProcessors"].ToString();
        foreach (ManagementObject queryObj in moc)
        {
            Id = queryObj.Properties["name"].Value.ToString() + " " + queryObj.Properties["CurrentClockSpeed"].Value.ToString()
               + " " + "MHz" + " (" + queryObj.Properties["NumberOfCores"].Value.ToString() + "C/" + logical + "T)";
            break;
        }
        Id = Id.Replace("(R)","");
        Id = Id.Replace("(TM)", "");
        Id = Id.Replace("(tm)", "");
        return Id;
    }

    //Fetches the GPU information
    public static string GetGPUInfo()
    {
        string gpuname = "", gpuramStr;
        double gpuram;

        ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_VideoController");

        foreach(ManagementObject queryObj in searcher.Get())
        {      
            if(queryObj["DeviceID"].ToString().Equals("VideoController1"))
            {
                gpuram = Convert.ToInt64(queryObj["AdapterRAM"]);
                gpuram = Math.Round(gpuram / 1048576, 0);
                if (Math.Ceiling(Math.Log10(gpuram)) > 3)
                    gpuramStr = Convert.ToString(Math.Round(gpuram / 1024, 1)) + " GB";
                else
                    gpuramStr = gpuram + " MB";
                gpuname = queryObj["Caption"].ToString() + " (" + gpuramStr + ")";
            }            
        }
        return gpuname;
    }

    //Fetches the operation mode that the storage is running (IDE/AHCI/NVMe)
    public static string GetStorageOperation()
    {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_SCSIController");

        foreach (ManagementObject queryObj in searcher.Get())
            if (queryObj["Name"].ToString().Contains("NVM"))
                return "NVMe";    
        
        searcher = new ManagementObjectSearcher("select * from Win32_IDEController");
        
        foreach (ManagementObject queryObj in searcher.Get())
            if (queryObj["Name"].ToString().Contains("AHCI"))
                return "AHCI";
        return "IDE/Legacy";
    }

    //Fetches the type of drive the system has (SSD or HDD), and the quantity of each
    public static string GetStorageType()
    {
        int j = 0;

        if (getOSInfoAux().Equals("10") || getOSInfoAux().Equals("8.1") || getOSInfoAux().Equals("8"))
        {
            int size = 10, i = 0;
            string[] type = new string[size];
            string concat, msftName = "Msft Virtual Disk";

            ManagementScope scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from MSFT_PhysicalDisk");
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
            return "Desconhecido (provavelmente HDD)";
    }

    //Fetches the SSD/HDD total size (sums all drives sizes)
    public static string GetHDSize()
    {
        int i = 0;
        double dresult = 0;
        string dresultStr;

        if (getOSInfoAux().Equals("10") || getOSInfoAux().Equals("8.1") || getOSInfoAux().Equals("8"))
        {
            ManagementScope scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from MSFT_PhysicalDisk");
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
                foreach (ManagementObject queryObj in searcher.Get())
                    dresult += Convert.ToInt64(queryObj.Properties["Size"].Value.ToString());                    
        }
        else
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
                if (d.IsReady == true && d.DriveType != DriveType.Network)
                    dresult += d.TotalSize;
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

    //Fetches the primary MAC Address
    public static string GetMACAddress()
    {
        string MACAddress = "";

        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        ManagementObjectCollection moc = mc.GetInstances();
                
        foreach (ManagementObject mo in moc)
        {
            string[] gat = (string[])mo["DefaultIPGateway"];
            if (MACAddress == String.Empty)
                if ((bool)mo["IPEnabled"] == true && gat != null)
                    MACAddress = mo["MacAddress"].ToString();
            mo.Dispose();
        }
        if (MACAddress != "")
            return MACAddress;
        else
            return null;
    }

    //Fetches the primary IP address
    public static string GetIPAddress()
    {
        string[] IPAddress = null;
        try
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                string[] gat = (string[])mo["DefaultIPGateway"];
                if ((bool)mo["IPEnabled"] == true && gat != null)
                    IPAddress = (string[])mo["IPAddress"];
                mo.Dispose();
            }
            return IPAddress[0];
        }
        catch
        {
            return null;
        }
    }

    //Fetches the computer's manufacturer
    public static string GetBoardMaker()
    {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_ComputerSystem");
        
        foreach (ManagementObject queryObj in searcher.Get())
        {
            try
            {
                return queryObj.GetPropertyValue("Manufacturer").ToString();
            }
            catch
            {
            }
        }
        return unknown;
    }

    //Fetches the computer's model
    public static string GetModel()
    {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_ComputerSystem");
        
        foreach (ManagementObject queryObj in searcher.Get())
        {
            try
            {
                return queryObj.GetPropertyValue("Model").ToString();
            }
            catch
            {
            }
        }
        return unknown;
    }

    //Fetches the motherboard serial number
    public static string GetBoardProductId()
    {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_BaseBoard");
        
        foreach (ManagementObject queryObj in searcher.Get())
        {
            try
            {
                return queryObj.GetPropertyValue("SerialNumber").ToString();
            }
            catch
            {
            }
        }
        return unknown;
    }

    //Fetches the amount of RAM of the system
    public static string GetPhysicalMemory()
    {
        long MemSize = 0;
        long mCap;

        ManagementScope scope = new ManagementScope();
        ObjectQuery objQuery = new ObjectQuery("select * from Win32_PhysicalMemory");
        ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, objQuery);
        ManagementObjectCollection moc = searcher.Get();

        foreach (ManagementObject queryObj in moc)
        {
            mCap = Convert.ToInt64(queryObj["Capacity"]);
            MemSize += mCap;
        }
        MemSize = (MemSize / 1024) / 1024 / 1024;
        return MemSize.ToString() + " GB";
    }

    //Fetches the number of RAM slots on the system
    public static string GetNumRamSlots()
    {
        int MemSlots = 0;

        ManagementScope scope = new ManagementScope();
        ObjectQuery objQuery = new ObjectQuery("select * from Win32_PhysicalMemoryArray");
        ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, objQuery);
        ManagementObjectCollection moc = searcher.Get();

        foreach (ManagementObject queryObj in moc)
            if(Convert.ToString(queryObj["Tag"]).Equals("Physical Memory Array 0"))
                MemSlots = Convert.ToInt32(queryObj["MemoryDevices"]);
        return MemSlots.ToString();
    }

    //Fetches the number of free RAM slots on the system
    public static int GetNumFreeRamSlots(int num)
    {
        int i = 0;
        string[] MemSlotsUsed = new string[num];

        ManagementScope scope = new ManagementScope();
        ObjectQuery objQuery = new ObjectQuery("select * from Win32_PhysicalMemory");
        ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, objQuery);
        ManagementObjectCollection moc = searcher.Get();

        foreach (ManagementObject queryObj in moc)
        {
            if(!Convert.ToString(queryObj["DeviceLocator"]).Contains("SYSTEM ROM"))
            {
                MemSlotsUsed[i] = Convert.ToString(queryObj["DeviceLocator"]);
                i++;
            }            
        }        
        return i;
    }

    //Fetches the default gateway of the NIC
    public static string GetDefaultIPGateway()
    {
        string gateway = "";

        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        ManagementObjectCollection moc = mc.GetInstances();        

        foreach (ManagementObject queryObj in moc)
        {
            if (gateway == String.Empty)
                if ((bool)queryObj["IPEnabled"] == true)
                    gateway = queryObj["DefaultIPGateway"].ToString();
            queryObj.Dispose();
        }
        gateway = gateway.Replace(":", "");
        return gateway;
    }

    //Fetches the the NT version
    static string getOSInfoAux()
    {
        string operatingSystem = "";

        OperatingSystem os = Environment.OSVersion;
        Version vs = os.Version;        

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

    //Fetches the operating system information
    public static string GetOSInformation()
    {
        string getOSMajor = getOSInfoAux();
        string displayVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion", "").ToString();
        string releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "releaseId", "").ToString();

        ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
        
        foreach (ManagementObject queryObj in searcher.Get())
        {
            try
            {
                if (getOSInfoAux().Equals("10"))
                {
                    if(Convert.ToInt32(releaseId) <= 2004)
                        return (((string)queryObj["Caption"]).Trim() + ", v" + releaseId + ", build " + (string)queryObj["Version"] + ", " + (string)queryObj["OSArchitecture"]).Substring(10);
                    else
                        return (((string)queryObj["Caption"]).Trim() + ", v" + displayVersion + ", build " + (string)queryObj["Version"] + ", " + (string)queryObj["OSArchitecture"]).Substring(10);
                }                    
                else
                    return (((string)queryObj["Caption"]).Trim() + ", build " + (string)queryObj["Version"] + ", " + (string)queryObj["OSArchitecture"]).Substring(10);
            }
            catch
            {
            }
        }
        return unknown;
    }

    //Fetches the computer's hostname
    public static string GetComputerName()
    {
        string info = "";

        ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
        ManagementObjectCollection moc = mc.GetInstances();
        
        foreach (ManagementObject queryObj in moc)
            info = (string)queryObj["Name"];
        return info;
    }

    //Fetches the BIOS version
    public static string GetComputerBIOS()
    {
        string biosVersion = "";

        ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_BIOS");
        ManagementObjectCollection moc = searcher.Get();

        foreach (ManagementObject queryObj in moc)
            biosVersion = (string)queryObj["SMBIOSBIOSVersion"];
        return biosVersion;
    }

    //Fetches the BIOS type (BIOS or UEFI)
    public static string GetBIOSType()
    {
        try
        {
            PowerShell PowerShellInst = PowerShell.Create();        
            PowerShellInst.AddScript("Confirm-SecureBootUEFI");        
            Collection<PSObject> PSOutput = PowerShellInst.Invoke();

            foreach (PSObject queryObj in PSOutput)
                return "UEFI";
            return "BIOS";
        }
        catch
        {
            return "BIOS";
        }        
    }

    //Fetches the Secure Boot status
    public static string GetSecureBoot()
    {        
        try
        {
            string secBoot = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecureBoot\State", "UEFISecureBootEnabled", 0).ToString();
            if (secBoot.Equals("0"))
                return "Desativado";
            else
                return "Ativado";
        }
        catch(NullReferenceException e)
        {
            return "Não suportado";
        }
    }

    //Auxiliary method for GetStorageType method, that groups the same objects in a list and counts them
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