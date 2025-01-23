using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CaliberType
{
    Low,
    Medium,
    High,
    Shotgun
}

public class Bullet : MonoBehaviour
{
    public float damage;
    public CaliberType caliberType;
    private int penetrationCount = 0; // Tracks how many penetrations the bullet can do
    public float dropOffThreshold; 
    public float dropOffMultiplier = 0.4f;
    public int bulletID;
    private static int bulletIDCounter = 0;
    private Vector2 initialPosition;

    // This method will initialize penetration count based on the caliber
    private void Start()
    {
        bulletID = bulletIDCounter++;
        initialPosition = transform.position; 
    }
    public void InitializePenetration()
    {
        switch (caliberType)
        {
            case CaliberType.Low:
                penetrationCount = 0;  // Low caliber can't penetrate
                break;
            case CaliberType.Medium:
                penetrationCount = 1;  // Medium caliber can penetrate 1 target
                break;
            case CaliberType.High:
                penetrationCount = 4;  // High caliber can penetrate 2 targets
                break;
            case CaliberType.Shotgun:
                penetrationCount = 0;  // Shotgun pellets can't penetrate
                break;
        }
    }
    private void Update()
    {
        CheckDamageDropOff();
    }
     private void CheckDamageDropOff()
    {
        float distanceTraveled = Vector2.Distance(initialPosition, transform.position);

        if (distanceTraveled >= dropOffThreshold)
        {
            ApplyDropOff();
        }
    }
     private void ApplyDropOff()
    {
        
        damage *= dropOffMultiplier; // Reduce damage instantly
        enabled = false; // Disable further updates to prevent repeated application
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null)
        {
            // Apply damage based on the caliber type
            switch (caliberType)
            {
                case CaliberType.Low:
                    zombie.ZombieTakeDamage(damage, DamageType.LowcaliberBullet);
                    DDAdataCollector.Instance.OnBulletHit(bulletID); // Notify hit
                    Destroy(this.gameObject);
                    break;

                case CaliberType.Medium:
                    zombie.ZombieTakeDamage(damage, DamageType.MediumcaliberBullet);
                    DDAdataCollector.Instance.OnBulletHit(bulletID); // Notify hit
                    HandlePenetration();
                    break;

                case CaliberType.High:
                    zombie.ZombieTakeDamage(damage, DamageType.HighcalliberBullet);
                    DDAdataCollector.Instance.OnBulletHit(bulletID); // Notify hit
                    HandlePenetration();
                    break;

                case CaliberType.Shotgun:
                    zombie.ZombieTakeDamage(damage, DamageType.ShotgunPellet);
                    DDAdataCollector.Instance.OnBulletHit(bulletID); // Notify hit
                    Destroy(this.gameObject);
                    break;
            }
        }

        if (other.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }
    }

    private void HandlePenetration()
    {
        if (penetrationCount > 0)
        {
            penetrationCount--; // Decrease penetration count on hit
            switch (caliberType)
            {
                case CaliberType.Medium:
                damage *= 0.5f;
                break;
                case CaliberType.High:
                damage *= 0.8f;
                break; 
            }
        }
        else
        {
            Destroy(this.gameObject); // Destroy bullet if it has no penetrations left
        }
    }
    public int GetPenetrationCount()
    {
        return penetrationCount;
    }
}


