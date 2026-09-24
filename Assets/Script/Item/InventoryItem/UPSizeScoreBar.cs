using UnityEngine;

public class UPSizeScoreBar : MonoBehaviour
{
    public RectTransform rectTransform;
    private int _lastChildCount = -1;

    private void Start()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        UpdateSize();
    }

    private void Update()
    {
        int currentCount = transform.childCount;
        if (currentCount != _lastChildCount)
        {
            UpdateSize();
        }
    }

    public void UpdateSize()
    {
        if (rectTransform == null) return;

        _lastChildCount = transform.childCount;
        int multiplier = _lastChildCount / 20 + 1;

        Vector2 size = rectTransform.sizeDelta;
        size.y = 600f * multiplier;
        rectTransform.sizeDelta = size;
    }
}
