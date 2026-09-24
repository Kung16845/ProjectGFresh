using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWorkshop : MonoBehaviour
{
    public int availableCar;
    public int UnaviableCar;
    public float FuelRefillRate;
    public TimeManager timeManager;
    public DateTime dateTime;
    public BuildManager buildManager;
    public int currentDay;
    public Building building;
    public UpgradeUi upgradeUi;
    public UpgradeBuilding upgradeBuilding;
    public UImanger uImanger;
    public Globalstat globalstat;
    private int StandardFuelcost = 5;
    public bool hasUpgradeApplied;

    private void Start()
    {
        uImanger = FindFirstObjectByType<UImanger>();
        timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        globalstat = GameManager.Instance != null ? GameManager.Instance.globalstat : FindFirstObjectByType<Globalstat>();
        buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();

        // Always get components directly on this GameObject
        building = GetComponent<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();

        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
            if (dateTime != null) currentDay = dateTime.day;
        }

        availableCar = 1;
        FuelRefillRate = 1;
    }

    private void OnMouseDown()
    {
        if (building != null && building.isfinsih && upgradeBuilding != null && !upgradeBuilding.isUpgradBuilding)
        {
            if (uImanger != null)
            {
                uImanger.ToggleUIPanel(UImanger.UIPanel.CarWorkshopUI);
                
                if (upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel && !hasUpgradeApplied)
                {
                    uImanger.DisableUIPanel(UImanger.UIPanel.CarUpgradeWorkshopUI);
                    if (globalstat != null)
                    {
                        globalstat.availablecar += 1;
                    }
                    StandardFuelcost = 3;
                    hasUpgradeApplied = true;
                }
            }
        }
    }

    public void AssignUpgradeData()
    {
        if (upgradeUi == null)
        {
            upgradeUi = FindFirstObjectByType<UpgradeUi>();
        }
        if (upgradeUi != null && upgradeBuilding != null)
        {
            upgradeUi.Initialize(upgradeBuilding);
        }
    }

    public void AddCartoGlobalstat()
    {
        if (globalstat != null)
        {
            globalstat.availablecar += 1;
            globalstat.UnaviableCar -= 1;
        }
        if (buildManager != null)
        {
            buildManager.fuel -= StandardFuelcost;
        }
    }

    public void DecreaseCartoGlobalstat()
    {
        if (globalstat != null)
        {
            globalstat.availablecar -= 1;
        }
    }
}