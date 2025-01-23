using System.Collections.Generic;
using UnityEngine;

public class DDAdataCollector : MonoBehaviour
{
    public static DDAdataCollector Instance;

    // Public fields to store the data

    public float killPerMinute; // Now represents average KPM
    public float accuracy;
    public int multiKillCount;
    public float barrierDamage;
    public float valueFail;

    // Private fields for calculations
    private int totalKills;
    private int totalBulletsFired;
    private int totalBulletsHit;

    // Keep track of which bullets have already counted as hits
    private HashSet<int> bulletsThatHit;

    // Variable to track game start time
    private float gameStartTime;

    // List for multi-kill tracking (kept as per original functionality)
    [SerializeField]
    private List<float> recentKills = new List<float>();

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            bulletsThatHit = new HashSet<int>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize game start time
        gameStartTime = Time.time;

        // Reference to SaveDataDDA to assign this collector
        SaveDataDDA saveDataDDA = FindObjectOfType<SaveDataDDA>();
        if (saveDataDDA != null)
        {
            saveDataDDA.scriptDDAdataCollector = this;
        }
        else
        {
            Debug.LogError("SaveDataDDA instance not found in the scene.");
        }
    }

    private void Update()
    {
        UpdateKillPerMinute();
        UpdateAccuracy();
    }

    /// <summary>
    /// Calculates the average Kill Per Minute (KPM) since the game started.
    /// </summary>
    private void UpdateKillPerMinute()
    {
        float elapsedTime = Time.time - gameStartTime;
        if (elapsedTime > 0f)
        {
            killPerMinute = totalKills / (elapsedTime / 60f);
        }
        else
        {
            killPerMinute = 0f;
        }
    }

    /// <summary>
    /// Calculates the shooting accuracy based on bullets fired and bullets hit.
    /// </summary>
    private void UpdateAccuracy()
    {
        if (totalBulletsFired > 0)
        {
            // Ensure totalBulletsHit does not exceed totalBulletsFired
            int hits = Mathf.Min(totalBulletsHit, totalBulletsFired);
            accuracy = (float)hits / totalBulletsFired * 100f;
        }
        else
        {
            accuracy = 0f;
        }
    }

    /// <summary>
    /// Method to be called when a bullet is fired.
    /// </summary>
    /// <param name="bulletID">Unique identifier for the bullet.</param>
    public void OnBulletFired(int bulletID)
    {
        totalBulletsFired++;
        bulletsThatHit.Remove(bulletID); // Ensure it's not already counted
    }

    /// <summary>
    /// Method to be called when a bullet hits a zombie.
    /// </summary>
    /// <param name="bulletID">Unique identifier for the bullet.</param>
    public void OnBulletHit(int bulletID)
    {
        // Only increment totalBulletsHit if this bullet hasn't been counted as a hit yet
        if (!bulletsThatHit.Contains(bulletID))
        {
            totalBulletsHit++;
            bulletsThatHit.Add(bulletID);
        }
    }

    /// <summary>
    /// Method to be called when a zombie is killed.
    /// </summary>
    public void OnZombieKilled()
    {
        totalKills++;
        float currentTime = Time.time;

        // Add the kill timestamp for multi-kill tracking
        recentKills.Add(currentTime);

        // Remove kills that happened more than 3 seconds ago for multi-kill
        recentKills.RemoveAll(t => t < currentTime - 3f);

        // Check if a multi-kill has been achieved
        if (recentKills.Count >= 8)
        {
            multiKillCount++;
            recentKills.Clear(); // Reset the recent kills to avoid overlapping counts
        }
    }

    /// <summary>
    /// Method to be called when the barrier takes damage.
    /// </summary>
    /// <param name="damage">Amount of damage taken by the barrier.</param>
    public void OnBarrierDamage(float damage)
    {
        barrierDamage += damage;
    }

    /// <summary>
    /// Gathers all relevant data into a list.
    /// </summary>
    /// <returns>List containing KPM, accuracy, multi-kill count, and barrier damage.</returns>
    public List<float> GetData()
    {
        List<float> data = new List<float>
        {
            killPerMinute,
            accuracy,
            multiKillCount,
            barrierDamage
        };
        return data;
    }
}
