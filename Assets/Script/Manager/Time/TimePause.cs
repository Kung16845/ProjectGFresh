using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePause : MonoBehaviour
{
    public TimeManager timeManager;
    // Start is called before the first frame update
    void Awake()
    {
        timeManager = FindObjectOfType<TimeManager>();
    }
    private void Update() {
        if(this.gameObject.activeSelf)
        {
            timeManager.TimeStop();
        }
        else 
        {
            timeManager.TimeContinue();
        }
    }
    private void OnEnable() {
        timeManager.TimeStop();
    }
    private void OnDisable() {
        timeManager.TimeContinue();
    }
  
}
