using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class CraftingItem
{
    public int itemID;
    public float craftingTime; // 1000 = 1 hr
    public List<RecipeItem> recipeItems; // Items needed to craft this item
    public int Ammoneeded;
    public int Fuelneeded;
    public int Steelneeded;
    public int Plankneeded;
    public int Foodneeded;
    public int amountProduced = 1; // Number of items or ammo produced per crafting job
    [HideInInspector] public string itemName;
    [HideInInspector] public Sprite itemIcon;
    [HideInInspector] public int rarity;
}


[System.Serializable]
public class RecipeItem
{
    public int itemID;
    public int amountNeeded;

    // These properties will be auto-assigned
    [HideInInspector] public string itemName;
    [HideInInspector] public Sprite itemIcon;
}

