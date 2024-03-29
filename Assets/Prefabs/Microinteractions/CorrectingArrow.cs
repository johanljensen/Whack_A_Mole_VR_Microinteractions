using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectingArrow : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer arrowSprite;

    Transform playerCursor;
    Mole activeMole;

    [SerializeField]
    float minArrowShowDistance;

    [SerializeField]
    float awayDistanceToShow;

    public void StartArrowEffect()
    {
        StartCoroutine(UpdateArrow());
    }

    public void UpdateActiveMole(Mole newMole)
    {
        activeMole = newMole;
        lastClosestDistance = Vector3.Distance(transform.position, playerCursor.position);
        lastTrackedDistance = lastClosestDistance;
    }

    float lastClosestDistance;
    float lastTrackedDistance;

    private IEnumerator UpdateArrow()
    {
        //Wait one frame for code to set the first active mole
        yield return null;


        playerCursor = FindObjectOfType<BasicLaserCursor>().transform;
        Color arrowColour = arrowSprite.color;
        bool showArrow = false;
        float loopTime = 0;

        while(true)
        {
            showArrow = TestShowArrow();

            if(showArrow)
            {
                transform.position = playerCursor.transform.position;
                transform.LookAt(activeMole.transform.position);
                arrowSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 90));

                while (arrowSprite.color.a < .7)
                {
                    loopTime += Time.deltaTime;
                    arrowColour.a = Mathf.Lerp(0, 1, loopTime);
                    arrowSprite.color = arrowColour;
                    transform.position += transform.forward * Time.deltaTime;
                    yield return null;
                }
                loopTime = 0;
                while(arrowSprite.color.a > 0)
                {
                    loopTime += Time.deltaTime;
                    arrowColour.a = Mathf.Lerp(.7f, 0, loopTime);
                    arrowSprite.color = arrowColour;
                    transform.position += transform.forward * Time.deltaTime;
                    yield return null;
                }
                showArrow = false;
            }

            yield return null;
        }
    }

    private bool TestShowArrow()
    {
        float distance = Vector3.Distance(transform.position, playerCursor.position);
        
        if(distance < minArrowShowDistance) { return false; }
        if(distance < lastTrackedDistance) { return false; }

        if (distance < lastClosestDistance)
        {
            lastClosestDistance = distance; 
        }

        if(distance > lastClosestDistance + awayDistanceToShow)
        {
            return true;
        }

        return false;
    }
}
