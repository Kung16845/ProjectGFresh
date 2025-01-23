using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Import the DoTween namespace

public class UIExpiditionManager : MonoBehaviour
{
    public UIInventoryEX uIInventoryEX;
    public GameObject UIcarinvent; 
    public ActionController actionController;
    public SpriteRenderer spriteRenderer; // For color transitions on a SpriteRenderer
    public bool isInventorycarActive = false;
    public bool inrange;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Ensure a SpriteRenderer is on the GameObject
        if(uIInventoryEX.isuseCar)
        {
            isInventorycarActive = true;        
            ToggleCarInventory();
        }
        else if(!uIInventoryEX.isuseCar)
        {
            this.gameObject.SetActive(false);
        }

    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            inrange = true;
            HighlightObject(true); // Turn the object green
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            inrange = false;
            if(isInventorycarActive = true)
                ToggleCarInventory();
            HighlightObject(false); // Reset the color
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && inrange)
        {
            ToggleCarInventory();
        }
    }

    void ToggleCarInventory()
    {
        isInventorycarActive = !isInventorycarActive;
        if (isInventorycarActive)
        {
            ToggleUI(true);
        }
        else
        {
            ToggleUI(false);
        }   
    }

    void ToggleUI(bool isCarinventAcrive)
    {
        if (UIcarinvent != null) UIcarinvent.SetActive(isCarinventAcrive);
    }

    void HighlightObject(bool highlight)
    {
        if (spriteRenderer != null)
        {
            if (highlight)
            {
                // Transition the color to green
                spriteRenderer.DOColor(Color.green, 0.5f); // Smooth transition over 0.5 seconds
            }
            else
            {
                // Reset the color to white or original color
                spriteRenderer.DOColor(Color.white, 0.5f);
            }
        }
    }
}
