namespace ShipTracker;

public partial class MainForm : Form
{
  public MainForm()
  {
    InitializeComponent();

    shipTracker.ShipDataReceived += ShipTracker_ShipDataReceived;
    shipTracker.ShipSelected += ShipTracker_ShipSelected;
    shipTracker.StatusMessage += ShipTracker_StatusMessage;
    shipTracker.Exception += ShipTracker_Exception;

    // get the AISService key from the environment, so we don't expose the key in code
    textBoxApiKey.Text = Environment.GetEnvironmentVariable("AISSTREAM_API_KEY") ?? string.Empty;
  }

  void ShipTracker_ShipDataReceived(ShipData ship)
  {
    UpdateListRow(ship);
  }

  void ShipTracker_ShipSelected(ShipData ship)
  {
    var item = listViewShips.Items
      .Cast<ListViewItem>()
      .FirstOrDefault(lvi => lvi.Tag is long mmsi && mmsi == ship.Mmsi);

    if (item == null) return;

    item.Selected = true;
    item.EnsureVisible();
  }

  void ShipTracker_StatusMessage(string text)
  {
    labelStatus.Text = text;
  }

  void ShipTracker_Exception(Exception ex)
  {
    labelMessage.Text = $"Error: {ex.Message}";
  }

  async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    await DisconnectAsync();
  }

  async void ButtonConnect_Click(object sender, EventArgs e)
  {
    await ConnectAsync();
  }

  async void ButtonDisconnect_Click(object sender, EventArgs e)
  {
    await DisconnectAsync();
  }

  void TimerStaleShips_Tick(object sender, EventArgs e)
  {
    RemoveStaleShips();
  }

  async Task ConnectAsync()
  {
    var apiKey = textBoxApiKey.Text.Trim();
    if (string.IsNullOrWhiteSpace(apiKey))
    {
      MessageBox.Show(
          this,
          "Enter your AISStream.io API key. You can get one free at aisstream.io/account).",
          "API key required",
          MessageBoxButtons.OK,
          MessageBoxIcon.Warning);
      return;
    }

    buttonConnect.Enabled = false;

    try
    {
      shipTracker.ConnectAsync(apiKey);

      buttonDisconnect.Enabled = true;
      textBoxApiKey.Enabled = false;
    }
    catch (Exception ex)
    {
      buttonConnect.Enabled = true;
      MessageBox.Show(this, $"Could not connect: {ex.Message}", "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }

  async Task DisconnectAsync()
  {
    buttonDisconnect.Enabled = false;
    shipTracker.DisconnectAsync();
    buttonConnect.Enabled = true;
    textBoxApiKey.Enabled = true;
  }

  void UpdateListRow(ShipData ship)
  {
    var item = listViewShips.Items
      .Cast<ListViewItem>()
      .FirstOrDefault(lvi => lvi.Tag is long mmsi && mmsi == ship.Mmsi);

    if (item == null)
    {
      item = new ListViewItem(ship.Mmsi.ToString()) { Tag = ship.Mmsi };
      item.SubItems.Add("");
      item.SubItems.Add("");
      item.SubItems.Add("");
      item.SubItems.Add("");
      item.SubItems.Add("");
      item.SubItems.Add("");
      item.SubItems.Add("");
      item.SubItems.Add("");
      item.SubItems.Add("");
      listViewShips.Items.Add(item);
    }

    item.SubItems[1].Text = ship.VesselName;
    item.SubItems[2].Text = ship.ShipType.GetDescription();
    item.SubItems[3].Text = ship.Latitude.ToString("0.00000");
    item.SubItems[4].Text = ship.Longitude.ToString("0.00000");
    item.SubItems[5].Text = ship.SpeedKnots.ToString("0.0");
    item.SubItems[6].Text = ship.CourseDegrees.ToString("0");
    item.SubItems[7].Text = ship.NavigationalStatus.GetDescription();
    item.SubItems[8].Text = ship.Destination;
    item.SubItems[9].Text = ship.LastPositionUtc == default
        ? string.Empty
        : ship.LastPositionUtc.ToLocalTime().ToString("HH:mm:ss");

    UpdateShipCount();
  }

  void ListViewShips_DoubleClick(object? sender, EventArgs e)
  {
    if (listViewShips.SelectedItems.Count == 0) return;
    if (listViewShips.SelectedItems[0].Tag is not long mmsi) return;

    shipTracker.SelectShip(mmsi);
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

  void labelShipColor_Click(object sender, EventArgs e)
  {
    if (sender is not Label label) return;
    if (label.Tag is not string tag) return;         // e.g. labelFishingColor

    string labeNameWithoutColor = label.Name[..^5];  // e.g. labelFishing

    // put a box around the label showing the ShipType, e.g. labelFishing
    foreach (Label l in panelLegend.Controls.OfType<Label>())
    {
      if (l.Name.EndsWith("Color")) continue;
      l.BorderStyle = l.Name == labeNameWithoutColor ? BorderStyle.FixedSingle : BorderStyle.None;
    }

    // show only ships of the selected type
    if (!Enum.TryParse<AisShipType>(tag, out var type)) return;

    shipTracker.ShowShipType(type);
    UpdateShipCount();
  }

  void UpdateShipCount()
  {
    labelShipsShown.Text = $"Ships shown: {shipTracker.ShipsShown}";
    labelShipsFound.Text = $"Ships found: {shipTracker.Ships.Count}";
  }
}
