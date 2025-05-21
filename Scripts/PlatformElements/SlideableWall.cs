using UnityEngine;
using ShadowFort.Utilities;
using System;



public class SlideableWall : MonoBehaviour
{

    public static event Action<SlideableWall> OnWallEnabled;
    public static event Action<SlideableWall> OnWallDisabled;

    private void OnEnable()
    {
        OnWallEnabled?.Invoke(this);
    }

    private void OnDisable()
    {
        OnWallDisabled?.Invoke(this);
    }


}