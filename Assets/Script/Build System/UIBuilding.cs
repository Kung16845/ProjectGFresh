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
        textNameBuild.text = building.nameBuild;
        textDescriveBuild.text = building.detailBuild;
        textPlankCost.text = building.plankCost.ToString();
        textSteelCost.text = building.steelCost.ToString();
        textNpcCost.text = building.npcCost.ToString();
        textDayCost.text = building.dayCost.ToString();
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
