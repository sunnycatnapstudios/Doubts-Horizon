using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartySlot : MonoBehaviour {
    public string Name;
    public Image profile;
    public CharacterStats playerStats;
    public GameObject healthbarCasing;
    public Image healthBarBar, healthBarTail;
    public float currentHealth, maxHealth;
    public Vector3 defaultImagePosition, initialBarPosition;
    public bool _isHighlighted;
    private GameStatsManager gameStatsManager;
    public _PartyManager _partyManager;
    public _BattleUIHandler _battleUIHandler;
    public TextMeshProUGUI playerHealthIndicator;
    private float fadeDuration = .5f, delayBeforeFade = 1.2f;

    void OnEnable() {
        _partyManager = GameStatsManager.Instance.GetComponentInChildren<_PartyManager>();
        _battleUIHandler = GameStatsManager.Instance.GetComponentInChildren<_BattleUIHandler>();
        defaultImagePosition = profile.transform.localPosition;
        initialBarPosition = healthbarCasing.transform.localPosition;
        UpdateHealthBar(currentHealth);
        playerHealthIndicator.color = new Color(playerHealthIndicator.color.r, playerHealthIndicator.color.g, playerHealthIndicator.color.b, 0f);
    }

    public void Initialize(CharacterStats member) {
        if (member == null) {
            Debug.LogError("null member");
            return;
        }

        playerStats = member;
        Name = member.Name;
        SetHealth(member.currentHealth, member.maxHealth);
        profile.sprite = _partyManager.characterProfiles.Find(image => image.name == member.Name);
    }

    public void SelectTarget() {
        if (_battleUIHandler != null && Name != "" && _battleUIHandler.canSelect) {
            _battleUIHandler.ReceiveTargetSelection(Name);
        }
    }

    public void isHighlighted() { _isHighlighted = true; }
    public void isNotHighlighted() { _isHighlighted = false; }

    public void HighlightImage() {
        if (profile.transform.localPosition.y < defaultImagePosition.y + 10) {
            profile.transform.localPosition += Vector3.up * 10f * 10f * Time.unscaledDeltaTime;
        } else if (profile.transform.localPosition.y > defaultImagePosition.y + 10) {
            profile.transform.localPosition = defaultImagePosition + Vector3.up * 10f;
        }
    }
    public void UnHighlightImage() {
        if (profile.transform.localPosition.y > defaultImagePosition.y) {
            profile.transform.localPosition -= Vector3.up * 10f * 10f * Time.unscaledDeltaTime;
        } else if (profile.transform.localPosition.y < defaultImagePosition.y) {
            profile.transform.localPosition = defaultImagePosition;
        }
    }

    public void SetHealth(float currenthealth, float maxhealth) {
        currentHealth = currenthealth;
        maxHealth = maxhealth;
        healthBarBar.fillAmount = currentHealth / maxHealth;
        UpdateHealthBar(currentHealth);
    }

    public void UpdateHealthBar(float currenthealth) {
        currentHealth = currenthealth;
    }

    public IEnumerator JutterHealthBar(float duration, float strength) {
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            float jutterAmount = Mathf.Sin(elapsedTime * 30f) * strength;  // Small oscillations
            healthbarCasing.transform.localPosition = initialBarPosition + new Vector3(jutterAmount, 0f, 0f); // Jutter left/right

            elapsedTime += Time.unscaledDeltaTime;
            yield return null; // Wait for next frame
        }

        healthbarCasing.transform.localPosition = initialBarPosition; // Return to original position after jutter
    }

    private IEnumerator FadeOutHealthText() {
        playerHealthIndicator.color = new Color(playerHealthIndicator.color.r, playerHealthIndicator.color.g, playerHealthIndicator.color.b, 1f);
        float timeElapsed = 0f;

        yield return new WaitForSecondsRealtime(delayBeforeFade); // Wait for the specified delay before starting fade out

        Color startColor = playerHealthIndicator.color;

        while (timeElapsed < fadeDuration) {
            float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);
            playerHealthIndicator.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            timeElapsed += Time.unscaledDeltaTime; // Increment time

            yield return null; // Wait for the next frame
        }

        // Ensure the text is fully transparent at the end
        playerHealthIndicator.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
    }
    private IEnumerator LerpHealthBarColor(Color targetColor, float duration) {
        healthBarBar.color = Color.green;
        Color startColor = Color.green;
        float elapsedTime = 0f;

        yield return new WaitForSecondsRealtime(.5f);

        while (elapsedTime < duration) {
            healthBarBar.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        healthBarBar.color = targetColor;
    }

    public void ShowHealthChange() {
        if (!gameObject.activeInHierarchy) return;

        // Show health text and start fading it out
        StopAllCoroutines(); // Stop previous coroutines
        StartCoroutine(FadeOutHealthText());
    }
    public void HealHealthBar() {
        if (!gameObject.activeInHierarchy) return;

        StopAllCoroutines(); // Stop previous coroutines
        StartCoroutine(FadeOutHealthText());
        StartCoroutine(LerpHealthBarColor(Color.red, fadeDuration * 2f));
    }

    void Awake() {
        gameStatsManager = GameStatsManager.Instance;
        // _partyManager = GameStatsManager.Instance.GetComponentInChildren<_PartyManager>();
        // _battleUIHandler = GameStatsManager.Instance.GetComponentInChildren<_BattleUIHandler>();
    }

    void Update() {
        if (healthBarTail.fillAmount > healthBarBar.fillAmount && healthBarBar.fillAmount != 0) {
            healthBarTail.fillAmount = Mathf.Lerp(healthBarTail.fillAmount, healthBarBar.fillAmount, Time.unscaledDeltaTime * 5);
        } else if (healthBarBar.fillAmount == 0) {
            healthBarTail.fillAmount = 0;
        } else { healthBarTail.fillAmount = healthBarBar.fillAmount; }

        if (_isHighlighted && _battleUIHandler.canSelect) {
            HighlightImage();
        } else {
            UnHighlightImage();
        }

        if (healthBarBar.fillAmount == 0) { playerHealthIndicator.text = ""; } else {
            playerHealthIndicator.text = (((int)(healthBarTail.fillAmount * 100f)).ToString() + "%");
        }
        healthBarBar.fillAmount = (float)playerStats.currentHealth / playerStats.maxHealth;
    }
}
