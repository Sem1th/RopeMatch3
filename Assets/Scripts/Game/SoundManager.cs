using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Delete Sounds")]
    public AudioClip[] deleteSounds;
    [Range(0f, 1f)]
    public float deleteVolume = 0.8f;

    [Header("Background Music")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)]
    public float backgroundVolume = 0.3f;

    [Header("Game Over Sound")]
    public AudioClip gameOverSound;
    [Range(0f, 1f)]
    public float gameOverVolume = 0.9f;

    [Header("Drop Sound")]
    public AudioClip dropSound;
    [Range(0f, 1f)]
    public float dropVolume = 0.7f;

    private AudioSource backgroundAudioSource;

    [SerializeField] private float defaultGlobalVolume = 0.5f;
    private float currentGlobalVolume;
    private bool isSoundMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        LoadSettings();
        SetupAudio();
    }

    private void Start()
    {
        SetupAudio();
    }

    private void OnEnable()
    {
        SetupAudio();
    }

    private void LoadSettings()
    {
        currentGlobalVolume = PlayerPrefs.GetFloat("GlobalVolume", defaultGlobalVolume);
        isSoundMuted = PlayerPrefs.GetInt("IsSoundMuted", 0) == 1;
        ApplyVolume();
        Debug.Log($"Loaded: volume={currentGlobalVolume}, muted={isSoundMuted}");
    }

    public void SetGlobalVolume(float volume)
    {
        currentGlobalVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("GlobalVolume", currentGlobalVolume);
        PlayerPrefs.Save();
        ApplyVolume();
    }

    public float GetGlobalVolume()
    {
        return currentGlobalVolume;
    }

    private void SetupAudio()
    {
        if (backgroundMusic != null && backgroundAudioSource != null)
        {
            backgroundAudioSource.clip = backgroundMusic;
            backgroundAudioSource.loop = true;
            ApplyVolume();
            EnsureBackgroundPlaying();
        }
    }

    private void ApplyVolume()
    {
        float effectiveVolume = isSoundMuted ? 0f : currentGlobalVolume;
        if (backgroundAudioSource != null)
            backgroundAudioSource.volume = backgroundVolume * effectiveVolume;
        UpdateAudioListener();
    }

    private void UpdateAudioListener()
    {
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener != null)
        {
            listener.enabled = !isSoundMuted;
            Debug.Log($"AudioListener enabled: {listener.enabled}");
        }
    }

    public void ToggleMute()
    {
        isSoundMuted = !isSoundMuted;
        PlayerPrefs.SetInt("IsSoundMuted", isSoundMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyVolume();
        EnsureBackgroundPlaying(); // Убеждаемся, что музыка играет после переключения
        Debug.Log($"Toggled mute to: {isSoundMuted}");
    }

    private void EnsureBackgroundPlaying()
    {
        if (backgroundAudioSource != null && backgroundMusic != null && !isSoundMuted && !backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Play();
            Debug.Log("Ensuring background music is playing");
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundAudioSource != null && !backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Play();
            Debug.Log("Playing background music manually");
        }
    }

    public void StopBackgroundMusic()
    {
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.Stop();
            Debug.Log("Stopping background music");
        }
    }

    public AudioClip GetRandomDeleteSound()
    {
        if (deleteSounds == null || deleteSounds.Length == 0)
            return null;
        return deleteSounds[Random.Range(0, deleteSounds.Length)];
    }

    public void PlayDeleteSound(Vector3 position)
    {
        if (deleteSounds == null || deleteSounds.Length == 0) return;
        AudioClip clip = GetRandomDeleteSound();
        if (clip != null)
        {
            float volume = deleteVolume * (isSoundMuted ? 0f : currentGlobalVolume);
            if (volume > 0f)
            {
                AudioSource.PlayClipAtPoint(clip, position, volume);
            }
            Debug.Log($"Playing delete sound, volume={volume}, muted={isSoundMuted}");
        }
    }

    public void PlayDropSound(Vector3 position)
    {
        if (dropSound != null)
        {
            float volume = dropVolume * (isSoundMuted ? 0f : currentGlobalVolume);
            if (volume > 0f)
            {
                AudioSource.PlayClipAtPoint(dropSound, position, volume);
            }
            Debug.Log($"Playing drop sound, volume={volume}, muted={isSoundMuted}");
        }
    }

    public void PlayGameOverSound()
    {
        if (gameOverSound != null)
        {
            float volume = gameOverVolume * (isSoundMuted ? 0f : currentGlobalVolume);
            if (volume > 0f)
            {
                AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position, volume);
            }
            Debug.Log($"Playing game over sound, volume={volume}, muted={isSoundMuted}");
        }
    }
}