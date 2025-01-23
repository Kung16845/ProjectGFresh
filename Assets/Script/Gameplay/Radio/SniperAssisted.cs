using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SniperAssisted : MonoBehaviour
{
    [Header("Sniper Assistant Configuration")]
    public float assistantDuration = 10f; // Duration of assistance in seconds
    public float damage = 50f; // Damage dealt per shot
    public DamageType damageType = DamageType.HighcalliberBullet; // Type of damage
    public float shotCooldown = 1f; // Cooldown between shots in seconds
    public float abilityCooldown = 20f; // Cooldown till ability can be used again

    [Header("UI Elements")]
    public Button sniperButton; // Button to activate sniper
    public Slider cooldownSlider; // Slider to track cooldown

    private bool isAvailable = true;
    private bool isActive = false;
    private bool isUIInteractable =  false; // Flag to check if the UI is interactable

    private void Start()
    {
        if (sniperButton != null)
        {
            sniperButton.onClick.AddListener(() => ActivateSniperAssistance());
        }

        if (cooldownSlider != null)
        {
            cooldownSlider.maxValue = assistantDuration; // Set the max value for the active duration
            cooldownSlider.value = 0;
        }

        // Ensure the UI is always visible and interactable by default
        if (sniperButton != null)
        {
            sniperButton.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        // Check for the "Tab" key to toggle UI interactivity
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleUIInteractability();
        }
        if (Input.GetKeyDown(KeyCode.T) && cooldownSlider.value == 0)
        {
            ActivateSniperAssistance(true); // Force activation
        }
        // If the ability is not available and not active, update the cooldown slider
        if (!isAvailable && !isActive)
        {
            cooldownSlider.maxValue = abilityCooldown; // Set the slider to track cooldown
            cooldownSlider.value -= Time.deltaTime;

            if (cooldownSlider.value <= 0f)
            {
                isAvailable = true;
                if (sniperButton != null)
                {
                    sniperButton.interactable = true;
                }
            }
        }
    }

    private void ToggleUIInteractability()
    {
        // Toggle the interactability of the UI elements
        isUIInteractable = !isUIInteractable;

        // Set the interactability of the button based on the flag
        if (sniperButton != null)
        {
            sniperButton.interactable = isUIInteractable;
        }
    }

    private void ActivateSniperAssistance(bool forceActivate = false)
    {
        // Do not allow activation if ability is already active or in cooldown
        if (isActive || (!isAvailable && !forceActivate))
        {
            Debug.LogWarning("Cannot activate sniper assistance: either already active or in cooldown.");
            return;
        }

        SoundManager.Instance.PlaySound("Radio");
        isAvailable = false;
        isActive = true;

        if (sniperButton != null)
        {
            sniperButton.interactable = false;
        }

        // Reset and start the slider for active duration
        cooldownSlider.maxValue = assistantDuration;
        cooldownSlider.value = 0;

        StartCoroutine(SniperAssistanceRoutine());
    }

    private IEnumerator SniperAssistanceRoutine()
    {
        float elapsed = 0f;

        while (elapsed < assistantDuration)
        {
            if (elapsed % shotCooldown < Time.deltaTime) // Perform damage periodically without skipping
            {
                DealRandomDamage();
            }

            elapsed += Time.deltaTime;

            // Update slider continuously during active phase
            if (cooldownSlider != null)
            {
                cooldownSlider.value = elapsed;
            }

            yield return null;
        }

        isActive = false;

        // Start cooldown phase
        cooldownSlider.maxValue = abilityCooldown;
        cooldownSlider.value = abilityCooldown;
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        while (cooldownSlider.value > 0f)
        {
            cooldownSlider.value -= Time.deltaTime;
            yield return null;
        }
    }

    private void DealRandomDamage()
    {
        // Find all zombies in the scene
        Zombie[] zombies = FindObjectsOfType<Zombie>();
        if (zombies.Length == 0) return;

        // Pick a random zombie
        Zombie target = zombies[Random.Range(0, zombies.Length)];
        if (target != null)
        {
            target.ZombieTakeDamage(damage, damageType);
            SoundManager.Instance.PlaySound("M24");
        }
    }
}
