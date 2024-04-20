using System.Collections.Generic;
using UnityEngine;

public class RouteDefiner : MonoBehaviour
{
    private static RouteDefiner instance;
    public static RouteDefiner Instance {
        get { return instance; }
    }
    [SerializeField] Star StartPointStar;
    [SerializeField] Star EndPointStar;

    [SerializeField]List<Star> starRoute = new List<Star>();
    [SerializeField] Material defaultStarMaterial;
    [SerializeField] Material startPointStarMaterial;
    [SerializeField] Material endPointStarMaterial;
    private void Awake() {
        instance = this;
    }
    public void EventClickAction(Star star) {
        if(StartPointStar == null) {
            StartPointStar = star;
            star.meshRenderer.material = startPointStarMaterial;
        } else if(EndPointStar == null) {
            EndPointStar = star;
            star.meshRenderer.material = endPointStarMaterial;
        } else if(star == StartPointStar) {
            StartPointStar = null;
            star.meshRenderer.material = defaultStarMaterial;
        } else if(star == EndPointStar) {
            EndPointStar = null;
            star.meshRenderer.material = defaultStarMaterial;
        } else {
            Debug.Log("Start Point and End Point are both already set.");
        }
    }
}
