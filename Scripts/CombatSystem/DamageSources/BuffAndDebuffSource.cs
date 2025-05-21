using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuffAndDebuffSource : MonoBehaviour
{
    [Header("Base Parameters")]
    [SerializeField] private Faction faction;
    [SerializeField] protected bool hasLifeTime = true; // For aoe effects that are not dependent on the animation duration
    [SerializeField] protected float lifeTime = 5f;

    [Header("Buff Parameters")]
    [SerializeField] private bool hasBuff;
    [SerializeField] private BuffType buffType;
    [SerializeField] private float buffAmount;
    [SerializeField] [Range(1f, 300f)] private float buffDuration;

    [Header("Debuff Parameters")]
    [SerializeField] private bool hasDebuff;
    [SerializeField] private DebuffType debuffType;
    [SerializeField] private float debuffAmount;
    [SerializeField] private float debuffDuration;

    [Header("UI Related Parameters")]
    [SerializeField] Texture2D effectIcon;


    public Faction Faction => faction;
    public bool HasLifeTime => hasLifeTime;
    public float LifeTime => lifeTime;
    public bool HasBuff => hasBuff;
    public BuffType BuffType => buffType;
    public float BuffAmount => buffAmount;
    public float BuffDuration => buffDuration;
    public bool HasDebuff => hasDebuff;
    public DebuffType DebuffType => debuffType;
    public float DebuffAmount => debuffAmount;
    public float DebuffDuration => debuffDuration;


    private Collider cachedCollider;

    private void Awake()
    {
        cachedCollider = GetComponent<Collider>();
        if (!cachedCollider.isTrigger)
        {
            Debug.LogError($"The Collider on {gameObject.name} must be set as a trigger!");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable target))
        {
            EventManager.Instance.CombatHits.TriggerBuffDebuffHit(this, other.gameObject);
        }
    }


    private void OnDrawGizmos()
    {
        if (cachedCollider == null)
            cachedCollider = GetComponent<Collider>();

        Gizmos.color = Color.yellow;

        if (cachedCollider is SphereCollider sphereCollider)
        {
            Vector3 position = transform.position + sphereCollider.center;
            Gizmos.DrawSphere(position, sphereCollider.radius);
        }
        else if (cachedCollider is BoxCollider boxCollider)
        {
            Vector3 position = transform.position + boxCollider.center;
            Gizmos.DrawWireCube(position, boxCollider.size);
        }
        else if (cachedCollider is CapsuleCollider capsuleCollider)
        {
            Vector3 position = transform.position + capsuleCollider.center;
            Gizmos.DrawWireSphere(position + Vector3.up * (capsuleCollider.height / 2 - capsuleCollider.radius), capsuleCollider.radius);
            Gizmos.DrawWireSphere(position + Vector3.down * (capsuleCollider.height / 2 - capsuleCollider.radius), capsuleCollider.radius);
        }
        else
        {
            Debug.LogWarning($"Collider type {cachedCollider.GetType()} is not supported for visualization.");
        }
    }
}
