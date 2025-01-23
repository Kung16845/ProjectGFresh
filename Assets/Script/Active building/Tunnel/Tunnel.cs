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
    public TextMeshProUGUI tunnelStatusText; // Combined status and hint

    private int lastRewardDay = -1;

    void OnMouseDown()
    {
        uImanger.ToggleUIPanel(UImanger.UIPanel.TunnelUI);

        UpdateTunnelStatus();
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        dateTime = timeManager.dateTime;
    }

    void Update()
    {
        WaitClearingRock();
        CheckMaintenance();
        GiveDailyReward();
    }

    public void WaitClearingRock()
    {
        if (dateTime.day >= finishDayBuildingTime && isclearing)
        {
            isclearing = false;
            tuneelisopen = true;
            globalstat.Tunnelaviable = true;
            buildManager.npc += npcCost;
            return;
        }
    }

    public void InitializeOpenGateway()
    {
        isclearing = true;
        ItemData itemDataToRemove = new ItemData
        {
            idItem = 1020304,
            count = 5
        };
        inventoryItemPresent.RemoveItem(itemDataToRemove);
        dateTime = timeManager.dateTime;
        finishDayBuildingTime += dateTime.day + daycost;
        buildManager.npc -= npcCost;
        uImanger.DisableUIPanel(UImanger.UIPanel.ClearingTunnelUI);
        uImanger.DisableUIPanel(UImanger.UIPanel.TunnelUI);
    }

    void CheckMaintenance()
    {
        if (tuneelisopen && !buildManager.iswateractive)
        {
            spriteRenderer.enabled = false;
            tuneelisopen = false;
            globalstat.Tunnelaviable = false;
        }
        else if (tuneelisopen)
        {
            spriteRenderer.enabled = false;
            tuneelisopen = true;
            globalstat.Tunnelaviable = true;
        }
    }

    private List<ItemReward> shuffledRewardsPool = new List<ItemReward>();

    public void GiveDailyReward()
    {
        // Do not give daily rewards if water is unavailable
        if (!buildManager.iswateractive)
        {
            return;
        }

        if (tuneelisopen && dateTime.day != lastRewardDay)
        {
            lastRewardDay = dateTime.day;

            // Shuffle the rewards pool daily
            ShuffleRewardsPool();

            // Create a list to store eligible rewards based on chance
            List<ItemReward> validRewards = new List<ItemReward>();
            foreach (var reward in shuffledRewardsPool)
            {
                if (Random.value <= reward.chance)
                {
                    validRewards.Add(reward);
                }
            }

            // Limit to a maximum of 3 unique items
            int maxItemsToGive = Mathf.Min(3, validRewards.Count);
            for (int i = 0; i < maxItemsToGive; i++)
            {
                ItemReward chosenReward = validRewards[i];
                int randomAmount = Random.Range(chosenReward.minAmount, chosenReward.maxAmount + 1);

                dailyGive.AddItemByID(chosenReward.itemID, randomAmount);
            }
        }
    }

    void ShuffleRewardsPool()
    {
        shuffledRewardsPool.Clear();
        shuffledRewardsPool.AddRange(rewardsPool);

        // Shuffle using Fisher-Yates algorithm
        for (int i = shuffledRewardsPool.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            ItemReward temp = shuffledRewardsPool[i];
            shuffledRewardsPool[i] = shuffledRewardsPool[randomIndex];
            shuffledRewardsPool[randomIndex] = temp;
        }
    }
    private void UpdateTunnelStatus()
    {
        if (!tuneelisopen && !isclearing)
        {
            int currentDynamite = inventoryItemPresent.GetItemCountByID(1020304);
            int requiredDynamite = 5;
            tunnelStatusText.text = $"The tunnel is blocked. If we clear it, we might find something useful. Rumor has it the military left supplies here. <color=#FFFF00>Dynamite collected: {currentDynamite}/{requiredDynamite}</color>";
        }
        else if (isclearing && !buildManager.iswateractive)
        {
            tunnelStatusText.text = "The tunnel is being cleared, but it's flooded. This might take longer than expected.";
        }
        else if (isclearing && buildManager.iswateractive)
        {
            tunnelStatusText.text = "The tunnel is being cleared, and the water is being drained.";
        }
        else if (tuneelisopen && !buildManager.iswateractive)
        {
            tunnelStatusText.text = "The tunnel is open, but it's still flooded with water. We can't explore for supplies yet.";
        }
        else if (tuneelisopen && buildManager.iswateractive)
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
