using Cinemachine;
using KBCore.Refs;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : ValidatedMonoBehaviour 
{
    [Header("References")]
    [SerializeField, Anywhere] InputReader input;
    [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;

    [Header("Settings")]
    [SerializeField, Range(0.5f, 3f)] float speedMultiplier = 1f;

    bool isRMBPressed;
    bool isCameraMovementLock;

    private void OnEnable()
    {
        input.Look += OnLook;
        input.EnableMouseControlCamera += OnEnableMouseControlCamera;
        input.DisableMouseControlCamera += OnDisableMouseControlCamera;
    }


    private void OnDisable()
    {
        input.Look -= OnLook;
        input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
        input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
    }



    private void OnEnableMouseControlCamera()
    {
        isRMBPressed = true;

        //Lock the cursor to the center of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(DisableMouseForFrame());
    }

    private IEnumerator DisableMouseForFrame()
    {
        isCameraMovementLock = true;
        yield return new WaitForEndOfFrame();
        isCameraMovementLock = false;
    }

    private void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
    {
        if (isCameraMovementLock) return;
        if(isDeviceMouse && !isRMBPressed) return;

        //If deviece is mouse use FixedDeltaTime, otherwise use DeltaTime
        float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

        //Set the camera axis values
        freeLookVCam.m_XAxis.m_InputAxisValue = cameraMovement.x * speedMultiplier * deviceMultiplier;
        freeLookVCam.m_YAxis.m_InputAxisValue = cameraMovement.y * speedMultiplier * deviceMultiplier;
    }


    private void OnDisableMouseControlCamera()
    {
        isRMBPressed = false;

        //Unlock the cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Reset camera axis values to prevent jumping when re-enabling mouse control
        freeLookVCam.m_XAxis.m_InputAxisValue = 0f;
        freeLookVCam.m_YAxis.m_InputAxisValue = 0f;
    }
}
