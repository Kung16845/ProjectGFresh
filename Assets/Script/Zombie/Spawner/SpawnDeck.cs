using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnDeck
{
    public string deckName;
    public float deckDuration;
    public int deckTier;    // Corrected variable name
    public int deckID;      // Corrected variable name
    public List<SpawnWave> spawnWaves;
}
