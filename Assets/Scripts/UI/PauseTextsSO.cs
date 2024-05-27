using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PauseTextsSO : ScriptableObject
{
    [TextArea(3,10)]
    public string PleaseWaitText = "Please wait for the therapist to resume the game";
    [TextArea(3, 10)]
    public string DescribeEffectPlease = "With this algorithm, the feedback clearly corresponded to whether I was fast or slow." +
        " (highly disagree) 1 2 3 4 5 6 7 (highly agree)";
    [TextArea(3, 10)]
    public string AffectYourGameplay = "Please explain what happened";
    [TextArea(3, 10)]
    public string ChangedDifficulty = "How much feedback did you get? (nothing) 1 2 3 4 5 6 7 (a lot)";
}
