using UnityEngine;


using ShadowFort.Utilities;
using Animancer;
using System.Collections.Generic;
using System;
using System.Collections;


/// <summary>
/// Here player's combat related inputs are filtered by environmental restrictions, if there is none, the event is fired.
/// </summary>
public class PlayerCombatInputFilterer : MonoBehaviour
{
    [Header("Input Cooldowns")]
    [Range(1f, 4f), SerializeField] private float drawSheatheInputCooldown;
    private float drawSheatheElapsedTime = 2f;


    [Space(30), Header("References")]
    [SerializeField] private CharacterActor characterActor;
    [SerializeField] private CharacterStateController stateController;
    [SerializeField] private CharacterBrain characterBrain;
    [SerializeField] private CharacterLocalEventManager playerLocalEventManager;
    [SerializeField] private InputBufferController inputBufferController;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (characterActor == null)
            Debug.LogWarning($"{name}: CharacterActor is null!", this);

        if (stateController == null)
            Debug.LogWarning($"{name}: StateController is null!", this);

        if (characterBrain == null)
            Debug.LogWarning($"{name}: CharacterBrain is null!", this);

        if (playerLocalEventManager == null)
            Debug.LogWarning($"{name}: PlayerLocalEventManager is null!", this);
    }
#endif

    private void Start()
    {

    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        CheckDrawSheatheInput();
        CheckLightAttackInput();
        CheckBlockInput();
        
    }

    public bool HasDrawSheatheRestrictions()
    {
        if (drawSheatheElapsedTime <= drawSheatheInputCooldown)
            return true;

        if (characterActor.BusyComboLocked || characterActor.BusyInputLocked || characterActor.BusyAnimStateLocked)
            return true;

        if (stateController.CurrentState is not NormalMovementXY)
            return true;

        return false;
    }
    private void CheckDrawSheatheInput()
    {
        drawSheatheElapsedTime += Time.deltaTime;

        if (HasDrawSheatheRestrictions())
            return;

        if (characterBrain.CharacterActions.drawSheathe.Started)
        {
            characterActor.IsInCombatState = !characterActor.IsInCombatState;
            playerLocalEventManager.CharacterActions.T_OnDrawSheatheStarted();
            drawSheatheElapsedTime = 0f;
        }
    }


    public bool HasLightAttackRestrictions()
    {

        if (stateController.CurrentState is not NormalMovementXY)
            return true;

        if (!characterActor.IsInCombatState)
            return true;

        return false;
    }

    private void CheckLightAttackInput()
    {
        if (HasLightAttackRestrictions())
            return;

        // Tuþ basýlmadýysa çýk
        if (!characterBrain.CharacterActions.lightAttack.Started)
            return;

        // Hangi input tipini kullanacaðýmýzý seç
        bool grounded = characterActor.IsGrounded;
        BufferedInputType t = grounded ?
                               BufferedInputType.LightAttack :
                               BufferedInputType.AirLightAttack;
        float lifetime = grounded ?
                               inputBufferController.LightAttackBufferTime :
                               inputBufferController.AirLightAttackBufferTime;

        // Þu anda saldýrý yapýlabilir mi?
        bool locked = characterActor.BusyComboLocked ||
                      characterActor.BusyInputLocked ||
                      characterActor.BusyAnimStateLocked;

        if (locked)
        {
            // Yapýlamýyorsa sadece kuyrukla
            inputBufferController.BufferInput(t, lifetime);
            return;
        }

        // Yapýlabiliyorsa event’i hemen gönder
        if (grounded)
            playerLocalEventManager.CharacterActions.T_OnGroundLightAttackStarted();
        else
            playerLocalEventManager.CharacterActions.T_OnAirLightAttackStarted();
    }


    public bool HasBlockRestrictions()
    {
        if (stateController.CurrentState is not NormalMovementXY)
            return true;

        if(!characterActor.IsInCombatState)
            return true;

        if (characterActor.BusyCombatStateTransitionLocked)
            return true;

        return false;
    }
    private void CheckBlockInput()
    {
        if (HasBlockRestrictions())
            return;

        if (characterBrain.CharacterActions.block.value)
        {
            playerLocalEventManager.CharacterActions.T_OnBlockStarted();
        }
        else if (characterBrain.CharacterActions.block.Canceled)
        {
            playerLocalEventManager.CharacterActions.T_OnBlockStopped();
        }
    }
}
