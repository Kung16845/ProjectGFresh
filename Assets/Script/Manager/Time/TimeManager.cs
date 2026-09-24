using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public SaveDataDDA saveDataDDA;
    public DateTime dateTime;

    [Header("Tick Setting")]
    [SerializeField] private int tickSeconedIncrease = 1;
    [SerializeField] private int speedGame = 1;

    [Min(0)]
    public int dayCountAttack;
    public int currentTickSeconedIncrease;
    public float timeBetweenTicks = 1f;
    public float currentTimeBetweenTricks = 0f;

    public static UnityAction<DateTime> OnDateTimeChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dateTime == null)
        {
            dateTime = new DateTime(0, 6, 0, false);
        }

        currentTickSeconedIncrease = tickSeconedIncrease;
    }

    private void Start()
    {
        OnDateTimeChanged?.Invoke(dateTime);
    }

    public void SkipDayTime()
    {
        if (dateTime == null) return;
        dateTime.hour = 17;
        dateTime.minutes = 59;
        OnDateTimeChanged?.Invoke(dateTime);
    }

    public void AccelerateTime(int speed)
    {
        speedGame = speed;
        currentTickSeconedIncrease = tickSeconedIncrease * speed;
    }

    public void SetDayNightAttack()
    {
        dayCountAttack = Random.Range(1, 3);
        if (dateTime != null)
        {
            dateTime.isDayNight = true;
        }
    }

    private void Update()
    {
        if (dateTime == null) return;

        currentTimeBetweenTricks += Time.deltaTime;
        if (currentTimeBetweenTricks >= timeBetweenTicks)
        {
            currentTimeBetweenTricks = 0f;
            Tick();
        }

        if (!dateTime.isDayNight && dayCountAttack <= 0)
        {
            SetDayNightAttack();
        }
    }

    public void Tick()
    {
        AdvanceTime();
    }

    public void AdvanceTime()
    {
        if (dateTime == null) return;
        dateTime.AdvanceMinutes(currentTickSeconedIncrease);
        OnDateTimeChanged?.Invoke(dateTime);
    }

    public void TimeStop()
    {
        currentTickSeconedIncrease = 0;
    }

    public void TimeContinue()
    {
        currentTickSeconedIncrease = (speedGame > 0) ? (tickSeconedIncrease * speedGame) : tickSeconedIncrease;
    }
}

[System.Serializable]
public class DateTime
{
    public int day;
    public int hour;
    public int minutes;
    public bool isDayNight;

    public DateTime(int day, int hour, int minutes, bool isHaveDayNight)
    {
        this.day = day;
        this.hour = hour;
        this.minutes = minutes;
        this.isDayNight = isHaveDayNight;
    }

    public void SetTimeStartDay()
    {
        hour = 6;
        minutes = 0;
    }

    public void SetTimeNightDay()
    {
        hour = 21;
        minutes = 30;
    }

    public void AdvanceMinutes(int secondToAdvanceBy)
    {
        minutes += secondToAdvanceBy;
        if (minutes >= 60)
        {
            hour += minutes / 60;
            minutes %= 60;
            AdvanceDay();
        }
    }

    public void AdvanceDay()
    {
        if (!isDayNight)
        {
            if (hour >= 18)
            {
                SetTimeStartDay();
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SaveGame();
                }
                day++;
            }
        }
        else
        {
            if (hour >= 24)
            {
                hour %= 24;
            }

            if (hour >= 4 && hour < 18)
            {
                isDayNight = false;
                day++;
                if (SceneSystem.Instance != null)
                {
                    SceneSystem.Instance.ReturnToMainScene();
                }
            }
            else if (hour >= 18 && TimeManager.Instance != null && TimeManager.Instance.dayCountAttack <= 0)
            {
                SetTimeNightDay();
                if (SceneSystem.Instance != null)
                {
                    SceneSystem.Instance.SwitchScene(1);
                }
            }
        }
    }
}
