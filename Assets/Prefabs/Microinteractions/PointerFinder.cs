using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerFinder : MonoBehaviour
{
    //needs to know cursor and mole
    //update target mole on mole activation
    //require viewing angle to active mole
    //update on cursor movement
    //disappear in proximity to mole

    Transform activeMole;
    Transform playerCursor;
    Transform headCamera;

    [SerializeField]
    float viewAngleToActivate;

    [SerializeField]
    float minDistanceToShow;

    [SerializeField]
    float effectSpeed;

    [SerializeField]
    LineRenderer lineRenderer;
    [SerializeField]
    Color lineColor;

    [SerializeField]
    float maxAlpha = 50;
    float currentAlpha = 0;
    [SerializeField]
    float alphaFactor = 0.2f;
    [SerializeField]
    float lineWidth = 0.2f;

    private void Awake()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    public void SetTransform(Transform moleTransform)
    {
        transform.position = moleTransform.position;
        transform.parent = moleTransform;
    }

    public void StartFinder(Transform moleTransform)
    {
        headCamera = Camera.main.transform;
        activeMole = moleTransform;
        playerCursor = FindObjectOfType<BasicLaserCursor>().transform;

        StartCoroutine(PointToMole());
    }

    private IEnumerator PointToMole()
    {
        Debug.Log("Start finder routine");
        while (true)
        {
            Vector3 lookingDirection = headCamera.transform.forward;
            Vector3 relativeDirection = activeMole.position - headCamera.position;

            float viewAngle = Vector3.Angle(lookingDirection, relativeDirection);

            float pointerDistance = Vector3.Distance(activeMole.position, playerCursor.position);

            //Debug.Log("TEST");
            //Debug.Log(viewAngle + " : " + viewAngleToActivate + " - " + (viewAngle < viewAngleToActivate));
            //Debug.Log(pointerDistance + " : " + minDistanceToShow + " - " + (pointerDistance > minDistanceToShow));


            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            //Debug.Log("Looking for condition");

            if (viewAngle < viewAngleToActivate && pointerDistance > minDistanceToShow)
            {
                lineRenderer.enabled = true;
                //Debug.Log("Updating line");

                Vector3[] positions = new Vector3[] { activeMole.position, playerCursor.position };
                lineRenderer.SetPositions(positions);

                Debug.Log("Distance: " + (pointerDistance-minDistanceToShow));
                float desiredAlpha = (pointerDistance - minDistanceToShow) * alphaFactor;
                Debug.Log("Current Alpha: " + currentAlpha);
                Debug.Log("Desired Alpha: " + desiredAlpha);
                if(desiredAlpha > maxAlpha) { desiredAlpha = maxAlpha; }

                currentAlpha = Mathf.Lerp(currentAlpha, desiredAlpha, alphaFactor);

                Gradient lineGradient = new Gradient();
                lineGradient.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(lineColor, 1.0f),
                    new GradientColorKey(lineColor, 0.0f)
                };

                lineGradient.alphaKeys = new GradientAlphaKey[] { 
                    new GradientAlphaKey(currentAlpha, 1.0f),
                    new GradientAlphaKey(currentAlpha, 0.1f),
                    new GradientAlphaKey(0.0f, 0.0f)
                };

                //lineRenderer.startColor = startColor;
                //lineRenderer.endColor = endColor;

                lineRenderer.colorGradient = lineGradient;

                /*PointerArrow version
                Debug.Log("CONFIRM FINDER");
                arrowSprite.gameObject.SetActive(true);

                float moveTimer = 0;
                transform.position = playerCursor.position;

                transform.LookAt(activeMole.position);
                arrowSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 90));

                while (moveTimer < effectSpeed)
                {
                    Vector3 newPosition = Vector3.Lerp(playerCursor.position, activeMole.position, moveTimer / effectSpeed);

                    arrowSprite.transform.position = newPosition;
                    moveTimer += Time.deltaTime;
                    yield return null;
                }

                arrowSprite.gameObject.SetActive(false);
                */
            }
            else
            {
                lineRenderer.enabled = false;
            }

            yield return null;
        }
    }
}
