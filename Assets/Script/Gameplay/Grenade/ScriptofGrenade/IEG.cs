using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IEG : MonoBehaviour
{
    public float damage;
    public float Delay;
    private Collider2D collider2D;
    private AudioSource audioSource; // Reference to the AudioSource

    void Start()
    {
        collider2D = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            float duration = Delay + 0.6f; // Total duration
            audioSource.pitch = 1f / duration; // Adjust pitch to match duration
            audioSource.Play(); // Start playing the sound
        }

        StartCoroutine(DestroyAfterDelay(Delay));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.ZombieTakeDamage(damage, DamageType.Explosive);
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
