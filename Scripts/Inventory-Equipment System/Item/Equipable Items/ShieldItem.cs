using UnityEngine;

[CreateAssetMenu(fileName = "ShieldItem", menuName = "NocturneKeepInteractive/Item/Equipable/Shield")]
public class ShieldItem : EquipableItem
{
    [Header("Shield Object")]
    [SerializeField] private GameObject shieldModel;

    [Header("Base Parameters")]
    [SerializeField] private float shieldBlockRate;
    [SerializeField] private float shieldPoiseAmount;

    [Header("Shield Rune")]
    [SerializeField] private bool hasRune;
    [SerializeField] private ShieldRuneItem shieldRune;


    public GameObject ShieldModel => shieldModel;
    public float ShieldBlockRate => shieldBlockRate;
    public float ShieldPoiseAmount => shieldPoiseAmount;
    public bool HasRune => hasRune;
    public ShieldRuneItem ShieldRune => shieldRune;


    public void EquipNewShieldRune(ShieldRuneItem newShieldRune)
    {
        shieldRune = newShieldRune;
    }

}
