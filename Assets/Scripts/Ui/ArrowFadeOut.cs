using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFadeOut : MonoBehaviour {
    public Transform playerTransform;

    public float fadeStartDistance = 5.0f;
    public float fadeEndDistance = 1.0f;
    public float fadeSpeed = 5.0f;

    private SpriteRenderer spriteRenderer;

    // Current opacity target
    private float targetAlpha = 1.0f;
    private float currentAlpha = 1.0f;

    void Start() {
        // Try to get the appropriate renderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerTransform == null) {
            // Attempt to find the player in the scene
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (playerTransform == null) {
                Debug.LogError("ArrowFadeOut: No player transform assigned and no GameObject with 'Player' tag found");
            }
        }
    }

    void Update() {
        if (playerTransform == null)
            return;

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Calculate target alpha based on distance
        if (distanceToPlayer <= fadeEndDistance) {
            targetAlpha = 0f;
        } else if (distanceToPlayer >= fadeStartDistance) {
            targetAlpha = 1f;
        } else {
            // Smoothly interpolate alpha between start and end distances
            float t = (distanceToPlayer - fadeEndDistance) / (fadeStartDistance - fadeEndDistance);
            targetAlpha = Mathf.Clamp01(t);
        }

        // Smoothly transition current alpha to target
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

        // Apply alpha to the appropriate component
        if (spriteRenderer != null) {
            Color color = spriteRenderer.color;
            color.a = currentAlpha;
            spriteRenderer.color = color;
        }
    }

    public void RotateArrow(float rotation) {
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
}
