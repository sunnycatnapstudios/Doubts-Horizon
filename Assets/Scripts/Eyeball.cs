using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Eyeball : MonoBehaviour {
    public RectTransform eyeCenter, eyeSclera, eyePupil;
    public Canvas canvas;
    public float radius = 75, smoothSpeed = 10f;
    public Vector3 initialBarPosition;
    public Animator pupilAnimator;
    public bool isDialated, realisticEyeTwitch, idleState, isLookingAtCursor;
    public float currentPupilRadius, dilatedPupilRadius = 200, normalPupilRadius = 50, squishFactor;
    private Coroutine dilationCoroutine;

    public Vector2 offset, hardOffset;
    public float distance;

    public float scaleFactor, stretchFactor, softRadius, hardRadius;
    public float movementRange, movementDuration, pupilAnimationSpeed, randomDilation;
    public Vector2 randomMovement, randomIdleMovement;
    public bool isAttacking, isHit, inAnimation, testingSomething = false;

    void OnEnable() {
        pupilAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        initialBarPosition = eyePupil.localPosition;
        currentPupilRadius = normalPupilRadius;
        dilatedPupilRadius = normalPupilRadius * 1.2f;

        eyePupil.localPosition = Vector3.zero;
        eyePupil.localScale = new Vector3(1f, 1f, 1f);

        randomDilation = scaleFactor = 1;
        isLookingAtCursor = true;

        canvas = this.transform.GetComponentInParent<Canvas>();

        if (!testingSomething) {
            StartCoroutine(RandomEyeState());
        }
        StartCoroutine(RandomPupilDialation());
        StartCoroutine(RandomPupilMovement());
    }
    public void StartDilatePupil() {
        isDialated = true;
        pupilAnimator.speed = pupilAnimationSpeed;
        if (dilationCoroutine != null) StopCoroutine(dilationCoroutine); // Stop only this coroutine
        dilationCoroutine = StartCoroutine(DilatePupil(dilatedPupilRadius, isDialated));
    }
    public void EndDilatePupil() {
        isDialated = false;
        pupilAnimator.speed = 1f;
        if (dilationCoroutine != null) StopCoroutine(dilationCoroutine); // Stop only this coroutine
        dilationCoroutine = StartCoroutine(DilatePupil(normalPupilRadius, isDialated));
    }
    private IEnumerator DilatePupil(float targetRadius, bool condition) {
        float elapsedTime = 0f;
        float startRadius = currentPupilRadius;
        float duration = 0.3f; // Adjust dilation time

        while (elapsedTime < duration && isDialated == condition) {

            if (inAnimation) {
                yield return null;
                continue;
            }

            currentPupilRadius = Mathf.Lerp(startRadius, targetRadius, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        currentPupilRadius = targetRadius;
        dilationCoroutine = null; // Reset reference when done
    }

    private IEnumerator RandomPupilMovement() {
        while (realisticEyeTwitch) {
            if (inAnimation) {
                yield return null;
                continue;
            }

            // Generate a small random vector within the movement range
            randomMovement = new Vector2(
                Random.Range(-movementRange, movementRange), // Random X movement
                Random.Range(-movementRange / 2, 0f)  // Random Y movement
            );

            yield return new WaitForSecondsRealtime(Random.Range(.2f, movementDuration));
        }
    }
    private IEnumerator RandomEyeState() {
        while (true) {
            if (inAnimation) {
                yield return null;
                continue;
            }

            isLookingAtCursor = !isLookingAtCursor;

            if (!isLookingAtCursor) {
                int lookCount = Random.Range(2, 5);
                for (int i = 0; i < lookCount; i++) {
                    offset = new Vector2(Random.Range(-60f, 20f), Random.Range(-40f, 20f));
                    hardOffset = offset;
                    yield return new WaitForSecondsRealtime(Random.Range(0.5f, 2f));
                }
            }
            yield return new WaitForSecondsRealtime(Random.Range(3f, 6f));
        }
    }
    private IEnumerator RandomPupilDialation() {
        while (true) {
            if (inAnimation) {
                yield return null;
                continue;
            }

            randomDilation = Random.Range(.9f, 1.1f);
            yield return new WaitForSecondsRealtime(Random.Range(.7f, 3f));
        }
    }
    // private IEnumerator RandomMouthLol()
    // {
    // Shapes the Eyeball into a vertical slit, closes it, then opens it to reveal teeth and a tongue
    // also preferably spawns a tongue
    // teeth follow the same rules to shorten them the closer they are to the edge, and are layered in a way that overlaps elements in an interesting way
    // slightly angled to the right, facing the player in action, and slightly shifts Horiz:Vert 3:2
    // yield return null;
    // }
    public void Attack() {
        isAttacking = false;
        inAnimation = true;
        StartCoroutine(BiteDown());
    }
    private IEnumerator BiteDown() {
        isLookingAtCursor = false;
        randomDilation = 1;

        float randomBiteVal = Random.value;
        bool isVerticalSquish = randomBiteVal > 0.5f;

        for (int repeat = 0; repeat < 2; repeat++) {

            float elapsedTime = 0f; float duration = .5f; float pupilSquishStartTime = duration * 0.8f;
            while (elapsedTime < duration) {
                float t = elapsedTime / duration;
                float easedT = 1f - Mathf.Pow(1f - t, 3);

                currentPupilRadius = Mathf.Lerp(currentPupilRadius, 150, easedT);
                offset = Vector2.Lerp(offset, new Vector2(-13, -5), easedT);
                hardOffset = offset;

                if (elapsedTime > pupilSquishStartTime) {
                    float squishT = (elapsedTime - pupilSquishStartTime) / (duration - pupilSquishStartTime);
                    float pupilWidth = Mathf.Lerp(1f, .1f, squishT);

                    eyePupil.localScale = isVerticalSquish ? new Vector3(1f, pupilWidth, 1f)
                                                            : new Vector3(pupilWidth, 1f, 1f);
                }

                elapsedTime += Time.unscaledDeltaTime * 5f;
                yield return null;
            }
            currentPupilRadius = 150;
            offset = new Vector2(-13, -5);
            hardOffset = offset;
            eyePupil.localScale = new Vector3(1f, .1f, 1f);

            yield return new WaitForSecondsRealtime(.1f);

            elapsedTime = 0f;
            while (elapsedTime < duration) {
                float t = elapsedTime / duration;
                float easedT = 1f - Mathf.Pow(1f - t, 3);

                float pupilWidth = Mathf.Lerp(.1f, 1f, easedT);
                eyePupil.localScale = isVerticalSquish ? new Vector3(1f, pupilWidth, 1f)
                                                        : new Vector3(pupilWidth, 1f, 1f);

                elapsedTime += Time.unscaledDeltaTime * 5f;
                yield return null;
            }

            yield return new WaitForSecondsRealtime(.1f);
        }

        yield return new WaitForSecondsRealtime(.3f);

        isLookingAtCursor = true;
        inAnimation = false;
        currentPupilRadius = normalPupilRadius;
        eyePupil.localScale = Vector2.one;
    }
    public void Hit() {
        isHit = false;
        inAnimation = true;
        StartCoroutine(SpinEyeball(1f, 100f, 5f));
    }
    private IEnumerator SpinEyeball(float duration, float arcHeight, float speed) {
        isLookingAtCursor = false;
        randomDilation = 1;

        Vector2 startPosition = new Vector2(-300, -150);
        Vector2 endPosition = new Vector2(300, 200);

        float upperBound = 200; float lowerBound = -200; float yOffsetChange = 25;


        currentPupilRadius = dilatedPupilRadius;

        float elapsedTime = 0f;
        float speedFactor = 1f;

        int spins = Random.Range(2, 4);
        int yDirection = 1;

        // SPIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIINNNNNNNNNNNNN
        for (int i = 0; i < spins; i++) {
            while (elapsedTime < duration) {
                float t = elapsedTime / duration;
                float easedT = 1f - Mathf.Pow(1f - t, 3); // Smooth ease-out

                float x = Mathf.Lerp(startPosition.x, endPosition.x, easedT);
                float y = Mathf.Lerp(startPosition.y, endPosition.y, easedT);

                offset = new Vector2(x, y);
                hardOffset = offset;

                elapsedTime += Time.unscaledDeltaTime * speed * speedFactor;
                // elapsedTime += Time.unscaledDeltaTime * speed;
                yield return null;
            }
            speedFactor = ((float)spins - i) / (float)spins;
            yield return new WaitForSecondsRealtime(0.1f); // Short pause at the end

            elapsedTime = 0f;
            offset = startPosition;
            hardOffset = offset;
            eyePupil.localPosition = startPosition;

            // Adjust y-values for the next spin
            startPosition.y += yOffsetChange * yDirection;
            endPosition.y -= yOffsetChange * yDirection;

            // If the y-values exceed bounds, reverse the direction
            if (endPosition.y <= lowerBound || endPosition.y >= upperBound) { yDirection *= -1; }

        }
        offset = new Vector2(-37.5f, -25f);
        hardOffset = offset;
        currentPupilRadius = 110;
        yield return new WaitForSecondsRealtime(.5f);

        isLookingAtCursor = true;
        inAnimation = false;
        currentPupilRadius = normalPupilRadius;
    }

    void Update() {
        if (isLookingAtCursor) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                eyeCenter.parent as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out Vector2 localMousePos
            );

            Vector2 eyeCenterWorldPos = eyeCenter.position;
            Vector2 eyeCenterLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                eyeCenter.parent as RectTransform,
                eyeCenterWorldPos,
                canvas.worldCamera,
                out eyeCenterLocalPos
            );

            Vector2 targetPos = localMousePos;
            softRadius = radius * 0.75f; // Inner free movement range
            hardRadius = radius - (eyePupil.sizeDelta.x / 2); // Max range

            offset = targetPos - eyeCenterLocalPos;
            hardOffset = offset;

            distance = offset.magnitude;

            if (distance > hardRadius) {
                offset = offset.normalized * hardRadius; // Keep it within max radius
            } else {
                float t = Mathf.InverseLerp(0, radius, distance); // Normalize between 0 and full radius
                float scale = Mathf.Lerp(1f, 0.6f, t * t); // Smooth falloff
                offset = Vector2.Lerp(offset, offset * scale, Time.unscaledDeltaTime * 10f); // Smooth transition
            }
        }

        // Apply smooth movement
        // eyePupil.localPosition = Vector2.Lerp(eyePupil.localPosition, (offset+randomMovement), smoothSpeed * Time.unscaledDeltaTime);
        // float pupilLookAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        // scaleFactor = Mathf.Lerp(1f, .4f, offset.magnitude / currentPupilRadius);

        // eyePupil.rotation = Quaternion.Euler(0, 0, (pupilLookAngle+(randomMovement.x)/2));
        // eyePupil.sizeDelta = new Vector2(scaleFactor * currentPupilRadius * randomDilation, currentPupilRadius * randomDilation);

        scaleFactor = Mathf.Lerp(1f, .4f, hardOffset.magnitude / currentPupilRadius);

        float sizeScaleFactor = Mathf.Clamp01(distance / 250);
        float exponentialScale = .8f + (1.1f - .8f) * Mathf.Abs(1 - (sizeScaleFactor * sizeScaleFactor));
        float pupilLookAngle = Mathf.Atan2(hardOffset.y, hardOffset.x) * Mathf.Rad2Deg;

        float eyeSizeX = currentPupilRadius * exponentialScale * randomDilation * scaleFactor;
        float eyeSizeY = currentPupilRadius * exponentialScale * randomDilation;

        eyePupil.rotation = Quaternion.Euler(0, 0, (pupilLookAngle + (randomMovement.x) / 4));
        eyePupil.sizeDelta = new Vector2(eyeSizeX, eyeSizeY);
        eyePupil.localPosition = Vector2.Lerp(eyePupil.localPosition, (offset + randomMovement), smoothSpeed * Time.unscaledDeltaTime);

        if (isHit) { Hit(); }
        if (isAttacking) { Attack(); }
    }
}
