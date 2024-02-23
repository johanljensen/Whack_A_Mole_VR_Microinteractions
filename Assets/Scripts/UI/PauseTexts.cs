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

    [InspectorButton("OnButtonClickedAlgo")]
    public bool setTextAlgorithm;

    [InspectorButton("OnButtonClickedExplain")]
    public bool setTextExplain;

    [InspectorButton("OnButtonClickedHowMuch")]
    public bool setTextHowMuchFeedback;

    [InspectorButton("OnButtonClickedHowFeel")]
    public bool setTextHowGoodFeedback;


    private void OnEnable()
    {
        instructionText = GetComponent<Text>();
        //SetText(pauseTexts.PleaseWaitText);
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
    private void OnButtonClickedAlgo()
    {
        SetText(pauseTexts.AlgorithmRateText);
        Debug.Log(pauseTexts.AlgorithmRateText);
    }
    private void OnButtonClickedExplain()
    {
        SetText(pauseTexts.PleaseExplainText);
        Debug.Log(pauseTexts.PleaseExplainText);
    }
    private void OnButtonClickedHowMuch()
    {
        SetText(pauseTexts.HowMuchFeedbackText);
        Debug.Log(pauseTexts.HowMuchFeedbackText);
    }
    private void OnButtonClickedHowFeel()
    {
        SetText(pauseTexts.HowFeelFeedbackText);
        Debug.Log(pauseTexts.HowFeelFeedbackText);
    }
}
