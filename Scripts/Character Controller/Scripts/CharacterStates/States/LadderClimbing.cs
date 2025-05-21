using System.Collections.Generic;
using UnityEngine;

using ShadowFort.Utilities;


public class LadderClimbing : CharacterState
{

    [Header("Offset")]

    [SerializeField]
    protected bool useIKOffsetValues = false;

    [Condition("useIKOffsetValues", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField]
    protected Vector3 rightFootOffset = Vector3.zero;

    [Condition("useIKOffsetValues", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField]
    protected Vector3 leftFootOffset = Vector3.zero;

    [Condition("useIKOffsetValues", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField]
    protected Vector3 rightHandOffset = Vector3.zero;

    [Condition("useIKOffsetValues", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField]
    protected Vector3 leftHandOffset = Vector3.zero;


    [Header("Bottom and Top Exit Distances")]
    [SerializeField] private float topExitStartDistance;
    [SerializeField] private float bottomExitStartDistance;


    [Header("Ladder Entry Timers")]
    [Range(0.1f, 3f), SerializeField] private float ladderBottomEntryDuration;
    private float ladderBottomEntryElapsedTime;

    [Range(0.1f, 3f), SerializeField] private float ladderTopEntryDuration;
    private float ladderTopEntryElapsedTime;

    [Range(0.1f, 3f), SerializeField] private float ladderAirborneCatchDuration;
    private float ladderAirborneCatchElapsedTime;

    [Space(10)]

    [Header("Ladder Exit Timers")]
    [Range(0.1f, 3f), SerializeField] private float ladderBottomExitDuration;
    private float ladderBottomExitElapsedTime;

    [Range(0.1f, 3f), SerializeField] private float ladderTopExitDuration;
    private float ladderTopExitElapsedTime;

    [Space(20)]

    [Header("Character Mover Timers")]
    [Range(0.1f, 3f), SerializeField] private float topEnterMoveTimer;
    [Range(0.1f, 3f), SerializeField] private float topEnterRotateTimer;
    [Space(10)]
    [Range(0.1f, 3f), SerializeField] private float bottomEnterMoveTimer;
    [Range(0.1f, 3f), SerializeField] private float bottomEnterRotateTimer;
    [Space(10)]
    [Range(0.1f, 3f), SerializeField] private float airborneCatchMoveTimer;
    [Range(0.1f, 3f), SerializeField] private float airborneCatchRotateTimer;

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


    public enum LadderSubState
    {
        None,
        TopEntry,
        BottomEntry,
        AirborneCatch,
        TopExit,
        BottomExit,
        Idle,
        LadderUp,
        LadderDown,
    }

    public LadderSubState ladderSubState;

    public void ChangeSubState(LadderSubState newState)
    {
        if (ladderSubState == newState)
            return;

        ladderSubState = newState;
    }



    protected LadderObject currentLadder = null;
    protected Vector3 targetPosition = Vector3.zero;
    protected int currentClimbingAnimation = 0;
    protected bool forceExit = false;
    protected AnimatorStateInfo animatorStateInfo;
    public bool IsLaderBottom = false;

    public InterpolationType interpolationType = InterpolationType.EaseInOut;

    private Vector3 initialPosition;


    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Start()
    {
        base.Start();

        if (CharacterActor.Animator == null)
        {
            Debug.Log("The LadderClimbing state needs the character to have a reference to an Animator component. Destroying this state...");
            Destroy(this);
        }
    }



    public override bool CheckEnterTransition(CharacterState fromState)
    {
        if (CharacterActions.interact.StartedElapsedTime <= 0.02f)
        {
            for (int i = 0; i < CharacterActor.Triggers.Count; i++)
            {
                Trigger trigger = CharacterActor.Triggers[i];

                LadderObject ladder = WorldObjectPooler.Instance.ActiveLadders.GetOrRegisterValue(trigger.transform);

                if (ladder == null)
                    continue;

                currentLadder = ladder;

                float distanceToTop = Vector3.Distance(CharacterActor.Position, currentLadder.TopReference.position);
                float distanceToBottom = Vector3.Distance(CharacterActor.Position, currentLadder.BottomReference.position);

                IsLaderBottom = distanceToBottom < distanceToTop;

                return true;
            }
        }

        return false;
    }


    public override void EnterBehaviour(float dt, CharacterState fromState)
    {
        base.EnterBehaviour(dt, fromState);


        initialPosition = CharacterActor.Position;




        if (CharacterActor.IsGrounded)
        {
            targetPosition = IsLaderBottom ? currentLadder.BottomReference.position : currentLadder.TopReference.position;
            float moveTimer = IsLaderBottom ? bottomEnterMoveTimer : topEnterMoveTimer;
            float rotateTimer = IsLaderBottom ? bottomEnterRotateTimer : topEnterRotateTimer;
            ladderSubState = IsLaderBottom ? LadderSubState.BottomEntry : LadderSubState.TopEntry;

            characterMoverAndRotator.StartMoveUpdatePosition(initialPosition, targetPosition, moveTimer, interpolationType);
            characterMoverAndRotator.StartRotate(CharacterActor.Forward, currentLadder.FacingDirectionVector, rotateTimer, interpolationType);

        }
        else
        {
            ladderSubState = LadderSubState.AirborneCatch;

            targetPosition.x = currentLadder.BottomReference.position.x;
            if (CharacterActor.IsFalling)
            {
                print("falling");
                targetPosition.y = CharacterActor.Position.y - 0.5f;

            }
            else
            {
                print("Not falling");
                targetPosition.y = CharacterActor.Position.y + 0.5f;
            }

            targetPosition.z = CharacterActor.Position.z;

            characterMoverAndRotator.StartMoveUpdatePosition(initialPosition, targetPosition, airborneCatchMoveTimer, interpolationType);
            characterMoverAndRotator.StartRotate(CharacterActor.Forward, currentLadder.FacingDirectionVector, airborneCatchRotateTimer, interpolationType);
        }



        CharacterActor.Velocity = Vector3.zero;
        CharacterActor.IsKinematic = true;
        CharacterActor.alwaysNotGrounded = true;

    }

    protected override void Update()
    {
        base.Update();
    }

    public override void UpdateBehaviour(float dt)
    {
        var currentAnimState = CharacterActor._Animancer.States.Current;
        float normalizedTime = currentAnimState.NormalizedTime;
        float animSpeed = currentAnimState.Speed;

        switch (ladderSubState)
        {
            case LadderSubState.TopEntry:
                ladderTopEntryElapsedTime += dt;
                if (ladderTopEntryElapsedTime >= ladderTopEntryDuration)
                {
                    ladderTopEntryElapsedTime = 0f;
                    ChangeSubState(LadderSubState.Idle);
                }
                break;

            case LadderSubState.BottomEntry:
                ladderBottomEntryElapsedTime += dt;
                if (ladderBottomEntryElapsedTime >= ladderBottomEntryDuration)
                {
                    ladderBottomEntryElapsedTime = 0f;
                    ChangeSubState(LadderSubState.Idle);
                }
                break;

            case LadderSubState.AirborneCatch:
                ladderAirborneCatchElapsedTime += dt;
                if (ladderAirborneCatchElapsedTime >= ladderAirborneCatchDuration)
                {
                    ladderAirborneCatchElapsedTime = 0f;
                    ChangeSubState(LadderSubState.Idle);
                }
                break;

            case LadderSubState.Idle:
                if (CharacterActions.jump.StartedElapsedTime <= 0.02f)
                {
                    forceExit = true;
                    break;
                }

                if (CharacterActions.movement.Up)
                {
                    if (Vector3.Distance(CharacterActor.Position, currentLadder.TopReference.position) < topExitStartDistance)
                    {
                        ChangeSubState(LadderSubState.TopExit);
                    }
                    else
                    {
                        ChangeSubState(LadderSubState.LadderUp);
                    }
                }
                else if (CharacterActions.movement.Down)
                {
                    if (Vector3.Distance(CharacterActor.Position, currentLadder.BottomReference.position) < bottomExitStartDistance)
                    {
                        ladderSubState = LadderSubState.BottomExit;
                    }
                    else
                    {
                        ladderSubState = LadderSubState.LadderDown;
                    }
                }
                break;


            case LadderSubState.LadderUp:
                if (normalizedTime >= 1f)
                {
                    if (CharacterActions.movement.Up)
                    {
                        ChangeSubState(LadderSubState.LadderUp);

                    }
                    else if (CharacterActions.movement.Down)
                    {
                        ChangeSubState(LadderSubState.LadderDown);
                    }
                    else
                    {
                        ChangeSubState(LadderSubState.Idle);
                    }
                }

                if (Vector3.Distance(CharacterActor.Position, currentLadder.TopReference.position) < topExitStartDistance)
                {
                    ChangeSubState(LadderSubState.TopExit);
                }
                break;


            case LadderSubState.LadderDown:
                if (CharacterActions.jump.StartedElapsedTime <= 0.02f)
                {
                    forceExit = true;
                    break;
                }

                if (normalizedTime <= 0f)
                {
                    if (CharacterActions.movement.Up)
                    {
                        ChangeSubState(LadderSubState.LadderUp);

                    }
                    else if (CharacterActions.movement.Down)
                    {
                        ChangeSubState(LadderSubState.LadderDown);
                    }
                    else
                    {
                        ChangeSubState(LadderSubState.Idle);
                    }
                }

                if (Vector3.Distance(CharacterActor.Position, currentLadder.BottomReference.position) < bottomExitStartDistance)
                {
                    ChangeSubState(LadderSubState.BottomExit);
                }
                break;


            case LadderSubState.TopExit:
                ladderTopExitElapsedTime += dt;
                if (ladderTopExitElapsedTime >= ladderTopExitDuration)
                {
                    ladderTopExitElapsedTime = 0f;
                    forceExit = true;
                }
                break;

            case LadderSubState.BottomExit:
                ladderBottomExitElapsedTime += dt;
                if (ladderBottomExitElapsedTime >= ladderBottomExitDuration)
                {
                    ladderBottomExitElapsedTime = 0f;
                    forceExit = true;
                }
                break;

        }
    }


    public override void CheckExitTransition()
    {
        if (forceExit)
            CharacterStateController.EnqueueTransition<NormalMovementXY>();
    }
    public override void ExitBehaviour(float dt, CharacterState toState)
    {
        base.ExitBehaviour(dt, toState);


        forceExit = false;
        CharacterActor.Up = Vector3.up;
        CharacterActor.IsKinematic = false;
        CharacterActor.Velocity = Vector3.zero;
        CharacterActor.alwaysNotGrounded = false;
        currentLadder = null;
        ladderSubState = LadderSubState.None;
        CharacterStateController.ResetIKWeights();
        CharacterActor.ForceGrounded();
    }


    public override void UpdateIK(int layerIndex)
    {
        if (!useIKOffsetValues)
            return;


        UpdateIKElement(AvatarIKGoal.LeftFoot, leftFootOffset);
        UpdateIKElement(AvatarIKGoal.RightFoot, rightFootOffset);
        UpdateIKElement(AvatarIKGoal.LeftHand, leftHandOffset);
        UpdateIKElement(AvatarIKGoal.RightHand, rightHandOffset);

    }

    void UpdateIKElement(AvatarIKGoal avatarIKGoal, Vector3 offset)
    {
        // Get the original (weight = 0) ik position.
        CharacterActor.Animator.SetIKPositionWeight(avatarIKGoal, 0f);
        Vector3 originalRightFootPosition = CharacterActor.Animator.GetIKPosition(avatarIKGoal);

        print(originalRightFootPosition);

        // Affect the original ik position with the offset.
        CharacterActor.Animator.SetIKPositionWeight(avatarIKGoal, 1f);
        CharacterActor.Animator.SetIKPosition(avatarIKGoal, originalRightFootPosition + offset);
    }


}