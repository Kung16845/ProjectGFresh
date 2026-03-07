using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BuildingType
{
    Large,
    Medium,
    Small
}
public class Building : MonoBehaviour
{
    [Header("Data Source")]
    public BuildingData buildingData;

    [Header("Identity")]
    public string nameBuild;
    public string detailBuild;

    [Header("Cost")]
    public int steelCost;
    public int plankCost;
    public int workerPointsCost;
    public int buildTimeHours;
    public BuildingType buildingType;

    [Header("Specialist")]
    public bool requiresSpecialist;
    public SpecialistRoleNpc requiredSpecialist;

    [Header("Runtime State")]
    public int finishBuildingHour;
    public bool isBuilding;
    public bool isFinished;

    [Header("References")]
    public TimeManager timeManager;
    public DateTime dateTime;
    public SpriteRenderer spriteRenderer;
    public Sprite OriginalSprite;
    public Sprite ConstructSprite;
    public BuildManager buildManager;

    // Keep old name as property for code that still reads it
    public bool isfinsih
    {
        get => isFinished;
        set => isFinished = value;
    }

    private void Awake()
    {
        // If BuildingData is assigned, populate fields from it
        if (buildingData != null)
        {
            nameBuild = buildingData.buildingName;
            steelCost = buildingData.steelCost;
            plankCost = buildingData.plankCost;
            workerPointsCost = buildingData.workerPointsRequired;
            buildTimeHours = buildingData.buildTimeHours;
            buildingType = buildingData.slotSize;
            requiresSpecialist = buildingData.requiresSpecialist;
            requiredSpecialist = buildingData.requiredSpecialist;
        }

        timeManager = GameManager.Instance.timeManager;
        buildManager = GameManager.Instance.buildManager;
        dateTime = timeManager.dateTime;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        isBuilding = true;
        isFinished = false;
    }

    private void Update()
    {
        WaitBuilding();
    }

    public void WaitBuilding()
    {
        if (isFinished) return;

        if (dateTime.TotalHours >= finishBuildingHour && isBuilding)
        {
            isBuilding = false;
            isFinished = true;
            spriteRenderer.sprite = OriginalSprite;
        }
        else if (isBuilding)
        {
            spriteRenderer.sprite = ConstructSprite;
        }
    }
}
