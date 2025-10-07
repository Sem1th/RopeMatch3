using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private int score = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        // DontDestroyOnLoad(gameObject);
    }

    public static int GetScoreForColor(int colorIndex)
    {
        // Red=100, Purple=70, Green=50
        switch (colorIndex)
        {
            case 0: return 100;
            case 1: return 70;
            case 2: return 50;
            default: return 0;
        }
    }

    public void AddScore(int s) { score += s; }
    public int GetScore() => score;
    public void Reset() => score = 0;
}

