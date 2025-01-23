using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public SceneSystem sceneSystem1;
    public SaveDataDDA saveDataDDA;
    public DateTime dateTime;
    [Header("Tick Setting")]
    [SerializeField] private int tickSeconedIncrease;
    [SerializeField] private int speedGame;
    public int currentTickSeconedIncrease;
    public float timeBetweenTicks = 1;
    public float currentTimeBetweenTricks = 0;
    public static UnityAction<DateTime> OnDateTimeChanged;
    private void Awake()
    {   
        
        dateTime = new DateTime(0, 0, 0, true, sceneSystem1,saveDataDDA,FindObjectOfType<GameManager>());
        dateTime.SetTimeStartDay();
        currentTickSeconedIncrease = tickSeconedIncrease;
    }
    public void SkipDayTime()
    {
        dateTime.hour = 17;
        dateTime.minutes = 59;
    } 
    public void AccelerateTime(int speed)
    {
        speedGame = speed;
        currentTickSeconedIncrease = tickSeconedIncrease * speed;
    }
    public void SetDayNIght()
    {
        dateTime.isDayNight = true;
    }
    private void Start()
    {
        Debug.Log("Start Scene");
        OnDateTimeChanged?.Invoke(dateTime);
        // sceneSystem1 = FindObjectOfType<SceneSystem>();
        // dateTime.sceneSystem = sceneSystem1;
    }
    private void Update()
    {
        currentTimeBetweenTricks += Time.deltaTime;
        if (currentTimeBetweenTricks > timeBetweenTicks)
        {
            currentTimeBetweenTricks = 0;
            Tick();
            
        }
    }
    public void Tick()
    {
        AdvanceTime();
    }
    public void AdvanceTime()
    {
        dateTime.AdvanceMinutes(currentTickSeconedIncrease);
    }
    public void TimeStop()
    {
        currentTickSeconedIncrease = 0;
    }
    public void TimeContinue()
    {
        currentTickSeconedIncrease = tickSeconedIncrease;
    }
}
[System.Serializable]
public class DateTime
{   
    public int day;
    public int hour;
    public int minutes;
    public bool isDayNight;
    public SceneSystem sceneSystem;
    public SaveDataDDA saveDataDDA;
    public GameManager gameManager;
    public DateTime(int day, int hour, int minutes, bool isHaveDayNight, SceneSystem sceneSystem, SaveDataDDA saveDataDDA,GameManager gameManager)
    {
        this.day = day;
        this.hour = hour;
        this.minutes = minutes;
        this.isDayNight = isHaveDayNight;
        this.sceneSystem = sceneSystem;
        this.saveDataDDA = saveDataDDA;
        this.gameManager = gameManager;
    }
    public void SetTimeStartDay()
    {   
        
        this.day++;
        this.hour = 6;
        this.minutes = 0;
        gameManager.SaveGame();
    }
    public void SetTimeNightDay()
    {
        this.hour = 21;
        this.minutes = 30;
    }
    public void AdvanceMinutes(int secondToAdvanceBy)
    {
        if (minutes + secondToAdvanceBy >= 60)
        {
            minutes = (minutes + secondToAdvanceBy) % 60;
            hour++;
            AdvanceDay();
        }
        else
        {
            minutes += secondToAdvanceBy;
        }
    }

    public void AdvanceDay()
    {
        if (!isDayNight)
        {
            if (this.hour == 18 && this.minutes == 0)
            {
                SetTimeStartDay();

                // this.day++;
            }
        }
        else if(isDayNight)
        {

            if (this.hour == 24 && this.minutes == 0)
            {
                this.hour = 0;
            }
            else if (this.hour == 4 && this.minutes == 0)
            {

                // this.isDayNight = false;
                // this.day++;
                // SetTimeStartDay();
                // sceneSystem.SwitchScene(0);
                // saveDataDDA.AddData();
                sceneSystem.ReturnToMainScene();

            }
            else if (SceneManager.GetActiveScene().buildIndex == 0 && this.hour == 18 && this.minutes >= 0)
            {
                Debug.Log("Sceneswitch");
                SetTimeNightDay();
                sceneSystem.SwitchScene(1);
            }
        }
    }
}
