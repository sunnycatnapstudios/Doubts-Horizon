using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour {
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
    public bool isReloading = false;
    public float reloadSpeed = .1f;

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxDrawSlingshot;
        public AudioClip sfxBulletShot;
    }

    [SerializeField] private AudioClips audioClips;

    void ResetProjectile() {
        shoot = false;
        sprender.enabled = false;
        circollider2D.enabled = false;
        UpdateUI();
    }

    public void ChangeBulletCount(int newCount) {
        bulletCount = Mathf.Clamp(newCount, 0, bulletRef.Length);
        UpdateUI();
    }

    private IEnumerator ReloadBulletsSequentially() {
        isReloading = true;
        yield return new WaitForSeconds(reloadCooldown); // Initial cooldown

        for (int i = 0; i < bulletRef.Length; i++) {
            ChangeBulletCount(i + 1); // Refill bullets one by one
            yield return new WaitForSeconds(reloadSpeed); // Wait between refills
        }

        isReloading = false;
    }

    void UpdateUI() {
        for (int i = 0; i < bulletRef.Length; i++) {
            if (i < bulletCount) {
                bulletRef[i].sprite = fullSprite;
                bulletRef[i].enabled = true;
            } else {
                bulletRef[i].sprite = emptySprite;
                bulletRef[i].enabled = true;
            }
        }
    }

    void Start() {
        sprender = GetComponent<SpriteRenderer>();
        circollider2D = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        target = Player.GetComponent<Transform>();
        ResetProjectile();
        UpdateUI();

        transform.position = Vector3.down * .8f + target.position;
        DIR = Vector3.down;
    }

    void Update() {
        UpdateUI();
        if (Input.GetMouseButtonDown(1) && !shoot) {
            AudioManager.Instance.PlaySound(audioClips.sfxDrawSlingshot);
        }

        if (Input.GetMouseButtonUp(1) && !shoot && bulletCount > 0 && !isReloading) {
            ChangeBulletCount(bulletCount - 1);
            shoot = true;
            sprender.enabled = true;
            circollider2D.enabled = true;
            refCheck = transform.position;
            AudioManager.Instance.PlaySound(audioClips.sfxBulletShot);
        }

        if (bulletCount <= 0 && !isReloading) {
            StartCoroutine(ReloadBulletsSequentially());
        }

        if (shoot) {
            transform.position += DIR * (Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, refCheck) >= maxDistance) {
                ResetProjectile();
            }

        } else {
            if (Player.faceRight) {
                transform.position = Vector3.right * .8f + target.position;
                DIR = Vector3.right;
            }

            if (Player.faceLeft) {
                transform.position = Vector3.left * .8f + target.position;
                DIR = Vector3.left;
            }

            if (Player.faceUp) {
                transform.position = Vector3.up * .8f + target.position;
                DIR = Vector3.up;
            }

            if (Player.faceDown) {
                transform.position = Vector3.down * .8f + target.position;
                DIR = Vector3.down;
            }
        }
    }

    // Reset the projectile upon collision
    private void OnTriggerEnter2D(Collider2D other) {
        ResetProjectile();
    }
}
