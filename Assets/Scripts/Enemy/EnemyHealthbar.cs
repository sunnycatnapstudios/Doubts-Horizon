using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    
    public Image healthBarBar, healthBarTail;
    public float maxHealth;
    private GameStatsManager gameStatsManager;
    private _PartyManager _partyManager;
    private _BattleUIHandler _battleUIHandler;

    public void SetHealth(float currentHealth)
    {
        maxHealth = currentHealth;
    }

    public void UpdateHealthBar(float currentHealth)
    {
        healthBarBar.fillAmount = currentHealth / maxHealth;
    }

    void Start()
    {
        gameStatsManager = GameStatsManager.Instance;
        _partyManager = GameStatsManager.Instance.GetComponentInChildren<_PartyManager>();
        _battleUIHandler = GameStatsManager.Instance.GetComponentInChildren<_BattleUIHandler>();
    }
    void Update()
    {
        if (healthBarTail.fillAmount >= healthBarBar.fillAmount){
            healthBarTail.fillAmount = Mathf.Lerp(healthBarTail.fillAmount, healthBarBar.fillAmount, Time.unscaledDeltaTime * 5);
        } else {healthBarTail.fillAmount = healthBarBar.fillAmount;}

        healthBarBar.fillAmount = (float)_battleUIHandler.currentEnemyCurrentHealth/_battleUIHandler.currentEnemyMaxHealth;
    }
}
