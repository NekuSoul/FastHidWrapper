using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using static FastHidWrapper.NativeMethods;

namespace FastHidWrapper
{
	public class HidDevice
	{
		private HidAttributes _hidAttributes;
		private HidCapabilities _hidCapabilities;
		private readonly IntPtr _hidDeviceObject;
		private IntPtr _hidDeviceReadObject;
		private IntPtr _hidDeviceWriteObject;

		public int VendorId => _hidAttributes.VendorID;
		public int ProductId => _hidAttributes.ProductID;
		public ushort Usage => _hidCapabilities.Usage;
		public ushort UsagePage => _hidCapabilities.UsagePage;

		public string Path { get; }

		internal HidDevice(string path)
		{
			Path = path;
			_hidDeviceObject = OpenDevice(AccessNone);
			QueryAttributes();
			QueryCapabilities();
		}

		private IntPtr OpenDevice(uint access)
		{
			var security = new SecurityAttributes
			{
				InheritHandle = true,
				SecurityDescriptor = IntPtr.Zero
			};
			security.Size = Marshal.SizeOf(security);

			return CreateFile(
				Path,
				access,
				FileShareReadWrite,
				ref security,
				OpenExisting,
				0,
				0);
		}

		private void QueryAttributes()
		{
			_hidAttributes = new HidAttributes();
			_hidAttributes.Size = Marshal.SizeOf(_hidAttributes);
			HidD_GetAttributes(_hidDeviceObject, ref _hidAttributes);
		}

		private void QueryCapabilities()
		{
			_hidCapabilities = new HidCapabilities();
			var capabilitiyPointer = new IntPtr();

			if (!HidD_GetPreparsedData(_hidDeviceObject, ref capabilitiyPointer))
				return;

			HidP_GetCaps(capabilitiyPointer, ref _hidCapabilities);
			HidD_FreePreparsedData(capabilitiyPointer);
		}

		public static HidDevice GetDevice(int vendorId, int productId, ushort usagePage, ushort usage)
			=> HidDeviceEnumerator.GetHidDevices()
				.FirstOrDefault(
					device =>
						device.VendorId == vendorId
						&& device.ProductId == productId
						&& usagePage == device.UsagePage
						&& usage == device.Usage);

		public void Write(byte[] data)
		{
			if (_hidDeviceReadObject == default)
				_hidDeviceReadObject = OpenDevice(AccessRead);

			if (_hidDeviceWriteObject == default)
				_hidDeviceWriteObject = OpenDevice(AccessWrite);

			var dataBuffer = new byte[_hidCapabilities.OutputReportByteLength];
			Array.Copy(data, dataBuffer, data.Length);

			var nativeOverlapped = new NativeOverlapped();
			bool result = WriteFile(_hidDeviceWriteObject, dataBuffer, (uint)dataBuffer.Length, out var written, ref nativeOverlapped);
			var lastWin32Error = Marshal.GetLastWin32Error();
		}
	}
}
