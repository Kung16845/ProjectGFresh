using UnityEngine;
using UnityEngine.UI;

public class ResourceStatusTracker : MonoBehaviour
{
    [Header("Dependencies")]
    public BuildManager buildManager; // Reference to the BuildManager script

    [Header("Resource Status")]
    public bool isWaterAvailable;
    public bool isElectricityAvailable;

    [Header("UI Elements")]
    public Image waterStatusImage;  // Image object to represent water status
    public Image electricityStatusImage; // Image object to represent electricity status

    [Header("Colors - Water")]
    [SerializeField] private string waterActiveColorHex = "#4E4531"; // Water active color
    [SerializeField] private string waterInactiveColorHex = "#808080"; // Inactive color (gray)

    [Header("Colors - Electricity")]
    [SerializeField] private string electricityActiveColorHex = "#32CD32"; // Electricity active color (lime green)
    [SerializeField] private string electricityInactiveColorHex = "#808080"; // Inactive color (gray)

    private Color waterActiveColor;
    private Color waterInactiveColor;
    private Color electricityActiveColor;
    private Color electricityInactiveColor;

    void OnEnable()
    {
        // Convert hex color strings to Unity Color objects for water
        if (!ColorUtility.TryParseHtmlString(waterActiveColorHex, out waterActiveColor))
        {
            Debug.LogError("Invalid water active color hex string!");
        }
        if (!ColorUtility.TryParseHtmlString(waterInactiveColorHex, out waterInactiveColor))
        {
            Debug.LogError("Invalid water inactive color hex string!");
        }

        // Convert hex color strings to Unity Color objects for electricity
        if (!ColorUtility.TryParseHtmlString(electricityActiveColorHex, out electricityActiveColor))
        {
            Debug.LogError("Invalid electricity active color hex string!");
        }
        if (!ColorUtility.TryParseHtmlString(electricityInactiveColorHex, out electricityInactiveColor))
        {
            Debug.LogError("Invalid electricity inactive color hex string!");
        }

        // Initialize BuildManager reference if not assigned
        if (buildManager == null)
        {
            buildManager = FindObjectOfType<BuildManager>();
        }
                // Sync status with BuildManager
        isWaterAvailable = buildManager.iswateractive;
        isElectricityAvailable = buildManager.iselecticitiesactive;

        // Update the UI based on the current status
        UpdateResourceStatus();
    }


    private void UpdateResourceStatus()
    {
        // Update water status image color
        if (waterStatusImage != null)
        {
            waterStatusImage.color = isWaterAvailable ? waterActiveColor : waterInactiveColor;
        }

        // Update electricity status image color
        if (electricityStatusImage != null)
        {
            electricityStatusImage.color = isElectricityAvailable ? electricityActiveColor : electricityInactiveColor;
        }
    }
}
