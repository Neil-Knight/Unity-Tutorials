using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Path))]
public class PathNetworkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Path network = (Path)target;
        network.DisplayMode = (PathDisplayMode)EditorGUILayout.EnumPopup("Display Mode", network.DisplayMode);

        if(network.DisplayMode == PathDisplayMode.Paths)
        {
            network.UIStart = EditorGUILayout.IntSlider("Waypoint Start", network.UIStart, 0, network.nodes.Count - 1);
            network.UIEnd = EditorGUILayout.IntSlider("Waypoint End", network.UIEnd, 0, network.nodes.Count - 1);
        }

        DrawDefaultInspector();
    }

    private void OnSceneGUI()
    {
        Path network = (Path)target;

        for (int i = 0; i < network.nodes.Count; i++)
        {
            if (network.nodes[i] != null)
                Handles.Label(network.nodes[i].position, "Waypoint " + i.ToString());
        }

        if (network.DisplayMode == PathDisplayMode.Connections)
        {
            Vector3[] linePoints = new Vector3[network.nodes.Count + 1];

            for (int i = 0; i <= network.nodes.Count; i++)
            {
                int index = i != network.nodes.Count ? i : 0;
                if (network.nodes[index] != null)
                    linePoints[i] = network.nodes[index].position;
                else
                    linePoints[i] = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
            }

            Handles.color = Color.cyan;
            Handles.DrawPolyLine(linePoints);
        }
        else if (network.DisplayMode == PathDisplayMode.Paths)
        {
            UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
            if (network.nodes[network.UIStart] != null && network.nodes[network.UIEnd] != null)
            {
                Vector3 from = network.nodes[network.UIStart].position;
                Vector3 to = network.nodes[network.UIEnd].position;
                UnityEngine.AI.NavMesh.CalculatePath(from, to, UnityEngine.AI.NavMesh.AllAreas, path);
                Handles.color = Color.yellow;
                Handles.DrawPolyLine(path.corners);
            }
        }
    }
}
