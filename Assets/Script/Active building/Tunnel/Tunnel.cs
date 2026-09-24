using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tunnel : MonoBehaviour
{
    public bool tuneelisopen;
    public TimeManager timeManager;
    public DailyGive dailyGive;
    public DateTime dateTime;
    public int currentDay;
    public UImanger uImanger;
    public BuildManager buildManager;
    public Globalstat globalstat;
    public InventoryItemPresent inventoryItemPresent;
    public List<ItemReward> rewardsPool;
    public SpriteRenderer spriteRenderer;
    private int daycost = 1;
    public bool isclearing;
    private int npcCost = 1;
    public int finishDayBuildingTime = 0;
    public TextMeshProUGUI tunnelStatusText;

    private int lastRewardDay = -1;
    private List<ItemReward> shuffledRewardsPool = new List<ItemReward>();

    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (timeManager == null) timeManager = GameManager.Instance != null ? GameManager.Instance.timeManager : FindFirstObjectByType<TimeManager>();
        if (dailyGive == null) dailyGive = FindFirstObjectByType<DailyGive>();
        if (uImanger == null) uImanger = FindFirstObjectByType<UImanger>();
        if (buildManager == null) buildManager = GameManager.Instance != null ? GameManager.Instance.buildManager : FindFirstObjectByType<BuildManager>();
        if (globalstat == null) globalstat = GameManager.Instance != null ? GameManager.Instance.globalstat : FindFirstObjectByType<Globalstat>();
        if (inventoryItemPresent == null) inventoryItemPresent = FindFirstObjectByType<InventoryItemPresent>();
    }

    private void Start()
    {
        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
            if (dateTime != null) currentDay = dateTime.day;
        }
    }

    private void OnMouseDown()
    {
        if (uImanger != null)
        {
            uImanger.ToggleUIPanel(UImanger.UIPanel.TunnelUI);
        }

        UpdateTunnelStatus();
    }

    private void Update()
    {
        if (dateTime == null && timeManager != null)
        {
            dateTime = timeManager.dateTime;
        }

        WaitClearingRock();
        CheckMaintenance();
        GiveDailyReward();
    }

    public void WaitClearingRock()
    {
        if (dateTime != null && dateTime.day >= finishDayBuildingTime && isclearing)
        {
            isclearing = false;
            tuneelisopen = true;
            if (globalstat != null) globalstat.Tunnelaviable = true;
            if (buildManager != null) buildManager.npc += npcCost;
        }
    }

    public void InitializeOpenGateway()
    {
        isclearing = true;

        if (inventoryItemPresent != null)
        {
            inventoryItemPresent.RemoveItem(new ItemData { idItem = 1020304, count = 5 });
        }

        if (timeManager != null)
        {
            dateTime = timeManager.dateTime;
        }

        int currentDayVal = dateTime != null ? dateTime.day : 0;
        finishDayBuildingTime = currentDayVal + daycost;

        if (buildManager != null)
        {
            buildManager.npc -= npcCost;
        }

        if (uImanger != null)
        {
            uImanger.DisableUIPanel(UImanger.UIPanel.ClearingTunnelUI);
            uImanger.DisableUIPanel(UImanger.UIPanel.TunnelUI);
        }
    }

    private void CheckMaintenance()
    {
        if (buildManager == null || globalstat == null) return;

        if (tuneelisopen && !buildManager.iswateractive)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            tuneelisopen = false;
            globalstat.Tunnelaviable = false;
        }
        else if (tuneelisopen)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            tuneelisopen = true;
            globalstat.Tunnelaviable = true;
        }
    }

    public void GiveDailyReward()
    {
        if (buildManager == null || !buildManager.iswateractive || dailyGive == null)
        {
            return;
        }

        if (tuneelisopen && dateTime != null && dateTime.day != lastRewardDay)
        {
            lastRewardDay = dateTime.day;

            ShuffleRewardsPool();

            List<ItemReward> validRewards = new List<ItemReward>();
            for (int i = 0; i < shuffledRewardsPool.Count; i++)
            {
                ItemReward reward = shuffledRewardsPool[i];
                if (Random.value <= reward.chance)
                {
                    validRewards.Add(reward);
                }
            }

            int maxItemsToGive = Mathf.Min(3, validRewards.Count);
            for (int i = 0; i < maxItemsToGive; i++)
            {
                ItemReward chosenReward = validRewards[i];
                int randomAmount = Random.Range(chosenReward.minAmount, chosenReward.maxAmount + 1);
                dailyGive.AddItemByID(chosenReward.itemID, randomAmount);
            }
        }
    }

    private void ShuffleRewardsPool()
    {
        shuffledRewardsPool.Clear();
        if (rewardsPool != null)
        {
            shuffledRewardsPool.AddRange(rewardsPool);

            for (int i = shuffledRewardsPool.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                ItemReward temp = shuffledRewardsPool[i];
                shuffledRewardsPool[i] = shuffledRewardsPool[randomIndex];
                shuffledRewardsPool[randomIndex] = temp;
            }
        }
    }

    private void UpdateTunnelStatus()
    {
        if (tunnelStatusText == null) return;

        bool hasWater = buildManager != null && buildManager.iswateractive;

        if (!tuneelisopen && !isclearing)
        {
            int currentDynamite = inventoryItemPresent != null ? inventoryItemPresent.GetItemCountByID(1020304) : 0;
            int requiredDynamite = 5;
            tunnelStatusText.text = $"The tunnel is blocked. If we clear it, we might find something useful. Rumor has it the military left supplies here. <color=#FFFF00>Dynamite collected: {currentDynamite}/{requiredDynamite}</color>";
        }
        else if (isclearing && !hasWater)
        {
            tunnelStatusText.text = "The tunnel is being cleared, but it's flooded. This might take longer than expected.";
        }
        else if (isclearing && hasWater)
        {
            tunnelStatusText.text = "The tunnel is being cleared, and the water is being drained.";
        }
        else if (tuneelisopen && !hasWater)
        {
            tunnelStatusText.text = "The tunnel is open, but it's still flooded with water. We can't explore for supplies yet.";
        }
        else if (tuneelisopen && hasWater)
        {
            tunnelStatusText.text = "The tunnel is now open and clear! It's a safe route for expeditions and supplies are accessible.";
        }
        else
        {
            tunnelStatusText.text = string.Empty;
        }
    }
}

[System.Serializable]
public class ItemReward
{
    public int itemID;
    public float chance;
    public int minAmount;
    public int maxAmount;
}
