using System;
using System.Linq;
using System.Runtime.InteropServices;
using static FastHidWrapper.NativeMethods;

namespace FastHidWrapper
{
	public class HidDevice
	{
		private HidAttributes _hidAttributes;
		private HidCapabilities _hidCapabilities;
		private IntPtr deviceObject;

		public int VendorId => _hidAttributes.VendorID;
		public int ProductId => _hidAttributes.ProductID;
		public short Usage => _hidCapabilities.Usage;
		public short UsagePage => _hidCapabilities.UsagePage;

		public string Path { get; }

		internal HidDevice(string path)
		{
			Path = path;
			OpenDevice();

			// TODO: Implement Capabilities
		}

		private void OpenDevice()
		{
			var security = new SecurityAttributes
			{
				InheritHandle = true,
				SecurityDescriptor = IntPtr.Zero
			};
			security.Size = Marshal.SizeOf(security);

			deviceObject = CreateFile(Path, AccessNone, FileShareRead, ref security, OpenExisting, FileFlagOverlapped, 0);

			_hidAttributes = new HidAttributes();
			_hidAttributes.Size = Marshal.SizeOf(_hidAttributes);
			HidD_GetAttributes(deviceObject, ref _hidAttributes);
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
		}
	}
}
