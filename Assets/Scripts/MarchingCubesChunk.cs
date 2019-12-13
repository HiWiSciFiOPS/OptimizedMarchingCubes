using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes
{
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class MarchingCubesChunk : MonoBehaviour
    {
        //constants
        /// <summary>
        /// the chunnk size in unity units
        /// </summary>
        public static int size = 10;

        const string greaterErrorMessage = "Surface level may not be greater than 1";
        const string lessErrorMessage = "Surface level may not be smaller than 0";

        //neighbours
        /// <summary>
        /// xpos, xneg, ypos, yneg, zpos, zneg
        /// </summary>
        public MarchingCubesChunk[] neighbours = new MarchingCubesChunk[6];

        //chunk location
        /// <summary>
        /// The Chunk position in the chunk grid
        /// </summary>
        public Vector3Int position = new Vector3Int(0, 0, 0);

        /// <summary>
        /// has the chunk been loaded?
        /// </summary>
        public bool isLoaded = false;

        //point grid
        public float[,,] values = new float[size,size,size];
        public int xLength { get { return values.GetLength(0); } }
        public int yLength { get { return values.GetLength(1); } }
        public int zLength { get { return values.GetLength(2); } }

        //Mesh generation
        public void GenerateMesh(float surflevel, float density)
        {
            GenerateMesh(surflevel, density, true, false);
        }

        public void GenerateMesh(float surflevel, float density, bool useCollider, bool sharedVertices)
        {
            if (surflevel > 1)
                throw new Exception(greaterErrorMessage);
            else if (surflevel < 0)
                throw new Exception(lessErrorMessage);

            Mesh m = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            int[] currTri = { -1, -1 };

            float[] val = new float[8];
            int cubeindex;
            int[] triData = new int[LookupTable.triTable[0].Length];
            Vector3 vertPosition;
            int vert;

            for (int x = 0; x < xLength-1; x++)
            {
                for (int y = 0; y < yLength-1; y++)
                {
                    for (int z = 0; z < zLength-1; z++)
                    {
                        val[0] = values[x, y, z];
                        val[1] = values[x, y, z + 1];
                        val[2] = values[x + 1, y, z + 1];
                        val[3] = values[x + 1, y, z];
                        val[4] = values[x, y + 1, z];
                        val[5] = values[x, y + 1, z + 1];
                        val[6] = values[x + 1, y + 1, z + 1];
                        val[7] = values[x + 1, y + 1, z];

                        cubeindex = 0;
                        if (val[0] < surflevel) cubeindex |= 1;
                        if (val[1] < surflevel) cubeindex |= 2;
                        if (val[2] < surflevel) cubeindex |= 4;
                        if (val[3] < surflevel) cubeindex |= 8;
                        if (val[4] < surflevel) cubeindex |= 16;
                        if (val[5] < surflevel) cubeindex |= 32;
                        if (val[6] < surflevel) cubeindex |= 64;
                        if (val[7] < surflevel) cubeindex |= 128;

                        triData = LookupTable.triTable[cubeindex];

                        for (int i = 0; i < triData.Length; i++)
                        {
                            //exit for loop if last vertex has been read out
                            if (triData[i] == -1)
                                break;

                            float vertXPos = 0;
                            float vertYPos = 0;
                            float vertZPos = 0;
                            switch (triData[i])
                            {
                                case 0:
                                    vertXPos = x * density;
                                    vertYPos = y * density;
                                    vertZPos = z * density + 0.5f * density;
                                    break;
                                case 1:
                                    vertXPos = x * density + 0.5f * density;
                                    vertYPos = y * density;
                                    vertZPos = z * density + 1 * density;
                                    break;
                                case 2:
                                    vertXPos = x * density + 1 * density;
                                    vertYPos = y * density;
                                    vertZPos = z * density + 0.5f * density;
                                    break;
                                case 3:
                                    vertXPos = x * density + 0.5f * density;
                                    vertYPos = y * density;
                                    vertZPos = z * density;
                                    break;
                                case 4:
                                    vertXPos = x * density;
                                    vertYPos = y * density + 1 * density;
                                    vertZPos = z * density + 0.5f * density;
                                    break;
                                case 5:
                                    vertXPos = x * density + 0.5f * density;
                                    vertYPos = y * density + 1 * density;
                                    vertZPos = z * density + 1 * density;
                                    break;
                                case 6:
                                    vertXPos = x * density + 1 * density;
                                    vertYPos = y * density + 1 * density;
                                    vertZPos = z * density + 0.5f * density;
                                    break;
                                case 7:
                                    vertXPos = x * density + 0.5f * density;
                                    vertYPos = y * density + 1 * density;
                                    vertZPos = z * density;
                                    break;
                                case 8:
                                    vertXPos = x * density;
                                    vertYPos = y * density + 0.5f * density;
                                    vertZPos = z * density;
                                    break;
                                case 9:
                                    vertXPos = x * density;
                                    vertYPos = y * density + 0.5f * density;
                                    vertZPos = z * density + 1 * density;
                                    break;
                                case 10:
                                    vertXPos = x * density + 1 * density;
                                    vertYPos = y * density + 0.5f * density;
                                    vertZPos = z * density + 1 * density;
                                    break;
                                case 11:
                                    vertXPos = x * density + 1 * density;
                                    vertYPos = y * density + 0.5f * density;
                                    vertZPos = z * density;
                                    break;
                            }
                            vertPosition = new Vector3(vertXPos, vertYPos, vertZPos);

                            vert = -1;

                            if (sharedVertices)
                            {
                                for (int r = 0; r < vertices.Count; r++)
                                {
                                    if (vertices[r] == vertPosition)
                                    {
                                        vert = r;
                                    }
                                }
                            }

                            if (vert == -1)
                            {
                                vertices.Add(vertPosition);
                                vert = vertices.Count - 1;
                            }

                            if (currTri[0] != -1 && currTri[1] != -1)
                            {
                                triangles.Add(currTri[0]);
                                triangles.Add(currTri[1]);
                                triangles.Add(vert);
                                currTri[0] = -1;
                                currTri[1] = -1;
                            }
                            else
                            {
                                if (currTri[0] != -1)
                                {
                                    currTri[1] = vert;
                                }
                                else
                                {
                                    currTri[0] = vert;
                                }
                            }
                        }
                    }
                }
            }

            m.SetVertices(vertices);
            m.SetTriangles(triangles, 0);
            Vector2[] uvs = new Vector2[vertices.Count];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            }
            m.uv = uvs;
            m.RecalculateNormals();
            m.name = "Generated Mesh " + position.x + ":" + position.y + ":" + position.z;

            GetComponent<MeshFilter>().mesh = m;
            if (useCollider)
            {
                GetComponent<MeshCollider>().sharedMesh = null;
            }
        }

        public void ClearMesh()
        {
            GetComponent<MeshFilter>().mesh = null;
            if (GetComponent<MeshCollider>())
                GetComponent<MeshCollider>().sharedMesh = null;
        }
    }
}