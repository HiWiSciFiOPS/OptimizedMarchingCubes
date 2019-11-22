using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes
{
    public class MarchingCubesTerrain : MonoBehaviour
    {
        public List<MarchingCubesChunk> chunks = new List<MarchingCubesChunk>();
        public float surfaceLevel = 0.5f;
        Mesh mesh;
        public string saveFile = "Assets/voxelterrain.dat";

        public void AddChunk(Vector3Int position)
        {
            if (!ChunkAvailable(position))
            {
                GameObject chunk = new GameObject();
                chunk.name = "Chunk " + position.x + ":" + position.y + ":" + position.z;
                chunk.transform.parent = transform;
                chunk.transform.position = new Vector3(position.x * MarchingCubesChunk.size, position.y * MarchingCubesChunk.size, position.z * MarchingCubesChunk.size);
                MarchingCubesChunk cc = chunk.AddComponent<MarchingCubesChunk>();

                for (int i = 0; i < chunks.Count; i++)
                {
                    if (chunks[i].position.x == position.x)
                    {
                        if (chunks[i].position.x == position.x - 1)
                        {
                            chunks[i].neighbours[0] = cc;
                        }
                        else if (chunks[i].position.x == position.x + 1)
                        {
                            chunks[i].neighbours[1] = cc;
                        }
                    }
                    else if (chunks[i].position.y == position.y)
                    {
                        if (chunks[i].position.y == position.y - 1)
                        {
                            chunks[i].neighbours[2] = cc;
                        }
                        else if (chunks[i].position.y == position.y + 1)
                        {
                            chunks[i].neighbours[3] = cc;
                        }
                    }
                    else if (chunks[i].position.z == position.z)
                    {
                        if (chunks[i].position.z == position.z - 1)
                        {
                            chunks[i].neighbours[4] = cc;
                        }
                        else if (chunks[i].position.z == position.z + 1)
                        {
                            chunks[i].neighbours[5] = cc;
                        }
                    }
                }
                chunk.AddComponent<MeshFilter>();
                MeshRenderer mr = chunk.AddComponent<MeshRenderer>();
                mr.sharedMaterial = Resources.Load<Material>("Terrain");
                MarchingCubesChunk c = chunk.AddComponent<MarchingCubesChunk>();
                c.values[2, 2, 2] = 1;

                chunks.Add(c);

                Generate();
            }
            else
            {
                Debug.Log("Chunk already existed");
            }
        }

        public void eradicateChunk(Vector3Int position)
        {
            if (ChunkAvailable(position))
            {
                MarchingCubesChunk chunk = GetChunk(position);
                if (ChunkAvailable(position + new Vector3Int(-1, 0, 0)))
                {
                    GetChunk(position + new Vector3Int(-1, 0, 0)).neighbours[0] = null;
                }
                if (ChunkAvailable(position + new Vector3Int(1, 0, 0)))
                {
                    GetChunk(position + new Vector3Int(1, 0, 0)).neighbours[1] = null;
                }
                if (ChunkAvailable(position + new Vector3Int(0, -1, 0)))
                {
                    GetChunk(position + new Vector3Int(0, -1, 0)).neighbours[2] = null;
                }
                if (ChunkAvailable(position + new Vector3Int(0, 1, 0)))
                {
                    GetChunk(position + new Vector3Int(0, 1, 0)).neighbours[3] = null;
                }
                if (ChunkAvailable(position + new Vector3Int(0, 0, -1)))
                {
                    GetChunk(position + new Vector3Int(0, 0, -1)).neighbours[4] = null;
                }
                if (ChunkAvailable(position + new Vector3Int(0, 0, 1)))
                {
                    GetChunk(position + new Vector3Int(0, 0, 1)).neighbours[5] = null;
                }
                chunks.Remove(chunk);
            }
        }

        public bool ChunkAvailable(Vector3Int position)
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].position == position)
                {
                    return true;
                }
            }
            return false;
        }

        public MarchingCubesChunk GetChunk(Vector3Int position)
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].position == position)
                {
                    return chunks[i];
                }
            }
            return null;
        }

        public void Generate()
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                chunks[i].GenerateMesh(surfaceLevel);
            }
        }
    }
}