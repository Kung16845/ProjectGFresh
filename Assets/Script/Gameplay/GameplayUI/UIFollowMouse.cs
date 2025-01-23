using UnityEngine;

public class UIFollowMouse : MonoBehaviour
{
    [Header("UI Element to Follow Mouse")]
    public RectTransform uiElement; // The UI element to move
    public Canvas canvas;           // Reference to the Canvas

    private RectTransform canvasRectTransform;

    private void Start()
    {
        if (uiElement == null)
        {
            Debug.LogError("UI Element is not assigned!");
            return;
        }

        if (canvas == null)
        {
            canvas = uiElement.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Canvas not found in the hierarchy!");
                return;
            }
        }

        canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        FollowMouse();
    }

    private void FollowMouse()
    {
        if (uiElement == null || canvas == null) return;

        // Convert mouse position to UI position
        Vector2 mousePosition = Input.mousePosition;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Directly use the screen position
            uiElement.position = mousePosition;
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            // Convert screen position to world position in Canvas space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform, 
                mousePosition, 
                canvas.worldCamera, 
                out Vector2 localPoint
            );
            uiElement.localPosition = localPoint;
        }
    }
}
