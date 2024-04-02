using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MoleFill : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer fillImage;

    float progressScale;

    [SerializeField]
    float scaleModifier;

    bool isAwaitingUpdate = true;
    bool isBeingUpdated = false;

    private void Awake()
    {
        fillImage.transform.localScale = Vector3.zero;
        Debug.Log("Made a MoleFill");
    }

    public void SetTransform(Transform moleTransform)
    {
        transform.parent = moleTransform;
        transform.position = moleTransform.position;
        transform.rotation = moleTransform.rotation;
    }

    public void ProgressFillEffect(float time, float timer)
    {
        UpdateOutlineFill(timer / time);

        if (isAwaitingUpdate)
        {
            StartCoroutine(CheckForUpdateBreak());
        }
    }

    private void UpdateOutlineFill(float progress)
    {
        Debug.Log(progress);

        progressScale = progress * scaleModifier / 10;

        isBeingUpdated = true;
        fillImage.transform.localScale = new Vector3(progressScale, progressScale, progressScale); ;

        if (progress >= 1)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator CheckForUpdateBreak()
    {
        isAwaitingUpdate = false;
        while (isBeingUpdated)
        {
            isBeingUpdated = false;
            yield return null;
            yield return null;
        }

        fillImage.transform.localScale = Vector3.zero;
        isAwaitingUpdate = true;
    }
}
