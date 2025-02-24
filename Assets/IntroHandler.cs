using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class IntroHandler : MonoBehaviour
{
    private CanvasGroup introCanvasGroup, overworldHUDCanvasGroup;
    private RectTransform introRectTransform, titleRectTransform;
    private float fadeDuration = 2f, zoomDuration = 3f, startTopOffset = 0f, endTopOffset = 120f;
    public float cinematicCamMax = 10f, cinematicCamMin = 8f;
    private CinemachineVirtualCamera cinemachineCamera;
    private Player player;

    private bool isAnimating = true;

    void Start()
    {
        introCanvasGroup = GameObject.FindGameObjectWithTag("Intro Screen").GetComponent<CanvasGroup>();
        overworldHUDCanvasGroup = GameObject.FindGameObjectWithTag("Overworld UI").GetComponent<CanvasGroup>();
        cinemachineCamera = GameObject.FindGameObjectWithTag("CM Cam").GetComponent<CinemachineVirtualCamera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        introRectTransform = introCanvasGroup.GetComponent<RectTransform>();
        titleRectTransform = GameObject.FindGameObjectWithTag("Title Card").GetComponent<RectTransform>();

        player.canControlCam = false;
        player.isPlayerInControl = true;
        cinemachineCamera.m_Lens.OrthographicSize = cinematicCamMax;

        overworldHUDCanvasGroup.alpha = 0f;
        
        TitleScreen();
    }

    public void TitleScreen()
    {
        // player.movePoint.transform.position = new Vector3(75f,-21f, 0);
        player.transform.position = new Vector3(62.5f,-26f, 0);
        StartCoroutine(IdleTitleAnimation());
    }

    public void StartGame()
    {
        isAnimating = false; // Stop idle animations
        StopAllCoroutines(); // Stops all animations running
        StartCoroutine(FadeCanvas(1, 0));
        StartCoroutine(ZoomCamera(cinematicCamMax, cinematicCamMin));
    }
    private IEnumerator FadeCanvas(float startAlpha, float endAlpha)
    {
        introCanvasGroup.interactable = introCanvasGroup.blocksRaycasts = false;
        
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            introCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            introRectTransform.offsetMax = new Vector2(introRectTransform.offsetMax.x, Mathf.Lerp(startTopOffset, endTopOffset, t));
            yield return null;
        }
        introCanvasGroup.alpha = endAlpha;
        introRectTransform.offsetMax = new Vector2(introRectTransform.offsetMax.x, endTopOffset);
    }
    private IEnumerator ZoomCamera(float start, float end)
    {
        yield return new WaitForSecondsRealtime(0.2f);
        player.moveSpeed = 3f;
        player.movePoint.transform.position = new Vector3(57.5f, -26f, 0);
        float elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            cinemachineCamera.m_Lens.OrthographicSize = Mathf.Lerp(start, end, elapsed / zoomDuration);
            yield return null;
        }
        cinemachineCamera.m_Lens.OrthographicSize = end;
        player.isPlayerInControl = false;
    }
    private IEnumerator IdleTitleAnimation()
    {
        Vector2 startPos = titleRectTransform.anchoredPosition;
        float hoverAmount = 5f; // Maximum vertical movement
        float speed = 0.5f; // Slower movement (lower frequency)

        while (isAnimating)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * speed) * hoverAmount;
            titleRectTransform.anchoredPosition = new Vector2(startPos.x, newY);
            yield return null;
        }
    }
}
