using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

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
        public GridPoint[,,] values = new GridPoint[size, size, size];
        public int xLength { get { return values.GetLength(0); } }
        public int yLength { get { return values.GetLength(1); } }
        public int zLength { get { return values.GetLength(2); } }

        //Mesh generation
        public void GenerateMesh(float surflevel, float density)
        {
            GenerateMesh(surflevel, density, true);
        }

        private float thread_surflevel;
        private float thread_density;
        private Mesh thread_mesh;
        private List<Vector3> thread_vertices = new List<Vector3>();
        private List<Color> thread_colors = new List<Color>();
        private List<int> thread_triangles = new List<int>();
        private void GenerateMeshThreaded()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<int> triangles = new List<int>();
            int[] currTri = { -1, -1 };

            GridPoint[] val = new GridPoint[8];
            int cubeindex;
            int[] triData = new int[LookupTable.triTable[0].Length];
            Vector3 vertPosition;
            int vert;

            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        try
                        {
                            val[0] = values[x, y, z];

                            bool xEdge = false;
                            bool yEdge = false;
                            bool zEdge = false;
                            if (x >= xLength - 1)
                                xEdge = true;
                            if (y >= yLength - 1)
                                yEdge = true;
                            if (z >= zLength - 1)
                                zEdge = true;

                            if (!zEdge)
                                val[1] = values[x, y, z + 1];
                            else if (neighbours[4] != null)
                                val[1] = neighbours[4].values[x, y, 0];
                            else
                                continue;

                            if (!xEdge && !zEdge)
                                val[2] = values[x + 1, y, z + 1];
                            else if (((xEdge && zEdge) && neighbours[0] != null) && neighbours[0].neighbours[4] != null)
                                val[2] = neighbours[0].neighbours[4].values[0, y, 0];
                            else if (xEdge && neighbours[0] != null)
                                val[2] = neighbours[0].values[0, y, z + 1];
                            else if (zEdge && neighbours[4] != null)
                                val[2] = neighbours[4].values[x + 1, y, 0];
                            else
                                continue;

                            if (!xEdge)
                                val[3] = values[x + 1, y, z];
                            else if (xEdge && neighbours[0] != null)
                                val[3] = neighbours[0].values[0, y, z];
                            else
                                continue;

                            if (!yEdge)
                                val[4] = values[x, y + 1, z];
                            else if (yEdge && neighbours[2] != null)
                                val[4] = neighbours[2].values[x, 0, z];
                            else
                                continue;

                            if (!yEdge && !zEdge)
                                val[5] = values[x, y + 1, z + 1];
                            else if (((yEdge && zEdge) && neighbours[2] != null) && neighbours[2].neighbours[4] != null)
                                val[5] = neighbours[2].neighbours[4].values[x, 0, 0];
                            else if (yEdge && neighbours[2] != null)
                                val[5] = neighbours[2].values[x, 0, z + 1];
                            else if (zEdge && neighbours[4] != null)
                                val[5] = neighbours[4].values[x, y + 1, 0];
                            else
                                continue;

                            if (!xEdge && !yEdge && !zEdge)
                                val[6] = values[x + 1, y + 1, z + 1];
                            else if ((((xEdge && yEdge && zEdge) && neighbours[0] != null) && neighbours[0].neighbours[2] != null) && neighbours[0].neighbours[2].neighbours[4] != null)
                                val[6] = neighbours[0].neighbours[2].neighbours[4].values[0, 0, 0];
                            else if (((xEdge && yEdge) && neighbours[0] != null) && neighbours[0].neighbours[2] != null)
                                val[6] = neighbours[0].neighbours[2].values[0, 0, z + 1];
                            else if (((xEdge && zEdge) && neighbours[0] != null) && neighbours[0].neighbours[4] != null)
                                val[6] = neighbours[0].neighbours[4].values[0, y + 1, 0];
                            else if (((yEdge && zEdge) && neighbours[2] != null) && neighbours[2].neighbours[4] != null)
                                val[6] = neighbours[2].neighbours[4].values[x + 1, 0, 0];
                            else if (xEdge && neighbours[0] != null)
                                val[6] = neighbours[0].values[0, y + 1, z + 1];
                            else if (yEdge && neighbours[2] != null)
                                val[6] = neighbours[2].values[x + 1, 0, z + 1];
                            else if (zEdge && neighbours[4] != null)
                                val[6] = neighbours[4].values[x + 1, y + 1, 0];
                            else
                                continue;

                            if (!xEdge && !yEdge)
                                val[7] = values[x + 1, y + 1, z];
                            else if (((xEdge && yEdge) && neighbours[0] != null) && neighbours[0].neighbours[2] != null)
                                val[7] = neighbours[0].neighbours[2].values[0, 0, z];
                            else if (xEdge && neighbours[0] != null)
                                val[7] = neighbours[0].values[0, y + 1, z];
                            else if (yEdge && neighbours[2] != null)
                                val[7] = neighbours[2].values[x + 1, 0, z];
                            else
                                continue;
                        }
                        catch
                        {
                            continue;
                        }

                        // get position of mesh in tritable
                        cubeindex = 0;
                        if (val[0].v < thread_surflevel) cubeindex |= 1;
                        if (val[1].v < thread_surflevel) cubeindex |= 2;
                        if (val[2].v < thread_surflevel) cubeindex |= 4;
                        if (val[3].v < thread_surflevel) cubeindex |= 8;
                        if (val[4].v < thread_surflevel) cubeindex |= 16;
                        if (val[5].v < thread_surflevel) cubeindex |= 32;
                        if (val[6].v < thread_surflevel) cubeindex |= 64;
                        if (val[7].v < thread_surflevel) cubeindex |= 128;

                        // get mesh for current cube
                        triData = LookupTable.triTable[cubeindex];

                        for (int i = 0; i < triData.Length; i++)
                        {
                            // exit for loop if last vertex has been read out
                            if (triData[i] == -1)
                                break;

                            float vertXPos = 0;
                            float vertYPos = 0;
                            float vertZPos = 0;
                            Color vertCol = new Color(255, 105, 180);
                            float interpolationVal = 0;
                            switch (triData[i])
                            {
                                case 0:
                                    interpolationVal = vertPos(in val, 0, 1);
                                    vertCol = vertColor(in val, interpolationVal, 0, 1);
                                    vertXPos = x * thread_density;
                                    vertYPos = y * thread_density;
                                    vertZPos = z * thread_density + interpolationVal * thread_density;
                                    break;
                                case 1:
                                    interpolationVal = vertPos(in val, 1, 2);
                                    vertCol = vertColor(in val, interpolationVal, 1, 2);
                                    vertXPos = x * thread_density + interpolationVal * thread_density;
                                    vertYPos = y * thread_density;
                                    vertZPos = z * thread_density + thread_density;
                                    break;
                                case 2:
                                    interpolationVal = vertPos(in val, 3, 2);
                                    vertCol = vertColor(in val, interpolationVal, 3, 2);
                                    vertXPos = x * thread_density + thread_density;
                                    vertYPos = y * thread_density;
                                    vertZPos = z * thread_density + interpolationVal * thread_density;
                                    break;
                                case 3:
                                    interpolationVal = vertPos(in val, 0, 3);
                                    vertCol = vertColor(in val, interpolationVal, 0, 3);
                                    vertXPos = x * thread_density + interpolationVal * thread_density;
                                    vertYPos = y * thread_density;
                                    vertZPos = z * thread_density;
                                    break;
                                case 4:
                                    interpolationVal = vertPos(in val, 4, 5);
                                    vertCol = vertColor(in val, interpolationVal, 4, 5);
                                    vertXPos = x * thread_density;
                                    vertYPos = y * thread_density + thread_density;
                                    vertZPos = z * thread_density + interpolationVal * thread_density;
                                    break;
                                case 5:
                                    interpolationVal = vertPos(in val, 5, 6);
                                    vertCol = vertColor(in val, interpolationVal, 5, 6);
                                    vertXPos = x * thread_density + interpolationVal * thread_density;
                                    vertYPos = y * thread_density + thread_density;
                                    vertZPos = z * thread_density + thread_density;
                                    break;
                                case 6:
                                    interpolationVal = vertPos(in val, 7, 6);
                                    vertCol = vertColor(in val, interpolationVal, 7, 6);
                                    vertXPos = x * thread_density + thread_density;
                                    vertYPos = y * thread_density + thread_density;
                                    vertZPos = z * thread_density + interpolationVal * thread_density;
                                    break;
                                case 7:
                                    interpolationVal = vertPos(in val, 4, 7);
                                    vertCol = vertColor(in val, interpolationVal, 4, 7);
                                    vertXPos = x * thread_density + interpolationVal * thread_density;
                                    vertYPos = y * thread_density + thread_density;
                                    vertZPos = z * thread_density;
                                    break;
                                case 8:
                                    interpolationVal = vertPos(in val, 0, 4);
                                    vertCol = vertColor(in val, interpolationVal, 0, 4);
                                    vertXPos = x * thread_density;
                                    vertYPos = y * thread_density + interpolationVal * thread_density;
                                    vertZPos = z * thread_density;
                                    break;
                                case 9:
                                    interpolationVal = vertPos(in val, 1, 5);
                                    vertCol = vertColor(in val, interpolationVal, 1, 5);
                                    vertXPos = x * thread_density;
                                    vertYPos = y * thread_density + interpolationVal * thread_density;
                                    vertZPos = z * thread_density + thread_density;
                                    break;
                                case 10:
                                    interpolationVal = vertPos(in val, 2, 6);
                                    vertCol = vertColor(in val, interpolationVal, 2, 6);
                                    vertXPos = x * thread_density + thread_density;
                                    vertYPos = y * thread_density + interpolationVal * thread_density;
                                    vertZPos = z * thread_density + thread_density;
                                    break;
                                case 11:
                                    interpolationVal = vertPos(in val, 3, 7);
                                    vertCol = vertColor(in val, interpolationVal, 3, 7);
                                    vertXPos = x * thread_density + thread_density;
                                    vertYPos = y * thread_density + interpolationVal * thread_density;
                                    vertZPos = z * thread_density;
                                    break;
                            }
                            vertPosition = new Vector3(vertXPos, vertYPos, vertZPos);


                            vertices.Add(vertPosition);
                            colors.Add(vertCol);
                            vert = vertices.Count - 1;

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

            thread_vertices = vertices;
            thread_colors = colors;
            thread_triangles = triangles;
        }

        private float vertPos(in GridPoint[] val, int from, int to)
        {
            return -((thread_surflevel - val[from].v) / (val[from].v - val[to].v));
        }

        private Color vertColor(in GridPoint[] val, float position, int from, int to)
        {
            return Color.Lerp(val[from].c, val[to].c, position);
        }

        private Thread generatingThread;
        public void GenerateMesh(float surflevel, float density, bool useCollider)
        {
            if (surflevel > 1)
                throw new Exception(greaterErrorMessage);
            else if (surflevel < 0)
                throw new Exception(lessErrorMessage);

            if (generatingThread != null)
            {
                if (generatingThread.IsAlive)
                {
                    generatingThread.Abort();
                }
            }

            if (waiter != null)
            {
                StopCoroutine(waiter);
            }

            thread_mesh = new Mesh();
            thread_surflevel = surflevel;
            thread_density = density;
            generatingThread = new Thread(new ThreadStart(GenerateMeshThreaded));
            generatingThread.Start();
            waiter = StartCoroutine(ThreadWaiter(useCollider));
        }

        private Coroutine waiter;

        IEnumerator ThreadWaiter(bool useCollider)
        {
            while (generatingThread.IsAlive) yield return null;

            thread_mesh.SetVertices(thread_vertices);
            thread_mesh.SetColors(thread_colors);
            thread_mesh.SetTriangles(thread_triangles, 0);
            Vector2[] uvs = new Vector2[thread_vertices.Count];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(thread_vertices[i].x, thread_vertices[i].z);
            }
            thread_mesh.uv = uvs;
            thread_mesh.RecalculateNormals();
            thread_mesh.name = "Generated Mesh " + position.x + ":" + position.y + ":" + position.z;

            GetComponent<MeshFilter>().mesh = thread_mesh;
            if (useCollider)
            {
                GetComponent<MeshCollider>().sharedMesh = thread_mesh;
            }
        }

        public void ClearMesh()
        {
            GetComponent<MeshFilter>().mesh = null;
            if (GetComponent<MeshCollider>())
                GetComponent<MeshCollider>().sharedMesh = null;
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        values[x, y, z].v = 0f;
                    }
                }
            }
        }
    }
}