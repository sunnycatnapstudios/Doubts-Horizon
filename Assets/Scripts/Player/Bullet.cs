﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// public class Bullet : MonoBehaviour
// {
//     public Animator anim;
//     public Player Player;
//     Transform target;
//     public float speed, maxDistance = 50;
//     public bool shoot;
//     public Vector3 DIR, refCheck;
//     public SpriteRenderer sprender;
//     private CircleCollider2D circollider2D;
//     public LayerMask enemy;
//     public Image[] bulletRef;
//     public Sprite fullSprite, holdingSprite, emptySprite;
//     public int bulletCount = 3;

//     void ResetProjectile() {
//         shoot = false;
//         sprender.enabled = false;
//         circollider2D.enabled = false;

//         UpdateUI();
//     }

//     public void ChangeBulletCount(int newCount)
//     {
//         bulletCount = Mathf.Clamp(newCount, 0,bulletRef.Length);
//         UpdateUI();
//     }

//     void UpdateUI()
//     {
//         for (int i = 0; i < bulletRef.Length; i++)
//         {
//             if (Input.GetMouseButton(1) && i+1 == bulletCount) {
//                 bulletRef[i].sprite = holdingSprite;
//             } else {
//                 bulletRef[i].sprite = fullSprite;
//             }

//             if (i < bulletCount)
//             {
//                 // bulletRef[i].sprite = fullSprite;
//                 bulletRef[i].enabled = true;
//             } else 
//             {
//                 // bulletRef[i].sprite = emptySprite;
//                 bulletRef[i].enabled = false;
//             }
//         }
//     }

//     // Start is called before the first frame update
//     void Start()
//     {
//         sprender = GetComponent<SpriteRenderer>();
//         circollider2D = GetComponent<CircleCollider2D>();
//         anim =  GetComponent<Animator>();
//         target = Player.GetComponent<Transform>();

//         ResetProjectile(); transform.position = Vector3.down*.8f + target.position; DIR = Vector3.down;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         UpdateUI();

//         // Controls Bullet Position and Firing
//         if (Input.GetMouseButtonUp(1) && !shoot) {
//             ChangeBulletCount(bulletCount-1);
//             shoot = true; sprender.enabled = true; circollider2D.enabled = true;
//             refCheck = transform.position;
//         }
//         if (shoot) {
//             if (Physics2D.OverlapCircle((transform.position), .2f, enemy)) {
//                 ResetProjectile();
//             } else {
//                 transform.position += DIR*Time.deltaTime*speed;

//                 if (Vector3.Distance(transform.position, refCheck)>=maxDistance) {
//                     ResetProjectile();
//                 }
//             }
//         } else{

//             if (Player.faceRight) {
//                 transform.position = Vector3.right*.8f + target.position; DIR = Vector3.right;
//             }
//             if (Player.faceLeft){
//                 transform.position = Vector3.left*.8f + target.position; DIR = Vector3.left;
//             }
//             if (Player.faceUp){
//                 transform.position = Vector3.up*.8f + target.position; DIR = Vector3.up;
//             }
//             if (Player.faceDown){
//                 transform.position = Vector3.down*.8f + target.position; DIR = Vector3.down;
//             }
//         }
//     }
// }
public class Bullet : MonoBehaviour
{
    public Animator anim;
    public Player Player;
    Transform target;
    public float speed, maxDistance = 50;
    public bool shoot;
    public Vector3 DIR, refCheck;
    public SpriteRenderer sprender;
    private CircleCollider2D circollider2D;
    public LayerMask enemy;
    public Image[] bulletRef;
    public Sprite fullSprite, holdingSprite, emptySprite;
    public int bulletCount = 3;
    public float reloadCooldown = 2.0f; // Cooldown time before refilling bullets
    private bool isReloading = false;
    public float reloadSpeed = .1f;

    void ResetProjectile() {
        shoot = false;
        sprender.enabled = false;
        circollider2D.enabled = false;
        UpdateUI();
    }

    public void ChangeBulletCount(int newCount)
    {
        bulletCount = Mathf.Clamp(newCount, 0, bulletRef.Length);
        UpdateUI();
    }

    private IEnumerator ReloadBulletsSequentially()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadCooldown); // Initial cooldown

        for (int i = 0; i < bulletRef.Length; i++)
        {
            ChangeBulletCount(i + 1); // Refill bullets one by one
            yield return new WaitForSeconds(reloadSpeed); // Wait between refills
        }

        isReloading = false;
    }

    void UpdateUI()
    {
        for (int i = 0; i < bulletRef.Length; i++)
        {
            if (i < bulletCount)
            {
                bulletRef[i].sprite = fullSprite;
                bulletRef[i].enabled = true;
            }
            else
            {
                bulletRef[i].sprite = emptySprite;
                bulletRef[i].enabled = true;
            }
        }
    }

    void Start()
    {
        sprender = GetComponent<SpriteRenderer>();
        circollider2D = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        target = Player.GetComponent<Transform>();
        ResetProjectile();
        UpdateUI();

        transform.position = Vector3.down * .8f + target.position; 
        DIR = Vector3.down;
    }

    void Update()
    {
        UpdateUI();

        if (Input.GetMouseButtonUp(1) && !shoot && bulletCount > 0 && !isReloading)
        {
            ChangeBulletCount(bulletCount - 1);
            shoot = true; 
            sprender.enabled = true; 
            circollider2D.enabled = true;
            refCheck = transform.position;
            // TODO replace with better method later
            AudioManager.Instance.PlaySound("Sfx_EnterLevel");
        }
        if (bulletCount <= 0 && !isReloading)
        {
            StartCoroutine(ReloadBulletsSequentially());
        }

        if (shoot)
        {
            if (Physics2D.OverlapCircle(transform.position, .2f, enemy))
            {
                ResetProjectile();
            }
            else
            {
                transform.position += DIR * Time.deltaTime * speed;

                if (Vector3.Distance(transform.position, refCheck) >= maxDistance)
                {
                    ResetProjectile();
                }
            }
        }
        else
        {
            if (Player.faceRight)
            {
                transform.position = Vector3.right * .8f + target.position; DIR = Vector3.right;
            }
            if (Player.faceLeft)
            {
                transform.position = Vector3.left * .8f + target.position; DIR = Vector3.left;
            }
            if (Player.faceUp)
            {
                transform.position = Vector3.up * .8f + target.position; DIR = Vector3.up;
            }
            if (Player.faceDown)
            {
                transform.position = Vector3.down * .8f + target.position; DIR = Vector3.down;
            }
        }
    }
}
