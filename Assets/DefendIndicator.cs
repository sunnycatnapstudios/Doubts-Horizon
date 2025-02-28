using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefendIndicator : MonoBehaviour
{
    public bool inAnimation = false, isAssigned = false, overEdge;
    private RectTransform rectTransform;
    public RectTransform parentRectTransform;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    public Vector3 rectScreenPos, parentScreenPos;
    public float parentWidth;
    void CheckIfOverEdge()
    {
        rectScreenPos = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
        parentScreenPos = RectTransformUtility.WorldToScreenPoint(null, parentRectTransform.position);

        parentWidth = parentRectTransform.rect.width / 3;

        overEdge = (rectScreenPos.x > parentScreenPos.x+parentWidth||rectScreenPos.x < parentScreenPos.x-parentWidth);
    }

    void Update()
    {
        if (!inAnimation && isAssigned && parentRectTransform != null)
        {
            CheckIfOverEdge();
        }
    }
}
