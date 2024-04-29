using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StarMapNameGenerator))]
public class StarMapNameGeneratorEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if(GUILayout.Button("Add Names")) {
            StarMapNameGenerator generatorw = (StarMapNameGenerator)target;
            generatorw.AddNames();
        }
    }
}
