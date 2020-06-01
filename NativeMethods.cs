using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace FastHidWrapper
{
	internal static class NativeMethods
	{
		[Flags]
		internal enum DiGetClassFlags
		{
			DIGCF_DEFAULT = 0x01,
			DIGCF_PRESENT = 0x02,
			DIGCF_ALLCLASSES = 0x04,
			DIGCF_PROFILE = 0x08,
			DIGCF_DEVICEINTERFACE = 0x10,
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct DeviceInfoData
		{
			int Size;
			Guid ClassGuid;
			int DeviceInstance;
			IntPtr Reserved;

			public void Initialize()
			{
				Size = Marshal.SizeOf(this);
				DeviceInstance = 0;
				ClassGuid = Guid.Empty;
				Reserved = IntPtr.Zero;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct DeviceInterfaceData
		{
			int Size;
			Guid InterfaceClassGuid;
			int Flags;
			IntPtr Reserved;

			public void Initialize()
			{
				Size = Marshal.SizeOf(this);
			}
		}

		[DllImport("setupapi.dll")]
		public static extern bool SetupDiGetDeviceRegistryProperty(
			IntPtr deviceInfoSet,
			ref DeviceInfoData deviceInfoData,
			int propertyVal,
			ref int propertyRegDataType,
			byte[] propertyBuffer,
			int propertyBufferSize,
			ref int requiredSize);

		[DllImport("hid.dll")]
		internal static extern void HidD_GetHidGuid(ref Guid hidGuid);

		[DllImport("setupapi.dll")]
		internal static extern bool SetupDiEnumDeviceInterfaces(
			IntPtr deviceInfoSet,
			ref DeviceInfoData deviceInfoData,
			ref Guid interfaceClassGuid,
			int memberIndex,
			ref DeviceInterfaceData deviceInterfaceData);

		[DllImport("setupapi.dll")]
		internal static extern bool SetupDiEnumDeviceInfo(
			IntPtr deviceInfoSet,
			int memberIndex,
			ref DeviceInfoData deviceInfoData);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr SetupDiGetClassDevs(
			ref Guid classGuid,
			string enumerator,
			int hwndParent,
			DiGetClassFlags flags);

		[DllImport("setupapi.dll")]
		internal static extern int SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);
	}
}