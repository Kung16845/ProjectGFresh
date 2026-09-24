using UnityEngine;

public class TimePause : MonoBehaviour
{
    public TimeManager timeManager;

    private void Awake()
    {
        EnsureTimeManager();
    }

    private void OnEnable()
    {
        EnsureTimeManager();
        if (timeManager != null)
        {
            timeManager.TimeStop();
        }
    }

    private void OnDisable()
    {
        if (timeManager != null)
        {
            timeManager.TimeContinue();
        }
    }

    private void EnsureTimeManager()
    {
        if (timeManager == null)
        {
            timeManager = TimeManager.Instance != null ? TimeManager.Instance : FindFirstObjectByType<TimeManager>();
        }
    }
}
