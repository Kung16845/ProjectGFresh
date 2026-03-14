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
        // Start targeting mode if the "AirStrike" button is pressed and not on cooldown.
        // Note: "AirStrike" must be set up in Project Settings > Input Manager.
        if (Input.GetButtonDown("AirStrike") && canUseAirStrike)
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
            // Cancel targeting on right click
            else if (Input.GetMouseButtonDown(1))
            {
                CancelTargeting();
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
        // This implementation is more robust for a 2D game with an orthographic camera.
        Vector3 mousePos = Input.mousePosition;
        // Set Z to the distance from the camera to the game plane. For a 2D game, this is often 0.
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
        worldPoint.z = 0f; 
        return worldPoint;
    }

    private void CancelTargeting()
    {
        isTargeting = false;
        if (currentMarker != null)
        {
            Destroy(currentMarker);
        }
    }

    private IEnumerator ExecuteAirStrike(Vector3 position)
    {
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

        // Clean up the marker if it still exists (it shouldn't if logic is correct, but good practice)
        if (currentMarker != null)
        {
            Destroy(currentMarker);
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
