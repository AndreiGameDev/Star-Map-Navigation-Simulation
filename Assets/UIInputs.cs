using UnityEngine;

public class UIInputs : MonoBehaviour
{
    InputManager inputManager;
    PlayerInputMap playerInputMap;
    private void Start() {
        inputManager = InputManager.Instance;
        playerInputMap = inputManager.playerInputMap;
    }

    public bool SwapCamera() {
        return playerInputMap.UI.SwapCamera.IsPressed();
    }
}
