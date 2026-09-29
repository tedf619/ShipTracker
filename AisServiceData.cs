using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShipTracker;

/// <summary>
/// Top level envelope AISStream.io sends for every message:
/// </summary>
public sealed class AisEnvelope
{
    [JsonPropertyName("MessageType")]
    public string MessageType { get; set; } = string.Empty;

    [JsonPropertyName("MetaData")]
    public AisMetaData? MetaData { get; set; }

    [JsonPropertyName("Message")]
    public JsonElement Message { get; set; }
}

/// <summary>
/// Data attached to every AIS message (MMSI, ship name, last known position).
/// Not every field is populated for every message type.
/// </summary>
public sealed class AisMetaData
{
    [JsonPropertyName("MMSI")]
    public long Mmsi { get; set; }

    [JsonPropertyName("ShipName")]
    public string? ShipName { get; set; }

    [JsonPropertyName("Latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("Longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("time_utc")]
    public string? TimeUtc { get; set; }
}

/// <summary>
/// Decoded ITU-R M.1371 position report.
/// </summary>
public sealed class PositionReport
{
    [JsonPropertyName("UserID")]
    public long UserId { get; set; }

    [JsonPropertyName("NavigationalStatus")]
    public int NavigationalStatus { get; set; }

    [JsonPropertyName("Sog")]
    public double Sog { get; set; }  // speed over ground

    [JsonPropertyName("Cog")]
    public double Cog { get; set; }  // course over ground

    [JsonPropertyName("TrueHeading")]
    public int TrueHeading { get; set; }  // may differ from course due to wind or ocean currents

    [JsonPropertyName("Latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("Longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("Raim")]
    public bool Raim { get; set; }  // uses Receiver Autonomous Integrity Monitoring, i.e. redundant GPS satellites

    [JsonPropertyName("Valid")]
    public bool Valid { get; set; }  // false if position reported is not reliable
}

/// <summary>
/// Decoded static/voyage data (Message.ShipStaticData).
/// </summary>
public sealed class ShipStaticData
{
    [JsonPropertyName("ImoNumber")]
    public long ImoNumber { get; set; }  // International Marittime Organization number. Unique 7-digit number for each ship

    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    [JsonPropertyName("CallSign")]
    public string? CallSign { get; set; }  // radio call sign assigned by flag country. Up to 7 characters

    [JsonPropertyName("Type")]
    public int Type { get; set; }  // type of ship, e.g. Cargo, Tanker, etc.

    [JsonPropertyName("Destination")]
    public string? Destination { get; set; }  // where the ship is headed

    [JsonPropertyName("MaximumStaticDraught")]
    public double MaximumStaticDraught { get; set; }
}
