using System;
using System.Linq;
using System.Text;
using System.Threading;
using HidLibrary;

namespace Codeuctivity.Pages
{
  public static class TemperReader
  {
    internal static TempReaderResult GetTemperature()
    {
      var deviceList = HidDevices.Enumerate().ToArray();
      var control = HidDevices.Enumerate(VendorId, ProductId).SingleOrDefault(x => x.DevicePath.Contains("mi_00"));
      var bulk = HidDevices.Enumerate(VendorId, ProductId).SingleOrDefault(x => x.DevicePath.Contains("mi_01"));
      
      if (control == null || bulk == null)
      {
        return new TempReaderResult("Temper device not found", null);
      }

      control.ReadManufacturer(out var manufacturerRaw);
      control.ReadProduct(out var productRaw);
      var manufacturer = Encoding.Unicode.GetString(manufacturerRaw).TrimEnd('\0');
      var product = Encoding.Unicode.GetString(productRaw).TrimEnd('\0');
      Console.WriteLine("Manufacturer: {0}", manufacturer);
      Console.WriteLine("Product: {0}", product);

      WriteAndReadReport(control, 0x01, bulkInit);
      WriteAndReadReport(bulk, 0x00, bulkInit);
      WriteAndReadReport(bulk, 0x00, hidRawFirmware);

      for (var i = 0; i < 2; i++)
      {
        WriteAndReadReport(bulk, 0x00, temperatureHumidityRaw);
      }

      if (!bulk.IsOpen)
      {
        return new TempReaderResult("Temper initialization failed", null);
      }
      var temperatureCelsius = GetTemperatureCelsius(bulk);

      return new TempReaderResult($"{temperatureCelsius:0.###} °C", temperatureCelsius);
    }

    private static readonly byte[] controlInit = { 0x01, 0x01 };
    private static readonly byte[] temperatureHumidityRaw = { 0x01, 0x80, 0x33, 0x01, 0x00, 0x00, 0x00, 0x00 };
    private static readonly byte[] bulkInit = { 0x01, 0x82, 0x77, 0x01, 0x00, 0x00, 0x00, 0x00 };
    private static readonly byte[] hidRawFirmware = { 0x01, 0x86, 0xff, 0x01, 0x00, 0x00, 0x00, 0x00 };
    private const int VendorId = 0x413D;
    private const int ProductId = 0x2107;

    private static HidReport WriteAndReadReport(IHidDevice device, byte reportId, byte[] data)
    {
      var outData = device.CreateReport();
      outData.ReportId = reportId;
      outData.Data = data;
      device.WriteReport(outData);
      while (outData.ReadStatus != HidDeviceData.ReadStatus.Success)
      {
        Thread.Sleep(1);
      }
      var report = device.ReadReport();
      return report;
    }

    private static double GetTemperatureCelsius(IHidDevice bulk)
    {
      var report = WriteAndReadReport(bulk, 0x00, temperatureHumidityRaw);
      var rawReading = (report.Data[3] & 0xFF) + (report.Data[2] << 8);

      const double calibrationOffset = -1.70;
      const double calibrationScale = 2.74301;
      var temperatureCelsius = (calibrationScale * (rawReading * (125.0 / 32000.0))) + calibrationOffset;
      return temperatureCelsius;
    }
  }
}