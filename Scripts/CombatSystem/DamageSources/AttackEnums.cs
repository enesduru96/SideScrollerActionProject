using UnityEngine;

public enum AttackID
{
    None = 0,
    LightAttack_1,
    LightAttack_2,
    LightAttack_3,
    LightAttack_4,
    LightAttack_5,
    LightAttack_6,
    AirLightAttack_1,
    AirLightAttack_2,
    AirLightAttack_3,
    HeavyAttack_1,
    HeavyAttack_2,
    HeavyAttack_3,
    HeavyAttack_4,
    HeavyAttack_5,
    HeavyAttack_6,
    AirHeavyAttack_1,
    AirHeavyAttack_2,
    AirHeavyAttack_3,
    SpecialAttack_1,
    SpecialAttack_2,
    SpecialAttack_3,
    SpecialAttack_4,
    SpecialAttack_5,
    SpecialAttack_6,
    SpecialAttack_7,
    SpecialAttack_8,
    AirSpecialAttack_1,
    AirSpecialAttack_2,
    AirSpecialAttack_3,
    ConsumableAttack_1,
    ConsumableAttack_2,
    ConsumableAttack_3,
}

public enum AttackType
{
    Grounded,
    Airborne,
}

public enum StatusEffect
{
    None,
    Bleed,
    Poison,
    Burn
}

public enum DamageType
{
    LightAttackDamage,
    HeavyAttackDamage,
    SpecialAttackDamage,
    FireDamage,
    IceDamage,
    MagicDamage,
    TrapDamage
}

public enum DamageSpawnPoint
{
    None,
    MainWeapon,
    SecondHandItem,
    RightHand,
    LeftHand,
    RightFoot,
    LeftFoot,
    Head,
    Chest,
}


public enum BuffType
{
    Heal,
    HealOverTime,
    IncreaseDamage,
    IncreaseDefense,
    IncreaseAllResist,
    IncreaseAttackSpeed,
    IncreaseMoveSpeed
}

public enum DebuffType
{
    DecreaseDamage,
    DecreaseDefense,
    DecreaseAttackSpeed,
    DecreaseMoveSpeed,
    DecreaseAllResist
}

public enum AttackMovementDirection
{
    Forward,
    Backward,
    Up,
    ForwardUp,
    BackwardUp,
}