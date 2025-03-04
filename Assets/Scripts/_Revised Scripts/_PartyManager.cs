using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _PartyManager : MonoBehaviour
{
    private GameStatsManager gameStatsManager;

    public int partyCount;
    public GameObject partyMemberTemplate;
    public List<RuntimeAnimatorController> partyAnimControllers = new List<RuntimeAnimatorController>();
    public List<Sprite> characterProfiles = new List<Sprite>();

    // UI Elements to Update
    private PartySideBar partySideBar;

    void AssignAnimator(GameObject memberObject, string memberName)
    {
        Animator anim = memberObject.GetComponent<Animator>();

        RuntimeAnimatorController matchingController = partyAnimControllers.Find(controller => controller.name == memberName);

        if (matchingController != null) {anim.runtimeAnimatorController = matchingController;}
        else {
            Debug.LogWarning($"No matching animator found for {memberName}. Check if the controller is named correctly.");
        }
    }

    public void UpdatePartyCount()
    {
        foreach (var obj in gameStatsManager.spawnedPartyMembers) {Destroy(obj);}
        gameStatsManager.spawnedPartyMembers.Clear();

        // Spawn GameObjects for each current party member
        for (int i = 0; i < gameStatsManager.currentPartyMembers.Count; i++)
        {
            GameObject newPartyObject = Instantiate(partyMemberTemplate, GameObject.FindGameObjectWithTag("PartyMembers Group").transform);

            newPartyObject.name = gameStatsManager.currentPartyMembers[i].Name;
            newPartyObject.GetComponent<Follower>().order = i+1;
            AssignAnimator(newPartyObject, gameStatsManager.currentPartyMembers[i].Name);

            gameStatsManager.spawnedPartyMembers.Add(newPartyObject);

            partyCount = gameStatsManager.currentPartyMembers.Count;
        }

        Debug.Log($"Current Party Count: {gameStatsManager.currentPartyMembers.Count}");
        UpdatePartyUI();
    }
    public void AddToParty(string memberName)
    {
        if (gameStatsManager.allPartyMembers.ContainsKey(memberName) && !gameStatsManager.currentPartyMembers.Contains(gameStatsManager.allPartyMembers[memberName]))
        {
            gameStatsManager.currentPartyMembers.Add(gameStatsManager.allPartyMembers[memberName]);
            Debug.Log($"{memberName} has joined the party!");
            UpdatePartyCount();
        } else {Debug.Log($"{memberName} is already in the party or doesn't exist.");}
    }
    public void RemoveFromParty(string memberName)
    {
        var memberToRemove = gameStatsManager.currentPartyMembers.Find(member => member.Name == memberName);
        if (memberToRemove != null)
        {
            gameStatsManager.currentPartyMembers.Remove(memberToRemove);
            Debug.Log($"{memberName} has been removed from the party.");
            UpdatePartyCount();
        }
        else
        {
            Debug.Log($"{memberName} is not in the party.");
        }
    }
    public void TakeDamage(string memberName, int damage)
    {
        CharacterStats member = gameStatsManager.currentPartyMembers.Find(m => m.Name == memberName);

        if (member != null)
        {
            member.currentHealth -= damage;
            Debug.Log($"{member.Name} took {damage} damage!");


            if (member.currentHealth <= 0)
            {
                member.currentHealth = 0;
                Debug.Log($"{member.Name} has been defeated!");
                RemoveFromParty(memberName);
            }
        }
    }

    public void UpdatePartyUI()
    {
        partySideBar.UpdateSideBar();
    }
    
    public void Awake()
    {
    //     gameStatsManager = GameStatsManager.Instance;

        // partySideBar = GameObject.FindWithTag("PartySidebar").GetComponent<PartySideBar>();
        
        // partySideBar = GameObject.GetComponent<PartySideBar>();
    }
    void Start()
    {
        gameStatsManager = GameStatsManager.Instance;
        GameObject player = GameObject.FindWithTag("Player");
        partyMemberTemplate = player.GetComponentInChildren<Follower>().gameObject;

        partySideBar = GameObject.FindWithTag("PartySideBar").GetComponent<PartySideBar>();
        
        // partySideBar = GameObject.GetComponent<PartySideBar>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (gameStatsManager.currentPartyMembers.Count<gameStatsManager.allPartyMembers.Count)
            {
                foreach (CharacterStats member in gameStatsManager.allPartyMembers.Values){
                    if (!gameStatsManager.currentPartyMembers.Contains(member))
                    {
                        AddToParty(member.Name);
                        break;
                    }
                }
            }
            else {Debug.Log("Spawned All Party Members");}
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (gameStatsManager.currentPartyMembers.Count>0)
            {
                RemoveFromParty(gameStatsManager.currentPartyMembers[Random.Range(0, gameStatsManager.currentPartyMembers.Count-1)].Name);
            }
            else {Debug.Log("Culled All Party Members");}
        }
    } 
}
