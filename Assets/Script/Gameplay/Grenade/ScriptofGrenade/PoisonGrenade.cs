using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGrenade : MonoBehaviour
{
    public float initialTickDamage = 1f; // Base tick damage
    public float tickInterval = 1f;     // Time between ticks
    public float poisonDuration = 5f;  // Duration of the poison after exiting the area
    public float damageBuildRate = 0.5f; // How much the build-up damage increases per second
    public float explosionRadius = 3f;  // Radius of the poison grenade's effect
    public float lifetime = 5f;         // Time before the grenade is destroyed

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
            // Start poison effect
            zombie.StartPoisonEffect(initialTickDamage, tickInterval, damageBuildRate, poisonDuration, this);
            affectedZombies.Add(zombie);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null && affectedZombies.Contains(zombie))
        {
            // Apply the accumulated poison damage
            zombie.StopPoisonEffect(this);
            affectedZombies.Remove(zombie);
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Stop tracking zombies still in the area
        foreach (Zombie zombie in affectedZombies)
        {
            zombie.StopPoisonEffect(this);
        }

        Destroy(this.gameObject);
    }
    private IEnumerator DestroyFailedsafe(float delay)
    {
         yield return new WaitForSeconds(delay+0.1f);
         Destroy(this.gameObject);
    }
}
