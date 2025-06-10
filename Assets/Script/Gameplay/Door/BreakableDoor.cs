using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BreakableDoor : MonoBehaviour
{
    public bool Isusecrownbar;
    public bool IsuseChainsaw;
    public bool IsuseBoltCutter;
    public bool IsuseShovel;
    public bool IsuseNothing; // New condition

    public bool isopen = false;
    public bool inrange = false;

    [Tooltip("Time to open the door in milliseconds. For example, 5000 = 5 seconds.")]
    public float openDuration; // Time to open the door in milliseconds.
    private float openProgress = 0f;
    private ActionController actionController;

    public Slider OpenProgressSlider;
    private SpriteRenderer spriteRenderer;
    private UIcontrollerExpidition uIcontrollerExpidition;

    private UIInventoryEX inventory; // Use the extended class.

    // Map the boolean flags to their respective item IDs
    private Dictionary<int, bool> requiredItems;

    void Awake()
    {
        if (isopen)
        {
            this.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        actionController = FindObjectOfType<ActionController>();
        uIcontrollerExpidition = FindObjectOfType<UIcontrollerExpidition>();
        OpenProgressSlider.gameObject.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();
        inventory = FindObjectOfType<UIInventoryEX>(); // Find the extended inventory class.

        // Initialize the requiredItems dictionary
        requiredItems = new Dictionary<int, bool>
        {
            { 1020502, Isusecrownbar },
            { 1020504, IsuseChainsaw },
            { 1020501, IsuseBoltCutter },
            { 1020503, IsuseShovel }
        };

        OpenProgressSlider.minValue = 0f;
        OpenProgressSlider.maxValue = openDuration;
        OpenProgressSlider.value = 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inrange = true;
            PlayerHasRequiredItem();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inrange = false;
            spriteRenderer.DOColor(Color.white, 0.5f);
            ResetProgress();
        }
    }

    void Update()
    {
        if (inrange && !isopen)
        {
            // Check if the player has required items or is using nothing
            if (PlayerHasRequiredItem())
            {
                if (IsuseNothing && Input.GetKey(KeyCode.F))
                {
                    // Open the door instantly if "use nothing" condition is active
                    OpenDoor();
                }
                else if (Input.GetKey(KeyCode.F))
                {
                    OpenProgressSlider.gameObject.SetActive(true);
                    actionController.canwalk = false;

                    // Increment the progress based on the time elapsed.
                    openProgress += Time.deltaTime * 1000f; // Convert seconds to milliseconds.

                    // Update slider value.
                    OpenProgressSlider.value = openProgress;

                    if (openProgress >= openDuration)
                    {
                        OpenDoor();
                    }
                }
                else
                {
                    ResetProgress();
                }
            }
            else
            {
                ResetProgress();
            }
        }
    }

    private void OpenDoor()
    {
        isopen = true;
        OpenProgressSlider.gameObject.SetActive(false);
        actionController.canwalk = true;
        this.gameObject.SetActive(false);
        // Add further logic here, such as removing the door or allowing player to pass.
    }

    private void ResetProgress()
    {
        openProgress = 0f;
        OpenProgressSlider.value = 0f;
        OpenProgressSlider.gameObject.SetActive(false);
    }

    private bool PlayerHasRequiredItem()
    {
        if (inventory == null || inventory.listItemDataInventoryEquipment == null)
            return false;

        // Filter active item requirements (where the boolean flag is true).
        var activeRequirements = requiredItems.Where(kvp => kvp.Value).Select(kvp => kvp.Key);

        // Check if any of the active required items exist in the player's equipment inventory.
        bool hasRequiredItem = inventory.listItemDataInventoryEquipment
            .Any(item => activeRequirements.Contains(item.idItem));

        // Check the "use nothing" condition
        if (IsuseNothing)
        {
            spriteRenderer.DOColor(Color.green, 0.5f);
            return true; // "Use nothing" always allows access
        }

        // Update color based on item possession
        if (!hasRequiredItem)
        {
            spriteRenderer.DOColor(Color.red, 0.5f);
        }
        else
        {
            spriteRenderer.DOColor(Color.green, 0.5f);
        }

        return hasRequiredItem;
    }
}
