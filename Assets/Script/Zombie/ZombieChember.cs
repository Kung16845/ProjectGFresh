using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieChember : Zombie
{
    [Header("Speed Boost Settings")]
    public GameObject speedBoostAreaPrefab;
    public float areaRadius = 3f;
    public float areaDuration = 10f;
    public float speedIncreaseAmount = 1.5f;          // Movement speed multiplier
    public float attackSpeedIncreaseAmount = 1.5f;    // Attack speed multiplier
    public float speedIncreaseDuration = 5f;
    public void SetZombieCostumeId()
    {
        string mutationCode = GetMutationCode(mutationType);
        idZombieCoustume = $"30104{mutationCode}";
    }
    protected override void Start()
    {
        base.Start();
        SetZombieCostumeId();
    }
    protected override void InitializeDamageMultipliers()
    {
        base.InitializeDamageMultipliers(); // Initialize with default multipliers

        damageMultipliers[DamageType.Poison] = 0;
    }

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
                    attackSpeedIncreaseAmount, // Pass the attack speed increase
                    speedIncreaseDuration
                );
            }
        }
    }
    private void Update()
    {
        if (currentHp <= 0)
        {
            currentState = ZombieState.Dead;
            return;
        }
        if (HasReachedAttackPoint())
        {
            rb2D.linearVelocity = Vector2.zero;
            ZombieAttack();
        }
        else
        {
            ZombieMoveFindBarrier();
        }
    }
}
