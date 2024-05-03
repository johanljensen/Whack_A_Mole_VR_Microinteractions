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
    Transform arrowSprite;

    public void SetTransform(Transform cursorTransform)
    {
        transform.position = cursorTransform.position;
        //transform.parent = cursorTransform;

        arrowSprite.gameObject.SetActive(false);
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

            if (viewAngle < viewAngleToActivate && pointerDistance > minDistanceToShow)
            {
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
            }

            yield return new WaitForSeconds(.1f);
        }
    }
}
