using UnityEngine;

/// <summary>
/// Holds all the stat values a building contributes to Globalstat at a given level.
/// Leave unused fields at 0 — ApplyTo/RemoveFrom only touch non-zero values.
/// </summary>
[System.Serializable]
public struct BuildingContribution
{
    [Header("Expedition")]
    public float riskReduction;    // Beacon, Watchtower
    public int   outpostLimit;     // Beacon
    public float rewardSpeed;      // Beacon

    [Header("Community")]
    public float npcChange;        // Beacon, Watchtower
    public int   beds;             // SmallBed, MediumBed, Lounge
    public int   curebeds;         // Clinic, FieldHospital
    public float discontent;       // Clinic, FieldHospital, Lounge
    public float healingSpeed;     // FieldHospital

    [Header("Vehicle")]
    public int availableCars;      // CarWorkshop

    [Header("Crafting")]
    public int craftingSlots;      // Workshop
    public int chemicalSlots;      // ChemicalLab
    public int moonshineSlots;     // Moonshine

    public void ApplyTo(Globalstat g)
    {
        if (riskReduction  != 0) g.expiditionrisk         -= riskReduction;
        if (outpostLimit   != 0) g.OutpostLimit            += outpostLimit;
        if (rewardSpeed    != 0) g.OutpostrewardAmp        -= rewardSpeed;
        if (npcChange      != 0) g.Npcchange               += npcChange;
        if (beds           != 0) g.AddBedsFromBuilding(beds);
        if (curebeds       != 0) g.Totalcurebed            += curebeds;
        if (discontent     != 0) g.Discontent              -= discontent;
        if (healingSpeed   != 0) g.Healingspeed            += healingSpeed;
        if (availableCars  != 0) g.availablecar             += availableCars;
        if (craftingSlots  != 0) g.CraftingSlot            += craftingSlots;
        if (chemicalSlots  != 0) g.ChemicalCraftingSlot    += chemicalSlots;
        if (moonshineSlots != 0) g.MoonshineCraftingSlot   += moonshineSlots;
    }

    public void RemoveFrom(Globalstat g)
    {
        if (riskReduction  != 0) g.expiditionrisk         += riskReduction;
        if (outpostLimit   != 0) g.OutpostLimit            -= outpostLimit;
        if (rewardSpeed    != 0) g.OutpostrewardAmp        += rewardSpeed;
        if (npcChange      != 0) g.Npcchange               -= npcChange;
        if (beds           != 0) g.AddBedsFromBuilding(-beds);
        if (curebeds       != 0) g.Totalcurebed            -= curebeds;
        if (discontent     != 0) g.Discontent              += discontent;
        if (healingSpeed   != 0) g.Healingspeed            -= healingSpeed;
        if (availableCars  != 0) g.availablecar             -= availableCars;
        if (craftingSlots  != 0) g.CraftingSlot            -= craftingSlots;
        if (chemicalSlots  != 0) g.ChemicalCraftingSlot    -= chemicalSlots;
        if (moonshineSlots != 0) g.MoonshineCraftingSlot   -= moonshineSlots;
    }
}
