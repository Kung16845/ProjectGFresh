using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIItemData : MonoBehaviour
{
    public TextMeshProUGUI count;
    public int idItem;
    public string nameItem;
    public SlotType slotType;
    public LootingSystem originatingLootSystem;
    public TradesystemScript originatingTradesystem;
    public SlotType slotTypeParent;
    public Image itemIconImage; // Keep this as Image because it's a UI element

    public void UpdateDataUI(ItemClass itemClass)
    {
        int countItem = itemClass.quantityItem;
        // Debug.Log("count : " + countItem);
        if (itemIconImage != null && itemClass.IconSprite != null)
        {
            // Assign the sprite from itemClass.IconSprite to itemIconSprite
            itemIconImage.sprite = itemClass.IconSprite.sprite;
        }

        if (slotTypeParent == SlotType.SlotBoxes)
        {
            count.text = countItem.ToString();
        }
        else
        {
            count.text = countItem.ToString() + "/" + itemClass.maxCountItem.ToString();
        }
    }
}
