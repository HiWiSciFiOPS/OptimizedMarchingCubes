using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MarchingCubes
{
    [CustomEditor(typeof(Chunk))]
    public class MarchingCubesChunkEditor : Editor
    {
        private void OnEnable()
        {
            Selection.activeGameObject = ((Chunk)target).transform.parent.gameObject;
        }
    }
}