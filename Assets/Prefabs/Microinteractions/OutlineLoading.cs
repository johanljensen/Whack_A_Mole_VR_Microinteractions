using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineLoading : MonoBehaviour
{
    [SerializeField]
    Image outlineImage;

    bool isAwaitingUpdate = true;
    bool isBeingUpdated = false;

    private void Awake()
    {
        outlineImage.fillAmount = 0;
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

        if(isAwaitingUpdate)
        {
            StartCoroutine(CheckForUpdateBreak());
        }
    }

    private void UpdateOutlineFill(float progress)
    {
        //Debug.Log(progress);

        isBeingUpdated = true;
        outlineImage.fillAmount = progress;

        if(progress >= 1)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator CheckForUpdateBreak()
    {
        isAwaitingUpdate = false;
        while(isBeingUpdated)
        {
            isBeingUpdated = false;
            yield return null;
            yield return null;
        }

        outlineImage.fillAmount = 0;
        isAwaitingUpdate = true;
    }
}