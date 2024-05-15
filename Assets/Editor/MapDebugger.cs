using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapDebugger : Editor {

    private void OnSceneGUI() {
        MapGenerator myObj = (MapGenerator)target;

        // Draw the inner radius disc
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(myObj.transform.position, myObj.transform.up, myObj.innerRadius);

        Handles.color = Color.red;
        // Draw the outer radius disc
        Handles.DrawWireDisc(myObj.transform.position, myObj.transform.up, myObj.outerRadius + myObj.innerRadius);

        Handles.color = Color.blue;
        // Draw the limit discs at vertical limits
        Handles.DrawWireDisc(myObj.transform.position - new Vector3(0, myObj.verticalLimit, 0), myObj.transform.up, myObj.outerRadius);
        Handles.DrawWireDisc(myObj.transform.position + new Vector3(0, myObj.verticalLimit, 0), myObj.transform.up, myObj.outerRadius);
    }
}
