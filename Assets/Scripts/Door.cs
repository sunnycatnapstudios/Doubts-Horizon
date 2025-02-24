using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    Player player;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [Serializable]
    private struct AudioClips {
        public AudioClip sfxLocked;
        public AudioClip sfxUnlock;
        public AudioClip sfxOpen;
        public AudioClip sfxClose;
    }

    [SerializeField] private AudioClips audioClips;
    bool interactable = false;

    public Sprite closed, open;
    public bool isOpen, isLocked;

    // Start is called before the first frame update
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (isOpen) {
            Open();
        } else {
            Close();
        }
    }

    void Update() {
        if (interactable && Input.GetKeyDown(KeyCode.E)) {
            Interact();
        }
    }

    void OnTriggerEnter2D(Collider2D collison) {
        if (collison.gameObject.tag == "Player") {
            interactable = true;
        }
    }

    void OnTriggerExit2D(Collider2D collison) {
        if (collison.gameObject.tag == "Player") {
            interactable = false;
        }
    }

    void Interact() {
        if (isLocked) {
            if (player.inventory.hasItemByName("Key")) {
                Unlock();
                player.inventory.removeItemByName("Key");
            } else {
                Debug.Log(GetHashCode().ToString() + ": locked");
                AudioManager.Instance.PlaySound(audioClips.sfxLocked);
            }
        } else {
            if (isOpen) {
                Close();
            } else {
                Open();
            }
        }
    }

    void Close() {
        Debug.Log(GetHashCode().ToString() + ": close");
        spriteRenderer.sprite = closed;
        isOpen = false;
        this.gameObject.layer = LayerMask.NameToLayer("Can't Traverse");
        if (interactable) {
            AudioManager.Instance.PlaySound(audioClips.sfxClose);
        }
    }

    void Open() {
        Debug.Log(GetHashCode().ToString() + ": open");
        spriteRenderer.sprite = open;
        isOpen = true;
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        AudioManager.Instance.PlaySound(audioClips.sfxOpen);
    }

    void Lock() {
        Debug.Log(GetHashCode().ToString() + ": lock");
        isLocked = true;
    }

    void Unlock() {
        Debug.Log(GetHashCode().ToString() + ": unlock");
        isLocked = false;
        AudioManager.Instance.PlaySound(audioClips.sfxUnlock);
    }
}

