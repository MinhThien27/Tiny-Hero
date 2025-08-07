using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader", order = 0)]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<Vector2, bool> Look = delegate { };
    public event UnityAction EnableMouseControlCamera = delegate { };
    public event UnityAction DisableMouseControlCamera = delegate { };

    PlayerInputActions inputActions;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();
            inputActions.Player.SetCallbacks(this);
        }
    }

    public void EnablePlayerActions()
    {
        inputActions.Enable();
    }

    public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>();

    public void OnMove(InputAction.CallbackContext context)
    {
        Move.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
    }

    private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device is Mouse;

    public void OnFire(InputAction.CallbackContext context)
    {
        //nope
    }

    public void OnMouseControlCamera(InputAction.CallbackContext context)
    {
        switch(context.phase)
        {
            case InputActionPhase.Started:
                EnableMouseControlCamera.Invoke();
                break;
            case InputActionPhase.Canceled:
                DisableMouseControlCamera.Invoke();
                break;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}
