using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIExSelectPlace : MonoBehaviour
{
    public TextMeshProUGUI textNamePlace;
    public TextMeshProUGUI textDescriptPlace;
    public TextMeshProUGUI textETA;
    public Image imagePlace;
    public float riskValue;
    public int indexSceneExpendition;
    public Globalstat globalstat;
    public Transform transformParentUIEx;
    public Button buttonWalk;
    public Button buttonCar;
    public Button Underground;

    private void Awake() {
        ExpenditionManager expenditionManager = FindObjectOfType<ExpenditionManager>();
        expenditionManager.transformsUIEx = transformParentUIEx;
    }
    public void SetInfoPlaceSelect(DataExpenditionUI dataExpendition)
    {
        textNamePlace.text = dataExpendition.namePlace;
        textDescriptPlace.text = dataExpendition.infoDescriptPlace;
        textETA.text = dataExpendition.infoETA;
        imagePlace.sprite = dataExpendition.spriteImagePlace;
        riskValue = dataExpendition.riskEvent;
        indexSceneExpendition = dataExpendition.indexSceneExpendition;

        // Check if cars are available
        if(globalstat.Tunnelaviable)
        {
            Underground.gameObject.SetActive(true);
            SetButton(Underground, dataExpendition.timescaleWalk, false, false, true); // Tunnel mode
        }
        else if(globalstat.Tunnelaviable)
            Underground.gameObject.SetActive(false);
        if (globalstat.availablecar > 0)
        {
            buttonCar.gameObject.SetActive(true);
            SetButton(buttonCar, dataExpendition.timescaleCar, true, false, false); // Car mode
        }
        if (globalstat.availablecar <= 0)
            buttonCar.gameObject.SetActive(false);
        SetButton(buttonWalk, dataExpendition.timescaleWalk, false, true, false); // Walk mode
    }

    public void SetButton(Button button, float timescale, bool isCar, bool isWalk, bool isTunnel)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => AddButtonExpendition(button, timescale, isCar, isWalk, isTunnel));
    }
    public void AddButtonExpendition(Button button, float timescale, bool isCar, bool isWalk, bool isTunnel)
    {
        // Debug.Log("Add Button for Car, Walk, or Tunnel");
        if (transformParentUIEx.childCount == 2) return;

        // Log transport mode for debugging purposes
        if (isCar) Debug.Log("Selected mode: Car");
        if (isWalk) Debug.Log("Selected mode: Walk");
        if (isTunnel) Debug.Log("Selected mode: Tunnel");

        // Pass the transportation mode to the expedition manager
        ExpenditionManager.Instance.CreateInventorySetExpendition(timescale, riskValue, indexSceneExpendition, isCar, isWalk, isTunnel);
    }
}
