using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPool : MonoBehaviour
{
    private float barrierDamagePerTick;
    private float zombieDamagePerTick;
    private float duration;
    private float radius;
    private float damageInterval;

    private Dictionary<Zombie, Coroutine> zombieDamageCoroutines = new Dictionary<Zombie, Coroutine>();
    private Coroutine barrierDamageCoroutine;
    private Barrier barrier;
    private Collider2D collider2D;
    
    void Start()
    {
        collider2D = GetComponent<Collider2D>();
        StartCoroutine(DestroyAfterDelay(0.5f));
    }
    public void Initialize(float barrierDamage, float zombieDamage, float duration, float radius, float interval)
    {
        this.barrierDamagePerTick = barrierDamage;
        this.zombieDamagePerTick = zombieDamage;
        this.duration = duration;
        this.radius = radius;
        this.damageInterval = interval;

        // Set the scale of the acid pool based on the radius
        this.transform.localScale = new Vector3(radius * 2, radius * 2, 1f);

        // Start the coroutine to manage the acid pool's lifetime
        StartCoroutine(ApplyDamageOverTime());

        // Check for overlapping barrier at initialization
        CheckForOverlappingBarrier();
    }

    private void CheckForOverlappingBarrier()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D collider in colliders)
        {
            Barrier barrierComponent = collider.GetComponent<Barrier>();
            if (barrierComponent != null && barrierDamageCoroutine == null)
            {
                barrier = barrierComponent;
                barrierDamageCoroutine = StartCoroutine(ApplyDamageOverTimeToBarrier(barrier));
                break; // Assuming there's only one barrier to affect
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null && !zombieDamageCoroutines.ContainsKey(zombie))
        {
            Coroutine coroutine = StartCoroutine(ApplyDamageOverTimeToZombie(zombie));
            zombieDamageCoroutines.Add(zombie, coroutine);
        }

        Barrier barrierComponent = other.GetComponent<Barrier>();
        if (barrierComponent != null && barrierDamageCoroutine == null)
        {
            barrier = barrierComponent;
            barrierDamageCoroutine = StartCoroutine(ApplyDamageOverTimeToBarrier(barrier));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null && zombieDamageCoroutines.ContainsKey(zombie))
        {
            StopCoroutine(zombieDamageCoroutines[zombie]);
            zombieDamageCoroutines.Remove(zombie);
        }

        Barrier barrierComponent = other.GetComponent<Barrier>();
        if (barrierComponent != null && barrierDamageCoroutine != null)
        {
            StopCoroutine(barrierDamageCoroutine);
            barrierDamageCoroutine = null;
            barrier = null;
        }
    }

    private IEnumerator ApplyDamageOverTimeToZombie(Zombie zombie)
    {
        while (zombie != null && zombie.gameObject != null)
        {
            zombie.ZombieTakeDamage(zombieDamagePerTick, DamageType.Acid);
            yield return new WaitForSeconds(damageInterval);
        }
        zombieDamageCoroutines.Remove(zombie);
    }

    private IEnumerator ApplyDamageOverTimeToBarrier(Barrier barrier)
    {
        while (barrier != null)
        {
            barrier.BarrierTakeDamage(barrierDamagePerTick);
            yield return new WaitForSeconds(damageInterval);
        }
    }

    private IEnumerator ApplyDamageOverTime()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Destroy the acid pool after its duration
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        // Stop all coroutines to prevent errors
        foreach (var coroutine in zombieDamageCoroutines.Values)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        zombieDamageCoroutines.Clear();

        if (barrierDamageCoroutine != null)
        {
            StopCoroutine(barrierDamageCoroutine);
            barrierDamageCoroutine = null;
        }
    }
     private IEnumerator DestroyAfterDelay(float Delay)
    {
        yield return new WaitForSeconds(Delay);
        collider2D.enabled = false;
        yield return new WaitForSeconds(0.60f);
        Destroy(this.gameObject);
    }
}
