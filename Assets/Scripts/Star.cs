using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// Star routes and route connections get made within this script.
/// </summary>
public class Star : MonoBehaviour
{
    // Route dictionary = Star routing to, Lenght.
    public Dictionary<Star,StarValues> routeDictionary = new Dictionary<Star, StarValues>();
    [SerializeField] int maxRoutes;
    public float rangeToCheck = 10f;
    [SerializeField] GameObject RoutePreviewer;
    public MeshRenderer meshRenderer;
    public Dictionary<Star, LineRenderer> routeConnectors = new Dictionary<Star, LineRenderer>();
    public int dangerValue;
    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        maxRoutes = Random.Range(0, 3);
        dangerValue = Random.Range(0, 100);
        if(maxRoutes > 0) {
            LooksForRoute();
        }
    }

    private void Start() {
        AssignsRouteConnectorPositions();
    }
    /// <summary>
    /// Checks within a sphere any stars that could be a route
    /// </summary>
    private void LooksForRoute() {
        RaycastHit[] stars = Physics.SphereCastAll(transform.position, rangeToCheck, transform.forward, rangeToCheck);
        if(stars.Length > 0) {
            for(int i = 0; i < maxRoutes; i++) {
                int index = Random.Range(0, stars.Length);
                Star star = stars[index].transform.GetComponent<Star>();
                AssignRouteInDictionary(star);
            }
        }
    }
    /// <summary>
    /// Assigns route connectors for every connection this star has.
    /// </summary>
    private void AssignsRouteConnectorPositions() {
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
    /// <summary>
    /// Assigns star to the route dictionary.
    /// </summary>
    /// <param name="star">Star that will be added to the dictionary</param>
    private void AssignRouteInDictionary(Star star) {
        if(!routeDictionary.ContainsKey(star)) {
            float distance = Vector3.Distance(transform.position, star.transform.position);
            distance = System.MathF.Round(distance, 2);
            routeDictionary.Add(star, new StarValues(distance, star.dangerValue));
            if(!star.routeDictionary.ContainsKey(this)) {
                star.routeDictionary.Add(this, new StarValues(distance, dangerValue));
            }
        }
    }

    /// <summary>
    /// Data type for stars that get used in Pathfinding Algorithm.
    /// </summary>
    public class StarValues {
        public float distance;
        public int dangerLevel;
        public StarValues(float a_distance, int a_dangerLevel) {
            distance = a_distance;
            dangerLevel = a_dangerLevel;
        }
    }
}