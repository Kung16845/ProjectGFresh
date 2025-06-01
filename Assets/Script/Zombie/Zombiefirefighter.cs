using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombiefirefighter : Zombie
{
    protected override void Start()
    {
        base.Start();
        SetZombieCostumeId();
        InitializeDamageMultipliers();
    }
    public void SetZombieCostumeId()
    {
        string mutationCode = GetMutationCode(mutationType);
        idZombieCoustume = $"30102{mutationCode}";
    }
    protected override void InitializeDamageMultipliers()
    {
        base.InitializeDamageMultipliers(); // Initialize with default multipliers

        // Set Fire damage multiplier to 0 (immune to fire)
        damageMultipliers[DamageType.Fire] = 0f;
    }
    void Update()
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
