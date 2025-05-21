using UnityEditor;
using UnityEngine;


/// <summary>
/// Stores the character information and combat stats (saveable)
/// Will be used by various parties on awake.
/// </summary>

[CreateAssetMenu(fileName = "CharacterProfile", menuName = "NocturneKeepInteractive/Identities/CharacterProfile")]
public class CharacterProfile : ScriptableObject
{
    [Header("Character Information")]
    public Texture2D CharacterIcon;
    public string CharacterName;
    public Faction CharacterFaction;


    [Header("Base Animations")]
    public StateAnimations_NormalMovement animListNormalMovement;
    public StateAnimations_NormalMovement_Combat animListNormalMovementCombat;


    [Header("Base Stats")]
    public float MaxHealth;
    public float PoiseAmount;
    public float baseArmour;
    public float BaseAttackPower;
    public float BaseAttackSpeed;
    public float BaseArmorPenetration;


    [Header("Hit Reactions")]
    public bool CanBeStunned;
    public bool CanBePushedBack;
    public bool CanBeKnockedDown;
    public bool CanBeLaunchedToAir;


    [Header("Special Properties")]
    public bool EnrageOnLowHealth;
    [Range(0f, 100f)] public float EnrageHealthThreshold;
    [Range(0f, 100f)] public float HealthRegenPerSecond;


    [Header("Resistances (%)")]
    [Range(0f, 100f)] public float PoisonResistance;
    [Range(0f, 100f)] public float BleedResistance;
    [Range(0f, 100f)] public float FireResistance;
    [Range(0f, 100f)] public float IceResistance;
    [Range(0f, 100f)] public float MagicResistance;


    [Header("Leveled Resistances")] // If level is higher than the attacker's level, is not affected
    [Range(0, 10)] public int StunResistance;
    [Range(0, 10)] public int PushbackResistance;
    [Range(0, 10)] public int PushbackForceReduction;
    [Range(0, 10)] public int KnockdownResistance;
    [Range(0, 10)] public int AirLaunchResistance;


    [Space(50)]

    [Header("Actor Body Parameters")]
    [Range(0f, 20f)] public float CharacterHeight;
    [Range(0f, 20f)] public float CharacterWidth;
    [Range(0f, 1000f)] public float CharacterMass;

    [Header("Dash State Parameters")]
    [Range(0f, 100f)] public float CharacterDashVelocity;
    [Range(0f, 5f)] public float CharacterDashDuration;
    [Range(0, 5)] public int AvailableNotGroundedDashes;

    [Header("Movement State Parameters")]
    [Range (0f, 1f)] public float SpeedMinThresholdForAnim = 0.2f;
    [Range(1f, 20f)] public float WalkSpeed;
    [Range(1f, 20f)] public float RunSpeed;
    [Range(1f, 20f)] public float SprintSpeed;
    [Range(1f, 20f)] public float CharacterJumpHeight;
    [Range(0, 3)] public int AvailableAirJumps;






    [Header("Save Profile")]
    public bool isSaved;

}
