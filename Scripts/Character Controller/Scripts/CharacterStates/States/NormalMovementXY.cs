using UnityEngine;
using ShadowFort.Utilities;


public class NormalMovementXY : CharacterState
{

    [Space(10)]

    public PlanarMovementParameters planarMovementParameters = new PlanarMovementParameters();

    public VerticalMovementParameters verticalMovementParameters = new VerticalMovementParameters();

    public CrouchParameters crouchParameters = new CrouchParameters();

    public LookingDirectionParameters lookingDirectionParameters = new LookingDirectionParameters();

    [Space(10)]

    [Header("References")]

    [SerializeField] private InputBufferController inputBufferController;


    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


    #region Events	

    public event System.Action OnJumpPerformed;

    public event System.Action OnGroundedJumpPerformed;

    public event System.Action<int> OnNotGroundedJumpPerformed;

    #endregion


    protected MaterialController materialController = null;
    protected int notGroundedJumpsLeft = 0;
    protected bool isAllowedToCancelJump = false;
    protected bool wantToRun = false;
    protected float currentPlanarSpeedLimit = 0f;

    protected bool groundedJumpAvailable = false;

    protected Vector3 jumpDirection = default;
    protected Vector3 targetLookingDirection = default;

    protected float targetHeight = 1f;

    protected bool wantToCrouch = false;
    protected bool isCrouched = false;

    protected PlanarMovementParameters.PlanarMovementProperties currentMotion = new PlanarMovementParameters.PlanarMovementProperties();
    private bool reducedAirControlFlag = false;
    private float reducedAirControlInitialTime = 0f;
    private float reductionDuration = 0.5f;


    protected override void Awake()
    {
        base.Awake();

        notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps;

        materialController = this.GetComponentInBranch<CharacterActor, MaterialController>();


    }

    protected virtual void OnValidate()
    {
        verticalMovementParameters.OnValidate();
    }

    protected override void Start()
    {
        base.Start();

        targetHeight = CharacterActor.DefaultBodySize.y;

        float minCrouchHeightRatio = CharacterActor.BodySize.x / CharacterActor.BodySize.y;
        crouchParameters.heightRatio = Mathf.Max(minCrouchHeightRatio, crouchParameters.heightRatio);

        inputBufferController = CharacterActor.GetComponentInBranch<InputBufferController>();
    }

    protected virtual void OnEnable()
    {
        CharacterActor.OnTeleport += OnTeleport;
    }

    protected virtual void OnDisable()
    {
        CharacterActor.OnTeleport -= OnTeleport;
    }

    void OnTeleport(Vector3 position, Quaternion rotation)
    {
        targetLookingDirection = CharacterActor.Forward;
        isAllowedToCancelJump = false;
    }

    public bool UseGravity
    {
        get => verticalMovementParameters.useGravity;
        set => verticalMovementParameters.useGravity = value;
    }


    public override void EnterBehaviour(float dt, CharacterState fromState)
    {
        base.EnterBehaviour(dt, fromState);


        targetLookingDirection = CharacterActor.Forward;

        CharacterActor.alwaysNotGrounded = false;

        // Grounded jump
        groundedJumpAvailable = false;
        if (CharacterActor.IsGrounded)
        {
            if (verticalMovementParameters.canJumpOnUnstableGround || CharacterActor.IsStable)
            {
                groundedJumpAvailable = true;
            }
        }

        // Wallside to NormalMovement transition
        if (fromState == CharacterStateController.GetState<WallSliding>())
        {
            // "availableNotGroundedJumps + 1" because the update code will consume one jump!
            notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps + 1;

            // Reduce the amount of air control (acceleration and deceleration) for 0.5 seconds.
            ReduceAirControl(0.5f);
        }

        currentPlanarSpeedLimit = Mathf.Max(CharacterActor.PlanarVelocity.magnitude, planarMovementParameters.baseSpeedLimit);

        CharacterActor.UseRootMotion = false;
    }

    public override void UpdateBehaviour(float dt)
    {

        HandleSize(dt);
        HandleVelocity(dt);
        HandleRotation(dt);

        if (CharacterActions.jump.Started && !CharacterActions.crouch.value)
            inputBufferController.BufferInput(BufferedInputType.Jump, inputBufferController.JumpBufferTime);

    }

    private bool isOnCoyoteTime = false;

    protected override void Update()
    {
        base.Update();

        if (CharacterActor.CurrentState == CharacterActorState.NotGrounded)
        {
            if (CharacterActor.NotGroundedTime <= verticalMovementParameters.coyoteTime && groundedJumpAvailable)
            {
                isOnCoyoteTime = true;
            }
            else if (CharacterActor.NotGroundedTime > verticalMovementParameters.coyoteTime)
            {
                isOnCoyoteTime = false;
            }
        }
    }

    public override void PreCharacterSimulation(float dt)
    {
        if (!CharacterActor.IsAnimatorValid())
            return;

    }

    public override void PostCharacterSimulation(float dt)
    {
        if (!CharacterActor.IsAnimatorValid())
            return;


    }


    public override void CheckExitTransition()
    {
        if (CharacterActor.Triggers.Count != 0)
        {
            CharacterStateController.EnqueueTransition<VaultJumping>();
            CharacterStateController.EnqueueTransition<WallSliding>();
            CharacterStateController.EnqueueTransition<LadderClimbing>();
            CharacterStateController.EnqueueTransition<RopeClimbing>();
        }

        if (CharacterActions.jetPack.value)
        {
            CharacterStateController.EnqueueTransition<JetPack>();
        }
        else if (CharacterActions.dash.StartedElapsedTime <= 0.02f)
        {
            CharacterStateController.EnqueueTransition<Dash>();
        }

        else if (!CharacterActor.IsGrounded)
        {
            if (!CharacterActions.crouch.value)
                CharacterStateController.EnqueueTransition<WallSliding>();


            if (CharacterActor.NotOnLedgeTime >= 0.2f)
                CharacterStateController.EnqueueTransition<LedgeHanging>();
        }
        else if (
                CharacterActor.IsGrounded &&
                (CharacterActor.GroundObject.CompareTag("Free Ledge") || CharacterActor.GroundObject.CompareTag("Braced Ledge")) &&
                CharacterActions.movement.Down &&
                CharacterActions.interactLite.StartedElapsedTime <= 0.02f
            )
        {
            CharacterStateController.EnqueueTransition<LedgeHanging>();
        }
    }

    public override void ExitBehaviour(float dt, CharacterState toState)
    {
        base.ExitBehaviour(dt, toState);

        reducedAirControlFlag = false;
        CharacterActor.IsBlocking = false;
    }



    public void ReduceAirControl(float reductionDuration = 0.5f)
    {
        reducedAirControlFlag = true;
        reducedAirControlInitialTime = Time.time;
        this.reductionDuration = reductionDuration;
    }

    void SetMotionValues(Vector3 targetPlanarVelocity)
    {
        float angleCurrentTargetVelocity = Vector3.Angle(CharacterActor.PlanarVelocity, targetPlanarVelocity);

        switch (CharacterActor.CurrentState)
        {
            case CharacterActorState.StableGrounded:

                currentMotion.acceleration = planarMovementParameters.stableGroundedAcceleration;
                currentMotion.deceleration = planarMovementParameters.stableGroundedDeceleration;
                currentMotion.angleAccelerationMultiplier = planarMovementParameters.stableGroundedAngleAccelerationBoost.Evaluate(angleCurrentTargetVelocity);

                break;

            case CharacterActorState.UnstableGrounded:
                currentMotion.acceleration = planarMovementParameters.unstableGroundedAcceleration;
                currentMotion.deceleration = planarMovementParameters.unstableGroundedDeceleration;
                currentMotion.angleAccelerationMultiplier = planarMovementParameters.unstableGroundedAngleAccelerationBoost.Evaluate(angleCurrentTargetVelocity);

                break;

            case CharacterActorState.NotGrounded:

                if (reducedAirControlFlag)
                {
                    float time = Time.time - reducedAirControlInitialTime;
                    if (time <= reductionDuration)
                    {
                        currentMotion.acceleration = (planarMovementParameters.notGroundedAcceleration / reductionDuration) * time;
                        currentMotion.deceleration = (planarMovementParameters.notGroundedDeceleration / reductionDuration) * time;
                    }
                    else
                    {
                        reducedAirControlFlag = false;

                        currentMotion.acceleration = planarMovementParameters.notGroundedAcceleration;
                        currentMotion.deceleration = planarMovementParameters.notGroundedDeceleration;
                    }

                }
                else
                {
                    currentMotion.acceleration = planarMovementParameters.notGroundedAcceleration;
                    currentMotion.deceleration = planarMovementParameters.notGroundedDeceleration;
                }

                currentMotion.angleAccelerationMultiplier = planarMovementParameters.notGroundedAngleAccelerationBoost.Evaluate(angleCurrentTargetVelocity);

                break;

        }


        // Material values
        if (materialController != null)
        {
            if (CharacterActor.IsGrounded)
            {
                currentMotion.acceleration *= materialController.CurrentSurface.accelerationMultiplier * materialController.CurrentVolume.accelerationMultiplier;
                currentMotion.deceleration *= materialController.CurrentSurface.decelerationMultiplier * materialController.CurrentVolume.decelerationMultiplier;
            }
            else
            {
                currentMotion.acceleration *= materialController.CurrentVolume.accelerationMultiplier;
                currentMotion.deceleration *= materialController.CurrentVolume.decelerationMultiplier;
            }
        }

    }

    private float horizontalInput;
    private float verticalInput;


    protected virtual void ProcessPlanarMovement(float dt)
    {

        if (//CharacterActor.BusyCombatStateTransitionLocked ||
            CharacterActor.BusyComboLocked ||
            CharacterActor.BusyInputLocked)
            return;

        Vector2 normalizedInput = CharacterActions.movement.value;
        horizontalInput = Mathf.Abs(normalizedInput.x) >= 0.15f ? Mathf.Sign(normalizedInput.x) : 0f;
        verticalInput = Mathf.Abs(normalizedInput.y) >= 0.15f ? Mathf.Sign(normalizedInput.y) : 0f;

        Vector3 movementDirection = CharacterStateController.MovementReferenceRight * horizontalInput;


        float speedMultiplier = materialController != null ?
        materialController.CurrentSurface.speedMultiplier * materialController.CurrentVolume.speedMultiplier : 1f;


        bool needToAccelerate = CustomUtilities.Multiply(movementDirection, currentPlanarSpeedLimit).sqrMagnitude >= CharacterActor.PlanarVelocity.sqrMagnitude;

        Vector3 targetPlanarVelocity = default;
        switch (CharacterActor.CurrentState)
        {
            case CharacterActorState.NotGrounded:

                if (CharacterActor.WasGrounded)
                    currentPlanarSpeedLimit = Mathf.Max(CharacterActor.PlanarVelocity.magnitude, planarMovementParameters.baseSpeedLimit);

                targetPlanarVelocity = CustomUtilities.Multiply(movementDirection, speedMultiplier, currentPlanarSpeedLimit);

                break;
            case CharacterActorState.StableGrounded:


                // Run ------------------------------------------------------------
                if (planarMovementParameters.runInputMode == InputMode.Toggle)
                {
                    if (CharacterActions.run.Started)
                        wantToRun = !wantToRun;
                }
                else
                {
                    wantToRun = CharacterActions.run.value;
                }

                if (wantToCrouch || !planarMovementParameters.canRun)
                    wantToRun = false;


                if (isCrouched)
                {
                    currentPlanarSpeedLimit = planarMovementParameters.baseSpeedLimit * crouchParameters.speedMultiplier;
                }
                else
                {
                    if (CharacterActor.IsBlocking)
                    {
                        currentPlanarSpeedLimit = planarMovementParameters.blockingSpeedLimit;
                    }
                    else
                    {
                        currentPlanarSpeedLimit = wantToRun ? planarMovementParameters.boostSpeedLimit : planarMovementParameters.baseSpeedLimit;
                    }
                }

                targetPlanarVelocity = CustomUtilities.Multiply(movementDirection, speedMultiplier, currentPlanarSpeedLimit);

                break;
            case CharacterActorState.UnstableGrounded:

                currentPlanarSpeedLimit = planarMovementParameters.baseSpeedLimit;

                targetPlanarVelocity = CustomUtilities.Multiply(movementDirection, speedMultiplier, currentPlanarSpeedLimit);


                break;
        }

        SetMotionValues(targetPlanarVelocity);


        float acceleration = currentMotion.acceleration;



        if (needToAccelerate)
        {
            acceleration *= currentMotion.angleAccelerationMultiplier;
        }
        else
        {
            acceleration = currentMotion.deceleration;
        }



        CharacterActor.PlanarVelocity = Vector3.MoveTowards(
            CharacterActor.PlanarVelocity,
            targetPlanarVelocity,
            acceleration * dt
        );
    }


    void ProcessVerticalMovement(float dt)
    {
        ProcessGravity(dt);
        ProcessJump(dt);

    }

    protected virtual void ProcessGravity(float dt)
    {
        if (!verticalMovementParameters.useGravity)
            return;

        verticalMovementParameters.UpdateParameters();

        float gravityMultiplier = 1f;

        if (materialController != null)
            gravityMultiplier = CharacterActor.LocalVelocity.y >= 0 ?
                materialController.CurrentVolume.gravityAscendingMultiplier :
                materialController.CurrentVolume.gravityDescendingMultiplier;

        float gravity = gravityMultiplier * verticalMovementParameters.gravity;

        if (!CharacterActor.IsStable)
            CharacterActor.VerticalVelocity += CustomUtilities.Multiply(-CharacterActor.Up, gravity, dt);


    }

    #region Jumping

    protected bool UnstableGroundedJumpAvailable => !verticalMovementParameters.canJumpOnUnstableGround && CharacterActor.CurrentState == CharacterActorState.UnstableGrounded;

    public enum JumpResult
    {
        Invalid,
        Grounded,
        NotGrounded
    }

    JumpResult CanJump(bool jumpBuffered)
    {
        JumpResult jumpResult = JumpResult.Invalid;

        if (!verticalMovementParameters.canJump)
            return jumpResult;

        if (isCrouched)
            return jumpResult;

        switch (CharacterActor.CurrentState)
        {
            case CharacterActorState.StableGrounded:

                bool jumpStarted = CharacterActions.jump.Started;

                if (groundedJumpAvailable && !CharacterActions.crouch.value && (jumpBuffered || jumpStarted))
                    jumpResult = JumpResult.Grounded;

                break;

            case CharacterActorState.NotGrounded:

                if (!CharacterActions.crouch.value)
                {
                    if (CharacterActor.NotGroundedTime
                            <= verticalMovementParameters.coyoteTime
                        && groundedJumpAvailable)
                    {
                        jumpResult = JumpResult.Grounded;         // coyote
                    }
                    else if (jumpBuffered && notGroundedJumpsLeft != 0)
                    {
                        jumpResult = JumpResult.NotGrounded;      // double jump
                    }
                }
                break;

            case CharacterActorState.UnstableGrounded:

                jumpStarted = CharacterActions.jump.Started;
                if (verticalMovementParameters.canJumpOnUnstableGround && (jumpBuffered || jumpStarted))
                    jumpResult = JumpResult.Grounded;

                break;
        }

        return jumpResult;
    }


    protected virtual void ProcessJump(float dt)
    {
        if (
            CharacterActor.BusyComboLocked ||
            CharacterActor.BusyInputLocked)
            return;

        ProcessRegularJump(dt);
        ProcessJumpDown(dt);
    }


    protected virtual bool ProcessJumpDown(float dt)
    {
        if (!verticalMovementParameters.canJumpDown)
            return false;

        if (!CharacterActor.IsStable)
            return false;

        if (!CharacterActor.IsGroundAOneWayPlatform)
            return false;

        if (verticalMovementParameters.filterByTag)
        {
            if (!CharacterActor.GroundObject.CompareTag(verticalMovementParameters.jumpDownTag))
                return false;
        }

        if (!ProcessJumpDownAction())
            return false;

        JumpDown(dt);

        return true;
    }


    protected virtual bool ProcessJumpDownAction()
    {
        return isCrouched && CharacterActions.jump.StartedElapsedTime <= 0.02f;
    }


    protected virtual void JumpDown(float dt)
    {

        float groundDisplacementExtraDistance = 0f;

        Vector3 groundDisplacement = CustomUtilities.Multiply(CharacterActor.GroundVelocity, dt);

        if (!CharacterActor.IsGroundAscending)
            groundDisplacementExtraDistance = groundDisplacement.magnitude;

        CharacterActor.ForceNotGrounded();

        CharacterActor.Position -=
            CustomUtilities.Multiply(
                CharacterActor.Up,
                CharacterConstants.ColliderMinBottomOffset + verticalMovementParameters.jumpDownDistance + groundDisplacementExtraDistance
            );

        CharacterActor.VerticalVelocity -= CustomUtilities.Multiply(CharacterActor.Up, verticalMovementParameters.jumpDownVerticalVelocity);
    }


    void ResetJump()
    {
        notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps;
        groundedJumpAvailable = true;
    }



    protected virtual void ProcessRegularJump(float dt)
    {

        if (CharacterActor.IsGrounded)
        {
            if (verticalMovementParameters.canJumpOnUnstableGround || CharacterActor.IsStable)
            {
                ResetJump();
            }
        }



        bool jumpBuffered = inputBufferController.HasBufferedInput(BufferedInputType.Jump);
        if (!jumpBuffered)
            return;



        if (isAllowedToCancelJump)
        {
            if (verticalMovementParameters.cancelJumpOnRelease)
            {
                if (CharacterActions.jump.StartedElapsedTime >= verticalMovementParameters.cancelJumpMaxTime || CharacterActor.IsFalling)
                {
                    isAllowedToCancelJump = false;
                }
                else if (!CharacterActions.jump.value && CharacterActions.jump.StartedElapsedTime >= verticalMovementParameters.cancelJumpMinTime)
                {
                    // Get the velocity mapped onto the current jump direction
                    Vector3 projectedJumpVelocity = Vector3.Project(CharacterActor.Velocity, jumpDirection);

                    CharacterActor.Velocity -= CustomUtilities.Multiply(projectedJumpVelocity, 1f - verticalMovementParameters.cancelJumpMultiplier);

                    isAllowedToCancelJump = false;
                }
            }
        }
        else
        {

            JumpResult jumpResult = CanJump(jumpBuffered);
            if (jumpResult == JumpResult.Invalid)
                return;


            switch (jumpResult)
            {
                case JumpResult.Grounded:
                    groundedJumpAvailable = false;
                    break;

                case JumpResult.NotGrounded:
                    notGroundedJumpsLeft--;
                    break;
            }


            // Events ---------------------------------------------------
            if (CharacterActor.IsGrounded || (!CharacterActor.IsGrounded && isOnCoyoteTime))
                OnGroundedJumpPerformed?.Invoke();
            else
                OnNotGroundedJumpPerformed?.Invoke(notGroundedJumpsLeft);

            OnJumpPerformed?.Invoke();

            // Define the jump direction ---------------------------------------------------
            jumpDirection = SetJumpDirection();

            // Force "not grounded" state.     
            if (CharacterActor.IsGrounded)
                CharacterActor.ForceNotGrounded();

            // First remove any velocity associated with the jump direction.
            CharacterActor.Velocity -= Vector3.Project(CharacterActor.Velocity, jumpDirection);
            CharacterActor.Velocity += CustomUtilities.Multiply(jumpDirection, verticalMovementParameters.jumpSpeed);

            if (verticalMovementParameters.cancelJumpOnRelease)
                isAllowedToCancelJump = true;

        }


        if (!inputBufferController.TryConsumeInput(BufferedInputType.Jump))
            return;


    }

    protected virtual Vector3 SetJumpDirection()
    {
        return CharacterActor.Up;
    }

    #endregion


    #region Rotation

    private bool HasRotationConstraints()
    {
        if (CharacterActor.BusyComboLocked || CharacterActor.BusyInputLocked || CharacterActor.BusyAnimStateLockedRootMotion)
        {
            return true;
        }

        if (CharacterActor.IsBlocking && (CharacterActor.Forward.x <= -0.99 || CharacterActor.Forward.x >= 0.99))
        {
            return true;
        }

        return false;
    }

    protected virtual void HandleRotation(float dt)
    {
        if (HasRotationConstraints())
            return;

        HandleLookingDirection(dt);
    }

    void HandleLookingDirection(float dt)
    {
        if (!lookingDirectionParameters.changeLookingDirection)
            return;



        switch (lookingDirectionParameters.lookingDirectionMode)
        {
            case LookingDirectionParameters.LookingDirectionMode.Movement:

                switch (CharacterActor.CurrentState)
                {
                    case CharacterActorState.NotGrounded:

                        SetTargetLookingDirection(lookingDirectionParameters.notGroundedLookingDirectionMode);

                        break;
                    case CharacterActorState.StableGrounded:

                        SetTargetLookingDirection(lookingDirectionParameters.stableGroundedLookingDirectionMode);

                        break;
                    case CharacterActorState.UnstableGrounded:

                        SetTargetLookingDirection(lookingDirectionParameters.unstableGroundedLookingDirectionMode);

                        break;
                }

                break;

            case LookingDirectionParameters.LookingDirectionMode.ExternalReference:

                break;

            case LookingDirectionParameters.LookingDirectionMode.Target:

                targetLookingDirection = (lookingDirectionParameters.target.position - CharacterActor.Position);
                targetLookingDirection.Normalize();

                break;
        }


        if (CharacterActions.movement.value.x == 0f && !CharacterActor.IsOnUnstableGround)
        {
            if (CharacterActor.IsFacingRight())
            {
                targetLookingDirection = Vector3Utility.AlmostRight;
            }
            else
            {
                targetLookingDirection = Vector3Utility.AlmostLeft;
            }
        }

        Quaternion targetDeltaRotation = Quaternion.FromToRotation(CharacterActor.Forward, targetLookingDirection);
        Quaternion currentDeltaRotation = Quaternion.identity;
        if (CharacterActor.IsGrounded)
            currentDeltaRotation = Quaternion.Slerp(Quaternion.identity, targetDeltaRotation, lookingDirectionParameters.speed * dt);
        else
            currentDeltaRotation = Quaternion.Slerp(Quaternion.identity, targetDeltaRotation, lookingDirectionParameters.airRotateSpeed * dt);


        CharacterActor.SetYaw(currentDeltaRotation * CharacterActor.Forward);

    }

    void SetTargetLookingDirection(LookingDirectionParameters.LookingDirectionMovementSource lookingDirectionMode)
    {
        if (!CharacterActor.IsOnUnstableGround)
        {
            if (Mathf.Abs(CharacterActions.movement.value.x) < 0.15f)
                return;
        }


        var movementDirection = CharacterStateController.MovementReferenceRight * CharacterActions.movement.value.x;
        if (lookingDirectionMode == LookingDirectionParameters.LookingDirectionMovementSource.Input)
        {
            if (movementDirection != Vector3.zero)
                targetLookingDirection = movementDirection;
            else
                targetLookingDirection = CharacterActor.Forward;
        }
        else
        {
            if (CharacterActor.PlanarVelocity != Vector3.zero)
            {
                targetLookingDirection = Vector3.ProjectOnPlane(CharacterActor.PlanarVelocity, CharacterActor.Up);
            }
            else
            {
                targetLookingDirection = CharacterActor.Forward;
            }
        }
    }

    #endregion


    #region Crouching

    protected virtual void HandleSize(float dt)
    {
        // Get the crouch input state 
        if (crouchParameters.enableCrouch)
        {
            if (crouchParameters.inputMode == InputMode.Toggle)
            {
                if (CharacterActions.crouch.Started)
                    wantToCrouch = !wantToCrouch;
            }
            else
            {
                wantToCrouch = CharacterActions.crouch.value;
            }

            if (!crouchParameters.notGroundedCrouch && !CharacterActor.IsGrounded)
                wantToCrouch = false;

            if (CharacterActor.IsGrounded && wantToRun)
                wantToCrouch = false;
        }
        else
        {
            wantToCrouch = false;
        }

        if (wantToCrouch)
            Crouch(dt);
        else
            StandUp(dt);
    }

    void Crouch(float dt)
    {
        CharacterActor.SizeReferenceType sizeReferenceType = CharacterActor.IsGrounded ?
            CharacterActor.SizeReferenceType.Bottom : crouchParameters.notGroundedReference;

        bool validSize = CharacterActor.CheckAndInterpolateHeight(
            CharacterActor.DefaultBodySize.y * crouchParameters.heightRatio,
            crouchParameters.sizeLerpSpeed * dt,
            sizeReferenceType);

        if (validSize)
            isCrouched = true;
    }

    void StandUp(float dt)
    {
        CharacterActor.SizeReferenceType sizeReferenceType = CharacterActor.IsGrounded ?
            CharacterActor.SizeReferenceType.Bottom : crouchParameters.notGroundedReference;

        bool validSize = CharacterActor.CheckAndInterpolateHeight(
            CharacterActor.DefaultBodySize.y,
            crouchParameters.sizeLerpSpeed * dt,
            sizeReferenceType);

        if (validSize)
            isCrouched = false;
    }

    #endregion

    protected virtual void HandleVelocity(float dt)
    {
        ProcessVerticalMovement(dt);
        ProcessPlanarMovement(dt);
    }
}