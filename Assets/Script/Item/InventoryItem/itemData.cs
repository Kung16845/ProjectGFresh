using System;
using UnityEngine;

[Serializable]
public class ItemData 
{   
    public string nameItem;
    public int idItem;
    public int count;
    public int maxCount;
    public Itemtype itemtype;
    public SlotType parantslotType;

    public ItemData() { }

    public ItemData(ItemData other)
    {
        if (other == null) return;
        nameItem = other.nameItem;
        idItem = other.idItem;
        count = other.count;
        maxCount = other.maxCount;
        itemtype = other.itemtype;
        parantslotType = other.parantslotType;
    }

    public ItemData Clone()
    {
        return new ItemData(this);
    }
}

public enum Itemtype
{   
    Weapon,
    Vest,
    Backpack,
    Tool,
    Grenade,
    Ammo,
    Pill,
    General
}