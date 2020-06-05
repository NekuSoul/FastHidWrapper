using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static FastHidWrapper.NativeMethods;

namespace FastHidWrapper
{
	internal static class HidDeviceEnumerator
	{
		private static Guid _hidGuid;

		static HidDeviceEnumerator()
		{
			HidD_GetHidGuid(ref _hidGuid);
		}

		internal static IEnumerable<HidDevice> GetHidDevices()
		{
			var deviceInfoSet = SetupDiGetClassDevs(
				ref _hidGuid,
				null,
				0,
				DiGetClassFlags.DIGCF_PRESENT | DiGetClassFlags.DIGCF_DEVICEINTERFACE);

			var deviceInfos = EnumerateDeviceInfo(deviceInfoSet);

			var deviceInterfaceInfos =
				deviceInfos.SelectMany(deviceInfo => EnumerateDeviceInterfaces(deviceInfoSet, deviceInfo));

			var paths = deviceInterfaceInfos.Select(
				deviceInterfaceInfo => GetDevicePath(deviceInfoSet, deviceInterfaceInfo)).ToList();

			SetupDiDestroyDeviceInfoList(deviceInfoSet);

			foreach (var path in paths)
			{
				yield return new HidDevice(path);
			}
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
				ref _hidGuid,
				interfaceIndex,
				ref deviceInterfaceData))
			{
				yield return deviceInterfaceData;

				interfaceIndex++;
			}
		}

		private static string GetDevicePath(IntPtr deviceInfoSet, DeviceInterfaceData deviceInterfaceData)
		{
			var size = 0;
			var interfaceDetailData = new DeviceInterfaceDetailData
			{
				Size = IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8
			};

			SetupDiGetDeviceInterfaceDetailBuffer(
				deviceInfoSet,
				ref deviceInterfaceData,
				IntPtr.Zero, 
				0,
				ref size,
				IntPtr.Zero);

			var success = SetupDiGetDeviceInterfaceDetail(
				deviceInfoSet,
				ref deviceInterfaceData,
				ref interfaceDetailData,
				size,
				ref size,
				IntPtr.Zero);

			return !success ? null : interfaceDetailData.DevicePath;
		}
	}
}
