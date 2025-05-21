using UnityEngine;
using ShadowFort.Utilities;
using Animancer;

[CreateAssetMenu(fileName = "AttackElement", menuName = "NocturneKeepInteractive/Combat System/AttackElement")]
public class AttackElement : ScriptableObject
{
    [Header("Attack Properties")]
    public string attackName;
    public AttackID attackID;
    public AttackType attackType;
    public ClipTransition attackAnimation;
    public Easing.Function animEasingFunction;

    public bool IsFirstAttack;

    [Space(30)]

    [Header("Damage Source")]
    public DamageSource damageSource;

    [Header("Weapon Trail Activation")]
    public bool activateWeaponTrail;

    [Condition("activateWeaponTrail", ConditionAttribute.ConditionType.IsTrue)]
    [Range(0f, 2f)]
    public float weaponTrailSpawnMoment;
    [Range(0f, 2f)]
    public float weaponTrailLifetime;


    [Space(30)]

    [Header("Busy Lock Durations")]

    [Tooltip("Wait before next combo step")]
    [Range(0f, 3f)]
    public float comboLockDuration;

    [Tooltip("Wait before moving/dashing/jumping")]
    [Range(0f, 3f)]
    public float inputLockDuration;

    [Tooltip("Wait before playing required animations (e.g. air attack -> ground contact -> fall animation)")]
    [Range(0f, 3f)]
    public float animStateLockDuration;

    [Tooltip("Wait before resetting combo and returning to idle state (if not already idle)")]
    [Range(0f, 3f)]
    public float returnToIdleLockDuration;


    [Space(30)]


    [Header("Caster Movement")]
    public AttackMovementDirection attackMovementDirection;
    [Range(0f, 1f)] public float attackMovementDuration;
    [Range(0f, 30f)] public float attackMoveForce;

    [Header("Caster Movement Curve")]
    public bool useCurveForMotion;
    public Vector3 curveMovementDirection;
    [Range(0, 1)] public float curveMovementDuration;
    public AnimationCurve velocityCurve;

    [Header("Attack Caster Properties")]
    public bool EnemyCannotInterrupt;
    public bool IsImmuneToDamage;
    public bool IsCancellable;






}