using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MobileBlur : MonoBehaviour
{
    public Material blurMat;
    public Material blendMat;

    public float intensity = 1.0f;
    public float spread = 3.0f;

    private Camera m_Cam;

    void Awake()
    {
        m_Cam = Camera.main;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (blurMat == null || blendMat == null) return;

        RenderTexture buffer1 = RenderTexture.GetTemporary(src.width/4, src.height/4);
        RenderTexture buffer2 = RenderTexture.GetTemporary(src.width/4, src.height/4);

        //Blur
        float offset = spread / 100f;
        blurMat.SetVector("offset", new Vector4(0.0f, offset, 0.0f, 0.0f));
        Graphics.Blit(src, buffer1, blurMat);
        /*blurMat.SetVector("offset", new Vector4(offset, 0.0f, 0.0f, 0.0f));
        Graphics.Blit(buffer1, buffer2, blurMat);
        blurMat.SetVector("offset", new Vector4(0.0f, offset, 0.0f, 0.0f));
        Graphics.Blit(buffer2, buffer1, blurMat);
        blurMat.SetVector("offset", new Vector4(offset, 0.0f, 0.0f, 0.0f));
        Graphics.Blit(buffer1, buffer2, blurMat);*/

        //blending
        blendMat.SetFloat("intensity", intensity);
        Graphics.Blit(buffer2, dst, blendMat);

        RenderTexture.ReleaseTemporary(buffer1);
        RenderTexture.ReleaseTemporary(buffer2);
    }
}
