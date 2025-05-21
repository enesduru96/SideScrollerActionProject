using Drakkar.GameUtils;
using System.Collections;
using UnityEngine;

public class MeleeWeaponTrail : MonoBehaviour
{
    [SerializeField] private DrakkarTrail trail;


    private float trailSpawnMomentTimer = 0f;
    private bool trailSpawnStarted = false;

    private float trailLifetimeTimer = 0f;
    private bool trailActive = false;
    public void ActivateTrailAfterDelay(float spawnMoment, float trailLifetime)
    {
        trailSpawnMomentTimer = spawnMoment;
        trailLifetimeTimer = trailLifetime;
        trailSpawnStarted = true;
    }

    private void Update()
    {
        if (trailActive)
        {
            trailLifetimeTimer -= Time.deltaTime;
            if (trailLifetimeTimer < 0f)
            {
                DeactivateTrail();
            }

            return;
        }

        if (trailSpawnStarted)
        {
            trailSpawnMomentTimer -= Time.deltaTime;
            if(trailSpawnMomentTimer < 0f)
            {
                trailSpawnStarted = false;
                trailActive = true;
                trail.Begin();
            }
        }

    }

    public void DeactivateTrail()
    {
        trailSpawnStarted = false;
        trailActive = false;
        trail.End();
    }
}
