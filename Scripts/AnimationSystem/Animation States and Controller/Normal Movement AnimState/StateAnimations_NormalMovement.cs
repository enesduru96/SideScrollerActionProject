using UnityEngine;
using Animancer;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StateAnimations_NormalMovement", menuName = "NocturneKeepInteractive/AnimationSystem/StateAnimations_NormalMovement")]
public class StateAnimations_NormalMovement : StateAnimations_NormalMovement_Base
{
    [Header("Idle")]
    public ClipTransition idle;
    public ClipTransition idleBlocking;

    [Header("Walking")]
    public ClipTransition walkForward;
    public ClipTransition walkForwardBlocking;

    public ClipTransition walkBackward;
    public ClipTransition walkBackwardBlocking;

    [Header("Running")]
    public ClipTransition runForward;
    public ClipTransition runBackward;

    [Header("Turning")]
    public ClipTransition runningTurnBackToLeft;
    public ClipTransition runningTurnBackToRight;

    [Header("Sprinting")]
    public ClipTransition sprintForward;

    [Header("Jumping")]
    public ClipTransition jumpStart;
    public ClipTransition jumpLoopAscending;
    public ClipTransition jumpLoopDescending;
    public ClipTransition landingAnimaiton;

    [Header("Dashing")]
    public ClipTransition dashStart;
    public ClipTransition dashLoop;
    public ClipTransition dashEnd;

    [Header("Switch")]
    public ClipTransition switchIdleToCombat;

    [Header("Hit Lists")]
    public List<ClipTransition> hitLightList;
    public List<ClipTransition> hitHeavyList;


    public override ClipTransition Idle => idle;
    public override ClipTransition IdleBlocking => idleBlocking;
    public override ClipTransition WalkForward => walkForward;
    public override ClipTransition WalkForwardBlocking => walkForwardBlocking;

    public override ClipTransition WalkBackward => walkBackward;
    public override ClipTransition WalkBackwardBlocking => walkBackwardBlocking;

    public override ClipTransition RunForward => runForward;
    public override ClipTransition RunBackward => runBackward;


    public override ClipTransition RunningTurnBackToLeft => runningTurnBackToLeft;
    public override ClipTransition RunningTurnBackToRight => runningTurnBackToRight;


    public override ClipTransition JumpStart => jumpStart;
    public override ClipTransition JumpLoopAscending => jumpLoopAscending;
    public override ClipTransition JumpLoopDescending => jumpLoopDescending;
    public override ClipTransition LandingAnimaiton => landingAnimaiton;


    public override ClipTransition DashStart => dashStart;
    public override ClipTransition DashLoop => dashLoop;
    public override ClipTransition DashEnd => dashEnd;


    public override List<ClipTransition> HitLightList => hitLightList;
    public override List<ClipTransition> HitHeavyList => hitHeavyList;

    public override ClipTransition SprintForward => sprintForward;

}
