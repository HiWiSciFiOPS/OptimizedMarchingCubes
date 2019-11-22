using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MarchingCubes
{
    [CustomEditor(typeof(MarchingCubesChunk))]
    public class MarchingCubesChunkEditor : Editor
    {
        private void OnEnable()
        {
            Selection.activeGameObject = ((MarchingCubesChunk)target).transform.parent.gameObject;
        }
    }
}