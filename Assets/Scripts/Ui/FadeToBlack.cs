using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    Image image;
    public float fadeSpeed = 1f;
    DeathDialogue dialogue;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        image.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   public  IEnumerator fadetoblack() {
        image.enabled = true;
        while (image.color.a < 1) {
            float fadeAmount = image.color.a + (Time.unscaledDeltaTime * fadeSpeed);
            Color newColor = new Color(image.color.r, image.color.g, image.color.b,
                fadeAmount);
            image.color = newColor;
            yield return null;
        }

        //StartCoroutine(fadeout());
    }

   public  IEnumerator fadeout() {
        
        while (image.color.a > 0) {
            float fadeAmount = image.color.a - (Time.unscaledDeltaTime * fadeSpeed);
            Color newColor = new Color(image.color.r, image.color.g, image.color.b,
                fadeAmount);
            image.color = newColor;
            yield return null;
        }
        image.enabled = false;


    }


}
