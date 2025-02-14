using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember
{
    public string Name { get; private set; }
    public int Damage { get; set; }
    public int Health { get; set; }
    public bool isCombatant { get; set; }

    public int curHealth;


    public PartyMember(string name, int damage, int health, bool iscombatant)
    {
        Name = name;
        Damage = damage;
        Health = health;
        isCombatant = iscombatant;
        curHealth = health;
    }
}

public class PartyManager : MonoBehaviour
{
    public int partyCount;
    public GameObject partyMemberTemplate;

    public Dictionary<string, PartyMember> playerStats = new Dictionary<string, PartyMember>
    {
        { "Player", new PartyMember("Player", 50, 150, true)}
    };
    public Dictionary<string, PartyMember> allPartyMembers = new Dictionary<string, PartyMember>
    {
        { "MemberA", new PartyMember("MemberA", 35, 100, true)},
        { "MemberB", new PartyMember("MemberB", 7, 120, false)},
        { "MemberC", new PartyMember("MemberC", 5, 150, false)},
        { "MemberD", new PartyMember("MemberD", 25, 80, true)},
        { "MemberE", new PartyMember("MemberE", 42, 170, true)}
    };
    public List<RuntimeAnimatorController> partyAnimControllers = new List<RuntimeAnimatorController>();

    public List<PartyMember> currentPartyMembers = new List<PartyMember>();
    public List<PartyMember> currentPlayer = new List<PartyMember>();
    public List<GameObject> spawnedPartyMembers = new List<GameObject>();
    public List<PartyMember> partyMemberList = new List<PartyMember>();

    void AssignAnimator(GameObject memberObject, string memberName)
    {
        Animator anim = memberObject.GetComponent<Animator>();

        for (int i = 0; i < partyMemberList.Count; i++)
        {
            if (partyMemberList[i].Name == memberName && i < partyAnimControllers.Count)
            {
                anim.runtimeAnimatorController = partyAnimControllers[i];
                break;
            }
        }
    }

    void UpdatePartyCount()
    {
        //Refresh Onscreen Party when there is a change
        foreach (var obj in spawnedPartyMembers) {Destroy(obj);}
        spawnedPartyMembers.Clear();

        // Spawn GameObjects for each current party member
        for (int i = 0; i < currentPartyMembers.Count; i++)
        {
            GameObject newPartyObject = Instantiate(partyMemberTemplate);

            newPartyObject.name = currentPartyMembers[i].Name;
            newPartyObject.GetComponent<Follower>().order = i+1;
            AssignAnimator(newPartyObject, currentPartyMembers[i].Name);

            spawnedPartyMembers.Add(newPartyObject);

            partyCount = currentPartyMembers.Count;
        }

        Debug.Log($"Current Party Count: {currentPartyMembers.Count}");
    }

    public void AddToParty(string memberName)
    {
        if (allPartyMembers.ContainsKey(memberName) && !currentPartyMembers.Contains(allPartyMembers[memberName]))
        {
            currentPartyMembers.Add(allPartyMembers[memberName]);
            Debug.Log($"{memberName} has joined the party!");
            UpdatePartyCount();
        }
        else
        {
            Debug.Log($"{memberName} is already in the party or doesn't exist.");
        }
    }

    public void RemoveFromParty(string memberName)
    {
        var memberToRemove = currentPartyMembers.Find(member => member.Name == memberName);
        if (memberToRemove != null)
        {
            currentPartyMembers.Remove(memberToRemove);
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
        PartyMember member = currentPartyMembers.Find(m => m.Name == memberName);

        if (member != null)
        {
            member.Health -= damage;
            Debug.Log($"{member.Name} took {damage} damage!");


            if (member.Health <= 0)
            {
                member.Health = 0;
                Debug.Log($"{member.Name} has been defeated!");
                RemoveFromParty(memberName);
            }
        }
    }

    // void ShowFloatingText(string memberName, int damage, Color color)
    // {
    //     GameObject memberObject = GameObject.Find(memberName); // Find the GameObject representing the character
    //     if (memberObject != null && floatingTextPrefab != null)
    //     {
    //         // Vector3 spawnPosition = memberObject.transform.position + new Vector3(0, 1.5f, 0);
    //         Vector3 spawnPosition = floatingTextPrefab.transform.position + new Vector3(0, 1.5f, 0);
    //         // GameObject floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
    //         GameObject floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
    //         floatingText.GetComponent<DamageIndicator>().SetText(damage.ToString(), color);
    //     }
    // }

    void Start()
    {
        partyMemberList = new List<PartyMember>(allPartyMembers.Values);
        currentPlayer.Add(playerStats["Player"]);

//         AddToParty("MemberA");
//         AddToParty("MemberB");
//         AddToParty("MemberC");
//         AddToParty("MemberD");
//         AddToParty("MemberE");
    }

    void Update()
    {
        if (false && Input.GetKeyDown(KeyCode.P)) {
            if (currentPartyMembers.Count<allPartyMembers.Count)
            {
                // Debug.Log("Added PartyMember");
                // AddToParty("MemberD");
                // AddToParty("MemberA");

                // AddToParty(partyMemberList[currentPartyMembers.Count].Name);

                foreach (PartyMember member in partyMemberList){
                    if (!currentPartyMembers.Contains(member))
                    {
                        AddToParty(member.Name);
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("Spawned All Party Members");
            }
        }
        if (false && Input.GetKeyDown(KeyCode.O)) {
            if (currentPartyMembers.Count>0)
            {
                // Debug.Log("Removed PartyMember");
                
                RemoveFromParty(currentPartyMembers[Random.Range(0, currentPartyMembers.Count-1)].Name);
            }
            else
            {
                Debug.Log("Culled All Party Members");
            }
        }
    }
}
