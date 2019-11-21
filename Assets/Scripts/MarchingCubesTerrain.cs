using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes
{
    public class Terrain : MonoBehaviour
    {
        public List<Chunk> chunks = new List<Chunk>();
        public float surfaceLevel = 0.5f;
        Mesh mesh;
        public string saveFile = "Assets/voxelterrain.dat";

        public void AddChunk(Vector3Int position)
        {
            GameObject chunk = new GameObject();
            chunk.name = "Chunk " + position.x + ":" + position.y + ":" + position.z;
            chunk.transform.parent = transform;
            Chunk cc = chunk.AddComponent<Chunk>();

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
            Chunk c = chunk.AddComponent<Chunk>();
            c.values[2, 2, 2] = 1;

            chunks.Add(c);

            Generate();
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