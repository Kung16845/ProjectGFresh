using UnityEngine;

public class ObjectHighlight2D : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    private Color originalOutlineColor; // Stores the original outline color for reset
    public Color highlightColor = Color.green; // The color to highlight with (green by default)
    private bool outlineEnabled = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on this object.");
            return;
        }

        // Ensure the object uses a material with a shader that supports outlines
        if (!spriteRenderer.material.HasProperty("_OutlineColor"))
        {
            Debug.LogWarning("Material does not support outline shader. Assign an appropriate material.");
            return;
        }

        // Save the original outline color
        originalOutlineColor = spriteRenderer.material.GetColor("_OutlineColor");
    }

    void OnMouseEnter()
    {
        if (spriteRenderer != null && spriteRenderer.material.HasProperty("_OutlineColor"))
        {
            spriteRenderer.material.SetColor("_OutlineColor", highlightColor);
            spriteRenderer.material.SetFloat("_OutlineEnabled", 1.0f); // Enable outline (if shader supports toggle)
            outlineEnabled = true;
        }
    }

    void OnMouseExit()
    {
        if (spriteRenderer != null && outlineEnabled)
        {
            spriteRenderer.material.SetColor("_OutlineColor", originalOutlineColor);
            spriteRenderer.material.SetFloat("_OutlineEnabled", 0.0f); // Disable outline
            outlineEnabled = false;
        }
    }
}
