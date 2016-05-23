using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;
using System;

public class CustomMaterialEditor : ShaderGUI
{

    public enum BlendMode
    {
        First = 0,
        Background = 1000,
        Opaque = 2000,
        Fade = 3000,       // Old school alpha-blending mode, fresnel does not affect amount of transparency
        Transparent = 4000 // Physically plausible transparency mode, implemented as alpha pre-multiply
    }
    MaterialEditor m_MaterialEditor;
    Material m_Material;

    public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));


    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);

        m_MaterialEditor = materialEditor;
        m_Material = materialEditor.target as Material;

        RenderQueueRange();
    }

    void RenderQueueRange()
    {
        int mode = m_Material.renderQueue / 1000;
        int sort = m_Material.renderQueue % 1000;

        EditorGUI.BeginChangeCheck();
        mode = EditorGUILayout.Popup("Layer", mode, blendNames);
        sort = EditorGUILayout.IntField("Order", sort);

        if (EditorGUI.EndChangeCheck())
        {
            m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
            m_Material.renderQueue = mode * 1000 + sort;
        }

        EditorGUI.showMixedValue = false;
    }
}