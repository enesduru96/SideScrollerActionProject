using UnityEngine;
using ShadowFort.Utilities;
using Animancer;
using System.Collections;

public class Ledge_AnimState : CharacterAnimState
{

    [Header("State Animations")]
    [SerializeField] private StateAnimations_Ledge ledgeAnimList;
    private LedgeHanging ledgeState;

    public enum SubState
    {
        None,
        BracedEnterAbove,
        BracedEnterForward,
        BracedEnterForwardTough,
        BracedJumpingDown,
        BracedJumpingBack,
        BracedClimbingUp,
        BracedJumpingUpToLedge,

        FreeEnterAbove,
        FreeEnterForward,
        FreeClimbingUp,

        BracedIdleToLeft,
        BracedIdleToRight,

        BracedIdleLookingBackToLeft,
        BracedIdleLookingBackToRight,
        BracedIdleLookingDownToLeft,
        BracedIdleLookingDownToRight,
        BracedIdleLookingUp,
        FreeIdle,
        LookDownBraced,
        LookDownFree,
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


    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        ledgeState = characterStateController.GetComponent<LedgeHanging>();
    }


    public override void EnterAnimState(CharacterAnimState fromState)
    {

    }

    public override void ExitAnimState(CharacterAnimState toState)
    {
        StartCoroutine(ResetSubState());
    }
    private IEnumerator ResetSubState()
    {
        yield return new WaitForSeconds(0.15f);

        currentSubState = SubState.None;
    }

    private void HandleBracedIdleLookDirection()
    {
        bool willLookBackToLeft = (characterActor.IsFacingRight() && characterStateController.CharacterBrain.CharacterActions.movement.Left);
        bool willLookBackToRight = (characterActor.IsFacingLeft() && characterStateController.CharacterBrain.CharacterActions.movement.Right);

        switch (characterActor.IsFacingLeft())
        {
            case true:
                if (characterStateController.CharacterBrain.CharacterActions.movement.Down)
                    TransitionToSubState(SubState.BracedIdleLookingDownToLeft, ledgeAnimList.LookDownBracedLoopToLeft, 0.2f);

                else if (characterStateController.CharacterBrain.CharacterActions.movement.Up && ledgeState.HasJumpableLedgeAbove)
                    TransitionToSubState(SubState.BracedIdleLookingUp, ledgeAnimList.LookUpBracedLoopToLeft, 0.2f);

                else if (characterStateController.CharacterBrain.CharacterActions.movement.Right)
                    TransitionToSubState(SubState.BracedIdleLookingBackToLeft, ledgeAnimList.LookBackLoopToRight, 0.2f);
                else
                    TransitionToSubState(SubState.BracedIdleToLeft, ledgeAnimList.IdleBracedToLeft, 0.2f);

                break;


            case false:
                if (characterStateController.CharacterBrain.CharacterActions.movement.Down)
                    TransitionToSubState(SubState.BracedIdleLookingDownToLeft, ledgeAnimList.LookDownBracedLoopToRight, 0.2f);

                else if (characterStateController.CharacterBrain.CharacterActions.movement.Up && ledgeState.HasJumpableLedgeAbove)
                    TransitionToSubState(SubState.BracedIdleLookingUp, ledgeAnimList.LookUpBracedLoopToRight, 0.2f);

                else if (characterStateController.CharacterBrain.CharacterActions.movement.Left)
                    TransitionToSubState(SubState.BracedIdleLookingBackToLeft, ledgeAnimList.LookBackLoopToLeft, 0.2f);
                else
                    TransitionToSubState(SubState.BracedIdleToLeft, ledgeAnimList.IdleBracedToRight, 0.2f);
                break;

        }

    }

    public override void UpdateAnimState()
    {
        switch (ledgeState.ledgeSubState)
        {
            case LedgeHanging.LedgeSubState.BracedIdle:
                HandleBracedIdleLookDirection();
                break;

            case LedgeHanging.LedgeSubState.BracedEnteringForward:
                if(characterActor.IsFacingLeft())
                    TransitionToSubState(SubState.BracedEnterForward, ledgeAnimList.EnterLedgeBracedForwardToLeft, 0.1f);
                else
                    TransitionToSubState(SubState.BracedEnterForward, ledgeAnimList.EnterLedgeBracedForwardToRight, 0.1f);
                break;

            case LedgeHanging.LedgeSubState.BracedEnteringAbove:
                if(characterActor.IsFacingLeft())
                    TransitionToSubState(SubState.BracedEnterAbove, ledgeAnimList.EnterLedgeBracedAboveToLeft, 0.1f);
                else
                    TransitionToSubState(SubState.BracedEnterAbove, ledgeAnimList.EnterLedgeBracedAboveToRight, 0.1f);
                break;

            case LedgeHanging.LedgeSubState.BracedJumpingDown:
                TransitionToSubState(SubState.BracedJumpingDown, ledgeAnimList.JumpDownBraced, 0.1f);
                break;

            case LedgeHanging.LedgeSubState.BracedJumpingBackToRight:
                TransitionToSubState(SubState.BracedJumpingBack, ledgeAnimList.JumpBackStartToRight, 0.1f);
                break;

            case LedgeHanging.LedgeSubState.BracedJumpingBackToLeft:
                TransitionToSubState(SubState.BracedJumpingBack, ledgeAnimList.JumpBackStartToLeft, 0.1f);
                break;

            case LedgeHanging.LedgeSubState.BracedClimbingUp:
                TransitionToSubState(SubState.BracedClimbingUp, ledgeAnimList.ClimbUpBraced, 0.1f);
                break;

            case LedgeHanging.LedgeSubState.BracedJumpingUpToLedge:
                TransitionToSubState(SubState.BracedJumpingUpToLedge, ledgeAnimList.JumpUpToLedgeBraced, 0.1f);
                break;
        }


    }
}
