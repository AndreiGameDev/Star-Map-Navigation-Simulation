using UnityEngine;
public class ApplicationSettings : MonoBehaviour {
    public static ApplicationSettings instance;
    private void Awake() {
        if(instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
