using Cinemachine;
using KBCore.Refs;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

//Freelook camera manager for handling mouse control and camera movement
public class CameraManager : ValidatedMonoBehaviour
{
    [Header("References")]
    [SerializeField, Anywhere] PlayerController player;
    [SerializeField, Anywhere] InputReader input;
    [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;

    [Header("Settings")]
    [SerializeField, Range(0.5f, 3f)] float speedMultiplier = 1f;

    bool isRMBPressed;
    bool cameraMovementLock;
    public bool isAimingWithBow;

    void OnEnable()
    {
        input.Look += OnLook;
        input.EnableMouseControlCamera += OnEnableMouseControlCamera;
        input.DisableMouseControlCamera += OnDisableMouseControlCamera;
    }

    void OnDisable()
    {
        input.Look -= OnLook;
        input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
        input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
    }

    void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
    {
        if (cameraMovementLock) return;

        if (isDeviceMouse && !isRMBPressed && !isAimingWithBow) return;

        float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

        freeLookVCam.m_XAxis.m_InputAxisValue = cameraMovement.x * speedMultiplier * deviceMultiplier;
        freeLookVCam.m_YAxis.m_InputAxisValue = cameraMovement.y * speedMultiplier * deviceMultiplier;

        player.AlignToCamera();
    }



    void OnEnableMouseControlCamera()
    {
        isRMBPressed = true;

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(DisableMouseForFrame());
    }

    void OnDisableMouseControlCamera()
    {
        isRMBPressed = false;

        // Unlock the cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Reset the camera axis to prevent jumping when re-enabling mouse control
        freeLookVCam.m_XAxis.m_InputAxisValue = 0f;
        freeLookVCam.m_YAxis.m_InputAxisValue = 0f;
    }

    IEnumerator DisableMouseForFrame()
    {
        cameraMovementLock = true;
        yield return new WaitForEndOfFrame();
        cameraMovementLock = false;
    }
}
