using UnityEngine;

public class InputManager : MonoBehaviour {
    PlayerInputMap playerInputMap;
    private static InputManager instance;
    public static InputManager Instance {
        get { return instance; }
    }
    private void Awake() {
        if(instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }
        playerInputMap = new PlayerInputMap();
        playerInputMap.Enable();
    }

    public Vector2 MousePosition() {
        return playerInputMap.UI.Point.ReadValue<Vector2>().normalized;
    }

    public bool HasClicked() {
        return playerInputMap.UI.Click.triggered;
    }
}
