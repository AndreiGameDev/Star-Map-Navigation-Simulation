using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    CameraController cameraController;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] TextMeshProUGUI textUI;
    private void Awake() {
        cameraController = FindAnyObjectByType<CameraController>();
        if(PlayerPrefs.HasKey("floatSensitivity")) {
            sensitivitySlider.value = PlayerPrefs.GetFloat("floatSensitivity");
            cameraController.cameraSensitivity = PlayerPrefs.GetFloat("floatSensitivity");
            textUI.text = PlayerPrefs.GetFloat("floatSensitivity").ToString();
        }
    }

    public void ChangeSensitivity(float value) {
        PlayerPrefs.SetFloat("floatSensitivity", value);
        cameraController.cameraSensitivity = value;
        textUI.text = value.ToString();
    }
}
