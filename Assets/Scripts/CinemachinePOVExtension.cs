using Cinemachine;
using UnityEngine;

public class CinemachinePOVExtension : CinemachineExtension {
    [SerializeField] float cameraSensitivity;
    [SerializeField] float clampAngle = 180f;

    InputManager inputManager;
    Vector3 startingRotation;
    private void Start() {
        inputManager = InputManager.Instance;
    }
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime) {
        if(vcam.Follow && stage == CinemachineCore.Stage.Aim) {
            if(startingRotation == null) {
                startingRotation = transform.localRotation.eulerAngles;
            }
            Vector2 input = inputManager.cameraLook;
            startingRotation.x += input.x * cameraSensitivity * Time.deltaTime;
            startingRotation.y += input.y * cameraSensitivity * Time.deltaTime;
            startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);
            state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);
        }
    }
}
