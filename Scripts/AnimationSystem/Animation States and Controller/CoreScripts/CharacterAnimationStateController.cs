using UnityEngine;
using ShadowFort.Utilities;
using Animancer;
using System.Collections.Generic;
using System;
using System.Collections;


public class CharacterAnimationStateController : MonoBehaviour
{
    [Header("Profile References")]
    [SerializeField] private CharacterProfileStorage _characterProfileStorage;
    [SerializeField] private CharacterProfile _characterProfile;

    [Space(30)]

    [Header("Animation Parameters")]
    public ClipTransition CurrentAnim;

    [Space(30)]

    [Header("Avatar Masks")]
    public AvatarMask UpperBodyMask;
    public AvatarMask ChestHeadMask;
    public AvatarMask LowerBodyMask;
    public AvatarMask LeftArmMask;
    public AvatarMask RightArmMask;
    public AnimancerLayer MaskedAnimancerLayer;

    #region PRIVATE REFERENCES

    private CharacterLocalEventManager _playerLocalEventManager;

    private AnimancerComponent _animancer;
    protected CharacterActor _characterActor;
    private CharacterStateController _characterStateController;

    [SerializeField] private CharacterAnimState initialAnimState;
    [SerializeField] private CharacterAnimState previousAnimState;
    [SerializeField] private CharacterAnimState currentAnimState;

    #endregion

    #region AnimState Caching and Transitioning

    private readonly Dictionary<string, CharacterAnimState> animStates = new Dictionary<string, CharacterAnimState>();
    private readonly Dictionary<Type, string> stateNameCache = new Dictionary<Type, string>();

    private void CacheAnimStates()
    {
        CharacterAnimState[] statesArray = _characterActor.GetComponentsInChildren<CharacterAnimState>();
        for (int i = 0; i < statesArray.Length; i++)
        {
            CharacterAnimState state = statesArray[i];
            string stateName = state.GetType().Name;

            if (GetState(stateName) != null)
            {
                Debug.Log("Warning: GameObject " + state.gameObject.name + " has the state " + stateName + " repeated in the hierarchy.");
                continue;
            }

            animStates.Add(stateName, state);
        }
    }
    public CharacterAnimState GetState(string stateName)
    {
        animStates.TryGetValue(stateName, out CharacterAnimState state);

        return state;
    }
    private string GetAnimStateName<T>() where T : CharacterAnimState
    {
        Type type = typeof(T);

        if (!stateNameCache.TryGetValue(type, out string stateName))
        {
            stateName = type.Name;
            stateNameCache[type] = stateName; // Önbelleðe al
        }

        return stateName;
    }
    public CharacterAnimState GetAnimState<T>() where T : CharacterAnimState
    {
        string stateName = GetAnimStateName<T>();
        return GetState(stateName);
    }
    public void ChangeAnimState<T>() where T : CharacterAnimState
    {
        CharacterAnimState state = GetAnimState<T>();

        if (state == null || currentAnimState == state)
            return;

        ChangeAnimState(state);
    }
    private void ChangeAnimState(CharacterAnimState animState)
    {
        if (animState == null)
            return;

        previousAnimState = currentAnimState;
        currentAnimState = animState;

        previousAnimState.ExitAnimState(animState);
        currentAnimState.EnterAnimState(previousAnimState);
    }

    #endregion


    private void OnEnable()
    {
        _characterStateController.OnStateChange += HandleCharacterStateChange;
    }

    private void OnDisable()
    {
        _characterStateController.OnStateChange -= HandleCharacterStateChange;
    }

    private void Awake()
    {
        _playerLocalEventManager = this.GetComponentInBranch<CharacterActor, CharacterLocalEventManager>();

        _characterProfileStorage = this.GetComponentInBranch<CharacterProfileStorage>();
        _characterProfile = _characterProfileStorage.characterProfile;
        _characterActor = this.GetComponentInBranch<CharacterActor>();
        _characterStateController = _characterActor.GetComponentInBranch<CharacterStateController>();
        _animancer = _characterActor.GetComponentInBranch<AnimancerComponent>();
        _animancer.Layers[0].ApplyAnimatorIK = true;
        _animancer.Layers[1].ApplyAnimatorIK = true;

        MaskedAnimancerLayer = _animancer.Layers[1];
        MaskedAnimancerLayer.SetDebugName("Masked Layer");

        CacheAnimStates();
    }
    private void Start()
    {
        currentAnimState = initialAnimState;
        currentAnimState.EnterAnimState(initialAnimState);
    }

    private void HandleCharacterStateChange(CharacterState fromState, CharacterState toState)
    {
        if (currentAnimState == toState)
            return;

        switch (toState)
        {
            case NormalMovementXY:
                ChangeAnimState<NormalMovementXY_AnimState>();
                break;

            case Dash:
                ChangeAnimState<Dash_AnimState>();
                break;

            case LedgeHanging:
                ChangeAnimState<Ledge_AnimState>();
                break;

            case LadderClimbing:
                ChangeAnimState<Ladder_AnimState>();
                break;

            case WallSliding:
                break;

            case RopeClimbing:
                break;

            case VaultJumping:
                ChangeAnimState<Vault_AnimState>();
                break;

            case DamagedState:
                break;
        }
    }





    void Update()
    {
        currentAnimState.UpdateAnimState();
    }
}
