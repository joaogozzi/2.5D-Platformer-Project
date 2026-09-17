using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Variables

    //Singleton
    private static InputManager _instance;
    public static InputManager Instance => _instance;

    //Action Map
    private InputActionsMap _inputActions;

    //Public events
    public event System.Action<Vector2> OnPlayerMove;
    public event System.Action<Vector2> OnPlayerLook;
    public event System.Action OnJump;
    public event System.Action OnCrouch;
    public event System.Action OnInteract;
    public event System.Action OnAttack;
    public event System.Action OnDash;
    public event System.Action OnPause;

    //public event System.Action<int> OnControlUpdate;

    #endregion

    #region Default Methods

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _inputActions = new InputActionsMap();
        SetupInputCallbacks();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        InputSystem.onDeviceChange += HandleDeviceChange;
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        InputSystem.onDeviceChange -= HandleDeviceChange;
    }

    #endregion

    #region Setup Methods

    private void SetupInputCallbacks()
    {
        //Movement
        _inputActions.Player.Move.performed += ctx => OnPlayerMove?.Invoke(ctx.ReadValue<Vector2>());
        _inputActions.Player.Move.canceled += ctx => OnPlayerMove?.Invoke(Vector2.zero);

        //Look (para mouse/controle direito)
        _inputActions.Player.Look.performed += ctx => OnPlayerLook?.Invoke(ctx.ReadValue<Vector2>());

        //Jump
        _inputActions.Player.Jump.performed += ctx => OnJump?.Invoke();

        //Crouch
        _inputActions.Player.Crouch.performed += ctx => OnCrouch?.Invoke();

        //Interact
        _inputActions.Player.Interact.performed += ctx => OnInteract?.Invoke();

        //Attack
        _inputActions.Player.Attack.performed += ctx => OnAttack?.Invoke();

        //Dash
        _inputActions.Player.Dash.performed += ctx => OnDash?.Invoke();

        //Pause
        _inputActions.Player.Pause.performed += ctx => OnPause?.Invoke();
    }

    public InputActionsMap GetInputActions() => _inputActions;

    #endregion

    #region Devices Controller

    private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed || change == InputDeviceChange.Reconnected)
            UpdateCurrentControlScheme();
    }

    private void UpdateCurrentControlScheme()
    {
        // Verifica se há gamepads conectados
        var gamepads = Gamepad.all;

        if (gamepads.Count > 0)
            SwitchToGamepad();
        else
            SwitchToKeyboardMouse();
    }

    public void SwitchToKeyboardMouse()
    {
        _inputActions.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
    }

    public void SwitchToGamepad()
    {
        _inputActions.bindingMask = InputBinding.MaskByGroup("Gamepad");
    }

    #endregion
}