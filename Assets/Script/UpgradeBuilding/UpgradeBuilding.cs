using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UpgradeBuilding : MonoBehaviour
{
    public string nameBuild;
    public string detailBuild;

    // Upgrade levels
    public List<UpgradeLevel> upgradeLevels = new List<UpgradeLevel>();

    public int currentLevel = 1;
    public int maxLevel;

    public bool isUpgradBuilding;
    public bool isFinishedUpgrad;

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

    public int finishDayBuildingUpgradTime;

    void Awake()
    {
        uImanger = FindObjectOfType<UImanger>();
        timeManager = GameManager.Instance.timeManager;
        buildManager = GameManager.Instance.buildManager;
        dateTime = timeManager.dateTime;
        building = GetComponent<Building>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isUpgradBuilding = false;
        isFinishedUpgrad = false;
        maxLevel = upgradeLevels.Count+1;
        WaitUpgrade();
    }
    void Start()
    {
        foreach (var builtBuilding in buildManager.builtBuildings)
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
        if (building.isfinsih && !isUpgradBuilding && currentLevel < maxLevel)
        {
            Debug.Log("showupgradeUI");
            // uImanger.ActiveUpgradeUI();
            // upgradeUi = FindObjectOfType<UpgradeUi>();
            // upgradeUi.Initialize(this);
        }
    }

    void WaitUpgrade()
    {
        if (dateTime.day >= finishDayBuildingUpgradTime && isUpgradBuilding)
        {
            UpgradeLevel completedLevel = upgradeLevels[currentLevel - 1];

            buildManager.npc += completedLevel.npcCost;
            spriteRenderer.sprite = completedLevel.levelSprite;
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
                npc.isWorking = false; 
                assignedSpecialistNpc = null;
            }

            Debug.Log("Upgraded to Level " + currentLevel);
        }
        else if (dateTime.day < finishDayBuildingUpgradTime)
        {
            spriteRenderer.sprite = ConstructSprite;
        }
    }
    // public void DisableCollider()
    // {
    //     Collider2D collider = GetComponent<Collider2D>();
    //     if (collider != null)
    //     {
    //         collider.enabled = false;
    //     }
    // }

    // public void EnableCollider()
    // {
    //     Collider2D collider = GetComponent<Collider2D>();
    //     if (collider != null)
    //     {
    //         collider.enabled = true;
    //     }
    // }
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

    // Add this line to include the required specialist role
    public bool isNeedSpecialist;
    public SpecialistRoleNpc requiredSpecialist;
}
