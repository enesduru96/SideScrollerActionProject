using UnityEngine;
using Animancer;
using ShadowFort.Utilities;
using System.Collections;

public class AnimationPlayer : MonoBehaviour
{
    private System.Action OnAnimationEndCallback;
    private AnimancerComponent _animancer;

    private void Awake()
    {
        _animancer = GetComponent<AnimancerComponent>();
    }

    public void PlayAnimation(ClipTransition animationClip, int layerIndex, Easing.Function _easingFunction, System.Action onEndCallback = null)
    {
        OnAnimationEndCallback = onEndCallback;
        AnimancerState state = _animancer.Layers[layerIndex].Play(animationClip);
        state.FadeGroup.SetEasing(_easingFunction);

        if (onEndCallback != null)
            state.Events(this).OnEnd ??= HandleAnimationEnd;
    }

    public void PlayAnimationWithZeroTime(ClipTransition animationClip, int layerIndex, Easing.Function _easingFunction, System.Action onEndCallback = null)
    {
        OnAnimationEndCallback = onEndCallback;
        AnimancerState state = _animancer.Layers[layerIndex].Play(animationClip);
        state.FadeGroup.SetEasing(_easingFunction);
        state.Time = 0f;
    }

    public void TransitionToAnimation(ClipTransition animationClip, float transitionDuration, Easing.Function _easingFunction, int layerIndex, System.Action onEndCallback = null)
    {
        OnAnimationEndCallback = onEndCallback;
        AnimancerState state = _animancer.Layers[layerIndex].Play(animationClip, transitionDuration);
        state.FadeGroup.SetEasing(_easingFunction);

        if (onEndCallback != null)
            state.Events(this).OnEnd ??= HandleAnimationEnd;
    }

    public void TransitionToAnimationDefaultDuration(ClipTransition animationClip, Easing.Function _easingFunction, int layerIndex, System.Action onEndCallback = null)
    {
        OnAnimationEndCallback = onEndCallback;
        AnimancerState state = _animancer.Layers[layerIndex].Play(animationClip);
        state.FadeGroup.SetEasing(_easingFunction);

        if (onEndCallback != null)
            state.Events(this).OnEnd ??= HandleAnimationEnd;
    }

    private void HandleAnimationEnd()
    {
        OnAnimationEndCallback?.Invoke();
    }

}
