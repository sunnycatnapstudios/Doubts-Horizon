using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject infoButton;
    private ButtonInfoPosition buttonInfoPosition;
    public List<string> buttonInfoList = new List<string>();

    private Coroutine stopCheckCoroutine;
    private Coroutine hoverCoroutine;
    private Vector2 stoppedPointerPos;
    private bool isHovering;
    private float movementThreshold = 20f;

    void OnEnable()
    {
        infoButton = GameObject.FindGameObjectWithTag("InfoButton");
        buttonInfoPosition = infoButton.GetComponent<ButtonInfoPosition>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true; // Mark that we're inside the button area
        if (stopCheckCoroutine != null)
        {
            StopCoroutine(stopCheckCoroutine);
        }
        stopCheckCoroutine = StartCoroutine(CheckIfStoppedMoving());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        StopAllCoroutines();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        buttonInfoPosition.NVM(true);
    }

    private IEnumerator CheckIfStoppedMoving()
    {
        float stopThreshold = movementThreshold / 2;
        Vector2 lastPosition = Input.mousePosition;
        float elapsedTime = 0f;

        while (isHovering) // Run as long as the cursor is inside
        {
            yield return null;
            Vector2 currentPos = Input.mousePosition;
            float distance = Vector2.Distance(lastPosition, currentPos);

            if (distance > stopThreshold)
            {
                elapsedTime = 0f;  // Reset timer if user moves too much
                lastPosition = currentPos;
            }
            else
            {
                elapsedTime += Time.unscaledDeltaTime;
                if (elapsedTime >= 0.3f) // Cursor has stopped for a certain duration
                {
                    stoppedPointerPos = currentPos;

                    if (hoverCoroutine == null) // Start hover check only if not already running
                    {
                        hoverCoroutine = StartCoroutine(HoverCheck());
                    }
                    elapsedTime = 0f; // Reset to allow rechecking after hover opens
                }
            }
        }
    }

    private IEnumerator HoverCheck()
    {
        buttonInfoPosition.IWantAHint(buttonInfoList);

        while (isHovering)
        {
            Vector2 currentPointerPos = Input.mousePosition;
            float distance = Vector2.Distance(stoppedPointerPos, currentPointerPos);

            if (distance > movementThreshold)
            {
                StartCoroutine(DelayedClose());
                hoverCoroutine = null;
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator DelayedClose()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        buttonInfoPosition.NVM(false);
        hoverCoroutine = null;
    }

    private void StopAllCoroutines()
    {
        if (stopCheckCoroutine != null)
        {
            StopCoroutine(stopCheckCoroutine);
            stopCheckCoroutine = null;
        }
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }
        buttonInfoPosition.NVM(false);
    }
}