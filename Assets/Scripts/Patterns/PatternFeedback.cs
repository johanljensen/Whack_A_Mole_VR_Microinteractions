using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PatternFeedback
{
    public enum FeedbackType
    {
        None,
        CursorTrail_Operation,
        CorrectingArrow_Operation,
        GuidingWhistle_Operation,
        OutlineAnimation_Operation,
        OutlineLoading_Action,
        MolePulses_Operation,
        MoleExplode_Action,
        MoleFill_Action,
        CheckmarkPop_Action,
        MoleDepleted_Action,
        ShootToNext_Action,
        HeatmapOrder_Task,
        HeatmapChart_Task,
        HeatmapTier_Task
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


    private static bool vibrateController = false;
    public static void SetShouldVibrateController(bool setState)
    {  vibrateController = setState; }
    public static bool ShouldVibrateController()
    { return vibrateController; }


    private static bool playAudio = false;
    public static void SetShouldPlaySoundEffect(bool setState)
    { playAudio = setState; }
    public static bool ShouldPlaySoundEffect()
    { return playAudio; }


    private static bool showPerformanceText = false;
    public static void SetShouldShowPerformanceText(bool setState)
    { showPerformanceText = setState; }
    public static bool ShouldShowPerformanceText()
    { return showPerformanceText; }


    private static bool giveNegativeFeedback = false;
    public static void SetShouldGiveNegativeFeedback(bool setState)
    { giveNegativeFeedback = setState; }
    public static bool ShouldGiveNegativeFeedback()
    { return giveNegativeFeedback; }
}
