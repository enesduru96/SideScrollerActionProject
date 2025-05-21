using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using ShadowFort.Utilities;
using System;

public class BlockingState : CharacterState
{
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
        if (!CharacterActor.IsGrounded || (CharacterActor.GroundObject == null || !CharacterActor.GroundObject.CompareTag("Floor")))
            return false;


        return false;
    }



    //TODO: Use input buffer in case the player presses the button early
    public override void EnterBehaviour(float dt, CharacterState fromState)
    {
        base.EnterBehaviour(dt, fromState);

    }

    public override void UpdateBehaviour(float dt)
    {

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