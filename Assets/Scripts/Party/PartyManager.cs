using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public int partyCount;
    public GameObject partyMemberTemplate;
    private List<GameObject> partyMembers = new List<GameObject>();
    private List<Survivor> members = new List<Survivor>();
    public List<Survivor> Members { get { return members; } }
    GameObject partySpawn;
    // public GameObject floatingTextPrefab;


    public Survivor player;
    //public Dictionary<string, PartyMember> allPartyMembers = new Dictionary<string, PartyMember>
    //{
    //    { "MemberA", new PartyMember("MemberA", 35, 100, true)},
    //    { "MemberB", new PartyMember("MemberB", 7, 120, false)},
    //    { "MemberC", new PartyMember("MemberC", 5, 150, false)},
    //    { "MemberD", new PartyMember("MemberD", 25, 80, true)},
    //    { "MemberE", new PartyMember("MemberE", 42, 170, true)}
    //};
    public List<RuntimeAnimatorController> partyAnimControllers = new List<RuntimeAnimatorController>();

    public List<Survivor> currentPartyMembers = new List<Survivor>();
   
    public List<GameObject> spawnedPartyMembers = new List<GameObject>();
   

    public List<CharacterStats> getStats() {
        List<CharacterStats> mylist = new List<CharacterStats>();

        foreach( Survivor survivor in members) {
            mylist.Add(survivor.GetCharStats());

        }

        return mylist;

    }
   
    void AssignAnimator(GameObject memberObject,Survivor member)
    {
        Animator anim = memberObject.GetComponent<Animator>();

        
           
        anim.runtimeAnimatorController = member.Animcontroller;

           
    }
    public Survivor getPlayer()
    {
        return player;
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
            AssignAnimator(newPartyObject, currentPartyMembers[i]);

            spawnedPartyMembers.Add(newPartyObject);

            partyCount = currentPartyMembers.Count;
        }

        Debug.Log($"Current Party Count: {currentPartyMembers.Count}");
    }

    public void AddToParty(Survivor member)
    {
        if (!currentPartyMembers.Contains(member))
        {
            member.CurHealth = member.Health;
            currentPartyMembers.Add(member);
            Debug.Log($"{member.GetName()} has joined the party!");
            UpdatePartyCount();
        }
        else
        {
            Debug.Log($"{member.GetName()} is already in the party or doesn't exist.");
        }
    }

    public void RemoveFromParty(Survivor member)
    {
        
        if (currentPartyMembers.Contains(member))
        {
            currentPartyMembers.Remove(member);
            Debug.Log($"{member.GetName()} has been removed from the party.");
            UpdatePartyCount();
        }
        else
        {
            Debug.Log($"{member.GetName()} is not in the party.");
        }
    }

    public void TakeDamage(Survivor member, int damage)
    {
       
        if (member != null)
        {
            member.DecHealth( damage);
            Debug.Log($"{member.Name} took {damage} damage!");


            if (member.CurHealth <= 0)
            {
                member.CurHealth = 0;
                Debug.Log($"{member.Name} has been defeated!");
                RemoveFromParty(member); //should have a death funciton later
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
        //partyMemberList = new List<PartyMember>(allPartyMembers.Values);
        //currentPlayer.Add(playerStats["Player"]);
        
//         AddToParty("MemberA");
//         AddToParty("MemberB");
//         AddToParty("MemberC");
//         AddToParty("MemberD");
//         AddToParty("MemberE");
        player.CurHealth = player.Health;
    }

    

    void Update()
    {
        //if (false && Input.GetKeyDown(KeyCode.P)) {
        //    //if (currentPartyMembers.Count<allPartyMembers.Count)
        //    {
        //        // Debug.Log("Added PartyMember");
        //        // AddToParty("MemberD");
        //        // AddToParty("MemberA");

        //        // AddToParty(partyMemberList[currentPartyMembers.Count].Name);

        //        foreach (PartyMember member in partyMemberList){
        //            if (!currentPartyMembers.Contains(member))
        //            {
        //                AddToParty(member.Name);
        //                break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("Spawned All Party Members");
        //    }
        //}
        if (false && Input.GetKeyDown(KeyCode.O)) {
            if (currentPartyMembers.Count>0)
            {
                // Debug.Log("Removed PartyMember");
                
                RemoveFromParty(currentPartyMembers[Random.Range(0, currentPartyMembers.Count-1)]);
            }
            else
            {
                Debug.Log("Culled All Party Members");
            }
        }
    }
}
