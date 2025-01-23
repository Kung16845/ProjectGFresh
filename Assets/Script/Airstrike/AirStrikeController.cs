using UnityEngine;
using UnityEngine.UI; // For the Slider
using System.Collections;

public class AirStrikeController : MonoBehaviour
{
    [Header("References")]
    public GameObject targetingMarkerPrefab; // The indicator that follows the mouse
    public GameObject explosionPrefab;       // The explosion prefab (with Explosion script)
    public Slider cooldownSlider;            // UI Slider to show cooldown progress

    [Header("Settings")]
    public float targetingRadius = 1f;          // Radius for the explosion damage area
    public float barrierDamage = 50f;           // Damage to barriers
    public float zombieDamage = 100f;           // Damage to zombies
    public float countdownBeforeStrike = 2f;    // Time after placement before explosion occurs
    public float abilityCooldown = 5f;          // Cooldown time after air strike is used

    private bool isTargeting = false;
    private bool canUseAirStrike = true;
    private GameObject currentMarker;

    private void Update()
    {
        // Start targeting mode if pressed '4' and not on cooldown
        if (Input.GetKeyDown(KeyCode.Alpha4) && canUseAirStrike)
        {
            StartTargeting();
        }

        // If currently targeting, follow the mouse and detect placement click
        if (isTargeting && currentMarker != null)
        {
            // Update marker position to mouse position in world space
            Vector3 mousePosition = GetMouseWorldPosition();
            currentMarker.transform.position = mousePosition;

            // Confirm placement on left click
            if (Input.GetMouseButtonDown(0))
            {
                // Fix the marker position and start the countdown
                isTargeting = false;
                StartCoroutine(ExecuteAirStrike(mousePosition));
            }
        }
    }

    private void StartTargeting()
    {
        isTargeting = true;
        // Instantiate the targeting marker
        if (targetingMarkerPrefab != null)
        {
            currentMarker = Instantiate(targetingMarkerPrefab, GetMouseWorldPosition(), Quaternion.identity);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Ensure the camera is set properly or adjust this value depending on your setup
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private IEnumerator ExecuteAirStrike(Vector3 position)
    {
        // Destroy marker after placement
        if (currentMarker != null)
        {
            Destroy(currentMarker);
        }

        // Wait for the countdown
        yield return new WaitForSeconds(countdownBeforeStrike);

        // Instantiate explosion
        if (explosionPrefab != null)
        {
            GameObject explosionObject = Instantiate(explosionPrefab, position, Quaternion.identity);
            Explosion explosion = explosionObject.GetComponent<Explosion>();
            if (explosion != null)
            {
                explosion.Initialize(barrierDamage, zombieDamage, targetingRadius);
            }
        }

        // Start cooldown
        StartCoroutine(AirStrikeCooldown());
    }

    private IEnumerator AirStrikeCooldown()
    {
        canUseAirStrike = false;
        float cooldownTimeRemaining = abilityCooldown;

        // Enable and initialize slider if available
        if (cooldownSlider != null)
        {
            cooldownSlider.gameObject.SetActive(true);
            cooldownSlider.maxValue = abilityCooldown;
            cooldownSlider.value = abilityCooldown;
        }

        while (cooldownTimeRemaining > 0)
        {
            cooldownTimeRemaining -= Time.deltaTime;
            if (cooldownSlider != null)
            {
                cooldownSlider.value = cooldownTimeRemaining;
            }
            yield return null;
        }

        if (cooldownSlider != null)
        {
            cooldownSlider.gameObject.SetActive(false);
        }

        canUseAirStrike = true;
    }
}
