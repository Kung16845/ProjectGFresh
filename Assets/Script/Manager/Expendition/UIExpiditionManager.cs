using UnityEngine;
using DG.Tweening;

public class UIExpiditionManager : MonoBehaviour
{
    public UIInventoryEX uIInventoryEX;
    public GameObject UIcarinvent; 
    public ActionController actionController;
    public SpriteRenderer spriteRenderer;
    public bool isInventorycarActive = false;
    public bool inrange;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (uIInventoryEX != null)
        {
            if (uIInventoryEX.isuseCar)
            {
                isInventorycarActive = true;
                ToggleCarInventory();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            inrange = true;
            HighlightObject(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inrange = false;
            if (isInventorycarActive)
            {
                ToggleCarInventory();
            }
            HighlightObject(false);
        }
    }

    private void Update()
    {
        if (inrange && Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCarInventory();
        }
    }

    private void ToggleCarInventory()
    {
        isInventorycarActive = !isInventorycarActive;
        ToggleUI(isInventorycarActive);
    }

    private void ToggleUI(bool isCarinventActive)
    {
        if (UIcarinvent != null)
        {
            UIcarinvent.SetActive(isCarinventActive);
        }
    }

    private void HighlightObject(bool highlight)
    {
        if (spriteRenderer != null)
        {
            DOTween.Kill(spriteRenderer);
            spriteRenderer.DOColor(highlight ? Color.green : Color.white, 0.5f);
        }
    }

    private void OnDestroy()
    {
        if (spriteRenderer != null)
        {
            DOTween.Kill(spriteRenderer);
        }
    }
}
