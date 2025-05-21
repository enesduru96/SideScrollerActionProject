using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "NocturneKeepInteractive/Database/WeaponDatabase")]
public class WeaponDatabase : BaseItemDatabase<WeaponItem> { }

public static class WeaponDatabaseProvider
{
    public static WeaponDatabase Instance =>
        ItemDatabaseProvider<WeaponDatabase, WeaponItem>.Instance;
}