using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private StatManager statManager;
    public float currentStamina;
    private bool isSprinting = false;
    private bool canSprint = true;
    private ActionController actionController;
    private AnimationController animationController;
    private StaminaUI staminaUI;

    void Start()
    {
        statManager = GetComponent<StatManager>();
        actionController = GetComponent<ActionController>();
        animationController = GetComponent<AnimationController>();
        staminaUI = FindObjectOfType<StaminaUI>();

        currentStamina = statManager.maxStamina;
        if (staminaUI != null)
        {
            staminaUI.InitializeStaminaSlider(statManager.maxStamina);
        }
    }

    void Update()
    {
        if (actionController != null && actionController.canwalk)
        {
            HandleMovement();
            HandleStamina();
        }
    }

    private void HandleMovement()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector2 direction = new Vector2(horizontal, vertical).normalized;

        if (direction.magnitude > 0)
        {
            // Flip character's direction based on horizontal input
            if (horizontal != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = horizontal > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                transform.localScale = scale;
            }

            // Check if the player is sprinting
            if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && canSprint)
            {
                animationController.isrun = true;
                animationController.iswalk = false;
                isSprinting = true;
                transform.Translate(direction * statManager.sprintSpeed * Time.deltaTime);
            }
            else
            {
                animationController.iswalk = true;
                animationController.isrun = false;
                isSprinting = false;
                transform.Translate(direction * statManager.speed * Time.deltaTime);
            }
        }
        else
        {
            animationController.iswalk = false;
            animationController.isrun = false;
        }
    }

    private void HandleStamina()
    {
        if (isSprinting)
        {
            currentStamina -= statManager.staminaConsumeSpeed * Time.deltaTime;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                canSprint = false;
                isSprinting = false;
            }
        }
        else
        {
            currentStamina += statManager.staminaRecoverSpeed * Time.deltaTime;
            if (currentStamina > statManager.maxStamina)
            {
                currentStamina = statManager.maxStamina;
            }

            if (currentStamina >= 30f) // Assuming min stamina to sprint is 30
            {
                canSprint = true;
            }
        }
        if (staminaUI != null)
        {
            staminaUI.UpdateStaminaSlider(currentStamina);
        }
    }

    // Method called when stats change
    public void OnStatsChanged()
    {
        // Adjust current stamina if max stamina has changed
        if (currentStamina > statManager.maxStamina)
        {
            currentStamina = statManager.maxStamina;
        }
          if (staminaUI != null)
        {
            staminaUI.InitializeStaminaSlider(statManager.maxStamina);
        }
    }
}

