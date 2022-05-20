using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace HardwareInformation
{
	public static class HardwareInfo
	{
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
				   + " " + StringsAndConstants.frequency + " (" + queryObj.Properties["NumberOfCores"].Value.ToString() + "C/" + logical + "T)";
				break;
			}
			Id = Id.Replace("(R)", "");
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

			foreach (ManagementObject queryObj in searcher.Get())
			{
				if (!queryObj["Caption"].ToString().Equals("Microsoft Remote Display Adapter"))
				{
					if (queryObj["MaxRefreshRate"] != null)
					{
						gpuram = Convert.ToInt64(queryObj["AdapterRAM"]);
						gpuram = Math.Round(gpuram / 1048576, 0);
						if (Math.Ceiling(Math.Log10(gpuram)) > 3)
							gpuramStr = Convert.ToString(Math.Round(gpuram / 1024, 1)) + " " + StringsAndConstants.gb;
						else
							gpuramStr = gpuram + " " + StringsAndConstants.mb;
						gpuname = queryObj["Caption"].ToString() + " (" + gpuramStr + ")";
					}
				}
			}
			gpuname = gpuname.Replace("(R)", "");
			gpuname = gpuname.Replace("(TM)", "");
			gpuname = gpuname.Replace("(tm)", "");
			return gpuname;
		}

		//Fetches the operation mode that the storage is running (IDE/AHCI/NVMe)
		public static string GetStorageOperation()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_SCSIController");

			foreach (ManagementObject queryObj in searcher.Get())
				if (queryObj["Name"].ToString().Contains("NVM"))
					return StringsAndConstants.nvme;

			searcher = new ManagementObjectSearcher("select * from Win32_IDEController");

			foreach (ManagementObject queryObj in searcher.Get())
				if ((queryObj["Name"].ToString().Contains(StringsAndConstants.ahci) || queryObj["Name"].ToString().Contains(StringsAndConstants.sata)) && !queryObj["Name"].ToString().Contains(StringsAndConstants.raid))
					return StringsAndConstants.ahci;

			return StringsAndConstants.ide;
		}

		//Fetches the type of drive the system has (SSD or HDD), and the quantity of each
		public static string GetStorageType()
		{
			//int j = 0;
			double dresult = 0;
			string dresultStr = "";

			if (getOSInfoAux().Equals(StringsAndConstants.windows10) || getOSInfoAux().Equals(StringsAndConstants.windows8_1) || getOSInfoAux().Equals(StringsAndConstants.windows8))
			{
				int size = 10, i = 0;
				string[] type = new string[size];
				string[] bytesHDD = new string[size];
				string[] bytesSSD = new string[size];
				string concat = "", msftName = "Msft Virtual Disk";

				ManagementScope scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
				ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from MSFT_PhysicalDisk");
				scope.Connect();
				searcher.Scope = scope;

				foreach (ManagementObject queryObj in searcher.Get())
				{
					if (!Convert.ToString(queryObj["FriendlyName"]).Equals(msftName))
					{
						if ((Convert.ToInt16(queryObj["MediaType"]).Equals(3) || Convert.ToInt16(queryObj["MediaType"]).Equals(4) || Convert.ToInt16(queryObj["MediaType"]).Equals(0)) && !Convert.ToInt16(queryObj["BusType"]).Equals(7))
						{
							dresult = Convert.ToInt64(queryObj.Properties["Size"].Value.ToString());
							dresult = Math.Round(dresult / 1000000000, 0);

							if (Math.Log10(dresult) > 2.9999)
								dresultStr = Convert.ToString(Math.Round(dresult / 1000, 1)) + " " + StringsAndConstants.tb;
							else
								dresultStr = dresult + " " + StringsAndConstants.gb;

							switch (Convert.ToInt16(queryObj["MediaType"]))
							{
								case 3:
									type[i] = StringsAndConstants.hdd;
									bytesHDD[i] = dresultStr;
									i++;
									break;
								case 4:
									type[i] = StringsAndConstants.ssd;
									bytesSSD[i] = dresultStr;
									i++;
									break;
								case 0:
									type[i] = StringsAndConstants.hdd;
									bytesHDD[i] = dresultStr;
									i++;
									break;
							}
						}
					}
				}

				var typeSliced = type.Take(i);
				var typeSlicedHDD = bytesHDD.Take(i);
				var typeSlicedSSD = bytesSSD.Take(i);
				searcher.Dispose();
				concat = countDistinct(typeSliced.ToArray(), typeSlicedHDD.ToArray(), typeSlicedSSD.ToArray());

				return concat;
			}
			else
			{
				int size = 10, i = 0;
				string[] type = new string[size];
				string[] bytesHDD = new string[size];
				string concat = "";

				ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");

				foreach (ManagementObject queryObj in searcher.Get())
				{
					dresult = Convert.ToInt64(queryObj.Properties["Size"].Value.ToString());
					dresult = Math.Round(dresult / 1000000000, 0);

					if (Math.Log10(dresult) > 2.9999)
						dresultStr = Convert.ToString(Math.Round(dresult / 1000, 1)) + " " + StringsAndConstants.tb;
					else
						dresultStr = dresult + " " + StringsAndConstants.gb;
					type[i] = StringsAndConstants.hdd;
					bytesHDD[i] = dresultStr;
					i++;
				}
				var typeSliced = type.Take(i);
				var typeSlicedHDD = bytesHDD.Take(i);
				searcher.Dispose();
				concat = countDistinct(typeSliced.ToArray(), typeSlicedHDD.ToArray(), typeSlicedHDD.ToArray());

				return concat;
			}
			//return StorageDetail.HasNominalMediaRotationRate("\\\\.\\PhysicalDrive0");
		}

		//Auxiliary method for GetStorageType method, that groups the same objects in a list and counts them
		public static string countDistinct(string[] array, string[] array2, string[] array3)
		{
			string result = "";
			int j = 0;
			List<string> sizesHDD = new List<string>();
			List<string> sizesSSD = new List<string>();
			char[] comma = { ',', ' ' };
			var groups = array.GroupBy(z => z);

			foreach (var group in groups)
			{
				j = 0;
				result += group.Count() + "x " + group.Key;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == group.Key)
					{
						array[i] = "";
						if (group.Key == StringsAndConstants.hdd)
						{
							sizesHDD.Add(array2[i]);
							j++;
						}
						else
						{
							sizesSSD.Add(array3[i]);
							j++;
						}
						if (group.Count() == j)
						{
							if (group.Key == StringsAndConstants.hdd)
								result += " (" + string.Join(", ", sizesHDD) + ")" + ", ";
							else
								result += " (" + string.Join(", ", sizesSSD) + ")" + ", ";
							break;
						}
					}
				}
			}

			return result.TrimEnd(comma);
		}

		//Fetches the SSD/HDD total size (sums all drives sizes)
		public static string GetHDSize()
		{
			int i = 0;
			double dresult = 0;
			string dresultStr;

			if (getOSInfoAux().Equals(StringsAndConstants.windows10) || getOSInfoAux().Equals(StringsAndConstants.windows8_1) || getOSInfoAux().Equals(StringsAndConstants.windows8))
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
				if (i == 0)
					foreach (ManagementObject queryObj in searcher.Get())
						dresult += Convert.ToInt64(queryObj.Properties["Size"].Value.ToString());
			}
			else
			{
				ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");

				foreach (ManagementObject queryObj in searcher.Get())
				{
					dresult += Convert.ToInt64(queryObj.Properties["Size"].Value.ToString());
				}
			}
			dresult = Math.Round(dresult / 1000000000, 0);
			if (Math.Log10(dresult) > 2.9999)
			{
				dresultStr = Convert.ToString(Math.Round(dresult / 1000, 1)) + " " + StringsAndConstants.tb;
				return dresultStr;
			}
			else
			{
				dresultStr = dresult + " " + StringsAndConstants.gb;
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
			return StringsAndConstants.unknown;
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
			return StringsAndConstants.unknown;
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
			return StringsAndConstants.unknown;
		}

		//Fetches the amount of RAM of the system
		public static string GetPhysicalMemory()
		{
			long MemSize = 0;
			long mCap;
			string mType = "", mSpeed = "";

			ManagementScope scope = new ManagementScope();
			ObjectQuery objQuery = new ObjectQuery("select * from Win32_PhysicalMemory");
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, objQuery);
			ManagementObjectCollection moc = searcher.Get();

			foreach (ManagementObject queryObj in moc)
			{
				if (!Convert.ToString(queryObj["DeviceLocator"]).Contains(StringsAndConstants.systemRom))
				{
					mCap = Convert.ToInt64(queryObj["Capacity"]);
					MemSize += mCap;
					if (getOSInfoAux().Equals(StringsAndConstants.windows10))
					{
						if (queryObj["SMBIOSMemoryType"].ToString().Equals(StringsAndConstants.ddr4smbios))
						{
							mType = StringsAndConstants.ddr4;
							mSpeed = " " + queryObj["Speed"].ToString() + StringsAndConstants.frequency;
						}
						else if (queryObj["SMBIOSMemoryType"].ToString().Equals(StringsAndConstants.ddr3smbios))
						{
							mType = StringsAndConstants.ddr3;
							mSpeed = " " + queryObj["Speed"].ToString() + StringsAndConstants.frequency;
						}
						else if (queryObj["SMBIOSMemoryType"].ToString().Equals("3"))
						{
							mType = "";
							mSpeed = "";
						}
						else
						{
							mType = StringsAndConstants.ddr2;
							mSpeed = " " + queryObj["Speed"].ToString() + StringsAndConstants.frequency;
						}
					}
					else
					{
						if (queryObj["MemoryType"].ToString().Equals(StringsAndConstants.ddr3memorytype))
						{
							mType = StringsAndConstants.ddr3;
							mSpeed = " " + queryObj["Speed"].ToString() + StringsAndConstants.frequency;
						}
						else if (queryObj["MemoryType"].ToString().Equals("2") || queryObj["MemoryType"].ToString().Equals("0"))
						{
							mType = "";
							mSpeed = "";
						}
						else
						{
							mType = StringsAndConstants.ddr2;
							mSpeed = " " + queryObj["Speed"].ToString() + StringsAndConstants.frequency;
						}
					}
				}
			}
			MemSize = (MemSize / 1024) / 1024 / 1024;
			return MemSize.ToString() + " " + StringsAndConstants.gb + " " + mType + mSpeed;
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
				if (Convert.ToString(queryObj["Tag"]).Equals("Physical Memory Array 0"))
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
				if (!Convert.ToString(queryObj["DeviceLocator"]).Contains(StringsAndConstants.systemRom))
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

		//Fetches the OS architecture
		public static string getOSArch()
		{
			bool is64bit = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"));
			if (is64bit)
				return StringsAndConstants.arch64;
			else
				return StringsAndConstants.arch32;
		}

		//Fetches the NT version
		public static string getOSInfoAux()
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
							operatingSystem = StringsAndConstants.windows7;
						else if (vs.Minor == 2)
							operatingSystem = StringsAndConstants.windows8;
						else
							operatingSystem = StringsAndConstants.windows8_1;
						break;
					case 10:
						operatingSystem = StringsAndConstants.windows10;
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
			string displayVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion", "").ToString();
			string releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "releaseId", "").ToString();

			ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

			foreach (ManagementObject queryObj in searcher.Get())
			{
				try
				{
					if (getOSInfoAux().Equals(StringsAndConstants.windows10))
					{
						if (Convert.ToInt32(releaseId) <= 2004)
							return (((string)queryObj["Caption"]).Trim() + ", v" + releaseId + ", " + StringsAndConstants.build + (string)queryObj["Version"] + ", " + (string)queryObj["OSArchitecture"]).Substring(10);
						else
							return (((string)queryObj["Caption"]).Trim() + ", v" + displayVersion + ", " + StringsAndConstants.build + (string)queryObj["Version"] + ", " + (string)queryObj["OSArchitecture"]).Substring(10);
					}
					else
						return (((string)queryObj["Caption"]).Trim() + " " + (string)queryObj["CSDVersion"] + ", " + StringsAndConstants.build + (string)queryObj["Version"] + ", " + (string)queryObj["OSArchitecture"]).Substring(10);
				}
				catch
				{
				}
			}
			return StringsAndConstants.unknown;
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

		//Fetches the BIOS type (BIOS or UEFI) on Windows 7
		public const int ERROR_INVALID_FUNCTION = 1;
		[DllImport("kernel32.dll",
			EntryPoint = "GetFirmwareEnvironmentVariableW",
			SetLastError = true,
			CharSet = CharSet.Unicode,
			ExactSpelling = true,
			CallingConvention = CallingConvention.StdCall)]
		public static extern int GetFirmwareType(string lpName, string lpGUID, IntPtr pBuffer, uint size);
		public static string GetBIOSType7()
		{
			GetFirmwareType("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0);

			if (Marshal.GetLastWin32Error() == ERROR_INVALID_FUNCTION)
				return StringsAndConstants.bios;
			else
				return StringsAndConstants.uefi;
		}

		//Fetches the BIOS type (BIOS or UEFI) on Windows 8 and later
		[DllImport("kernel32.dll")]
		static extern bool GetFirmwareType(ref uint FirmwareType);
		public static string GetBIOSType()
		{
			if (getOSInfoAux().Equals(StringsAndConstants.windows10) || getOSInfoAux().Equals(StringsAndConstants.windows8_1) || getOSInfoAux().Equals(StringsAndConstants.windows8))
			{
				uint firmwaretype = 0;
				if (GetFirmwareType(ref firmwaretype))
				{
					if (firmwaretype == 1)
						return StringsAndConstants.bios;
					else if (firmwaretype == 2)
						return StringsAndConstants.uefi;
				}
				return StringsAndConstants.notDetermined;
			}
			else
				return GetBIOSType7();
		}

		//Fetches the Secure Boot status (alternative method)
		public static string GetSecureBootAlt()
		{
			try
			{
				PowerShell PowerShellInst = PowerShell.Create();
				PowerShellInst.AddScript("Confirm-SecureBootUEFI");
				Collection<PSObject> PSOutput = PowerShellInst.Invoke();

				foreach (PSObject queryObj in PSOutput)
					return StringsAndConstants.activated;
				return StringsAndConstants.deactivated;
			}
			catch
			{
				return StringsAndConstants.notSupported;
			}
		}

		//Fetches the Secure Boot status
		public static string GetSecureBoot()
		{
			try
			{
				string secBoot = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecureBoot\State", "UEFISecureBootEnabled", 0).ToString();
				if (secBoot.Equals("0"))
					return StringsAndConstants.deactivated;
				else
					return StringsAndConstants.activated;
			}
			catch
			{
				return StringsAndConstants.notSupported;
			}
		}

		//Fetches the Virtualization Technology status
		public static string GetVirtualizationTechnology()
		{
			int flag = 0;

			if (!getOSInfoAux().Equals(StringsAndConstants.windows7))
			{
				ManagementClass mc = new ManagementClass("win32_processor");
				ManagementObjectCollection moc = mc.GetInstances();

				foreach (ManagementObject queryObj in moc)
				{
					if (queryObj["VirtualizationFirmwareEnabled"].ToString().Equals("True"))
						flag = 2;
					else if (GetHyperVStatus())
						flag = 2;
				}
				if (flag != 2)
				{
					if (GetBIOSType() == StringsAndConstants.uefi)
						flag = 1;
					else
						flag = 0;
				}
			}

			if (flag == 2)
				return StringsAndConstants.activated;
			else if (flag == 1)
				return StringsAndConstants.deactivated;
			else
				return StringsAndConstants.notSupported;
		}

		//Fetches the Hyper-V installation status
		public static bool GetHyperVStatus()
		{
			string featureName;
			UInt32 featureToggle;

			ManagementClass mc = new ManagementClass("Win32_OptionalFeature");
			ManagementObjectCollection moc = mc.GetInstances();
			foreach (ManagementObject queryObj in moc)
			{
				featureName = (string)queryObj.Properties["Name"].Value;
				featureToggle = (UInt32)queryObj.Properties["InstallState"].Value;

				if ((featureName.Equals("Microsoft-Hyper-V") && featureToggle.Equals(1)) || (featureName.Equals("Microsoft-Hyper-V-Hypervisor") && featureToggle.Equals(1)) || (featureName.Equals("Containers-DisposableClientVM") && featureToggle.Equals(1)))
					return true;
			}
			return false;
		}

		public static string GetSMARTStatus()
		{
			string statusCaption, statusValue;
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * from Win32_DiskDrive");
			ManagementObjectCollection moc = searcher.Get();
			foreach (ManagementObject queryObj in moc)
			{
				statusCaption = (string)queryObj.Properties["Caption"].Value;
				statusValue = (string)queryObj.Properties["Status"].Value;
				if (statusValue == StringsAndConstants.predFail)
					return statusCaption;
			}
			return StringsAndConstants.ok;
		}

		public static string GetTPMStatus()
		{
			string isActivated = "", isEnabled = "", specVersion = "";
			ManagementScope scope = new ManagementScope(@"\\.\root\cimv2\Security\MicrosoftTPM");
			ObjectQuery query = new ObjectQuery("select * from Win32_Tpm");
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

			foreach (ManagementObject queryObj in searcher.Get())
			{
				isActivated = queryObj.Properties["IsActivated_InitialValue"].Value.ToString();
				isEnabled = queryObj.Properties["IsEnabled_InitialValue"].Value.ToString();
				specVersion = queryObj.Properties["SpecVersion"].Value.ToString();
			}
			if (specVersion != "")
				specVersion = specVersion.Substring(0, 3);
			else
				specVersion = StringsAndConstants.notExistant;
			return specVersion;
		}


	}
}