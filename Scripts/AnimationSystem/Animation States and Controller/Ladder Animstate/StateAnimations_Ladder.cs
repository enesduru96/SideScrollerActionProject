using Animancer;
using UnityEngine;

[CreateAssetMenu(fileName = "StateAnimations_Ladder", menuName = "NocturneKeepInteractive/AnimationSystem/StateAnimations_Ladder")]
public class StateAnimations_Ladder : ScriptableObject
{
    [Header("Entries")]
    public ClipTransition LadderTopEntry;
    public ClipTransition LadderBottomEntry;

    [Header("Idle")]
    public ClipTransition LadderIdle;

    [Header("Exits")]
    public ClipTransition LadderTopExit;
    public ClipTransition LadderBottomExit;

    [Header("Movements")]
    public ClipTransition LadderUp;
    public ClipTransition LadderDown;

    [Header("Airborne Catch")]
    public ClipTransition LadderCatch;
}
