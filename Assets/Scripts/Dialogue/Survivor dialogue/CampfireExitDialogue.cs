using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireExitDialogue : MonoBehaviour {
    private DialogueInputHandler dialogueInputHandler;
    private DialogueBoxHandler npcDialogueHandler;
    private Player player;
    public Vector3 location;

    private PartyManager manager;
    [SerializeField] private Sprite highlightSprite;
    [SerializeField] private Sprite normalSprite;
    private LevelTransition levelTransition;
    private AudioTransition audioTransition;
    public List<GameObject> transitions;
    //public GameObject FireplaceTransition;
    public List<GameObject> objects;
    private BattleTransition _battleTransition;
    bool hasFinished = false;
    List<Survivor> kicked;

    [Serializable]
    private struct AudioClips {
        public AudioClip sfxTalkingBlip;
    }

    [SerializeField] private AudioClips audioClips;

    void Start() {
        //objects = FireplaceTransition.GetComponent<fireplace>().objects;
        dialogueInputHandler = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<DialogueInputHandler>();
        npcDialogueHandler = GetComponent<DialogueBoxHandler>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        manager = GameObject.FindGameObjectWithTag("Player").GetComponent<PartyManager>();
        _battleTransition = GameObject.FindGameObjectWithTag("Out Transition").GetComponent<BattleTransition>();
        levelTransition = GetComponent<LevelTransition>();
        audioTransition = GetComponent<AudioTransition>();

        npcDialogueHandler.SetSfxTalkingClip(audioClips.sfxTalkingBlip);

        string takeMeTag = "endingDialogue"+gameObject.GetHashCode().ToString();
        Action takeMe = () => {
            Debug.Log("Take me callback.");
            Transform transform = GameObject.FindGameObjectWithTag("Player").transform;
            Inventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

            //if (inventory.hasItemByName("Ration")) {
            //    inventory.removeItemByName("Ration");
            //    npcDialogueHandler.dialogueContents.Add("You sleep full and satiated");
            //    npcDialogueHandler.lastLineDisplayed = false;
            //    npcDialogueHandler.currentLineIndex += 1;

            //} else {
            //    npcDialogueHandler.dialogueContents.Add("you go to sleep on an empty stomach");
            //    npcDialogueHandler.lastLineDisplayed = false;
            //    npcDialogueHandler.currentLineIndex += 1;
            //}
            //player.movePoint.transform.position = location;
            //player.transform.position = location;
            //StartCoroutine(levelTransition.PerformLevelTransition());   // Use our level transition logic instead
            audioTransition.TriggerAudioTransition();
            hasFinished = true;
            //kickUnfed();
            GameStatsManager.Instance._dialogueHandler.CloseDialogueBox();
            npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
            StartCoroutine(endFireplaceScene());
        };
        dialogueInputHandler.AddDialogueChoice(takeMeTag, takeMe);




        npcDialogueHandler.dialogueContents = new List<string> {
            "All people not fed will not survive to the next morning.",
            "If you do not leave a ration for yourself it will negatively affect your health.",
            "Are you sure you're ready to leave?",
            $"<link=\"{takeMeTag}\"><b><#d4af37>Click</color></b></link> if youre ready to go back or press E to Stay"
        };

        //npcDialogueHandler.afterDialogue = new Action(AfterDialogue);
    }

    void Update() {
    }

    void BeforeDialogue() {
        if (hasFinished) {
            npcDialogueHandler.dialogueContents = new List<string> { "Just a comfy bed" };
            npcDialogueHandler.afterDialogue = null;

        }


    }

    void AfterDialogue() {
        Debug.Log("Completed dialogue.");
        if (hasFinished) {
            npcDialogueHandler.dialogueContents = new List<string> { "Just a comfy bed" };
        }
    }
    private IEnumerator endFireplaceScene() {

        StartCoroutine(GameObject.FindGameObjectWithTag("BlackFadeIn").gameObject.GetComponent<FadeToBlack>().fadetoblack());
        yield return new WaitForSecondsRealtime(1f);
        GameStatsManager.Instance.nightFilter.SetActive(false);
        foreach(GameObject teleporter in transitions) {
            teleporter.SetActive(true);
        }
        foreach (GameObject people in objects) {
            people.SetActive(false);

        }
        kickUnfed();
        yield return new WaitForSecondsRealtime(1.5f);

        StartCoroutine(GameObject.FindGameObjectWithTag("BlackFadeIn").gameObject.GetComponent<FadeToBlack>().fadeout());

        //FireplaceTransition.GetComponent<fireplace>().enabled= false;

        gameObject.GetComponent<CampfireExitDialogue>().enabled= false;


    }
    private void kickUnfed() {
        GameObject[] followers = GameObject.FindGameObjectsWithTag("Followers");
        //followers = GameObject.FindGameObjectWithTag("Feed1").GetComponent<fireplace>().getFollowers();

        foreach (GameObject follower in followers) {
            follower.GetComponent<SpriteRenderer>().enabled = true;

        }
        List<Survivor> iterator = new List<Survivor>(manager.currentPartyMembers);
        kicked = new List<Survivor>();
        foreach (Survivor survivor in iterator) {
            if (survivor.UnKickable) {
                continue;
            }
            if (survivor.Fed) {

                survivor.Fed = false;

            } else {
                kicked.Add(survivor);
                manager.RemoveFromParty(survivor);
                Debug.Log($"Kicked {survivor.GetName()} from party");
            }
        }

        if (kicked.Count > 0) {
            _battleTransition.teammMateDeath(kicked);   // death scene with each kicked survivor
        }
    }


    public void highlightBedSprite() {
        GetComponentInParent<SpriteRenderer>().sprite = highlightSprite;


    }


    public void deselectBedSprite() {
        GetComponentInParent<SpriteRenderer>().sprite = normalSprite;
    }
}
