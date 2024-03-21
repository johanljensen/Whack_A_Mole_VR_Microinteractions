using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckmarkPop : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer checkmark;

    [SerializeField]
    private SpriteRenderer circleOutline;

    [SerializeField]
    private Vector3 positionOffset;

    private void Start()
    {
        Debug.Log("Made checkmark prefab instance");
        if(checkmark == null || circleOutline == null)
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

    public void StartFeedback(Color enabledColor, Color colorFeedback, Color disabledColor, Material meshMaterial, float duration, float waitTime, float feedback, GameObject perfText,  float perf)
    {
        Debug.Log("Start Checkmark effect");
        StartCoroutine(ChangeColorOverTime(enabledColor, colorFeedback, disabledColor, meshMaterial, 0.15f, 0.15f, feedback, perfText, perf));
    }

    IEnumerator ChangeColorOverTime(Color colorStart, Color colorFeedback, Color colorEnd, Material meshMaterial, float duration, float waitTime, float feedback, GameObject perfText, float perf)
    {
        Debug.Log("Coroutine GO");

        // Debug Info: performance indication
        var txt = perfText.GetComponentInChildren<Text>();
        if (perf != -1f)
        {
            perfText.SetActive(true);
            txt.text = perf.ToString("0.00");
        }

        float popScale = feedback + 1.0f;
        // float popScale = (feedback * 0.45f) + 1.05f; // other possibility
        Debug.Log("PopScale: " + popScale);
        Vector3 normalSize = transform.localScale;
        Vector3 feedbackSize = transform.localScale * popScale;
        checkmark.color = colorFeedback;
        float elapsedTime = 0;
        circleOutline.color = new Color(circleOutline.color.r, circleOutline.color.g, circleOutline.color.b, colorFeedback.a);
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(normalSize, feedbackSize, (elapsedTime / duration));
            transform.position += new Vector3(0, 1f, 0) * Time.deltaTime;
            meshMaterial.color = Color.Lerp(colorStart, colorFeedback, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
            Debug.Log(new Vector3(0, 10f, 0) * Time.deltaTime);
            Debug.Log(transform.position);
        }

        // Hold the end color for 0.1 seconds
        yield return new WaitForSeconds(waitTime);
        // Then transition back to the start color
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(feedbackSize, normalSize, (elapsedTime / duration));
            meshMaterial.color = Color.Lerp(colorFeedback, colorStart, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        // Then transition to the end color
        elapsedTime = 0;
        Color checkmarkOpacity = checkmark.color;
        while (elapsedTime < 0.8f)
        {
            checkmarkOpacity.a = Mathf.Lerp(colorFeedback.a, 0, (elapsedTime / 0.8f));
            checkmark.color = checkmarkOpacity;
            circleOutline.color = new Color(circleOutline.color.r, circleOutline.color.g, circleOutline.color.b, Mathf.Lerp(colorFeedback.a, 0, (elapsedTime / 0.8f)));
            meshMaterial.color = Color.Lerp(colorStart, colorEnd, (elapsedTime / 0.8f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        circleOutline.color = new Color(circleOutline.color.r, circleOutline.color.g, circleOutline.color.b, 0.0f);
        meshMaterial.color = colorEnd;
        checkmarkOpacity.a = 0f; // force  0 opacity at end.
        checkmark.color = checkmarkOpacity;
        transform.localScale = normalSize;
        perfText.SetActive(false);

        Debug.Log("Coroutine END");
        Destroy(gameObject);
    }
}
