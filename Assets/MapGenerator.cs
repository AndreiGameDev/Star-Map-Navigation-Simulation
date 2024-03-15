using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Range(5, 100)] 
    public float innerRadius = 2f;
    [Range(1, 5)]
    public float outerRadius;
    [Range(1, 5)]
    public float verticalLimit;
    [Range(1, 100)]
    public float starDisplacement;

    [SerializeField] GameObject starPrefab;
    [Range(1,500)]
    [SerializeField] int starsToSpawn = 10;
    private void Awake() {
        for(int i = 0; i < starsToSpawn; i++) {
            float radius = innerRadius / 2;
            radius *= i;
            
            Vector3 spawnPosition = new Vector3(radius + Random.Range(-starDisplacement, starDisplacement), Random.Range(-verticalLimit, verticalLimit), radius + Random.Range(-starDisplacement, starDisplacement));
            Instantiate(starPrefab, spawnPosition , Quaternion.identity);
        }
    }
}


