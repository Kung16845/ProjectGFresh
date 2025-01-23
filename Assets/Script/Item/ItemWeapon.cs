using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeapon : ItemClass
{   
    [Header("Quality")]
    public int Repairmaterial;
    public float Quality;  
    [Header("Stat Weapon")]
    public int rateOfFire;
    public int handling; 
    public float accuracy; 
    public int capacity; 
    public float stability; 
    public float damage; 
    public bool fullAuto; 
    public bool isShotgun;
    public float damageDropOff;
    public int Pellets;
    public float Spreadangle;
    public Ammotype ammoType; 
    public Reloadtype reloadtype;
    public AudioClip Gunshotsound;
    public Sprite gunsprite;
    private string GetReloadAnimationName(Reloadtype reloadtype)
    {
        switch (reloadtype)
        {
            case Reloadtype.AssaultRifle:
                return "ReloadGenericRifle";
            case Reloadtype.SMG:
                return "ReloadMp5";
            case Reloadtype.Shotgunpump:
                return "ReloadShotgun";
            case Reloadtype.Sniper:
                return "ReloadSniper";
            case Reloadtype.Pistol:
                return "ReloadSidearm";
            default:
                return "ReloadGenericRifle";
        }
    }
    public override Dictionary<string, float> GetStats()
    {
        return new Dictionary<string, float>
        {
            { "Damage", damage },
            { "Rate of Fire", rateOfFire },
            { "handling", handling },
            { "Accuracy", accuracy },
            { "capacity", capacity },
            { "stability", stability },
            { "damageDropOff", damageDropOff },
            { "rarityItem", rarityItem}
        };
    }

    public override Dictionary<string, float> GetMaxStatValues()
    {
        return new Dictionary<string, float>
        {
            { "Damage", 350 },
            { "Rate of Fire", 1000},
            { "handling", 100 },
            { "Accuracy", 100 },
            { "capacity", 100 },
            { "stability", 100 },
            { "damageDropOff", 30 },
            { "rarityItem", 5}
        };
    }
}

public enum Ammotype
{
    HighCaliber, 
    MediumCaliber,
    LowCaliber,
    Shotgun
}
public enum Reloadtype
{
    AssaultRifle, 
    SMG,
    Shotgunpump,
    Sniper,
    Pistol
}