using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public abstract class MeshEffectBase : MonoBehaviour {
    public Material m_Material;

    public int m_ScreenGridXRes = 30;
    public int m_ScreenGridYRes = 25;

    protected Camera m_Camera;
    protected Mesh m_RenderMesh;

    private GameObject m_GameObj;

    private void Start()
    {
        m_Camera = GetComponent<Camera>();

        //create mesh
        createMesh();
    }

    private void createMesh()
    {
        m_GameObj = new GameObject();

        MeshFilter mf = m_GameObj.AddComponent<MeshFilter>();
        MeshRenderer mr = m_GameObj.AddComponent<MeshRenderer>();

        mr.material = m_Material;
        mr.enabled = true;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
        mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mr.motionVectors = false;
        mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        m_RenderMesh = mf.mesh;

        int numVerts = m_ScreenGridXRes * m_ScreenGridYRes;
        int numTris = (m_ScreenGridXRes - 1) * (m_ScreenGridYRes - 1) * 2;

        Vector3[] verts = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts]; // we fill UVs even if it is not used by shader to make Unity happy
        int[] tris = new int[numTris * 3];

        for (int y = 0; y < m_ScreenGridYRes; y++)
        {
            for (int x = 0; x < m_ScreenGridXRes; x++)
            {
                int idx = y * m_ScreenGridXRes + x;

                verts[idx].x = (float)x / (m_ScreenGridXRes - 1);
                verts[idx].y = (float)y / (m_ScreenGridYRes - 1);
                verts[idx].z = 0;

                uv[idx].x = verts[idx].x;
                uv[idx].y = verts[idx].y;
            }
        }

        int currIdx = 0;

        for (int y = 0; y < m_ScreenGridYRes - 1; y++)
        {
            for (int x = 0; x < m_ScreenGridXRes - 1; x++)
            {
                // 0   1
                // +---+
                // |   |
                // +---+
                // 3   2

                int i0 = x + y * m_ScreenGridXRes;
                int i1 = (x + 1) + y * m_ScreenGridXRes;
                int i2 = (x + 1) + (y + 1) * m_ScreenGridXRes;
                int i3 = x + (y + 1) * m_ScreenGridXRes;

                tris[currIdx++] = i3;
                tris[currIdx++] = i1;
                tris[currIdx++] = i0;

                tris[currIdx++] = i3;
                tris[currIdx++] = i2;
                tris[currIdx++] = i1;
            }
        }

        m_RenderMesh.vertices = verts;
        m_RenderMesh.uv = uv;
        m_RenderMesh.triangles = tris;

        m_GameObj.SetActive(false);
    }

    protected abstract void OnRenderImage(RenderTexture src, RenderTexture dest);
}
