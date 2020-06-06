using System;
using System.Runtime.InteropServices;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace FastHidWrapper
{
	internal static class NativeMethods
	{
		#region Constants
		
		internal const uint AccessNone = 0x0;
		internal const uint AccessRead = 0x80000000;
		internal const uint AccessWrite = 0x40000000;
		internal const int FileFlagOverlapped = 0x40000000;
		internal const short FileShareRead = 0x1;
		internal const short FileShareReadWrite = 0x3;
		internal const short OpenExisting = 0x3;

		#endregion

		#region Enums

		[Flags]
		internal enum DiGetClassFlags
		{
			DIGCF_DEFAULT = 0x01,
			DIGCF_PRESENT = 0x02,
			DIGCF_ALLCLASSES = 0x04,
			DIGCF_PROFILE = 0x08,
			DIGCF_DEVICEINTERFACE = 0x10,
		}

		#endregion

		#region Structs

		[StructLayout(LayoutKind.Sequential)]
		internal struct DeviceInfoData
		{
			private int Size;
			private Guid ClassGuid;
			private int DeviceInstance;
			private IntPtr Reserved;

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
			private int Size;
			private Guid InterfaceClassGuid;
			private uint Flags;
			private IntPtr Reserved;

			public void Initialize()
			{
				Size = Marshal.SizeOf(this);
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal struct DeviceInterfaceDetailData
		{
			internal int Size;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			internal string DevicePath;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct HidAttributes
		{
			internal int Size;
			internal ushort VendorID;
			internal ushort ProductID;
			internal short VersionNumber;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct HidCapabilities
		{
			internal ushort Usage;
			internal ushort UsagePage;
			internal ushort InputReportByteLength;
			internal ushort OutputReportByteLength;
			internal ushort FeatureReportByteLength;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
			internal ushort[] Reserved;

			internal ushort NumberLinkCollectionNodes;
			internal ushort NumberInputButtonCaps;
			internal ushort NumberInputValueCaps;
			internal ushort NumberInputDataIndices;
			internal ushort NumberOutputButtonCaps;
			internal ushort NumberOutputValueCaps;
			internal ushort NumberOutputDataIndices;
			internal ushort NumberFeatureButtonCaps;
			internal ushort NumberFeatureValueCaps;
			internal ushort NumberFeatureDataIndices;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct SecurityAttributes
		{
			internal int Size;
			internal IntPtr SecurityDescriptor;
			internal bool InheritHandle;
		}

		#endregion

		#region Methods

		#region hid.dll

		[DllImport("hid.dll")]
		internal static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

		[DllImport("hid.dll")]
		internal static extern bool HidD_GetAttributes(IntPtr hidDeviceObject, ref HidAttributes attributes);

		[DllImport("hid.dll")]
		internal static extern int HidP_GetCaps(IntPtr preparsedData, ref HidCapabilities capabilities);

		[DllImport("hid.dll")]
		internal static extern void HidD_GetHidGuid(ref Guid hidGuid);

		[DllImport("hid.dll")]
		internal static extern bool HidD_GetPreparsedData(IntPtr hidDeviceObject, ref IntPtr preparsedData);

		#endregion

		#region kernel32.dll

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr CreateFile(
			string fileName,
			uint desiredAccess,
			int shareMode,
			ref SecurityAttributes securityAttributes,
			int creationDisposition,
			int flagsAndAttributes,
			int templateFile);

		[DllImport("kernel32.dll")]
		internal static extern bool WriteFile(
			IntPtr hFile,
			byte[] lpBuffer,
			uint nNumberOfBytesToWrite,
			out uint lpNumberOfBytesWritten,
			[In] ref System.Threading.NativeOverlapped lpOverlapped);

		#endregion

		#region setupapi.dll

		[DllImport("setupapi.dll", CharSet = CharSet.Auto, EntryPoint = "SetupDiGetDeviceInterfaceDetail")]
		internal static extern bool SetupDiGetDeviceInterfaceDetailBuffer(
			IntPtr deviceInfoSet,
			ref DeviceInterfaceData deviceInterfaceData,
			IntPtr deviceInterfaceDetailData,
			int deviceInterfaceDetailDataSize,
			ref int requiredSize,
			IntPtr deviceInfoData);

		[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
		internal static extern bool SetupDiGetDeviceInterfaceDetail(
			IntPtr deviceInfoSet,
			ref DeviceInterfaceData deviceInterfaceData,
			ref DeviceInterfaceDetailData deviceInterfaceDetailData,
			int deviceInterfaceDetailDataSize,
			ref int requiredSize,
			IntPtr deviceInfoData);

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

		#endregion

		#endregion
	}
}
