using UnityEngine;

public class GameTip : MonoBehaviour
{
    private bool hasTapped = false;
    [SerializeField] private float scaleSpeed = 1f; // Скорость анимации
    [SerializeField] private float scaleRange = 0.1f; // Амплитуда изменения масштаба
    private Pendulum pendulum;

    void Start()
    {
        pendulum = FindObjectOfType<Pendulum>(); // Находим маятник в сцене
        if (pendulum == null)
        {
            Debug.LogError("GameTip: Pendulum not found in scene!");
        }
    }

    void Update()
    {
        if (!hasTapped)
        {
            // Анимация увеличения/уменьшения
            float scale = 1f + Mathf.PingPong(Time.time * scaleSpeed, scaleRange);
            transform.localScale = new Vector3(scale, scale, 1f);

            // Проверка первого касания или клика
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                hasTapped = true;
                if (pendulum != null)
                {
                    pendulum.DropCircle(); // Падаем шар при первом тапе
                }
                Destroy(gameObject); // Удаляем подсказку
            }
        }
    }
}
