using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script controlls the sensitivty of the camera and adjusts the sensitivty through UI
/// </summary>
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
        gameObject.SetActive(false);
    }

    public void ChangeSensitivity(float value) {
        PlayerPrefs.SetFloat("floatSensitivity", value);
        cameraController.cameraSensitivity = value;
        textUI.text = value.ToString();
    }
}
