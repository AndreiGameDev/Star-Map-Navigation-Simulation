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
    [SerializeField]Slider outerRadius;
    [SerializeField]Slider verticalLimit;
    [SerializeField]Slider starDisplacement;
    [SerializeField]Slider starsToSpawn;

    string keyInnerRadius = MapGenerationKeys.KeyInnerRadius();
    string keyOuterRadius = MapGenerationKeys.KeyOuterRadius();
    string keyVerticalLimit = MapGenerationKeys.KeyVerticalLimit();
    string keyStarDisplacement = MapGenerationKeys.KeyStarDisplacement();
    string keyStarsToSpawn = MapGenerationKeys.KeyStarsToSpawn();

    private void Awake() {
        if(PlayerPrefs.HasKey(keyInnerRadius)) {
           innerRadius.value = PlayerPrefs.GetFloat(keyInnerRadius);
        } else {
            innerRadius.value = 25;
            PlayerPrefs.SetFloat(keyInnerRadius, innerRadius.value);
        }

        if(PlayerPrefs.HasKey(keyOuterRadius)) {
            outerRadius.value = PlayerPrefs.GetFloat(keyOuterRadius);
        } else {
            outerRadius.value = 25;
            PlayerPrefs.SetFloat (keyOuterRadius, outerRadius.value);
        }

        if(PlayerPrefs.HasKey(keyVerticalLimit)) {
            verticalLimit.value = PlayerPrefs.GetFloat(keyVerticalLimit);
        } else {
            verticalLimit.value = 25;
            PlayerPrefs.SetFloat(keyVerticalLimit, verticalLimit.value);
        }

        if(PlayerPrefs.HasKey(keyStarDisplacement)) {
            starDisplacement.value = PlayerPrefs.GetFloat(keyStarDisplacement);
        } else {
            starDisplacement.value = 25;
            PlayerPrefs.SetFloat(keyStarDisplacement, starDisplacement.value);
        }

        if(PlayerPrefs.HasKey(keyStarsToSpawn)) {
            starsToSpawn.value = (int)PlayerPrefs.GetFloat(keyStarsToSpawn);
            Debug.Log(PlayerPrefs.GetFloat(keyStarsToSpawn));
        } else {
            starsToSpawn.value = 100;
            PlayerPrefs.SetFloat(keyStarsToSpawn, starsToSpawn.value);
        }
    }

    public void ChangeInnerRadius(float value) {
        PlayerPrefs.SetFloat(keyInnerRadius, value);
    }
    public void ChangeOuterRadius(float value) {
        PlayerPrefs.SetFloat (keyOuterRadius, value);
    }

    public void ChangeVerticalLimit(float value) {
        PlayerPrefs.SetFloat(keyVerticalLimit, value);
    }

    public void ChangeStarDisplacement(float value) {
        PlayerPrefs.SetFloat(keyStarDisplacement, value);
    }
    public void ChangeStarsToSpawn(float value) {
        PlayerPrefs.SetFloat(keyStarsToSpawn, (int)value);
    }

}
