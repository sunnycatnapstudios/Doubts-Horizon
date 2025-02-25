using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;

public class Player : MonoBehaviour
{

    [HideInInspector] public Animator anim;
    [HideInInspector] public SpriteRenderer spritestate;
    [HideInInspector] public Inventory inventory;

    public float moveSpeed = 5f, movementInputDelay = 0.05f, moveConstant, sprintConstant = 1.7f, sneakConstant = .5f;
    [HideInInspector] public float moveSprint, moveSneak;
    private Vector3 pointRef;
    [HideInInspector] public Transform movePoint;
    public List<Vector3> moveHist = new List<Vector3>();

    public LayerMask noPass, NPC;

    public AudioSource walkAudi;
    public int walkAudiCount;

    public ParticleSystem partiSystem;

    public CinemachineVirtualCamera vcam;
    public float camMax, camMin;
    public bool isZooming, canControlCam = true;

    public bool isMoving;
    public bool faceLeft, faceRight, faceUp, faceDown = true;
    int animCount;

    public Vector2 lastInput;
    public Vector2 playerInput;

    private PartyManager partyManager;
    private _PartyManager _partyManager;

    public bool isPlayerInControl;
    public bool isSneaking, isSprinting;


    void ViewMap(bool cancontrolcam)
    {
        isZooming = Input.GetKey(KeyCode.Q);
        float targetSize = isZooming ? camMax : camMin;

        if (cancontrolcam) {
            vcam.m_Lens.OrthographicSize = Mathf.MoveTowards(vcam.m_Lens.OrthographicSize,targetSize,targetSize * Time.deltaTime * 2);
        }
    }

    void UpdateMoveHist()
    {
        if (moveHist.Count < _partyManager.partyCount&&
            movePoint.position == pointRef)
        {
            // for (int i = 0; i<partyManager.partyCount; i++) 
            {moveHist.Add(movePoint.position);}
        }
        if (movePoint.position != pointRef) {moveHist.Add(pointRef);}
        if (moveHist.Count > _partyManager.partyCount) {moveHist.RemoveAt(0);}
    }


    void Awake()
    {
        
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        spritestate = GetComponent<SpriteRenderer>();
        walkAudi = GetComponent<AudioSource>();
        inventory = GetComponent<Inventory>();
        partyManager = GetComponent<PartyManager>();

        _partyManager = GameStatsManager.Instance._partyManager;

        // // Sets Up Variables to prevent confusion
        // moveSpeed = 5f; sprintConstant = 1.7f; sneakConstant = .5f; movementInputDelay = .1f;
        // partyCount = 4;
        // camMax = 10; camMin = 6;
        // maxStamina = 100f; sprintCost = 35f;


        movePoint.parent = null;
        moveConstant = moveSpeed; moveSprint = moveSpeed*sprintConstant; moveSneak = moveSpeed*sneakConstant;


        // moveHist = new List<Vector3>();
        // for (int i = 0; i<partyManager.partyCount; i++) {moveHist.Add(movePoint.position); Debug.Log("AAAAAAAAA");}
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 startRef = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed*Time.deltaTime);
        Vector3 endRef = transform.position;
        isMoving = startRef!=endRef;
        
        isSneaking = Input.GetMouseButton(1);
        isSprinting = Input.GetKey(KeyCode.LeftShift) && GameStatsManager.Instance.CanSprint() && !isSneaking;
        GameStatsManager.Instance.isCurrentlySprinting = isSprinting && isMoving;

        GameStatsManager.Instance.Sprint();

        if (!isPlayerInControl) {
            moveSpeed = isSneaking ? moveSneak : ((isSprinting&&isMoving) ? moveSprint : moveConstant);
        }
        if (isSprinting && isMoving) {if (animCount<=0){partiSystem.Play(); animCount+=1;}}
        else 
        {
            animCount=0;
            partiSystem.Stop();
        }

    
        ViewMap(canControlCam);

        if (Vector3.Distance(transform.position, movePoint.position) <= movementInputDelay && !isZooming && !isPlayerInControl){

            pointRef = movePoint.position;
            
            playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector3 moveDir = Vector3.zero;
            
            if (playerInput.x != 0 && lastInput.x == 0) {
                lastInput = new Vector2(playerInput.x, 0f);
            }
            else if (playerInput.y != 0 && lastInput.y == 0) {
                lastInput = new Vector2(0f, playerInput.y);
            }

            if (playerInput.x * lastInput.x == 0-1f || playerInput.y * lastInput.y == 0-1f) {lastInput = playerInput;}

            if (playerInput == Vector2.zero) {lastInput = Vector2.zero;}
            moveDir = new Vector3(lastInput.x, lastInput.y, 0f);

            // if (playerInput!=Vector2.zero)
            if (moveDir != Vector3.zero)
            {
                // Vector3 moveDir = new Vector3(playerInput.x, playerInput.y, 0f).normalized;

                if (isTraversable(movePoint.position+moveDir))
                {
                    movePoint.position+=moveDir;

                    // if (playerInput.x!=0)
                    if (moveDir.x!=0)
                    {
                        spritestate.flipX = moveDir.x<0;
                        partiSystem.transform.eulerAngles = new Vector3(0f, moveDir.x < 0 ? 90f : -90f, 90f);

                        faceLeft = moveDir.x < 0; faceRight = moveDir.x > 0;
                        faceUp = faceDown = false;

                        anim.Play("Walk Left");
                    }
                    else if (moveDir.y!=0)
                    {
                        bool movingUp = moveDir.y > 0;
                        anim.Play(movingUp ? "Walk Up" : "Walk Down");
                        partiSystem.transform.eulerAngles = new Vector3(movingUp ? 90f : -90f, 0f, 0f);

                        faceUp = movingUp; faceDown = !movingUp;
                        faceLeft = faceRight = false;
                    }
                }
            }
            UpdateMoveHist();
        }
        if (!walkAudi.isPlaying && walkAudiCount <=0 && isMoving) {
            walkAudi.Play(); walkAudiCount+=1; walkAudi.Play();
        }
        else if (!isMoving) {walkAudi.Stop(); walkAudiCount = 0;}
    }

    bool isTraversable(Vector2 pos) {
        if (Physics2D.OverlapCircleAll(pos, .2f, noPass).Any(c => !c.isTrigger)) {
            return false;
        }
        if (Physics2D.OverlapCircleAll(pos, .2f, NPC).Any(c => !c.isTrigger)) {
            return false;
        }
        return true;
    }
}
