using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UpgradeBuilding : MonoBehaviour
{
    public int currentLevel = 1;
    public int maxLevel;

    public bool isUpgradBuilding;
    public bool isFinishedUpgrad;

    public UpgradeUi upgradeUi;
    public Sprite ConstructSprite;
    public Building building;
    public UImanger uImanger;
    public NpcClass assignedSpecialistNpc;
    public BuiltBuildingInfo builtBuildingInfo;

    public int finishUpgradeHour;

    private BuildingData BuildingData => building.buildingData;

    void Awake()
    {
        uImanger = FindObjectOfType<UImanger>();
        building = GetComponent<Building>();
        isUpgradBuilding = false;
        isFinishedUpgrad = false;
        maxLevel = BuildingData != null ? BuildingData.MaxLevel : 1;
    }
    void Start()
    {
        WaitUpgrade();

        foreach (var builtBuilding in building.buildManager.builtBuildings)
        {
            if (builtBuilding.building == this.gameObject)
            {
                builtBuildingInfo = builtBuilding;
                break;
            }
        }
    }
    void LateUpdate()
    {
        if (isUpgradBuilding)
        {
            WaitUpgrade();
        }
    }

    void OnMouseDown()
    {
        // Skip if the active building script handles its own click (e.g. Garden, Waterpump, Solar)
        BaseBuilding activeBuilding = GetComponent<BaseBuilding>();
        if (activeBuilding != null && activeBuilding.HasOwnClickHandler) return;

        if (building.isfinsih && !isUpgradBuilding && currentLevel < maxLevel)
        {
            if (upgradeUi == null)
                upgradeUi = FindObjectOfType<UpgradeUi>(true);
            if (upgradeUi != null)
            {
                upgradeUi.gameObject.SetActive(true);
                upgradeUi.Initialize(this);
            }
        }
    }

    void WaitUpgrade()
    {
        if (building.dateTime.TotalHours >= finishUpgradeHour && isUpgradBuilding)
        {
            BuildingUpgradeData completedLevel = BuildingData.GetUpgradeData(currentLevel + 1);
            if (completedLevel == null) return;

            building.buildManager.npc += completedLevel.workerPointsRequired;
            building.spriteRenderer.sprite = completedLevel.levelSprite;
            isUpgradBuilding = false;
            currentLevel++;

            // Update the level in builtBuildingInfo
            if (builtBuildingInfo != null)
            {
                builtBuildingInfo.level = currentLevel;
            }

            if (currentLevel == maxLevel)
            {
                isFinishedUpgrad = true;
            }

            // Return the assigned specialist NPC to the available list
            if (assignedSpecialistNpc != null)
            {
                NpcManager npcManager = GameManager.Instance.npcManager;
                NpcClass npc = npcManager.GetNpcById(assignedSpecialistNpc.idnpc);
                if (npc != null) npc.isWorking = false;
                assignedSpecialistNpc = null;
            }

            Debug.Log("Upgraded to Level " + currentLevel);
        }
        else if (building.dateTime.TotalHours < finishUpgradeHour)
        {
            building.spriteRenderer.sprite = ConstructSprite;
        }
    }
}
