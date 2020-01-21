using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MarchingCubes
{
    [CustomEditor(typeof(MarchingCubesTerrain))]
    public class MarchingCubesTerrainEditor : Editor
    {
        const string createMenuLocation = "GameObject/3D Object/Marching cubes Terrain";
        const string newTerrainName = "new Voxel Terrain";
        MarchingCubesTerrain mct;

        int selectedOption = 0;
        const int TOOLS_MESH_ID = 0;
        const int TOOLS_FOLIAGE_ID = 1;
        const int TOOLS_CHUNK_ID = 2;
        const int TOOLS_SAVELOAD_ID = 3;
        const int TOOLS_SETTINGS_ID = 4;

        static GameObject currFoliageObj;
        static bool randomRotation = true;

        Vector3Int addChunkPosition = new Vector3Int(0, 0, 0);
        Texture[] textures;
        static bool drawDebugField = false;
        static bool prevDrawDebugField = false;
        static float prevSurfLevel = 0;

        static float brushSize = 1.0f;
        static float brushOpacity = 0.5f;
        static float brushDistance = 5.0f;
        static Color brushColor = new Color(0, 1, 0);
        static bool generateWhileEditing = false;
        static BrushType selectedBrush;

        private void OnEnable()
        {
            //get target reference
            mct = (MarchingCubesTerrain)target;

            //INIT Toolbar
            textures = new Texture[5];
            textures[TOOLS_MESH_ID] = Resources.Load<Texture>("MarchingCubes/Mesh");
            textures[TOOLS_FOLIAGE_ID] = Resources.Load<Texture>("MarchingCubes/Foliage");
            textures[TOOLS_CHUNK_ID] = Resources.Load<Texture>("MarchingCubes/Chunk");
            textures[TOOLS_SAVELOAD_ID] = Resources.Load<Texture>("MarchingCubes/File");
            textures[TOOLS_SETTINGS_ID] = Resources.Load<Texture>("MarchingCubes/Settings");
        }

        public enum BrushType
        {
            none,
            mesh,
            color
        }

        public override void OnInspectorGUI()
        {
            selectedOption = GUILayout.Toolbar(selectedOption, textures, GUILayout.Width(40 * textures.Length), GUILayout.Height(40));

            if (selectedOption == TOOLS_MESH_ID)
            {
                //mesh tools
                mct.surfaceLevel = EditorGUILayout.Slider("Surface Level", mct.surfaceLevel, 0, 1);
                if (prevSurfLevel != mct.surfaceLevel)
                {
                    prevSurfLevel = mct.surfaceLevel;
                    mct.Generate();
                }
                mct.density = EditorGUILayout.Slider("Density", mct.density, 1, mct.density + 1);

                //brush Tools
                selectedBrush = (BrushType)EditorGUILayout.EnumPopup("Brush Type", selectedBrush);
                if (selectedBrush == BrushType.color)
                {
                    brushColor = EditorGUILayout.ColorField("Brush Color", brushColor);
                }
                brushSize = EditorGUILayout.FloatField("Brush Size", brushSize);
                brushDistance = EditorGUILayout.FloatField("Brush Distance", brushDistance);
                brushOpacity = EditorGUILayout.FloatField("Brush Opacity", brushOpacity);

                generateWhileEditing = EditorGUILayout.Toggle("Generate Mesh while editing", generateWhileEditing);

                //mesh buttons
                if (GUILayout.Button("Generate Mesh"))
                {
                    mct.Generate();
                }
                if (GUILayout.Button("Clear Mesh"))
                {
                    mct.ClearMesh();
                }
            }
            else if (selectedOption == TOOLS_FOLIAGE_ID)
            {
                //Foliage tools
                currFoliageObj = (GameObject)EditorGUILayout.ObjectField("", currFoliageObj, typeof(GameObject));
                randomRotation = GUILayout.Toggle(randomRotation, "random rotation");
            }
            else if (selectedOption == TOOLS_CHUNK_ID)
            {
                //chunk tools
                addChunkPosition = EditorGUILayout.Vector3IntField("add chunk position", addChunkPosition);
                if (GUILayout.Button("Add Chunk"))
                {
                    mct.AddChunk(addChunkPosition);
                }
                if (GUILayout.Button("Remove Chunk"))
                {
                    mct.EradicateChunk(addChunkPosition);
                }
            }
            else if (selectedOption == TOOLS_SAVELOAD_ID)
            {
                //save load
                mct.saveFile = EditorGUILayout.TextField("File path", mct.saveFile);
                if (GUILayout.Button("Save"))
                {
                    mct.Save(mct.saveFile);
                }
                if (GUILayout.Button("Load"))
                {
                    mct.Load(mct.saveFile);
                }
            }
            else if (selectedOption == TOOLS_SETTINGS_ID)
            {
                //terrain settings
                drawDebugField = EditorGUILayout.Toggle("Draw Debug points", drawDebugField);
                if (drawDebugField != prevDrawDebugField)
                {
                    EditorWindow.GetWindow<SceneView>().Repaint();
                    prevDrawDebugField = drawDebugField;
                }
            }
        }

        private void OnSceneGUI()
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            MarchingCubesChunk cc;
            Vector3 pos;
            for (int i = 0; i < mct.chunks.Count; i++)
            {
                cc = mct.chunks[i];
                pos = cc.transform.position;
                Handles.color = Color.red;
                if (cc.neighbours[1] == null && cc.neighbours[5] == null)
                    Handles.DrawLine(Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size)), Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size)));
                if (cc.neighbours[3] == null && cc.neighbours[5] == null)
                    Handles.DrawLine(Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size)), Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size)));
                if (cc.neighbours[3] == null && cc.neighbours[1] == null)
                    Handles.DrawLine(Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size)), Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size + MarchingCubesChunk.size)));
                if (cc.neighbours[0] == null && cc.neighbours[3] == null)
                    Handles.DrawLine(Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size)), Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size + MarchingCubesChunk.size)));
                if (cc.neighbours[0] == null && cc.neighbours[5] == null)
                    Handles.DrawLine(Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size)), Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size)));
                if (cc.neighbours[2] == null && cc.neighbours[5] == null)
                    Handles.DrawLine(Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size)), Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size)));
                if (cc.neighbours[2] == null && cc.neighbours[1] == null)
                    Handles.DrawLine(Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size)), Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size + MarchingCubesChunk.size)));
                if (cc.neighbours[4] == null && cc.neighbours[3] == null)
                    Handles.DrawLine(Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size + MarchingCubesChunk.size)), Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size + MarchingCubesChunk.size)));
                if (cc.neighbours[4] == null && cc.neighbours[1] == null)
                    Handles.DrawLine(Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size + MarchingCubesChunk.size)), Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size + MarchingCubesChunk.size)));
                if (cc.neighbours[2] == null && cc.neighbours[0] == null)
                    Handles.DrawLine(Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size + MarchingCubesChunk.size)), Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size)));
                if (cc.neighbours[4] == null && cc.neighbours[2] == null)
                    Handles.DrawLine(Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size + MarchingCubesChunk.size)), Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size + MarchingCubesChunk.size)));
                if (cc.neighbours[4] == null && cc.neighbours[0] == null)
                    Handles.DrawLine(Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size + MarchingCubesChunk.size)), Vector3.Scale(mct.transform.localScale, mct.transform.position + new Vector3(cc.position.x * MarchingCubesChunk.size + MarchingCubesChunk.size, cc.position.y * MarchingCubesChunk.size, cc.position.z * MarchingCubesChunk.size + MarchingCubesChunk.size)));

                if (drawDebugField)
                {
                    for (int x = 0; x < cc.xLength; x++)
                    {
                        for (int y = 0; y < cc.yLength; y++)
                        {
                            for (int z = 0; z < cc.zLength; z++)
                            {
                                Handles.color = new Color(255 - cc.values[x, y, z].v * 255, 255 - cc.values[x, y, z].v * 255, 255 - cc.values[x, y, z].v * 255);
                                Handles.SphereHandleCap(0, new Vector3(x * mct.density + cc.position.x * MarchingCubesChunk.size, y * mct.density + cc.position.y * MarchingCubesChunk.size, z * mct.density + cc.position.z * MarchingCubesChunk.size), Quaternion.identity, 0.1f, EventType.Repaint);
                            }
                        }
                    }
                }
            }
            Handles.color = Color.white;
            Handles.SphereHandleCap(0, mct.transform.position, Quaternion.identity, 1, EventType.Repaint);



            if (selectedOption == TOOLS_MESH_ID && selectedBrush != BrushType.none)
            {
                int controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);

                Event e = Event.current;
                Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                if (e.type == EventType.Layout)
                {
                    HandleUtility.AddDefaultControl(controlID);
                }

                if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
                {
                    Vector3 brushPosition = worldRay.GetPoint(brushDistance);

                    // MESH BRUSH
                    if (selectedBrush == BrushType.mesh)
                    {
                        for (int c = 0; c < mct.chunks.Count; c++)
                        {
                            if (Vector3.Distance(mct.chunks[c].position * MarchingCubesChunk.size, brushPosition) < brushSize + MarchingCubesChunk.size * 1.6f)
                            {
                                for (int x = 0; x < mct.chunks[c].xLength; x++)
                                {
                                    for (int y = 0; y < mct.chunks[c].yLength; y++)
                                    {
                                        for (int z = 0; z < mct.chunks[c].zLength; z++)
                                        {
                                            float dist = Vector3.Distance(new Vector3(x * mct.density + mct.chunks[c].position.x * MarchingCubesChunk.size, y * mct.density + mct.chunks[c].position.y * MarchingCubesChunk.size, z * mct.density + mct.chunks[c].position.z * MarchingCubesChunk.size), brushPosition);
                                            if (dist < brushSize/2)
                                            {
                                                mct.chunks[c].values[x, y, z].v += (-(dist-brushSize)/brushSize) * brushOpacity * Time.deltaTime;
                                                if (mct.chunks[c].values[x, y, z].v > 1)
                                                {
                                                    mct.chunks[c].values[x, y, z].v = 1;
                                                }
                                                else if (mct.chunks[c].values[x, y, z].v < 0)
                                                {
                                                    mct.chunks[c].values[x, y, z].v = 0;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (generateWhileEditing)
                            mct.Generate();
                    }

                    // COLOR BRUSH
                    else if (selectedBrush == BrushType.color)
                    {
                        for (int c = 0; c < mct.chunks.Count; c++)
                        {
                            if (Vector3.Distance(mct.chunks[c].position * MarchingCubesChunk.size, brushPosition) < brushSize + MarchingCubesChunk.size * 1.6f)
                            {
                                for (int x = 0; x < mct.chunks[c].xLength; x++)
                                {
                                    for (int y = 0; y < mct.chunks[c].yLength; y++)
                                    {
                                        for (int z = 0; z < mct.chunks[c].zLength; z++)
                                        {
                                            float dist = Vector3.Distance(new Vector3(x * mct.density + mct.chunks[c].position.x * MarchingCubesChunk.size, y * mct.density + mct.chunks[c].position.y * MarchingCubesChunk.size, z * mct.density + mct.chunks[c].position.z * MarchingCubesChunk.size), brushPosition);
                                            if (dist < brushSize / 2)
                                            {
                                                mct.chunks[c].values[x, y, z].c = brushColor;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (generateWhileEditing)
                            mct.Generate();
                    }
                    e.Use();
                }
                if (e.type == EventType.Repaint)
                {
                    Handles.color = new Color(0, 0, 255, 1f);
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
                    Handles.SphereHandleCap(0, worldRay.GetPoint(brushDistance), Quaternion.identity, brushSize, EventType.Repaint);
                }
            }
        }

        [MenuItem(createMenuLocation)]
        public static void CreateVoxelTerrain()
        {
            GameObject terrainGo = new GameObject();
            terrainGo.name = newTerrainName;
            terrainGo.transform.position = new Vector3(0, 0, 0);
            MarchingCubesTerrain terrain = terrainGo.AddComponent<MarchingCubesTerrain>();

            EditorUtility.SetDirty(terrain.gameObject);
            terrain.AddChunk(new Vector3Int(0, 0, 0));

            Selection.activeObject = terrainGo;

            terrain.Generate();
        }
    }
}