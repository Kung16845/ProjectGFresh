using UnityEngine;

public class CarWorkshop : BaseBuilding
{
    private UImanger uImanger;
    private UpgradeUi upgradeUi;

    public int StandardFuelCost => upgradeBuilding.currentLevel >= upgradeBuilding.maxLevel ? 3 : 5;

    protected override void Start()
    {
        base.Start();
        uImanger = FindObjectOfType<UImanger>();
    }

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

    void OnMouseDown()
    {
        if (building.isfinsih && !upgradeBuilding.isUpgradBuilding)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.CarWorkshopUI);
            if (upgradeBuilding.currentLevel == upgradeBuilding.maxLevel)
            {
                uImanger.DisableUIPanel(UImanger.UIPanel.CarUpgradeWorkshopUI);
            }
        }
    }

    public void AssignUpgradeData()
    {
        upgradeUi = FindObjectOfType<UpgradeUi>();
        upgradeUi.Initialize(upgradeBuilding);
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
