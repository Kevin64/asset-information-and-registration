using System;
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
      ManagementClass mangnmt = new ManagementClass("Win32_LogicalDisk");
      ManagementObjectCollection mcol = mangnmt.GetInstances();
      long dresult = 0;
      string result = "";
      foreach (ManagementObject strt in mcol)
      {
         dresult += Convert.ToInt64(strt["Size"]) / 1000 / 1000 / 1000;
      }
      return Convert.ToString(dresult).Truncate(3) + " GB";
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
      ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
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
      ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
      foreach (ManagementObject wmi in searcher.Get())
      {
         foreach(PropertyData data in wmi.Properties)
         {
            return data.Name + " " + data.Value;
         }
         
      }
      searcher.Dispose();
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

   public static string GetOSInformation()
   {
      ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
      foreach (ManagementObject wmi in searcher.Get())
      {
         try
         {
            return ((string)wmi["Caption"]).Trim() + ", " + (string)wmi["Version"] + ", " + (string)wmi["OSArchitecture"];
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
}