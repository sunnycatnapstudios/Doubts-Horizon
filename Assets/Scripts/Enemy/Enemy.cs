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
        public List<AudioClip> sfxMonsterSounds; // List of monster sounds to cycle through
    }

    [SerializeField] private AudioClips audioClips;
    public float monsterSoundInterval = 8f;
    private float monsterSoundIntervalCounter = 0f;

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
            _battleUIHandler.curEnemy = gameObject;
            _battleUIHandler.enemyObjectManager = enemyObjectManager;
            _battleUIHandler.EnterCombat();

            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();

            // Enemy "killed"
            stun = true;
            stunTimer = float.NegativeInfinity;
            enemyAnim.Play("Stun Down");
        }
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
            AudioManager.Instance.PlaySound(audioClips.sfxOnHitByBullet);
        }
    }

    //play a random monster sound
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

        // Very simple logic to occasionally play monster sound
        monsterSoundIntervalCounter += Time.deltaTime;
        if (monsterSoundIntervalCounter > monsterSoundInterval) {
            PlayMonsterSound();
            monsterSoundIntervalCounter -= monsterSoundInterval;
        }

        Vector3 direction = target.position - transform.position;
        direction.Normalize();

        if (hitByBullet || stun) // Stun Enemy
        {
            hitByBullet = false;
            searchTimer = 0f;
            if (stunTimer <= stunTime) {
                stunTimer += Time.deltaTime;
                stun = true;
            } else {
                stun = false;
            }

            if (stunTimer <= stunTime) {
                stun = true;
                stunTimer += Time.deltaTime;
                detectRange = 0f;
                enemyAnim.Play("Stun Down");
            } else {
                detectRange = wakeRange;
                stunTimer = 0f;
                searching = true;
                stun = false;
            }
        } else if (playerDist <= detectRange || attack && !stun) {
            // Attack Player // Will be changed later to account for pathfinding
            if (!demotestFreeze) {
                transform.position =
                    Vector3.MoveTowards(transform.position, target.position, attackSpeed * Time.deltaTime);
            }

            attack = true;
            EnterCombat(Physics2D.OverlapCircle((transform.position), caughtRange, player));
            searchTimer = 0f;
            detectRange = baseRange;
            intervalCheck = .4f;
            searching = false;
            if (playerDist >= pursueRange) {
                attack = false;
            }
        } else if (searching) // Search for Player w Temp Increased Radius
        {
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
        } // Enemy Idle Movement Path

        if (Mathf.Abs(transform.position.x - refX) > Mathf.Abs(transform.position.y - refY) && !stun && !searching) {
            if (transform.position.x - refX > 0) {
                spriteState.flipX = true;
            } else {
                spriteState.flipX = false;
            }

            enemyAnim.Play("Enem Left");
        } else if (Mathf.Abs(transform.position.x - refX) <= Mathf.Abs(transform.position.y - refY) && !stun &&
                   !searching) {
            if (transform.position.y - refY > 0) {
                enemyAnim.Play("Enem Up");
            } else {
                enemyAnim.Play("Enem Down");
            }
        }
    }
}
