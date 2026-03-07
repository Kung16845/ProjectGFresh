using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUi : MonoBehaviour
{
    public UpgradeBuilding currentBuildingScript;
    public BuildManager buildManager;
    public DateTime dateTime;
    public TimeManager timeManager;
    public UImanger uImanger;
    public Image image;
    public GameObject WaterImage;
    public GameObject ElectricityImage;
    public TextMeshProUGUI textNameBuild;
    public TextMeshProUGUI textDescribeBuild;
    public TextMeshProUGUI textPlankCost;
    public TextMeshProUGUI textSteelCost;
    public TextMeshProUGUI textNpcCost;
    public TextMeshProUGUI textDayCost;
    public TextMeshProUGUI requiredSpecialistText;
    public NpcManager npcManager;
    public List<SpecialistIcon> specialistIcons = new List<SpecialistIcon>();

    // Add an Image component to display the specialist icon
    public Image requiredSpecialistIcon;
    void Awake()
    {
        buildManager = FindObjectOfType<BuildManager>();
        timeManager = FindObjectOfType<TimeManager>();
        uImanger = FindObjectOfType<UImanger>();
        npcManager = FindObjectOfType<NpcManager>();
    }

    void Start()
    {
        dateTime = timeManager.dateTime;
    }

    public void Initialize(UpgradeBuilding upgradeBuilding)
    {
        currentBuildingScript = upgradeBuilding;
        SetDataUpgrade();
    }

    private BuildingUpgradeData GetNextUpgradeData()
    {
        int nextLevel = currentBuildingScript.currentLevel + 1;
        return currentBuildingScript.building.buildingData.GetUpgradeData(nextLevel);
    }

   public void SetDataUpgrade()
    {
        BuildingUpgradeData nextLevel = GetNextUpgradeData();

        if (nextLevel == null)
        {
            Debug.Log("No further upgrades available.");
            return;
        }

        textPlankCost.text = nextLevel.plankCost.ToString();
        textSteelCost.text = nextLevel.steelCost.ToString();
        textNpcCost.text = nextLevel.workerPointsRequired.ToString();
        textDayCost.text = nextLevel.buildTimeHours.ToString();
        image.sprite = nextLevel.levelSprite;
        WaterImage.SetActive(nextLevel.requiresWater);
        ElectricityImage.SetActive(nextLevel.requiresElectricity);

        // Update the UI for required specialist
        if (nextLevel.requiresSpecialist)
        {
            requiredSpecialistText.text = nextLevel.requiredSpecialist.ToString();
            requiredSpecialistText.gameObject.SetActive(true);

            // Set the specialist icon
            Sprite specialistSprite = GetSpecialistIcon(nextLevel.requiredSpecialist);
            if (specialistSprite != null)
            {
                requiredSpecialistIcon.sprite = specialistSprite;
                requiredSpecialistIcon.gameObject.SetActive(true);
            }
            else
            {
                requiredSpecialistIcon.gameObject.SetActive(false);
                Debug.LogWarning($"No icon found for specialist: {nextLevel.requiredSpecialist}");
            }
        }
        else
        {
            requiredSpecialistText.gameObject.SetActive(false);
            requiredSpecialistIcon.gameObject.SetActive(false);
        }
    }

     private bool AreUpgradeConditionsMet()
    {
        BuildingUpgradeData nextLevel = GetNextUpgradeData();
        if (nextLevel == null)
        {
            Debug.Log("Already at max level.");
            return false;
        }

        var conditions = new List<(bool condition, string failMessage)>
        {
            (!nextLevel.requiresWater || buildManager.iswateractive, "Water is required but not active."),
            (!nextLevel.requiresElectricity || buildManager.iselecticitiesactive, "Electricity is required but not active."),
            (!nextLevel.requiresSpecialist || HasRequiredSpecialist(nextLevel.requiredSpecialist), $"A {nextLevel.requiredSpecialist} specialist is required but not available."),
        };

        foreach (var (condition, failMessage) in conditions)
        {
            if (!condition)
            {
                Debug.Log(failMessage);
                return false;
            }
        }

        return true;
    }
    private Sprite GetSpecialistIcon(SpecialistRoleNpc role)
    {
        SpecialistIcon specialistIcon = specialistIcons.Find(icon => icon.role == role);
        if (specialistIcon != null)
        {
            return specialistIcon.icon;
        }
        return null;
    }
    private void AssignSpecialistToUpgrade(SpecialistRoleNpc requiredSpecialist)
    {
        // Find the NPC with the required specialist role
        NpcClass specialistNpc = npcManager.GetNpcByClass(requiredSpecialist);

        if (specialistNpc != null)
        {
            specialistNpc.isWorking = true;
            // Store a reference to the NPC in the building
            currentBuildingScript.assignedSpecialistNpc = specialistNpc;
        }
    }
    private bool HasRequiredSpecialist(SpecialistRoleNpc requiredSpecialist)
    {
        // Check if there is at least one NPC with the required specialist role
        return npcManager.GetNpcByClass(requiredSpecialist) != null;
    }
    private bool AreResourcesSufficient()
    {
        BuildingUpgradeData nextLevel = GetNextUpgradeData();
        if (nextLevel == null)
            return false;

        return buildManager.steel >= nextLevel.steelCost &&
               buildManager.plank >= nextLevel.plankCost &&
               buildManager.npc >= nextLevel.workerPointsRequired;
    }

    public void ConfirmUpgrade()
    {
        if (AreUpgradeConditionsMet())
        {
            if (AreResourcesSufficient())
            {
                BuildingUpgradeData nextLevel = GetNextUpgradeData();
                if (nextLevel == null) return;

                // Subtract resources
                buildManager.steel -= nextLevel.steelCost;
                buildManager.plank -= nextLevel.plankCost;
                buildManager.npc -= nextLevel.workerPointsRequired;

                // Conditionally assign the specialist NPC to the upgrade task
                if (nextLevel.requiresSpecialist)
                {
                    AssignSpecialistToUpgrade(nextLevel.requiredSpecialist);
                }

                currentBuildingScript.isUpgradBuilding = true;
                currentBuildingScript.finishUpgradeHour = dateTime.TotalHours + nextLevel.buildTimeHours;

                this.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Not enough resources to upgrade.");
            }
        }
    }

}
[System.Serializable]
public class SpecialistIcon
{
    public SpecialistRoleNpc role;
    public Sprite icon;
}
