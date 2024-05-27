using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineAnimation : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer animSprite;

    [SerializeField]
    float minScale;
    [SerializeField]
    float maxScale;

    [SerializeField]
    float speedScale;
    float animSpeed;

    [SerializeField] //The angle above which the pulse will be the biggest
    float viewAngleMax;
    [SerializeField] //The angle under which the pulse will be the smallest
    float viewAngleMin;

    bool pulseOut = true;
    private Transform headCamera;

    [SerializeField]
    bool useCursorDistance;

    public void SetTransform(Transform moleTransform)
    {
        transform.parent = moleTransform;
        transform.position = moleTransform.position;
        transform.rotation = moleTransform.rotation;

        headCamera = Camera.main.transform;
    }

    public void StartAnimEffect()
    {
        StartCoroutine(AnimateOverTime());
    }

    private IEnumerator AnimateOverTime()
    {
        float animTimer = 0;

        while(true)
        {
            UpdatePulseSpeed();
            while(animSpeed == 0)
            {
                yield return null;
                UpdatePulseSpeed();
            }

            if(pulseOut)
            {
                animTimer += Time.deltaTime;
                float animScale = Mathf.Lerp(minScale, maxScale, animTimer * animSpeed);
                animSprite.transform.localScale = Vector3.one * animScale;

                yield return null;

                if(animScale >= maxScale)
                {
                    pulseOut = false;
                    animTimer = 0;
                }
            }
            else if(!pulseOut)
            {
                animTimer += Time.deltaTime;
                float animScale = Mathf.Lerp(maxScale, minScale, animTimer * animSpeed);
                animSprite.transform.localScale = Vector3.one * animScale;

                yield return null;

                if (animScale <= minScale)
                {
                    pulseOut = true;
                    animTimer = 0;
                }
            }
        }
    }

    private void UpdatePulseSpeed()
    {
        Vector3 lookingDirection = headCamera.transform.forward;
        Vector3 relativeDirection = transform.position - headCamera.position;

        float viewAngle = Vector3.Angle(lookingDirection, relativeDirection);

        if (viewAngle > viewAngleMax) { animSpeed = 1; }
        else if (viewAngle < viewAngleMin) { animSpeed = 0; }
        else
        {
            animSpeed = (viewAngle - viewAngleMin) / (viewAngleMax - viewAngleMin);
        }

        //Debug.Log(headCamera.name);
        //Debug.Log(viewAngle + " : " + animSpeed);

        if (useCursorDistance)
        {
            //Used for headset-less testing, disable for headset use
            BasicLaserCursor cursor = FindObjectOfType<BasicLaserCursor>();

            float distance = Vector3.Distance(transform.position, cursor.transform.position);

            if (distance > 8) { animSpeed = 1; }
            else if (distance < 1) { animSpeed = 0; }
            else
            {
                animSpeed = (distance - 1) / (8 - 1);
            }
            Debug.Log(animSpeed);
        }
        animSpeed = animSpeed * speedScale;
    }
}
