namespace TelemetryService;

public class TelemetryResponse
{
    public long DeviceId { get; set; }
    public double Temperature { get; set; }
    public DateTime Timestamp { get; set; }
}