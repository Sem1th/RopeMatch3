using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColumnController : MonoBehaviour
{
    public int columnIndex;
    public Transform[] slots; // Слоты, нижний -> верхний (3)
    public GameObject overTrigger; // Ссылка на триггер

    public List<GameObject> stack = new List<GameObject>();
    public bool IsFull => stack.Count >= slots.Length;

    private void Start()
    {
        // Проверка корректности slots
        if (slots == null || slots.Length != 3)
        {
            Debug.LogError($"Column {columnIndex} has invalid slots array. Expected 3 slots, found {slots?.Length ?? 0}.");
        }

        // Проверка и настройка триггера
        if (overTrigger == null)
        {
            Debug.LogError($"Column {columnIndex}: OverTrigger not assigned in inspector!");
        }
        else
        {
            overTrigger.SetActive(false); // Изначально выключен
            BoxCollider2D triggerCol = overTrigger.GetComponent<BoxCollider2D>();
            if (triggerCol == null || !triggerCol.isTrigger)
            {
                Debug.LogError($"Column {columnIndex}: OverTrigger must have BoxCollider2D with Is Trigger!");
            }
        }
    }

    private void Update()
    {
        // Активируем триггер, если колонка заполнена
        if (overTrigger != null && stack.Count == 3 && !overTrigger.activeSelf)
        {
            overTrigger.SetActive(true);
            Debug.Log($"Column {columnIndex}: OverTrigger activated!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var circle = other.GetComponent<Circle>();
        if (circle == null || circle.IsPlaced) return;

        TryAddCircle(other.gameObject);
    }

    private void TryAddCircle(GameObject circleObj)
    {
        if (IsFull || stack.Contains(circleObj)) return;

        circleObj.transform.SetParent(transform);
        stack.Add(circleObj);

        var circle = circleObj.GetComponent<Circle>();
        circle.AttachToPendulum();

        // Защита от выхода за границы slots
        int slotIndex = Mathf.Min(stack.Count - 1, slots.Length - 1);
        Vector3 targetPos = slots[slotIndex].position;
        StartCoroutine(SmoothSnapToSlot(circle, targetPos));

        Debug.Log($"Column {columnIndex}: Added circle at slot {slotIndex} with color: {circle.color}, position: {circle.transform.position}");
    }

    private IEnumerator SmoothSnapToSlot(Circle circle, Vector3 targetPos)
    {
        if (circle == null) yield break;

        float t = 0f;
        Vector3 start = circle.transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            circle.transform.position = Vector3.Lerp(start, targetPos, t);
            yield return null;
        }

        circle.transform.position = targetPos;
        circle.PlaceToSlot();

        // Лог финальной позиции
        Debug.Log($"Column {columnIndex}: Circle snapped to slot {stack.Count - 1} with color: {circle.color}, final position: {targetPos}");

        // Обновляем грид сразу после анимации
        BoardController.Instance.UpdateColumnFromStack(columnIndex, stack);
        BoardController.Instance.UpdateCell(columnIndex, stack.Count - 1, circle);
    }

    public void RemoveCircle(Circle circle)
    {
        int index = stack.FindIndex(go => go != null && go.GetComponent<Circle>() == circle);
        if (index < 0) return;

        stack.RemoveAt(index);
        Debug.Log($"Column {columnIndex} stack after remove: {string.Join(", ", stack.ConvertAll(go => go != null ? go.GetComponent<Circle>()?.color.ToString() : "null"))}");

        // Деактивируем триггер, если колонка стала менее заполненной
        if (overTrigger != null && stack.Count < 3)
        {
            overTrigger.SetActive(false);
            Debug.Log($"Column {columnIndex}: OverTrigger deactivated!");
        }
    }

    public IEnumerator CollapseColumn()
    {
        float duration = 0.3f;
        List<Vector3> startPositions = new List<Vector3>();
        for (int i = 0; i < stack.Count; i++)
        {
            if (stack[i] != null)
                startPositions.Add(stack[i].transform.position);
            else
                startPositions.Add(slots[i].position); // Запасная позиция
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            for (int i = 0; i < stack.Count; i++)
            {
                if (stack[i] == null) continue;
                Vector3 target = slots[i].position;
                stack[i].transform.position = Vector3.Lerp(startPositions[i], target, t);
            }
            yield return null;
        }

        for (int i = 0; i < stack.Count; i++)
        {
            if (stack[i] == null) continue;
            stack[i].transform.position = slots[i].position;
            stack[i].GetComponent<Circle>()?.PlaceToSlot();
        }

        // Очищаем stack от null перед обновлением
        stack.RemoveAll(go => go == null);
        BoardController.Instance.UpdateColumnFromStack(columnIndex, stack);
    }

    public void ClearAll()
    {
        foreach (var o in stack)
        {
            if (o != null)
            {
                if (ObjectPool.Instance != null)
                    ObjectPool.Instance.Release("Circle", o);
                else
                    Destroy(o);
            }
        }
        stack.Clear();
        if (overTrigger != null)
        {
            overTrigger.SetActive(false);
            Debug.Log($"Column {columnIndex}: OverTrigger deactivated!");
        }
        BoardController.Instance.UpdateColumnFromStack(columnIndex, stack);
    }
}



