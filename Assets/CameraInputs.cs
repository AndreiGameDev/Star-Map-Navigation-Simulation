using UnityEngine;

public class CameraInputs : MonoBehaviour
{
    InputManager inputManager;
    PlayerInputMap playerInputMap;
    private void Start() {
        inputManager = InputManager.Instance;
        playerInputMap = inputManager.playerInputMap;
    }
    public Vector2 CameraMove() {
        return playerInputMap.Player.Move.ReadValue<Vector2>().normalized;
    }
    public bool CameraFloat() {
        return playerInputMap.Player.Float.IsPressed();
    }

    public Vector2 CameraLook() {
        return playerInputMap.Player.Look.ReadValue<Vector2>().normalized;
    }
    public bool SwapCamera() {
        return playerInputMap.Player.SwapCamera.IsPressed();
    }
}
