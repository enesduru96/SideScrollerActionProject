using System.Collections;
using UnityEngine;
using ShadowFort.Utilities;


/// <summary>
/// This class represents a state, that is, a basic element used by the character state controller (finite state machine).
/// </summary>
public abstract class CharacterState : MonoBehaviour, IUpdatable
{

    private float stateElapsedTime = 0f;
    public float StateElapsedTime => stateElapsedTime;
    private bool hasEnteredState = false;


    [Header("Combat")]
    public bool canFightInState;


    public float GetElapsedTime()
    {
        return stateElapsedTime;
    }

    protected CharacterMoverAndRotator characterMoverAndRotator;

    public enum InterpolationType
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }
    protected float ApplyInterpolation(float t, InterpolationType _interpolationType)
    {
        switch (_interpolationType)
        {
            case InterpolationType.Linear:
                return t;

            case InterpolationType.EaseIn:
                return t * t;

            case InterpolationType.EaseOut:
                return 1 - Mathf.Pow(1 - t, 2);

            case InterpolationType.EaseInOut:
                return Mathf.SmoothStep(0, 1, t);

            default:
                return t;
        }
    }


    protected CharacterLocalEventManager playerLocalEventManager;

    //_____________________________________________________________________________________________________________________________




    [SerializeField]
    bool overrideAnimatorController = true;

    [Condition("overrideAnimatorController", ConditionAttribute.ConditionType.IsTrue)]
    [SerializeField]
    RuntimeAnimatorController runtimeAnimatorController = null;

    /// <summary>
    /// Gets the hash value (Animator) associated with this state, based on its name.
    /// </summary>
    public int StateNameHash { get; private set; }

    /// <summary>
    /// Gets the state runtime animator controller.
    /// </summary>
    public RuntimeAnimatorController RuntimeAnimatorController => runtimeAnimatorController;


    public bool OverrideAnimatorController => overrideAnimatorController;

    /// <summary>
    /// Gets the CharacterActor component of the gameObject.
    /// </summary>
    public CharacterActor CharacterActor { get; private set; }

    /// <summary>
    /// Gets the CharacterBrain component of the gameObject.
    /// </summary>
    // public CharacterBrain CharacterBrain{ get; private set; }
    CharacterBrain CharacterBrain = null;

    /// <summary>
    /// Gets the current brain actions CharacterBrain component of the gameObject.
    /// </summary>
    public CharacterActions CharacterActions
    {
        get
        {
            return CharacterBrain == null ? DefaultCharacterActions : CharacterBrain.CharacterActions;
        }
    }

    /// <summary>
    /// Gets the CharacterStateController component of the gameObject.
    /// </summary>
    public CharacterStateController CharacterStateController { get; private set; }

    CharacterActions DefaultCharacterActions = CharacterActions.CreateDefaultActions();

    protected virtual void Awake()
    {
        CharacterActor = this.GetComponentInBranch<CharacterActor>();
        CharacterStateController = this.GetComponentInBranch<CharacterActor, CharacterStateController>();
        CharacterBrain = this.GetComponentInBranch<CharacterActor, CharacterBrain>();

        characterMoverAndRotator = this.GetComponentInBranch<CharacterMoverAndRotator>();

        if(CharacterActor.IsPlayer)
            playerLocalEventManager = this.GetComponentInBranch<CharacterActor, CharacterLocalEventManager>();
    }


    protected virtual void Start()
    {
        StateNameHash = Animator.StringToHash(this.GetType().Name);
    }


    /// <summary>
    /// This method runs once when the state has entered the state machine.
    /// </summary>
    public virtual void EnterBehaviour(float dt, CharacterState fromState)
    {
        hasEnteredState = true;
    }

    /// <summary>
    /// This methods runs before the main Update method.
    /// </summary>
    public virtual void PreUpdateBehaviour(float dt)
    {
    }

    /// <summary>
    /// This method runs frame by frame, and should be implemented by the derived state class.
    /// </summary>
    public abstract void UpdateBehaviour(float dt);


    protected virtual void Update()
    {
        if (hasEnteredState)
        {
            stateElapsedTime += Time.deltaTime;
        }
    }


    /// <summary>
    /// This methods runs after the main Update method.
    /// </summary>
    public virtual void PostUpdateBehaviour(float dt)
    {
    }

    /// <summary>
    /// This methods runs just before the character physics simulation.
    /// </summary>
    public virtual void PreCharacterSimulation(float dt)
    {
    }

    /// <summary>
    /// This methods runs after the character physics simulation.
    /// </summary>
    public virtual void PostCharacterSimulation(float dt)
    {
    }

    /// <summary>
    /// This method runs once when the state has exited the state machine.
    /// </summary>
    public virtual void ExitBehaviour(float dt, CharacterState toState)
    {
        hasEnteredState = false;
        stateElapsedTime = 0f;
    }

    /// <summary>
    /// Checks if the required conditions to exit this state are true. If so it returns the desired state (null otherwise). After this the state machine will
    /// proceed to evaluate the "enter transition" condition on the target state.
    /// </summary>
    public virtual void CheckExitTransition()
    {
    }

    /// <summary>
    /// Checks if the required conditions to enter this state are true. If so the state machine will automatically change the current state to the desired one.
    /// </summary>  
    public virtual bool CheckEnterTransition(CharacterState fromState)
    {
        return true;
    }

    /// <summary>
    /// This methods runs after getting all the ik positions, rotations and their respective weights. Use it to modify the ik data of the humanoid rig.
    /// </summary>
    public virtual void UpdateIK(int layerIndex)
    {
    }

    public virtual string GetInfo()
    {
        return "";
    }

    /// <summary>
    /// Checks if the Animator component associated with the character is "valid" or not.
    /// </summary>
    /// <returns>True if the Animator is valid, false otherwise.</returns>
    public bool IsAnimatorValid() => CharacterActor.IsAnimatorValid();

}
