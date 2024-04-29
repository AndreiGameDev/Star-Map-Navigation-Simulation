using UnityEngine;
using System.IO;

public class StarMapNameGenerator : MonoBehaviour
{
    [SerializeField]string[] allNames;
    public void AddNames() {
        string filePath = Path.Combine(Application.dataPath, "StarNameData.txt");
        allNames = File.ReadAllLines(filePath);
    }
    public void PrintRandomNames(int count) {
        for(int i = 0; i < count; i++) {
            Debug.Log(GenerateNameString());
        }
    }

    public string GenerateNameString() {
        int randomInteger = Random.Range(0, allNames.Length-1);
        Debug.Log(allNames.Length);
        return allNames[randomInteger];
    }
}
