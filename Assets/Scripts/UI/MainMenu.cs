using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string gameSceneName;

    void Start()
    {
        // Проверяем кнопки
        if (playButton == null || quitButton == null)
        {
            Debug.LogError("MainMenu: Play or Quit button not assigned!");
            return;
        }

        // Назначаем действия кнопкам
        playButton.onClick.AddListener(LoadGameScene);
        quitButton.onClick.AddListener(QuitGame);

        // Убеждаемся, что фоновая музыка играет
        if (SoundManager.Instance != null && SoundManager.Instance.backgroundMusic != null)
        {
            if (!SoundManager.Instance.GetComponent<AudioSource>().isPlaying)
            {
                SoundManager.Instance.GetComponent<AudioSource>().Play();
            }
        }
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName); 
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
