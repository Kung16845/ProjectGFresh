 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globalstat : MonoBehaviour
{
    [Header("DDAPoint")]
    public float CommunitysupplyPoint;
    public float Skillplaypoint;
    public float itemponts;
    public int ProgressDay;
    public float succesfulDefense;
    [Header("Action Speed")]
    public float ActionSpeed;
    public float Healingspeed;
    public float Craftingspeed;
    public float BuildingSpeed;
    [Header("DamageFactor")]
    public float WeapondDamge;
    public float Damageincrease;
    public float Carendurance;
    public float fuelefficiency;
    [Header("Expidition")]
    public float RadioCooldownspeed;
    public float OutpostrewardAmp;
    public float expiditionspeed;
    public float expiditionrisk;
    public int availablecar;
    public int UnaviableCar;
    public int OutpostLimit;
    public bool Tunnelaviable = false;
    [Header("Community Stat")]
    public float illResistance;
    public float Npcchange;
    public int activeBed;
    public int Totalcurebed;
    public int Activecurebed;
    public int Usedcurebed;
    public float Discontent;
    public float Growingspeed;
    public int CraftingSlot;
    public int ChemicalCraftingSlot;
    public int MoonshineCraftingSlot;
    public int usedCraftingSlot;
    public int usedChemicalCraftingSlot;
    public int usedMoonshineCraftingSlot;
    public bool SatelliteOnline;
    public bool ReconActive;
    [Header("Exipiditionaction")]
    public bool expiditionactiveeventactive = false;
    private int bedFactor = 0;
    //ActiveBed
    void Start()
    {
        Tunnel tunnel = FindObjectOfType<Tunnel>();
        if(tunnel != null)
        {
            if(tunnel.tuneelisopen)
                Tunnelaviable = true;
            else
                Tunnelaviable = false;
        }
    }
    public void AddBedsFromBuilding(int bedContribution)
    {
        bedFactor += bedContribution;
        UpdateActiveBedCount();
    }
    public void UpdateBuildingBedContribution(int oldContribution, int newContribution)
    {
        // Subtract the old contribution and add the new one
        bedFactor = bedFactor - oldContribution + newContribution;
        UpdateActiveBedCount();
    }
    private void UpdateActiveBedCount()
    {
        activeBed = bedFactor;  // Set activeBed to the total contribution so far
    }
    //Healingspeed
    public void IncreaseHealingSpeed(float HealdingSpeedContribution)
    {
        Healingspeed += HealdingSpeedContribution;
    }
    public void UpdateHealingSpeedContribution(float oldContribution, float newContribution)
    {
        Healingspeed = Healingspeed + newContribution - oldContribution;
    }
    //Active Curebed
    public void IncreaseCurebed(int Curebedcontribution)
    {
        Totalcurebed += Curebedcontribution;
    }
    public void UpdateCurebedContribution(int oldContribution, int newContribution)
    {
        Totalcurebed = Totalcurebed + newContribution - oldContribution;
        Activecurebed += newContribution; 
    }
    //Discontent
    public void DecreaseDiscontent(float discontentContribution)
    {
        Discontent -= discontentContribution;
    }
    public void UpdateDiscontentContribution(float oldContribution, float newContribution)
    {
        Discontent = Discontent + oldContribution - newContribution;
    }
    //OutpostLimit
    public void IncreaseOutpostlimit(int Outpostcontribution)
    {
        OutpostLimit += Outpostcontribution;
    }
    public void UpdateoOutpostContribution(int oldContribution, int newContribution)
    {
        OutpostLimit = OutpostLimit + newContribution - oldContribution;
    }
    //Riskofexpidition
    public void DecreaseRiskofExipidition(float RiskContribution)
    {
        expiditionrisk -= RiskContribution;
    }
    public void UpdateRiskofExipiditionContribution(float oldContribution, float newContribution)
    {
        Debug.Log("Function Called");
        expiditionrisk = expiditionrisk + oldContribution - newContribution;
    }
    //IncreaseReward speed
    public void IncreaseRewardspeed(float RewardSpeedContribution)
    {
       OutpostrewardAmp -= RewardSpeedContribution;
    }
    public void UpdateRewardSpeedContribution(float oldContribution, float newContribution)
    {
       OutpostrewardAmp = OutpostrewardAmp + newContribution - oldContribution;
    }
    //IncreaseNpcchage
    public void IncreaseNpcchange(float NpcchangeContribution)
    {
       Npcchange += NpcchangeContribution;
    }
    public void UpdateNpcchangeContribution(float oldContribution, float newContribution)
    {
       Npcchange = Npcchange + newContribution - oldContribution;
    }
    public void CalculateActionSpeed(float factornumber)
    {
        ActionSpeed = 1 + factornumber; 
    }
     public void UpdateCraftingSlots(int newSlotValue)
    {
        CraftingSlot = newSlotValue;
    }
     public void UpdateChemicalCraftingSlot(int newSlotValue)
    {
        ChemicalCraftingSlot = newSlotValue;
    }
    public void UpdateMoonshineCraftingSlot(int newSlotValue)
    {
        MoonshineCraftingSlot = newSlotValue;
    }
}