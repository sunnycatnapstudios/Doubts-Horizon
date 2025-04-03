using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

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

    public EnemyObjectManager enemyObjectManager;

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxOnHitByBullet;
        public List<AudioClip> sfxMonsterSounds; 
    }

    [SerializeField] private AudioClips audioClips;
    public float monsterSoundInterval = 8f;
    private float monsterSoundIntervalCounter = 0f;

    // Stuck detection variables
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float stuckThreshold = 3f;
    private bool isMovingBack = false;
    private Vector3 lastStuckPosition;
    private float stuckInRadiusTimer = 0f;
    private float stuckInRadiusThreshold = 1f; 
    private Vector3 lastChasePosition;
    private float minMovementThreshold = 2f; 
    private float chaseTimer = 0f;
    private float maxChaseTime = 5f;
    private Vector3 startPosition;
    private float patrolCheckTimer = 0f;
    private float detectionCooldownTimer = 0f;
    private bool detectionDisabled = false; 
    private float chaseCheckTimer = 0f; 
    private float chaseCheckInterval = 2f; 
    
    void Start() {
        startPosition = transform.position; 
        lastStuckPosition = transform.position;
        startPos = transform.position;
        enemyAnim = GetComponent<Animator>();
        spriteState = GetComponent<SpriteRenderer>();
        detectRange = baseRange;
        target = GameObject.FindGameObjectWithTag("Player").transform;

        overworldUI = GameObject.FindWithTag("Overworld UI");

        gameStatsManager = GameStatsManager.Instance;
        _battleUIHandler = GameStatsManager.Instance._battleUIHandler;
        
        lastPosition = transform.position; 
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }

    void EnterCombat(bool iscaught) {
        if (iscaught && !caught) {
            caught = true;
            _battleUIHandler.curEnemy = gameObject;
            _battleUIHandler.enemyObjectManager = enemyObjectManager;
            _battleUIHandler.EnterCombat();

            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();

            StartCoroutine(KillEnemyAfterDelay(0.4f)); 
        }
    }

    IEnumerator KillEnemyAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);

        if (caught) { 
            stun = true;
            stunTimer = float.NegativeInfinity;
            enemyAnim.Play("Stun Down");
        }
    }

   IEnumerator MoveBackToStart() {
        bool wasPathReturn = pathReturn;
        Vector3 wasPathBounds = pathBounds;
        float wasCounter = counter_;
        int wasTargetIndex = currentTargetIndex;

        while (Vector3.Distance(transform.position, startPos) > 0.1f) {
            transform.position = Vector3.MoveTowards(transform.position, startPos, enemySpeed * Time.deltaTime);
            yield return null;
        }
        pathReturn = wasPathReturn;
        pathBounds = wasPathBounds;
        counter_ = wasCounter;
        currentTargetIndex = wasTargetIndex;
        
        isMovingBack = false;
        stuckTimer = 0f;
        lastStuckPosition = transform.position;
    }

    void EnemyPatrol() {
        if (pathDist.x * pathDist.y == 0) {
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
            Vector3[] squarePoints = new Vector3[] {
                startPos,
                startPos + new Vector3(pathDist.x, 0, 0),
                startPos + new Vector3(pathDist.x, pathDist.y, 0),
                startPos + new Vector3(0, pathDist.y, 0)
            };
            counter_ += Time.deltaTime;
            if (counter_ >= 1.5f) {
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
        } else if (other.CompareTag("Bullet")) {
            hitByBullet = true;
            AudioManager.Instance.PlaySound(audioClips.sfxOnHitByBullet);
        }
    }

    private void PlayMonsterSound() {
        if (audioClips.sfxMonsterSounds.Count > 0) {
            int randomClipIndex = UnityEngine.Random.Range(0, audioClips.sfxMonsterSounds.Count);
            AudioClip randomSound = audioClips.sfxMonsterSounds[randomClipIndex];
            AudioManager.Instance.PlaySound(randomSound);
        }
    }


void Update() {
    if (caught) return;

    playerDist = Vector3.Distance(target.position, transform.position);
    refX = transform.position.x;
    refY = transform.position.y;

    if (detectionDisabled) {
        detectionCooldownTimer += Time.deltaTime;
        if (detectionCooldownTimer >= 2f) { // after 2 secs, re-enable detection
            detectionDisabled = false;
            detectionCooldownTimer = 0f;
            Debug.Log("Detection re-enabled.");
        }
        return; 
    }

    // Stuck detection for general movement
    if (Vector3.Distance(transform.position, lastStuckPosition) < minMovementThreshold) {
        stuckTimer += Time.deltaTime;

        if (stuckTimer >= stuckThreshold && !isMovingBack) {
            Debug.Log("Enemy stuck at position! Resetting to start...");
            isMovingBack = true;
            StartCoroutine(MoveBackToStart());
        }
    } else {
        if (Vector3.Distance(transform.position, lastStuckPosition) > minMovementThreshold * 2) {
            stuckTimer = 0f;
            lastStuckPosition = transform.position;
        }
    }

    if (attack && !isMovingBack) {
        if (Vector3.Distance(transform.position, lastChasePosition) < minMovementThreshold) {
            stuckInRadiusTimer += Time.deltaTime;

            if (stuckInRadiusTimer >= stuckInRadiusThreshold) {
                Debug.Log("Enemy stuck chasing! Returning to patrol...");
                attack = false;
                detectRange = baseRange;
                stuckInRadiusTimer = 0f;
                searching = true;
            }
        } else {
            stuckInRadiusTimer = 0f;
            lastChasePosition = transform.position;
        }
    } else {
        stuckInRadiusTimer = 0f;
    }

    monsterSoundIntervalCounter += Time.deltaTime;
    if (monsterSoundIntervalCounter > monsterSoundInterval) {
        PlayMonsterSound();
        monsterSoundIntervalCounter = 0f;
    }

    if (hitByBullet || stun) {
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
        return; // Skip further movement if stunned
    }

    // If enemy detects player, chase and attack
    if ((playerDist <= detectRange || attack) && !demotestFreeze) {
        if (isMovingBack) {
            Debug.Log("Player detected while returning! Resuming chase.");
            isMovingBack = false; 
            StopAllCoroutines(); 
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, attackSpeed * Time.deltaTime);
        attack = true;
        chaseTimer += Time.deltaTime; // increase chase timer

        EnterCombat(Physics2D.OverlapCircle(transform.position, caughtRange, player));
        searchTimer = 0f;
        detectRange = baseRange;
        intervalCheck = 0.4f;
        searching = false;

        if (chaseTimer >= 5f) {  
            Debug.Log("Chase lasted 5 seconds. Entering detection cooldown.");
            attack = false;
            isMovingBack = true;
            chaseTimer = 0f; // Reset chase timer
            detectionDisabled = true; 
            StartCoroutine(MoveBackToStart());
        }
    }

    // If enemy is searching for player after losing sight
    else if (searching) {
        enemyAnim.Play("Enem Left");
        if (searchTimer <= 2.1f) {
            searchTimer += Time.deltaTime;
            if (searchTimer >= intervalCheck) {
                spriteState.flipX = !spriteState.flipX;
                intervalCheck += 0.4f;
            }
        } else {
            enemyAnim.Play("Enem Down");
            searchTimer = 0f;
            detectRange = baseRange;
            intervalCheck = 0.4f;
            searching = false;
        }
    } 
    // Default enemy patrol when not engaging with player
    else if (!demotestFreeze && !isMovingBack) {
        EnemyPatrol();

        // Check for the player every 2 seconds while patrolling
        patrolCheckTimer += Time.deltaTime;
        if (patrolCheckTimer >= 2f) {
            patrolCheckTimer = 0f;
            if (playerDist <= detectRange) {
                Debug.Log("Player detected during patrol check! Restarting chase.");
                attack = true;
            }
        }
    }

    if (!stun && !searching && !isMovingBack) {
        if (Mathf.Abs(transform.position.x - refX) > Mathf.Abs(transform.position.y - refY)) {
            spriteState.flipX = transform.position.x - refX > 0;
            enemyAnim.Play("Enem Left");
        } else {
            enemyAnim.Play(transform.position.y - refY > 0 ? "Enem Up" : "Enem Down");
        }
    }
}



}