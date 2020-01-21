using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MarchingCubes
{
    [CustomEditor(typeof(LoadMarchingCubesTerrainOnEnable))]
    public class LoadMarchingCubesTerrainOnEnableEditor : Editor
    {
        LoadMarchingCubesTerrainOnEnable lmctoe;

        public void OnEnable()
        {
            lmctoe = (LoadMarchingCubesTerrainOnEnable)target;
        }

        public override void OnInspectorGUI()
        {
            lmctoe.path = EditorGUILayout.TextField("Load Terrain from", lmctoe.path);
        }
    }
}