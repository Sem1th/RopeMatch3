using UnityEngine;

public class CircleFactory : MonoBehaviour
{
    public static CircleFactory Instance { get; private set; }

    [Tooltip("Assign 3 circle prefabs (Red, Purple, Green)")]
    public GameObject[] circlePrefabs; // Size = 3

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Создать круг и вернуть GameObject
    public GameObject CreateRandomCircle(Vector3 position, Transform parent = null)
    {
        if (circlePrefabs == null || circlePrefabs.Length == 0)
        {
            Debug.LogError("CircleFactory: no prefabs assigned!");
            return null;
        }

        int i = Random.Range(0, circlePrefabs.Length);
        GameObject go = Instantiate(circlePrefabs[i], position, Quaternion.identity);
        if (parent != null) go.transform.SetParent(parent);

        // По умолчанию при создании будет кинематиком (для подвеса)
        var rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Убедимся, что компонент Circle правильно настроен (если prefab содержит нужный enum — ок)
        var circle = go.GetComponent<Circle>();
        if (circle == null) Debug.LogWarning("CircleFactory: prefab has no Circle component");

        return go;
    }
}

