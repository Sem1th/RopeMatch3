using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatchDetector
{
    private static readonly Dictionary<CircleColor, int> ScoreMap = new Dictionary<CircleColor, int>()
    {
        { CircleColor.Red, 10 },
        { CircleColor.Green, 20 },
        { CircleColor.Purple, 30 }
    };

    public static IEnumerator HandleMatch()
    {
        var grid = BoardController.Instance.GetGrid();
        bool hadMatches = false;
        int totalScore = 0;

        do
        {
            // Отладка состояния грида
            Debug.Log("Grid state before match check:");
            for (int r = 0; r < 3; r++)
                Debug.Log($"Row {r}: {grid[0, r]?.color}, {grid[1, r]?.color}, {grid[2, r]?.color}");

            var matchedCells = GetMatchedCells(grid);
            if (matchedCells.Count == 0) break;

            hadMatches = true;
            HashSet<int> changedColumns = new HashSet<int>();

            Debug.Log($"Found {matchedCells.Count} matched cells: {string.Join(", ", matchedCells)}");

            foreach (var cell in matchedCells)
            {
                var circle = grid[cell.col, cell.row];
                if (circle == null) continue;

                if (ScoreMap.TryGetValue(circle.color, out int points))
                    totalScore += points;

                // Эффект удаления: два партикл-эффекта
                if (ObjectPool.Instance != null)
                {
                    // Первый эффект (сзади)
                    var fx1 = ObjectPool.Instance.Get("ParticleCircle");
                    if (fx1 != null)
                    {
                        fx1.transform.position = circle.transform.position + new Vector3(0, 0, 1); // Сзади (Z+1)
                        fx1.SetActive(true);
                        BoardController.Instance.StartCoroutine(ReleaseAfterTime(fx1, 0.5f));
                    }

                    // Второй эффект (поверх)
                    var fx2 = ObjectPool.Instance.Get("ParticleExplosion");
                    if (fx2 != null)
                    {
                        fx2.transform.position = circle.transform.position + new Vector3(0, 0, -1); // Поверх (Z-1)
                        fx2.SetActive(true);
                        BoardController.Instance.StartCoroutine(ReleaseAfterTime(fx2, 0.5f));
                    }
                }

                // Звук удаления через SoundManager
                if (SoundManager.Instance != null)
                {
                    AudioClip deleteSound = SoundManager.Instance.GetRandomDeleteSound();
                    if (deleteSound != null)
                    {
                        AudioSource.PlayClipAtPoint(deleteSound, circle.transform.position);
                    }
                }

                circle.gameObject.SetActive(false);
                ObjectPool.Instance?.Release("Circle", circle.gameObject);

                BoardController.Instance.RemoveCell(cell.col, cell.row);
                var column = BoardController.Instance.GetColumnController(cell.col);
                if (column != null)
                    column.RemoveCircle(circle);

                changedColumns.Add(cell.col);
            }

            // Ждем анимацию сдвига для всех измененных колонок
            foreach (var col in changedColumns)
            {
                var column = BoardController.Instance.GetColumnController(col);
                if (column != null)
                    yield return BoardController.Instance.StartCoroutine(column.CollapseColumn());
            }

            // Пересобираем грид после всех анимаций
            BoardController.Instance.RebuildGrid();
            grid = BoardController.Instance.GetGrid();

            // Отладка состояния stack после анимации
            foreach (var col in changedColumns)
            {
                var column = BoardController.Instance.GetColumnController(col);
                if (column != null)
                {
                    Debug.Log($"Column {col} stack after collapse: {string.Join(", ", column.stack.ConvertAll(go => go != null ? go.GetComponent<Circle>()?.color.ToString() : "null"))}");
                }
            }

        } while (true);

        if (hadMatches)
        {
            ScoreManager.Instance?.AddScore(totalScore);
            UIManager.Instance?.UpdateScore(ScoreManager.Instance.GetScore());
        }

        // Проверяем game over после всех матчей
        if (BoardController.Instance.IsFull() && !GameplayManager.Instance.IsGameOver)
            GameplayManager.Instance.GameOver();

        yield return null;
    }

    private static List<(int col, int row)> GetMatchedCells(Circle[,] grid)
    {
        List<(int col, int row)> matched = new List<(int col, int row)>();

        // Горизонтали с отладкой цветов
        for (int r = 0; r < 3; r++)
        {
            Debug.Log($"Checking row {r}: {grid[0,r]?.color}, {grid[1,r]?.color}, {grid[2,r]?.color}");
            if (grid[0,r] != null && grid[1,r] != null && grid[2,r] != null)
            {
                if (grid[0,r].color == grid[1,r].color && grid[1,r].color == grid[2,r].color)
                {
                    Debug.Log($"Horizontal match at row {r}: ({0},{r}), ({1},{r}), ({2},{r})");
                    matched.AddRange(new (int,int)[]{ (0,r), (1,r), (2,r) });
                }
            }
        }

        // Вертикали
        for (int c = 0; c < 3; c++)
        {
            if (grid[c,0] != null && grid[c,1] != null && grid[c,2] != null)
            {
                if (grid[c,0].color == grid[c,1].color && grid[c,1].color == grid[c,2].color)
                {
                    Debug.Log($"Vertical match at col {c}: ({c},0), ({c},1), ({c},2)");
                    matched.AddRange(new (int,int)[]{ (c,0), (c,1), (c,2) });
                }
            }
        }

        // Диагонали
        Circle center = grid[1,1];
        if (center != null)
        {
            if (grid[0,0] != null && grid[2,2] != null)
            {
                if (grid[0,0].color == center.color && grid[2,2].color == center.color)
                {
                    Debug.Log("Diagonal match: (0,0), (1,1), (2,2)");
                    matched.AddRange(new (int,int)[]{ (0,0), (1,1), (2,2) });
                }
            }

            if (grid[2,0] != null && grid[0,2] != null)
            {
                if (grid[2,0].color == center.color && grid[0,2].color == center.color)
                {
                    Debug.Log("Diagonal match: (2,0), (1,1), (0,2)");
                    matched.AddRange(new (int,int)[]{ (2,0), (1,1), (0,2) });
                }
            }
        }

        return new List<(int,int)>(new HashSet<(int,int)>(matched));
    }

    private static IEnumerator ReleaseAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        if (obj.name.Contains("ParticleCircle"))
            ObjectPool.Instance.Release("ParticleCircle", obj);
        else if (obj.name.Contains("ParticleExplosion"))
            ObjectPool.Instance.Release("ParticleExplosion", obj);
    }
}
