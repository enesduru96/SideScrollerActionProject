using UnityEngine;



//TODO: After prototyping, convert to event-based system so that PerceptionAI only notifies when perception events occur like:
// Saw the player, heard the player, player is attacking me, player is walking/running/dashing towards/away from me, player is blocking, jumping etc.

[RequireComponent(typeof(SphereCollider))]
public class PerceptionAI : MonoBehaviour
{
    [Header("Hearing (Ses Algýsý)")]
    [SerializeField, Tooltip("The radius AI can hear the player inside")]
    private float hearingRadius = 5f;

    [SerializeField, Tooltip("Target layer mask for AI to hear the player")]
    private LayerMask hearingMask;

    [Space(20)]

    [Header("Vision")]
    [SerializeField, Tooltip("AI’s vision distance (meter)")]
    private float visionDistance = 10f;

    private float visionHeight = 1.5f;


    [SerializeField, Tooltip("The target (usually player) layerMask")]
    private LayerMask targetMask;

    [SerializeField]
    private LayerMask obstacleMask;


    private SphereCollider hearingCollider;
    private AIFollowBehaviour2D followBehaviour;


    private Transform heardTarget = null;
    private Transform seenTarget = null;

    void Awake()
    {
        hearingCollider = GetComponent<SphereCollider>();
        hearingCollider.isTrigger = true;
        hearingCollider.radius = hearingRadius;
        hearingCollider.center = Vector3.zero;

        followBehaviour = GetComponent<AIFollowBehaviour2D>();
        if (followBehaviour == null)
            Debug.LogError("AIFollowBehaviour2D component bulunamadý!", this);
    }

    // Change the collider radius when the perception radius changes
    void OnValidate()
    {
        var col = GetComponent<SphereCollider>();
        if (col != null)
            col.radius = hearingRadius;
    }

    void Update()
    {
        VisionCheck();


        // Testing followBehaviour -> will be eliminated by events
        if (seenTarget != null)
            followBehaviour.SetFollowTarget(seenTarget);
        else if (heardTarget != null)
            followBehaviour.SetFollowTarget(heardTarget);
        else
            followBehaviour.SetFollowTarget(null);
    }

    void VisionCheck()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * visionHeight;
        int combinedMask = targetMask | obstacleMask;

        if (Physics.Raycast(rayOrigin, transform.forward, out RaycastHit hit, visionDistance, combinedMask))
        {
            if (((1 << hit.collider.gameObject.layer) & targetMask) != 0)
                seenTarget = hit.collider.transform;
            else
                seenTarget = null;
        }
        else
        {
            seenTarget = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hearingMask) != 0)
            heardTarget = other.transform;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == heardTarget)
            heardTarget = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);

        Gizmos.color = Color.cyan;
        Vector3 rayOrigin = transform.position + Vector3.up * visionHeight;
        Gizmos.DrawLine(rayOrigin, rayOrigin + transform.forward * visionDistance);
    }
}