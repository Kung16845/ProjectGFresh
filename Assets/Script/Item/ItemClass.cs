using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
public class ItemClass : MonoBehaviour
{   
    [Header("Stat")]
    public string nameItem;
    [TextArea(15,15)]
    public string Describetion;
    public int quantityItem;
    public int maxCountItem;
    public Itemtype itemtype;
    [Header("Item value")]
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
}
