using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes
{
    [RequireComponent(typeof(MarchingCubesTerrain))]
    [AddComponentMenu("VoxelTerrain/Load on enable")]
    public class LoadMarchingCubesTerrainOnEnable : MonoBehaviour
    {
        public string path;
        private void OnEnable()
        {
            GetComponent<MarchingCubesTerrain>().Load(path);
            Debug.Log("Terrain from \"" + path + "\" loaded");
        }
    }
}