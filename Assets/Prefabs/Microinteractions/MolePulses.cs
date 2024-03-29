using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolePulses : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer pulseSprite;

    [SerializeField] //The biggest full size a pulse can be
    float pulseMax;
    [SerializeField] //The smallest full size a pulse can be
    float pulseMin;
    [SerializeField] //The time it takes for the pulse to reach full size
    float pulseSpeed;

    [SerializeField] //The angle above which the pulse will be the biggest
    float viewAngleMax;
    [SerializeField] //The angle under which the pulse will be the smallest
    float viewAngleMin;

    float scaleValue; //The active scaleValue for the current pulse

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

    public void StartPulseEffect()
    {
        StartCoroutine(PulseOverTime());
    }

    private IEnumerator PulseOverTime()
    {
        bool activePulse = true;
        float scaleTimer = 0;
        float pulseTime = 0;
        Color pulseColor = pulseSprite.color;

        while(true)
        {
            //Update scale for new pulse, we don't want to adjust scale while pulse is playing
            UpdatePulseScale();
            float maxScale = pulseMin + (pulseMax - pulseMin) * scaleValue;

            //Reset timer
            pulseTime = 0;

            while (pulseTime < pulseSpeed)
            {

                pulseTime += Time.deltaTime;
                scaleTimer = pulseTime / pulseSpeed;

                float pulseScale = Mathf.Lerp(0, maxScale, scaleTimer);
                //Debug.Log(pulseScale + " : " + pulseTime + " : " + scaleTimer);

                pulseSprite.transform.localScale = new Vector3(pulseScale, pulseScale, .1f);

                pulseColor.a = 2 - scaleTimer * 2;
                pulseSprite.color = pulseColor;

                yield return null;
            }

            yield return null;
        }
    }

    private void UpdatePulseScale()
    {
        Vector3 lookingDirection = headCamera.transform.forward;
        Vector3 relativeDirection = headCamera.position - transform.position;

        float viewAngle = Vector3.Angle(lookingDirection, relativeDirection);

        if (viewAngle > viewAngleMax) { scaleValue = 1; }
        else if (viewAngle < viewAngleMin) { scaleValue = 0; }
        else
        {
            scaleValue = (viewAngle - viewAngleMin) / (pulseMax - viewAngleMin);
        }

        if (useCursorDistance)
        {
            //Used for headset-less testing, disable for headset use
            BasicLaserCursor cursor = FindObjectOfType<BasicLaserCursor>();

            float distance = Vector3.Distance(transform.position, cursor.transform.position);
            Debug.Log(distance);

            if (distance > 8) { scaleValue = 1; }
            else if (distance < 1) { scaleValue = 0; }
            else
            {
                scaleValue = (distance - 1) / (8 - 1);
            }
        }
    }
}
