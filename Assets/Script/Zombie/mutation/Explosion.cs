using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private float barrierDamage;
    private float zombieDamage;
    private float radius;
    private Barrier barrier;
    private Collider2D collider2D;
    void Start()
    {
        collider2D = GetComponent<Collider2D>();
        StartCoroutine(DestroyAfterDelay(0.5f));
    }

    public void Initialize(float barrierDamage, float zombieDamage, float radius)
    {
        this.barrierDamage = barrierDamage;
        this.zombieDamage = zombieDamage;
        this.radius = radius;
        this.transform.localScale = new Vector3(radius * 2, radius * 2, 1f);
        CheckForOverlappingBarrier();
        StartCoroutine(CreateArea());
    }
    private IEnumerator CreateArea()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    private void CheckForOverlappingBarrier()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D collider in colliders)
        {
            Barrier barrierComponent = collider.GetComponent<Barrier>();
            if (barrierComponent != null)
            {
                barrier = barrierComponent;
                barrierComponent.BarrierTakeDamage(barrierDamage);
                break; 
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Barrier barrierComponent = other.GetComponent<Barrier>();
        if (barrierComponent != null)
        {
            Debug.Log("Barrierfound");
            barrierComponent.BarrierTakeDamage(barrierDamage);
        }
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.ZombieTakeDamage(zombieDamage, DamageType.Explosive);
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
