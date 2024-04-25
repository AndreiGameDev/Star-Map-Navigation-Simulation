using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwapCameraScript : MonoBehaviour {
    InputManager inputManager;
    [SerializeField] GameObject angledCamera;
    [SerializeField] GameObject freeCamera;
    [SerializeField] Image progressBar;
    private Coroutine holdKeyCoroutine;
    private bool isCoroutineActive;

    private void Start() {
        inputManager = InputManager.Instance;
        isCoroutineActive = false;
    }

    private void Update() {
        if(!isCoroutineActive && IsHoldingSwapCameraKey()) {
            holdKeyCoroutine = StartCoroutine(HoldKey());
        }
    }

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

    void SwapCameraMode() {
        if(inputManager.currentInputeMode == InputMode.UI) {
            inputManager.SetInputMode(InputMode.FreeCamera);
            freeCamera.SetActive(true);
            angledCamera.SetActive(false);
        } else if(inputManager.currentInputeMode == InputMode.FreeCamera) {
            inputManager.SetInputMode(InputMode.UI);
            angledCamera.SetActive(true);
            freeCamera.SetActive(false);
        }
    }

    IEnumerator HoldKey() {
        isCoroutineActive = true;  // Set flag when coroutine starts
        float timer = 0f;
        float targetTime = 1f;
        while(IsHoldingSwapCameraKey()) {
            timer += Time.deltaTime;
            progressBar.fillAmount = Mathf.Clamp01(timer / targetTime);

            if(timer > targetTime) {
                SwapCameraMode();
                progressBar.fillAmount = 0f;
                break;  // Break the loop after task completion
            }

            yield return null;
        }
        progressBar.fillAmount = 0f;
        isCoroutineActive = false;  // Reset flag when coroutine ends
    }
}
