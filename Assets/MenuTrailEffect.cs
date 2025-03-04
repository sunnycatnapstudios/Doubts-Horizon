using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuTrailEffect : MonoBehaviour
{
    public GameObject menuItem;
    public GameObject ghostPrefab;
    public float ghostLifetime = 0.5f;
    public float spawnDelay = 0.001f;

    private bool isAnimating = false;

    void OnEnable()
    {
        menuItem = this.gameObject;
    }
    public void AnimateMenuItem()
    {
        if (!isAnimating) StartCoroutine(SpawnTrailEffect());
    }

    IEnumerator SpawnTrailEffect()
    {
        isAnimating = true;

        for (int i = 0; i < 100; i++)  // Adjust number of trails
        {
            CreateGhost(i);
            yield return new WaitForSecondsRealtime(spawnDelay);
        }

        isAnimating = false;
    }

    void CreateGhost(int interation)
    {
        GameObject ghost = Instantiate(ghostPrefab, menuItem.transform.position, Quaternion.identity, menuItem.transform.parent);
        ghost.SetActive(true);
        ghost.transform.SetAsFirstSibling(); // Ensure the trail appears behind

        // Color color = Color(ghost.GetComponent<Image>().color.r, ghost.GetComponent<Image>().color.g, ghost.GetComponent<Image>().color.b, (100-interation)/100)
        Color color = new Color((100-interation)/100, (100-interation)/100, (100-interation)/100, (100-interation)/100);
        ghost.GetComponent<Image>().color = color;

        CanvasGroup canvasGroup = ghost.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = ghost.AddComponent<CanvasGroup>();

        StartCoroutine(FadeOutAndDestroy(ghost, canvasGroup));
    }

    IEnumerator FadeOutAndDestroy(GameObject ghost, CanvasGroup canvasGroup)
    {
        float elapsed = 0f;

        while (elapsed < ghostLifetime)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / ghostLifetime);
            yield return null;
        }

        Destroy(ghost);
    }
}
