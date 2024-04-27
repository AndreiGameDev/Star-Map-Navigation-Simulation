using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Star : MonoBehaviour
{
    // Route dictionary = Star routing to, Lenght.
    public Dictionary<Star,float> routeDictionary = new Dictionary<Star, float>();
    [SerializeField] int maxRoutes;
    [SerializeField] float rangeToCheck = 10f;
    [SerializeField] GameObject RoutePreviewer;
    public MeshRenderer meshRenderer;
    public Dictionary<Star, LineRenderer> routeConnectors = new Dictionary<Star, LineRenderer>();
    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        maxRoutes = Random.Range(0, 3);
        if(maxRoutes > 0) {
            RaycastHit[] stars = Physics.SphereCastAll(transform.position, rangeToCheck, transform.forward, rangeToCheck);
            if(stars.Length > 0) {
                for(int i = 0; i < maxRoutes; i++) {
                    int index = Random.Range(0, stars.Length);
                    Star star = stars[index].transform.GetComponent<Star>();
                    if(!routeDictionary.ContainsKey(star)) {
                        float distance = Vector3.Distance(transform.position, star.transform.position);
                        //https://discussions.unity.com/t/round-float-with-2-decimal/81611/3
                        distance = System.MathF.Round(distance, 2);
                        routeDictionary.Add(star, distance);
                        if(!star.routeDictionary.ContainsKey(this)) {
                            star.routeDictionary.Add(this, distance);
                        }
                    }
                }
            }
        }
    }

    private void Start() {
        foreach(Star star in routeDictionary.Keys) {
            if(!routeConnectors.ContainsKey(star)) {
                GameObject tempObject = Instantiate(RoutePreviewer, transform.position, Quaternion.identity, transform);
                LineRenderer lineRenderer = tempObject.GetComponent<LineRenderer>();
                routeConnectors.Add(star, lineRenderer);
                if(!star.routeConnectors.ContainsKey(this)) {
                    star.routeConnectors.Add(this, lineRenderer);
                }
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, star.transform.position);
            }
        }
    }
}