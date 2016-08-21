using UnityEngine;
public class VertexBlur : MeshEffectBase {
    protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
    {

        /*Vector4 uvOffs = Vector4.zero;

        uvOffs.x = 0.5f / src.width;
        uvOffs.y = 0.5f / src.height;
        uvOffs.z = 1;
        uvOffs.w = (float)src.height / src.width;*/

        m_Material.mainTexture = src;
        //m_Material.SetVector("_UVOffsAndAspectScale", uvOffs);

        if (m_Material.SetPass(0))
            Graphics.DrawMeshNow(m_RenderMesh, Matrix4x4.identity);

        Graphics.Blit(m_Material.mainTexture, dest, m_Material);
    }
}
