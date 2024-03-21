using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PatternFeedback
{
    public enum FeedbackType
    {
        None,
        CursorTrail_Operation,
        Checkmark_Action,
        MoleExplode_Action,
        Heatmap_Task
    }

    private static FeedbackType selectedFeedback = FeedbackType.None;

    public static void SetFeedbackType(FeedbackType newType)
    {
        selectedFeedback = newType;
    }

    public static FeedbackType GetFeedbackType()
    {
        return selectedFeedback;
    }
}
