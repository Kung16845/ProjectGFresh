using UnityEngine;

public class CountdownTimeDay : MonoBehaviour
{
    public float timeScale;
    public float ratio;
    public float timeInSeconds;
    public int finishDayCraftingTime;
    public int finishHourCraftingTime;
    public int finishMinutesCraftingTime;

    public GameObject iconCompleteSend;
    public TimeManager timeManager;
    public UIInventoryEX uIInventoryEX;

    public void SaveDayFinishExpenditionInUIEX()
    {
        if (uIInventoryEX == null) return;
        uIInventoryEX.finishDayCraftingTime = finishDayCraftingTime;
        uIInventoryEX.finishHourCraftingTime = finishHourCraftingTime;
        uIInventoryEX.finishMinutesCraftingTime = finishMinutesCraftingTime;
    }

    public void SetStartExpendition()
    {
        if (timeManager == null)
        {
            timeManager = TimeManager.Instance != null ? TimeManager.Instance : FindFirstObjectByType<TimeManager>();
        }

        if (timeManager == null || timeManager.dateTime == null)
        {
            Debug.LogWarning("[CountdownTimeDay] TimeManager or dateTime is null!");
            return;
        }

        ratio = timeScale / 1000f;
        timeInSeconds = ratio * 60f;

        int totalMinutesToAdd = (int)timeInSeconds;
        int currentDay = timeManager.dateTime.day;
        int currentHour = timeManager.dateTime.hour;
        int currentMinutes = timeManager.dateTime.minutes;

        int newMinutes = currentMinutes + totalMinutesToAdd;
        int newHour = currentHour + (newMinutes / 60);
        newMinutes %= 60;

        if (newHour >= 18)
        {
            finishDayCraftingTime = currentDay + 1;
            finishHourCraftingTime = 6 + (newHour - 18);
            finishMinutesCraftingTime = newMinutes;
        }
        else
        {
            finishDayCraftingTime = currentDay;
            finishHourCraftingTime = newHour;
            finishMinutesCraftingTime = newMinutes;
        }

        SaveDayFinishExpenditionInUIEX();
    }

    private void Update()
    {
        if (timeManager == null)
        {
            timeManager = TimeManager.Instance != null ? TimeManager.Instance : FindFirstObjectByType<TimeManager>();
            if (timeManager == null) return;
        }

        if (timeManager.dateTime == null) return;

        bool isComplete = (timeManager.dateTime.day > finishDayCraftingTime) ||
                          (timeManager.dateTime.day == finishDayCraftingTime &&
                           (timeManager.dateTime.hour > finishHourCraftingTime ||
                            (timeManager.dateTime.hour == finishHourCraftingTime && timeManager.dateTime.minutes >= finishMinutesCraftingTime)));

        if (isComplete)
        {
            if (uIInventoryEX != null)
            {
                if (!uIInventoryEX.isArriveEx && !uIInventoryEX.isArriveHome)
                {
                    uIInventoryEX.isArriveEx = true;
                }
                else
                {
                    uIInventoryEX.isArriveHome = true;
                }
            }

            if (iconCompleteSend != null)
            {
                iconCompleteSend.SetActive(true);
            }

            Destroy(this);
        }
    }
}
