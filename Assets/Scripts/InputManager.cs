using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Input manager used while in the star map
/// </summary>
public class InputManager : MonoBehaviour {     
    private static InputManager instance;
    public static InputManager Instance {
        get { return instance; }
    }
    [HideInInspector]public InputActionMap playerMap;
    [HideInInspector]public InputActionMap uiMap;
    public PlayerInput playerInput;
    public Vector2 cameraMove;
    public Vector2 cameraLook;
    public bool cameraFloat;
    public bool cameraSwap;
    private void Awake() {
        instance = this;
        playerInput = GetComponent<PlayerInput>();
        playerMap = playerInput.actions.FindActionMap("Player");
        uiMap = playerInput.actions.FindActionMap("UI");
    }
    void OnSwapCamera(InputValue value) {
        SwapCamera(value.isPressed);
    }
    void OnMove(InputValue value) {
        CameraMove(value.Get<Vector2>());
    }
    void OnLook(InputValue value) {
        CameraLook(value.Get<Vector2>());
    }
    void OnFloat(InputValue value) {
        CameraFloat(value.isPressed);
    }
    public void CameraMove(Vector2 value) {
        cameraMove = value;
    }
    public void CameraFloat(bool value) {
        cameraFloat = value;
    }
    public void CameraLook(Vector2 value) {
        cameraLook = value;
    }
    public void SwapCamera( bool value) {
        cameraSwap = value;
    }
}
