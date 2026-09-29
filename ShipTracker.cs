using System.Text.Json;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;

namespace ShipTracker;

public partial class ShipTracker : GMapControl
{
  private static readonly double[][] BoundingBoxMalaccaStrait =
  [
    [6.5, 95.0],   // north-west corner
    [1.0, 104.5],  // south-east corner
  ];

  readonly PointLatLng MalaccaStraitCenter = new(3.2, 100.2);
  readonly AisService aisService = new();
  public Dictionary<long, ShipData> Ships { get; } = [];
  readonly GMapOverlay overlayShips = new("ships");
  AisShipType shipTypeSelected = AisShipType.All;

  // only count ships with an assigned position
  public int ShipsShown { get => overlayShips.Markers.Where(m=> m is MarkerShip ms && m.Position.Lat != 0 & m.Position.Lng != 0).ToList().Count; }

  public ShipTracker()
  {
    InitializeComponent();
    InitializeMap();

    aisService.StatusChanged += FireStatusMessage;
    aisService.MessageReceived += AisService_MessageReceived;
    aisService.Exception += FireException;
  }

  void InitializeMap()
  {
    GMaps.Instance.Mode = AccessMode.ServerAndCache;

    base.MapProvider = GMapProviders.OpenStreetMap;
    base.Position = MalaccaStraitCenter;
    base.MinZoom = 3;
    base.MaxZoom = 19;
    base.Zoom = 7;
    base.DragButton = MouseButtons.Left;
    base.CanDragMap = true;
    base.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
    base.ShowCenter = false;
    base.IgnoreMarkerOnMouseWheel = true;   // so zooming works when on a flight marker

    base.Overlays.Add(overlayShips);

    // draw the tracked bounding box, to highlight the area we're tracking
    var boundaryOverlay = new GMapOverlay("tracking-area");
    var corners = new List<PointLatLng>
    {
      new(BoundingBoxMalaccaStrait[0][0], BoundingBoxMalaccaStrait[0][1]),
      new(BoundingBoxMalaccaStrait[0][0], BoundingBoxMalaccaStrait[1][1]),
      new(BoundingBoxMalaccaStrait[1][0], BoundingBoxMalaccaStrait[1][1]),
      new(BoundingBoxMalaccaStrait[1][0], BoundingBoxMalaccaStrait[0][1]),
      new(BoundingBoxMalaccaStrait[0][0], BoundingBoxMalaccaStrait[0][1]),
    };

    var boundaryPolygon = new GMapPolygon(corners, "tracking-area")
    {
      Stroke = new Pen(Color.FromArgb(180, 30, 144, 255), 2),
      Fill = new SolidBrush(Color.FromArgb(20, 30, 144, 255)),
    };
    boundaryOverlay.Polygons.Add(boundaryPolygon);
    base.Overlays.Add(boundaryOverlay);
  }

  public async void ConnectAsync(string apiKey)
  {
    try
    {
      await aisService.ConnectAsync(apiKey, [BoundingBoxMalaccaStrait], messageTypeFilter: ["PositionReport", "ShipStaticData"]);
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message, "Couldn't Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }

  public async void DisconnectAsync()
  {
    await aisService.DisconnectAsync();
  }

  void AisService_MessageReceived(AisEnvelope message)
  {
    if (message.MetaData is null) return;

    switch (message.MessageType)
    {
      case "PositionReport":  // dynamic, frequent messages
        if (message.Message.TryGetProperty("PositionReport", out var posElement))
        {
          var report = posElement.Deserialize<PositionReport>();
          if (report is not null)
            InvokeIfNeeded(() => HandlePositionReport(message.MetaData, report));
        }
        break;

      case "ShipStaticData":  // static, infrequent messages
        if (message.Message.TryGetProperty("ShipStaticData", out var staticElement))
        {
          var staticData = staticElement.Deserialize<ShipStaticData>();
          if (staticData is not null)
            InvokeIfNeeded(() => HandleStaticData(message.MetaData, staticData));
        }
        break;
    }
  }

  void HandlePositionReport(AisMetaData meta, PositionReport report)
  {
    var ship = GetShip(meta.Mmsi, meta.ShipName);

    // Prefer the decoded report's coordinates.Fall back to metadata.
    ship.Latitude = report.Latitude != 0 ? report.Latitude : meta.Latitude;
    ship.Longitude = report.Longitude != 0 ? report.Longitude : meta.Longitude;
    ship.SpeedKnots = report.Sog;
    ship.CourseDegrees = report.Cog;
    ship.HeadingDegrees = report.TrueHeading;
    ship.NavigationalStatus = GetNavigationalStatusCode(report.NavigationalStatus);
    ship.LastPositionUtc = DateTime.UtcNow;

    if (!string.IsNullOrWhiteSpace(meta.ShipName))
      ship.VesselName = meta.ShipName.Trim();

    UpdateMarker(ship);
    FireShipDataReceived(ship);
  }

  void HandleStaticData(AisMetaData meta, ShipStaticData staticData)
  {
    var ship = GetShip(meta.Mmsi, meta.ShipName);

    ship.ShipType = GetShipType(staticData.Type);

    if (!string.IsNullOrWhiteSpace(staticData.Name))
      ship.VesselName = staticData.Name.Trim();

    if (!string.IsNullOrWhiteSpace(staticData.CallSign))
      ship.CallSign = staticData.CallSign.Trim();

    if (!string.IsNullOrWhiteSpace(staticData.Destination))
      ship.Destination = staticData.Destination.Trim();

    UpdateMarker(ship);
    FireShipDataReceived(ship);
  }

  ShipData GetShip(long mmsi, string? shipName)
  {
    if (Ships.TryGetValue(mmsi, out var existing))
      return existing;

    var ship = new ShipData
    {
      Mmsi = mmsi,
      VesselName = string.IsNullOrWhiteSpace(shipName) ? $"MMSI {mmsi}" : shipName.Trim(),
    };

    Ships[mmsi] = ship;
    return ship;
  }

  public void SelectShip(long mmsi)
  {
    if (!Ships.TryGetValue(mmsi, out var ship)) return;

    foreach (GMapMarker marker in overlayShips.Markers)
    {
      if (marker is not MarkerShip ms) continue;
      ms.IsSelected = ms.Mmsi == mmsi;

      if (!ms.IsSelected) continue;

      if (ship.Latitude > 0 && ship.Longitude > 0)
        Position = new PointLatLng(ship.Latitude, ship.Longitude);  // center the map on the ship
    }

    MakeShipTopMost(mmsi);
  }

  public void ShowShipType(AisShipType shipType)
  {
    shipTypeSelected = shipType;

    overlayShips.Markers.Clear();

    List<ShipData> ships = Ships.Values.ToList();

    if (shipType != AisShipType.All)
      ships = Ships.Values.Where(s => s.ShipType == shipType).ToList();

    if (ships.Count == 0) return;

    foreach (var ship in ships)
    {
      if (ship.Marker == null) continue;
      overlayShips.Markers.Add(ship.Marker);
    }
  }

  protected override void OnMouseDown(MouseEventArgs e)
  {
    base.OnMouseDown(e);
    if (e.Button != MouseButtons.Left) return;

    long mmsi = 0;
    foreach (var marker in overlayShips.Markers)
    {
      if (marker is MarkerShip ms)
      {
        ms.IsSelected = marker.IsMouseOver;
        if (mmsi > 0) continue;
        mmsi = ms.IsSelected ? ms.Mmsi : 0;
      }
    }

    if (mmsi == 0) return;

    MakeShipTopMost(mmsi);

    if (!Ships.TryGetValue(mmsi, out var ship)) return;

    FireShipSelected(ship);
  }

  void MakeShipTopMost(long mmsi)
  {
    // make the indicated ship appear in front of all the other markers
    MarkerShip? marker = (MarkerShip?)overlayShips.Markers.FirstOrDefault((m) => (m is MarkerShip ms) && ms.IsSelected);
    if (marker == null) return;
    overlayShips.Markers.Remove(marker);
    overlayShips.Markers.Add(marker);
  }

  void UpdateMarker(ShipData ship)
  {
    var position = new PointLatLng(ship.Latitude, ship.Longitude);

    if (ship.Marker is null)
    {
      ship.Marker = new MarkerShip(position, ship.Mmsi, ship.VesselName, ship.HeadingDegrees) //GetMarkerTypeFor(ship.NavigationalStatus))
      {
        ShipType = ship.ShipType,
        Speed = ship.SpeedKnots,
        Course = (float)ship.CourseDegrees,
        NavigationalStatus = ship.NavigationalStatus,
        ToolTipMode = MarkerTooltipMode.OnMouseOver,
      };

      if (shipTypeSelected == AisShipType.All || shipTypeSelected == ship.ShipType)
        overlayShips.Markers.Add(ship.Marker);
    }
    else
    {
      ship.Marker.ShipType = ship.ShipType;
      ship.Marker.Position = position;
      ship.Marker.Speed = ship.SpeedKnots;
      ship.Marker.Course = (float)ship.CourseDegrees;
      ship.Marker.NavigationalStatus = ship.NavigationalStatus;
    }

    ship.Marker.ToolTipText =
        $"\n{ship.VesselName} (MMSI {ship.Mmsi})   Type: {ship.ShipType}\n" +
        $"Speed: {ship.SpeedKnots:0.0} kts   Course: {ship.CourseDegrees:0}°\n" +
        $"{ship.NavigationalStatus.GetDescription()}";
  }

  public void RemoveShips(List<ShipData> ships)
  {
    foreach (var ship in ships)
    {
      if (ship.Marker is not null)
        overlayShips.Markers.Remove(ship.Marker);

      Ships.Remove(ship.Mmsi);
    }
  }

  void InvokeIfNeeded(Action action)
  {
    if (IsDisposed) return;

    if (InvokeRequired)
      BeginInvoke(action);
    else
      action();
  }

  AisShipType GetShipType(int shipType) =>
  shipType switch
  {
    30 => AisShipType.Fishing,
    31 or 32 => AisShipType.Towing,
    33 => AisShipType.DredgingOrUnderwaterOperations,
    35 => AisShipType.Military,
    36 or 37 => AisShipType.PleasureCraft,
    >= 40 and <= 49 => AisShipType.HighSpeedCraft,
    >= 50 and <= 55 => AisShipType.SpecialCraftOrService,
    >= 60 and <= 69 => AisShipType.Passenger,
    >= 70 and <= 79 => AisShipType.Cargo,
    >= 80 and <= 89 => AisShipType.Tanker,
    _ => AisShipType.Unknown,
  };

  AisNavigationalStatus GetNavigationalStatusCode(int code)
  {
    if (code < 0) return AisNavigationalStatus.Unknown;
    if (code > (int)AisNavigationalStatus.Unknown) return AisNavigationalStatus.Unknown;
    AisNavigationalStatus status = (AisNavigationalStatus)code;
    return status;
  }


  #region Events
  public delegate void ShipHandler(ShipData ship);
  public event ShipHandler? ShipDataReceived;
  void FireShipDataReceived(ShipData ship)
  {
    if (InvokeRequired)
      BeginInvoke(FireShipDataReceived, ship);
    else
      ShipDataReceived?.Invoke(ship);
  }

  public event ShipHandler? ShipSelected;
  void FireShipSelected(ShipData ship)
  {
    if (InvokeRequired)
      BeginInvoke(FireShipSelected, ship);
    else
      ShipSelected?.Invoke(ship);
  }

  public delegate void TextHandler(string text);
  public event TextHandler? StatusMessage;
  void FireStatusMessage(string text)
  {
    if (InvokeRequired)
      BeginInvoke(FireStatusMessage, text);
    else
      StatusMessage?.Invoke(text);
  }

  public delegate void ExceptionHandler(Exception ex);
  public event ExceptionHandler? Exception;
  void FireException(Exception ex)
  {
    if (InvokeRequired)
      BeginInvoke(FireException, ex);
    else
      Exception?.Invoke(ex);
  }

  #endregion
}
