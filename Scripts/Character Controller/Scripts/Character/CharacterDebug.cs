using UnityEngine;
using UnityEngine.UI;
using ShadowFort.Utilities;
using System;
using TMPro;
using System.Text;


/// <summary>
/// This class is used for debug purposes, mainly to print information on screen about the collision flags, certain values and/or triggering events.
/// </summary>
[DefaultExecutionOrder(ExecutionOrder.CharacterActorOrder)]
public class CharacterDebug : MonoBehaviour
{

    [SerializeField]
    CharacterActor characterActor = null;

    [Header("Character Info")]
    [SerializeField]
    TMP_Text text = null;

    [SerializeField]
    TMP_Text wallSideText = null;

    [SerializeField]
    TMP_Text wallHitLocalVelocityText = null;

    [SerializeField]
    TMP_Text wallHitRelativeVelocityText = null;

    [SerializeField]
    TMP_Text characterInputText = null;

    [SerializeField]
    private CharacterState _state;


    [Header("Events")]

    [SerializeField]
    bool printEvents = false;

    [Header("Stability")]

    [SerializeField]
    Renderer stabilityIndicator;

    [Condition("stabilityIndicator", ConditionAttribute.ConditionType.IsNotNull, ConditionAttribute.VisibilityType.NotEditable)]
    [SerializeField]
    Color stableColor = new Color(0f, 1f, 0f, 0.5f);

    [Condition("stabilityIndicator", ConditionAttribute.ConditionType.IsNotNull, ConditionAttribute.VisibilityType.NotEditable)]
    [SerializeField]
    Color unstableColor = new Color(1f, 0f, 0f, 0.5f);

    int colorID = Shader.PropertyToID("_Color");
    float time = 0f;


    private readonly StringBuilder sb = new StringBuilder(128);
    private string lastCharacterInfo = "";
    private Vector2 previousMovementValue = Vector2.zero;

    void UpdateCharacterInfoText()
    {
        if (text == null) return;

        if (time > 0.2f)
        {
            Vector2 movementValue = _state.CharacterActions.movement.value;

            if (movementValue != previousMovementValue)
            {
                previousMovementValue = movementValue;
                sb.Clear();
                sb.Append("Movement: ").Append(movementValue.x.ToString("F2"))
                  .Append(", ").Append(movementValue.y.ToString("F2"));

                characterInputText.SetText(sb.ToString());
            }

            string newCharacterInfo = characterActor.GetCharacterInfo();
            if (!newCharacterInfo.Equals(lastCharacterInfo))
            {
                lastCharacterInfo = newCharacterInfo;
                text.SetText(newCharacterInfo);
            }

            time = 0f;
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    void OnWallHit(Contact contact)
    {
        wallHitLocalVelocityText.SetText(characterActor.LocalInputVelocity.ToString("F2"));
        wallHitRelativeVelocityText.SetText(contact.normal.ToString("F2"));
        wallSideText.SetText(contact.normal.x > 0 ? "left" : "right");
    }
    void OnGroundedStateEnter(Vector3 localVelocity) => Debug.Log("OnEnterGroundedState, localVelocity : " + localVelocity.ToString("F3"));
    void OnGroundedStateExit() => Debug.Log("OnExitGroundedState");
    void OnStableStateEnter(Vector3 localVelocity) => Debug.Log("OnStableStateEnter, localVelocity : " + localVelocity.ToString("F3"));
    void OnStableStateExit() => Debug.Log("OnStableStateExit");
    void OnHeadHit(Contact contact) => Debug.Log("OnHeadHit");
    void OnTeleportation(Vector3 position, Quaternion rotation) => Debug.Log("OnTeleportation, position : " + position.ToString("F3") + " and rotation : " + rotation.ToString("F3"));

    #region Messages
    void FixedUpdate()
    {
        if (characterActor == null)
        {
            enabled = false;
            return;
        }

        UpdateCharacterInfoText();
    }

    void Update()
    {
        if (stabilityIndicator != null)
            stabilityIndicator.sharedMaterial.SetColor(colorID, characterActor.IsStable ? stableColor : unstableColor);
    }

    void OnEnable()
    {
        if (!printEvents)
            return;

        characterActor.OnHeadHit += OnHeadHit;
        characterActor.OnWallHit += OnWallHit;
        characterActor.OnGroundedStateEnter += OnGroundedStateEnter;
        characterActor.OnGroundedStateExit += OnGroundedStateExit;
        characterActor.OnStableStateEnter += OnStableStateEnter;
        characterActor.OnStableStateExit += OnStableStateExit;
        characterActor.OnTeleport += OnTeleportation;
    }

    void OnDisable()
    {
        if (!printEvents)
            return;

        characterActor.OnHeadHit -= OnHeadHit;
        characterActor.OnWallHit -= OnWallHit;
        characterActor.OnGroundedStateEnter -= OnGroundedStateEnter;
        characterActor.OnGroundedStateExit -= OnGroundedStateExit;
        characterActor.OnStableStateEnter += OnStableStateEnter;
        characterActor.OnStableStateExit += OnStableStateExit;
        characterActor.OnTeleport -= OnTeleportation;
    }
    #endregion
}
