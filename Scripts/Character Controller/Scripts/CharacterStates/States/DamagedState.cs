using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using ShadowFort.Utilities;
using System;


public class DamagedState : CharacterState
{
    // TODO: Use a general combat human animations pool for victim and damaged anims ???
    [Header("Normal Damaged Animations")]
    [SerializeField] private List<AnimationClip> normalFrontDamagedAnimations = new List<AnimationClip>();
    [SerializeField] private List<AnimationClip> normalBackDamagedAnimations = new List<AnimationClip>();

    [Header("Heavy Damaged Animations")]
    [SerializeField] private List<AnimationClip> heavyFrontDamagedAnimations = new List<AnimationClip>();
    [SerializeField] private List<AnimationClip> heavyBackDamagedAnimations = new List<AnimationClip>();

    [Header("Execution Victim Animations")]
    [SerializeField] private List<AnimationClip> victimAnimations = new List<AnimationClip>();

    [Header("Base Parameters")]
    private float duration;
    private bool forceExit;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }


    private void ResetAllValues()
    {

    }



    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Start()
    {
        base.Start();

        if (CharacterActor.Animator == null)
        {
            Debug.Log("The VaultJumping state needs the character to have a reference to an Animator component. Destroying this state...");
            Destroy(this);
        }
    }

    public override bool CheckEnterTransition(CharacterState fromState)
    {
        return true;
    }


    public override void EnterBehaviour(float dt, CharacterState fromState)
    {
        base.EnterBehaviour(dt, fromState);
    }

    public override void UpdateBehaviour(float dt)
    {

        if (StateElapsedTime >= 0.15f)
        {
            forceExit = true;
        }
    }

    public override void CheckExitTransition()
    {
        if (forceExit)
        {
            CharacterStateController.EnqueueTransition<NormalMovementXY>();
            forceExit = false;
        }
    }


    public override void ExitBehaviour(float dt, CharacterState toState)
    {
        base.ExitBehaviour(dt, toState);

        ResetAllValues();
    }

}