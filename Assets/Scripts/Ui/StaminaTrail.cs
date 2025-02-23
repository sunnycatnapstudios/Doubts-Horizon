using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaTrail : MonoBehaviour
{
    public Image barRef, trailRef;
    public float trailSize = 5f;

    private Color bStartColor = new Color(0.227f, 0.482f, 0.043f), bEndColor = new Color(0.482f, 0.043f, 0.043f);
    private Color sStartColor = new Color(0.423f, 1f, 0.004f), sEndColor = new Color(1f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        trailRef = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (trailRef.fillAmount >= barRef.fillAmount){
            trailRef.fillAmount = Mathf.Lerp(trailRef.fillAmount, barRef.fillAmount, Time.deltaTime * trailSize);
        } else {trailRef.fillAmount = barRef.fillAmount;}

        // Gradually Shifts the Color based on Image Lenght
        float gradBar = Mathf.InverseLerp(.4f, 0f, barRef.fillAmount);
        
        barRef.color = Color.Lerp(sStartColor, sEndColor, gradBar);
        trailRef.color = Color.Lerp(bStartColor, bEndColor, gradBar);
    }
}
