using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Blur : MonoBehaviour {

    public Material m_Material;

    [Range(0f, 1f), Tooltip("Blur strength.")]
    public float Strength = 0.1f;

    [Tooltip("Focus point.")]
    public Vector2 Center = new Vector2(0.5f, 0.5f);

    [Range(1, 3)]
    public float m_Factor = 1;

    private RenderTexture m_tex;
    private Camera m_camera;

    void Awake()
    {
        m_camera = GetComponent<Camera>();
    }

    void OnPreRender()
    {
        m_tex = RenderTexture.GetTemporary((int)(m_camera.pixelWidth / m_Factor), (int)(m_camera.pixelHeight / m_Factor), 16);
        m_camera.targetTexture = m_tex;
    }

    void OnPostRender()
    {
        m_camera.targetTexture = null;
        if (Strength <= 0f && m_Material == null)
        {
            Graphics.Blit(m_tex, (RenderTexture)null);
        }
        else
        {

            m_Material.SetVector("_Center", Center);
            m_Material.SetVector("_Params", new Vector4(Strength, 0, 0, 0));


            Graphics.Blit(m_tex, null, m_Material);
        }

        RenderTexture.ReleaseTemporary(m_tex);

    }
}
