using UnityEngine;

public class CarWorkshop : BaseBuilding
{
    public int StandardFuelCost => upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel ? 3 : 5;

    protected override BuildingContribution GetContributionForLevel(int level)
    {
        switch (level)
        {
            case 2:
                return new BuildingContribution { availableCars = 2 };
            default:
                return new BuildingContribution { availableCars = 1 };
        }
    }

    public void AddCartoGlobalstat()
    {
        Globalstat g = GameManager.Instance.globalstat;
        g.availablecar += 1;
        BuildManager.Instance.fuel -= StandardFuelCost;
        g.UnaviableCar -= 1;
    }

    public void DecreaseCartoGlobalstat()
    {
        GameManager.Instance.globalstat.availablecar -= 1;
    }
}
