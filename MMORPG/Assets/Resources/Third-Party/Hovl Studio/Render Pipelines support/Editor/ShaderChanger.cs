using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class RPChanger : EditorWindow
{

    [MenuItem("Tools/RP changer for Hovl Studio assets")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RPChanger));
    }

    public void OnGUI()
    {
        GUILayout.Label("Change VFX pipeline to:");

        if (GUILayout.Button("Standard RP"))
        {
            FindShaders();
            ChangeToSRP();
        }
        if (GUILayout.Button("Lightweight RP"))
        {
            FindShaders();
            ChangeToLWRP();
        }
        if (GUILayout.Button("HD RP (From Unity 2018.3+)"))
        {
            FindShaders();
            ChangeToHDRP();
        }
    }

    Shader Add_CG;
    Shader Blend_CG;
    Shader LightGlow;
    Shader Lit_CenterGlow;
    Shader Blend_TwoSides;
    Shader Blend_Normals;
    Shader Ice;
    Shader Distortion;
    Shader ParallaxIce;
    Shader BlendDistort;
    Shader VolumeLaser;

    Shader Add_CG_URP;
    Shader Blend_CG_URP;
    Shader LightGlow_URP;
    Shader Lit_CenterGlow_URP;
    Shader Blend_TwoSides_URP;
    Shader Blend_Normals_URP;
    Shader Ice_URP;
    Shader Distortion_URP;
    Shader ParallaxIce_URP;
    Shader BlendDistort_URP;
    Shader VolumeLaser_URP;

    Shader Add_CG_HDRP;
    Shader Blend_CG_HDRP;
    Shader LightGlow_HDRP;
    Shader Lit_CenterGlow_HDRP;
    Shader Blend_TwoSides_HDRP;
    Shader Blend_Normals_HDRP;
    Shader Ice_HDRP;
    Shader Distortion_HDRP;
    Shader ParallaxIce_HDRP;
    Shader BlendDistort_HDRP;
    Shader VolumeLaser_HDRP;

    Material[] shaderMaterials;

    private void FindShaders()
    {
        //These 9 lines will be removed after updating all VFX assets from Hovl Studio. It can take 2 month...
        if (Shader.Find("ERB/Particles/Add_CenterGlow") != null) Add_CG = Shader.Find("ERB/Particles/Add_CenterGlow");
        if (Shader.Find("ERB/Particles/Blend_CenterGlow") != null) Blend_CG = Shader.Find("ERB/Particles/Blend_CenterGlow");
        if (Shader.Find("ERB/Particles/LightGlow") != null) LightGlow = Shader.Find("ERB/Particles/LightGlow");
        if (Shader.Find("ERB/Particles/Lit_CenterGlow") != null) Lit_CenterGlow = Shader.Find("ERB/Particles/Lit_CenterGlow");
        if (Shader.Find("ERB/Particles/Blend_TwoSides") != null) Blend_TwoSides = Shader.Find("ERB/Particles/Blend_TwoSides");
        if (Shader.Find("ERB/Particles/Blend_Normals") != null) Blend_Normals = Shader.Find("ERB/Particles/Blend_Normals");
        if (Shader.Find("ERB/Particles/Ice") != null) Ice = Shader.Find("ERB/Particles/Ice");
        if (Shader.Find("ERB/Particles/Distortion") != null) Distortion = Shader.Find("ERB/Particles/Distortion");
        if (Shader.Find("ERB/Opaque/ParallaxIce") != null) ParallaxIce = Shader.Find("ERB/Opaque/ParallaxIce");

        if (Shader.Find("Hovl/Particles/Add_CenterGlow") != null) Add_CG = Shader.Find("Hovl/Particles/Add_CenterGlow");
        if (Shader.Find("Hovl/Particles/Blend_CenterGlow") != null) Blend_CG = Shader.Find("Hovl/Particles/Blend_CenterGlow");
        if (Shader.Find("Hovl/Particles/LightGlow") != null) LightGlow = Shader.Find("Hovl/Particles/LightGlow");
        if (Shader.Find("Hovl/Particles/Lit_CenterGlow") != null) Lit_CenterGlow = Shader.Find("Hovl/Particles/Lit_CenterGlow");
        if (Shader.Find("Hovl/Particles/Blend_TwoSides") != null) Blend_TwoSides = Shader.Find("Hovl/Particles/Blend_TwoSides");
        if (Shader.Find("Hovl/Particles/Blend_Normals") != null) Blend_Normals = Shader.Find("Hovl/Particles/Blend_Normals");
        if (Shader.Find("Hovl/Particles/Ice") != null) Ice = Shader.Find("Hovl/Particles/Ice");
        if (Shader.Find("Hovl/Particles/Distortion") != null) Distortion = Shader.Find("Hovl/Particles/Distortion");
        if (Shader.Find("Hovl/Opaque/ParallaxIce") != null) ParallaxIce = Shader.Find("Hovl/Opaque/ParallaxIce");
        if (Shader.Find("Hovl/Particles/BlendDistort") != null) BlendDistort = Shader.Find("Hovl/Particles/BlendDistort");
        if (Shader.Find("Hovl/Particles/VolumeLaser") != null) VolumeLaser = Shader.Find("Hovl/Particles/VolumeLaser");

        if (Shader.Find("ERB/LWRP/Particles/LightGlow") != null) LightGlow_URP = Shader.Find("ERB/LWRP/Particles/LightGlow");
        if (Shader.Find("ERB/LWRP/Particles/Lit_CenterGlow") != null) Lit_CenterGlow_URP = Shader.Find("ERB/LWRP/Particles/Lit_CenterGlow");
        else { if (Shader.Find("Shader Graphs/LWRP_Lit_CenterGlow") != null) Lit_CenterGlow_URP = Shader.Find("Shader Graphs/LWRP_Lit_CenterGlow"); }
        if (Shader.Find("ERB/LWRP/Particles/Blend_TwoSides") != null) Blend_TwoSides_URP = Shader.Find("ERB/LWRP/Particles/Blend_TwoSides");
        else { if (Shader.Find("Shader Graphs/LWRP_Blend_TwoSides") != null) Blend_TwoSides_URP = Shader.Find("Shader Graphs/LWRP_Blend_TwoSides"); }
        if (Shader.Find("ERB/LWRP/Particles/Blend_Normals") != null) Blend_Normals_URP = Shader.Find("ERB/LWRP/Particles/Blend_Normals");
        else { if (Shader.Find("Shader Graphs/LWRP_Blend_Normals") != null) Blend_Normals_URP = Shader.Find("Shader Graphs/LWRP_Blend_Normals"); }
        if (Shader.Find("ERB/LWRP/Particles/Ice") != null) Ice_URP = Shader.Find("ERB/LWRP/Particles/Ice");
        else { if (Shader.Find("Shader Graphs/LWRP_Ice") != null) Ice_URP = Shader.Find("Shader Graphs/LWRP_Ice"); }
        if (Shader.Find("Shader Graphs/LWRP_Distortion") != null) Distortion_URP = Shader.Find("Shader Graphs/LWRP_Distortion");
        if (Shader.Find("Shader Graphs/LWRP_ParallaxIce") != null) ParallaxIce_URP = Shader.Find("Shader Graphs/LWRP_ParallaxIce");
        if (Shader.Find("Shader Graphs/LWRP_Add_CG") != null) Add_CG_URP = Shader.Find("Shader Graphs/LWRP_Add_CG");
        if (Shader.Find("Shader Graphs/LWRP_Blend_CG") != null) Blend_CG_URP = Shader.Find("Shader Graphs/LWRP_Blend_CG");
        if (Shader.Find("Shader Graphs/LWRP_BlendDistort") != null) BlendDistort_URP = Shader.Find("Shader Graphs/LWRP_BlendDistort");
        if (Shader.Find("Shader Graphs/URP_VolumeLaser") != null) VolumeLaser_URP = Shader.Find("Shader Graphs/URP_VolumeLaser");

        if (Shader.Find("ERB/HDRP/Particles/LightGlow") != null) LightGlow_HDRP = Shader.Find("ERB/HDRP/Particles/LightGlow");
        if (Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow") != null) Lit_CenterGlow_HDRP = Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow");
        else { if (Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow") != null) Lit_CenterGlow_HDRP = Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow"); }
        if (Shader.Find("ERB/HDRP/Particles/Blend_TwoSides") != null) Blend_TwoSides_HDRP = Shader.Find("ERB/HDRP/Particles/Blend_TwoSides");
        else { if (Shader.Find("Shader Graphs/HDRP_Blend_TwoSides") != null) Blend_TwoSides_HDRP = Shader.Find("Shader Graphs/HDRP_Blend_TwoSides"); }
        if (Shader.Find("ERB/HDRP/Particles/Blend_Normals") != null) Blend_Normals_HDRP = Shader.Find("ERB/HDRP/Particles/Blend_Normals");
        else { if (Shader.Find("Shader Graphs/HDRP_Blend_Normals") != null) Blend_Normals_HDRP = Shader.Find("Shader Graphs/HDRP_Blend_Normals"); }
        if (Shader.Find("ERB/HDRP/Particles/Ice") != null) Ice_HDRP = Shader.Find("ERB/HDRP/Particles/Ice");
        else { if (Shader.Find("Shader Graphs/HDRP_Ice") != null) Ice_HDRP = Shader.Find("Shader Graphs/HDRP_Ice"); }
        if (Shader.Find("Shader Graphs/HDRP_Distortion") != null) Distortion_HDRP = Shader.Find("Shader Graphs/HDRP_Distortion");
        if (Shader.Find("Shader Graphs/HDRP_ParallaxIce") != null) ParallaxIce_HDRP = Shader.Find("Shader Graphs/HDRP_ParallaxIce");
        if (Shader.Find("Shader Graphs/HDRP_Add_CG") != null) Add_CG_HDRP = Shader.Find("Shader Graphs/HDRP_Add_CG");
        if (Shader.Find("Shader Graphs/HDRP_Blend_CG") != null) Blend_CG_HDRP = Shader.Find("Shader Graphs/HDRP_Blend_CG");
        if (Shader.Find("Shader Graphs/HDRP_BlendDistort") != null) BlendDistort_HDRP = Shader.Find("Shader Graphs/HDRP_BlendDistort");
        if (Shader.Find("Shader Graphs/HDRP_VolumeLaser") != null) VolumeLaser_HDRP = Shader.Find("Shader Graphs/HDRP_VolumeLaser");

        string[] folderMat = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        shaderMaterials = new Material[folderMat.Length];

        for (int i = 0; i < folderMat.Length; i++)
        {
            var patch = AssetDatabase.GUIDToAssetPath(folderMat[i]);
            shaderMaterials[i] = (Material)AssetDatabase.LoadAssetAtPath(patch, typeof(Material));
        }
    }

    private void ChangeToLWRP()
    {

        foreach (var material in shaderMaterials)
        {
            if (Shader.Find("ERB/LWRP/Particles/LightGlow") != null)
            {
                if (material.shader == LightGlow || material.shader == LightGlow_HDRP)
                {
                    material.shader = LightGlow_URP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/LWRP/Particles/Lit_CenterGlow") != null || Shader.Find("Shader Graphs/LWRP_Lit_CenterGlow") != null)
            {
                if (material.shader == Lit_CenterGlow || material.shader == Lit_CenterGlow_HDRP)
                {
                    material.shader = Lit_CenterGlow_URP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/LWRP/Particles/Blend_TwoSides") != null || Shader.Find("Shader Graphs/LWRP_Blend_TwoSides") != null)
            {
                if (material.shader == Blend_TwoSides || material.shader == Blend_TwoSides_HDRP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        material.shader = Blend_TwoSides_URP;
                        if (material.HasProperty("_MainTexTiling"))
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.HasProperty("_NoiseTiling"))
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                    }
                    else
                        material.shader = Blend_TwoSides_URP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/LWRP/Particles/Blend_Normals") != null || Shader.Find("Shader Graphs/LWRP_Blend_Normals") != null)
            {
                if (material.shader == Blend_Normals || material.shader == Blend_Normals_HDRP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        material.shader = Blend_Normals_URP;
                        if (material.HasProperty("_MainTexTiling"))
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.HasProperty("_NoiseTiling"))
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                    }
                    else
                        material.shader = Blend_Normals_URP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/LWRP/Particles/Ice") != null || Shader.Find("Shader Graphs/LWRP_Ice") != null)
            {
                if (material.shader == Ice || material.shader == Ice_HDRP)
                {
                    if (material.GetTextureScale("_MainTex") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        material.shader = Ice_URP;
                        if (material.HasProperty("_Tiling"))
                            material.SetVector("_Tiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                    }
                    else
                        material.shader = Ice_URP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/LWRP_ParallaxIce") != null)
            {
                if (material.shader == ParallaxIce || material.shader == ParallaxIce_HDRP)
                {
                    if (material.GetTextureScale("_Emission") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_Emission");
                        Vector2 MainOffset = material.GetTextureOffset("_Emission");
                        material.shader = ParallaxIce_URP;
                        if (material.HasProperty("_EmissionTiling"))
                            material.SetVector("_EmissionTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                    }
                    else
                        material.shader = ParallaxIce_URP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/LWRP_Distortion") != null)
            {
                if (material.shader == Distortion || material.shader == Distortion_HDRP)
                {
                    material.SetFloat("_ZWrite", 0);
                    material.shader = Distortion_URP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/LWRP_Add_CG") != null)
            {
                if (material.shader == Add_CG || material.shader == Add_CG_HDRP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null
                        || material.GetTextureScale("_Flow") != null || material.GetTextureScale("_Mask") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        Vector2 FlowScale = material.GetTextureScale("_Flow");
                        Vector2 FlowOffset = material.GetTextureOffset("_Flow");
                        Vector2 MaskScale = material.GetTextureScale("_Mask");
                        Vector2 MaskOffset = material.GetTextureOffset("_Mask");
                        material.shader = Add_CG_URP;
                        if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0);
                        if (material.HasProperty("_MainTexTiling"))
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.HasProperty("_NoiseTiling"))
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                        if (material.HasProperty("_FlowTiling"))
                            material.SetVector("_FlowTiling", new Vector4(FlowScale[0], FlowScale[1], FlowOffset[0], FlowOffset[1]));
                        if (material.HasProperty("_MaskTiling"))
                            material.SetVector("_MaskTiling", new Vector4(MaskScale[0], MaskScale[1], MaskOffset[0], MaskOffset[1]));
                    }
                    else
                        material.shader = Add_CG_URP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/LWRP_Blend_CG") != null)
            {
                if (material.shader == Blend_CG || material.shader == Blend_CG_HDRP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null
                        || material.GetTextureScale("_Flow") != null || material.GetTextureScale("_Mask") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        Vector2 FlowScale = material.GetTextureScale("_Flow");
                        Vector2 FlowOffset = material.GetTextureOffset("_Flow");
                        Vector2 MaskScale = material.GetTextureScale("_Mask");
                        Vector2 MaskOffset = material.GetTextureOffset("_Mask");
                        material.shader = Blend_CG_URP;
                        if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0);
                        if (material.HasProperty("_MainTexTiling"))
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.HasProperty("_NoiseTiling"))
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                        if (material.HasProperty("_FlowTiling"))
                            material.SetVector("_FlowTiling", new Vector4(FlowScale[0], FlowScale[1], FlowOffset[0], FlowOffset[1]));
                        if (material.HasProperty("_MaskTiling"))
                            material.SetVector("_MaskTiling", new Vector4(MaskScale[0], MaskScale[1], MaskOffset[0], MaskOffset[1]));
                    }
                    else
                        material.shader = Blend_CG_URP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/LWRP_BlendDistort") != null)
            {
                if (material.shader == BlendDistort || material.shader == BlendDistort_HDRP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null
                        || material.GetTextureScale("_Flow") != null || material.GetTextureScale("_Mask") != null || material.GetTextureScale("_NormalMap") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        Vector2 FlowScale = material.GetTextureScale("_Flow");
                        Vector2 FlowOffset = material.GetTextureOffset("_Flow");
                        Vector2 MaskScale = material.GetTextureScale("_Mask");
                        Vector2 MaskOffset = material.GetTextureOffset("_Mask");
                        Vector2 NormalScale = material.GetTextureScale("_NormalMap");
                        Vector2 NormalOffset = material.GetTextureOffset("_NormalMap");
                        material.shader = BlendDistort_URP;
                        if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0);
                        if (material.HasProperty("_MainTexTiling"))
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.HasProperty("_NoiseTiling"))
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                        if (material.HasProperty("_FlowTiling"))
                            material.SetVector("_FlowTiling", new Vector4(FlowScale[0], FlowScale[1], FlowOffset[0], FlowOffset[1]));
                        if (material.HasProperty("_MaskTiling"))
                            material.SetVector("_MaskTiling", new Vector4(MaskScale[0], MaskScale[1], MaskOffset[0], MaskOffset[1]));
                        if (material.HasProperty("_NormalMapTiling"))
                            material.SetVector("_NormalMapTiling", new Vector4(NormalScale[0], NormalScale[1], NormalOffset[0], NormalOffset[1]));
                    }
                    else
                        material.shader = BlendDistort_URP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/URP_VolumeLaser") != null)
            {
                if (material.shader == VolumeLaser || material.shader == VolumeLaser_HDRP)
                {
                    material.shader = VolumeLaser_URP;
                }
            }
        }
    }


    private void ChangeToSRP()
    {

        foreach (var material in shaderMaterials)
        {
            if (Shader.Find("ERB/Particles/LightGlow") != null || Shader.Find("Hovl/Particles/LightGlow") != null)
            {
                if (material.shader == LightGlow_URP || material.shader == LightGlow_HDRP)
                {
                    material.shader = LightGlow;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Particles/Lit_CenterGlow") != null || Shader.Find("Hovl/Particles/Lit_CenterGlow") != null)
            {
                if (material.shader == Lit_CenterGlow_URP || material.shader == Lit_CenterGlow_HDRP)
                {
                    material.shader = Lit_CenterGlow;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Particles/Blend_TwoSides") != null || Shader.Find("Hovl/Particles/Blend_TwoSides") != null)
            {
                if (material.shader == Blend_TwoSides_URP || material.shader == Blend_TwoSides_HDRP)
                {
                    if (material.HasProperty("_MainTexTiling") && material.HasProperty("_NoiseTiling"))
                    {
                        Vector4 MainTiling = material.GetVector("_MainTexTiling");
                        Vector4 NoiseTiling = material.GetVector("_NoiseTiling");
                        material.shader = Blend_TwoSides;
                        if (material.GetTextureScale("_MainTex") != null && material.GetTextureScale("_Noise") != null)
                        {
                            material.SetTextureScale("_MainTex", new Vector2(MainTiling[0], MainTiling[1]));
                            material.SetTextureOffset("_MainTex", new Vector2(MainTiling[2], MainTiling[3]));
                            material.SetTextureScale("_Noise", new Vector2(NoiseTiling[0], NoiseTiling[1]));
                            material.SetTextureOffset("_Noise", new Vector2(NoiseTiling[2], NoiseTiling[3]));
                        }
                    }
                    else
                        material.shader = Blend_TwoSides;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Particles/Blend_Normals") != null || Shader.Find("Hovl/Particles/Blend_Normals") != null)
            {
                if (material.shader == Blend_Normals_URP || material.shader == Blend_Normals_HDRP)
                {
                    if (material.HasProperty("_MainTexTiling") && material.HasProperty("_NoiseTiling"))
                    {
                        Vector4 MainTiling = material.GetVector("_MainTexTiling");
                        Vector4 NoiseTiling = material.GetVector("_NoiseTiling");
                        material.shader = Blend_Normals;
                        if (material.GetTextureScale("_MainTex") != null && material.GetTextureScale("_Noise") != null)
                        {
                            material.SetTextureScale("_MainTex", new Vector2(MainTiling[0], MainTiling[1]));
                            material.SetTextureOffset("_MainTex", new Vector2(MainTiling[2], MainTiling[3]));
                            material.SetTextureScale("_Noise", new Vector2(NoiseTiling[0], NoiseTiling[1]));
                            material.SetTextureOffset("_Noise", new Vector2(NoiseTiling[2], NoiseTiling[3]));
                        }
                    }
                    else
                        material.shader = Blend_Normals;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Particles/Ice") != null || Shader.Find("Hovl/Particles/Ice") != null)
            {
                if (material.shader == Ice_URP || material.shader == Ice_HDRP)
                {
                    if (material.HasProperty("_Tiling"))
                    {
                        Vector4 MainTiling = material.GetVector("_Tiling");
                        material.shader = Ice;
                        if (material.GetTextureScale("_MainTex") != null)
                        {
                            material.SetTextureScale("_MainTex", new Vector2(MainTiling[0], MainTiling[1]));
                            material.SetTextureOffset("_MainTex", new Vector2(MainTiling[2], MainTiling[3]));
                        }
                    }
                    else
                        material.shader = Ice;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Opaque/ParallaxIce") != null || Shader.Find("Hovl/Opaque/ParallaxIce") != null)
            {
                if (material.shader == ParallaxIce_URP || material.shader == ParallaxIce_HDRP)
                {
                    if (material.HasProperty("_EmissionTiling"))
                    {
                        Vector4 MainTiling = material.GetVector("_EmissionTiling");
                        material.shader = ParallaxIce;
                        if (material.GetTextureScale("_Emission") != null)
                        {
                            material.SetTextureScale("_Emission", new Vector2(MainTiling[0], MainTiling[1]));
                            material.SetTextureOffset("_Emission", new Vector2(MainTiling[2], MainTiling[3]));
                        }
                    }
                    else
                        material.shader = ParallaxIce;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Particles/Distortion") != null || Shader.Find("Hovl/Particles/Distortion") != null)
            {
                if (material.shader == Distortion_URP || material.shader == Distortion_HDRP)
                {
                    material.shader = Distortion;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Particles/Add_CenterGlow") != null || Shader.Find("Hovl/Particles/Add_CenterGlow") != null)
            {
                if (material.shader == Add_CG_URP || material.shader == Add_CG_HDRP)
                {
                    if (material.HasProperty("_MainTexTiling") && material.HasProperty("_NoiseTiling")
                        && material.HasProperty("_FlowTiling") && material.HasProperty("_MaskTiling"))
                    {
                        Vector4 MainTiling = material.GetVector("_MainTexTiling");
                        Vector4 NoiseTiling = material.GetVector("_NoiseTiling");
                        Vector4 FlowTiling = material.GetVector("_FlowTiling");
                        Vector4 MaskTiling = material.GetVector("_MaskTiling");
                        material.shader = Add_CG;
                        if (material.GetTextureScale("_MainTex") != null && material.GetTextureScale("_Noise") != null)
                        {
                            material.SetTextureScale("_MainTex", new Vector2(MainTiling[0], MainTiling[1]));
                            material.SetTextureOffset("_MainTex", new Vector2(MainTiling[2], MainTiling[3]));
                            material.SetTextureScale("_Noise", new Vector2(NoiseTiling[0], NoiseTiling[1]));
                            material.SetTextureOffset("_Noise", new Vector2(NoiseTiling[2], NoiseTiling[3]));
                            material.SetTextureScale("_Flow", new Vector2(FlowTiling[0], FlowTiling[1]));
                            material.SetTextureOffset("_Flow", new Vector2(FlowTiling[2], FlowTiling[3]));
                            material.SetTextureScale("_Mask", new Vector2(MaskTiling[0], MaskTiling[1]));
                            material.SetTextureOffset("_Mask", new Vector2(MaskTiling[2], MaskTiling[3]));
                        }
                    }
                    else
                        material.shader = Add_CG;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Particles/Blend_CenterGlow") != null || Shader.Find("Hovl/Particles/Blend_CenterGlow") != null)
            {
                if (material.shader == Blend_CG_URP || material.shader == Blend_CG_HDRP)
                {
                    if (material.HasProperty("_MainTexTiling") && material.HasProperty("_NoiseTiling")
                        && material.HasProperty("_FlowTiling") && material.HasProperty("_MaskTiling"))
                    {
                        Vector4 MainTiling = material.GetVector("_MainTexTiling");
                        Vector4 NoiseTiling = material.GetVector("_NoiseTiling");
                        Vector4 FlowTiling = material.GetVector("_FlowTiling");
                        Vector4 MaskTiling = material.GetVector("_MaskTiling");
                        material.shader = Blend_CG;
                        if (material.GetTextureScale("_MainTex") != null && material.GetTextureScale("_Noise") != null)
                        {
                            material.SetTextureScale("_MainTex", new Vector2(MainTiling[0], MainTiling[1]));
                            material.SetTextureOffset("_MainTex", new Vector2(MainTiling[2], MainTiling[3]));
                            material.SetTextureScale("_Noise", new Vector2(NoiseTiling[0], NoiseTiling[1]));
                            material.SetTextureOffset("_Noise", new Vector2(NoiseTiling[2], NoiseTiling[3]));
                            material.SetTextureScale("_Flow", new Vector2(FlowTiling[0], FlowTiling[1]));
                            material.SetTextureOffset("_Flow", new Vector2(FlowTiling[2], FlowTiling[3]));
                            material.SetTextureScale("_Mask", new Vector2(MaskTiling[0], MaskTiling[1]));
                            material.SetTextureOffset("_Mask", new Vector2(MaskTiling[2], MaskTiling[3]));
                        }
                    }
                    else
                        material.shader = Blend_CG;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Hovl/Particles/BlendDistort") != null)
            {
                if (material.shader == BlendDistort_URP || material.shader == BlendDistort_HDRP)
                {
                    if (material.HasProperty("_MainTexTiling") && material.HasProperty("_NoiseTiling")
                        && material.HasProperty("_FlowTiling") && material.HasProperty("_MaskTiling") && material.HasProperty("_NormalMapTiling"))
                    {
                        Vector4 MainTiling = material.GetVector("_MainTexTiling");
                        Vector4 NoiseTiling = material.GetVector("_NoiseTiling");
                        Vector4 FlowTiling = material.GetVector("_FlowTiling");
                        Vector4 MaskTiling = material.GetVector("_MaskTiling");
                        Vector4 NormalTiling = material.GetVector("_NormalMapTiling");
                        material.shader = BlendDistort;
                        if (material.GetTextureScale("_MainTex") != null && material.GetTextureScale("_Noise") != null)
                        {
                            material.SetTextureScale("_MainTex", new Vector2(MainTiling[0], MainTiling[1]));
                            material.SetTextureOffset("_MainTex", new Vector2(MainTiling[2], MainTiling[3]));
                            material.SetTextureScale("_Noise", new Vector2(NoiseTiling[0], NoiseTiling[1]));
                            material.SetTextureOffset("_Noise", new Vector2(NoiseTiling[2], NoiseTiling[3]));
                            material.SetTextureScale("_Flow", new Vector2(FlowTiling[0], FlowTiling[1]));
                            material.SetTextureOffset("_Flow", new Vector2(FlowTiling[2], FlowTiling[3]));
                            material.SetTextureScale("_Mask", new Vector2(MaskTiling[0], MaskTiling[1]));
                            material.SetTextureOffset("_Mask", new Vector2(MaskTiling[2], MaskTiling[3]));
                            material.SetTextureScale("_NormalMap", new Vector2(NormalTiling[0], NormalTiling[1]));
                            material.SetTextureOffset("_NormalMap", new Vector2(NormalTiling[2], NormalTiling[3]));
                        }
                    }
                    else
                        material.shader = BlendDistort;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Hovl/Particles/VolumeLaser") != null)
            {
                if (material.shader == VolumeLaser_URP || material.shader == VolumeLaser_HDRP)
                {
                    material.shader = VolumeLaser;
                }
            }
        }
    }

    private void ChangeToHDRP()
    {
        foreach (var material in shaderMaterials)
        {
            if (Shader.Find("ERB/HDRP/Particles/LightGlow") != null)
            {
                if (material.shader == LightGlow || material.shader == LightGlow_URP)
                {
                    material.shader = LightGlow_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow") != null)
            {
                if (material.shader == Lit_CenterGlow || material.shader == Lit_CenterGlow_URP)
                {
                    material.shader = Lit_CenterGlow_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/HDRP/Particles/Blend_TwoSides") != null || Shader.Find("Shader Graphs/HDRP_Blend_TwoSides") != null)
            {
                if (material.shader == Blend_TwoSides || material.shader == Blend_TwoSides_URP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        material.shader = Blend_TwoSides_HDRP;
                        if (material.HasProperty("_MainTexTiling"))
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.HasProperty("_NoiseTiling"))
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                    }
                    else
                        material.shader = Blend_TwoSides_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/HDRP/Particles/Blend_Normals") != null || Shader.Find("Shader Graphs/HDRP_Blend_Normals") != null)
            {
                if (material.shader == Blend_Normals || material.shader == Blend_Normals_URP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        material.shader = Blend_Normals_HDRP;
                        if (material.HasProperty("_MainTexTiling"))
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.HasProperty("_NoiseTiling"))
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                    }
                    else
                        material.shader = Blend_Normals_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/HDRP/Particles/Ice") != null || Shader.Find("Shader Graphs/HDRP_Ice") != null)
            {
                if (material.shader == Ice || material.shader == Ice_URP)
                {
                    if (material.GetTextureScale("_MainTex") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        material.shader = Ice_HDRP;
                        if (material.HasProperty("_Tiling"))
                            material.SetVector("_Tiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                    }
                    else
                        material.shader = Ice_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Opaque/ParallaxIce") != null || Shader.Find("Shader Graphs/HDRP_ParallaxIce") != null)
            {
                if (material.shader == ParallaxIce || material.shader == ParallaxIce_URP)
                {
                    if (material.GetTextureScale("_Emission") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_Emission");
                        Vector2 MainOffset = material.GetTextureOffset("_Emission");
                        material.shader = ParallaxIce_HDRP;
                        if (material.HasProperty("_EmissionTiling"))
                            material.SetVector("_EmissionTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                    }
                    else
                        material.shader = ParallaxIce_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/HDRP_Distortion") != null)
            {
                if (material.shader == Distortion || material.shader == Distortion_URP)
                {
                    material.SetFloat("_ZWrite", 0);
                    material.shader = Distortion_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/HDRP_Add_CG") != null)
            {
                if (material.shader == Add_CG || material.shader == Add_CG_URP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null
                        || material.GetTextureScale("_Flow") != null || material.GetTextureScale("_Mask") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        Vector2 FlowScale = material.GetTextureScale("_Flow");
                        Vector2 FlowOffset = material.GetTextureOffset("_Flow");
                        Vector2 MaskScale = material.GetTextureScale("_Mask");
                        Vector2 MaskOffset = material.GetTextureOffset("_Mask");
                        material.SetFloat("_StencilRef", 0);
                        material.SetFloat("_AlphaDstBlend", 1);
                        material.SetFloat("_DstBlend", 1);
                        material.SetFloat("_ZWrite", 0);
                        material.SetFloat("_SrcBlend", 1);
                        material.EnableKeyword("_BLENDMODE_ADD _DOUBLESIDED_ON _SURFACE_TYPE_TRANSPARENT");
                        material.SetShaderPassEnabled("TransparentBackface", false);
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetFloat("_CullModeForward", 0);
                        material.shader = Add_CG_HDRP;
                        if (material.HasProperty("_MainTexTiling"))
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.HasProperty("_NoiseTiling"))
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                        if (material.HasProperty("_FlowTiling"))
                            material.SetVector("_FlowTiling", new Vector4(FlowScale[0], FlowScale[1], FlowOffset[0], FlowOffset[1]));
                        if (material.HasProperty("_MaskTiling"))
                            material.SetVector("_MaskTiling", new Vector4(MaskScale[0], MaskScale[1], MaskOffset[0], MaskOffset[1]));
                    }
                    else
                        material.shader = Add_CG_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/HDRP_Blend_CG") != null)
            {
                if (material.shader == Blend_CG || material.shader == Blend_CG_URP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null
                        || material.GetTextureScale("_Flow") != null || material.GetTextureScale("_Mask") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        Vector2 FlowScale = material.GetTextureScale("_Flow");
                        Vector2 FlowOffset = material.GetTextureOffset("_Flow");
                        Vector2 MaskScale = material.GetTextureScale("_Mask");
                        Vector2 MaskOffset = material.GetTextureOffset("_Mask");
                        material.SetFloat("_ZWrite", 0);
                        material.SetFloat("_StencilRef", 0);
                        material.SetShaderPassEnabled("TransparentBackface", false);
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetFloat("_AlphaDstBlend", 10);
                        material.SetFloat("_DstBlend", 10);
                        material.SetFloat("_SrcBlend", 1);
                        material.EnableKeyword("_BLENDMODE_ALPHA _DOUBLESIDED_ON _SURFACE_TYPE_TRANSPARENT");
                        if (material.HasProperty("_CullModeForward")) material.SetFloat("_CullModeForward", 0);
                        material.shader = Blend_CG_HDRP;
                        if (material.HasProperty("_MainTexTiling"))
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.HasProperty("_NoiseTiling"))
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                        if (material.HasProperty("_FlowTiling"))
                            material.SetVector("_FlowTiling", new Vector4(FlowScale[0], FlowScale[1], FlowOffset[0], FlowOffset[1]));
                        if (material.HasProperty("_MaskTiling"))
                            material.SetVector("_MaskTiling", new Vector4(MaskScale[0], MaskScale[1], MaskOffset[0], MaskOffset[1]));
                    }
                    else
                        material.shader = Blend_CG_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/HDRP_BlendDistort") != null)
            {
                if (material.shader == BlendDistort || material.shader == BlendDistort_URP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null
                        || material.GetTextureScale("_Flow") != null || material.GetTextureScale("_Mask") != null || material.GetTextureScale("_NormalMap") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        Vector2 FlowScale = material.GetTextureScale("_Flow");
                        Vector2 FlowOffset = material.GetTextureOffset("_Flow");
                        Vector2 MaskScale = material.GetTextureScale("_Mask");
                        Vector2 MaskOffset = material.GetTextureOffset("_Mask");
                        Vector2 NormalScale = material.GetTextureScale("_NormalMap");
                        Vector2 NormalOffset = material.GetTextureOffset("_NormalMap");
                        material.shader = BlendDistort_HDRP;
                        if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0);
                        if (material.HasProperty("_MainTexTiling"))
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.HasProperty("_NoiseTiling"))
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                        if (material.HasProperty("_FlowTiling"))
                            material.SetVector("_FlowTiling", new Vector4(FlowScale[0], FlowScale[1], FlowOffset[0], FlowOffset[1]));
                        if (material.HasProperty("_MaskTiling"))
                            material.SetVector("_MaskTiling", new Vector4(MaskScale[0], MaskScale[1], MaskOffset[0], MaskOffset[1]));
                        if (material.HasProperty("_NormalMapTiling"))
                            material.SetVector("_NormalMapTiling", new Vector4(NormalScale[0], NormalScale[1], NormalOffset[0], NormalOffset[1]));
                    }
                    else
                        material.shader = BlendDistort_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/HDRP_VolumeLaser") != null)
            {
                if (material.shader == VolumeLaser || material.shader == VolumeLaser_URP)
                {
                    material.shader = VolumeLaser_HDRP;
                }
            }
        }
    }
}