using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoleExplode : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer boomSprite;

    [SerializeField]
    private Vector3 positionOffset;

    private void Start()
    {
        Debug.Log("Made checkmark prefab instance");
        if (boomSprite == null)
        {
            Debug.LogError("ERROR: Checkmark sprite references not set");
        }
    }

    public void SetTransform(Transform moleTransform)
    {
        Debug.Log("Set checkmark position");
        transform.parent = moleTransform;
        transform.position = moleTransform.position + positionOffset;
    }

    public void StartFeedback(Color enabledColor, Color colorFeedback, Color disabledColor, Material meshMaterial, float duration, float waitTime, float feedback)
    {
        Debug.Log("Start Checkmark effect");
        StartCoroutine(CircularExpansion(enabledColor, colorFeedback, disabledColor, meshMaterial, duration, waitTime, feedback));
    }

    IEnumerator CircularExpansion(Color colorStart, Color colorFeedback, Color colorEnd, Material meshMaterial, float duration, float waitTime, float feedback)
    {
        float popScale = feedback + 5.0f;
        // float popScale = (feedback * 0.45f) + 1.05f; // other possibility
        Debug.Log("PopScale: " + popScale);
        Vector3 normalSize = transform.localScale;
        Vector3 feedbackSize = transform.localScale * popScale;
        boomSprite.color = colorFeedback;
        float elapsedTime = 0;
        while (elapsedTime < duration * 0.7f)
        {
            transform.localScale = Vector3.Lerp(normalSize, feedbackSize, (elapsedTime / duration));
            meshMaterial.color = Color.Lerp(colorStart, colorFeedback, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Color colorFeedbackFaded = new Color(colorFeedback.r, colorFeedback.g, colorFeedback.b, 0);
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(normalSize, feedbackSize, (elapsedTime / duration));
            meshMaterial.color = Color.Lerp(colorStart, colorFeedbackFaded, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Hold the end color for 0.1 seconds
        yield return new WaitForSeconds(waitTime);

        boomSprite.color = colorEnd;
        transform.localScale = normalSize;
    }
}
