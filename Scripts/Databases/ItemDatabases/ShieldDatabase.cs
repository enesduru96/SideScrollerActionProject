using UnityEngine;

[CreateAssetMenu(fileName = "ShieldDatabase", menuName = "NocturneKeepInteractive/Database/ShieldDatabase")]
public class ShieldDatabase : BaseItemDatabase<ShieldItem> { }

public static class ShieldDatabaseProvider
{
    public static ShieldDatabase Instance =>
        ItemDatabaseProvider<ShieldDatabase, ShieldItem>.Instance;
}