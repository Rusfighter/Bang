using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MaterialCombiner))]
public class MaterialCombinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MaterialCombiner myScript = (MaterialCombiner)target;
        if (GUILayout.Button("Copy meshes"))
        {
            myScript.DeepCloneMeshes();
        }

        if (GUILayout.Button("Combine meshes"))
        {
            myScript.CombineMeshes();
        }

        if (GUILayout.Button("Combine texture (atlassing)"))
        {
            myScript.CombineTextures();
        }

        if (GUILayout.Button("Enable all meshes"))
        {
            myScript.EnableAllMf();
        }
    }
}
