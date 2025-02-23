using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnIndicator : MonoBehaviour
{
    public List<Image> turnOrderImages; // List of images that represent each character's turn
    public List<Vector3> targetPositions; // List of target positions for each image
    public float moveSpeed = 5f; // How fast the images should move

    public int currentTurnIndex = 0; // Index of the current turn
    public bool isMoving = false; // To check if the image is still moving

    public BattleUiHandler BattleUiHandler;
    public GameObject turnImagePrefab;
    // Sprite characterSprite;

    public void SetupTurnIndicator(int orderCount)
    {
        targetPositions.Clear();
        turnOrderImages.Clear();

        for (int i = 0; i < orderCount; i++)
        {
            targetPositions.Add(new Vector3(-40+(40*i), 0, 0));

            // Instantiate new turn order image
            GameObject newImageObj = Instantiate(turnImagePrefab, transform);
            newImageObj.name = "Image"; newImageObj.SetActive(true);
            Image newImage = newImageObj.GetComponent<Image>();

            Sprite characterSprite = BattleUiHandler.profileImages.Find(sprite => sprite.name == BattleUiHandler.battleOrder[i].Name);

            newImageObj.transform.localPosition = targetPositions[i]; // Set position
            turnOrderImages.Add(newImage);

            if (characterSprite != null) {turnOrderImages[i].sprite = characterSprite;}
            if (BattleUiHandler.battleOrder[i].IsEnemy) {
                foreach (var profilePic in BattleUiHandler.profileImages) {
                    if (profilePic.name == "Enemy") {turnOrderImages[i].sprite = profilePic;}
                }
            }
        }
        StartCoroutine(UpdateTurnOrderPosition());
    }

    private IEnumerator UpdateTurnOrderPosition()
    {
        while (true)
        {
            currentTurnIndex = BattleUiHandler.currentTurnIndex;

            for (int i = 0; i < turnOrderImages.Count; i++)
            {
                Vector3 targetPos = targetPositions[(i - currentTurnIndex + 1 + turnOrderImages.Count) % turnOrderImages.Count]; // Circular shift logic
                if (i != (currentTurnIndex + 1) % turnOrderImages.Count) {targetPos.y = -5;}
                
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
