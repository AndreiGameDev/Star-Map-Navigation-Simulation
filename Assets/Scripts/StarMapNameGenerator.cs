using UnityEngine;
using System.IO;

public class StarMapNameGenerator : MonoBehaviour
{
    [SerializeField]string[] allNames;
    private void Awake() {
        string filePath = Path.Combine(Application.dataPath, "StarNameData.txt");
        allNames = File.ReadAllLines(filePath);
    }

    public void PrintRandomNames(int count) {
        for(int i = 0; i < count; i++) {
            Debug.Log(GenerateNameString());
        }
    }

    public string GenerateNameString() {
        int randomInteger = Random.Range(0, allNames.Length);
        return allNames[randomInteger];
    }
}
