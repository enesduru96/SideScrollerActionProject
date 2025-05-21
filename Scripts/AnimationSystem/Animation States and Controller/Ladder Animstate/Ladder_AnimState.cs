using UnityEngine;
using ShadowFort.Utilities;
using System.Collections;
using Animancer;


public class Ladder_AnimState : CharacterAnimState
{
    [Header("State Animations")]
    [SerializeField] private StateAnimations_Ladder ladderAnimList;
    
    private LadderClimbing ladderState;

    public enum SubState
    {
        None,
        TopEntry,
        BottomEntry,
        AirborneCatch,
        Idle,
        MovingUp,
        MovingDown,
        TopExit,
        BottomExit,
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

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        ladderState = characterStateController.GetComponent<LadderClimbing>();
    }


    public override void EnterAnimState(CharacterAnimState fromState)
    {
        characterActor.SetUpRootMotion(true, PhysicsActor.RootMotionVelocityType.SetVelocity, false);

        if (!characterActor.IsGrounded)
        {
            TransitionToSubState(SubState.AirborneCatch, ladderAnimList.LadderCatch, 0.2f);
            return;
        }
        
        if (ladderState.IsLaderBottom)
        {
            TransitionToSubState(SubState.BottomEntry, ladderAnimList.LadderBottomEntry, 0f);
        }
        else
        {
            TransitionToSubState(SubState.TopEntry, ladderAnimList.LadderTopEntry, 0f);
        }
    }

    public override void UpdateAnimState()
    {
        switch (ladderState.ladderSubState)
        {
            case LadderClimbing.LadderSubState.BottomEntry:
                break;

            case LadderClimbing.LadderSubState.TopEntry:
                TransitionToSubState(SubState.TopEntry, ladderAnimList.LadderTopEntry, 0f);
                break;

            case LadderClimbing.LadderSubState.AirborneCatch:
                break;


            case LadderClimbing.LadderSubState.Idle:
                TransitionToSubState(SubState.Idle, ladderAnimList.LadderIdle, 0.05f);
                break;


            case LadderClimbing.LadderSubState.LadderUp:
                TransitionToSubState(SubState.MovingUp, ladderAnimList.LadderUp, 0.05f);
                break;

            case LadderClimbing.LadderSubState.LadderDown:
                TransitionToSubState(SubState.MovingDown, ladderAnimList.LadderDown, 0.05f);
                break;


            case LadderClimbing.LadderSubState.TopExit:
                TransitionToSubState(SubState.TopExit, ladderAnimList.LadderTopExit, 0.05f);
                break;

            case LadderClimbing.LadderSubState.BottomExit:
                TransitionToSubState(SubState.BottomExit, ladderAnimList.LadderBottomExit, 0.05f);
                break;
        }
    }

    public override void ExitAnimState(CharacterAnimState toState)
    {
        currentSubState = SubState.None;
        characterActor.UseRootMotion = false;
    }

}
