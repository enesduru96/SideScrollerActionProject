using UnityEngine;
using ShadowFort.Utilities;
using System.Collections.Generic;
using UnityEngine.Pool;


[RequireComponent(typeof(Collider))]
public abstract class DamageSource : MonoBehaviour
{

    #region Exposed Fields

    [Header("Pooling")]
    [SerializeField] private bool isPooled;
    [SerializeField] private int pooledCount;

    [Space(20)]

    [Header("Spawn Point")]
    [SerializeField] private DamageSpawnPoint damageSpawnLocation;
    [SerializeField] private Vector3 spawnPositionOffset;
    [SerializeField] private Vector3 spawnRotationOffset;

    [Space(20)]

    [Header("Lifetime")]
    [Range(0f, 5f), SerializeField] private float spawnMoment = 0f;
    [Range(0f, 30f), SerializeField] private float lifeTime = 0f;

    [Header("Hit Stop")]
    [SerializeField] private bool hasHitStop = false;
    [Range(0f, 0.3f), SerializeField] private float hitStopDuration = 0f;

    [Space(20)]

    [Header("Base Fields")]
    [SerializeField] private CharacterProfile _owner;
    [SerializeField] private CharacterActor _ownerActor;
    [SerializeField] private Faction _faction;
    [SerializeField] private GameObject _onHitVfxObject;

    [Space(20)]

    [Header("Damage Properties")]
    [SerializeField] private DamageType _damageType;
    [SerializeField] private float _baseDamage;
    [SerializeField] private float _poiseDamage;
    [SerializeField] private float _armorPenetration;
    [SerializeField] private bool _canHitCritical;
    [SerializeField] private float _criticalMultiplier;

    [Space(20)]

    [Header("Damage Over Time")]
    [SerializeField] private bool _hasDamageOverTime;
    [SerializeField] private float damageOverTimeInterval = 1f;

    [Space(20)]

    [Header("Block/Parry")]
    [SerializeField] private bool _canBeBlocked;
    [SerializeField] private bool _canBeParried;

    [Space(20)]

    [Header("Stun")]
    [SerializeField] private bool _canStun;
    [Condition("_canStun", ConditionAttribute.ConditionType.IsTrue)]
    [Range(1, 10), SerializeField] private int _stunPower;
    [Condition("_canStun", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private float _stunDuration;

    [Space(20)]

    [Header("Pushback")]
    [SerializeField] private bool _canPushback;
    [Condition("_canPushback", ConditionAttribute.ConditionType.IsTrue)]
    [Range(1, 10), SerializeField] private int _pushbackPower;

    [Condition("_canPushback", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private float _pushbackMovementForce;

    [Condition("_canPushback", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private float _pushbackDuration;

    [Condition("_canPushback", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private bool _hasPushbackAnimOverride;

    [Condition("_hasPushbackAnimOverride", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private AnimationClip _pushbackOverrideAnimation;


    [Space(20)]


    [Header("Knockdown")]
    [SerializeField] private bool _canKnockdown;

    [Condition("_canKnockdown", ConditionAttribute.ConditionType.IsTrue)]
    [Range(1, 10), SerializeField] private int _knockdownPower;

    [Condition("_canKnockdown", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private float _knockdownDuration;

    [Condition("_canKnockdown", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private bool _hasKnockdownAnimOverride;

    [Condition("_hasKnockdownAnimOverride", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private AnimationClip _knockdownOverrideAnimation;


    [Space(20)]


    [Header("Air Launch")]
    [SerializeField] private bool _canLaunchToAir;

    [Condition("_canLaunchToAir", ConditionAttribute.ConditionType.IsTrue)]
    [Range(1, 10), SerializeField] private int _airLaunchPower;

    [Condition("_canLaunchToAir", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private float _airLaunchMovementForce;

    [Condition("_canLaunchToAir", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private float _airDuration;

    [Condition("_canLaunchToAir", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private bool _hasAirLaunchAnimOverride;

    [Condition("_hasAirLaunchAnimOverride", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField] private AnimationClip _airLaunchOverrideAnimation;


    [Space(20)]


    [Header("Status Effects")]
    [SerializeField] private StatusEffect _statusEffect;
    [Condition("_statusEffect", ConditionAttribute.ConditionType.IsFalse)]
    [SerializeField] private float _statusEffectDamage;
    [Condition("_statusEffect", ConditionAttribute.ConditionType.IsFalse)]
    [SerializeField] private float _statusEffectDuration;


    [Space(20)]


    [Header("Collision Overlap Settings")]
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float overlapInterval = 0.02f;
    [SerializeField] private float normalDamageCooldown = 3f;

    #endregion


    #region Public Getters

    public bool IsPooled => isPooled;
    public int PooledCount => pooledCount;

    public DamageSpawnPoint DamageSpawnLocation => damageSpawnLocation;
    public Vector3 SpawnPositionOffset => spawnPositionOffset;
    public Vector3 SpawnRotationOffset => spawnRotationOffset;

    public float SpawnMoment => spawnMoment;
    public float LifeTime => lifeTime;

    public bool HasHitStop => hasHitStop;
    public float HitStopDuration => hitStopDuration;

    public CharacterProfile Owner => _owner;
    public CharacterActor OwnerActor { get { return _ownerActor; } set { _ownerActor = value; } }
    public Faction Faction => _faction;
    public GameObject OnHitVFX => _onHitVfxObject;


    public DamageType DamageType => _damageType;
    public float BaseDamage => _baseDamage;
    public float PoiseDamage => _poiseDamage;
    public float ArmorPenetration => _armorPenetration;


    public bool CanHitCritical => _canHitCritical;
    public float CriticalMultiplier => _criticalMultiplier;


    public bool HasDamageOverTime => _hasDamageOverTime;


    public bool CanBeBlocked => _canBeBlocked;
    public bool CanBeParried => _canBeParried;


    public bool CanStun => _canStun;
    public float StunDuration => _stunDuration;


    public bool CanPushback => _canPushback;
    public int PushbackPower => _pushbackPower;
    public float PushbackMovementForce => _pushbackMovementForce;
    public float PushbackDuration => _pushbackDuration;
    public bool HasPushbackAnimOverride => _hasPushbackAnimOverride;
    public AnimationClip PushbackOverrideAnimation => _pushbackOverrideAnimation;


    public bool CanKnockDown => _canKnockdown;
    public int KnockdownPower => _knockdownPower;
    public float KnocdownDuration => _knockdownDuration;
    public bool HasKnockdownAnimOverride => _hasKnockdownAnimOverride;
    public AnimationClip KnockdownOverrideAnimation => _knockdownOverrideAnimation;


    public bool CanLaunchToAir => _canLaunchToAir;
    public int AirLaunchPower => _airLaunchPower;
    public float AirLaunchMovementForce => _airLaunchMovementForce;
    public bool HasAirLaunchAnimOverride => _hasAirLaunchAnimOverride;
    public AnimationClip AirLaunchOverrideAnimation => _airLaunchOverrideAnimation;
    public float AirDuration => _airDuration;



    public StatusEffect StatusEffect => _statusEffect;
    public float StatusEffectDamage => _statusEffectDamage;
    public float StatusEffectDuration => _statusEffectDuration;

    #endregion


    #region Internal Fields

    private Collider _cachedCollider;

    [SerializeField] private Collider[] _overlapResults = new Collider[12];
    private float _overlapTimer = 0f;

    private Dictionary<Collider, float> _damageTimers = new Dictionary<Collider, float>();
    private List<Collider> _cachedKeysToRemove;
    private float _dictCleanTimer = 0f;
    private const float _dictCleanInterval = 10f;

    private enum ColliderType { Sphere, Box, Unsupported }
    private ColliderType _colliderType;


    private CharacterActor characterActor;

    #endregion

    private ObjectPool<DamageSource> _pool;

    public void SetPool(ObjectPool<DamageSource> pool)
    {
        _pool = pool;
    }

    protected abstract void InitializeDamageObject();

    protected virtual void Awake()
    {

        _cachedCollider = GetComponent<Collider>();
        if (!_cachedCollider.isTrigger)
            Debug.LogError($"The Collider on {gameObject.name} must be set as a trigger!");

        if (_cachedCollider is SphereCollider)
            _colliderType = ColliderType.Sphere;
        else if (_cachedCollider is BoxCollider)
            _colliderType = ColliderType.Box;
        else
            _colliderType = ColliderType.Unsupported;

        if (_colliderType == ColliderType.Unsupported)
            Debug.LogError("Unsupported collider type for DamageSource!");

        _cachedKeysToRemove = new List<Collider>(12);
    }


    private float spawnStartElapsedTime = 0f;
    private bool spawnStarted = false;

    private float spawnEndElapsedTime = 0f;
    private bool hasSpawned = false;
    protected virtual void OnEnable()
    {
        spawnStartElapsedTime = 0f;
        spawnStarted = true;
    }

    protected virtual void OnDisable()
    {
        spawnStarted = false;
        hasSpawned = false;

        spawnStartElapsedTime = 0f;
        spawnEndElapsedTime = 0f;
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (hasSpawned)
        {
            _overlapTimer = overlapInterval;
            HandleOverlaps();
            HandleCleanUp();
        }


        if (spawnStarted)
        {
            spawnStartElapsedTime += Time.deltaTime;
            if(spawnStartElapsedTime >= spawnMoment)
            {
                spawnStarted = false;
                hasSpawned = true;
                spawnStartElapsedTime = 0f;
                spawnEndElapsedTime = 0f;
                _cachedCollider.enabled = true;
            }
        }

    }

    private void HandleOverlaps()
    {
        foundEnemy = false;

        _overlapTimer += Time.deltaTime;

        if (_overlapTimer >= overlapInterval)
        {
            _overlapTimer = 0f;

            switch (_colliderType)
            {
                case ColliderType.Sphere:
                    ProcessSphereOverlap((SphereCollider)_cachedCollider);
                    break;
                case ColliderType.Box:
                    ProcessBoxOverlap((BoxCollider)_cachedCollider);
                    break;
                case ColliderType.Unsupported:
                    break;
            }
        }
    }

    private void ProcessSphereOverlap(SphereCollider sphereCollider)
    {
        Vector3 position = transform.position + sphereCollider.center;
        float radius = sphereCollider.radius;

        int numColliders = Physics.OverlapSphereNonAlloc(
            position,
            radius,
            _overlapResults,
            targetLayers
        );

        ProcessCollisions(numColliders);
    }

    private void ProcessBoxOverlap(BoxCollider boxCollider)
    {
        Vector3 position = transform.position + boxCollider.center;
        Vector3 size = boxCollider.size / 2f;
        Quaternion rotation = transform.rotation;

        int numColliders = Physics.OverlapBoxNonAlloc(
            position,
            size,
            _overlapResults,
            rotation,
            targetLayers
        );

        ProcessCollisions(numColliders);
    }

    private bool foundEnemy = false;
    private void ProcessCollisions(int numColliders)
    {
        float currentTime = Time.time;

        for (int i = 0; i < numColliders; i++)
        {
            Collider collider = _overlapResults[i];

            if (collider.isTrigger)
                continue;

            if (!collider.TryGetComponent<IDamageable>(out IDamageable target))
                continue;

            foundEnemy = true;

            if (_hasDamageOverTime)
            {
                if (!_damageTimers.ContainsKey(collider) || currentTime - _damageTimers[collider] >= damageOverTimeInterval)
                {
                    EventManager.Instance.CombatHits.TriggerCombatHit(this, collider.gameObject);
                    _damageTimers[collider] = currentTime;
                }
            }
            else
            {
                if (!_damageTimers.ContainsKey(collider))
                {
                    EventManager.Instance.CombatHits.TriggerCombatHit(this, collider.gameObject);
                    _damageTimers[collider] = currentTime;

                    Vector3 hitSourcePosition = _ownerActor.PlayerInventory.EquippedWeaponInstance.WeaponTip.position;
                    Vector3 hitPoint = collider.ClosestPoint(hitSourcePosition);
                    Vector3 direction = hitPoint - hitSourcePosition;
                    Quaternion hitRotation = Quaternion.LookRotation(-direction);

                    Instantiate(_onHitVfxObject, hitPoint, hitRotation);
                }
                else if (currentTime - _damageTimers[collider] >= normalDamageCooldown)
                {
                    EventManager.Instance.CombatHits.TriggerCombatHit(this, collider.gameObject);
                    _damageTimers[collider] = currentTime;


                    Vector3 hitSourcePosition = _ownerActor.PlayerInventory.EquippedWeaponInstance.WeaponTip.position;
                    Vector3 hitPoint = collider.ClosestPoint(hitSourcePosition);
                    Vector3 direction = hitPoint - hitSourcePosition;
                    Quaternion hitRotation = Quaternion.LookRotation(-direction);

                    Instantiate(_onHitVfxObject, hitPoint, hitRotation);
                }
            }
        }
    }


    private void HandleCleanUp()
    {
        _dictCleanTimer += Time.deltaTime;
        if (_dictCleanTimer >= _dictCleanInterval)
        {
            if (_damageTimers.Count == 0)
                return;

            _dictCleanTimer = 0f;
            CleanOldDamageTimers();
        }
    }
    private void CleanOldDamageTimers()
    {
        float currentTime = Time.time;

        _cachedKeysToRemove.Clear();

        foreach (var entry in _damageTimers)
        {
            if (currentTime - entry.Value > _dictCleanInterval)
            {
                _cachedKeysToRemove.Add(entry.Key);
            }
        }

        for (int i = 0; i < _cachedKeysToRemove.Count; i++)
        {
            _damageTimers.Remove(_cachedKeysToRemove[i]);
        }

    }


    private void OnDrawGizmos()
    {
        if (hasSpawned)
        {
            if (_cachedCollider == null)
                _cachedCollider = GetComponent<Collider>();

            if(foundEnemy)
                Gizmos.color = new Color(0f, 1f, 0f, 0.4f);
            else
                Gizmos.color = new Color(1f, 0f, 0f, 0.4f);

            if (_cachedCollider is SphereCollider sphereCollider)
            {
                Vector3 position = transform.position + sphereCollider.center;
                Gizmos.DrawSphere(position, sphereCollider.radius);
            }
            else if (_cachedCollider is BoxCollider boxCollider)
            {
                Vector3 position = transform.position + boxCollider.center;
                Gizmos.DrawCube(position, boxCollider.size);
            }
            else
            {
                Debug.LogWarning("Unsupported collider type for Gizmos visualization.");
            }
        }

    }

}