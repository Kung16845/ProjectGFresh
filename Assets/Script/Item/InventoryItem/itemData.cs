using System;
using System.Collections;
using System.Collections.Generic;
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