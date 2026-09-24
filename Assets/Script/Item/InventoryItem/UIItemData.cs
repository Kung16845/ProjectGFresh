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
    public Image itemIconImage;

    public void UpdateDataUI(ItemClass itemClass)
    {
        if (itemClass == null) return;

        int countItem = itemClass.quantityItem;

        if (itemIconImage != null)
        {
            if (itemClass.IconSprite != null && itemClass.IconSprite.sprite != null)
            {
                itemIconImage.sprite = itemClass.IconSprite.sprite;
            }
            else if (itemClass.itemIcon != null)
            {
                itemIconImage.sprite = itemClass.itemIcon;
            }
        }

        if (count != null)
        {
            if (slotTypeParent == SlotType.SlotBoxes)
            {
                count.text = countItem.ToString();
            }
            else
            {
                count.text = $"{countItem}/{itemClass.maxCountItem}";
            }
        }
    }
}
