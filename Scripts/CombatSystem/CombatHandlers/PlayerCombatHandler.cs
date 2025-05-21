using UnityEngine;


using ShadowFort.Utilities;
using Animancer;
using System.Collections.Generic;
using System.Collections;

public class PlayerCombatHandler : BaseCombatHandler
{

    [Header("Attack Window")]
    [SerializeField] private float maxAttackDelay = 0.5f;
    [SerializeField] private float idleDelay = 0.5f;


    [Header("Combo & Idle States")]
    [SerializeField] private float comboTimer = 0f;
    [SerializeField] private bool isComboActive = false;
    [SerializeField] private float idleTimer = 0f;
    [SerializeField] private bool isReturningToIdle = false;




    [Header("Combat State Transition Parameters")]
    [SerializeField] private float combatTransitionDuration = 1f;


    [Header("Weapon and Shield")]
    [SerializeField] private GameObject playerSword;
    [SerializeField] private GameObject playerShield;


    [Space(30), Header("References")]
    [SerializeField] private CharacterLocalEventManager playerLocalEventManager;
    [SerializeField] private CharacterActor characterActor;
    [SerializeField] private CharacterStateController stateController;
    [SerializeField] private CharacterBrain characterBrain;
    [SerializeField] private CharacterMoverAndRotator characterMover;

    [SerializeField] private CharacterObjectPooler objectPooler;
    [SerializeField] private CharacterBodyReferences characterSkeletonTransforms;
    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private AnimancerComponent animancer;
    [SerializeField] private HitStopHandler hitStopHandler;
    [SerializeField] private InputBufferController inputBufferController;

    private float testAttackSpeed = 1.2f;

    private AttackID lastRealAttack = AttackID.None;
    private AttackID lastPlannedAttack = AttackID.None;

    protected override void OnEnable()
    {
        base.OnEnable();

        playerLocalEventManager.CharacterActions.OnDrawSheatheStarted += HandleDrawSheatheStarted;

        playerLocalEventManager.CharacterBusyState.OnReturnToIdleLockReleased += HandleReturnToIdleLockReleased;
        playerLocalEventManager.CharacterBusyState.OnCombatStateTransitionReleased += CompleteCombatStateTransition;


        //Attack Events
        playerLocalEventManager.CharacterActions.OnGroundLightAttackStarted += HandleGroundedLightAttack;
        playerLocalEventManager.CharacterActions.OnAirLightAttackStarted += HandleAirLightAttack;


        //Block
        playerLocalEventManager.CharacterActions.OnBlockStarted += StartBlocking;
        playerLocalEventManager.CharacterActions.OnBlockStopped += StopBlocking;

        playerLocalEventManager.PlayerCombatHandler.OnPlayerWeaponsDisabled += DisableWeapons;
        playerLocalEventManager.PlayerCombatHandler.OnPlayerWeaponsEnabled += EnableWeapons;

        EventManager.Instance.CombatHits.OnCombatHit += HandleOutgoingHit;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        playerLocalEventManager.CharacterActions.OnDrawSheatheStarted -= HandleDrawSheatheStarted;

        playerLocalEventManager.CharacterBusyState.OnReturnToIdleLockReleased -= HandleReturnToIdleLockReleased;
        playerLocalEventManager.CharacterBusyState.OnCombatStateTransitionReleased -= CompleteCombatStateTransition;

        //Attack Events
        playerLocalEventManager.CharacterActions.OnGroundLightAttackStarted -= HandleGroundedLightAttack;
        playerLocalEventManager.CharacterActions.OnAirLightAttackStarted -= HandleAirLightAttack;

        //Block
        playerLocalEventManager.CharacterActions.OnBlockStarted -= StartBlocking;
        playerLocalEventManager.CharacterActions.OnBlockStopped -= StopBlocking;


        playerLocalEventManager.PlayerCombatHandler.OnPlayerWeaponsDisabled -= DisableWeapons;
        playerLocalEventManager.PlayerCombatHandler.OnPlayerWeaponsEnabled -= EnableWeapons;

        EventManager.Instance.CombatHits.OnCombatHit -= HandleOutgoingHit;
    }


    protected virtual void HandleOutgoingHit(DamageSource damageSource, GameObject damageableTarget)
    {
        if (damageSource.OwnerActor != characterActor)
            return;

        if (damageSource.HasHitStop)
            hitStopHandler.TryApplyHitStop(damageSource.HitStopDuration);
    }

    private void DisableWeapons()
    {
        playerSword.SetActive(false);
        playerShield.SetActive(false);
    }
    private void EnableWeapons()
    {
        playerSword.SetActive(true);
        playerShield.SetActive(true);
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (characterActor == null)
            Debug.LogWarning($"{name}: CharacterActor is null!", this);

        if (objectPooler == null)
            Debug.LogWarning($"{name}: ObjectPooler is null!", this);

        if (characterSkeletonTransforms == null)
            Debug.LogWarning($"{name}: SkeletonTransforms is null!", this);

        if (playerInventory == null)
            Debug.LogWarning($"{name}: PlayerInventory is null!", this);

        if (stateController == null)
            Debug.LogWarning($"{name}: StateController is null!", this);

        if (characterBrain == null)
            Debug.LogWarning($"{name}: CharacterBrain is null!", this);

        if (characterMover == null)
            Debug.LogWarning($"{name}: CharacterMover is null!", this);

        if (playerLocalEventManager == null)
            Debug.LogWarning($"{name}: PlayerLocalEventManager is null!", this);
    }
#endif

    private void Start()
    {
        PreloadPooledDamageSources();
    }

    private void PreloadPooledDamageSources()
    {
        foreach (AttackElement attack in attackStorage.GetAllAttacks())
        {
            if (attack.damageSource == null)
                continue;

            if (attack.damageSource.IsPooled)
            {
                int poolCount = attack.damageSource.PooledCount;
                objectPooler.PreloadDamageSource(attack.damageSource.gameObject, attack.attackName, poolCount, characterActor);
            }
        }
    }

    private void HandleReturnToIdleLockReleased()
    {
        ResetCombo();
    }

    private void Update()
    {
        if (!characterActor.IsInCombatState)
            return;

        HandleComboTimer();

        HandleIdleTimer();
    }


    #region BLOCKING

    private void StartBlocking()
    {
        characterActor.IsBlocking = true;
    }

    private void StopBlocking()
    {
        characterActor.IsBlocking = false;
    }

    #endregion


    private void HandleDrawSheatheStarted()
    {
        playerLocalEventManager.CombatHandler.T_OnSetCombatTransitionLock(combatTransitionDuration);

        characterActor.IsBlocking = false;
    }

    private void CompleteCombatStateTransition()
    {
        Debug.Log(characterActor.IsInCombatState ? "Combat state activated" : "Combat state deactivated");
        ResetCombo();
    }



    private void HandleGroundedLightAttack()
    {

        bool restart = isReturningToIdle || !isComboActive || lastRealAttack == AttackID.None || comboTimer > maxAttackDelay;


        CancelIdle();

        ExecuteAttack(restart ? AttackID.LightAttack_1 : GetNextLightAttack());
    }

    private void HandleAirLightAttack()
    {

        bool restart = isReturningToIdle || !isComboActive || lastRealAttack == AttackID.None || comboTimer > maxAttackDelay;

        CancelIdle();

        ExecuteAttack(restart ? AttackID.AirLightAttack_1 : GetNextLightAttackAir());
    }


    private void HandleComboTimer()
    {
        if (!isComboActive)
            return;

        comboTimer += Time.deltaTime;

        if (!characterActor.BusyComboLocked)
        {
            if (characterActor.IsGrounded && inputBufferController.TryConsumeInput(BufferedInputType.LightAttack))
            {
                ExecuteAttack(GetNextLightAttack());
            }
            else if (!characterActor.IsGrounded && inputBufferController.TryConsumeInput(BufferedInputType.AirLightAttack))
            {
                ExecuteAttack(GetNextLightAttackAir());
            }
        }

        if (comboTimer > maxAttackDelay)
        {
            ResetCombo();
            isReturningToIdle = true;
            idleTimer = 0f;
        }
    }
    private void HandleIdleTimer()
    {
        if (!isReturningToIdle)
            return;

        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDelay)
        {
            ResetCombo();
            isReturningToIdle = false;
        }
    }
    

    private void ResetCombo()
    {
        comboTimer = 0f;
        isComboActive = false;
        lastRealAttack = AttackID.None;

        inputBufferController.ClearBuffer();
    }
    private void CancelIdle()
    {
        isReturningToIdle = false;
        idleTimer = 0f;
    }





    #region ATTACK METHODS

    private AttackID GetNextLightAttack()
    {
        List<AttackElement> attacks = attackStorage.LightAttacks;
        int attackCount = attacks.Count;

        if (attackCount == 0) return AttackID.None;

        int nextAttackIndex = 0;

        if (lastRealAttack != AttackID.None)
        {
            nextAttackIndex = (attacks.FindIndex(a => a.attackID == lastRealAttack) + 1) % attackCount;
        }

        return attacks[nextAttackIndex].attackID;
    }

    private AttackID GetNextLightAttackAir()
    {
        if (lastRealAttack == AttackID.AirLightAttack_1)
            return AttackID.AirLightAttack_2;

        switch (lastPlannedAttack)
        {
            case AttackID.None:
            default:
                return AttackID.AirLightAttack_1;

            case AttackID.AirLightAttack_1:
                return AttackID.AirLightAttack_2;

            case AttackID.AirLightAttack_2:
                return AttackID.AirLightAttack_3;

            case AttackID.AirLightAttack_3:
                return AttackID.AirLightAttack_1;
        }
    }

    private void ExecuteAttack(AttackID attackID)
    {

        if (characterActor.BusyCombatStateTransitionLocked)
            return;

        AttackElement attackElement = attackStorage.GetAttack(attackID);
        if (attackElement == null)
        {
            Debug.LogWarning("AttackData not found -> " + attackID);
            return;
        }

        lastRealAttack = attackID;

        bool willPlayerMoveWithAttack = attackElement.attackType == AttackType.Grounded ? true : false;
        RotateAndPushCharacter(attackElement, willPlayerMoveWithAttack);

        SpawnDamageSource(attackElement);

        playerLocalEventManager.CombatHandler.T_OnSetAttackLocks(attackElement);
        playerLocalEventManager.PlayerCombatHandler.T_OnPlayerAttackPerformed(attackElement);

        if (attackElement.activateWeaponTrail)
        {
            MeleeWeaponTrail weaponTrail = playerInventory.EquippedWeaponInstance.WeaponTrail;
            weaponTrail.ActivateTrailAfterDelay(attackElement.weaponTrailSpawnMoment, attackElement.weaponTrailLifetime);
        }

        isComboActive = true;
        comboTimer = 0f;
    }

    private void SpawnDamageSource(AttackElement attackElement)
    {
        if (attackElement.damageSource == null)
        {
            Debug.LogWarning($"No DamageSource defined for attack {attackElement.attackName}!");
            return;
        }

        DamageSource damageSource = objectPooler.GetDamageSource(
            attackElement.damageSource.gameObject,
            attackElement.attackName,
            Vector3.zero,
            Quaternion.identity,
            characterActor
        );

        Transform spawnParent = characterSkeletonTransforms.GetDamageSpawnPoint(damageSource.DamageSpawnLocation);

        if (attackElement.damageSource is MeleeDamageSource)
        {
            damageSource.transform.SetParent(null);
            damageSource.transform.position = new Vector3(spawnParent.position.x, spawnParent.position.y, characterActor.Position.z) + damageSource.SpawnPositionOffset;
            damageSource.transform.rotation = spawnParent.rotation * Quaternion.Euler(damageSource.SpawnRotationOffset);
        }
        else
        {
            damageSource.transform.SetParent(null);
            damageSource.transform.position = new Vector3(spawnParent.position.x, spawnParent.position.y, characterActor.Position.z) + damageSource.SpawnPositionOffset;
            damageSource.transform.rotation = spawnParent.rotation * Quaternion.Euler(damageSource.SpawnRotationOffset);
        }

        damageSource.GetComponent<Collider>().enabled = false;
        damageSource.gameObject.SetActive(true);

        StartCoroutine(DespawnDamageSourceAfterLifetime(attackElement, damageSource, damageSource.LifeTime));
    }

    private IEnumerator DespawnDamageSourceAfterLifetime(AttackElement attackElement, DamageSource damageSource, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        objectPooler.ReturnDamageSource(attackElement.attackName, damageSource);
    }


    private void RotateAndPushCharacter(AttackElement attackData, bool moveWithAttack)
    {

        Vector3 direction;
        bool inputIsLeft = characterBrain.CharacterActions.movement.Left;
        bool inputIsRight = characterBrain.CharacterActions.movement.Right;

        if (inputIsLeft)
            direction = Vector3.left;
        else if (inputIsRight)
            direction = Vector3.right;
        else
            direction = characterActor.IsFacingRight() ? Vector3.right : Vector3.left;

        characterMover.StartRotate(characterActor.Forward, direction, 0.1f, CharacterState.InterpolationType.Linear);

        if (moveWithAttack)
            ApplyPush(direction, attackData);

    }

    private void ApplyPush(Vector3 direction, AttackElement attackData)
    {
        if (attackData.useCurveForMotion && attackData.velocityCurve != null)
        {
            characterMover.StartVelocityPushCurve(direction, attackData.attackMovementDuration, attackData.velocityCurve);
        }
        else
        {
            characterMover.StartVelocityPush(direction, attackData.attackMovementDuration, attackData.attackMoveForce);
        }
    }

    #endregion


}
