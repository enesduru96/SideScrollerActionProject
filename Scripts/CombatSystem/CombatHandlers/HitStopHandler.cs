using UnityEngine;
using Animancer;

using System.Collections;
using ShadowFort.Utilities;

public class HitStopHandler : MonoBehaviour
{
    [SerializeField] private AnimancerComponent animancerComponent;
    [SerializeField] private CharacterActor characterActor;

    [SerializeField] private float recoverDelay = 0.2f;

    private Coroutine hitStopCoroutine;

    private void Start()
    {
        characterActor = this.GetComponentInBranch<CharacterActor>();
        animancerComponent = characterActor.GetComponentInBranch<AnimancerComponent>();
    }

    public bool TryApplyHitStop(float pauseDuration)
    {
        if (hitStopCoroutine != null || pauseDuration <= 0f)
            return false;

        hitStopCoroutine = StartCoroutine(HitStopCoroutine(pauseDuration));
        return true;
    }

    private IEnumerator HitStopCoroutine(float pauseDuration)
    {
        var state = animancerComponent.States.Current;
        bool wasPlaying = state.IsPlaying;

        state.IsPlaying = false;

        yield return Wait.ForSecondsRealtime(pauseDuration);

        state.IsPlaying = wasPlaying;

        if (pauseDuration < recoverDelay)
            yield return Wait.ForSecondsRealtime(recoverDelay);

        hitStopCoroutine = null;
    }


    private void OnDisable()
    {
        if (hitStopCoroutine != null)
        {
            StopCoroutine(hitStopCoroutine);
            hitStopCoroutine = null;
        }
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (characterActor == null)
            Debug.LogWarning($"{name}: CharacterActor is null!", this);

        if (animancerComponent == null)
            Debug.LogWarning($"{name}: animancerComponent is null!", this);
    }
#endif
}
