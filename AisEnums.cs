using System.ComponentModel;
using System.Reflection;

namespace ShipTracker;

// Automatic Identification System - international ship type
public enum AisShipType
{
  [Description("All")] All,
  [Description("Unknown")] Unknown,
  [Description("Fishing")] Fishing,
  [Description("Towing")] Towing,
  [Description("Dredging or Underwater Operations")] DredgingOrUnderwaterOperations,
  [Description("Pleasure craft")] PleasureCraft,
  [Description("Passenger")] Passenger,
  [Description("High speed craft")] HighSpeedCraft,
  [Description("Special craft or service")] SpecialCraftOrService,
  [Description("Military")] Military,
  [Description("Cargo")] Cargo,
  [Description("Tanker")] Tanker
}


// Automatic Identification System - international ship navigation status
public enum AisNavigationalStatus
{
  [Description("Under way by engine")] UnderWayByEngine = 0,
  [Description("At anchor")] AtAnchor = 1,
  [Description("Not under command")] NotUnderCommand = 2,
  [Description("Restricted maneuverability")] RestrictedManeuverability = 3,
  [Description("Constrained by draught")] ConstrainedByDdraught = 4,
  [Description("Moored")] Moored = 5,
  [Description("Aground")] Aground = 6,
  [Description("Fishing")] Fishing = 7,
  [Description("Under way by sail")] UnderWayBysail = 8,
  [Description("Reserved for high-speed craft")] Reserved1 = 9,
  [Description("Reserved for ground-effect aircraft)")] Reserved2 = 10,
  [Description("Towing astern")] TowingAstern = 11,
  [Description("Pushing/towing alongside")] TowingAlongside = 12,
  [Description("Reserved")] Reserved3 = 13,
  [Description("Using Search And Rescue Transmitter")] InDistress = 14,
  [Description("Unknown")] Unknown = 15
}

public static class EnumExtensions
{
  public static string GetDescription(this Enum value)
  {
    FieldInfo? field = value.GetType().GetField(value.ToString());
    if (field == null) return value.ToString();

    DescriptionAttribute? attribute = field.GetCustomAttribute<DescriptionAttribute>();
    return attribute != null ? attribute.Description : value.ToString();
  }
}
