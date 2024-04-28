using UnityEngine;

public class CameraController : MonoBehaviour
{
    InputManager inputManager;
    [SerializeField] float cameraSpeed = 2f;
    [SerializeField] float cameraSensitivity = 150f;
    float xRotation;
    [SerializeField] Transform cameraHolder;
    private void Start() {
        inputManager = InputManager.Instance;
    }
    private void Update() {
        Look();
        Move();
    }

    void Move() {
        Vector3 moveVector = inputManager.cameraInputs.CameraMove();
        cameraHolder.position += (moveVector.y * transform.forward) * cameraSpeed;
        cameraHolder.position += (moveVector.x * transform.right) * cameraSpeed;
        if(inputManager.cameraInputs.CameraFloat()) {
            cameraHolder.position += Vector3.up * cameraSpeed;
        }
    }

    void Look() { 
        Vector2 lookVector = inputManager.cameraInputs.CameraLook();
        xRotation -= (lookVector.y * Time.deltaTime) * cameraSensitivity;
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        cameraHolder.Rotate(Vector3.up * (lookVector.x * Time.deltaTime) * cameraSensitivity);
    }
}
