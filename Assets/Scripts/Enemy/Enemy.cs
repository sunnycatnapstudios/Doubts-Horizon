using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    GameStatsManager gameStatsManager;
    _BattleUIHandler _battleUIHandler;

    public bool attack, stun, hitByBullet, searching, pathReturn, caught, demotestFreeze; // Enemy States
    public float enemySpeed = 3, attackSpeed = 5; // Enemy Speeds
    private float counter_ = 0f, stunTime = 4f, stunTimer, searchTimer, intervalCheck = .4f; // Enemy Timers

    public float
        detectRange,
        caughtRange = 1f,
        baseRange,
        pursueRange,
        wakeRange = 4.5f,
        playerDist,
        refX,
        refY; // Enemy Navigation

    private Vector3 startPos, pathBounds; // Enemy Positions

    public Transform target;
    public LayerMask projectile, player;

    public Vector2 pathDist;

    private Animator enemyAnim;
    private SpriteRenderer spriteState;

    int currentTargetIndex;

    public GameObject overworldUI, combatUI;

    void Start() {
        startPos = transform.position;
        enemyAnim = GetComponent<Animator>();
        spriteState = GetComponent<SpriteRenderer>();
        detectRange = baseRange;
        target = GameObject.FindGameObjectWithTag("Player").transform;

        overworldUI = GameObject.FindWithTag("Overworld UI");

        gameStatsManager = GameStatsManager.Instance;
        _battleUIHandler = GameStatsManager.Instance._battleUIHandler;
    }

    void OnDrawGizmos() {
        // Draws a Debug for NPC interact radius
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }

    void EnterCombat(bool iscaught) {
        if (iscaught && !caught) {
            caught = true;

            // StartCoroutine(CaptureScreen());
            // Time.timeScale = 0;
            _battleUIHandler.EnterCombat();
            stun = true;
            stunTimer = float.NegativeInfinity;
            enemyAnim.Play("Stun Down");
        }
    }

    public IEnumerator CaptureScreen() {
        yield return new WaitForEndOfFrame();
        yield return null;

        Camera.main.Render();

        Texture2D screenTexture = ScreenCapture.CaptureScreenshotAsTexture();

        GameObject screenOverlay = new GameObject("ScreenOverlay");
        combatUI.SetActive(true);
        //         overworldUI.SetActive(false);
        screenOverlay.transform.SetParent(combatUI.transform, false);

        // Leave enemy stunned after battle, because it looks cool
        stun = true;
        stunTimer = float.NegativeInfinity;
        enemyAnim.Play("Stun Down");

        RawImage overlayImage = screenOverlay.AddComponent<RawImage>();
        overlayImage.texture = screenTexture;

        RectTransform overlayRect = screenOverlay.GetComponent<RectTransform>();
        overlayRect.sizeDelta = new Vector2(Screen.width, Screen.height);
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.pivot = new Vector2(0.5f, 0.5f);

        // Start the animation
        StartCoroutine(ZoomInAnimation(screenOverlay, overlayImage));
    }

    IEnumerator ZoomInAnimation(GameObject overlay, RawImage overlayImage) {
        RectTransform rect = overlay.GetComponent<RectTransform>();

        // Start with slightly zoomed out
        float initialScale = .8f;
        rect.localScale = new Vector3(initialScale, initialScale, 1f);

        // Wait for a short delay
        yield return new WaitForSecondsRealtime(0.8f);

        float duration = 1.5f; // Animation duration
        float time = 0f;

        Color startColor = overlayImage.color;
        Color targetColor = new Color(0, 0, 0, 0); // Fully dark and transparent

        while (time < duration) {
            time += Time.unscaledDeltaTime;
            float t = time / duration; // Normalize time (0 to 1)

            // Zoom in effect
            float scale = Mathf.Lerp(initialScale, 2.5f, t);
            rect.localScale = new Vector3(scale, scale, 1f);

            // Rotation effect
            float rotation = Mathf.Lerp(0f, 30f, t);
            rect.rotation = Quaternion.Euler(0, 0, rotation);

            // Darkening & Opacity fade-out
            overlayImage.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        // Destroy object after animation completes
        Destroy(overlay);
        // caught = false;
    }

    void EnemyPatrol() {
        if (pathDist.x * pathDist.y == 0) {
            // Checks if it's a straight line

            pathBounds = startPos + new Vector3(pathDist.x, pathDist.y, 0f);

            if (!pathReturn) {
                counter_ += Time.deltaTime;
                if (counter_ >= 1.5f) {
                    transform.position =
                        Vector3.MoveTowards(transform.position, pathBounds, enemySpeed * Time.deltaTime);
                    if (transform.position == pathBounds) {
                        counter_ = 0f;
                        pathReturn = true;
                    }
                }
            } else {
                counter_ += Time.deltaTime;
                if (counter_ >= 1.5f) {
                    transform.position = Vector3.MoveTowards(transform.position, startPos, enemySpeed * Time.deltaTime);
                    if (transform.position == startPos) {
                        counter_ = 0f;
                        pathReturn = false;
                    }
                }
            }
        } else {
            // Handles Square Paths
            Vector3[] squarePoints = new Vector3[] {
                startPos,
                startPos + new Vector3(pathDist.x, 0, 0),
                startPos + new Vector3(pathDist.x, pathDist.y, 0),
                startPos + new Vector3(0, pathDist.y, 0)
            };
            counter_ += Time.deltaTime;
            if (counter_ >= 1.5) {
                transform.position = Vector3.MoveTowards(transform.position, squarePoints[currentTargetIndex],
                    enemySpeed * Time.deltaTime);

                if (transform.position == squarePoints[currentTargetIndex]) {
                    counter_ = 0f;
                    currentTargetIndex = (currentTargetIndex + 1) % 4;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            // if (playerDist <= detectRange || attack && !stun) {
            //
            // }
        } else if (other.CompareTag("Bullet")) {
            hitByBullet = true;
        }
    }

 void Update() {
    if (caught) return;
    
    playerDist = Vector3.Distance(target.position, transform.position);
    refX = transform.position.x;
    refY = transform.position.y;

    Vector3 direction = (target.position - transform.position).normalized;
    
    RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1.0f, LayerMask.GetMask("Obstacles"));

    if (hit.collider != null) {
        // Obstacle detected, back off slightly
        transform.position = Vector3.MoveTowards(transform.position, transform.position - direction, enemySpeed * Time.deltaTime);
    } else if (hitByBullet || stun) {
        // Stun Logic
        hitByBullet = false;
        searchTimer = 0f;
        if (stunTimer <= stunTime) {
            stunTimer += Time.deltaTime;
            stun = true;
            detectRange = 0f;
            enemyAnim.Play("Stun Down");
        } else {
            detectRange = wakeRange;
            stunTimer = 0f;
            searching = true;
            stun = false;
        }
    } else if (playerDist <= detectRange || attack && !stun) {
        // Move towards player if no obstacle detected
        if (!demotestFreeze) {
            transform.position = Vector3.MoveTowards(transform.position, target.position, attackSpeed * Time.deltaTime);
        }
        attack = true;
        EnterCombat(Physics2D.OverlapCircle(transform.position, caughtRange, player));
        searchTimer = 0f;
        detectRange = baseRange;
        intervalCheck = .4f;
        searching = false;
        if (playerDist >= pursueRange) {
            attack = false;
        }
    } else if (searching) {
        enemyAnim.Play("Enem Left");
        if (searchTimer <= 2.1f) {
            searchTimer += Time.deltaTime;
            if (searchTimer >= intervalCheck) {
                spriteState.flipX = !spriteState.flipX;
                intervalCheck += .4f;
            }
        } else {
            enemyAnim.Play("Enem Down");
            searchTimer = 0f;
            detectRange = baseRange;
            intervalCheck = .4f;
            searching = false;
        }
    } else if (!demotestFreeze) {
        EnemyPatrol();
    }

    // Enemy Movement Animation
    if (Mathf.Abs(transform.position.x - refX) > Mathf.Abs(transform.position.y - refY) && !stun && !searching) {
        spriteState.flipX = transform.position.x - refX > 0;
        enemyAnim.Play("Enem Left");
    } else if (Mathf.Abs(transform.position.x - refX) <= Mathf.Abs(transform.position.y - refY) && !stun && !searching) {
        enemyAnim.Play(transform.position.y - refY > 0 ? "Enem Up" : "Enem Down");
    }
}

        
    
}
