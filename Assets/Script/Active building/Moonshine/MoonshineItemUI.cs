using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MoonshineItemUI : MonoBehaviour
{
    public Image itemIconImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI craftingTimeText;

    private CraftingItem craftingItemData;
    private MoonshinUI moonshinUI;

    public Button buttonComponent; // Assign this in the Inspector

    public void Initialize(CraftingItem itemData, MoonshinUI parentUI)
    {
        craftingItemData = itemData;
        moonshinUI = parentUI;

        if (itemIconImage != null)
            itemIconImage.sprite = itemData.itemIcon;

        if (itemNameText != null)
            itemNameText.text = $"{itemData.itemName} X {itemData.amountProduced }";

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
        if (moonshinUI != null)
        {
            moonshinUI.DisplaySelectedItemDetails(craftingItemData);
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
