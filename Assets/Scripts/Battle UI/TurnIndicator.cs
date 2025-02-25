using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnIndicator : MonoBehaviour
{
    public List<Image> turnOrderImages; // List of images that represent each character's turn
    public List<Vector3> targetPositions; // List of target positions for each image
    private float moveSpeed = 5f; // How fast the images should move

    public int currentTurnIndex = 0; // Index of the current turn
    public bool isMoving = false; // To check if the image is still moving

    public BattleUiHandler BattleUiHandler;
    private GameStatsManager gameStatsManager;
    private _PartyManager _partyManager;
    private _BattleUIHandler _battleUIHandler;
    public GameObject turnImagePrefab;
    // Sprite characterSprite;
    
    void Start()
    {
        gameStatsManager = GameStatsManager.Instance;
        _partyManager = GameStatsManager.Instance.GetComponentInChildren<_PartyManager>();
        _battleUIHandler = GameStatsManager.Instance.GetComponentInChildren<_BattleUIHandler>();
    }
    void OnEnable()
    {
        // StartCoroutine(GetPrefab());
    }

    // IEnumerator GetPrefab()
    // {
    //     while (GetComponentInChildren<Image>() == null)
    //     {
    //         yield return null;
    //     }
    //     turnImagePrefab = GameObject.Find("TurnIndicator");
    // }

    public void SetupTurnIndicator(int orderCount)
    {
        targetPositions.Clear();
        turnOrderImages.Clear();

        for (int i = 0; i < orderCount; i++)
        {
            targetPositions.Add(new Vector3(-40+(40*i), 0, 0));

            // Instantiate new turn order image
            GameObject newImageObj = Instantiate(turnImagePrefab, transform);
            newImageObj.name = "Turn Image"+(i+1); newImageObj.SetActive(true);
            Image newImage = newImageObj.transform.Find("Profile").GetComponent<Image>();

            Sprite characterSprite = _partyManager.characterProfiles.Find(sprite => sprite.name == _battleUIHandler.battleOrder[i].Name);

            newImageObj.transform.localPosition = targetPositions[i]; // Set position
            turnOrderImages.Add(newImage.gameObject.transform.parent.GetComponent<Image>());

            if (characterSprite != null) {turnOrderImages[i].transform.Find("Profile").GetComponent<Image>().sprite = characterSprite;}
            if (_battleUIHandler.battleOrder[i].isEnemy) {
                foreach (var profilePic in _partyManager.characterProfiles) {
                    if (profilePic.name == "Enemy") {turnOrderImages[i].transform.Find("Profile").GetComponent<Image>().sprite = profilePic;}
                }
            }
        }
        StartCoroutine(UpdateTurnOrderPosition());
    }

    private IEnumerator UpdateTurnOrderPosition()
    {
        while (true)
        {
            currentTurnIndex = _battleUIHandler.currentTurnIndex;

            for (int i = 0; i < turnOrderImages.Count; i++)
            {
                Vector3 targetPos = targetPositions[(i - currentTurnIndex + 1 + turnOrderImages.Count) % turnOrderImages.Count]; // Circular shift logic
                if (i != (currentTurnIndex) % turnOrderImages.Count) {targetPos.y = 7;}
                
                turnOrderImages[i].transform.localPosition = Vector3.Lerp(
                    turnOrderImages[i].transform.localPosition,
                    targetPos,
                    moveSpeed * Time.unscaledDeltaTime);
                
                if (i == (currentTurnIndex) % turnOrderImages.Count){
                    turnOrderImages[i].rectTransform.sizeDelta = new Vector2(40, 40);
                    turnOrderImages[i].color = Color.white;
                    // turnOrderImages[i].GetComponent<Outline>().effectDistance = new Vector2(5, -5);
                } else {
                    turnOrderImages[i].rectTransform.sizeDelta = new Vector2(30, 30);
                    turnOrderImages[i].color = new Color(1f, 1f, 1f, 0.5f);
                    // turnOrderImages[i].GetComponent<Outline>().effectDistance = new Vector2(1.5f, -1.5f);
                }

            }

            yield return null; // Wait for the next frame
        }
    }
    public void ClearTurnIndicators()
    {
        foreach (Image turnImage in turnOrderImages)
        {
            Destroy(turnImage.gameObject); // Destroy the GameObject of each turn indicator image
        }
        turnOrderImages.Clear(); // Clear the list of turn indicator images
        targetPositions.Clear(); // Clear the list of target positions
    }
}
