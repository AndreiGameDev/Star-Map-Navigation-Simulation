using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Star : MonoBehaviour
{
    // Route dictionary = Star routing to, Lenght.
    Dictionary<Star,float> routeDictionary = new Dictionary<Star, float>();
    [SerializeField] int maxRoutes;
    [SerializeField] float rangeToCheck = 10f;
    [SerializeField] GameObject RoutePreviewer;
    private void Awake() {
        maxRoutes = Random.Range(0, 3);
        RaycastHit[] stars = Physics.SphereCastAll(transform.position, rangeToCheck, transform.forward, rangeToCheck);
        if(stars.Length > 0) {
            for(int i = 0; i < maxRoutes; i++) {
                int index = Random.Range(0, stars.Length);
                Debug.Log(maxRoutes);
                Debug.Log(stars.Length);
                Debug.Log(index);
                Star star = stars[index].transform.GetComponent<Star>();
                if(!routeDictionary.ContainsKey(star)) {
                    float distance = Vector3.Distance(transform.position, star.transform.position);
                    routeDictionary.Add(star, distance);
                }
            }

            foreach(Star star in routeDictionary.Keys) {
                GameObject tempObject = Instantiate(RoutePreviewer, transform.position, Quaternion.identity, transform);
                LineRenderer lineRenderer = tempObject.GetComponent<LineRenderer>();
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, star.transform.position);
            }
        }
        

        
    }
}