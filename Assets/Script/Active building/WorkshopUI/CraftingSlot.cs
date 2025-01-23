using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSlot : MonoBehaviour
{
    public Image itemIconImage;
    public TextMeshProUGUI itemNameText;
    public Slider progressSlider;
    public TextMeshProUGUI timeRemainingText; // Time remaining text
    public Button cancelButton;

    // Debugging control
    public bool enableDebugLogs = false;

    private CraftingItem craftingItem;
    private InventoryItemPresent inventoryItemPresent;
    private float craftingTimeInSeconds;

    public void Initialize(CraftingItem itemData, InventoryItemPresent inventory)
    {
        craftingItem = itemData;
        inventoryItemPresent = inventory;

        if (itemIconImage != null)
            itemIconImage.sprite = itemData.itemIcon;

        if (itemNameText != null)
            itemNameText.text = itemData.itemName;

        craftingTimeInSeconds = itemData.craftingTime / 1000f * 60f; // Convert to seconds

        // Initialize the progress slider
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = craftingTimeInSeconds;
            progressSlider.value = 0f;
        }

        // Initialize the time remaining text
        if (timeRemainingText != null)
        {
            timeRemainingText.text = FormatTime(craftingTimeInSeconds);
        }

        // Start the countdown
        StartCoroutine(StartCraftingCountdown());
    }

    private IEnumerator StartCraftingCountdown()
    {
        float timeElapsed = 0f;
        while (timeElapsed < craftingTimeInSeconds)
        {
            // Update progress slider
            if (progressSlider != null)
                progressSlider.value = timeElapsed;

            // Update time remaining text
            if (timeRemainingText != null)
            {
                float timeRemaining = craftingTimeInSeconds - timeElapsed;
                timeRemainingText.text = FormatTime(timeRemaining);
            }

            // Debugging output
            if (enableDebugLogs)
            {
                Debug.Log($"Crafting '{craftingItem.itemName}': {timeElapsed:F2}/{craftingTimeInSeconds:F2} seconds elapsed.");
            }

            yield return null; // Wait for the next frame
            timeElapsed += Time.deltaTime;
        }

        // Ensure the slider is full at the end
        if (progressSlider != null)
            progressSlider.value = craftingTimeInSeconds;

        // Update time remaining text to zero
        if (timeRemainingText != null)
            timeRemainingText.text = "00:00";

        // Crafting complete
        OnCraftingComplete();
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnCraftingComplete()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"Crafting complete: {craftingItem.itemName}");
        }

        // Add crafted item to inventory
        ItemData craftedItemData = new ItemData
        {
            idItem = craftingItem.itemID,
            count = 1 // Adjust quantity as needed
        };
        inventoryItemPresent.AddItem(craftedItemData);

        // Notify the crafting controller to remove this slot
        CraftingController craftingController = FindObjectOfType<CraftingController>();
        if (craftingController != null)
        {
            craftingController.RemoveCraftingSlot(gameObject);
        }
        else
        {
            Debug.LogError("CraftingController not found.");
            Destroy(gameObject);
        }
    }

    public void OnCancelButtonClicked()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"Crafting cancelled: {craftingItem.itemName}");
        }

        // Stop the crafting coroutine
        StopCoroutine(StartCraftingCountdown());

        // Optionally, refund materials here

        // Remove the crafting slot
        CraftingController craftingController = FindObjectOfType<CraftingController>();
        if (craftingController != null)
        {
            craftingController.RemoveCraftingSlot(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
