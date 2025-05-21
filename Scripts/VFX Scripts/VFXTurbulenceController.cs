using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class VFXTurbulenceController : MonoBehaviour
{
    private enum eVfxType
    {
        TriggerBurst,
        ChangeTurbulence,
        ChangeTurbulenceSmooth
    }

    [SerializeField] private eVfxType vfxType = eVfxType.TriggerBurst;

    public VisualEffect vfx;
    public string burstEventName = "OnBurst";
    public string continuousEventName = "OnContinuous";
    public string stopEventName = "OnStop";
    public string directionPropertyName = "TurbulenceDirection";

    [Range(0.1f, 5f)] public float interval = 1f;
    [SerializeField] private bool updateDynamically = true;


    private float timer = 0f;


    [Header("Smooth Lerp Settings")]
    [Range(0.1f, 10f)] public float smoothSpeed = 1f;
    private float lerpProgress = 0f;

    private bool isLerping = false;
    private Vector3 currentDirection;
    private Vector3 targetDirection;


    public void TriggerBurst()
    {
        Vector3 newDirection = Random.onUnitSphere;
        vfx.SetVector3(directionPropertyName, newDirection);
        vfx.SendEvent(burstEventName);
    }

    private void ChangeTurbulence()
    {
        Vector3 newDirection = Random.onUnitSphere;
        vfx.SetVector3(directionPropertyName, newDirection);
    }

    private void ChangeTurbulenceSmooth()
    {
        targetDirection = Random.onUnitSphere;
        lerpProgress = 0f;
        isLerping = true;
    }

    private void Start()
    {
        Vector3 newDirection = Random.onUnitSphere;
        currentDirection = newDirection;
        targetDirection = newDirection;
        vfx.SetVector3(directionPropertyName, newDirection);

        TriggerBurst();
    }

    private void Update()
    {
        if (!updateDynamically)
            return;


        timer += Time.deltaTime;

        if (timer > interval)
        {
            timer = 0f;

            switch (vfxType)
            {
                case eVfxType.TriggerBurst:
                    TriggerBurst();
                    break;
                case eVfxType.ChangeTurbulence:
                    ChangeTurbulence();
                    break;
                case eVfxType.ChangeTurbulenceSmooth:
                    ChangeTurbulenceSmooth();
                    break;
            }
        }

        // Smooth turbulence
        if (isLerping && vfxType == eVfxType.ChangeTurbulenceSmooth)
        {
            lerpProgress += Time.deltaTime * smoothSpeed;
            float t = Mathf.Clamp01(lerpProgress);

            currentDirection = Vector3.Slerp(currentDirection, targetDirection, t);
            vfx.SetVector3(directionPropertyName, currentDirection);

            if (t >= 1f)
                isLerping = false;
        }

    }

}