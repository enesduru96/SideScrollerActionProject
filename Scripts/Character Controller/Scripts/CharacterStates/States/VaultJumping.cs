using UnityEngine;
using ShadowFort.Utilities;
using System;

public class VaultJumping : CharacterState
{

    public enum VaultSubState
    {
        None,
        VaultingSlowToLeft,
        VaultingFastToLeft,
        VaultingSlowToRight,
        VaultingFastToRight,
    }

    public VaultSubState vaultSubState;

    private void ChangeSubState(VaultSubState newState)
    {
        if (vaultSubState == newState)
            return;

        vaultSubState = newState;
    }

    [Header("Button Buffer")]
    [SerializeField] private float vaultButtonBufferTime = 0.2f;

    [Header("Kinematic Push Movement")]
    [SerializeField] private bool useKinematicPush;
    [Condition("useKinematicPush", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private float kinematicPushAmountSlow = 2f;
    [Condition("useKinematicPush", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private float kinematicPushAmountFast = 5f;

    [Header("Interpolation")]
    [SerializeField] private InterpolationType movementInterpolation = InterpolationType.EaseInOut;
    [Condition("useKinematicPush", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private float fastVaultPositionTimeKinematic = 0.2f;
    [Condition("useKinematicPush", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private float slowVaultPositionTimeKinematic = 0.5f;

    [SerializeField] private float slowVaultPositionTime = 0.5f;
    [SerializeField] private float slowVaultRotationTime = 0.2f;

    [SerializeField] private float fastVaultPositionTime = 0.2f;
    [SerializeField] private float fastVaultRotationTime = 0.2f;


    [Header("Sprint Vault")]
    [Range(0, 4), SerializeField] private float sprintVaultCooldown = 2f;
    private float sprintVaultElapsedTime = 0f;
    private bool hasSprintVaulted = false;
    private bool hasEnteredFast = false;


    [Header("References")]
    [SerializeField] private VaultObject currentVault = null;
    [SerializeField] private Transform targetPoint = null;
    private CharacterIKHandler characterIKHandler;
    private CharacterProfileStorage characterProfileStorage;
    private CharacterProfile characterProfile;

    private Transform leftHandTargetTransform;
    private Transform rightHandTargetTransform;
    private bool forceExit = false;
    private bool targetIsLeft = false;

    private float vaultExitTime;
    private float exitElapsedTime = 0f;

    private void ResetAllValues()
    {
        currentVault = null;
        targetPoint = null;
        forceExit = false;
        hasEnteredFast = false;

        exitElapsedTime = 0f;
        vaultExitTime = 0f;
    }


    protected override void Awake()
    {
        base.Awake();

        characterIKHandler = this.GetComponentInBranch<CharacterActor, CharacterIKHandler>();
        characterProfileStorage = this.GetComponentInBranch<CharacterActor, CharacterProfileStorage>();
        characterProfile = characterProfileStorage.characterProfile;
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


    private bool HasVaultRestrictions()
    {
        if (!CharacterActor.IsGrounded)
        {
            return true;
        }

        if (CharacterActor.GroundObject == null)
        {
            return true;
        }

        if (!CharacterActor.GroundObject.CompareTag("Floor"))
        {
            return true;
        }

        if (CharacterActor.Triggers.Count == 0)
            return true;

        return false;
    }
    public override bool CheckEnterTransition(CharacterState fromState)
    {

        if (HasVaultRestrictions())
        {
            return false;
        }

        for (int i = 0; i < CharacterActor.Triggers.Count; i++)
        {
            Trigger trigger = CharacterActor.Triggers[i];
            VaultObject vault = WorldObjectPooler.Instance.ActiveVaults.GetOrRegisterValue(trigger.transform);

            if (vault == null)
                continue;

            currentVault = vault;


            bool vaultIsLeftSide = CharacterActor.ObjectIsLeftSide(currentVault.transform);
            bool vaultIsRightSide = CharacterActor.ObjectIsRightSide(currentVault.transform);
            float inputDirection = CharacterActions.movement.value.x;

            if ((vaultIsLeftSide && inputDirection > 0) || (vaultIsRightSide) && inputDirection < 0)
            {
                return false;
            }


            if (trigger.firstContact && CharacterActor.PlanarVelocity.magnitude >= characterProfile.SprintSpeed * 0.99)
            {
                hasEnteredFast = true;
                if (hasSprintVaulted)
                    return false;
                return true;
            }
            else if (CharacterActor.PlanarVelocity.x < characterProfile.SprintSpeed && CharacterActions.interact.StartedElapsedTime <= vaultButtonBufferTime)
            {
                hasEnteredFast = false;
                return true;
            }
        }

        return false;
    }




    private void SetupHandIK()
    {
        characterIKHandler.SmoothSetIKWeightLeftHand(1f, 0.1f);
        leftHandTargetTransform = currentVault.LeftHandTarget.transform;
        rightHandTargetTransform = currentVault.RightHandTarget.transform;

        if (targetIsLeft)
            characterIKHandler.SetLeftHandTarget(leftHandTargetTransform);
        else
            characterIKHandler.SetLeftHandTarget(rightHandTargetTransform);
    }
    public override void EnterBehaviour(float dt, CharacterState fromState)
    {
        base.EnterBehaviour(dt, fromState);

        CharacterActor.IsKinematic = true;

        float distanceToLeft = Vector3.Distance(CharacterActor.Position, currentVault.LeftEntryPoint.position);
        float distanceToRight = Vector3.Distance(CharacterActor.Position, currentVault.RightEntryPoint.position);
        targetIsLeft = distanceToLeft < distanceToRight;
        targetPoint = targetIsLeft ? currentVault.RightEntryPoint : currentVault.LeftEntryPoint;

        if (useKinematicPush)
        {
            Vector3 pushDirection = targetPoint.forward;
            float pushSpeed = hasEnteredFast ? kinematicPushAmountFast : kinematicPushAmountSlow;
            float pushDuration = hasEnteredFast ? fastVaultPositionTimeKinematic : slowVaultPositionTimeKinematic;

            characterMoverAndRotator.StartKinematicMoveAddPosition(pushDirection, pushSpeed, pushDuration);
            SetExitTime(pushDuration);
        }
        else
        {
            Vector3 startPosition = CharacterActor.Position;
            Vector3 targetPosition = targetPoint.position;
            float positionDuration = hasEnteredFast ? fastVaultPositionTime : slowVaultPositionTime;

            characterMoverAndRotator.StartMoveUpdatePosition(startPosition, targetPosition, positionDuration, movementInterpolation);
        }

        float rotationDuration = hasEnteredFast ? fastVaultRotationTime : slowVaultRotationTime;
        characterMoverAndRotator.StartRotate(CharacterActor.Forward, targetPoint.forward, rotationDuration, movementInterpolation);


        SetupHandIK();


        CharacterActor.SetUpRootMotion(false, PhysicsActor.RootMotionVelocityType.SetVelocity, true);


        if (hasEnteredFast)
        {
            ChangeSubState(VaultSubState.VaultingFastToRight);
            hasSprintVaulted = true;
        }
        else
        {
            ChangeSubState(VaultSubState.VaultingSlowToRight);
        }

    }
    private void SetExitTime(float movementDuration)
    {
        vaultExitTime = movementDuration;
    }


    public override void UpdateBehaviour(float dt)
    {

    }

    protected void LateUpdate()
    {
        base.Update();


        if (hasSprintVaulted)
        {
            sprintVaultElapsedTime += Time.deltaTime;
            if (sprintVaultElapsedTime >= sprintVaultCooldown)
            {
                sprintVaultElapsedTime = 0f;
                hasSprintVaulted = false;
            }
        }

        if (vaultExitTime != 0f)
        {
            exitElapsedTime += Time.deltaTime;
            if (exitElapsedTime >= vaultExitTime)
            {
                forceExit = true;
            }
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

        CharacterActor.IsKinematic = false;


        if (hasEnteredFast)
        {
            CharacterActor.PlanarVelocity = CharacterActor.Forward * 7f;
        }

        ResetAllValues();

        characterIKHandler.SmoothSetIKWeightLeftHand(0f, 0.2f);
    }

}