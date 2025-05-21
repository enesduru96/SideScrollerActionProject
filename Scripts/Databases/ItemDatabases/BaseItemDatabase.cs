using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;

public abstract class BaseItemDatabase<T> : ScriptableObject where T : GameItem
{
    [SerializeField] protected List<T> allItems = new();
    protected ReadOnlyCollection<T> readOnlyItems;
    protected Dictionary<string, T> itemDict;

    public ReadOnlyCollection<T> AllItems => readOnlyItems;

    protected virtual void OnEnable()
    {
        RebuildDictionary();
    }

    public virtual void RebuildDictionary()
    {
        itemDict = new Dictionary<string, T>();
        readOnlyItems = allItems.AsReadOnly();

        foreach (var item in allItems)
        {
            if (!string.IsNullOrEmpty(item.GUID))
            {
                itemDict[item.GUID] = item;
            }
        }
    }

    public virtual T GetItemByGUID(string guid)
    {
        itemDict.TryGetValue(guid, out var item);
        return item;
    }

    public virtual void AddItem(T item)
    {
        if (allItems.Contains(item))
            return;

        allItems.Add(item);
        RebuildDictionary();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log($"Item {item.name} (GUID: {item.GUID}) added to database.");
    }
}