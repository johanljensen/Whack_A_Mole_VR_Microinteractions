﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class PointerTrailHandler : MonoBehaviour
{

    [SerializeField]
    private Pointer controllerParent;

    [SerializeField]
    bool showPerformance;
    [SerializeField]
    bool playSound;

    [SerializeField]
    private PerformanceManager performanceManager;
    [SerializeField]
    private Color trailColor = new Color(0.118f, 0.486f, 0.718f, 1f);  // this is now the field for trail color

    [SerializeField]
    private SoundManager soundManager;

    [SerializeField]
    private SoundManager.Sound trailSound;
    private float currentPitch = 0.5f;  // Initial pitch
    private float currentVolume = -1f;  // Initial volume


    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;

    private float speed;
    private float perf;
    private Text txt; 
    [SerializeField, Range(0.1f, 2f)]
    private float maxTrailLength = 1.0f; // max trail length
    [SerializeField, Range(0.1f, 2f)]
    private float minTrailLength = 0.0f;

    [SerializeField, Range(0.1f, 2f)]
    private float minTargetPitch = 0.7f;

    [SerializeField, Range(1f, 4f)]
    private float maxTargetPitch = 1.3f;


    private bool isVisible = true;
    private bool configVisibility = true;
    private bool lastRuntimeVisibilityState;
    private bool performanceText = false;
    
    [SerializeField]
    private GameObject perfTextPrefab;

    private void OnEnable()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (!trailRenderer)
        {
            trailRenderer = GetComponent<TrailRenderer>();
            if (trailRenderer == null)
                Debug.LogError("Missing TrailRenderer component!");
        }

        if (!spriteRenderer)
        {
            spriteRenderer = GetComponentInParent<SpriteRenderer>();
            if (spriteRenderer == null)
                Debug.LogError("Missing SpriteRenderer component in parent!");
        }

        trailRenderer.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0f);
    }

    private void Awake()
    {
        if (isVisible && playSound)
        {
            soundManager.PlaySoundLooped(gameObject, trailSound);
            soundManager.ChangeVolume(trailSound, 0f);  // Set initial volume to 0
        }

        //Set default values in case we don't want the trail to vary with the performance algorithm
        speed = 0.5f;
        perf = 0.5f;
    }

    private void Update()
    {
        if (!trailRenderer || !spriteRenderer)
            return;

        if (showPerformance)
        {
            ControllerName controllerName = controllerParent.GetControllerName();
            speed = performanceManager.GetInstantJudgement(controllerName);
            perf = performanceManager.GetInstantPerformance(controllerName);
        }

        UpdateRuntimeVisibility();
        // If the final visibility is true, then update the trail properties
        if (IsTrulyVisible())
        {
            UpdateTrailProperties();
        }
        else
        {
            soundManager.ChangeVolume(trailSound, 0f);
        }
    }
    private void OnDestroy()
    {
        // Stop playing the sound when the script is destroyed
        soundManager.StopSound(trailSound);
    }


    private void UpdateTrailProperties()
    {
        if (trailRenderer != null)
        {
            float normalizedSpeed = speed;
            if (performanceText) {
                if (txt == null) {
                    txt = Instantiate(perfTextPrefab, spriteRenderer.transform).GetComponentInChildren<Text>();
                }
                txt.text = perf.ToString("0.00");
            }

            if (playSound)
            {
                float targetVolume = Mathf.Lerp(0f, 1f, normalizedSpeed);
                if (currentVolume == -1f)
                {
                    currentVolume = targetVolume;
                }
                else
                {
                    currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * 2f);
                }

                float targetPitch = Mathf.Lerp(minTargetPitch, maxTargetPitch, normalizedSpeed);
                currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * 2f);

                soundManager.ChangePitch(trailSound, currentPitch);
                soundManager.ChangeVolume(trailSound, currentVolume);
                //soundManager.ChangeVolume(trailSound, (speed > 0.0f) ? Mathf.Lerp(0f, 1f, speed) : 0f);
            }


            float targetDiameter = spriteRenderer.bounds.size.x;
            float currentLength = Mathf.Min(Mathf.Lerp(minTrailLength, maxTrailLength, normalizedSpeed), maxTrailLength);
            float currentWidth = Mathf.Lerp(0, targetDiameter * 0.8f, normalizedSpeed);
            Color currentColor = new Color(trailColor.r, trailColor.g, trailColor.b, Mathf.Lerp(0.5f, 1f, normalizedSpeed));

            trailRenderer.time = currentLength;
            trailRenderer.startWidth = currentWidth;
            trailRenderer.endWidth = 0f;
            trailRenderer.startColor = currentColor;
            trailRenderer.endColor = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
        }
    }

    private float GetNonLinearSpeed()
    {
        return Mathf.Pow(speed, 0.3f);
    }

    private void UpdateRuntimeVisibility()
    {
        bool currentRuntimeVisibility;

        //// If the speed is less than 0.05f, set the runtime visibility to false
        //if (speed < 0.05f)
        //{
        //    currentRuntimeVisibility = false;
        //    soundManager.ChangeVolume(trailSound, 0f);
        //}
        // //If the speed is greater than or equal to 0.05f, set the runtime visibility to true
        //else
        currentRuntimeVisibility = true;
        //{
        //}

        // Only change the runtime visibility if it is different from the previous state
        if (currentRuntimeVisibility != lastRuntimeVisibilityState)
        {
            lastRuntimeVisibilityState = currentRuntimeVisibility;
            SetRuntimeVisibility(currentRuntimeVisibility);
        }
    }




    private bool IsTrulyVisible()
    {
        // final visibility is the combination of config visibility and runtime visibility
        return configVisibility && isVisible;
    }

    // Used to set the visibility of the trail from the modifiers manager
    internal void SetConfigVisibility(bool visibility, bool withText)
    {
        configVisibility = visibility;
        performanceText = withText;
        UpdateVisibility();
    }

    // Used to set the visibility of the trail at runtime (e.g. when the speed is too low)
    internal void SetRuntimeVisibility(bool visibility)
    {
        isVisible = visibility;
        UpdateVisibility();
    }

    // Used to update the visibility of the trail based on the final visibility state
    private void UpdateVisibility()
    {
        if (!trailRenderer)
            return;

        bool finalVisibility = IsTrulyVisible();
        trailRenderer.enabled = finalVisibility;
    }
}