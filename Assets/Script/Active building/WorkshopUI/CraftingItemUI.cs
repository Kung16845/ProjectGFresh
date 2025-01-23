using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CraftingItemUI : MonoBehaviour
{
    public Image itemIconImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI craftingTimeText;

    private CraftingItem craftingItemData;
    private WorkshopUI workshopUI;

    public Button buttonComponent; // Assign this in the Inspector

    public void Initialize(CraftingItem itemData, WorkshopUI parentUI)
    {
        craftingItemData = itemData;
        workshopUI = parentUI;

        if (itemIconImage != null)
            itemIconImage.sprite = itemData.itemIcon;

        if (itemNameText != null)
            itemNameText.text = $"{itemData.itemName} X {itemData.amountProduced}";

        if (craftingTimeText != null)
            craftingTimeText.text =  (itemData.craftingTime / 1000f).ToString("F1") + " hr";

        // Add click listener
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (workshopUI != null)
        {
            workshopUI.DisplaySelectedItemDetails(craftingItemData);
        }
    }

    private void OnDestroy()
    {
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveListener(OnClick);
        }
    }
}

