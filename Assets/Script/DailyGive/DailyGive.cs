using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyGive : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject DailygiveUI; // The UI panel to display rewards
    public TextMeshProUGUI dailyGiveStatusText; // Text component to list rewards

    [Header("References")]
    public TimeManager timeManager;
    public InventoryItemPresent inventoryItemPresent;

    [Header("Daily Rewards")]
    public List<ItemData> listItemsTogiveDaily = new List<ItemData>();
    public List<UIItemData> listUIItemPrefab;
    public DailyGiveUI dailyGiveUIHandler;

    private int lastRewardDay = -1; // Tracks the last day rewards were given
     public List<ItemData> supplyDropItems = new List<ItemData>();
    public List<ItemData> listItemsToGiveDaily = new List<ItemData>();

    // Countdown Timer Variables
    private bool isSupplyDropActive = false;
    private int supplyDropCountdown;
    private SuuplyDropType currentSupplyDropType;
    public Button SupplyDropButton;


    void Start()
    {
        if (timeManager == null)
        {
            timeManager = FindObjectOfType<TimeManager>();
        }

        if (inventoryItemPresent == null)
        {
            inventoryItemPresent = InventoryItemPresent.Instance;
        }
    }

    void Update()
    {
        CheckTimeForDailyGive();
    }

    /// <summary>
    /// Checks if the time is 7 AM and it's a new day to show the daily reward UI.
    /// </summary>
    private void CheckTimeForDailyGive()
    {
        if (timeManager.dateTime.hour == 7 && timeManager.dateTime.day != lastRewardDay)
        {
            // Ensure this only triggers once per day at 7 AM
            lastRewardDay = timeManager.dateTime.day;
            Debug.Log("New day detected. Showing daily rewards.");

            // Call the function to show the daily rewards
            ShowDailyGiveUI();
        }
    }

    public void ProcessDailyRewards(List<ItemData> itemsToGive)
    {
        if (itemsToGive == null || itemsToGive.Count == 0)
            return;

        // Clear previous daily rewards
        listItemsTogiveDaily.Clear();

        // Add items to the daily reward list
        foreach (var item in itemsToGive)
        {
            listItemsTogiveDaily.Add(item);
            inventoryItemPresent.AddItem(item); // Add item to inventory
        }

        // Show the UI if there are rewards
        if (listItemsTogiveDaily.Count > 0)
        {
            Debug.Log(listItemsTogiveDaily.Count);
            ShowDailyGiveUI();
        }
    }

    public void CloseDailyGiveUI()
    {
        if (DailygiveUI != null)
        {
            DailygiveUI.SetActive(false);
        }
    }

    private void ShowDailyGiveUI()
    {
        // Check if there are any rewards to show
        if (listItemsTogiveDaily == null || listItemsTogiveDaily.Count == 0)
        {
            Debug.Log("No rewards to show. Hiding UI.");
            if (DailygiveUI != null)
            {
                DailygiveUI.SetActive(false);
            }
            return;
        }

        // Clear the container and show the rewards
        if (DailygiveUI != null && dailyGiveUIHandler != null)
        {
            DailygiveUI.SetActive(true); // Show the UI panel
            dailyGiveUIHandler.ClearContainer();

            foreach (var item in listItemsTogiveDaily)
            {
                inventoryItemPresent.AddItem(item);
                dailyGiveUIHandler.CreateUIItemInContainer(item);
            }
        }
        else
        {
            Debug.LogWarning("DailygiveUI or dailyGiveUIHandler is not assigned.");
        }
        listItemsTogiveDaily.Clear();
    }


    public void AddItemByID(int itemID, int count)
    {
        // Find the UIItemData associated with the given itemID
        UIItemData uiItemData = listUIItemPrefab.FirstOrDefault(item => item.idItem == itemID);
        ItemClass itemClass = uiItemData.GetComponent<ItemClass>();
        if (uiItemData == null)
        {
            Debug.LogWarning($"No UIItemData found for itemID: {itemID}");
            return;
        }

        // Construct a new ItemData object based on the UIItemData template
        ItemData newItemData = new ItemData
        {
            nameItem = uiItemData.nameItem,
            idItem = uiItemData.idItem,
            count = count,
            maxCount = itemClass.maxCountItem,
            itemtype = itemClass.itemtype,
        };

        // Use the existing AddItem method to handle addition logic
        AddItem(newItemData);
    }

    public void AddItem(ItemData itemDataAdd)
    {
        ItemData itemDataInList = this.listItemsTogiveDaily.FirstOrDefault(item => item.idItem == itemDataAdd.idItem && item.count != item.maxCount);
        if (itemDataInList != null)
        {
            itemDataInList.count = itemDataInList.count + itemDataAdd.count;
        }
        else if (itemDataInList == null)
        {
            listItemsTogiveDaily.Add(itemDataAdd);
        }
    }
}
