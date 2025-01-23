using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeItemUI : MonoBehaviour
{
    public Image itemIconImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI amountText;

    public void Initialize(RecipeItem recipeItemData, int amountHave)
    {
        if (itemIconImage != null)
            itemIconImage.sprite = recipeItemData.itemIcon;

        if (itemNameText != null)
            itemNameText.text = recipeItemData.itemName;

        if (amountText != null)
        {
            amountText.text = $" {amountHave} / {recipeItemData.amountNeeded}";

            // Change color dynamically based on whether the player has enough items
            if (amountHave < recipeItemData.amountNeeded)
            {
                amountText.color = Color.yellow; // Not enough items
            }
            else
            {
                amountText.color = Color.white; // Sufficient items
            }
        }
    }

}

