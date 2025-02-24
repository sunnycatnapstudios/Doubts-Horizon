using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private UIItem itemPrefab;

    [SerializeField]
    private UIPartyMember memberPrefab;

    public Item test;
    //private Sprite image;
    public int quantity;
    public string title, description;

    [SerializeField]
    private RectTransform partyPanel;


    [SerializeField]
    private RectTransform contentPanel;

    [SerializeField]
    private UIDescription descriptionUI;

    List<UIItem> listOfItems= new List<UIItem>();
    List<UIPartyMember> listOfMembers = new List<UIPartyMember>();
    private PartyManager partyManager;
    
    void Start()
    {

        partyManager = GameObject.FindWithTag("Player").GetComponent<PartyManager>();
        Debug.Log(partyManager);
    }
    public void InitializeInventory(int inventorySlotsAmount) {
        for (int i = 0; i < inventorySlotsAmount; i++)
        {
            UIItem item = Instantiate(itemPrefab,Vector3.zero, Quaternion.identity);
            item.transform.SetParent(contentPanel, false);
            listOfItems.Add(item);
            item.OnItemClick += HandleItemSelection;
            
        }
    
    }
    private void Awake()
    {
        Hide();
        descriptionUI.ResetDescription();

    }
    
    public void InitializeParty()
    {
        

        partyManager = GameObject.FindWithTag("Player").GetComponent<PartyManager>();
       
            UIPartyMember person = Instantiate(memberPrefab, Vector3.zero, Quaternion.identity);
            person.transform.SetParent(partyPanel, false);
            listOfMembers.Add(person);
            person.OnItemClicked += HandlePartySelection;
            Debug.Log("dsfa");

        
    }

    public void AddPartyMember()
    {
        UIPartyMember person = Instantiate(memberPrefab, Vector3.zero, Quaternion.identity);
        person.transform.SetParent(partyPanel, false);
        // person.transform.localScale = Vector3.one;
        listOfMembers.Add(person);
        person.OnItemClicked += HandlePartySelection;


    }

    public void fixPartyUIMembers(int len)
      
    {
        len += 1;
        if (len > listOfMembers.Count)
        {
            for (int i = listOfMembers.Count; i < len; i++)
            {
                UIPartyMember person = Instantiate(memberPrefab, Vector3.zero, Quaternion.identity);
                person.transform.SetParent(partyPanel, false);
                // person.transform.localScale = Vector3.one;
                listOfMembers.Add(person);
                person.OnItemClicked += HandlePartySelection;
            }
        }

    }
    private void clearUIInventory() {
        foreach (UIItem item in listOfItems) {
            item.ResetData();
        }
        foreach (UIPartyMember member in listOfMembers) {
            Destroy(member);
        }
    }




    private void HandlePartySelection(UIPartyMember member)
    {
        Debug.Log("here????");
        
           
            Survivor held = member.GetMember();
            if (held != null)
            {
                descriptionUI.SetDescription(held.GetName(), held.GetHealth().ToString());
                ClearSelected();
                member.Selected();
            }
        
    }

    private void HandleItemSelection(UIItem item)
    {
       
           
            Item held = item.getItem();
        if (held != null)
        {
            descriptionUI.SetDescription(held.GetName(), held.GetDesc());
            ClearSelected();
            item.Selected();
        }
        
       }

    public void DisplayItem() { }

    public void Show(Dictionary<string, Slot> inventory)
    {
        clearUIInventory();
        gameObject.SetActive(true);
        descriptionUI.ResetDescription();
        int counter = 0;
        foreach(Slot Slot in inventory.Values) {
            listOfItems[counter].SetdisplayItem(Slot.GetItem(),Slot.getCount());
            Debug.Log(Slot.ToString());
            counter++;    
        }
        counter = 0;
        partyManager = GameObject.FindWithTag("Player").GetComponent<PartyManager>();
        fixPartyUIMembers(partyManager.currentPartyMembers.Count);
        listOfMembers[0].SetdisplayItem(partyManager.getPlayer());
        counter = 1;
        foreach (Survivor member in partyManager.currentPartyMembers)
        {
            Debug.Log(counter);
            Debug.Log(listOfMembers.Count);
            listOfMembers[counter].SetdisplayItem(member);
            counter++;
        }

    }
    public void ClearSelected()
    {
        foreach(UIItem item in listOfItems)
        {
            item.Deselect();
        }
        foreach(UIPartyMember member in listOfMembers)
        {
            member.Deselect();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        ClearSelected();
    }
}
