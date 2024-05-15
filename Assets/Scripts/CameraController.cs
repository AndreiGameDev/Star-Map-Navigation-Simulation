using UnityEngine;
using UnityEngine.Windows;

public class CameraController : MonoBehaviour {
    InputManager inputManager;
    [SerializeField] float cameraSpeed = 100f;
    public float cameraSensitivity = 150f;
    float xRotation;

    public GameObject cinemachineTargetGO;
    Transform cameraTransform;
    private void Start() {
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
    }
    private void Update() {
        Move();
    }
    private void LateUpdate() {
        Look();

    }
    private void OnEnable() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnDisable() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    void Move() {
        Vector3 moveVector = inputManager.cameraMove;
        transform.position += moveVector.y * cameraTransform.forward * cameraSpeed * Time.deltaTime;
        transform.position += moveVector.x * transform.right * cameraSpeed * Time.deltaTime;
        if(inputManager.cameraFloat) {
            transform.position += Vector3.up * cameraSpeed * Time.deltaTime;
        }
    }

    void Look() {
        Vector2 lookVector = inputManager.cameraLook;
        xRotation -= lookVector.y * cameraSensitivity * Time.deltaTime;
        xRotation = ClampAngle(xRotation, -90f, 90f);
        transform.Rotate(Vector3.up * lookVector.x * cameraSensitivity * Time.deltaTime);
        cinemachineTargetGO.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
        if(lfAngle < -360f) lfAngle += 360f;
        if(lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
