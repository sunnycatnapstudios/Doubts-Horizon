using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DamageIndicator : MonoBehaviour
{
    public float moveSpeed = 1f, fadeDuration = 1f;
    public TextMeshProUGUI textMesh;
    private Color textColor;
    public float t, distanceBeforePop;
    Vector3 initialPosition;

    private float startTime;
    public float lifetime = .7f; // Total animation time
    public float moveDistance = 70f;
    public bool isHealing;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textColor = textMesh.color;
        initialPosition = transform.position;
        startTime = Time.unscaledTime;
    }

    public void SetText(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
        textColor = color;
    }

    // void Update()
    // {
    //     transform.position += Vector3.up * moveSpeed * Time.unscaledDeltaTime;

    //     if (transform.position.y > initialPosition.y+distanceBeforePop)
    //     {
    //         transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.5f, 0, 0), t*.5f);
    //         t+= Time.unscaledDeltaTime;
    //     } if (transform.localScale.y<=0)
    //     {
    //         t=0;
    //         Destroy(gameObject);
    //     }

    //     textMesh.characterSpacing += 100f * Time.unscaledDeltaTime;
    //     textMesh.color = textColor;
    // }
    void Update()
    {
        float progress = (Time.unscaledTime - startTime) / lifetime;

        if (isHealing)
        {
            // Smooth gentle float
            float moveOffset = Mathf.SmoothStep(0, moveDistance * 0.6f, progress);
            transform.position = initialPosition + Vector3.up * moveOffset;

            // Gentle pulse effect
            float scaleFactor = 1 + Mathf.Sin(progress * Mathf.PI) * 0.2f;
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        } else
        {
            // More intense pop-up effect for damage
            float moveOffset = Mathf.SmoothStep(0, moveDistance, progress);
            transform.position = initialPosition + Vector3.up * moveOffset;

            // Stronger pop effect
            float scaleFactor = Mathf.Sin(progress * Mathf.PI) * 1.5f + 1.0f;
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }
        // // Smooth upward motion
        // float moveOffset = Mathf.SmoothStep(0, moveDistance, progress);
        // transform.position = initialPosition + Vector3.up * moveOffset;

        // // "Pop" Effect: Grow then shrink
        // float scaleFactor = Mathf.Sin(progress * Mathf.PI) * 1.5f + 1.0f;
        // transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);

        // // Fade out near the end
        // float fadeFactor = Mathf.Clamp01(1 - (progress * 1.5f)); 
        // textMesh.color = new Color(textColor.r, textColor.g, textColor.b, fadeFactor);

        // // Slight character spacing increase for extra effect
        // textMesh.characterSpacing = Mathf.Lerp(0, 10, progress);

        // Destroy when finished
        if (progress >= 1) {Destroy(gameObject);}
    }
}