using UnityEngine;
using ShadowFort.Utilities;
using Animancer;

public class Dash_AnimState : CharacterAnimState
{

    [Header("State Animations")]
    [SerializeField] private StateAnimations_Dash dashAnimList;


    [Header("Speed Parameters")]
    [SerializeField] private float dashStartDuration = 0.2f;
    private float dashStartElapsedTime = 0f;

    [SerializeField] private float dashLoopDuration;
    private float dashLoopElapsedTime = 0f;

    [SerializeField] private float dashEndDuration = 0.2f;
    private float dashEndElapsedTime = 0f;


    private Dash _dashState;

    private enum SubState
    {
        None,
        DashStart,
        DashLoop,
        DashEnd
    }
    private SubState currentSubState = SubState.None;
    private bool TryChangeSubState(SubState newSubState)
    {
        if (currentSubState == newSubState)
            return false;

        switch (currentSubState)
        {

            case SubState.None:
                currentSubState = newSubState;
                return true;


            default:
                currentSubState = newSubState;
                return true;


                //case SubState.DashLoop:
                //    if (newState == PlayerAnimState.JumpStart)
                //    {
                //        HandleDashJumpAnimation();
                //    }
                //    currentSubState = newSubState;
                //    return true;



        }

    }
    private void TransitionToSubState(SubState newSubState, ClipTransition anim, float transitionDuration)
    {
        if (!TryChangeSubState(newSubState))
            return;

        characterAnimStateController.CurrentAnim = anim;
        animationPlayer.TransitionToAnimation(anim, transitionDuration, Easing.Function.Linear, 0);


    }




    private void OnEnable()
    {
        characterLocalEventManager.CharacterInput.OnPlayerDashStarted += StartDashAnimation;
    }

    private void OnDisable()
    {
        characterLocalEventManager.CharacterInput.OnPlayerDashStarted -= StartDashAnimation;
    }



    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Start()
    {
        base.Start();

        _dashState = characterStateController.GetComponent<Dash>();
    }



    private void StartDashAnimation()
    {
        dashStartDuration = _dashState.Duration * 0.4f;
        dashLoopDuration = _dashState.Duration * 0.3f;
        dashEndDuration = _dashState.Duration * 0.3f;

        dashStartElapsedTime = 0f;
        characterAnimStateController.CurrentAnim = dashAnimList.DashStart;
        animationPlayer.TransitionToAnimationDefaultDuration(characterAnimStateController.CurrentAnim, Easing.Function.Linear, 0);
        TryChangeSubState(SubState.DashStart);
    }


    public override void EnterAnimState(CharacterAnimState fromState)
    {
        //Debug.Log("Entering Dash Animation State");
    }


    public override void UpdateAnimState()
    {
        UpdateDashAnimation();
    }

    private void UpdateDashAnimation()
    {
        switch (currentSubState)
        {
            case SubState.DashStart:
                dashStartElapsedTime += Time.deltaTime;
                if (dashStartElapsedTime >= dashStartDuration)
                {
                    dashStartElapsedTime = 0f;
                    TransitionToSubState(SubState.DashLoop, dashAnimList.DashLoop, 0.1f);
                }
                break;

            case SubState.DashLoop:
                dashLoopElapsedTime += Time.deltaTime;
                if (dashLoopElapsedTime >= dashLoopDuration)
                {
                    dashLoopElapsedTime = 0f;
                    TransitionToSubState(SubState.DashEnd, dashAnimList.DashEnd, 0.1f);
                }
                break;

            case SubState.DashEnd:
                dashEndElapsedTime += Time.deltaTime;
                if (dashEndElapsedTime >= dashEndDuration)
                {
                    dashEndElapsedTime = 0f;
                    TryChangeSubState(SubState.None);
                    characterAnimStateController.ChangeAnimState<NormalMovementXY_AnimState>();
                }
                break;
        }
    }




    public override void ExitAnimState(CharacterAnimState toState)
    {
        //Debug.Log("Exiting Dash Animation State");
    }




}