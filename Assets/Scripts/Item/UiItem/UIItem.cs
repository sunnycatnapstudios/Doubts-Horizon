using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class UIItem : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private TMP_Text quantity;
    [SerializeField]
    private Image image;
    private bool empty = true;
    [SerializeField]
    private Image SelectedImage;

    public event System.Action<UIItem> OnItemClick;

    private Item heldItem;


    void Awake()
    {
        ResetData();
        Deselect();
        
    }
    public void SetItem(Item item)
    {
        item = heldItem;
    }

    public void OnPointerClick(BaseEventData data)
    {
        
        PointerEventData pointerData = (PointerEventData)data; 
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            
                Debug.Log("here");
                OnItemClick?.Invoke(this);
                
          
        }


    }
    public void Deselect()
    {
        SelectedImage.enabled = false;

    }
    public void ResetData()
    {
        this.image.gameObject.SetActive(false);
        heldItem = null;
        
        empty = true;
    }
    public void Selected()
    {
        SelectedImage.enabled = true;

    }

    public Item getItem()
    {
        return heldItem;
    }


    public void SetdisplayItem( Item item, int quantity)
    {
        this.image.gameObject.SetActive(true);
        this.image.sprite = item.GetSprite();
        this.quantity.text = quantity.ToString();
        heldItem= item;

        empty = false;

    }
}
