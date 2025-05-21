using UnityEngine;
using UnityEditor;


public abstract class BaseItemDatabaseEditor<TItem, TDatabase> : Editor
    where TItem : GameItem
    where TDatabase : BaseItemDatabase<TItem>
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TDatabase database = (TDatabase)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Scan & Add Missing Items"))
        {
            int addedCount = 0;
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(TItem).Name}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TItem item = AssetDatabase.LoadAssetAtPath<TItem>(path);

                if (item != null && !database.AllItems.Contains(item))
                {
                    database.AddItem(item);
                    addedCount++;
                }
            }

            EditorUtility.SetDirty(database);
            Debug.Log($"{typeof(TDatabase).Name} updated. {addedCount} new items added.");
        }
    }
}

