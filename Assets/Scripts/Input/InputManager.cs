using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public static PlayerInput PlayerInput;

    public Vector2 moveInput { get; private set; }
    public bool jumpJustPressed { get; private set; }
    public bool jumpHeld { get; private set; }
    public bool jumpReleased { get; private set; }
    public bool dashInput { get; private set; }
    public bool menuInput { get; private set; }

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction menuAction;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        PlayerInput = GetComponent<PlayerInput>();

        SetupInputActions();
    }

    private void Update()
    {
        UpdateInput();
    }

    private void SetupInputActions()
    {
        moveAction = PlayerInput.actions["Move"];
        jumpAction = PlayerInput.actions["Jump"];
        dashAction = PlayerInput.actions["Dash"];
        menuAction = PlayerInput.actions["Menu"];
    }

    private void UpdateInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        jumpJustPressed = jumpAction.WasPressedThisFrame();
        jumpHeld = jumpAction.IsPressed();
        jumpReleased = jumpAction.WasReleasedThisFrame();
        dashInput = dashAction.WasPressedThisFrame();
        menuInput = menuAction.WasPressedThisFrame();
    }
}
