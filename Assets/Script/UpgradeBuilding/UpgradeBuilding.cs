using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradeBuilding : MonoBehaviour
{
    [Header("Building Info")]
    public string nameBuild;
    public string detailBuild;

    [Header("Upgrade Levels")]
    public List<UpgradeLevel> upgradeLevels = new List<UpgradeLevel>();
    public int currentLevel = 1;
    public int maxLevel;

    [Header("Upgrade State")]
    public bool isUpgradBuilding;
    public bool isFinishedUpgrad;
    public int finishDayBuildingUpgradTime;

    [Header("Components & Visuals")]
    public UpgradeUi upgradeUi;
    public TimeManager timeManager;
    public DateTime dateTime;
    public SpriteRenderer spriteRenderer;
    public Sprite ConstructSprite;
    public BuildManager buildManager;
    public Building building;
    public UImanger uImanger;
    public NpcClass assignedSpecialistNpc;
    public BuiltBuildingInfo builtBuildingInfo;

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            timeManager = GameManager.Instance.timeManager;
            buildManager = GameManager.Instance.buildManager;
        }

        if (timeManager == null)
        {
            timeManager = FindFirstObjectByType<TimeManager>();
        }

        if (buildManager == null)
        {
            buildManager = FindFirstObjectByType<BuildManager>();
        }

        uImanger = FindFirstObjectByType<UImanger>();
        building = GetComponent<Building>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
        }

        isUpgradBuilding = false;
        isFinishedUpgrad = false;
        maxLevel = upgradeLevels.Count + 1;
    }

    private void Start()
    {
        if (buildManager != null && buildManager.builtBuildings != null)
        {
            GameObject currentObj = gameObject;
            for (int i = 0; i < buildManager.builtBuildings.Count; i++)
            {
                BuiltBuildingInfo builtBuilding = buildManager.builtBuildings[i];
                if (builtBuilding != null && builtBuilding.building != null && builtBuilding.building.gameObject == currentObj)
                {
                    builtBuildingInfo = builtBuilding;
                    break;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (isUpgradBuilding)
        {
            WaitUpgrade();
        }
    }

    public void OnMouseDown()
    {
        if (building != null && building.isfinsih && !isUpgradBuilding && currentLevel < maxLevel)
        {
            // Debug.Log("showupgradeUI");
        }
    }

    public void WaitUpgrade()
    {
        if (dateTime == null)
        {
            if (timeManager != null) dateTime = timeManager.dateTime;
            if (dateTime == null) return;
        }

        if (dateTime.day >= finishDayBuildingUpgradTime && isUpgradBuilding)
        {
            int levelIndex = currentLevel - 1;
            if (levelIndex >= 0 && levelIndex < upgradeLevels.Count)
            {
                UpgradeLevel completedLevel = upgradeLevels[levelIndex];

                if (buildManager != null)
                {
                    buildManager.npc += completedLevel.npcCost;
                }

                if (spriteRenderer != null && completedLevel.levelSprite != null)
                {
                    spriteRenderer.sprite = completedLevel.levelSprite;
                }
            }

            isUpgradBuilding = false;
            currentLevel++;

            if (builtBuildingInfo != null)
            {
                builtBuildingInfo.level = currentLevel;
            }

            if (currentLevel >= maxLevel)
            {
                isFinishedUpgrad = true;
            }

            // Return the assigned specialist NPC to available
            if (assignedSpecialistNpc != null && GameManager.Instance != null && GameManager.Instance.npcManager != null)
            {
                NpcClass npc = GameManager.Instance.npcManager.GetNpcById(assignedSpecialistNpc.idnpc);
                if (npc != null)
                {
                    npc.isWorking = false;
                }
                assignedSpecialistNpc = null;
            }

            Debug.Log($"Upgraded to Level {currentLevel}");
        }
        else if (dateTime.day < finishDayBuildingUpgradTime && isUpgradBuilding)
        {
            // Only set ConstructSprite once, avoid per-frame assignment
            if (spriteRenderer != null && ConstructSprite != null && spriteRenderer.sprite != ConstructSprite)
            {
                spriteRenderer.sprite = ConstructSprite;
            }
        }
    }
}

[System.Serializable]
public class UpgradeLevel
{
    public int levelNumber;
    public int steelCost;
    public int plankCost;
    public int npcCost;
    public int dayCost;
    public Sprite levelSprite;
    public bool isneedwater;
    public bool isneedElecticities;
    public bool isNeedSpecialist;
    public SpecialistRoleNpc requiredSpecialist;
}
