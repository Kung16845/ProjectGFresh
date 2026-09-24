using System.Collections.Generic;
using UnityEngine;

public class UIBuildingControl : MonoBehaviour
{
    public BuildManager buildManager;
    public List<UIBuilding> uIBuildings = new List<UIBuilding>();

    // Centralized counter for active UIBuildingControl instances
    private static int activeInstances = 0;

    public void ShowUIBuildingSize(int indexSize)
    {
        BuildingType targetType = (BuildingType)indexSize;
        for (int i = 0; i < uIBuildings.Count; i++)
        {
            UIBuilding uIBuilding = uIBuildings[i];
            if (uIBuilding != null && uIBuilding.building != null)
            {
                uIBuilding.gameObject.SetActive(uIBuilding.building.buildingType == targetType);
            }
        }
    }

    private void OnEnable()
    {   
        ShowUIBuildingSize(0);

        if (buildManager == null)
        {
            buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : BuildManager.Instance;
        }

        activeInstances++;

        // Only disable colliders when this is the first active instance
        if (activeInstances == 1 && buildManager != null)
        {
            buildManager.DisableColliders();
        }
    }

    private void OnDisable()
    {
        activeInstances--;

        // If no active instances remain, enable the colliders
        if (activeInstances <= 0)
        {
            activeInstances = 0;
            if (buildManager != null)
            {
                buildManager.EnableColliders();
            }
        }
    }
}
