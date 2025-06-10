using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieRiotshield : Zombie
{
    protected override void Start()
    {
        base.Start();
        SetZombieCostumeId();
    }
    public void SetZombieCostumeId()
    {
        string mutationCode = GetMutationCode(mutationType);
        idZombieCoustume = $"30109{mutationCode}";
    }
    protected override void InitializeDamageMultipliers()
    {
        base.InitializeDamageMultipliers();
        damageMultipliers[DamageType.Fire] = 0.8f;
        damageMultipliers[DamageType.LowcaliberBullet] = 0.25f;
        damageMultipliers[DamageType.Acid] = 1.5f;
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
