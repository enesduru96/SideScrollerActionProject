
using ShadowFort.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Change it to "CharacterInventoryManager" and make "CharacterInventory" a SO. Therefore saving will be easier

//TODO 2:
// When ledge hanging, change shield and sword hand transforms for better visuals:
// shield hand transform rotation: -151.818, 62.019, 33.586 // position: -0.1105995, -0.02244796, 0.00660843
// sword hand transform rotation to : -13.245, 40.71, -49.9 // position: 0.1064125, -0.04074648, 1.087412e-05

public class PlayerInventory : MonoBehaviour
{
    private CharacterLocalEventManager playerLocalEventManager;
    private CharacterActor characterActor;
    private CharacterProfileStorage characterProfileStorage;
    private CharacterProfile characterProfile;
    private CharacterBodyReferences characterBodyReferences;

    [Header("Inventory")]
    [SerializeField] private List<GameItem> characterInventory = new();


    [Header("Equipped Items")]
    [SerializeField] private WeaponItem equippedWeaponItem;
    [SerializeField] private ShieldItem equippedShieldItem;
    [SerializeField] private TalismanItem equippedTalismanItem;
    [SerializeField] private ShieldRuneItem equippedShieldRuneItem;
    public List<GameItem> CharacterInventory => characterInventory;
    public WeaponItem EquippedWeaponItem => equippedWeaponItem;
    public TalismanItem EquippedTalismanItem => equippedTalismanItem;
    public ShieldItem EquippedShieldItem => equippedShieldItem;
    public ShieldRuneItem EquippedShieldRuneItem => equippedShieldRuneItem;


    [Header("Draw Delays")]
    [Range(0.1f, 1f), SerializeField] private float drawSwordDelay = 0.42f;
    [Range(0.1f, 1f), SerializeField] private float drawShieldDelay = 0.2f;

    [Header("Sheathe Delays")]
    [Range(0.1f, 1f), SerializeField] private float sheatheSwordDelay = 0.25f;
    [Range(0.1f, 1f), SerializeField] private float sheatheShieldDelay = 0.45f;






    private GameObject currentShieldModel;
    public GameObject CurrentShieldModel => currentShieldModel;



    private WeaponInstance equippedWeaponInstance;
    public WeaponInstance EquippedWeaponInstance => equippedWeaponInstance;


    private GameObject weaponPrefab;




    //TODO: Stop instantiating weapons at start, prepare pooling

    private void OnEnable()
    {
        playerLocalEventManager.CharacterActions.OnDrawSheatheStarted += HandleDrawSheathe;
    }

    private void OnDisable()
    {
        playerLocalEventManager.CharacterActions.OnDrawSheatheStarted -= HandleDrawSheathe;
    }
    private void Awake()
    {
        //int itemDatabaseItemCount = DatabaseManager.Instance.ItemDatabase.AllItems.Count;

        characterActor = this.GetComponentInBranch<CharacterActor>();
        characterBodyReferences = this.GetComponentInBranch<CharacterActor, CharacterBodyReferences>();

        characterProfileStorage = GetComponent<CharacterProfileStorage>();
        characterProfile = characterProfileStorage.characterProfile;

        playerLocalEventManager = this.GetComponentInBranch<CharacterActor, CharacterLocalEventManager>();
    }

    private void Start()
    {
        weaponPrefab = Instantiate(equippedWeaponItem.WeaponPrefab, characterBodyReferences.SwordHolderTransform);
        equippedWeaponInstance = weaponPrefab.GetComponent<WeaponInstance>();

        equippedWeaponInstance.ApplySheatheTransforms();

        currentShieldModel = Instantiate(equippedShieldItem.ShieldModel, characterBodyReferences.ShieldHolderTransform);
    }

    private void HandleDrawSheathe()
    {
        if (characterActor.IsInCombatState)
        {
            DrawWeapon();

        }
        else
        {
            SheatheWeapon();
        }
    }

    private Coroutine DrawSheatheWeaponCoroutine;
    private Coroutine DrawSheatheShieldCoroutine;
    private void DrawWeapon()
    {
        DrawSheatheWeaponCoroutine = StartCoroutine(ChangeWeaponTransform(equippedWeaponInstance, characterBodyReferences.SwordHandTransform, drawSwordDelay, true));
        DrawSheatheShieldCoroutine = StartCoroutine(ChangeShieldTransform(currentShieldModel, characterBodyReferences.ShieldHandTransform, drawShieldDelay, true));
    }
    private void SheatheWeapon()
    {
        DrawSheatheWeaponCoroutine = StartCoroutine(ChangeWeaponTransform(equippedWeaponInstance, characterBodyReferences.SwordHolderTransform, sheatheSwordDelay, false));
        DrawSheatheShieldCoroutine = StartCoroutine(ChangeShieldTransform(currentShieldModel, characterBodyReferences.ShieldHolderTransform, sheatheShieldDelay, false));
    }

    private IEnumerator ChangeWeaponTransform(WeaponInstance weaponInstance, Transform newTransform, float waitAmount, bool isEquipping)
    {
        yield return Wait.ForSeconds(waitAmount);

        weaponInstance.transform.parent = newTransform;

        if (isEquipping)
            weaponInstance.ApplyEquipTransforms();
        else
            weaponInstance.ApplySheatheTransforms();

        DrawSheatheWeaponCoroutine = null;
    }

    private IEnumerator ChangeShieldTransform(GameObject shieldModel, Transform newTransform, float waitAmount, bool isEquipping)
    {
        yield return Wait.ForSeconds(waitAmount);

        shieldModel.transform.parent = newTransform;
        shieldModel.transform.position = newTransform.position;
        shieldModel.transform.rotation = newTransform.rotation;

        DrawSheatheShieldCoroutine = null;
    }

    private void EquipTalisman(TalismanItem newTalisman)
    {

    }
    private void EquipShield(ShieldItem newShield)
    {

    }
    private void EquipShieldRune(ShieldRuneItem newShieldRune)
    {

    }


}
