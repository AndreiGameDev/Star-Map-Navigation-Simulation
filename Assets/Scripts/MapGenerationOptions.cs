using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
public class MapGenerationKeys {
    public static string KeyInnerRadius() {
        return "DataInnerRadius";
    }
    public static string KeyOuterRadius() {
        return "DataOutterRadius";
    }
    public static string KeyVerticalLimit() {
        return "DataVerticalLimit";
    }
    public static string KeyStarDisplacement() {
        return "DataStarDisplacement";
    }
    public static string KeyStarsToSpawn() {
        return "DataStarsToSpawn";
    }
}
public class MapGenerationOptions : MonoBehaviour
{
    [SerializeField] Slider innerRadius;
    [SerializeField] TextMeshProUGUI valueInnerRadiusUI;
    [SerializeField]Slider outerRadius;
    [SerializeField] TextMeshProUGUI valueOuterRadiusUI;
    [SerializeField]Slider verticalLimit;
    [SerializeField] TextMeshProUGUI valueVerticalLimitUI;
    [SerializeField]Slider starDisplacement;
    [SerializeField] TextMeshProUGUI valueStarDisplacementUI;
    [SerializeField]Slider starsToSpawn;
    [SerializeField] TextMeshProUGUI valueStarsToSpawnUI;

    string keyInnerRadius = MapGenerationKeys.KeyInnerRadius();
    string keyOuterRadius = MapGenerationKeys.KeyOuterRadius();
    string keyVerticalLimit = MapGenerationKeys.KeyVerticalLimit();
    string keyStarDisplacement = MapGenerationKeys.KeyStarDisplacement();
    string keyStarsToSpawn = MapGenerationKeys.KeyStarsToSpawn();

    private void Awake() {
        if(PlayerPrefs.HasKey(keyInnerRadius)) {
           innerRadius.value = PlayerPrefs.GetFloat(keyInnerRadius);
           valueInnerRadiusUI.text = MathF.Round(innerRadius.value, 2).ToString();
        } else {
            innerRadius.value = 25;
            PlayerPrefs.SetFloat(keyInnerRadius, innerRadius.value);
            valueInnerRadiusUI.text = MathF.Round(innerRadius.value, 2).ToString();
        }

        if(PlayerPrefs.HasKey(keyOuterRadius)) {
            outerRadius.value = PlayerPrefs.GetFloat(keyOuterRadius);
            valueOuterRadiusUI.text = MathF.Round(outerRadius.value, 2).ToString();
        } else {
            outerRadius.value = 25;
            PlayerPrefs.SetFloat (keyOuterRadius, outerRadius.value);
            valueOuterRadiusUI.text = MathF.Round(outerRadius.value, 2).ToString();
        }

        if(PlayerPrefs.HasKey(keyVerticalLimit)) {
            verticalLimit.value = PlayerPrefs.GetFloat(keyVerticalLimit);
            valueVerticalLimitUI.text = MathF.Round(verticalLimit.value, 2).ToString();
        } else {
            verticalLimit.value = 25;
            PlayerPrefs.SetFloat(keyVerticalLimit, verticalLimit.value);
            valueVerticalLimitUI.text= MathF.Round(verticalLimit.value, 2).ToString();
        }

        if(PlayerPrefs.HasKey(keyStarDisplacement)) {
            starDisplacement.value = PlayerPrefs.GetFloat(keyStarDisplacement);
            valueStarDisplacementUI.text = MathF.Round(starDisplacement.value, 2).ToString();
        } else {
            starDisplacement.value = 25;
            PlayerPrefs.SetFloat(keyStarDisplacement, starDisplacement.value);
            valueStarDisplacementUI.text = MathF.Round(starDisplacement.value, 2).ToString();
        }

        if(PlayerPrefs.HasKey(keyStarsToSpawn)) {
            starsToSpawn.value = (int)PlayerPrefs.GetFloat(keyStarsToSpawn);
            valueStarsToSpawnUI.text = starsToSpawn.value.ToString();
        } else {
            starsToSpawn.value = 100;
            PlayerPrefs.SetFloat(keyStarsToSpawn, starsToSpawn.value);
            valueStarsToSpawnUI.text = starsToSpawn.value.ToString();
        }
    }

    public void ChangeInnerRadius(float value) {
        PlayerPrefs.SetFloat(keyInnerRadius, value);
        valueInnerRadiusUI.text = MathF.Round(value, 2).ToString(); ;
    }
    public void ChangeOuterRadius(float value) {
        PlayerPrefs.SetFloat (keyOuterRadius, value);
        valueOuterRadiusUI.text = MathF.Round(value, 2).ToString(); ;
    }

    public void ChangeVerticalLimit(float value) {
        PlayerPrefs.SetFloat(keyVerticalLimit, value);
        valueVerticalLimitUI.text = MathF.Round(value, 2).ToString(); ;
    }

    public void ChangeStarDisplacement(float value) {
        PlayerPrefs.SetFloat(keyStarDisplacement, value);
        valueStarDisplacementUI.text = MathF.Round(value, 2).ToString(); ;
    }
    public void ChangeStarsToSpawn(float value) {
        PlayerPrefs.SetFloat(keyStarsToSpawn, (int)value);
        valueStarsToSpawnUI.text = value.ToString();
    }

    
}
