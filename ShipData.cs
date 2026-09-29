namespace ShipTracker;

public class ShipData
{
  public long Mmsi { get; set; }
  public string VesselName { get; set; } = "Unknown";
  public AisShipType ShipType { get; set; } = AisShipType.Unknown;
  public string CallSign { get; set; } = string.Empty;
  public string Destination { get; set; } = string.Empty;
  public double Latitude { get; set; }
  public double Longitude { get; set; }
  public double SpeedKnots { get; set; }
  public double CourseDegrees { get; set; }
  public float HeadingDegrees { get; set; }
  public AisNavigationalStatus NavigationalStatus { get; set; } = AisNavigationalStatus.Unknown;
  public DateTime LastPositionUtc { get; set; }
  public MarkerShip? Marker { get; set; }
}



