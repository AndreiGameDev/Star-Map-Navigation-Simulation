using UnityEngine;

public class InputManager : MonoBehaviour {
    public PlayerInputMap playerInputMap;
    private static InputManager instance;
    public static InputManager Instance {
        get { return instance; }
    }

    public CameraInputs cameraInputs;
    public UIInputs uIInputs;
    public InputMode currentInputeMode;
    private void Awake() {
        if(instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }
        playerInputMap = new PlayerInputMap();
        playerInputMap.Enable();
        cameraInputs = GetComponent<CameraInputs>();
        uIInputs = GetComponent<UIInputs>();   
        SetInputMode(InputMode.UI);
    }
    public void SetInputMode(InputMode input) {
        switch(input) {
            case InputMode.UI:
                currentInputeMode = InputMode.UI;
                cameraInputs.enabled = false;
                uIInputs.enabled = true;
                break;
            case InputMode.FreeCamera:
                currentInputeMode = InputMode.FreeCamera;
                uIInputs.enabled = false;
                cameraInputs.enabled = true;
                break;
        }
    }
}

public enum InputMode {
    UI,
    FreeCamera
}