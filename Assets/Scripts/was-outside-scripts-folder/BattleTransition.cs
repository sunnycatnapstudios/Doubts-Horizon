using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleTransition : MonoBehaviour {
    public Image left, right;

    private Transform deathTransform;
    public TextMeshProUGUI text, buttonText, RIPText;
    public Image black, button;
    private Transform teammateDeath;
    public Image teammateDeathImage;
    //public TextMeshProUGUI teammateText;
    private Image teammateDeathBackground;
    private Survivor deadGuy;
    private GameObject deathDialogue;
    private Stack<Survivor> survivorsToKill = new Stack<Survivor>();
    private bool currentlyInTeammateDeath = false;


    public bool _start;
    public float fadeSpeed = 0.5f;

    void Start() {
        left = this.transform.Find("Left").GetComponent<Image>();
        right = this.transform.Find("Right").GetComponent<Image>();
        currentlyInTeammateDeath = false;

        // Behold, the most jank method of fading in a death animation
        deathTransform = this.transform.Find("Death");
        black = deathTransform.Find("Black").GetComponent<Image>();
        text = deathTransform.Find("Died Text").GetComponent<TextMeshProUGUI>();
        buttonText = deathTransform.Find("Button").Find("DeathButtonText").GetComponent<TextMeshProUGUI>();
        button = deathTransform.Find("Button").GetComponent<Image>();

        left.fillAmount = right.fillAmount = 0;

        left.fillOrigin = (int)Image.OriginHorizontal.Left;
        right.fillOrigin = (int)Image.OriginHorizontal.Right;

        teammateDeath = this.transform.Find("TeammateDeath");
        Debug.Log(teammateDeath+"teammatedeath name");
        RIPText = teammateDeath.Find("TeammateName").GetComponent<TextMeshProUGUI>();
        teammateDeathImage = teammateDeath.Find("TeammateImage").GetComponent<Image>();
        teammateDeathBackground = teammateDeath.Find("Background").GetComponent<Image>();
        deathDialogue = teammateDeath.Find("DeathDialogue").gameObject;





    }

    public void LeaveBattle() {
        _start = false;
        StartCoroutine(LeaveBattleAnim());
    }

    public IEnumerator LeaveBattleAnim() {
        left.fillOrigin = (int)Image.OriginHorizontal.Left;
        right.fillOrigin = (int)Image.OriginHorizontal.Right;

        float elapsedTime = 0f;
        float duration = .5f;
        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3);

            left.fillAmount = right.fillAmount = Mathf.Lerp(0f, 1f, easedT);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        left.fillAmount = right.fillAmount = 1f;

        left.fillOrigin = (int)Image.OriginHorizontal.Right;
        right.fillOrigin = (int)Image.OriginHorizontal.Left;

        yield return new WaitForSecondsRealtime(.4f);

        elapsedTime = 0f;
        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3);

            left.fillAmount = right.fillAmount = Mathf.Lerp(1f, 0f, easedT);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        left.fillAmount = right.fillAmount = 0f;

        left.fillOrigin = (int)Image.OriginHorizontal.Left;
        right.fillOrigin = (int)Image.OriginHorizontal.Right;
    }

    public void HadDied() {
        deathTransform.gameObject.SetActive(true);
        _start = false;
        StartCoroutine(HadDiedAnim());
    }

    public void teammMateDeath(List<Survivor> survivors) {
        foreach (Survivor s in survivors) {
            survivorsToKill.Push(s);
        }
        teammMateDeath(survivorsToKill.Pop());  // Start with the first survivor
    }
    public void teammMateDeath(Survivor survivor) {
        //teammateDeath = this.transform.Find("TeammateDeath");
        if(currentlyInTeammateDeath == true) {
            return;
        } else {
            currentlyInTeammateDeath = true;
        }
        deadGuy = survivor;
        Debug.Log(survivor);
        DeathDialogue dialogueholder = deathDialogue.GetComponent<DeathDialogue>();
        dialogueholder.setSurvivor(survivor);
        dialogueholder.setTransition(this);
        dialogueholder.resetBeforeAndAfterDialogue();
        teammateDeath.gameObject.SetActive(true);
        //_start = false;

        StartCoroutine(teammateDeathAnim());
    }

    public IEnumerator teammateDeathAnim() {
        teammateDeathBackground.color = new Color(teammateDeathBackground.color.r, teammateDeathBackground.color.g, teammateDeathBackground.color.b,
                0); ;
        RIPText.color = new Color(RIPText.color.r, RIPText.color.g, RIPText.color.b, 0);
        teammateDeathImage.color = new Color(teammateDeathImage.color.r, teammateDeathImage.color.g, teammateDeathImage.color.b,
                0);

        while (teammateDeathBackground.color.a < 1) {
            float fadeAmount = teammateDeathBackground.color.a + (Time.unscaledDeltaTime * fadeSpeed);
            Color newColor = new Color(teammateDeathBackground.color.r, teammateDeathBackground.color.g, teammateDeathBackground.color.b,
                fadeAmount);
            teammateDeathBackground.color = newColor;

            newColor = new Color(RIPText.color.r, RIPText.color.g, RIPText.color.b, fadeAmount);
            RIPText.color = newColor;
            //buttonText.color = newColor;

            newColor = new Color(teammateDeathImage.color.r, teammateDeathImage.color.g, teammateDeathImage.color.b,
                fadeAmount);
            teammateDeathImage.color = newColor;
            Debug.Log(teammateDeathBackground.color.a);
            yield return null;
        }
        Debug.Log("I GET HERE IN DEATH ANIME");
        //GameObject thing = new GameObject();

        //thing.AddComponent<DialogueBoxHandler>().dialogueContents = new List<string> { "i am here ",deadGuy.name };

        GameStatsManager.Instance._dialogueHandler.OpenDialogueWith(deathDialogue);
        //StartCoroutine( closeTeammateDeathScreen());
        //GameObject.Destroy( thing );

        // Show Dialog after fade out
        //textHandler.StartDialogue();
    }


    public void closeGreyScreen() {

    }
    public IEnumerator closeTeammateDeathScreen() {
        Debug.Log("I GET HERE IN DEATH ANIME ONEEE");
        yield return new WaitForSecondsRealtime(3);
        while (teammateDeathBackground.color.a > 0) {
            float fadeAmount = teammateDeathBackground.color.a - (Time.unscaledDeltaTime * fadeSpeed);
            Color newColor = new Color(teammateDeathBackground.color.r, teammateDeathBackground.color.g, teammateDeathBackground.color.b,
                fadeAmount);
            teammateDeathBackground.color = newColor;

            newColor = new Color(RIPText.color.r, RIPText.color.g, RIPText.color.b, fadeAmount);
            RIPText.color = newColor;
            //buttonText.color = newColor;

            newColor = new Color(teammateDeathImage.color.r, teammateDeathImage.color.g, teammateDeathImage.color.b,
                fadeAmount);
            teammateDeathImage.color = newColor;
            Debug.Log(teammateDeathBackground.color.a);
            yield return null;
        }
        Debug.Log("I GET HERE IN DEATH ANIME TWOO");
        teammateDeath.gameObject.SetActive(false);
        currentlyInTeammateDeath = false;

        // If we're calling with a list of survivors, trigger the next animation in stack
        if (survivorsToKill.Count > 0) {
            teammMateDeath(survivorsToKill.Pop());
        }
    }


    public IEnumerator HadDiedAnim() {
        while (black.color.a < 1) {
            float fadeAmount = black.color.a + (Time.unscaledDeltaTime * fadeSpeed);
            Color newColor = new Color(black.color.r, black.color.g, black.color.b,
                fadeAmount);
            black.color = newColor;

            newColor = new Color(text.color.r, text.color.g, text.color.b, fadeAmount);
            text.color = newColor;
            buttonText.color = newColor;

            newColor = new Color(button.color.r, button.color.g, button.color.b,
                fadeAmount);
            button.color = newColor;
            yield return null;
        }

        // Show Dialog after fade out
        //textHandler.StartDialogue();
    }

    void Update() {
        if (_start) {
            LeaveBattle();
        }
    }
}
