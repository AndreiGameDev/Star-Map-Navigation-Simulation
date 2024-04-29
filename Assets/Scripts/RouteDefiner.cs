using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] List<Star> starRoute = new List<Star>();
    // Potential routes for star selection
    List<Star> potentialStarRoutes = new List<Star>();
    // All star path connectors
    List<LineRenderer> starRouteConnectors = new List<LineRenderer>();
    MapGenerator mapGenerator;
    [SerializeField] TextMeshProUGUI routeTextUI;
    [SerializeField] GameObject image;
    Dictionary<Star, List<StarRoute>> starRoutesDictionary = new Dictionary<Star, List<StarRoute>>();
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
    public void EventClickAction(Star star) { // Click interaction with the route pathfinder
        // If the start star is not assigned then assign it
        // Else if the end point star is not assigned, the star clicked is not the start oiunt star and it's within reach then select it
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
    public void SearchDefinedRoute() {
        // If statement checks if we have a start and end point selected and if it contains a route.
        if(StartPointStar != null && EndPointStar != null && starRoutesDictionary.ContainsKey(StartPointStar)) {
            ResetPath(true); // Cleans up previous stuff
            foreach(StarRoute starData in starRoutesDictionary[StartPointStar]) { // Digs the route
                if(starData.endPoint == EndPointStar) {
                    starRoute = starData.pathToEndPoint;
                }
            }
            SearchAndEnableRouteConnectors();

            HighLightStartAndEndPoint();

            // UI Text route display
            foreach(Star star in starRoute) {
                string text = routeTextUI.text;
                float distance = 0;
                string distanceText = "";
                if(starRoute[starRoute.IndexOf(star)] != EndPointStar) {
                    distance = star.routeDictionary[starRoute[starRoute.IndexOf(star) + 1]].distance; // Index needs to be + 1 coz it's calculating distance to next star
                    distanceText = distance + " Galaxy Miles away from next star.";
                } else {
                    distance = 0;
                    distanceText = "Destination point";
                }
                routeTextUI.text = text + (starRoute.IndexOf(star) + 1) + ". " + star.name + " - " + distanceText + "\n";
            }
        }
    }
    public void AutoSearchSafestRoute() {
        if(StartPointStar != null && EndPointStar != null && starRoutesDictionary.ContainsKey(StartPointStar)) {
            ResetPath(true);
            StarRouteDangerData tempData = FindSafestPath(StartPointStar, EndPointStar);
            starRoute = tempData.route;
            SearchAndEnableRouteConnectors();
            HighLightStartAndEndPoint();
            routeTextUI.text = "Found the most safe route available, displaying route with a danger level of: " + tempData.dangerLevel + ".\n";
            // UI Text route display
            foreach(Star star in starRoute) {
                string text = routeTextUI.text;
                float distance = 0;
                string distanceText = "";
                if(starRoute[starRoute.IndexOf(star)] != EndPointStar) {
                    distance = star.routeDictionary[starRoute[starRoute.IndexOf(star) + 1]].distance; // Index needs to be + 1 coz it's calculating distance to next star
                    distanceText = distance + " Galaxy Miles away from next star.";
                } else {
                    distance = 0;
                    distanceText = "Destination point";
                }
                routeTextUI.text = text + (starRoute.IndexOf(star) + 1) + ". " + star.name + " - " + distanceText + "\n";
            }
        }
    }
    
    public void ResetPath(bool isPathfinding) { // Resets UI, Star colors, route connectors and cleans up route list
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
    private void PrecalculateStarRoutePaths(Star StartPoint) {
        if(!starRoutesDictionary.ContainsKey(StartPoint)) {
            // Display Image to explain paths are being calculated
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
            // Turn off image 
        }
    }
    void AvailableEndPointShowcase() {
        if(starRoutesDictionary.ContainsKey(StartPointStar)) {
            foreach(StarRoute starPathData in starRoutesDictionary[StartPointStar]) {
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
    static void AddTostarRoutesDictionary(Dictionary<Star, List<StarRoute>> dictionary, Star key, StarRoute value) {
        if(dictionary.ContainsKey(key) == false) {
            dictionary[key] = new List<StarRoute>();
        }
        dictionary[key].Add(value);
    }
    private void HighLightStartAndEndPoint() {
        // Makes sure there is material set on path
        StartPointStar.meshRenderer.material = startPointStarMaterial;
        EndPointStar.meshRenderer.material = endPointStarMaterial;
    }
    private void SearchAndEnableRouteConnectors() {
        // Digs the route connectors
        for(int i = 0; i < starRoute.Count - 1; i++) {
            Star startStar = starRoute[i];
            Star destinationStar = starRoute[i + 1];
            starRouteConnectors.Add(startStar.routeConnectors[destinationStar]);
        }
        // Colour route connectors and star paths
        for(int i = 1; i < starRoute.Count - 1; i++) {
            starRoute[i].meshRenderer.material = starPathMaterial;
        }
        // Activate the route connectors
        foreach(LineRenderer route in starRouteConnectors) {
            route.gameObject.SetActive(true);
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
    public static StarRouteDangerData FindLowestDangerLevelRoute(List<List<Star>> StarRouteLists) {
        List<StarRouteDangerData> dangerStarRoutes = new List<StarRouteDangerData>();
        foreach(List<Star> starList in StarRouteLists) {
            int dangerLevel = 0;
            foreach(Star route in starList) {
                dangerLevel += route.dangerValue;
            }
            dangerStarRoutes.Add(new StarRouteDangerData(starList, dangerLevel));
        }
        return SortByLowestDanger(dangerStarRoutes);
    }

    private static StarRouteDangerData SortByLowestDanger(List<StarRouteDangerData> _starRoutesDangerLevel) {
        for(int i = 0; i < _starRoutesDangerLevel.Count; i++) {
            for(int j = 0; j < _starRoutesDangerLevel.Count - 1; j++) {
                StarRouteDangerData first = _starRoutesDangerLevel[j];
                StarRouteDangerData second = _starRoutesDangerLevel[j + 1];
                if(first.dangerLevel < second.dangerLevel) {
                    _starRoutesDangerLevel[j] = second;
                    _starRoutesDangerLevel[j + 1] = first;
                }
            }
        }
        return _starRoutesDangerLevel[0];
    }

    public static StarRouteDistanceData FindLowestDistanceLevelRoute(List<List<Star>> StarRouteLists) {
        List<StarRouteDistanceData> starRoutesDistanceInfo = new List<StarRouteDistanceData>();
        float distance = 0;
        foreach(List<Star> starlist in StarRouteLists) {
            foreach(Star route in starlist) {
                for(int i = 0; i < starlist.Count; i++) {
                    if(i != starlist.Count) {
                        Star destination = starlist[i + 1];
                        distance += starlist[i].routeDictionary[destination].distance;
                    }
                }
                starRoutesDistanceInfo.Add(new StarRouteDistanceData(starlist, distance));
            }
        }
        return SortByLowestDistance(starRoutesDistanceInfo);
    }

    private static StarRouteDistanceData SortByLowestDistance(List<StarRouteDistanceData> _staRoutesDistanceData) {
        for(int i = 0; i < _staRoutesDistanceData.Count; i++) {
            for(int j = 0; j < _staRoutesDistanceData.Count - 1; j++) {
                StarRouteDistanceData first = _staRoutesDistanceData[j];
                StarRouteDistanceData second = _staRoutesDistanceData[j + 1];
                if(first.distance < second.distance) {
                    _staRoutesDistanceData[j] = second;
                    _staRoutesDistanceData[j + 1] = first;
                }
            }
        }

        return _staRoutesDistanceData[0];
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
                float distance = nextStar.star.routeDictionary[currentStarNode.star].distance + currentStarNode.smallestDistanceToStart;

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
    public StarRouteDangerData FindSafestPath(Star startStar, Star endGoalStar) {
        List<StarPathDanger> priorityList = new List<StarPathDanger>();

        //This is what the algorithm will loop through and use to know what stars are connected to what other stars in the galaxy graph.
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

    public struct StarRouteDistanceData {
        public List<Star> route;
        public float distance;
        public StarRouteDistanceData(List<Star> _route, float _distance) {
            route = _route;
            distance = _distance;
        }
    }
}
