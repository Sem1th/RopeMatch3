using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundSprite; // Фон для движения
    [SerializeField] private float moveSpeed = 0.12f; // Скорость движения
    [SerializeField] private float moveDistance = 1.0f; // Амплитуда движения

    private Vector3 startPosition;
    private float timeCounter = 0f;

    private void Awake()
    {
        if (backgroundSprite == null)
        {
            backgroundSprite = GetComponent<SpriteRenderer>();
            if (backgroundSprite == null)
            {
                Debug.LogError("BackgroundMover: SpriteRenderer not found on this object!");
            }
        }
        startPosition = backgroundSprite.transform.position;
    }

    private void Update()
    {
        if (backgroundSprite != null)
        {
            // Легкое движение влево-вправо с синусоидой
            timeCounter += Time.deltaTime * moveSpeed;
            float offset = Mathf.Sin(timeCounter) * moveDistance;
            backgroundSprite.transform.position = startPosition + new Vector3(offset, 0f, 0f);
        }
    }
}
