using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadowFort.Utilities;


public class WallSliding : CharacterState
{

    [Header("Filter")]

    [SerializeField]
    protected bool filterByTag = true;

    [Condition("filterByTag", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField]
    protected string wallTag = "WallSlide";


    [Header("Slide")]

    [SerializeField]
    protected float slideAcceleration = 10f;

    [Range(0f, 1f)]
    [SerializeField]
    protected float initialIntertia = 0.4f;

    [Header("Grab")]

    public bool enableGrab = true;

    public bool enableClimb = true;

    [Condition("enableClimb", ConditionAttribute.ConditionType.IsTrue)]
    public float wallClimbHorizontalSpeed = 1f;

    [Condition("enableClimb", ConditionAttribute.ConditionType.IsTrue)]
    public float wallClimbVerticalSpeed = 3f;

    [Condition("enableClimb", ConditionAttribute.ConditionType.IsTrue)]
    public float wallClimbAcceleration = 10f;



    [Header("Size")]

    [SerializeField]
    protected bool modifySize = true;

    [Condition("modifySize", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField]
    protected float height = 1.5f;

    [Header("Jump")]

    [SerializeField]
    protected float jumpNormalVelocity = 5f;

    [SerializeField]
    protected float jumpVerticalVelocity = 10f;

    [Header("Animation")]

    [SerializeField]
    protected string horizontalVelocityParameter = "HorizontalVelocity";

    [SerializeField]
    protected string verticalVelocityParameter = "VerticalVelocity";

    [SerializeField]
    protected string grabParameter = "Grab";

    [SerializeField]
    protected string movementDetectedParameter = "MovementDetected";

    protected bool wallJump = false;
    protected Vector2 initialSize = Vector2.zero;
    protected Dictionary<Transform, SlideableWall> walls = new Dictionary<Transform, SlideableWall>();

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


    private void OnEnable()
    {
        SlideableWall.OnWallEnabled += AddWall;
        SlideableWall.OnWallDisabled += RemoveWall;
    }

    private void OnDisable()
    {
        SlideableWall.OnWallEnabled -= AddWall;
        SlideableWall.OnWallDisabled -= RemoveWall;
    }


    private void AddWall(SlideableWall wall)
    {
        if (!walls.ContainsKey(wall.transform))
            walls.Add(wall.transform, wall);
    }

    private void RemoveWall(SlideableWall wall)
    {
        if (walls.ContainsKey(wall.transform))
            walls.Remove(wall.transform);
    }

    public override bool CheckEnterTransition(CharacterState fromState)
    {
        if (CharacterActor.IsGrounded && !CharacterActions.movement.Up || !IsGrabbing)
        {
            return false;
        }

        if (CharacterActor.IsAscending)
        {
            return false;
        }

        if (!CharacterActor.WallCollision)
            return false;

        if (!CharacterActor.WallContact.gameObject.CompareTag("WallSlide"))
            return false;

        // Replaced by trigger system not to create unwanted collisions with slide walls

        //if (CharacterActor.Triggers.Count == 0)
        //{
        //    return false;
        //}

        //if (!CharacterActor.CurrentTrigger.gameObject.CompareTag("WallSlide"))
        //    return false;


        for (int i = 0; i < CharacterActor.Triggers.Count; i++)
        {
            Trigger trigger = CharacterActor.Triggers[i];
            SlideableWall slideableWall = walls.GetOrRegisterValue(trigger.transform);

        }


        //if (filterByTag)
        //    if (!CharacterActor.WallContact.gameObject.CompareTag(wallTag))
        //        return false;

        //if (!CheckCenterRay())
        //    return false;

        return true;
    }

    public override void EnterBehaviour(float dt, CharacterState fromState)
    {
        base.EnterBehaviour(dt, fromState);


        CharacterActor.ForceNotGrounded();

        CharacterActor.UseRootMotion = false;

        CharacterActor.Velocity *= initialIntertia;
        CharacterActor.SetYaw(-CharacterActor.WallContact.normal);
        //CharacterActor.SetYaw(CharacterActor.CurrentTrigger.gameObject.transform.right);

        if (modifySize)
        {
            initialSize = CharacterActor.BodySize;
            CharacterActor.SetSize(new Vector2(initialSize.x, height), CharacterActor.SizeReferenceType.Center);
        }
    }

    public override void UpdateBehaviour(float dt)
    {
        if (IsGrabbing)
        {
            Vector3 rightDirection = Vector3.ProjectOnPlane(CharacterStateController.MovementReferenceRight, CharacterActor.WallContact.normal);
            rightDirection.Normalize();

            Vector3 upDirection = CharacterActor.Up;
            Vector3 targetVelocity = enableClimb ? CharacterActions.movement.value.x * rightDirection * wallClimbHorizontalSpeed +
            CharacterActions.movement.value.y * upDirection * wallClimbVerticalSpeed : Vector3.zero;

            CharacterActor.Velocity = Vector3.MoveTowards(CharacterActor.Velocity, targetVelocity, wallClimbAcceleration * dt);

        }
        else
        {
            //CharacterActor.VerticalVelocity += -CharacterActor.Up * slideAcceleration * dt;
            CharacterActor.Velocity = Vector3.zero;
        }
    }

    public override void PostUpdateBehaviour(float dt)
    {
        if (!CharacterActor.IsAnimatorValid())
            return;

        CharacterActor.Animator.SetFloat(horizontalVelocityParameter, CharacterActor.LocalVelocity.x);
        CharacterActor.Animator.SetFloat(verticalVelocityParameter, CharacterActor.LocalVelocity.y);
        CharacterActor.Animator.SetBool(grabParameter, IsGrabbing);
        CharacterActor.Animator.SetBool(movementDetectedParameter, CharacterActions.movement.Detected);

    }

    public override void CheckExitTransition()
    {
        if (CharacterActor.IsGrounded || !IsGrabbing)
        {
            CharacterStateController.EnqueueTransition<NormalMovementXY>();
        }
        else if (CharacterActions.jump.StartedElapsedTime <= 0.025f)
        {
            wallJump = true;

            CharacterStateController.EnqueueTransition<NormalMovementXY>();

        }
        else
        {
            CharacterStateController.EnqueueTransition<LedgeHanging>();
        }
    }

    public override void ExitBehaviour(float dt, CharacterState toState)
    {
        base.ExitBehaviour(dt, toState);

        if (wallJump)
        {
            wallJump = false;

            // Do a 180 degrees turn.
            CharacterActor.TurnAround();

            // Apply the wall jump velocity.
            CharacterActor.Velocity = jumpVerticalVelocity * CharacterActor.Up + jumpNormalVelocity * CharacterActor.transform.forward;
        }

        if (modifySize)
        {
            CharacterActor.SizeReferenceType sizeReferenceType = CharacterActor.IsGrounded ?
                CharacterActor.SizeReferenceType.Bottom : CharacterActor.SizeReferenceType.Top;
            CharacterActor.SetSize(initialSize, sizeReferenceType);
        }
    }



    protected bool IsGrabbing => CharacterActions.run.value && enableGrab;

    protected virtual bool CheckCenterRay()
    {
        HitInfoFilter filter = new HitInfoFilter(
            CharacterActor.PhysicsComponent.CollisionLayerMask,
            true,
            true
        );

        CharacterActor.PhysicsComponent.Raycast(
            out HitInfo centerRayHitInfo,
            CharacterActor.Center,
            -CharacterActor.WallContact.normal * 1.2f * CharacterActor.BodySize.x,
            in filter
        );

        return centerRayHitInfo.hit && centerRayHitInfo.transform.gameObject == CharacterActor.WallContact.gameObject;
    }

    public override void UpdateIK(int layerIndex)
    {
        if (!CharacterActor.IsAnimatorValid())
            return;

        if (IsGrabbing && CharacterActions.movement.Detected)
        {
            CharacterActor.Animator.SetLookAtWeight(Mathf.Clamp01(CharacterActor.Velocity.magnitude), 0f, 0.2f);
            CharacterActor.Animator.SetLookAtPosition(CharacterActor.Position + CharacterActor.Velocity);
        }
        else
        {
            CharacterActor.Animator.SetLookAtWeight(0f);
        }

    }



}
