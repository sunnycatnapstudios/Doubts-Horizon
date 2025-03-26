using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour {
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

    List<UIItem> listOfItems = new List<UIItem>();
    List<UIPartyMember> listOfMembers = new List<UIPartyMember>();
    private PartyManager partyManager;

    private Item selectedItem;
    private Survivor selectedMember;

    private Item usingItem;
    private Player player;

    //for updating view in inventory after item because im stupid long sigh
    private UIPartyMember _uIPartyMember;
    private UIItem _uIItem;

    [SerializeField]

    private Button useButton;
    //[SerializeField]
    private TMPro.TMP_Text buttonText;
    private ButtonState state;
    enum ButtonState {
        item,
        member
    }
    void Start() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        partyManager = GameObject.FindWithTag("Player").GetComponent<PartyManager>();

        Debug.Log(partyManager);
    }
    public void InitializeInventory(int inventorySlotsAmount) {
        for (int i = 0; i < inventorySlotsAmount; i++) {
            UIItem item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(contentPanel, false);
            listOfItems.Add(item);
            item.OnItemClick += HandleItemSelection;

        }

    }
    private void Awake() {
        Hide();
        descriptionUI.ResetDescription();
        useButton.onClick.AddListener(OnButtonClick);
        buttonText = useButton.GetComponentInChildren<TMPro.TMP_Text>();
        buttonText.text = "use";





    }

    public void InitializeParty() {


        //partyManager = GameObject.FindWithTag("Player").GetComponent<PartyManager>();

        //    UIPartyMember person = Instantiate(memberPrefab, Vector3.zero, Quaternion.identity);
        //    person.transform.SetParent(partyPanel, false);
        //    listOfMembers.Add(person);
        //    person.OnItemClicked += HandlePartySelection;
        //    //Debug.Log("dsfa");

    }

    public void AddPartyMember() {
        UIPartyMember person = Instantiate(memberPrefab, Vector3.zero, Quaternion.identity);
        person.transform.SetParent(partyPanel, false);
        // person.transform.localScale = Vector3.one;
        listOfMembers.Add(person);
        person.OnItemClicked += HandlePartySelection;


    }

    public void fixPartyUIMembers(int len) {
        //len += 1;
        if (len > listOfMembers.Count) {
            for (int i = listOfMembers.Count; i < len; i++) {
                UIPartyMember person = Instantiate(memberPrefab, Vector3.zero, Quaternion.identity);
                person.transform.SetParent(partyPanel, false);
                // person.transform.localScale = Vector3.one;
                listOfMembers.Add(person);
                person.OnItemClicked += HandlePartySelection;
            }
        }

    }
    private void clearUIInventory() {
        Debug.Log("clearing");
        foreach (UIItem item in listOfItems) {
            item.ResetData();
        }
        foreach (UIPartyMember member in listOfMembers) {
            Destroy(member.gameObject);
        }
        listOfMembers.Clear();
    }




    private void HandlePartySelection(UIPartyMember member) {

        Survivor held = member.GetMember();
        if (held != null) {
            descriptionUI.SetDescription(held.GetName(), held.GetInventoryDialogue());
            ClearSelected();
            member.Selected();
            Debug.Log("here????");
            selectedMember = member.GetMember();
            if (usingItem != null) {
                buttonText.text = "use " + usingItem.GetName() + " on " + selectedMember.GetName();
                _uIPartyMember = member;
            } else if (!held.UnKickable) {


                useButton.gameObject.SetActive(true);
                buttonText.text = "kick" + held.GetName();
                state = ButtonState.member;
            } else {
                refreshButton();
                member.Selected();
            }
        }



    }

    private void HandleItemSelection(UIItem item) {


        Item held = item.getItem();
        if (held != null) {

            descriptionUI.SetDescription(held.GetName(), held.GetDesc());
            ClearSelected();
            refreshButton();
            item.Selected();
            UsableInInventory thisItem = item.getItem() as UsableInInventory;
            if (thisItem != null) {
                selectedItem = held;
                useButton.gameObject.SetActive(true);
                state = ButtonState.item;

            }


        }

    }
    private void OnButtonClick() {

        Debug.Log($"UIInventory state {state.ToString()}");
        switch (state) {
            case (ButtonState.item):

                if (usingItem == null && selectedMember == null) {
                    usingItem = selectedItem;
                    buttonText.text = "use on Who?";
                } else if (usingItem != null && selectedMember != null) {
                    buttonText.text = "using on " + selectedMember.GetName();
                    //use item

                    UsableInInventory thisItem = usingItem as UsableInInventory;
                    if (thisItem != null) {
                        thisItem.UseOnMember(selectedMember);
                        _uIPartyMember.SetdisplayItem(selectedMember);

                        player.inventory.removeItemByName(usingItem.GetName());
                        Show(player.inventory.getInventory());
                    }
                    //after
                    refreshButton();

                }
                break;
            case (ButtonState.member):
                partyManager.RemoveFromParty(selectedMember);
                Show(player.inventory.getInventory());
                refreshButton();


                break;
        }


    }
    private void refreshButton() {
        usingItem = null;
        ClearSelected();
        buttonText.text = "use";
        useButton.gameObject.SetActive(false);

    }



    public void DisplayItem() { }

    public void Show(Dictionary<string, Slot> inventory) {
        Debug.Log("showing");
        ClearSelected();
        clearUIInventory();
        useButton.gameObject.SetActive(false);
        gameObject.SetActive(true);
        descriptionUI.ResetDescription();
        int counter = 0;
        foreach (Slot Slot in inventory.Values) {
            listOfItems[counter].SetdisplayItem(Slot.GetItem(), Slot.getCount());

            Debug.Log(Slot.ToString());
            counter++;
        }
        counter = 0;
        partyManager = GameObject.FindWithTag("Player").GetComponent<PartyManager>();
        fixPartyUIMembers(partyManager.currentPartyMembers.Count);
        //listOfMembers[0].SetdisplayItem(partyManager.getPlayer());
        //counter = 1;
        foreach (Survivor member in partyManager.currentPartyMembers) {
            listOfMembers[counter].SetdisplayItem(member);
            counter++;
        }

    }
    public void ClearSelected() {
        foreach (UIItem item in listOfItems) {
            item.Deselect();
        }
        foreach (UIPartyMember member in listOfMembers) {
            member.Deselect();
        }
        selectedMember = null;
        selectedItem = null;
        _uIPartyMember = null;

    }

    public void Hide() {
        gameObject.SetActive(false);
        ClearSelected();
    }
}
