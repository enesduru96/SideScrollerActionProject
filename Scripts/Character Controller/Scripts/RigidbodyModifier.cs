﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadowFort.Utilities;


public class RigidbodyModifier : MonoBehaviour
{
    [SerializeField]
    AddMode mode = AddMode.AddForce;

    [SerializeField]
    Vector3 localAddVector = Vector3.zero;

    [Min(0.01f)]
    [SerializeField]
    float dragMultiplier = 1f;

    [Min(0.01f)]
    [SerializeField]
    float massMultiplier = 1f;

    enum AddMode
    {
        AddForce,
        Accelerate,
        AddVelocity
    }

    Vector3 worldAddVector = default(Vector3);

    Dictionary<Transform, Rigidbody> rigidbodies = new Dictionary<Transform, Rigidbody>();

    void OnTriggerEnter(Collider otherCollider)
    {

        Rigidbody rigidbody = rigidbodies.GetOrRegisterValue<Transform, Rigidbody>(otherCollider.transform);

        if (rigidbody == null)
            return;

        rigidbody.mass *= massMultiplier;

        rigidbody.linearDamping *= dragMultiplier;


    }

    void OnTriggerExit(Collider otherCollider)
    {
        Rigidbody rigidbody;
        rigidbodies.TryGetValue(otherCollider.transform, out rigidbody);

        if (rigidbody == null)
            return;

        rigidbody.mass /= massMultiplier;

        rigidbody.linearDamping /= dragMultiplier;

        rigidbodies.Remove(otherCollider.transform);

    }

    void Start()
    {
        worldAddVector = transform.TransformDirection(localAddVector);
    }

    void FixedUpdate()
    {
        foreach (var rigidbody in rigidbodies.Values)
        {
            switch (mode)
            {
                case AddMode.AddForce:
                    rigidbody.AddForce(worldAddVector);
                    break;
                case AddMode.Accelerate:
                    rigidbody.linearVelocity += worldAddVector * Time.deltaTime;
                    break;
                case AddMode.AddVelocity:
                    rigidbody.linearVelocity += worldAddVector;

                    break;
            }

        }
    }
}