using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeatmapHandler : MonoBehaviour
{
    [SerializeField]
    Material lineMaterial;

    public void ProcessTaskHeatmap(string feedbackType, Dictionary<int, Mole> moles, PerfData perfLeft, PerfData perfRight, List<(int id, float val)> molePerf, float duration, SoundManager soundManager)
    {
        Debug.Log("Yes3");
        // TODO: Currently mole performance is just averaged across both controllers.
        // The number of shots go from 1 to max number. if maxShot is less than 1, no moles were shot.
        if (perfRight.maxShot < 1) return;

        // perfR.maxShot and perfL.maxShot are the same value.
        for (int i = 1; i <= perfRight.maxShot; i++)
        {
            List<float> perfs = new List<float>();

            int id = -1;
            if (perfRight.moleShootOrder.ContainsKey(i))
            {
                id = perfRight.moleShootOrder[i];
                perfs.AddRange(perfRight.lastJudgesByMole[id]);
            }
            if (perfLeft.moleShootOrder.ContainsKey(i))
            {
                id = perfLeft.moleShootOrder[i];
                perfs.AddRange(perfLeft.lastJudgesByMole[id]);
            }
            if (perfs.Count > 0)
            {
                (int id, float val) perf = (id, perfs.Average());
                molePerf.Add(perf);
            }
        }

        foreach (var moleP in molePerf)
        {
            Debug.Log(moleP.id + " : " + moleP.val);
        }

        Debug.Log(feedbackType);
        switch(feedbackType)
        {
            case "HeatmapOrder":
                StartCoroutine(ShowOrderFeedback(moles, duration, molePerf, 0.15f, soundManager));
                break;
            case "HeatmapChart":
                StartCoroutine(ShowChartFeedback(moles, duration, molePerf, 0.15f, soundManager));
                break;
            case "HeatmapTier":
                StartCoroutine(ShowTierFeedback(moles, duration, molePerf, 0.15f, soundManager));
                break;
            default:
                break;
        }
    }

    private IEnumerator ShowOrderFeedback(Dictionary<int, Mole> moles, float duration, List<(int id, float val)> molePerf, float animationDelay, SoundManager soundManager)
    {
        float timeSpent = 0f;

        foreach (var fb in molePerf)
        {
            if (fb.id != -1)
            {
                moles[fb.id].PlayFeedback(fb.val, duration - timeSpent);
                soundManager.PlaySoundWithPitch(gameObject, SoundManager.Sound.greenMoleHit, fb.val);
                timeSpent += animationDelay;
                yield return new WaitForSeconds(animationDelay);
            }
        }
    }

    private IEnumerator ShowChartFeedback(Dictionary<int, Mole> moles, float duration, List<(int id, float val)> molePerf, float animationDelay, SoundManager soundManager)
    {
        Debug.Log("CHART CHART CHART");
        float timeSpent = 0f;
        Mole prevMole = null;
        float lineSpeed = 20;
        float bounceBuffer = 0.2f;

        List<GameObject> lineList = new List<GameObject>();

        foreach (var fb in molePerf)
        {
            if (fb.id != -1)
            {
                Mole nextMole = moles[fb.id];
                if(prevMole != null)
                {
                    LineRenderer moleConnector = new GameObject().AddComponent<LineRenderer>();
                    lineList.Add(moleConnector.gameObject);

                    moleConnector.material = lineMaterial;
                    moleConnector.SetPositions(new Vector3[] { prevMole.transform.position, prevMole.transform.position });
                    moleConnector.startColor = Color.green; moleConnector.endColor = Color.green;
                    moleConnector.startWidth = .1f; moleConnector.endWidth = 0.1f;

                    while((moleConnector.GetPosition(1) - nextMole.transform.position).magnitude > bounceBuffer)
                    {
                        Vector3 positionChange = (nextMole.transform.position - moleConnector.GetPosition(1)).normalized * lineSpeed * Time.deltaTime;
                        moleConnector.SetPosition(1, moleConnector.GetPosition(1) + positionChange);
                        timeSpent += Time.deltaTime;
                        yield return null;
                    }
                }

                nextMole.PlayFeedback(fb.val, duration - timeSpent);
                soundManager.PlaySoundWithPitch(gameObject, SoundManager.Sound.greenMoleHit, fb.val);

                timeSpent += animationDelay;
                prevMole = nextMole;
                yield return new WaitForSeconds(animationDelay);
            }
        }

        yield return new WaitForSeconds(2);

        foreach(GameObject line in lineList)
        {
            Destroy(line);
        }
    }


    private IEnumerator ShowTierFeedback(Dictionary<int, Mole> moles, float duration, List<(int id, float val)> molePerf, float animationDelay, SoundManager soundManager)
    {
        float timeSpent = 0f;
        float tierDelayTime = 1.5f;

        List<(int id, float val)> tier1 = new List<(int id, float val)>();
        List<(int id, float val)> tier2 = new List<(int id, float val)>();
        List<(int id, float val)> tier3 = new List<(int id, float val)>();
        List<(int id, float val)> tier4 = new List<(int id, float val)>();
        List<(int id, float val)> tier5 = new List<(int id, float val)>();

        foreach (var moleP in molePerf)
        {
            switch (moleP.val)
            {
                case > 0.8f: tier1.Add(moleP); break;
                case > 0.6f: tier2.Add(moleP); break;
                case > 0.4f: tier3.Add(moleP); break;
                case > 0.2f: tier4.Add(moleP); break;
                default: tier5.Add(moleP); break;
            }
        }

        List<List<(int id, float val)>> tierLists = new List<List<(int id, float val)>>() { tier5, tier4, tier3, tier2, tier1 };

        foreach(var list in tierLists)
        {
            if (list.Count > 0)
            {
                foreach (var moleP in list)
                {
                    moles[moleP.id].PlayFeedback(moleP.val, duration - timeSpent);
                    soundManager.PlaySoundWithPitch(gameObject, SoundManager.Sound.greenMoleHit, moleP.val);
                    Debug.Log(moleP.val);
                }

                timeSpent += tierDelayTime;
                yield return new WaitForSeconds(tierDelayTime);
            }
        }
    }
}
