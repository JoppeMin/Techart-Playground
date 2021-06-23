using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(MeshShape))]
public class EditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshShape meshShape = (MeshShape)target;

        if(meshShape.size.magnitude > 50)
        {
            EditorGUILayout.HelpBox("Creating larger sizes is not recommended \n as this will hinder performance tremendously", MessageType.Warning);
        }
        
        if (GUILayout.Button("Generate Mesh"))
        {
            meshShape.GenerateNewMesh();
        }
    }
}
