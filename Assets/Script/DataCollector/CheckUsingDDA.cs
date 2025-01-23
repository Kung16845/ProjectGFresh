using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckUsingDDA : MonoBehaviour
{
    public bool isUsingDDA; 
    public bool ActiveTutorial = true;
    public MainSpawner mainSpawner;
    public void USEDDA()
    {
        isUsingDDA = true;
    }
    public void NOTUSEDDA()
    {
        isUsingDDA = false;
    }
    public void ReplayTutorial()
    {
        ActiveTutorial = true;
    }
    public void DisableTutorial()
    {
        ActiveTutorial = false;
    }
}
