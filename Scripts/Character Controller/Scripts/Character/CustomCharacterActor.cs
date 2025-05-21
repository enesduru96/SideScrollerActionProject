using UnityEngine;

public class CustomCharacterActor : CharacterActor
{
    [Header("2D Constraints")]
    public bool lockZMovement = true;
    public float ZPosition = 0f;

    [Header("Smoothing Settings")]
    public float zSmoothSpeed = 5f;  // Ge�i� h�z�

    protected override void PreSimulationUpdate(float dt)
    {
        base.PreSimulationUpdate(dt);
        if (lockZMovement)
        {
            SmoothZPosition(dt);
        }
    }

    protected override void PostSimulationUpdate(float dt)
    {
        base.PostSimulationUpdate(dt);
        if (lockZMovement)
        {
            SmoothZPosition(dt);
        }
    }

    private void SmoothZPosition(float dt)
    {
        var p = Position;
        // Z eksenindeki pozisyonu yumu�atarak hedef pozisyona yakla�t�r
        p.z = Mathf.Lerp(p.z, ZPosition, zSmoothSpeed * dt);
        Position = p;
    }

}
