using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeapondNpc : Weapon
{
    public List<Zombie> listZombiesInRanageAttack;
    public Zombie targetAttack;
    // Start is called before the first frame update
    private void Update()
    {
        if (isReloading) return;

        if (listZombiesInRanageAttack.Count != 0)
        {
            TargetClosestZombieInRanageAttack();
        }
        else
        {
            targetAttack = null;
            return;
        }


        if (fullAuto)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }
    public void TargetClosestZombieInRanageAttack()
    {
        if (listZombiesInRanageAttack.Count == 0) return;

        Zombie zombieTarget = listZombiesInRanageAttack[0];
        Transform transformZombie = zombieTarget.transform;
        float closeDistance = Vector2.Distance(transformZombie.position, this.transform.position);

        foreach (Zombie zombie in listZombiesInRanageAttack)
        {
            float distance = Vector2.Distance(zombie.transform.position, this.transform.position);
            if (distance < closeDistance)
            {
                closeDistance = distance;
                transformZombie = zombie.transform;
                targetAttack = zombie;
            }
        }

        Vector2 directionShot = (transformZombie.position - this.transform.position).normalized;
        bulletDirection = directionShot;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null)
        {
            listZombiesInRanageAttack.Add(zombie);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null)
        {
            listZombiesInRanageAttack.Remove(zombie);
        }
    }
}
