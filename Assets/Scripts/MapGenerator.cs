using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Range(5, 100)] 
    public float innerRadius = 60;
    [Range(1, 50)]
    public float outerRadius = 11;
    [Range(1, 50)]
    public float verticalLimit = 15;
    [Range(10, 100)]
    public float starDisplacement = 15;

    [SerializeField] GameObject starPrefab;
    [Range(1,500)]
    [SerializeField] int starsToSpawn = 100;
    [SerializeField] Transform starHolderTransform;
    StarMapNameGenerator nameGenerator;
    public List<Star> Stars = new List<Star>();
    private void Awake() {
        nameGenerator = GetComponent<StarMapNameGenerator>();
    }
    private void Start() {
        for(int i = 0; i < starsToSpawn; i++) {
            // Generate a random angle
            float angle = Random.Range(0f, Mathf.PI * 2);

            // Generate a random distance from the center within the innerRadius
            float radius = innerRadius;

            // Calculate the spawn position using polar to Cartesian conversion
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            float y = Random.Range(-verticalLimit, verticalLimit);

            // Create the spawn position
            Vector3 spawnPosition = new Vector3(x + Random.Range(-starDisplacement, starDisplacement), y, z + Random.Range(-starDisplacement, starDisplacement));

            // Check for any potential star overlaps
            if(Physics.CheckSphere(spawnPosition, 3f) == false) {
                // Instantiate the starPrefab
                GameObject tempObjectInformation = Instantiate(starPrefab, spawnPosition, Quaternion.identity, starHolderTransform);
                tempObjectInformation.name = nameGenerator.GenerateNameString();
                Stars.Add(tempObjectInformation.GetComponent<Star>());
                tempObjectInformation = null;
            } else {
                i--;
            }
        }
    }

}


