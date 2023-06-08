# FastHidWrapper

## Description

This is a .NET library for sending data to HID devices on Windows. Primarily made for sending data to QMK-powered keyboards through the raw HID interface.

## Usage

This example sends two bytes containing the values 0xBE and 0xEF to a QMK-powered keyboard.

```cs
int vendorId = 0x3535;
int productId = 0x3510;
ushort usagePage = 0xFF60;
ushort usage = 0x0061;
byte[] bytes = new byte[] { 0x00, 0xBE, 0xEF };

HidDevice hidDevice = HidDevice.GetDevice(vendorId, productId, usagePage, usage);

hidDevice.Write(bytes);
```
