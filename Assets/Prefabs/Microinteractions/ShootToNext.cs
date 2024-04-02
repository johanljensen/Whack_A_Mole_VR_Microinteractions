using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootToNext : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer arrowSprite;

    [SerializeField]
    float moveSpeed;

    [SerializeField]
    float connectDistance;

    bool moving = false;

    private void Awake()
    {
        arrowSprite.enabled = false;
    }

    public void SetTransform(Transform moleTransform)
    {
        transform.position = moleTransform.position;
    }

    public void ShootToNewMole(Mole newMole)
    {
        if(!moving)
        {
            StartCoroutine(MoveTowardsMole(newMole.transform.position));
        }
    }

    private IEnumerator MoveTowardsMole(Vector3 molePosition)
    {
        moving = true;
        arrowSprite.enabled = true;

        transform.LookAt(molePosition);
        arrowSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 90));

        while (Vector3.Distance(transform.position, molePosition) > connectDistance)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
