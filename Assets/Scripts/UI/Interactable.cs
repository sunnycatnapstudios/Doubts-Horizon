using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 2f; 
    public LayerMask playerLayer; 
    public string promptText = "e"; 

    [Header("Pop-Up Prompt")]
    public GameObject popUpPrefab; 
    private GameObject currentPopUp; 
    private TMP_Text popUpText; 
    private Animator popUpAnimator; 

    [Header("Interaction Events")]
    public UnityEngine.Events.UnityEvent onInteract; 

    private bool playerInRange = false;

    void Update()
    {
        playerInRange = Physics2D.OverlapCircle(transform.position, interactRange, playerLayer);

        if (playerInRange)
        {
            if (currentPopUp == null)
            {
                CreatePopUp();
            }
            else
            {
                UpdatePopUpPosition();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                onInteract.Invoke(); 
            }
        }
        else
        {
            if (currentPopUp != null)
            {
                Destroy(currentPopUp);
                currentPopUp = null;
            }
        }
    }

    void CreatePopUp()
    {
        currentPopUp = Instantiate(popUpPrefab,
            Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1f),
            Quaternion.identity,
            GameObject.FindGameObjectWithTag("Overworld UI").transform);

        popUpText = currentPopUp.GetComponentInChildren<TMP_Text>();
        if (popUpText != null)
        {
            popUpText.text = promptText;
        }

        popUpAnimator = currentPopUp.GetComponent<Animator>();
        if (popUpAnimator != null)
        {
            popUpAnimator.SetTrigger("PopIn"); 
        }

        currentPopUp.transform.SetSiblingIndex(0);
    }

    void UpdatePopUpPosition()
    {
        if (currentPopUp != null)
        {
            currentPopUp.transform.position =
                Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }

    void OnDestroy()
    {
        if (currentPopUp != null)
        {
            Destroy(currentPopUp);
        }
    }
}