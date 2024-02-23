using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PauseTextsSO : ScriptableObject
{
    [TextArea(3,10)]
    public string PleaseWaitText = "Please wait for the therapist to resume the game";
    [TextArea(3, 10)]
    public string AlgorithmRateText = "With this algorithm, the feedback clearly corresponded to whether I was fast or slow." +
        " (highly disagree) 1 2 3 4 5 6 7 (highly agree)";
    [TextArea(3, 10)]
    public string PleaseExplainText = "Please explain what happened";
    [TextArea(3, 10)]
    public string HowMuchFeedbackText = "How much feedback did you get? (nothing) 1 2 3 4 5 6 7 (a lot)";
    [TextArea(3, 10)]
    public string HowFeelFeedbackText = "Overall, How good did the feedback feel? (not at all good) 1 2 3 4 5 6 7 (very good)";
}
