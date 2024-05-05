using UnityEngine;

public class CameraController : MonoBehaviour
{
    InputManager inputManager;
    [SerializeField] float cameraSpeed = 200f;
    [SerializeField] float cameraSensitivity = 150f;
    float xRotation;
    [SerializeField] Transform cameraHolder;
    private void Start() {
        inputManager = InputManager.Instance;
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
        Vector3 moveVector = inputManager.cameraInputs.CameraMove();
        cameraHolder.position += moveVector.y * transform.forward * cameraSpeed * Time.deltaTime;
        cameraHolder.position += moveVector.x * transform.right * cameraSpeed * Time.deltaTime;
        if(inputManager.cameraInputs.CameraFloat()) {
            cameraHolder.position += Vector3.up * cameraSpeed * Time.deltaTime;
        }
    }

    void Look() { 
        Vector2 lookVector = inputManager.cameraInputs.CameraLook();
        xRotation -= lookVector.y * cameraSensitivity * Time.deltaTime;
        
        cameraHolder.Rotate(Vector3.up * lookVector.x * cameraSensitivity * Time.deltaTime );
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
