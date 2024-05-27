using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Mole;

public class MicrointeractionManager : MonoBehaviour
{
    public static MicrointeractionManager Instance;
    public static MicrointeractionManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<MicrointeractionManager>();

            if (Instance == null)
            {
                Debug.LogError("Could not find MicrointeractionManager in the scene hierarchy");
            }
        }

        return Instance;
    }

    [SerializeField]
    bool MicrointeractionsOn;

    [Header("Microinteraction prefabs")]
    [SerializeField]
    CheckmarkPop checkmarkPopPrefab;
    [SerializeField]
    MoleExplode moleExplodePrefab;
    [SerializeField]
    CheckmarkHeatmap checkmarkHeatmapPrefab;
    [SerializeField]
    OutlineAnimation outlineAnimationPrefab;
    [SerializeField]
    MolePulses molePulsesPrefab;
    [SerializeField]
    MoleFill moleFillPrefab;
    [SerializeField]
    OutlineLoading outlineLoadingPrefab;
    [SerializeField]
    CorrectingArrow correctingArrowPrefab;
    [SerializeField]
    GuidingWhistle guidingWhistlePrefab;
    [SerializeField]
    ShootToNext shootToNextPrefab;
    [SerializeField]
    PointerFinder pointerFinderPrefab;
    [SerializeField]
    MovementGuide movementGuidePrefab;

    public void MicrointeractionEndOldModifier()
    {
        if (!MicrointeractionsOn) { return; }

        if(Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.MovementGuide_Operation)
        {
            MovementGuide moveGuide = FindObjectOfType<MovementGuide>();
            Destroy(moveGuide.gameObject);
        }
    }

    public void MicroInteractionStartNewModifier()
    {
        if (!MicrointeractionsOn) { return; }

        if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.MovementGuide_Operation)
        {
            MovementGuide moveGuide = Instantiate(movementGuidePrefab);
            moveGuide.SetTransform();
            moveGuide.StartGuide();
        }
    }

    public void MicroInteractionMoleSpawn(Mole newMole)
    {
        if (!MicrointeractionsOn) { return; }

        if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.MolePulses_Operation)
        {
            MolePulses molePulses = Instantiate(molePulsesPrefab);
            molePulses.SetTransform(newMole.transform);
            molePulses.StartPulseEffect();
        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.MoleFill_Action)
        {
            Instantiate(moleFillPrefab).GetComponent<MoleFill>().SetTransform(newMole.transform);
        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.OutlineAnimation_Operation)
        {
            OutlineAnimation outlineAnim = Instantiate(outlineAnimationPrefab);
            outlineAnim.SetTransform(newMole.transform);
            outlineAnim.StartAnimEffect();
        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.OutlineLoading_Action)
        {
            Instantiate(outlineLoadingPrefab).GetComponent<OutlineLoading>().SetTransform(newMole.transform);
        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.GuidingWhistle_Operation)
        {

        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.CorrectingArrow_Operation)
        {
            CorrectingArrow correctionArrow = FindObjectOfType<CorrectingArrow>();
            if (correctionArrow == null)
            {
                correctionArrow = Instantiate(correctingArrowPrefab);
                correctionArrow.StartArrowEffect();
            }
            correctionArrow.UpdateActiveMole(newMole);
        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.ShootToNext_Action)
        {
            //Visual was spawned at the previous mole, find it and make it travel to the new mole
            ShootToNext shootToNextInstance = FindObjectOfType<ShootToNext>();
            if (shootToNextInstance != null)
            {
                shootToNextInstance.ShootToNewMole(newMole);
            }
        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.PointerFinder_Operation)
        {
            PointerFinder finder = Instantiate(pointerFinderPrefab);
            finder.SetTransform(newMole.transform);
            finder.StartFinder(newMole.transform);
        }
    }

    public void MicrointeractionMolePopComplete(Mole poppedMole, Material meshMaterial,
        Color enabledColor, Color colorFeedback, Color disabledColor, Color feedbackLowColor, float feedback)
    {
        if (!MicrointeractionsOn) { return; }

        if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.CheckmarkPop_Action)
        {
            CheckmarkPop checkmarkPop = Instantiate(checkmarkPopPrefab);
            checkmarkPop.SetTransform(poppedMole.transform);
            checkmarkPop.StartFeedback(enabledColor, colorFeedback, disabledColor, meshMaterial, 0.15f, 0.15f, feedback);

        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.MoleExplode_Action)
        {
            MoleExplode moleExplosion = Instantiate(moleExplodePrefab);
            moleExplosion.SetTransform(poppedMole.transform);
            moleExplosion.StartFeedback(enabledColor, colorFeedback, disabledColor, meshMaterial, 0.15f, 0.15f, feedback);
        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.MoleDepleted_Action)
        {
            meshMaterial.color = feedbackLowColor;
        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.ShootToNext_Action)
        {
            ShootToNext shootToNext = Instantiate(shootToNextPrefab);
            shootToNext.SetTransform(poppedMole.transform);
        }
    }

    OutlineLoading outlineLoader;
    MoleFill moleFiller;
    public void MicrointeractionMolePopProgress(Mole mole, float dwellTime, float dwellTimer)
    {
        if (!MicrointeractionsOn) { return; }

        if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.OutlineLoading_Action)
        {
            if (outlineLoader == null)
            {
                outlineLoader = mole.GetComponentInChildren<OutlineLoading>();
            }
            if (outlineLoader != null)
            {
                outlineLoader.ProgressFillEffect(dwellTime, dwellTimer);
            }
        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.MoleFill_Action)
        {
            if (moleFiller == null)
            {
                moleFiller = mole.GetComponentInChildren<MoleFill>();
            }
            if (moleFiller != null)
            {
                moleFiller.ProgressFillEffect(dwellTime, dwellTimer);
            }
        }
    }

    public void MicrointeractionEndContinuous(Mole disablingMole)
    {
        if (!MicrointeractionsOn) { return; }

        if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.MolePulses_Operation)
        {
            MolePulses pulses = disablingMole.GetComponentInChildren<MolePulses>();
            if (pulses != null)
            {
                Destroy(pulses.gameObject);
            }
        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.OutlineAnimation_Operation)
        {
            OutlineAnimation animation = disablingMole.GetComponentInChildren<OutlineAnimation>();
            if (animation != null)
            {
                Destroy(animation.gameObject);
            }
        }
        else if (Microinteractions.GetSelectedFeedback() == Microinteractions.FeedbackType.OutlineLoading_Action)
        {
            OutlineLoading loadRing = disablingMole.GetComponentInChildren<OutlineLoading>();
            if (loadRing != null)
            {
                Destroy(loadRing.gameObject);
            }
        }
        else
        {
            PointerFinder finder = disablingMole.GetComponentInChildren<PointerFinder>();
            if (finder != null)
            {
                Destroy(finder.gameObject);
            }
        }
    }
}
