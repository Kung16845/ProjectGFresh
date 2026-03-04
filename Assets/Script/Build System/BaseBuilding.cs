using UnityEngine;

/// <summary>
/// Base class for all active buildings.
/// Handles stat contribution to Globalstat automatically on build completion and upgrade.
/// Subclasses only need to implement GetContributionForLevel().
/// </summary>
public abstract class BaseBuilding : MonoBehaviour
{
    public Building building { get; private set; }
    public UpgradeBuilding upgradeBuilding { get; private set; }
    protected Globalstat globalstat;

    private BuildingContribution appliedContribution;
    private int lastKnownLevel = -1;
    private bool contributionApplied = false;

    protected virtual void Start()
    {
        building = GetComponent<Building>();
        upgradeBuilding = GetComponent<UpgradeBuilding>();
        globalstat = GameManager.Instance.globalstat;
    }

    protected virtual void Update()
    {
        // Apply contribution once when construction finishes
        if (!contributionApplied && building.isfinsih)
        {
            lastKnownLevel = upgradeBuilding != null ? upgradeBuilding.currentLevel : 1;
            appliedContribution = GetContributionForLevel(lastKnownLevel);
            appliedContribution.ApplyTo(globalstat);
            contributionApplied = true;
        }

        // Update contribution when an upgrade completes (skip if no upgrade component)
        if (contributionApplied && upgradeBuilding != null && upgradeBuilding.currentLevel != lastKnownLevel)
        {
            BuildingContribution newContribution = GetContributionForLevel(upgradeBuilding.currentLevel);
            appliedContribution.RemoveFrom(globalstat);
            newContribution.ApplyTo(globalstat);
            appliedContribution = newContribution;
            lastKnownLevel = upgradeBuilding.currentLevel;
        }
    }

    /// <summary>
    /// Return the stat contribution for the given level.
    /// Only set the fields your building uses — leave the rest at 0.
    /// </summary>
    protected abstract BuildingContribution GetContributionForLevel(int level);
}
