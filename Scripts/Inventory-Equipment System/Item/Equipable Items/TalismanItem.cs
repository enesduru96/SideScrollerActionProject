using UnityEngine;

[CreateAssetMenu(fileName = "TalismanItem", menuName = "NocturneKeepInteractive/Item/Equipable/Talisman")]
public class TalismanItem : EquipableItem
{
    [Header("Talisman Object")]
    [SerializeField] private GameObject talismanModel;

    [Header("Effects")]
    [SerializeField] private eTalismanEffect talismanEffect;
    [Range(1, 50), SerializeField] private float talismanEffectAmount;
    [SerializeField] private bool effectIsPercent;


    public GameObject TalismanModel => talismanModel;
    public eTalismanEffect TalismanEffect => talismanEffect;
    public float TalismanEffectAmount => talismanEffectAmount;
    public bool EffectIsPercent => effectIsPercent;
}