using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject weaponVisualRoot;
    [SerializeField] private MeleeWeaponTrail weaponTrail;

    [Header("Transforms")]
    [SerializeField] private Transform weaponMiddle;
    [SerializeField] private Transform weaponTip;

    [Header("Offsets")]
    [SerializeField] private Vector3 weaponHandPositionOffset;
    [SerializeField] private Vector3 weaponHandRotationOffset;

    [SerializeField] private Vector3 weaponHolsterPositionOffset;
    [SerializeField] private Vector3 weaponHolsterRotationOffset;

    public GameObject WeaponVisualRoot => weaponVisualRoot;
    public Vector3 WeaponHandPositionOffset => weaponHandPositionOffset;
    public Vector3 WeaponHandRotationOffset => weaponHandRotationOffset;

    public Vector3 WeaponHolsterPositionOffset => weaponHolsterPositionOffset;
    public Vector3 WeaponHolsterRotationOffset => weaponHolsterRotationOffset;


    public Transform WeaponMiddle => weaponMiddle;
    public Transform WeaponTip => weaponTip;
    public MeleeWeaponTrail WeaponTrail => weaponTrail;


    public void ApplyEquipTransforms()
    {
        transform.localPosition = weaponHandPositionOffset;
        transform.localEulerAngles = weaponHandRotationOffset;
    }

    public void ApplySheatheTransforms()
    {
        transform.localPosition = weaponHolsterPositionOffset;
        transform.localEulerAngles = weaponHolsterRotationOffset;
    }
}
