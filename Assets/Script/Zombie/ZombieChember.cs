using UnityEngine;

public class ZombieChember : Zombie
{
    [Header("Speed Boost Settings")]
    public GameObject speedBoostAreaPrefab;
    public float areaRadius = 3f;
    public float areaDuration = 10f;
    public float speedIncreaseAmount = 1.5f;
    public float attackSpeedIncreaseAmount = 1.5f;
    public float speedIncreaseDuration = 5f;

    protected override void OnDeath()
    {
        SpawnSpeedBoostArea();
    }

    private void SpawnSpeedBoostArea()
    {
        if (speedBoostAreaPrefab != null)
        {
            GameObject area = Instantiate(speedBoostAreaPrefab, transform.position, Quaternion.identity);
            SpeedBoostArea areaScript = area.GetComponent<SpeedBoostArea>();
            if (areaScript != null)
            {
                areaScript.Initialize(
                    areaRadius,
                    areaDuration,
                    speedIncreaseAmount,
                    attackSpeedIncreaseAmount,
                    speedIncreaseDuration
                );
            }
        }
    }
}
