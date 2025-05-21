using System.Collections.Generic;
using UnityEngine;

using ShadowFort.Utilities;
using System.Collections;
using System;


public class LedgeHanging : CharacterState
{

    public Action<LedgeSubState> OnLedgeSubStateChanged;


    public enum LedgeSubState
    {
        None,
        BracedEnteringForward,
        BracedEnteringForwardTough,
        BracedEnteringAbove,
        BracedIdle,
        BracedJumpingDown,
        BracedJumpingBackToRight,
        BracedJumpingBackToLeft,
        BracedClimbingUp,
        BracedJumpingUpToLedge,
        FreeEnteringForward,
        FreeEnteringForwardTough,
        FreeEnteringAbove,
        FreeIdle,
        FreeJumpingDown,
        FreeClimbingUp,
    }

    public LedgeSubState ledgeSubState;

    private void ChangeSubState(LedgeSubState newState)
    {
        if (ledgeSubState == newState)
            return;

        ledgeSubState = newState;
        OnLedgeSubStateChanged?.Invoke(newState);
    }

    [Header("Filter")]

    [SerializeField]
    protected LayerMask layerMask = 0;
    [SerializeField]
    protected bool filterByTag = false;
    [SerializeField]
    protected string tagNameFreeLedge = "Untagged";
    [SerializeField]
    protected string tagNameBracedLedge = "Untagged";
    [SerializeField]
    protected bool detectRigidbodies = false;


    [Header("Detection")]

    [SerializeField]
    protected bool groundedDetection = false;
    [Tooltip("How far the hands are from the character along the forward direction.")]
    [Min(0f)]
    [SerializeField]
    protected float forwardDetectionOffset = 0.5f;
    [Tooltip("How far the hands are from the character along the up direction.")]
    [Min(0.05f)]
    [SerializeField]
    protected float upwardsDetectionOffset = 1.8f;
    [Min(0.05f)]
    [SerializeField]
    protected float separationBetweenHands = 1f;
    [Tooltip("The distance used by the raycast methods.")]
    [Min(0.05f)]
    [SerializeField]
    protected float ledgeDetectionDistance = 0.05f;


    [Header("Timers")]

    [SerializeField]
    protected float forwardEntryTime = 0f;
    [SerializeField]
    protected float aboveEntryTime = 0f;


    [Header("Movement")]

    public float ledgeJumpVelocity = 10f;
    public float jumpBackVelocity = 10f;
    [SerializeField]
    protected bool autoClimbUp = true;
    [Tooltip("If the previous state (\"fromState\") is contained in this list the autoClimbUp flag will be triggered.")]
    [SerializeField]
    protected CharacterState[] forceAutoClimbUpStates = null;



    [Header("Animation")]

    [SerializeField]
    protected bool topUpRootMotion = false;
    [SerializeField]
    protected bool jumpDownRootMotion = false;
    [SerializeField]
    private float ledgeTransitionTime = 0.5f;
    [SerializeField]
    private LedgeObject currentLedgeObject;
    [SerializeField]
    private float bracedAboveTransitionTime;
    [SerializeField]
    private float freeAboveTransitionTime;







    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    protected const float MaxLedgeVerticalAngle = 50f;


    public enum LedgeHangingState
    {
        Idle,
        ClimbingUpBraced,
        ClimbingUpFree,
        EnteringBracedAbove,
        EnteringFreeAbove,
        EnteringBracedForward,
        EnteringFreeForward,
        BracedLookingDown,
        FreeLookingDown,
    }

    [SerializeField] protected LedgeHangingState state;


    protected bool forceExit = false;
    protected bool forceAutoClimbUp = false;





    // _LEDGE OBJECT PARAMETERS__________________________________________________________________________________________

    [SerializeField]
    private float ledgeSplineBottomOffsetX;

    [SerializeField]
    private float ledgeSplineBottomOffsetY;

    [SerializeField]
    private float ledgeSplineTopOffsetX;

    [SerializeField]
    private float ledgeSplineTopOffsetY;




    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        if (CharacterActor.Animator == null)
        {
            Debug.Log("The LedgeHanging state needs the character to have a reference to an Animator component. Destroying this state...");
            Destroy(this);
        }

    }

    public override void CheckExitTransition()
    {
        if (forceExit)
            CharacterStateController.EnqueueTransition<NormalMovementXY>();

    }

    HitInfo leftHitInfo = new HitInfo();
    HitInfo rightHitInfo = new HitInfo();


    public override bool CheckEnterTransition(CharacterState fromState)
    {
        if (!groundedDetection && CharacterActor.IsGrounded)
            return false;


        if (!IsValidLedge(CharacterActor.Position))
        {
            return false;
        }

        return true;
    }

    Vector3 initialPosition;

    public InterpolationType interpolationType = InterpolationType.Linear;


    public override void EnterBehaviour(float dt, CharacterState fromState)
    {
        base.EnterBehaviour(dt, fromState);

        isExiting = false;
        exitElapsedTime = 0f;

        forceExit = false;

        initialPosition = CharacterActor.Position;
        CharacterActor.BusyComboLocked = true;
        CharacterActor.IsOnLedge = true;
        CharacterActor.alwaysNotGrounded = false;
        CharacterActor.Velocity = Vector3.zero;
        CharacterActor.IsKinematic = true;

        CharacterActor.SetSize(CharacterActor.DefaultBodySize, CharacterActor.SizeReferenceType.Top);

        Vector3 referencePosition = 0.5f * (leftHitInfo.point + rightHitInfo.point);
        Vector3 headToReference = referencePosition - CharacterActor.Top;

        Vector3 localPosition = currentLedgeObject.movementPath.Spline[0].Position;
        Vector3 worldPosition = currentLedgeObject.movementPath.transform.TransformPoint(localPosition);

        HasJumpableLedgeAbove = currentLedgeObject.HasJumpableLedgeAbove;


        Vector3 _offset = Vector3.zero;
        switch (ledgeEntryType)
        {
            case LedgeEntryType.FreeAbove:
                characterMoverAndRotator.StartLedgeAboveEntry(freeEnterAboveDuration, currentLedgeObject.movementPath);
                characterMoverAndRotator.StartRotate(CharacterActor.Forward, -currentLedgeObject.transform.right, freeAboveTransitionTime, interpolationType);
                ChangeSubState(LedgeSubState.FreeEnteringAbove);
                break;

            case LedgeEntryType.BracedAbove:
                CharacterActor.SetYaw(currentLedgeObject.AboveBracedEntryPoint.transform.right);
                characterMoverAndRotator.StartLedgeAboveEntry(bracedEnterAboveDuration, currentLedgeObject.movementPath);
                characterMoverAndRotator.StartRotate(CharacterActor.Forward, -currentLedgeObject.transform.right, freeAboveTransitionTime, interpolationType);
                ChangeSubState(LedgeSubState.BracedEnteringAbove);
                break;

            case LedgeEntryType.BracedForward:
                _offset = new Vector3(ledgeSplineBottomOffsetX, ledgeSplineBottomOffsetY, 0f);
                characterMoverAndRotator.StartRotate(CharacterActor.Forward, -currentLedgeObject.transform.right, ledgeTransitionTime, interpolationType);
                characterMoverAndRotator.StartMoveUpdatePosition(initialPosition, worldPosition + _offset, ledgeTransitionTime, interpolationType);
                ChangeSubState(LedgeSubState.BracedEnteringForward);
                break;

            case LedgeEntryType.FreeForward:
                _offset = new Vector3(ledgeSplineBottomOffsetX, ledgeSplineBottomOffsetY, 0f);
                characterMoverAndRotator.StartRotate(CharacterActor.Forward, -currentLedgeObject.transform.right, ledgeTransitionTime, interpolationType);
                characterMoverAndRotator.StartMoveUpdatePosition(initialPosition, worldPosition + _offset, ledgeTransitionTime, interpolationType);
                ChangeSubState(LedgeSubState.FreeEnteringForward);
                break;
        }

        // Determine if the character should skip the "hanging" state and go directly to the "climbing" state.
        for (int i = 0; i < forceAutoClimbUpStates.Length; i++)
        {
            CharacterState state = forceAutoClimbUpStates[i];
            if (fromState == state)
            {
                forceAutoClimbUp = true;
                break;
            }
        }

        //EventManager.Instance.CombatHandler.TriggerPlayerWeaponsDisabled();

    }


    public override void ExitBehaviour(float dt, CharacterState toState)
    {
        base.ExitBehaviour(dt, toState);

        ClearLedgeFlags();
        CharacterActor.RigidbodyComponent.IsKinematic = false;
        forceAutoClimbUp = false;


        if (willJumpBack)
        {
            //CharacterActor.Position = initialPosition;

            willJumpBack = false;
            //characterMoverAndRotator.StartRotate(CharacterActor.Forward, jumpBackDirection, 0.1f, interpolationType);
            CharacterActor.Velocity = CharacterActor.Up * ledgeJumpVelocity + jumpBackDirection * jumpBackVelocity;
            CharacterActor.ForceNotGrounded();
        }
        else
        {
            CharacterActor.Velocity = Vector3.zero;
        }

        currentLedgeObject.SetActorWithDelay(null, 1);
        currentLedgeObject = null;

        //EventManager.Instance.CombatHandler.TriggerPlayerWeaponsEnabled();

    }


    private bool isExiting = false;
    private float exitElapsedTime = 0f;
    private float exitDuration = 0.15f;
    private void LateUpdate()
    {
        if (isExiting)
        {
            exitElapsedTime += Time.deltaTime;
            if (exitElapsedTime > exitDuration)
            {
                ChangeSubState(LedgeSubState.None);
                isExiting = false;
            }
        }

    }

    private void ClearLedgeFlags()
    {
        CharacterActor.IsOnLedge = false;
        ledgeEntryType = LedgeEntryType.None;
        isExiting = true;
        forceExit = false;
    }


    bool CheckValidClimb()
    {
        HitInfoFilter ledgeHitInfoFilter = new HitInfoFilter(layerMask, false, true);
        bool overlap = CharacterActor.CharacterCollisions.CheckOverlap(
            (leftHitInfo.point + rightHitInfo.point) / 2f,
            CharacterActor.StepOffset,
            in ledgeHitInfoFilter
        );

        return !overlap;
    }

    bool willJumpBack = false;
    Vector3 jumpBackDirection = Vector3.zero;

    [Range(0.1f, 2f), SerializeField] private float bracedEnterAboveDuration;
    [SerializeField] private float bracedEnterAboveElapsedTime;

    [Range(0.1f, 2f), SerializeField] private float bracedEnterForwardDuration;
    [SerializeField] private float bracedEnterForwardElapsedTime;

    [Range(0.1f, 2f), SerializeField] private float bracedEnterForwardToughDuration;
    [SerializeField] private float bracedEnterForwardToughElapsedTime;

    [Range(0.1f, 2f), SerializeField] private float bracedJumpDownDuration;
    [SerializeField] private float bracedJumpDownElapsedTime;

    [Range(0.01f, 2f), SerializeField] private float bracedJumpBackDuration;
    [SerializeField] private float bracedJumpBackElapsedTime;

    [Range(0.1f, 2f), SerializeField] private float bracedClimbUpDuration;
    [SerializeField] private float bracedClimbUpElapsedTime;

    [Range(0.1f, 2f), SerializeField] private float bracedJumpUpToLedgeDuration;
    [SerializeField] protected float bracedJumpUpToLedgeElapsedTime;


    [Range(0.1f, 2f), SerializeField] private float freeEnterAboveDuration;
    [SerializeField] private float freeEnterAboveElapsedTime;

    private void ChangeCurrentLedgeWithAbove()
    {
        currentLedgeObject.SetActorWithDelay(null, 1);
        currentLedgeObject = currentLedgeObject.AboveLedgeObject;
        HasJumpableLedgeAbove = currentLedgeObject.HasJumpableLedgeAbove;
    }
    public bool HasJumpableLedgeAbove = false;
    public override void UpdateBehaviour(float dt)
    {
        switch (ledgeSubState)
        {
            #region BRACED LEDGE STATES

            case LedgeSubState.BracedEnteringAbove:
                bracedEnterAboveElapsedTime += dt;
                if (bracedEnterAboveElapsedTime >= bracedEnterAboveDuration)
                {
                    bracedEnterAboveElapsedTime = 0f;
                    ChangeSubState(LedgeSubState.BracedEnteringForward);
                }
                break;

            case LedgeSubState.BracedEnteringForward:
                bracedEnterForwardElapsedTime += dt;
                if (bracedEnterForwardElapsedTime >= bracedEnterForwardDuration)
                {
                    bracedEnterForwardElapsedTime = 0f;
                    ChangeSubState(LedgeSubState.BracedIdle);
                }
                break;

            case LedgeSubState.BracedEnteringForwardTough:
                bracedEnterForwardToughElapsedTime += dt;
                if (bracedEnterForwardToughElapsedTime >= bracedEnterForwardToughDuration)
                {
                    bracedEnterForwardToughElapsedTime = 0f;
                    ChangeSubState(LedgeSubState.BracedIdle);
                }
                break;


            case LedgeSubState.BracedClimbingUp:
                bracedClimbUpElapsedTime += dt;
                if (bracedClimbUpElapsedTime >= bracedClimbUpDuration)
                {
                    bracedClimbUpElapsedTime = 0f;
                    CharacterActor.ForceGrounded();
                    forceExit = true;
                }
                break;

            case LedgeSubState.BracedJumpingUpToLedge:
                bracedJumpUpToLedgeElapsedTime += dt;
                if (bracedJumpUpToLedgeElapsedTime >= bracedJumpUpToLedgeDuration)
                {
                    ChangeCurrentLedgeWithAbove();
                    bracedJumpUpToLedgeElapsedTime = 0f;
                    ChangeSubState(LedgeSubState.BracedIdle);
                }
                break;

            case LedgeSubState.BracedJumpingDown:
                CharacterActor.ForceNotGrounded();
                bracedJumpDownElapsedTime += dt;
                if (bracedJumpDownElapsedTime >= bracedJumpDownDuration)
                {
                    bracedJumpDownElapsedTime = 0f;
                    forceExit = true;
                }
                break;

            case LedgeSubState.BracedJumpingBackToRight:
                bracedJumpBackElapsedTime += dt;
                if (bracedJumpBackElapsedTime >= bracedJumpBackDuration)
                {
                    bracedJumpBackElapsedTime = 0f;
                    forceExit = true;
                }
                break;

            case LedgeSubState.BracedJumpingBackToLeft:
                bracedJumpBackElapsedTime += dt;
                if (bracedJumpBackElapsedTime >= bracedJumpBackDuration)
                {
                    bracedJumpBackElapsedTime = 0f;
                    forceExit = true;
                }
                break;

            case LedgeSubState.BracedIdle:
                CharacterActor.ForceNotGrounded();
                CharacterActor.SetUpRootMotion(true, PhysicsActor.RootMotionVelocityType.SetVelocity, false);

                if (CharacterActor.IsFacingRight())
                {
                    CharacterActor.SetYaw(Vector3Utility.AlmostRight);
                }
                else
                {
                    CharacterActor.SetYaw(Vector3Utility.AlmostLeft);
                }

                // Climb Up
                if (CharacterActions.movement.Up && CharacterActions.jump.StartedElapsedTime <= bracedEnterForwardDuration)
                {

                    if (currentLedgeObject.HasJumpableLedgeAbove)
                    {
                        CharacterActor.SetUpRootMotion(true, PhysicsActor.RootMotionVelocityType.SetVelocity, false);
                        //characterMoverAndRotator.StartKinematicMoveAddPosition(currentLedgeObject.transform.right, 3f, 0.1f);
                        ChangeSubState(LedgeSubState.BracedJumpingUpToLedge);
                    }
                    else
                    {
                        if (!currentLedgeObject.canBeClimbedUp)
                            return;
                        //if (!CheckValidClimb())
                        //    return;

                        ChangeSubState(LedgeSubState.BracedClimbingUp);


                        if (topUpRootMotion)
                            CharacterActor.SetUpRootMotion(true, PhysicsActor.RootMotionVelocityType.SetVelocity, false);
                        else
                            characterMoverAndRotator.StartLedgeTopUp(bracedClimbUpDuration, currentLedgeObject.movementPath);
                    }

                    break;
                }

                // Jump Down
                else if (CharacterActions.movement.Down && CharacterActions.jump.StartedElapsedTime <= 0.025f)
                {
                    if (jumpDownRootMotion)
                    {
                        CharacterActor.SetUpRootMotion(true, PhysicsActor.RootMotionVelocityType.SetVelocity, false);
                    }
                    ChangeSubState(LedgeSubState.BracedJumpingDown);
                    break;
                }


                else if (CharacterActions.jump.StartedElapsedTime <= 0.025f)
                {
                    if (CharacterActor.IsFacingRight() && CharacterActions.movement.Left)
                    {
                        willJumpBack = true;
                        CharacterActor.SetYaw(Vector3Utility.AlmostRight);
                        jumpBackDirection = Vector3Utility.AlmostLeft;
                        characterMoverAndRotator.StartRotate(CharacterActor.Forward, jumpBackDirection, 0.1f, interpolationType);
                        ChangeSubState(LedgeSubState.BracedJumpingBackToLeft);

                    }
                    else if (CharacterActor.IsFacingLeft() && CharacterActions.movement.Right)
                    {
                        willJumpBack = true;
                        CharacterActor.SetYaw(Vector3Utility.AlmostLeft);
                        jumpBackDirection = Vector3Utility.AlmostRight;
                        characterMoverAndRotator.StartRotate(CharacterActor.Forward, jumpBackDirection, 0.1f, interpolationType);
                        ChangeSubState(LedgeSubState.BracedJumpingBackToRight);
                    }
                }


                break;

            #endregion



            case LedgeSubState.FreeEnteringAbove:
                break;
            case LedgeSubState.FreeEnteringForward:
                break;
            case LedgeSubState.FreeEnteringForwardTough:
                break;

            case LedgeSubState.FreeClimbingUp:
                break;
            case LedgeSubState.FreeJumpingDown:
                break;

            case LedgeSubState.FreeIdle:
                break;


        }


    }

    private enum LedgeEntryType
    {
        None,
        BracedForward,
        BracedAbove,
        FreeForward,
        FreeAbove
    }

    private LedgeEntryType ledgeEntryType;

    private bool enterBracedFromAbove;
    private bool enterBracedForward;
    private bool enterFreeFromAbove;
    private bool enterFreeForward;

    bool IsValidLedge(Vector3 characterPosition)
    {
        //if (!CharacterActor.WallCollision)
        //    return false;


        // Entering from above
        if (CharacterActor.GroundObject != null)
        {
            if (CharacterActor.GroundObject.CompareTag(tagNameFreeLedge))
            {
                ledgeEntryType = LedgeEntryType.FreeAbove;
            }
            else if (CharacterActor.GroundObject.CompareTag(tagNameBracedLedge))
            {
                ledgeEntryType = LedgeEntryType.BracedAbove;
            }

            currentLedgeObject = CharacterActor.GroundObject?.GetComponent<LedgeObject>();
            currentLedgeObject.SetActor(CharacterActor);
            return true;
        }


        // Enter Normally
        DetectLedge(
            characterPosition,
            out leftHitInfo,
            out rightHitInfo
        );

        if (!leftHitInfo.hit || !rightHitInfo.hit)
        {
            return false;
        }

        if (filterByTag)
        {
            if ((!leftHitInfo.transform.CompareTag(tagNameBracedLedge) || !rightHitInfo.transform.CompareTag(tagNameBracedLedge)) &&
                (!leftHitInfo.transform.CompareTag(tagNameFreeLedge) || !rightHitInfo.transform.CompareTag(tagNameFreeLedge)))
            {
                return false;
            }
        }


        if (leftHitInfo.transform.CompareTag(tagNameBracedLedge) || rightHitInfo.transform.CompareTag(tagNameBracedLedge))
        {
            ledgeEntryType = LedgeEntryType.BracedForward;
        }
        else if (leftHitInfo.transform.CompareTag(tagNameFreeLedge) || rightHitInfo.transform.CompareTag(tagNameFreeLedge))
        {
            ledgeEntryType = LedgeEntryType.FreeForward;
        }

        Vector3 interpolatedNormal = Vector3.Normalize(leftHitInfo.normal + rightHitInfo.normal);
        float ledgeAngle = Vector3.Angle(CharacterActor.Up, interpolatedNormal);
        if (ledgeAngle > MaxLedgeVerticalAngle)
            return false;

        if (leftHitInfo.transform.gameObject != null)
        {
            currentLedgeObject = leftHitInfo.transform?.gameObject?.GetComponent<LedgeObject>();
            currentLedgeObject.SetActor(CharacterActor);
        }


        return true;
    }


    void DetectLedge(Vector3 position, out HitInfo leftHitInfo, out HitInfo rightHitInfo)
    {
        HitInfoFilter ledgeHitInfoFilter = new HitInfoFilter(layerMask, !detectRigidbodies, true);
        leftHitInfo = new HitInfo();
        rightHitInfo = new HitInfo();

        Vector3 forwardDirection = CharacterActor.WallCollision ? -CharacterActor.WallContact.normal : CharacterActor.Forward;
        Vector3 sideDirection = Vector3.Cross(CharacterActor.Up, forwardDirection);

        // Check if there is an object above
        Vector3 upDetection = position + CharacterActor.Up * (upwardsDetectionOffset);

        CharacterActor.PhysicsComponent.Raycast(
            out HitInfo auxHitInfo,
            CharacterActor.Center,
            upDetection - CharacterActor.Center,
            in ledgeHitInfoFilter
        );


        if (auxHitInfo.hit)
        {
            return;
        }

        Vector3 middleOrigin = upDetection + forwardDirection * (forwardDetectionOffset);

        Vector3 leftOrigin = middleOrigin - sideDirection * (separationBetweenHands / 2f);
        Vector3 rightOrigin = middleOrigin + sideDirection * (separationBetweenHands / 2f);

        CharacterActor.PhysicsComponent.Raycast(
            out leftHitInfo,
            leftOrigin,
            -CharacterActor.Up * ledgeDetectionDistance,
            in ledgeHitInfoFilter
        );


        CharacterActor.PhysicsComponent.Raycast(
            out rightHitInfo,
            rightOrigin,
            -CharacterActor.Up * ledgeDetectionDistance,
            in ledgeHitInfoFilter
        );



    }


    public override void PreCharacterSimulation(float dt)
    {

    }

#if UNITY_EDITOR

    CharacterBody characterBody = null;

    void OnValidate()
    {
        characterBody = this.GetComponentInBranch<CharacterBody>();
    }

    void OnDrawGizmos()
    {
        Vector3 forwardDirection = transform.forward;

        Vector3 sideDirection = Vector3.Cross(transform.up, forwardDirection);
        Vector3 middleOrigin = transform.position + transform.up * (upwardsDetectionOffset) + forwardDirection * (forwardDetectionOffset);
        Vector3 leftOrigin = middleOrigin - sideDirection * (separationBetweenHands / 2f);
        Vector3 rightOrigin = middleOrigin + sideDirection * (separationBetweenHands / 2f);

        CustomUtilities.DrawArrowGizmo(leftOrigin, leftOrigin - transform.up * ledgeDetectionDistance, Color.red, 0.15f);
        CustomUtilities.DrawArrowGizmo(rightOrigin, rightOrigin - transform.up * ledgeDetectionDistance, Color.red, 0.15f);
    }

#endif

}

