namespace ShipTracker;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }
    private System.Windows.Forms.Panel panelTop = null!;
    private System.Windows.Forms.Label apiKeyLabel = null!;
    private System.Windows.Forms.TextBox textBoxApiKey = null!;
    private System.Windows.Forms.Button buttonConnect = null!;
    private System.Windows.Forms.Button buttonDisconnect = null!;
    private System.Windows.Forms.ListView listViewShips = null!;
    private System.Windows.Forms.ColumnHeader columnMmsi = null!;
    private System.Windows.Forms.ColumnHeader columName = null!;
    private System.Windows.Forms.ColumnHeader columLatitude = null!;
    private System.Windows.Forms.ColumnHeader columLongitude = null!;
    private System.Windows.Forms.ColumnHeader columSpeed = null!;
    private System.Windows.Forms.ColumnHeader columCourse = null!;
    private System.Windows.Forms.ColumnHeader columStatus = null!;
    private System.Windows.Forms.ColumnHeader columDestination = null!;
    private System.Windows.Forms.ColumnHeader columUpdated = null!;

  private void InitializeComponent()
  {
    components = new System.ComponentModel.Container();
    panelTop = new Panel();
    buttonDisconnect = new Button();
    buttonConnect = new Button();
    textBoxApiKey = new TextBox();
    apiKeyLabel = new Label();
    listViewShips = new ListView();
    columnMmsi = new ColumnHeader();
    columName = new ColumnHeader();
    columType = new ColumnHeader();
    columLatitude = new ColumnHeader();
    columLongitude = new ColumnHeader();
    columSpeed = new ColumnHeader();
    columCourse = new ColumnHeader();
    columStatus = new ColumnHeader();
    columDestination = new ColumnHeader();
    columUpdated = new ColumnHeader();
    panelStatusBar = new Panel();
    labelMessage = new Label();
    labelStatus = new Label();
    labelShipsShown = new Label();
    labelShipsFound = new Label();
    panelMiddle = new Panel();
    splitter1 = new Splitter();
    panelMap = new Panel();
    shipTracker = new ShipTracker();
    panelLegend = new Panel();
    labelAllTypesColor = new Label();
    labelAllTypes = new Label();
    labelTowing = new Label();
    labelTowingColor = new Label();
    labelUnknownColor = new Label();
    labelUnknown = new Label();
    labelTankerColor = new Label();
    labelTanker = new Label();
    labelCargoColor = new Label();
    labelCargo = new Label();
    labelPassengerColor = new Label();
    labelPassenger = new Label();
    labelSpecialColor = new Label();
    labelSpecial = new Label();
    labelHighSpeedColor = new Label();
    labelHighSpeed = new Label();
    labelPleasureColor = new Label();
    labelMilitaryColor = new Label();
    labelPleasure = new Label();
    labelMilitary = new Label();
    labelWorkBoat = new Label();
    labelWorkBoatColor = new Label();
    labelFishing = new Label();
    labelFishingColor = new Label();
    label1 = new Label();
    timerStaleShips = new System.Windows.Forms.Timer(components);
    toolTip = new ToolTip(components);
    panelTop.SuspendLayout();
    panelStatusBar.SuspendLayout();
    panelMiddle.SuspendLayout();
    panelMap.SuspendLayout();
    panelLegend.SuspendLayout();
    SuspendLayout();
    // 
    // panelTop
    // 
    panelTop.BackColor = SystemColors.Control;
    panelTop.BorderStyle = BorderStyle.FixedSingle;
    panelTop.Controls.Add(buttonDisconnect);
    panelTop.Controls.Add(buttonConnect);
    panelTop.Controls.Add(textBoxApiKey);
    panelTop.Controls.Add(apiKeyLabel);
    panelTop.Dock = DockStyle.Top;
    panelTop.Location = new Point(0, 0);
    panelTop.Name = "panelTop";
    panelTop.Padding = new Padding(12, 10, 12, 10);
    panelTop.Size = new Size(1070, 51);
    panelTop.TabIndex = 0;
    // 
    // buttonDisconnect
    // 
    buttonDisconnect.Enabled = false;
    buttonDisconnect.Location = new Point(584, 11);
    buttonDisconnect.Name = "buttonDisconnect";
    buttonDisconnect.Size = new Size(85, 26);
    buttonDisconnect.TabIndex = 1;
    buttonDisconnect.Text = "Disconnect";
    toolTip.SetToolTip(buttonDisconnect, "Disconnect from service");
    buttonDisconnect.UseVisualStyleBackColor = true;
    buttonDisconnect.Click += ButtonDisconnect_Click;
    // 
    // buttonConnect
    // 
    buttonConnect.Location = new Point(492, 11);
    buttonConnect.Name = "buttonConnect";
    buttonConnect.Size = new Size(86, 26);
    buttonConnect.TabIndex = 0;
    buttonConnect.Text = "Connect";
    toolTip.SetToolTip(buttonConnect, "Connect to AISStream.io service");
    buttonConnect.UseVisualStyleBackColor = true;
    buttonConnect.Click += ButtonConnect_Click;
    // 
    // textBoxApiKey
    // 
    textBoxApiKey.Location = new Point(124, 13);
    textBoxApiKey.Name = "textBoxApiKey";
    textBoxApiKey.PlaceholderText = "Paste your AISStream.io API key here";
    textBoxApiKey.Size = new Size(320, 23);
    textBoxApiKey.TabIndex = 2;
    toolTip.SetToolTip(textBoxApiKey, "This app checks the Environment variable \"AISSTREAM_API_KEY\" for your API key. \r\nIf one isn't found, you can enter the key here.");
    textBoxApiKey.UseSystemPasswordChar = true;
    // 
    // apiKeyLabel
    // 
    apiKeyLabel.AutoSize = true;
    apiKeyLabel.ForeColor = SystemColors.ControlText;
    apiKeyLabel.Location = new Point(12, 16);
    apiKeyLabel.Name = "apiKeyLabel";
    apiKeyLabel.Size = new Size(106, 15);
    apiKeyLabel.TabIndex = 0;
    apiKeyLabel.Text = "AISStream API key:";
    // 
    // listViewShips
    // 
    listViewShips.Columns.AddRange(new ColumnHeader[] { columnMmsi, columName, columType, columLatitude, columLongitude, columSpeed, columCourse, columStatus, columDestination, columUpdated });
    listViewShips.Dock = DockStyle.Bottom;
    listViewShips.FullRowSelect = true;
    listViewShips.GridLines = true;
    listViewShips.Location = new Point(0, 412);
    listViewShips.MultiSelect = false;
    listViewShips.Name = "listViewShips";
    listViewShips.Size = new Size(1070, 147);
    listViewShips.TabIndex = 0;
    listViewShips.UseCompatibleStateImageBehavior = false;
    listViewShips.View = View.Details;
    listViewShips.DoubleClick += ListViewShips_DoubleClick;
    // 
    // columnMmsi
    // 
    columnMmsi.Text = "MMSI";
    columnMmsi.Width = 85;
    // 
    // columName
    // 
    columName.Text = "Ship name";
    columName.Width = 140;
    // 
    // columType
    // 
    columType.Text = "Type";
    columType.Width = 90;
    // 
    // columLatitude
    // 
    columLatitude.Text = "Latitude";
    columLatitude.Width = 75;
    // 
    // columLongitude
    // 
    columLongitude.Text = "Longitude";
    columLongitude.Width = 75;
    // 
    // columSpeed
    // 
    columSpeed.Text = "Speed (kts)";
    columSpeed.Width = 75;
    // 
    // columCourse
    // 
    columCourse.Text = "Course";
    // 
    // columStatus
    // 
    columStatus.Text = "Status";
    columStatus.Width = 150;
    // 
    // columDestination
    // 
    columDestination.Text = "Destination";
    columDestination.Width = 120;
    // 
    // columUpdated
    // 
    columUpdated.Text = "Updated";
    columUpdated.Width = 75;
    // 
    // panelStatusBar
    // 
    panelStatusBar.BorderStyle = BorderStyle.Fixed3D;
    panelStatusBar.Controls.Add(labelMessage);
    panelStatusBar.Controls.Add(labelStatus);
    panelStatusBar.Controls.Add(labelShipsShown);
    panelStatusBar.Controls.Add(labelShipsFound);
    panelStatusBar.Dock = DockStyle.Bottom;
    panelStatusBar.Location = new Point(0, 610);
    panelStatusBar.Name = "panelStatusBar";
    panelStatusBar.Size = new Size(1070, 25);
    panelStatusBar.TabIndex = 2;
    // 
    // labelMessage
    // 
    labelMessage.Location = new Point(0, 0);
    labelMessage.Name = "labelMessage";
    labelMessage.Size = new Size(443, 21);
    labelMessage.TabIndex = 2;
    labelMessage.Text = "Click a color in the Ship Legend to show only that type";
    labelMessage.TextAlign = ContentAlignment.MiddleLeft;
    // 
    // labelStatus
    // 
    labelStatus.AutoEllipsis = true;
    labelStatus.Dock = DockStyle.Right;
    labelStatus.Location = new Point(491, 0);
    labelStatus.Name = "labelStatus";
    labelStatus.Size = new Size(314, 21);
    labelStatus.TabIndex = 1;
    labelStatus.Text = "Server disconnected";
    labelStatus.TextAlign = ContentAlignment.MiddleLeft;
    // 
    // labelShipsShown
    // 
    labelShipsShown.Dock = DockStyle.Right;
    labelShipsShown.Location = new Point(805, 0);
    labelShipsShown.Name = "labelShipsShown";
    labelShipsShown.Size = new Size(140, 21);
    labelShipsShown.TabIndex = 3;
    labelShipsShown.Text = "Ships Shown: 0";
    labelShipsShown.TextAlign = ContentAlignment.MiddleLeft;
    // 
    // labelShipsFound
    // 
    labelShipsFound.Dock = DockStyle.Right;
    labelShipsFound.Location = new Point(945, 0);
    labelShipsFound.Name = "labelShipsFound";
    labelShipsFound.Size = new Size(121, 21);
    labelShipsFound.TabIndex = 0;
    labelShipsFound.Text = "Ships Found: 0";
    labelShipsFound.TextAlign = ContentAlignment.MiddleLeft;
    // 
    // panelMiddle
    // 
    panelMiddle.Controls.Add(splitter1);
    panelMiddle.Controls.Add(panelMap);
    panelMiddle.Controls.Add(listViewShips);
    panelMiddle.Dock = DockStyle.Fill;
    panelMiddle.Location = new Point(0, 51);
    panelMiddle.Name = "panelMiddle";
    panelMiddle.Size = new Size(1070, 559);
    panelMiddle.TabIndex = 3;
    // 
    // splitter1
    // 
    splitter1.Dock = DockStyle.Bottom;
    splitter1.Location = new Point(0, 408);
    splitter1.Name = "splitter1";
    splitter1.Size = new Size(1070, 4);
    splitter1.TabIndex = 14;
    splitter1.TabStop = false;
    // 
    // panelMap
    // 
    panelMap.Controls.Add(shipTracker);
    panelMap.Controls.Add(panelLegend);
    panelMap.Dock = DockStyle.Fill;
    panelMap.Location = new Point(0, 0);
    panelMap.Name = "panelMap";
    panelMap.Size = new Size(1070, 412);
    panelMap.TabIndex = 13;
    // 
    // shipTracker
    // 
    shipTracker.Bearing = 0F;
    shipTracker.Dock = DockStyle.Fill;
    shipTracker.EmptyTileColor = Color.Navy;
    shipTracker.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
    shipTracker.Location = new Point(110, 0);
    shipTracker.MaxZoom = 19;
    shipTracker.MinZoom = 3;
    shipTracker.MouseWheelZoomEnabled = true;
    shipTracker.Name = "shipTracker";
    shipTracker.RetryLoadTile = 0;
    shipTracker.SelectedAreaFillColor = Color.FromArgb(33, 65, 105, 225);
    shipTracker.ShowTileGridLines = false;
    shipTracker.Size = new Size(960, 412);
    shipTracker.TabIndex = 12;
    shipTracker.Zoom = 7D;
    // 
    // panelLegend
    // 
    panelLegend.BorderStyle = BorderStyle.FixedSingle;
    panelLegend.Controls.Add(labelAllTypesColor);
    panelLegend.Controls.Add(labelAllTypes);
    panelLegend.Controls.Add(labelTowing);
    panelLegend.Controls.Add(labelTowingColor);
    panelLegend.Controls.Add(labelUnknownColor);
    panelLegend.Controls.Add(labelUnknown);
    panelLegend.Controls.Add(labelTankerColor);
    panelLegend.Controls.Add(labelTanker);
    panelLegend.Controls.Add(labelCargoColor);
    panelLegend.Controls.Add(labelCargo);
    panelLegend.Controls.Add(labelPassengerColor);
    panelLegend.Controls.Add(labelPassenger);
    panelLegend.Controls.Add(labelSpecialColor);
    panelLegend.Controls.Add(labelSpecial);
    panelLegend.Controls.Add(labelHighSpeedColor);
    panelLegend.Controls.Add(labelHighSpeed);
    panelLegend.Controls.Add(labelPleasureColor);
    panelLegend.Controls.Add(labelMilitaryColor);
    panelLegend.Controls.Add(labelPleasure);
    panelLegend.Controls.Add(labelMilitary);
    panelLegend.Controls.Add(labelWorkBoat);
    panelLegend.Controls.Add(labelWorkBoatColor);
    panelLegend.Controls.Add(labelFishing);
    panelLegend.Controls.Add(labelFishingColor);
    panelLegend.Controls.Add(label1);
    panelLegend.Dock = DockStyle.Left;
    panelLegend.Location = new Point(0, 0);
    panelLegend.Name = "panelLegend";
    panelLegend.Size = new Size(110, 412);
    panelLegend.TabIndex = 11;
    // 
    // labelAllTypesColor
    // 
    labelAllTypesColor.BackColor = Color.White;
    labelAllTypesColor.BorderStyle = BorderStyle.FixedSingle;
    labelAllTypesColor.Location = new Point(12, 342);
    labelAllTypesColor.Name = "labelAllTypesColor";
    labelAllTypesColor.Size = new Size(20, 19);
    labelAllTypesColor.TabIndex = 26;
    labelAllTypesColor.Tag = "All";
    toolTip.SetToolTip(labelAllTypesColor, "Unknown. Ship types are reported infrequently, typically every 5-20 minutes.");
    labelAllTypesColor.Click += labelShipColor_Click;
    // 
    // labelAllTypes
    // 
    labelAllTypes.AutoSize = true;
    labelAllTypes.BorderStyle = BorderStyle.FixedSingle;
    labelAllTypes.ForeColor = Color.Teal;
    labelAllTypes.Location = new Point(35, 344);
    labelAllTypes.Name = "labelAllTypes";
    labelAllTypes.Size = new Size(23, 17);
    labelAllTypes.TabIndex = 25;
    labelAllTypes.Tag = "All";
    labelAllTypes.Text = "All";
    toolTip.SetToolTip(labelAllTypes, "Unknown. Ship types are reported infrequently, typically every 5-20 minutes.");
    // 
    // labelTowing
    // 
    labelTowing.AutoSize = true;
    labelTowing.ForeColor = Color.Teal;
    labelTowing.Location = new Point(35, 64);
    labelTowing.Name = "labelTowing";
    labelTowing.Size = new Size(45, 15);
    labelTowing.TabIndex = 24;
    labelTowing.Tag = "Towing";
    labelTowing.Text = "Towing";
    toolTip.SetToolTip(labelTowing, "Tug boat");
    // 
    // labelTowingColor
    // 
    labelTowingColor.BackColor = Color.Yellow;
    labelTowingColor.BorderStyle = BorderStyle.FixedSingle;
    labelTowingColor.Location = new Point(12, 62);
    labelTowingColor.Name = "labelTowingColor";
    labelTowingColor.Size = new Size(20, 19);
    labelTowingColor.TabIndex = 23;
    labelTowingColor.Tag = "Towing";
    toolTip.SetToolTip(labelTowingColor, "Tug boat");
    labelTowingColor.Click += labelShipColor_Click;
    // 
    // labelUnknownColor
    // 
    labelUnknownColor.BackColor = Color.LightGray;
    labelUnknownColor.BorderStyle = BorderStyle.FixedSingle;
    labelUnknownColor.Location = new Point(12, 314);
    labelUnknownColor.Name = "labelUnknownColor";
    labelUnknownColor.Size = new Size(20, 19);
    labelUnknownColor.TabIndex = 22;
    labelUnknownColor.Tag = "Unknown";
    toolTip.SetToolTip(labelUnknownColor, "Unknown. Ship types are reported infrequently, typically every 5-20 minutes.");
    labelUnknownColor.Click += labelShipColor_Click;
    // 
    // labelUnknown
    // 
    labelUnknown.AutoSize = true;
    labelUnknown.ForeColor = Color.Teal;
    labelUnknown.Location = new Point(35, 316);
    labelUnknown.Name = "labelUnknown";
    labelUnknown.Size = new Size(58, 15);
    labelUnknown.TabIndex = 21;
    labelUnknown.Tag = "Unknown";
    labelUnknown.Text = "Unknown";
    toolTip.SetToolTip(labelUnknown, "Unknown. Ship types are reported infrequently, typically every 5-20 minutes.");
    // 
    // labelTankerColor
    // 
    labelTankerColor.BackColor = Color.Red;
    labelTankerColor.BorderStyle = BorderStyle.FixedSingle;
    labelTankerColor.Location = new Point(12, 286);
    labelTankerColor.Name = "labelTankerColor";
    labelTankerColor.Size = new Size(20, 19);
    labelTankerColor.TabIndex = 20;
    labelTankerColor.Tag = "Tanker";
    toolTip.SetToolTip(labelTankerColor, "Tanker ship");
    labelTankerColor.Click += labelShipColor_Click;
    // 
    // labelTanker
    // 
    labelTanker.AutoSize = true;
    labelTanker.ForeColor = Color.Teal;
    labelTanker.Location = new Point(35, 289);
    labelTanker.Name = "labelTanker";
    labelTanker.Size = new Size(41, 15);
    labelTanker.TabIndex = 19;
    labelTanker.Tag = "Tanker";
    labelTanker.Text = "Tanker";
    toolTip.SetToolTip(labelTanker, "Tanker ship");
    // 
    // labelCargoColor
    // 
    labelCargoColor.BackColor = Color.Orange;
    labelCargoColor.BorderStyle = BorderStyle.FixedSingle;
    labelCargoColor.Location = new Point(12, 258);
    labelCargoColor.Name = "labelCargoColor";
    labelCargoColor.Size = new Size(20, 19);
    labelCargoColor.TabIndex = 18;
    labelCargoColor.Tag = "Cargo";
    toolTip.SetToolTip(labelCargoColor, "Cargo ship");
    labelCargoColor.Click += labelShipColor_Click;
    // 
    // labelCargo
    // 
    labelCargo.AutoSize = true;
    labelCargo.ForeColor = Color.Teal;
    labelCargo.Location = new Point(35, 261);
    labelCargo.Name = "labelCargo";
    labelCargo.Size = new Size(39, 15);
    labelCargo.TabIndex = 17;
    labelCargo.Tag = "Cargo";
    labelCargo.Text = "Cargo";
    toolTip.SetToolTip(labelCargo, "Cargo ship");
    // 
    // labelPassengerColor
    // 
    labelPassengerColor.BackColor = Color.Blue;
    labelPassengerColor.BorderStyle = BorderStyle.FixedSingle;
    labelPassengerColor.Location = new Point(12, 230);
    labelPassengerColor.Name = "labelPassengerColor";
    labelPassengerColor.Size = new Size(20, 19);
    labelPassengerColor.TabIndex = 16;
    labelPassengerColor.Tag = "Passenger";
    toolTip.SetToolTip(labelPassengerColor, "Cruise ship or ferry");
    labelPassengerColor.Click += labelShipColor_Click;
    // 
    // labelPassenger
    // 
    labelPassenger.AutoSize = true;
    labelPassenger.ForeColor = Color.Teal;
    labelPassenger.Location = new Point(35, 233);
    labelPassenger.Name = "labelPassenger";
    labelPassenger.Size = new Size(60, 15);
    labelPassenger.TabIndex = 15;
    labelPassenger.Tag = "Passenger";
    labelPassenger.Text = "Passenger";
    toolTip.SetToolTip(labelPassenger, "Cruise ship or ferry");
    // 
    // labelSpecialColor
    // 
    labelSpecialColor.BackColor = Color.Teal;
    labelSpecialColor.BorderStyle = BorderStyle.FixedSingle;
    labelSpecialColor.Location = new Point(12, 202);
    labelSpecialColor.Name = "labelSpecialColor";
    labelSpecialColor.Size = new Size(20, 19);
    labelSpecialColor.TabIndex = 14;
    labelSpecialColor.Tag = "HighSpeedCraft";
    toolTip.SetToolTip(labelSpecialColor, "Search and rescue, port tender or law enforcement");
    labelSpecialColor.Click += labelShipColor_Click;
    // 
    // labelSpecial
    // 
    labelSpecial.AutoSize = true;
    labelSpecial.ForeColor = Color.Teal;
    labelSpecial.Location = new Point(35, 204);
    labelSpecial.Name = "labelSpecial";
    labelSpecial.Size = new Size(44, 15);
    labelSpecial.TabIndex = 13;
    labelSpecial.Tag = "HighSpeedCraft";
    labelSpecial.Text = "Special";
    toolTip.SetToolTip(labelSpecial, "Search and rescue, port tender or law enforcement");
    // 
    // labelHighSpeedColor
    // 
    labelHighSpeedColor.BackColor = Color.LightBlue;
    labelHighSpeedColor.BorderStyle = BorderStyle.FixedSingle;
    labelHighSpeedColor.Location = new Point(12, 174);
    labelHighSpeedColor.Name = "labelHighSpeedColor";
    labelHighSpeedColor.Size = new Size(20, 19);
    labelHighSpeedColor.TabIndex = 12;
    labelHighSpeedColor.Tag = "HighSpeedCraft";
    toolTip.SetToolTip(labelHighSpeedColor, "Speed boat");
    labelHighSpeedColor.Click += labelShipColor_Click;
    // 
    // labelHighSpeed
    // 
    labelHighSpeed.AutoSize = true;
    labelHighSpeed.ForeColor = Color.Teal;
    labelHighSpeed.Location = new Point(35, 176);
    labelHighSpeed.Name = "labelHighSpeed";
    labelHighSpeed.Size = new Size(70, 15);
    labelHighSpeed.TabIndex = 11;
    labelHighSpeed.Tag = "HighSpeedCraft";
    labelHighSpeed.Text = "High speed ";
    toolTip.SetToolTip(labelHighSpeed, "Speed boat");
    // 
    // labelPleasureColor
    // 
    labelPleasureColor.BackColor = Color.LightGreen;
    labelPleasureColor.BorderStyle = BorderStyle.FixedSingle;
    labelPleasureColor.Location = new Point(12, 146);
    labelPleasureColor.Name = "labelPleasureColor";
    labelPleasureColor.Size = new Size(20, 19);
    labelPleasureColor.TabIndex = 10;
    labelPleasureColor.Tag = "PleasureCraft";
    toolTip.SetToolTip(labelPleasureColor, "Yacht or pleasure boat");
    labelPleasureColor.Click += labelShipColor_Click;
    // 
    // labelMilitaryColor
    // 
    labelMilitaryColor.BackColor = Color.Black;
    labelMilitaryColor.BorderStyle = BorderStyle.FixedSingle;
    labelMilitaryColor.Location = new Point(12, 118);
    labelMilitaryColor.Name = "labelMilitaryColor";
    labelMilitaryColor.Size = new Size(20, 19);
    labelMilitaryColor.TabIndex = 9;
    labelMilitaryColor.Tag = "Military";
    toolTip.SetToolTip(labelMilitaryColor, "Warship");
    labelMilitaryColor.Click += labelShipColor_Click;
    // 
    // labelPleasure
    // 
    labelPleasure.AutoSize = true;
    labelPleasure.ForeColor = Color.Teal;
    labelPleasure.Location = new Point(35, 149);
    labelPleasure.Name = "labelPleasure";
    labelPleasure.Size = new Size(51, 15);
    labelPleasure.TabIndex = 8;
    labelPleasure.Tag = "PleasureCraft";
    labelPleasure.Text = "Pleasure";
    toolTip.SetToolTip(labelPleasure, "Yacht or pleasure boat");
    // 
    // labelMilitary
    // 
    labelMilitary.AutoSize = true;
    labelMilitary.ForeColor = Color.Teal;
    labelMilitary.Location = new Point(35, 121);
    labelMilitary.Name = "labelMilitary";
    labelMilitary.Size = new Size(47, 15);
    labelMilitary.TabIndex = 6;
    labelMilitary.Tag = "Military";
    labelMilitary.Text = "Military";
    toolTip.SetToolTip(labelMilitary, "Warship");
    // 
    // labelWorkBoat
    // 
    labelWorkBoat.AutoSize = true;
    labelWorkBoat.ForeColor = Color.Teal;
    labelWorkBoat.Location = new Point(35, 93);
    labelWorkBoat.Name = "labelWorkBoat";
    labelWorkBoat.Size = new Size(62, 15);
    labelWorkBoat.TabIndex = 4;
    labelWorkBoat.Tag = "DredgingOrUnderwaterOperations";
    labelWorkBoat.Text = "Work boat";
    toolTip.SetToolTip(labelWorkBoat, "Dredging or underwater operations ");
    // 
    // labelWorkBoatColor
    // 
    labelWorkBoatColor.BackColor = Color.Pink;
    labelWorkBoatColor.BorderStyle = BorderStyle.FixedSingle;
    labelWorkBoatColor.Location = new Point(12, 90);
    labelWorkBoatColor.Name = "labelWorkBoatColor";
    labelWorkBoatColor.Size = new Size(20, 19);
    labelWorkBoatColor.TabIndex = 3;
    labelWorkBoatColor.Tag = "DredgingOrUnderwaterOperations";
    toolTip.SetToolTip(labelWorkBoatColor, "Dredging or underwater operations ");
    labelWorkBoatColor.Click += labelShipColor_Click;
    // 
    // labelFishing
    // 
    labelFishing.AutoSize = true;
    labelFishing.ForeColor = Color.Teal;
    labelFishing.Location = new Point(35, 36);
    labelFishing.Name = "labelFishing";
    labelFishing.Size = new Size(45, 15);
    labelFishing.TabIndex = 2;
    labelFishing.Tag = "Fishing";
    labelFishing.Text = "Fishing";
    toolTip.SetToolTip(labelFishing, "Commercial fishing");
    // 
    // labelFishingColor
    // 
    labelFishingColor.BackColor = Color.Green;
    labelFishingColor.BorderStyle = BorderStyle.FixedSingle;
    labelFishingColor.Location = new Point(12, 34);
    labelFishingColor.Name = "labelFishingColor";
    labelFishingColor.Size = new Size(20, 19);
    labelFishingColor.TabIndex = 1;
    labelFishingColor.Tag = "Fishing";
    toolTip.SetToolTip(labelFishingColor, "Commercial fishing");
    labelFishingColor.Click += labelShipColor_Click;
    // 
    // label1
    // 
    label1.AutoSize = true;
    label1.ForeColor = Color.Teal;
    label1.Location = new Point(12, 6);
    label1.Name = "label1";
    label1.Size = new Size(72, 15);
    label1.TabIndex = 0;
    label1.Text = "Ship Legend";
    // 
    // timerStaleShips
    // 
    timerStaleShips.Interval = 30000;
    timerStaleShips.Tick += TimerStaleShips_Tick;
    // 
    // toolTip
    // 
    toolTip.AutomaticDelay = 50;
    toolTip.AutoPopDelay = 5000;
    toolTip.InitialDelay = 50;
    toolTip.ReshowDelay = 10;
    // 
    // MainForm
    // 
    AutoScaleDimensions = new SizeF(7F, 15F);
    AutoScaleMode = AutoScaleMode.Font;
    ClientSize = new Size(1070, 635);
    Controls.Add(panelMiddle);
    Controls.Add(panelStatusBar);
    Controls.Add(panelTop);
    MinimumSize = new Size(1000, 600);
    Name = "MainForm";
    StartPosition = FormStartPosition.CenterScreen;
    Text = "ShipTracker - Singapore and the Strait of Malacca";
    FormClosing += MainForm_FormClosing;
    panelTop.ResumeLayout(false);
    panelTop.PerformLayout();
    panelStatusBar.ResumeLayout(false);
    panelMiddle.ResumeLayout(false);
    panelMap.ResumeLayout(false);
    panelLegend.ResumeLayout(false);
    panelLegend.PerformLayout();
    ResumeLayout(false);
  }

  private Panel panelStatusBar;
  private Label labelShipsFound;
  private Label labelMessage;
  private Label labelStatus;
  private Panel panelMiddle;
  private ColumnHeader columType;
  private System.Windows.Forms.Timer timerStaleShips;
  private ToolTip toolTip;
  private Panel panelLegend;
  private Label labelTowing;
  private Label labelTowingColor;
  private Label labelUnknownColor;
  private Label labelUnknown;
  private Label labelTankerColor;
  private Label labelTanker;
  private Label labelCargoColor;
  private Label labelCargo;
  private Label labelPassengerColor;
  private Label labelPassenger;
  private Label labelSpecialColor;
  private Label labelSpecial;
  private Label labelHighSpeedColor;
  private Label labelHighSpeed;
  private Label labelPleasureColor;
  private Label labelMilitaryColor;
  private Label labelPleasure;
  private Label labelMilitary;
  private Label labelWorkBoat;
  private Label labelWorkBoatColor;
  private Label labelFishing;
  private Label labelFishingColor;
  private Label label1;
  private ShipTracker shipTracker;
  private Splitter splitter1;
  private Panel panelMap;
  private Label labelAllTypesColor;
  private Label labelAllTypes;
  private Label labelShipsShown;
}
