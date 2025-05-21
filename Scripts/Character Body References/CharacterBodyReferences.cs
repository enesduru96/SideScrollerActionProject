using UnityEngine;

public class CharacterBodyReferences : MonoBehaviour
{
    [Header("Character Mesh")]
    [SerializeField] private SkinnedMeshRenderer characterMesh;


    [Header("Weapon and Shield Transforms")]
    [SerializeField] private Transform swordHandTransform;
    [SerializeField] private Transform shieldHandTransform;

    [SerializeField] private Transform swordHolderTransform;
    [SerializeField] private Transform shieldHolderTransform;


    [Header("Body Transforms")]
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightFootTransform;
    [SerializeField] private Transform leftFootTransform;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform chestTransform;

    public SkinnedMeshRenderer CharacterMesh => characterMesh;

    public Transform SwordHandTransform => swordHandTransform;
    public Transform ShieldHandTransform => shieldHandTransform;
    public Transform SwordHolderTransform => swordHolderTransform;
    public Transform ShieldHolderTransform => shieldHolderTransform;


    public Transform RightHandTransform => rightHandTransform;
    public Transform LeftHandTransform => leftHandTransform;
    public Transform RightFootTransform => rightFootTransform;
    public Transform LeftFootTransform => leftFootTransform;
    public Transform HeadTransform => headTransform;
    public Transform ChestTransform => chestTransform;



    public Transform GetDamageSpawnPoint(DamageSpawnPoint spawnPoint)
    {
        switch (spawnPoint)
        {
            case DamageSpawnPoint.None:
                return null;
            case DamageSpawnPoint.MainWeapon:
                return SwordHandTransform;
            case DamageSpawnPoint.SecondHandItem:
                return ShieldHandTransform;
            case DamageSpawnPoint.LeftHand:
                return leftHandTransform;
            case DamageSpawnPoint.RightHand:
                return rightHandTransform;
            case DamageSpawnPoint.LeftFoot:
                return leftFootTransform;
            case DamageSpawnPoint.RightFoot:
                return leftFootTransform;
            case DamageSpawnPoint.Chest:
                return ChestTransform;
            case DamageSpawnPoint.Head:
                return HeadTransform;
            default:
                return null;
        }
    }
}
