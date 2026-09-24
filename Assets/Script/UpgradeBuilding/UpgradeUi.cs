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
    public Image requiredSpecialistIcon;

    private void Awake()
    {
        if (buildManager == null)
        {
            buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();
        }

        if (timeManager == null)
        {
            timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        }

        if (uImanger == null)
        {
            uImanger = FindFirstObjectByType<UImanger>();
        }

        if (npcManager == null)
        {
            npcManager = GameManager.Instance != null ? GameManager.Instance.npcManager : FindFirstObjectByType<NpcManager>();
        }
    }

    private void Start()
    {
        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
        }
    }

    public void Initialize(UpgradeBuilding upgradeBuilding)
    {
        currentBuildingScript = upgradeBuilding;
        SetDataUpgrade();
    }

    public void SetDataUpgrade()
    {
        if (currentBuildingScript == null) return;

        int nextLevelIndex = currentBuildingScript.currentLevel - 1;

        if (nextLevelIndex >= currentBuildingScript.upgradeLevels.Count)
        {
            Debug.Log("No further upgrades available.");
            return;
        }

        UpgradeLevel nextLevel = currentBuildingScript.upgradeLevels[nextLevelIndex];

        if (textPlankCost != null) textPlankCost.text = nextLevel.plankCost.ToString();
        if (textSteelCost != null) textSteelCost.text = nextLevel.steelCost.ToString();
        if (textNpcCost != null) textNpcCost.text = nextLevel.npcCost.ToString();
        if (textDayCost != null) textDayCost.text = nextLevel.dayCost.ToString();

        if (image != null) image.sprite = nextLevel.levelSprite;
        if (WaterImage != null) WaterImage.SetActive(nextLevel.isneedwater);
        if (ElectricityImage != null) ElectricityImage.SetActive(nextLevel.isneedElecticities);

        // Update the UI for required specialist
        if (nextLevel.isNeedSpecialist)
        {
            if (requiredSpecialistText != null)
            {
                requiredSpecialistText.text = nextLevel.requiredSpecialist.ToString();
                requiredSpecialistText.gameObject.SetActive(true);
            }

            Sprite specialistSprite = GetSpecialistIcon(nextLevel.requiredSpecialist);
            if (requiredSpecialistIcon != null)
            {
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
        }
        else
        {
            if (requiredSpecialistText != null) requiredSpecialistText.gameObject.SetActive(false);
            if (requiredSpecialistIcon != null) requiredSpecialistIcon.gameObject.SetActive(false);
        }
    }

    private bool AreUpgradeConditionsMet()
    {
        if (currentBuildingScript == null) return false;

        int nextLevelIndex = currentBuildingScript.currentLevel - 1;
        if (nextLevelIndex < 0 || nextLevelIndex >= currentBuildingScript.upgradeLevels.Count) return false;

        UpgradeLevel nextLevel = currentBuildingScript.upgradeLevels[nextLevelIndex];

        if (nextLevel.isneedwater && (buildManager == null || !buildManager.iswateractive))
        {
            Debug.Log("Water is required but not active.");
            return false;
        }

        if (nextLevel.isneedElecticities && (buildManager == null || !buildManager.iselecticitiesactive))
        {
            Debug.Log("Electricity is required but not active.");
            return false;
        }

        if (nextLevel.isNeedSpecialist && !HasRequiredSpecialist(nextLevel.requiredSpecialist))
        {
            Debug.Log($"A {nextLevel.requiredSpecialist} specialist is required but not available.");
            return false;
        }

        return true;
    }

    private Sprite GetSpecialistIcon(SpecialistRoleNpc role)
    {
        SpecialistIcon specialistIcon = specialistIcons.Find(icon => icon.role == role);
        return specialistIcon != null ? specialistIcon.icon : null;
    }

    private void AssignSpecialistToUpgrade(SpecialistRoleNpc requiredSpecialist)
    {
        if (npcManager == null || currentBuildingScript == null) return;

        NpcClass specialistNpc = npcManager.GetNpcByClass(requiredSpecialist);
        if (specialistNpc != null)
        {
            specialistNpc.isWorking = true;
            currentBuildingScript.assignedSpecialistNpc = specialistNpc;
        }
    }

    private bool HasRequiredSpecialist(SpecialistRoleNpc requiredSpecialist)
    {
        if (npcManager == null) return false;
        return npcManager.GetNpcByClass(requiredSpecialist) != null;
    }

    private bool AreResourcesSufficient()
    {
        if (currentBuildingScript == null || buildManager == null) return false;

        int nextLevelIndex = currentBuildingScript.currentLevel - 1;
        if (nextLevelIndex < 0 || nextLevelIndex >= currentBuildingScript.upgradeLevels.Count) return false;

        UpgradeLevel nextLevel = currentBuildingScript.upgradeLevels[nextLevelIndex];

        return buildManager.steel >= nextLevel.steelCost &&
               buildManager.plank >= nextLevel.plankCost &&
               buildManager.npc >= nextLevel.npcCost;
    }

    public void ConfirmUpgrade()
    {
        if (currentBuildingScript == null) return;

        if (AreUpgradeConditionsMet())
        {
            if (AreResourcesSufficient())
            {
                int nextLevelIndex = currentBuildingScript.currentLevel - 1;
                UpgradeLevel nextLevel = currentBuildingScript.upgradeLevels[nextLevelIndex];

                // Subtract resources
                buildManager.steel -= nextLevel.steelCost;
                buildManager.plank -= nextLevel.plankCost;
                buildManager.npc -= nextLevel.npcCost;

                // Conditionally assign the specialist NPC
                if (nextLevel.isNeedSpecialist)
                {
                    AssignSpecialistToUpgrade(nextLevel.requiredSpecialist);
                }

                currentBuildingScript.isUpgradBuilding = true;

                if (dateTime == null && timeManager != null)
                {
                    dateTime = timeManager.dateTime;
                }

                int currentDay = dateTime != null ? dateTime.day : 0;
                currentBuildingScript.finishDayBuildingUpgradTime = currentDay + nextLevel.dayCost;

                // Set construction sprite immediately
                if (currentBuildingScript.spriteRenderer != null && currentBuildingScript.ConstructSprite != null)
                {
                    currentBuildingScript.spriteRenderer.sprite = currentBuildingScript.ConstructSprite;
                }

                gameObject.SetActive(false);
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