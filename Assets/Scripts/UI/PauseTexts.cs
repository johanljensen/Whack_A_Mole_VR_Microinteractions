using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PauseTexts : MonoBehaviour
{
    [SerializeField]
    public PauseTextsSO pauseTexts;

    [SerializeField]
    private Text instructionText;

    [InspectorButton("OnButtonClickedWait")]
    public bool SetTextWait;

    [InspectorButton("OnButtonClickedDescribe")]
    public bool setTextDescribe;

    [InspectorButton("OnButtonClickedAffect")]
    public bool setTextAffect;

    [InspectorButton("OnButtonClickedDifficulty")]
    public bool setTextDifficulty;


    private void OnEnable()
    {
        instructionText = GetComponent<Text>();
        SetText(pauseTexts.PleaseWaitText);
    }

    public void SetText(string newText)
    {
        if (instructionText != null)
        {
            instructionText.text = newText;
            SceneView.RepaintAll();
        }
    }

    private void OnButtonClickedWait()
    {
        SetText(pauseTexts.PleaseWaitText);
        Debug.Log(pauseTexts.PleaseWaitText);
    }
    private void OnButtonClickedDescribe()
    {
        SetText(pauseTexts.DescribeEffectPlease);
        Debug.Log(pauseTexts.DescribeEffectPlease);
    }
    private void OnButtonClickedAffect()
    {
        SetText(pauseTexts.AffectYourGameplay);
        Debug.Log(pauseTexts.AffectYourGameplay);
    }
    private void OnButtonClickedDifficulty()
    {
        SetText(pauseTexts.ChangedDifficulty);
        Debug.Log(pauseTexts.ChangedDifficulty);
    }
}
