using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;


//TODO: Currently I'm using only one <T> which is DamageSource, I need to modify the class to pass any Type from other poolable object scripts like vfx, damagesource, enemy etc.

public class CharacterObjectPooler : MonoBehaviour
{
    private Dictionary<string, ObjectPool<DamageSource>> damageSourcePools = new Dictionary<string, ObjectPool<DamageSource>>();

    [SerializeField] private int damageSourceMaxSize = 10;

    private ObjectPool<DamageSource> GetOrCreatePool(GameObject prefab, string attackName, CharacterActor ownerActor)
    {
        if (!damageSourcePools.TryGetValue(attackName, out var pool))
        {

            pool = new ObjectPool<DamageSource>(
                createFunc: () => InstantiateNewDamageSource(prefab, ownerActor),
                actionOnGet: (obj) => obj.gameObject.SetActive(true),
                actionOnRelease: (obj) =>
                {
                    obj.gameObject.SetActive(false);
                    obj.transform.SetParent(null);
                },
                actionOnDestroy: (obj) => Destroy(obj.gameObject),
                collectionCheck: false,
                defaultCapacity: damageSourceMaxSize,
                maxSize: damageSourceMaxSize
            );

            damageSourcePools[attackName] = pool;
        }
        return pool;
    }

    private DamageSource InstantiateNewDamageSource(GameObject prefab, CharacterActor ownerActor)
    {
        GameObject damageObject = Instantiate(prefab);
        DamageSource damageSource = damageObject.GetComponent<DamageSource>();
        damageSource.OwnerActor = ownerActor;
        return damageSource;
    }

    public DamageSource GetDamageSource(GameObject prefab, string attackName, Vector3 position, Quaternion rotation, CharacterActor ownerActor)
    {
        if (prefab == null)
        {
            Debug.LogError("[POOL] Define DamageSource prefab!");
            return null;
        }

        var pool = GetOrCreatePool(prefab, attackName, ownerActor);

        DamageSource obj = pool.Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        return obj;
    }

    public void ReturnDamageSource(string attackName, DamageSource obj)
    {
        if (string.IsNullOrEmpty(attackName) || obj == null)
        {
            //Debug.LogError("[POOL] ReturnDamageSource için geçersiz parametre!");
            return;
        }

        if (!damageSourcePools.ContainsKey(attackName))
        {
            //Debug.LogError($"[POOL] {attackName} için bir pool bulunamadý!");
            return;
        }

        damageSourcePools[attackName].Release(obj);
    }

    public void PreloadDamageSource(GameObject prefab, string attackName, int count, CharacterActor ownerActor)
    {
        var pool = GetOrCreatePool(prefab, attackName, ownerActor);

        //Debug.Log($"[POOL] {attackName} için {count} adet preload ediliyor.");

        for (int i = 0; i < count; i++)
        {
            DamageSource obj = pool.Get();
            obj.gameObject.SetActive(false);
            pool.Release(obj);
        }
    }
}