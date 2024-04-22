using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapDebugger : Editor {

    private void OnSceneGUI() {
        Handles.color = Color.yellow;
        MapGenerator myObj = (MapGenerator)target;
        Handles.DrawWireDisc(myObj.transform.position, myObj.transform.up, myObj.innerRadius);
        Handles.color = Color.red;
        Handles.DrawWireDisc(myObj.transform.position, myObj.transform.up, myObj.innerRadius - myObj.outerRadius);
        Handles.DrawWireDisc(myObj.transform.position, myObj.transform.up, myObj.innerRadius + myObj.outerRadius);
        Handles.color = Color.blue;
        //Handles.DrawWireDisc(myObj.transform.position, myObj.transform.right, myObj.innerRadius - myObj.outerRadius);
        //Handles.DrawWireDisc(myObj.transform.position, myObj.transform.right, myObj.innerRadius + myObj.outerRadius);
        Handles.DrawWireDisc(myObj.transform.position - new Vector3(0, myObj.verticalLimit, 0), myObj.transform.up, myObj.innerRadius);
        Handles.DrawWireDisc(myObj.transform.position + new Vector3(0, myObj.verticalLimit, 0), myObj.transform.up, myObj.innerRadius);
    }
}
