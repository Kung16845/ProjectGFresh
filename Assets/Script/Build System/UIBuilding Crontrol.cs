using System.Collections.Generic;
using UnityEngine;

public class UIBuildingControl : MonoBehaviour
{
    public BuildManager buildManager;
    public List<UIBuilding> uIBuildings;

    // Centralized counter for active UIBuildingControl instances
    private static int activeInstances = 0;
    public void ShowUIBuildingSize(int indexSize)
    {
        foreach (UIBuilding uIBuilding in uIBuildings)
        {
            if (uIBuilding.building.buildingType == (BuildingType)indexSize)
            {
                uIBuilding.gameObject.SetActive(true);
            }
            else
            {
                uIBuilding.gameObject.SetActive(false);
            }
        }
    }
    void OnEnable()
    {   
        ShowUIBuildingSize(0);
        if (buildManager == null)
        {
            buildManager = GameManager.Instance.buildManager;
        }

        // Increment the active instance counter
        activeInstances++;

        // Only disable colliders when this is the first active instance
        if (activeInstances == 1 && buildManager != null)
        {
            buildManager.DisableColliders();
        }
    }

    void OnDisable()
    {
        // Decrement the active instance counter
        activeInstances--;

        // If no active instances remain, enable the colliders
        if (activeInstances == 0 && buildManager != null)
        {
            buildManager.EnableColliders();
        }
    }
}
