using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MainSpawner : MonoBehaviour
{
    [Header("Lanes Configuration")]
    public List<Lane> lanes = new List<Lane>();

    [Header("Spawn Decks Configuration")]
    public List<SpawnDeck> ActiveSpawnDecks = new List<SpawnDeck>();  // Active decks being used
    public List<SpawnDeck> StorageDecks = new List<SpawnDeck>();      // Saved decks waiting to be used
    private List<MutationType> currentDeckMutations; 
    [Header("Selected Mutations")]
    [SerializeField]
    private List<MutationType> persistentMutations = new List<MutationType>();
    public List<MutationSelectionRule> mutationSelectionRules = new List<MutationSelectionRule>();

    [Header("Global Mutation Settings")]
    public MutationType globalMutationType = MutationType.None;
    [Range(0f, 1f)]
    public float mutationApplyRate = 0f;

    [Header("DDA Settings")]
    public bool useDDA = false;  // Toggle to enable/disable DDA system

    private SaveDataDDA saveDataDDA;  // Reference to SaveDataDDA script
    private int currentDeckIndex;
    public TutorialManager tutorialManager;
    private Coroutine deckCoroutine;
    [Header("UI Elements")]
    public TextMeshProUGUI startDelayText;
    [Header("Wave Announcement UI")]
    public Image Waveannoucer; 

    [Header("Spawn Timing")]
    public float startDelay = 0f;

    // Variable to determine which decks to use
    public int desiredDeckTier = 1; // Set this in the Inspector or via code

    // **Tracking Variables**
    [Header("Spawn Tracking")]
    public float currentDeckDurationLeft = 0f;     // Time left for the current deck
    public float totalDurationLeft = 0f;           // Total time left for all active decks
    public int totalZombiesLeft = 0;               // Total zombies left to spawn
    public int currentDeckZombiesLeft = 0;         // Zombies left in the current deck

    public List<SpawnDeck> remainingDecks = new List<SpawnDeck>(); // Decks yet to be spawned
    public CheckUsingDDA checkUsingDDA;
    
    private void Awake() {
        checkUsingDDA = FindObjectOfType<CheckUsingDDA>();
        checkUsingDDA.mainSpawner = this;
        useDDA = checkUsingDDA.isUsingDDA;
    }
    private void Start()
    {
        LaneManager.Instance.RegisterLanes(lanes);
        InitializeSpawnPoints();

        // Initialize saveDataDDA
        saveDataDDA = FindObjectOfType<SaveDataDDA>();
        
        if (saveDataDDA == null)
        {
            Debug.LogError("SaveDataDDA not found in the scene.");
        }

        if(tutorialManager.isturorialnight)
        {
            startDelayText.gameObject.SetActive(false);
            AddActiveDeck(601010001);// Add infinity spawnDeck
        }
        else
        {
            CalculateDecks();
            StartCoroutine(StartSpawningAfterDelay());
        }
        InitializeTracking();
        currentDeckIndex = 0;
    }

    private void InitializeSpawnPoints()
    {
        foreach (Lane lane in lanes)
        {
            // Ensure the spawn point has a SpawnPoint component
            SpawnPoint spawnPoint = lane.spawnPoint.GetComponent<SpawnPoint>();
            if (spawnPoint == null)
            {
                spawnPoint = lane.spawnPoint.gameObject.AddComponent<SpawnPoint>();
            }

            spawnPoint.Initialize(lane);
        }
    }
    private IEnumerator CountdownStartDelay()
    {
        float timeLeft = startDelay;
        while (timeLeft > 0)
        {
            if (startDelayText != null)
            {
                startDelayText.text = $"{timeLeft:F1} ";
            }

            yield return new WaitForSeconds(0.1f); // Update every 0.1 seconds for smoother UI
            timeLeft -= 0.1f;
        }
        startDelayText.gameObject.SetActive(false);
        SoundManager.Instance.PlaySound("Siren");
        // SoundManager.Instance.PlaySound("Playmusic");
    }

    private IEnumerator StartSpawningAfterDelay()
    {
        if (startDelay > 0f)
        {
            StartCoroutine(CountdownStartDelay());
            yield return new WaitForSeconds(startDelay);
        }

        StartNextDeck();
    }

    /// <summary>
    /// Calculates and sets the ActiveSpawnDecks based on the DDA setting and ensures total duration <= 360 seconds.
    /// </summary>
    public void CalculateDecks()
    {
        ActiveSpawnDecks.Clear();
        float totalDuration = 0f;

        if (useDDA && saveDataDDA != null && saveDataDDA.dataDDACollection != null)
        {
            DataDDA avgData = saveDataDDA.dataDDACollection.averageData;
            desiredDeckTier = CalculateDesiredTier(avgData);
            Debug.Log($"DDA enabled. Desired deck tier: {desiredDeckTier}");

            // Find decks matching the desiredTier
            List<SpawnDeck> possibleDecks = StorageDecks.FindAll(deck => deck.deckTier == desiredDeckTier);

            if (possibleDecks.Count == 0)
            {
                Debug.LogWarning($"No decks found with tier {desiredDeckTier}. Falling back to random selection.");
                SelectRandomDecks(ref ActiveSpawnDecks, 360f, StorageDecks);
                return;
            }

            // Shuffle the possibleDecks using Fisher-Yates Shuffle
            ShuffleListInPlace(possibleDecks);

            // Add decks until totalDuration reaches 360 seconds
            foreach (var deck in possibleDecks)
            {
                if (totalDuration + deck.deckDuration <= 360f)
                {
                    ActiveSpawnDecks.Add(deck);
                    totalDuration += deck.deckDuration;
                }

                if (totalDuration >= 360f)
                    break;
            }

            Debug.Log($"Selected {ActiveSpawnDecks.Count} decks with total duration {totalDuration} seconds.");
        }
        else
        {
            Debug.Log("DDA disabled. Selecting decks randomly.");
            SelectRandomDecks(ref ActiveSpawnDecks, 360f, StorageDecks);
        }

        // Optional: If ActiveSpawnDecks is empty after selection, handle accordingly
        if (ActiveSpawnDecks.Count == 0)
        {
            Debug.LogWarning("No decks selected. Please check StorageDecks and selection criteria.");
        }
    }

    private void ShuffleListInPlace<T>(List<T> list)
    {
        System.Random rand = new System.Random();
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            // Swap list[i] with list[j]
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private void SelectRandomDecks(ref List<SpawnDeck> selectedDecks, float maxDuration, List<SpawnDeck> deckPool)
    {
        selectedDecks.Clear();
        persistentMutations = GetRandomMutations(2);
        float totalDuration = 0f;

        if (deckPool == null || deckPool.Count == 0)
        {
            Debug.LogError("Deck pool is empty or null. Cannot select decks.");
            return;
        }

        List<SpawnDeck> availableDecks = new List<SpawnDeck>(deckPool);
        System.Random rand = new System.Random();
        while (availableDecks.Count > 0 && totalDuration < maxDuration)
        {
            int index = rand.Next(availableDecks.Count);
            SpawnDeck selectedDeck = availableDecks[index];

            if (totalDuration + selectedDeck.deckDuration <= maxDuration)
            {
                selectedDecks.Add(selectedDeck);
                totalDuration += selectedDeck.deckDuration;
            }

            // Remove the selected deck to avoid duplication
            availableDecks.RemoveAt(index);
        }

        Debug.Log($"Randomly selected {selectedDecks.Count} decks with total duration {totalDuration} seconds.");
    }

    private float CalculateDDAPoint(float killPerMinute,float accuracy,float barrierDamage,float multikill,float failedattempt)
    {
        float DDASkillplayPoint;
        killPerMinute *= 2; 
        accuracy *= 1;
        barrierDamage = ((5000 - barrierDamage)/500) * 10;
        multikill *= 5;
        failedattempt *= 75;
        return DDASkillplayPoint = (killPerMinute + accuracy + barrierDamage + multikill - failedattempt);
    }
    private int CalculateDesiredTier(DataDDA avgData)
    {
        float skillpoint = CalculateDDAPoint(avgData.killPerMinute,avgData.accuracy,avgData.barrierDamage,avgData.multiKillCount,avgData.valueFail);
        if (persistentMutations.Count == 0)
        {
            persistentMutations = GetSelectedMutations(skillpoint); // Store in persistentMutations
            Debug.Log("Mutations selected for all decks: " + string.Join(", ", persistentMutations));
        }
        else
        {
            Debug.Log("Using persistent mutations: " + string.Join(", ", persistentMutations));
        }

            // Start the current deck using the persistent mutations
        Debug.Log(skillpoint);
        if (skillpoint >= 320)
        {
            return 4;
        }
        else if (skillpoint >=225)
        {
            return 3;
        }
        else if (skillpoint >= 150)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    /// <summary>
    /// Initializes tracking variables based on ActiveSpawnDecks.
    /// </summary>
    private void InitializeTracking()
    {
        totalDurationLeft = 0f;
        totalZombiesLeft = 0;

        foreach (var deck in ActiveSpawnDecks)
        {
            totalDurationLeft += deck.deckDuration;

            foreach (var wave in deck.spawnWaves)
            {
                foreach (var laneConfig in wave.laneSpawnConfigs)
                {
                    foreach (var zombieQueue in laneConfig.zombieSpawnQueue)
                    {
                        totalZombiesLeft += zombieQueue.quantity;
                    }
                }
            }
        }

        // Initialize remainingDecks
        remainingDecks = new List<SpawnDeck>(ActiveSpawnDecks);

        Debug.Log($"Tracking Initialized: Total Duration Left = {totalDurationLeft}s, Total Zombies Left = {totalZombiesLeft}");
    }

    // Existing AddActiveDeck method remains unchanged
    public void AddActiveDeck(int deckID)
    {
        // // Find the deck with the given deckID in StorageDecks
        SpawnDeck deckToAdd = StorageDecks.Find(deck => deck.deckID == deckID);
        DataDDA avgData = saveDataDDA.dataDDACollection.averageData;
        float skillpoint = CalculateDDAPoint(avgData.killPerMinute,avgData.accuracy,avgData.barrierDamage,avgData.multiKillCount,avgData.valueFail);
        if (persistentMutations.Count == 0)
        {
            persistentMutations = GetSelectedMutations(skillpoint); // Store in persistentMutations
            Debug.Log("Mutations selected for all decks: " + string.Join(", ", persistentMutations));
        }
        else
        {
            Debug.Log("Using persistent mutations: " + string.Join(", ", persistentMutations));
        }
        if (deckToAdd != null)
        {
            ActiveSpawnDecks.Add(deckToAdd);
            Debug.Log($"Added deck '{deckToAdd.deckName}' (ID: {deckID}) to ActiveSpawnDecks.");

            // Update tracking variables
            totalDurationLeft += deckToAdd.deckDuration;

            foreach (var wave in deckToAdd.spawnWaves)
            {
                foreach (var laneConfig in wave.laneSpawnConfigs)
                {
                    foreach (var zombieQueue in laneConfig.zombieSpawnQueue)
                    {
                        totalZombiesLeft += zombieQueue.quantity;
                    }
                }
            }

            remainingDecks.Add(deckToAdd);
        }
        else
        {
            Debug.LogWarning($"Deck with ID {deckID} not found in StorageDecks.");
        }
    }
    public bool isCompleteSpawned = false;
    public void StartNextDeck()
    {
        if (currentDeckIndex < ActiveSpawnDecks.Count)
        {
            Debug.Log("StartNextDeck.");
            SpawnDeck currentDeck = ActiveSpawnDecks[currentDeckIndex];
            StartCoroutine(ProcessDeck(currentDeck));
        }
        else
        {   
            isCompleteSpawned = true;
            Debug.Log("All spawn decks have been completed.");
        }
    }
    private IEnumerator ProcessDeck(SpawnDeck deck)
    {
        // Initialize tracking variables
        currentDeckDurationLeft = deck.deckDuration;
        currentDeckZombiesLeft = 0;

        Debug.Log($"Processing Deck '{deck.deckName}' with Duration {deck.deckDuration}s.");

        foreach (var wave in deck.spawnWaves)
        {
            foreach (var laneConfig in wave.laneSpawnConfigs)
            {
                SpawnPoint spawnPoint = GetSpawnPointByLaneID(laneConfig.laneID);
                if (spawnPoint != null)
                {
                    // Use the persistent mutations for all decks
                    spawnPoint.StartSpawningQueue(
                        laneConfig.zombieSpawnQueue,
                        persistentMutations,
                        mutationApplyRate
                    );
                }
                else
                {
                    Debug.LogWarning($"No spawn point found for lane ID {laneConfig.laneID}");
                }
            }
            // Wait for the wave's timeUntilNextWave
            Debug.Log($"Waiting for {wave.timeUntilNextWave}s before next wave.");
            yield return new WaitForSeconds(wave.timeUntilNextWave);
            SoundManager.Instance.PlaySound("ZombieScream");
        }
        Debug.Log($"Deck '{deck.deckName}' completed.");
        currentDeckDurationLeft = 0f;
        currentDeckZombiesLeft = 0;

        currentDeckIndex++;

        // Proceed to the next deck
        StartNextDeck();
    }
    private IEnumerator FlashWaveAnnouncement(float duration)
    {
        float timeElapsed = 0f;
        bool isVisible = true;

        while (timeElapsed < duration)
        {
            if (Waveannoucer != null)
            {
                Waveannoucer.gameObject.SetActive(isVisible);
            }

            isVisible = !isVisible; // Toggle visibility
            timeElapsed += 0.5f;   // Adjust flashing speed (0.25 seconds per toggle)
            yield return new WaitForSeconds(0.5f);
        }

        if (Waveannoucer != null)
        {
            Waveannoucer.gameObject.SetActive(false); // Ensure UI is hidden after flashing
        }
    }
    private SpawnPoint GetSpawnPointByLaneID(int laneID)
    {
        Lane lane = lanes.Find(l => l.laneID == laneID);
        if (lane != null)
        {
            return lane.spawnPoint.GetComponent<SpawnPoint>();
        }
        return null;
    }

    public void OnZombieSpawned(int zombiesSpawned, float deckDuration)
    {
        totalZombiesLeft -= zombiesSpawned;
        currentDeckZombiesLeft -= zombiesSpawned;

        // Clamp the values to prevent negative numbers
        totalZombiesLeft = Mathf.Max(totalZombiesLeft, 0);
        currentDeckZombiesLeft = Mathf.Max(currentDeckZombiesLeft, 0);

        Debug.Log($"Zombies Spawned: {zombiesSpawned}, Zombies Left: {totalZombiesLeft}");
    }

    public void OnZombieQueueStarted(int zombiesToSpawn, float deckDuration)
    {
        totalZombiesLeft += zombiesToSpawn;
        currentDeckZombiesLeft += zombiesToSpawn;

        Debug.Log($"Zombie Queue Started: {zombiesToSpawn} zombies to spawn.");
    }

    private void Update()
    {
        // Decrement the deck duration
        if (currentDeckDurationLeft > 0f)
        {
            currentDeckDurationLeft -= Time.deltaTime;
            if (currentDeckDurationLeft < 0f)
                currentDeckDurationLeft = 0f;

            // Update the total duration left
            totalDurationLeft = Mathf.Max(totalDurationLeft - Time.deltaTime, 0f);

        }
    }
    public List<MutationType> GetSelectedMutations(float skillPoint)
    {
        foreach (var rule in mutationSelectionRules)
        {
            if (skillPoint >= rule.minSkillPoint)
            {
                return SelectRandomMutations(rule.availableMutations, rule.mutationsToSelect);
            }
        }
        return new List<MutationType> { MutationType.None };
    }
    public List<MutationType> GetRandomMutations(int count)
    {
        // Get all possible MutationTypes (excluding None)
        List<MutationType> allMutations = new List<MutationType>();
        foreach (MutationType mutation in System.Enum.GetValues(typeof(MutationType)))
        {
            if (mutation != MutationType.None)
            {
                allMutations.Add(mutation);
            }
        }

        return SelectRandomMutations(allMutations, count);
    }
    private List<MutationType> SelectRandomMutations(List<MutationType> availableMutations, int count)
    {
        List<MutationType> selectedMutations = new List<MutationType>();
        List<MutationType> pool = new List<MutationType>(availableMutations);
        System.Random rand = new System.Random();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = rand.Next(pool.Count);
            selectedMutations.Add(pool[index]);
            pool.RemoveAt(index); // Prevent selecting the same mutation multiple times
        }

        return selectedMutations;
    }
}
