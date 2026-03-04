using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Building/BuildingData")]
public class BuildingData : ScriptableObject
{
    [Header("Identity")]
    public int id;
    public string buildingName;

    [Header("Slot")]
    public BuildingType slotSize;

    [Header("Construction Cost")]
    public int steelCost;
    public int plankCost;
    public int workerPointsRequired;
    public int buildTimeHours;

    [Header("Specialist")]
    public bool requiresSpecialist;
    public SpecialistRoleNpc requiredSpecialist;

    [Header("Upgrades")]
    public List<BuildingUpgradeData> upgradeLevels;

    public int MaxLevel => upgradeLevels.Count + 1;

    public BuildingUpgradeData GetUpgradeData(int toLevel)
    {
        int index = toLevel - 2; // level 2 = index 0
        if (index >= 0 && index < upgradeLevels.Count)
            return upgradeLevels[index];
        return null;
    }
}

[Serializable]
public class BuildingUpgradeData
{
    [Header("Cost")]
    public int steelCost;
    public int plankCost;
    public int workerPointsRequired;
    public int buildTimeHours;

    [Header("Requirements")]
    public bool requiresWater;
    public bool requiresElectricity;
    public bool requiresSpecialist;
    public SpecialistRoleNpc requiredSpecialist;

    [Header("Visual")]
    public Sprite levelSprite;
}
