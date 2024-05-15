using UnityEngine;

public class RotateCameraMenu : MonoBehaviour
{
    [SerializeField] float speed;
    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }

}
