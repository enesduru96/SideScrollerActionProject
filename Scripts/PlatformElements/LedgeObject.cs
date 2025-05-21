using UnityEngine;
using System.Collections;
using UnityEngine.Splines;


public enum LedgeNeighbourUp
{
    None,
    SidewaysBracedLedge,
    SidewaysFreeHangLedge
}


public class LedgeObject : MonoBehaviour
{
    [Header("Movement Spline and Points")]
    public SplineContainer movementPath;
    public Transform AboveBracedEntryPoint;

    [Header("Parameters")]
    public bool canBeClimbedUp;

    [Header("Neighbouring Ledges")]
    public bool HasJumpableLedgeAbove;
    public LedgeObject AboveLedgeObject;

    [Header("Current user")]
    public CharacterActor currentActor;

    public void SetActor(CharacterActor _actor) => currentActor = _actor;

    public IEnumerator SetActorWithDelay(CharacterActor _actor, float time)
    {
        yield return new WaitForSeconds(time);
        currentActor = _actor;
    }

    void Start()
    {
        if (movementPath == null)
        {
            Debug.LogError("SplineContainer atanmamýþ!");
            return;
        }
    }
}