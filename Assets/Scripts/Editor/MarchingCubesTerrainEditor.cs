using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MarchingCubes
{
    [CustomEditor(typeof(Terrain))]
    public class MarchingCubesTerrainEditor : Editor
    {
        const string createMenuLocation = "GameObject/3D Object/Voxel Terrain";
        const string newTerrainName = "new Voxel Terrain";
        Terrain mct;
        int selectedOption = 0;

        public override void OnInspectorGUI()
        {
            mct.surfaceLevel = EditorGUILayout.Slider("wee", mct.surfaceLevel, 0, 1);
            selectedOption = GUILayout.Toolbar(selectedOption, new GUIContent[] { new GUIContent("w"), new GUIContent("a") });
            if (GUILayout.Button("Generate Mesh"))
            {
                mct.Generate();
            }
        }

        private void OnEnable()
        {
            mct = (Terrain)target;
        }

        private void OnSceneGUI()
        {
            Handles.color = Color.red;
            Chunk cc;
            Vector3 pos;
            for (int i = 0; i < mct.chunks.Count; i++)
            {
                cc = mct.chunks[i];
                pos = cc.transform.position;
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                Handles.DrawLine(new Vector3(cc.position.x + pos.x, cc.position.y + pos.y, cc.position.z + pos.z), new Vector3(cc.position.x + pos.x, cc.position.y + pos.y + Chunk.size, cc.position.z + pos.z));
                Handles.DrawLine(new Vector3(cc.position.x + pos.x, cc.position.y + pos.y, cc.position.z + pos.z), new Vector3(cc.position.x + pos.x + Chunk.size, cc.position.y + pos.y, cc.position.z + pos.z));
                Handles.DrawLine(new Vector3(cc.position.x + pos.x, cc.position.y + pos.y, cc.position.z + pos.z), new Vector3(cc.position.x + pos.x, cc.position.y + pos.y, cc.position.z + pos.z + Chunk.size));
                Handles.DrawLine(new Vector3(cc.position.x + pos.x + Chunk.size, cc.position.y + pos.y, cc.position.z + pos.z), new Vector3(cc.position.x + pos.x + Chunk.size, cc.position.y + pos.y, cc.position.z + pos.z + Chunk.size));
                Handles.DrawLine(new Vector3(cc.position.x + pos.x + Chunk.size, cc.position.y + pos.y, cc.position.z + pos.z), new Vector3(cc.position.x + pos.x + Chunk.size, cc.position.y + pos.y + Chunk.size, cc.position.z + pos.z));
                Handles.DrawLine(new Vector3(cc.position.x + pos.x, cc.position.y + pos.y + Chunk.size, cc.position.z + pos.z), new Vector3(cc.position.x + pos.x + Chunk.size, cc.position.y + pos.y + Chunk.size, cc.position.z + pos.z));
                Handles.DrawLine(new Vector3(cc.position.x + pos.x, cc.position.y + pos.y + Chunk.size, cc.position.z + pos.z), new Vector3(cc.position.x + pos.x, cc.position.y + pos.y + Chunk.size, cc.position.z + pos.z + Chunk.size));
                Handles.DrawLine(new Vector3(cc.position.x + pos.x, cc.position.y + pos.y, cc.position.z + pos.z + Chunk.size), new Vector3(cc.position.x + pos.x + Chunk.size, cc.position.y + pos.y, cc.position.z + pos.z + Chunk.size));
                Handles.DrawLine(new Vector3(cc.position.x + pos.x, cc.position.y + pos.y, cc.position.z + pos.z + Chunk.size), new Vector3(cc.position.x + pos.x, cc.position.y + pos.y + Chunk.size, cc.position.z + pos.z + Chunk.size));
                Handles.DrawLine(new Vector3(cc.position.x + pos.x + Chunk.size, cc.position.y + pos.y + Chunk.size, cc.position.z + pos.z + Chunk.size), new Vector3(cc.position.x + pos.x + Chunk.size, cc.position.y + pos.y + Chunk.size, cc.position.z + pos.z));
                Handles.DrawLine(new Vector3(cc.position.x + pos.x + Chunk.size, cc.position.y + pos.y + Chunk.size, cc.position.z + pos.z + Chunk.size), new Vector3(cc.position.x + pos.x, cc.position.y + pos.y + Chunk.size, cc.position.z + pos.z + Chunk.size));
                Handles.DrawLine(new Vector3(cc.position.x + pos.x + Chunk.size, cc.position.y + pos.y + Chunk.size, cc.position.z + pos.z + Chunk.size), new Vector3(cc.position.x + pos.x + Chunk.size, cc.position.y + pos.y, cc.position.z + pos.z + Chunk.size));
            }
        }

        [MenuItem(createMenuLocation)]
        public static void createVoxelTerrain()
        {
            GameObject terrainGo = new GameObject();
            terrainGo.name = newTerrainName;
            terrainGo.transform.position = new Vector3(0, 0, 0);
            Terrain terrain = terrainGo.AddComponent<Terrain>();

            terrain.AddChunk(new Vector3Int(0, 0, 0));

            Selection.activeObject = terrainGo;
        }
    }
}