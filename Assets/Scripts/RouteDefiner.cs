using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// Script which finds routes and is used for Star Galaxy Navigation
/// </summary>
public class RouteDefiner : MonoBehaviour {
    private static RouteDefiner instance;
    public static RouteDefiner Instance {
        get { return instance; }
    }
    Star StartPointStar;
    Star EndPointStar;
    List<Star> galaxyStarList = new List<Star>();
    [SerializeField] List<Star> starRoute = new List<Star>();
    List<Star> potentialStarRoutes = new List<Star>();
    List<LineRenderer> potentialStarRouteConnector = new List<LineRenderer>();
    List<LineRenderer> starRouteConnectors = new List<LineRenderer>();
    MapGenerator mapGenerator;
    Dictionary<Star, List<StarRoute>> starRoutesDictionary = new Dictionary<Star, List<StarRoute>>();
    [Header("UI")]
    [SerializeField] TextMeshProUGUI routeTextUI;
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
    }

    /// <summary>
    /// Handles the event when a star is clicked for route selection.
    /// </summary>
    /// <param name="star">The star that was clicked.</param>
    public void EventClickAction(Star star) {
        if(StartPointStar == null) {
            StartPointStar = star;
            star.meshRenderer.material = startPointStarMaterial;
            PrecalculateStarRoutePaths(star);
            AvailableEndPointShowcase();
        } else if(EndPointStar == null && StartPointStar != star && potentialStarRoutes.Contains(star)) {
            ResetPotentialEndPoints();
            EndPointStar = star;
            star.meshRenderer.material = endPointStarMaterial;
        } else {
            Debug.Log("Start Point and End Point are both already set.");
        }
    }

    /// UI Button Method to search the shortest route
    public void SearchDefinedRoute() {
        if(StartPointStar != null && EndPointStar != null && starRoutesDictionary.ContainsKey(StartPointStar)) {
            ResetPath(true);
            foreach(StarRoute starData in starRoutesDictionary[StartPointStar]) {
                if(starData.endPoint == EndPointStar) {
                    starRoute = starData.pathToEndPoint;
                }
            }
            SearchAndEnableRouteConnectors();
            HighLightStartAndEndPoint();
            SetUITextRoute();
        }
    }

    /// <summary>
    /// Sets the UI text to display the route information.
    /// </summary>
    private void SetUITextRoute() {
        foreach(Star star in starRoute) {
            string text = routeTextUI.text;
            float distance = 0;
            string distanceText = "";
            if(starRoute[starRoute.IndexOf(star)] != EndPointStar) {
                distance = star.routeDictionary[starRoute[starRoute.IndexOf(star) + 1]].distance;
                distanceText = distance + " Galaxy Miles away from next star.";
            } else {
                distance = 0;
                distanceText = "Destination point";
            }
            routeTextUI.text = text + (starRoute.IndexOf(star) + 1) + ". " + star.name + " - " + distanceText + "\n";
        }
    }

    /// <summary>
    /// Automatically searches for the safest route between the start and end points.
    /// </summary>
    public void AutoSearchSafestRoute() {
        if(StartPointStar != null && EndPointStar != null && starRoutesDictionary.ContainsKey(StartPointStar)) {
            ResetPath(true);
            StarRouteDangerData tempData = FindSafestPath(StartPointStar, EndPointStar);
            starRoute = tempData.route;
            SearchAndEnableRouteConnectors();
            HighLightStartAndEndPoint();
            routeTextUI.text = "Found the most safe route available, displaying route with a danger level of: " + tempData.dangerLevel + ".\n";
            SetUITextSafeRoute();
        }
    }

    /// <summary>
    /// Sets the UI text to display the safest route information.
    /// </summary>
    private void SetUITextSafeRoute() {
        foreach(Star star in starRoute) {
            string text = routeTextUI.text;
            float distance = 0;
            string distanceText = "";
            if(starRoute[starRoute.IndexOf(star)] != EndPointStar) {
                distance = star.routeDictionary[starRoute[starRoute.IndexOf(star) + 1]].distance;
                distanceText = distance + " Galaxy Miles away from next star.";
            } else {
                distance = 0;
                distanceText = "Destination point";
            }
            routeTextUI.text = text + (starRoute.IndexOf(star) + 1) + ". " + star.name + " - " + distanceText + "\n";
        }
    }

    /// <summary>
    /// Resets the path, UI, star colors, and route connectors.
    /// </summary>
    /// <param name="isPathfinding"> If it's set to false this won't reset StartPoint and EndStarPoint for pathfinding functions. Use True inside methods. Use false for when used in UI. </param>
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
            starRouteConnectors.Clear();
            routeTextUI.text = "";
        } else {
            ResetPotentialEndPoints();
            ResetPathPoints(isPathfinding);
        }
    }

    /// <summary>
    /// Precalculates the route paths from the start point to all other stars.
    /// </summary>
    /// <param name="StartPoint">The starting star.</param>
    private void PrecalculateStarRoutePaths(Star StartPoint) {
        if(!starRoutesDictionary.ContainsKey(StartPoint)) {
            for(int i = 0; i < galaxyStarList.Count; i++) {
                Star Destination = galaxyStarList[i];
                if(StartPoint == Destination) {
                    break;
                } else {
                    if(ReturnsPath(StartPoint, Destination)) {
                        AddTostarRoutesDictionary(starRoutesDictionary, StartPoint, new StarRoute(Destination, FindPath(StartPoint, Destination)));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Finds every available path and displays them with a different colour.
    /// </summary>
    void AvailableEndPointShowcase() {
        if(starRoutesDictionary.ContainsKey(StartPointStar)) {
            foreach(StarRoute starPathData in starRoutesDictionary[StartPointStar]) {
                potentialStarRoutes.Add(starPathData.endPoint);
            }
            for(int j = 0; j < starRoutesDictionary[StartPointStar].Count; j++) {
                for(int p = 0; p < starRoutesDictionary[StartPointStar][j].pathToEndPoint.Count - 1; p++) {
                    Star startStar = starRoutesDictionary[StartPointStar][j].pathToEndPoint[p];
                    Star destinationStar = starRoutesDictionary[StartPointStar][j].pathToEndPoint[p + 1];
                    if(potentialStarRouteConnector.Contains(startStar.routeConnectors[destinationStar]) == false) {
                        potentialStarRouteConnector.Add(startStar.routeConnectors[destinationStar]);
                    }
                    foreach(LineRenderer line in potentialStarRouteConnector) {
                        line.gameObject.SetActive(true);
                    }
                }
            }
            foreach(Star star in potentialStarRoutes) {
                star.meshRenderer.material = starPathMaterial;
            }
        }
    }

    /// <summary>
    /// Resets the material and clears the list for potential end points for the star selected.
    /// </summary>
    void ResetPotentialEndPoints() {
        foreach(Star star in potentialStarRoutes) {
            star.meshRenderer.material = defaultStarMaterial;
        }
        foreach(LineRenderer line in potentialStarRouteConnector) {
            line.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Checks if a path exists between two stars.
    /// </summary>
    /// <param name="star1">The starting star.</param>
    /// <param name="star2">The destination star.</param>
    /// <returns>True if a path exists, false otherwise.</returns>
    bool ReturnsPath(Star star1, Star star2) {
        return FindPath(star1, star2).Count > 0;
    }

    /// <summary>
    /// Adds a route to the star routes dictionary.
    /// </summary>
    /// <param name="dictionary">The dictionary to add to.</param>
    /// <param name="key">The starting star.</param>
    /// <param name="value">The route to add.</param>
    static void AddTostarRoutesDictionary(Dictionary<Star, List<StarRoute>> dictionary, Star key, StarRoute value) {
        if(dictionary.ContainsKey(key) == false) {
            dictionary[key] = new List<StarRoute>();
        }
        dictionary[key].Add(value);
    }

    /// <summary>
    /// Highlights the start and end points with specified materials.
    /// </summary>
    private void HighLightStartAndEndPoint() {
        StartPointStar.meshRenderer.material = startPointStarMaterial;
        EndPointStar.meshRenderer.material = endPointStarMaterial;
    }

    /// <summary>
    /// Searches and enables the route connectors for the defined route.
    /// </summary>
    private void SearchAndEnableRouteConnectors() {
        for(int i = 0; i < starRoute.Count - 1; i++) {
            Star startStar = starRoute[i];
            Star destinationStar = starRoute[i + 1];
            starRouteConnectors.Add(startStar.routeConnectors[destinationStar]);
        }
        foreach(LineRenderer route in starRouteConnectors) {
            route.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Resets the path points and their materials.
    /// </summary>
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

    /// <summary>
    /// Finds the path between two stars using the shortest distance algorithm.
    /// </summary>
    /// <param name="startStar">The starting star.</param>
    /// <param name="endGoalStar">The destination star.</param>
    /// <returns>List of stars representing the path.</returns>
    public List<Star> FindPath(Star startStar, Star endGoalStar) {
        List<StarPath> priorityList = new List<StarPath>();
        // Generates star paths for each star so the variables can be stored
        foreach(Star star in galaxyStarList) {
            StarPath newPath = new StarPath(star, null);
            if(star == startStar) {
                newPath.shortestPathToStart = newPath;
                newPath.smallestDistanceToStart = 0.0f;
            }
            priorityList.Add(newPath);
        }
        priorityList = OrderPriorityListByDistance(priorityList);

        while(priorityList.Count > 0) {
            StarPath currentStarNode = priorityList[0];
            /// Calculates the distance and checks if it's the shortest path found yet to the start, if it is then it gets set as current star node and it's distance gets stored as next star
            foreach(StarPath nextStar in GetStarPathsFromRoutesDictionary(currentStarNode.star.routeDictionary, priorityList)) {
                float distance = nextStar.star.routeDictionary[currentStarNode.star].distance + currentStarNode.smallestDistanceToStart;
                if(distance < nextStar.smallestDistanceToStart) {
                    nextStar.smallestDistanceToStart = distance;
                    nextStar.shortestPathToStart = currentStarNode;
                }
            }
            priorityList.Remove(currentStarNode);
            if(priorityList.Count > 0) {
                priorityList = OrderPriorityListByDistance(priorityList);
                // Returns the path if the top priority star is the destination point since it means it found a path.
                if(priorityList[0].star == endGoalStar && priorityList[0].smallestDistanceToStart != float.MaxValue) {
                    List<Star> pathToEnd = new List<Star>();
                    StarPath backtrackStar = priorityList[0];
                    pathToEnd.Add(backtrackStar.star);
                    while(backtrackStar.star != startStar) {
                        backtrackStar = backtrackStar.shortestPathToStart;
                        pathToEnd.Add(backtrackStar.star);
                    }
                    pathToEnd.Reverse();
                    return pathToEnd;
                }
            }
        }

        return new List<Star>();
    }

    /// <summary>
    /// Finds the safest path between two stars based on danger levels.
    /// </summary>
    /// <param name="startStar">The starting star.</param>
    /// <param name="endGoalStar">The destination star.</param>
    /// <returns>StarRouteDangerData containing the route and danger level.</returns>
    public StarRouteDangerData FindSafestPath(Star startStar, Star endGoalStar) {
        List<StarPathDanger> priorityList = new List<StarPathDanger>();

        // Generates star danger paths for each star so the variables can be stored
        foreach(Star star in galaxyStarList) {
            StarPathDanger newPath = new StarPathDanger(star, null);
            if(star == startStar) {
                newPath.safestPathToStart = newPath;
                newPath.lowestDangerLevelPathToStart = 0;
            }
            priorityList.Add(newPath);
        }
        priorityList = OrderPriorityBySafety(priorityList);

        while(priorityList.Count > 0) {
            StarPathDanger currentStarNode = priorityList[0];
            // This checks the danger value of the path and records the lowest found danger route
            foreach(StarPathDanger nextStar in GetStarPathsDangerFromRoutesDictionary(currentStarNode.star.routeDictionary, priorityList)) {
                int dangerLevel = nextStar.star.routeDictionary[currentStarNode.star].dangerLevel + currentStarNode.lowestDangerLevelPathToStart;
                if(dangerLevel < nextStar.lowestDangerLevelPathToStart) {
                    nextStar.lowestDangerLevelPathToStart = dangerLevel;
                    nextStar.safestPathToStart = currentStarNode;
                }
            }
            priorityList.Remove(currentStarNode);
            if(priorityList.Count > 0) {
                priorityList = OrderPriorityBySafety(priorityList);
                if(priorityList[0].star == endGoalStar && priorityList[0].lowestDangerLevelPathToStart != int.MaxValue) {
                    List<Star> pathToEnd = new List<Star>();
                    StarPathDanger backtrackStar = priorityList[0];
                    int danger = backtrackStar.lowestDangerLevelPathToStart;

                    pathToEnd.Add(backtrackStar.star);

                    while(backtrackStar.star != startStar) {
                        backtrackStar = backtrackStar.safestPathToStart;
                        pathToEnd.Add(backtrackStar.star);
                    }
                    Debug.Log(backtrackStar.lowestDangerLevelPathToStart);
                    pathToEnd.Reverse();
                    return new StarRouteDangerData(pathToEnd,danger);
                }
            }
        }

        return new StarRouteDangerData();
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
    private static List<StarPathDanger> OrderPriorityBySafety(List<StarPathDanger> a_pathList) {
        for(int i = 0; i < a_pathList.Count; i++) {
            for(int j = 0; j < a_pathList.Count - 1; j++) {
                StarPathDanger first = a_pathList[j];
                StarPathDanger second = a_pathList[j + 1];
                if(first.lowestDangerLevelPathToStart > second.lowestDangerLevelPathToStart) {
                    a_pathList[j] = second;
                    a_pathList[j + 1] = first;
                }
            }
        }

        return a_pathList;
    }
    private static List<StarPath> GetStarPathsFromRoutesDictionary(Dictionary<Star, Star.StarValues> starRoutes, List<StarPath> priorityList) {
        List<StarPath> result = new List<StarPath>();

        foreach(KeyValuePair<Star, Star.StarValues> route in starRoutes) {
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
    private static List<StarPathDanger> GetStarPathsDangerFromRoutesDictionary(Dictionary<Star, Star.StarValues> starRoutes, List<StarPathDanger> priorityList) {
        List<StarPathDanger> result = new List<StarPathDanger>();

        foreach(KeyValuePair<Star, Star.StarValues> route in starRoutes) {
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
    public class StarPathDanger {
        public Star star;
        public StarPathDanger safestPathToStart;
        public int lowestDangerLevelPathToStart;
        public StarPathDanger(Star _star, StarPathDanger _safestPathToStart) {
            star = _star;
            safestPathToStart = _safestPathToStart;
            lowestDangerLevelPathToStart = int.MaxValue;
        }
    }
    //Dictionary Data
    public struct StarRoute {
        public Star endPoint;
        public List<Star> pathToEndPoint;
        public StarRoute(Star _endPoint, List<Star> _pathToEndPoint) {
            endPoint = _endPoint;
            pathToEndPoint = _pathToEndPoint;
        }
    }

    public struct StarRouteDangerData {
        public List<Star> route;
        public int dangerLevel;
        public StarRouteDangerData(List<Star> _route, int _dangerLevel) {
            route = _route;
            dangerLevel = _dangerLevel;
        }
    }
}
