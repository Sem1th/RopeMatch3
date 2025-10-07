using UnityEngine;

public class Pendulum : MonoBehaviour
{
    [Header("Pendulum")]
    public float amplitude = 30f; // Угол вращения маятника
    public float period = 2f; // Период вращения/скорость

    [Header("Circle spawn")]
    public Transform attachPoint; // Точка спавна на маятнике

    private float timeCounter = 0f;
    private GameObject currentCircle;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        SpawnAttachedCircle();
    }

    void Update()
    {
        timeCounter += Time.deltaTime;
        float angle = amplitude * Mathf.Cos(2f * Mathf.PI * timeCounter / period);
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    void SpawnAttachedCircle()
    {
        if (CircleFactory.Instance != null)
        {
            currentCircle = CircleFactory.Instance.CreateRandomCircle(attachPoint.position, attachPoint);
            if (currentCircle != null)
            {
                var circleComp = currentCircle.GetComponent<Circle>();
                if (circleComp != null) circleComp.AttachToPendulum();
            }
        }
        else
        {
            Debug.LogError("Pendulum: CircleFactory not found!");
        }
    }

    // Публичный метод, вызывается GameplayManager / InputHandler
    public void DropCircle()
    {
        if (currentCircle == null) return;

        // Определяем текущую x позицию маятника
        float pendulumX = attachPoint.position.x;

        // Находим ближайшую колонку
        int closestColumn = FindClosestColumn(pendulumX);

        // Получаем позицию центра колонки
        var targetColumn = BoardController.Instance.GetColumnController(closestColumn);
        if (targetColumn != null)
        {
            Vector3 dropPosition = targetColumn.transform.position;
            dropPosition.y = attachPoint.position.y; // Сохраняем высоту маятника для старта падения

            currentCircle.transform.SetParent(null);
            var circle = currentCircle.GetComponent<Circle>();
            if (circle != null) circle.Release(dropPosition);

            currentCircle = null;

            Invoke(nameof(SpawnAttachedCircle), 0.45f);
        }
        
        // Проигрываем звук падения
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayDropSound(attachPoint.position);
        }
    }

    private int FindClosestColumn(float x)
    {
        float minDist = float.MaxValue;
        int closest = 0;

        for (int i = 0; i < 3; i++)
        {
            var column = BoardController.Instance.GetColumnController(i);
            if (column != null)
            {
                float dist = Mathf.Abs(x - column.transform.position.x);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = i;
                }
            }
        }

        return closest;
    }
}


