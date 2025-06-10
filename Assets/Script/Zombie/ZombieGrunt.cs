using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGrunt : Zombie
{   
    public bool Isrunner;
    private string zombietype; // Type of zombie (e.g., "02" for grunts)

    protected override void Start()
    {
        base.Start();
        SetZombieCostumeId();
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

    public void SetZombieCostumeId()
    {
        if(Isrunner)
        {
            zombietype = "08";
        }
        else
        {
            zombietype = "01";
        }
        string mutationCode = GetMutationCode(mutationType);
        idZombieCoustume = $"301{zombietype}{mutationCode}";
    }
}
