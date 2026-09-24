using UnityEngine;

public class ResizeUIInventory : MonoBehaviour
{
    public GameObject RightBookInventoryUI;
    public GameObject CarInventoryUI;
    public RectTransform rectTransformUIInventory;

    private float _lastTargetWidth = -1f;

    private void Start()
    {
        if (rectTransformUIInventory == null)
        {
            rectTransformUIInventory = GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        if (rectTransformUIInventory == null) return;

        bool isExpanded = (RightBookInventoryUI != null && RightBookInventoryUI.activeSelf) || 
                          (CarInventoryUI != null && CarInventoryUI.activeSelf);
        float targetWidth = isExpanded ? 1600f : 800f;

        if (Mathf.Abs(_lastTargetWidth - targetWidth) > 0.01f)
        {
            _lastTargetWidth = targetWidth;
            Vector2 size = rectTransformUIInventory.sizeDelta;
            size.x = targetWidth;
            rectTransformUIInventory.sizeDelta = size;
        }
    }
}