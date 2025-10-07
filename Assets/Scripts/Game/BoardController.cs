using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance { get; private set; }

    private Circle[,] grid = new Circle[3, 3];
    public ColumnController[] columns; // Колонки для заполнения

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (columns == null || columns.Length != 3)
        {
            columns = GetComponentsInChildren<ColumnController>();
            if (columns.Length != 3)
                Debug.LogError($"BoardController: Expected 3 columns, found {columns.Length}");
        }

        for (int c = 0; c < 3; c++)
            for (int r = 0; r < 3; r++)
                grid[c, r] = null;
    }

    public void UpdateColumnFromStack(int columnIndex, List<GameObject> stack)
    {
        if (columnIndex < 0 || columnIndex >= 3) return;

        // Очищаем grid для этой колонки
        for (int r = 0; r < 3; r++) grid[columnIndex, r] = null;

        // Отладка состояния stack
        Debug.Log($"Updating column {columnIndex} from stack: {string.Join(", ", stack.ConvertAll(go => go != null ? go.GetComponent<Circle>()?.color.ToString() : "null"))}");

        // Обновляем grid из stack с логами
        for (int i = 0; i < stack.Count && i < 3; i++)
        {
            if (stack[i] != null)
            {
                var circleComp = stack[i].GetComponent<Circle>();
                if (circleComp != null)
                {
                    grid[columnIndex, i] = circleComp;
                    Debug.Log($"Cell ({columnIndex},{i}) updated with color: {circleComp.color}, position: {circleComp.transform.position}");
                }
            }
        }

        // Запускаем HandleMatch как корутину
        StartCoroutine(MatchDetector.HandleMatch());
    }

    public void RebuildGrid()
    {
        for (int c = 0; c < 3; c++)
        {
            // Очищаем stack от null перед обновлением
            var column = columns[c];
            column.stack.RemoveAll(go => go == null);
            UpdateColumnFromStack(c, column.stack);
        }
    }

    public bool IsFull()
    {
        foreach (var col in columns)
            if (!col.IsFull) return false;
        return true;
    }

    public Circle[,] GetGrid() => grid;

    public ColumnController GetColumnController(int index)
    {
        if (columns == null || index < 0 || index >= columns.Length) return null;
        return columns[index];
    }

    public void RemoveCell(int col, int row)
    {
        if (col < 0 || col >= 3 || row < 0 || row >= 3) return;
        grid[col, row] = null;
    }

    public void UpdateCell(int columnIndex, int rowIndex, Circle circle)
    {
        if (columnIndex >= 0 && columnIndex < 3 && rowIndex >= 0 && rowIndex < 3)
            grid[columnIndex, rowIndex] = circle;
    }
}


