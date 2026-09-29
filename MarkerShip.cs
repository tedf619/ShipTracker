using System.Drawing.Drawing2D;

using GMap.NET;
using GMap.NET.WindowsForms;

namespace ShipTracker;

public class MarkerShip : GMapMarker
{
  // Vessel Attributes
  public long Mmsi { get; set; }  // Maritime Mobile Service Identity, a 9-digit number that uniquely identifies a ship
  public AisShipType ShipType { get; set; }
  public string Description => ShipType.GetDescription();
  public string VesselName { get; set; }
  public float Course { get; set; } // Heading in degrees (0 = North)
  public double Speed { get; set; }
  public AisNavigationalStatus NavigationalStatus { get; set; } = AisNavigationalStatus.Unknown;
  public Color Color => GetShipColor();
  public Color BorderColor { get; set; } = Color.DarkBlue;
  public Color SelectionColor { get; set; } = Color.Gold;
  public float BorderWidth { get; set; } = 2.0f;
  public bool IsSelected { get; set; } = false;

  public Color GetShipColor() => ShipType switch
  {
    AisShipType.Fishing => Color.Green,
    AisShipType.Towing => Color.Yellow,
    AisShipType.DredgingOrUnderwaterOperations => Color.Pink,
    AisShipType.Military => Color.Black,
    AisShipType.PleasureCraft => Color.LightGreen,
    AisShipType.HighSpeedCraft => Color.LightBlue,
    AisShipType.SpecialCraftOrService => Color.Teal,
    AisShipType.Passenger => Color.Blue,
    AisShipType.Cargo => Color.Orange,
    AisShipType.Tanker => Color.Red,
    _ => Color.LightGray,
  };

  static readonly PointF[] BaseHullPolygon = new PointF[]
  {
          new PointF(0, -16),    // Bow (Front Tip)
          new PointF(5, -7),     // Starboard Midship
          new PointF(5, 16),     // Starboard Stern
          new PointF(-5, 16),   // Port Stern
          new PointF(-5, -7)     // Port Midship
  };

  public MarkerShip(PointLatLng pos, long mmsi, string vesselName, float course = 0f)
      : base(pos)
  {
    Mmsi = mmsi;
    VesselName = vesselName;
    Course = course;
    Size = new Size(10, 32);  // hit-test bounding box size surrounding the marker
    Offset = new Point(-Size.Width / 2, -Size.Height / 2); // used to ensure LocalPosition is anchored to the center of the marker shape
  }

  public override void OnRender(Graphics g)
  {
    if (g == null) return;

    GraphicsState state = g.Save();
    g.SmoothingMode = SmoothingMode.AntiAlias;

    DrawShip(g);

    g.Restore(state);
  }

  void DrawShip(Graphics g)
  {
    // LocalPosition is calculated by GMap.NET as the top-left screen pixel for this marker.
    // Calculate exact center pixel coordinates.
    float centerX = LocalPosition.X - Offset.X;
    float centerY = LocalPosition.Y - Offset.Y;

    // move graphics origin to marker center on the map screen
    g.TranslateTransform(centerX, centerY);

    // rotate by vessel's heading
    g.RotateTransform(Course);

    if (ShipType != AisShipType.Unknown)
      centerX = LocalPosition.X - Offset.X;

    Color activeFill = IsSelected ? Color.White : Color;
    Color activeBorder = IsSelected ? SelectionColor : BorderColor;
    float activePenWidth = IsSelected ? BorderWidth + 1.5f : BorderWidth;

    // draw ship outline
    using (SolidBrush fillBrush = new SolidBrush(activeFill))
    using (Pen outlinePen = new Pen(activeBorder, activePenWidth))
    {
      g.FillPolygon(fillBrush, BaseHullPolygon);
      g.DrawPolygon(outlinePen, BaseHullPolygon);
    }
  }
}