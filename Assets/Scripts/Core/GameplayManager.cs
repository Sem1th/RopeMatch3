using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [Header("Core References")]
    [SerializeField] private BoardController boardController;
    [SerializeField] private Pendulum pendulum;
    [SerializeField] private CircleFactory circleFactory;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private string menuSceneName;

    private bool isGameOver;

    public bool IsGameOver => isGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnTap += OnTap;
        }
        else
        {
            Debug.LogError("GameplayManager: InputHandler.Instance is null!");
        }
        isGameOver = false;
        UpdateReferences();
        PlayBackgroundMusicIfAvailable();
    }

    private void OnEnable()
    {
        UpdateReferences();
        PlayBackgroundMusicIfAvailable();
    }

    private void OnDestroy()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnTap -= OnTap;
        }
    }

    private void UpdateReferences()
    {
        if (pendulum == null)
        {
            pendulum = FindObjectOfType<Pendulum>();
            if (pendulum == null)
            {
                Debug.LogError("GameplayManager: Pendulum not found!");
            }
        }
        if (boardController == null)
        {
            boardController = FindObjectOfType<BoardController>();
            if (boardController == null)
            {
                Debug.LogError("GameplayManager: BoardController not found!");
            }
        }
        if (circleFactory == null)
        {
            circleFactory = FindObjectOfType<CircleFactory>();
            if (circleFactory == null)
            {
                Debug.LogError("GameplayManager: CircleFactory not found!");
            }
        }
        if (restartButton == null)
        {
            restartButton = GameObject.Find("RestartButton")?.GetComponent<Button>();
            if (restartButton == null)
            {
                Debug.LogWarning("GameplayManager: RestartButton not found!");
            }
        }
        if (backToMenuButton == null)
        {
            backToMenuButton = GameObject.Find("BackToMenuButton")?.GetComponent<Button>();
            if (backToMenuButton == null)
            {
                Debug.LogWarning("GameplayManager: BackToMenuButton not found!");
            }
        }

        RegisterButtons();
    }

    private void RegisterButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }
        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.RemoveAllListeners();
            backToMenuButton.onClick.AddListener(BackToMenu);
        }
    }

    private void PlayBackgroundMusicIfAvailable()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBackgroundMusic();
        }
    }

    public void RestartGame()
    {
        isGameOver = false;
        ScoreManager.Instance?.Reset();
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBackgroundMusic();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBackgroundMusic();
        }
        SceneManager.LoadScene(menuSceneName);
    }

    private void OnTap(Vector2 pos)
    {
        if (isGameOver) return;

        if (pendulum == null)
        {
            pendulum = FindObjectOfType<Pendulum>();
            if (pendulum == null)
            {
                Debug.LogError("GameplayManager: Pendulum not found in OnTap!");
                return;
            }
        }

        pendulum.DropCircle();
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayDropSound(pos);
        }
        Debug.Log($"Tapped at {pos}, dropping circle");
    }

    public void OnCircleLanded()
    {
        if (boardController != null && boardController.IsFull())
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over!");
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBackgroundMusic();
            SoundManager.Instance.PlayGameOverSound();
        }
        UIManager.Instance?.ShowScoreScreen();
        // FindObjectOfType<PauseManager>()?.TogglePause();
    }
}

