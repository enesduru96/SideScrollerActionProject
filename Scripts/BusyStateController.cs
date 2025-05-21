using UnityEngine;
using ShadowFort.Utilities;
using System;

public class BusyStateController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterLocalEventManager characterLocalEventManager;
    [SerializeField] private CharacterActor characterActor;


    [Header("Lock Durations")]
    [SerializeField] private float comboLockDuration;
    private float comboLockElapsedTime;

    [SerializeField] private float inputLockDuration;
    private float inputLockElapsedTime;

    [SerializeField] private float animStateLockDuration;
    private float animStateLockElapsedTime;

    [SerializeField] private float rootAnimStateLockDuration;
    private float rootAnimStateLockElapsedTime;

    [SerializeField] private float returnToIdleLockDuration;
    private float returnToIdleLockElapsedTime;

    [SerializeField] private float combatStateTransitionLockDuration;
    private float combatStateTransitionLockElapsedTime;

    [SerializeField] private float dashLockDuration;
    private float DashLockElapsedTime;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (characterActor == null)
            Debug.LogWarning($"{name}: CharacterActor is null!", this);

        if (characterLocalEventManager == null)
            Debug.LogWarning($"{name}: characterLocalEventManager is null!", this);
    }
#endif


    private void OnEnable()
    {
        characterLocalEventManager.CombatHandler.OnSetAttackLocks += SetAttackLocks;
        characterLocalEventManager.CombatHandler.OnSetCombatTransitionLock += SetCombatTransitionLock;
        characterLocalEventManager.CharacterActions.OnTurnBackRunningStarted += SetAnimStateLockRootMotion;
        characterLocalEventManager.CharacterBusyState.OnRunningTurnBackAnimReleased += ReleaseRootMotion;
    }

    private void OnDisable()
    {
        characterLocalEventManager.CombatHandler.OnSetAttackLocks -= SetAttackLocks;
        characterLocalEventManager.CombatHandler.OnSetCombatTransitionLock -= SetCombatTransitionLock;
        characterLocalEventManager.CharacterActions.OnTurnBackRunningStarted -= SetAnimStateLockRootMotion;
        characterLocalEventManager.CharacterBusyState.OnRunningTurnBackAnimReleased -= ReleaseRootMotion;
    }

    private void ReleaseRootMotion()
    {
        characterActor.UseRootMotion = false;
    }

    public void SetAttackLocks(AttackElement attackElement)
    {
        characterActor.BusyComboLocked = true;
        comboLockDuration = attackElement.comboLockDuration;

        characterActor.BusyInputLocked = true;
        inputLockDuration = attackElement.inputLockDuration;

        characterActor.BusyAnimStateLocked = true;
        animStateLockDuration = attackElement.animStateLockDuration;

        characterActor.BusyReturnToIdleLocked = true;
        returnToIdleLockDuration = attackElement.returnToIdleLockDuration;

        comboLockElapsedTime = 0f;
        inputLockElapsedTime = 0f;
        animStateLockElapsedTime = 0f;
        returnToIdleLockElapsedTime = 0f;

    }

    public void SetCombatTransitionLock(float duration)
    {
        characterActor.BusyCombatStateTransitionLocked = true;
        combatStateTransitionLockDuration = duration;
        combatStateTransitionLockElapsedTime = 0f;
    }

    public void SetAnimStateLockRootMotion(float duration)
    {
        characterActor.SetUpRootMotion(true, PhysicsActor.RootMotionVelocityType.SetVelocity, true);
        characterActor.BusyAnimStateLockedRootMotion = true;
        rootAnimStateLockDuration = duration;
    }

    void Update()
    {
        CheckComboLock();
        CheckInputLock();
        CheckAnimStateLock();
        CheckRootAnimStateLock();
        CheckReturnToIdleLock();
        CheckCombatStateTransitionLock();
    }

    private void CheckComboLock()
    {
        if (!characterActor.BusyComboLocked)
            return;

        comboLockElapsedTime += Time.deltaTime;
        if (comboLockElapsedTime >= comboLockDuration)
        {
            characterActor.BusyComboLocked = false;
            comboLockElapsedTime = 0f;
            characterLocalEventManager.CharacterBusyState.T_OnComboLockReleased();
        }
    }

    private void CheckInputLock()
    {
        if (!characterActor.BusyInputLocked)
            return;

        inputLockElapsedTime += Time.deltaTime;
        if (inputLockElapsedTime >= inputLockDuration)
        {
            characterActor.BusyInputLocked = false;
            inputLockElapsedTime = 0f;
            characterLocalEventManager.CharacterBusyState.T_OnInputLockReleased();
        }
    }

    private void CheckAnimStateLock()
    {
        if (!characterActor.BusyAnimStateLocked)
            return;

        animStateLockElapsedTime += Time.deltaTime;
        if (animStateLockElapsedTime >= animStateLockDuration)
        {
            characterActor.BusyAnimStateLocked = false;
            animStateLockElapsedTime = 0f;
            characterLocalEventManager.CharacterBusyState.T_OnAnimStateLockReleased();
        }
    }

    private void CheckRootAnimStateLock()
    {
        if (!characterActor.BusyAnimStateLockedRootMotion)
            return;

        rootAnimStateLockElapsedTime += Time.deltaTime;
        if(rootAnimStateLockElapsedTime >= rootAnimStateLockDuration)
        {
            characterActor.BusyAnimStateLockedRootMotion = false;
            rootAnimStateLockElapsedTime = 0f;
            characterLocalEventManager.CharacterBusyState.T_OnRunningTurnBackAnimReleased();
        }
    }

    private void CheckReturnToIdleLock()
    {
        if (!characterActor.BusyReturnToIdleLocked)
            return;

        returnToIdleLockElapsedTime += Time.deltaTime;
        if (returnToIdleLockElapsedTime >= returnToIdleLockDuration)
        {
            characterActor.BusyReturnToIdleLocked = false;
            returnToIdleLockElapsedTime = 0f;
            characterLocalEventManager.CharacterBusyState.T_OnReturnToIdleLockReleased();
        }
    }

    private void CheckCombatStateTransitionLock()
    {
        if (!characterActor.BusyCombatStateTransitionLocked)
            return;


        combatStateTransitionLockElapsedTime += Time.deltaTime;
        if(combatStateTransitionLockElapsedTime >= combatStateTransitionLockDuration)
        {
            characterActor.BusyCombatStateTransitionLocked = false;
            combatStateTransitionLockElapsedTime = 0f;
            characterLocalEventManager.CharacterBusyState.T_OnCombatStateTransitionReleased();
        }
    }
}
