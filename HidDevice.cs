using System;

namespace FastHidWrapper
{
	public class HidDevice
	{
		internal HidDevice()
		{
		}

		public static HidDevice GetDevice(int vendorId, int productId, ushort usagePage, ushort usage)
			=> HidDeviceEnumerator.GetDevice(vendorId, productId, usagePage, usage);

		public void Write(byte[] data)
		{
		}
	}
}