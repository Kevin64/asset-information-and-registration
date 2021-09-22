﻿using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Text;

public static class StorageDetail
{
	// For CreateFile to get handle to drive
	private const uint GENERIC_READ = 0x80000000;
	private const uint GENERIC_WRITE = 0x40000000;
	private const uint FILE_SHARE_READ = 0x00000001;
	private const uint FILE_SHARE_WRITE = 0x00000002;
	private const uint OPEN_EXISTING = 3;
	private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

	// CreateFile to get handle to drive
	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern SafeFileHandle CreateFileW(
		[MarshalAs(UnmanagedType.LPWStr)]
		string lpFileName,
		uint dwDesiredAccess,
		uint dwShareMode,
		IntPtr lpSecurityAttributes,
		uint dwCreationDisposition,
		uint dwFlagsAndAttributes,
		IntPtr hTemplateFile);

	// For control codes
	private const uint FILE_DEVICE_CONTROLLER = 0x00000004;
	private const uint IOCTL_SCSI_BASE = FILE_DEVICE_CONTROLLER;
	private const uint METHOD_BUFFERED = 0;
	private const uint FILE_READ_ACCESS = 0x00000001;
	private const uint FILE_WRITE_ACCESS = 0x00000002;

	private static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
	{
		return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct STORAGE_PROPERTY_QUERY
	{
		public uint PropertyId;
		public uint QueryType;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		public byte[] AdditionalParameters;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct DEVICE_SEEK_PENALTY_DESCRIPTOR
	{
		public uint Version;
		public uint Size;
		[MarshalAs(UnmanagedType.U1)]
		public bool IncursSeekPenalty;
	}

	// DeviceIoControl to check no seek penalty
	[DllImport("kernel32.dll", EntryPoint = "DeviceIoControl",
				SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool DeviceIoControl(
		SafeFileHandle hDevice,
		uint dwIoControlCode,
		ref STORAGE_PROPERTY_QUERY lpInBuffer,
		uint nInBufferSize,
		ref DEVICE_SEEK_PENALTY_DESCRIPTOR lpOutBuffer,
		uint nOutBufferSize,
		out uint lpBytesReturned,
		IntPtr lpOverlapped);

	// For DeviceIoControl to check nominal media rotation rate
	private const uint ATA_FLAGS_DATA_IN = 0x02;

	[StructLayout(LayoutKind.Sequential)]
	private struct ATA_PASS_THROUGH_EX
	{
		public ushort Length;
		public ushort AtaFlags;
		public byte PathId;
		public byte TargetId;
		public byte Lun;
		public byte ReservedAsUchar;
		public uint DataTransferLength;
		public uint TimeOutValue;
		public uint ReservedAsUlong;
		public IntPtr DataBufferOffset;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public byte[] PreviousTaskFile;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public byte[] CurrentTaskFile;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct ATAIdentifyDeviceQuery
	{
		public ATA_PASS_THROUGH_EX header;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public ushort[] data;
	}

	// DeviceIoControl to check nominal media rotation rate
	[DllImport("kernel32.dll", EntryPoint = "DeviceIoControl",
				SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool DeviceIoControl(
		SafeFileHandle hDevice,
		uint dwIoControlCode,
		ref ATAIdentifyDeviceQuery lpInBuffer,
		uint nInBufferSize,
		ref ATAIdentifyDeviceQuery lpOutBuffer,
		uint nOutBufferSize,
		out uint lpBytesReturned,
		IntPtr lpOverlapped);

	// For error message
	private const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

	[DllImport("kernel32.dll", SetLastError = true)]
	static extern uint FormatMessage(
		uint dwFlags,
		IntPtr lpSource,
		uint dwMessageId,
		uint dwLanguageId,
		StringBuilder lpBuffer,
		uint nSize,
		IntPtr Arguments);

	// Method for nominal media rotation rate
	// (Administrative privilege is required)
	public static string HasNominalMediaRotationRate(string sDrive)
	{
		SafeFileHandle hDrive = CreateFileW(
			sDrive,
			GENERIC_READ | GENERIC_WRITE, // Administrative privilege is required
			FILE_SHARE_READ | FILE_SHARE_WRITE,
			IntPtr.Zero,
			OPEN_EXISTING,
			FILE_ATTRIBUTE_NORMAL,
			IntPtr.Zero);

		if (hDrive == null || hDrive.IsInvalid)
		{
			string message = GetErrorMessage(Marshal.GetLastWin32Error());
			return "CreateFile failed. " + message;
		}

		uint IOCTL_ATA_PASS_THROUGH = CTL_CODE(
			IOCTL_SCSI_BASE, 0x040b, METHOD_BUFFERED,
			FILE_READ_ACCESS | FILE_WRITE_ACCESS); // From ntddscsi.h

		ATAIdentifyDeviceQuery id_query = new ATAIdentifyDeviceQuery();
		id_query.data = new ushort[256];

		id_query.header.Length = (ushort)Marshal.SizeOf(id_query.header);
		id_query.header.AtaFlags = (ushort)ATA_FLAGS_DATA_IN;
		id_query.header.DataTransferLength =
			(uint)(id_query.data.Length * 2); // Size of "data" in bytes
		id_query.header.TimeOutValue = 3; // Sec
		id_query.header.DataBufferOffset = (IntPtr)Marshal.OffsetOf(
			typeof(ATAIdentifyDeviceQuery), "data");
		id_query.header.PreviousTaskFile = new byte[8];
		id_query.header.CurrentTaskFile = new byte[8];
		id_query.header.CurrentTaskFile[6] = 0xec; // ATA IDENTIFY DEVICE

		uint retval_size;

		bool result = DeviceIoControl(
			hDrive,
			IOCTL_ATA_PASS_THROUGH,
			ref id_query,
			(uint)Marshal.SizeOf(id_query),
			ref id_query,
			(uint)Marshal.SizeOf(id_query),
			out retval_size,
			IntPtr.Zero);

		hDrive.Close();

		if (result == false)
		{
			string message = GetErrorMessage(Marshal.GetLastWin32Error());
			return "DeviceIoControl failed. " + message;
		}
		else
		{
			// Word index of nominal media rotation rate
			// (1 means non-rotate device)
			const int kNominalMediaRotRateWordIndex = 217;

			if (id_query.data[kNominalMediaRotRateWordIndex] == 1)
			{
				return "SSD";
			}
			else
			{
				return "HDD";
			}
		}
	}

	// Method for error message
	private static string GetErrorMessage(int code)
	{
		StringBuilder message = new StringBuilder(255);

		FormatMessage(
			FORMAT_MESSAGE_FROM_SYSTEM,
			IntPtr.Zero,
			(uint)code,
			0,
			message,
			(uint)message.Capacity,
			IntPtr.Zero);

		return message.ToString();
	}
}
