using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ChemicalItemUI : MonoBehaviour
{
    public Image itemIconImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI craftingTimeText;

    private CraftingItem craftingItemData;
    private ChemicalUi chemicalUi;

    public Button buttonComponent; // Assign this in the Inspector

    public void Initialize(CraftingItem itemData, ChemicalUi parentUI)
    {
        craftingItemData = itemData;
        chemicalUi = parentUI;

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
        if (chemicalUi != null)
        {
            chemicalUi.DisplaySelectedItemDetails(craftingItemData);
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
