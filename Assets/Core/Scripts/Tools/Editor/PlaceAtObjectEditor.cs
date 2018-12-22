using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(PlaceAtObject))]
public class PlaceAtObjectEditor : Editor
{
    SerializedProperty ReplacementDatas;
    SerializedProperty DestroyAfterSpawn;

    void OnEnable()
    {
        ReplacementDatas = serializedObject.FindProperty("ReplacementDatas");
        DestroyAfterSpawn = serializedObject.FindProperty("DestroyAfterSpawn");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        PlaceAtObject myScript = (PlaceAtObject)target;
        EditorGUILayout.PropertyField(ReplacementDatas, true);
        EditorGUILayout.PropertyField(DestroyAfterSpawn, true);

        if (GUILayout.Button("Spawn Object")) myScript.DoTask();

        serializedObject.ApplyModifiedProperties();
    }
}
