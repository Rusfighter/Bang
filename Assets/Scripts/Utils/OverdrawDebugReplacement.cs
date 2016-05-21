using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class OverdrawDebugReplacement : MonoBehaviour
{
    public Shader _OverdrawShader;

    private Camera _Camera;
    private bool _SceneFogSettings = false;

    void OnEnable()
    {
        _SceneFogSettings = RenderSettings.fog;
        RenderSettings.fog = false;

        // not set in the editor inspector
        if (_OverdrawShader == null)
        {
            // It must be added on Project Settings -> Graphics -> Always Include Shader if you want to see it on the build.
            _OverdrawShader = Shader.Find("Custom/OverdrawDebugReplacement");
        }

        _Camera = GetComponent<Camera>();

        if (_OverdrawShader != null && _Camera != null)
        {
            RenderSettings.fog = false;
            Camera camera = GetComponent<Camera>();
            camera.SetReplacementShader(_OverdrawShader, "");
        }
        else
        {
            Debug.LogWarning("Can't use OverdrawDebugReplace. Check if script is attached to a camera object.");
        }
    }

    void OnDisable()
    {
        if (_Camera != null)
        {
            RenderSettings.fog = _SceneFogSettings;
            GetComponent<Camera>().ResetReplacementShader();
        }
    }
}