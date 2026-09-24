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

    private void Awake()
    {
        ExpenditionManager expenditionManager = ExpenditionManager.Instance != null ? ExpenditionManager.Instance : FindFirstObjectByType<ExpenditionManager>();
        if (expenditionManager != null && transformParentUIEx != null)
        {
            expenditionManager.transformsUIEx = transformParentUIEx;
        }
    }

    public void SetInfoPlaceSelect(DataExpenditionUI dataExpendition)
    {
        if (dataExpendition == null) return;

        if (globalstat == null)
        {
            globalstat = FindFirstObjectByType<Globalstat>();
        }

        if (textNamePlace != null) textNamePlace.text = dataExpendition.namePlace;
        if (textDescriptPlace != null) textDescriptPlace.text = dataExpendition.infoDescriptPlace;
        if (textETA != null) textETA.text = dataExpendition.infoETA;
        if (imagePlace != null) imagePlace.sprite = dataExpendition.spriteImagePlace;
        riskValue = dataExpendition.riskEvent;
        indexSceneExpendition = dataExpendition.indexSceneExpendition;

        bool isTunnelAvailable = globalstat != null && globalstat.Tunnelaviable;
        if (Underground != null)
        {
            if (isTunnelAvailable)
            {
                Underground.gameObject.SetActive(true);
                SetButton(Underground, dataExpendition.timescaleWalk, false, false, true);
            }
            else
            {
                Underground.gameObject.SetActive(false);
            }
        }

        bool hasCar = globalstat != null && globalstat.availablecar > 0;
        if (buttonCar != null)
        {
            if (hasCar)
            {
                buttonCar.gameObject.SetActive(true);
                SetButton(buttonCar, dataExpendition.timescaleCar, true, false, false);
            }
            else
            {
                buttonCar.gameObject.SetActive(false);
            }
        }

        if (buttonWalk != null)
        {
            SetButton(buttonWalk, dataExpendition.timescaleWalk, false, true, false);
        }
    }

    public void SetButton(Button button, float timescale, bool isCar, bool isWalk, bool isTunnel)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => AddButtonExpendition(button, timescale, isCar, isWalk, isTunnel));
    }

    public void AddButtonExpendition(Button button, float timescale, bool isCar, bool isWalk, bool isTunnel)
    {
        if (transformParentUIEx != null && transformParentUIEx.childCount >= 2) return;

        if (isCar) Debug.Log("Selected mode: Car");
        if (isWalk) Debug.Log("Selected mode: Walk");
        if (isTunnel) Debug.Log("Selected mode: Tunnel");

        if (ExpenditionManager.Instance != null)
        {
            ExpenditionManager.Instance.CreateInventorySetExpendition(timescale, riskValue, indexSceneExpendition, isCar, isWalk, isTunnel);
        }
    }
}
