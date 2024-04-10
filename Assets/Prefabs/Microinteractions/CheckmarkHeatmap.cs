using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckmarkHeatmap : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer checkmark;

    [SerializeField]
    private SpriteRenderer circleOutline;

    [SerializeField]
    private Vector3 positionOffset;

    [SerializeField]
    private float fadeInTime;

    [SerializeField]
    private float fadeOutTime;

    private void Start()
    {
        if(checkmark == null || circleOutline == null)
        {
            Debug.LogError("ERROR: Checkmark sprite references not set");
        }
    }

    public void SetTransform(Transform moleTransform)
    {
        transform.parent = moleTransform;
        transform.position = moleTransform.position + positionOffset;
    }

    public void StartFeedback(Color colorFeedback, Material meshMaterial, float waitTime, float feedback)
    {
        StartCoroutine(ChangeColorOverTime(colorFeedback, meshMaterial, waitTime, feedback));
    }

    IEnumerator ChangeColorOverTime(Color colorFeedback, Material meshMaterial, float waitTime, float feedback)
    {
        float popScale = feedback + 1.0f;

        Vector3 normalSize = transform.localScale;
        Vector3 feedbackSize = transform.localScale * popScale;

        Color colorStart = meshMaterial.color;
        checkmark.color = colorFeedback;
        circleOutline.color = new Color(circleOutline.color.r, circleOutline.color.g, circleOutline.color.b, colorFeedback.a);

        float elapsedTime = 0;
        while (elapsedTime < fadeInTime)
        {
            transform.localScale = Vector3.Lerp(normalSize, feedbackSize, (elapsedTime / fadeInTime));
            meshMaterial.color = Color.Lerp(colorStart, colorFeedback, (elapsedTime / fadeInTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Remain fully visible for synchronized time
        yield return new WaitForSeconds(waitTime - fadeInTime - fadeOutTime);
        // Then start fading away

        elapsedTime = 0;
        while (elapsedTime < fadeOutTime * 0.3f)
        {
            transform.localScale = Vector3.Lerp(feedbackSize, normalSize, (elapsedTime / fadeOutTime));
            meshMaterial.color = Color.Lerp(colorFeedback, colorStart, (elapsedTime / fadeOutTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Color checkmarkOpacity = checkmark.color;
        Color outlineOpacity = circleOutline.color;
        while (elapsedTime < fadeOutTime)
        {
            transform.localScale = Vector3.Lerp(feedbackSize, normalSize, (elapsedTime / fadeOutTime));
            meshMaterial.color = Color.Lerp(colorFeedback, colorStart, (elapsedTime / fadeOutTime));
            elapsedTime += Time.deltaTime;

            float fadeValue = Mathf.Lerp(colorFeedback.a, 0, (elapsedTime / fadeOutTime));
            checkmarkOpacity.a = fadeValue;
            checkmark.color = checkmarkOpacity;
            outlineOpacity.a = fadeValue;
            circleOutline.color = outlineOpacity;
            yield return null;
        }

        /*
        circleOutline.color = new Color(circleOutline.color.r, circleOutline.color.g, circleOutline.color.b, 0.0f);
        meshMaterial.color = colorEnd;
        checkmarkOpacity.a = 0f; // force  0 opacity at end.
        checkmark.color = checkmarkOpacity;
        transform.localScale = normalSize;
        */

        Debug.Log("Coroutine END");
        Destroy(gameObject);
    }
}
