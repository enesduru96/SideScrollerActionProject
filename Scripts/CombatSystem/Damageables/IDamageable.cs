using UnityEngine;

public interface IDamageable
{
    DamageableType DamageableType { get; }
    ScriptableObject Profile { get; }
    void TakeDamage(DamageSource damageObject);
    void Heal(float amount);
    float GetCurrentHealth();
}