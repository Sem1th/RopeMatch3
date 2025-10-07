using UnityEngine;
using System.Collections.Generic;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance { get; private set; }
    public GameObject particlePrefab;
    public int initialSize = 5;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < initialSize; i++)
        {
            var go = Instantiate(particlePrefab);
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }

    public void PlayAt(Vector3 pos)
    {
        GameObject p;
        if (pool.Count > 0) p = pool.Dequeue();
        else p = Instantiate(particlePrefab);

        p.transform.position = pos;
        p.SetActive(true);

        var ps = p.GetComponent<ParticleSystem>();
        ps.Play();
        // Возврат в пул после длительности
        float dur = ps.main.duration + ps.main.startLifetime.constantMax;
        StartCoroutine(DeactivateAfter(p, dur));
    }

    System.Collections.IEnumerator DeactivateAfter(GameObject go, float t)
    {
        yield return new WaitForSeconds(t);
        go.SetActive(false);
        pool.Enqueue(go);
    }
}

