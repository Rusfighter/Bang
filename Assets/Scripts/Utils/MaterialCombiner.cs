using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;

public class MaterialCombiner : MonoBehaviour {

    public class SubMeshContainer
    {
        public MeshFilter mf;
        public int idx;

        public SubMeshContainer(MeshFilter m, int idx)
        {
            mf = m;
            this.idx = idx;
        }
    }

    public class MeshData
    {
        public Vector3 vertice;
        public Vector2 uv;
        public Vector2 uv2;

        public MeshData(Vector3 vertice, Vector2 uv)
        {
            this.vertice = vertice;
            this.uv = uv;
        }
    }

    public class Triangle
    {
        public MeshData v1;
        public MeshData v2;
        public MeshData v3;

        public Triangle(MeshData v1, MeshData v2, MeshData v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    public class TextureTriangles
    {
        public Material material;
        public int[] triangles;
        public Mesh mesh;
        public MeshRenderer renderer;
        public int subMeshIdx;
    }

    public class Grid<T>
    {
        private List<T>[,,] items;

        public Grid(int width, int height, int length)
        {
            items = new List<T>[width, height, length];
        }

        public void Add(T t, int x, int y, int z)
        {
            if (items[x, y, z] == null)
                items[x, y, z] = new List<T>();


            items[x, y, z].Add(t);
        }

        public List<List<T>> getList()
        {
            List<List<T>> t = new List<List<T>>();

            for (int i = 0; i < items.GetLength(0); i++)
                for (int j = 0; j < items.GetLength(1); j++)
                    for (int k = 0; k < items.GetLength(2); k++)
                        if (items[i, j, k] != null)
                            t.Add(items[i, j, k]);
            return t;
        }
    }


    public int m_MaxVerticesPerObject = 60000;
    public Vector3 m_GridSize = new Vector3(1, 1, 1);

    public void EnableAllMf()
    {
        gameObject.SetActiveRecursively(true);
    }

    public void DeepCloneMeshes()
    {
        GameObject clone = new GameObject(gameObject.name + "_clone");
        clone.transform.parent = transform.parent;
        MeshFilter[] mfs = GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter mf in mfs)
        {
            GameObject go = DeepCopy(mf.gameObject);
            go.transform.parent = clone.transform;

            go.transform.position = mf.transform.position;
            go.transform.rotation = mf.transform.rotation;
            go.transform.localScale = mf.transform.lossyScale;

            mf.gameObject.SetActive(false);
        }
    }

    public void CombineMeshes()
    {
        if (m_MaxVerticesPerObject % 3 != 0)
        {
            m_MaxVerticesPerObject -= m_MaxVerticesPerObject % 3;
        }

        Grid<MeshFilter> grid = PlaceObjectsInGrid(gameObject, (int)m_GridSize.x, (int)m_GridSize.z, (int)m_GridSize.y);

        int j = 0;
        GameObject clone = gameObject;
        //each grid item
        foreach (List<MeshFilter> list in grid.getList())
        {
            j++;
            GameObject g = new GameObject("Grid_"+j);
            g.transform.parent = clone.transform;
           
            foreach (MeshFilter mf in list)
                if (mf != null) mf.gameObject.transform.parent = g.transform;


            CombineMeshes(g, m_MaxVerticesPerObject);
            g.isStatic = true;

        }
    }
    
    public void CombineTextures()
    {
        CombineTexturesWithSameMaterial(gameObject);
    }

    private static Grid<MeshFilter> PlaceObjectsInGrid(GameObject parent, int width, int length, int height)
    {
        MeshFilter[] mfs = parent.GetComponentsInChildren<MeshFilter>();
        Grid<MeshFilter> grid = new Grid<MeshFilter>(width, height, length);

        if (mfs.Length == 0) return grid;

        Vector3 minimums = mfs[0].transform.position;
        Vector3 maximums = minimums;

        foreach (MeshFilter mf in mfs)
        {
            Vector3 position = mf.transform.position;

            if (position.x <= minimums.x) minimums = new Vector3(position.x, minimums.y, minimums.z);
            if (position.y <= minimums.y) minimums = new Vector3(minimums.x, position.y, minimums.z);
            if (position.z <= minimums.z) minimums = new Vector3(minimums.x, minimums.y, position.z);

            if (position.x >= maximums.x) maximums = new Vector3(position.x + 0.1f, maximums.y, maximums.z);
            if (position.y >= maximums.y) maximums = new Vector3(maximums.x, position.y + 0.1f, maximums.z);
            if (position.z >= maximums.z) maximums = new Vector3(maximums.x, maximums.y, position.z + 0.1f);
        }

        float gridWidth = (maximums.x - minimums.x) / (width);
        float gridLength = (maximums.z - minimums.z) / (length);
        float gridHeight = (maximums.y - minimums.y) / (height);


        gridWidth = gridWidth != 0 ? gridWidth : float.MaxValue;
        gridLength = gridLength != 0 ? gridLength : float.MaxValue;
        gridHeight = gridHeight != 0 ? gridHeight : float.MaxValue;


        foreach (MeshFilter mf in mfs)
        {
            Vector3 position = mf.transform.position;

            int gridX, gridZ, gridY;

            gridX = Mathf.FloorToInt((position.x - minimums.x) / gridWidth);
            gridY = Mathf.FloorToInt((position.y - minimums.y) / gridHeight);
            gridZ = Mathf.FloorToInt((position.z - minimums.z) / gridLength);
            grid.Add(mf, gridX, gridY, gridZ);
        }

        return grid;
    }

    public static void CombineTexturesWithSameMaterial(GameObject gameObject)
    {
        Dictionary<Shader, List<TextureTriangles>> shaders = new Dictionary<Shader, List<TextureTriangles>>();

        MeshFilter[] mfs = gameObject.GetComponentsInChildren<MeshFilter>();

        //get materials with different shader
        foreach (MeshFilter mf in mfs)
        {
            Mesh m = mf.sharedMesh;
            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            for (int i = 0; i < m.subMeshCount; i++)
            {
                Material material = mr.sharedMaterials[i];
                List<TextureTriangles> temp;
                TextureTriangles data = new TextureTriangles();
                data.material = material;
                data.mesh = m;
                data.triangles = m.GetTriangles(i);
                data.renderer = mr;
                data.subMeshIdx = i;

                if (shaders.TryGetValue(material.shader, out temp))
                {
                    temp.Add(data);
                }
                else
                {
                    temp = new List<TextureTriangles>();
                    temp.Add(data);
                    shaders.Add(material.shader, temp);
                }
            }
        }

        foreach (KeyValuePair<Shader, List<TextureTriangles>> entry in shaders)
        {
            //sort by texture
            Dictionary<Texture2D, List<TextureTriangles>> textures = new Dictionary<Texture2D, List<TextureTriangles>>();

            for (int i = 0; i < entry.Value.Count; i++)
            {
                TextureTriangles triangles = entry.Value[i];
                Texture2D tex = (Texture2D)triangles.material.mainTexture;

                List<TextureTriangles> obj;

                //already some rect
                if (textures.TryGetValue(tex, out obj))
                {
                    obj.Add(triangles);
                }
                else
                {
                    obj = new List<TextureTriangles>();
                    obj.Add(triangles);
                    textures.Add(tex, obj);
                }
            }

            //the batch for texture
            List<Texture2D> textureBatch = new List<Texture2D>();
            //objects for the texture that is batched
            List<List<TextureTriangles>> objectsBatch = new List<List<TextureTriangles>>();

            int maxBatch = 2048 * 2048;
            int batchSize = maxBatch;

            foreach (KeyValuePair<Texture2D, List<TextureTriangles>> entryT in textures)
            {
                Texture2D tex = entryT.Key;
                int size = tex.width * tex.height;
                if (size < maxBatch)
                {
                    batchSize -= tex.width * tex.height;
                    textureBatch.Add(tex);
                    objectsBatch.Add(entryT.Value);
                }

                if (batchSize <= 0)
                {
                    //combine
                    MergeTextures(textureBatch, objectsBatch, entry.Key);
                    batchSize = maxBatch;
                }
            }

            //combine left overs
            MergeTextures(textureBatch, objectsBatch, entry.Key);
        }
    }

    private static Texture2D ExportTexture(Texture2D tex, string path)
    {
        File.WriteAllBytes(path, tex.EncodeToPNG());
        DestroyImmediate(tex);
        AssetDatabase.Refresh();
        return (Texture2D)(AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)));
    }

    private static void MergeTextures(List<Texture2D> textureBatch, List<List<TextureTriangles>> objectsBatch, Shader shader)
    {
        if (textureBatch.Count < 2) return;
        Texture2D combinedTexture = new Texture2D(2048, 2048);

        //make textures readable
        foreach (Texture2D texture in textureBatch)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
            ti.isReadable = true;
            ti.textureFormat = TextureImporterFormat.ARGB32;
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }


        Rect[] rects = combinedTexture.PackTextures(textureBatch.ToArray(), 2);
        Debug.Log("created atlas");

        //string filePath = Application.dataPath + "/" + System.DateTime.UtcNow.Millisecond + "_test.png";
        //File.WriteAllBytes(filePath, combinedTexture.EncodeToPNG());
        Texture2D finalTex = ExportTexture(combinedTexture, "Assets/"+System.DateTime.UtcNow.Millisecond + "_test.png");

        Material newMaterial = new Material(shader);
        newMaterial.mainTexture = finalTex;

        for (int i = 0; i < rects.Length; i++)
        {
            Rect rect = rects[i];

            foreach (TextureTriangles t in objectsBatch[i])
            {
                Vector2[] newUvs = t.mesh.uv;
                bool isTilled = false;
                foreach (int j in t.triangles)
                    if (newUvs[j].x < 0 || newUvs[j].y < 0 || newUvs[j].x > 1 || newUvs[j].y > 1)
                        isTilled = true;

                //if (isTilled) continue;

                foreach (int j in t.triangles)
                {
                    newUvs[j] = new Vector2((t.mesh.uv[j].x * rect.width) + rect.x, (t.mesh.uv[j].y * rect.height) + rect.y);
                }

                Material[] newMaterials = t.renderer.sharedMaterials;
                newMaterials[t.subMeshIdx] = newMaterial;

                t.renderer.sharedMaterials = newMaterials;
                t.mesh.uv = newUvs;
            }
        }

        textureBatch.Clear();
        objectsBatch.Clear();
    }

    private static void CombineMeshes(GameObject parent, int maxSize)
    {
        Dictionary<Material, List<SubMeshContainer>> dictionary = getMeshesPerMaterials(parent);

        foreach (KeyValuePair<Material, List<SubMeshContainer>> entry in dictionary)
        {
            List<Triangle> allTriangles = new List<Triangle>();
            List<SubMeshContainer> l = entry.Value;

            foreach (SubMeshContainer container in l)
            {
                Mesh mesh = container.mf.sharedMesh;
                int[] triangles = mesh.GetTriangles(container.idx);

                List<MeshData> data = new List<MeshData>();

                foreach (int idx in triangles)
                    data.Add(new MeshData(container.mf.transform.localToWorldMatrix.MultiplyPoint(mesh.vertices[idx]), mesh.uv[idx]));

                for (int i = 0; i < data.Count; i += 3)
                {
                    MeshData v1 = data[i];
                    MeshData v2 = data[i + 1];
                    MeshData v3 = data[i + 2];
                    Triangle triangle = new Triangle(v1, v2, v3);
                    allTriangles.Add(triangle);
                }
            }



            while (allTriangles.Count > 0)
            {
                List<Triangle> triangles = new List<Triangle>();
                for (int i = allTriangles.Count - 1; i >= 0; i--)
                {
                    Triangle t = allTriangles[i];
                    triangles.Add(t);
                    allTriangles.RemoveAt(i);

                    if (triangles.Count > maxSize / 3)
                        break;
                }
                createMesh(triangles, entry.Key, parent);
            }
        }

        foreach (KeyValuePair<Material, List<SubMeshContainer>> entry in dictionary)
        {
            foreach (SubMeshContainer m in entry.Value)
                if (m.mf != null) DestroyImmediate(m.mf.gameObject);
        }
    }

    private static void createMesh(List<Triangle> triangles, Material material, GameObject parent)
    {
        List<Vector3> tempVert = new List<Vector3>();
        List<Vector2> tempUv = new List<Vector2>();
        //List<Vector2> tempUv2 = new List<Vector2>();

        foreach (Triangle triangle in triangles)
        {
            tempVert.Add(triangle.v1.vertice);
            tempVert.Add(triangle.v2.vertice);
            tempVert.Add(triangle.v3.vertice);

            tempUv.Add(triangle.v1.uv);
            tempUv.Add(triangle.v2.uv);
            tempUv.Add(triangle.v3.uv);

            /*tempUv2.Add(triangle.v1.uv2);
            tempUv2.Add(triangle.v2.uv2);
            tempUv2.Add(triangle.v3.uv2);*/
        }

        int[] tris = new int[tempVert.Count];
        for (int i = 0; i < tris.Length; i++)
        {
            tris[i] = i;
        }

        Mesh newMesh = new Mesh();

        newMesh.vertices = tempVert.ToArray();
        newMesh.triangles = tris;
        newMesh.uv = tempUv.ToArray();
        //newMesh.uv2 = tempUv2.ToArray();
        Unwrapping.GenerateSecondaryUVSet(newMesh);

        newMesh.Optimize();
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();


        //create new object
        GameObject newObject = new GameObject(material.name + "(clone + merged)");
        newObject.transform.parent = parent.transform;
        newObject.AddComponent<MeshRenderer>().sharedMaterial = material;
        newObject.AddComponent<MeshFilter>().mesh = newMesh;
        newObject.isStatic = true;
    }

    private static Dictionary<Material, List<SubMeshContainer>> getMeshesPerMaterials(GameObject parent)
    {
        Dictionary<Material, List<SubMeshContainer>> materials = new Dictionary<Material, List<SubMeshContainer>>();
        MeshFilter[] mfs = parent.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter mf in mfs)
        {
            Mesh m = mf.sharedMesh;
            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            for (int i = 0; i< m.subMeshCount;i++)
            {
                Material material = mr.sharedMaterials[i];

                List<SubMeshContainer> temp = null;
                SubMeshContainer container = new SubMeshContainer(mf,i);

                if (materials.TryGetValue(material, out temp)){
                    temp.Add(container);
                } else {
                    temp = new List<SubMeshContainer>();
                    temp.Add(container);
                    materials.Add(material, temp);
                }
            }
        }

        return materials;
    }

    public static GameObject DeepCopy(GameObject subject)
    {
        GameObject clone = Instantiate(subject);

        MeshCollider clonemc = clone.GetComponent<MeshCollider>();

        MeshFilter mf = clone.GetComponent<MeshFilter>();
        mf.sharedMesh = Instantiate(subject.GetComponent<MeshFilter>().sharedMesh);

        if (clonemc != null)
            clonemc.sharedMesh = clone.GetComponent<MeshFilter>().sharedMesh;

        return clone;

    }
}

#endif