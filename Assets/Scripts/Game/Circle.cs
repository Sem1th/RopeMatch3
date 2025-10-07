using UnityEngine;

public enum CircleColor { Red = 0, Green = 1, Purple = 2 }

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Circle : MonoBehaviour
{

    public CircleColor color;
    public bool IsPlaced { get; private set; } = false;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Шар висит на маятнике
    public void AttachToPendulum()
    {
        IsPlaced = false;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (col != null)
            col.enabled = false; // Не сталкивается с колонкой
    }

    // Отпускаем шар с маятника с заданной позицией
    public void Release(Vector3 dropPosition)
    {
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;       // Сбросим остаточную скорость
            rb.angularVelocity = 0f;
            transform.position = dropPosition; // Устанавливаем позицию над колонкой
        }

        if (col != null)
            col.enabled = true;
    }

    // Шар приземлился в слот
    public void PlaceToSlot()
    {
        IsPlaced = true;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static; // Фиксируем на месте
        }

        if (col != null)
            col.enabled = true;

        transform.rotation = Quaternion.identity;
    }

    // Сброс состояния шара (для перезапуска)
    public void ResetCircle()
    {
        IsPlaced = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (col != null)
            col.enabled = true;

        transform.SetParent(null);
        transform.rotation = Quaternion.identity;
    }
}

