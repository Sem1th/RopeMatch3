using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [System.Serializable]
    public class PoolEntry
    {
        public string key;
        public GameObject prefab;
        public int initialSize = 5;
    }

    public PoolEntry[] pools;

    private Dictionary<string, Queue<GameObject>> poolMap = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        foreach (var entry in pools)
        {
            var q = new Queue<GameObject>();
            for (int i = 0; i < entry.initialSize; i++)
            {
                var go = Instantiate(entry.prefab, transform);
                go.SetActive(false);
                q.Enqueue(go);
            }
            poolMap[entry.key] = q;
        }
    }

    public GameObject Get(string key)
    {
        if (!poolMap.ContainsKey(key))
        {
            Debug.LogWarning($"Pool with key '{key}' not found.");
            return null;
        }

        var q = poolMap[key];
        GameObject go;
        if (q.Count > 0) go = q.Dequeue();
        else
        {
            // Поиск префаба
            var entry = System.Array.Find(pools, p => p.key == key);
            if (entry == null) return null;
            go = Instantiate(entry.prefab, transform);
        }

        go.SetActive(true);
        return go;
    }

    public void Release(string key, GameObject go)
    {
        if (go == null) return;
        go.SetActive(false);
        if (!poolMap.ContainsKey(key))
        {
            Destroy(go);
            return;
        }
        poolMap[key].Enqueue(go);
    }
}

