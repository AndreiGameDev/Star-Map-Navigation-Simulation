using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    
    public float innerRadius = 60;
    public float outerRadius = 11;
    public float verticalLimit = 15;
    public float starDisplacement = 25;
    [SerializeField] GameObject starPrefab;
    [SerializeField] int starsToSpawn = 100;
    [SerializeField] Transform starHolderTransform;
    StarMapNameGenerator nameGenerator;
    public List<Star> Stars = new List<Star>();
    string keyInnerRadius = MapGenerationKeys.KeyInnerRadius();
    string keyOuterRadius = MapGenerationKeys.KeyOuterRadius();
    string keyVerticalLimit = MapGenerationKeys.KeyVerticalLimit();
    string keyStarDisplacement = MapGenerationKeys.KeyStarDisplacement();
    string keyStarsToSpawn = MapGenerationKeys.KeyStarsToSpawn();
    private void Awake() {
        nameGenerator = GetComponent<StarMapNameGenerator>();
        if(PlayerPrefs.HasKey(keyInnerRadius)) {
            innerRadius = PlayerPrefs.GetFloat(keyInnerRadius);
        }
        if(PlayerPrefs.HasKey(keyOuterRadius)) {
            outerRadius = PlayerPrefs.GetFloat(keyOuterRadius);
        }
        if(PlayerPrefs.HasKey(keyVerticalLimit)) {
            verticalLimit = PlayerPrefs.GetFloat(keyVerticalLimit);
        }
        if(PlayerPrefs.HasKey(keyStarDisplacement)) {
            starDisplacement = PlayerPrefs.GetFloat(keyStarDisplacement);
        }
        if(PlayerPrefs.HasKey(keyStarsToSpawn)) {
            starsToSpawn = (int)PlayerPrefs.GetFloat(keyStarsToSpawn);
        }
    }

    
    private void OnEnable() {
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
                tempObjectInformation.GetComponent<Star>().rangeToCheck = starDisplacement + 1;
                Stars.Add(tempObjectInformation.GetComponent<Star>());
                tempObjectInformation = null;
            } else {
                i--;
            }
        }
    }

}


