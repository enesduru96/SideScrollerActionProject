using System.Collections.Generic;
using UnityEngine;
using ShadowFort.Utilities;
using System;
using Animancer;


[RequireComponent(typeof(CharacterAnimationStateController))]
public abstract class CharacterAnimState : MonoBehaviour
{
    protected CharacterActor characterActor;
    protected CharacterLocalEventManager characterLocalEventManager;

    protected AnimancerComponent _animancer;
    protected AnimationPlayer animationPlayer;

    protected CharacterStateController characterStateController;
    protected CharacterAnimationStateController characterAnimStateController;

    protected virtual void Awake()
    {
        characterActor = this.GetComponentInBranch<CharacterActor>();
        characterLocalEventManager = this.GetComponentInBranch<CharacterActor, CharacterLocalEventManager>();

        _animancer = this.GetComponentInBranch<CharacterActor, AnimancerComponent>();
        animationPlayer = this.GetComponentInBranch<CharacterActor, AnimationPlayer>();

        characterStateController = this.GetComponentInBranch<CharacterActor, CharacterStateController>();
        characterAnimStateController = GetComponent<CharacterAnimationStateController>();

    }


    protected virtual void Start() { }

    public abstract void EnterAnimState(CharacterAnimState fromState);
    public abstract void UpdateAnimState();
    public abstract void ExitAnimState(CharacterAnimState toState);

}
