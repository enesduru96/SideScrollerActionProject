#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CharacterActor))]
public class CharacterActorEditor : Editor
{
    CharacterActor characterActor = null;

    void OnEnable()
    {
        characterActor = target as CharacterActor;
    }

    void OnDisable()
    {
        Tools.hidden = false;
    }

    public override void OnInspectorGUI()
    {

        if (Physics.simulationMode != SimulationMode.FixedUpdate)
            EditorGUILayout.HelpBox("(Physics) Only FixedUpdate simulation mode is supported!", MessageType.Error);

        EditorGUIUtility.labelWidth += 75;
        DrawDefaultInspector();
        EditorGUIUtility.labelWidth -= 75;
    }

    void OnSceneGUI()
    {
        if (!Application.isPlaying)
            return;

        if (Tools.current == Tool.Move)
        {
            Tools.hidden = true;

            EditorGUI.BeginChangeCheck();

            Vector3 targetPosition = Handles.PositionHandle(characterActor.transform.position, characterActor.transform.rotation);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(characterActor, "Change character position");
                characterActor.Teleport(targetPosition);
            }
        }
        else if (Tools.current == Tool.Rotate)
        {
            Tools.hidden = true;

            EditorGUI.BeginChangeCheck();

            Quaternion targetRotation = Handles.RotationHandle(characterActor.transform.rotation, characterActor.transform.position);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(characterActor, "Change character rotation");
                characterActor.Teleport(characterActor.Position, targetRotation);
            }
        }
        else
        {
            Tools.hidden = false;
        }
    }
}
#endif
