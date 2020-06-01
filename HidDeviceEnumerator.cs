using System;
using System.Collections.Generic;
using System.Linq;
using static FastHidWrapper.NativeMethods;

namespace FastHidWrapper
{
	internal static class HidDeviceEnumerator
	{
		private static Guid hidGuid;

		static HidDeviceEnumerator()
		{
			HidD_GetHidGuid(ref hidGuid);
		}

		public static HidDevice GetDevice(int vendorId, int productId, ushort usagePage, ushort usage)
		{
			var deviceInfoSet = SetupDiGetClassDevs(
				ref hidGuid,
				null,
				0,
				DiGetClassFlags.DIGCF_PRESENT | DiGetClassFlags.DIGCF_DEVICEINTERFACE);

			var deviceInfos = EnumerateDeviceInfo(deviceInfoSet);
			var deviceInterfaceInfos = deviceInfos.SelectMany(di => EnumerateDeviceInterfaces(deviceInfoSet, di));
			
			SetupDiDestroyDeviceInfoList(deviceInfoSet);

			return new HidDevice();
		}

		private static IEnumerable<DeviceInfoData> EnumerateDeviceInfo(IntPtr deviceInfoSet)
		{
			var deviceIndex = 0;

			var deviceInfoData = new DeviceInfoData();
			deviceInfoData.Initialize();

			while (SetupDiEnumDeviceInfo(deviceInfoSet, deviceIndex, ref deviceInfoData))
			{
				deviceIndex++;

				yield return deviceInfoData;
			}
		}

		private static IEnumerable<DeviceInterfaceData> EnumerateDeviceInterfaces(
			IntPtr deviceInfoSet,
			DeviceInfoData deviceInfoData)
		{
			var deviceInterfaceData = new DeviceInterfaceData();
			deviceInterfaceData.Initialize();
			var interfaceIndex = 0;

			while (SetupDiEnumDeviceInterfaces(
				deviceInfoSet,
				ref deviceInfoData,
				ref hidGuid,
				interfaceIndex,
				ref deviceInterfaceData))
			{
				yield return deviceInterfaceData;

				interfaceIndex++;
			}
		}
	}
}
