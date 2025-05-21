using UnityEngine;
using Animancer;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "StateAnimations_Dash", menuName = "NocturneKeepInteractive/AnimationSystem/StateAnimations_Dash")]
public class StateAnimations_Dash : ScriptableObject
{
    public ClipTransition DashStart;
    public ClipTransition DashLoop;
    public ClipTransition DashEnd;
}
