using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public enum DamageType
{

    LowcaliberBullet,
    MediumcaliberBullet,
    ShotgunPellet,
    HighcalliberBullet,
    Pulse,
    Fire,
    Acid,
    Explosive,
    Poison,
    // Future damage types can be added here
}
public enum MutationType
{
    None,
    Spike,
    Acid,
    Exploder,
    ArmourShell
}
public enum ZombieState
{
    Moving,
    Attacking,
    Stopped,
    Dead,
    // Add other states as needed
}

public class Zombie : MonoBehaviour
{
    private Dictionary<object, PoisonEffect> activePoisonEffects = new Dictionary<object, PoisonEffect>();
    private Dictionary<object, BurnEffect> activeBurnEffects = new Dictionary<object, BurnEffect>();
    protected Dictionary<DamageType, float> damageMultipliers;
    private float speedMultiplier = 1f;     // Movement speed multiplier
    private float attackSpeedMultiplier = 1f;
    public float currentHp;
    public float maxHp;
    public float maxArmourHp;
    public float ArmourHp;
    public float currentSpeed;
    public float maxSpeed;
    public float attackDamage;
    public float attackTimer;
    public float countTimer;
    public Rigidbody2D rb2D;
    public Barrier barrier;
    public bool canmove;
    private Coroutine poisonCoroutine;
    private float buildUpDamage = 0f; // Accumulated poison damage
    public float movementSpeed = 1.0f;       // Movement speed
    private bool isapply;

    // Fields for Engaging Area
    public Lane currentLane;
    public bool isInEngagingArea = false;    // Whether the zombie is in the Engaging Area

    [Header("Damage Effects")]
    public float slowdownAmount = 0.25f;           // Amount to slow down (e.g., 0.5 means half speed)
    public float damageEffectDuration = 0.01f;       // Duration of the slowdown and red color effect

    private float originalSpeed;
    public AnimationControllerGrunt animationControllerGrunt;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine damageEffectCoroutine;
    private Animator animator;
    [Header("Mutation Settings")]
    public MutationType mutationType = MutationType.None;
     [Header("Allowed Mutations")]
    public bool allowSpikeMutation = true;
    public bool allowAcidMutation = true;
    public bool allowExploderMutation = true;
    public bool allowArmourShellMutation = true;
    [Range(1, 3)]
    public int mutationTier = 1;                  // Tier 1 to 3

    [Header("Acid Mutation Settings")]
    public GameObject acidPoolPrefab;
    public float[] acidBarrierDamage = { 5f, 7f, 9f };   // Damage per tick for barrier
    public float[] acidZombieDamage = { 7f, 10f, 15f };  // Damage per tick for zombies
    public float[] acidDuration = { 5f, 10f, 15f };      // Duration of acid pool
    public float[] acidRadius = { 1f, 2f, 2.5f };          // Effect radius per tier

    [Header("Exploder Mutation Settings")]
    public GameObject explosionPrefab;
    public float[] explosionBarrierDamage = { 50f, 70f, 100f };
    public float[] explosionZombieDamage = { 55f, 100f, 150f };
    public float[] explosionRadius = { 2f, 3f, 4f };

    [Header("State Tracking")]
    public ZombieState currentState;
    private float damageEffectDurationRemaining;
    private float previousDirectionX = 1f;
    [Header("DataTracker")]
    public DDAdataCollector ddadataCollector;
    [Header("ID Zombie Costume")]
    public string idZombieCoustume;
    [SerializeField]
    public GameObject hitMarkerPrefab; // Assign your hit marker prefab in the Inspector
    private RawImage[] hitMarkerImages;    // Array to store references to marker images
    private Coroutine hideMarkerCoroutine;
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        canmove = true;
        isapply = false;
        animationControllerGrunt =  GetComponent<AnimationControllerGrunt>();
        currentSpeed = maxSpeed;
        currentHp = maxHp;
        originalSpeed = maxSpeed;
        rb2D = GetComponent<Rigidbody2D>();
        InitializeDamageMultipliers();
        if (hitMarkerPrefab == null)
        {
            Transform canvasTransform = GameObject.Find("Canvas").transform; // Replace "Canvas" with your actual Canvas name
            hitMarkerPrefab = canvasTransform.Find("Hitmarker")?.gameObject; 
        }
        if (hitMarkerPrefab != null)
        {
            hitMarkerImages = hitMarkerPrefab.GetComponentsInChildren<RawImage>();
        }
    }
    public ZombieState CurrentState
    {
        get { return currentState; }
        private set { currentState = value; }
    }
    void Update()
    {
    }
    public void ZombieMoveFindBarrier()
    {
        // Move towards the attack point
        if (canmove)
        {
            if (currentLane != null && currentLane.attackPoint != null)
            {
                currentState = ZombieState.Moving;
                Vector2 direction = (currentLane.attackPoint.position - transform.position).normalized;
                rb2D.velocity = direction * currentSpeed;

                previousDirectionX = direction.x;
                // Flip the sprite based on movement direction along the x-axis
                if (direction.x < 0)
                {
                    transform.localScale = new Vector3(0.7f, 0.7f, 1); // Flip left
                }
                else if (direction.x > 0)
                {
                    transform.localScale = new Vector3(-0.7f, 0.7f, 1); // Face right
                }
            }
        }
    }
    public bool HasReachedAttackPoint()
    {
        if (currentLane != null && currentLane.attackPoint != null)
        {
            float distanceToAttackPoint = Vector2.Distance(transform.position, currentLane.attackPoint.position);
            float thresholdDistance = 0.1f;
            if (distanceToAttackPoint <= thresholdDistance)
            {
                // Zombie has reached the attack point, set to Attacking state
                if(animationControllerGrunt != null)
                {
                animationControllerGrunt.Isreach = true;
                }
                rb2D.velocity = Vector2.zero;
                currentState = ZombieState.Attacking;

                // Keep the sprite direction based on the previous movement
                if (previousDirectionX < 0)
                {
                    transform.localScale = new Vector3(0.7f, 0.7f, 1); // Keep facing left
                }
                else if (previousDirectionX > 0)
                {
                    transform.localScale = new Vector3(-0.7f, 0.7f, 1); // Keep facing right
                }
                return true;
            }
        }
        return false;
    }


    public void ZombieAttack()
    {
        if (barrier != null)
        {   
            if(mutationType == MutationType.Exploder)
            {
                ZombieTakeDamage(10000, DamageType.Explosive);
                int index = mutationTier - 1;
                barrier.BarrierTakeDamage(explosionBarrierDamage[index]);
                Debug.Log(explosionBarrierDamage[index]);
            }
            if (countTimer > 0)
            {
                countTimer -= Time.deltaTime * attackSpeedMultiplier;
            }
            else
            {
                SoundManager.Instance.PlaySound("ZombieAttackBarrier");
                barrier.BarrierTakeDamage(attackDamage);
                countTimer = attackTimer;
            }
        }
    }
        private IEnumerator HideHitMarkerAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // Adjust delay as needed
        hitMarkerPrefab.SetActive(false);
    }

    public virtual void ZombieTakeDamage(float damage, DamageType damageType, float extraMultiplier = 1f)
    {
        // Calculate adjusted damage based on multipliers
        float multiplier = 1f;
        if (damageMultipliers != null && damageMultipliers.ContainsKey(damageType))
        {
            multiplier = damageMultipliers[damageType];
        }
        float adjustedDamage = damage * multiplier * extraMultiplier;
        Color hitMarkerColor = Color.white; // Default color
        Color orange = new Color(185f / 255f, 128f / 255f, 64f / 255f);
        Color LightBlue = new Color(64f / 255f, 185f / 255f, 177f / 255f);

        if (ArmourHp > 0)
        {
            if (damageType == DamageType.HighcalliberBullet)
            {
                float reducedDamage = adjustedDamage * 0.85f;
                ArmourHp -= reducedDamage;
                ApplyOverflowDamageToHealth();
                hitMarkerColor = LightBlue; // Armor hit
            }
            else if (damageType == DamageType.MediumcaliberBullet)
            {
                float reducedDamage = adjustedDamage * 0.75f;
                ArmourHp -= reducedDamage;
                ApplyOverflowDamageToHealth();
                hitMarkerColor = LightBlue; // Armor hit
            }
            else if (damageType == DamageType.LowcaliberBullet || damageType == DamageType.ShotgunPellet)
            {
                float reducedDamage = adjustedDamage * 0.50f;
                ArmourHp -= reducedDamage;
                ApplyOverflowDamageToHealth();
                hitMarkerColor = LightBlue; // Armor hit
            }
            else if (damageType == DamageType.Explosive)
            {
                float damageToArmor = adjustedDamage * 0.60f;
                float damageToHealth = adjustedDamage * 0.40f;
                ArmourHp -= damageToArmor;
                currentHp -= damageToHealth;
                ApplyOverflowDamageToHealth();
            }
            else if (damageType == DamageType.Pulse)
            {
                ArmourHp = 0f;
                hitMarkerColor = LightBlue; // Armor hit
            }
            else
            {
                ArmourHp -= adjustedDamage;
                ApplyOverflowDamageToHealth();
            }
            SoundManager.Instance.PlaySound("ArmourHit");
        }
        else
        {
            SoundManager.Instance.PlaySound("FLeshhit");
            currentHp -= adjustedDamage;

            if (adjustedDamage < 10)
            {
                hitMarkerColor = Color.grey; // Low damage
            }
        }

        if (damageType == DamageType.Fire)
        {
            SoundManager.Instance.PlaySound("ZombieBurnt");
            hitMarkerColor = orange; // Fire damage
        }
        else if (damageType == DamageType.Poison || damageType == DamageType.Acid)
        {
            hitMarkerColor = Color.green; // Poison or acid
        }

        // Show the hit marker with the determined color
        ShowHitMarker(hitMarkerColor);
        FlashSpritesRed();
        ApplyDamageEffects(damageType);
        CheckForDeath();
    }
    private bool isFlashing = false; 
    private float flashCooldown = 0.5f; 
    private void FlashSpritesRed()
    {
        if (isFlashing) return; // Prevent multiple calls during cooldown

        isFlashing = true;

        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            Color originalColor = sprite.color;
            sprite.DOColor(Color.red, 0.2f) // Tween to red over 0.2 seconds
                .OnComplete(() =>
                {
                    sprite.DOColor(originalColor, 0.2f); // Return to the original color over 0.8 seconds
                });
        }

        // Set a delay to reset the cooldown flag
        StartCoroutine(ResetFlashingCooldown());
    }

    private IEnumerator ResetFlashingCooldown()
    {
        yield return new WaitForSeconds(flashCooldown); // Wait for cooldown duration
        isFlashing = false; // Allow the function to be called again
    }
    private IEnumerator ArmourBroken()
    {
        canmove = false;
        yield return new WaitForSeconds(3f);
        canmove = true;
    }
    private void ApplyOverflowDamageToHealth()
    {
        if (ArmourHp < 0)
        {
            currentHp += ArmourHp; // ArmourHp is negative
            ArmourHp = 0f;
            StartCoroutine(ArmourBroken());
        }
    }

    private void ApplyDamageEffects(DamageType damageType)
    {
        if (damageType != DamageType.Acid && damageType != DamageType.Fire && damageType != DamageType.Poison)
        {
            if (damageEffectCoroutine != null)
            {
                StopCoroutine(damageEffectCoroutine);
                speedMultiplier = 1.0f;
                UpdateCurrentSpeed();
            }
            damageEffectCoroutine = StartCoroutine(DamageEffect());
        }
    }

    private void CheckForDeath()
    {
        if (currentHp <= 0)
        {
            OnDeath();
            DDAdataCollector.Instance.OnZombieKilled(); // Notify the data collector
            StartCoroutine(DelayDead());
        }
    }
    private IEnumerator DelayDead()
    {
        if(animationControllerGrunt != null)
        {
            animationControllerGrunt.IsDead = true; 
            
            currentState = ZombieState.Dead;
            canmove = false;
            rb2D.velocity = Vector2.zero; // Immediately stop movement

            countTimer = Mathf.Infinity; 
            DisableCollider(); // Prevent interactions
            // Get the Animator component from the AnimationControllerGrunt
            Animator animator = animationControllerGrunt.GetComponent<Animator>();

            // Ensure the animator exists and wait for the death animation to finish
            if (animator != null)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                while (stateInfo.normalizedTime < 1.0f || !stateInfo.IsName("Zombie_Dead"))
                {
                    yield return null; // Wait for the animation to finish
                    stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                }
            }
            else
            {
                Debug.LogError("Animator not found on AnimationControllerGrunt!");
            }
            Destroy(this.gameObject);
        }
    }

    private IEnumerator DamageEffect()
    {
        // Slow down the zombie
        if(animationControllerGrunt != null)
        {
            if(!animationControllerGrunt.IsDead)
            {
            speedMultiplier = slowdownAmount;
            UpdateCurrentSpeed();
            }
        }
        // Change sprite color to red
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

        // Wait for the effect duration
        yield return new WaitForSeconds(damageEffectDuration);

        // Reset speed multiplier to original value
        speedMultiplier = 1.0f;
        UpdateCurrentSpeed();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        damageEffectCoroutine = null;
    }

    protected virtual void OnDeath()
    {
        if (currentState == ZombieState.Dead) return;
        currentState = ZombieState.Dead;
        switch (mutationType)
        {
            case MutationType.Acid:
                ApplyAcidDeathEffect();
                break;
            case MutationType.Exploder:
                ApplyExploderDeathEffect();
                break;
                // Other mutations have no death effect
        }
    }
    private void ApplyMutationEffects()
    {
        switch (mutationType)
        {
            case MutationType.Spike:
                ApplySpikeMutation();
                maxHp += 75;
                break;
            case MutationType.ArmourShell:
                ApplyArmourShellMutation();
                break;
            case MutationType.Acid:
                maxHp += 150;
                break;
            case MutationType.Exploder:
                maxHp += 50;
                break;
                // Acid and Exploder mutations have effects on death
        }
    }
    private void ApplySpikeMutation()
    {
        float damageMultiplier = 1f;
        switch (mutationTier)
        {
            case 1:
                damageMultiplier = 1.3f;
                break;
            case 2:
                damageMultiplier = 1.45f;
                break;
            case 3:
                damageMultiplier = 1.8f;
                break;
        }
        attackDamage *= damageMultiplier;
    }
    private void ApplyArmourShellMutation()
    {
        float extraArmourHp = 0f;
        switch (mutationTier)
        {
            case 1:
                extraArmourHp = 50f;
                break;
            case 2:
                extraArmourHp = 100f;
                break;
            case 3:
                extraArmourHp = 150f;
                break;
        }
        ArmourHp += extraArmourHp;
        maxArmourHp += extraArmourHp;
    }
    private void ApplyAcidDeathEffect()
    {
        // Instantiate an acid pool at the zombie's position
        if (acidPoolPrefab != null)
        {
            GameObject acidPool = Instantiate(acidPoolPrefab, transform.position, Quaternion.identity);
            AcidPool acidPoolScript = acidPool.GetComponent<AcidPool>();
            if (acidPoolScript != null)
            {
                int index = mutationTier - 1; // Adjust for array indexing
                acidPoolScript.Initialize(
                    acidBarrierDamage[index],
                    acidZombieDamage[index],
                    acidDuration[index],
                    acidRadius[index],
                    1// Damage interval
                );
            }
        }
    }

    private void ApplyExploderDeathEffect()
    {
        // Instantiate an explosion at the zombie's position
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
            Explosion explosionScript = explosion.GetComponent<Explosion>();
            if (explosionScript != null)
            {
                int index = mutationTier - 1; // Adjust for array indexing
                explosionScript.Initialize(
                    explosionBarrierDamage[index],
                    explosionZombieDamage[index],
                    explosionRadius[index]
                );
            }
        }
    }
    private class BurnEffect
    {
        public float tickDamage;
        public float tickInterval;
        public float durationRemaining;
        public Coroutine burnCoroutine;
    }
    public void ApplyBurnEffect(float tickDamage, float tickInterval, float duration, object source)
    {
        if (activeBurnEffects.ContainsKey(source))
        {
            // Refresh existing burn effect
            activeBurnEffects[source].durationRemaining = duration;
        }
        else
        {
            // Create a new burn effect
            BurnEffect newEffect = new BurnEffect
            {
                tickDamage = tickDamage,
                tickInterval = tickInterval,
                durationRemaining = duration
            };
            newEffect.burnCoroutine = StartCoroutine(HandleBurnEffect(newEffect));
            activeBurnEffects.Add(source, newEffect);
        }
    }

    private IEnumerator HandleBurnEffect(BurnEffect burnEffect)
    {
        while (burnEffect.durationRemaining > 0f)
        {
            ZombieTakeDamage(burnEffect.tickDamage, DamageType.Fire);
            yield return new WaitForSeconds(burnEffect.tickInterval);
            burnEffect.durationRemaining -= burnEffect.tickInterval;
        }

        // Automatically clean up when the effect ends
        foreach (var pair in activeBurnEffects)
        {
            if (pair.Value == burnEffect)
            {
                activeBurnEffects.Remove(pair.Key);
                break;
            }
        }
    }
    private class PoisonEffect
    {
        public float tickDamage;
        public float tickInterval;
        public float damageBuildRate;
        public float durationRemaining;
        public float accumulatedDamage;
        public Coroutine poisonCoroutine;
        public Coroutine buildUpCoroutine;
    }
    public void StartPoisonEffect(float tickDamage, float tickInterval, float damageBuildRate, float poisonDuration, object source)
    {
        if (activePoisonEffects.ContainsKey(source))
        {
            // Refresh poison effect
            PoisonEffect effect = activePoisonEffects[source];
            effect.durationRemaining = poisonDuration;
        }
        else
        {
            // Create new poison effect
            PoisonEffect newEffect = new PoisonEffect
            {
                tickDamage = tickDamage,
                tickInterval = tickInterval,
                damageBuildRate = damageBuildRate,
                durationRemaining = poisonDuration,
                accumulatedDamage = 0f
            };

            newEffect.poisonCoroutine = StartCoroutine(HandlePoisonTick(newEffect));
            newEffect.buildUpCoroutine = StartCoroutine(AccumulateBuildUpDamage(newEffect));
            activePoisonEffects.Add(source, newEffect);
        }
    }

    public void StopPoisonEffect(object source)
    {
        if (activePoisonEffects.ContainsKey(source))
        {
            PoisonEffect effect = activePoisonEffects[source];

            // Stop poison tick and build-up accumulation
            if (effect.poisonCoroutine != null) StopCoroutine(effect.poisonCoroutine);
            if (effect.buildUpCoroutine != null) StopCoroutine(effect.buildUpCoroutine);

            // Apply accumulated build-up damage over the remaining duration
            if (effect.accumulatedDamage > 0)
            {
                StartCoroutine(ApplyBuildUpDamage(effect.accumulatedDamage, effect.durationRemaining));
            }

            activePoisonEffects.Remove(source);
        }
    }

    private IEnumerator HandlePoisonTick(PoisonEffect effect)
    {
        while (true)
        {
            ZombieTakeDamage(effect.tickDamage, DamageType.Poison);
            yield return new WaitForSeconds(effect.tickInterval);
        }
    }

    private IEnumerator AccumulateBuildUpDamage(PoisonEffect effect)
    {
        while (true)
        {
            effect.accumulatedDamage += effect.damageBuildRate * effect.tickInterval;
            yield return new WaitForSeconds(effect.tickInterval);
        }
    }

    private IEnumerator ApplyBuildUpDamage(float totalDamage, float duration)
    {
        float damagePerTick = totalDamage;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ZombieTakeDamage(damagePerTick, DamageType.Poison);
            yield return new WaitForSeconds(1f); // Apply damage every second
            elapsed += 1f;
        }
    }
    protected virtual void InitializeDamageMultipliers()
    {
        damageMultipliers = new Dictionary<DamageType, float>
        {
            { DamageType.HighcalliberBullet, 1f },
            { DamageType.LowcaliberBullet, 1f },
            { DamageType.MediumcaliberBullet, 1f },
            { DamageType.ShotgunPellet, 1f },
            { DamageType.Pulse, 1f },
            { DamageType.Fire, 1f },
            { DamageType.Acid, 1f },
            { DamageType.Explosive, 1f },
        };
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        Barrier triggerbarrier = other.GetComponent<Barrier>();
        if (triggerbarrier != null)
        {
            barrier = triggerbarrier;
        }
    }
    public void IncreaseSpeed(float multiplier)
    {
        if(!isapply)
        {
            speedMultiplier *= multiplier;
            UpdateAnimationSpeed();
            UpdateCurrentSpeed();
            isapply= true;
        }
    }

    public void ResetSpeed()
    {
        speedMultiplier = 1f;
        UpdateCurrentSpeed();
        UpdateAnimationSpeed();
    }
    private void UpdateAnimationSpeed()
    {
        if (animator != null)
        {
            animator.speed = attackSpeedMultiplier;
        }
    }
    private void UpdateCurrentSpeed()
    {
        currentSpeed = maxSpeed * speedMultiplier;
    }
    public void IncreaseAttackSpeed(float multiplier)
    {
        if(!isapply)
        {
        attackSpeedMultiplier *= multiplier;
        }
    }

    public void ResetAttackSpeed()
    {
        attackSpeedMultiplier = 1f;
    }
    public virtual void SetLane(Lane lane)
    {
        currentLane = lane;

        // Get the Barrier component from the attack point
        if (currentLane.attackPoint != null)
        {
            barrier = currentLane.attackPoint.GetComponent<Barrier>();
        }
    }
    public void SetTier(int tier)
    {
        mutationTier = tier;
        ApplyMutationEffects();
    }
    public void StopZombie()
    {
        currentState = ZombieState.Stopped;
        rb2D.velocity = Vector2.zero;
        // Additional logic for stopping
    }
    public void ResumeZombie()
    {
        currentState = ZombieState.Moving;
        // Additional logic for resuming movement
    }
    private void OnEnable()
    {
        ManagerZombie.Instance.RegisterZombie(this);
    }

    private void OnDisable()
    {
        ManagerZombie.Instance.UnregisterZombie(this);
    }
    public void DisableCollider()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
    public void SetMutationType(MutationType mutationType)
    {
        if (IsMutationAllowed(mutationType))
        {
            this.mutationType = mutationType;
        }
        else
        {
            Debug.Log($"Mutation {mutationType} is not allowed. Setting mutation type to None.");
            this.mutationType = MutationType.None;
        }

        ApplyMutationEffects(); // Apply the mutation effects
    }
    private bool IsMutationAllowed(MutationType mutationType)
    {
        switch (mutationType)
        {
            case MutationType.Spike:
                return allowSpikeMutation;
            case MutationType.Acid:
                return allowAcidMutation;
            case MutationType.Exploder:
                return allowExploderMutation;
            case MutationType.ArmourShell:
                return allowArmourShellMutation;
            case MutationType.None:
                return true; // Always allow 'None'
            default:
                return true; // Default to true for any future mutations
        }
    }
    public MutationType GetMutationType()
    {
        return mutationType;
    }
    public string GetMutationCode(MutationType mutation)
    {
        switch (mutation)
        {
            case MutationType.None:
                return "01";
            case MutationType.Spike:
                return "02";
            case MutationType.Acid:
                return "03";
            case MutationType.Exploder:
                return "04";
            case MutationType.ArmourShell:
                return "05";
            default:
                return "00"; // Default for undefined mutations
        }
    }
    private void ShowHitMarker(Color markerColor)
    {
        if (hitMarkerPrefab == null || hitMarkerImages == null)
            return;

        // Set the color for each marker RawImage
        foreach (var rawImage in hitMarkerImages)
        {
            rawImage.color = markerColor;
        }

        // Activate the hit marker object
        hitMarkerPrefab.SetActive(true);

        // Reset and start a coroutine to hide the hit marker after a short delay
        if (hideMarkerCoroutine != null)
        {
            StopCoroutine(hideMarkerCoroutine);
        }
        hideMarkerCoroutine = StartCoroutine(HideHitMarkerAfterDelay());
    }

}
