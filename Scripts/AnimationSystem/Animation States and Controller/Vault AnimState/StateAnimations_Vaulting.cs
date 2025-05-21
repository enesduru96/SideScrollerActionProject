using UnityEngine;
using Animancer;

[CreateAssetMenu(fileName = "StateAnimations_Vaulting", menuName = "NocturneKeepInteractive/AnimationSystem/StateAnimations_Vaulting")]
public class StateAnimations_Vaulting : ScriptableObject
{
    public ClipTransition VaultSlow;
    public ClipTransition VaultFast;
}
