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
		private IntPtr _hidDeviceObject;

		public int VendorId => _hidAttributes.VendorID;
		public int ProductId => _hidAttributes.ProductID;
		public ushort Usage => _hidCapabilities.Usage;
		public ushort UsagePage => _hidCapabilities.UsagePage;

		public string Path { get; }

		internal HidDevice(string path)
		{
			Path = path;
			OpenDevice();
			QueryAttributes();
			QueryCapabilities();
		}

		private void OpenDevice()
		{
			var security = new SecurityAttributes
			{
				InheritHandle = true,
				SecurityDescriptor = IntPtr.Zero
			};
			security.Size = Marshal.SizeOf(security);

			_hidDeviceObject = CreateFile(
				Path,
				AccessNone,
				FileShareRead,
				ref security,
				OpenExisting,
				FileFlagOverlapped,
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
		}
	}
}
