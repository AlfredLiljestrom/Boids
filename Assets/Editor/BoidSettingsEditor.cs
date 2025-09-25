using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoidSettings))]
public class BoidSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector (so fields still show)
        DrawDefaultInspector();

        BoidSettings settings = (BoidSettings)target;

        // Add a button
        if (GUILayout.Button("Reset Values"))
        {
            settings.ResetValues();
            EditorUtility.SetDirty(settings);
        }
    }
}

