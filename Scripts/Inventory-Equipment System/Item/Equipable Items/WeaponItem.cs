using UnityEngine;



[CreateAssetMenu(fileName = "WeaponItem", menuName = "NocturneKeepInteractive/Item/Equipable/Weapon")]
public class WeaponItem : EquipableItem
{
    [Header("Weapon Object")]
    [SerializeField] private GameObject weaponPrefab;

    [Header("Base Parameters")]
    [SerializeField] private float weaponBaseDamage;

    [Header("Special Effects")]
    [SerializeField] private eWeaponItemEffect weaponSpecialEffect;
    [SerializeField] private float weaponSpecialEffectAmount;
    [SerializeField] private bool effectIsPercent;



    public GameObject WeaponPrefab => weaponPrefab;
    public float WeaponBaseDamage => weaponBaseDamage;
    public eWeaponItemEffect WeaponSpecialEffect => weaponSpecialEffect;
    public float WeaponSpecialEffectAmount => weaponSpecialEffectAmount;
    public bool EffectIsPercent => effectIsPercent;

}
