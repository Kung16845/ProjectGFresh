using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemClass : MonoBehaviour
{   
    [Header("Stat")]
    public string nameItem;
    [TextArea(5, 10)]
    public string Describetion;
    public int quantityItem;
    public int maxCountItem;
    public Itemtype itemtype;

    [Header("Item Value")]
    public int rarityItem;
    public float tradeValueItem;
    public bool carftableItem;
    public int disassembleItem;
    public int idItem;

    [Header("DDA Stat")]
    public string pointType;
    public int point;

    [Header("Icon")]
    public Image IconSprite;
    public Sprite itemIcon;

    public virtual Dictionary<string, float> GetStats()
    {
        return new Dictionary<string, float>();
    }

    public virtual Dictionary<string, float> GetMaxStatValues()
    {
        return new Dictionary<string, float>();
    }

    public virtual ItemData ToItemData()
    {
        return new ItemData
        {
            nameItem = nameItem,
            idItem = idItem,
            count = quantityItem,
            maxCount = maxCountItem,
            itemtype = itemtype
        };
    }
}
