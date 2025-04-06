using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class ChickenBob : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public float speed = 1f;

    public List<Sprite> leftWalkSprites;
    public List<Sprite> rightWalkSprites;
    public float animationFrameRate = 0.1f; 

    private Vector3 startPos;
    private SpriteRenderer spriteRenderer;
    private float animationTimer;
    private int currentFrame;
    private bool facingRight = true;

    void Start()
    {
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

        float newX = startPos.x + Mathf.PingPong(Time.time * speed, 2f) - 1f;

        float deltaX = newX - transform.position.x;
        if (deltaX > 0.001f)
        {
            facingRight = true;
        }
        else if (deltaX < -0.001f)
        {
            facingRight = false;
        }

        AnimateChicken();

        // Apply position
        transform.position = new Vector3(newX, newY, startPos.z);
    }

    void AnimateChicken()
    {
        animationTimer += Time.deltaTime;
        if (animationTimer >= animationFrameRate)
        {
            animationTimer = 0f;
            currentFrame = (currentFrame + 1) % 3; 

            if (facingRight)
            {
                if (rightWalkSprites.Count >= 3)
                    spriteRenderer.sprite = rightWalkSprites[currentFrame];
            }
            else
            {
                if (leftWalkSprites.Count >= 3)
                    spriteRenderer.sprite = leftWalkSprites[currentFrame];
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Chicken collided with: " + collision.gameObject.name);
    }
}
