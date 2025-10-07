using UnityEngine;

public class OverTrigger : MonoBehaviour
{
    private ColumnController parentColumn;

    private void Start()
    {
        // Находим родительский ColumnController
        parentColumn = transform.parent.GetComponent<ColumnController>();
        if (parentColumn == null)
        {
            Debug.LogError("OverTrigger: Parent ColumnController not found!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameObject.activeSelf) return;

        var circle = other.GetComponent<Circle>();
        if (circle != null && !circle.IsPlaced)
        {
            Debug.Log($"Game Over: Circle hit over trigger of column {parentColumn.columnIndex} at {other.transform.position}!");
            GameplayManager.Instance.GameOver();
        }
    }
}
