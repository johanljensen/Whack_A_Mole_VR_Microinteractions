﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

/*
Basic implementation of the pointer abstract class. Simply changes the color of the laser and 
the Cursor on shoot.
*/

public class BasicPointer : Pointer
{
    [SerializeField]
    private Color shootColor;

    [SerializeField]
    private Color badShootColor;

    [SerializeField]
    private float laserExtraWidthShootAnimation = .05f;

    private float shootTimeLeft;
    private float totalShootTime;
    private float pulseClock = 0f;
    private float dwellTime = 3f;
    private float dwellTimer = 0f;
    private float dwellduration = 0f;
    private delegate void Del();
    private string hover = "";

    internal Vector3 MappedPosition { get; private set; }

    /*
    public Vector3 CalculateDirection()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction.y += 0.10f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction.x -= 0.10f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction.y -= 0.10f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction.x += 0.10f;
        }
        return direction.normalized;
    }
    */

    MicrointeractionManager microManager;
    private void Awake()
    {
        microManager = MicrointeractionManager.GetInstance();
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!active) return;
            if (active)
            {
                float v = Input.GetAxisRaw("Vertical");
                float h = Input.GetAxisRaw("Horizontal");
                float mouseSpeed = 1;
                float keyboardSpeed = 0.4f;

                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    float h1 = mouseSpeed * Input.GetAxis("Mouse X");
                    float v1 = mouseSpeed * Input.GetAxis("Mouse Y");

                    this.transform.Translate(h1, v1, 0);
                }
                else
                {
                    Vector3 direction = new Vector3(h, v, 0f).normalized * keyboardSpeed;
                    this.transform.Translate(direction * 1 * Time.deltaTime);
                }
                PointerControl();
            }
        }
        
    }

    public override void ShowTaskFeedback(float duration, List<(int id, float val)> molePerf, float animationDelay)
    {
        StartCoroutine(WaitShowTaskFeedback(duration, molePerf, animationDelay));
    }
    
    private IEnumerator WaitShowTaskFeedback(float duration, List<(int id, float val)> molePerf, float animationDelay) {
        float timeSpent = 0f;

        foreach (var fb in molePerf) {
            if (fb.id != -1) {
                Pulse(duration:0.1f, frequency:150, amplitude:fb.val * 50);
                timeSpent += animationDelay;
                yield return new WaitForSeconds(animationDelay);
            }
        }
    }

    public override void PositionUpdated()
    {
        if (!active) return;
        PointerControl();
    }

    // Function called on VR update, since it can be faster/not synchronous to Update() function. Makes the Pointer slightly more reactive.
    private void PointerControl()
    {
        if (!active) return;

        Vector2 pos = new Vector2(laserOrigin.transform.position.x, laserOrigin.transform.position.y);
        MappedPosition = laserMapper.ConvertMotorSpaceToWallSpace(pos);
        Vector3 origin = laserOrigin.transform.position;
        Vector3 rayDirection = (MappedPosition - origin).normalized;

        onPointerMove.Invoke(new MoveData
        {
            controllerPos = pos,
            cursorPos = MappedPosition,
            name = controllerName
        });

        RaycastHit hit;
        if (Physics.Raycast(laserOrigin.transform.position + laserOffset, rayDirection, out hit, 100f, Physics.DefaultRaycastLayers))
        {
            //UpdateLaser(true, hitPosition: laserOrigin.transform.InverseTransformPoint(hit.point), rayDirection: laserOrigin.transform.InverseTransformDirection(rayDirection));
            Vector3 hitPosition = laserOrigin.transform.InverseTransformPoint(hit.point);
            laser.SetPosition(1, hitPosition);
            cursor.SetPosition(hitPosition);
            hoverMole(hit);
        }
        else
        {
            Vector3 rayPosition = laserOrigin.transform.InverseTransformDirection(rayDirection) * maxLaserLength;
            laser.SetPosition(1, rayPosition);
            cursor.SetPosition(rayPosition);
            //UpdateLaser(false, rayDirection: laserOrigin.transform.InverseTransformDirection(rayDirection) * maxLaserLength);
        }
        //if (SteamVR.active)
        //{
            if (hit.collider)
            {
                Mole mole;
                if (hit.collider.gameObject.TryGetComponent<Mole>(out mole))
                {
                    Mole.States moleAnswer = mole.GetState();
                    if (moleAnswer == Mole.States.Enabled)
                    {
                        if (hover == string.Empty)
                        {
                            hover = mole.GetId().ToString();
                            loggerNotifier.NotifyLogger("Pointer Hover Begin", EventLogger.EventType.PointerEvent, new Dictionary<string, object>()
                            {
                                {"ControllerHover", hover},
                                {"ControllerName", gameObject.name}
                            });
                        }
                        dwellduration += Time.deltaTime;
                        dwellTimer = dwellTimer + 0.1f;

                        if (microManager != null)
                        {
                            microManager.MicrointeractionMolePopProgress(mole, dwellTime, dwellTimer);
                        }

                        //Debug.Log("SHOOTING MOLE:" + dwellTimer + " : " + dwellTime);
                        if (dwellTimer > dwellTime)
                        {
                            pointerShootOrder++;
                            loggerNotifier.NotifyLogger(overrideEventParameters: new Dictionary<string, object>(){
                                {"ControllerSmoothed", directionSmoothed},
                                {"ControllerAimAssistState", System.Enum.GetName(typeof(Pointer.AimAssistStates), aimAssistState)},
                                {"LastShotControllerRawPointingDirectionX", transform.forward.x},
                                {"LastShotControllerRawPointingDirectionY", transform.forward.y},
                                {"LastShotControllerRawPointingDirectionZ", transform.forward.z},
                                {"LastShotBubbleRawPointingDirectionX", laserOrigin.transform.forward.x},
                                {"LastShotBubbleRawPointingDirectionY", laserOrigin.transform.forward.y},
                                {"LastShotBubbleRawPointingDirectionZ", laserOrigin.transform.forward.z},
                                {"LastShotBubbleFilteredPointingDirectionX", rayDirection.x},
                                {"LastShotBubbleFilteredPointingDirectionY", rayDirection.y},
                                {"LastShotBubbleFilteredPointingDirectionZ", rayDirection.z},
                                {"ControllerHover", "NULL"},
                                {"PointerShootOrder", "NULL"},
                                {"ControllerName", "NULL"},
                            });

                            loggerNotifier.NotifyLogger("Pointer Shoot", EventLogger.EventType.PointerEvent, new Dictionary<string, object>()
                            {
                                {"PointerShootOrder", pointerShootOrder},
                                {"ControllerName", gameObject.name}
                            });
                            Shoot(hit, dwellduration);
                        }
                    }
                    else
                    {
                        OperationFeedback();
                        CheckHoverEnd();
                        if (dwellTimer > 0f)
                        {
                            dwellTimer = dwellTimer - 0.1f;
                            dwellduration = 0f;
                        }
                    }
                }
                else
                {
                    CheckHoverEnd();
                    if (dwellTimer > 0f)
                    {
                        dwellTimer = dwellTimer - 0.1f;
                        dwellduration = 0f;
                    }
                }
            }
            else
            {
                CheckHoverEnd();
                if (dwellTimer > 0f)
                {
                    dwellTimer = dwellTimer - 0.1f;
                    dwellduration = 0f;
                }
            }
        //}
    }


    private void CheckHoverEnd()
    {
        if (hover != string.Empty)
        {
            loggerNotifier.NotifyLogger("Pointer Hover End", EventLogger.EventType.PointerEvent, new Dictionary<string, object>()
            {
                {"ControllerHover", hover},
                {"ControllerName", gameObject.name}
            });
            hover = "";

        }
    }

    private void OperationFeedback() {
        if (!Microinteractions.ShouldVibrateController()) return;

        if (pulseClock > 0.06) {
            float val = performanceManager.GetInstantJudgement(controllerName);
            Pulse(duration:0.02f, frequency:50, amplitude:val * 35);
            pulseClock = 0f;
        } else {
            pulseClock += Time.deltaTime;
        }
    }

    // Implementation of the behavior of the Pointer on shoot. 
    protected override void PlayShoot(bool correctHit)
    {
        Color newColor;

        if (correctHit) newColor = shootColor;
        else newColor = badShootColor;

        if (!Microinteractions.ShouldGiveNegativeFeedback())
        {
            // don't show badShootColor if performance feedback is disabled.
            newColor = shootColor;
        }

        StartCoroutine(PlayShootAnimation(.5f, newColor));
    }

    // Ease function, Quart ratio.
    private float EaseQuartOut(float k)
    {
        return 1f - ((k -= 1f) * k * k * k);
    }

    // IEnumerator playing the shooting animation.
    private IEnumerator PlayShootAnimation(float duration, Color transitionColor)
    {
        shootTimeLeft = duration;
        totalShootTime = duration;

        // Generation of a color gradient from the shooting color to the default color (idle).
        Gradient colorGradient = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[2] { new GradientColorKey(laser.startColor, 0f), new GradientColorKey(transitionColor, 1f) };
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2] { new GradientAlphaKey(laser.startColor.a, 0f), new GradientAlphaKey(transitionColor.a, 1f) };
        colorGradient.SetKeys(colorKey, alphaKey);

        // Playing of the animation. The laser and Cursor color and scale are interpolated following the easing curve from the shooting values (increased size, red/green color)
        // to the idle values
        while (shootTimeLeft > 0f)
        {
            float shootRatio = (totalShootTime - shootTimeLeft) / totalShootTime;
            float newLaserWidth = 0f;
            Color newLaserColor = new Color();

            newLaserWidth = laserWidth + ((1 - EaseQuartOut(shootRatio)) * laserExtraWidthShootAnimation);
            newLaserColor = colorGradient.Evaluate(1 - EaseQuartOut(shootRatio));

            laser.startWidth = newLaserWidth;
            laser.endWidth = newLaserWidth;

            laser.startColor = newLaserColor;
            laser.endColor = newLaserColor;
            cursor.SetColor(newLaserColor);
            cursor.SetScaleRatio(newLaserWidth / laserWidth);

            shootTimeLeft -= Time.deltaTime;

            yield return null;
        }

        // When the animation is finished, resets the laser and Cursor to their default values. 
        laser.startWidth = laserWidth;
        laser.endWidth = laserWidth;
        laser.startColor = startLaserColor;
        laser.endColor = EndLaserColor;
        cursor.SetColor(EndLaserColor);
        cursor.SetScaleRatio(1f);
    }
}
