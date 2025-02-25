using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Eyeball : MonoBehaviour
{
    public RectTransform eyeCenter, eyeSclera, eyePupil;
    public Canvas canvas;
    public float radius = 75, rotationSpeed = 360f, smoothSpeed = 8f;
    public Vector3 initialBarPosition;
    public Animator pupilAnimator;
    public bool isDialated, realisticEyeTwitch, idleState, isLookingAtCursor;
    public float currentPupilRadius, dilatedPupilRadius = 200, normalPupilRadius = 50, squishFactor;
    private Coroutine dilationCoroutine;

    public Vector2 offset;
    public float distance;

    public float scaleFactor;
    public float movementRange, movementDuration, pupilAnimationSpeed, randomDilation;
    public Vector2 randomMovement, randomIdleMovement;

    void OnEnable()
    {
        pupilAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        initialBarPosition = eyePupil.localPosition;
        currentPupilRadius = normalPupilRadius;

        canvas = this.transform.GetComponentInParent<Canvas>();

        StartCoroutine(RandomPupilMovement());
        StartCoroutine(RandomEyeState());
        StartCoroutine(RandomPupilDialation());
    }
    public void StartDilatePupil()
    {
        isDialated = true;
        pupilAnimator.speed = pupilAnimationSpeed;
        if (dilationCoroutine != null) StopCoroutine(dilationCoroutine); // Stop only this coroutine
        dilationCoroutine = StartCoroutine(DilatePupil(dilatedPupilRadius, isDialated));
    }
    public void EndDilatePupil()
    {
        isDialated = false;
        pupilAnimator.speed = 1f;
        if (dilationCoroutine != null) StopCoroutine(dilationCoroutine); // Stop only this coroutine
        dilationCoroutine = StartCoroutine(DilatePupil(normalPupilRadius, isDialated));
    }
    private IEnumerator DilatePupil(float targetRadius, bool condition)
    {
        // yield return null;
        float elapsedTime = 0f;
        float startRadius = currentPupilRadius;
        float duration = 0.3f; // Adjust dilation time

        while (elapsedTime < duration && isDialated == condition)
        {
            currentPupilRadius = Mathf.Lerp(startRadius, targetRadius, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        currentPupilRadius = targetRadius;
        dilationCoroutine = null; // Reset reference when done
    }

    private IEnumerator RandomPupilMovement()
    {
        while (realisticEyeTwitch)
        {
            // Generate a small random vector within the movement range
            randomMovement = new Vector2(
                Random.Range(-movementRange, movementRange), // Random X movement
                Random.Range(-movementRange/2, 0f)  // Random Y movement
            );

            yield return new WaitForSecondsRealtime(Random.Range(.2f, movementDuration));
        }
    }
    private IEnumerator RandomEyeState()
    {
        while(true) {
            isLookingAtCursor = !isLookingAtCursor;

            if (!isLookingAtCursor)
            {
                int lookCount = Random.Range(2, 5);
                for (int i = 0; i < lookCount; i++)
                {
                    offset = new Vector2(Random.Range(-60f, 20f), Random.Range(-40f, 20f));
                    yield return new WaitForSecondsRealtime(Random.Range(0.5f, 2f));
                }
            }
            yield return new WaitForSecondsRealtime(Random.Range(3f, 6f));
        }
    }
    private IEnumerator RandomPupilDialation()
    {
        while (true)
        {
            randomDilation = Random.Range(.8f, 1.3f);
            yield return new WaitForSecondsRealtime(Random.Range(.7f, 3f));
        }
    }

    void Update()
    {
        if (isLookingAtCursor)
        {
            // Convert mouse position to local position relative to the eye's parent
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                eyeCenter.parent as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out Vector2 localMousePos
            );

            // Get the world position of the eye center
            Vector2 eyeCenterWorldPos = eyeCenter.position; // Use world position

            // Convert world position back to local position relative to the parent
            Vector2 eyeCenterLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                eyeCenter.parent as RectTransform,
                eyeCenterWorldPos,
                canvas.worldCamera,
                out eyeCenterLocalPos
            );

            Vector2 targetPos = localMousePos;

            // Calculate the distance from the eye center
            offset = targetPos - eyeCenterLocalPos;
            distance = offset.magnitude;

            // Clamp the pupil's movement to the max radius
            if (distance > radius)
            {
                offset = offset.normalized * radius;
            }
        }

        // Apply smooth movement
        eyePupil.localPosition = Vector2.Lerp(eyePupil.localPosition, (offset+randomMovement), smoothSpeed * Time.unscaledDeltaTime);

        // eyePupil.Rotate(Vector3.forward, rotationSpeed * Time.unscaledDeltaTime);
        // eyeSclera.Rotate(Vector3.forward, -rotationSpeed/6 * Time.unscaledDeltaTime);

        // eyePupil.sizeDelta = new Vector2(currentPupilRadius, currentPupilRadius);

        float pupilLookAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        // float smoothAngle = Mathf.LerpAngle(eyePupil.eulerAngles.z, pupilLookAngle, rotationSpeed * Time.unscaledDeltaTime);
        scaleFactor = Mathf.Lerp(1f, .4f, offset.magnitude / currentPupilRadius);

        eyePupil.rotation = Quaternion.Euler(0, 0, (pupilLookAngle+(randomMovement.x)/2));
        eyePupil.sizeDelta = new Vector2(scaleFactor * currentPupilRadius * randomDilation, currentPupilRadius * randomDilation);
    }
}