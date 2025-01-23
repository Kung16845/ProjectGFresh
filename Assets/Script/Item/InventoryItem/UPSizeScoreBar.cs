using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UPSizeScoreBar : MonoBehaviour
{
    public RectTransform rectTransform;
    // Update is called once per frame
    void Update()
    {
        int mutipal = transform.childCount / 20 +1;

        Vector2 size = rectTransform.sizeDelta;
        size.y = 600 * mutipal;
        rectTransform.sizeDelta = size;

    }
}
