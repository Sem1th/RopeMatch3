using UnityEngine;
using System;

public enum SceneState { MainMenu, Game, Score }

public class SceneStateMachine : MonoBehaviour
{
    public static SceneStateMachine Instance { get; private set; }

    public SceneState CurrentState { get; private set; }

    public event Action<SceneState> OnStateChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(SceneState state)
    {
        if (CurrentState == state) return;
        CurrentState = state;
        // Можно выполнить дополнительные действия при смене состояния
        OnStateChanged?.Invoke(state);

        // Синхронизируем с загрузкой сцен (GameManager)
        switch (state)
        {
            case SceneState.MainMenu: GameManager.Instance.LoadMainMenu(); break;
            case SceneState.Game: GameManager.Instance.LoadGame(); break;
        }
    }

    // Альтернативный метод — автоопределение состояния по имени сцены
}

