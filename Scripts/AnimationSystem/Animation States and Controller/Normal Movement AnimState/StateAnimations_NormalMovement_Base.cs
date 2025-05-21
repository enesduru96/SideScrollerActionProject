using UnityEngine;
using Animancer;
using System.Collections.Generic;


public abstract class StateAnimations_NormalMovement_Base : ScriptableObject
{
    public abstract ClipTransition Idle { get; }
    public abstract ClipTransition IdleBlocking { get; }
    
    public abstract ClipTransition WalkForward { get; }
    public abstract ClipTransition WalkForwardBlocking { get; }
    public abstract ClipTransition WalkBackward { get; }
    public abstract ClipTransition WalkBackwardBlocking { get; }

    public abstract ClipTransition RunForward { get; }
    public abstract ClipTransition SprintForward { get; }
    public abstract ClipTransition RunBackward { get; }

    public abstract ClipTransition JumpStart { get; }
    public abstract ClipTransition JumpLoopAscending { get; }
    public abstract ClipTransition JumpLoopDescending { get; }
    public abstract ClipTransition LandingAnimaiton { get; }


    public abstract ClipTransition DashStart { get; }
    public abstract ClipTransition DashLoop { get; }
    public abstract ClipTransition DashEnd { get; }


    public abstract List<ClipTransition> HitLightList { get; }
    public abstract List<ClipTransition> HitHeavyList { get; }



    public abstract ClipTransition RunningTurnBackToLeft { get; }
    public abstract ClipTransition RunningTurnBackToRight { get; }
}
