using System.Collections;
using UnityEngine;

public class MeleeDamageSource : DamageSource
{

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void InitializeDamageObject()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Update()
    {
        base.Update();

        if (OwnerActor == null || !gameObject.activeInHierarchy)
        {
            return;
        }

        transform.position = new Vector3(OwnerActor.transform.position.x + (OwnerActor.Forward.x * SpawnPositionOffset.x), OwnerActor.transform.position.y + SpawnPositionOffset.y, OwnerActor.transform.position.z);
        
    }
}
