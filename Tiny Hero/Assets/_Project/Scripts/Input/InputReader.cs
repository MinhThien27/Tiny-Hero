using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static TinyHero;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader", order = 0)]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<Vector2, bool> Look = delegate { };
    public event UnityAction EnableMouseControlCamera = delegate { };
    public event UnityAction DisableMouseControlCamera = delegate { };
    public event UnityAction<bool> Jump = delegate { };
    public event UnityAction<bool> Dash = delegate { };
    public event UnityAction Attack = delegate { };

    TinyHero inputActions;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new TinyHero();
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
        if (context.phase == InputActionPhase.Started)
        {
            Attack.Invoke();
            Debug.Log("Has invoke Attack");
        }
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
        switch(context.phase)
        {
            case InputActionPhase.Started:
                Jump.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                Jump.Invoke(false);
                break;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Dash.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                Dash.Invoke(false);
                break;
        }
    }
}
