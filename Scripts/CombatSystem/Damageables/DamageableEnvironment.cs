using UnityEngine;

public class DamageableEnvironment : MonoBehaviour, IDamageable
{
    public DamageableType DamageableType => DamageableType.Character;


    [Header("Damageable Profile")]
    [SerializeField] private BreakableProfile breakableProfile;
    public ScriptableObject Profile => breakableProfile;


    private float currentHealth;

    private void Awake()
    {
        currentHealth = breakableProfile.maxHealth;
    }

    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        
    }

    public virtual void TakeDamage(DamageSource damageObject)
    {

    }

    public virtual void Heal(float damage)
    {

    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }


}
