using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementGuide : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer arrowSprite;

    Transform playerCursor;
    List<PositionHistory> trackedPositions;

    [SerializeField]
    float trackTime;

    [SerializeField]
    float minDist;
    [SerializeField]
    float maxDist;

    [SerializeField]
    float arrowOffsetBase;
    [SerializeField]
    float arrowOffsetModifier;

    class PositionHistory
    {
        public PositionHistory(Vector3 pos)
        {
            position = pos;
            time = Time.time;
        }

        public Vector3 position;
        public float time;
    }

    private void Awake()
    {
        trackedPositions = new List<PositionHistory>();
        arrowSprite.enabled = false;
    }

    public void SetTransform()
    {
        playerCursor = FindObjectOfType<BasicLaserCursor>().transform;

        transform.position = playerCursor.position;
        transform.parent = playerCursor;
    }

    public void StartGuide()
    {
        StartCoroutine(UpdateGuide());
    }

    private IEnumerator UpdateGuide()
    {
        while (true)
        {
            trackedPositions.Add(new PositionHistory(playerCursor.position));

            while (trackedPositions[0].time < Time.time - trackTime)
            {
                trackedPositions.RemoveAt(0);
            }

            if (trackedPositions.Count > 1)
            {
                float trackedDistance = Vector3.Distance(trackedPositions[0].position, playerCursor.position);

                if (trackedDistance > minDist)
                {
                    arrowSprite.enabled = true;

                    transform.LookAt(playerCursor.position + (trackedPositions.Last().position - trackedPositions[0].position));
                    arrowSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 90));
                    arrowSprite.transform.localPosition = new Vector3(0, 0, arrowOffsetBase + trackedDistance * arrowOffsetModifier);
                }
                else
                {
                    arrowSprite.enabled = false;
                }
            }
            yield return null;
        }
    }
}
