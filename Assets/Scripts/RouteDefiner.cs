using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RouteDefiner : MonoBehaviour {
    private static RouteDefiner instance;
    public static RouteDefiner Instance {
        get { return instance; }
    }
    Star StartPointStar;
    Star EndPointStar;
    //All stars in the hiearchy
    List<Star> galaxyStarList = new List<Star>();
    // All stars inside the path
    [SerializeField]List<Star> starRoute = new List<Star>();
    // Potential routes for star selection
    List<Star> potentialStarRoutes = new List<Star>();
    // All star path connectors
    List<LineRenderer> starRouteConnectors = new List<LineRenderer>();
    MapGenerator mapGenerator;
    [SerializeField] TextMeshProUGUI routeTextUI;

    Dictionary<Star, List<StarPathData>> starRoutesDictionary = new Dictionary<Star, List<StarPathData>>();
    [Header("StarMaterials")]
    [SerializeField] Material defaultStarMaterial;
    [SerializeField] Material startPointStarMaterial;
    [SerializeField] Material endPointStarMaterial;
    [SerializeField] Material starPathMaterial;
    [Header("StarConnectorMaterials")]
    [SerializeField] Material routePathMaterial;
    private void Awake() {
        instance = this;

        mapGenerator = GetComponent<MapGenerator>();
    }
    private void Start() {
        galaxyStarList = mapGenerator.Stars;
        PrecalculateStarRoutePaths();
    }

    private void PrecalculateStarRoutePaths() {
        foreach(Star StartPoint in galaxyStarList) {
            for(int i = 0; i < galaxyStarList.Count; i++) {
                Star Destination = galaxyStarList[i];
                if(StartPoint == Destination) {
                    break;
                } else {
                    if(ReturnsPath(StartPoint, Destination)) {
                        AddTostarRoutesDictionary(starRoutesDictionary, StartPoint, new StarPathData(Destination, FindPath(StartPoint, Destination)));
                    }
                }
            }
        }
    }

    void AvailableEndPointShowcase() {
        if(starRoutesDictionary.ContainsKey(StartPointStar)) {
            foreach(StarPathData starPathData in starRoutesDictionary[StartPointStar]) {
                potentialStarRoutes.Add(starPathData.endPoint);
            }
            foreach(Star star in potentialStarRoutes) {
                star.meshRenderer.material = starPathMaterial;
            }
        }
    }

    void ResetPotentialEndPoints() {
        foreach(Star star in potentialStarRoutes) {
            star.meshRenderer.material = defaultStarMaterial;
        }
    }
    bool ReturnsPath(Star star1, Star star2) {
        if(FindPath(star1, star2).Count > 0) {
            return true;
        } else {
            return false;
        }
    }
    static void AddTostarRoutesDictionary(Dictionary<Star, List<StarPathData>> dictionary, Star key, StarPathData value) {
        if(dictionary.ContainsKey(key) == false) {
            dictionary[key] = new List<StarPathData>();
        }
        dictionary[key].Add(value);
    }
    public void EventClickAction(Star star) {
        if(StartPointStar == null ) {
            StartPointStar = star;
            star.meshRenderer.material = startPointStarMaterial;
            AvailableEndPointShowcase();
        } else if(EndPointStar == null && StartPointStar != star && potentialStarRoutes.Contains(star)) {
            ResetPotentialEndPoints();
            EndPointStar = star;
            star.meshRenderer.material = endPointStarMaterial;
        } else {
            Debug.Log("Start Point and End Point are both already set.");
        }
    }

    public void LocatePathAndConnectors() {
        if(StartPointStar != null && EndPointStar != null && starRoutesDictionary.ContainsKey(StartPointStar)) {
            ResetPath(true);
            foreach(StarPathData starData in starRoutesDictionary[StartPointStar]) {
                if(starData.endPoint == EndPointStar) {
                    starRoute = starData.pathToEndPoint;
                }
            }
            
            //starRoute = FindPath(StartPointStar, EndPointStar);
            for(int i = 0; i < starRoute.Count - 1; i++) {
                Star startStar = starRoute[i];
                Star destinationStar = starRoute[i + 1];
                starRouteConnectors.Add(startStar.routeConnectors[destinationStar]);
            }
            // Colour route connectors and star paths
            for(int i = 1; i < starRoute.Count - 1; i++) {
                starRoute[i].meshRenderer.material = starPathMaterial;
            }
            foreach(LineRenderer route in starRouteConnectors) {
                route.gameObject.SetActive(true);
            }
            StartPointStar.meshRenderer.material = startPointStarMaterial;
            EndPointStar.meshRenderer.material = endPointStarMaterial;

            foreach(Star star in starRoute) {
                string text = routeTextUI.text;
                float distance = 0;
                string distanceText = "";
                if(starRoute[starRoute.IndexOf(star)] != EndPointStar) {
                    distance = star.routeDictionary[starRoute[starRoute.IndexOf(star) + 1]];
                    distanceText = distance + " Galaxy Miles away from next star.";
                } else {
                    distance = 0;
                    distanceText = "Destination point";
                }
                routeTextUI.text = text + (starRoute.IndexOf(star) + 1) + ". " + star.name + " - " + distanceText + "\n";
            }
        }
    }

    public void ResetPath(bool isPathfinding) {
        if(starRouteConnectors.Count > 0 || starRoute.Count > 0) {
            ResetPotentialEndPoints();
            ResetPathPoints(isPathfinding);
            foreach(Star star in starRoute) {
                star.meshRenderer.material = defaultStarMaterial;
            }
            foreach(LineRenderer route in starRouteConnectors) {
                route.gameObject.SetActive(false);
            }
            routeTextUI.text = "";
        } else {
            ResetPotentialEndPoints();
            ResetPathPoints(isPathfinding);
        }
    }

    private void ResetPathPoints(bool isPathfinding) {
        if(StartPointStar != null && isPathfinding == false) {
            StartPointStar.meshRenderer.material = defaultStarMaterial;
            StartPointStar = null;
        }
        if(EndPointStar != null && isPathfinding == false) {
            EndPointStar.meshRenderer.material = defaultStarMaterial;
            EndPointStar = null;
        }
    }

    public List<Star> FindPath(Star startStar, Star endGoalStar) {
        List<StarPath> priorityList = new List<StarPath>();

        //This is what the algorithm will loop through and use to know what stars are connected to what other stars in the galaxy graph.
        foreach(Star star in galaxyStarList) {
            StarPath newPath = new StarPath(star, null);
            if(star == startStar) {
                newPath.shortestPathToStart = newPath;
                newPath.smallestDistanceToStart = 0.0f;
            }
            priorityList.Add(newPath);
        }
        priorityList = OrderPriorityListByDistance(priorityList);


        //Go through the priority list and for each star look at it's connections and find the distance to the start star.
        //We know that there is a path between the start and end stars when the end node is at the start of the priority list and it has the highest priority.
        while(priorityList.Count > 0) {
            StarPath currentStarNode = priorityList[0];//This should be the starting star on the first iteration. This is the star with the top priority to look at.

            //Check all the stars that connect to the current star and calculate the distance to that star from the start node.
            //Once this has been done update the star path information for each node so we know the shortest path to the node.
            foreach(StarPath nextStar in GetStarPathsFromRoutesDictionary(currentStarNode.star.routeDictionary, priorityList)) {
                //Calculate the distance to the starting star by adding the distance from the start node to the current star onto the distance from the current star to the next star.
                float distance = nextStar.star.routeDictionary[currentStarNode.star] + currentStarNode.smallestDistanceToStart;

                //Check if this new path's distance is shorter than the current path to this star from the start node.
                if(distance < nextStar.smallestDistanceToStart) {
                    //Coming from the start via the current star is the shortest path to this star so update the path information.
                    nextStar.smallestDistanceToStart = distance;
                    nextStar.shortestPathToStart = currentStarNode;//This variable/line of code contains the information that tells you what path is currently the shortest one back to the starting star.
                }
            }

            //Once we reach this point the current star at the top of the priority list has fully been checked!!!
            //Now we should remove it from the priority list and then reorder it for the next iteration of the loop.
            priorityList.Remove(currentStarNode);
            if(priorityList.Count > 0) {
                priorityList = OrderPriorityListByDistance(priorityList);
                if(priorityList[0].star == endGoalStar && priorityList[0].smallestDistanceToStart != float.MaxValue) {
                    //If the star with the highest priority is now the end star then that means we have found the shortest path to the end.
                    //However the distance to the start must not be infinite else that just means we haven't found a connection.
                    //as the end star will only have the highest priority when it has the shortest distance value so we now need to backtrack and construct the path list to return.
                    List<Star> pathToEnd = new List<Star>();
                    StarPath backtrackStar = priorityList[0]; //This is currently the end goal star.
                    pathToEnd.Add(backtrackStar.star);

                    while(backtrackStar.star != startStar) {
                        //We are now retracing our steps. Going through each star that is the shortest path to the current one and then adding it to the path to the end goal list.
                        backtrackStar = backtrackStar.shortestPathToStart;
                        pathToEnd.Add(backtrackStar.star);
                    }

                    //The path to the start star from the end star has been constructed. We now want to reverse it so that that the path given is from the start star to the end star instead.
                    pathToEnd.Reverse();
                    return pathToEnd;//We have the path to the goal star so we now want to return it!!!
                }
            }
        }

        //If we get out of the loop then there is no path to the end as the priority list has ran out without getting to the end star.
        return new List<Star>();
    }
    private static List<StarPath> OrderPriorityListByDistance(List<StarPath> a_pathList) {
        for(int i = 0; i < a_pathList.Count; i++) {
            for(int j = 0; j < a_pathList.Count - 1; j++) {
                StarPath first = a_pathList[j];
                StarPath second = a_pathList[j + 1];
                if(first.smallestDistanceToStart > second.smallestDistanceToStart) {
                    a_pathList[j] = second;
                    a_pathList[j + 1] = first;
                }
            }
        }

        return a_pathList;
    }
    private static List<StarPath> GetStarPathsFromRoutesDictionary(Dictionary<Star, float> starRoutes, List<StarPath> priorityList) {
        List<StarPath> result = new List<StarPath>();

        foreach(KeyValuePair<Star, float> route in starRoutes) {
            Star star = route.Key;
            for(int i = 0; i < priorityList.Count; i++) {
                if(priorityList[i].star == star) {
                    result.Add(priorityList[i]);
                    break;
                }
            }
        }
        return result;
    }
    public class StarPath {
        public Star star;
        public StarPath shortestPathToStart;
        public float smallestDistanceToStart;

        public StarPath(Star a_current, StarPath a_shortest) {
            star = a_current;
            shortestPathToStart = a_shortest;
            smallestDistanceToStart = float.MaxValue;
        }
    }

    public struct StarPathData {
        public Star endPoint;
        public List<Star> pathToEndPoint;
        public StarPathData(Star EndPoint, List<Star> PathToEndPoint) {
            this.endPoint = EndPoint;
            this.pathToEndPoint = PathToEndPoint;
            PathToEndPoint = new List<Star>();
        }
    }
}
