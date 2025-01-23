using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChargingZombieState
{
    Charging,
    Boosted,
    Stopped,
    Normal
}
public class ZombieCharger : Zombie
{
    [Header("Charging Zombie Settings")]
    public float chargeUpTime = 2f;             // Time to charge up before speeding up
    public float boostedSpeed = 5f;             // Speed during the boosted phase
    public float boostedDamage = 50f;           // Damage when hitting during boosted phase
    public float normalDamage = 10f;            // Normal attack damage
    public float damageThreshold = 20f;         // Damage amount that triggers weakness
    public float stopDuration = 3f;             // Time to stop when weakness is triggered

    [Header("Damage Resistance and Weakness")]
    public float bulletDamageReduction = 0.5f;     // Reduce bullet damage by 50%
    public float weaknessDamageMultiplier = 1.5f;  // Damage multiplier when in weakness state

    [Header("State Tracking (For Debugging)")]
    [SerializeField] private ChargingZombieState ChargercurrentState;
    [SerializeField] private float chargeTimer = 0f;
    [SerializeField] private float stopTimer = 0f;
    [SerializeField] private float accumulatedDamage = 0f;
    public AnimationControllerCharger animationControllerCharger;

    // Existing fields...
    private Color originalColor;
    public void SetZombieCostumeId()
    {
        string mutationCode = GetMutationCode(mutationType);
        idZombieCoustume = $"30107{mutationCode}";
    }
    protected override void Start()
    {
        base.Start();
        SetZombieCostumeId();
        ChargercurrentState= ChargingZombieState.Charging;
        chargeTimer = chargeUpTime;
        attackDamage = normalDamage;  // Set initial attack damage
        currentSpeed = 0f;            // Start stationary

        // Initialize components

        InitializeDamageMultipliers();
    }

    private void Update()
    {
        CheckForDeathCharger();
        switch (ChargercurrentState)
        {
            case ChargingZombieState.Stopped:
                HandleStoppedState();
                break;
            case ChargingZombieState.Charging:
                HandleChargingState();
                break;
            case ChargingZombieState.Boosted:
                HandleBoostedState();
                break;
            case ChargingZombieState.Normal:
                 if (HasReachedAttackPoint())
                {
                    rb2D.velocity = Vector2.zero;
                    animationControllerCharger.Ischarged = false;
                    animationControllerCharger.Isstunt = false;
                    animationControllerCharger.Isreach = true;
                    ZombieAttack();
                }
                break;
        }
    }
    private void CheckForDeathCharger()
    {
        if (currentHp <= 0)
        {
            animationControllerCharger.Ischarged = false;
            animationControllerCharger.Isreach = false;
            StartCoroutine(DelayDead());
        }
    }
    private IEnumerator DelayDead()
    {
            animationControllerCharger.IsDead = true; 
            animationControllerCharger.Isstunt = false;
            
            currentState = ZombieState.Dead;
            canmove = false;
            rb2D.velocity = Vector2.zero; // Immediately stop movement

            countTimer = Mathf.Infinity; 
            DisableCollider(); // Prevent interactions
            // Get the Animator component from the animationControllerCharger
            Animator animator = animationControllerCharger.GetComponent<Animator>();

            // Ensure the animator exists and wait for the death animation to finish
            if (animator != null)
            {
                Debug.Log("Checknull");
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                while (stateInfo.normalizedTime < 1.0f || !stateInfo.IsName("Zombie_Dead"))
                {
                    yield return null; // Wait for the animation to finish
                    stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                }
            }
            else
            {
                Debug.LogError("Animator not found on animationControllerCharger!");
            }
            Destroy(this.gameObject);
    }
    private void HandleStoppedState()
    {
        stopTimer -= Time.deltaTime;
        if (stopTimer <= 0f)
        {
            // Transition back to charging state
            ChargercurrentState = ChargingZombieState.Charging;
            chargeTimer = chargeUpTime;
            accumulatedDamage = 0f;
            currentSpeed = 0f;
            attackDamage = normalDamage;
        }
        else
        {
            animationControllerCharger.Ischarged = false;
            if(!animationControllerCharger.IsDead)
            {
                animationControllerCharger.Isstunt = true;
            }
            rb2D.velocity = Vector2.zero;
        }
    }
    private void HandleChargingState()
    {
        chargeTimer -= Time.deltaTime;
        if (chargeTimer <= 0f)
        {
            // Transition to boosted phase
            ChargercurrentState = ChargingZombieState.Boosted;
            currentSpeed = boostedSpeed;
            attackDamage = boostedDamage;
        }
        else
        {
            // Optional: Show charging animation or effects
            currentSpeed = 0f; // Remain stationary during charging
        }

        // Move towards the barrier (if desired during charging)
        ZombieMoveFindBarrier();
    }
    private void HandleBoostedState()
    {
        animationControllerCharger.Isstunt = false;
        animationControllerCharger.Ischarged = true;
        if (HasReachedAttackPoint())
        {
            ChargercurrentState = ChargingZombieState.Normal;
            rb2D.velocity = Vector2.zero;
            animationControllerCharger.Ischarged = false;
            animationControllerCharger.Isstunt = false;
            animationControllerCharger.Isreach = true;
            ZombieAttack();
        }
    }

    protected override void InitializeDamageMultipliers()
    {
        base.InitializeDamageMultipliers();
        damageMultipliers[DamageType.LowcaliberBullet] = bulletDamageReduction;
        damageMultipliers[DamageType.MediumcaliberBullet] = bulletDamageReduction;
        damageMultipliers[DamageType.HighcalliberBullet] = bulletDamageReduction;
    }
    public override void ZombieTakeDamage(float damage, DamageType damageType, float extraMultiplier = 1f)
    {
        // Apply damage multiplier when in weakness state
        if (ChargercurrentState == ChargingZombieState.Stopped)
        {
            extraMultiplier *= weaknessDamageMultiplier;
            Debug.Log($"Weakness state: applying damage multiplier of {weaknessDamageMultiplier}");
        }

        // Call base class method with the extra multiplier
        base.ZombieTakeDamage(damage, damageType, extraMultiplier);

        if (ChargercurrentState == ChargingZombieState.Stopped)
        {
            // Already in weakness state, no need to accumulate damage
            return;
        }

        // Check for weaknesses
        if (damageType == DamageType.Explosive || damageType == DamageType.Pulse)
        {
            TriggerWeakness();
        }
        else
        {
            // Accumulate damage
            accumulatedDamage += damage;

            if (accumulatedDamage >= damageThreshold)
            {
                TriggerWeakness();
            }
        }
    }
    private void TriggerWeakness()
    {
        // Stop the zombie
        ChargercurrentState = ChargingZombieState.Stopped;
        stopTimer = stopDuration;
        currentSpeed = 0f;
        rb2D.velocity = Vector2.zero;
        attackDamage = normalDamage;
        accumulatedDamage = 0f;

        // Remove bullet damage reduction during weakness
        bulletDamageReduction = 1f; // No reduction
        damageMultipliers[DamageType.LowcaliberBullet] = bulletDamageReduction;
        damageMultipliers[DamageType.MediumcaliberBullet] = bulletDamageReduction;
        damageMultipliers[DamageType.HighcalliberBullet] = bulletDamageReduction;
        // Optionally: Play a stunned animation or effect
    }

}
