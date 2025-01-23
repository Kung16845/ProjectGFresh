using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
        uIInventoryEX.finishDayCraftingTime = finishDayCraftingTime;
        uIInventoryEX.finishHourCraftingTime = finishHourCraftingTime;
        uIInventoryEX.finishMinutesCraftingTime = finishMinutesCraftingTime;
    }
    public void SetStartExpendition()
    {
       
        timeManager = FindObjectOfType<TimeManager>();
        ratio = timeScale / 1000f;
        timeInSeconds = ratio * 60;
      

        if (timeManager.dateTime.hour + (timeInSeconds / 60) >= 18)
        {
            finishDayCraftingTime = timeManager.dateTime.day + 1;
            finishHourCraftingTime = 6 + timeManager.dateTime.hour + (int)(timeInSeconds / 60) - 18;
            finishMinutesCraftingTime = timeManager.dateTime.minutes +(int)timeInSeconds % 60;
        }
        else
        {
            finishDayCraftingTime = timeManager.dateTime.day;
            finishHourCraftingTime = timeManager.dateTime.hour + (int)(timeInSeconds / 60);
            finishMinutesCraftingTime = timeManager.dateTime.minutes +(int)timeInSeconds % 60;
        }
        
        SaveDayFinishExpenditionInUIEX();
        // Debug.Log("Day : " + finishDayCraftingTime + " Hour : " + finishHourCraftingTime
        // + " Minutes : " + finishMinutesCraftingTime);

    }
    // Update is called once per frame
    void Update()
    {   


        if (timeManager.dateTime.day > finishDayCraftingTime  || 
        (timeManager.dateTime.day == finishDayCraftingTime &&
        timeManager.dateTime.hour >= finishHourCraftingTime &&
        timeManager.dateTime.minutes >= finishMinutesCraftingTime) )
        {   
            if(!uIInventoryEX.isArriveEx && !uIInventoryEX.isArriveHome)
                uIInventoryEX.isArriveEx = true;
            else 
            {   
                uIInventoryEX.isArriveHome = true;
            }
            iconCompleteSend.gameObject.SetActive(true);
            Destroy(this);
        }
    }

}
