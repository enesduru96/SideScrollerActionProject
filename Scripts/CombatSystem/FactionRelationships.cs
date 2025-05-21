using System.Collections.Generic;

public static class FactionRelationships
{
    private static readonly Dictionary<(Faction attacker, Faction target), bool> factionMap = new()
    {
        { (Faction.Player, Faction.PlayerAlly), false },
        { (Faction.Player, Faction.Enemy), true },
        { (Faction.Player, Faction.Neutral), false },
        { (Faction.Player, Faction.Environment), true },

        { (Faction.PlayerAlly, Faction.Player), false },
        { (Faction.PlayerAlly, Faction.Enemy), true },
        { (Faction.PlayerAlly, Faction.Neutral), false },
        { (Faction.PlayerAlly, Faction.Environment), true },

        { (Faction.Enemy, Faction.Player), true },
        { (Faction.Enemy, Faction.PlayerAlly), true },
        { (Faction.Enemy, Faction.Neutral), false },
        { (Faction.Enemy, Faction.Environment), true },

    };

    public static bool IsFactionEnemy(Faction attacker, Faction target)
    {
        return factionMap.TryGetValue((attacker, target), out var isEnemy) && isEnemy;
    }

    public static bool IsFactionAlly(Faction attacker, Faction target)
    {
        return factionMap.TryGetValue((attacker, target), out var isEnemy) && !isEnemy;
    }
}