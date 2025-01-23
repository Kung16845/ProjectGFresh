using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov : MonoBehaviour
{
    public float initialDamage;      // Damage dealt on impact
    public float tickDamage;         // Damage dealt per tick
    public float tickInterval = 1f;  // Time interval between ticks
    public float burnDuration = 5f;  // How long the burning lasts
    public float lifetime = 3f;      // Time before the Molotov is destroyed

    private HashSet<Zombie> affectedZombies = new HashSet<Zombie>();

    void Start()
    {
        StartCoroutine(DestroyAfterDelay(lifetime));
        StartCoroutine(DestroyFailedsafe(lifetime));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null && !affectedZombies.Contains(zombie))
        {
            // Apply initial damage
            zombie.ZombieTakeDamage(initialDamage, DamageType.Fire);

            // Add Molotov effect to zombie and track it
            zombie.ApplyBurnEffect(tickDamage, tickInterval, burnDuration, this);
            affectedZombies.Add(zombie);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null && affectedZombies.Contains(zombie))
        {
            zombie.ApplyBurnEffect(tickDamage, tickInterval, burnDuration, this);
            affectedZombies.Remove(zombie);
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (Zombie zombie in affectedZombies)
        {
            Debug.Log("ApplyDOT");
            zombie.ApplyBurnEffect(tickDamage, tickInterval, burnDuration, this);
            affectedZombies.Remove(zombie);
        }
        Destroy(this.gameObject);
    }
    private IEnumerator DestroyFailedsafe(float delay)
    {
         yield return new WaitForSeconds(delay+0.1f);
         Destroy(this.gameObject);
    }
}
