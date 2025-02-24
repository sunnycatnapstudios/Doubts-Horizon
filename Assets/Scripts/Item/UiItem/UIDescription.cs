using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIDescription : MonoBehaviour
{
    [SerializeField]
    private TMP_Text title;
    [SerializeField]
    private TMP_Text description;

   
    public void Awake()
    {
        ResetDescription();
    }

    public void ResetDescription()
    {
        this.title.text = "";
        this.description.text = "";
    }

    // Update is called once per frame
    

    public void SetDescription(string name, string description)
    {
        this.title.text = name;
        this.description.text = description;


    }
}
