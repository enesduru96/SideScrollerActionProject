using Animancer;
using UnityEngine;

public class Vault_AnimState : CharacterAnimState
{
    public enum SubState
    {
        None,
        VaultingSlowToLeft,
        VaultingFastToLeft,
        VaultingSlowToRight,
        VaultingFastToRight,
    }
    public SubState currentSubState = SubState.None;
    private bool TryChangeSubState(SubState newSubState)
    {
        if (currentSubState == newSubState)
            return false;

        switch (currentSubState)
        {

            case SubState.None:
                currentSubState = newSubState;
                return true;


            default:
                currentSubState = newSubState;
                return true;

        }

    }
    private void TransitionToSubState(SubState newSubState, ClipTransition anim, float transitionDuration)
    {
        if (!TryChangeSubState(newSubState))
            return;

        characterAnimStateController.CurrentAnim = anim;
        animationPlayer.TransitionToAnimation(anim, transitionDuration, Easing.Function.Linear, 0);


    }


    [SerializeField] StateAnimations_Vaulting vaultingAnimList;
    VaultJumping vaultState;

    protected override void Awake()
    {
        base.Awake();


    }

    protected override void Start()
    {
        base.Start();

        vaultState = characterStateController.GetComponent<VaultJumping>();
    }


    public override void EnterAnimState(CharacterAnimState fromState)
    {
        
    }

    public override void UpdateAnimState()
    {
        switch (vaultState.vaultSubState)
        {
            case VaultJumping.VaultSubState.None:
                break;

            case VaultJumping.VaultSubState.VaultingSlowToRight:
                TransitionToSubState(SubState.VaultingSlowToRight, vaultingAnimList.VaultSlow, 0.1f);
                break;

            case VaultJumping.VaultSubState.VaultingFastToRight:
                TransitionToSubState(SubState.VaultingFastToRight, vaultingAnimList.VaultFast, 0.1f);
                break;

            case VaultJumping.VaultSubState.VaultingSlowToLeft:
                break;

            case VaultJumping.VaultSubState.VaultingFastToLeft:
                break;


            default:
                break;
        }
    }

    public override void ExitAnimState(CharacterAnimState toState)
    {
        currentSubState = SubState.None;
    }


}
