using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPartyMember : MonoBehaviour
{
    
    

    private Survivor member;

    
    [SerializeField]
    private Image image;
    private bool empty = true;
    [SerializeField]
    private Image SelectedImage;
    [SerializeField]
    private TMP_Text nameText;
    [SerializeField]
    private TMP_Text description;

    public List<Sprite> profileImages;
    public event Action<UIPartyMember> OnItemClicked;

    void Awake()
    {
        ResetData();
        Deselect();

    }

    public void OnPointerClicked(BaseEventData data)
    {

        PointerEventData pointerData = (PointerEventData)data;
        Debug.Log("hssere");
        if (pointerData.button == PointerEventData.InputButton.Left)
        {

            Debug.Log("hssere");
            OnItemClicked?.Invoke(this);

        }
    }


    public void Deselect()
    {
        SelectedImage.enabled = false;

    }
    public void ResetData()
    {
        this.image.gameObject.SetActive(false);
        empty = true;
    }
    public void Selected()
    {
        SelectedImage.enabled = true;

    }

    public Survivor GetMember()
    {
        return member;
    }


    public void SetdisplayItem(Survivor member)
    {
        this.image.gameObject.SetActive(true);
        this.image.sprite = member.GetSprite();
        nameText.text = member.GetName();
        description.text = member.Fed.ToString();
        
       
        this.member = member;

        empty = false;

    }
}
