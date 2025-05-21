using System;
using UnityEngine;



using ShadowFort.Utilities;
using System.Collections;
using Animancer;

public class DamageableCharacter : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private CharacterProfileStorage profileStorage;
    [SerializeField] private CharacterProfile characterProfile;
    [SerializeField] private CharacterActor characterActor;
    [SerializeField] private CharacterStateController stateController;
    [SerializeField] private CharacterMoverAndRotator characterMover;

    public CharacterStateController StateController => stateController;
    public CharacterActor CharacterActor => characterActor;
    public ScriptableObject Profile => characterProfile;
    public DamageableType DamageableType => DamageableType.Character;


    #region Private Declaration

    // Character Information
    private Texture2D _characterIcon;
    private string _characterName;
    private Faction _characterFaction;

    // Base Stats
    private float _maxHealth;
    private float _currentHealth;
    private float _poiseAmount;
    private float _armour;
    private float _attackPower;
    private float _armourPenetration;

    // "Hit Reactions"
    private bool _canBeStunned;
    private bool _canBePushedBack;
    private bool _canBeKnockedDown;
    private bool _canBeLaunchedToAir;

    // Resistances (%)
    private float _poisonResistance;
    private float _bleedResistance;
    private float _fireResistance;
    private float _iceResistance;
    private float _magicResistance;

    // Leveled Resistances
    private int stunResistance;
    private int pushbackResistance;
    private int pushbackForceReduction;
    private int knockdownResistance;
    private int airLaunchResistance;

    // Special Properties
    private bool _enrageOnLowHealth;
    private float _enrageHealthThreshold;
    private float _healthRegenPerSecond;

    #endregion

    #region Public Getters

    // Character Information
    public Texture2D CharacterIcon => _characterIcon;
    public string CharacterName => _characterName;
    public Faction CharacterFaction => _characterFaction;

    // Base Stats
    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;
    public float PoiseAmount => _poiseAmount;
    public float Armour => _armour;
    public float AttackPower => _attackPower;
    public float ArmourPenetration => _armourPenetration;

    // Hit Reactions
    public bool CanBeStunned => _canBeStunned;
    public bool CanBeKnockedBack => _canBePushedBack;
    public bool CanBeKnockedDown => _canBeKnockedDown;
    public bool CanBeLaunchedToAir => _canBeLaunchedToAir;

    // Resistances (%)
    public float PoisonResistance => _poisonResistance;
    public float BleedResistance => _bleedResistance;
    public float FireResistance => _fireResistance;
    public float IceResistance => _iceResistance;
    public float MagicResistance => _magicResistance;


    // Leveled Resistances
    public int StunResistance => stunResistance;
    public int PushbackResistance => pushbackResistance;
    public int PushbackForceReduction => pushbackForceReduction;
    public int KnockdownResistance => knockdownResistance;
    public int AirLaunchResistance => airLaunchResistance;


    // Special Properties
    public bool EnrageOnLowHealth => _enrageOnLowHealth;
    public float EnrageHealthThreshold => _enrageHealthThreshold;
    public float HealthRegenPerSecond => _healthRegenPerSecond;

    #endregion


    [SerializeField] private DamageSource latestDamageSource;
    [SerializeField] private AnimancerComponent _animancer;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (characterActor == null)
            Debug.LogWarning($"{name}: CharacterActor is null!", this);

        if (stateController == null)
            Debug.LogWarning($"{name}: StateController is null!", this);

        if (characterMover == null)
            Debug.LogWarning($"{name}: CharacterMover is null!", this);

        if (profileStorage == null)
            Debug.LogWarning($"{name}: profileStorage is null!", this);
    }
#endif

    private void Awake()
    {
        characterProfile = profileStorage.characterProfile;
    }

    private void Start()
    {
        GetStatsFromProfile();
    }

    private void GetStatsFromProfile()
    {
        _characterIcon = characterProfile.CharacterIcon;
        _characterName = characterProfile.CharacterName;
        _characterFaction = characterProfile.CharacterFaction;

        _maxHealth = characterProfile.MaxHealth;
        _currentHealth = _maxHealth;
        _poiseAmount = characterProfile.PoiseAmount;
        _armour = characterProfile.baseArmour;
        _attackPower = characterProfile.BaseAttackPower;
        _armourPenetration = characterProfile.BaseArmorPenetration;

        _poisonResistance = characterProfile.PoisonResistance;
        _bleedResistance = characterProfile.BleedResistance;
        _fireResistance = characterProfile.FireResistance;
        _iceResistance = characterProfile.IceResistance;
        _magicResistance = characterProfile.MagicResistance;

        _healthRegenPerSecond = characterProfile.HealthRegenPerSecond;

        _canBeStunned = characterProfile.CanBeStunned;
        _canBePushedBack = characterProfile.CanBePushedBack;
        _canBeKnockedDown = characterProfile.CanBeKnockedDown;
        _enrageOnLowHealth = characterProfile.EnrageOnLowHealth;
        _enrageHealthThreshold = characterProfile.EnrageHealthThreshold;
    }

    // After getting stats, recalculate them with equipped items (or permanent buffs)
    // and prepare the final stats of the character
    private void CalculateItemEffects()
    {

    }

    private void OnEnable()
    {
        EventManager.Instance.CombatHits.OnCombatHit += HandleIncomingHit;

    }

    private void OnDisable()
    {
        EventManager.Instance.CombatHits.OnCombatHit -= HandleIncomingHit;
    }

    private bool processingHit = false;
    protected virtual void HandleIncomingHit(DamageSource damageSource, GameObject damageableTarget)
    {
        if (damageableTarget != gameObject)
            return;

        if (processingHit)
            return;


        if (latestDamageSource == damageSource)
            return;


        if (stateController.CurrentState is BlockingState)
        {
            float blockElapsedTime = stateController.CurrentState.GetElapsedTime();

            if (blockElapsedTime <= 0.2f)
            {
                Debug.Log("Perfect Block");
            }
            else
            {
                Debug.Log("Normal Block");
            }
        }
        else
        {

            TakeDamage(damageSource);
        }


        CalculateDamageReduction(damageSource);

        //Debug.Log($"I'm hit: {damageSource.BaseDamage}");
        EventManager.Instance.CombatHits.TriggerCharacterTookDamage(this, damageSource);
    }


    public void StopTime(float pauseDuration)
    {
        if (waiting)
            return;

        if (_animancer == null)
            return;

        _animancer.States.Current.IsPlaying = false;


        StartCoroutine(StopTimeCoroutine(pauseDuration));
    }

    bool waiting = false;
    private IEnumerator StopTimeCoroutine(float pauseDuration)
    {
        waiting = true;
        yield return new WaitForSeconds(pauseDuration);
        _animancer.States.Current.IsPlaying = true;
        waiting = false;
    }

    private void CalculateDamageReduction(DamageSource damageSource)
    {

    }

    private void Update()
    {
        if (!willResetDamageSource)
            return;

        sourceResetElapsedTime += Time.deltaTime;
        if (sourceResetElapsedTime > sourceResetDuration)
        {
            willResetDamageSource = false;
            sourceResetElapsedTime = 0f;
            latestDamageSource = null;
        }
    }

    private bool willResetDamageSource = false;
    [SerializeField] private float sourceResetDuration = 0.5f;
    private float sourceResetElapsedTime = 0f;
    public virtual void TakeDamage(DamageSource damageSource)
    {
        willResetDamageSource = true;
        latestDamageSource = damageSource;

        _currentHealth -= damageSource.BaseDamage;

        Debug.Log($"{gameObject.name} took {damageSource.BaseDamage} damage. Remaining health: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            stateController.ForceState<DamagedState>();
            HandlePushback(damageSource);
            StopTime(0.05f);
        }

    }


    private void HandlePushback(DamageSource damageSource)
    {
        if (!_canBePushedBack || !damageSource.CanPushback)
            return;

        // Hasar kaynaðýna doðru yön vektörünü hesapla
        Vector3 directionToDamageSource = (damageSource.OwnerActor.transform.position - transform.position).normalized;

        // Sadece X ekseni üzerinde knockback yönünü hesapla
        Vector3 pushbackDirection = Vector3.zero;
        float dotProductX = Vector3.Dot(directionToDamageSource, Vector3.right);

        if (dotProductX > 0)
            pushbackDirection.x = -1; // Saðdan vuruldu, sola doðru it
        else if (dotProductX < 0)
            pushbackDirection.x = 1; // Soldan vuruldu, saða doðru it

        pushbackDirection.z = 0;
        pushbackDirection.y = 0;


        //characterActor.Animator.CrossFade("Hit_Combat_Large_F", 0.2f);

        characterMover.StartVelocityPush(pushbackDirection, damageSource.PushbackDuration, damageSource.PushbackMovementForce);

        //knockbackCoroutine = StartCoroutine(ApplyKnockBack(knockbackForceVector, 0.1f));
    }


    private IEnumerator ApplyKnockBack(Vector3 force, float duration)
    {
        processingHit = true;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            characterActor.Velocity = force;

            yield return null;
        }


        processingHit = false;

    }


    private void HandleKnockDown()
    {

    }

    public virtual void Heal(float healAmount)
    {
        _currentHealth = Mathf.Min(_currentHealth + healAmount, _maxHealth);
        Debug.Log($"{gameObject.name} healed {healAmount}. Current health: {_currentHealth}");
    }

    protected virtual void RegenerateHealth(float healAmount)
    {

    }

    protected virtual void Die()
    {
        gameObject.SetActive(false);
    }


    public float GetCurrentHealth()
    {
        return _currentHealth;
    }
}