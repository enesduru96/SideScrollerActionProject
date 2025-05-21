﻿using ShadowFort.Utilities;
using UnityEngine;


public class CharacterCollisions3D : CharacterCollisions
{
    protected override void Awake()
    {
        base.Awake();

        PhysicsComponent = gameObject.AddComponent<PhysicsComponent3D>();
    }

    public override float ContactOffset => Physics.defaultContactOffset;
    public override float CollisionRadius => CharacterActor.BodySize.x / 2f;
}
