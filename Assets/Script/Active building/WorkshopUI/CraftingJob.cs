[System.Serializable]
public class CraftingJob
{
    public CraftingItem craftingItem;
    public float timeRemaining;
    public bool isComplete;
    public CraftingSource source; // Added field

    public CraftingJob(CraftingItem item, float actionSpeed, CraftingSource jobSource)
    {
        craftingItem = item;
        timeRemaining = (item.craftingTime / 1000f * 60f) / actionSpeed; // Shorten time based on action speed
        isComplete = false;
        source = jobSource;
    }
}
public enum CraftingSource
{
    Workshop,
    ChemicalLab,
    Moonshine,
    // Add other crafting sources as needed
}