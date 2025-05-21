using System.Collections.Generic;
using UnityEngine;
using ShadowFort.Utilities;



public enum DashMode
{
    FacingDirection,
    InputDirection
}

[AddComponentMenu("Character Controller Pro/Demo/Character/States/Dash")]
public class Dash : CharacterState
{

    [Min(0f)]
    [SerializeField]
    protected float initialVelocity = 12f;

    [Min(0f)]
    [SerializeField]
    protected float duration;
    public float Duration => duration;

    [SerializeField]
    protected AnimationCurve movementCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [Min(0f)]
    [SerializeField]
    protected int availableNotGroundedDashes = 1;

    [SerializeField]
    protected bool ignoreSpeedMultipliers = false;

    [SerializeField]
    protected bool forceNotGrounded = true;

    [Tooltip("Should the dash stop when we hit an obstacle (wall collision)?")]
    [SerializeField]
    protected bool cancelOnContact = true;


    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


    protected MaterialController materialController = null;

    protected int airDashesLeft;
    protected float dashCursor = 0;

    protected Vector3 dashDirection = Vector2.right;

    protected bool isDone = true;

    protected float currentSpeedMultiplier = 1f;

    #region Events



    /// <summary>
    /// This event is called when the dash action has ended.
    /// 
    /// The direction of the dash is passed as an argument.
    /// </summary>
    public event System.Action OnDashEnd;

    #endregion

    void OnEnable()
    {
        CharacterActor.OnGroundedStateEnter += OnGroundedStateEnter;
    }
    void OnDisable()
    {
        CharacterActor.OnGroundedStateEnter -= OnGroundedStateEnter;
    }

    public override string GetInfo()
    {
        return "This state is entirely based on particular movement, the \"dash\". This movement is normally a fast impulse along " +
        "the forward direction. In this case the movement can be defined by using an animation curve (time vs velocity)";
    }

    void OnGroundedStateEnter(Vector3 localVelocity) => airDashesLeft = availableNotGroundedDashes;

    bool EvaluateCancelOnContact() => CharacterActor.WallContacts.Count != 0;

    protected override void Awake()
    {
        base.Awake();

        materialController = this.GetComponentInBranch<CharacterActor, MaterialController>();
        airDashesLeft = availableNotGroundedDashes;
    }



    public override bool CheckEnterTransition(CharacterState fromState)
    {
        if (CharacterActor.BusyComboLocked || CharacterActor.BusyCombatStateTransitionLocked)
            return false;

        if (!CharacterActor.IsGrounded && airDashesLeft <= 0)
            return false;

        if (!CharacterActor.IsInCombatState)
            return false;

        return true;
    }

    public override void CheckExitTransition()
    {
        if (isDone)
        {
            CharacterStateController.EnqueueTransition<NormalMovementXY>();
        }
    }


    public override void EnterBehaviour(float dt, CharacterState fromState)
    {
        base.EnterBehaviour(dt, fromState);


        if (forceNotGrounded)
            CharacterActor.alwaysNotGrounded = true;

        CharacterActor.UseRootMotion = false;

        if (CharacterActor.IsGrounded)
        {

            if (!ignoreSpeedMultipliers)
            {
                currentSpeedMultiplier = materialController != null ? materialController.CurrentSurface.speedMultiplier * materialController.CurrentVolume.speedMultiplier : 1f;
            }

        }
        else
        {

            if (!ignoreSpeedMultipliers)
            {
                currentSpeedMultiplier = materialController != null ? materialController.CurrentVolume.speedMultiplier : 1f;
            }

            airDashesLeft--;
        }


        bool inputIsLeft = CharacterActions.movement.Left;
        bool inputIsRight = CharacterActions.movement.Right;

        if (inputIsLeft)
        {
            dashDirection = Vector3.left;
        }

        else if (inputIsRight)
        {
            dashDirection = Vector3.right;
        }

        else
            dashDirection = CharacterActor.IsFacingRight() ? Vector3.right : Vector3.left;

        Vector3 characterLookAt;
        characterLookAt = dashDirection == Vector3.right ? Vector3Utility.AlmostRight : Vector3Utility.AlmostLeft;
        characterMoverAndRotator.StartRotate(CharacterActor.Forward, characterLookAt, 0.1f, InterpolationType.Linear);

        ResetDash();

        if (CharacterActor.IsPlayer)
        {
            playerLocalEventManager.CharacterInput.TriggerPlayerDashStarted();
        }
    }

    public override void ExitBehaviour(float dt, CharacterState toState)
    {
        base.ExitBehaviour(dt, toState);

        if (forceNotGrounded)
            CharacterActor.alwaysNotGrounded = false;

        if (OnDashEnd != null)
            OnDashEnd?.Invoke();
    }

    public override void UpdateBehaviour(float dt)
    {
        Vector3 dashVelocity = initialVelocity * currentSpeedMultiplier * movementCurve.Evaluate(dashCursor) * dashDirection;

        CharacterActor.Velocity = dashVelocity;

        float animationDt = dt / duration;
        dashCursor += animationDt;

        if (dashCursor >= 1)
        {
            isDone = true;
            dashCursor = 0;
        }

    }

    public override void PostUpdateBehaviour(float dt)
    {
        if (cancelOnContact)
            isDone |= EvaluateCancelOnContact();
    }

    public virtual void ResetDash()
    {
        CharacterActor.Velocity = Vector3.zero;
        isDone = false;
        dashCursor = 0;
    }
}





