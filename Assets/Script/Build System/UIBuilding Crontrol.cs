using UnityEngine;

public class UIBuildingControl : MonoBehaviour
{
    public BuildManager buildManager;

    // Centralized counter for active UIBuildingControl instances
    private static int activeInstances = 0;

    void OnEnable()
    {
        if (buildManager == null)
        {
            buildManager = FindObjectOfType<BuildManager>();
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
