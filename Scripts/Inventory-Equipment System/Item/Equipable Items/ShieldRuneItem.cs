using UnityEngine;

[CreateAssetMenu(fileName = "ShieldRune", menuName = "NocturneKeepInteractive/Item/Equipable/Shield Rune")]
public class ShieldRuneItem : EquipableItem
{
    [SerializeField] private eShieldRuneEffect shieldRuneEffect;
    [SerializeField] private float shieldRuneEffectAmount;
    [SerializeField] private bool effectIsPercent;

    public eShieldRuneEffect ShieldRuneEffect => shieldRuneEffect;
    public float ShieldRuneEffectAmount => shieldRuneEffectAmount;
    public bool EffectIsPercent => effectIsPercent;

}
