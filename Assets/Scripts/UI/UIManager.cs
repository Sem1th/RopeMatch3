using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TMP_Text scoreText; // Счет
    [SerializeField] private TMP_Text scoreFinalText; // Итоговый счет
    [SerializeField] private GameObject scoreScreen; // Панель итогов
    [SerializeField] private GameObject pausePanel; // Панель паузы
    [SerializeField] private Button resumeButton; // Кнопка возобновления
    [SerializeField] private Button pauseButton; // Новая кнопка паузы

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        UpdateReferences();
    }

    private void OnEnable()
    {
        UpdateReferences();
    }

    private void UpdateReferences()
    {
        if (scoreText == null)
        {
            scoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
            if (scoreText == null)
            {
                Debug.LogWarning("UIManager: ScoreText not found!");
            }
        }
        if (scoreScreen == null)
        {
            scoreScreen = GameObject.Find("ScorePanel")?.gameObject;
            if (scoreScreen == null)
            {
                Debug.LogWarning("UIManager: ScorePanel not found!");
            }
        }
        if (scoreFinalText == null)
        {
            scoreFinalText = GameObject.Find("FinalScoreText")?.GetComponent<TMP_Text>(); 
            if (scoreFinalText == null)
            {
                Debug.LogWarning("UIManager: FinalScoreText not found!");
            }
        }
        if (pausePanel == null)
        {
            pausePanel = GameObject.Find("PausePanel")?.gameObject;
            if (pausePanel == null)
            {
                Debug.LogWarning("UIManager: PausePanel not found!");
            }
            else
            {
                pausePanel.SetActive(false);
            }
        }
        if (resumeButton == null)
        {
            resumeButton = GameObject.Find("ResumeButton")?.GetComponent<Button>();
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(ResumeGame);
            }
            else
            {
                Debug.LogWarning("UIManager: ResumeButton not found!");
            }
        }
        if (pauseButton == null)
        {
            pauseButton = GameObject.Find("PauseButton")?.GetComponent<Button>();
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(TogglePause);
            }
            else
            {
                Debug.LogWarning("UIManager: PauseButton not found!");
            }
        }
    }

    public void ShowScoreScreen()
    {
        if (scoreScreen != null)
        {
            scoreScreen.SetActive(true);
            if (scoreFinalText != null)
            {
                scoreFinalText.text = $"Счет: {ScoreManager.Instance.GetScore()}"; // Используем ScoreManager
            }
            else
            {
                Debug.LogWarning("UIManager: scoreFinalText is null, cannot update final score!");
            }
        }
        else
        {
            Debug.LogWarning("UIManager: scoreScreen is null, cannot show score screen!");
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Счет: {score}";
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
            Time.timeScale = isPaused ? 0f : 1f; // Пауза/возобновление времени
            if (SoundManager.Instance != null)
            {
                if (isPaused)
                {
                    SoundManager.Instance.StopBackgroundMusic();
                }
                else
                {
                    SoundManager.Instance.PlayBackgroundMusic();
                }
            }
        }
    }

    public void ResumeGame()
    {
        TogglePause();
    }
}

