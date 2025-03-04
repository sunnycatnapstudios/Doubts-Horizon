using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TypeWriter : MonoBehaviour {
    TextMeshProUGUI _tmpProText;
    string writer;
    List<string> dialogueChoices = new List<string>();

    [SerializeField] float delayBeforeStart = 0f;
    [SerializeField] float timeBtwChars = 0.02f;
    [SerializeField] string leadingChar = ""; // TODO: do we ever plan to use this? can it be removed?
    [SerializeField] bool leadingCharBeforeDelay = false;

    public bool hasStartedTyping = false, isTyping = false, skipTyping = false;
    public bool waitingForPause = false, waitingForResponse = false, choiceMade = false;
    public float textChirp;
    
    public bool dropTextEffect = false;
    public _DialogueInputHandler _dialogueInputHandler;


    private AudioClip _sfxTyping;

    public void SetSfxTyping(AudioClip clip) {
        _sfxTyping = clip;
    }

    public void StartTypewriter(string newText) {
        _tmpProText.text = "";
        if (isTyping) StopAllCoroutines();
        writer = newText;
    }

    IEnumerator TypeWriterTMP() {
        isTyping = true;
        _tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";
        yield return new WaitForSeconds(delayBeforeStart);
        textChirp = 0f;

        for (int i = 0; i < writer.Length; ++i) {
            // if (skipTyping && !waitingForPause) {
            //     _tmpProText.text = writer.Replace("{pause}", "").Replace("{dialoguePrompt}", "");
            //     break;
            // }
            if (skipTyping && !waitingForPause) {
                _tmpProText.text = System.Text.RegularExpressions.Regex.Replace(
                    writer.Replace("{pause}", ""), 
                    @"\{dialoguePrompt:[^}]*\}", 
                    ""
                );
                break;
            }

            if (writer.Substring(i).StartsWith("{pause}")) {
                waitingForPause = true;
                while (true) {
                    if (Input.GetKeyDown(KeyCode.E)) break; // Wait for player input
                    yield return null;
                }
                i += 6;
                waitingForPause = false;
                continue;
            }
            if (writer.Substring(i).StartsWith("{dialoguePrompt:")) {
                waitingForResponse = true;
                int endIdx = writer.IndexOf("}", i);
                if (endIdx != -1) {
                    string choiceStr = writer.Substring(i + 15, endIdx - (i + 15));
                    dialogueChoices = new List<string>(choiceStr.Split('|'));
                    ShowChoices();
                    i = endIdx;
                }
                while (!choiceMade) {
                    yield return null;
                }
                waitingForResponse = false;
                continue;
            }
                
            // If there is a style tag attach the whole thing
            if (writer[i] == '<') {
                int start = i;
                while (i < writer.Length) {
                    ++i;
                    if (writer[i] == '>') {
                        break;
                    }
                }

                _tmpProText.text += writer.Substring(start, i - start);
                if (i >= writer.Length) {
                    break;
                }
            }

            char c = writer[i];
            if (_tmpProText.text.Length > 0) {
                _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
            }

            // Apply drop effect
            string formattedChar = dropTextEffect ? $"<voffset=10>{c}</voffset>" : c.ToString();

            _tmpProText.text += c;
            _tmpProText.text += leadingChar;

            if (dropTextEffect) {
                StartCoroutine(AnimateDrop(_tmpProText.text.Length - 1));
            }

            textChirp += .095f;

            if (textChirp >= 0.2f) {
                textChirp = 0f;
                AudioManager.Instance.PlaySound(_sfxTyping);
            }

            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "") {
            _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
        }

        isTyping = false;
        skipTyping = false;
        waitingForPause = false;
    }
    IEnumerator AnimateDrop(int charIndex) {
        TMP_TextInfo textInfo = _tmpProText.textInfo;
        float elapsedTime = 0f;
        float dropDistance = -5f; // How far down the letter starts

        while (elapsedTime < 0.1f) { // Duration of drop animation
            elapsedTime += Time.deltaTime;
            float offset = Mathf.Lerp(dropDistance, 0, elapsedTime / 0.1f); // Smooth drop
            
            _tmpProText.ForceMeshUpdate(); // Ensure text is updated
            if (charIndex >= textInfo.characterCount) break; // Safety check

            TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            for (int i = 0; i < 4; i++) {
                vertices[vertexIndex + i].y -= offset; // Apply vertical offset
            }

            _tmpProText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }
    }

    void Start() {
        _tmpProText = GetComponent<TextMeshProUGUI>();

        if (_tmpProText != null) {
            writer = _tmpProText.text;
            _tmpProText.text = "";
        }
    }
    
    void ShowChoices() {
        Debug.Log($"Prompt: {dialogueChoices[0]}\nChoices: {dialogueChoices[1]}, {dialogueChoices[2]}");
        
        if (_dialogueInputHandler != null) {
            _dialogueInputHandler.ShowChoices(dialogueChoices[0], dialogueChoices[1], dialogueChoices[2]);
        } else {
            Debug.Log("Input Handler not reached, not assigned, or not setup");
        }
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.E) && isTyping && _tmpProText.text.Length > 3 && !waitingForPause) {
            skipTyping = true;
        }

        // Start typing only after activation and delay
        if (hasStartedTyping && !isTyping) {
            hasStartedTyping = false;
            StartCoroutine("TypeWriterTMP");
        }

        if (waitingForResponse && Input.GetKeyDown(KeyCode.R)) {
            Debug.Log("Choice confirmed!");
            choiceMade = true;
        }
    }
}
