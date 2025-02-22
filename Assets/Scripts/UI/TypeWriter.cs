using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypeWriter : MonoBehaviour
{
    TMP_Text _tmpProText;
	string writer;
    
    [SerializeField] float delayBeforeStart = 0f;
	[SerializeField] float timeBtwChars = 0.1f;
	[SerializeField] string leadingChar = "";
	[SerializeField] bool leadingCharBeforeDelay = false;

	public bool hasStartedTyping = false, isTyping = false, skipTyping = false;
	public float textChirp;

	public void StartTypewriter(string newText)
	{
		_tmpProText.text = "";
		if (isTyping) StopAllCoroutines();
		writer = newText;
		StartCoroutine("TypeWriterTMP");
	}

    IEnumerator TypeWriterTMP()
    {
		isTyping = true;
        _tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";
        yield return new WaitForSeconds(delayBeforeStart);
		textChirp = 0f;

        foreach (char c in writer)
		{
			if (skipTyping) {
				_tmpProText.text = writer;
				break;
			}

			if (_tmpProText.text.Length > 0)
			{
				_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
			}
			_tmpProText.text += c;
			_tmpProText.text += leadingChar;

			textChirp+=.095f;

			if (GetComponent<AudioSource>() != null && textChirp >= 0.2f) {
				GetComponent<AudioSource>().Play();
				textChirp = 0f;
			}
			yield return new WaitForSeconds(timeBtwChars);
		}

		if (leadingChar != "")
		{
			_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
		}
		isTyping = false; skipTyping = false;
    }

    void Start()
    {
        _tmpProText = GetComponent<TMP_Text>();

        if (_tmpProText != null)
		{
			writer = _tmpProText.text;
			_tmpProText.text = "";

			// StartCoroutine("TypeWriterTMP");
		}
    }

    // Update is called once per frame
    void Update()
    {
		// if (Input.GetKeyDown(KeyCode.E) && isTyping && _tmpProText.text.Length > 3) {skipTyping = true;}

        // Start typing only after activation and delay
		if (hasStartedTyping && !isTyping)
        {
            hasStartedTyping = false;
			StartCoroutine("TypeWriterTMP");
			
        }
    }
}
