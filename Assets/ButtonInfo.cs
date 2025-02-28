using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject infoButton;
    private ButtonInfoPosition buttonInfoPosition;
    public List<string> buttonInfoList = new List<string>();

    void OnEnable()
    {
        infoButton = GameObject.FindGameObjectWithTag("InfoButton");
        buttonInfoPosition = infoButton.GetComponent<ButtonInfoPosition>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonInfoPosition.IWantAHint(buttonInfoList);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonInfoPosition.NVM(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        buttonInfoPosition.NVM(true);
    }
}
