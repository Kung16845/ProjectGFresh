using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuilding : MonoBehaviour
{
    public Building building;
    public BuildManager buildManager;
    public Image image;
    public TextMeshProUGUI textNameBuild;
    public TextMeshProUGUI textDescriveBuild;
    public TextMeshProUGUI textPlankCost;
    public TextMeshProUGUI textSteelCost;
    public TextMeshProUGUI textNpcCost;
    public TextMeshProUGUI textDayCost;

    void Awake()
    {
        // Initialize BuildManager
        if (buildManager == null)
        {
            buildManager = BuildManager.Instance;
        }
    }

    public void SetDataBuild()
    {
        BuildingData data = building.buildingData;
        if (data != null)
        {
            textNameBuild.text = data.buildingName;
            textPlankCost.text = data.plankCost.ToString();
            textSteelCost.text = data.steelCost.ToString();
            textNpcCost.text = data.workerPointsRequired.ToString();
            textDayCost.text = data.buildTimeHours.ToString();
        }
        else
        {
            textNameBuild.text = building.nameBuild;
            textPlankCost.text = building.plankCost.ToString();
            textSteelCost.text = building.steelCost.ToString();
            textNpcCost.text = building.workerPointsCost.ToString();
            textDayCost.text = building.buildTimeHours.ToString();
        }
        textDescriveBuild.text = building.detailBuild;
        image.sprite = building.GetComponent<SpriteRenderer>().sprite;
        buildManager.building = building;
    }

    // void OnEnable()
    // {
    //     // Disable colliders when the UI is active
    //     DisableColliders();
    // }

    // void OnDisable()
    // {
    //     // Re-enable colliders when the UI is deactivated
    //     EnableColliders();
    // }

    public void DisableColliders()
    {
        foreach (Collider2D col in buildManager.collidersToManage)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }

    public void EnableColliders()
    {
        foreach (Collider2D col in buildManager.collidersToManage)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }
    }
}
