using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTransition : MonoBehaviour {
    public Image left, right;
    public bool _start;

    void Start() {
        left = this.transform.Find("Left").GetComponent<Image>();
        right = this.transform.Find("Right").GetComponent<Image>();

        left.fillAmount = right.fillAmount = 0;

        left.fillOrigin = (int)Image.OriginHorizontal.Left;
        right.fillOrigin = (int)Image.OriginHorizontal.Right;
    }

    public void LeaveBattle() {
        _start = false;
        StartCoroutine(LeaveBattleAnim());
    }
    public IEnumerator LeaveBattleAnim() {
        left.fillOrigin = (int)Image.OriginHorizontal.Left;
        right.fillOrigin = (int)Image.OriginHorizontal.Right;

        float elapsedTime = 0f; float duration = .5f;
        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3);

            left.fillAmount = right.fillAmount = Mathf.Lerp(0f, 1f, easedT);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        left.fillAmount = right.fillAmount = 1f;

        left.fillOrigin = (int)Image.OriginHorizontal.Right;
        right.fillOrigin = (int)Image.OriginHorizontal.Left;

        yield return new WaitForSecondsRealtime(.4f);

        elapsedTime = 0f;
        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3);

            left.fillAmount = right.fillAmount = Mathf.Lerp(1f, 0f, easedT);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        left.fillAmount = right.fillAmount = 0f;

        left.fillOrigin = (int)Image.OriginHorizontal.Left;
        right.fillOrigin = (int)Image.OriginHorizontal.Right;
    }
    void Update() {
        if (_start) {
            LeaveBattle();
        }
    }
}
