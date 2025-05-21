using UnityEngine;

public class LocalPositionClamper : MonoBehaviour
{
    [Header("Enable Clamping for Axes")]
    public bool clampX = false;
    public bool clampY = false;
    public bool clampZ = false;

    [Header("Clamp Ranges")]
    public Vector2 xRange = new Vector2(-5f, 5f);
    public Vector2 yRange = new Vector2(-5f, 5f);
    public Vector2 zRange = new Vector2(-5f, 5f);

    void Update()
    {
        Vector3 localPosition = transform.localPosition;

        if (clampX)
        {
            localPosition.x = Mathf.Clamp(localPosition.x, xRange.x, xRange.y);
        }

        if (clampY)
        {
            localPosition.y = Mathf.Clamp(localPosition.y, yRange.x, yRange.y);
        }

        if (clampZ)
        {
            localPosition.z = Mathf.Clamp(localPosition.z, zRange.x, zRange.y);
        }

        transform.localPosition = localPosition;
    }
}