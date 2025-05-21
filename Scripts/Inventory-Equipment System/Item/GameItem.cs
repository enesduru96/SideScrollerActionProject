using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using System;

public abstract class GameItem : ScriptableObject
{
    [SerializeField] private string guid;

    [SerializeField] private LocalizedString itemName;
    [SerializeField] private LocalizedString itemDescription;
    [SerializeField] private Texture2D itemIcon;

    public string GUID => guid;
    public LocalizedString ItemName => itemName;
    public LocalizedString ItemDescription => itemDescription;
    public Texture2D ItemIcon => itemIcon;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(guid))
        {
            guid = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

}
