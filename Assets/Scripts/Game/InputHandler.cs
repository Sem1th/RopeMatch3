using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    public event Action<Vector2> OnTap; // Координаты касаний

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (OnTap == null)
        {
            Debug.LogWarning("InputHandler: No subscribers to OnTap event!");
            return;
        }

        // Touch
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Ended)
            {
                if (!IsPointerOverUI(t.fingerId))
                {
                    Vector2 wp = Camera.main.ScreenToWorldPoint(t.position);
                    OnTap?.Invoke(wp);
                    Debug.Log($"Touch tap at world position: {wp}");
                }
            }
        }
        else // Mouse
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverUI())
                {
                    Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    OnTap?.Invoke(wp);
                    Debug.Log($"Mouse tap at world position: {wp}");
                }
            }
        }
    }

    bool IsPointerOverUI() => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    bool IsPointerOverUI(int touchId) => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touchId);
}


