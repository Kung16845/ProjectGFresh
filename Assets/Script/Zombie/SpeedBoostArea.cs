using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostArea : MonoBehaviour
{
    private float areaRadius;
    private float areaDuration;
    private float speedIncreaseAmount;
    private float speedIncreaseDuration;
    private float attackSpeedIncreaseAmount;

    private CircleCollider2D circleCollider;
    private Dictionary<Zombie, Coroutine> affectedZombies = new Dictionary<Zombie, Coroutine>();

     public void Initialize(float radius, float duration, float speedIncrease, float attackSpeedIncrease, float speedDuration)
    {
        this.areaRadius = radius;
        this.areaDuration = duration;
        this.speedIncreaseAmount = speedIncrease;
        this.attackSpeedIncreaseAmount = attackSpeedIncrease; // Set attack speed increase
        this.speedIncreaseDuration = speedDuration;
        this.transform.localScale = new Vector3(radius, radius, 1f);

        // Start the area duration coroutine
        StartCoroutine(AreaDurationCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null && !affectedZombies.ContainsKey(zombie))
        {
            Coroutine coroutine = StartCoroutine(ApplySpeedBoost(zombie));
            affectedZombies.Add(zombie, coroutine);
        }
    }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     Zombie zombie = other.GetComponent<Zombie>();
    //     if (zombie != null && affectedZombies.ContainsKey(zombie))
    //     {
    //         StopCoroutine(affectedZombies[zombie]);
    //         zombie.ResetSpeed();
    //         zombie.ResetAttackSpeed();
    //         affectedZombies.Remove(zombie);
    //     }
    // }

    private IEnumerator ApplySpeedBoost(Zombie zombie)
    {
        // Increase zombie's speed
        zombie.IncreaseSpeed(speedIncreaseAmount);
        zombie.IncreaseAttackSpeed(attackSpeedIncreaseAmount);

        // Wait for the speed increase duration
        yield return new WaitForSeconds(speedIncreaseDuration);

        // Reset zombie's speed if it's still in the area
        zombie.ResetSpeed();

        // Remove zombie from the affected list
        affectedZombies.Remove(zombie);
    }

    private IEnumerator AreaDurationCoroutine()
    {
        // Wait for the area duration
        yield return new WaitForSeconds(areaDuration);

        // Destroy the area effect
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        foreach (var kvp in affectedZombies)
        {
            if (kvp.Value != null)
            {
                StopCoroutine(kvp.Value);
            }
            kvp.Key.ResetSpeed();
            kvp.Key.ResetAttackSpeed();
        }
        affectedZombies.Clear();
    }
}
