# ShipTracker

A C# WinForms desktop app to track ships on a map, based on AIS (Automatic Identification System) data. Developed with .Net 10.

The app uses OpenStreetMap to display a map of the area around Singapore. You can easily change the area displayed by editing a few hardcoded values.
Ships are displayed with colors according to the ship type. Hovering the mouse over a ship will bring up a tooltip with
ship information. The following figure shows ShipTracker in action.

<img width="1072" height="667" alt="image" src="https://github.com/user-attachments/assets/4b5c53b2-a709-42dd-8bc5-472c9cb86e27" />

*Figure 1* - ShipTracker, showing the busy shipping traffic around Singapore.
<br><br>

The table under the map shows details about all the ships being tracked. By clicking one of the colors in the Ship Legend box on the left, you can filter the displayed ships, eliminating all but those of a given type.
The following figure shows the filter set to **Tanker** ships:

<img width="1072" height="667" alt="image" src="https://github.com/user-attachments/assets/36b1b006-88b5-431a-8ce4-205d03b965de" />

*Figure 2* - Showing only ships of one type.
<br><br>

You can click a ship to select it. By doing so the ship turns white with a yellow border. The table under the map will show the row describing the ship.
If ships start overlapping on the map, zoom in with the mouse wheel. Mousing over a ship gives you a tooltip with some details, as shown in the next figure.

<img width="1072" height="667" alt="image" src="https://github.com/user-attachments/assets/ad71ad8e-95a1-4740-bc49-c5d571acb50d" />

*Figure 3* - A tooltip showing ship information.
<br><br>

## The UI Layout

The user interface is made up of three layers of docked controls, as shown in the following figure.

<img width="632" height="461" alt="image" src="https://github.com/user-attachments/assets/b449c30c-cc34-4319-a773-8269720464d7" />


*Figure 4* - The UI as three layers of docked controls.

* *Layer 1*. Shown in red. It lays out the horizontal areas using panels. PanelMiddle has Dock=Fill so it fills all space in the middle of the screen.
* *Layer 2*. Shown in blue. It divides the middle panel into two. The right side contains the panel that hosts the map. It has Dock=Fill.
* *Layer 3*. Show in purple. It handles the map, with Dock=Fill.

In order to achieve the proper layout, the various controls must be added in the correct back-to-front order on the parent form or panel. Items with Dock=Fill must always be added last.
Here is how the controls must be added:

* *Layer 1*. PanelTop, PanelStatusBar, List View, Splitter, PanelMiddle.
* *Layer 2*. PanelLegend, Panel Map.
* *Layer 3*. UserControl ShipTracker.

## How it Works

There are two essential parts to the system:

1. UserControl ShipTracker, which uses a GMapControl to download OpenStreetMap tiles and display the maps with any overlays it contains.
2. AISStream.io, the web service that provides the ship data.

UserControl ShipTracker is derived from GMapControl, which in turn in derived from UserControl, as shown in the next figure.

<img width="128" height="375" alt="image" src="https://github.com/user-attachments/assets/3075fc91-fbee-4e75-aacd-0924b201072f" />

*Figure 5* - The class hierarchy of ShipTracker.

GMapControl is contained in the NuGet package ```ibdg.GMap.NET.WinForms (2.1.8)```. The control handles all the heavy lifting of downloading map tiles, zooming, panning, showing overlays, etc.
When the app starts, ShipTracker calls InitializeMap to download the OpenStreetMap tiles and display the map, as shown in the following pseudo code.

```csharp
public partial class ShipTracker : GMapControl
{
  // the area around Singapore we're interested in
  static readonly double[][] BoundingBoxMalaccaStrait =
  [
    [6.5, 95.0],   // north-west corner
    [1.0, 104.5],  // south-east corner
  ];

  GMapOverlay overlayShips = new("ships");

  public ShipTracker()
  {
    InitializeComponent();
    InitializeMap();
    //...
  }

  void InitializeMap()
  {
    GMaps.Instance.Mode = AccessMode.ServerAndCache;  // download tiles and save in a local cache

    base.MapProvider = GMapProviders.OpenStreetMap;
    base.Position = MalaccaStraitCenter;             // center map
    base.MinZoom = 3;
    base.MaxZoom = 19;
    base.Zoom = 7;
    base.DragButton = MouseButtons.Left;
    base.CanDragMap = true;
    base.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
    base.ShowCenter = false;
    base.IgnoreMarkerOnMouseWheel = true;   // so zooming works when on a ship marker

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
  //...
}
```

*Listing 1* - Initializing the map.

## Handling Ship Data

The AISStream.io service sends periodic ship data over a websocket connection. To get this data you have to first setup the websocket and send a "subscription" indicating what types of data you're interested in, and for what geographic area.
When ship data is received back, it is processed by the creation and placement of a ship marker on the map. When additional data is returned for a ship already on the map, its position and heading are updated. The following figure shows the overall sequence diagram.

<img width="997" height="652" alt="image" src="https://github.com/user-attachments/assets/691774f3-4bb5-4992-8197-9048c4902887" />

*Figure 6* - How the app handles ship data received from AISStream.io.

The following listing shows the connection logic in AisService.

```csharp
public async Task ConnectAsync(string apiKey, double[][][] boundingBoxes, string[] messageTypeFilter)
{
    //...
  webSocket = new ClientWebSocket();
  webSocket.Options.DangerousDeflateOptions = new WebSocketDeflateOptions();

  FireStatusChanged("Connecting to AISStream.io...");
  await webSocket.ConnectAsync(Endpoint, CancellationToken.None).ConfigureAwait(false);

  var subscription = new Dictionary<string, object?>
  {
    ["APIKey"] = apiKey,
    ["BoundingBoxes"] = boundingBoxes,
  };

  var payload = JsonSerializer.SerializeToUtf8Bytes(subscription);

  await webSocket.SendAsync(payload, WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None)
                 .ConfigureAwait(false);

  FireStatusChanged("Server connected");

  receiveLoopCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);
  receiveLoopTask = Task.Run(() => ListenForMessagesAsync(receiveLoopCancellationToken.Token), CancellationToken.None);
}
```
*Listing 2* - Connecting to the AISStream.io web service with a websocket.

The subscription specifies two things:

  1. The API key. This is required to access the service.
  2. The BoundingBoxes. This is an array of rectangles, each defined by its diagonally-opposed corners. We only specify one rectangle.
  3. The MessageTypeFilter. This specifies the message types we want to receive.

There are two types of messages we're interested in:

  1. Static. Carried by ShipStaticData messages. These are sent infrequently and define things like the ship type and its destination.
  2. Dynamic. Carried by PositionReport messages. There are sent frequently and define the ship's position and speed.

Once the websocket is established with AISStream.io, we run a Task called ListenForMessagesAsync to receive messages, as shown in the following listing without the exception handling logic.

```csharp
 async Task ListenForMessagesAsync(CancellationToken token)
 {
   var buffer = new byte[32 * 1024];  // the receive buffer
   using var messageStream = new MemoryStream();

   while (webSocket is { State: WebSocketState.Open } && !token.IsCancellationRequested)
   {
     messageStream.SetLength(0);  // initialize stream
     WebSocketReceiveResult result;
     do
     {
       result = await webSocket.ReceiveAsync(buffer, token).ConfigureAwait(false);  // receive the message
       if (result.MessageType == WebSocketMessageType.Close)
       {
         FireStatusChanged($"Server closed the connection ({result.CloseStatusDescription}).");
         return;
       }
       messageStream.Write(buffer, 0, result.Count);  // copy message to a stream
     } while (!result.EndOfMessage);

     messageStream.Position = 0;
    var envelope = JsonSerializer.Deserialize<AisEnvelope>(messageStream);  // convert message to JSON

    if (envelope is not null)
      FireMessageReceived(envelope);  // notify ShipTracker
   }
 }
```
*Listing 3* - Handling websocket messages sent from AISStream.io.

## JSON Messages Received

When AisService receives a message, it fires a MessageReceived event. The payload consists of a JSON Envelope that looks like this:

```json
{
  "MessageType": <Type>,     // e.g. "PositionReport" or "ShipStaticData"
  "MetaData": {              // provides data about the message
    "MMSI":      <number>,   // 9-digit ship identification number
    "ShipName":  <string>,   // e.g. "OCEAN EXPLORER"
    "latitude":  <number>,   // e.g.  37.7749
    "longitude": <number>,   // e.g. -122.4194
    "time_utc":  <datetime>  // e.g. "2026-09-29 12:30:00.000000000 +0000 UTC"
  },
  "Message": {
    "PositionReport": {...}
     or
    "ShipStaticData": {...}
    }
  }
}
```
*Listing 4* - The JSON message Envelope.

The PositionReport object is sent frequently and contains data about where the ship is. In JSON it looks like this, with some sample data:

```json
  "PositionReport": {
    "MessageID": 1,
    "RepeatIndicator": 0,
    "UserID": 366123450,
    "Valid": true,
    "NavigationalStatus": 0,
    "RateOfTurn": 0,
    "Sog": 12.4,
    "PositionAccuracy": true,
    "Longitude": -122.4194,
    "Latitude": 37.7749,
    "Cog": 180.5,
    "TrueHeading": 181,
    "Timestamp": 35,
    "SpecialManoeuvreIndicator": 0,
    "Spare": 0,
    "Raim": false,
    "CommunicationState": 82431
  }
 ```
*Listing 5* - The JSON PositionReport message.

The ShipStaticData object is sent much less frequently and contains data about the ship type, its destination and more. In JSON it looks like this, with some sample data:

```json
  "ShipStaticData": {
    "MessageID": 5,
    "RepeatIndicator": 0,
    "UserID": 366123450,
    "Valid": true,
    "AisVersion": 0,
    "ImoNumber": 9388053,
    "CallSign": "WDC1234",
    "Name": "OCEAN EXPLORER",
    "Type": 70,
    "Dimension": {
      "A": 100,
      "B": 20,
      "C": 8,
      "D": 8
    },
    "FixType": 1,
    "Eta": {
      "Month": 10,
      "Day": 5,
      "Hour": 14,
      "Minute": 0
    },
    "MaximumDraught": 8.2,
    "Destination": "NEW YORK",
    "Dte": 0,
    "Spare": false
  }
```
*Listing 6* - The JSON ShipStaticData message.

All the JSON data received is deserialized into C# classes for easy processing. This is accomplished in the listing 3 with the code

```csharp var envelope = JsonSerializer.Deserialize<AisEnvelope>(messageStream);```

## Putting Ships on the Map

GMap.NET support overlays to show things on the map. ShipTracker uses an overlay for the rectangle that denotes the area being tracked.
Another overlay is used to hold "markers", one for each ship. When the ShipTracker control handles MessageReceived events, it extracts the relevant ship information and puts a marker on the map at the ship's position.
The following listing shows the code.

```csharp
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
```

  *Listing 7* - The method ShipTracker.UpdateMarker, which adds and updates markers on the map.

## Drawing a Ship Silhouette

Ships are drawn as markers with a given shape and color. Ship markers are handled with a class shockingly called MarkerShip. The following listing shows its rendering logic.

```csharp
public class MarkerShip : GMapMarker
{
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

  //...

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
```
*Listing 8* - The rendering logic of MarkerShip.

To paint ships at the right location and pointing in the right direction, the code uses Graphics transforms. First the Graphics surface is translated so its center is where the ship is. Then the surface is rotated according to the ship heading. Only then is the ship outline drawn. After drawing the ship, the Graphics surface is restored to its original location and orientation.

## Filtering Ships by Type

  Given the clutter of ships around ports or in shipping lanes, it can become hard to see individual ships. The colored boxes on the left of the screen let you remove all the ships except those of a given type.
  The code is pretty simple and looks like this:

```csharp
 public void ShowShipType(AisShipType shipType)
 {
   shipTypeSelected = shipType;

   overlayShips.Markers.Clear();

   List<ShipData> ships = Ships.Values.ToList();

   if (shipType != AisShipType.All)
     ships = Ships.Values.Where(s => s.ShipType == shipType).ToList();

   foreach (var ship in ships)
   {
     if (ship.Marker == null) continue;
     overlayShips.Markers.Add(ship.Marker);
   }
 }
```
*Listing 9* - The method ShipTracker.ShowShipType, which only shows ships of a given type.

## Selecting a Ship

If you want to track a specific ship, you can select it in two ways:

  1. By clicking it on the map.
  2. By double-clicking it in the ListView.

In both cases the ship will turn white and show up in front of all other markers. Here is the basic code:

```csharp
public partial class ShipTracker : GMapControl
{
  //...
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

  void MakeShipTopMost(long mmsi)
  {
    // make the indicated ship appear in front of all the other markers
    MarkerShip? marker = (MarkerShip?)overlayShips.Markers.FirstOrDefault((m) => (m is MarkerShip ms) && ms.IsSelected);
    if (marker == null) return;
    overlayShips.Markers.Remove(marker);
    overlayShips.Markers.Add(marker);
  }
  //...
}
```
*Listing 10* - Selecting a ship on the map.

## Removing Stale Ships

Ships don't broadcast AIS data at all times. If they turn their transmitter off after sending PositionReport messages but before sending a single ShipStaticData, we know the ship exists but we don't know what type it is.
If the ship type remains unknown for more than 20 minutes, we assume the ship is no longer sending AIS data, so we remove the ship from the map. Here is the main code.

```csharp
public partial class MainForm : Form
{
  //...

  void TimerStaleShips_Tick(object sender, EventArgs e)
  {
    RemoveStaleShips();
  }

  void RemoveStaleShips()
  {
    var cutoff = DateTime.UtcNow.AddMinutes(-20);
    var staleShips = shipTracker.Ships.Values
        .Where(s => s.ShipType == AisShipType.Unknown && s.LastPositionUtc < cutoff)
        .ToList();

    shipTracker.RemoveShips(staleShips);
    foreach (var ship in staleShips)
    {
      var item = listViewShips.Items.Cast<ListViewItem>()
          .FirstOrDefault(i => i.Tag is long m && m == ship.Mmsi);
      if (item is not null)
        listViewShips.Items.Remove(item);
    }

    UpdateShipCount();
  }
}
```
*Listing 11* - Removing ships that fail to send ShipStaticData messages.

## Getting an API Key for AISStream.io

To access the web service, you need an API key. Keys are free and can be obtained at https://aisstream.io. The site looks like this:

<img width="1071" height="617" alt="image" src="https://github.com/user-attachments/assets/9933e9d6-194b-4e9a-8cc0-6549c86c3296" />

*Figure 7* - The aisstream.io web site.

Click the **API Keys** button and you'll get the **Credentials and notifications** page, shown in the next figure.

<img width="1071" height="617" alt="image" src="https://github.com/user-attachments/assets/f08374a4-236f-4a83-be7e-8314f245b6c5" />

*Figure 8* - The aisstream.io page providing free keys.

Click the **Create API key** button and you'll get a key, which is a long hex number in string format. Save this string somewhere. At runtime, paste it into 
ShipTracker's API textbox. Since I didn't want to hard-code my API key in the app, I saved it in an Environment variable on my machine.
I called the variable "AISSTREAM_API_KEY". At runtime, the app attempts to read this variable. If found, it saves the value in the API textbox using this code:

```chsarp
public partial class MainForm : Form
{
  public MainForm()
  {
  //...
    if (textBoxApiKey.Text.Length > 0) return;

    // get the AISService key from the environment, so we don't expose the key in code
    textBoxApiKey.Text = Environment.GetEnvironmentVariable("AISSTREAM_API_KEY") ?? string.Empty;
  }
  //...
}
```
*Listing 12* - Reading the API key from an Environment variable.

If the Environment variable isn't found on your machine, you can paste your saved key in the textbox. Or you can put it in the textbox Text property.

## Last Notes

ShipTracker was written with the bare minimum logic to get you started with maritime tracking. There are may ways you might want to improve the app, such as:

* Allowing the user to specify a different geographic area to track.
* Adding the ability to load/save markers to a file.
* Using additional web services to get more information about a ship, its owner, its cargo, etc.
* Tracking a selected ship as it moves outside the original area of interest.

Many thanks to Gemini AI for its help!
