// TODO REMOVE
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject interactionPrompt;  // To display "Press E to interact"
    public GameObject interactedPrompt;  // to display Interacted
    private bool isPlayerNearby = false;

    void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false); 
        }
        
        if (interactedPrompt != null)
        {
            interactedPrompt.SetActive(false); 
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true); 
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false); 
            }
        }
    }

    void Interact()
    {
        Debug.Log("Interacted with NPC");

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);  
        }

        if (interactedPrompt != null)
        {
            interactedPrompt.SetActive(true);  
        }
    }
}
