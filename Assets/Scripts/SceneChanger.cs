using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static void ChangeScene(int scene) {
        SceneManager.LoadScene(scene);
    }
}
