using UnityEngine;
using Animancer;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StateAnimations_NormalMovement_Combat", menuName = "NocturneKeepInteractive/AnimationSystem/StateAnimations_NormalMovement_Combat")]
public class StateAnimations_NormalMovement_Combat : StateAnimations_NormalMovement_Base
{
    [Header("Idle Combat")]
    public ClipTransition idle;

    [Header("Walking Combat")]
    public ClipTransition walkForward;
    public ClipTransition walkBackward;

    [Header("Running Combat")]
    public ClipTransition runForward;
    public ClipTransition runBackward;

    [Header("Turning")]
    public ClipTransition runningTurnBackToLeft;
    public ClipTransition runningTurnBackToRight;

    [Header("Sprinting Combat")]
    public ClipTransition sprintForward;

    [Header("Jumping Combat")]
    public ClipTransition jumpStart;
    public ClipTransition jumpLoopAscending;
    public ClipTransition jumpLoopDescending;
    public ClipTransition landingAnimation;

    [Header("Blocking")]
    public ClipTransition idleBlocking;
    public ClipTransition walkForwardBlocking;
    public ClipTransition walkBackwardBlocking;
    public ClipTransition getHitBlocking;
    public ClipTransition getHitBlockingHeavy;
    public ClipTransition blockBreak;

    [Header("Dashing")]
    public ClipTransition dashStart;
    public ClipTransition dashLoop;
    public ClipTransition dashEnd;

    [Header("Switch")]
    public ClipTransition switchCombatToIdle;


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
    public override ClipTransition LandingAnimaiton => landingAnimation;



    public override ClipTransition DashStart => dashStart;
    public override ClipTransition DashLoop => dashLoop;
    public override ClipTransition DashEnd => dashEnd;


    public override List<ClipTransition> HitLightList => hitLightList;
    public override List<ClipTransition> HitHeavyList => hitHeavyList;

    public override ClipTransition SprintForward => sprintForward;
}
