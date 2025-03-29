using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    [HideInInspector] public Animator anim;
    [HideInInspector] public SpriteRenderer spritestate;
    [HideInInspector] public Inventory inventory;

    public float movementInputDelay = 0.05f;
    public float moveSpeed, moveConstant, moveSprint, moveSneak;
    private Vector3 pointRef;
    [HideInInspector] public Transform movePoint;
    public List<Vector3> moveHist = new List<Vector3>();

    public LayerMask noPass, NPC;

    public AudioSource walkAudi;
    public int walkAudiCount;

    public ParticleSystem partiSystem;

    public CinemachineVirtualCamera vcam;
    public PolygonCollider2D cameraCollider;
    public float camMax, camMin, currentCamSize;
    public bool isZooming, canControlCam = true;

    public bool isMoving;
    public bool faceLeft, faceRight, faceUp, faceDown = true;
    int animCount;

    public Vector2 lastInput;
    public Vector2 playerInput;

    private PartyManager partyManager;
    public Bullet bullet;

    public bool isPlayerInControl;
    public bool isSneaking, isSprinting;


    void ViewMap(bool cancontrolcam) {
        isZooming = Input.GetKey(KeyCode.Q);

        if (cancontrolcam) {
            currentCamSize = Mathf.Clamp(currentCamSize, 4f, 6f);
            float targetSize = isZooming ? camMax : (currentCamSize >= 6) ? 6 : (currentCamSize <= 4) ? 4 : currentCamSize;
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, targetSize, targetSize * Time.deltaTime);

            if (!isZooming) currentCamSize -= Input.mouseScrollDelta.y * .4f;
        }
    }

    void UpdateMoveHist() {
        if (moveHist.Count < partyManager.partyCount &&
            movePoint.position == pointRef) {
            Vector3 spawnPos = GetValidSpawnPosition();
            moveHist.Add(spawnPos);
        }

        if (movePoint.position != pointRef) { moveHist.Add(pointRef); }

        if (moveHist.Count > partyManager.partyCount) { moveHist.RemoveAt(0); }
    }
    Vector3 GetValidSpawnPosition() {
        Vector3[] possibleOffsets = new Vector3[]
        {
            new Vector3(-1, 0, 0),
            new Vector3(-1, -1, 0),
            new Vector3(0, -1, 0),
            new Vector3(1, -1, 0),
            new Vector3(1, 0, 0),
            new Vector3(-1, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0)
        };

        foreach (Vector3 offset in possibleOffsets) {
            Vector3 candidate = pointRef + offset;
            if (!moveHist.Contains(candidate) && isTraversable(candidate)) {
                return candidate; // Return the first valid position in order
            }
        }

        return pointRef; // Fallback
    }


    void Awake() {

    }
    void Start() {
        anim = GetComponent<Animator>();
        spritestate = GetComponent<SpriteRenderer>();
        walkAudi = GetComponent<AudioSource>();
        inventory = GetComponent<Inventory>();
        partyManager = GetComponent<PartyManager>();

        movePoint.parent = null;
        moveConstant = moveSpeed;

        currentCamSize = camMin;

        // Default sprint speed if unset in inspector
        if (moveSprint == 0f) {
            moveSprint = 8.5f;
        }
    }

    void Update() {
        Vector3 startRef = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        Vector3 endRef = transform.position;
        isMoving = startRef != endRef;
        bool wasStill = lastInput == Vector2.zero;
        bool movingHorizontal = startRef.x != endRef.x;
        bool movingVertical = startRef.y != endRef.y;

        isSneaking = Input.GetMouseButton(1);
        isSprinting = Input.GetKey(KeyCode.LeftShift) && GameStatsManager.Instance.CanSprint() && !isSneaking;
        GameStatsManager.Instance.isCurrentlySprinting = isSprinting && isMoving;

        GameStatsManager.Instance.Sprint();

        if (!isPlayerInControl) {
            moveSpeed = (isSneaking && !bullet.isReloading) ? moveSneak : ((isSprinting && isMoving) ? moveSprint : moveConstant);
        }
        if (isSprinting && isMoving) { if (animCount <= 0) { partiSystem.Play(); animCount += 1; } } else {
            animCount = 0;
            partiSystem.Stop();
        }

        ViewMap(canControlCam);

        playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Vector3.Distance(transform.position, movePoint.position) <= movementInputDelay && !isZooming && !isPlayerInControl) {

            pointRef = movePoint.position;

            if (playerInput.x != 0 && lastInput.x == 0) {
                lastInput = new Vector2(playerInput.x, 0f);
            } else if (playerInput.y != 0 && lastInput.y == 0) {
                lastInput = new Vector2(0f, playerInput.y);
            }

            if (playerInput.x * lastInput.x == -1f || playerInput.y * lastInput.y == -1f) { lastInput = playerInput; }

            if (playerInput == Vector2.zero) { lastInput = Vector2.zero; }
            Vector3 moveDir = new Vector3(lastInput.x, lastInput.y, 0f);

            if (isTraversable(movePoint.position + moveDir)) {
                movePoint.position += moveDir;
            }

            if (moveDir.x != 0 && (wasStill || movingHorizontal)) {
                spritestate.flipX = moveDir.x < 0;
                partiSystem.transform.eulerAngles = new Vector3(0f, moveDir.x < 0 ? 90f : -90f, 90f);

                faceLeft = moveDir.x < 0; faceRight = moveDir.x > 0;
                faceUp = faceDown = false;

                anim.Play("Walk Left");
            } else if (moveDir.y != 0 && (wasStill || movingVertical)) {
                bool movingUp = moveDir.y > 0;
                anim.Play(movingUp ? "Walk Up" : "Walk Down");
                partiSystem.transform.eulerAngles = new Vector3(movingUp ? 90f : -90f, 0f, 0f);

                faceUp = movingUp; faceDown = !movingUp;
                faceLeft = faceRight = false;
            }
            UpdateMoveHist();
        }
        if (!walkAudi.isPlaying && walkAudiCount <= 0 && isMoving) {
            walkAudi.Play();
            walkAudiCount += 1;
            walkAudi.Play();
        } else if (!isMoving) {
            walkAudi.Stop();
            walkAudiCount = 0;
        }
    }

    bool isTraversable(Vector2 pos) {
        if (Physics2D.OverlapCircleAll(pos, .2f, noPass).Any(c => !c.isTrigger)) {
            return false;
        }
        if (Physics2D.OverlapCircleAll(pos, .2f, NPC).Any(c => !c.isTrigger)) {
            return false;
        }

        // Check if position is inside the camera polygon
        if (cameraCollider) {
            return cameraCollider.OverlapPoint(pos);
        }
        return true;
    }
}
