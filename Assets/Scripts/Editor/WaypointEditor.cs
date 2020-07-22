using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var o = target as Waypoint;

        if(GUILayout.Button("Setup")) {
            o.CollectWaypointsFromChilds();
        }

        //if(GUILayout.Button("Assign Id to all")) {
        //    o.AssignIdToAll();
        //}
        //if(GUILayout.Button("Auto pair all")) {
        //    o.AutoPairAll();
        //}
        //if(GUILayout.Button("Reset all pairs")) {
        //    o.ResetAll();
        //}
    }
}
