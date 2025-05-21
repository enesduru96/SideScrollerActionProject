using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;


#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public struct Vector2Action
{
    public Vector2 value;

    private Vector2 previousValue;

    //public event Action<Vector2> OnValueChanged;

    //public event Action OnReleased;

    public void Reset()
    {
        value = Vector2.zero;
        //previousValue = Vector2.zero;

        RightReleased = false;
        LeftReleased = false;
        UpReleased = false;
        DownReleased = false;

    }

    public void Update(float dt)
    {
        RightReleased = previousValue.x > 0 && value.x <= 0;
        LeftReleased = previousValue.x < 0 && value.x >= 0;
        UpReleased = previousValue.y > 0 && value.y <= 0;
        DownReleased = previousValue.y < 0 && value.y >= 0;

        previousValue = value;
    }


    public bool Detected => value != Vector2.zero;

    public bool Right => value.x > 0;
    public bool Left => value.x < 0;
    public bool Up => value.y > 0;
    public bool Down => value.y < 0;


    public bool RightReleased { get; private set; }
    public bool LeftReleased { get; private set; }
    public bool UpReleased { get; private set; }
    public bool DownReleased { get; private set; }
}





// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
// EDITOR ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

#if UNITY_EDITOR


[CustomPropertyDrawer(typeof(Vector2Action))]
public class Vector2ActionEditor : PropertyDrawer
{


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty value = property.FindPropertyRelative("value");

        Rect fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;
        fieldRect.width = 100;

        EditorGUI.LabelField(fieldRect, label);

        fieldRect.x += 110;

        EditorGUI.PropertyField(fieldRect, value, GUIContent.none);


        EditorGUI.EndProperty();
    }



}


#endif
