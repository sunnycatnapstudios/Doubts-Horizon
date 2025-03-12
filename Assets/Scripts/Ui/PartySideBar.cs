using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartySideBar : MonoBehaviour {
    public GameObject profilePrefab;
    // public List<Image> sideBarProfilePictures;
    public List<GameObject> sideBarSlots;

    private GameStatsManager gameStatsManager;
    private _PartyManager _partyManager;
    private PartyManager partyManager;

    public void UpdateSideBar() {
        foreach (var slot in sideBarSlots) { Destroy(slot); }
        sideBarSlots.Clear();
        foreach (Survivor member in gameStatsManager.currentSurvivors) {
            if (member.Name == "maincharacter") {continue;}

            GameObject newSideBarProfile = Instantiate(profilePrefab, transform);
            newSideBarProfile.SetActive(true);
            newSideBarProfile.transform.SetSiblingIndex(0);

            Sprite profilePic = member.GetSprite();
            //profilePic = member.GetSprite();
            if (profilePic != null) {
                newSideBarProfile.GetComponent<Image>().sprite = profilePic;
            } else { Debug.Log($"No matching profilePic found for {member.Name}. Check if the Sprite is properly named"); }
            Debug.Log(member.ToString());

            newSideBarProfile.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(35, 35);

            newSideBarProfile.transform.Find("Name").GetComponent<TMP_Text>().text = member.Name;

            // newSideBarProfile.transform.Find("Health").GetComponent<TMP_Text>().text = $"{member.currentHealth}/{member.maxHealth}";
            newSideBarProfile.transform.Find("Health Bar Base").Find("Health").GetComponent<TMP_Text>().text = $"{member.currentHealth}/{member.maxHealth}";
            newSideBarProfile.transform.Find("Health Bar Base").Find("Healthbar").GetComponent<Image>().fillAmount = member.currentHealth / member.maxHealth;

            sideBarSlots.Add(newSideBarProfile);
            newSideBarProfile.name = "Party Slot" + sideBarSlots.Count;

        }
    }

    IEnumerator WaitForPartyManager() {
        while (GameStatsManager.Instance == null) {
            yield return null; // Wait until it's ready
        }

        gameStatsManager = GameStatsManager.Instance;
        //_partyManager = gameStatsManager._partyManager;
    }

    void Awake() {
         // gameStatsManager = GameStatsManager.Instance;
        // _partyManager = GameStatsManager.Instance._partyManager;
        //StartCoroutine(WaitForPartyManager());
    }

    void Start() {
        gameStatsManager = GameStatsManager.Instance;
        UpdateSideBar();
    }

    // Update is called once per frame
    void Update() {
        this.GetComponent<VerticalLayoutGroup>().spacing = Mathf.Lerp(
            this.GetComponent<VerticalLayoutGroup>().spacing,
            15 - ((sideBarSlots.Count - 1) * 2),
            Time.deltaTime * 10f);
    }
}
