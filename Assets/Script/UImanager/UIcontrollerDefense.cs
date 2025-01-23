using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIcontrollerDefense : MonoBehaviour
{
    public GameObject InventoryUI;
    public GameObject BoxItemUI;
    public GameObject BarrierHPUI;
    public GameObject PlayerUI;
    public GameObject MainBox;
    public ActionController actionController;

    public float nearR = 1f; // Red component for "near" color
    public float nearG = 0.5f; // Green component for "near" color
    public float nearB = 0.5f; // Blue component for "near" color
    public float defaultR = 1f; // Red component for default color
    public float defaultG = 1f; // Green component for default color
    public float defaultB = 1f; // Blue component for default color

    private SpriteRenderer spriteRenderer; // SpriteRenderer for 2D objects
    private Renderer objectRenderer; // Renderer for 3D objects
    private bool isPlayerNear = false;
    private bool isInventoryActive = false;
    public bool isfrsttime = true;
    private bool isBoxActive = false;

    void Start()
    {
        actionController.canuseweapon = false;

        // Get SpriteRenderer or Renderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectRenderer = GetComponent<Renderer>();

        // Set the initial color
        SetDefaultColor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isfrsttime)
        {
            ToggleInventoryUI();
        }

        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !isfrsttime)
        {
            ToggleBoxUI();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            SetNearColor(); // Change to "near" color
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            SetDefaultColor(); // Revert to default color

            if (isBoxActive)
            {
                DisableBoxUI(); // Automatically disable the box UI when the player leaves the range
            }
        }
    }

    void ToggleUI(bool mainboxActive, bool inventoryActive, bool barrierHPActive, bool playerActive, bool boxItemActive)
    {
        if (MainBox != null) MainBox.SetActive(mainboxActive);
        if (InventoryUI != null) InventoryUI.SetActive(inventoryActive);
        if (BarrierHPUI != null) BarrierHPUI.SetActive(barrierHPActive);
        if (PlayerUI != null) PlayerUI.SetActive(playerActive);
        if (BoxItemUI != null) BoxItemUI.SetActive(boxItemActive);
    }

    void ToggleInventoryUI()
    {
        isInventoryActive = !isInventoryActive;
        if (isInventoryActive)
        {
            actionController.canuseweapon = false;
            ToggleUI(true, true, false, false, false);
        }
        else
        {
            actionController.canuseweapon = true;
            ToggleUI(false, false, true, true, false);
        }
    }

    void ToggleBoxUI()
    {
        isBoxActive = !isBoxActive;

        if (isBoxActive)
        {
            ActiveBoxUI();
        }
        else
        {
            DisableBoxUI();
        }
    }

    void ActiveBoxUI()
    {
        actionController.canuseweapon = false;
        ToggleUI(true, true, false, false, true);
    }

    void DisableBoxUI()
    {
        actionController.canuseweapon = true;
        ToggleUI(false, false, true, true, false);
    }

    public void canopeninvent()
    {
        isfrsttime = false;
    }

    private void SetNearColor()
    {
        Color nearColor = new Color(nearR, nearG, nearB, 1f); // Alpha is set to 1 (fully visible)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = nearColor;
        }
        else if (objectRenderer != null)
        {
            objectRenderer.material.color = nearColor;
        }
    }

    private void SetDefaultColor()
    {
        Color defaultColor = new Color(defaultR, defaultG, defaultB, 1f); // Alpha is set to 1 (fully visible)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = defaultColor;
        }
        else if (objectRenderer != null)
        {
            objectRenderer.material.color = defaultColor;
        }
    }
}
