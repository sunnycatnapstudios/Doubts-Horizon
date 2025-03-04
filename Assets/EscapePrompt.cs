using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EscapePrompt : MonoBehaviour
{
    public _BattleUIHandler _battleUIHandler;
    public Animator escapePrompt;
    public TextMeshProUGUI escapePercentage;
    private CharacterWobble escapePercentageWobble;

    public int escapePercentageVal;

    void Start()
    {
        escapePrompt = GetComponent<Animator>();
        escapePercentageWobble = escapePercentage.GetComponent<CharacterWobble>();
    }
    void Update()
    {
        _battleUIHandler = GameObject.FindGameObjectWithTag("Combat UI").GetComponentInChildren<PartySlotHandler>()._battleUIHandler;
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            OpenPrompt();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            ClosePrompt();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            ShowRoulette();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            CloseRoulette();
        }
    }
    public void OpenPrompt()
    {
        escapePercentageVal = _battleUIHandler.escapeChance;
        // escapePercentageVal = 87; escapePercentage.text = $"%";
        StartCoroutine(AnimatePercentage(escapePercentageVal));

        escapePrompt.Play("Appear");
    }
    private IEnumerator AnimatePercentage(int targetValue)
    {
        int currentValue = 0;
        float duration = .7f;
        float elapsedTime = 0f;
        escapePercentage.text = $"0%";

        yield return new WaitForSecondsRealtime(.3f);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            currentValue = Mathf.RoundToInt(Mathf.Lerp(0, targetValue, elapsedTime / duration));
            escapePercentage.text = $"{currentValue}%";
            yield return null;
        }

        escapePercentage.text = $"{targetValue}%";
    }
    public void ClosePrompt()
    {
        escapePrompt.Play("Close");
        
        // escapePrompt.Play("Hidden");
    }
    public void ShowRoulette()
    {
        escapePrompt.Play("RollChance");
    }
    public void CloseRoulette()
    {
        escapePrompt.Play("CloseRoll");
    }
}
