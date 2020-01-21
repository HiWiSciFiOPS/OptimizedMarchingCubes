using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MarchingCubesTerrain))]
    [AddComponentMenu("VoxelTerrain/Load on enable")]
    public class LoadMarchingCubesTerrainOnEnable : MonoBehaviour
    {
        public string path;
        public bool LoadByItselfInGame = true;

        private void Awake()
        {
            if (LoadByItselfInGame) {
                Load(path);
            }
            else
            {
#if UNITY_EDITOR
                Load(path);
#endif
            }
        }

        private void Load(string path)
        {
            GetComponent<MarchingCubesTerrain>().Load(path);
            Debug.Log("Terrain from \"" + path + "\" loaded");
        }
    }
}