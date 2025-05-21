using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ShadowFort.Utilities;

public class AIFollowBehaviour2D : CharacterAIBehaviour
{
    [SerializeField]
    Transform followTarget = null;

    [Tooltip("Desired distance to the target. if the distance to the target is less than this value the character will not move.")]
    [SerializeField]
    float reachDistance = 3f;

    [Tooltip("The wait time between actions updates.")]
    [Min(0f)]
    [SerializeField]
    float refreshTime = 0.65f;

    float timer = 0f;

    protected CharacterStateController stateController = null;

    protected override void Awake()
    {
        base.Awake();

        stateController = this.GetComponentInBranch<CharacterActor, CharacterStateController>();
        stateController.MovementReferenceMode = MovementReferenceParameters.MovementReferenceMode.World;
    }

    void OnEnable()
    {

    }

    public override void EnterBehaviour(float dt)
    {
        timer = refreshTime;
    }

    public override void UpdateBehaviour(float dt)
    {
        timer += dt;
        if (timer < refreshTime)
            return;

        timer = 0f;
        UpdateFollow2D();
    }

    void UpdateFollow2D()
    {
        if (followTarget == null)
            return;

        Vector3 diff = followTarget.position - transform.position;

        diff.z = 0f;

        if (diff.magnitude <= reachDistance)
        {
            characterActions.Reset();
            return;
        }

        Vector3 moveDir = diff.normalized;

        SetMovementAction(moveDir);
    }


    public void SetFollowTarget(Transform newTarget, bool forceUpdate = true)
    {
        followTarget = newTarget;
        if (forceUpdate)
            timer = refreshTime;
    }
}
