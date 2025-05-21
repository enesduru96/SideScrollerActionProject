using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadowFort.Utilities;
using System;


public class VaultObject : MonoBehaviour
{

    [Header("Debug")]
    [SerializeField]
    private bool showGizmos = true;
    private bool showHandTarget = true;

    [Header("Vault Points")]
    [SerializeField] private Transform leftEntryPoint;
    [SerializeField] private Transform rightEntryPoint;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;

    public Transform LeftEntryPoint => leftEntryPoint;
    public Transform RightEntryPoint => rightEntryPoint;
    public Transform LeftHandTarget => leftHandTarget;
    public Transform RightHandTarget => rightHandTarget;


    private void OnEnable()
    {
        if (WorldObjectPooler.Instance != null)
            WorldObjectPooler.Instance.RegisterVault(this);
    }

    private void OnDisable()
    {
        if (WorldObjectPooler.Instance != null)
            WorldObjectPooler.Instance.UnregisterVault(this);
    }

    void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        if (leftEntryPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(leftEntryPoint.position, 0.2f);
            CustomUtilities.DrawArrowGizmo(leftEntryPoint.position, leftEntryPoint.position + leftEntryPoint.transform.forward, Color.blue);
        }

        if (rightEntryPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(rightEntryPoint.position, 0.2f);
            CustomUtilities.DrawArrowGizmo(rightEntryPoint.position, rightEntryPoint.position + rightEntryPoint.transform.forward, Color.red);

        }


        if (!showHandTarget || leftHandTarget == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(leftHandTarget.position, 0.2f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(rightHandTarget.position, 0.2f);

    }
}