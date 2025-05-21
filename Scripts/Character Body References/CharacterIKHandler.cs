using UnityEngine;
using UnityEngine.Animations.Rigging;
using ShadowFort.Utilities;
using Animancer;

public class CharacterIKHandler : MonoBehaviour
{
    [SerializeField] private bool useIK = true;

    [Header("Rig and Contstraint")]
    [SerializeField] private Rig mainIKRig;
    [SerializeField] private TwoBoneIKConstraint leftArmConstraint;
    [SerializeField] private TwoBoneIKConstraint rightArmConstraint;

    [Header("Left Arm")]
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform leftElbowHint;
    [SerializeField] private Transform leftInitialPoint;
    [SerializeField] private Transform leftHandFollowGoal;

    [Header("Right Arm")]
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform rightElbowHint;
    [SerializeField] private Transform rightInitialPoint;
    [SerializeField] private Transform rightHandFollowGoal;

    [Header("References")]
    [SerializeField] private AnimancerComponent _Animancer;

    private PlayableOutputRefresher _OutputRefresher;


    private void Start()
    {
        _Animancer = GetComponentInChildren<AnimancerComponent>();
        _OutputRefresher = new(_Animancer);
    }


    private float targetIKWeightLeft = 0f;
    private float weightSettingDurationLeft = 0f;
    private float weightSettingElapsedTimeLeft = 0f;
    private bool isSettingWeightLeft = false;
    private float initialMainIKWeight;
    private float initialConstraintWeightLeft;

    public void SmoothSetIKWeightLeftHand(float targetWeight, float duration)
    {
        weightSettingDurationLeft = duration;
        targetIKWeightLeft = targetWeight;
        weightSettingElapsedTimeLeft = 0f;
        initialMainIKWeight = mainIKRig.weight;
        initialConstraintWeightLeft = leftArmConstraint.weight;
        isSettingWeightLeft = true;
    }


    private float weightSettingDurationRight = 0f;
    private float targetIKWeightRight = 0f;
    private float weightSettingElapsedTimeRight = 0f;
    private float initialConstraintWeightRight;
    private bool isSettingWeightRight = false;

    public void SmoothSetIKWeightRightHand(float targetWeight, float duration)
    {
        weightSettingDurationRight = duration;
        targetIKWeightRight = targetWeight;
        weightSettingElapsedTimeRight = 0f;
        initialMainIKWeight = mainIKRig.weight;
        initialConstraintWeightRight = rightArmConstraint.weight;
        isSettingWeightRight = true;
    }

    private void ReturnLeftArmToDefault()
    {
        leftHandFollowGoal.parent = leftArmConstraint.transform;
        leftHandFollowGoal.position = leftInitialPoint.position;
        leftHandFollowGoal.rotation = leftInitialPoint.rotation;
    }
    private void ReturnRightArmToDefault()
    {
        rightHandFollowGoal.parent = rightArmConstraint.transform;
        rightHandFollowGoal.position = rightInitialPoint.position;
        rightHandFollowGoal.rotation = rightInitialPoint.rotation;
    }

    public void SetLeftHandTarget(Transform targetTransform)
    {
        leftHandFollowGoal.parent = null;
        leftHandFollowGoal.position = targetTransform.position;
        leftHandFollowGoal.rotation = targetTransform.rotation;
    }

    public void SetRightHandTarget(Transform targetTransform)
    {
        rightHandFollowGoal.parent = null;
        rightHandFollowGoal.position = targetTransform.position;
        rightHandFollowGoal.rotation = targetTransform.rotation;
    }


    public void UpdateMovement(float deltaTime)
    {
        if (isSettingWeightLeft)
        {
            weightSettingElapsedTimeLeft += deltaTime;
            float t = Mathf.Clamp01(weightSettingElapsedTimeLeft / weightSettingDurationLeft);

            mainIKRig.weight = Mathf.Lerp(initialMainIKWeight, targetIKWeightLeft, t);
            leftArmConstraint.weight = Mathf.Lerp(initialConstraintWeightLeft, targetIKWeightLeft, t);
            _OutputRefresher.Refresh();

            if (t >= 1f)
            {
                if (mainIKRig.weight == 0f)
                {
                    ReturnLeftArmToDefault();
                }
                isSettingWeightLeft = false;
            }
        }


        if (isSettingWeightRight)
        {
            weightSettingElapsedTimeRight += deltaTime;
            float t = Mathf.Clamp01(weightSettingElapsedTimeRight / weightSettingDurationRight);
            mainIKRig.weight = Mathf.Lerp(initialMainIKWeight, targetIKWeightRight, t);
            rightArmConstraint.weight = Mathf.Lerp(initialConstraintWeightRight, targetIKWeightRight, t);
            _OutputRefresher.Refresh();

            if (t >= 1f)
            {
                if (mainIKRig.weight == 0f)
                {
                    ReturnRightArmToDefault();
                }
                isSettingWeightRight = false;
            }
        }
    }


    void Update()
    {
        if (useIK)
        {
            UpdateMovement(Time.deltaTime);

            leftHandTarget.position = leftHandFollowGoal.position;
            leftHandTarget.rotation = leftHandFollowGoal.rotation;

            rightHandTarget.position = rightHandFollowGoal.position;
            rightHandTarget.rotation = rightHandFollowGoal.rotation;
        }


        _OutputRefresher.Refresh();
    }


}
