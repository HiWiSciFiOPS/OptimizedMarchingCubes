using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;

namespace MarchingCubes
{
    [RequireComponent(typeof(Transform))]
    public class MarchingCubesTerrain : MonoBehaviour
    {
        /// <summary>
        /// all
        /// </summary>
        public List<MarchingCubesChunk> chunks = new List<MarchingCubesChunk>();
        public float surfaceLevel = 0.5f;
        Mesh mesh;
        public float density;
        public string saveFile = "Assets/voxelterrain/";

        /// <summary>
        /// Adds a chunk to the terrain
        /// </summary>
        /// <param name="position">the position for the new chunk in the chunk grid</param>
        /// <returns>the created chunk or null if no chunk could be created</returns>
        public MarchingCubesChunk AddChunk(Vector3Int position)
        {
            if (!ChunkAvailable(position))
            {
                GameObject chunk = new GameObject();
                chunk.name = "Chunk " + position.x + ":" + position.y + ":" + position.z;
                chunk.transform.parent = transform;
                chunk.transform.position = new Vector3(position.x * MarchingCubesChunk.size, position.y * MarchingCubesChunk.size, position.z * MarchingCubesChunk.size);

                MarchingCubesChunk cc = chunk.AddComponent<MarchingCubesChunk>();
                cc.position = position;
                
                chunks.Add(cc);

                for (int i = 0; i < chunks.Count; i++)
                {
                    if (chunks[i].position.x == position.x && chunks[i].position.y == position.y)
                    {
                        if (chunks[i].position.z == position.z - 1)
                        {
                            chunks[i].neighbours[4] = cc;
                            cc.neighbours[5] = chunks[i];
                        }
                        else if (chunks[i].position.z == position.z + 1)
                        {
                            chunks[i].neighbours[5] = cc;
                            cc.neighbours[4] = chunks[i];
                        }
                    }
                    else if (chunks[i].position.x == position.x && chunks[i].position.z == position.z)
                    {
                        if (chunks[i].position.y == position.y - 1)
                        {
                            chunks[i].neighbours[2] = cc;
                            cc.neighbours[3] = chunks[i];
                        }
                        else if (chunks[i].position.y == position.y + 1)
                        {
                            chunks[i].neighbours[3] = cc;
                            cc.neighbours[2] = chunks[i];
                        }
                    }
                    else if (chunks[i].position.y == position.y && chunks[i].position.z == position.z)
                    {
                        if (chunks[i].position.x == position.x - 1)
                        {
                            chunks[i].neighbours[0] = cc;
                            cc.neighbours[1] = chunks[i];
                        }
                        else if (chunks[i].position.x == position.x + 1)
                        {
                            chunks[i].neighbours[1] = cc;
                            cc.neighbours[0] = chunks[i];
                        }
                    }
                }
                chunk.GetComponent<MeshFilter>();
                MeshRenderer mr = chunk.GetComponent<MeshRenderer>();
                mr.sharedMaterial = Resources.Load<Material>("Terrain");
                cc.values[2, 2, 2].v = 1;
                for (int x = 0; x < cc.xLength; x++)
                {
                    for (int y = 0; y < cc.yLength; y++)
                    {
                        for (int z = 0; z < cc.zLength; z++)
                        {
                            cc.values[x, y, z].c = Color.green;
                        }
                    }
                }

#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(gameObject);
#endif
                if (chunks.Count == 1) {
                    Generate();
                }

                return cc;
            }
            return null;
        }

        /// <summary>
        /// Removes a chunk from the terrain
        /// </summary>
        /// <param name="position">The position of the Chunk to remove in the chunk grid</param>
        public void EradicateChunk(Vector3Int position)
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
#if UNITY_EDITOR
                DestroyImmediate(chunk.gameObject);
#else
                Destroy(chunk.gameObject);
#endif
            }
        }

        /// <summary>
        /// Check if a chunk exists
        /// </summary>
        /// <param name="position">The position of the chunk to check for in the chunk grid</param>
        /// <returns>true if the chunk does exist</returns>
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

        /// <summary>
        /// find a chunk at a position
        /// </summary>
        /// <param name="position">the position of the chunk to look for in the chunk grid</param>
        /// <returns>the chunk at the given position in the chunk grid or null if it could not be found</returns>
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

        /// <summary>
        /// generates the mesh from the scalar field of every chunk
        /// </summary>
        public void Generate()
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                chunks[i].GenerateMesh(surfaceLevel, density);
            }
        }

        /// <summary>
        /// clears the mesh
        /// </summary>
        public void ClearMesh()
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                chunks[i].ClearMesh();
            }
        }

        /// <summary>
        /// saves the terrain into a folder on the hard drive
        /// </summary>
        /// <param name="path">the path to the folder to store the terrain in</param>
        public void Save(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".dat") || files[i].EndsWith(".dat.meta"))
                {
                    string[] split = files[i].Split('/');
                    split = split[split.Length - 1].Split('\\');
                    split[0] = split[split.Length-1];
                    if (split[0] == "terrain.dat" || split[0] == "terrain.dat.meta" || split[0].StartsWith("chunk"))
                    {
                        File.Delete(files[i]);
                    }
                }
            }

            saveFile = path;
            BinaryWriter writer = new BinaryWriter(File.Open(path + "/terrain.dat", FileMode.Create));

            writer.Write(MarchingCubesChunk.size);
            writer.Write(chunks.Count);

            writer.Close();
            writer.Dispose();

            for (int i = 0; i < chunks.Count; i++)
            {
                writer = new BinaryWriter(File.Open(path + "/chunk" + i + ".dat", FileMode.Create));

                writer.Write(chunks[i].position.x);
                writer.Write(chunks[i].position.y);
                writer.Write(chunks[i].position.z);

                for (int x = 0; x < chunks[i].xLength; x++)
                {
                    for (int y = 0; y < chunks[i].yLength; y++)
                    {
                        for (int z = 0; z < chunks[i].zLength; z++)
                        {
                            writer.Write(chunks[i].values[x, y, z].v);
                            writer.Write(chunks[i].values[x, y, z].c.r);
                            writer.Write(chunks[i].values[x, y, z].c.g);
                            writer.Write(chunks[i].values[x, y, z].c.b);
                        }
                    }
                }

                writer.Close();
                writer.Dispose();
            }
        }

        /// <summary>
        /// Loads the terrain from a given folder
        /// </summary>
        /// <param name="path">the path to the folder containing the files</param>
        public void Load(string path)
        {
            if (!File.Exists(path + "/terrain.dat"))
                throw new Exception("terrain file not found");
            saveFile = path;
            BinaryReader reader = new BinaryReader(File.Open(path + "/terrain.dat", FileMode.Open));
            
            MarchingCubesChunk.size = reader.ReadInt32();
            int chunkCount = reader.ReadInt32();
            
            reader.Close();
            reader.Dispose();

            MarchingCubesChunk[] ccs = GetComponentsInChildren<MarchingCubesChunk>();
            chunks = new List<MarchingCubesChunk>();
            for (int i = 0; i < ccs.Length; i++)
            {
#if UNITY_EDITOR
                DestroyImmediate(ccs[i].gameObject);
#else
                Destroy(ccs[i].gameObject);
#endif
            }

            for (int i = 0; i < chunkCount; i++)
            {
                reader = new BinaryReader(File.Open(path + "/chunk" + i + ".dat", FileMode.Open));

                Vector3Int pos = new Vector3Int(0, 0, 0);
                pos.x = reader.ReadInt32();
                pos.y = reader.ReadInt32();
                pos.z = reader.ReadInt32();
                MarchingCubesChunk cc = AddChunk(pos);

                for (int x = 0; x < chunks[i].xLength; x++)
                {
                    for (int y = 0; y < chunks[i].yLength; y++)
                    {
                        for (int z = 0; z < chunks[i].zLength; z++)
                        {
                            chunks[i].values[x, y, z].v = reader.ReadSingle();
                            chunks[i].values[x, y, z].c = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        }
                    }
                }

                reader.Close();
                reader.Dispose();
            }

            Generate();
        }
    }
}