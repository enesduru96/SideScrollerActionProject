using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        string key = prefab.name;

        if (!pools.ContainsKey(key))
        {
            pools[key] = new Queue<GameObject>();
        }

        if (pools[key].Count > 0)
        {
            var obj = pools[key].Dequeue();
            obj.SetActive(true);
            return obj;
        }

        // Havuzda yoksa yeni oluþtur
        var newObj = Instantiate(prefab);
        newObj.name = key;
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        string key = obj.name;

        if (!pools.ContainsKey(key))
        {
            pools[key] = new Queue<GameObject>();
        }

        obj.SetActive(false);
        pools[key].Enqueue(obj);
    }
}