using SharpPcap;

namespace AlbionRadar.Entities;

public class DisplayableCaptureDevice
{
    public ICaptureDevice Device { get; }
    public string DescriptionOrName { get; }

    public DisplayableCaptureDevice(ICaptureDevice device, int index)
    {
        Device = device;
        DescriptionOrName = $"{index + 1}. {(string.IsNullOrEmpty(device.Description) ? " (No description available)" : device.Description)}";
    }
}