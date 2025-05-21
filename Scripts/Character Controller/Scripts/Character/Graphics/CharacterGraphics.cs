using System.Collections;
using System.Collections.Generic;
using ShadowFort.Utilities;
using UnityEngine;


/// <summary>
/// The root abstract class for all the graphics-based components.
/// </summary>
public abstract class CharacterGraphics : MonoBehaviour
{
    protected CharacterActor CharacterActor { get; private set; }
    protected CharacterBody CharacterBody => CharacterActor.CharacterBody;

    protected virtual void OnValidate()
    {
        CharacterActor = this.GetComponentInBranch<CharacterActor>();

        if (CharacterActor == null)
        {
            Debug.Log("Warning: No CharacterActor component detected in this hierarchy.");
        }
    }

    protected virtual void Awake()
    {
        CharacterActor = this.GetComponentInBranch<CharacterActor>();

        if (CharacterActor == null)
        {
            Debug.Log("Warning: No CharacterActor component detected in this hierarchy.");
            enabled = false;
            return;
        }

        //RootController = CharacterActor.GetComponentInChildren<CharacterGraphicsRootController>();
    }


}
