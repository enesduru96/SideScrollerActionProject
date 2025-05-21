using UnityEngine;

public static class ItemDatabaseProvider<TDatabase, TItem>
    where TDatabase : BaseItemDatabase<TItem>
    where TItem : GameItem
{
    private static TDatabase instance;

    public static TDatabase Instance
    {
        get
        {
            if (instance == null)
                Load();
            return instance;
        }
    }

    public static void Load()
    {
        string path = typeof(TDatabase).Name; // Örn: "WeaponDatabase"
        instance = Resources.Load<TDatabase>(path);
        if (instance == null)
        {
            Debug.LogError($"[ItemDatabaseProvider] {typeof(TDatabase).Name} not found in Resources!");
        }
    }

    public static void Inject(TDatabase db)
    {
        instance = db;
    }
}