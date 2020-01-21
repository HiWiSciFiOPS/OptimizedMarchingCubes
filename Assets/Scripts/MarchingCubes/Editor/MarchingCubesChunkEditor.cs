using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MarchingCubes
{
    [CustomEditor(typeof(MarchingCubesChunk))]
    public class MarchingCubesChunkEditor : Editor
    {
        private MarchingCubesChunk mcc;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.color = Color.red;
            Handles.DrawLine(mcc.transform.position, mcc.transform.position + new Vector3(1, 0, 0) * MarchingCubesChunk.size);
            Handles.DrawLine(mcc.transform.position, mcc.transform.position + new Vector3(0, 1, 0) * MarchingCubesChunk.size);
            Handles.DrawLine(mcc.transform.position, mcc.transform.position + new Vector3(0, 0, 1) * MarchingCubesChunk.size);
            
            Handles.DrawLine(mcc.transform.position + new Vector3(1, 1, 1) * MarchingCubesChunk.size, mcc.transform.position + new Vector3(0, 1, 1) * MarchingCubesChunk.size);
            Handles.DrawLine(mcc.transform.position + new Vector3(1, 1, 1) * MarchingCubesChunk.size, mcc.transform.position + new Vector3(1, 0, 1) * MarchingCubesChunk.size);
            Handles.DrawLine(mcc.transform.position + new Vector3(1, 1, 1) * MarchingCubesChunk.size, mcc.transform.position + new Vector3(1, 1, 0) * MarchingCubesChunk.size);

            Handles.DrawLine(mcc.transform.position + new Vector3(1, 0, 0) * MarchingCubesChunk.size, mcc.transform.position + new Vector3(1, 1, 0) * MarchingCubesChunk.size);
            Handles.DrawLine(mcc.transform.position + new Vector3(1, 0, 0) * MarchingCubesChunk.size, mcc.transform.position + new Vector3(1, 0, 1) * MarchingCubesChunk.size);
            Handles.DrawLine(mcc.transform.position + new Vector3(0, 1, 0) * MarchingCubesChunk.size, mcc.transform.position + new Vector3(1, 1, 0) * MarchingCubesChunk.size);
            Handles.DrawLine(mcc.transform.position + new Vector3(0, 1, 0) * MarchingCubesChunk.size, mcc.transform.position + new Vector3(0, 1, 1) * MarchingCubesChunk.size);
            Handles.DrawLine(mcc.transform.position + new Vector3(0, 0, 1) * MarchingCubesChunk.size, mcc.transform.position + new Vector3(1, 0, 1) * MarchingCubesChunk.size);
            Handles.DrawLine(mcc.transform.position + new Vector3(0, 0, 1) * MarchingCubesChunk.size, mcc.transform.position + new Vector3(0, 1, 1) * MarchingCubesChunk.size);

            // itself
            if (Handles.Button(Vector3.Scale(mcc.transform.parent.localScale, mcc.transform.position + new Vector3(MarchingCubesChunk.size * 0.5f, MarchingCubesChunk.size * 0.5f, MarchingCubesChunk.size * 0.5f)), Quaternion.identity, MarchingCubesChunk.size / 8, MarchingCubesChunk.size / 16, Handles.SphereHandleCap))
            {
                GameObject parent = mcc.transform.parent.gameObject;
                parent.GetComponent<MarchingCubesTerrain>().EradicateChunk(mcc.position);
                Selection.activeGameObject = parent;
                return;
            }

            // posX
            if (Handles.Button(Vector3.Scale(mcc.transform.parent.localScale, mcc.transform.position + new Vector3(MarchingCubesChunk.size * 0.5f + MarchingCubesChunk.size, MarchingCubesChunk.size * 0.5f, MarchingCubesChunk.size * 0.5f)), Quaternion.identity, MarchingCubesChunk.size / 8, MarchingCubesChunk.size / 16, Handles.SphereHandleCap))
            {
                MarchingCubesTerrain terrain = mcc.transform.parent.GetComponent<MarchingCubesTerrain>();
                Vector3Int pos = mcc.position + new Vector3Int(1, 0, 0);
                if (terrain.ChunkAvailable(pos))
                {
                    terrain.EradicateChunk(pos);
                    return;
                }
                else
                {
                    terrain.AddChunk(pos);
                }
            }

            // negX
            if (Handles.Button(Vector3.Scale(mcc.transform.parent.localScale, mcc.transform.position + new Vector3(-(MarchingCubesChunk.size * 0.5f), MarchingCubesChunk.size * 0.5f, MarchingCubesChunk.size * 0.5f)), Quaternion.identity, MarchingCubesChunk.size / 8, MarchingCubesChunk.size / 16, Handles.SphereHandleCap))
            {
                MarchingCubesTerrain terrain = mcc.transform.parent.GetComponent<MarchingCubesTerrain>();
                Vector3Int pos = mcc.position + new Vector3Int(-1, 0, 0);
                if (terrain.ChunkAvailable(pos))
                {
                    terrain.EradicateChunk(pos);
                    return;
                }
                else
                {
                    terrain.AddChunk(pos);
                }
            }

            // posY
            if (Handles.Button(Vector3.Scale(mcc.transform.parent.localScale, mcc.transform.position + new Vector3(MarchingCubesChunk.size * 0.5f, MarchingCubesChunk.size * 0.5f + MarchingCubesChunk.size, MarchingCubesChunk.size * 0.5f)), Quaternion.identity, MarchingCubesChunk.size / 8, MarchingCubesChunk.size / 16, Handles.SphereHandleCap))
            {
                MarchingCubesTerrain terrain = mcc.transform.parent.GetComponent<MarchingCubesTerrain>();
                Vector3Int pos = mcc.position + new Vector3Int(0, 1, 0);
                if (terrain.ChunkAvailable(pos))
                {
                    terrain.EradicateChunk(pos);
                    return;
                }
                else
                {
                    terrain.AddChunk(pos);
                }
            }

            // negY
            if (Handles.Button(Vector3.Scale(mcc.transform.parent.localScale, mcc.transform.position + new Vector3(MarchingCubesChunk.size * 0.5f, -(MarchingCubesChunk.size * 0.5f), MarchingCubesChunk.size * 0.5f)), Quaternion.identity, MarchingCubesChunk.size / 8, MarchingCubesChunk.size / 16, Handles.SphereHandleCap))
            {
                MarchingCubesTerrain terrain = mcc.transform.parent.GetComponent<MarchingCubesTerrain>();
                Vector3Int pos = mcc.position + new Vector3Int(0, -1, 0);
                if (terrain.ChunkAvailable(pos))
                {
                    terrain.EradicateChunk(pos);
                    return;
                }
                else
                {
                    terrain.AddChunk(pos);
                }
            }

            // posZ
            if (Handles.Button(Vector3.Scale(mcc.transform.parent.localScale, mcc.transform.position + new Vector3(MarchingCubesChunk.size * 0.5f, MarchingCubesChunk.size * 0.5f, MarchingCubesChunk.size * 0.5f + MarchingCubesChunk.size)), Quaternion.identity, MarchingCubesChunk.size / 8, MarchingCubesChunk.size / 16, Handles.SphereHandleCap))
            {
                MarchingCubesTerrain terrain = mcc.transform.parent.GetComponent<MarchingCubesTerrain>();
                Vector3Int pos = mcc.position + new Vector3Int(0, 0, 1);
                if (terrain.ChunkAvailable(pos))
                {
                    terrain.EradicateChunk(pos);
                    return;
                }
                else
                {
                    terrain.AddChunk(pos);
                }
            }

            // negZ
            if (Handles.Button(Vector3.Scale(mcc.transform.parent.localScale, mcc.transform.position + new Vector3(MarchingCubesChunk.size * 0.5f, MarchingCubesChunk.size * 0.5f, -(MarchingCubesChunk.size * 0.5f))), Quaternion.identity, MarchingCubesChunk.size / 16, MarchingCubesChunk.size / 16, Handles.SphereHandleCap))
            {
                MarchingCubesTerrain terrain = mcc.transform.parent.GetComponent<MarchingCubesTerrain>();
                Vector3Int pos = mcc.position + new Vector3Int(0, 0, -1);
                if (terrain.ChunkAvailable(pos))
                {
                    terrain.EradicateChunk(pos);
                    return;
                }
                else
                {
                    terrain.AddChunk(pos);
                }
            }
        }

        private void OnEnable()
        {
            mcc = (MarchingCubesChunk)target;
            //Selection.activeGameObject = ((MarchingCubesChunk)target).transform.parent.gameObject;
        }
    }
}