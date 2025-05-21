using UnityEngine;
using ShadowFort.Utilities;
using Animancer;
using System.Collections;

public class NormalMovementXY_AnimState : CharacterAnimState
{
    [SerializeField] private StateAnimations_NormalMovement animListNormalMovement;
    [SerializeField] private StateAnimations_NormalMovement_Combat animListNormalMovementCombat;


    [SerializeField] private float jumpStartDuration = 0.2f;
    private float jumpStartElapsedTime = 0f;

    [SerializeField] private float jumpEndDuration = 0.1f;
    private float jumpEndElapsedTime = 0f;

    [SerializeField] private float runningTurnBackDuration = 0.3f;

    private StateAnimations_NormalMovement_Base animListCurrent;
    private NormalMovementXY normalMovementState;
    private LedgeHanging ledgeHangingState;
    private CharacterProfileStorage characterProfileStorage;
    private CharacterProfile characterProfile;

    #region SubState Methods

    private enum SubState
    {
        None,
        SwitchingCombatState,
        Attacking,
        AirAttacking,
        JumpStart,
        JumpStartFromBracedLedge,
        ExitingVault,
        JumpLoopAscending,
        JumpLoopDescending,
        Landing,


    }
    [SerializeField] private SubState currentSubState = SubState.None;

    private bool TryChangeSubState(SubState newSubState)
    {
        if (currentSubState == newSubState)
            return false;

        switch (currentSubState)
        {
            case SubState.Attacking:
                if (characterActor.BusyComboLocked)
                    return false;
                else
                {
                    currentSubState = newSubState;
                    return true;
                }

            case SubState.None:
                currentSubState = newSubState;
                return true;


            default:
                currentSubState = newSubState;
                return true;


                //case SubState.DashLoop:
                //    if (newState == PlayerAnimState.JumpStart)
                //    {
                //        HandleDashJumpAnimation();
                //    }
                //    currentSubState = newSubState;
                //    return true;



        }

    }
    private void TransitionToSubState(SubState newSubState, ClipTransition anim, float transitionDuration)
    {
        if (!TryChangeSubState(newSubState))
            return;

        characterAnimStateController.CurrentAnim = anim;
        animationPlayer.TransitionToAnimation(anim, transitionDuration, Easing.Function.Linear, 0);


    }

    #endregion


    [SerializeField] private AvatarMask avatarMask;

    private void SubscribeToEvents()
    {
        characterLocalEventManager.CharacterActions.OnDrawSheatheStarted += HandleIdleCombatSwitch;
        characterLocalEventManager.PlayerCombatHandler.OnPlayerAttackPerformed += PlayAttackAnimation;

        characterLocalEventManager.CharacterBusyState.OnReturnToIdleLockReleased += ReturnToIdle;
        characterLocalEventManager.CharacterBusyState.OnCombatStateTransitionReleased += CompleteCombatStateChange;

        EventManager.Instance.CombatHits.OnCharacterTookDamage += HandleTakeDamageAnimation;
        EventManager.Instance.CombatHits.OnCharacterPushedBack += HandlePushbackAnimation;

        normalMovementState.OnGroundedJumpPerformed += HandleJumpStartAnim;
    }
    private void UnsubscribeFromEvents()
    {
        characterLocalEventManager.CharacterActions.OnDrawSheatheStarted -= HandleIdleCombatSwitch;
        characterLocalEventManager.PlayerCombatHandler.OnPlayerAttackPerformed -= PlayAttackAnimation;


        characterLocalEventManager.CharacterBusyState.OnReturnToIdleLockReleased -= ReturnToIdle;
        characterLocalEventManager.CharacterBusyState.OnCombatStateTransitionReleased -= CompleteCombatStateChange;

        EventManager.Instance.CombatHits.OnCharacterTookDamage -= HandleTakeDamageAnimation;
        EventManager.Instance.CombatHits.OnCharacterPushedBack -= HandlePushbackAnimation;

        normalMovementState.OnGroundedJumpPerformed -= HandleJumpStartAnim;
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    protected override void Awake()
    {
        base.Awake();

        normalMovementState = characterStateController.GetComponent<NormalMovementXY>();
        ledgeHangingState = characterStateController.GetComponent<LedgeHanging>();
        characterProfileStorage = this.GetComponentInBranch<CharacterActor, CharacterProfileStorage>();
        characterProfile = characterProfileStorage.characterProfile;
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void EnterAnimState(CharacterAnimState fromState)
    {
        if (characterActor == null)
        {
            Debug.LogError("_characterActor is not initialized. Ensure Start is called before EnterAnimState.");
            return;
        }

        if (ledgeHangingState.ledgeSubState == LedgeHanging.LedgeSubState.BracedJumpingBackToRight || ledgeHangingState.ledgeSubState == LedgeHanging.LedgeSubState.BracedJumpingBackToLeft)
        {
            currentSubState = SubState.JumpStartFromBracedLedge;
        }
        else if (fromState is Vault_AnimState)
        {
            currentSubState = SubState.ExitingVault;
        }
        else
        {
            animListCurrent = characterActor.IsInCombatState ? animListNormalMovementCombat : animListNormalMovement;
            animationPlayer.TransitionToAnimation(animListCurrent.Idle, 0.1f, Easing.Function.Linear, 0);
        }

    }
    public override void UpdateAnimState()
    {

        HandlePlanarMovementAnim();

        HandleJumpingAnim();

        HandleFallingAnim();
    }
    public override void ExitAnimState(CharacterAnimState toState)
    {
        currentSubState = SubState.None;
    }


    #region COMBAT

    private void HandleIdleCombatSwitch()
    {
        //if (!characterActor.IsGrounded)
        //{
        //    return;
        //}

        bool nextCombatState = characterActor.IsInCombatState;
        PlayCombatStateChangeAnimation(nextCombatState);
        TryChangeSubState(SubState.SwitchingCombatState);
    }
    private void CompleteCombatStateChange()
    {
        animListCurrent = characterActor.IsInCombatState ? animListNormalMovementCombat : animListNormalMovement;
        TryChangeSubState(SubState.None);
    }
    private void ReturnToIdle()
    {
        currentSubState = SubState.None;
    }
    public void PlayCombatStateChangeAnimation(bool isInCombatState)
    {
        ClipTransition transitionAnimation = isInCombatState ? animListNormalMovement.switchIdleToCombat : animListNormalMovementCombat.switchCombatToIdle;
        characterAnimStateController.CurrentAnim = transitionAnimation;

        animListCurrent = characterActor.IsInCombatState ? animListNormalMovementCombat : animListNormalMovement;

        characterAnimStateController.MaskedAnimancerLayer.Mask = avatarMask;
        characterAnimStateController.MaskedAnimancerLayer.IsAdditive = false;
        characterAnimStateController.MaskedAnimancerLayer.Play(transitionAnimation);
        StartCoroutine(ResetMaskedLayerWeight());

        //animationPlayer.PlayAnimation(transitionAnimation, 0, Easing.Function.Linear);
        //characterActor.Velocity = Vector3.zero;


    }

    [SerializeField] private float maskedLayerWaitTime = 0.1f;
    IEnumerator ResetMaskedLayerWeight()
    {
        float duration = 0.7f;
        float elapsedTime = 0f;
        float initialWeight = 1f;

        yield return new WaitForSeconds(maskedLayerWaitTime);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newWeight = Mathf.Lerp(initialWeight, 0, elapsedTime / duration);
            characterAnimStateController.MaskedAnimancerLayer.SetWeight(newWeight);
            yield return null;
        }

        characterAnimStateController.MaskedAnimancerLayer.SetWeight(0);
    }

    private void PlayAttackAnimation(AttackElement attackElement)
    {
        AttackID attackID = attackElement.attackID;
        if (attackElement.IsFirstAttack)
        {
            animationPlayer.PlayAnimationWithZeroTime(attackElement.attackAnimation, 0, attackElement.animEasingFunction);
        }
        else
        {
            animationPlayer.PlayAnimation(attackElement.attackAnimation, 0, attackElement.animEasingFunction);

        }
        characterAnimStateController.CurrentAnim = attackElement.attackAnimation;

        currentSubState = attackElement.attackType == AttackType.Grounded ? SubState.Attacking : SubState.AirAttacking;
    }


    private void HandleTakeDamageAnimation(DamageableCharacter character, DamageSource damageSource)
    {
        if (character.CharacterActor != characterActor)
            return;

        //_characterAnimStateController.MaskedAnimancerLayer.Mask = _chestHeadMask;
        characterAnimStateController.MaskedAnimancerLayer.IsAdditive = true;


        int randomIndex = UnityEngine.Random.Range(0, animListCurrent.HitLightList.Count);
        ClipTransition randomHitClip = animListCurrent.HitLightList[randomIndex];

        characterAnimStateController.MaskedAnimancerLayer.Play(randomHitClip);

    }
    private void HandlePushbackAnimation(DamageableCharacter character, DamageSource damageSource)
    {
        if (character.gameObject != this)
            return;

    }

    #endregion


    #region PLANAR MOVEMENT

    private bool HasPlanarMovementStateRestrictions()
    {
        if (characterActor.BusyInputLocked)
        {
            return true;
        }

        if (!characterActor.IsGrounded)
        {
            return true;
        }


        if (characterActor.BusyAnimStateLocked)
        {
            return true;
        }

        if (characterActor.BusyAnimStateLockedRootMotion)
        {
            return true;
        }

        if (currentSubState == SubState.Landing)
        {
            return true;
        }

        //if (characterActor.BusyCombatStateTransitionLocked)
        //{
        //    return true;
        //}

        if (isJumpingBackFromLedge)
        {
            return true;
        }

        if (currentSubState == SubState.ExitingVault)
        {
            return true;
        }

        return false;
    }

    CharacterActions characterActions;
    private ClipTransition GetPlanarMovementAnimation()
    {
        float planarVelocityX = characterActor.PlanarVelocity.x;
        bool facingRight = characterActor.IsFacingRight();
        bool facingLeft = characterActor.IsFacingLeft();

        characterActions = characterStateController.CharacterBrain.CharacterActions;

        if (characterActions.movement.value.x == 0f)
        {
            if (characterActor.IsBlocking)
            {
                return animListCurrent.IdleBlocking;
            }

            return animListCurrent.Idle;
        }


        // If Character is Turning

        if (facingRight && planarVelocityX >= characterProfile.RunSpeed && characterActions.movement.value.x < 0f)
        {
            characterLocalEventManager.CharacterActions.T_OnTurnBackRunningStarted(runningTurnBackDuration);
            return animListCurrent.RunningTurnBackToLeft;
        }
        else if (facingLeft && planarVelocityX <= -characterProfile.RunSpeed && characterActions.movement.value.x > 0f)
        {
            characterLocalEventManager.CharacterActions.T_OnTurnBackRunningStarted(runningTurnBackDuration);
            return animListCurrent.RunningTurnBackToRight;
        }

        if (facingRight)
        {
            if (characterActor.IsBlocking && characterActor.IsInCombatState && planarVelocityX > characterProfile.SpeedMinThresholdForAnim)
                return animListCurrent.WalkForwardBlocking;

            if (characterActor.IsBlocking && characterActor.IsInCombatState && planarVelocityX < -characterProfile.SpeedMinThresholdForAnim)
                return animListCurrent.WalkBackwardBlocking;


            if (planarVelocityX > characterProfile.RunSpeed && characterActions.run.value)
                return animListCurrent.SprintForward;

            if (planarVelocityX > characterProfile.WalkSpeed && planarVelocityX <= characterProfile.SprintSpeed)
                return animListCurrent.RunForward;

            if (planarVelocityX > characterProfile.SpeedMinThresholdForAnim && planarVelocityX <= characterProfile.WalkSpeed)
                return animListCurrent.WalkForward;

            if (planarVelocityX < -characterProfile.WalkSpeed)
                return animListCurrent.RunBackward;

            if (planarVelocityX < -characterProfile.SpeedMinThresholdForAnim && planarVelocityX >= -characterProfile.WalkSpeed)
                return animListCurrent.WalkBackward;

            if (planarVelocityX < characterProfile.SpeedMinThresholdForAnim)
            {
                if (characterActor.IsBlocking)
                    return animListCurrent.IdleBlocking;
                else
                    return animListCurrent.Idle;
            }


        }
        else if (facingLeft)
        {
            if (characterActor.IsBlocking && characterActor.IsInCombatState && planarVelocityX < -characterProfile.SpeedMinThresholdForAnim)
                return animListCurrent.WalkForwardBlocking;

            if (characterActor.IsBlocking && characterActor.IsInCombatState && planarVelocityX > characterProfile.SpeedMinThresholdForAnim)
                return animListCurrent.WalkBackwardBlocking;

            if (planarVelocityX < -characterProfile.RunSpeed && characterActions.run.value)
                return animListCurrent.SprintForward;

            if (planarVelocityX < -characterProfile.WalkSpeed && planarVelocityX >= -characterProfile.SprintSpeed)
                return animListCurrent.RunForward;

            if (planarVelocityX < -characterProfile.SpeedMinThresholdForAnim && planarVelocityX >= -characterProfile.WalkSpeed)
                return animListCurrent.WalkForward;

            if (planarVelocityX > characterProfile.WalkSpeed)
                return animListCurrent.RunBackward;

            if (planarVelocityX > characterProfile.SpeedMinThresholdForAnim && planarVelocityX <= characterProfile.WalkSpeed)
                return animListCurrent.WalkBackward;

            if (planarVelocityX > -characterProfile.SpeedMinThresholdForAnim)
            {
                if (characterActor.IsBlocking)
                    return animListCurrent.IdleBlocking;
                else
                    return animListCurrent.Idle;
            }
        }

        return null;
    }

    private void HandlePlanarMovementAnim()
    {

        if (HasPlanarMovementStateRestrictions())
        {
            return;
        }

        ClipTransition newAnim = GetPlanarMovementAnimation();
        if (newAnim != null && characterAnimStateController.CurrentAnim != newAnim)
        {
            characterAnimStateController.CurrentAnim = newAnim;
            animationPlayer.TransitionToAnimationDefaultDuration(newAnim, Easing.Function.Linear, 0);
        }


    }

    #endregion


    #region JUMPING - FALLING

    [SerializeField] private float jumpFromBracedLedgeDuration = 0.2f;
    [SerializeField] private float jumpFromBracedLedgeElapsedTime = 0f;
    [SerializeField] private bool isJumpingBackFromLedge = false;

    private void HandleJumpStartAnim()
    {
        if (TryChangeSubState(SubState.JumpStart))
        {
            jumpStartElapsedTime = 0f;

            ClipTransition nextAnimation = animListCurrent.JumpStart;
            characterAnimStateController.CurrentAnim = animListCurrent.JumpStart;
            animationPlayer.TransitionToAnimation(nextAnimation, 0.02f, Easing.Function.Linear, 0);
        }
    }
    private bool HasJumpingRestrictions()
    {
        if (characterActor.BusyAnimStateLocked)
            return true;

        return false;
    }

    [SerializeField] private float exitVaultDuration;
    [SerializeField] private float exitVaultDurationSprint;
    private float exitVaultElapsedTime;
    private void HandleJumpingAnim()
    {
        if (HasJumpingRestrictions())
            return;

        switch (currentSubState)
        {
            case SubState.JumpStart:
                jumpStartElapsedTime += Time.deltaTime;
                if (jumpStartElapsedTime >= jumpStartDuration && !characterActor.IsGrounded)
                {
                    TransitionToSubState(SubState.JumpLoopAscending, animListCurrent.JumpLoopAscending, 0.1f);
                }
                else if (jumpStartElapsedTime >= jumpStartDuration && characterActor.IsGrounded)
                {
                    TransitionToSubState(SubState.Landing, animListCurrent.LandingAnimaiton, 0f);
                }
                break;

            case SubState.JumpStartFromBracedLedge:
                isJumpingBackFromLedge = true;
                break;

            case SubState.ExitingVault:
                if (characterStateController.CharacterBrain.CharacterActions.run.value)
                {
                    exitVaultElapsedTime += Time.deltaTime;
                    if (exitVaultElapsedTime >= exitVaultDurationSprint)
                    {
                        exitVaultElapsedTime = 0f;
                        TransitionToSubState(SubState.None, animListCurrent.SprintForward, 0.2f);
                    }
                }
                else
                {
                    exitVaultElapsedTime += Time.deltaTime;
                    if (exitVaultElapsedTime >= exitVaultDuration)
                    {
                        exitVaultElapsedTime = 0f;
                        TransitionToSubState(SubState.None, animListCurrent.RunForward, 0.3f);
                    }
                }

                break;

            case SubState.JumpLoopAscending:
                if (characterActor.IsFalling)
                {
                    TransitionToSubState(SubState.JumpLoopDescending, animListCurrent.JumpLoopDescending, 0.1f);
                }
                break;

            case SubState.JumpLoopDescending:
                if (characterActor.IsGrounded)
                {
                    jumpEndElapsedTime = 0f;
                    TransitionToSubState(SubState.Landing, animListCurrent.LandingAnimaiton, 0.1f);
                }
                break;

            case SubState.AirAttacking:
                if (characterActor.IsGrounded)
                {
                    jumpEndElapsedTime = 0f;
                    TransitionToSubState(SubState.Landing, animListCurrent.LandingAnimaiton, 0.1f);
                }
                break;

            case SubState.Landing:
                jumpEndElapsedTime += Time.deltaTime;
                if (jumpEndElapsedTime >= jumpEndDuration)
                {
                    TransitionToSubState(SubState.None, animListCurrent.Idle, 0.1f);
                }
                break;
        }


        if (isJumpingBackFromLedge)
        {
            jumpFromBracedLedgeElapsedTime += Time.deltaTime;
            if (jumpFromBracedLedgeElapsedTime >= jumpFromBracedLedgeDuration)
            {
                jumpFromBracedLedgeElapsedTime = 0f;
                isJumpingBackFromLedge = false;
                TransitionToSubState(SubState.JumpLoopDescending, animListCurrent.JumpLoopDescending, 0.3f);
            }
        }
    }


    private bool HasFallingRestrictions()
    {
        if (characterActor.BusyComboLocked || characterActor.BusyAnimStateLocked)
            return true;

        return false;
    }
    private void HandleFallingAnim()
    {
        if (HasFallingRestrictions())
            return;

        if (!characterActor.IsGrounded && characterActor.IsFalling)
        {
            TransitionToSubState(SubState.JumpLoopDescending, animListCurrent.JumpLoopDescending, 0.1f);
        }

    }

    #endregion

    private void HandleBlocking()
    {
        if (!characterActor.IsInCombatState)
            return;


    }



}
