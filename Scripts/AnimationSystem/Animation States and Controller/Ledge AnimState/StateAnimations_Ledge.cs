using UnityEngine;
using Animancer;

[CreateAssetMenu(fileName = "StateAnimations_Ledge", menuName = "NocturneKeepInteractive/AnimationSystem/StateAnimations_Ledge")]
public class StateAnimations_Ledge : ScriptableObject
{
    [Header("Braced Ledge")]
    [Space(20)]

    [Header("Entry to Left Side")]
    public ClipTransition EnterLedgeBracedAboveToLeft;
    public ClipTransition EnterLedgeBracedForwardToLeft;
    public ClipTransition EnterLedgeBracedForwardToughToLeft;

    [Header("Entry to Right Side")]
    public ClipTransition EnterLedgeBracedAboveToRight;
    public ClipTransition EnterLedgeBracedForwardToRight;
    public ClipTransition EnterLedgeBracedForwardToughToRight;

    [Header("Main")]
    public ClipTransition IdleBracedToLeft;
    public ClipTransition IdleBracedToRight;
    public ClipTransition ClimbUpBraced;
    public ClipTransition JumpDownBraced;
    public ClipTransition JumpUpToLedgeBraced;

    [Space(20)]
    [Header("Look Back to Right Side")]
    public ClipTransition LookBackStartToRight;
    public ClipTransition LookBackLoopToRight;
    public ClipTransition LookBackEndToRight;
    public ClipTransition JumpBackStartToRight;
    [Header("Look Back to Left Side")]
    public ClipTransition LookBackStartToLeft;
    public ClipTransition LookBackLoopToLeft;
    public ClipTransition LookBackEndToLeft;
    public ClipTransition JumpBackStartToLeft;

    [Space(20)]
    [Header("Look Down to Left Side")]
    public ClipTransition LookDownBracedStartToLeft;
    public ClipTransition LookDownBracedLoopToLeft;
    public ClipTransition LookDownBracedEndToLeft;
    [Header("Look Down to Right Side")]
    public ClipTransition LookDownBracedStartToRight;
    public ClipTransition LookDownBracedLoopToRight;
    public ClipTransition LookDownBracedEndToRight;

    [Space(20)]
    [Header("Look Up To Left Side")]
    public ClipTransition LookUpBracedStartToLeft;
    public ClipTransition LookUpBracedLoopToLeft;
    public ClipTransition LookUpBracedEndToLeft;
    [Header("Look Up To Right Side")]
    public ClipTransition LookUpBracedStartToRight;
    public ClipTransition LookUpBracedLoopToRight;
    public ClipTransition LookUpBracedEndToRight;


    [Space(50)]

    [Header("Free Ledge")]
    public ClipTransition EnterLedgeFreeAbove;
    public ClipTransition EnterLedgeFreeForward;
    public ClipTransition IdleFree;
    public ClipTransition ClimbUpFree;
    public ClipTransition LookDownFree;
    public ClipTransition LookUpFree;
}
