using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ImageEffectOptimalization : MonoBehaviour {
    [Range(1, 3)]
    public float m_Factor = 1;
    public bool m_On = true;

    private RenderTexture m_tex;
    private Camera m_camera;

    void Awake()
    {
        m_camera = GetComponent<Camera>();
    }

    void OnPreRender()
    {
        if (!m_On) return;
        m_tex = RenderTexture.GetTemporary((int)(m_camera.pixelWidth / m_Factor), (int)(m_camera.pixelHeight /m_Factor));
        m_camera.targetTexture = m_tex;
    }

    void OnPostRender()
    {
        if (!m_On) return;
        m_camera.targetTexture = null;
        RenderTexture.ReleaseTemporary(m_tex);
    }
}
