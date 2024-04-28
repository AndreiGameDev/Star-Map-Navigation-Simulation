using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwapCameraScript : MonoBehaviour {
    InputManager inputManager;
    [SerializeField] GameObject cameraPlayer;
    [SerializeField] Image progressBar;
    private bool isCoroutineActive; // Need this to make sure coroutine has accurate checks if player is pressing input or not
    PhysicsRaycaster physicsRaycaster;
    CameraController cameraController;
    private void Awake() {
        cameraController = cameraPlayer.GetComponent<CameraController>();
        physicsRaycaster = cameraController.GetComponent<PhysicsRaycaster>();
    }
    private void Start() {
        inputManager = InputManager.Instance;
        isCoroutineActive = false;
    }

    private void Update() {
        if(!isCoroutineActive && IsHoldingSwapCameraKey()) {
            StartCoroutine(HoldKey());
        }
    }

    // Checks for key press in the appropriate UI
    bool IsHoldingSwapCameraKey() {
        switch(inputManager.currentInputeMode) {
            case InputMode.UI:
                return inputManager.uIInputs.SwapCamera();
            case InputMode.FreeCamera:
                return inputManager.cameraInputs.SwapCamera();
            default:
                return false;
        }
    }

    // Swaps camera and inputs to the other mode
    void SwapCameraMode() {
        if(inputManager.currentInputeMode == InputMode.UI) {
            inputManager.SetInputMode(InputMode.FreeCamera);
            physicsRaycaster.enabled = false;
            cameraController.enabled = true;
        } else if(inputManager.currentInputeMode == InputMode.FreeCamera) {
            inputManager.SetInputMode(InputMode.UI);
            physicsRaycaster.enabled = true;
            cameraController.enabled = false;
        }
    }

    // Timer animation and making sure it updates accordingly to the hold time and it resets when not pressed
    IEnumerator HoldKey() {
        isCoroutineActive = true;
        float timer = 0f;
        float targetTime = 1f;
        while(IsHoldingSwapCameraKey()) {
            timer += Time.deltaTime;
            progressBar.fillAmount = Mathf.Clamp01(timer / targetTime);

            if(timer > targetTime) {
                SwapCameraMode();
                progressBar.fillAmount = 0f;
                break;
            }

            yield return null;
        }
        progressBar.fillAmount = 0f;
        isCoroutineActive = false;
    }
}
